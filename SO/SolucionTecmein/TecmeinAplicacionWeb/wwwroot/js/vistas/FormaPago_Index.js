$(document).ready(function () {
    $("#formaPagoForm").submit(function (event) {
        event.preventDefault();

        const modelo = {
            SecFormaPago: $("#SecFormaPago").val(),
            Descripcion: $("#Descripcion").val(),
            EstaActivo: $("#EstaActivo").is(":checked") ? 1 : 0
        };

        let url = "";
        let method = "";

        if (modelo.SecFormaPago == 0 || modelo.SecFormaPago == "") {
            url = "/FormaPago/Crear";
            method = "POST";
        } else {
            url = "/FormaPago/Editar";
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
                swal("Listo!", "La forma de pago ha sido guardada", "success");
                // Optionally, update the hidden SecFormaPago if it was a creation
                if (modelo.SecFormaPago == 0 || modelo.SecFormaPago == "") {
                    $("#SecFormaPago").val(responseJson.objeto.secFormaPago);
                }
            } else {
                swal("Error!", responseJson.mensajes, "error");
            }
        })
        .catch((error) => {
            swal("Error!", "No se pudo guardar la forma de pago", "error");
        });
    });
});
