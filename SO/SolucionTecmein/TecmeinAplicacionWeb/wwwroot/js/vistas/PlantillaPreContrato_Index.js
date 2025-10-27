const MODELO_BASE = {
    secPlantillaPreContrato: 0,
    nombre: "",
    secTipoDocumento: 0,
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

function mostrarModal(modelo = MODELO_BASE, tiposDocumento = []) {
    $("#txtId").val(modelo.secPlantillaPreContrato);
    $("#txtNombre").val(modelo.nombre);
    $("#cboEstado").val(modelo.estaActivo ? "1" : "0");

    const cboTipoDocumento = $("#cboTipoDocumento");
    cboTipoDocumento.empty().append($("<option>").val("").text("-- Seleccione un Tipo de Documento --"));
    if (tiposDocumento.length > 0) {
        tiposDocumento.forEach(tipo => {
            cboTipoDocumento.append($("<option>").val(tipo.secuencial).text(tipo.descripcion));
        });
    }
    cboTipoDocumento.val(modelo.secTipoDocumento || "");
    $("#modalData").modal("show");
}

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/PlantillaPreContrato/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function(json) { return json.data && json.data.$values ? json.data.$values : json.data; },
            "error": function(jqXHR) { manejarErrorFetch(jqXHR.responseJSON, "Cargar Plantillas"); }
        },
        "columns": [
            { "data": "secPlantillaPreContrato", "visible": false },
            { "data": "nombre" },
            { "data": "nombreTipoDocumento" },
            { "data": "estaActivo", "render": data => data ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            {
                "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button><button class="btn btn-secondary btn-parrafos btn-sm"><i class="fas fa-paragraph"></i></button><button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button></div>',
                "orderable": false, "searchable": false, "width": "120px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: ['excelHtml5', 'pageLength'],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" }
    });

    function obtenerTiposDocumentoYMostrarModal(modelo = MODELO_BASE) {
        fetch('/TipoDocumento/ListaActivos')
            .then(response => {
                if (!response.ok) return response.json().then(err => Promise.reject(err));
                return response.json();
            })
            .then(responseJson => {
                const tiposDocumento = responseJson.data && responseJson.data.$values ? responseJson.data.$values : responseJson.data;
                mostrarModal(modelo, tiposDocumento);
            })
            .catch(error => manejarErrorFetch(error, "Cargar Tipos de Documento"));
    }

    $("#btnNuevo").click(() => obtenerTiposDocumentoYMostrarModal());

    $("#tbdata tbody").on("click", ".btn-editar", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(fila).data();
        obtenerTiposDocumentoYMostrarModal(data);
    });

    $("#btnGuardar").click(function () {
        const modelo = {
            secPlantillaPreContrato: parseInt($("#txtId").val()) || 0,
            nombre: $("#txtNombre").val(),
            secTipoDocumento: parseInt($("#cboTipoDocumento").val()),
            estaActivo: $("#cboEstado").val() == "1"
        };

        if (!modelo.nombre || modelo.nombre.trim() === "" || !modelo.secTipoDocumento) {
            toastr.warning("Por favor, complete todos los campos requeridos.", "Campos Incompletos");
            return;
        }

        const esNuevo = modelo.secPlantillaPreContrato === 0;
        const url = esNuevo ? '/PlantillaPreContrato/Crear' : '/PlantillaPreContrato/Editar';
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
                    Swal.fire('Listo!', `La plantilla fue ${esNuevo ? 'creada' : 'editada'} exitosamente.`, 'success');
                } else {
                    Swal.fire('Error', responseJson.mensajes, 'error');
                }
            })
            .catch(err => manejarErrorFetch(err, "Guardar Plantilla", modalContent));
    });

    $("#tbdata tbody").on("click", ".btn-eliminar", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(fila).data();

        Swal.fire({ /* ... */ }).then((result) => {
            if (result.isConfirmed) {
                const sweetAlertOverlay = $(".swal2-container");
                sweetAlertOverlay.LoadingOverlay("show");
                fetch(`/PlantillaPreContrato/Eliminar?id=${data.secPlantillaPreContrato}`, { method: "DELETE" })
                    .then(response => {
                        if (!response.ok) return response.json().then(err => Promise.reject(err));
                        return response.json();
                    })
                    .then(responseJson => {
                        sweetAlertOverlay.LoadingOverlay("hide");
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            Swal.fire('Listo!', 'La plantilla fue eliminada.', 'success');
                        } else {
                            Swal.fire('Error', responseJson.mensajes, 'error');
                        }
                    })
                    .catch(err => manejarErrorFetch(err, "Eliminar Plantilla", sweetAlertOverlay));
            }
        });
    });

    $("#tbdata tbody").on("click", ".btn-parrafos", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(fila).data();
        window.location.href = `/PlantillaPreContratoParrafo/Parrafos?secPlantillaPreContrato=${data.secPlantillaPreContrato}`;
    });
});
