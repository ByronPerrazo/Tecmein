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
    // ... (inicialización de datatable y otros listeners sin cambios) ...

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
});