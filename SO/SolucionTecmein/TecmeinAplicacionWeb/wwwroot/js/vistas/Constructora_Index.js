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
                    "defaultContent":
                        '<div class="dropdown">' +
                        '<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">' +
                        '<i class="fas fa-cog text-warning mr-1"></i> Acciones' +
                        '</button>' +
                        '<div class="dropdown-menu">' +
                        '<a class="dropdown-item btn-editar" href="#"><i class="fas fa-pencil-alt text-primary mr-2"></i> Editar</a>' +
                        '<a class="dropdown-item btn-eliminar" href="#"><i class="fas fa-trash-alt text-danger mr-2"></i> Eliminar</a>' +
                        '</div>' +
                        '</div>',
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
                    title: 'Constructoras',
                    filename: 'Reporte Constructoras Registradas',
                    className: 'btn btn-link btn-sm p-1',
                    exportOptions: { columns: [1, 2, 3, 4, 5, 6] }
                },
                {
                    text: '<i class="fas fa-file-pdf text-danger fa-lg"></i>',
                    extend: 'pdfHtml5',
                    title: 'Constructoras',
                    filename: 'Reporte Constructoras Registradas',
                    className: 'btn btn-link btn-sm p-1',
                    exportOptions: { columns: [1, 2, 3, 4, 5, 6] }
                },
                {
                    text: '<i class="fas fa-print text-primary fa-lg"></i>',
                    extend: 'print',
                    title: 'Constructoras',
                    className: 'btn btn-link btn-sm p-1',
                    exportOptions: { columns: [1, 2, 3, 4, 5, 6] }
                }
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json",
                search: "",
                searchPlaceholder: "Buscar...",
                lengthMenu: "Mostrar _MENU_"
            },
            initComplete: function() {
                $("#btnNuevo").appendTo(".toolbar-left");
            }
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