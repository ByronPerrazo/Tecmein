
function validarFormulario(objeto, validaciones) {
    var campoEvaluar = objeto;

    for (var i = 0; i < validaciones.length; i++) {
        var tipoValidacion = validaciones[i].tipo;
        var mensajeError = validaciones[i].mensaje;

        if (tipoValidacion === 'texto' && campoEvaluar.trim() === '') {
            toastr.warning(mensajeError);
            return false;
        } else if (tipoValidacion === 'correo' && !/\S+@\S+\.\S+/.test(campoEvaluar)) {
            toastr.warning(mensajeError);
            return false;
        } else if (tipoValidacion === 'numero' && (isNaN(campoEvaluar) || campoEvaluar <= 0)) {
            toastr.warning(mensajeError);
            return false;
        }
    }

    return true;
}

//// Uso de la función de validación
//var validaciones = [
//    { tipo: 'texto', mensaje: 'El campo de nombre no puede estar vacío' },
//    { tipo: 'correo', mensaje: 'El correo electrónico no es válido' },
//    { tipo: 'numero', mensaje: 'El número debe ser mayor a cero' }
//];

//validarFormulario(objeto, validaciones);
