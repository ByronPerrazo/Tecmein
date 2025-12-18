@section Scripts {
$(function () {
    // 1. Desactivar el manejo de errores por defecto de DataTables globalmente
    // Esto evita que DataTables muestre su propio mensaje de error.
    if ($.fn.dataTable) {
        $.fn.dataTable.ext.errMode = 'none';
    }

    // 2. Configuración global de AJAX para manejar errores (incluidos los de DataTables)
    $(document).ajaxError(function (event, jqxhr, settings, thrownError) {
        if (jqxhr.status === 403) {
            Swal.fire({
                icon: 'error',
                title: 'Acceso Denegado',
                text: 'Su usuario no tiene acceso a esta opción.',
                confirmButtonText: 'Entendido'
            }).then(() => { // AÑADIDO: Redirigir después de cerrar el SweetAlert
                window.location.href = '/Home/Index'; 
            });
        } else if (jqxhr.status === 401) {
            // Manejar 401 Unauthorized (sesión expirada, no autenticado)
            Swal.fire({
                icon: 'warning',
                title: 'Sesión Expirada',
                text: 'Su sesión ha expirado o no está autenticado. Por favor, inicie sesión de nuevo.',
                confirmButtonText: 'Entendido'
            }).then(() => {
                window.location.href = '/Acceso/Login'; // Redirigir al login
            });
        } else if (jqxhr.status >= 500) {
            // Manejar errores del servidor (5xx)
            Swal.fire({
                icon: 'error',
                title: 'Error del Servidor',
                text: 'Ocurrió un error inesperado en el servidor. Por favor, intente de nuevo más tarde.',
                confirmButtonText: 'Entendido'
            });
        } else if (jqxhr.status >= 400) {
            // Manejar otros errores del cliente (4xx, excepto 401, 403)
            Swal.fire({
                icon: 'warning',
                title: 'Solicitud Inválida',
                text: 'La solicitud no pudo ser procesada. Por favor, verifique los datos.',
                confirmButtonText: 'Entendido'
            });
        }
    });
});
}