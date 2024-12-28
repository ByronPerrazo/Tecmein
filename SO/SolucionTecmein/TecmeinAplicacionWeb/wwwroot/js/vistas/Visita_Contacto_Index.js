

const cmboConstructora = document.getElementById('cboEmpresa');
$(document).ready( function () {

    fetch("EmpresaConstructora")
                .then(
                    respuesta => {
                        return respuesta.ok
                            ? respuesta.json()
                            : Promise.reject(respuesta);
                    }
                )
                .then(
                    respuestaJson => {
                        listaCompletaCanton = respuestaJson;
                        respuestaJson
                            .forEach(item => {
                                $("#cboEmpresa")
                                    .append(
                                        $("<option>")
                                            .val(item.secuencial)
                                            .text(item.nombre.trim())
                                    )
                            })
                        cmboConstructora.value = '-1';
                    }
                )
                .catch(error => {
                    console.error('Error al obtener la lista de Empresas Contructoras:', error);
                });
});

const cboContactos = document.getElementById('cboContactos');
const contactosLista = [];
cmboConstructora.onchange = function () {

    cboContactos.innerHTML = '';

    var contrucSelect = cmboConstructora.value;

    fetch("Contactos")
        .then(
            respuesta => {
                return respuesta.ok
                    ? respuesta.json()
                    : Promise.reject(respuesta);
            }
        )
        .then(
            respuestaJson => {
                respuestaJson
                    .forEach(item => {
                        if (item.secConstructora == contrucSelect) {

                            contactosLista.push(item);
                            $("#cboContactos")
                                .append(
                                    $("<option>")
                                        .val(item.secuencial)
                                        .text(item.nombres + ' ' + item.apellidos)
                                )
                        }
                    })
                cboContactos.value = '-1';
            }
        )
        .catch(error => {
            console.error('Error al obtener la lista de Contactos:', error);
        });
};

const contactoInfo = document.getElementById('txtDescripcionContacto');

cboContactos.onchange = function () {

    var secuecial = cboContactos.value;
    var contactoSeleccionado = contactosLista.find(x => x.secuencial == secuecial);
    var nombreCompleto = contactoSeleccionado.nombres +' '+ contactoSeleccionado.apellidos ; 
    var telefoContacto = contactoSeleccionado.telefono; // Obtiene la llave secuencial
    var correoContacto = contactoSeleccionado.correo; // Obtiene la llave secuencial

    contactoInfo.textContent = ' ' + nombreCompleto +
                               '\n Email: ' + correoContacto + ' ' +
                               '\n Telef: ' + telefoContacto + ' ';

    contactoInfo.style.height = contactoInfo.scrollHeight + 'px'; // adapta el tamaño de la casilla
};
