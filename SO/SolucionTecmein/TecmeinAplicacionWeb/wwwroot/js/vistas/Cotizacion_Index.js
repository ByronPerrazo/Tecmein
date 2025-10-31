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
    impuestoCotizaciones: []
}

let tablaData;
let activeTaxes = [];

// #region Funciones de Utilidad y Cálculo
function manejarErrorFetch(error, operacion) {
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

function mostrarDesgloseImpuestos(impuestosCotizacion) {
    const divImpuestosDetalle = $('#divImpuestosDetalle');
    divImpuestosDetalle.empty();
    const impuestos = impuestosCotizacion ? (impuestosCotizacion.$values || impuestosCotizacion) : [];

    if (Array.isArray(impuestos) && impuestos.length > 0) {
        impuestos.forEach(impuesto => {
            divImpuestosDetalle.append(`<strong>${impuesto.nombreImpuesto} (${impuesto.tipoImpuestoDescripcion}): </strong><span>${impuesto.valorImpuesto.toFixed(2)}</span><br>`);
        });
    } else {
        divImpuestosDetalle.append('<strong>No hay impuestos aplicados.</strong><br>');
    }
}

function calcularImpuestosEnFrontend(subtotal) {
    let valorIVACalculado = 0, valorImportacionCalculado = 0;
    let impuestosAplicados = [];

    if (activeTaxes && activeTaxes.length > 0) {
        const ivaImpuesto = activeTaxes.find(i => i.vigente && i.esIva);
        if (ivaImpuesto && ivaImpuesto.porcentaje) {
            valorIVACalculado = subtotal * (ivaImpuesto.porcentaje / 100);
            impuestosAplicados.push({ nombreImpuesto: ivaImpuesto.descripcion, tipoImpuestoDescripcion: ivaImpuesto.nombreTipoImpuesto, valorImpuesto: valorIVACalculado });
        }

        const importacionImpuesto = activeTaxes.find(i => i.vigente && i.esImportacion);
        if (importacionImpuesto) {
            valorImportacionCalculado = importacionImpuesto.porcentaje ? subtotal * (importacionImpuesto.porcentaje / 100) : (importacionImpuesto.valorFijo || 0);
            if (valorImportacionCalculado > 0) {
                impuestosAplicados.push({ nombreImpuesto: importacionImpuesto.descripcion, tipoImpuestoDescripcion: importacionImpuesto.nombreTipoImpuesto, valorImpuesto: valorImportacionCalculado });
            }
        }
    }

    const valorImpuestosTotal = valorIVACalculado + valorImportacionCalculado;
    return { valorImpuestos: valorImpuestosTotal, totalConImpuestos: subtotal + valorImpuestosTotal, impuestosAplicados };
}

function calcularFilaDetalle(fila) {
    const cantidad = parseFloat(String(fila.find('.cantidad').val()).replace(',', '.')) || 0;
    const valorCompra = parseFloat(String(fila.find('.valor-compra').val()).replace(',', '.')) || 0;
    const margenGanancia = parseFloat(String(fila.find('.margen-ganancia').val()).replace(',', '.')) || 0;

    const valorVentaUnitario = valorCompra * (1 + margenGanancia / 100);
    const totalFila = valorVentaUnitario * cantidad;

    fila.find('.valor-venta-unitario').text(valorVentaUnitario.toFixed(2));
    fila.find('.total-fila').text(totalFila.toFixed(2));
}

function calcularTotalesGenerales() {
    let subtotal = 0;
    $('.total-fila').each(function() { subtotal += parseFloat($(this).text()) || 0; });
    $('#spanSubtotal').text(subtotal.toFixed(2));

    const calculosImpuestos = calcularImpuestosEnFrontend(subtotal);
    $('#spanImpuestos').text(calculosImpuestos.valorImpuestos.toFixed(2));
    $('#spanTotal').text(calculosImpuestos.totalConImpuestos.toFixed(2));
    mostrarDesgloseImpuestos(calculosImpuestos.impuestosAplicados);
} // End of calcularTotalesGenerales

function handleVisitaChange(visitaId, idCotizacionActual) {
    if (!visitaId) { limpiarModal(); return; }

    // Only fetch equipment if it's a new quotation (idCotizacionActual === 0)
    if (idCotizacionActual === 0) {
        $('#btnGuardar').prop('disabled', true);
        fetch(`/Cotizacion/VerificarVisita?visitaId=${visitaId}`)
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                if(responseJson.valor) {
                    toastr.warning(`La visita seleccionada ya tiene una cotización activa.`);
                } else {
                    $('#btnGuardar').prop('disabled', false);
                }
            }).catch(err => manejarErrorFetch(err, "Verificación de Visita"));

        fetch(`/Visita/EquiposDeVisita?secuencialVisita=${visitaId}`)
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(responseJson => {
                const tbody = $("#tbDetalles tbody");
                tbody.empty();
                if (responseJson.data && responseJson.data.$values && responseJson.data.$values.length > 0) {
                    responseJson.data.$values.forEach(equipo => {
                        const fila = `\n                            <tr class=\"text-xs\" data-id-equipo=\"0\" data-sec-equipo-visita=\"${equipo.secuencial}\" data-esta-activo=\"1\">\n                                <td>${equipo.descripcionImpresa}</td>\n                                <td><input type=\"number\" class=\"form-control form-control-sm cantidad\" value=\"1\" min=\"1\"></td>\n                                <td><input type=\"text\" class=\"form-control form-control-sm valor-compra\" value=\"0\"></td>\n                                <td><input type=\"text\" class=\"form-control form-control-sm margen-ganancia\" value=\"0\"></td>\n                                <td class=\"valor-venta-unitario\">0.00</td>\n                                <td class=\"total-fila\">0.00</td>\n                                <td>\n                                    <button class=\"btn btn-danger btn-sm btn-eliminar-item\" data-toggle=\"tooltip\" title=\"Eliminar Equipo\">\n                                        <i class=\"fas fa-trash\"></i>\n                                    </button>\n                                </td>\n                            </tr>`;
                        const newRow = $(fila);
                        tbody.append(newRow);
                        calcularFilaDetalle(newRow); // Recalculate for the newly added row
                    });
                } else {
                    tbody.append('<tr><td colspan=\"7\">No hay equipos registrados para esta visita.</td></tr>');
                }
            }).catch(err => manejarErrorFetch(err, "Cargar Equipos de Visita"));
    } else {
        // If we are editing, the save button should be enabled by default
        $('#btnGuardar').prop('disabled', false);
    }
    
    fetch(`/Visita/ObtenerDetalleVisita?secuencialVisita=${visitaId}`)
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            if (responseJson.estado) {
                const visita = responseJson.objeto;
                $('#txtNombreObra').val(visita.nombre);
                $('#pDireccionProyecto').text(visita.direccion);
                $('#pProvincia').text(visita.nombreProvincia);
                $('#pCanton').text(visita.nombreCanton);
                $('#pParroquia').text(visita.nombreParroquia);
                $('#pConstructora').text(visita.nombreConstructora);
                $('#pContacto').text(visita.nombreContacto);
                $('#pCorreoContacto').text(visita.correoContacto);
                $('#pTelefonoContacto').text(visita.telefonoContacto);
                $('#pUsuarioGenerador').text(visita.nombreUsuario);

                $('#visitDetailsContent, #hrContactDetails, #contactDetailsContent, #hrUserGenerator, #userGeneratorContent').show();
            } else {
                toastr.error("No se pudieron cargar los detalles de la visita.");
            }
        }).catch(err => manejarErrorFetch(err, "Cargar Detalles de Visita"));
}

// #endregion

// #region Lógica del Modal Principal (Cotización)
function limpiarModal() {
    $("#txtId").val("0");
    $("#cboVisita").val("");
    $('#txtNombreObra').val('');
    $("#tbDetalles tbody").empty();
    $('#visitDetailsContent, #hrContactDetails, #contactDetailsContent, #hrUserGenerator, #userGeneratorContent').hide();
    $('#pDireccionProyecto, #pProvincia, #pCanton, #pParroquia, #pConstructora, #pContacto, #pCorreoContacto, #pTelefonoContacto, #pUsuarioGenerador').text('');
    calcularTotalesGenerales();
    $('#spanImpuestos, #spanTotal').text("0.00");
    $('#divImpuestosDetalle').empty();
    $('#btnGuardar').prop('disabled', true);
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
            console.log("Detalle de cotización cargado:", detalle); // Debugging line
            const fila = `
                <tr class="text-xs" data-id-equipo="${detalle.secuencial}" data-sec-equipo-visita="${detalle.secEquipoVisita || ''}" data-esta-activo="${detalle.estaActivo || 1}">
                    <td>${detalle.detalleEquipo}</td>
                    <td><input type="number" class="form-control form-control-sm cantidad" value="${detalle.cantidad}" min="1"></td>
                    <td><input type="text" class="form-control form-control-sm valor-compra" value="${detalle.valorCompra}"></td>
                    <td><input type="text" class="form-control form-control-sm margen-ganancia" value="${detalle.margenGanancia}"></td>
                    <td class="valor-venta-unitario">${(detalle.valorCompra * (1 + detalle.margenGanancia / 100)).toFixed(2)}</td>
                    <td class="total-fila">${detalle.total.toFixed(2)}</td>
                    <td>
                        <button class="btn btn-danger btn-sm btn-eliminar-item" data-toggle="tooltip" title="Eliminar Equipo">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>`;
            const newRow = $(fila); // Convert the string to a jQuery object
            $("#tbDetalles tbody").append(newRow);
            calcularFilaDetalle(newRow); // Recalculate for the newly added row
        });
    }

    if (modelo.secVisita > 0) {
        // Temporarily unbind the change event to prevent it from firing when setting the value
        $("#cboVisita").off('change');
        $("#cboVisita").val(modelo.secVisita);
        // Rebind the change event after setting the value
        $("#cboVisita").on('change', function() {
            handleVisitaChange($(this).val(), parseInt($('#txtId').val()));
        });
        // Manually trigger the logic for obtaining visit details, but not equipment if editing
        handleVisitaChange(modelo.secVisita, modelo.secuencial);
    }

    $('#btnGuardar').prop('disabled', modelo.secuencial <= 0);
    $('#chkEnviadoProveedor').prop('checked', modelo.enviadoProveedor);
    $('#chkEnviadoCliente').prop('checked', modelo.enviadoCliente);
    $("#spanSubtotal").text(modelo.subtotal.toFixed(2));
    $("#spanImpuestos").text(modelo.valorImpuestos.toFixed(2));
    $("#spanTotal").text(modelo.totalConImpuestos.toFixed(2));
    mostrarDesgloseImpuestos(modelo.impuestoCotizaciones);
    $("#modalData").modal("show");
} // End of mostrarModal

// #endregion

$(document).ready(function () {

    fetch('/Impuesto/ListaActivos')
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => { activeTaxes = responseJson.data.$values || responseJson.data; })
        .catch(err => manejarErrorFetch(err, "Carga de Impuestos"));

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        ajax: {
            url: '/Cotizacion/Lista',
            type: "GET",
            datatype: "json",
            dataSrc: function (json) {
                // Handle the $values property if present
                return json.data.$values || json.data;
            }
        },
        "columns": [
            { data: "secuencial", visible: false },
            { data: "nombreObra" },
            { data: "nombreContacto" },
            { data: "enviadoProveedor", render: function (data) { return data ? '<span class="badge badge-success">Sí</span>' : '<span class="badge badge-danger">No</span>'; } },
            { data: "enviadoCliente", render: function (data) { return data ? '<span class="badge badge-success">Sí</span>' : '<span class="badge badge-danger">No</span>'; } },
            { data: "confirmacion", render: function (data) { return data ? '<span class="badge badge-success">Sí</span>' : '<span class="badge badge-danger">No</span>'; } },
            { data: "nombreUsuario" },
            { data: "nombreUsuarioModifica" },
            { data: "estaActivo", render: function (data) { return data === 1 ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>'; } },
            {
                "defaultContent":
                    '<div class="btn-group" role="group">' +
                    '<button class="btn btn-primary btn-editar btn-sm" title="Editar"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-info btn-sm btn-seguimiento" title="Seguimiento"><i class="fas fa-history"></i></button>' +
                    '<button class="btn btn-warning btn-historial btn-sm" title="Historial"><i class="fas fa-book-open"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm" title="Eliminar"><i class="fas fa-trash-alt"></i></button>' +
                    '</div>',
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: 'Reporte de Cotizaciones',
                filename: 'Reporte de Cotizaciones',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8]
                }
            },
            {
                text: 'Exportar PDF',
                extend: 'pdfHtml5',
                title: 'Reporte de Cotizaciones',
                filename: 'Reporte de Cotizaciones',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8]
                }
            },
            'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    fetch('/Visita/ListaParaCotizacion')
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(respuestaJson => {
            console.log("Respuesta JSON de ListaParaCotizacion:", respuestaJson);
            const visitas = respuestaJson.data.$values || respuestaJson.data;
            console.log("Visitas procesadas:", visitas);
            const cboVisita = $('#cboVisita');
            cboVisita.empty().append('<option value="">Seleccione una visita</option>');
            if (visitas && Array.isArray(visitas)) {
                visitas.forEach(visita => { cboVisita.append(`<option value="${visita.secuencial}">${visita.nombre}</option>`); });
            }
        }).catch(err => manejarErrorFetch(err, "Carga de Visitas"));

    $('#cboVisita').change(function() {
        handleVisitaChange($(this).val(), parseInt($('#txtId').val()));
    });

    $('#btnGenerarPdfCliente').click(function () {
        const idCotizacion = parseInt($('#txtId').val());
        if (idCotizacion === 0) {
            Swal.fire("Advertencia", "Debe guardar la cotización antes de generar el PDF para el cliente.", "warning");
            return;
        }

        // Show loading overlay
        Swal.fire({
            title: 'Generando PDF...',
            text: 'Por favor, espere.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        fetch(`/Cotizacion/GenerarPDF?idCotizacion=${idCotizacion}`)
            .then(response => {
                Swal.close(); // Close loading overlay
                if (!response.ok) {
                    // Attempt to read error message from response body
                    return response.text().then(errorText => {
                        try {
                            const errorJson = JSON.parse(errorText);
                            throw new Error(errorJson.mensajes || errorText);
                        } catch {
                            throw new Error(errorText);
                        }
                    });
                }
                return response.blob();
            })
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                window.open(url, '_blank');
                window.URL.revokeObjectURL(url); // Clean up the object URL
            })
            .catch(error => {
                Swal.close(); // Ensure loading overlay is closed on error
                Swal.fire("Error", error.message || "Ocurrió un error al generar el PDF para el cliente.", "error");
            });
    });

    $('#btnGenerarPdf').click(function () {
        const idCotizacion = parseInt($('#txtId').val());
        if (idCotizacion === 0) {
            Swal.fire("Advertencia", "Debe guardar la cotización antes de generar el PDF.", "warning");
            return;
        }

        // Show loading overlay
        Swal.fire({
            title: 'Generando PDF de Solicitud...',
            text: 'Por favor, espere.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        fetch(`/Cotizacion/GenerarPDFSolicitud?idCotizacion=${idCotizacion}`)
            .then(response => {
                Swal.close(); // Close loading overlay
                if (!response.ok) {
                    // Attempt to read error message from response body
                    return response.text().then(errorText => {
                        try {
                            const errorJson = JSON.parse(errorText);
                            throw new Error(errorJson.mensajes || errorText);
                        } catch {
                            throw new Error(errorText);
                        }
                    });
                }
                return response.blob();
            })
            .then(blob => {
                const url = window.URL.createObjectURL(blob);
                window.open(url, '_blank');
                window.URL.revokeObjectURL(url); // Clean up the object URL
            })
            .catch(error => {
                Swal.close(); // Ensure loading overlay is closed on error
                Swal.fire("Error", error.message || "Ocurrió un error al generar el PDF de solicitud.", "error");
            });
    });

    $('#btnGuardar').click(function () {
        // --- LÓGICA DE CONSTRUCCIÓN DE MODELO RESTAURADA ---
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
        const method = 'POST';

        const formData = new FormData();
        formData.append('modelo', JSON.stringify(modelo));

        $("#modalData .modal-content").LoadingOverlay("show");
        fetch(url, { method: method, body: formData })
            .then(response => {
                $("#modalData .modal-content").LoadingOverlay("hide");
                if (!response.ok) {
                    // Check if the response is JSON before parsing
                    const contentType = response.headers.get("content-type");
                    if (contentType && contentType.indexOf("application/json") !== -1) {
                        return response.json().then(err => Promise.reject(err));
                    } else {
                        // If not JSON, create a custom error object
                        return response.text().then(text => Promise.reject({ status: response.status, mensajes: text }));
                    }
                }
                return response.json();
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
                manejarErrorFetch(err, "Guardar Cotización");
            });
    });

    $("#tbdata tbody").on("click", ".btn-editar", function () {
        let filaSeleccionada;
        if ($(this).closest("tr").hasClass("child")) {
            filaSeleccionada = $(this).closest("tr").prev();
        } else {
            filaSeleccionada = $(this).closest("tr");
        }
        const data = tablaData.row(filaSeleccionada).data();

        $("#modalData .modal-content").LoadingOverlay("show");
        fetch(`/Cotizacion/Detalle?id=${data.secuencial}`)
            .then(response => {
                $("#modalData .modal-content").LoadingOverlay("hide");
                if (!response.ok) return response.json().then(err => Promise.reject(err));
                return response.json();
            })
            .then(cotizacionCompleta => { mostrarModal(cotizacionCompleta); })
            .catch(err => {
                $("#modalData .modal-content").LoadingOverlay("hide");
                manejarErrorFetch(err, "Cargar Detalle de Cotización");
            });
    });

    $("#tbdata tbody").on("click", ".btn-eliminar", function () {
        // ...
        Swal.fire({ /* ... */ }).then((result) => {
            if (result.isConfirmed) {
                $(".showSweetAlert").LoadingOverlay("show");
                fetch(`/Cotizacion/Eliminar?id=${data.secuencial}`, { method: "DELETE" })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        if (!response.ok) return response.json().then(err => Promise.reject(err));
                        return response.json();
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
                        manejarErrorFetch(err, "Eliminar Cotización");
                    });
            }
        });
    });



    // Click handler for the "Seguimiento" button in the DataTable
    $("#tbdata tbody").on("click", ".btn-seguimiento", function () {
        let filaSeleccionada;
        if ($(this).closest("tr").hasClass("child")) {
            filaSeleccionada = $(this).closest("tr").prev();
        } else {
            filaSeleccionada = $(this).closest("tr");
        }
        const data = tablaData.row(filaSeleccionada).data();
        const idCotizacion = data.secuencial;

        mostrarHistorialSeguimiento(idCotizacion);
    });

    // Event listeners for dynamic calculation in the detail table
    $("#tbDetalles tbody").on("change keyup", ".cantidad, .valor-compra, .margen-ganancia", function () {
        const fila = $(this).closest("tr");
        calcularFilaDetalle(fila);
        calcularTotalesGenerales();
    });

    // Lógica de Seguimientos también refactorizada para usar manejarErrorFetch
    // ...

    // New handler for history button
    $("#tbdata tbody").on("click", ".btn-historial", function () {
        let filaSeleccionada;
        if ($(this).closest("tr").hasClass("child")) {
            filaSeleccionada = $(this).closest("tr").prev();
        } else {
            filaSeleccionada = $(this).closest("tr");
        }
        const data = tablaData.row(filaSeleccionada).data();
        const idCotizacion = data.secuencial;

        $('#modalHistorialLabel').text(`Historial de Cambios - Cotización #${idCotizacion}`);
        const container = $('#historial-cards-container');
        container.empty().html('<p class="text-center">Cargando historial...</p>'); // Show loading message
        $('#modalHistorial').modal('show');

        const ajaxUrl = `/Cotizacion/HistorialCambios?idCotizacion=${idCotizacion}`;

        fetch(ajaxUrl)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`Error al cargar el historial: ${response.statusText}`);
                }
                return response.json();
            })
            .then(json => {
                container.empty(); // Clear loading message
                const eventos = json.data.$values || json.data;

                if (eventos && eventos.length > 0) {
                    eventos.forEach(evento => {
                        const cardHtml = `
                            <div class="card shadow-sm mb-3">
                                <div class="card-header bg-light py-2">
                                    <h6 class="m-0 font-weight-bold text-primary">
                                        <i class="fas fa-user-clock"></i> ${evento.nombreUsuario || 'Sistema'}
                                        <small class="float-right text-muted">${evento.fechaHora}</small>
                                    </h6>
                                </div>
                                <div class="card-body py-2">
                                    <p class="mb-1"><strong>Evento:</strong> <span class="badge badge-info">${evento.tipoEvento}</span></p>
                                    <p class="mb-1"><strong>Detalle:</strong></p>
                                    <p class="text-monospace bg-white p-2 rounded border" style="font-size: 0.85rem;">${evento.detalle}</p>
                                </div>
                            </div>`;
                        container.append(cardHtml);
                    });
                } else {
                    container.html('<p class="text-center text-muted">No hay historial de cambios para esta cotización.</p>');
                }
            })
            .catch(error => {
                container.html(`<p class="text-center text-danger">${error.message}</p>`);
                console.error("Error en fetch historial:", error);
            });
    });
});
