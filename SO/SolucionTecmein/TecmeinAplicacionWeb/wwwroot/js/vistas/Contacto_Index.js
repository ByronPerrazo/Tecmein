const MODELO_CONTACTO = {
    secuencial: "",
    secConstructora: "",
    nombreConstructora: "",
    titulo: "",
    nombres: "",
    apellidos: "",
    telefono: "",
    correo: "",
    estaActivo: 1,
}

$(document).ready( function () {

    fetch("EmpresaConstructora")
                .then(
                    respuesta => {
                        return respuesta.ok
                            ? respuesta.json()
                            : Promise.reject(respuesta);
                    }
                )
                .then(
                    respuestaJson => {
                        listaCompletaCanton = respuestaJson;
                        respuestaJson
                            .forEach(item => {
                                $("#cboEmpresa")
                                    .append(
                                        $("<option>")
                                            .val(item.secuencial)
                                            .text(item.nombre.trim())
                                    )
                            })

                    }
                )
                .catch(error => {
                    console.error('Error al obtener la lista de Empresas Contructoras:', error);
                });


    tablaData =
        $('#tbdata').DataTable({
            responsive: true,
            "ajax": {
                "url": 'Lista',
                "type": "GET",
                "datatype": "json"
            },
            "columns": [
                { data: "secuencial", visible: false },
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
                    title: 'Productos',
                    filename: 'Reporte de Productos',
                    exportOptions: {
                        columns: [0, 2, 3, 4, 5, 6]
                    }
                }, 'pageLength'
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });


});


function limpiarModal() {
    $("#txtId").val('');
    $("#secConstructora").val(0);
    $("#nombreConstructora").val('');
    $("#cboEmpresa").val($("#cboEmpresa option:first").val());
    $("#titulo").val('');
    $("#txtNombres").val('');
    $("#txtApellidos").val('');
    $("#txTelefono").val('');
    $("#txtCorreo").val('');
    $("#cboEstado").val(1);
}

function mostrarModal(modelo = MODELO_CONTACTO) {
    limpiarModal();

    $("#txtId").val(modelo.secuencial);
   
    
    $("#cboEmpresa").val(modelo.secConstructora == "" ? $("#cboEmpresa option:first").val() : modelo.secConstructora)

    $("#titulo").val(modelo.titulo );
    $("#txtNombres").val(modelo.nombres );
    $("#txtApellidos").val(modelo.apellidos);
    $("#txTelefono").val(modelo.telefono);
    $("#txtCorreo").val(modelo.correo);
    $("#cboEstado").val(modelo.estaActivo);

    $("#modalData").modal("show")
};



let esEdicion;
$("#btnNuevoContacto").click(function () {
    limpiarModal();
    esEdicion = false;

    mostrarModal(MODELO_CONTACTO)
})
$("#btnGuardarContacto").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");

    inputs_vacios.forEach(x => {
        const mensaje = `Debe llenar el campo: "${x.name}"\n`;
        toastr.warning("", mensaje);
        return;
    });

    if (inputs_vacios.length > 0) {
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    const selects = document.querySelectorAll("select.input-validar");

    const selectsConValorDeshabilitado = Array.from(selects).filter(select => {
        const selectedOption = select.options[select.selectedIndex];
        return selectedOption.disabled && selectedOption.selected;
    });

    selectsConValorDeshabilitado.map(select => {
        const mensaje = `Debe seleccionar una opción valida en : "${select.name}"\n`;
        toastr.warning("", mensaje);
        selectsConValorDeshabilitado[0].focus();
        return;
    });
    
    let secuencialVisita = $("#txtId").val().trim() == "" ? "0" : $("#txtId").val().trim();

    const modeloVisita = structuredClone(MODELO_CONTACTO);
          modeloVisita["secuencial"] = secuencialVisita;
          modeloVisita["nombres"] = $("#txtNombres").val().trim();
          modeloVisita["apellidos"] = $("#txtApellidos").val();
          modeloVisita["correo"] = $("#txtCorreo").val();
          modeloVisita["telefono"] = $("#txTelefono").val();
          modeloVisita["secConstructora"] = parseInt($("#cboEmpresa").val());
          modeloVisita["titulo"] = $("#cboAbreviatura").val();
          modeloVisita["estaActivo"] = $("#cboEstado").val();

    const datosFormulario = new FormData();
    datosFormulario.append("modelo", JSON.stringify(modeloVisita));


    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (!esEdicion) {

        fetch("CrearContacto", {
            method: "POST",
            body: datosFormulario
        })
            .then(response => {
                $("#modalData")
                    .find("div.modal-content")
                    .LoadingOverlay("hide");
                return response.ok
                    ? response.json()
                    : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!",
                        "Contacto " + responseJson.objeto.nombres + " Creado ",
                        "success");
                }
                else {
                    swal("Fallo!", responseJson.mensajes, "error");
                }
            });
    }
    else {

        fetch("Editar", {
            method: "PUT",
            body: datosFormulario
        })
            .then(response => {
                $("#modalData")
                    .find("div.modal-content")
                    .LoadingOverlay("hide");
                return response.ok
                    ? response.json()
                    : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {

                    tablaData
                        .row(filaSeleccionada)
                        .data(responseJson.objeto)
                        .draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!",
                        "Contacto " + responseJson.objeto.nombres + " Editado ",
                        "success");
                }
                else {
                    swal("Fallo!", responseJson.mensajes, "error");
                }
            });

    }

});

let filaSeleccionada;
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

    let fila
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    swal({
        title: "Está Seguro de Eliminar?",
        text: `Eliminar El Contacto "${data.nombres}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`Eliminar?secuencial=${data.secuencial}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok
                            ? response.json()
                            : Promise.reject(response);
                    }).then(responseJson => {
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw(false);

                            swal("Listo!", " El Contacto " + data.nombres + " fue Eliminado", "success");
                        }
                        else {
                            swal("Fallo!", responseJson.mensajes, "error");
                        }
                    });
            }
        }
    )
})

