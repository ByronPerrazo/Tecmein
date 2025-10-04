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
                return json.data.$values;
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
                    '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm mr-2"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: 'Contactos',
                filename: 'Reporte de Contactos',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
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
    if (listaConstructoras.length > 0) {
        listaConstructoras.forEach(item => {
            cboEmpresa.append(
                $("<option>").val(item.secuencial).text(item.nombre)
            )
        });
    }
    cboEmpresa.val(modelo.secConstructora);

    $("#modalData").modal("show")
};

let esEdicion = false;

$("#btnNuevoContacto").click(function () {
    limpiarModal();
    esEdicion = false;

    // Para un nuevo contacto, necesitamos la lista de constructoras
    fetch("/Contacto/EmpresaConstructora")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            mostrarModal(MODELO_CONTACTO, responseJson);
        })
        .catch(error => {
            console.error('Error al obtener la lista de constructoras para nuevo contacto:', error);
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

    const selects = document.querySelectorAll("select.input-validar");

    const selectsConValorDeshabilitado = Array.from(selects).filter(select => {
        const selectedOption = select.options[select.selectedIndex];
        return selectedOption.disabled && selectedOption.selected;
    });

    if (selectsConValorDeshabilitado.length > 0) {
        const mensaje = `Debe seleccionar una opción válida en : "${selectsConValorDeshabilitado[0].name}"`;
        toastr.warning("", mensaje);
        selectsConValorDeshabilitado[0].focus();
        return;
    }

    const modeloContacto = structuredClone(MODELO_CONTACTO);
    modeloContacto["secuencial"] = $("#txtId").val();
    modeloContacto["nombres"] = $("#txtNombres").val();
    modeloContacto["apellidos"] = $("#txtApellidos").val();
    modeloContacto["correo"] = $("#txtCorreo").val();
    modeloContacto["telefono"] = $("#txTelefono").val();
    modeloContacto["secConstructora"] = parseInt($("#cboEmpresa").val());
    modeloContacto["titulo"] = $("#titulo").val(); // Asumiendo que #titulo es el input para el título
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
        return response.ok ? response.json() : Promise.reject(response);
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
        console.error("Error al guardar:", error);
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
            return response.ok ? response.json() : Promise.reject(response);
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
            console.error("Error en la llamada fetch para editar:", error);
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
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw();
                    Swal.fire("Listo!", "El Contacto fue eliminado", "success");
                } else {
                    Swal.fire("Fallo!", responseJson.mensajes, "error");
                }
            });
        }
    });
});