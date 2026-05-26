const MODELO_CONTACTO = {
    secuencial: 0,
    secConstructora: "",
    titulo: "",
    nombres: "",
    apellidos: "",
    telefono: "",
    correo: "",
    estaActivo: 1,
}

let tablaData;
let filaSeleccionada;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": 'Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                // Manejo de la respuesta del servidor que puede venir con $values
                return json.data && json.data.$values ? json.data.$values : json.data;
            }
        },
        "columns": [
            { data: "secuencial", visible: false, searchable: false },
            { data: "nombreConstructora", searchable: true },
            { data: "titulo", searchable: true },
            { data: "nombres", searchable: true },
            { data: "apellidos", searchable: true, width: "100px" },
            { data: "correo", searchable: true },
            { data: "telefono", searchable: true, width: "80px" },
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
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
        buttons: [
            {
                text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                extend: 'excelHtml5',
                title: 'Contactos',
                filename: 'Reporte de Contactos',
                exportOptions: { columns: [1, 2, 3, 4, 5, 6, 7] },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-file-pdf text-danger fa-lg"></i>',
                extend: 'pdfHtml5',
                title: 'Contactos',
                filename: 'Reporte de Contactos',
                exportOptions: { columns: [1, 2, 3, 4, 5, 6, 7] },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-print text-primary fa-lg"></i>',
                extend: 'print',
                title: 'Contactos',
                exportOptions: { columns: [1, 2, 3, 4, 5, 6, 7] },
                className: 'btn btn-link btn-sm p-1'
            }
        ],
        language: {
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
        },
        initComplete: function() {
            $("#btnNuevoContacto").appendTo(".toolbar-left");
            $("#btnNuevoContacto").closest(".row").show();
        }
    });

});

function limpiarModal() {
    $("#txtId").val(0);
    $("#cboEmpresa").val($("#cboEmpresa option:first").val());
    $("#titulo").val('');
    $("#txtNombres").val('');
    $("#txtApellidos").val('');
    $("#txTelefono").val('');
    $("#txtCorreo").val('');
    $("#cboEstado").val(1);
}

function mostrarModal(modelo = MODELO_CONTACTO, listaConstructoras = []) {
    limpiarModal();

    $("#txtId").val(modelo.secuencial);
    $("#titulo").val(modelo.titulo);
    $("#txtNombres").val(modelo.nombres);
    $("#txtApellidos").val(modelo.apellidos);
    $("#txTelefono").val(modelo.telefono);
    $("#txtCorreo").val(modelo.correo);
    $("#cboEstado").val(modelo.estaActivo);

    const cboEmpresa = $("#cboEmpresa");
    cboEmpresa.empty();
    cboEmpresa.append($("<option disabled selected>-- Seleccione una --</option>"));
    if (listaConstructoras.length > 0) {
        listaConstructoras.forEach(item => {
            cboEmpresa.append(
                $("<option>").val(item.secuencial).text(item.nombre)
            )
        });
    }
    if (modelo.secConstructora) {
        cboEmpresa.val(modelo.secConstructora);
    }

    $("#modalData").modal("show")
};

let esEdicion = false;

$("#btnNuevoContacto").click(function () {
    limpiarModal();
    esEdicion = false;

    fetch("/Contacto/EmpresaConstructora")
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorJson => Promise.reject(errorJson));
            }
            return response.json();
        })
        .then(responseJson => {
            mostrarModal(MODELO_CONTACTO, responseJson);
        })
        .catch(error => {
            if (error && error.mensajes) {
                Swal.fire("Fallo!", error.mensajes, "error");
            } else {
                console.error('Error al obtener la lista de constructoras:', error);
                Swal.fire("Fallo!", "Ocurrió un error al cargar los datos iniciales.", "error");
            }
        });
});

$("#btnGuardarContacto").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");

    if (inputs_vacios.length > 0) {
        const mensaje = `Debe llenar el campo: "${inputs_vacios[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    if ($('#cboEmpresa').val() === null || $('#cboEmpresa').val() === "") {
        toastr.warning("", "Debe seleccionar una empresa");
        return;
    }

    const modeloContacto = structuredClone(MODELO_CONTACTO);
    modeloContacto["secuencial"] = $("#txtId").val();
    modeloContacto["nombres"] = $("#txtNombres").val();
    modeloContacto["apellidos"] = $("#txtApellidos").val();
    modeloContacto["correo"] = $("#txtCorreo").val();
    modeloContacto["telefono"] = $("#txTelefono").val();
    modeloContacto["secConstructora"] = parseInt($("#cboEmpresa").val());
    modeloContacto["titulo"] = $("#titulo").val();
    modeloContacto["estaActivo"] = $("#cboEstado").val();

    const datosFormulario = new FormData();
    datosFormulario.append("modelo", JSON.stringify(modeloContacto));

    const url = esEdicion ? "Editar" : "CrearContacto";
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
            Swal.fire("Listo!", `Contacto ${esEdicion ? 'editado' : 'creado'} correctamente`, "success");
        } else {
            Swal.fire("Fallo!", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        if (error && error.mensajes) {
            Swal.fire("Fallo!", error.mensajes, "error");
        } else {
            console.error("Error al guardar:", error);
            Swal.fire("Fallo!", "Ocurrió un error inesperado al guardar.", "error");
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
    const secuencialContacto = data.secuencial;

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(`/Contacto/ObtenerParaEditar?secuencial=${secuencialContacto}`)
        .then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            if (!response.ok) {
                return response.json().then(errorJson => Promise.reject(errorJson));
            }
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                mostrarModal(responseJson.objeto.contacto, responseJson.objeto.listaConstructoras);
            } else {
                Swal.fire("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            if (error && error.mensajes) {
                Swal.fire("Fallo!", error.mensajes, "error");
            } else {
                console.error("Error en la llamada fetch para editar:", error);
                Swal.fire("Fallo!", "Ocurrió un error inesperado al cargar los datos.", "error");
            }
        });
});

$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "Está Seguro de Eliminar?",
        text: `Eliminar El Contacto "${data.nombres}"`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar"
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
                    tablaData.row(fila).remove().draw();
                    Swal.fire("Listo!", "El Contacto fue eliminado", "success");
                } else {
                    Swal.fire("Fallo!", responseJson.mensajes, "error");
                }
            })
            .catch(error => {
                $(".showSweetAlert").LoadingOverlay("hide");
                if (error && error.mensajes) {
                    Swal.fire("Fallo!", error.mensajes, "error");
                } else {
                    console.error("Error al eliminar:", error);
                    Swal.fire("Fallo!", "Ocurrió un error inesperado al eliminar.", "error");
                }
            });
        }
    });
});
