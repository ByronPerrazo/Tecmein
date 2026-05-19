let tablaFormaPago;

$(document).ready(function () {
    function manejarErrorFetch(error, operacion) {
        console.error(`Error en ${operacion}:`, error);
        if (error && error.mensajes) {
            Swal.fire("Error", error.mensajes, "error");
        } else {
            Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
        }
    }

    tablaFormaPago = $('#tbFormaPago').DataTable({
        responsive: true,
        ajax: {
            url: '/FormaPago/Lista', 
            type: "GET",
            datatype: "json",
            dataSrc: function(json) {
                return json.objeto.$values || json.objeto; 
            },
            error: function (xhr, error, thrown) {
                console.error("Error al cargar datos del DataTable:", error, thrown, xhr);
                Swal.fire("Error de Carga", "No se pudieron cargar las formas de pago. Verifique la consola para más detalles.", "error");
            }
        },
        columns: [
            { data: "descripcion" },
            { 
                data: "estaActivo",
                render: function (data) {
                    return data ? '<span class="badge badge-success">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                "defaultContent": '<div class="btn-group" role="group" aria-label="Acciones de Forma de Pago">' +
                                    '<button class="btn btn-primary btn-editar btn-sm" title="Editar"><i class="fas fa-pencil-alt"></i></button>' +
                                    '<button class="btn btn-danger btn-eliminar btn-sm" title="Eliminar"><i class="fas fa-trash-alt"></i></button>' +
                                  '</div>',
                "orderable": false,
                "searchable": false,
                "width": "100px"
            }
        ],
        dom: "Bfrtip",
        buttons: [
            {
                text: '<i class="fas fa-file-excel"></i> Excel',
                extend: 'excelHtml5',
                title: 'Formas de Pago',
                filename: 'Reporte Formas de Pago',
                exportOptions: {
                    columns: [0, 1]
                }
            },
            {
                text: '<i class="fas fa-file-pdf"></i> PDF',
                extend: 'pdfHtml5',
                title: 'Formas de Pago',
                filename: 'Reporte Formas de Pago',
                exportOptions: {
                    columns: [0, 1]
                }
            },
            {
                text: '<i class="fas fa-print"></i> Imprimir',
                extend: 'print',
                title: 'Formas de Pago',
                exportOptions: {
                    columns: [0, 1]
                }
            },
            'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        }
    });

    $('#btnNuevaFormaPago').on('click', function () {
        $('#formaPagoForm')[0].reset(); // Resetear el formulario
        $('#SecFormaPago').val("0"); // Establecer ID a 0 para indicar nuevo registro
        $('#EstaActivo').prop('checked', true); // Marcar "Está Activo" por defecto
        $('#modalFormaPago').modal('show'); // Abrir el modal
    });

    $('#btnGuardarFormaPago').on('click', function () {
        const modelo = {
            SecFormaPago: $('#SecFormaPago').val(),
            Descripcion: $('#Descripcion').val(),
            EstaActivo: $('#EstaActivo').is(':checked')
        };

        if (modelo.Descripcion.trim() === "") {
            Swal.fire("Validación", "La descripción no puede estar vacía.", "warning");
            return;
        }

        const esNuevo = modelo.SecFormaPago == 0 || modelo.SecFormaPago == "";
        const url = esNuevo ? "/FormaPago/Crear" : "/FormaPago/Editar";
        const method = esNuevo ? "POST" : "PUT";

        fetch(url, {
            method: method,
            headers: {
                "Content-Type": "application/json; charset=utf-8",
            },
            body: JSON.stringify(modelo)
        })
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(responseJson => {
            if (responseJson.estado) {
                Swal.fire("Guardado", "La forma de pago ha sido guardada con éxito.", "success");
                $('#modalFormaPago').modal('hide'); // Cerrar el modal
                tablaFormaPago.ajax.reload(); // Recargar el DataTable
            } else {
                Swal.fire("Error", responseJson.mensajes, "error");
            }
        })
        .catch((error) => {
            manejarErrorFetch(error, "Guardar Forma de Pago");
        });
    });

    $('#tbFormaPago tbody').on('click', '.btn-editar', function () {
        const data = tablaFormaPago.row($(this).parents('tr')).data();
        
        $('#SecFormaPago').val(data.secFormaPago);
        $('#Descripcion').val(data.descripcion);
        $('#EstaActivo').prop('checked', data.estaActivo);

        $('#modalFormaPago').modal('show');
    });

    $('#tbFormaPago tbody').on('click', '.btn-eliminar', function () {
        const data = tablaFormaPago.row($(this).parents('tr')).data();
        
        Swal.fire({
            title: '¿Está seguro?',
            text: `¿Desea eliminar la forma de pago "${data.descripcion}"?`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/FormaPago/Eliminar?secFormaPago=${data.secFormaPago}`, {
                    method: 'DELETE'
                })
                .then(response => response.json())
                .then(responseJson => {
                    if (responseJson.estado) {
                        Swal.fire('¡Eliminado!', 'La forma de pago ha sido eliminada.', 'success');
                        tablaFormaPago.ajax.reload();
                    } else {
                        Swal.fire('Error', responseJson.mensajes, 'error');
                    }
                })
                .catch(error => {
                    manejarErrorFetch(error, "Eliminar Forma de Pago");
                });
            }
        });
    });
});