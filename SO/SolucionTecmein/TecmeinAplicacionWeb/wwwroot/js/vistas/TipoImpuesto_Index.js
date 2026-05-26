const MODELO_BASE = {
    secuencial: 0,
    nombre: "",
    esIva: false,
    esImportacion: false,
    estaActivo: true
}

let tablaData;
let filaSeleccionada;
let esEdicion = false;

function manejarErrorFetch(error, operacion, overlayElement) {
    if (overlayElement) $(overlayElement).LoadingOverlay("hide");
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

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

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": 'Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) { return json.data && json.data.$values ? json.data.$values : json.data; },
            "error": function(jqXHR) { manejarErrorFetch(jqXHR.responseJSON, "Cargar Tipos de Impuesto"); }
        },
        "columns": [
            { data: "secuencial", visible: false },
            { data: "nombre" },
            { data: "esIva", render: data => data ? '<span class="badge badge-info">Sí</span>' : '<span class="badge badge-danger">No</span>' },
            { data: "esImportacion", render: data => data ? '<span class="badge badge-info">Sí</span>' : '<span class="badge badge-danger">No</span>' },
            { data: "estaActivo", render: data => data ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            {
                "data": "secuencial",
                "render": function (data, type, row) {
                    return `<div class="dropdown">` +
                           `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #004A93; border-color: #004A93;">` +
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
                title: 'Tipos de Impuesto',
                filename: 'Reporte Tipos de Impuesto',
                exportOptions: {
                    columns: [1, 2, 3, 4]
                },
                className: 'btn btn-link btn-sm p-1'
            }
        ],
        language: lenguajeEspanol,
        initComplete: function() {
            $("#btnNuevo").appendTo(".toolbar-left");
            $("#btnNuevo").closest(".row").show();
        }
    });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtSecuencial").val(modelo.secuencial);
    $("#txtNombre").val(modelo.nombre);
    $("#cboEsIva").val(modelo.esIva.toString());
    $("#cboEsImportacion").val(modelo.esImportacion.toString());
    $("#cboEstaActivo").val(modelo.estaActivo.toString());
    $("#modalData").modal("show");
}

$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal();
});

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;
    filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$("#btnGuardar").click(function () {
    const modelo = {
        secuencial: $("#txtSecuencial").val(),
        nombre: $("#txtNombre").val(),
        esIva: $("#cboEsIva").val() === "true",
        esImportacion: $("#cboEsImportacion").val() === "true",
        estaActivo: $("#cboEstaActivo").val() === "true"
    };

    if (!modelo.nombre || modelo.nombre.trim() === "") {
        toastr.warning("El campo Nombre es obligatorio.");
        return;
    }

    const url = esEdicion ? "Editar" : "Crear";
    const method = esEdicion ? "PUT" : "POST";
    const modalContent = $("#modalData .modal-content");

    modalContent.LoadingOverlay("show");

    fetch(url, { method: method, headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(modelo) })
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(responseJson => {
            modalContent.LoadingOverlay("hide");
            if (responseJson.estado) {
                if (esEdicion) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                } else {
                    tablaData.row.add(responseJson.objeto).draw(false);
                }
                $("#modalData").modal("hide");
                Swal.fire("Listo!", `Tipo de Impuesto ${esEdicion ? 'editado' : 'creado'} correctamente`, "success");
            } else {
                Swal.fire("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            manejarErrorFetch(error, "Guardar Tipo de Impuesto", modalContent);
        });
});

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(fila).data();

    Swal.fire({ /* ... */ }).then((result) => {
        if (result.isConfirmed) {
            const sweetAlertOverlay = $(".swal2-container");
            sweetAlertOverlay.LoadingOverlay("show");

            fetch(`Eliminar?id=${data.secuencial}`, { method: "DELETE" })
                .then(response => {
                    if (!response.ok) return response.json().then(err => Promise.reject(err));
                    return response.json();
                })
                .then(responseJson => {
                    sweetAlertOverlay.LoadingOverlay("hide");
                    if (responseJson.estado) {
                        tablaData.row(fila).remove().draw();
                        Swal.fire("Listo!", "El Tipo de Impuesto fue eliminado", "success");
                    } else {
                        Swal.fire("Fallo!", responseJson.mensajes, "error");
                    }
                })
                .catch(error => {
                    manejarErrorFetch(error, "Eliminar Tipo de Impuesto", sweetAlertOverlay);
                });
        }
    });
});
