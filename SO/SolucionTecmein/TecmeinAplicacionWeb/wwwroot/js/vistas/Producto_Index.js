
const MODELO_BASE = {
    secuencial: "",
    secTipoProducto: 0,
    nombreTipoProducto: "",
    nombre: "",
    marca: "",
    sistema: "",
    capacidad: "",
    motor: "",
    stock: 0,
    urlImagen: "",
    nombreImagen: "",
    precio: 0,
    descripcion: "",
    estaActivo: 1,
}

$(document).ready(function () {
    fetch("ListaTipoProducto")
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
                        $("#cboTipoProducto")
                            .append(
                                $("<option>")
                                    .val(item.secuencial)
                                    .text(item.nombre)
                            )
                    })

            }
        ).catch(error => {
            console.error('Error al obtener lista de Tipos Productos:', error);
        });

    tablaData =
    $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": 'Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { data: "secuencial", visible: false, searchable: true },

            {
                data: "urlImagen", render: function (data) {
                    return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>`;
                }

            },
            { data: "nombre", searchable: true },
            { data: "marca", searchable: true },
            { data: "sistema" },
            { data: "capacidad", visible: false },
            { data: "motor", visible: false },
            { data: "descripcion" },
            { data: "nombreTipoProducto", searchable: true },
            { data: "stock" },
            { data: "precio" },
            
            {
                data: "estaActivo", render: function (data) {
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
                title: 'Productos',
                filename: 'Reporte de Productos',
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
    $("#txtMarca").val(modelo.marca)
    
    $("#cboSistema").val(modelo.sistema == "" ? $("#cboSistema option:first").val() : modelo.sistema)
    $("#txtCapacidad").val(modelo.capacidad)

    $("#cboMotor").val(modelo.motor == "" ? $("#cboMotor option:first").val() : modelo.motor)
    $("#txtDescripcion").val(modelo.descripcion)

    $("#cboTipoProducto").val(modelo.secTipoProducto == 0 ? $("#cboTipoProducto option:first").val() : modelo.secTipoProducto)
    $("#txtStock").val(modelo.stock)
    $("#txtPrecio").val(modelo.precio)
    $("#cboEstado").val(modelo.estaActivo)
    $("#txtImagen").val("")

    $("#imgProducto").attr("src", modelo.urlImagen)
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
    modelo["marca"] = $("#txtMarca").val().trim();
    modelo["sistema"] = $("#cboSistema").val().trim();
    modelo["capacidad"] = $("#txtCapacidad").val().trim();
    modelo["motor"] = $("#cboMotor").val().trim();
    modelo["descripcion"] = $("#txtDescripcion").val().trim();
    modelo["secTipoProducto"] = parseInt($("#cboTipoProducto").val().trim());
    modelo["stock"] = $("#txtStock").val().trim();
    modelo["estaActivo"] = $("#cboEstado").val();
    modelo["precio"] = parseFloat($("#txtPrecio").val().trim());

    const inputImagen = document.getElementById("txtImagen");
    const datosFormulario = new FormData();
    datosFormulario.append("imagen", inputImagen.files[0]);
    datosFormulario.append("modelo", JSON.stringify(modelo));

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (!esEdicion) {

        fetch("Crear", {
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
                    swal("Listo!", "Producto " + responseJson.objeto.nombre + " Creado ", "success");
                }
                else {
                    swal("Fallo!", responseJson.mensajes, "error");
                }
            });
    } else {

        fetch("Editar", {
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
                    swal("Listo!", "Producto " + responseJson.objeto.nombre + " Editado ", "success");
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

                fetch(`Eliminar?secuencial=${data.secuencial}`, {
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

                            swal("Listo!", " El producto " + data.nombre + " fue Eliminado", "success");
                        }
                        else {
                            swal("Fallo!", responseJson.mensajes, "error");
                        }
                    });

            }
        }

    )

})