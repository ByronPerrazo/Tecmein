const MODELO_BASE = {
    secuencial: 0,
    nombre: "",
    esIva: false,
    esImportacion: false,
    estaActivo: true
}

let tablaData;
let filaSeleccionada;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
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
            { data: "secuencial", visible: false, searchable: false },
            { data: "nombre" },
            {
                data: "esIva", render: function (data) {
                    if (data)
                        return '<span class="badge badge-info">Sí</span>';
                    else
                        return '<span class="badge badge-danger">No</span>';
                }
            },
            {
                data: "esImportacion", render: function (data) {
                    if (data)
                        return '<span class="badge badge-info">Sí</span>';
                    else
                        return '<span class="badge badge-danger">No</span>';
                }
            },
            {
                data: "estaActivo", render: function (data) {
                    if (data)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button></div>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: 'Tipos de Impuesto',
                filename: 'Reporte Tipos de Impuesto',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtSecuencial").val(modelo.secuencial)
    $("#txtNombre").val(modelo.nombre)
    $("#cboEsIva").val(modelo.esIva.toString())
    $("#cboEsImportacion").val(modelo.esImportacion.toString())
    $("#cboEstaActivo").val(modelo.estaActivo.toString())

    $("#modalData").modal("show")
};

let esEdicion = false;

$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal();
})

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");

    if (inputs_vacios.length > 0) {
        const mensaje = `Debe llenar el campo: "${inputs_vacios[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["secuencial"] = $("#txtSecuencial").val();
    modelo["nombre"] = $("#txtNombre").val();
    modelo["esIva"] = $("#cboEsIva").val() === "true";
    modelo["esImportacion"] = $("#cboEsImportacion").val() === "true";
    modelo["estaActivo"] = $("#cboEstaActivo").val() === "true";

    const url = esEdicion ? "Editar" : "Crear";
    const method = esEdicion ? "PUT" : "POST";

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(url, {
        method: method,
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        if (responseJson.estado) {
            if(esEdicion) {
                tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
            } else {
                tablaData.row.add(responseJson.objeto).draw(false);
            }
            $("#modalData").modal("hide");
            Swal.fire("Listo!", `Tipo de Impuesto ${esEdicion ? 'editado' : 'creado'} correctamente`, "success");
        } else {
            Swal.fire("Fallo!", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        console.error("Error al guardar:", error);
    });
});

$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "Está Seguro de Eliminar?",
        text: `Eliminar el tipo de impuesto "${data.nombre}"`, // Changed to nombre
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`Eliminar?id=${data.secuencial}`, {
                method: "DELETE"
            })
            .then(response => {
                $(".showSweetAlert").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw();
                    Swal.fire("Listo!", "El Tipo de Impuesto fue eliminado", "success");
                } else {
                    Swal.fire("Fallo!", responseJson.mensajes, "error");
                }
            });
        }
    });
});