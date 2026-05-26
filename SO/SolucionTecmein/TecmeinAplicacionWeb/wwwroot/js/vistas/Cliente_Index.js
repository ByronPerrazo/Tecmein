var tablaClientes;
$(document).ready(function () {
    // Función de manejo de errores centralizada para AJAX
    function manejarErrorAjax(jqXHR, textStatus, errorThrown) {
        if (jqXHR.responseJSON && jqXHR.responseJSON.mensajes) {
            Swal.fire("Error", jqXHR.responseJSON.mensajes, "error");
        } else {
            Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor o procesar la respuesta.", "error");
        }
    }

    // Inicializar DataTable de Clientes
    tablaClientes = $('#tbdataClientes').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Cliente/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (response) {
                if (response.estado) {
                    return response.objeto.$values;
                } else {
                    console.error("Error al cargar la lista de clientes: " + response.mensajes);
                    Swal.fire("Error", "No se pudo cargar la lista de clientes: " + response.mensajes, "error");
                    return [];
                }
            },
            "error": manejarErrorAjax
        },
        "columns": [
            { "data": "numeroCliente" },
            { "data": "nombreConstructora" },
            { "data": "fechaCreacion" },
            {
                "data": "estaActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-success">Activo</span>';
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
                title: 'Clientes',
                filename: 'Reporte Clientes',
                exportOptions: { columns: [0, 1, 2, 3] },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-file-pdf text-danger fa-lg"></i>',
                extend: 'pdfHtml5',
                title: 'Clientes',
                filename: 'Reporte Clientes',
                exportOptions: { columns: [0, 1, 2, 3] },
                className: 'btn btn-link btn-sm p-1'
            },
            {
                text: '<i class="fas fa-print text-primary fa-lg"></i>',
                extend: 'print',
                title: 'Clientes',
                exportOptions: { columns: [0, 1, 2, 3] },
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
            $("#btnNuevoCliente").appendTo(".toolbar-left");
            $("#btnNuevoCliente").closest(".row").show();
        }
    });

    // Evento para el botón 'Crear Nuevo Cliente'
    $('#btnNuevoCliente').on('click', function () {
        abrirModalCliente();
    });

    // Evento para el botón 'Guardar Cliente' en el modal de Cliente
    $('#btnGuardarCliente').on('click', function () {
        if ($('#modalCliente #cboConstructoraCliente').val() === "") {
            Swal.fire("Atención", "Debe seleccionar una constructora", "warning");
            return;
        }

        var cliente = {
            secCliente: $('#modalCliente #txtSecCliente').val(),
            secConstructora: $('#modalCliente #cboConstructoraCliente').val(),
            numeroCliente: $('#modalCliente #txtNumeroCliente').val(),
            estaActivo: $('#modalCliente #chkEstaActivo').prop('checked')
        };

        var url = cliente.secCliente == "0" ? '/Cliente/Crear' : '/Cliente/Editar';
        var type = cliente.secCliente == "0" ? 'POST' : 'PUT';

        $.ajax({
            url: url,
            type: type,
            contentType: 'application/json',
            data: JSON.stringify(cliente),
            success: function (response) {
                if (response.estado) {
                    Swal.fire("Listo!", cliente.secCliente == "0" ? "Cliente creado exitosamente" : "Cliente actualizado exitosamente", "success");
                    tablaClientes.ajax.reload();
                    $('#modalCliente').modal('hide');
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: manejarErrorAjax
        });
    });

    // Evento para el botón 'Editar' en la tabla de Clientes
    $('#tbdataClientes tbody').on('click', '.btn-editar', function () {
        var data = tablaClientes.row($(this).parents('tr')).data();
        abrirModalCliente(data);
    });

    // Evento para el botón 'Eliminar' en la tabla de Clientes
    $('#tbdataClientes tbody').on('click', '.btn-eliminar', function () {
        var data = tablaClientes.row($(this).parents('tr')).data();

        Swal.fire({
            title: '¿Está seguro?',
            text: `Eliminar el cliente "${data.numeroCliente}"`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'No, cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Cliente/Eliminar',
                    type: 'DELETE',
                    data: { secCliente: data.secCliente },
                    success: function (response) {
                        if (response.estado) {
                            tablaClientes.ajax.reload();
                            Swal.fire("Eliminado!", "El cliente ha sido eliminado.", "success");
                        } else {
                            Swal.fire("Error", response.mensajes, "error");
                        }
                    },
                    error: manejarErrorAjax
                });
            }
        });
    });

    // Función para abrir el modal de Cliente (Crear/Editar)
    function abrirModalCliente(data = null) {
        $('#modalCliente #txtSecCliente').val('0');
        $('#modalCliente #cboConstructoraCliente').val('');
        $('#modalCliente #txtNumeroCliente').val('');
        $('#modalCliente #chkEstaActivo').prop('checked', true);

        $.ajax({
            url: '/Cliente/ListarConstructoras',
            type: 'GET',
            success: function (response) {
                if (response.estado) {
                    var combo = $('#modalCliente #cboConstructoraCliente');
                    combo.empty();
                    combo.append($('<option>', { value: '', text: 'Seleccione Constructora' }));
                    $.each(response.objeto.$values, function (i, constructora) {
                        combo.append($('<option>', { value: constructora.secuencial, text: constructora.nombre }));
                    });

                    if (data) { // Modo edición
                        if ($('#modalCliente #cboConstructoraCliente option[value="' + data.secConstructora + '"]').length === 0) {
                            $('#modalCliente #cboConstructoraCliente').append($('<option>', {
                                value: data.secConstructora,
                                text: data.nombreConstructora
                            }));
                        }

                        $('#modalCliente #txtSecCliente').val(data.secCliente);
                        $('#modalCliente #cboConstructoraCliente').val(data.secConstructora);
                        $('#modalCliente #txtNumeroCliente').val(data.numeroCliente).prop('readonly', true);
                        $('#modalCliente #chkEstaActivo').prop('checked', data.estaActivo == 1);
                        combo.prop('disabled', true);
                    } else { // Modo creación
                        $('#modalCliente #txtNumeroCliente').prop('readonly', false);
                        combo.prop('disabled', false);
                    }
                    $('#modalCliente').modal('show');
                } else {
                    Swal.fire('Error al cargar constructoras', response.mensajes, 'error');
                }
            },
            error: manejarErrorAjax
        });
    }
});