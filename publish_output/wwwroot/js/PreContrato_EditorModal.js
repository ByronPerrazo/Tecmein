$(document).ready(function () {
    let editorInstance;

    // Función para inicializar el editor TinyMCE
    function inicializarTinyMCE(contenidoInicial = "") {
        if (editorInstance) {
            tinymce.remove(editorInstance); // Asegurarse de que no haya instancias duplicadas
        }
        tinymce.init({
            selector: 'textarea#editorContenidoPreContrato',
            plugins: 'lists link image table code help wordcount',
            toolbar: 'undo redo | blocks | bold italic | alignleft aligncenter alignright | indent outdent | bullist numlist | code | table',
            language: 'es',
            height: 500,
            setup: function (editor) {
                editor.on('init', function () {
                    editor.setContent(contenidoInicial);
                    editorInstance = editor; // Guardar la instancia del editor
                });
            }
        });
    }

    // Función global para abrir el modal del editor y cargar el contenido
    window.abrirEditorModal = function (secPreContrato) {
        $("#hiddnSecPreContratoEditor").val(secPreContrato);
        
        // Obtener el contenido pre-populado (plantilla + placeholders)
        $.get(`/PreContrato/PrevisualizarContenido/${secPreContrato}`, function(response) {
            if(response.estado) {
                inicializarTinyMCE(response.objeto.contenido);
                $('#modalEditorPreContrato').modal('show');
            } else {
                Swal.fire("Error", response.mensajes, "error");
            }
        }).fail(function() {
            Swal.fire("Error", "No se pudo obtener el contenido para el editor.", "error");
        });
    };

    // Manejador para el botón "Guardar Contenido" del editor
    $("#btnGuardarContenidoEditor").click(function() {
        const secPreContrato = parseInt($("#hiddnSecPreContratoEditor").val());
        const contenido = tinymce.get('editorContenidoPreContrato').getContent();

        if (!secPreContrato) {
            Swal.fire("Error", "ID de Pre-Contrato no encontrado.", "error");
            return;
        }

        if (!contenido) {
            Swal.fire("Advertencia", "El contenido del editor no puede estar vacío.", "warning");
            return;
        }

        const datos = {
            SecPreContrato: secPreContrato,
            Contenido: contenido
        };

        $.ajax({
            url: "/PreContrato/ActualizarContenido", // Nuevo endpoint para actualizar solo el contenido
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(datos),
            success: function(response) {
                if (response.estado) {
                    Swal.fire("¡Guardado!", "Contenido del pre-contrato actualizado.", "success").then(() => {
                        $('#modalEditorPreContrato').modal('hide');
                        // Opcional: recargar la tabla principal si es necesario
                        // tablaPreContratos.ajax.reload(null, false);
                    });
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: function() {
                Swal.fire("Error", "No se pudo comunicar con el servidor para guardar el contenido.", "error");
            }
        });
    });

    // Cargar TinyMCE dinámicamente (similar a PlantillaPreContratoParrafo_Parrafos.js)
    fetch("/api/config/tinymce-key")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(config => {
            const script = document.createElement('script');
            script.src = `https://cdn.tiny.cloud/1/${config.apiKey}/tinymce/7/tinymce.min.js`;
            script.referrerpolicy = 'origin';
            // No inicializamos TinyMCE aquí, solo cargamos el script.
            // La inicialización se hará en inicializarTinyMCE() cuando se abra el modal.
            document.head.appendChild(script);
        })
        .catch(error => {
            console.error("Error fatal al cargar TinyMCE:", error);
            Swal.fire("Error Crítico", "No se pudo cargar el editor de texto. Verifique la configuración del servidor.", "error");
        });
});
