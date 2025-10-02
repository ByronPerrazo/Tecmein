let tablaContratos;
let precontratosCargados = []; // Caché para los precontratos

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
    $("#cboEstadoDirecto").val(modelo.esActivo ? '1' : '0');
    $("#txtNombreProyecto").val(modelo.nombreProyecto);
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

// ... (El resto de las funciones de modal como abrirModalPlanDePago, generarParrillaCuotas se mantienen igual)
function abrirModalPlanDePago(idContrato, modelo = modeloBasePlanDePago) {
    $("#txtIdContratoPlan").val(idContrato);
    $("#modalPlanDePagoTitle").text(`Plan de Pagos para Contrato ${idContrato}`);
    $("#txtValorAnticipoPlan").val(modelo.valorAnticipo);
    $("#txtFechaAnticipoPlan").val(modelo.fechaAnticipo);
    $("#txtNumeroCuotasPlan").val(modelo.cuotas.length);
    generarParrillaCuotas(modelo.cuotas);

    // Cargar formas de pago en el combo (si es necesario en este modal)
    // $.ajax({ ... });

    $('#modalPlanDePago').modal('show');
}

function generarParrillaCuotas(cuotasExistentes = []) {
    const numCuotas = parseInt($("#txtNumeroCuotasPlan").val()) || 0;
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
                <td><input type="number" class="form-control form-control-sm monto-cuota" value="${cuota.montoEsperado}" min="0"></td>
            </tr>
        `;
        tbody.append(row);
    }
    // Re-inicializar datepickers para los nuevos campos
    $('.input-group.date').datepicker({ format: "dd/mm/yyyy", language: "es", autoclose: true, todayHighlight: true });
}
// #endregion

$(document).ready(function () {
    // Inicializar Datepickers
    $('.input-group.date').datepicker({ format: "dd/mm/yyyy", language: "es", autoclose: true, todayHighlight: true });

    // DataTable
    tablaContratos = $('#tbContrato').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Contrato/Listar',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (response) {
                // Adaptado para la respuesta estandarizada del backend
                if (response && response.data && response.data.$values) {
                    return response.data.$values;
                }
                // Fallback para la estructura anterior si fuera necesario
                return response.data || [];
            }
        },
        "columns": [
            { "data": "nombreObra" },
            { "data": "fechaFirma" },
            { "data": "nombreUsuarioCarga" },
            { "data": "rutaArchivo", render: data => data ? `<a href="${data}" target="_blank">Ver Archivo</a>` : "Sin archivo" },
            { "data": "esActivo", render: data => data ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                                  '<button class="btn btn-info btn-plan-pagos btn-sm mr-2"><i class="fas fa-dollar-sign"></i></button>' +
                                  '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false, "searchable": false, "width": "120px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: ['excelHtml5', 'pageLength'],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" },
    });

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
            $('#txtNombreProyecto').prop('readonly', false).val('');
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
                abrirModalPlanDePago(idContrato, response.objeto);
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

    $('#btnGuardarPlanDePago').on('click', function () {
        const idContrato = $("#txtIdContratoPlan").val();
        const idPlanDePago = modeloBasePlanDePago.idPlanDePago; // Se actualiza al cargar el modal

        const cuotas = [];
        $("#tbodyCuotas tr").each(function (index) {
            const fechaVencimiento = $(this).find(".fecha-cuota").val();
            const montoEsperado = parseFloat($(this).find(".monto-cuota").val()) || 0;
            cuotas.push({
                numeroCuota: index + 1,
                fechaVencimiento: fechaVencimiento,
                montoEsperado: montoEsperado,
                estado: "Pendiente" // Estado inicial
            });
        });

        const modeloPlan = {
            idPlanDePago: idPlanDePago,
            idContrato: idContrato,
            valorAnticipo: parseFloat($("#txtValorAnticipoPlan").val()) || 0,
            fechaAnticipo: $("#txtFechaAnticipoPlan").val(),
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
    // #endregion

});
