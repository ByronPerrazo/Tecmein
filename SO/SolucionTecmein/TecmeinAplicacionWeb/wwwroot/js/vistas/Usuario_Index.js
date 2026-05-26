const MODELO_BASE = {
    secuencial: "",
    nombre: "",
    correo: "",
    telefono: "",
    secRol: 0,
    esActivo: 1,
    urlFoto: ""
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
                // Asegurarse de que el serializador no cause problemas
                return json.data && json.data.$values ? json.data.$values : json.data;
            }
        },
        "columns": [
            { data: "secuencial", visible: false, searchable: false },
            {
                data: 'urlFoto', render: function (data) {
                    const imageUrl = data && data.trim() !== '' ? data : 'https://via.placeholder.com/60';
                    return `<img style="height:60px" src="${imageUrl}" class="rounded mx-auto d-block"/>`;
                }
            },
            { data: "nombre" },
            { data: "correo" },
            { data: "telefono" },
            { data: "nombreRol" },
            {
                data: "esActivo", render: function (data) {
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
                title: 'Usuarios',
                filename: 'Reporte Usuarios',
                exportOptions: { columns: [0, 2, 3, 4, 5, 6] },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-file-pdf text-danger fa-lg"></i>',
                extend: 'pdfHtml5',
                title: 'Usuarios',
                filename: 'Reporte Usuarios',
                exportOptions: { columns: [0, 2, 3, 4, 5, 6] },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-print text-primary fa-lg"></i>',
                extend: 'print',
                title: 'Usuarios',
                exportOptions: { columns: [0, 2, 3, 4, 5, 6] },
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
            $("#btnNuevo").appendTo(".toolbar-left");
            $("#btnNuevo").closest(".row").show();
        }
    });

});

function mostrarModal(modelo = MODELO_BASE, listaRoles = []) {
    $("#txtId").val(modelo.secuencial)
    $("#txtNombre").val(modelo.nombre)
    $("#txtCorreo").val(modelo.correo)
    $("#txtTelefono").val(modelo.telefono)
    $("#cboEstado").val(modelo.esActivo)
    $("#txtFoto").val("")
    $("#imgUsuario").attr("src", modelo.urlFoto)

    const cboRol = $("#cboRol");
    cboRol.empty();
    if (listaRoles.length > 0) {
        listaRoles.forEach(item => {
            cboRol.append(
                $("<option>").val(item.secuencial).text(item.descripcion)
            )
        });
    }
    cboRol.val(modelo.secRol);

    $("#modalData").modal("show")
};

let esEdicion = false;

$("#btnNuevo").click(function () {
    esEdicion = false;
    fetch("/Usuario/ListaRol")
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorJson => Promise.reject(errorJson));
            }
            return response.json();
        })
        .then(responseJson => {
            mostrarModal(MODELO_BASE, responseJson.$values);
        })
        .catch(error => {
            if (error && error.mensajes) {
                Swal.fire("Fallo!", error.mensajes, "error");
            } else {
                console.error('Error al obtener la lista de roles:', error);
                Swal.fire("Fallo!", "Ocurrió un error al cargar los roles.", "error");
            }
        });
})

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    const secuencialUsuario = data.secuencial;

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(`/Usuario/ObtenerParaEditar?secuencialUsuario=${secuencialUsuario}`)
        .then(response => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            if (!response.ok) {
                return response.json().then(errorJson => Promise.reject(errorJson));
            }
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                mostrarModal(responseJson.objeto.usuario, responseJson.objeto.listaRoles);
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
    modelo["secuencial"] = parseInt($("#txtId").val()) || 0;
    modelo["nombre"] = $("#txtNombre").val();
    modelo["correo"] = $("#txtCorreo").val();
    modelo["telefono"] = $("#txtTelefono").val();
    modelo["secRol"] = $("#cboRol").val();
    modelo["esActivo"] = $("#cboEstado").val();

    const inputImagen = document.getElementById("txtFoto");
    const datosFormulario = new FormData();

    if (inputImagen.files && inputImagen.files[0]) {
        const imageKey = esEdicion ? "Foto" : "imagen";
        datosFormulario.append(imageKey, inputImagen.files[0]);
    }
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
            return response.json().then(errorJson => {
                return Promise.reject(errorJson);
            });
        }
        return response.json();
    })
    .then(responseJson => {
        if (responseJson.estado) {
            if(esEdicion) {
                tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
            } else {
                tablaData.row.add(responseJson.objeto).draw(false);
            }
            $("#modalData").modal("hide");
            Swal.fire("Listo!", `Usuario ${esEdicion ? 'editado' : 'creado'} correctamente`, "success");
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
            Swal.fire("Fallo!", "Ocurrió un error inesperado.", "error");
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
        text: `Eliminar el usuario "${data.nombre}"`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`Eliminar?secuencialUsuario=${data.secuencial}`, {
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
                    Swal.fire("Listo!", "El Usuario fue eliminado", "success");
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

// Lógica para la vista previa de la imagen
$("#txtFoto").change(function() {
    const input = this;
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
            $('#imgUsuario').attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
});
