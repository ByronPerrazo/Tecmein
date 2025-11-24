var tablaPlanesPago; // Variable global para la DataTable principal
var IdPlanDePagoSeleccionado; // Para pasar entre modales

$(document).ready(function () {

    // Inicializar DataTable de Planes de Pago
    tablaPlanesPago = $('#tblPlanesPago').DataTable({
        "ajax": {
            "url": "/Financiero/ListaPlanesPago",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (response) {
                if (response.estado && response.objeto && Array.isArray(response.objeto.$values)) {
                    return response.objeto.$values;
                } else {
                    console.error("Error cargando datos desde el servidor: " + (response.mensajes || "Formato de datos inesperado o sin array de valores."));
                    return [];
                }
            }
        },
        "columns": [
            { "data": "idPlanDePago" },
            { "data": "numeroContrato" },
            { "data": "nombreCliente" },
            { "data": "valorTotalContrato" },
            { "data": "montoPagado" },
            { "data": "saldoPendiente" },
            { "data": "estadoPlan" },
            {
                "data": null,
                "defaultContent": '<button class="btn btn-info btn-sm btn-detalle-plan"><i class="fas fa-eye"></i> Ver Detalle</button>'
            }
        ],
        "language": {
            "url": "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" // Spanish language for DataTables
        },
        "responsive": true
    });

    // Manejar click en botón "Ver Detalle"
    $('#tblPlanesPago tbody').on('click', '.btn-detalle-plan', function () {
        var data = tablaPlanesPago.row($(this).parents('tr')).data();
        IdPlanDePagoSeleccionado = data.idPlanDePago;

        // Cargar y mostrar el modal de detalle
        cargarDetallePlanDePago(IdPlanDePagoSeleccionado);
    });

    // Manejar click en botón "Registrar Pago" dentro del modal de detalle
    $('#detallePlanPagoModal').on('click', '#btnRegistrarPagoModal', function () {
        $('#detallePlanPagoModal').modal('hide'); // Ocultar modal de detalle
        $('#registroPagoIdPlanDePago').val(IdPlanDePagoSeleccionado);
        $('#registroPagoFecha').val(new Date().toISOString().slice(0, 10)); // Fecha actual
        // Asignar RegistradoPorUsuarioId (desde un claim o variable global, ejemplo: @User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
        // Por ahora un valor quemado, esto debe venir del backend o del contexto del usuario logueado
        $('#registroPagoUsuarioId').val(1); 
        $('#registrarPagoModal').modal('show');
    });

    // Manejar click en botón "Guardar Pago" en el modal de registro
    $('#btnGuardarPago').on('click', function () {
        var formData = {
            IdPlanDePago: parseInt($('#registroPagoIdPlanDePago').val()),
            Monto: parseFloat($('#registroPagoMonto').val()),
            FechaPago: $('#registroPagoFecha').val(),
            ComprobanteUrl: $('#registroPagoComprobante').val(),
            RegistradoPorUsuarioId: parseInt($('#registroPagoUsuarioId').val())
        };

        if (isNaN(formData.Monto) || formData.Monto <= 0) {
            Swal.fire("Error", "Ingrese un monto válido para el pago.", "error");
            return;
        }
        if (!formData.FechaPago) {
            Swal.fire("Error", "Seleccione una fecha para el pago.", "error");
            return;
        }

        $.ajax({
            url: "/Financiero/RegistrarPago",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                if (response.estado) {
                    Swal.fire("Registrado!", "El pago ha sido registrado exitosamente.", "success");
                    $('#registrarPagoModal').modal('hide');
                    tablaPlanesPago.ajax.reload(); // Recargar la tabla principal
                } else {
                    Swal.fire("Error", response.mensajes || "No se pudo registrar el pago.", "error");
                }
            },
            error: function (error) {
                console.error("Error al registrar pago:", error);
                Swal.fire("Error", "Ocurrió un error al intentar registrar el pago.", "error");
            }
        });
    });



});

// Función para cargar el detalle del plan de pago en el modal
function cargarDetallePlanDePago(idPlanDePago) {
    $.ajax({
        url: "/Financiero/ObtenerDetallePlanDePago?idPlanDePago=" + idPlanDePago,
        type: "GET",
        success: function (response) {
            if (response.estado && response.objeto) {
                var plan = response.objeto;
                
                // Actualizar los campos del modal de detalle
                $('#modalNumeroContrato').text(plan.numeroContrato);
                $('#modalNombreCliente').text(plan.nombreCliente);
                $('#modalValorContrato').text(plan.valorContrato.toLocaleString('es-ES', { style: 'currency', currency: 'USD' }));
                $('#modalValorAnticipo').text(plan.valorAnticipo.toLocaleString('es-ES', { style: 'currency', currency: 'USD' }));
                $('#modalNumeroCuotas').text(plan.numeroCuotas);
                $('#modalFechaPrimeraCuota').text(new Date(plan.fechaPrimeraCuota).toLocaleDateString('es-ES'));
                $('#modalMontoPagadoTotal').text(plan.montoPagadoTotal.toLocaleString('es-ES', { style: 'currency', currency: 'USD' }));
                $('#modalSaldoPendienteTotal').text(plan.saldoPendienteTotal.toLocaleString('es-ES', { style: 'currency', currency: 'USD' }));

                // Destruir y reinicializar DataTable de Cuotas
                if ($.fn.DataTable.isDataTable('#tblCuotasPlanPago')) {
                    $('#tblCuotasPlanPago').DataTable().destroy();
                }

                $('#tblCuotasPlanPago').DataTable({
                    "data": plan.cuotas.$values || [], // Accediendo a $values
                    "columns": [
                        { "data": "numeroCuota" },
                        { "data": "montoEsperado" },
                        { "data": "montoPagado" },
                        { "data": "fechaVencimiento",
                          "render": function (data) {
                              return new Date(data).toLocaleDateString('es-ES');
                          }
                        },
                        { "data": "estado" }
                    ],
                    "language": {
                        "url": "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
                    },
                    "responsive": true,
                    "paging": false, // No paginar las cuotas en el modal
                    "info": false,   // No mostrar información de paginación
                    "searching": false // No buscar en las cuotas
                });

                $('#detallePlanPagoModal').modal('show');
            } else {
                Swal.fire("Error", response.mensajes || "No se pudo cargar el detalle del plan de pago.", "error");
            }
        },
        error: function (error) {
            console.error("Error al obtener detalle del plan de pago:", error);
            Swal.fire("Error", "Ocurrió un error al cargar el detalle del plan de pago.", "error");
        }
    });
}