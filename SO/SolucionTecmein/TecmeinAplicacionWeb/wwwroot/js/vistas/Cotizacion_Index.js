const MODELO_BASE = {
    secuencial: 0,
    secVisita: 0,
    nombreObra: "",
    nombreContacto: "",
    enviadoProveedor: false,
    enviadoCliente: false,
    confirmacion: false,
    subtotal: 0,
    valorIVA: 0,
    valorImportacion: 0,
    valorImpuestos: 0,
    totalConImpuestos: 0,
    estaActivo: 1,
    cotizaciondetalles: [],
    impuestoCotizaciones: [] // Asegúrate de que esta propiedad exista en tu modelo
}

let tablaData;

// Nueva función para mostrar el desglose de impuestos
function mostrarDesgloseImpuestos(impuestosCotizacion) {
    const divImpuestosDetalle = $('#divImpuestosDetalle');
    divImpuestosDetalle.empty(); // Limpiar cualquier desglose anterior

    // Asegúrate de que impuestosCotizacion sea un array y no esté vacío
    const impuestos = impuestosCotizacion ? (impuestosCotizacion.$values || impuestosCotizacion) : [];

    if (Array.isArray(impuestos) && impuestos.length > 0) {
        impuestos.forEach(impuesto => {
            const html = `
                <strong>${impuesto.nombreImpuesto} (${impuesto.tipoImpuestoDescripcion}): </strong><span>${impuesto.valorImpuesto.toFixed(2)}</span><br>
            `;
            divImpuestosDetalle.append(html);
        });
    } else {
        // Si no hay impuestos, puedes mostrar un mensaje o dejarlo vacío
        divImpuestosDetalle.append('<strong>No hay impuestos aplicados.</strong><br>');
    }
}

function actualizarEstadoBotonPdfCliente() {
    let habilitar = true;
    let mensaje = "";

    if (!$('#chkEnviadoProveedor').is(':checked')) {
        habilitar = false;
        mensaje = "Debe marcar 'Enviado a Proveedor' primero.";
    }

    if ($('#tbDetalles tbody tr').length === 0) {
        habilitar = false;
        mensaje = "Debe haber al menos un equipo en la cotización.";
    }

    if (habilitar) {
        $('#tbDetalles tbody tr').each(function () {
            const fila = $(this);
            const valorCompra = parseFloat(fila.find('.valor-compra').val()) || 0;
            const margenGanancia = parseFloat(fila.find('.margen-ganancia').val()) || 0;

            if (valorCompra <= 0 || margenGanancia <= 0) {
                habilitar = false;
                mensaje = "Todos los equipos deben tener un 'Valor Compra' y 'Margen (%)' mayor a 0.";
                return false;
            }
        });
    }

    const boton = $('#btnGenerarPdfCliente');
    const contenedor = boton.parent();

    boton.prop('disabled', !habilitar);

    if (!habilitar) {
        contenedor.attr('data-original-title', mensaje);
        boton.css('pointer-events', 'none');
    } else {
        contenedor.removeAttr('data-original-title');
        boton.css('pointer-events', 'auto');
    }
}

function calcularImpuestosEnFrontend(subtotal) {
    let valorIVACalculado = 0;
    let valorImportacionCalculado = 0;
    let impuestosAplicados = []; // To store details for display

    if (activeTaxes && activeTaxes.length > 0) {
        const ivaImpuesto = activeTaxes.find(i => i.vigente && i.esIva);
        if (ivaImpuesto && ivaImpuesto.porcentaje) {
            valorIVACalculado = subtotal * (ivaImpuesto.porcentaje / 100);
            impuestosAplicados.push({
                nombreImpuesto: ivaImpuesto.descripcion,
                tipoImpuestoDescripcion: ivaImpuesto.nombreTipoImpuesto,
                valorImpuesto: valorIVACalculado
            });
        }

        const importacionImpuesto = activeTaxes.find(i => i.vigente && i.esImportacion);
        if (importacionImpuesto) {
            if (importacionImpuesto.porcentaje) {
                valorImportacionCalculado = subtotal * (importacionImpuesto.porcentaje / 100);
            } else if (importacionImpuesto.valorFijo) {
                valorImportacionCalculado = importacionImpuesto.valorFijo;
            }
            if (valorImportacionCalculado > 0) {
                impuestosAplicados.push({
                    nombreImpuesto: importacionImpuesto.descripcion,
                    tipoImpuestoDescripcion: importacionImpuesto.nombreTipoImpuesto,
                    valorImpuesto: valorImportacionCalculado
                });
            }
        }
    }

    const valorImpuestosTotal = valorIVACalculado + valorImportacionCalculado;
    const totalConImpuestos = subtotal + valorImpuestosTotal;

    return {
        valorIVA: valorIVACalculado,
        valorImportacion: valorImportacionCalculado,
        valorImpuestos: valorImpuestosTotal,
        totalConImpuestos: totalConImpuestos,
        impuestosAplicados: impuestosAplicados
    };
}

function calcularTotalesGenerales() {
    let subtotal = 0;
    $('.total-fila').each(function() {
        subtotal += parseFloat($(this).text()) || 0;
    });
    $('#spanSubtotal').text(subtotal.toFixed(2));

    // Calculate taxes on the frontend
    const calculosImpuestos = calcularImpuestosEnFrontend(subtotal);
    $('#spanImpuestos').text(calculosImpuestos.valorImpuestos.toFixed(2));
    $('#spanTotal').text(calculosImpuestos.totalConImpuestos.toFixed(2));
    mostrarDesgloseImpuestos(calculosImpuestos.impuestosAplicados);
}

function limpiarModal() {
    $("#txtId").val("0");
    $("#cboVisita").val("");
    $('#txtNombreObra').val('');
    $("#tbDetalles tbody").empty();
    
    $('#visitDetailsContent, #hrContactDetails, #contactDetailsContent, #hrUserGenerator, #userGeneratorContent').hide();

    $('#pDireccionProyecto, #pProvincia, #pCanton, #pParroquia, #pConstructora, #pContacto, #pCorreoContacto, #pTelefonoContacto, #pUsuarioGenerador').text('');

    calcularTotalesGenerales(); // Esto establecerá el subtotal a 0.00
    $('#spanImpuestos').text("0.00"); // Limpiar explícitamente el total de impuestos
    $('#spanTotal').text("0.00"); // Limpiar explícitamente el total
    $('#divImpuestosDetalle').empty(); // Limpiar el desglose de impuestos
    $('#btnGuardar').prop('disabled', true);
    actualizarEstadoBotonPdfCliente();
}



function mostrarModal(modelo = MODELO_BASE) {
    limpiarModal();

    $("#txtId").val(modelo.secuencial);
    $("#cboVisita").val(modelo.secVisita);
    
    $("#txtNombreObra").val(modelo.nombreObra);
    $("#pUsuarioCotizador").text(modelo.nombreUsuario || "N/A");
    $("#pUsuarioModifica").text(modelo.nombreUsuarioModifica || "N/A");

    $("#tbDetalles tbody").empty();

    let detalles = modelo.cotizaciondetalles ? (modelo.cotizaciondetalles.$values || modelo.cotizaciondetalles) : [];

    if (Array.isArray(detalles) && detalles.length > 0) {
        detalles.forEach(detalle => {
            const fila = `
                <tr data-id-equipo="${detalle.secuencial}" data-sec-equipo-visita="${detalle.secEquipoVisita || ''}" data-esta-activo="${detalle.estaActivo || 1}">
                    <td>${detalle.detalleEquipo}</td>
                    <td><input type="number" class="form-control form-control-sm cantidad" value="${detalle.cantidad || 1}" min="1" step="1"></td>
                    <td><input type="number" class="form-control form-control-sm valor-compra" value="${detalle.valorCompra || 0}" min="0" step="0.01"></td>
                    <td><input type="number" class="form-control form-control-sm margen-ganancia" value="${detalle.margenGanancia || 0}" min="0" max="100" step="0.01"></td>
                    <td class="total-fila">${detalle.total || '0.00'}</td>
                    <td><button type="button" class="btn btn-danger btn-sm btn-eliminar-detalle"><i class="fas fa-trash-alt"></i></button></td>
                </tr>`;
            $("#tbDetalles tbody").append(fila);
        });
    }

    if(modelo.secVisita > 0) {
        fetch(`/Visita/ObtenerDetalleVisita?secuencialVisita=${modelo.secVisita}`)
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(respuestaJson => {
                if (respuestaJson.estado && respuestaJson.objeto) {
                    const detalle = respuestaJson.objeto;
                    if (detalle.codigoEtapa === 'PRE' || detalle.codigoEtapa === 'SEG' || detalle.codigoEtapa === 'COT') {
                        $('.btn-eliminar-detalle').prop('disabled', true);
                        $('.cantidad').prop('disabled', true);
                        $('.valor-compra').prop('disabled', true);
                        $('.margen-ganancia').prop('disabled', true);
                        $('#btnGuardar').prop('disabled', true);
                    }
                }
            });
        $("#cboVisita").trigger('change');
    }

    if (modelo.secuencial > 0) {
        $('#btnGuardar').prop('disabled', false);
        
    } else {
        $('#seccionPreContrato').hide();
    }

    $('#chkEnviadoProveedor').prop('checked', modelo.enviadoProveedor);
    $('#chkEnviadoCliente').prop('checked', modelo.enviadoCliente);

    $("#spanSubtotal").text(modelo.subtotal.toFixed(2));
    $("#spanImpuestos").text(modelo.valorImpuestos.toFixed(2));
    $("#spanTotal").text(modelo.totalConImpuestos.toFixed(2));

    // Mostrar el desglose de impuestos
    mostrarDesgloseImpuestos(modelo.impuestoCotizaciones);

    actualizarEstadoBotonPdfCliente();
    
    $("#modalData").modal("show");
}



let activeTaxes = []; // Global variable to store active taxes


$(document).ready(function () {
    
    $('body').tooltip({ selector: '[data-toggle="tooltip"]' });

    // Fetch active taxes
    fetch('/Impuesto/ListaActivos')
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            activeTaxes = responseJson.data.$values || responseJson.data;
        })
        .catch(err => console.error("Error cargando impuestos activos:", err));

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Cotizacion/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function(json) {
                return json.data.$values || json.data;
            }
        },
        "columns": [
            { "data": "secuencial", "visible": false, "searchable": false },
            { "data": "nombreObra" },
            { "data": "nombreContacto" },
            { "data": "enviadoProveedor", "render": function (data) { return data ? '<span class="badge badge-success">Sí</span>' : '<span class="badge badge-danger">No</span>'; }, "className": "dt-center dt-compact-col", "width": "1%" },
            { "data": "enviadoCliente", "render": function (data) { return data ? '<span class="badge badge-success">Sí</span>' : '<span class="badge badge-danger">No</span>'; }, "className": "dt-center dt-compact-col", "width": "1%" },
            { "data": "confirmacion", "render": function (data) { return data ? '<span class="badge badge-success">Sí</span>' : '<span class="badge badge-danger">No</span>'; }, "className": "dt-center dt-compact-col", "width": "1%" },
            { "data": "nombreUsuario" },
            { "data": "nombreUsuarioModifica" },
            { "data": "estaActivo", "render": function (data) { return data == 1 ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>'; }, "className": "dt-center dt-compact-col", "width": "1%" },
            { "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm" title="Editar Cotización"><i class="fas fa-pencil-alt"></i></button><button class="btn btn-info btn-seguimiento btn-sm" title="Ver Seguimientos"><i class="fas fa-book-open"></i></button><button class="btn btn-danger btn-eliminar btn-sm" title="Eliminar Cotización"><i class="fas fa-trash-alt"></i></button></div>', "orderable": false, "searchable": false }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [ { text: 'Exportar Excel', extend: 'excelHtml5', title: 'Reporte de Cotizaciones', exportOptions: { columns: [1, 2, 3, 4, 5, 6, 7, 8] } }, 'pageLength' ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" }
    });

    fetch('/Visita/ListaParaCotizacion')
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(respuestaJson => {
            const visitas = respuestaJson.data.$values || respuestaJson.data;
            const cboVisita = $('#cboVisita');
            cboVisita.empty().append('<option value="">Seleccione una visita</option>');
            if (visitas && Array.isArray(visitas)) {
                visitas.forEach(visita => { cboVisita.append(`<option value="${visita.secuencial}">${visita.nombre}</option>`); });
            }
        }).catch(err => console.error("Error cargando visitas:", err));

    $('#cboVisita').change(function() {
        const visitaId = $(this).val();
        const tbDetallesBody = $('#tbDetalles tbody');
        const cotizacionId = parseInt($('#txtId').val()) || 0;

        $('#visitDetailsContent, #hrContactDetails, #contactDetailsContent, #hrUserGenerator, #userGeneratorContent').hide();

        if (visitaId) {
            if (cotizacionId === 0) {
                fetch(`/Cotizacion/VerificarVisita?visitaId=${visitaId}`)
                    .then(response => response.ok ? response.json() : Promise.reject(new Error('Error en la respuesta del servidor')))
                    .then(responseJson => {
                        $('#btnGuardar').prop('disabled', responseJson.valor);
                        if(responseJson.valor) toastr.warning(`La visita seleccionada ya tiene una cotización activa.`);
                    })
                    .catch(error => {
                        console.error("Error al verificar la visita:", error);
                        toastr.error("No se pudo verificar el estado de la visita.");
                        $('#btnGuardar').prop('disabled', true);
                    });
            }
            const nombreVisita = $(this).find('option:selected').text().toUpperCase();
            $('#txtNombreObra').val(nombreVisita).css('font-weight', 'bold');

            if (cotizacionId === 0) {
                fetch(`/Visita/EquiposDeVisita?secuencialVisita=${visitaId}`)
                    .then(response => response.ok ? response.json() : Promise.reject(response))
                    .then(respuestaJson => {
                        const equipos = respuestaJson.data.$values || respuestaJson.data;
                        tbDetallesBody.empty();
                        if (equipos && Array.isArray(equipos)) {
                            equipos.forEach(equipo => {
                                const fila = `
                                <tr data-id-equipo="0" data-sec-equipo-visita="${equipo.secuencial || ''}" data-esta-activo="1">
                                    <td>${equipo.detalleEspecifico}</td>
                                    <td><input type="number" class="form-control form-control-sm cantidad" value="${equipo.cantidad || 1}" min="1" step="1"></td>
                                    <td><input type="number" class="form-control form-control-sm valor-compra" value="0" min="0" step="0.01"></td>
                                    <td><input type="number" class="form-control form-control-sm margen-ganancia" value="0" min="0" max="100" step="0.01"></td>
                                    <td class="total-fila">0.00</td>
                                    <td><button type="button" class="btn btn-danger btn-sm btn-eliminar-detalle"><i class="fas fa-trash-alt"></i></button></td>
                                </tr>`;
                                tbDetallesBody.append(fila);
                            });
                        }
                        calcularTotalesGenerales();
                        actualizarEstadoBotonPdfCliente();
                    }).catch(err => console.error("Error cargando equipos:", err));
            }

            fetch(`/Visita/ObtenerDetalleVisita?secuencialVisita=${visitaId}`)
                .then(response => response.ok ? response.json() : Promise.reject(response))
                .then(respuestaJson => {
                    if (respuestaJson.estado && respuestaJson.objeto) {
                        const detalle = respuestaJson.objeto;
                        $('#pDireccionProyecto').text(detalle.direccion || 'N/A');
                        $('#pProvincia').text(detalle.nombreProvincia || 'N/A');
                        $('#pCanton').text(detalle.nombreCanton || 'N/A');
                        $('#pParroquia').text(detalle.nombreParroquia || 'N/A');
                        $('#visitDetailsContent').show();

                        if (detalle.nombreConstructora || detalle.nombreContacto || detalle.correoContacto || detalle.telefonoContacto) {
                            $('#pConstructora').text(detalle.nombreConstructora || 'N/A');
                            $('#pContacto').text(detalle.nombreContacto || 'N/A');
                            $('#pCorreoContacto').text(detalle.correoContacto || 'N/A');
                            $('#pTelefonoContacto').text(detalle.telefonoContacto || 'N/A');
                            $('#hrContactDetails, #contactDetailsContent').show();
                        }

                        if (detalle.nombreUsuario) {
                            $('#pUsuarioGenerador').text(detalle.nombreUsuario || 'N/A');
                            $('#hrUserGenerator, #userGeneratorContent').show();
                        }
                    } else {
                        console.error("Error al obtener detalle de visita:", respuestaJson.mensajes);
                        $('#visitDetailsContent, #hrContactDetails, #contactDetailsContent, #hrUserGenerator, #userGeneratorContent').hide();
                    }
                }).catch(err => {
                    console.error("Error en fetch de detalle de visita:", err);
                    $('#visitDetailsContent, #hrContactDetails, #contactDetailsContent, #hrUserGenerator, #userGeneratorContent').hide();
                });
        } else {
            limpiarModal();
        }
    });

    $(document).on('input', '.valor-compra, .margen-ganancia, .cantidad', function() {
        const fila = $(this).closest('tr');

        if ($(this).hasClass('margen-ganancia')) {
            let valor = parseFloat($(this).val());
            if (valor > 100) $(this).val(100);
            else if (valor < 0) $(this).val(0);
        }

        const cantidad = parseInt(fila.find('.cantidad').val()) || 1;
        const valorCompra = parseFloat(fila.find('.valor-compra').val()) || 0;
        const margenGanancia = parseFloat(fila.find('.margen-ganancia').val()) || 0;
        const totalFila = valorCompra * cantidad * (1 + margenGanancia / 100);
        fila.find('.total-fila').text(totalFila.toFixed(2));
        
        calcularTotalesGenerales();
        actualizarEstadoBotonPdfCliente();
    });

    $(document).on('click', '.btn-eliminar-detalle', function() {
        const fila = $(this).closest('tr');
        Swal.fire({
            title: '¿Está Seguro de Eliminar?',
            text: `Eliminar el equipo "${fila.find('td:first').text().substring(0, 25)}" de la cotización.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'No, cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fila.remove();
                calcularTotalesGenerales();
                actualizarEstadoBotonPdfCliente();
                toastr.success("Equipo eliminado.");
            }
        });
    });

    $('#btnGuardar').click(function () {
        const modelo = structuredClone(MODELO_BASE);
        modelo.secuencial = parseInt($('#txtId').val());
        modelo.secVisita = parseInt($('#cboVisita').val());
        modelo.enviadoProveedor = $('#chkEnviadoProveedor').is(':checked');
        modelo.enviadoCliente = $('#chkEnviadoCliente').is(':checked');

        const detalles = [];
        $('#tbDetalles tbody tr').each(function () {
            const fila = $(this);
            detalles.push({
                secuencial: parseInt(fila.data('id-equipo')) || 0,
                secCotizacion: modelo.secuencial,
                secEquipoVisita: parseInt(fila.data('sec-equipo-visita')) || null,
                detalleEquipo: fila.find('td:first').text(),
                cantidad: parseInt(fila.find('.cantidad').val()) || 1,
                valorCompra: parseFloat(String(fila.find('.valor-compra').val()).replace(',', '.')) || 0,
                margenGanancia: parseFloat(String(fila.find('.margen-ganancia').val()).replace(',', '.')) || 0,
                total: parseFloat(String(fila.find('.total-fila').text()).replace(',', '.')) || 0,
                estaActivo: parseInt(fila.data('esta-activo')) || 1
            });
        });
        modelo.cotizaciondetalles = detalles;

        const esNuevo = modelo.secuencial === 0;
        const url = esNuevo ? '/Cotizacion/Crear' : '/Cotizacion/Editar';
        const method = esNuevo ? 'POST' : 'PUT';

        const formData = new FormData();
        formData.append('modelo', JSON.stringify(modelo));

        $("#modalData .modal-content").LoadingOverlay("show");

        fetch(url, { method: method, body: formData })
            .then(response => {
                $("#modalData .modal-content").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.ajax.reload();
                    $('#modalData').modal('hide');
                    Swal.fire('Listo!', `La cotización fue ${esNuevo ? 'creada' : 'editada'} exitosamente.`, 'success');
                } else {
                    Swal.fire('Error', responseJson.mensajes, 'error');
                }
            }).catch(err => {
                $("#modalData .modal-content").LoadingOverlay("hide");
                Swal.fire('Error', 'No se pudo conectar con el servidor', 'error');
            });
    });

    $("#btnNuevo").click(function () {
        mostrarModal();
    });

    $("#tbdata tbody").on("click", ".btn-editar", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(fila).data();

        $("#modalData .modal-content").LoadingOverlay("show");

        fetch(`/Cotizacion/Detalle?id=${data.secuencial}`)
            .then(response => response.json())
            .then(cotizacionCompleta => {
                $("#modalData .modal-content").LoadingOverlay("hide");
                mostrarModal(cotizacionCompleta);
            })
            .catch(err => {
                $("#modalData .modal-content").LoadingOverlay("hide");
                Swal.fire('Error', 'No se pudo obtener la información de la cotización.', 'error');
            });
    });

    $("#tbdata tbody").on("click", ".btn-eliminar", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(fila).data();

        Swal.fire({
            title: 'Está Seguro de Eliminar?',
            text: `Eliminar la cotización para la obra "${data.nombreObra}"`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'No, cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $(".showSweetAlert").LoadingOverlay("show");
                fetch(`/Cotizacion/Eliminar?id=${data.secuencial}`, { method: "DELETE" })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            Swal.fire('Listo!', 'La cotización fue eliminada.', 'success');
                        } else {
                            Swal.fire('Error', responseJson.mensajes, 'error');
                        }
                    })
                    .catch(err => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        Swal.fire('Error', 'No se pudo conectar con el servidor.', 'error');
                    });
            }
        });
    });

    $('#btnGenerarPdf').on('click', function () {
        const cotizacionId = parseInt($('#txtId').val());

        if (!cotizacionId || cotizacionId === 0) {
            toastr.warning("Por favor, guarde la cotización antes de generar el PDF de solicitud.", "Aviso");
            return;
        }

        // Deshabilitar temporalmente para evitar doble click
        const boton = $(this);
        boton.prop('disabled', true);

        const url = `/Cotizacion/GenerarPDFSolicitud?idCotizacion=${cotizacionId}`;
        
        // Abrir la URL en una nueva pestaña. El navegador gestionará la descarga.
        window.open(url, '_blank');

        // Rehabilitar el botón después de un breve momento
        setTimeout(() => {
            boton.prop('disabled', false);
        }, 1000); // 1 segundo de espera
    });

    $('#btnGenerarPdfCliente').on('click', function () {
        const cotizacionId = parseInt($('#txtId').val());

        if (!cotizacionId || cotizacionId === 0) {
            toastr.warning("Por favor, guarde la cotización antes de generar el PDF.", "Aviso");
            return;
        }

        // Deshabilitar temporalmente para evitar doble click
        const boton = $(this);
        boton.prop('disabled', true);

        const url = `/Cotizacion/GenerarPDF?idCotizacion=${cotizacionId}`;
        
        // Abrir la URL en una nueva pestaña. El navegador gestionará la descarga.
        window.open(url, '_blank');

        // Rehabilitar el botón después de un breve momento
        setTimeout(() => {
            boton.prop('disabled', false);
        }, 1000); // 1 segundo de espera
    });

    

    

    $(document).on('input', '.valor-compra, .margen-ganancia', actualizarEstadoBotonPdfCliente);
    $('#chkEnviadoProveedor').on('change', actualizarEstadoBotonPdfCliente);
});

// --- Lógica para la gestión de Seguimientos ---

let tablaSeguimiento; // Para la DataTable de seguimientos
const MODELO_SEGUIMIENTO_BASE = {
    secSeguimiento: 0,
    secCotizacion: 0, // Se establecerá desde hiddenCotizacionId
    accion: "",
    detalle: "",
    fechaAccion: "",
    aceptacionCliente: false
};

function limpiarFormularioSeguimiento() {
    $("#txtIdSeguimiento").val("0");
    $("#txtAccion").val("");
    $("#txtDetalle").val("");
    $("#txtFechaAccion").val(""); // Limpiar input de fecha
    $("#chkAceptacionCliente").prop("checked", false);
}

function mostrarModalDataSeguimiento(modelo = MODELO_SEGUIMIENTO_BASE) {
    limpiarFormularioSeguimiento();

    $("#txtIdSeguimiento").val(modelo.secSeguimiento);
    $("#txtAccion").val(modelo.accion);
    $("#txtDetalle").val(modelo.detalle);
    // Formatear fecha para input type="date" (YYYY-MM-DD)
    if (modelo.fechaAccion) {
        const date = new Date(modelo.fechaAccion);
        const formattedDate = date.toISOString().split('T')[0];
        $("#txtFechaAccion").val(formattedDate);
    } else {
        $("#txtFechaAccion").val("");
    }
    $("#chkAceptacionCliente").prop("checked", modelo.aceptacionCliente);

    $("#modalDataSeguimiento").modal("show");
}

function abrirModalSeguimientos(cotizacionId) {
    $("#hiddenCotizacionId").val(cotizacionId); // Almacenar el ID de la cotización actual

    // Destruir DataTable existente si ya está inicializada
    if ($.fn.DataTable.isDataTable('#tbSeguimiento')) {
        tablaSeguimiento.destroy();
    }

    tablaSeguimiento = $('#tbSeguimiento').DataTable({
        responsive: true,
        "ajax": {
            "url": `/Seguimiento/Lista?secCotizacion=${cotizacionId}`,
            "type": "GET",
            "datatype": "json",
            "dataSrc": function(json) {
                return json.data.$values || json.data;
            }
        },
        "columns": [
            { "data": "accion" },
            { "data": "detalle" },
            { "data": "fechaAccion", "render": function(data) {
                // Formatear fecha a formato local
                const date = new Date(data);
                return date.toLocaleDateString();
            }},
            { "data": "aceptacionCliente", "render": function (data) { return data ? '<span class="badge badge-success">Sí</span>' : '<span class="badge badge-danger">No</span>'; } },
            { "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar-seguimiento btn-sm" title="Editar Seguimiento"><i class="fas fa-pencil-alt"></i></button><button class="btn btn-danger btn-eliminar-seguimiento btn-sm" title="Eliminar Seguimiento"><i class="fas fa-trash-alt"></i></button></div>', "orderable": false, "searchable": false }
        ],
        order: [[2, "desc"]], // Ordenar por FechaAccion descendente
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" }
    });

    $("#modalSeguimiento").modal("show");
}

$(document).ready(function () {
    // ... (código existente de Cotizacion_Index.js) ...

    // Evento para abrir el modal de seguimientos desde el botón en la tabla principal
    $("#tbdata tbody").on("click", ".btn-seguimiento", function () {
        const filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(filaSeleccionada).data();
        abrirModalSeguimientos(data.secuencial); // Pasar el ID de la cotización
    });

    // Evento para el botón "Nuevo Seguimiento" dentro del modal de seguimientos
    $("#btnNuevoSeguimiento").click(function () {
        mostrarModalDataSeguimiento();
    });

    // Evento para el botón "Editar" dentro de la tabla de seguimientos
    $("#tbSeguimiento tbody").on("click", ".btn-editar-seguimiento", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaSeguimiento.row(fila).data();
        mostrarModalDataSeguimiento(data);
    });

    // Evento para el botón "Guardar" en el modal de creación/edición de seguimiento
    $("#btnGuardarSeguimiento").click(function () {
        const modelo = structuredClone(MODELO_SEGUIMIENTO_BASE);
        modelo.secSeguimiento = parseInt($("#txtIdSeguimiento").val());
        modelo.secCotizacion = parseInt($("#hiddenCotizacionId").val()); // Obtener ID de cotización del campo oculto
        modelo.accion = $("#txtAccion").val();
        modelo.detalle = $("#txtDetalle").val();
        modelo.fechaAccion = $("#txtFechaAccion").val(); // Formato YYYY-MM-DD
        modelo.aceptacionCliente = $("#chkAceptacionCliente").is(":checked");

        // Validación básica
        if (modelo.accion.trim() === "" || modelo.detalle.trim() === "" || modelo.fechaAccion.trim() === "") {
            toastr.warning("Por favor, complete todos los campos obligatorios.", "Campos Incompletos");
            return;
        }

        const esNuevo = modelo.secSeguimiento === 0;
        const url = esNuevo ? '/Seguimiento/Crear' : '/Seguimiento/Editar';
        const method = esNuevo ? 'POST' : 'PUT';

        const formData = new FormData();
        formData.append('modelo', JSON.stringify(modelo));

        $("#modalDataSeguimiento .modal-content").LoadingOverlay("show");

        fetch(url, { method: method, body: formData })
            .then(response => {
                $("#modalDataSeguimiento .modal-content").LoadingOverlay("hide");
                if (!response.ok) {
                    // Si response.ok es false, intentar obtener JSON de error o simplemente rechazar
                    return response.json().catch(() => Promise.reject(new Error(`HTTP error! status: ${response.status}`)))
                                   .then(errorJson => Promise.reject(errorJson));
                }
                // Si response.ok es true, pero response.json() falla, significa que el cuerpo no es JSON válido
                return response.text().then(text => {
                    try {
                        return JSON.parse(text);
                    } catch (e) {
                        console.error("Failed to parse JSON response:", text, e);
                        return Promise.reject(new Error("Respuesta del servidor no es JSON válida."));
                    }
                });
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaSeguimiento.ajax.reload(); // Recargar la DataTable de seguimientos
                    $('#modalDataSeguimiento').modal('hide');
                    Swal.fire('Listo!', `El seguimiento fue ${esNuevo ? 'creado' : 'editado'} exitosamente.`, 'success');
                    
                } else {
                    Swal.fire('Error', responseJson.mensajes, 'error');
                }
            }).catch(err => {
                $("#modalDataSeguimiento .modal-content").LoadingOverlay("hide");
                // Manejo de errores más específico
                if (err instanceof Response) {
                    Swal.fire('Error', `Error de red o respuesta no válida del servidor (Status: ${err.status})`, 'error');
                } else if (err && err.mensajes) { // Si es un GenericResponse con mensajes
                    Swal.fire('Error', err.mensajes, 'error');
                } else if (err && err.message) { // Si es un Error de JS con mensaje
                    Swal.fire('Error', err.message, 'error');
                } else {
                    Swal.fire('Error', 'No se pudo conectar con el servidor o la respuesta no es JSON válida.', 'error');
                    console.error("Fetch error:", err); // Log del error real
                }
            });
    });

    // Evento para el botón "Eliminar" en la tabla de seguimientos
    $("#tbSeguimiento tbody").on("click", ".btn-eliminar-seguimiento", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaSeguimiento.row(fila).data();

        Swal.fire({
            title: '¿Está Seguro de Eliminar?',
            text: `Eliminar el seguimiento: "${data.accion}"`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'No, cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $(".showSweetAlert").LoadingOverlay("show");
                fetch(`/Seguimiento/Eliminar?secuencial=${data.secSeguimiento}`, { method: "DELETE" })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaSeguimiento.ajax.reload(); // Recargar la DataTable de seguimientos
                            Swal.fire('Listo!', 'El seguimiento fue eliminado.', 'success');
                        } else {
                            Swal.fire('Error', responseJson.mensajes, 'error');
                        }
                    })
                    .catch(err => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        Swal.fire('Error', 'No se pudo conectar con el servidor.', 'error');
                    });
            }
        });
    });
});