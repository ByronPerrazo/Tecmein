
const MODELO_BASE = {
    secuencial: "",
    nombre: "",
    correo: "",
    telefono: "",
    secRol: 0,
    nombrePerfil: "",
    nombreImagen: "",
    urlFoto: "",
    nombreFoto: "",
    esActivo: 1,
    fechaRegistro : ""
}

$(document).ready(function () {

    fetch("Usuario/ListaRol")
        .then(
            respuesta => {
                return respuesta.ok
                    ? respuesta.json()
                    : Promise.reject(respuesta);
            }
        ).then(
            respuestaJson => {
                respuestaJson
                    .forEach(item => {
                        $("#cboRol")
                            .append(
                                $("<option>")
                                    .val(item.secuencial)
                                    .text(item.descripcion)
                            )
                    })

            }
        ).catch(error => {
            console.error('Error al obtener la lista de perfiles:', error);
        });

    tablaData =
    $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": 'Usuario/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "secuencial", visible: true, searchable: true },

            {
                data: "urlFoto", render: function (data) {
                    return `<img style="height:60px" src="${data}" class="rounded mx-auto d-block"/>`;
                }

            },
            { data: "nombre", searchable: true },
            { data: "correo", searchable: true },
            { data: "telefono" },
            { data: "nombreRol" },
            {
                data: "esActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-info">Inactivo</span>';
                }
            },

            {
                defaultContent: '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                orderable: false,
                searchable: false,
                width: "80px"
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
                    columns: [0, 2,3,4,5,6]
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
    $("#txtCorreo").val(modelo.correo)
    $("#txtTelefono").val(modelo.telefono)
    $("#cboRol").val(modelo.secRol == "" ? $("#cboRol option:first").val() : modelo.secRol)
    $("#cboEstado").val(modelo.esActivo)
    $("#txtFoto").val(modelo.nombreFoto)
    $("#imgUsuario").attr("src", modelo.imagenUrl)
    $("#modalData").modal("show")
};

let esEdicion;
$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal()
})

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");

    inputs_vacios.forEach(x => {
        const mensaje = `Debe llenar el campo: "${x.name}"`;
        toastr.warning("", mensaje);
    });

    if (inputs_vacios.length > 0) {
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["secuencial"] = $("#txtId").val().trim();
    modelo["nombre"] = $("#txtNombre").val().trim();
    modelo["telefono"] = $("#txtTelefono").val().trim();
    modelo["secRol"] = parseInt($("#cboRol").val());
    modelo["esActivo"] = $("#cboEstado").val();
    modelo["correo"] = $("#txtCorreo").val().trim();

    const inputImagen = document.getElementById("txtFoto");
    const datosFormulario = new FormData();
    datosFormulario.append("imagen", inputImagen.files[0]);
    datosFormulario.append("modelo", JSON.stringify(modelo));

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (!esEdicion) {

        fetch("Usuario/Crear", {
            method: "POST",
            body: datosFormulario
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok
                    ? response.json()
                    : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!", "Usuario " + responseJson.objeto.nombre + " Creado con Exito", "success");
                }
                else {
                    swal("Fallo!", responseJson.mensajes, "error");
                }
            });
    } else {

        fetch("Usuario/Editar", {
            method: "PUT",
            body: datosFormulario
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok
                    ? response.json()
                    : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {
                    debugger;
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!", "Usuario " + responseJson.objeto.nombre + " Editado con Exito", "success");
                }
                else {
                    swal("Fallo!", responseJson.mensajes, "error");
                }
            });

    }

});
$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    data.nombreImagen
    mostrarModal(data);

})

$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    let fila
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
                        return response.ok
                            ? response.json()
                            : Promise.reject(response);
                    }).then(responseJson => {
                        debugger;
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw(false);

                            swal("Listo!", " El Usuario " + data.nombre + " fue Eliminado", "success");
                        }
                        else {
                            swal("Fallo!", responseJson.mensajes, "error");
                        }
                    });

            }
        }

    )

})