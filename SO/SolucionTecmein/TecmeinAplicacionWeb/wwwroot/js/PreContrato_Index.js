$(document).ready(function () {
    var tablaPreContratos;

    const spanishLanguage = {
        "url": "/js/datatables/i18n/Spanish.json"
    };

    function cargarDatos() {
        if ($.fn.DataTable.isDataTable('#tablaPreContratos')) {
            $('#tablaPreContratos').DataTable().destroy();
        }

        tablaPreContratos = $("#tablaPreContratos").DataTable({
            responsive: true,
            "ajax": {
                "url": "/PreContrato/Listar",
                "type": "GET",
                "datatype": "json",
                "dataSrc": function (json) {
                    return json.data && json.data.$values ? json.data.$values : json.data;
                }
            },
            "columns": [
                { "data": "secPreContrato" },
                { "data": "numeroCotizacion" },
                { "data": "nombreObra" },
                { 
                    "data": "fechaRegistro",
                    "render": function (data) {
                        return data ? new Date(data).toLocaleDateString() : "";
                    }
                },
                { "data": "version" },
                { "data": "estado" }, // Nueva columna para el estado
                {
                    "data": "secPreContrato",
                    "render": function (data, type, row) {
                        const btnEditar = `<button class="btn btn-primary btn-sm btn-editar" data-id="${data}" title="Editar"><i class="fas fa-pencil-alt"></i></button>`;
                        const btnHistorial = `<button class="btn btn-secondary btn-sm btn-historial" data-id="${data}" title="Ver Historial"><i class="fas fa-history"></i></button>`;
                        const btnEliminar = `<button class="btn btn-danger btn-sm btn-eliminar" data-id="${data}" title="Eliminar"><i class="fas fa-trash-alt"></i></button>`;
                        let btnAprobar = '';
                        if (row.estado !== "Aprobado") {
                            btnAprobar = `<button class="btn btn-success btn-sm btn-aprobar" data-id="${data}" title="Aprobar"><i class="fas fa-check"></i></button>`;
                        }
                        return `<div class="btn-group" role="group">${btnEditar}${btnHistorial}${btnEliminar}${btnAprobar}</div>`;
                    },
                    "orderable": false,
                    "searchable": false,
                    "width": "150px"
                }
            ],
            "order": [[0, "desc"]],
            "language": spanishLanguage
        });
    }

    function cargarDropdown(url, selector) {
        return $.ajax({ url: url, type: "GET" }).then(function(response) {
            const lista = response.data && response.data.$values ? response.data.$values : response.data;
            const $dropdown = $(selector);
            $dropdown.empty().append($("<option>").val("").text("Seleccionar..."));
            if (lista && lista.length > 0) {
                lista.forEach(function (item) {
                    $dropdown.append($("<option>").val(item.value).text(item.text));
                });
            }
        });
    }

    $("#btnNuevo").click(function () {
        $("#cboCotizacionesAceptadas, #SecFormaPago, #SecPlantillaPreContrato").val("");
        $("#ValorContrato, #ValorAnticipo, #NumeroCuotas, #Dias, #AniosGarantia, #MesesGarantia").val("0");
        $("#FechaAnticipo, #FechaPrimeraCuota, #PeriodoMantenimiento").val("");
        $("#PolizaGarantia").val("");
        $("#TipoDias").val("Hábiles");

        $.when(
            cargarDropdown("/PreContrato/ListaCotizacionesAprobadas", "#cboCotizacionesAceptadas"),
            cargarDropdown("/PreContrato/ListaFormasPago", "#SecFormaPago"),
            cargarDropdown("/PreContrato/ListaPlantillas", "#SecPlantillaPreContrato"),
            cargarDropdown("/PolizaGarantia/ListaParaDropdown", "#PolizaGarantia")
        ).done(() => $('#modalPreContrato').modal('show')).fail(() => {
            Swal.fire("Error", "Ocurrió un error al preparar el formulario.", "error");
        });
    });

    $("#cboCotizacionesAceptadas").change(function() {
        const cotizacionId = $(this).val();
        if (cotizacionId) {
            $.ajax({
                url: `/Cotizacion/Detalle/${cotizacionId}`,
                type: "GET",
                success: function(response) {
                    if (!response || typeof response.subtotal === 'undefined' || typeof response.valorImpuestos === 'undefined') {
                        Swal.fire("Error de Datos", "La respuesta no contiene los datos esperados.", "error");
                        return;
                    }
                    const total = parseFloat(response.subtotal) + parseFloat(response.valorImpuestos);
                    $("#ValorContrato").val(isNaN(total) ? "0" : total.toFixed(2));
                },
                error: () => {
                    $("#ValorContrato").val("0");
                    Swal.fire("Error de Comunicación", "No se pudieron obtener los detalles de la cotización.", "error");
                }
            });
        } else {
            $("#ValorContrato").val("0");
        }
    });

    let datosFormularioParaGuardar = null;

    $("#btnGenerarPreContrato").click(function() {
        const modelo = {
            SecCotizacion: parseInt($("#cboCotizacionesAceptadas").val()),
            SecFormaPago: parseInt($("#SecFormaPago").val()),
            SecPlantillaPreContrato: parseInt($("#SecPlantillaPreContrato").val()),
            ValorContrato: parseFloat($("#ValorContrato").val()),
            ValorAnticipo: parseFloat($("#ValorAnticipo").val()),
            FechaAnticipo: $("#FechaAnticipo").val(),
            NumeroCuotas: parseInt($("#NumeroCuotas").val()),
            FechaPrimeraCuota: $("#FechaPrimeraCuota").val(),
            Dias: parseInt($("#Dias").val()),
            TipoDias: $("#TipoDias").val(),
            PeriodoMantenimiento: $("#PeriodoMantenimiento").val(),
            AniosGarantia: parseInt($("#AniosGarantia").val()),
            MesesGarantia: parseInt($("#MesesGarantia").val()),
            PolizaGarantia: $("#PolizaGarantia").val()
        };

        if (!modelo.SecCotizacion || !modelo.SecPlantillaPreContrato) {
            Swal.fire("Atención", "Por favor, seleccione una cotización y una plantilla.", "warning");
            return;
        }
        datosFormularioParaGuardar = modelo;

        $.ajax({
            url: "/PreContrato/GenerarVistaPrevia",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(modelo),
            beforeSend: () => Swal.fire({ title: 'Generando vista previa...', allowOutsideClick: false, didOpen: () => Swal.showLoading() }),
            success: function(response) {
                Swal.close();
                if (response.estado) {
                    $("#contenidoVistaPrevia").html(response.objeto);
                    $('#modalPreContrato').modal('hide');
                    $('#modalVistaPrevia').modal('show');
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: () => Swal.fire("Error", "No se pudo generar la vista previa.", "error")
        });
    });

    $("#btnGuardarDefinitivo").click(function() {
        if (!datosFormularioParaGuardar) {
            Swal.fire("Error", "No hay datos para guardar.", "error");
            return;
        }

        const contenidoHtml = $("#contenidoVistaPrevia").html(); // Obtener el contenido HTML de la vista previa

        // Crear un nuevo objeto que incluya los datos del formulario y el contenido HTML
        const payload = {
            ...datosFormularioParaGuardar,
            ContenidoHtml: contenidoHtml
        };

        $.ajax({
            url: "/PreContrato/CrearDesdeModal",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(payload),
            beforeSend: () => Swal.fire({ title: 'Guardando...', allowOutsideClick: false, didOpen: () => Swal.showLoading() }),
            success: function(response) {
                if (response.estado) {
                    $('#modalVistaPrevia').modal('hide');
                    tablaPreContratos.ajax.reload(null, false);
                    Swal.fire("¡Guardado!", "El pre-contrato ha sido creado exitosamente.", "success");
                } else {
                    Swal.fire("Error al guardar", response.mensajes, "error");
                }
            },
            error: () => Swal.fire("Error de comunicación", "No se pudo comunicar con el servidor.", "error")
        });
    });

    $("#tablaPreContratos tbody").on("click", ".btn-editar", function () {
        var secPreContrato = $(this).data("id");
        if (secPreContrato && secPreContrato !== "undefined") {
            window.location.href = `/PreContrato/Editor/${secPreContrato}`;
        } else {
            Swal.fire("Error", "No se pudo obtener el ID del pre-contrato para editar.", "error");
        }
    });

    $("#tablaPreContratos tbody").on("click", ".btn-eliminar", function () {
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
                fetch(`/PreContrato/Eliminar/${id}`, { method: 'DELETE' })
                    .then(response => response.json())
                    .then(data => {
                        if (data.estado) {
                            tablaPreContratos.ajax.reload(null, false);
                            Swal.fire('¡Eliminado!', 'El pre-contrato ha sido eliminado.', 'success');
                        } else {
                            Swal.fire('Error', data.mensajes, 'error');
                        }
                    })
                    .catch(() => Swal.fire('Error', 'No se pudo comunicar con el servidor.', 'error'));
            }
        });
    });

    $("#tablaPreContratos tbody").on("click", ".btn-aprobar", function () {
        var id = $(this).data("id");
        Swal.fire({
            title: '¿Está seguro de aprobar este pre-contrato?',
            text: "Esta acción marcará el pre-contrato como aprobado y lo hará elegible para la creación de un contrato.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, aprobar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(`/PreContrato/Aprobar/${id}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            tablaPreContratos.ajax.reload(null, false);
                            Swal.fire('¡Aprobado!', data.message, 'success');
                        } else {
                            Swal.fire('Error', data.message, 'error');
                        }
                    })
                    .catch(() => Swal.fire('Error', 'No se pudo comunicar con el servidor.', 'error'));
            }
        });
    });

    let tablaHistorial;
    $("#tablaPreContratos tbody").on("click", ".btn-historial", function () {
        var id = $(this).data("id");
        if ($.fn.DataTable.isDataTable('#tablaHistorial')) {
            $('#tablaHistorial').DataTable().destroy();
        }
        tablaHistorial = $("#tablaHistorial").DataTable({
            responsive: true,
            "ajax": {
                "url": `/PreContrato/Historial/${id}`,
                "type": "GET",
                "datatype": "json",
                "dataSrc": function (json) {
                    return json.data && json.data.$values ? json.data.$values : json.data;
                }
            },
            "columns": [
                { "data": "version" },
                { "data": "fechaRegistro", "render": function(data) { return new Date(data).toLocaleString(); } },
                { "data": "nombreUsuarioCrea" },
                { "data": "estaActivo", "render": function(data) { return data ? '<span class="badge badge-success">Activa</span>' : '<span class="badge badge-secondary">Histórica</span>'; } },
                { "data": "secPreContrato", "render": function(data) { return `<button class="btn btn-info btn-sm btn-ver-version-historica" data-id="${data}" title="Ver Contenido"><i class="fas fa-eye"></i></button>`; }, "orderable": false, "searchable": false }
            ],
            "order": [[0, "desc"]],
            "language": spanishLanguage
        });
        $('#modalHistorial').modal('show');
    });

    $("#tablaHistorial tbody").on("click", ".btn-ver-version-historica", function() {
        var id = $(this).data("id");
        $.get(`/PreContrato/ContenidoParrafo/${id}`, function(response) {
            if(response.estado) {
                const iframe = document.getElementById('iframeContenidoHistorico');
                if (iframe) {
                    const iframeDoc = iframe.contentWindow.document;
                    iframeDoc.open();
                    iframeDoc.write(response.objeto.contenido);
                    iframeDoc.close();
                }
                $('#modalVerVersion').modal('show');
            } else {
                Swal.fire("Error", response.mensajes, "error");
            }
        }).fail(() => Swal.fire("Error de Comunicación", "No se pudo obtener el contenido de la versión.", "error"));
    });

    $("#btnCerrarVerVersion").click(() => $('#modalVerVersion').modal('hide'));

    cargarDatos();
});
