// Función para validar y mostrar notificaciones de error
function validarInput(control, input, tipo) {
    // Expresiones regulares para validación
    const regexTexto = /^[a-zA-Z\s]*$/;
    const regexEmail = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    const regexTelefono = /^(0[1-9]\d{8}|0[1-9]\d{9}|[1-9]\d{8})$/;
    const regexNumeroDecimal = /^\d+(\.\d{1,2})?$/;
    const regexEntero = /^\d+$/;
    const fechaMaquina = new Date();

    let inicioMensaje = 'Error: En ' + control.name ;
    let estaVacio = input == '' || input == null;
    // Validar el input según el tipo
    let mensajeError = '';
    if (estaVacio) {
        mensajeError = inicioMensaje + ' está vacío.\n';
    } else {
        switch (tipo) {
            case 'texto':
                if (!regexTexto.test(input)) {
                    mensajeError = inicioMensaje + ' solo se permiten letras y espacios.\n';
                }
                break;
            case 'email':
                if (!regexEmail.test(input)) {
                    mensajeError = inicioMensaje + ' el formato de email es inválido.\n';
                }
                break;
            case 'telefono':
                if (!regexTelefono.test(input)) {
                    mensajeError = inicioMensaje + ' el número de teléfono es inválido.\n';
                }
                break;
            case 'decimal':
                if (!regexNumeroDecimal.test(input)) {
                    mensajeError = inicioMensaje + ' el formato de número decimal es inválido.\n';
                }
                break;
            case 'entero':
                if (!regexEntero.test(input)) {
                    mensajeError = inicioMensaje + ' el formato de número entero es inválido.\n';
                }
                break;
            case 'fechamayor':
                if (fechaMaquina.getTime() >= new Date(input).getTime()) {
                    mensajeError = 'Error: Fecha inferior o igual a la Fecha Actual.\n';
                }
                break;
            default:
                mensajeError = 'Error: tipo de validación no reconocido.\n';
        }
    }
    return mensajeError;
}


// Función para validar automáticamente los controles

function validarFormulario() {
    let controles = document.querySelectorAll('.validar-texto, .validar-email, .validar-telefono, .validar-decimal, .validar-entero');
    let mensajeRespuesta = '';

    controles.forEach(control => {

        if (control.offsetParent !== null) {
            if (control.classList.contains('validar-texto')) {
                mensajeRespuesta += validarInput(control, control.value, 'texto') 
            } else if (control.classList.contains('validar-email')) {
                mensajeRespuesta += validarInput(control, control.value, 'email')
            } else if (control.classList.contains('validar-telefono')) {
                mensajeRespuesta += validarInput(control, control.value, 'telefono')
            } else if (control.classList.contains('validar-decimal')) {
                mensajeRespuesta += validarInput(control, control.value, 'decimal')
            } else if (control.classList.contains('validar-entero')) {
                mensajeRespuesta += validarInput(control, control.value, 'entero')
            }         
        }
    });

    return mensajeRespuesta;
}

//card - body Contacto

function validarFechaFormulario(valida) {
    let controles = document.querySelectorAll('.validar-fecha-mayor');
    let mensajeRespuesta = '';

    if (valida) { 
        controles
            .forEach(control => {
                    if (control.classList.contains('validar-fecha-mayor')) {
                         mensajeRespuesta += validarInput(control, control.value, 'fechamayor')
                     }

    });
    }
    return mensajeRespuesta;
}


function abrirModal() {
    // Mostrar la ventana modal
    document.getElementById('miModal').style.display = 'block';

    // Cargar el mapa de Google Maps con las coordenadas GPS deseadas
    // (Aquí deberás utilizar la API de Google Maps para cargar el mapa con las coordenadas específicas)
    // Ejemplo de carga de mapa con coordenadas de Sydney, Australia
    var mapOptions = {
        center: { lat: -33.8688, lng: 151.2093 },
        zoom: 8
    };
    var map = new google.maps.Map(document.getElementById('mapa'), mapOptions);
}