var tablaPlanesDePago;
var tablaCuotas;

function manejarErrorAjax(jqXHR, textStatus, errorThrown) {
    console.error(`Error AJAX: ${textStatus}`, errorThrown, jqXHR.responseText);
    const mensaje = jqXHR.responseJSON && jqXHR.responseJSON.mensajes ? jqXHR.responseJSON.mensajes : "Ocurrió un error de comunicación con el servidor.";
    Swal.fire("Error", mensaje, "error");
}

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

    tablaPlanesDePago = $('#tbPlanesDePago').DataTable({
        responsive: true,
        "ajax": {
            "url": '/PlanDePago/Lista',
            "type": "GET",
            "datatype": "json",
            "error": manejarErrorAjax
        },
        "columns": [
            { "data": "idPlanDePago" },
            { "data": "idContrato" },
            { "data": "descripcionFormaPago" },
            { 
                "data": "valorContrato",
                "render": function(data) {
                    return data ? data.toLocaleString('es-ES', { style: 'currency', currency: 'USD' }) : "$0.00";
                }
            },
            { 
                "data": "valorAnticipo",
                "render": function(data) {
                    return data ? data.toLocaleString('es-ES', { style: 'currency', currency: 'USD' }) : "$0.00";
                }
            },
            { "data": "numeroCuotas" },
            { 
                "data": "fechaPrimeraCuota",
                "render": function(data) {
                    if (!data) return "";
                    const fecha = new Date(data);
                    if (isNaN(fecha.getTime())) return data;
                    return fecha.toLocaleDateString('es-ES');
                }
            },
            { "data": "estaActivo", render: data => data ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            {
                "defaultContent": '<button class="btn btn-info btn-ver-cuotas btn-sm rounded-pill"><i class="fas fa-eye mr-1"></i> Ver Cuotas</button>',
                "orderable": false, "searchable": false, "width": "120px"
            }
        ],
        order: [[0, "desc"]],
        dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
        buttons: [
            {
                text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                extend: 'excelHtml5',
                title: 'Planes de Pago',
                filename: 'Reporte Planes de Pago',
                exportOptions: { columns: [0, 1, 2, 3, 4, 5, 6, 7] },
                className: 'btn btn-link btn-sm p-1'
            }
        ],
        language: lenguajeEspanol
    });

    $('#tbPlanesDePago tbody').on('click', '.btn-ver-cuotas', function () {
        var data = tablaPlanesDePago.row($(this).parents('tr')).data();
        abrirModalCuotas(data.idPlanDePago);
    });

    function abrirModalCuotas(idPlanDePago) {
        $('#planDePagoIdModal').text(idPlanDePago);

        if ($.fn.DataTable.isDataTable('#tbCuotas')) {
            $('#tbCuotas').DataTable().destroy();
        }
        tablaCuotas = $('#tbCuotas').DataTable({
            responsive: true,
            "ajax": {
                "url": '/PlanDePago/ObtenerCuotasPorPlanDePago?idPlanDePago=' + idPlanDePago,
                "type": "GET",
                "datatype": "json",
                "error": manejarErrorAjax
            },
            "columns": [
                { "data": "numeroCuota" },
                { 
                    "data": "montoEsperado",
                    "render": function(data) {
                        return data ? data.toLocaleString('es-ES', { style: 'currency', currency: 'USD' }) : "$0.00";
                    }
                },
                { 
                    "data": "fechaVencimiento",
                    "render": function(data) {
                        if (!data) return "";
                        const fecha = new Date(data);
                        if (isNaN(fecha.getTime())) return data;
                        return fecha.toLocaleDateString('es-ES');
                    }
                },
                {
                    "data": "estado", render: function (data) {
                        if (data == "Pendiente") return '<span class="badge badge-warning">Pendiente</span>';
                        if (data == "Pagada") return '<span class="badge badge-success">Pagada</span>';
                        if (data == "Vencida") return '<span class="badge badge-danger">Vencida</span>';
                        return data;
                    }
                },
                {
                    "defaultContent": '<div class="btn-group gap-1" role="group">' +
                                      '<button class="btn btn-success btn-sm btn-marcar-pagada rounded-pill mr-1"><i class="fas fa-check mr-1"></i> Pagada</button>' +
                                      '<button class="btn btn-danger btn-sm btn-marcar-vencida rounded-pill"><i class="fas fa-exclamation-triangle mr-1"></i> Vencida</button>' +
                                      '</div>',
                    "orderable": false, "searchable": false, "width": "180px"
                }
            ],
            order: [[0, "asc"]],
            language: lenguajeEspanol
        });

        $('#modalCuotas').modal('show');
    }

    $('#tbCuotas tbody').on('click', '.btn-marcar-pagada', function () {
        var data = tablaCuotas.row($(this).parents('tr')).data();
        Swal.fire({
            title: '¿Está seguro?',
            text: `Marcar la cuota #${data.numeroCuota} como Pagada`,
            icon: 'question',
            showCancelButton: true
        }).then(result => {
            if (result.isConfirmed) {
                actualizarEstadoCuota(data.idCuota, "Pagada");
            }
        });
    });

    $('#tbCuotas tbody').on('click', '.btn-marcar-vencida', function () {
        var data = tablaCuotas.row($(this).parents('tr')).data();
        Swal.fire({
            title: '¿Está seguro?',
            text: `Marcar la cuota #${data.numeroCuota} como Vencida`,
            icon: 'warning',
            showCancelButton: true
        }).then(result => {
            if (result.isConfirmed) {
                actualizarEstadoCuota(data.idCuota, "Vencida");
            }
        });
    });

    function actualizarEstadoCuota(idCuota, nuevoEstado) {
        $.ajax({
            url: '/PlanDePago/ActualizarEstadoCuota',
            type: 'PUT',
            data: { idCuota: idCuota, nuevoEstado: nuevoEstado },
            success: function (response) {
                if (response.estado) {
                    Swal.fire('Actualizado', 'Estado de cuota actualizado exitosamente.', 'success');
                    tablaCuotas.ajax.reload();
                } else {
                    Swal.fire('Error', 'Error al actualizar estado de cuota: ' + response.mensajes, 'error');
                }
            },
            error: manejarErrorAjax
        });
    }
});