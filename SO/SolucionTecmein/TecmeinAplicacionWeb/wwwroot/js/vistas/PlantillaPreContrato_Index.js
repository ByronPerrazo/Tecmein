$(document).ready(function () {
    $("#plantillaForm").submit(function (event) {
        event.preventDefault();

        const modelo = {
            SecPlantillaPreContrato: $("#SecPlantillaPreContrato").val(),
            Nombre: $("#Nombre").val(),
            NumeracionInicial: $("#NumeracionInicial").val(),
            EstaActivo: $("#EstaActivo").is(":checked") ? 1 : 0
        };

        let url = "";
        let method = "";

        if (modelo.SecPlantillaPreContrato == 0 || modelo.SecPlantillaPreContrato == "") {
            url = "/PlantillaPreContrato/Crear";
            method = "POST";
        } else {
            url = "/PlantillaPreContrato/Editar";
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
                swal("Listo!", "La plantilla ha sido guardada", "success");
                // Optionally, update the hidden SecPlantillaPreContrato if it was a creation
                if (modelo.SecPlantillaPreContrato == 0 || modelo.SecPlantillaPreContrato == "") {
                    $("#SecPlantillaPreContrato").val(responseJson.objeto.secPlantillaPreContrato);
                }
            } else {
                swal("Error!", responseJson.mensajes, "error");
            }
        })
        .catch((error) => {
            swal("Error!", "No se pudo guardar la plantilla", "error");
        });
    });
});
