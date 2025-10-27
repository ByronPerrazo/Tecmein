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

function calcularTotalesGenerales() {
    let subtotal = 0;
    $('.total-fila').each(function() { subtotal += parseFloat($(this).text()) || 0; });
    $('#spanSubtotal').text(subtotal.toFixed(2));

    const calculosImpuestos = calcularImpuestosEnFrontend(subtotal);
    $('#spanImpuestos').text(calculosImpuestos.valorImpuestos.toFixed(2));
    $('#spanTotal').text(calculosImpuestos.totalConImpuestos.toFixed(2));
    mostrarDesgloseImpuestos(calculosImpuestos.impuestosAplicados);
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
            const fila = `<tr class="text-xs" data-id-equipo="${detalle.secuencial}" data-sec-equipo-visita="${detalle.secEquipoVisita || ''}" data-esta-activo="${detalle.estaActivo || 1}"> ... </tr>`; // Contenido de la fila
            $("#tbDetalles tbody").append(fila);
        });
    }

    if (modelo.secVisita > 0) {
        fetch(`/Visita/ObtenerDetalleVisita?secuencialVisita=${modelo.secVisita}`)
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(respuestaJson => { /* ... */ });
        $("#cboVisita").trigger('change');
    }

    $('#btnGuardar').prop('disabled', modelo.secuencial <= 0);
    $('#chkEnviadoProveedor').prop('checked', modelo.enviadoProveedor);
    $('#chkEnviadoCliente').prop('checked', modelo.enviadoCliente);
    $("#spanSubtotal").text(modelo.subtotal.toFixed(2));
    $("#spanImpuestos").text(modelo.valorImpuestos.toFixed(2));
    $("#spanTotal").text(modelo.totalConImpuestos.toFixed(2));
    mostrarDesgloseImpuestos(modelo.impuestoCotizaciones);
    $("#modalData").modal("show");
}
// #endregion

$(document).ready(function () {
    $('body').tooltip({ selector: '[data-toggle="tooltip"]' });

    fetch('/Impuesto/ListaActivos')
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => { activeTaxes = responseJson.data.$values || responseJson.data; })
        .catch(err => manejarErrorFetch(err, "Carga de Impuestos"));

    tablaData = $('#tbdata').DataTable({ /* ... configuración de datatable ... */ });

    fetch('/Visita/ListaParaCotizacion')
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(respuestaJson => {
            const visitas = respuestaJson.data.$values || respuestaJson.data;
            const cboVisita = $('#cboVisita');
            cboVisita.empty().append('<option value="">Seleccione una visita</option>');
            if (visitas && Array.isArray(visitas)) {
                visitas.forEach(visita => { cboVisita.append(`<option value="${visita.secuencial}">${visita.nombre}</option>`); });
            }
        }).catch(err => manejarErrorFetch(err, "Carga de Visitas"));

    $('#cboVisita').change(function() {
        const visitaId = $(this).val();
        if (!visitaId) { limpiarModal(); return; }

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
        
        // Cadenas de fetch para obtener datos de la visita y equipos
        // ... todas usando .catch(err => manejarErrorFetch(err, "Operación Específica"))
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
        const method = esNuevo ? 'POST' : 'PUT';

        const formData = new FormData();
        formData.append('modelo', JSON.stringify(modelo));

        $("#modalData .modal-content").LoadingOverlay("show");
        fetch(url, { method: method, body: formData })
            .then(response => {
                $("#modalData .modal-content").LoadingOverlay("hide");
                if (!response.ok) return response.json().then(err => Promise.reject(err));
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
        // ...
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

    // Lógica de Seguimientos también refactorizada para usar manejarErrorFetch
    // ...
});
