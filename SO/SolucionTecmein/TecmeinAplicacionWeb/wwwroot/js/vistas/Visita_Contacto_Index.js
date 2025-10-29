
const MODELO_VISITA_CONTACTO = {
    secuencial: 0,
    secVisita: "",
    secContacto: "",
    estaActivo: 1,
}

let contactosCombo = [];

$(document).ready(function () {

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
                // Acceder a la propiedad $values debido a ReferenceHandler.Preserve
                const data = respuestaJson.$values || respuestaJson;
                contactosCombo = data; // Asignar la data correcta
                data
                    .forEach(item => {
                        $("#cboContactos")
                            .append(
                                $("<option>")
                                    .val(item.secuencial)
                                    .text(`${item.nombres} ${item.apellidos}`)
                            );
                    })
            }
        )
        .catch(error => {
            console.error('Error al obtener la lista de Constructora:', error);
        });


    $('#modalDataContacto').on('shown.bs.modal', function () {
        let textarea = document.getElementById('txtDescripcionContacto');
        if (textarea) {
            ajustarAlturaTextarea(textarea);
        }
    });

});

const cmboConstructora = document.getElementById('cboEmpresa');
const cboContactos = document.getElementById('cboContactos');


let contrucSelect;
let contactoSelect;
let visitaContactoSelecionada = 0;


$("#tbdata tbody").on("click", ".btn-default", function () {

    limpiarFormularioModalContacto()

    esEdicion = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    cmboConstructora.value = -1;

    const data = tablaData.row(filaSeleccionada).data();
    visitaContactoSelecionada = data.Secuencial;
    mostrarModalVisitaContacto(data);

    $("#modalDataContacto").modal("show");
})



async function mostrarModalVisitaContacto(mdlVisitaContacto = MODELO_VISITA_CONTACTO) {
    try {

        cboContactos.value = '-1';

        // Realizar la solicitud para obtener los datos
        const respuesta = await fetch(`VisitaContacto?secuencialVisita=${mdlVisitaContacto.Secuencial}`, {
            method: "GET",
        });

        // Verificar si la respuesta es exitosa
        if (!respuesta.ok) {
            throw new Error(`Error al obtener los datos: ${respuesta.status} ${respuesta.statusText}`);
        }

        // Convertir la respuesta a JSON
        const respuestaJson = await respuesta.json();

        const data = respuestaJson.objeto || respuestaJson;

        if (data != null) {

            // Asignar valores a los campos del formulario
            const secConstructora = data.secConstructora || "";
            contactoSelect = data.secContacto || "";


            $("#cboEmpresa").val(secConstructora === "" ? $("#cboEmpresa option:first").val() : secConstructora);
            $("#cboContactos").val(contactoSelect === "" ? $("#cboContactos option:first").val() : contactoSelect);

            const selectEmpresa = document.getElementById('cboEmpresa');
            const event = new Event('change');
            selectEmpresa.dispatchEvent(event);

            CargaDetalleContacto(contactoSelect);
            // Mostrar el modal

            let textarea = document.getElementById('txtDescripcionContacto');
            ajustarAlturaTextarea(textarea);
        }

    } catch (error) {
        console.error("Error al obtener la lista de Operadores:", error);
    }

}

function CargaDetalleContacto(secuencialContacto) {
    // Buscar el contacto seleccionado
    const contactoSeleccionado = contactosCombo.find(x => x.secuencial == secuencialContacto);

    // Validar si el contacto existe
    if (!contactoSeleccionado) {
        console.error('Contacto no encontrado');
        contactoInfo.textContent = 'Información no disponible';
        return;
    }

    // Desestructuración para extraer propiedades
    const { nombres, apellidos, telefono, correo } = contactoSeleccionado;

    // Construcción del texto usando plantillas literales
    const textoDetalle =
        `     ${nombres} ${apellidos}
     Email: ${correo}
     Telef: ${telefono} `;


    $("#txtDescripcionContacto").val(textoDetalle);
    let textarea = document.getElementById('txtDescripcionContacto');
    ajustarAlturaTextarea(textarea);

}

function ajustarAlturaTextarea(textarea) {
    try {
        textarea.style.height = 'auto'; // Resetear la altura
        textarea.style.height = textarea.scrollHeight + 'px';
    } catch (error) {
        console.error("Error al Extender Detalle:", error);
    }

}

$("#cboEmpresa").on("change", function () {
    const constructoraSeleccionada = $(this).val();
    cargarContactosPorConstructora(constructoraSeleccionada);
    contrucSelect = constructoraSeleccionada;

});

function cargarContactosPorConstructora(constructoraSeleccionada, contactoSeleccionado = "") {
    try {

        if (!constructoraSeleccionada)
            return;

        let listaFiltrada = contactosCombo.filter(x => x.secConstructora == constructoraSeleccionada)

        $("#cboContactos").empty();
        $("#txtDescripcionContacto").val("");

        if (listaFiltrada.length > 0) {
            listaFiltrada.forEach(contacto => {
                $("#cboContactos")
                    .append(
                        $("<option>")
                            .val(contacto.secuencial)
                            .text(`${contacto.nombres} ${contacto.apellidos}`)
                    );
            });
        } else {
            $("#cboContactos").append($("<option>").val(-1).text("No definido"));
            fetch(`/Cliente/Obtener?idConstructora=${constructoraSeleccionada}`)
                .then(response => response.json())
                .then(data => {
                    if (data.estado) {
                        const constructora = data.objeto;
                        const textoDetalle =
                            `     ${constructora.nombre}\n` +
                            `     Email: ${constructora.correo}\n` +
                            `     Telef: ${constructora.telefono}`;
                        $("#txtDescripcionContacto").val(textoDetalle);
                        let textarea = document.getElementById('txtDescripcionContacto');
                        ajustarAlturaTextarea(textarea);
                    }
                });
        }


        const selectElement = document.getElementById('cboContactos');
        const event = new Event('change');
        selectElement.dispatchEvent(event);

    } catch (error) {
        console.error("Error al cargar los contactos:", error);
    }
}

$("#cboContactos").on("change", function () {
    const contactoSeleccionado = $(this).val();
    CargaDetalleContacto(contactoSeleccionado);
    contactoSelect = contactoSeleccionado;
});

function limpiarFormularioModalContacto() {
    $("#cboEmpresa").val($("#cboEmpresa option:first").val());
    $("#cboContactos").val($("#cboContactos option:first").val());
    $("#txtDescripcionContacto").val("");
}

$("#btnGrdVisitaContacto").click(crearContactoVisita);

async function crearContactoVisita() {
    try {
        $("#modalData").find("div.modal-content").LoadingOverlay("show");

        const modeloContactoVisita = structuredClone(MODELO_VISITA_CONTACTO);
        modeloContactoVisita.secVisita = visitaContactoSelecionada;
        modeloContactoVisita.secContacto = contactoSelect;
        modeloContactoVisita.estaActivo = $("#cboEstado").val();

        const datosContactoVisita = new FormData();
        datosContactoVisita.append("modelo", JSON.stringify(modeloContactoVisita));

        const response = await fetch("ProcesoGuardasContactoVisita", {
            method: "POST",
            body: datosContactoVisita
        });

        if (response.ok) {
            const responseJson = await response.json();
            if (responseJson.estado) {
                $("#modalDataContacto").modal("hide");
                Swal.fire("Listo!", "Contacto de Visita Guardado", "success");

            } else {
                Swal.fire("Fallo!", responseJson.mensajes, "error");
            }
        } else {
            // Handle non-OK responses
            if (response.status === 403) {
                Swal.fire("Acceso Denegado", "No tiene permisos para actualizar contactos de visita.", "error");
            } else {
                // For other non-OK responses, try to get a more specific message if available
                const errorText = await response.text();
                Swal.fire("Error!", `Error en la respuesta del servidor: ${errorText || response.statusText}`, "error");
            }
        }
    } catch (error) {
        Swal.fire("Error!", error.message, "error");
    } finally {
        $("#modalDataContacto").find("div.modal-content").LoadingOverlay("hide");
    }
}


