const MODELO_BASE = {
    secuencial: "",
    nombre: "",
    urlCatalogo: "",
    fechaRegistro: "",
    estaActivo: 1
}

let tablaData;

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

    tablaData = $('#tbdataCatalogo').DataTable({
        responsive: true,
        "ajax": {
            "url": 'Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                return json.data && json.data.$values ? json.data.$values : json.data;
            }
        },
        "columns": [
            { data: "secuencial", visible: false, searchable: true },
            { data: "nombre", searchable: true },
            {
                data: "urlCatalogo", render: function (data) {
                    return `<a href="${data}" target="_blank" class="btn-link btn-sm mr-2"> Abrir Documento <i class="fas fa-external-link-alt"></i></a>`;
                },
                "width": "150px"
            },
            { data: "fechaRegistro", searchable: true, "width": "100px" },
            {
                data: "estaActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">Inactivo</span>';
                },
                "width": "50px"
            },
            {
                data: "secuencial",
                render: function (data, type, row) {
                    return `<div class="dropdown">` +
                           `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">` +
                           `<i class="fas fa-cog text-warning mr-1"></i> Acciones` +
                           `</button>` +
                           `<div class="dropdown-menu">` +
                           `<a class="dropdown-item btn-eliminar" href="#"><i class="fas fa-trash-alt text-danger mr-2"></i> Eliminar</a>` +
                           `</div>` +
                           `</div>`;
                },
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        order: [[0, "desc"]],
        dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
        buttons: [
            {
                text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                extend: 'excelHtml5',
                title: 'Catálogos',
                filename: 'Reporte de Catálogos',
                exportOptions: {
                    columns: [1, 2, 3, 4]
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

    // Actualizar label del input file con el nombre del archivo seleccionado
    $(".custom-file-input").on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
    });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial)
    $("#txtNombre").val(modelo.nombre)
    $("#txtUrlCatalogo").val(modelo.urlCatalogo)
    $("#txtFechaRegistro").val(modelo.fechaRegistro)
    $("#cboEstado").val(modelo.estaActivo)
    $("#txtDocumento").val("")
    $("#modalData").modal("show")
};

$("#btnNuevo").click(function () {
    mostrarModal();
})

$("#btnGuardarCat").click(function () {

    const modelo = structuredClone(MODELO_BASE);
    modelo["secuencial"] = 0;
    modelo["nombre"] = $("#txtNombre").val().trim();
    modelo["urlCatalogo"] = $("#txtUrlCatalogo").val().trim();
    modelo["fechaRegistro"] = $("#txtFechaRegistro").val().trim();
    modelo["estaActivo"] = $("#cboEstado").val();

    const inputDocumento = document.getElementById("txtDocumento");
    if (inputDocumento.files.length < 1) {
        toastr.warning("", "Debe seleccionar un documento PDF");
        return;
    }

    const datosFormulario = new FormData();
    datosFormulario.append("archivoPDF", inputDocumento.files[0]);
    datosFormulario.append("modelo", JSON.stringify(modelo));

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch("CrearCatalogo", {
        method: "POST",
        body: datosFormulario
    })
    .then(response => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        if (!response.ok) {
            return response.json().then(errorJson => Promise.reject(errorJson));
        }
        return response.json();
    })
    .then(responseJson => {
        if (responseJson.estado) {
            tablaData.ajax.reload();
            $("#modalData").modal("hide");
            Swal.fire("Listo!", "Catalogo " + responseJson.objeto.nombre + " Creado ", "success");
        } else {
            Swal.fire("Fallo!", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        if (error && error.mensajes) {
            Swal.fire("Fallo!", error.mensajes, "error");
        } else {
            console.error('Hubo un problema con la solicitud Fetch:', error);
            Swal.fire("Fallo!", "Ocurrió un error inesperado al guardar.", "error");
        }
    });
});

$("#tbdataCatalogo tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "¿Está Seguro de Eliminar?",
        text: `Eliminar el Catalogo "${data.nombre}"`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`Eliminar?secuencial=${data.secuencial}`, {
                method: "DELETE"
            })
            .then(response => {
                $(".showSweetAlert").LoadingOverlay("hide");
                if (!response.ok) {
                    return response.json().then(errorJson => Promise.reject(errorJson));
                }
                return response.json();
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw(false);
                    Swal.fire("Listo!", "El Catalogo " + data.nombre + " fue Eliminado", "success");
                } else {
                    Swal.fire("Fallo!", responseJson.mensajes, "error");
                }
            })
            .catch(error => {
                $(".showSweetAlert").LoadingOverlay("hide");
                if (error && error.mensajes) {
                    Swal.fire("Fallo!", error.mensajes, "error");
                } else {
                    console.error('Hubo un problema con la solicitud Fetch:', error);
                    Swal.fire("Fallo!", "Ocurrió un error inesperado al eliminar.", "error");
                }
            });
        }
    });
});