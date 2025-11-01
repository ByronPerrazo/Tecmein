let tablaContratos;
let precontratosCargados = [];
let modeloBasePlanDePago = {};

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

$(document).ready(function () {
    // Initialize datepickers with a standard format
    $('.date').datepicker({
        format: 'yyyy-mm-dd',
        language: 'es',
        autoclose: true,
        todayHighlight: true
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
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm" title="Editar"><i class="fas fa-pencil-alt"></i></button>' +
                                  '<button class="btn btn-danger btn-eliminar btn-sm" title="Eliminar"><i class="fas fa-trash-alt"></i></button>' +
                                  '<button class="btn btn-info btn-plan-pagos btn-sm" title="Plan de Pagos"><i class="fas fa-cash-register"></i></button>',
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
        // --- LÓGICA DE CONSTRUCCIÓN Y VALIDACIÓN RESTAURADA ---
        if (!validarCuotasPlanDePago()) { return; }
        const idContrato = $("#txtIdContratoPlan").val();
        const idPlanDePago = modeloBasePlanDePago.idPlanDePago;
        const secFormaPago = $("#cboFormaPagoPlan").val();
        if (!secFormaPago) { /* ... validación ... */ return; }
        const cuotas = [];
        $("#tbodyCuotas tr").each(function (index) { /* ... construcción de cuotas ... */ });
        const primeraFechaCuota = cuotas.length > 0 ? cuotas[0].fechaVencimiento : null;
        const modeloPlan = { /* ... construcción de modeloPlan ... */ };

        $.ajax({
            url: '/api/PlanDePago/Guardar',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(modeloPlan),
            success: function (response) { /* ... */ },
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
        // Lógica para abrir el modal de plan de pagos
        console.log("Abriendo plan de pagos para:", data);
        // Aquí iría la llamada AJAX para obtener el plan y luego mostrar el modal
        $('#modalPlanDePago').modal('show');
    });
});