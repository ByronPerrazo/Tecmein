const MODELO_BASE = {
    secTipoDocumento: 0,
    codigo: "",
    descripcion: "",
    estaActivo: true,
    secPlantilla: ""
}

let tablaData;

function manejarErrorFetch(error, operacion, overlayElement) {
    if (overlayElement) $(overlayElement).LoadingOverlay("hide");
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensaje) {
        Swal.fire("Error", error.mensaje, "error");
    } else if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secTipoDocumento);
    $("#txtCodigo").val(modelo.codigo);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboEstado").val(modelo.estaActivo ? "1" : "0");
    $("#cboPlantilla").val(modelo.secPlantilla || "");
    $("#modalData").modal("show");
}

$(document).ready(function () {

    // Cargar combo de Plantillas
    const urlPlantillas = `${window.location.origin}/PlantillaPreContrato/Lista`;
    fetch(urlPlantillas)
        .then(response => response.json())
        .then(data => {
            const plantillas = data.data && data.data.$values ? data.data.$values : data.data;
            if (plantillas) {
                const $cbo = $("#cboPlantilla");
                plantillas.forEach(item => {
                    // Mapeo correcto basado en el JSON del usuario
                    if (item.estaActivo === 1) { // Filtrar solo activos si es necesario
                        $cbo.append($("<option>").val(item.secPlantillaPreContrato).text(item.nombre));
                    }
                });
            }
        })
        .catch(error => console.error("Error al cargar plantillas:", error));

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/TipoDocumento/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) { return json.data && json.data.$values ? json.data.$values : json.data; },
            "error": function (jqXHR) { manejarErrorFetch(jqXHR.responseJSON, "Cargar Tipos de Documento"); }
        },
        "columns": [
            { "data": "secTipoDocumento", "visible": false },
            { "data": "codigo" },
            { "data": "descripcion" },
            { "data": "nombrePlantilla", "width": "20%" },
            { "data": "estaActivo", "render": data => data ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            { "data": "fechaRegistro" },
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
            secTipoDocumento: parseInt($("#txtId").val()) || 0,
            codigo: $("#txtCodigo").val(),
            descripcion: $("#txtDescripcion").val(),
            estaActivo: $("#cboEstado").val() == "1",
            secPlantilla: $("#cboPlantilla").val() ? parseInt($("#cboPlantilla").val()) : null
        };

        if (!modelo.descripcion || modelo.descripcion.trim() === "") {
            toastr.warning("Por favor, ingrese la descripción.", "Campo Requerido");
            return;
        }

        const esNuevo = modelo.secTipoDocumento === 0;
        const url = esNuevo ? '/TipoDocumento/Crear' : '/TipoDocumento/Editar';
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
                if (responseJson.resultado) {
                    tablaData.ajax.reload();
                    $("#modalData").modal("hide");
                    Swal.fire('Listo!', `El tipo de documento fue ${esNuevo ? 'creado' : 'editado'} exitosamente.`, 'success');
                } else {
                    Swal.fire('Error', responseJson.mensaje, 'error');
                }
            })
            .catch(err => manejarErrorFetch(err, "Guardar Tipo de Documento", modalContent));
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
                fetch(`/TipoDocumento/Eliminar?SecTipoDocumento=${data.secTipoDocumento}`, { method: "DELETE" })

                    .then(response => {
                        if (!response.ok) return response.json().then(err => Promise.reject(err));
                        return response.json();
                    })
                    .then(responseJson => {
                        sweetAlertOverlay.LoadingOverlay("hide");
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            Swal.fire('Listo!', 'El tipo de documento fue eliminado.', 'success');
                        } else {
                            Swal.fire('Error', responseJson.mensajes, 'error');
                        }
                    })
                    .catch(err => manejarErrorFetch(err, "Eliminar Tipo de Documento", sweetAlertOverlay));
            }
        });
    });
});
