const MODELO_BASE = {
    Secuencial: 0,
    Descripcion: "",
    FechaRegistro: "",
    EsActivo: 1
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
                "dataSrc": "data",
                "error": function (jqXHR, textStatus, errorThrown) {
                    // El manejador global en site.js ya se encarga de esto
                    // Pero si quieres un manejo específico aquí, puedes añadirlo.
                    // Por ahora, solo para evitar el error de DataTables si el global no lo atrapa.
                    if (jqXHR.status === 403) {
                        // No hacer nada, el site.js ya mostrará el Swal
                    } else {
                        // Manejo de otros errores si es necesario
                        console.error("Error AJAX en DataTables:", jqXHR.status, textStatus, errorThrown);
                    }
                }
            },
            "columns": [
                { data: "Secuencial", visible: false, searchable: false },
                { data: "Descripcion", searchable: true },
                { data: "FechaRegistroString", searchable: true, width: "100px" },
                {
                    data: "EsActivo", render: function (data) {
                        if (data == 1)
                            return '<span class="badge badge-info">Activo</span>';
                        else
                            return '<span class="badge badge-danger">Inactivo</span>';
                    }
                },

                {
                    "defaultContent":
                        '<div class="btn-group" role="group">' +
                        '<button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button>' +
                        '<a class="btn btn-info btn-sm" href="#"><i class="fas fa-user-shield"></i></a>' +
                        '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>' +
                        '</div>', // Botón Gestionar Permisos
                    "orderable": false,
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



}) // This closing brace was missing in the original string.
function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.Secuencial);
    $("#txtDescripcionRol").val(modelo.Descripcion);
    $("#cboEstado").val(modelo.EsActivo);

    // Lógica de permisos antigua ELIMINADA
    // if (modelo.oPermisosRol) {
    //     $("#checkConsultar").prop("checked", modelo.oPermisosRol.consultar == 1);
    //     $("#checkModificar").prop("checked", modelo.oPermisosRol.modificar == 1);
    //     $("#checkEliminar").prop("checked", modelo.oPermisosRol.eliminar == 1);
    // } else {
    //     $("#checkConsultar").prop("checked", false);
    //     $("#checkModificar").prop("checked", false);
    //     $("#checkEliminar").prop("checked", false);
    // }

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

    const modelo = {
        Secuencial: $("#txtId").val(),
        Descripcion: $("#txtDescripcionRol").val(),
        EsActivo: $("#cboEstado").val(),
        // oPermisosRol: {} // Lógica de permisos antigua ELIMINADA
    }

    const datosFormulario = new FormData();
    datosFormulario.append("modelo", JSON.stringify(modelo));

    const url = "ProcesaGuardarRol"; // El controlador ya no espera oPermisosRol
    const method = "POST";

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(url, {
        method: method,
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
                    if (esEdicion) {
                        tablaData.row(filaSeleccionada).data(respuestaJson.objeto).draw(false);
                    } else {
                        tablaData.row.add(respuestaJson.objeto).draw(false);
                    }
                    $("#modalData").modal("hide");
                    Swal.fire("Listo!", `Rol ${esEdicion ? 'editado' : 'creado'} correctamente`, "success");
                } else {
                    Swal.fire("Fallo!", respuestaJson.mensajes, "error");
                }
            }
        ).catch(error => {
            console.error('Error al Procesar Guardar Cambios:', error);
        });


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

    fetch(`RolPorSecuencial?secRol=${data.Secuencial}`)
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

    Swal.fire({
        title: "¿Está Seguro de Eliminar?",
        text: `Eliminar el rol "${data.Descripcion}"`, 
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`Eliminar?secuencial=${data.Secuencial}`, {
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

                        Swal.fire("Listo!", "El rol " + data.Descripcion + " Fue Eliminado", "success");
                    }
                    else {
                        Swal.fire("Fallo!", respuestaJson.mensajes, "error");
                    }
                });

        }
    });

})

// Manejador para el nuevo botón "Gestionar Permisos"
$("#tbdata tbody").on("click", ".btn-info", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaData.row(fila).data();
    window.location.href = `/Rol/GestionarPermisos?secRol=${data.Secuencial}`;
});
