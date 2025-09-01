const MODELO_BASE = {
    secPlantillaPreContratoParrafo: 0,
    secPlantillaPreContrato: 0,
    orden: 0,
    contenido: "",
    estaActivo: 1 // 1 para Activo, 0 para Inactivo
};

let tablaData;

// Esta función se llamará DESPUÉS de que el script de TinyMCE se haya cargado
function inicializarPagina() {
    // 1. Inicializar TinyMCE
    tinymce.init({
        selector: 'textarea#Contenido',
        plugins: 'lists link image table code help wordcount',
        toolbar: 'undo redo | blocks | bold italic | alignleft aligncenter alignright | indent outdent | bullist numlist | code | table',
        language: 'es'
    });

    // 2. Inicializar DataTable
    const secPlantilla = $("#SecPlantillaPreContrato").val();
    MODELO_BASE.secPlantillaPreContrato = parseInt(secPlantilla);

    tablaData = $('#tbParrafos').DataTable({
        responsive: true,
        "ajax": {
            "url": `/PlantillaPreContratoParrafo/Lista?secPlantillaPreContrato=${secPlantilla}`,
            "type": "GET",
            "datatype": "json",
            "dataSrc": "data.$values"
        },
        "columns": [
            { "data": "orden", "width": "10%" },
            { "data": "contenido", "render": function(data) { return data.length > 100 ? data.substr(0, 100) + '...' : data; } },
            {
                "data": "estaActivo", "render": function (data) {
                    return data ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>';
                }, "width": "10%"
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "10%"
            }
        ],
        order: [[0, "asc"]],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    // 3. Asignar eventos a los botones
    $("#btnNuevo").on("click", function () {
        mostrarModal();
    });

    $("#btnGuardar").on("click", function () {
        guardarCambios();
    });

    $('#tbParrafos tbody').on('click', '.btn-editar', function () {
        let data = tablaData.row($(this).parents('tr')).data();
        mostrarModal(data);
    });

    $('#tbParrafos tbody').on('click', '.btn-eliminar', function () {
        let data = tablaData.row($(this).parents('tr')).data();
        eliminar(data);
    });
}

$(document).ready(function () {
    // 1. Pedir la clave de API al servidor
    fetch("/api/config/tinymce-key")
        .then(response => {
            if (!response.ok) throw new Error("No se pudo obtener la clave de API.");
            return response.json();
        })
        .then(config => {
            // 2. Crear el tag de script dinámicamente con la clave obtenida
            const script = document.createElement('script');
            script.src = `https://cdn.tiny.cloud/1/${config.apiKey}/tinymce/7/tinymce.min.js`;
            script.referrerpolicy = 'origin';
            
            // 3. Cuando el script de TinyMCE termine de cargar, inicializar el resto de la página
            script.onload = () => {
                inicializarPagina();
            };
            
            document.head.appendChild(script);
        })
        .catch(error => {
            console.error("Error fatal al cargar TinyMCE:", error);
            Swal.fire("Error Crítico", "No se pudo cargar el editor de texto. Por favor, contacte al administrador.", "error");
        });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#SecPlantillaPreContratoParrafo").val(modelo.secPlantillaPreContratoParrafo);
    $("#Orden").val(modelo.orden);
    tinymce.get('Contenido').setContent(modelo.contenido || "");
    $("#EstaActivo").val(modelo.estaActivo ? 1 : 0);
    $('#parrafoModal').modal('show');
}

function guardarCambios() {
    const modelo = {
        secPlantillaPreContratoParrafo: parseInt($("#SecPlantillaPreContratoParrafo").val()),
        secPlantillaPreContrato: parseInt($("#SecPlantillaPreContrato").val()),
        orden: parseInt($("#Orden").val()),
        contenido: tinymce.get('Contenido').getContent(),
        estaActivo: $("#EstaActivo").val() == "1"
    };

    const esNuevo = modelo.secPlantillaPreContratoParrafo === 0;
    const url = esNuevo ? "/PlantillaPreContratoParrafo/Crear" : "/PlantillaPreContratoParrafo/Editar";
    const method = esNuevo ? "POST" : "PUT";

    fetch(url, {
        method: method,
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        if (response.ok) return response.json();
        else return response.json().then(err => Promise.reject(err));
    })
    .then(responseJson => {
        if (responseJson.estado) {
            tablaData.ajax.reload(null, false);
            $('#parrafoModal').modal('hide');
            Swal.fire("¡Listo!", "El párrafo ha sido guardado.", "success");
        } else {
            Swal.fire("Error", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        console.error("Error en fetch:", error);
        Swal.fire("Error", `No se pudo guardar el párrafo. ${error.mensajes || ''}`, "error");
    });
}

function eliminar(data) {
     Swal.fire({
        title: '¿Está seguro?',
        text: `Eliminar el párrafo con orden "${data.orden}"`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'No, cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`/PlantillaPreContratoParrafo/Eliminar?secPlantillaPreContratoParrafo=${data.secPlantillaPreContratoParrafo}`, {
                method: "DELETE"
            })
            .then(response => {
                if (response.ok) return response.json();
                else return response.json().then(err => Promise.reject(err));
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.ajax.reload(null, false);
                    Swal.fire("¡Eliminado!", "El párrafo ha sido eliminado.", "success");
                } else {
                    Swal.fire("Error", responseJson.mensajes, "error");
                }
            })
            .catch((error) => {
                console.error("Error en fetch:", error);
                Swal.fire("Error", `No se pudo eliminar el párrafo. ${error.mensajes || ''}`, "error");
            });
        }
    });
}
