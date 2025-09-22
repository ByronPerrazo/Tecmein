var tablaContratos;

$(document).ready(function () {
    // Inicializar Datepicker
    $('#txtFechaFirma').datepicker({ 
        format: 'dd/mm/yyyy',
        language: 'es',
        autoclose: true,
        todayHighlight: true
    });

    // Cargar PreContratos para el dropdown
    fetch("ListarPreContratos")
        .then(response => response.json())
        .then(dataJson => {
            $("#cboCotizacion").empty().append($('<option>').val('').text('Seleccionar'));
            let dataArray = [];
            if (dataJson && dataJson.data) {
                if (Array.isArray(dataJson.data.$values)) {
                    dataArray = dataJson.data.$values;
                } else if (Array.isArray(dataJson.data)) {
                    dataArray = dataJson.data;
                }
            }

            dataArray.forEach(item => {
                const numeroCotizacion = item.secCotizacion || 'N/A';
                const nombreObra = item.secCotizacionNavigation.secVisitaNavigation.nombre || 'Sin Nombre de Obra';
                $("#cboCotizacion").append($('<option>').val(item.secCotizacion).text(`${numeroCotizacion} - ${nombreObra}`));
            });
        });

    // Inicializar DataTable
    tablaContratos = $('#tbContrato').DataTable({
        responsive: true,
        "ajax": {
            "url": "Listar",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                // Debido a ReferenceHandler.Preserve en la serialización de .NET,
                // el array de datos viene en la propiedad $values.
                if (json.data && json.data.$values) {
                    return json.data.$values;
                }
                // Fallback por si la respuesta no tiene el wrapper $values
                return json.data || [];
            }
        },
        "columns": [
            { "data": "nombreObra" },
            { "data": "fechaFirma" },
            { "data": "nombreUsuarioCarga" },
            {
                "data": "rutaArchivo",
                "render": function(data, type, row) {
                    // Asegurarse de que la ruta es una URL válida
                    let url = data.startsWith('http') ? data : `/${data.replace(/\\/g, '/')}`;
                    return `<a href="${url}" target="_blank">${row.nombreArchivo}</a>`;
                }
            },
            {
                "data": "esActivo",
                "render": function(data) {
                    return data == 1 ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>';
                } 
             },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                                '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.21/i18n/Spanish.json"
        }
    });
});

function limpiarModal() {
    $('#txtIdContrato').val("0");
    $('#cboCotizacion').val('');
    $('#txtFechaFirma').val('');
    $('#cboEstado').val('1');
    $('#fileContrato').val('');
    $('#cboCotizacion').prop('disabled', false);
}

// Abrir modal para nuevo registro
$("#btnNuevo").click(function () {
    limpiarModal();
    $('#modalContrato').modal('show');
});

// Guardar
$("#btnGuardar").click(function () {
    var idContrato = $('#txtIdContrato').val();
    var idCotizacion = $('#cboCotizacion').val();
    var fechaFirma = $('#txtFechaFirma').val();
    var esActivo = $('#cboEstado').val();
    var archivo = $('#fileContrato')[0].files[0];

    if (!idCotizacion || !fechaFirma || (idContrato == "0" && !archivo)) {
        Swal.fire("Oops!", "Debe completar todos los campos obligatorios", "warning");
        return;
    }

    var formData = new FormData();
    var modelo = {
        IdContrato: parseInt(idContrato),
        IdCotizacion: parseInt(idCotizacion),
        FechaFirma: fechaFirma,
        EsActivo: parseInt(esActivo)
    };

    formData.append("modelo", JSON.stringify(modelo));
    formData.append("archivo", archivo); // El archivo puede ser null si se está editando

    const url = idContrato == "0" ? "Crear" : "Editar";
    const method = idContrato == "0" ? "POST" : "PUT";

    fetch(url, {
        method: method,
        body: formData
    })
    .then(response => {
        $('#modalContrato').modal('hide');
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        if(responseJson.success) {
            tablaContratos.ajax.reload();
            Swal.fire("Listo!", "Acción completada exitosamente", "success");
        } else {
            Swal.fire("Error", responseJson.message || "No se pudo completar la acción", "error");
        }
    }).catch(err => {
         Swal.fire("Error", "Ocurrió un error al procesar la solicitud", "error");
    })
});

// Editar
$('#tbContrato tbody').on('click', '.btn-editar', function () {
    var fila = $(this).closest('tr');
    if (fila.hasClass('child')) {
        fila = fila.prev();
    }
    var data = tablaContratos.row(fila).data();

    limpiarModal();

    $('#txtIdContrato').val(data.idContrato);
    $('#cboCotizacion').val(data.idCotizacion);
    $('#cboCotizacion').prop('disabled', true); // No se debería poder cambiar la cotización
    $('#txtFechaFirma').val(data.fechaFirma);
    // No se carga el archivo, el usuario debe seleccionar uno nuevo si desea cambiarlo

    $('#modalContrato').modal('show');
});

// Eliminar
$('#tbContrato tbody').on('click', '.btn-eliminar', function () {
    var fila = $(this).closest('tr');
    if (fila.hasClass('child')) {
        fila = fila.prev();
    }
    var data = tablaContratos.row(fila).data();

    Swal.fire({
        title: '¿Está seguro?',
        text: `¿Desea eliminar el contrato de la obra "${data.nombreObra}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'No, cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`Eliminar?id=${data.idContrato}`, { 
                method: "DELETE"
            })
            .then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.success) {
                    tablaContratos.row(fila).remove().draw();
                    Swal.fire('Eliminado!', 'El contrato ha sido eliminado.', 'success');
                } else {
                    Swal.fire('Error!', responseJson.message || 'No se pudo eliminar el contrato.', 'error');
                }
            })
            .catch(err => {
                 Swal.fire("Error", "Ocurrió un error al procesar la solicitud", "error");
            });
        }
    });
});