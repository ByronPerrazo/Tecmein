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

    // Configuración de idioma local en español para DataTable
    const lenguajeEspanol = {
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
    };

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
                data: "secFormaPago",
                render: function (data, type, row) {
                    return `<div class="dropdown">` +
                           `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">` +
                           `<i class="fas fa-cog text-warning mr-1"></i> Acciones` +
                           `</button>` +
                           `<div class="dropdown-menu">` +
                           `<a class="dropdown-item btn-editar" href="#"><i class="fas fa-pencil-alt text-primary mr-2"></i> Editar</a>` +
                           `<a class="dropdown-item btn-eliminar" href="#"><i class="fas fa-trash-alt text-danger mr-2"></i> Eliminar</a>` +
                           `</div>` +
                           `</div>`;
                },
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
        buttons: [
            {
                text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                extend: 'excelHtml5',
                title: 'Formas de Pago',
                filename: 'Reporte Formas de Pago',
                exportOptions: {
                    columns: [0, 1]
                },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-file-pdf text-danger fa-lg"></i>',
                extend: 'pdfHtml5',
                title: 'Formas de Pago',
                filename: 'Reporte Formas de Pago',
                exportOptions: {
                    columns: [0, 1]
                },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-print text-primary fa-lg"></i>',
                extend: 'print',
                title: 'Formas de Pago',
                exportOptions: {
                    columns: [0, 1]
                },
                className: 'btn btn-link btn-sm p-1'
            }
        ],
        "language": lenguajeEspanol,
        initComplete: function() {
            $("#btnNuevaFormaPago").appendTo(".toolbar-left");
            $("#btnNuevaFormaPago").closest(".row").show();
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
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaFormaPago.row(fila).data();
        
        $('#SecFormaPago').val(data.secFormaPago);
        $('#Descripcion').val(data.descripcion);
        $('#EstaActivo').prop('checked', data.estaActivo);

        $('#modalFormaPago').modal('show');
    });

    $('#tbFormaPago tbody').on('click', '.btn-eliminar', function () {
        let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
        const data = tablaFormaPago.row(fila).data();
        
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