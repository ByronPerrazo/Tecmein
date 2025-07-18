const MODELO_BASE = {
    secuencial: 0,
    descripcion: "",
    FechaRegistro: "",
    esActivo: 1
}

let tablaData;

$(document).ready(function () {

    // Listener de diagnóstico para el modal
    $('#modalAsignarMenus').on('shown.bs.modal', function () {
        console.log('Modal de asignación de menús reportado como visible por Bootstrap.');
    });

    tablaData =
        $('#tbdata').DataTable({
            responsive: true,
            "ajax": {
                "url": 'ListaRol',
                "type": "GET",
                "datatype": "json",
                "dataSrc": "data.$values",
            },
            "columns": [
                { data: "secuencial", visible: false },
                { data: "descripcion", searchable: true },
                { data: "fechaRegistro", searchable: true, width: "100px" },
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
                        '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                        '<button class="btn btn-danger btn-eliminar btn-sm mr-2"><i class="fas fa-trash-alt"></i></button>',
                    "orderable": true,
                    "searchable": false,
                    "width": "160px"
                }
            ],
            order: [[0, "asc"]],
            dom: "Bfrtip",
            buttons: [
                {
                    text: 'Exportar Excel',
                    extend: 'excelHtml5',
                    title: 'Productos',
                    filename: 'Reporte de Productos',
                    exportOptions: {
                        columns: [0, 1]
                    }
                }, 'pageLength'
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });



})
function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial);
    $("#txtDescripcionRol").val(modelo.descripcion);
    $("#cboEstado").val(modelo.esActivo);

    if (modelo.oPermisosRol) {
        $("#checkConsultar").prop("checked", modelo.oPermisosRol.consultar == 1);
        $("#checkModificar").prop("checked", modelo.oPermisosRol.modificar == 1);
        $("#checkEliminar").prop("checked", modelo.oPermisosRol.eliminar == 1);
    } else {
        $("#checkConsultar").prop("checked", false);
        $("#checkModificar").prop("checked", false);
        $("#checkEliminar").prop("checked", false);
    }

    $("#modalData").modal("show")
}

let esEdicion;
$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal();
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

    const modelo = {
        secuencial: $("#txtId").val(),
        descripcion: $("#txtDescripcionRol").val(),
        esActivo: $("#cboEstado").val(),
        oPermisosRol: {
            consultar: $("#checkConsultar").is(":checked") ? 1 : 0,
            modificar: $("#checkModificar").is(":checked") ? 1 : 0,
            eliminar: $("#checkEliminar").is(":checked") ? 1 : 0
        }
    }

    const datosFormulario = new FormData();
    datosFormulario.append("modelo", JSON.stringify(modelo));

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (!esEdicion) {

        fetch("ProcesaGuardarRol", {
            method: "POST",
            body: datosFormulario
        })

            .then(
                respuesta => {
                    $("#modalData").find("div.modal-content").LoadingOverlay("hide");
                    return respuesta.ok
                        ? respuesta.json()
                        : Promise.reject(respuesta);
                }
            ).then(
                respuestaJson => {
                    if (respuestaJson.estado) {
                        tablaData.row.add(respuestaJson.objeto).draw(false);
                        $("#modalData").modal("hide");
                        swal("Listo!", "Información Guardada con Éxito", "success");
                    } else {
                        swal("Fallo!", respuestaJson.mensajes, "error");
                    }
                }
            ).catch(error => {
                console.error('Error al Procesar Guardar Cambios:', error);
            });


    } else {

        fetch("ProcesaGuardarRol", {
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
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!", "Rol " + responseJson.objeto.descripcion + " Editado ", "success");
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

    fetch(`RolPorSecuencial?secRol=${data.secuencial}`)
        .then(response => response.json())
        .then(data => {
            mostrarModal(data);
        });

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
        title: "¿Está Seguro de Eliminar?",
        text: `Eliminar el rol "${data.descripcion}"`,
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

                            swal("Listo!", "El rol " + data.descripcion + " Fue Eliminado", "success");
                        }
                        else {
                            swal("Fallo!", responseJson.mensajes, "error");
                        }
                    });

            }
        }

    )

})

