var tablaData;

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": "/PlantillaPreContrato/Lista",
            "type": "GET",
            "datatype": "json",
            "dataSrc": "data.$values"
        },
        "columns": [
            { "data": "secPlantillaPreContrato", "visible": false, "searchable": false },
            { "data": "nombre" },
            { "data": "numeracionInicial" },
            { "data": "fechaRegistro" },
            {
                "data": "estaActivo", "render": function (valor) {
                    if (valor == 1) {
                        return '<span class="badge badge-success">Activo</span>'
                    } else {
                        return '<span class="badge badge-danger">Inactivo</span>'
                    }
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>' +
                    '<button class="btn btn-info btn-parrafos btn-sm ml-2"><i class="fas fa-list"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Guardar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Plantillas',
                exportOptions: {
                    columns: [1, 2, 3, 4]
                }
            },
            {
                text: 'Guardar PDF',
                extend: 'pdfHtml5',
                title: 'Reporte Plantillas',
                filename: 'Reporte Plantillas',
                exportOptions: {
                    columns: [1, 2, 3, 4]
                },
                customize: function (doc) {
                    doc.content[1].table.widths = ['25%', '25%', '25%', '25%']
                }
            },
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
});

function mostrarModal(data = null) {
    $("#txtSecPlantillaPreContrato").val(data ? data.secPlantillaPreContrato : 0);
    $("#txtNombre").val(data ? data.nombre : "");
    $("#txtNumeracionInicial").val(data ? data.numeracionInicial : "");
    $("#cboEstado").val(data ? data.estaActivo : 1);

    $('#modalData').modal('show');
}

function GuardarCambios() {
    var objeto = {
        SecPlantillaPreContrato: parseInt($("#txtSecPlantillaPreContrato").val()),
        Nombre: $("#txtNombre").val(),
        NumeracionInicial: $("#txtNumeracionInicial").val(),
        EstaActivo: parseInt($("#cboEstado").val())
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

// Evento para el botón de editar
$('#tbdata tbody').on('click', '.btn-editar', function () {
    var data = tablaData.row($(this).parents('tr')).data();
    mostrarModal(data);
});

// Evento para el botón de eliminar
$('#tbdata tbody').on('click', '.btn-eliminar', function () {
    var data = tablaData.row($(this).parents('tr')).data();

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
$('#tbdata tbody').on('click', '.btn-parrafos', function () {
    let data = tablaData.row($(this).parents('tr')).data();
    window.location.href = `/PlantillaPreContrato/Parrafos?id=${data.secPlantillaPreContrato}`;
});