
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
    estaActivo: ""
}

$(document).ready(function () {

    tablaData =
        $('#tbdata').DataTable({
            responsive: true,
            "ajax": {
                "url": 'Lista',
                "type": "GET",
                "datatype": "json",
                 "dataSrc": function (json) {
                    return json.data.$values;
                }
            },
            "columns": [
                { data: "secuencial", visible: false, searchable: true },
                { data: "nombre", searchable: true },
                { data: "identificacion", searchable: true },
                { data: "correo", searchable: true },
                { data: "direccion", searchable: true },
                { data: "telefono", searchable: true },

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
                    title: 'Tipo de Productos',
                    filename: 'Reporte Tipo Productos',
                    exportOptions: {
                        columns: [0, 1, 2]
                    }
                }, 'pageLength'
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });

})
function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial);
    $("#txtIndentificacion").val(modelo.identificacion);
    $("#txtRazonSocial").val(modelo.nombre);
    $("#txtCorreo").val(modelo.correo);
    $("#txtDireccion").val(modelo.direccion);
    $("#txTelefono").val(modelo.telefono);
    $("#txtCodigoOperador").val(modelo.codigoOperador);
    $("#cboEstado").val(modelo.estaActivo)
    $("#imgLogo").attr("src", modelo.urlLogo);

    $("#modalData").modal("show")
    }

let esEdicion;
$("#btnNuevaEmpresa").click(function () {
        esEdicion = false;
        mostrarModal();
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
        }

        const inputImagen = document.getElementById("txtLogo");
        const datosFormulario = new FormData();
        datosFormulario.append("logo", inputImagen.files[0]);
        datosFormulario.append("modelo", JSON.stringify(modelo));

        
        $("#modalData").find("div.modal-content").LoadingOverlay("show");

        if (!esEdicion) {

            fetch("GuardarCambios", {
                method: "POST",
                body: datosFormulario
            })
            
                .then(
                    respuesta => {
                        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                        return respuesta.ok
                            ? respuesta.json()
                            : Promise.reject(respuesta);
                    }
                ).then(
                    respuestaJson => {
                        if (respuestaJson.estado) {
                            const d = respuestaJson.objeto;

                            $("#imgLogo").attr("src", d.urlLogo);
                            tablaData.row.add(responseJson.objeto).draw(false);
                            $("#modalData").modal("hide");
                            swal("Listo!", "Información Guardada con Éxito", "success");
                        } else {
                            swal("Fallo!", respuestaJson.mensajes, "error");
                        }
                    }
                ).catch(error => {
                    console.error('Error al Procesar Guardar Cambios:', error);
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
                        $("#imgLogo").attr("src", responseJson.objeto.urlLogo);
                        swal("Listo!", "Empresa " + responseJson.objeto.nombre + " Editada ", "success");
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
        text: `Eliminar La Empresa "${data.nombre}"`,
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

                            swal("Listo!", " La Empresa " + data.nombre + " Fue Eliminada", "success");
                        }
                        else {
                            swal("Fallo!", responseJson.mensajes, "error");
                        }
                    });

            }
        }

    )

})

