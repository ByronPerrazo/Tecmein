const MODELO_BASE = {
    secPlantillaPreContratoParrafo: 0,
    secPlantillaPreContrato: 0,
    orden: 0,
    contenido: "",
    estaActivo: 1
};

let tablaData;

function manejarErrorFetch(error, operacion) {
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

function inicializarPagina() {
    tinymce.init({
        selector: 'textarea#Contenido',
        telemetry: false,
        plugins: 'lists link image table code help wordcount',
        toolbar: 'undo redo | blocks | bold italic | alignleft aligncenter alignright | indent outdent | bullist numlist | code | table',
        language: 'es',
        height: 350
    });

    const secPlantilla = $("#SecPlantillaPreContrato").val();
    MODELO_BASE.secPlantillaPreContrato = parseInt(secPlantilla);

    // Configuración de idioma local en español para DataTable
    const lenguajeEspanol = {
        processing:     "Procesando...",
        search:         "",
        searchPlaceholder: "Buscar...",
        lengthMenu:    "Mostrar _MENU_",
        info:           "Mostrando _START_ a _END_ de _TOTAL_ registros",
        infoEmpty:      "Mostrando 0 a 0 de 0 registros",
        infoFiltered:   "(filtrado de _MAX_ registros totales)",
        loadingRecords: "Cargando...",
        zeroRecords:    "No se encontraron resultados",
        emptyTable:     "Ningún dato disponible en esta tabla",
        paginate: {
            first:      "Primero",
            previous:   "Anterior",
            next:       "Siguiente",
            last:       "Último"
        }
    };

    tablaData = $('#tbParrafos').DataTable({
        responsive: true,
        "ajax": {
            "url": `/PlantillaPreContratoParrafo/Lista?secPlantillaPreContrato=${secPlantilla}`,
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) { return json.data && json.data.$values ? json.data.$values : json.data; },
            "error": function(jqXHR) { manejarErrorFetch(jqXHR.responseJSON, "Cargar Párrafos"); }
        },
        "columns": [
            { "data": "orden", "width": "10%" },
            { "data": "contenido", "render": function(data) { return data.length > 100 ? data.substr(0, 100) + '...' : data; } },
            { "data": "estaActivo", "render": data => data ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>', "width": "10%" },
            {
                data: "secPlantillaPreContratoParrafo",
                render: function (data, type, row) {
                    return `<div class="dropdown">` +
                           `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">` +
                           `<i class="fas fa-cog text-warning mr-1"></i> Acciones` +
                           `</button>` +
                           `<div class="dropdown-menu">` +
                           `<a class="dropdown-item btn-editar" href="#" data-id="${data}"><i class="fas fa-pencil-alt text-primary mr-2"></i> Editar</a>` +
                           `<a class="dropdown-item btn-maquetar" href="#" data-id="${data}"><i class="fas fa-magic text-info mr-2"></i> Maquetar con IA</a>` +
                           `<a class="dropdown-item btn-eliminar" href="#" data-id="${data}"><i class="fas fa-trash-alt text-danger mr-2"></i> Eliminar</a>` +
                           `</div>` +
                           `</div>`;
                },
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        order: [[0, "asc"]],
        dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
        buttons: [
            {
                text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                extend: 'excelHtml5',
                title: 'Párrafos de Plantilla',
                filename: 'Reporte Parrafos Plantilla',
                exportOptions: {
                    columns: [0, 1, 2]
                },
                className: 'btn btn-link btn-sm p-1'
            }
        ],
        "language": lenguajeEspanol,
        initComplete: function() {
            $("#toolbar-botones").appendTo(".toolbar-left");
            $("#toolbar-botones").closest(".row").show();
        }
    });

    $("#btnNuevo").on("click", () => mostrarModal());
    $("#btnGuardar").on("click", () => guardarCambios());

    $("#btnCargarWord").on("click", () => {
        $("#archivoWord").click();
    });

    $("#archivoWord").on("change", function (e) {
        const input = e.target;
        if (input.files.length === 0) {
            return;
        }

        const archivo = input.files[0];
        const secPlantilla = $("#SecPlantillaPreContrato").val();

        const formData = new FormData();
        formData.append("archivoWord", archivo);
        formData.append("secPlantillaPreContrato", secPlantilla);

        Swal.fire({
            title: 'Procesando archivo...',
            text: 'Por favor, espere mientras se cargan los párrafos.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        fetch("/PlantillaPreContrato/CargarParrafosDesdeWord", {
            method: "POST",
            body: formData
        })
        .then(response => {
            if (!response.ok) {
                return response.json().then(err => Promise.reject(err));
            }
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                tablaData.ajax.reload(null, false);

                if (responseJson.advertencias && responseJson.advertencias.length > 0) {
                    let htmlAdvertencias = '<ul class="list-group text-left" style="max-height: 200px; overflow-y: auto;">';
                    responseJson.advertencias.forEach(adv => {
                        htmlAdvertencias += `<li class="list-group-item">${adv}</li>`;
                    });
                    htmlAdvertencias += '</ul>';

                    Swal.fire({
                        title: "Carga completada con advertencias",
                        html: `Se encontraron los siguientes placeholders no reconocidos:<br><br>${htmlAdvertencias}`,
                        icon: 'warning',
                        confirmButtonText: 'Entendido'
                    });
                } else {
                    Swal.fire("¡Listo!", "Los párrafos se han cargado correctamente y sin advertencias.", "success");
                }
            } else {
                Swal.fire("Error", responseJson.mensajes, "error");
            }
        })
        .catch(error => manejarErrorFetch(error, "Cargar Archivo Word"))
        .finally(() => {
            $(input).val('');
        });
    });

    $('#tbParrafos tbody').on('click', '.btn-editar', function (e) {
        e.preventDefault();
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        let data = tablaData.row(fila).data();
        mostrarModal(data);
    });

    $('#tbParrafos tbody').on('click', '.btn-eliminar', function (e) {
        e.preventDefault();
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        let data = tablaData.row(fila).data();
        eliminar(data);
    });

    $('#tbParrafos tbody').on('click', '.btn-maquetar', function (e) {
        e.preventDefault();
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        let data = tablaData.row(fila).data();

        Swal.fire({
            title: 'Maquetando párrafo con IA...',
            text: 'Por favor, espere mientras se procesa el contenido.',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        fetch("/PlantillaPreContrato/MaquetarParrafo", {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ secPlantillaPreContratoParrafo: data.secPlantillaPreContratoParrafo })
        })
        .then(response => {
            if (!response.ok) {
                return response.json().then(err => Promise.reject(err));
            }
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                tablaData.ajax.reload(null, false);
                Swal.fire("¡Éxito!", "El párrafo ha sido maquetado con placeholders.", "success");
            } else {
                Swal.fire("Error", responseJson.mensajes, "error");
            }
        })
        .catch(error => manejarErrorFetch(error, "Maquetar Párrafo con IA"));
    });
}


}

$(document).ready(function () {
    fetch("/api/config/tinymce-key")
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(config => {
            const script = document.createElement('script');
            script.src = `https://cdn.tiny.cloud/1/${config.apiKey}/tinymce/7/tinymce.min.js`;
            script.referrerpolicy = 'origin';
            script.onload = () => inicializarPagina();
            document.head.appendChild(script);
        })
        .catch(() => Swal.fire("Error Crítico", "No se pudo cargar la configuración del editor de texto.", "error"));
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#SecPlantillaPreContratoParrafo").val(modelo.secPlantillaPreContratoParrafo);
    $("#Orden").val(modelo.orden);
    tinymce.get('Contenido').setContent(modelo.contenido || "");
    $("#EstaActivo").val(modelo.estaActivo ? 1 : 0);
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

    fetch(url, { method: method, headers: { "Content-Type": "application/json; charset=utf-8" }, body: JSON.stringify(modelo) })
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
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
        .catch(error => manejarErrorFetch(error, "Guardar Párrafo"));
}

function eliminar(data) {
    Swal.fire({ /* ... */
        title: "¿Está seguro de eliminar este parrafo?",
        text: "Una vez eliminada, no podrá recuperarse.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "No, cancelar"
    })
        .then((result) => {
        if (result.isConfirmed) {
            fetch(`/PlantillaPreContratoParrafo/Eliminar?secPlantillaPreContratoParrafo=${data.secPlantillaPreContratoParrafo}`, { method: "DELETE" })
                .then(response => {
                    if (!response.ok) return response.json().then(err => Promise.reject(err));
                    return response.json();
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        tablaData.ajax.reload(null, false);
                        Swal.fire("¡Eliminado!", "El párrafo ha sido eliminado.", "success");
                    } else {
                        Swal.fire("Error", responseJson.mensajes, "error");
                    }
                })
                .catch(error => manejarErrorFetch(error, "Eliminar Párrafo"));
        }
    });
}

function cargarParametros() {
    fetch("/DiccionarioParametro/ListaActivos")
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(responseJson => {
            const listaParametros = responseJson.data && responseJson.data.$values ? responseJson.data.$values : responseJson.data;
            const contenedor = $("#parametros-disponibles");
            contenedor.html("");
            if (listaParametros && listaParametros.length > 0) {
                 listaParametros.forEach(item => {
                    const boton = $("<button>").addClass("btn btn-sm btn-outline-primary m-1 parametro-item").text(item.parametro).attr("title", item.descripcion);
                    contenedor.append(boton);
                });
            } else {
                contenedor.html("<p class='text-muted'>No hay parámetros activos.</p>");
            }
        })
        .catch(error => {
            console.error("Error al cargar parámetros:", error);
            $("#parametros-disponibles").html("<p class='text-danger'>No se pudieron cargar los parámetros.</p>");
        });
}

$("#parametros-disponibles").on("click", ".parametro-item", function() {
    const parametroAInsertar = $(this).text();
    if (tinymce.activeEditor) {
        tinymce.activeEditor.execCommand('mceInsertContent', false, parametroAInsertar);
    }
});
