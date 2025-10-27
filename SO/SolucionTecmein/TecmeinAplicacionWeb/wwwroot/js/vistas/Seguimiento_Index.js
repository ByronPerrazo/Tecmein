function manejarErrorFetch(error, operacion) {
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

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

        if (modelo.Accion.trim() === "" || modelo.Detalle.trim() === "" || modelo.FechaAccion.trim() === "") {
            Swal.fire("Validación", "Acción, Detalle y Fecha son campos obligatorios.", "warning");
            return;
        }

        const esNuevo = modelo.SecSeguimiento == 0 || modelo.SecSeguimiento == "";
        const url = esNuevo ? "/Seguimiento/Crear" : "/Seguimiento/Editar";
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
                Swal.fire("Guardado", "El seguimiento ha sido guardado con éxito.", "success");
                if (esNuevo) {
                    $("#SecSeguimiento").val(responseJson.objeto.secSeguimiento);
                }
                // Aquí podrías agregar lógica para recargar la tabla de seguimientos si estuviera en la misma página
            } else {
                Swal.fire("Error", responseJson.mensajes, "error");
            }
        })
        .catch((error) => {
            manejarErrorFetch(error, "Guardar Seguimiento");
        });
    });
});