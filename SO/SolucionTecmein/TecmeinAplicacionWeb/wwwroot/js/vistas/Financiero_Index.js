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
                    const data = response.objeto.$values;
                    
                    // Calculate totals
                    let totalContratado = 0;
                    let totalPagado = 0;
                    let saldoTotal = 0;
                    data.forEach(item => {
                        totalContratado += item.valorTotalContrato;
                        totalPagado += item.montoPagado;
                        saldoTotal += item.saldoPendiente;
                    });

                    // Format currency
                    const formatCurrency = (value) => value.toLocaleString('es-ES', { style: 'currency', currency: 'USD' });

                    // Update stat cards
                    $('#totalContratado').text(formatCurrency(totalContratado));
                    $('#totalPagado').text(formatCurrency(totalPagado));
                    $('#saldoTotal').text(formatCurrency(saldoTotal));
                    $('#planesActivos').text(data.length);

                    return data;
                } else {
                    console.error("Error cargando datos desde el servidor: " + (response.mensajes || "Formato de datos inesperado o sin array de valores."));
                    return [];
                }
            }
        },
        "columns": [
            { "data": "idPlanDePago", "visible": false }, // Hidden ID column
            { "data": "nombreProyecto" }, // Display project name instead of contract number
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
        $('#registroPagoUsuarioId').val($('#currentUserId').val()); // Obtener del input hidden 
        $('#registrarPagoModal').modal('show');
    });

    // Manejar click en botón "Guardar Pago" en el modal de registro
    $('#btnGuardarPago').on('click', function () {
        // Validaciones básicas antes de construir FormData
        var monto = parseFloat($('#registroPagoMonto').val());
        var fechaPago = $('#registroPagoFecha').val();
        var idPlanDePago = parseInt($('#registroPagoIdPlanDePago').val());
        var registradoPorUsuarioId = parseInt($('#registroPagoUsuarioId').val());
        var comprobanteFile = $('#registroPagoComprobanteFile')[0].files[0];

        if (isNaN(monto) || monto <= 0) {
            Swal.fire("Error", "Ingrese un monto válido para el pago.", "error");
            return;
        }
        if (!fechaPago) {
            Swal.fire("Error", "Seleccione una fecha para el pago.", "error");
            return;
        }
        if (!idPlanDePago) {
            Swal.fire("Error", "ID de Plan de Pago no válido.", "error");
            return;
        }
        //if (!registradoPorUsuarioId) {
        //    Swal.fire("Error", "ID de Usuario no válido.", "error");
        //    return;
        //}


        var formData = new FormData();
        formData.append('IdPlanDePago', idPlanDePago);
        formData.append('Monto', monto);
        formData.append('FechaPago', fechaPago);
       // formData.append('RegistradoPorUsuarioId', registradoPorUsuarioId);

        if (comprobanteFile) {
            formData.append('ComprobanteFile', comprobanteFile);
        }

        $.ajax({
            url: "/api/Pago/Registrar", // Corregida la URL para apuntar al PagoController
            type: "POST",
            data: formData, // Usar FormData
            processData: false, // Importante: no procesar los datos
            contentType: false, // Importante: no establecer el tipo de contenido (FormData lo hará)
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

    // Manejar cambio en el input de archivo para previsualización
    $('#registroPagoComprobanteFile').on('change', function (event) {
        var reader = new FileReader();
        reader.onload = function(){
            var output = document.getElementById('imgComprobantePreview');
            if (event.target.files[0] && event.target.files[0].type.startsWith('image')) {
                output.src = reader.result;
                output.style.display = 'block';
            } else {
                output.style.display = 'none'; // Ocultar si no es una imagen o no hay archivo
            }
        };
        if (event.target.files[0]) {
            reader.readAsDataURL(event.target.files[0]);
        } else {
            $('#imgComprobantePreview').hide();
        }
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
                $('#modalNombreProyecto').text(plan.nombreProyecto);
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