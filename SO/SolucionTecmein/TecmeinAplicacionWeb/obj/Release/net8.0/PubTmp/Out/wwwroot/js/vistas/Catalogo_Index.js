const MODELO_BASE = {
    secuencial: "",
    nombre: "",
    urlCatalogo: "",
    fechaRegistro: "",
    estaActivo: 1
}

let tablaData;

$(document).ready(function () {
    tablaData = $('#tbdataCatalogo').DataTable({
        responsive: true,
        "ajax": {
            "url": 'Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                return json.data && json.data.$values ? json.data.$values : json.data;
            }
        },
        "columns": [
            { data: "secuencial", visible: false, searchable: true },
            { data: "nombre", searchable: true },
            {
                data: "urlCatalogo", render: function (data) {
                    return `<a href="${data}" target="_blank" class="btn-link btn-sm mr-2"> Abrir Documento <i class="fas fa-external-link-alt"></i></a>`;
                },
                "width": "150px"
            },
            { data: "fechaRegistro", searchable: true, "width": "100px" },
            {
                data: "estaActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">Inactivo</span>';
                },
                "width": "50px"
            },
            {
                "defaultContent":
                    '<div class="btn-group" role="group">' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>' +
                    '</div>',
                "orderable": false,
                "searchable": false,
                "width": "50px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: 'Catalogos',
                filename: 'Reporte de Catalogos',
                exportOptions: {
                    columns: [1, 2, 3, 4]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial)
    $("#txtNombre").val(modelo.nombre)
    $("#txtUrlCatalogo").val(modelo.urlCatalogo)
    $("#txtFechaRegistro").val(modelo.fechaRegistro)
    $("#cboEstado").val(modelo.estaActivo)
    $("#txtDocumento").val("")
    $("#modalData").modal("show")
};

$("#btnNuevo").click(function () {
    mostrarModal();
})

$("#btnGuardarCat").click(function () {

    const modelo = structuredClone(MODELO_BASE);
    modelo["secuencial"] = 0;
    modelo["nombre"] = $("#txtNombre").val().trim();
    modelo["urlCatalogo"] = $("#txtUrlCatalogo").val().trim();
    modelo["fechaRegistro"] = $("#txtFechaRegistro").val().trim();
    modelo["estaActivo"] = $("#cboEstado").val();

    const inputDocumento = document.getElementById("txtDocumento");
    if (inputDocumento.files.length < 1) {
        toastr.warning("", "Debe seleccionar un documento PDF");
        return;
    }

    const datosFormulario = new FormData();
    datosFormulario.append("archivoPDF", inputDocumento.files[0]);
    datosFormulario.append("modelo", JSON.stringify(modelo));

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch("CrearCatalogo", {
        method: "POST",
        body: datosFormulario
    })
    .then(response => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        if (!response.ok) {
            return response.json().then(errorJson => Promise.reject(errorJson));
        }
        return response.json();
    })
    .then(responseJson => {
        if (responseJson.estado) {
            tablaData.ajax.reload();
            $("#modalData").modal("hide");
            Swal.fire("Listo!", "Catalogo " + responseJson.objeto.nombre + " Creado ", "success");
        } else {
            Swal.fire("Fallo!", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        if (error && error.mensajes) {
            Swal.fire("Fallo!", error.mensajes, "error");
        } else {
            console.error('Hubo un problema con la solicitud Fetch:', error);
            Swal.fire("Fallo!", "Ocurrió un error inesperado al guardar.", "error");
        }
    });
});

$("#tbdataCatalogo tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "¿Está Seguro de Eliminar?",
        text: `Eliminar el Catalogo "${data.nombre}"`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`Eliminar?secuencial=${data.secuencial}`, {
                method: "DELETE"
            })
            .then(response => {
                $(".showSweetAlert").LoadingOverlay("hide");
                if (!response.ok) {
                    return response.json().then(errorJson => Promise.reject(errorJson));
                }
                return response.json();
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw(false);
                    Swal.fire("Listo!", "El Catalogo " + data.nombre + " fue Eliminado", "success");
                } else {
                    Swal.fire("Fallo!", responseJson.mensajes, "error");
                }
            })
            .catch(error => {
                $(".showSweetAlert").LoadingOverlay("hide");
                if (error && error.mensajes) {
                    Swal.fire("Fallo!", error.mensajes, "error");
                } else {
                    console.error('Hubo un problema con la solicitud Fetch:', error);
                    Swal.fire("Fallo!", "Ocurrió un error inesperado al eliminar.", "error");
                }
            });
        }
    });
});