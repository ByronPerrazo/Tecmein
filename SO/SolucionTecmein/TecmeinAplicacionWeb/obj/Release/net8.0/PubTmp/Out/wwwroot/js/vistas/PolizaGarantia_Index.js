const MODELO_BASE = {
    secuencial: 0,
    descripcion: "",
    estaActivo: 1
}

let tablaData;

function manejarErrorFetch(error, operacion, overlayElement) {
    if (overlayElement) $(overlayElement).LoadingOverlay("hide");
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboEstado").val(modelo.estaActivo ? "1" : "0");
    $("#modalData").modal("show");
}

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/PolizaGarantia/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function(json) { return json.data && json.data.$values ? json.data.$values : json.data; },
            "error": function(jqXHR) { manejarErrorFetch(jqXHR.responseJSON, "Cargar Pólizas"); }
        },
        "columns": [
            { "data": "secuencial", "visible": false },
            { "data": "descripcion" },
            { "data": "estaActivo", "render": data => data ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            { "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button><button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button></div>', "orderable": false, "searchable": false, "width": "80px" }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: ['excelHtml5', 'pageLength'],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" }
    });

    $("#btnNuevo").click(() => mostrarModal());

    $("#btnGuardar").click(function () {
        const modelo = {
            secuencial: parseInt($("#txtId").val()) || 0,
            descripcion: $("#txtDescripcion").val(),
            estaActivo: $("#cboEstado").val() == "1"
        };

        if (!modelo.descripcion || modelo.descripcion.trim() === "") {
            toastr.warning("Por favor, ingrese la descripción.", "Campo Requerido");
            return;
        }

        const esNuevo = modelo.secuencial === 0;
        const url = esNuevo ? '/PolizaGarantia/Crear' : '/PolizaGarantia/Editar';
        const method = esNuevo ? 'POST' : 'PUT';
        const modalContent = $("#modalData .modal-content");

        modalContent.LoadingOverlay("show");

        fetch(url, { method: method, headers: { "Content-Type": "application/json; charset=utf-8" }, body: JSON.stringify(modelo) })
            .then(response => {
                if (!response.ok) return response.json().then(err => Promise.reject(err));
                return response.json();
            })
            .then(responseJson => {
                modalContent.LoadingOverlay("hide");
                if (responseJson.estado) {
                    tablaData.ajax.reload();
                    $("#modalData").modal("hide");
                    Swal.fire('Listo!', `La póliza de garantía fue ${esNuevo ? 'creada' : 'editada'} exitosamente.`, 'success');
                } else {
                    Swal.fire('Error', responseJson.mensajes, 'error');
                }
            })
            .catch(err => manejarErrorFetch(err, "Guardar Póliza", modalContent));
    });

    $("#tbdata tbody").on("click", ".btn-editar", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(fila).data();
        mostrarModal(data);
    });

    $("#tbdata tbody").on("click", ".btn-eliminar", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(fila).data();

        Swal.fire({ /* ... */ }).then((result) => {
            if (result.isConfirmed) {
                const sweetAlertOverlay = $(".swal2-container");
                sweetAlertOverlay.LoadingOverlay("show");
                fetch(`/PolizaGarantia/Eliminar?id=${data.secuencial}`, { method: "DELETE" })
                    .then(response => {
                        if (!response.ok) return response.json().then(err => Promise.reject(err));
                        return response.json();
                    })
                    .then(responseJson => {
                        sweetAlertOverlay.LoadingOverlay("hide");
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            Swal.fire('Listo!', 'La póliza de garantía fue eliminada.', 'success');
                        } else {
                            Swal.fire('Error', responseJson.mensajes, 'error');
                        }
                    })
                    .catch(err => manejarErrorFetch(err, "Eliminar Póliza", sweetAlertOverlay));
            }
        });
    });
});
