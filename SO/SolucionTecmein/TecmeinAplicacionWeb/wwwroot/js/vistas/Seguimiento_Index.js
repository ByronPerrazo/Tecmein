let tablaSeguimiento;

// Using a more specific name to avoid conflicts if another script has the same function name.
function manejarErrorFetchSeguimiento(error, operacion) {
    console.error(`Error en ${operacion}:`, error);
    const mensaje = (error && error.mensajes) ? error.mensajes : `Ocurrió un error inesperado durante: ${operacion}.`;
    Swal.fire("Error", mensaje, "error");
}

function mostrarHistorialSeguimiento(idCotizacion) {
    $('#hiddenCotizacionId').val(idCotizacion);
    $('#modalSeguimiento').modal('show');

    const ajaxUrl = `/Seguimiento/Lista?secCotizacion=${idCotizacion}`;

    if ($.fn.DataTable.isDataTable('#tbSeguimiento')) {
        tablaSeguimiento.ajax.url(ajaxUrl).load();
    } else {
        tablaSeguimiento = $('#tbSeguimiento').DataTable({
            ajax: {
                url: ajaxUrl,
                type: "GET",
                datatype: "json",
                dataSrc: function (json) {
                    return json.data.$values || json.data;
                }
            },
            "columns": [
                { data: "accion" },
                { data: "detalle" },
                {
                    data: "fechaAccion",
                    render: function (data) {
                        if (data) {
                            const date = new Date(data);
                            const day = String(date.getDate()).padStart(2, '0');
                            const month = String(date.getMonth() + 1).padStart(2, '0'); // Month is 0-indexed
                            const year = date.getFullYear();
                            return `${day}/${month}/${year}`;
                        }
                        return '';
                    }
                },
                { data: "aceptacionCliente", render: function (data) { return data ? '<span class="badge badge-success">Sí</span>' : '<span class="badge badge-danger">No</span>'; } },
                {
                    "defaultContent":
                        '<div class="btn-group" role="group">' +
                        '<button class="btn btn-primary btn-editar-seguimiento btn-sm" title="Editar Seguimiento"><i class="fas fa-pencil-alt"></i></button>' +
                        '<button class="btn btn-danger btn-eliminar-seguimiento btn-sm" title="Eliminar Seguimiento"><i class="fas fa-trash-alt"></i></button>' +
                        '</div>',
                    "orderable": false,
                    "searchable": false,
                    "width": "100px"
                },
                { data: "secSeguimiento", visible: false } // Moved to the end
            ],
            order: [[2, "desc"]], // Order by date descending (index is now 2)
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });
    }
}

function limpiarModalSeguimiento() {
    $("#txtIdSeguimiento").val("0");
    $("#txtAccion").val("");
    $("#txtDetalle").val("");
    const today = new Date();
    const todayString = today.getFullYear() + '-' + String(today.getMonth() + 1).padStart(2, '0') + '-' + String(today.getDate()).padStart(2, '0');
    $("#txtFechaAccion").val(todayString);
    $("#chkAceptacionCliente").prop("checked", false);
}

function procederConGuardado(modelo) {
    const esNuevo = modelo.secSeguimiento === 0;
    const url = esNuevo ? '/Seguimiento/Crear' : '/Seguimiento/Editar';
    const method = esNuevo ? 'POST' : 'PUT';

    const formData = new FormData();
    formData.append('modelo', JSON.stringify(modelo));

    $("#modalDataSeguimiento .modal-content").LoadingOverlay("show");

    fetch(url, {
        method: method,
        body: formData
    })
    .then(response => {
        $("#modalDataSeguimiento .modal-content").LoadingOverlay("hide");
        if (!response.ok) return response.json().then(err => Promise.reject(err));
        return response.json();
    })
    .then(responseJson => {
        if (responseJson.estado) {
            $('#modalDataSeguimiento').modal('hide');
            Swal.fire('Listo!', `El seguimiento fue ${esNuevo ? 'creado' : 'editado'} exitosamente.`, 'success');
            tablaSeguimiento.ajax.reload();
        } else {
            Swal.fire('Error', responseJson.mensajes, 'error');
        }
    })
    .catch(err => {
        $("#modalDataSeguimiento .modal-content").LoadingOverlay("hide");
        manejarErrorFetchSeguimiento(err, "Guardar Seguimiento");
    });
}

$(document).ready(function () {
    // Handler for opening the new/edit tracking modal
    $('#btnNuevoSeguimiento').on('click', function () {
        limpiarModalSeguimiento();
        $('#modalDataSeguimientoLabel').text('Nuevo Seguimiento');
        $('#modalDataSeguimiento').modal('show');
    });

    // Handler for saving a tracking entry
    $('#btnGuardarSeguimiento').on('click', function () {
        const modelo = {
            secSeguimiento: parseInt($("#txtIdSeguimiento").val()) || 0,
            secCotizacion: parseInt($('#hiddenCotizacionId').val()),
            accion: $("#txtAccion").val(),
            detalle: $("#txtDetalle").val(),
            fechaAccion: $("#txtFechaAccion").val(),
            aceptacionCliente: $("#chkAceptacionCliente").is(":checked")
        };

        if (!modelo.accion || !modelo.detalle || !modelo.fechaAccion) {
            Swal.fire("Advertencia", "Acción, Detalle y Fecha son campos obligatorios.", "warning");
            return;
        }

        const hoy = new Date();
        hoy.setHours(0, 0, 0, 0);
        const parts = modelo.fechaAccion.split('-');
        const fechaSeleccionada = new Date(parts[0], parts[1] - 1, parts[2]);

        // 1. Future Date Check
        if (fechaSeleccionada > hoy) {
            Swal.fire("Error de Fecha", "No se puede seleccionar una fecha futura.", "error");
            return;
        }

        // 2. Old Date Check (only for new records)
        if (modelo.secSeguimiento === 0) {
            const fechasExistentes = [];
            tablaSeguimiento.rows().every(function () {
                const rowData = this.data();
                if (rowData && rowData.fechaAccion) {
                    fechasExistentes.push(new Date(rowData.fechaAccion));
                }
            });

            if (fechasExistentes.length > 0) {
                const fechaMinima = new Date(Math.min.apply(null, fechasExistentes));
                fechaMinima.setHours(0, 0, 0, 0);

                if (fechaSeleccionada < fechaMinima) {
                    Swal.fire({
                        title: 'Fecha Antigua Detectada',
                        text: 'Está registrando un seguimiento con una fecha anterior al primer seguimiento existente. ¿Desea continuar?',
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Sí, continuar',
                        cancelButtonText: 'Cancelar'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            procederConGuardado(modelo);
                        }
                    });
                    return; // Stop here and wait for user confirmation
                }
            }
        }

        procederConGuardado(modelo);
    });

    // Handler for editing a tracking entry from the table
    $('#tbSeguimiento tbody').on('click', '.btn-editar-seguimiento', function () {
        let fila = $(this).closest('tr');
        if (fila.hasClass('child')) {
            fila = fila.prev();
        }
        const data = tablaSeguimiento.row(fila).data();

        if (data) {
            $("#txtIdSeguimiento").val(data.secSeguimiento);
            $("#txtAccion").val(data.accion);
            $("#txtDetalle").val(data.detalle);
            const fecha = data.fechaAccion ? data.fechaAccion.split('T')[0] : '';
            $("#txtFechaAccion").val(fecha);
            $("#chkAceptacionCliente").prop("checked", data.aceptacionCliente);
            $('#modalDataSeguimientoLabel').text('Editar Seguimiento');
            $('#modalDataSeguimiento').modal('show');
        }
    });

    // Handler for deleting a tracking entry
    $('#tbSeguimiento tbody').on('click', '.btn-eliminar-seguimiento', function () {
        let fila = $(this).closest('tr');
        if (fila.hasClass('child')) {
            fila = fila.prev();
        }
        const data = tablaSeguimiento.row(fila).data();

        if (data) {
            Swal.fire({
                title: '¿Está seguro?',
                text: `¿Desea eliminar el seguimiento "${data.accion}"?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, eliminar',
                cancelButtonText: 'No, volver'
            }).then((result) => {
                if (result.isConfirmed) {
                    fetch(`/Seguimiento/Eliminar?secuencial=${data.secSeguimiento}`, { method: 'DELETE' })
                        .then(response => {
                            if (!response.ok) return response.json().then(err => Promise.reject(err));
                            return response.json();
                        })
                        .then(responseJson => {
                            if (responseJson.estado) {
                                Swal.fire('Eliminado!', 'El seguimiento ha sido eliminado.', 'success');
                                tablaSeguimiento.ajax.reload();
                            } else {
                                Swal.fire('Error', responseJson.mensajes, 'error');
                            }
                        })
                        .catch(err => {
                            manejarErrorFetchSeguimiento(err, "Eliminar Seguimiento");
                        });
                }
            });
        }
    });
});