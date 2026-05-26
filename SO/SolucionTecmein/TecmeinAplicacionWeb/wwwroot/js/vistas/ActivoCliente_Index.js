$(document).ready(function () {
    var tablaActivosCliente;

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

    function cargarDatos() {
        if ($.fn.DataTable.isDataTable('#tablaActivosCliente')) {
            $('#tablaActivosCliente').DataTable().destroy();
        }

        tablaActivosCliente = $("#tablaActivosCliente").DataTable({
            responsive: true,
            "ajax": {
                "url": "/ActivoCliente/ListarActivosPorCliente?secCliente=0",
                "type": "GET",
                "datatype": "json",
                "dataSrc": function (json) {
                    return json.data && json.data.$values ? json.data.$values : json.data;
                }
            },
            "columns": [
                { "data": "idActivoCliente" },
                { "data": "nombreCliente" },
                { "data": "descripcion" },
                {
                    "data": "fechaInstalacion",
                    "render": function (data) {
                        return data ? new Date(data).toLocaleDateString() : "";
                    }
                },
                { "data": "contratoOrigenNumero" },
                {
                    "data": "idActivoCliente",
                    "render": function (data, type, row) {
                        return `<div class="dropdown">` +
                               `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">` +
                               `<i class="fas fa-cog text-warning mr-1"></i> Acciones` +
                               `</button>` +
                               `<div class="dropdown-menu">` +
                               `<a class="dropdown-item btn-editar" href="#" data-id="${data}"><i class="fas fa-pencil-alt text-primary mr-2"></i> Editar</a>` +
                               `<a class="dropdown-item btn-eliminar" href="#" data-id="${data}"><i class="fas fa-trash-alt text-danger mr-2"></i> Eliminar</a>` +
                               `</div>` +
                               `</div>`;
                    },
                    "orderable": false,
                    "searchable": false,
                    "width": "120px"
                }
            ],
            "order": [[0, "desc"]],
            dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
            buttons: [
                {
                    text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                    extend: 'excelHtml5',
                    title: 'Activos de Clientes',
                    filename: 'Reporte Activos Clientes',
                    exportOptions: { columns: [0, 1, 2, 3, 4] },
                    className: 'btn btn-link btn-sm p-1'
                }
            ],
            "language": lenguajeEspanol,
            initComplete: function() {
                $("#btnNuevo").appendTo(".toolbar-left");
                $("#btnNuevo").closest(".row").show();
            }
        });
    }

    function cargarDropdown(url, selector, defaultValue = "Seleccionar...") {
        return $.ajax({ url: url, type: "GET" }).then(function(response) {
            const lista = response.data && response.data.$values ? response.data.$values : response.data;
            const $dropdown = $(selector);
            $dropdown.empty().append($("<option>").val("").text(defaultValue));
            if (lista && lista.length > 0) {
                lista.forEach(function (item) {
                    $dropdown.append($("<option>").val(item.value).text(item.text));
                });
            }
        });
    }

    $("#btnNuevo").click(function () {
        $("#txtIdActivoCliente").val("0");
        $("#cboCliente").val("");
        $("#txtDescripcion").val("");
        $("#txtFechaInstalacion").val("");
        $("#cboContratoOrigen").val("");

        // Cargar dropdowns para el modal
        $.when(
            cargarDropdown("/Cliente/ListaClientesActivos", "#cboCliente", "Seleccione un Cliente"), // Asumo un endpoint para listar clientes
            cargarDropdown("/Contrato/ListaContratosActivos", "#cboContratoOrigen", "Seleccione Contrato (Opcional)") // Asumo un endpoint para listar contratos
        ).done(() => $('#modalActivoCliente').modal('show')).fail(() => {
            Swal.fire("Error", "Ocurrió un error al preparar el formulario.", "error");
        });
    });

    $("#btnGuardar").click(function () {
        const idActivoCliente = parseInt($("#txtIdActivoCliente").val());

        const modelo = {
            IdActivoCliente: idActivoCliente,
            SecCliente: parseInt($("#cboCliente").val()),
            Descripcion: $("#txtDescripcion").val(),
            FechaInstalacion: $("#txtFechaInstalacion").val(),
            SecContratoOrigen: parseInt($("#cboContratoOrigen").val()) || null // Puede ser nulo
        };

        if (!modelo.SecCliente || isNaN(modelo.SecCliente)) {
            Swal.fire("Atención", "Por favor, seleccione un cliente.", "warning");
            return;
        }
        if (!modelo.Descripcion) {
            Swal.fire("Atención", "Por favor, ingrese una descripción.", "warning");
            return;
        }
        if (!modelo.FechaInstalacion) {
            Swal.fire("Atención", "Por favor, ingrese la fecha de instalación.", "warning");
            return;
        }

        const url = idActivoCliente === 0 ? "/ActivoCliente/Crear" : "/ActivoCliente/Editar";
        const type = idActivoCliente === 0 ? "POST" : "PUT";
        const successMessage = idActivoCliente === 0 ? "Activo de cliente creado exitosamente." : "Activo de cliente editado exitosamente.";

        $.ajax({
            url: url,
            type: type,
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(modelo),
            beforeSend: () => Swal.fire({ title: 'Guardando...', allowOutsideClick: false, didOpen: () => Swal.showLoading() }),
            success: function (response) {
                Swal.close();
                if (response.estado) {
                    $('#modalActivoCliente').modal('hide');
                    tablaActivosCliente.ajax.reload(null, false);
                    Swal.fire("¡Éxito!", successMessage, "success");
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: (error) => {
                Swal.close();
                Swal.fire("Error", `Error de comunicación: ${error.responseText}`, "error");
            }
        });
    });

    $("#tablaActivosCliente tbody").on("click", ".btn-editar", function (e) {
        e.preventDefault();
        const id = $(this).data("id");

        // Cargar datos existentes y dropdowns
        $.when(
            cargarDropdown("/Cliente/ListaClientesActivos", "#cboCliente", "Seleccione un Cliente"),
            cargarDropdown("/Contrato/ListaContratosActivos", "#cboContratoOrigen", "Seleccione Contrato (Opcional)")
        ).done(function() {
            $.ajax({
                url: `/ActivoCliente/Obtener/${id}`, // Asumo un endpoint para obtener un activo por ID
                type: "GET",
                beforeSend: () => Swal.fire({ title: 'Cargando Datos...', allowOutsideClick: false, didOpen: () => Swal.showLoading() }),
                success: function (response) {
                    Swal.close();
                    if (response.estado) {
                        const data = response.objeto;
                        $("#txtIdActivoCliente").val(data.idActivoCliente);
                        $("#cboCliente").val(data.secCliente);
                        $("#txtDescripcion").val(data.descripcion);
                        $("#txtFechaInstalacion").val(data.fechaInstalacion.split('T')[0]); // Formato YYYY-MM-DD
                        $("#cboContratoOrigen").val(data.secContratoOrigen);
                        $('#modalActivoCliente').modal('show');
                    } else {
                        Swal.fire("Error", response.mensajes, "error");
                    }
                },
                error: () => Swal.fire("Error", "No se pudieron cargar los datos para edición.", "error")
            });
        }).fail(() => {
            Swal.fire("Error", "Ocurrió un error al preparar el formulario.", "error");
        });
    });

    $("#tablaActivosCliente tbody").on("click", ".btn-eliminar", function () {
        var id = $(this).data("id");
        Swal.fire({
            title: '¿Está seguro?',
            text: "No podrá revertir esta acción",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/ActivoCliente/Eliminar/${id}`, { method: 'DELETE' })
                    .then(response => response.json())
                    .then(data => {
                        if (data.estado) {
                            tablaActivosCliente.ajax.reload(null, false);
                            Swal.fire('¡Eliminado!', 'El activo ha sido eliminado.', 'success');
                        } else {
                            Swal.fire('Error', data.mensajes, 'error');
                        }
                    })
                    .catch(() => Swal.fire('Error', 'No se pudo comunicar con el servidor.', 'error'));
            }
        });
    });


    cargarDatos();
});
