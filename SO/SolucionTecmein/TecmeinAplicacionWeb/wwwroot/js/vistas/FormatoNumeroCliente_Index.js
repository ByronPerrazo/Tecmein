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
    // Configuración de idioma local en español para DataTable
    const lenguajeEspanol = {
        processing:     "Procesando...",
        search:         "",
        searchPlaceholder: "Buscar...",
        lengthMenu:    "Mostrar _MENU_",
        info:           "Mostrando _START_ a _END_ de _TOTAL_ registros",
        infoEmpty:      "Mostrando 0 a 0 de 0 registros",
        infoFiltered:   "(filtrado de _MAX_ registros totales)",
        loadingRecords: "Cargando...",
        zeroRecords:    "No se encontraron resultados",
        emptyTable:     "Ningún dato disponible en esta tabla",
        paginate: {
            first:      "Primero",
            previous:   "Anterior",
            next:       "Siguiente",
            last:       "Último"
        }
    };

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
        columns: [
            { "data": "usaFormato", render: data => data ? '<span class="badge badge-info">Si</span>' : '<span class="badge badge-danger">No</span>' },
            { "data": "formato" },
            { "data": "numeroInicio" },
            { "data": "longitudNumero" },
            {
                data: "secFormatoNumeroCliente",
                render: function (data, type, row) {
                    return `<div class="dropdown">` +
                           `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">` +
                           `<i class="fas fa-cog text-warning mr-1"></i> Acciones` +
                           `</button>` +
                           `<div class="dropdown-menu">` +
                           `<a class="dropdown-item btn-editar" href="#"><i class="fas fa-pencil-alt text-primary mr-2"></i> Editar</a>` +
                           `</div>` +
                           `</div>`;
                },
                "orderable": false, "searchable": false, "width": "120px"
            }
        ],
        dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
        buttons: [
            {
                text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                extend: 'excelHtml5',
                title: 'Formatos de Número de Cliente',
                filename: 'Reporte Formatos Número Cliente',
                exportOptions: {
                    columns: [0, 1, 2, 3]
                },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-file-pdf text-danger fa-lg"></i>',
                extend: 'pdfHtml5',
                title: 'Formatos de Número de Cliente',
                filename: 'Reporte Formatos Número Cliente',
                exportOptions: {
                    columns: [0, 1, 2, 3]
                },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-print text-primary fa-lg"></i>',
                extend: 'print',
                title: 'Formatos de Número de Cliente',
                exportOptions: {
                    columns: [0, 1, 2, 3]
                },
                className: 'btn btn-link btn-sm p-1'
            }
        ],
        "processing": true,
        "language": lenguajeEspanol,
        "drawCallback": function(settings) {
            if (this.api().data().count() > 0) {
                $('#btnNuevo').hide();
            } else {
                $('#btnNuevo').show();
            }
        },
        initComplete: function() {
            $("#btnNuevo").appendTo(".toolbar-left");
            $("#btnNuevo").closest(".row").show();
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
