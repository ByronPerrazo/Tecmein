const MODELO_BASE = {
    secuencial: "0",
    urlLogo: "",
    nombreLogo: "",
    identificacion: "",
    nombre: "",
    correo: "",
    direccion: "",
    telefono: "",
    codigoOperador: "",
    estaActivo: 1
}

let tablaData;
let filaSeleccionada;
let esEdicion = false;

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
            "url": 'Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) { return json.data ? json.data.$values : []; },
            "error": function (jqXHR) { manejarErrorFetch(jqXHR.responseJSON, "Cargar Lista de Empresas"); }
        },
        "columns": [
            { data: "secuencial", visible: false },
            { data: "nombre" },
            { data: "identificacion" },
            { data: "correo" },
            { data: "direccion" },
            { data: "telefono" },
            { data: "estaActivo", render: function (data) { return data == 1 ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>'; } },
            {
                "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button>' +
                                  '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button></div>',
                "orderable": false, "searchable": false, "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: '<i class="fas fa-file-excel"></i> Excel',
                extend: 'excelHtml5',
                title: 'Empresas',
                filename: 'Reporte Empresas',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6]
                }
            },
            {
                text: '<i class="fas fa-file-pdf"></i> PDF',
                extend: 'pdfHtml5',
                title: 'Empresas',
                filename: 'Reporte Empresas',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6]
                }
            },
            {
                text: '<i class="fas fa-print"></i> Imprimir',
                extend: 'print',
                title: 'Empresas',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6]
                }
            },
            'pageLength'
        ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" },
    });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial);
    $("#txtIndentificacion").val(modelo.identificacion);
    $("#txtRazonSocial").val(modelo.nombre);
    $("#txtCorreo").val(modelo.correo);
    $("#txtDireccion").val(modelo.direccion);
    $("#txTelefono").val(modelo.telefono);
    $("#txtCodigoOperador").val(modelo.codigoOperador);
    $("#cboEstado").val(modelo.estaActivo);
    $("#imgLogo").attr("src", modelo.urlLogo);
    $("#txtLogo").val(''); // Limpiar el input de archivo
    $("#modalData").modal("show");
}

$("#btnNuevaEmpresa").click(function () {
    esEdicion = false;
    mostrarModal();
});

$("#btnGuardar").click(function () {
    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");

    if (inputs_vacios.length > 0) {
        toastr.warning(`Debe llenar el campo: "${inputs_vacios[0].name}"`);
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    const modelo = {
        secuencial: $("#txtId").val(),
        identificacion: $("#txtIndentificacion").val(),
        nombre: $("#txtRazonSocial").val(),
        correo: $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txTelefono").val(),
        codigoOperador: $("#txtCodigoOperador").val(),
        estaActivo: $("#cboEstado").val(),
        simboloMoneda: $("#txtSimboloMoneda").val()
    };

    const inputImagen = document.getElementById("txtLogo");
    const datosFormulario = new FormData();
    datosFormulario.append("logo", inputImagen.files[0]);
    datosFormulario.append("modelo", JSON.stringify(modelo));

    const url = esEdicion ? "Editar" : "GuardarCambios";
    const method = esEdicion ? "PUT" : "POST";
    const modalContent = $("#modalData .modal-content");

    modalContent.LoadingOverlay("show");

    fetch(url, { method: method, body: datosFormulario })
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(responseJson => {
            modalContent.LoadingOverlay("hide");
            if (responseJson.estado) {
                if (esEdicion) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                } else {
                    tablaData.row.add(responseJson.objeto).draw(false);
                }
                $("#modalData").modal("hide");
                Swal.fire("Listo!", `Empresa ${esEdicion ? 'editada' : 'guardada'} con éxito.`, "success");
            } else {
                Swal.fire("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            manejarErrorFetch(error, "Guardar Empresa", modalContent);
        });
});

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;
    filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "¿Está Seguro de Eliminar?",
        text: `Eliminar La Empresa "${data.nombre}"`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            const sweetAlertOverlay = $(".showSweetAlert");
            sweetAlertOverlay.LoadingOverlay("show");

            fetch(`Eliminar?secuencial=${data.secuencial}`, { method: "DELETE" })
                .then(response => {
                    if (!response.ok) return response.json().then(err => Promise.reject(err));
                    return response.json();
                })
                .then(responseJson => {
                    sweetAlertOverlay.LoadingOverlay("hide");
                    if (responseJson.estado) {
                        tablaData.row(fila).remove().draw(false);
                        Swal.fire("Listo!", `La Empresa "${data.nombre}" fue eliminada.`, "success");
                    } else {
                        Swal.fire("Fallo!", responseJson.mensajes, "error");
                    }
                })
                .catch(error => {
                    manejarErrorFetch(error, "Eliminar Empresa", sweetAlertOverlay);
                });
        }
    });
});