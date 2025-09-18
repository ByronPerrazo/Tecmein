const MODELO_BASE = {
    secuencial: 0,
    descripcion: "",
    estaActivo: 1
}

let tablaData;

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
            "dataSrc": function(json) {
                return json.data.$values || json.data;
            }
        },
        "columns": [
            { "data": "secuencial", "visible": false, "searchable": false },
            { "data": "descripcion" },
            { "data": "estaActivo", "render": function (data) { return data == 1 ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>'; } },
            { "defaultContent": '<button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button><button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>', "orderable": false, "searchable": false, "width": "80px" }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [ { text: 'Exportar Excel', extend: 'excelHtml5', title: 'Reporte de Pólizas de Garantía', exportOptions: { columns: [1, 2] } }, 'pageLength' ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" }
    });

    $("#btnNuevo").click(function () {
        mostrarModal();
    });

    $("#btnGuardar").click(function () {
        const modelo = structuredClone(MODELO_BASE);
        modelo.secuencial = parseInt($("#txtId").val()) || 0;
        modelo.descripcion = $("#txtDescripcion").val();
        modelo.estaActivo = $("#cboEstado").val() == "1";

        if (!modelo.descripcion || modelo.descripcion.trim() === "") {
            toastr.warning("Por favor, ingrese la descripción.", "Campo Requerido");
            return;
        }

        const esNuevo = modelo.secuencial === 0;
        const url = esNuevo ? '/PolizaGarantia/Crear' : '/PolizaGarantia/Editar';
        const method = esNuevo ? 'POST' : 'PUT';

        $("#modalData .modal-content").LoadingOverlay("show");

        fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json; charset=utf-8",
            },
            data: JSON.stringify(modelo),
            body: JSON.stringify(modelo)
        })
        .then(response => {
            $("#modalData .modal-content").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                tablaData.ajax.reload();
                $("#modalData").modal("hide");
                Swal.fire('Listo!', `La póliza de garantía fue ${esNuevo ? 'creada' : 'editada'} exitosamente.`, 'success');
            } else {
                Swal.fire('Error', responseJson.mensajes, 'error');
            }
        })
        .catch(err => {
            $("#modalData .modal-content").LoadingOverlay("hide");
            Swal.fire('Error', 'No se pudo conectar con el servidor', 'error');
        });
    });

    $("#tbdata tbody").on("click", ".btn-editar", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(fila).data();
        mostrarModal(data);
    });

    $("#tbdata tbody").on("click", ".btn-eliminar", function () {
        const fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaData.row(fila).data();

        Swal.fire({
            title: '¿Está Seguro de Eliminar?',
            text: `Eliminar la póliza de garantía: "${data.descripcion}"`, 
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'No, cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $(".showSweetAlert").LoadingOverlay("show");
                fetch(`/PolizaGarantia/Eliminar?id=${data.secuencial}`, { method: "DELETE" })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw();
                            Swal.fire('Listo!', 'La póliza de garantía fue eliminada.', 'success');
                        } else {
                            Swal.fire('Error', responseJson.mensajes, 'error');
                        }
                    })
                    .catch(err => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        Swal.fire('Error', 'No se pudo conectar con el servidor.', 'error');
                    });
            }
        });
    });
});