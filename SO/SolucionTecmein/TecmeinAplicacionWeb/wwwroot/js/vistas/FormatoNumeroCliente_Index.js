const MODELO_BASE = {
    secFormatoNumeroCliente: 0,
    usaFormato: true,
    formato: "CLI-{YYYY}-",
    numeroInicio: 1,
    longitudNumero: 5
}

let tablaData;

function manejarErrorFetch(error, operacion, overlayElement) {
    if (overlayElement) $(overlayElement).LoadingOverlay("hide");
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/FormatoNumeroCliente/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                if (json.estado && json.objeto && json.objeto.$values) {
                    return json.objeto.$values;
                }
                return [];
            },
            "error": function (jqXHR) { manejarErrorFetch(jqXHR.responseJSON, "Cargar Formato"); }
        },
        "columns": [
            { "data": "usaFormato", render: data => data ? '<span class="badge badge-info">Si</span>' : '<span class="badge badge-danger">No</span>' },
            { "data": "formato" },
            { "data": "numeroInicio" },
            { "data": "longitudNumero" },
            {
                "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button></div>',
                "orderable": false, "searchable": false, "width": "40px"
            }
        ],
        "processing": true,
        "language": { "url": "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" },
        "drawCallback": function(settings) {
            if (this.api().data().count() > 0) {
                $('#btnNuevo').hide();
            } else {
                $('#btnNuevo').show();
            }
        }
    });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secFormatoNumeroCliente);
    $("#checkUsaFormato").prop("checked", modelo.usaFormato);
    $("#txtFormato").val(modelo.formato);
    $("#txtNumeroInicio").val(modelo.numeroInicio);
    $("#txtLongitudNumero").val(modelo.longitudNumero);
    $("#modalData").modal("show");
}

$("#btnNuevo").click(() => mostrarModal(MODELO_BASE));

$("#tbdata tbody").on("click", ".btn-editar", function () {
    let filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$('#btnGuardar').on('click', function () {
    const longitudNumero = parseInt($("#txtLongitudNumero").val() || "0");

    if (longitudNumero < 3) {
        Swal.fire("Valor no válido", "La longitud del número debe ser como mínimo 3.", "warning");
        return;
    }

    const modelo = {
        SecFormatoNumeroCliente: parseInt($("#txtId").val() || "0"),
        UsaFormato: $("#checkUsaFormato").is(":checked"),
        Formato: $("#txtFormato").val(),
        NumeroInicio: parseInt($("#txtNumeroInicio").val() || "0"),
        LongitudNumero: longitudNumero
    };

    const modalContent = $("#modalData .modal-content");
    modalContent.LoadingOverlay("show");

    fetch("/FormatoNumeroCliente/Guardar", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        if (!response.ok) return response.json().then(err => Promise.reject(err));
        return response.json();
    })
    .then(responseJson => {
        modalContent.LoadingOverlay("hide");
        if (responseJson.estado) {
            tablaData.ajax.reload(null, false);
            $("#modalData").modal("hide");
            Swal.fire("Listo!", "La configuración fue guardada", "success");
        } else {
            Swal.fire("Lo sentimos", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        manejarErrorFetch(error, "Guardar Configuración", modalContent);
    });
});
