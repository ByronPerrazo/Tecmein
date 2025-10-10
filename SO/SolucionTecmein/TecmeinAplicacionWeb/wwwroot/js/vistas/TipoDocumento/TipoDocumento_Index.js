const MODELO_BASE = {
    secTipoDocumento: 0,
    codigo: "",
    descripcion: "",
    estaActivo: 1,
    fechaRegistro: ""
}

let tablaData;
let filaSeleccionada;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": "/TipoDocumento/Lista",
            "type": "GET",
            "datatype": "json",
            "dataSrc": function(json) { return json.data ? json.data.$values : []; },
        },
        "columns": [
            { "data": "secTipoDocumento", "visible": false, "searchable": false },
            { "data": "codigo" },
            { "data": "descripcion" },
            {
                "data": "estaActivo", "render": function (valor) {
                    if (valor == 1) {
                        return '<span class="badge badge-success">Activo</span>'
                    } else {
                        return '<span class="badge badge-danger">Inactivo</span>'
                    }
                }
            },
            { "data": "fechaRegistro" },
            {
                "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button></div>',
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
                title: 'Tipos de Documento',
                filename: 'Reporte Tipos de Documento',
                exportOptions: {
                    columns: [1, 2, 3, 4]
                }
            },
            {
                text: 'Exportar PDF',
                extend: 'pdfHtml5',
                title: 'Tipos de Documento',
                filename: 'Reporte Tipos de Documento',
                exportOptions: {
                    columns: [1, 2, 3, 4]
                },
                customize: function (doc) {
                    doc.content[1].table.widths = ['25%', '25%', '25%', '25%']
                }
            },
            'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    $("#btnNuevo").click(function () {
        mostrarModal();
    });

    $("#btnGuardar").click(function () {
        GuardarCambios();
    });

    $('#tbdata tbody').on('click', '.btn-editar', function () {
        if ($(this).closest("tr").hasClass("child")) {
            filaSeleccionada = $(this).closest("tr").prev();
        } else {
            filaSeleccionada = $(this).closest("tr");
        }
        const data = tablaData.row(filaSeleccionada).data();
        mostrarModal(data);
    });

    $('#tbdata tbody').on('click', '.btn-eliminar', function () {
        if ($(this).closest("tr").hasClass("child")) {
            filaSeleccionada = $(this).closest("tr").prev();
        } else {
            filaSeleccionada = $(this).closest("tr");
        }
        const data = tablaData.row(filaSeleccionada).data();
        Eliminar(data.secTipoDocumento);
    });

});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secTipoDocumento);
    $("#txtCodigo").val(modelo.codigo);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboEstado").val(modelo.estaActivo ? "1" : "0");

    $("#modalData").modal("show");
}

function GuardarCambios() {
    const inputs = $("input.input-validar");
    let inputs_vacios = [];

    inputs.each(function() {
        if ($(this).val().trim() === "") {
            inputs_vacios.push($(this));
        }
    });

    if (inputs_vacios.length > 0) {
        const mensaje = `Debe llenar el campo: "${inputs_vacios[0].attr("name")}"`;
        toastr.warning("", mensaje);
        inputs_vacios[0].focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo.secTipoDocumento = parseInt($("#txtId").val());
    modelo.codigo = $("#txtCodigo").val();
    modelo.descripcion = $("#txtDescripcion").val();
    modelo.estaActivo = $("#cboEstado").val() === "1";

    const url = modelo.secTipoDocumento == 0 ? "/TipoDocumento/Crear" : "/TipoDocumento/Editar";
    const method = modelo.secTipoDocumento == 0 ? "POST" : "PUT";

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(url, {
        method: method,
        headers: {
            "Content-Type": "application/json; charset=utf-8",
        },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        if (responseJson.resultado) {
            if (modelo.secTipoDocumento == 0) {
                tablaData.ajax.reload(); // Reload all data for new entry
            } else {
                // For edit, update only the row if the backend returns the updated object
                // Assuming backend returns the updated object, otherwise reload all
                tablaData.ajax.reload(); 
            }
            $("#modalData").modal("hide");
            Swal.fire("¡Guardado!", "Los cambios han sido guardados correctamente.", "success");
        } else {
            Swal.fire("Error", "No se pudieron guardar los cambios.", "error");
        }
    })
    .catch(error => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        console.error("Error al guardar:", error);
        Swal.fire("Error", "Ocurrió un error al intentar guardar los cambios.", "error");
    });
}

function Eliminar(secTipoDocumento) {
    Swal.fire({
        title: "¿Está seguro de eliminar este Tipo de Documento?",
        text: "Una vez eliminado, no podrá recuperarse.",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "No, cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            $(".showSweetAlert").LoadingOverlay("show");
            fetch(`/TipoDocumento/Eliminar?SecTipoDocumento=${secTipoDocumento}`, {
                method: "DELETE"
            })
            .then(response => {
                $(".showSweetAlert").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.resultado) {
                    tablaData.row(filaSeleccionada).remove().draw();
                    Swal.fire("¡Eliminado!", "El Tipo de Documento ha sido eliminado correctamente.", "success");
                } else {
                    Swal.fire("Error", "No se pudo eliminar el Tipo de Documento.", "error");
                }
            })
            .catch(error => {
                $(".showSweetAlert").LoadingOverlay("hide");
                console.error("Error al eliminar:", error);
                Swal.fire("Error", "Ocurrió un error al intentar eliminar el Tipo de Documento.", "error");
            });
        }
    });
}