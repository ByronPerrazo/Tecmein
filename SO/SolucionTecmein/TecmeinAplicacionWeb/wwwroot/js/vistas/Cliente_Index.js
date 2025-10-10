var tablaClientes;
$(document).ready(function () {
    // Inicializar DataTable de Clientes
    tablaClientes = $('#tbdataClientes').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Cliente/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (response) {
                if (response.estado) {
                    // DataTables espera un array, que se encuentra en la propiedad $values del objeto
                    return response.objeto.$values;
                } else {
                    // Si hubo un error, devuelve un array vacío para que la tabla no falle
                    console.error("Error al cargar la lista de clientes: " + response.mensajes);
                    return [];
                }
            }
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
                "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button></div>',
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
                title: '',
                filename: 'Reporte Clientes',
                exportOptions: {
                    columns: [0, 1, 2, 3]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

    // Evento para el botón 'Crear Nuevo Cliente'
    $('#btnNuevoCliente').on('click', function () {
        abrirModalCliente();
    });

    // Evento para el botón 'Guardar Cliente' en el modal de Cliente
    $('#btnGuardarCliente').on('click', function () {
        // Validación básica de campos
        if ($('#modalCliente #cboConstructoraCliente').val() === "") {
            Swal.fire("Atención", "Debe seleccionar una constructora", "warning");
            return;
        }

        var cliente = {
            secCliente: $('#modalCliente #txtSecCliente').val(),
            secConstructora: $('#modalCliente #cboConstructoraCliente').val(),
            numeroCliente: $('#modalCliente #txtNumeroCliente').val(), // Añadido
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
            error: function (error) {
                Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor.", "error");
            }
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
                    error: function (error) {
                        Swal.fire("Error de Comunicación", "No se pudo conectar con el servidor.", "error");
                    }
                });
            }
        });
    });

    // Función para abrir el modal de Cliente (Crear/Editar)
    function abrirModalCliente(data = null) {
        // Limpiar formulario
        $('#modalCliente #txtSecCliente').val('0');
        $('#modalCliente #cboConstructoraCliente').val('');
        $('#modalCliente #txtNumeroCliente').val('');
        $('#modalCliente #chkEstaActivo').prop('checked', true);

        // Cargar constructoras en el combo (solo las que no son clientes)
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
                        // Asegurarse de que la constructora del cliente actual esté en la lista para poder seleccionarla
                        if ($('#modalCliente #cboConstructoraCliente option[value="' + data.secConstructora + '"]').length === 0) {
                            $('#modalCliente #cboConstructoraCliente').append($('<option>', {
                                value: data.secConstructora,
                                text: data.nombreConstructora
                            }));
                        }

                        $('#modalCliente #txtSecCliente').val(data.secCliente);
                        $('#modalCliente #cboConstructoraCliente').val(data.secConstructora);
                        $('#modalCliente #txtNumeroCliente').val(data.numeroCliente).prop('readonly', true); // No se edita el número de cliente
                        $('#modalCliente #chkEstaActivo').prop('checked', data.estaActivo == 1);
                        combo.prop('disabled', true); // No se cambia la constructora en edición
                    } else { // Modo creación
                        $('#modalCliente #txtNumeroCliente').prop('readonly', false); // Se puede generar o dejar vacío para que el backend lo genere
                        combo.prop('disabled', false);
                    }
                    $('#modalCliente').modal('show');
                } else {
                    alert('Error al cargar constructoras: ' + response.mensajes);
                }
            },
            error: function (error) {
                alert('Error de comunicación al cargar constructoras: ' + error.responseText);
            }
        });
    }
});
