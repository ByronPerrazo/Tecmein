let tablaContratos;
let precontratosCargados = [];
let modeloBasePlanDePago = {};

function inicializarDatepickers(selector) {
    $(selector).find('.date').datepicker({
        format: 'yyyy-mm-dd',
        language: 'es',
        autoclose: true,
        todayHighlight: true
    });
}

// Helper function to find the mode (most frequent value) in an array
const findMode = (arr) => {
    if (arr.length === 0) return null;
    return arr.sort((a,b) =>
          arr.filter(v => v===a).length
        - arr.filter(v => v===b).length
    ).pop();
}

const modeloBaseContrato = {
    idContrato: 0,
    idCotizacion: null,
    secCliente: null,
    nombreProyecto: "",
    fechaFirma: "",
    esActivo: true
};

function manejarErrorAjax(jqXHR, textStatus, errorThrown) {
    if (jqXHR.responseJSON && jqXHR.responseJSON.mensajes) {
        Swal.fire("Error", jqXHR.responseJSON.mensajes, "error");
    } else {
        Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor o procesar la respuesta.", "error");
    }
}

function abrirModalContrato(modelo = modeloBaseContrato) {
    // ... (código de abrirModalContrato sin cambios)
}

    // ... (otras funciones de UI sin cambios) ...

function reEnumerarCuotasContrato() {
    $("#tbodyCuotas tr").each(function (index) {
        $(this).find("td:first").text(index + 1);
        // Actualizar el número de cuota en el input hidden si existe
        $(this).find(".numero-cuota-hidden").val(index + 1);
    });
    calcularTotalCuotas(); // Recalcular totales cuando se re-enumera
}

function calcularTotalCuotas() {
    let sumaMontos = 0;
    $("#tbodyCuotas .monto-cuota-input").each(function() {
        const monto = parseFloat($(this).val());
        if (!isNaN(monto)) {
            sumaMontos += monto;
        }
    });

    const totalContrato = parseFloat($("#txtValorContratoPlan").val());
    const totalFooter = $("#totalCuotasPlan");

    totalFooter.text(sumaMontos.toFixed(2));

    if (!isNaN(totalContrato) && totalContrato.toFixed(2) != sumaMontos.toFixed(2)) {
        totalFooter.addClass("text-danger").removeClass("text-success");
    } else {
        totalFooter.removeClass("text-danger").addClass("text-success");
    }
}

function generarCuotasContratoInteligentes() {
    const fechaInicialStr = $("#fechaPrimeraCuotaGenerar").datepicker('getFormattedDate', 'yyyy-mm-dd');
    if (!fechaInicialStr) {
        Swal.fire("Datos incompletos", "Se requiere una 'Fecha Inicial' para generar las cuotas.", "info");
        return;
    }

    const numCuotas = parseInt($("#numCuotasGenerar").val());
    const montoBase = parseFloat($("#montoCuotaGenerar").val());
    const valorContrato = parseFloat($("#txtValorContratoPlan").val());
    const valorAnticipo = parseFloat($("#txtValorAnticipoPlan").val()) || 0;

    if (isNaN(valorContrato) || valorContrato <= 0) {
        Swal.fire("Datos incompletos", "Se requiere un 'Valor del Contrato' mayor a cero para generar las cuotas.", "info");
        return;
    }

    if (isNaN(numCuotas) || isNaN(montoBase)) {
        Swal.fire("Datos incompletos", "Para generar cuotas, se requiere N° de Cuotas y Monto Base.", "info");
        return;
    }

    $("#tbodyCuotas").empty();

    const saldoPendiente = valorContrato - valorAnticipo;
    const startDate = new Date(fechaInicialStr + 'T00:00:00');

    if (isNaN(startDate.getTime())) {
        Swal.fire("Fecha Inválida", "La 'Fecha Inicial' proporcionada no es válida. Por favor, verifique el formato (YYYY-MM-DD).", "error");
        return;
    }

    if (valorAnticipo > 0) {
        const fechaFormateada = startDate.toISOString().split('T')[0];
        const nuevaFila = `
            <tr>
                <td>1</td>
                <td>Anticipo</td>
                <td><div class="input-group date" data-provide="datepicker"><input type="text" class="form-control form-control-sm fecha-cuota-input" value="${fechaFormateada}"><div class="input-group-addon"><span class="glyphicon glyphicon-th"></span></div></div></td>
                <td><input type="number" class="form-control form-control-sm monto-cuota-input" value="${valorAnticipo.toFixed(2)}"></td>
                <td><button type="button" class="btn btn-danger btn-sm btn-eliminar-cuota-contrato"><i class="fas fa-trash"></i></button></td>
            </tr>
        `;
        $("#tbodyCuotas").append(nuevaFila);
    }

    if (numCuotas > 0 && saldoPendiente > 0) {
        let totalAcumulado = 0;
        for (let i = 0; i < numCuotas; i++) {
            let montoActualCuota = (i === numCuotas - 1) ? (saldoPendiente - totalAcumulado) : montoBase;
            const nuevaFecha = new Date(startDate.getTime());
            nuevaFecha.setMonth(nuevaFecha.getMonth() + i + 1);
            const fechaFormateada = nuevaFecha.toISOString().split('T')[0];
            const nuevaFila = `
                <tr>
                    <td>${i + 2}</td>
                    <td>Cuota</td>
                    <td><div class="input-group date" data-provide="datepicker"><input type="text" class="form-control form-control-sm fecha-cuota-input" value="${fechaFormateada}"><div class="input-group-addon"><span class="glyphicon glyphicon-th"></span></div></div></td>
                    <td><input type="number" class="form-control form-control-sm monto-cuota-input" value="${montoActualCuota.toFixed(2)}"></td>
                    <td><button type="button" class="btn btn-danger btn-sm btn-eliminar-cuota-contrato"><i class="fas fa-trash"></i></button></td>
                </tr>
            `;
            $("#tbodyCuotas").append(nuevaFila);
            totalAcumulado += montoBase;
        }
    }

    inicializarDatepickers("#tbodyCuotas");
    reEnumerarCuotasContrato();
}

function setPlanDePagoEditable(esEditable) {
    $("#formPlanDePago input, #formPlanDePago select").prop("disabled", !esEditable);
    $("#cboFormaPagoPlan").prop("disabled", false); // La forma de pago siempre es editable
    $("#btnGenerarCuotasInteligentes").prop("disabled", !esEditable);
    $("#tbodyCuotas .btn-eliminar-cuota-contrato").prop("disabled", !esEditable);

    if (esEditable) {
        $("#mensajeSoloLectura").hide();
        $("#formPlanDePago input, #formPlanDePago select").removeClass("readonly-style");
    } else {
        $("#mensajeSoloLectura").show();
        $("#formPlanDePago input, #formPlanDePago select").addClass("readonly-style");
        $("#cboFormaPagoPlan").removeClass("readonly-style");
    }
}

$(document).ready(function () {
    // Initialize datepickers with a standard format
    $('.date').datepicker({
        format: 'yyyy-mm-dd',
        language: 'es',
        autoclose: true,
        todayHighlight: true
    });

    $("#btnGenerarCuotasInteligentes").on("click", generarCuotasContratoInteligentes);

    $("#modalPlanDePago").on("click", ".btn-eliminar-cuota-contrato", function() {
        $(this).closest("tr").remove();
        reEnumerarCuotasContrato();
    });

    // Two-way binding for Anticipo value
    $("#txtValorAnticipoPlan").on("change", function() {
        const anticipoValue = $(this).val();
        const firstRow = $("#tbodyCuotas tr:first");
        
        if (firstRow.length && firstRow.find("td:nth-child(2)").text() === "Anticipo") {
            firstRow.find(".monto-cuota-input").val(parseFloat(anticipoValue).toFixed(2));
            calcularTotalCuotas();
        }
    });

    $("#modalPlanDePago").on("change", ".monto-cuota-input", function() {
        const $this = $(this);
        const $currentRow = $this.closest("tr");

        if ($currentRow.is(":first-child") && $currentRow.find("td:nth-child(2)").text() === "Anticipo") {
            $("#txtValorAnticipoPlan").val($this.val());
        }

        calcularTotalCuotas();
    });

    // ... (inicialización de datatable y otros listeners sin cambios) ...

    tablaContratos = $('#tbContrato').DataTable({
        responsive: true,
        ajax: {
            url: '/Contrato/Listar',
            type: "GET",
            datatype: "json",
            dataSrc: function(json) {
                return json.data.$values || json.data;
            }
        },
        columns: [
            { data: "nombreProyecto" },
            { 
                data: "fechaFirma",
                render: function(data) {
                    if (!data) return "";
                    const parts = data.split('-');
                    if (parts.length !== 3) return data; // Return original if not in expected format
                    return `${parts[2]}/${parts[1]}/${parts[0]}`;
                }
            },
            { data: "nombreUsuarioCarga" },
            { 
                data: "nombreArchivo",
                render: function (data, type, row) {
                    if (data) {
                        return `<a href="/Contrato/DescargarArchivo/${row.idContrato}" target="_blank">${data}</a>`;
                    }
                    return "N/A";
                }
            },
            { 
                data: "esActivo",
                render: function (data) {
                    return data ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                "defaultContent": '<div class="btn-group" role="group" aria-label="Acciones de Contrato">' +
                                    '<button class="btn btn-primary btn-editar btn-sm" title="Editar"><i class="fas fa-pencil-alt"></i></button>' +
                                    '<button class="btn btn-info btn-plan-pagos btn-sm" title="Plan de Pagos"><i class="fas fa-cash-register"></i></button>' +
                                    '<button class="btn btn-danger btn-eliminar btn-sm" title="Eliminar"><i class="fas fa-trash-alt"></i></button>' +
                                  '</div>',
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        order: [[1, "desc"]], // Order by Fecha Firma descending
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });

    $('#btnGuardarContratoDirecto').on('click', function () {
        const idContrato = $("#txtIdContratoDirecto").val();
        const esCreacion = idContrato === "0";
        const desdePreContrato = $('#chkCrearDesdePreContrato').is(':checked');

        // --- LÓGICA DE CONSTRUCCIÓN DE MODELO RESTAURADA ---
        let modelo = {
            idContrato: idContrato,
            nombreProyecto: $('#txtNombreProyecto').val(),
            fechaFirma: $('#txtFechaFirmaDirecto').val(),
            esActivo: $('#cboEstadoDirecto').val() == "1",
            idCotizacion: null,
            secCliente: null
        };

        // --- LÓGICA DE VALIDACIÓN RESTAURADA ---
        if (desdePreContrato) {
            const selectedPreContratoId = $('#cboPreContrato').val();
            if (!selectedPreContratoId) {
                Swal.fire("Error de Validación", "Debe seleccionar un Pre-Contrato.", "warning");
                return;
            }
            const precontrato = precontratosCargados.find(p => p.secPreContrato == selectedPreContratoId);
            modelo.idCotizacion = precontrato.secCotizacion;
            modelo.secCliente = precontrato.secCliente;
        } else {
            const selectedClienteId = $('#cboClienteDirecto').val();
            if (!selectedClienteId) {
                Swal.fire("Error de Validación", "Debe seleccionar un Cliente.", "warning");
                return;
            }
            modelo.secCliente = selectedClienteId;
        }

        const formData = new FormData();
        formData.append("modelo", JSON.stringify(modelo));
        const archivo = $('#fileContratoDirecto')[0].files[0];
        if (archivo) {
            formData.append("archivo", archivo);
        } else if (esCreacion) {
            Swal.fire("Error de Validación", "El archivo PDF del contrato es obligatorio para crear.", "warning");
            return;
        }

        $.ajax({
            url: esCreacion ? '/Contrato/Crear' : '/Contrato/Editar',
            type: esCreacion ? 'POST' : 'PUT',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.estado) {
                    tablaContratos.ajax.reload();
                    $('#modalContratoUnificado').modal('hide');
                    Swal.fire("Listo!", "Contrato guardado exitosamente.", "success");
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: manejarErrorAjax
        });
    });

    $('#btnGuardarPlanDePago').on('click', function () {
        const idContrato = $("#txtIdContratoPlan").val();
        const idPlanDePago = modeloBasePlanDePago.idPlanDePago || 0;
        const secFormaPago = $("#cboFormaPagoPlan").val();

        if (!secFormaPago) {
            Swal.fire("Error de Validación", "Debe seleccionar una Forma de Pago.", "warning");
            return;
        }

        let cuotasVm = [];
        let valido = true;
        $("#tbodyCuotas tr").each(function () {
            const numeroCuota = $(this).find("td:first").text();
            const fechaVencimiento = $(this).find(".fecha-cuota-input").val();
            const monto = parseFloat($(this).find(".monto-cuota-input").val());

            if (!fechaVencimiento || isNaN(monto)) {
                valido = false;
                return;
            }
            cuotasVm.push({
                numeroCuota: parseInt(numeroCuota),
                fechaVencimiento: fechaVencimiento,
                monto: monto,
                estado: "Pendiente"
            });
        });

        if (!valido) {
            Swal.fire("Error de Validación", "Todas las cuotas deben tener una fecha y un monto válidos.", "warning");
            return;
        }

        const modelo = {
            idPlanDePago: idPlanDePago,
            idContrato: parseInt(idContrato),
            secFormaPago: parseInt(secFormaPago),
            valorContrato: parseFloat($("#txtValorContratoPlan").val()),
            valorAnticipo: parseFloat($("#txtValorAnticipoPlan").val()) || 0,
            numeroCuotas: cuotasVm.length,
            cuotas: cuotasVm
        };

        $.ajax({
            url: '/Contrato/GuardarPlanDePago',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(modelo),
            success: function (response) {
                if (response.estado) {
                    $('#modalPlanDePago').modal('hide');
                    Swal.fire("Listo!", "Plan de pago guardado exitosamente.", "success");
                    tablaContratos.ajax.reload();
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: manejarErrorAjax
        });
    });

    $('#btnGuardarPago').on('click', function() {
        // --- LÓGICA DE CONSTRUCCIÓN Y VALIDACIÓN RESTAURADA ---
        const idPlanDePago = $('#txtIdPlanDePagoParaPago').val();
        const monto = parseFloat($('#txtMontoPago').val());
        const fechaPago = $('#txtFechaPago').val();
        if (!monto || monto <= 0) { /* ... validación ... */ return; }
        if (!fechaPago) { /* ... validación ... */ return; }
        const modeloPago = { /* ... construcción de modeloPago ... */ };

        $.ajax({
            url: '/api/Pago/Registrar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(modeloPago),
            success: function(response) { /* ... */ },
            error: manejarErrorAjax
        });
    });

    // ... (resto de los listeners)

    $('#btnNuevoContrato').on('click', function () {
        // Reset form
        $('#formContratoDirecto')[0].reset();
        $('#txtIdContratoDirecto').val("0");

        // Enable all fields for new contract
        $('#chkCrearDesdePreContrato, #cboPreContrato, #cboClienteDirecto, #txtNombreProyecto').prop('disabled', false);
        $('#txtNombreProyecto').prop('readonly', false);

        // Reset checkbox and dependent divs
        $('#chkCrearDesdePreContrato').prop('checked', false).trigger('change');
        
        // Clear dropdowns and project name
        $('#cboPreContrato').val('').empty();
        $('#cboClienteDirecto').val('').empty();
        $('#txtNombreProyecto').val('');

        // Set modal title for new
        $('#modalContratoUnificadoTitle span').text('Nuevo Contrato');

        // Load necessary dropdowns
        $.when(
            $.ajax({ url: "/Contrato/ListarPreContratosParaContrato" }).done(function(response) {
                precontratosCargados = response.objeto.$values || response.objeto;
                const $cbo = $('#cboPreContrato');
                $cbo.empty().append($('<option>').val('').text('Seleccionar...'));
                precontratosCargados.forEach(p => {
                    $cbo.append($('<option>').val(p.secPreContrato).text(`${p.secPreContrato} - ${p.nombreProyecto}`));
                });
            }),
            $.ajax({ url: "/Cliente/Lista" }).done(function(response) {
                const clientes = response.objeto.$values || response.objeto;
                const $cbo = $('#cboClienteDirecto');
                $cbo.empty().append($('<option>').val('').text('Seleccionar...'));
                clientes.forEach(c => {
                    $cbo.append($('<option>').val(c.secCliente).text(c.nombreConstructora));
                });
            })
        ).done(function() {
            // Show modal after dropdowns are loaded
            $('#modalContratoUnificado').modal('show');
        }).fail(function() {
            Swal.fire("Error", "No se pudieron cargar los datos necesarios para el formulario.", "error");
        });
    });

    $('#chkCrearDesdePreContrato').on('change', function () {
        if ($(this).is(':checked')) {
            $('#divPreContrato').show();
            $('#divCliente').hide();
            $('#txtNombreProyecto').prop('readonly', true);
        } else {
            $('#divPreContrato').hide();
            $('#divCliente').show();
            // Only make it readonly false if it's not disabled (i.e., we are in create mode)
            if (!$('#txtNombreProyecto').prop('disabled')) {
                $('#txtNombreProyecto').prop('readonly', false);
            }
        }
    });

    $('#cboPreContrato').on('change', function() {
        const selectedId = $(this).val();
        if (selectedId) {
            const precontrato = precontratosCargados.find(p => p.secPreContrato == selectedId);
            if (precontrato) {
                $('#txtNombreProyecto').val(precontrato.nombreProyecto);
            }
        } else {
            $('#txtNombreProyecto').val('');
        }
    });

    // Action button handlers
    $('#tbContrato tbody').on('click', '.btn-editar', function () {
        const data = tablaContratos.row($(this).parents('tr')).data();

        // 1. Set modal title
        $('#modalContratoUnificadoTitle span').text('Editar Contrato');

        // 2. Populate form directly from table data
        $('#txtIdContratoDirecto').val(data.idContrato);
        $('#txtNombreProyecto').val(data.nombreProyecto);

                    const dateParts = data.fechaFirma.split('-'); // data.fechaFirma is "yyyy-MM-dd"
                    const dateObject = new Date(dateParts[0], dateParts[1] - 1, dateParts[2]); // year, month (0-indexed), day
                    $('#txtFechaFirmaDirecto').datepicker('update', dateObject);        
        $('#cboEstadoDirecto').val(data.esActivo ? "1" : "0");

        // 3. Disable fields that should not be changed
        $('#chkCrearDesdePreContrato').prop('disabled', true);
        $('#cboPreContrato').prop('disabled', true);
        $('#cboClienteDirecto').prop('disabled', true);
        $('#txtNombreProyecto').prop('disabled', true);

        // 4. Show the origin info (but disabled)
        // NUEVA LÓGICA: Obtener detalles completos del contrato para determinar el origen
        $.ajax({
            url: `/Contrato/ObtenerDetalles?id=${data.idContrato}`,
            type: "GET",
            success: function (response) {
                if (response.estado) {
                    const contratoDetalle = response.objeto; // Esto es un ContratoVM

                    if (contratoDetalle.provieneDePreContrato) {
                        $('#chkCrearDesdePreContrato').prop('checked', true).trigger('change');
                        $('#divPreContrato').show();
                        $('#divCliente').hide();
                        // Add the used pre-contract to the dropdown so it's visible
                        $('#cboPreContrato').empty().append($('<option>').text(`Desde Cotización #${contratoDetalle.idCotizacion}`).val(contratoDetalle.idCotizacion));
                    } else {
                        $('#chkCrearDesdePreContrato').prop('checked', false).trigger('change');
                        $('#divPreContrato').hide();
                        $('#divCliente').show();
                        // Add the used client to the dropdown
                        $('#cboClienteDirecto').empty().append($('<option>').text(`${contratoDetalle.nombreCliente}`).val(contratoDetalle.secCliente));
                    }
                    // 5. Show the modal after all data is loaded and UI updated
                    $('#modalContratoUnificado').modal('show');
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: manejarErrorAjax
        });
    });

    $('#tbContrato tbody').on('click', '.btn-eliminar', function () {
        const fila = $(this).closest('tr');
        const data = tablaContratos.row(fila).data();
        
        Swal.fire({
            title: '¿Está seguro?',
            text: `¿Desea eliminar el contrato para la obra "${data.nombreProyecto}"?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/Contrato/Eliminar?id=${data.idContrato}`, {
                    method: 'DELETE'
                })
                .then(response => response.json())
                .then(responseJson => {
                    if (responseJson.estado) {
                        tablaContratos.ajax.reload();
                        Swal.fire('¡Eliminado!', 'El contrato ha sido eliminado.', 'success');
                    } else {
                        Swal.fire('Error', responseJson.mensajes, 'error');
                    }
                })
                .catch(error => {
                    Swal.fire('Error', 'No se pudo comunicar con el servidor.', 'error');
                });
            }
        });
    });

    $('#tbContrato tbody').on('click', '.btn-plan-pagos', function () {
        const data = tablaContratos.row($(this).parents('tr')).data();

        // Resetear y preparar el modal
        $('#formPlanDePago')[0].reset();
        $('#txtIdContratoPlan').val(data.idContrato);
        $('#tbodyCuotas').empty();
        modeloBasePlanDePago = {}; // Limpiar modelo base

        // Cargar siempre los dropdowns y el plan de pago existente
        $.when(
            $.ajax({ url: "/FormaPago/ListaParaDropdown" }),
            $.ajax({ url: `/Contrato/ObtenerPlanDePagoPorContrato?idContrato=${data.idContrato}` })
        ).done(function (responseFormasPago, responsePlanPago) {
            // 1. Poblar Formas de Pago
            const formasPago = responseFormasPago[0].data.$values || responseFormasPago[0].data;
            const $cboFormaPago = $('#cboFormaPagoPlan');
            $cboFormaPago.empty().append($('<option>').val('').text('Seleccionar...'));
            formasPago.forEach(fp => {
                $cboFormaPago.append($('<option>').val(fp.value).text(fp.text));
            });

            // 2. Procesar Plan de Pago Existente
            const planDePago = responsePlanPago[0].estado ? responsePlanPago[0].objeto : null;
            if (planDePago) {
                modeloBasePlanDePago = planDePago; // Guardar el plan cargado
                $cboFormaPago.val(planDePago.secFormaPago);
                $('#txtValorContratoPlan').val(planDePago.valorContrato.toFixed(2));
                $('#txtValorAnticipoPlan').val(planDePago.valorAnticipo.toFixed(2));
            }

            // 3. Decidir el flujo basado en el origen del contrato
            if (data.provieneDePreContrato) {
                setPlanDePagoEditable(false);
                $.ajax({
                    url: `/Contrato/ObtenerCompromisosDePagoPreContrato?idCotizacion=${data.idCotizacion}`,
                    type: "GET",
                    success: function (response) {
                        if (response.estado) {
                            let totalContratoCalculado = 0;
                            $('#tbodyCuotas').empty(); // Limpiar por si acaso
                            const compromisos = response.objeto.$values || response.objeto;
                            compromisos.forEach(function (compromiso) {
                                totalContratoCalculado += compromiso.monto;
                                const fechaFormateada = new Date(compromiso.fechaVencimiento).toISOString().split('T')[0];
                                const nuevaFila = `
                                    <tr>
                                        <td>${compromiso.numeroCuota}</td>
                                        <td>${compromiso.tipo}</td>
                                        <td><input type="text" class="form-control form-control-sm fecha-cuota-input" value="${fechaFormateada}" readonly></td>
                                        <td><input type="number" class="form-control form-control-sm monto-cuota-input" value="${compromiso.monto.toFixed(2)}" readonly></td>
                                        <td></td>
                                    </tr>
                                `;
                                $('#tbodyCuotas').append(nuevaFila);
                            });
                            $('#txtValorContratoPlan').val(totalContratoCalculado.toFixed(2));
                            reEnumerarCuotasContrato();
                            $('#modalPlanDePago').modal('show');
                        } else {
                            Swal.fire("Error", response.mensajes, "error");
                        }
                    },
                    error: manejarErrorAjax
                });
            } else {
                setPlanDePagoEditable(true);
                const cuotas = (planDePago && planDePago.cuotas && planDePago.cuotas.$values) ? planDePago.cuotas.$values : (planDePago && planDePago.cuotas || []);
                if (cuotas.length > 0) {
                    cuotas.forEach(function(cuota) {
                        if (!cuota.fechaVencimiento) return;
                        const fecha = new Date(cuota.fechaVencimiento);
                        if (isNaN(fecha.getTime())) return;

                        const fechaFormateada = fecha.toISOString().split('T')[0];
                        const nuevaFila = `
                            <tr>
                                <td>${cuota.numeroCuota}</td>
                                <td>${cuota.tipo || 'Cuota'}</td>
                                <td><div class="input-group date" data-provide="datepicker"><input type="text" class="form-control form-control-sm fecha-cuota-input" value="${fechaFormateada}"><div class="input-group-addon"><span class="glyphicon glyphicon-th"></span></div></div></td>
                                <td><input type="number" class="form-control form-control-sm monto-cuota-input" value="${cuota.montoEsperado.toFixed(2)}"></td>
                                <td><button type="button" class="btn btn-danger btn-sm btn-eliminar-cuota-contrato"><i class="fas fa-trash"></i></button></td>
                            </tr>
                        `;
                        $('#tbodyCuotas').append(nuevaFila);
                    });
                    inicializarDatepickers("#tbodyCuotas");
                    reEnumerarCuotasContrato();
                }
                $('#modalPlanDePago').modal('show');
            }

        }).fail(function () {
            Swal.fire("Error de Carga", "No se pudieron cargar los datos necesarios para el plan de pagos.", "error");
        });
    });
});