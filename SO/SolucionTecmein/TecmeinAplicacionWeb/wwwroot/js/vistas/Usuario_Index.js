const MODELO_BASE = {
    secuencial: "",
    nombre: "",
    correo: "",
    telefono: "",
    secRol: 0,
    esActivo: 1,
    urlFoto: ""
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
                // Asegurarse de que el serializador no cause problemas
                return json.data && json.data.$values ? json.data.$values : json.data;
            }
        },
        "columns": [
            { data: "secuencial", visible: false, searchable: false },
            {
                data: 'urlFoto', render: function (data) {
                    return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>`;
                }
            },
            { data: "nombre" },
            { data: "correo" },
            { data: "telefono" },
            { data: "nombreRol" },
            {
                data: "esActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
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
                title: 'Usuarios',
                filename: 'Reporte Usuarios',
                exportOptions: {
                    columns: [0, 2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

});

function mostrarModal(modelo = MODELO_BASE, listaRoles = []) {
    $("#txtId").val(modelo.secuencial)
    $("#txtNombre").val(modelo.nombre)
    $("#txtCorreo").val(modelo.correo)
    $("#txtTelefono").val(modelo.telefono)
    $("#cboEstado").val(modelo.esActivo)
    $("#txtFoto").val("")
    $("#imgUsuario").attr("src", modelo.urlFoto)

    const cboRol = $("#cboRol");
    cboRol.empty();
    if (listaRoles.length > 0) {
        listaRoles.forEach(item => {
            cboRol.append(
                $("<option>").val(item.secuencial).text(item.descripcion)
            )
        });
    }
    cboRol.val(modelo.secRol);

    $("#modalData").modal("show")
};

let esEdicion = false;

$("#btnNuevo").click(function () {
    esEdicion = false;
    // Para un nuevo usuario, necesitamos la lista de roles
    fetch("/Usuario/ListaRol")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            mostrarModal(MODELO_BASE, responseJson);
        })
        .catch(error => {
            console.error('Error al obtener la lista de perfiles para nuevo usuario:', error);
        });
})

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    const secuencialUsuario = data.secuencial;

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(`/Usuario/ObtenerParaEditar?secuencialUsuario=${secuencialUsuario}`)
        .then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                mostrarModal(responseJson.objeto.usuario, responseJson.objeto.listaRoles);
            } else {
                swal("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            console.error("Error en la llamada fetch para editar:", error);
        });
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
    modelo["secuencial"] = $("#txtId").val();
    modelo["nombre"] = $("#txtNombre").val();
    modelo["correo"] = $("#txtCorreo").val();
    modelo["telefono"] = $("#txtTelefono").val();
    modelo["secRol"] = $("#cboRol").val();
    modelo["esActivo"] = $("#cboEstado").val();

    const inputImagen = document.getElementById("txtFoto");
    const datosFormulario = new FormData();
    datosFormulario.append("imagen", inputImagen.files[0]);
    datosFormulario.append("modelo", JSON.stringify(modelo));

    const url = esEdicion ? "Editar" : "Crear";
    const method = esEdicion ? "PUT" : "POST";

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(url, {
        method: method,
        body: datosFormulario
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
            swal("Listo!", `Usuario ${esEdicion ? 'editado' : 'creado'} correctamente`, "success");
        } else {
            swal("Fallo!", responseJson.mensajes, "error");
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

    swal({
        title: "Está Seguro de Eliminar?",
        text: `Eliminar el usuario "${data.nombre}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
    function (respuesta) {
        if (respuesta) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`Eliminar?secuencialUsuario=${data.secuencial}`, {
                method: "DELETE"
            })
            .then(response => {
                $(".showSweetAlert").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw();
                    swal("Listo!", "El Usuario fue eliminado", "success");
                } else {
                    swal("Fallo!", responseJson.mensajes, "error");
                }
            });
        }
    });
});