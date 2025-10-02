var tablaPlanesDePago;
var tablaCuotas;

$(document).ready(function () {
    // Inicializar DataTable de Planes de Pago
    tablaPlanesDePago = $('#tbPlanesDePago').DataTable({
        responsive: true,
        "ajax": {
            "url": '/PlanDePago/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idPlanDePago" },
            { "data": "idContrato" },
            { "data": "descripcionFormaPago" },
            { "data": "valorContrato" },
            { "data": "valorAnticipo" },
            { "data": "numeroCuotas" },
            { "data": "fechaPrimeraCuota" },
            {
                "data": "estaActivo", render: function (data) {
                    if (data == true)
                        return '<span class="badge badge-success">Activo</span>';
                    else
                        return '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                "defaultContent": '<button class="btn btn-info btn-ver-cuotas btn-sm"><i class="fas fa-eye"></i> Ver Cuotas</button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Planes de Pago',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    // Evento para el botón 'Ver Cuotas'
    $('#tbPlanesDePago tbody').on('click', '.btn-ver-cuotas', function () {
        var data = tablaPlanesDePago.row($(this).parents('tr')).data();
        abrirModalCuotas(data.idPlanDePago);
    });

    // Función para abrir el modal de Cuotas
    function abrirModalCuotas(idPlanDePago) {
        $('#planDePagoIdModal').text(idPlanDePago);

        // Inicializar o recargar DataTable de Cuotas
        if ($.fn.DataTable.isDataTable('#tbCuotas')) {
            $('#tbCuotas').DataTable().destroy();
        }
        tablaCuotas = $('#tbCuotas').DataTable({
            responsive: true,
            "ajax": {
                "url": '/PlanDePago/ObtenerCuotasPorPlanDePago?idPlanDePago=' + idPlanDePago,
                "type": "GET",
                "datatype": "json"
            },
            "columns": [
                { "data": "numeroCuota" },
                { "data": "montoEsperado" },
                { "data": "fechaVencimiento" },
                {
                    "data": "estado", render: function (data) {
                        if (data == "Pendiente")
                            return '<span class="badge badge-warning">Pendiente</span>';
                        else if (data == "Pagada")
                            return '<span class="badge badge-success">Pagada</span>';
                        else if (data == "Vencida")
                            return '<span class="badge badge-danger">Vencida</span>';
                        return data;
                    }
                },
                {
                    "defaultContent": '<button class="btn btn-success btn-marcar-pagada btn-sm mr-2"><i class="fas fa-check"></i> Marcar Pagada</button>' +
                                      '<button class="btn btn-danger btn-marcar-vencida btn-sm"><i class="fas fa-exclamation-triangle"></i> Marcar Vencida</button>',
                    "orderable": false,
                    "searchable": false,
                    "width": "150px"
                }
            ],
            order: [[0, "asc"]],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });

        $('#modalCuotas').modal('show');
    }

    // Evento para el botón 'Marcar Pagada' en la tabla de Cuotas
    $('#tbCuotas tbody').on('click', '.btn-marcar-pagada', function () {
        var data = tablaCuotas.row($(this).parents('tr')).data();
        if (confirm('¿Está seguro de marcar la cuota #' + data.numeroCuota + ' como Pagada?')) {
            actualizarEstadoCuota(data.idCuota, "Pagada");
        }
    });

    // Evento para el botón 'Marcar Vencida' en la tabla de Cuotas
    $('#tbCuotas tbody').on('click', '.btn-marcar-vencida', function () {
        var data = tablaCuotas.row($(this).parents('tr')).data();
        if (confirm('¿Está seguro de marcar la cuota #' + data.numeroCuota + ' como Vencida?')) {
            actualizarEstadoCuota(data.idCuota, "Vencida");
        }
    });

    // Función para actualizar el estado de una cuota
    function actualizarEstadoCuota(idCuota, nuevoEstado) {
        $.ajax({
            url: '/PlanDePago/ActualizarEstadoCuota',
            type: 'PUT',
            data: { idCuota: idCuota, nuevoEstado: nuevoEstado },
            success: function (response) {
                if (response.estado) {
                    alert('Estado de cuota actualizado exitosamente.');
                    tablaCuotas.ajax.reload();
                } else {
                    alert('Error al actualizar estado de cuota: ' + response.mensajes);
                }
            },
            error: function (error) {
                alert('Error de comunicación: ' + error.responseText);
            }
        });
    }
});
