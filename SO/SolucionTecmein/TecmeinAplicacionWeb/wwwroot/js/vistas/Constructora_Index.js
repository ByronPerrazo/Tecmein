const MODELO_BASE = {
    secuencial: 0,
    nombre: "",
    ruc: "",
    direccion: "",
    telefono: "",
    correo: "",
    atencion: "",
    administrador: "",
    telefonoAdministrador: "",
    correoAdministrador: "",
    estaActivo: 1,
}

let tablaData;
let filaSeleccionada;

$(document).ready(function () {
    tablaData =
        $('#tbdata').DataTable({
            responsive: true,
            "ajax": {
                "url": 'Lista',
                "type": "GET",
                "datatype": "json",
                "dataSrc": function(json) {
                    if (json && json.data && json.data.$values) {
                        return json.data.$values;
                    }
                    return [];
                }
            },
            "columns": [
                { data: "secuencial", visible: false, searchable: false },
                { data: "nombre", searchable: true },
                { data: "ruc", searchable: true },
                { data: "direccion", searchable: true },
                { data: "atencion", searchable: true },
                { data: "administrador", searchable: true },
                { data: "telefonoAdministrador", searchable: true },
                { data: "correoAdministrador", searchable: true },
                {
                    data: "estaActivo", render: function (data) {
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
                    title: 'Constructoras',
                    filename: 'Reporte Constructoras Registradas',
                    exportOptions: {
                        columns: [1, 2, 3, 4, 5, 6]
                    }
                }, 'pageLength'
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial)
    $("#txtNombre").val(modelo.nombre)
    $("#txtRuc").val(modelo.ruc)
    $("#txtDireccion").val(modelo.direccion)
    $("#txtAtencion").val(modelo.atencion)
    $("#txtTelefono").val(modelo.telefono)
    $("#txtCorreo").val(modelo.correo)
    $("#txtAdministrador").val(modelo.administrador)
    $("#txtTelefonoAdmin").val(modelo.telefonoAdministrador)
    $("#txtCorreoAdmin").val(modelo.correoAdministrador)
    $("#cboEstado").val(modelo.estaActivo)
    $("#modalData").modal("show")
};

let esEdicion = false;

$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal()
})

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");

    if (inputs_vacios.length > 0) {
        const mensaje = `Debe llenar el campo: "${inputs_vacios[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["secuencial"] = $("#txtId").val();
    modelo["nombre"] = $("#txtNombre").val();
    modelo["ruc"] = $("#txtRuc").val();
    modelo["direccion"] = $("#txtDireccion").val();
    modelo["atencion"] = $("#txtAtencion").val();
    modelo["telefono"] = $("#txtTelefono").val();
    modelo["correo"] = $("#txtCorreo").val();
    modelo["administrador"]  = $("#txtAdministrador").val()
    modelo["telefonoAdministrador"] = $("#txtTelefonoAdmin").val()
    modelo["correoAdministrador"] = $("#txtCorreoAdmin").val()
    modelo["estaActivo"] = $("#cboEstado").val();

    const datosFormulario = new FormData();
    datosFormulario.append("modelo", JSON.stringify(modelo));

    const url = esEdicion ? "Editar" : "Crear";
    const method = esEdicion ? "PUT" : "POST";

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(url, {
        method: method,
        body: datosFormulario
    })
    .then(response => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        if (!response.ok) {
            return response.json().then(errorJson => Promise.reject(errorJson));
        }
        return response.json();
    })
    .then(responseJson => {
        if (responseJson.estado) {
            if (esEdicion) {
                tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
            } else {
                tablaData.row.add(responseJson.objeto).draw(false);
            }
            $("#modalData").modal("hide");
            Swal.fire("Listo!", `Constructora ${esEdicion ? 'editada' : 'creada'} correctamente.`, "success");
        } else {
            Swal.fire("Fallo!", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        if (error && error.mensajes) {
            Swal.fire("Fallo!", error.mensajes, "error");
        } else {
            console.error('Error al guardar:', error);
            Swal.fire("Fallo!", "Ocurrió un error inesperado.", "error");
        }
    });
});

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
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
        title: "¿Está Seguro de Eliminar?",
        text: `Eliminar La Constructora "${data.nombre}"`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`Eliminar?secuencial=${data.secuencial}`, {
                method: "DELETE"
            })
            .then(response => {
                $(".showSweetAlert").LoadingOverlay("hide");
                if (!response.ok) {
                    return response.json().then(errorJson => Promise.reject(errorJson));
                }
                return response.json();
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw(false);
                    Swal.fire("Listo!", `La Constructora "${data.nombre}" fue eliminada.`, "success");
                } else {
                    Swal.fire("Fallo!", responseJson.mensajes, "error");
                }
            })
            .catch(error => {
                $(".showSweetAlert").LoadingOverlay("hide");
                if (error && error.mensajes) {
                    Swal.fire("Fallo!", error.mensajes, "error");
                } else {
                    console.error('Error al eliminar:', error);
                    Swal.fire("Fallo!", "Ocurrió un error inesperado al eliminar.", "error");
                }
            });
        }
    });
});