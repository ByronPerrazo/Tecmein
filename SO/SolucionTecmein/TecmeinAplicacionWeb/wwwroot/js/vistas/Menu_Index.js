const MODELO_BASE = {
    secuencial: 0,
    descripcion: "",
    secMenuPadre: null,
    icono: "",
    controlador: "",
    paginaAccion: "",
    esActivo: 1
}

let tablaData;
let filaSeleccionada;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Menu/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                if (json.estado) {
                    // Asegurarse de que el serializador no cause problemas
                    return json.objeto && json.objeto.$values ? json.objeto.$values : json.objeto;
                }
                return [];
            }
        },
        "columns": [
            { "data": "secuencial", "visible": true, "searchable": false },
            { "data": "descripcion" },
            { "data": "descripcionMenuPadre" },
            { "data": "icono", render: function (data) { return `<i class="${data}"></i>`; } },
            { "data": "controlador" },
            { "data": "paginaAccion" },
            {
                "data": "esActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Menus',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
})

function mostrarModal(modelo = MODELO_BASE, listaMenusPadre = [], iconosDisponibles = []) {
    $("#txtId").val(modelo.secuencial);
    $("#txtDescripcion").val(modelo.descripcion);
    //$("#txtIcono").val(modelo.icono);
    $("#txtControlador").val(modelo.controlador);
    $("#txtPaginaAccion").val(modelo.paginaAccion);
    $("#cboEstado").val(modelo.esActivo);

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
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            if (responseJson.estado) {
                mostrarModal(MODELO_BASE, responseJson.objeto.listaMenusPadre, responseJson.objeto.iconosDisponibles);
            } else {
                swal("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            console.error('Error al obtener la lista de menús padre e iconos para nuevo menú:', error);
        });
})

$("#btnGuardar").click(function () {

    const modelo = {
        secuencial: parseInt($("#txtId").val()),
        descripcion: $("#txtDescripcion").val(),
        secMenuPadre: $("#cboMenuPadre").val() === "" ? null : parseInt($("#cboMenuPadre").val()),
        icono: $("#txtIcono").val(),
        controlador: $("#txtControlador").val(),
        paginaAccion: $("#txtPaginaAccion").val(),
        esActivo: $("#cboEstado").val()
    }

    $("#modalData").find(".modal-content").LoadingOverlay("show");

    fetch("/Menu/ProcesaGuardarMenu", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo)
    })
        .then(response => {
            $("#modalData").find(".modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                tablaData.ajax.reload();
                $("#modalData").modal("hide");
                swal("Listo!", "El menú fue guardado", "success");
            } else {
                swal("Lo sentimos", responseJson.mensajes, "error");
            }
        })
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
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                mostrarModal(responseJson.objeto.menu, responseJson.objeto.listaMenusPadre, responseJson.objeto.iconosDisponibles);
            } else {
                swal("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            console.error("Error en la llamada fetch para editar:", error);
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

    swal({
        title: "¿Está seguro?",
        text: `Eliminar el menú "${data.descripcion}"`, 
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (isConfirm) {
            if (isConfirm) {
                fetch(`/Menu/Eliminar?secuencial=${data.secuencial}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            swal("Listo!", "El menú fue eliminado", "success");
                        } else {
                            swal("Lo sentimos", responseJson.mensaje, "error");
                        }
                    })
            }
        }
    )
})

function formatIcon(icon) {
    if (!icon.id) {
        return icon.text;
    }
    var $icon = $(
        '<span><i class="' + icon.id + '"></i></span>'
    );
    return $icon;
}