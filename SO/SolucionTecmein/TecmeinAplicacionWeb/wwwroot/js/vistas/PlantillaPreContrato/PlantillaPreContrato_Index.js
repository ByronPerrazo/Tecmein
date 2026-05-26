var tablaData;

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

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": "/PlantillaPreContrato/Lista",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function(json) { return json.data ? json.data.$values : []; },
        },
        "columns": [
            { "data": "secPlantillaPreContrato", "visible": false, "searchable": false },
            { "data": "nombre" },
            { "data": "numeracionInicial" },
            { "data": "descripcionTipoDocumento" },
            { "data": "fechaRegistro" },
            {
                "data": "estaActivo", "render": function (valor) {
                    if (valor == 1) {
                        return '<span class="badge badge-info">Activo</span>'
                    } else {
                        return '<span class="badge badge-danger">Inactivo</span>'
                    }
                }
            },
            {
                data: "secPlantillaPreContrato",
                render: function (data, type, row) {
                    return `<div class="dropdown">` +
                           `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">` +
                           `<i class="fas fa-cog text-warning mr-1"></i> Acciones` +
                           `</button>` +
                           `<div class="dropdown-menu">` +
                           `<a class="dropdown-item btn-editar" href="#"><i class="fas fa-pencil-alt text-primary mr-2"></i> Editar</a>` +
                           `<a class="dropdown-item btn-parrafos" href="#"><i class="fas fa-paragraph text-info mr-2"></i> Configurar Párrafos</a>` +
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
                title: 'Plantillas de Pre-Contrato',
                filename: 'Reporte Plantillas Pre-Contrato',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5]
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

    $("#btnNuevo").click(function () {
        mostrarModal();
    });

    $("#btnGuardar").click(function () {
        GuardarCambios();
    });

    // Evento para el botón de editar
    $('#tbdata tbody').on('click', '.btn-editar', function (e) {
        e.preventDefault();
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        var data = tablaData.row(fila).data();
        mostrarModal(data);
    });

    // Evento para el botón de eliminar
    $('#tbdata tbody').on('click', '.btn-eliminar', function (e) {
        e.preventDefault();
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        var data = tablaData.row(fila).data();

        Swal.fire({
            title: "¿Está seguro de eliminar esta plantilla?",
            text: "Una vez eliminada, no podrá recuperarse.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "No, cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/PlantillaPreContrato/Eliminar?SecPlantillaPreContrato=${data.secPlantillaPreContrato}`, {
                    method: "DELETE"
                })
                .then(response => response.json())
                .then(responseJson => {
                    if (responseJson.resultado) {
                        tablaData.ajax.reload();
                        Swal.fire("¡Eliminado!", "La plantilla ha sido eliminada correctamente.", "success");
                    } else {
                        Swal.fire("Error", "No se pudo eliminar la plantilla.", "error");
                    }
                })
                .catch(error => {
                    console.error("Error al eliminar:", error);
                    Swal.fire("Error", "Ocurrió un error al intentar eliminar la plantilla.", "error");
                });
            }
        });
    });

    // Evento para el botón de párrafos
    $('#tbdata tbody').on('click', '.btn-parrafos', function (e) {
        e.preventDefault();
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        let data = tablaData.row(fila).data();
        window.location.href = `/PlantillaPreContrato/Parrafos?id=${data.secPlantillaPreContrato}`;
    });
});

function mostrarModal(data = null) {
    $("#txtSecPlantillaPreContrato").val(data ? data.secPlantillaPreContrato : 0);
    $("#txtNombre").val(data ? data.nombre : "");
    $("#txtNumeracionInicial").val(data ? data.numeracionInicial : "");
    $("#cboEstado").val(data ? data.estaActivo : 1);

    fetch("/PlantillaPreContrato/ListaTiposDocumento")
        .then(response => response.json())
        .then(responseJson => {
            if (responseJson.data) {
                $("#cboTipoDocumento").empty();
                responseJson.data.$values.forEach(item => {
                    $("#cboTipoDocumento").append(new Option(item.text, item.value));
                });
                if (data) {
                    $("#cboTipoDocumento").val(data.secTipoDocumento);
                }
            }
        })
        .catch(error => {
            console.error("Error al cargar tipos de documento:", error);
        });

    $('#modalData').modal('show');
}

function GuardarCambios() {
    var objeto = {
        SecPlantillaPreContrato: parseInt($("#txtSecPlantillaPreContrato").val()),
        Nombre: $("#txtNombre").val(),
        NumeracionInicial: $("#txtNumeracionInicial").val(),
        EstaActivo: parseInt($("#cboEstado").val()),
        SecTipoDocumento: parseInt($("#cboTipoDocumento").val())
    }

    if (!objeto.Nombre || objeto.Nombre.trim() === "" || !objeto.SecTipoDocumento) {
        Swal.fire("Atención", "Por favor, complete todos los campos requeridos.", "warning");
        return;
    }

    fetch(`/PlantillaPreContrato/${objeto.SecPlantillaPreContrato == 0 ? "Crear" : "Editar"}`, {
        method: objeto.SecPlantillaPreContrato == 0 ? "POST" : "PUT",
        headers: {
            "Content-Type": "application/json; charset=utf-8",
        },
        body: JSON.stringify(objeto)
    })
    .then(response => response.json())
    .then(responseJson => {
        if (responseJson.resultado) {
            tablaData.ajax.reload();
            $('#modalData').modal('hide');
            Swal.fire("¡Guardado!", "Los cambios han sido guardados correctamente.", "success");
        } else {
            Swal.fire("Error", "No se pudieron guardar los cambios.", "error");
        }
    })
    .catch(error => {
        console.error("Error al guardar:", error);
        Swal.fire("Error", "Ocurrió un error al intentar guardar los cambios.", "error");
    });
}