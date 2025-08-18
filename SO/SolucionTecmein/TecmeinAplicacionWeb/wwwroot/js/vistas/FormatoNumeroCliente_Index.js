$(document).ready(function () {
    $("#formatoForm").submit(function (event) {
        event.preventDefault();

        const modelo = {
            SecFormatoNumeroCliente: $("#SecFormatoNumeroCliente").val(),
            SecEmpresa: $("#SecEmpresa").val(),
            UsaFormato: $("#UsaFormato").is(":checked"),
            Formato: $("#Formato").val(),
            NumeroInicio: $("#NumeroInicio").val()
        };

        fetch("/FormatoNumeroCliente/Guardar", {
            method: "POST",
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
                swal("Listo!", "La configuración ha sido guardada", "success");
            } else {
                swal("Error!", responseJson.mensajes, "error");
            }
        })
        .catch((error) => {
            swal("Error!", "No se pudo guardar la configuración", "error");
        });
    });
});
