$(document).ready(function () {
    function manejarErrorFetch(error, operacion) {
        console.error(`Error en ${operacion}:`, error);
        if (error && error.mensajes) {
            Swal.fire("Error", error.mensajes, "error");
        } else {
            Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
        }
    }

    $("#formaPagoForm").submit(function (event) {
        event.preventDefault();

        const modelo = {
            SecFormaPago: $("#SecFormaPago").val(),
            Descripcion: $("#Descripcion").val(),
            EstaActivo: $("#EstaActivo").is(":checked")
        };

        if (modelo.Descripcion.trim() === "") {
            Swal.fire("Validación", "La descripción no puede estar vacía.", "warning");
            return;
        }

        const esNuevo = modelo.SecFormaPago == 0 || modelo.SecFormaPago == "";
        const url = esNuevo ? "/FormaPago/Crear" : "/FormaPago/Editar";
        const method = esNuevo ? "POST" : "PUT";

        fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json; charset=utf-8",
            },
            body: JSON.stringify(modelo) // Corregido: no doble serialización
        })
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                Swal.fire("Guardado", "La forma de pago ha sido guardada con éxito.", "success"); // Corregido: Título del Swal
                if (esNuevo) {
                    // Actualizar el ID en el formulario para futuras ediciones sin recargar la página
                    $("#SecFormaPago").val(responseJson.objeto.secFormaPago);
                }
            } else {
                Swal.fire("Error", responseJson.mensajes, "error");
            }
        })
        .catch((error) => {
            manejarErrorFetch(error, "Guardar Forma de Pago");
        });
    });
});