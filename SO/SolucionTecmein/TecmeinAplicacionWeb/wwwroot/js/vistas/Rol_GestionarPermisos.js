$(document).ready(function () {
    $('#btnGuardarPermisos').click(function (e) {
        e.preventDefault(); // Evitar el envío normal del formulario

        var secRol = $('input[name="SecRol"]').val();
        var permisosSeleccionados = [];
        $('input[name="Permisos"]:checked').each(function () {
            permisosSeleccionados.push({
                IdPermiso: $(this).val(),
                Asignado: true // Siempre true si está seleccionado
            });
        });

        var modelo = {
            SecRol: parseInt(secRol),
            Permisos: permisosSeleccionados
        };

        // Mostrar overlay de carga
        $('body').LoadingOverlay("show"); // Asumiendo que LoadingOverlay está disponible

        fetch('@Url.Action("GuardarPermisos", "Rol")', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(modelo)
        })
        .then(response => {
            $('body').LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                Swal.fire("Listo!", "Permisos guardados correctamente", "success");
            } else {
                Swal.fire("Error!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            $('body').LoadingOverlay("hide");
            console.error('Error al guardar permisos:', error);
            Swal.fire("Error!", "Ocurrió un error al guardar los permisos.", "error");
        });
    });
});