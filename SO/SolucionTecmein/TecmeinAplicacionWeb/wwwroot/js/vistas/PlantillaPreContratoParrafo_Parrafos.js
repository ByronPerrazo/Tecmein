const MODELO_BASE = {
    secPlantillaPreContratoParrafo: 0,
    secPlantillaPreContrato: 0,
    orden: 0,
    contenido: "",
    estaActivo: 1
};

let tablaData;

function inicializarPagina() {
    tinymce.init({
        selector: 'textarea#Contenido',
        plugins: 'lists link image table code help wordcount',
        toolbar: 'undo redo | blocks | bold italic | alignleft aligncenter alignright | indent outdent | bullist numlist | code | table',
        language: 'es',
        height: 350
    });

    const secPlantilla = $("#SecPlantillaPreContrato").val();
    MODELO_BASE.secPlantillaPreContrato = parseInt(secPlantilla);

    tablaData = $('#tbParrafos').DataTable({
        responsive: true,
        "ajax": {
            "url": `/PlantillaPreContratoParrafo/Lista?secPlantillaPreContrato=${secPlantilla}`,
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                return json.data && json.data.$values ? json.data.$values : json.data;
            }
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
    fetch("/api/config/tinymce-key")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(config => {
            const script = document.createElement('script');
            script.src = `https://cdn.tiny.cloud/1/${config.apiKey}/tinymce/7/tinymce.min.js`;
            script.referrerpolicy = 'origin';
            script.onload = () => inicializarPagina();
            document.head.appendChild(script);
        })
        .catch(error => {
            console.error("Error fatal al cargar TinyMCE:", error);
            Swal.fire("Error Crítico", "No se pudo cargar el editor de texto.", "error");
        });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#SecPlantillaPreContratoParrafo").val(modelo.secPlantillaPreContratoParrafo);
    $("#Orden").val(modelo.orden);
    tinymce.get('Contenido').setContent(modelo.contenido || "");
    $("#EstaActivo").val(modelo.estaActivo ? 1 : 0);
    
    // Cargar los parámetros cada vez que se abre el modal
    cargarParametros();

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
    .then(response => response.ok ? response.json() : response.json().then(err => Promise.reject(err)))
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
            .then(response => response.ok ? response.json() : response.json().then(err => Promise.reject(err)))
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

function cargarParametros() {
    fetch("/DiccionarioParametro/ListaActivos")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            // Se verifica la estructura de datos que devuelve el serializador de .NET
            const listaParametros = responseJson.data && responseJson.data.$values ? responseJson.data.$values : responseJson.data;
            
            const contenedor = $("#parametros-disponibles");
            contenedor.html(""); // Limpiar antes de añadir

            if (listaParametros && listaParametros.length > 0) {
                 listaParametros.forEach(item => {
                    const boton = $("<button>")
                        .addClass("btn btn-sm btn-outline-primary m-1 parametro-item")
                        .text(item.parametro)
                        .attr("title", item.descripcion);
                    contenedor.append(boton);
                });
            } else {
                contenedor.html("<p class='text-muted'>No hay parámetros activos.</p>");
            }
        })
        .catch(error => {
            console.error("Error al cargar parámetros:", error);
            const contenedor = $("#parametros-disponibles");
            contenedor.html("<p class='text-danger'>No se pudieron cargar los parámetros.</p>");
        });
}

// Evento para insertar el parámetro en TinyMCE
$("#parametros-disponibles").on("click", ".parametro-item", function() {
    const parametroAInsertar = $(this).text();
    if (tinymce.activeEditor) {
        tinymce.activeEditor.execCommand('mceInsertContent', false, parametroAInsertar);
    }
});