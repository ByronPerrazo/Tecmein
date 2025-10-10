
const MODELO_BASE = {
    secuencial: 0,
    nombre: "",
    direccion: "",
    telefono: "",
    correo: "",
    atencion: "",
    administrador: "",
    telefonoAdministrador: "",
    correoAdministrador: "",
    estaActivo: 1,
}

$(document).ready(function () {
    debugger
    tablaData =
        $('#tbdata').DataTable({
            responsive: true,
            "ajax": {
                "url": 'Lista',
                "type": "GET",
                "datatype": "json",
                "dataSrc": function(json) {
                    // El endpoint de Constructora devuelve { data: { $values: [...] } }
                    if (json && json.data && json.data.$values) {
                        return json.data.$values;
                    }
                    return [];
                }
            },
            "columns": [
                { data: "secuencial", visible: false, searchable: false },
                { data: "nombre", searchable: true },
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
                        columns: [0, 1, 2]
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

let esEdicion;
$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal()
})


$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");

    inputs_vacios.forEach(x => {
        const mensaje = `Debe llenar el campo: "${x.name}"`;
        toastr.warning("", mensaje);
    });

    if (inputs_vacios.length > 0) {
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["secuencial"] = $("#txtId").val().trim();
    modelo["nombre"] = $("#txtNombre").val().trim();
    modelo["nombre"] = $("#txtNombre").val();
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

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (!esEdicion) {

        fetch("Crear", {
            method: "POST",
            body: datosFormulario
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok
                    ? response.json()
                    : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                } else {
                    Swal.fire("Fallo!", responseJson.mensajes, "error");
                }
            });
    } else {

        fetch("Editar", {
            method: "PUT",
            body: datosFormulario
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                return response.ok
                    ? response.json()
                    : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {
                    debugger;
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    Swal.fire("Listo!", "Constructora " + responseJson.objeto.nombre + " Editada ", "success");
                }
                else {
                    Swal.fire("Fallo!", responseJson.mensajes, "error");
                }
            });

    }

});

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    data.nombreImagen
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

    Swal.fire({
        title: "¿Está Seguro de Eliminar?",
        text: `Eliminar La Constructora "${data.nombre}"`,
        icon: "warning", // 'type' is deprecated, use 'icon'
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
                    return response.ok
                        ? response.json()
                        : Promise.reject(response);
                }).then(responseJson => {
                    debugger;
                    if (responseJson.estado) {
                        tablaData.row(fila).remove().draw(false);

                        Swal.fire("Listo!", " La Constructora " + data.nombre + " fue Eliminada", "success");
                    }
                    else {
                        Swal.fire("Fallo!", responseJson.mensajes, "error");
                    }
                });

        }
    })

})