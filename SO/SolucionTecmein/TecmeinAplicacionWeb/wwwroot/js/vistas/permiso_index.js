let tablaData;

const modeloBase = {
    idPermiso: "",
    descripcion: ""
};

$(document).ready(function () {
    // Configuración de idioma local en español para DataTable
    const lenguajeEspanol = {
        processing:     "Procesando...",
        search:         "",
        searchPlaceholder: "Buscar...",
        lengthMenu:    "Mostrar _MENU_",
        info:           "Mostrando _START_ a _END_ de _TOTAL_ registros",
        infoEmpty:      "Mostrando 0 a 0 de 0 registros",
        infoFiltered:   "(filtrado de _MAX_ registros totales)",
        loadingRecords: "Cargando...",
        zeroRecords:    "No se encontraron resultados",
        emptyTable:     "Ningún dato disponible en esta tabla",
        paginate: {
            first:      "Primero",
            previous:   "Anterior",
            next:       "Siguiente",
            last:       "Último"
        }
    };

    tablaData = $('#tablaPermisos').DataTable({
        responsive: true,
        "ajax": {
            "url": "/Permiso/Lista",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function(json) { return json.listaObjeto ? json.listaObjeto.$values : []; }
        },
        "columns": [
            { "data": "idPermiso" },
            { "data": "descripcion" },
            {
                data: "idPermiso",
                render: function (data, type, row) {
                    return `<div class="dropdown">` +
                           `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">` +
                           `<i class="fas fa-cog text-warning mr-1"></i> Acciones` +
                           `</button>` +
                           `<div class="dropdown-menu">` +
                           `<a class="dropdown-item btn-editar" href="#"><i class="fas fa-pencil-alt text-primary mr-2"></i> Editar</a>` +
                           `<a class="dropdown-item btn-eliminar" href="#"><i class="fas fa-trash-alt text-danger mr-2"></i> Eliminar</a>` +
                           `</div>` +
                           `</div>`;
                },
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        order: [[0, "asc"]],
        dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
        buttons: [
            {
                text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                extend: 'excelHtml5',
                title: 'Permisos',
                filename: 'Reporte Permisos',
                exportOptions: {
                    columns: [0, 1]
                },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-file-pdf text-danger fa-lg"></i>',
                extend: 'pdfHtml5',
                title: 'Permisos',
                filename: 'Reporte Permisos',
                exportOptions: {
                    columns: [0, 1]
                },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-print text-primary fa-lg"></i>',
                extend: 'print',
                title: 'Permisos',
                exportOptions: {
                    columns: [0, 1]
                },
                className: 'btn btn-link btn-sm p-1'
            }
        ],
        "language": lenguajeEspanol,
        initComplete: function() {
            $("#btnNuevo").appendTo(".toolbar-left");
            $("#btnNuevo").closest(".row").show();
        }
    });
});


function mostrarModal(modelo = modeloBase) {
    $("#txtIdPermiso").val(modelo.idPermiso);
    $("#txtIdPermisoInput").val(modelo.idPermiso);
    $("#txtDescripcion").val(modelo.descripcion);

    // Si es un nuevo permiso, el ID es editable. Si se edita, no.
    $("#txtIdPermisoInput").prop("disabled", modelo.idPermiso !== "");

    $('#modalData').modal('show');
}

$("#btnNuevo").click(function () {
    mostrarModal();
});

$("#btnGuardar").click(function () {
    const idOriginal = $("#txtIdPermiso").val(); // El ID antes de la edición
    const esNuevo = idOriginal === "";

    const modelo = {
        idPermiso: $("#txtIdPermisoInput").val(),
        descripcion: $("#txtDescripcion").val()
    };

    const url = esNuevo ? "/Permiso/Crear" : "/Permiso/Editar";
    const method = esNuevo ? "POST" : "PUT";

    fetch(url, {
        method: method,
        headers: {
            "Content-Type": "application/json; charset=utf-8"
        },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        if (responseJson.estado) {
            $('#modalData').modal('hide');
            Swal.fire("Listo!", "El permiso fue guardado", "success");
            tablaData.ajax.reload();
        } else {
            Swal.fire("Error", responseJson.mensaje, "error");
        }
    })
    .catch((error) => {
        Swal.fire("Error", "No se pudo guardar el permiso", "error");
    });
});

let filaSeleccionada;
$("#tablaPermisos tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$("#tablaPermisos tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "¿Está seguro?",
        text: `¿Eliminar el permiso "${data.idPermiso}"?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "No, cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`/Permiso/Eliminar?idPermiso=${data.idPermiso}`, {
                method: "DELETE"
            })
            .then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire("Listo!", "El permiso fue eliminado", "success");
                    tablaData.row(fila).remove().draw();
                } else {
                    Swal.fire("Error", responseJson.mensaje, "error");
                }
            })
            .catch((error) => {
                Swal.fire("Error", "No se pudo eliminar el permiso", "error");
            });
        }
    });
});