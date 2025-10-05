$(document).ready(function () {
    $("#seguimientoForm").submit(function (event) {
        event.preventDefault();

        const modelo = {
            SecSeguimiento: $("#SecSeguimiento").val(),
            SecCotizacion: $("#SecCotizacion").val(),
            Accion: $("#Accion").val(),
            Detalle: $("#Detalle").val(),
            FechaAccion: $("#FechaAccion").val(),
            AceptacionCliente: $("#AceptacionCliente").is(":checked")
        };

        let url = "";
        let method = "";

        if (modelo.SecSeguimiento == 0 || modelo.SecSeguimiento == "") {
            url = "/Seguimiento/Crear";
            method = "POST";
        } else {
            url = "/Seguimiento/Editar";
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
                Swal.fire("Listo!", "El seguimiento ha sido guardado", "success");
                // Optionally, update the hidden SecSeguimiento if it was a creation
                if (modelo.SecSeguimiento == 0 || modelo.SecSeguimiento == "") {
                    $("#SecSeguimiento").val(responseJson.objeto.secSeguimiento);
                }
            } else {
                Swal.fire("Error!", responseJson.mensajes, "error");
            }
        })
        .catch((error) => {
            Swal.fire("Error!", "No se pudo guardar el seguimiento", "error");
        });
    });
});
