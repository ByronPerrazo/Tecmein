var tablaPlanesDePago;
var tablaCuotas;

function manejarErrorAjax(jqXHR, textStatus, errorThrown) {
    console.error(`Error AJAX: ${textStatus}`, errorThrown, jqXHR.responseText);
    const mensaje = jqXHR.responseJSON && jqXHR.responseJSON.mensajes ? jqXHR.responseJSON.mensajes : "Ocurrió un error de comunicación con el servidor.";
    Swal.fire("Error", mensaje, "error");
}

$(document).ready(function () {
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
            { "data": "valorContrato" },
            { "data": "valorAnticipo" },
            { "data": "numeroCuotas" },
            { "data": "fechaPrimeraCuota" },
            { "data": "estaActivo", render: data => data ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            {
                "defaultContent": '<button class="btn btn-info btn-ver-cuotas btn-sm"><i class="fas fa-eye"></i> Ver Cuotas</button>',
                "orderable": false, "searchable": false, "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: ['excelHtml5', 'pageLength'],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" },
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
                { "data": "montoEsperado" },
                { "data": "fechaVencimiento" },
                {
                    "data": "estado", render: function (data) {
                        if (data == "Pendiente") return '<span class="badge badge-warning">Pendiente</span>';
                        if (data == "Pagada") return '<span class="badge badge-success">Pagada</span>';
                        if (data == "Vencida") return '<span class="badge badge-danger">Vencida</span>';
                        return data;
                    }
                },
                {
                    "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-success btn-marcar-pagada btn-sm"><i class="fas fa-check"></i> Marcar Pagada</button>' +
                                      '<button class="btn btn-danger btn-marcar-vencida btn-sm"><i class="fas fa-exclamation-triangle"></i> Marcar Vencida</button></div>',
                    "orderable": false, "searchable": false, "width": "150px"
                }
            ],
            order: [[0, "asc"]],
            language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" },
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