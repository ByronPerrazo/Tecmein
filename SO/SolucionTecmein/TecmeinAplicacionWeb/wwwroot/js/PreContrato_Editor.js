document.addEventListener('DOMContentLoaded', function () {

    tinymce.init({
        selector: '#editorPreContrato',
        plugins: 'anchor autolink charmap codesample emoticons image link lists media searchreplace table visualblocks wordcount',
        toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table | align lineheight | numlist bullist indent outdent | emoticons charmap | removeformat',
        height: 650,
        language: 'es'
    });

    document.getElementById('btnGuardar').addEventListener('click', function () {
        const cotizacionId = document.getElementById('hiddnCotizacionId').value;
        const contenido = tinymce.get('editorPreContrato').getContent();

        const data = {
            CotizacionId: parseInt(cotizacionId),
            Contenido: contenido
        };

        Swal.fire({
            title: 'Guardando...',
            text: 'Por favor espere.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        fetch('/PreContrato/GuardarPreContrato', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data),
        })
        .then(response => {
            if (!response.ok) {
                return response.json().then(err => { throw new Error(err.mensajes || 'Error del servidor al intentar guardar.'); });
            }
            return response.json();
        })
        .then(result => {
            Swal.close();
            if (result.estado) {
                Swal.fire({
                    icon: 'success',
                    title: '¡Guardado!',
                    text: 'Los cambios se han guardado correctamente.'
                });
            } else {
                throw new Error(result.mensajes || 'No se pudo guardar el pre-contrato.');
            }
        })
        .catch(error => {
            Swal.fire({
                icon: 'error',
                title: 'Error al guardar',
                text: error.message,
            });
        });
    });
});