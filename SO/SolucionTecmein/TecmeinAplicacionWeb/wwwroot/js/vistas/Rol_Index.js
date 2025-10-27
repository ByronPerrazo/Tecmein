const MODELO_BASE = {
    Secuencial: 0,
    Descripcion: "",
    EsActivo: 1
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

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": 'ListaRol',
            "type": "GET",
            "datatype": "json",
            "dataSrc": "data",
            "error": function (jqXHR) { manejarErrorFetch(jqXHR.responseJSON, "Cargar Roles"); }
        },
        "columns": [
            { data: "Secuencial", visible: false },
            { data: "Descripcion" },
            { data: "FechaRegistroString", width: "100px" },
            { data: "EsActivo", render: data => data == 1 ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            {
                "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button><a class="btn btn-info btn-sm" href="#"><i class="fas fa-user-shield"></i></a><button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button></div>',
                "orderable": false, "searchable": false, "width": "160px"
            }
        ],
        order: [[0, "asc"]],
        dom: "Bfrtip",
        buttons: ['excelHtml5', 'pageLength'],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" },
    });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.Secuencial);
    $("#txtDescripcionRol").val(modelo.Descripcion);
    $("#cboEstado").val(modelo.EsActivo);
    $("#modalData").modal("show");
}

$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal();
});

$("#btnGuardar").click(function () {
    if ($("#txtDescripcionRol").val().trim() === "") {
        toastr.warning("Debe llenar el campo Descripción");
        $("#txtDescripcionRol").focus();
        return;
    }

    const modelo = {
        Secuencial: $("#txtId").val(),
        Descripcion: $("#txtDescripcionRol").val(),
        EsActivo: $("#cboEstado").val(),
    };

    const datosFormulario = new FormData();
    datosFormulario.append("modelo", JSON.stringify(modelo));
    const modalContent = $("#modalData .modal-content");

    modalContent.LoadingOverlay("show");

    fetch("ProcesaGuardarRol", { method: "POST", body: datosFormulario })
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
                Swal.fire("Listo!", `Rol ${esEdicion ? 'editado' : 'creado'} correctamente`, "success");
            } else {
                Swal.fire("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            manejarErrorFetch(error, "Guardar Rol", modalContent);
        });
});

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;
    filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(filaSeleccionada).data();

    fetch(`RolPorSecuencial?secRol=${data.Secuencial}`)
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(data => {
            mostrarModal(data);
        })
        .catch(error => manejarErrorFetch(error, "Cargar Rol para Editar"));
});

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(fila).data();

    Swal.fire({ /* ... */ }).then((result) => {
        if (result.isConfirmed) {
            const sweetAlertOverlay = $(".swal2-container");
            sweetAlertOverlay.LoadingOverlay("show");

            fetch(`Eliminar?secuencial=${data.Secuencial}`, { method: "DELETE" })
                .then(response => {
                    if (!response.ok) return response.json().then(err => Promise.reject(err));
                    return response.json();
                })
                .then(responseJson => {
                    sweetAlertOverlay.LoadingOverlay("hide");
                    if (responseJson.estado) {
                        tablaData.row(fila).remove().draw(false);
                        Swal.fire("Listo!", `El rol "${data.Descripcion}" fue eliminado`, "success");
                    } else {
                        Swal.fire("Fallo!", responseJson.mensajes, "error");
                    }
                })
                .catch(error => {
                    manejarErrorFetch(error, "Eliminar Rol", sweetAlertOverlay);
                });
        }
    });
});

$("#tbdata tbody").on("click", ".btn-info", function () {
    const data = tablaData.row($(this).parents('tr')).data();
    window.location.href = `/Rol/GestionarPermisos?secRol=${data.Secuencial}`;
});