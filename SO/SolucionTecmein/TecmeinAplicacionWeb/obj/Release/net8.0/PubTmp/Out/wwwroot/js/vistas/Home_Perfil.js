
$(document).ready(function () {

    $(".container-fluid").LoadingOverlay("show");

    fetch("ObtenerUsuario")
        .then(
            respuesta => {
                $(".container-fluid").LoadingOverlay("hide");
                return respuesta.ok
                    ? respuesta.json()
                    : Promise.reject(respuesta);
            }
        ).then(
            respuestaJson => {
                if (respuestaJson.estado) {
                    const d = respuestaJson.objeto;
                    $("#imgFoto").attr("src", d.urlFoto);
                    $("#txtNombre").val(d.nombre);
                    $("#txtCorreo").val(d.correo);
                    $("#txTelefono").val(d.telefono);
                    $("#txtRol").val(d.nombreRol);
                    secuencialUserLog = parseInt(d.secuencial);
                } else {
                    swal("Fallo!", respuestaJson.mensajes, "error");
                }
            }
        ).catch(error => {
            console.error('Error al obtener los datos:', error);
        });
})


$("#btnGuardarCambios").click(function () {

    if ($("#txtCorreo").val().trim() == "") {
        const mensaje = `Debe llenar el campo Correo`;
        toastr.warning("", mensaje);
        ("#txtCorreo").focus();
        return;
    }

    if ($("#txTelefono").val().trim() == "") {
        const mensaje = `Debe llenar el campo Teléfono`;
        toastr.warning("", mensaje);
        ("#txTelefono").focus();
        return;
    }

    let modelo = {
        correo: $("#txtCorreo").val().trim(),
        telefono: $("#txTelefono").val().trim()
    }

    swal({
        title: "Está seguro?",
        text: `Se modificaran los datos de "${$("#txtNombre").val()}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, guardar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch("GuardarPerfil", {
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    body: JSON.stringify(modelo)
                })
                    .then(
                        respuesta => {
                            $(".showSweetAlert").LoadingOverlay("hide");
                            return respuesta.ok
                                ? respuesta.json()
                                : Promise.reject(respuesta);
                        }
                    ).then(
                        respuestaJson => {
                            if (respuestaJson.estado) {
                                swal("Listo!", "Información Guardada con Éxito", "success");
                            } else {
                                swal("Fallo!", respuestaJson.mensajes, "error");
                            }
                        }
                    ).catch(error => {
                        console.error('Error al Procesar Guardar Cambios:', error);
                    });

            }
        }
    )

});

$("#btnCambiarClave").click(function () {
    var claveActualForm = $("#txtClaveActual").val().trim();
    var claveNuevaForm = $("#txtClaveNueva").val().trim();
    var confirmaClaveForm = $("#txtConfirmarClave").val().trim();

    if (claveActualForm == "") {
        const mensaje = `Debe llenar el campo Clave Actual`;
        toastr.warning("", mensaje);
        ("#txtClaveActual").focus();
        return;
    }

    if (claveNuevaForm == "" || claveNuevaForm.length <= 5 || claveNuevaForm == claveActualForm) {
        const mensaje = `La nueva clave debe tener mas de 5 caracteres y ser distinta a la clave actual `;
        toastr.warning("", mensaje);
        ("#txtClaveNueva").focus();
        return;
    }

    if (confirmaClaveForm == "" || claveNuevaForm.length <= 5) {
        const mensaje = `Debe llenar el campo confirmar clave`;
        toastr.warning("", mensaje);
        ("#txtConfirmarClave").focus();
        return;
    }

    if (claveNuevaForm != confirmaClaveForm) {
        toastr.warning("", "El texto confirmar clave y nueva clave son distintas")
        return;
    }
        
   

    let modelo = {
        claveActual: claveActualForm,
        claveNueva: claveNuevaForm
    }

    //toastr.warning("","\t actual :" +
    //    modelo.claveActual + "\t nueva :" + modelo.claveNueva );



    swal({
        title: "Está seguro?",
        text: `Se cambiará la Clave de Acceso a "${$("#txtNombre").val()}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, guardar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch("CambiarClave", {
                    method: "POST",
                    headers: { "Content-Type": "application/json; charset=utf-8" },
                    body: JSON.stringify(modelo)
                })
                    .then(
                        respuesta => {
                            $(".showSweetAlert").LoadingOverlay("hide");
                            return respuesta.ok
                                ? respuesta.json()
                                : Promise.reject(respuesta);
                        }
                    ).then(
                        respuestaJson => {
                            if (respuestaJson.estado) {
                                swal("Listo!", "Clave Modificada con Éxito", "success");
                            } else {
                                swal("Fallo!", respuestaJson.mensajes, "error");
                            }
                        }
                    ).catch(error => {
                        console.error('Error al Procesar Guardar Cambios:', error);
                    });

            }
        }
    )

});