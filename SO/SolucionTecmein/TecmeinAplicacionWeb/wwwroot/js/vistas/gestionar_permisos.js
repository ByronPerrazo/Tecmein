$(document).ready(function () {
    $("#btnGuardarCambios").on("click", function () {
        const secRol = $("#secRol").val();

        const permisosAsignados = $(".permiso-check:checked").map(function () {
            return $(this).val();
        }).get();

        const menusAsignados = $(".menu-check:checked").map(function () {
            return parseInt($(this).val()); // Asegurarse de que los IDs de menú sean números
        }).get();

        const modelo = {
            SecRol: parseInt(secRol),
            PermisosAsignados: permisosAsignados,
            MenusAsignados: menusAsignados
        };

        // Muestra un overlay de carga
        $("div.card").LoadingOverlay("show");

        fetch("/Rol/GuardarPermisos", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json;charset=utf-8'
            },
            body: JSON.stringify(modelo)
        })
        .then(response => {
            $("div.card").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                swal("¡Listo!", "Los permisos se han actualizado correctamente", "success").then(() => {
                    // Opcional: redirigir a la lista de roles
                    window.location.href = "/Rol/Index";
                });
            } else {
                swal("Error", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            $("div.card").LoadingOverlay("hide");
            swal("Error", "No se pudo comunicar con el servidor", "error");
            console.error("Error en la solicitud fetch:", error);
        });
    });
});
