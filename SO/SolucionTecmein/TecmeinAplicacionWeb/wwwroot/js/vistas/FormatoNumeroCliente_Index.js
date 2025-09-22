const MODELO_BASE = {
    secFormatoNumeroCliente: 0,
    usaFormato: true,
    formato: "CLI-{YYYY}-",
    numeroInicio: 1,
    longitudNumero: 5
}

let tablaData;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/FormatoNumeroCliente/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                if (json.estado) {
                    return json.objeto && json.objeto.$values ? json.objeto.$values : json.objeto;
                }
                return [];
            }
        },
        "columns": [
            {
                "data": "usaFormato", render: function (data) {
                    if (data)
                        return '<span class="badge badge-info">Si</span>';
                    else
                        return '<span class="badge badge-danger">No</span>';
                }
            },
            { "data": "formato" },
            { "data": "numeroInicio" },
            { "data": "longitudNumero" },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "40px"
            }
        ],
        "processing": true,
        "language": {
            "url": "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
        "drawCallback": function( settings ) {
            if (this.api().data().count() > 0) {
                $('#btnNuevo').hide();
            } else {
                $('#btnNuevo').show();
            }
        }
    });
})

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secFormatoNumeroCliente);
    $("#checkUsaFormato").prop("checked", modelo.usaFormato);
    $("#txtFormato").val(modelo.formato);
    $("#txtNumeroInicio").val(modelo.numeroInicio);
    $("#txtLongitudNumero").val(modelo.longitudNumero);
    $("#modalData").modal("show");
}

$("#btnNuevo").click(function () {
    mostrarModal(MODELO_BASE);
})

$("#tbdata tbody").on("click", ".btn-editar", function () {
    let filaSeleccionada;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
})


$("#btnGuardar").click(function () {

    const modelo = {
        secFormatoNumeroCliente: parseInt($("#txtId").val()),
        usaFormato: $("#checkUsaFormato").is(":checked"),
        formato: $("#txtFormato").val(),
        numeroInicio: parseInt($("#txtNumeroInicio").val()),
        longitudNumero: parseInt($("#txtLongitudNumero
").val())
    }

    $("#modalData").find(".modal-content").LoadingOverlay("show");

    fetch("/FormatoNumeroCliente/Guardar", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        $("#modalData").find(".modal-content").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        if (responseJson.estado) {
            tablaData.ajax.reload(null, false); // No resetear paginación
            $("#modalData").modal("hide");
            Swal.fire("Listo!", "La configuración fue guardada", "success");
        } else {
            Swal.fire("Lo sentimos", responseJson.mensajes, "error");
        }
    })
})