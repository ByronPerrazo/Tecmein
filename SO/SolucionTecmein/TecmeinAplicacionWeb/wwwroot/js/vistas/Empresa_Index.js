$(document).ready(function () {

    $(".card-body").LoadingOverlay("show");

    fetch("Obtener")
        .then(
            respuesta => {
                $(".card-body").LoadingOverlay("hide");
                return respuesta.ok
                    ? respuesta.json()
                    : Promise.reject(respuesta);
            }
        ).then(
            respuestaJson => {
                if (respuestaJson.estado) {
                    const d = respuestaJson.objeto;
                    $("#txtNumeroDocumento").val(d.numeroDocumento);
                    $("#txtRazonSocial").val(d.nombre);
                    $("#txtCorreo").val(d.correo);
                    $("#txtDireccion").val(d.direccion);
                    $("#txTelefono").val(d.telefono);
                    $("#txtImpuesto").val(d.porcentajeImpuesto);
                    $("#txtSimboloMoneda").val(d.simboloMoneda);
                    $("#imgLogo").attr("src", d.urlLogo);
                } else {
                    swal("Fallo!", respuestaJson.mensajes, "error");
                }
            }
        ).catch(error => {
            console.error('Error al obtener los datos:', error);
        });
})

$("#btnGuardarCambios").click(function () {

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
        numeroDocumento: $("#txtNumeroDocumento").val(),
        nombre: $("#txtRazonSocial").val(),
        correo: $("#txtCorreo").val(),
        direccion: $("#txtDireccion").val(),
        telefono: $("#txTelefono").val(),
        porcentajeImpuesto: $("#txtImpuesto").val(),
        simboloMoneda: $("#txtSimboloMoneda").val()

    }

    const inputImagen = document.getElementById("txtLogo");
    const datosFormulario = new FormData();
    datosFormulario.append("logo", inputImagen.files[0]);
    datosFormulario.append("modelo", JSON.stringify(modelo));

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch("GuardarCambios", {
        method: "POST",
        body: datosFormulario
    })
        .then(
            respuesta => {
                $(".card-body").LoadingOverlay("hide");
                return respuesta.ok
                    ? respuesta.json()
                    : Promise.reject(respuesta);
            }
        ).then(
            respuestaJson => {
                if (respuestaJson.estado) {
                    const d = respuestaJson.objeto;
                   
                    $("#imgLogo").attr("src", d.urlLogo);

                    swal("Listo!", "Información Guardada con Éxito", "success");
                } else {
                    swal("Fallo!", respuestaJson.mensajes, "error");
                }
            }
        ).catch(error => {
            console.error('Error al Procesar Guardar Cambios:', error);
        });
})