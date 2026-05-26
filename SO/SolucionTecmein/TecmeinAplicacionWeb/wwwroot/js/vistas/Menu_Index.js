const MODELO_BASE = {
    secuencial: 0,
    descripcion: "",
    secMenuPadre: null,
    icono: "",
    controlador: "",
    paginaAccion: "",
    esActivo: 1,
    mostrarEnMenu: true,
    orden: 0
}

let tablaData;
let filaSeleccionada;

$(document).ready(function () {
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

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Menu/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                if (json.estado) {
                    return json.objeto && json.objeto.$values ? json.objeto.$values : json.objeto;
                }
                return [];
            }
        },
        "columns": [
            { "data": "secuencial", "visible": true, "searchable": false },
            { "data": "orden", "visible": false, "searchable": false },
            { "data": "descripcion" },
            { "data": "descripcionMenuPadre" },
            { "data": "icono", render: function (data) { return `<i class="${data}"></i>`; } },
            { "data": "controlador" },
            { "data": "paginaAccion" },
            {
                "data": "esActivo", render: function (data) {
                    return data == 1 ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                "data": "mostrarEnMenu", render: function (data) {
                    if (data)
                        return '<span class="badge badge-success">Visible</span>';
                    else
                        return '<span class="badge badge-secondary">Oculto</span>';
                }
            },
            {
                data: "secuencial",
                render: function (data, type, row) {
                    return `<div class="dropdown">` +
                           `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">` +
                           `<i class="fas fa-cog text-warning mr-1"></i> Acciones` +
                           `</button>` +
                           `<div class="dropdown-menu">` +
                           `<a class="dropdown-item btn-editar" href="#"><i class="fas fa-pencil-alt text-primary mr-2"></i> Editar</a>` +
                           `<a class="dropdown-item btn-eliminar" href="#"><i class="fas fa-trash-alt text-danger mr-2"></i> Eliminar</a>` +
                           `</div>` +
                           `</div>`;
                },
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        order: [[0, "desc"]],
        dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
        buttons: [
            {
                text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                extend: 'excelHtml5',
                title: 'Menús',
                filename: 'Reporte Menus',
                exportOptions: {
                    columns: [0, 2, 3, 5, 6, 7, 8]
                },
                className: 'btn btn-link btn-sm p-1'
            }
        ],
        "language": lenguajeEspanol,
        initComplete: function() {
            $("#btnNuevo, #btnSincronizar").appendTo(".toolbar-left");
            $("#btnNuevo").closest(".row").show();
        }
    });
});

function mostrarModal(modelo = MODELO_BASE, listaMenusPadre = [], iconosDisponibles = []) {
    $("#txtId").val(modelo.secuencial);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#txtOrden").val(modelo.orden);
    //$("#txtIcono").val(modelo.icono);
    $("#txtControlador").val(modelo.controlador);
    $("#txtPaginaAccion").val(modelo.paginaAccion);
    $("#cboEstado").val(modelo.esActivo);
    $("#checkMostrarEnMenu").prop("checked", modelo.mostrarEnMenu);

    const cboMenuPadre = $("#cboMenuPadre");
    cboMenuPadre.empty();
    cboMenuPadre.append($("<option>").val("").text("Sin Menú Padre"));
    if (listaMenusPadre.length > 0) {
        listaMenusPadre.forEach(item => {
            cboMenuPadre.append(
                $("<option>").val(item.secuencial).text(item.descripcion)
            );
        });
    }
    cboMenuPadre.val(modelo.secMenuPadre || "");

    const cboIcono = $("#txtIcono");
    cboIcono.empty();
    cboIcono.append($("<option>").val("").text("-- Seleccione un Icono --"));
    if (iconosDisponibles.length > 0) {
        iconosDisponibles.forEach(icono => {
            cboIcono.append(
                $("<option>").val(icono).text(icono)
            );
        });
    }
    cboIcono.val(modelo.icono || "");

    // Inicializar Select2 en el campo de íconos
    cboIcono.select2({
        templateResult: formatIcon,
        templateSelection: formatIcon,
        escapeMarkup: function (markup) { return markup; }
    });

    // Actualizar la vista previa del ícono cuando cambia la selección en Select2
    cboIcono.on('change', function () {
        var selectedIcon = $(this).val();
        $('#iconoPreviewModal').attr('class', selectedIcon);
    });

    // Establecer el valor inicial y la vista previa
    if (modelo.icono) {
        cboIcono.val(modelo.icono).trigger('change');
    }
    $("#iconoPreviewModal").attr('class', modelo.icono);

    $("#modalData").modal("show");
}

$("#btnNuevo").click(function () {
    // Para un nuevo menú, necesitamos la lista de menús padre y los iconos disponibles
    fetch("/Menu/ObtenerParaEditar?secuencial=0") // Usamos 0 para obtener solo la lista de iconos y menús padre
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorJson => {
                    return Promise.reject(errorJson);
                });
            }
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                mostrarModal(MODELO_BASE, responseJson.objeto.listaMenusPadre, responseJson.objeto.iconosDisponibles);
            } else {
                Swal.fire("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            console.error('Error al obtener la lista de menús padre e iconos para nuevo menú:', error);
            if (error && error.mensajes) {
                Swal.fire("Fallo!", error.mensajes, "error");
            } else {
                Swal.fire("Fallo!", "Ocurrió un error inesperado al cargar datos para nuevo menú.", "error");
            }
        });
})

$("#btnGuardar").click(function () {

    const modelo = {
        secuencial: parseInt($("#txtId").val()),
        descripcion: $("#txtDescripcion").val(),
        orden: parseInt($("#txtOrden").val()),
        secMenuPadre: $("#cboMenuPadre").val() === "" ? null : parseInt($("#cboMenuPadre").val()),
        icono: $("#txtIcono").val(),
        controlador: $("#txtControlador").val(),
        paginaAccion: $("#txtPaginaAccion").val(),
        esActivo: $("#cboEstado").val(),
        mostrarEnMenu: $("#checkMostrarEnMenu").is(":checked")
    }

    $("#modalData").find(".modal-content").LoadingOverlay("show");

    fetch("/Menu/ProcesaGuardarMenu", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo)
    })
        .then(response => {
            $("#modalData").find(".modal-content").LoadingOverlay("hide");
            if (!response.ok) {
                return response.json().then(errorJson => {
                    return Promise.reject(errorJson);
                });
            }
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                tablaData.ajax.reload();
                $("#modalData").modal("hide");
                Swal.fire("Listo!", "El menú fue guardado", "success");
            } else {
                Swal.fire("Lo sentimos", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            $("#modalData").find(".modal-content").LoadingOverlay("hide");
            if (error && error.mensajes) {
                Swal.fire("Fallo!", error.mensajes, "error");
            } else {
                console.error('Error al guardar el menú:', error);
                Swal.fire("Fallo!", "Ocurrió un error inesperado al guardar el menú.", "error");
            }
        });
})

let esEdicion;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    const secuencialMenu = data.secuencial;

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(`/Menu/ObtenerParaEditar?secuencial=${secuencialMenu}`)
        .then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            if (!response.ok) {
                return response.json().then(errorJson => {
                    return Promise.reject(errorJson);
                });
            }
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                mostrarModal(responseJson.objeto.menu, responseJson.objeto.listaMenusPadre, responseJson.objeto.iconosDisponibles);
            } else {
                Swal.fire("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            if (error && error.mensajes) {
                Swal.fire("Fallo!", error.mensajes, "error");
            } else {
                console.error("Error en la llamada fetch para editar:", error);
                Swal.fire("Fallo!", "Ocurrió un error inesperado al cargar datos para edición.", "error");
            }
        });
})


$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "¿Está seguro?",
        text: `Eliminar el menú "${data.descripcion}"`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`/Menu/Eliminar?secuencial=${data.secuencial}`, {
                method: "DELETE"
            })
                .then(response => {
                    if (!response.ok) {
                        return response.json().then(errorJson => {
                            return Promise.reject(errorJson);
                        });
                    }
                    return response.json();
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        tablaData.row(fila).remove().draw();
                        Swal.fire("Listo!", "El menú fue eliminado.", "success");
                    } else {
                        Swal.fire("Lo sentimos", responseJson.mensajes, "error");
                    }
                })
                .catch(error => {
                    if (error && error.mensajes) {
                        Swal.fire("Fallo!", error.mensajes, "error");
                    } else {
                        console.error('Error al eliminar:', error);
                        Swal.fire("Fallo!", "Ocurrió un error inesperado al eliminar el menú.", "error");
                    }
                });
        }
    })
})

$("#btnSincronizar").click(function () {
    Swal.fire({
        title: "¿Está seguro?",
        text: "Se buscarán nuevos módulos (controladores) en el sistema y se agregarán al menú. ¿Desea continuar?",
        icon: "info",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sí, sincronizar",
        cancelButtonText: "No, cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Sincronizando...',
                text: 'Por favor espere.',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading()
                }
            });

            fetch("/Menu/SincronizarModulos", {
                method: "POST",
                headers: { "Content-Type": "application/json; charset=utf-8" }
            })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(errorJson => Promise.reject(errorJson));
                }
                return response.json();
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.ajax.reload();
                    Swal.fire("Sincronización Completa", responseJson.objeto, "success");
                } else {
                    Swal.fire("Lo sentimos", responseJson.mensajes, "error");
                }
            })
            .catch(error => {
                Swal.close();
                if (error && error.mensajes) {
                    Swal.fire("Fallo!", error.mensajes, "error");
                } else {
                    console.error('Error al sincronizar:', error);
                    Swal.fire("Fallo!", "Ocurrió un error inesperado al sincronizar los módulos.", "error");
                }
            });
        }
    });
});

function formatIcon(icon) {
    if (!icon.id) {
        return icon.text;
    }
    var $icon = $(
        '<span><i class="' + icon.id + '"></i></span>'
    );
    return $icon;
}