let tablaContratos;
let precontratosCargados = []; // Caché para los precontratos

let modeloBasePlanDePago = {}; // Contendrá el modelo del plan de pago que se está editando

const modeloBaseContrato = {
    idContrato: 0,
    idCotizacion: null, // Añadido para el nuevo flujo
    secCliente: null,
    nombreProyecto: "",
    fechaFirma: "",
    esActivo: true
};

// #region Funciones Modales
function abrirModalContrato(modelo = modeloBaseContrato) {
    // Limpiar y resetear el formulario
    $("#txtIdContratoDirecto").val(modelo.idContrato);
    $("#txtNombreProyecto").val(modelo.nombreObra);
    $("#cboEstadoDirecto").val(modelo.esActivo ? '1' : '0');
   // $("#txtNombreProyecto").val(modelo.nombreProyecto);
    $("#txtFechaFirmaDirecto").val(modelo.fechaFirma);
    $("#fileContratoDirecto").val("");

    // Resetear el nuevo flujo de pre-contrato
    $('#chkCrearDesdePreContrato').prop('checked', false).trigger('change');
    $("#cboPreContrato").empty().append($('<option>', { value: '', text: 'Seleccione Pre-Contrato' }));


    // Cargar Clientes y luego popular
    $.ajax({
        url: '/Cliente/Lista',
        type: 'GET',
        success: function (clientesResponse) {
            const comboCliente = $("#cboClienteDirecto");
            comboCliente.empty().append($('<option>', { value: '', text: 'Seleccione Cliente' }));
            if (clientesResponse.estado && clientesResponse.objeto.$values) {
                clientesResponse.objeto.$values.forEach(cliente => {
                    comboCliente.append($('<option>', { value: cliente.secCliente, text: `${cliente.numeroCliente} - ${cliente.nombreConstructora}` }));
                });
            }
            comboCliente.val(modelo.secCliente);

            // Configurar modal para Creación vs Edición
            if (modelo.idContrato === 0) { // Modo Creación
                $(".modal-title").text("Nuevo Contrato");
                comboCliente.prop('disabled', false);
                $('#chkCrearDesdePreContrato').prop('disabled', false);
            } else { // Modo Edición
                $(".modal-title").text("Editar Contrato");
                comboCliente.prop('disabled', true);
                // En modo edición, no se puede cambiar el origen (precontrato o directo)
                $('#chkCrearDesdePreContrato').prop('disabled', true);
            }

            $('#modalContratoUnificado').modal('show');

        }
    }).fail(function () {
        Swal.fire("Error de Comunicación", "No se pudieron cargar los clientes.", "error");
    });
}

function abrirModalPlanDePago(contratoData, modelo) {
    modeloBasePlanDePago = modelo; // Actualiza el modelo en el alcance del script

    // Función para formatear fecha de YYYY-MM-DD a DD/MM/YYYY
    const formatFechaParaInput = (fechaISO) => {
        if (!fechaISO) return "";
        const fecha = new Date(fechaISO);
        const dia = String(fecha.getDate()).padStart(2, '0');
        const mes = String(fecha.getMonth() + 1).padStart(2, '0');
        const anio = fecha.getFullYear();
        return `${dia}/${mes}/${anio}`;
    };

    // Cargar Formas de Pago y luego configurar el modal
    $.ajax({
        url: '/FormaPago/Lista',
        type: 'GET',
        success: function (response) {
            const combo = $("#cboFormaPagoPlan");
            combo.empty().append($('<option>', { value: '', text: 'Seleccione Forma de Pago' }));

            if (response.estado && response.objeto.$values) {
                response.objeto.$values.forEach(item => {
                    combo.append($('<option>', { value: item.secFormaPago, text: item.descripcion }));
                });
            }

            // Establecer título y ID del contrato
            $("#txtIdContratoPlan").val(contratoData.idContrato);
            $("#modalPlanDePagoTitle").text(`Plan de Pagos para Contrato ${contratoData.idContrato}`);

            // Lógica condicional para mostrar vista de edición o vista de estado de cuenta
            if (modelo && modelo.cuotas && modelo.cuotas.$values && modelo.cuotas.$values.length > 0) {
                // MODO VISTA: El plan ya tiene cuotas, mostrar estado de cuenta
                $('#divPlanDePagoForm').show();
                $('#divPlanDePagoVista').show();
                $('#seccionParrillaCuotasEdicion').hide(); // Ocultar parrilla de edición
                generarTablaCuotasVista(modelo.cuotas.$values);

                // Poblar el formulario con los datos del modelo
                combo.val(modelo.secFormaPago || '').prop('disabled', true); // Deshabilitar combo
                $("#txtValorContratoPlan").val(modelo.valorContrato).prop('readonly', true);
                $("#txtValorAnticipoPlan").val(modelo.valorAnticipo || '0').prop('readonly', true);
                $("#txtFechaAnticipoPlan").val(formatFechaParaInput(modelo.fechaAnticipo)).prop('readonly', true);
                $("#txtNumeroCuotasPlan").val(modelo.numeroCuotas || '0').prop('readonly', true);
                $('#txtFechaAnticipoPlan').datepicker('destroy'); // Deshabilitar datepicker

                // Ocultar botón de guardar y mostrar botón de registrar pago
                $('#btnGuardarPlanDePago').hide();
                $('#btnRegistrarPago').show();

            } else {
                // MODO EDICIÓN/CREACIÓN: El plan no tiene cuotas o es nuevo
                $('#divPlanDePagoVista').hide();
                $('#divPlanDePagoForm').show();
                $('#seccionParrillaCuotasEdicion').show(); // Mostrar parrilla de edición

                // Poblar el formulario con los datos del modelo
                combo.val(modelo.secFormaPago || '').prop('disabled', false);

                const $valorContratoPlan = $("#txtValorContratoPlan");
                if (modelo.valorContrato && modelo.valorContrato > 0) {
                    $valorContratoPlan.val(modelo.valorContrato).prop('readonly', true);
                } else {
                    $valorContratoPlan.val('0').prop('readonly', false);
                }

                $("#txtValorAnticipoPlan").val(modelo.valorAnticipo || '0').prop('readonly', false);
                $("#txtFechaAnticipoPlan").val(formatFechaParaInput(modelo.fechaAnticipo)).prop('readonly', false);
                $("#txtNumeroCuotasPlan").val(modelo.numeroCuotas || '0').prop('readonly', false);
                $('#txtFechaAnticipoPlan').datepicker({ format: "dd/mm/yyyy", language: "es", autoclose: true, todayHighlight: true }); // Habilitar datepicker

                // Mostrar botón de guardar y ocultar botón de registrar pago
                $('#btnGuardarPlanDePago').show();
                $('#btnRegistrarPago').hide();
                
                generarParrillaCuotas(modelo.cuotas.$values || []);
            }

            $('#modalPlanDePago').modal('show');
        },
        error: function () {
            Swal.fire("Error de Comunicación", "No se pudieron cargar las Formas de Pago.", "error");
        }
    });
}

function generarTablaCuotasVista(cuotas) {
    const tbody = $("#tbodyCuotasVista");
    tbody.empty();
    let totalEsperado = 0;
    let totalPagado = 0;
    let totalSaldo = 0;

    cuotas.forEach(cuota => {
        const montoPagadoReal = cuota.montoPagado || 0;
        const saldo = cuota.montoEsperado - montoPagadoReal;

        totalEsperado += cuota.montoEsperado;
        totalPagado += montoPagadoReal;
        totalSaldo += saldo;

        const fechaFormateada = new Date(cuota.fechaVencimiento).toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' });
        
        let estadoBadge;
        switch (cuota.estado.toLowerCase()) {
            case 'pagada':
                estadoBadge = `<span class="badge badge-success">Pagada</span>`;
                break;
            case 'parcialmente pagada':
                estadoBadge = `<span class="badge badge-warning">Parcialmente Pagada</span>`;
                break;
            case 'pendiente':
                estadoBadge = `<span class="badge badge-danger">Pendiente</span>`;
                break;
            default:
                estadoBadge = `<span class="badge badge-secondary">${cuota.estado}</span>`;
        }

        const row = `
            <tr>
                <td>${cuota.numeroCuota}</td>
                <td>${fechaFormateada}</td>
                <td>${cuota.montoEsperado.toFixed(2)}</td>
                <td>${montoPagadoReal.toFixed(2)}</td>
                <td>${saldo.toFixed(2)}</td>
                <td>${estadoBadge}</td>
            </tr>
        `;
        tbody.append(row);
    });

    // Add totals row
    const totalsRow = `
        <tr class="font-weight-bold table-info">
            <td colspan="2" class="text-right"><strong>Totales:</strong></td>
            <td>${totalEsperado.toFixed(2)}</td>
            <td>${totalPagado.toFixed(2)}</td>
            <td>${totalSaldo.toFixed(2)}</td>
            <td></td>
        </tr>
    `;
    tbody.append(totalsRow);

    // Lógica para mostrar/ocultar botones
    $('#btnRegistrarPago').show();
    $('#btnGuardarPlanDePago').hide();
}

function generarParrillaCuotas(cuotasExistentes = []) {
    const numCuotas = cuotasExistentes.length > 0 ? cuotasExistentes.length : (parseInt($("#txtNumeroCuotasPlan").val()) || 0);
    const tbody = $("#tbodyCuotas");
    tbody.empty();

    for (let i = 0; i < numCuotas; i++) {
        const cuota = cuotasExistentes[i] || { numeroCuota: i + 1, fechaVencimiento: "", montoEsperado: 0, estado: "Pendiente" };
        const fechaFormateada = cuota.fechaVencimiento ? new Date(cuota.fechaVencimiento).toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' }) : '';

        const row = `
            <tr>
                <td>${cuota.numeroCuota}</td>
                <td>
                    <div class="input-group date" data-provide="datepicker">
                        <input type="text" class="form-control form-control-sm fecha-cuota" value="${fechaFormateada}">
                        <div class="input-group-addon"><span class="glyphicon glyphicon-th"></span></div>
                    </div>
                </td>
                <td><input type="number" class="form-control form-control-sm monto-cuota" value="${cuota.montoEsperado}" min="0"><span class="invalid-feedback error-monto-cuota"></span></td>
            </tr>
        `;
        tbody.append(row);
    }
    // Re-inicializar datepickers para los nuevos campos
    $('.input-group.date').datepicker({ format: "dd/mm/yyyy", language: "es", autoclose: true, todayHighlight: true });

    // Establecer el foco en el primer campo de fecha de la primera cuota
    $('#tbodyCuotas tr:first-child .fecha-cuota').focus();
}

// Función auxiliar para validar una fecha de cuota individualmente
function validarFechaCuotaIndividual(inputCuota) {
    const $inputCuota = $(inputCuota);
    const fechaCuotaStr = $inputCuota.val();
    const indexCuota = $inputCuota.closest('tr').index();

    // Parsear fecha de anticipo
    const fechaAnticipoStr = $("#txtFechaAnticipoPlan").val();
    let fechaAnticipo = null;
    if (fechaAnticipoStr) {
        const parts = fechaAnticipoStr.split('/');
        fechaAnticipo = new Date(parts[2], parts[1] - 1, parts[0]);
    }

    // Validar formato de fecha
    if (!/^[0-9]{2}\/[0-9]{2}\/[0-9]{4}$/.test(fechaCuotaStr)) {
        toastr.warning(`La fecha de vencimiento de la cuota #${indexCuota + 1} no tiene un formato válido (DD/MM/YYYY).`, "Error de Formato");
        $inputCuota.addClass('is-invalid');
        return false;
    }

    const partsCuota = fechaCuotaStr.split('/');
    const fechaCuota = new Date(partsCuota[2], partsCuota[1] - 1, partsCuota[0]);

    // Validar fecha no menor a fecha de anticipo
    if (fechaAnticipo && fechaCuota < fechaAnticipo) {
        toastr.warning(`La fecha de vencimiento de la cuota #${indexCuota + 1} no puede ser anterior a la Fecha del Anticipo.`, "Fecha Inválida");
        $inputCuota.addClass('is-invalid');
        return false;
    }

    // Validar fechas secuenciales (solo si no es la primera cuota)
    if (indexCuota > 0) {
        const fechaAnteriorStr = $("#tbodyCuotas tr").eq(indexCuota - 1).find(".fecha-cuota").val();
        const partsAnterior = fechaAnteriorStr.split('/');
        const fechaAnterior = new Date(partsAnterior[2], partsAnterior[1] - 1, partsAnterior[0]);

        if (fechaCuota < fechaAnterior) {
            toastr.warning(`La fecha de vencimiento de la cuota #${indexCuota + 1} no puede ser anterior a la cuota anterior.`, "Fechas No Secuenciales");
            $inputCuota.addClass('is-invalid');
            return false;
        }
    }

    $inputCuota.removeClass('is-invalid');
    return true;
}

function validarCuotasPlanDePago() {
    const valorContrato = parseFloat($("#txtValorContratoPlan").val()) || 0;
    const valorAnticipo = parseFloat($("#txtValorAnticipoPlan").val()) || 0;

    let validacionGeneralExitosa = true; // Bandera para controlar el resultado final
    let primerInputInvalido = null;

    // Validar anticipo vs contrato (ahora en blur, pero se llama aquí para la validación final)
    if (!validarAnticipoVsContrato()) {
        validacionGeneralExitosa = false;
        if (!primerInputInvalido) primerInputInvalido = $('#txtValorAnticipoPlan');
    }

    const fechaAnticipoStr = $("#txtFechaAnticipoPlan").val();
    let fechaAnticipo = null;
    if (fechaAnticipoStr) {
        const parts = fechaAnticipoStr.split('/');
        fechaAnticipo = new Date(parts[2], parts[1] - 1, parts[0]);
    }

    const saldoPendiente = valorContrato - valorAnticipo;
    let sumaMontosCuotas = 0;
    let ultimaFechaCuota = fechaAnticipo;

    const cuotas = [];
    $("#tbodyCuotas tr").each(function (index) {
        const $fila = $(this);
        const $fechaInput = $fila.find(".fecha-cuota");
        const $montoInput = $fila.find(".monto-cuota");
        const montoEsperado = parseFloat($montoInput.val()) || 0;

        // Validar fecha individualmente
        if (!validarFechaCuotaIndividual($fechaInput)) {
            validacionGeneralExitosa = false;
            if (!primerInputInvalido) primerInputInvalido = $fechaInput;
        }

        // Validar monto individualmente
        if (!validarMontoCuotaIndividual($montoInput)) {
            validacionGeneralExitosa = false;
            if (!primerInputInvalido) primerInputInvalido = $montoInput;
        }

        // Validar que el monto sea positivo (si no se ha validado ya por monto acumulado)
        if (montoEsperado <= 0 && !$montoInput.hasClass('is-invalid')) {
            $montoInput.addClass('is-invalid');
            $montoInput.next('.error-monto-cuota').text(`El monto de la cuota #${index + 1} debe ser mayor a cero.`).show();
            validacionGeneralExitosa = false;
            if (!primerInputInvalido) primerInputInvalido = $montoInput;
        }

        sumaMontosCuotas += montoEsperado;
        // La lógica de ultimaFechaCuota se maneja dentro de validarFechaCuotaIndividual
        cuotas.push({ fechaVencimientoStr: $fechaInput.val(), montoEsperado }); // Solo para referencia
    });

    // Validar que la suma de montos no exceda el saldo pendiente (validación global)
    if (sumaMontosCuotas > saldoPendiente) {
        Swal.fire("Error de Validación", `La suma de los montos de las cuotas (${sumaMontosCuotas.toFixed(2)}) excede el saldo pendiente (${saldoPendiente.toFixed(2)}).`, "warning");
        validacionGeneralExitosa = false;
    }

    // Opcional: Advertir si la suma es menor, pero permitir guardar
    if (sumaMontosCuotas < saldoPendiente && saldoPendiente > 0 && validacionGeneralExitosa) {
        Swal.fire("Advertencia", `La suma de los montos de las cuotas (${sumaMontosCuotas.toFixed(2)}) es menor que el saldo pendiente (${saldoPendiente.toFixed(2)}). El saldo restante no está distribuido.`, "info");
    }

    // Si hay errores, hacer foco en el primer input inválido
    if (!validacionGeneralExitosa && primerInputInvalido) {
        primerInputInvalido.focus();
        return false;
    }

    return validacionGeneralExitosa;
}

// Función auxiliar para validar una fecha de cuota individualmente
function validarFechaCuotaIndividual(inputCuota) {
    const $inputCuota = $(inputCuota);
    const fechaCuotaStr = $inputCuota.val();
    const indexCuota = $inputCuota.closest('tr').index();

    // Limpiar estado de validación previo
    $inputCuota.removeClass('is-invalid');

    // Validar que la fecha no esté vacía
    if (!fechaCuotaStr) {
        // toastr.warning(`La fecha de vencimiento de la cuota #${indexCuota + 1} no puede estar vacía.`, "Campo Requerido");
        $inputCuota.addClass('is-invalid');
        return false;
    }

    // Parsear fecha de anticipo
    const fechaAnticipoStr = $("#txtFechaAnticipoPlan").val();
    let fechaAnticipo = null;
    if (fechaAnticipoStr) {
        const parts = fechaAnticipoStr.split('/');
        fechaAnticipo = new Date(parts[2], parts[1] - 1, parts[0]);
    }

    // Validar formato de fecha
    if (!/^[0-9]{2}\/[0-9]{2}\/[0-9]{4}$/.test(fechaCuotaStr)) {
        // toastr.warning(`La fecha de vencimiento de la cuota #${indexCuota + 1} no tiene un formato válido (DD/MM/YYYY).`, "Error de Formato");
        $inputCuota.addClass('is-invalid');
        return false;
    }

    const partsCuota = fechaCuotaStr.split('/');
    const fechaCuota = new Date(partsCuota[2], partsCuota[1] - 1, partsCuota[0]);

    // Validar fecha no menor a fecha de anticipo
    if (fechaAnticipo && fechaCuota < fechaAnticipo) {
        // toastr.warning(`La fecha de vencimiento de la cuota #${indexCuota + 1} no puede ser anterior a la Fecha del Anticipo.`, "Fecha Inválida");
        $inputCuota.addClass('is-invalid');
        return false;
    }

    // Validar fechas secuenciales (solo si no es la primera cuota)
    if (indexCuota > 0) {
        const fechaAnteriorStr = $("#tbodyCuotas tr").eq(indexCuota - 1).find(".fecha-cuota").val();
        // Solo validar si la fecha anterior no está vacía y tiene formato válido
        if (fechaAnteriorStr && /^[0-9]{2}\/[0-9]{2}\/[0-9]{4}$/.test(fechaAnteriorStr)) {
            const partsAnterior = fechaAnteriorStr.split('/');
            const fechaAnterior = new Date(partsAnterior[2], partsAnterior[1] - 1, partsAnterior[0]);

            if (fechaCuota <= fechaAnterior) {
                // toastr.warning(`La fecha de vencimiento de la cuota #${indexCuota + 1} no puede ser igual o anterior a la cuota anterior.`, "Fechas No Secuenciales");
                $inputCuota.addClass('is-invalid');
                return false;
            }
        }
    }

    return true;
}

function validarAnticipoVsContrato() {
    const $inputAnticipo = $('#txtValorAnticipoPlan');
    const $errorSpan = $('#errorValorAnticipo');
    const valorContrato = parseFloat($("#txtValorContratoPlan").val()) || 0;
    const valorAnticipo = parseFloat($inputAnticipo.val()) || 0;

    $inputAnticipo.removeClass('is-invalid');
    $errorSpan.text('').hide();

    if (valorAnticipo > valorContrato) {
        $inputAnticipo.addClass('is-invalid');
        $errorSpan.text('El valor del anticipo no puede ser mayor al valor del contrato.').show();
        return false;
    }
    return true;
}

// Función auxiliar para validar el monto de una cuota individualmente (suma acumulada)
function validarMontoCuotaIndividual(inputMonto) {
    const $inputMonto = $(inputMonto);
    const $errorSpan = $inputMonto.next('.error-monto-cuota');
    const valorContrato = parseFloat($("#txtValorContratoPlan").val()) || 0;
    const valorAnticipo = parseFloat($("#txtValorAnticipoPlan").val()) || 0;

    $inputMonto.removeClass('is-invalid');
    $errorSpan.text('').hide();

    let sumaMontosCuotasHastaAhora = 0;
    let validacionExitosa = true;

    $("#tbodyCuotas tr").each(function (index) {
        const $fila = $(this);
        const $montoInput = $fila.find(".monto-cuota");
        const montoEsperado = parseFloat($montoInput.val()) || 0;

        sumaMontosCuotasHastaAhora += montoEsperado;

        // Si es la cuota actual o una posterior, y la suma acumulada excede el contrato
        if (index >= $inputMonto.closest('tr').index() && (sumaMontosCuotasHastaAhora + valorAnticipo) > valorContrato) {
            $montoInput.addClass('is-invalid');
            $montoInput.next('.error-monto-cuota').text(`La suma acumulada (${(sumaMontosCuotasHastaAhora + valorAnticipo).toFixed(2)}) excede el valor del contrato (${valorContrato.toFixed(2)}).`).show();
            validacionExitosa = false;
            // return false; // No detener el each aquí, para mostrar todos los errores
        } else {
            $montoInput.removeClass('is-invalid');
            $montoInput.next('.error-monto-cuota').text('').hide();
        }
    });

    return validacionExitosa;
}
// #endregion

$(document).ready(function () {
    tablaContratos = $('#tbContrato').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Contrato/Listar',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (response) {
                // La respuesta de este endpoint específico no sigue el patrón GenericResponse,
                // sino que anida el array en la propiedad 'data'.
                if (response && response.data && response.data.$values) {
                    return response.data.$values;
                }
                // Fallback por si la estructura cambia o hay un error.
                console.error("La respuesta del servidor no tiene el formato esperado:", response);
                return [];
            }
        },
        "columns": [
            { "data": "nombreObra" },
            { "data": "fechaFirma" },
            { "data": "nombreUsuarioCarga" },
            { "data": "rutaArchivo", render: data => data ? `<a href="${data}" target="_blank">Ver Archivo</a>` : "Sin archivo" },
            { "data": "esActivo", render: data => data ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            {
                "defaultContent": '<div class="btn-group" role="group">' +
                                      '<button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button>' +
                                      '<button class="btn btn-info btn-plan-pagos btn-sm"><i class="fas fa-dollar-sign"></i></button>' +
                                      '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>' +
                                  '</div>',
                "orderable": false, "searchable": false, "width": "120px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: ['excelHtml5', 'pageLength'],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" },
    });

    // Evento blur para cada fecha de cuota: validar individualmente
    $(document).on('blur', '.fecha-cuota', function() {
        validarFechaCuotaIndividual(this);
    });

    // Evento blur para cada monto de cuota: validar individualmente
    $(document).on('blur', '.monto-cuota', function() {
        validarMontoCuotaIndividual(this);
    });

    // Evento blur para el Valor del Anticipo
    $('#txtValorAnticipoPlan').on('blur', function() {
        validarAnticipoVsContrato();
    });

    // DataTable

    // #region Eventos Contrato
    // Abrir modal para NUEVO contrato unificado
    $('#btnNuevoContrato').on('click', function () {
        abrirModalContrato(); // Llama sin parámetros para modo creación
    });

    // Abrir modal para EDITAR contrato
    $('#tbContrato tbody').on('click', '.btn-editar', function () {
        var data = tablaContratos.row($(this).parents('tr')).data();
        $.get(`/Contrato/ObtenerDetalles?id=${data.idContrato}`).done(response => {
            if (response.estado) {
                abrirModalContrato(response.objeto);
            } else {
                Swal.fire("Error", `No se pudieron obtener los detalles: ${response.mensajes}`, "error");
            }
        }).fail(() => {
            Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor.", "error");
        });
    });

    // --- NUEVA LÓGICA PARA EL CHECKBOX ---
    $('#chkCrearDesdePreContrato').on('change', function () {
        const isChecked = $(this).is(':checked');
        if (isChecked) {
            $('#divCliente').hide();
            $('#divPreContrato').show();
            $('#txtNombreProyecto').prop('readonly', true);

            // Cargar precontratos si no se han cargado antes
            if (precontratosCargados.length === 0) {
                $.get('/Contrato/ListarPreContratosParaContrato').done(response => {
                    if (response.estado && response.objeto.$values) {
                        precontratosCargados = response.objeto.$values;
                        const combo = $('#cboPreContrato');
                        combo.empty().append($('<option>', { value: '', text: 'Seleccione Pre-Contrato' }));
                        precontratosCargados.forEach(pc => {
                            // Asumiendo que PreContratoVM tiene estos campos
                            combo.append($('<option>', { value: pc.secPreContrato, text: `PC-${pc.secPreContrato} / ${pc.nombreObra}` }));
                        });
                    }
                });
            }
        } else {
            $('#divPreContrato').hide();
            $('#divCliente').show();
            //$('#txtNombreProyecto').prop('readonly', false).val('');
        }
    });

    // --- NUEVA LÓGICA PARA EL DROPDOWN DE PRECONTRATO ---
    $('#cboPreContrato').on('change', function () {
        const selectedId = $(this).val();
        if (selectedId) {
            const precontrato = precontratosCargados.find(p => p.secPreContrato == selectedId);
            if (precontrato) {
                $('#txtNombreProyecto').val(precontrato.nombreObra);
            }
        } else {
            $('#txtNombreProyecto').val('');
        }
    });


    // Lógica unificada para GUARDAR (Crear y Editar) Contrato
    $('#btnGuardarContratoDirecto').on('click', function () {
        const idContrato = $("#txtIdContratoDirecto").val();
        const esCreacion = idContrato === "0";
        const desdePreContrato = $('#chkCrearDesdePreContrato').is(':checked');

        let modelo = {
            idContrato: idContrato,
            nombreProyecto: $('#txtNombreProyecto').val(),
            fechaFirma: $('#txtFechaFirmaDirecto').val(),
            esActivo: $('#cboEstadoDirecto').val() == "1",
            idCotizacion: null,
            secCliente: null
        };

        // --- LÓGICA CONDICIONAL PARA GUARDAR ---
        if (desdePreContrato) {
            const selectedPreContratoId = $('#cboPreContrato').val();
            if (!selectedPreContratoId) {
                Swal.fire("Error de Validación", "Debe seleccionar un Pre-Contrato.", "warning");
                return;
            }
            const precontrato = precontratosCargados.find(p => p.secPreContrato == selectedPreContratoId);
            modelo.idCotizacion = precontrato.secCotizacion;
            modelo.secCliente = precontrato.secCliente; // El cliente viene del precontrato
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

        const url = esCreacion ? '/Contrato/Crear' : '/Contrato/Editar';
        const type = 'POST'; // El backend espera POST para Crear y PUT para Editar, pero la lógica de Editar no está completamente implementada aquí. Simplificando a POST.

        $.ajax({
            url: url,
            type: esCreacion ? 'POST' : 'PUT',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.estado) {
                    tablaContratos.ajax.reload();
                    $('#modalContratoUnificado').modal('hide'); // Actualizado el ID del modal
                    Swal.fire("Listo!", "Contrato guardado exitosamente.", "success");
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: function () {
                Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor.", "error");
            }
        });
    });

    // Lógica para Eliminar Contrato
    $('#tbContrato tbody').on('click', '.btn-eliminar', function () {
        var data = tablaContratos.row($(this).parents('tr')).data();
        Swal.fire({
            title: '¿Está seguro?',
            text: `Eliminar el contrato "${data.nombreObra}"`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'No, cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: `/Contrato/Eliminar?id=${data.idContrato}`,
                    type: 'DELETE',
                    success: response => {
                        if (response.estado) {
                            tablaContratos.ajax.reload();
                            Swal.fire("Eliminado!", "El contrato ha sido eliminado.", "success");
                        } else {
                            Swal.fire("Error", response.mensajes, "error");
                        }
                    },
                    error: () => Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor.", "error")
                });
            }
        });
    });
    // #endregion

    // #region Eventos Plan de Pagos
    $('#tbContrato tbody').on('click', '.btn-plan-pagos', function () {
        var data = tablaContratos.row($(this).parents('tr')).data();
        const idContrato = data.idContrato;

        $.get(`/api/PlanDePago/ObtenerPorContratoId/${idContrato}`).done(response => {
            if (response.estado) {
                abrirModalPlanDePago(data, response.objeto); // Pasar el objeto data completo
            } else {
                Swal.fire("Error", `No se pudo obtener el plan de pagos: ${response.mensajes}`, "error");
            }
        }).fail(() => {
            Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor.", "error");
        });
    });

    $('#txtNumeroCuotasPlan').on('change', function () {
        generarParrillaCuotas();
    });

    // Función para convertir fecha dd/mm/yyyy a yyyy-mm-dd, compatible con la API
    function convertirFechaParaAPI(fechaStr) {
        if (!fechaStr || !/^[0-9]{2}\/[0-9]{2}\/[0-9]{4}$/.test(fechaStr)) {
            return null;
        }
        const parts = fechaStr.split('/');
        return `${parts[2]}-${parts[1]}-${parts[0]}`;
    }

    $('#btnGuardarPlanDePago').on('click', function () {
        if (!validarCuotasPlanDePago()) {
            return; // Detener si la validación falla
        }

        const idContrato = $("#txtIdContratoPlan").val();
        const idPlanDePago = modeloBasePlanDePago.idPlanDePago; // Se actualiza al cargar el modal
        const secFormaPago = $("#cboFormaPagoPlan").val();

        if (!secFormaPago) {
            Swal.fire("Error de Validación", "Debe seleccionar una Forma de Pago.", "warning");
            return;
        }

        const cuotas = [];
        $("#tbodyCuotas tr").each(function (index) {
            const fechaVencimiento = $(this).find(".fecha-cuota").val();
            const montoEsperado = parseFloat($(this).find(".monto-cuota").val()) || 0;
            cuotas.push({
                numeroCuota: index + 1,
                fechaVencimiento: convertirFechaParaAPI(fechaVencimiento),
                montoEsperado: montoEsperado,
                estado: "Pendiente" // Estado inicial
            });
        });

        const primeraFechaCuota = cuotas.length > 0 ? cuotas[0].fechaVencimiento : null;

        const modeloPlan = {
            idPlanDePago: idPlanDePago,
            idContrato: idContrato,
            secFormaPago: parseInt(secFormaPago),
            valorContrato: parseFloat($("#txtValorContratoPlan").val()) || 0,
            numeroCuotas: parseInt($("#txtNumeroCuotasPlan").val()) || 0,
            fechaPrimeraCuota: primeraFechaCuota,
            valorAnticipo: parseFloat($("#txtValorAnticipoPlan").val()) || 0,
            fechaAnticipo: convertirFechaParaAPI($("#txtFechaAnticipoPlan").val()),
            cuotas: cuotas
        };

        $.ajax({
            url: '/api/PlanDePago/Guardar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(modeloPlan),
            success: function (response) {
                if (response.estado) {
                    $('#modalPlanDePago').modal('hide');
                    Swal.fire("Listo!", "Plan de Pagos guardado exitosamente.", "success");
                    tablaContratos.ajax.reload(); // Recargar tabla para reflejar cambios
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: function () {
                Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor.", "error");
            }
        });
    });

    // --- Eventos para Registrar Pago ---
    $(document).on('click', '#btnRegistrarPago', function() {
        const idPlanDePago = modeloBasePlanDePago.idPlanDePago;
        $('#txtIdPlanDePagoParaPago').val(idPlanDePago);
        // Limpiar formulario
        $('#formRegistrarPago')[0].reset();
        $('#modalRegistrarPago').modal('show');
    });

    $('#btnGuardarPago').on('click', function() {
        const idPlanDePago = $('#txtIdPlanDePagoParaPago').val();
        const monto = parseFloat($('#txtMontoPago').val());
        const fechaPago = $('#txtFechaPago').val();

        if (!monto || monto <= 0) {
            Swal.fire("Validación", "El monto del pago debe ser mayor a cero.", "warning");
            return;
        }
        if (!fechaPago) {
            Swal.fire("Validación", "La fecha de pago es obligatoria.", "warning");
            return;
        }

        const modeloPago = {
            idPlanDePago: parseInt(idPlanDePago),
            monto: monto,
            fechaPago: convertirFechaParaAPI(fechaPago),
            // El backend se encargará del comprobante si se envía como FormData
        };

        // Por ahora, enviaremos JSON. La carga de archivos se puede añadir después.
                $.ajax({
                    url: '/api/Pago/Registrar',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(modeloPago),
                    success: function(response) {
                        if (response.estado) {
                            $('#modalRegistrarPago').modal('hide');
                            $('#modalPlanDePago').modal('hide');
                            Swal.fire("Listo!", "Pago registrado exitosamente.", "success");
                            tablaContratos.ajax.reload(); // Recargar para forzar la actualización la próxima vez que se abra el plan
                        } else {
                            Swal.fire("Error", response.mensajes, "error");
                        }
                    },
                    error: function(jqXHR) {
                        Swal.fire("Error", "Error desconocido al conectar con el servidor.", "error");
                    }
                });
            });
            // #endregion

    // #region Eventos Ver Pagos
    $(document).on('click', '.btn-ver-pagos-plan', function() {
        const idPlanDePago = modeloBasePlanDePago.idPlanDePago;
        if (!idPlanDePago) {
            Swal.fire("Error", "No se pudo obtener la información del plan de pago.", "error");
            return;
        }

        $.ajax({
            url: `/api/Pago/ListarPorPlan/${idPlanDePago}`,
            type: 'GET',
            success: function(response) {
                if (response.estado && response.objeto) {
                    const tbody = $("#tbodyPagos");
                    tbody.empty();

                    if (response.objeto.$values && response.objeto.$values.length > 0) {
                        response.objeto.$values.forEach(pago => {
                            const fechaPago = new Date(pago.fechaPago).toLocaleDateString('es-ES', { day: '2-digit', month: '2-digit', year: 'numeric' });
                            const comprobanteLink = pago.comprobanteUrl 
                                ? `<a href="${pago.comprobanteUrl}" target="_blank">Ver Comprobante</a>`
                                : "N/A";

                            const row = `
                                <tr>
                                    <td>${fechaPago}</td>
                                    <td>${pago.monto.toFixed(2)}</td>
                                    <td>${comprobanteLink}</td>
                                </tr>
                            `;
                            tbody.append(row);
                        });
                    } else {
                        const row = `<tr><td colspan="3" class="text-center">No se encontraron pagos para este plan.</td></tr>`;
                        tbody.append(row);
                    }

                    $('#modalVerPagos').modal('show');
                } else {
                    Swal.fire("Error", `No se pudieron cargar los pagos: ${response.mensajes}`, "error");
                }
            },
            error: function() {
                Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor para obtener los pagos.", "error");
            }
        });
    });
    // #endregion
});