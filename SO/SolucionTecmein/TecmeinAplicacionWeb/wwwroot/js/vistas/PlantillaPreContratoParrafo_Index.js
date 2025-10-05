$(document).ready(function () {
    $("#parrafoForm").submit(function (event) {
        event.preventDefault();

        const modelo = {
            SecPlantillaPreContratoParrafo: $("#SecPlantillaPreContratoParrafo").val(),
            SecPlantillaPreContrato: $("#SecPlantillaPreContrato").val(),
            Orden: $("#Orden").val(),
            Contenido: $("#Contenido").val(),
            EstaActivo: $("#EstaActivo").is(":checked") ? 1 : 0
        };

        let url = "";
        let method = "";

        if (modelo.SecPlantillaPreContratoParrafo == 0 || modelo.SecPlantillaPreContratoParrafo == "") {
            url = "/PlantillaPreContratoParrafo/Crear";
            method = "POST";
        } else {
            url = "/PlantillaPreContratoParrafo/Editar";
            method = "PUT";
        }

        fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json; charset=utf-8",
            },
            body: JSON.stringify({ modelo: JSON.stringify(modelo) })
        })
        .then(response => {
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                Swal.fire(, "El párrafo ha sido guardado", "success");
                // Optionally, update the hidden SecPlantillaPreContratoParrafo if it was a creation
                if (modelo.SecPlantillaPreContratoParrafo == 0 || modelo.SecPlantillaPreContratoParrafo == "") {
                    $("#SecPlantillaPreContratoParrafo").val(responseJson.objeto.secPlantillaPreContratoParrafo);
                }
            } else {
                Swal.fire("Error!", responseJson.mensajes, "error");
            }
        })
        .catch((error) => {
            Swal.fire("Error!", "No se pudo guardar el párrafo", "error");
        });
    });
});
