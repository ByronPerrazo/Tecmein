function manejarErrorFetch(error, operacion) {
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

$(document).ready(function () {
    $("#parrafoForm").submit(function (event) {
        event.preventDefault();

        const modelo = {
            SecPlantillaPreContratoParrafo: $("#SecPlantillaPreContratoParrafo").val(),
            SecPlantillaPreContrato: $("#SecPlantillaPreContrato").val(),
            Orden: $("#Orden").val(),
            Contenido: $("#Contenido").val(), // Asumiendo que es un textarea simple, no TinyMCE
            EstaActivo: $("#EstaActivo").is(":checked")
        };

        if (!modelo.Orden || !modelo.Contenido || modelo.Contenido.trim() === "") {
            Swal.fire("Validación", "El orden y el contenido son campos obligatorios.", "warning");
            return;
        }

        const esNuevo = modelo.SecPlantillaPreContratoParrafo == 0 || modelo.SecPlantillaPreContratoParrafo == "";
        const url = esNuevo ? "/PlantillaPreContratoParrafo/Crear" : "/PlantillaPreContratoParrafo/Editar";
        const method = esNuevo ? "POST" : "PUT";

        fetch(url, {
            method: method,
            headers: { "Content-Type": "application/json; charset=utf-8" },
            body: JSON.stringify(modelo) // Corregido: no doble serialización
        })
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                Swal.fire("Guardado", "El párrafo ha sido guardado con éxito.", "success");
                if (esNuevo) {
                    $("#SecPlantillaPreContratoParrafo").val(responseJson.objeto.secPlantillaPreContratoParrafo);
                }
            } else {
                Swal.fire("Error", responseJson.mensajes, "error");
            }
        })
        .catch((error) => {
            manejarErrorFetch(error, "Guardar Párrafo");
        });
    });
});