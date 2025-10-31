$(document).ready(function () {
    var tablaPreContratos;
    let totalCotizacion = 0;

    const spanishLanguage = {
        "url": "/js/datatables/i18n/Spanish.json"
    };

    // --- Lógica de UI y Validaciones para Compromisos ---
    function reEnumerarCuotas() {
        $("#tablaCompromisos tbody tr").each(function(index) {
            $(this).find(".numero-cuota").text(index + 1);
        });
    }

    function validarSumaCompromisos() {
        let sumaCompromisos = 0;
        $("#tablaCompromisos tbody .monto-compromiso").each(function() {
            const monto = parseFloat($(this).val());
            if (!isNaN(monto)) {
                sumaCompromisos += monto;
            }
        });

        const displaySuma = $("#sumaCompromisosDisplay");
        displaySuma.text(sumaCompromisos.toFixed(2));

        if (totalCotizacion > 0 && sumaCompromisos.toFixed(2) != totalCotizacion.toFixed(2)) {
            displaySuma.removeClass("text-success").addClass("text-danger");
        } else if (totalCotizacion > 0 && sumaCompromisos.toFixed(2) == totalCotizacion.toFixed(2)) {
            displaySuma.removeClass("text-danger").addClass("text-success");
        } else {
            displaySuma.removeClass("text-success text-danger");
        }
    }

    function validarFechasEnConflicto() {
        let hayConflicto = false;
        const filas = $("#tablaCompromisos tbody tr");
        filas.find('.fecha-compromiso').removeClass('is-invalid');

        for (let i = 1; i < filas.length; i++) {
            const fechaAnteriorStr = $(filas[i-1]).find('.fecha-compromiso').val();
            const fechaActualStr = $(filas[i]).find('.fecha-compromiso').val();

            if (fechaAnteriorStr && fechaActualStr) {
                const fechaAnterior = new Date(fechaAnteriorStr + 'T00:00:00');
                const fechaActual = new Date(fechaActualStr + 'T00:00:00');

                if (fechaActual <= fechaAnterior) {
                    hayConflicto = true;
                    $(filas[i-1]).find('.fecha-compromiso').addClass('is-invalid');
                    $(filas[i]).find('.fecha-compromiso').addClass('is-invalid');
                }
            }
        }
        return hayConflicto;
    }

    function validarPrimerCompromiso() {
        const primeraFila = $("#tablaCompromisos tbody tr:first");
        if (primeraFila.length > 0) {
            const tipo = primeraFila.find(".tipo-compromiso").val();
            if (tipo !== "Anticipo") {
                return false;
            }
        }
        return true;
    }

    function generarCuotasInteligentes() {
        const numCuotas = parseInt($("#numCuotas").val());
        const montoBase = parseFloat($("#montoCuota").val());
        const fechaPrimeraCuota = $("#fechaPrimeraCuota").val();

        if (isNaN(numCuotas) || isNaN(montoBase) || !fechaPrimeraCuota) {
            return;
        }
        if (numCuotas <= 0 || montoBase <= 0 || totalCotizacion <= 0) {
            Swal.fire("Datos incompletos", "Se requiere un Total de Cotización, un número de cuotas y un monto base mayores a cero.", "info");
            return;
        }

        if ((numCuotas - 1) * montoBase >= totalCotizacion) {
            Swal.fire("Monto Base Excesivo", "El monto base de la cuota es demasiado alto para el número de cuotas y el total de la cotización.", "error");
            return;
        }

        $("#tablaCompromisos tbody").empty();

        const startDate = new Date(fechaPrimeraCuota + 'T00:00:00');
        let totalAcumulado = 0;

        for (let i = 0; i < numCuotas; i++) {
            let montoActualCuota = montoBase;
            if (i === numCuotas - 1) {
                montoActualCuota = totalCotizacion - totalAcumulado;
            }

            const nuevaFecha = new Date(startDate.getTime());
            nuevaFecha.setDate(nuevaFecha.getDate() + (i * 30));

            const nuevaFila = $("#plantillaCompromiso").clone().removeAttr('id').removeAttr('style');
            
            if (i === 0) {
                nuevaFila.find(".tipo-compromiso").val("Anticipo");
            } else {
                nuevaFila.find(".tipo-compromiso").val("Cuota");
            }

            nuevaFila.find(".monto-compromiso").val(montoActualCuota.toFixed(2));
            nuevaFila.find(".fecha-compromiso").val(nuevaFecha.toISOString().split('T')[0]);
            
            $("#tablaCompromisos tbody").append(nuevaFila);
            totalAcumulado += montoBase;
        }

        reEnumerarCuotas();
        validarSumaCompromisos();
        validarFechasEnConflicto();
    }

    // --- Fin Lógica UI ---

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
                { "data": "estado" },
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
                        return `<div class="btn-group" role="group">${btnEditar}${btnHistorial}${btnAprobar}${btnEliminar}</div>`;
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
        $("#cboCotizacionesAceptadas").val("");
        $("#totalCotizacionDisplay").text("0.00");
        totalCotizacion = 0;
        $("#Dias, #AniosGarantia, #MesesGarantia").val("");
        $("#PeriodoMantenimiento, #PolizaGarantia, #TipoDias").val("");
        $("#tablaCompromisos tbody").empty();
        $("#numCuotas, #montoCuota, #fechaPrimeraCuota").val("");
        $("#SecPreContrato").val("0"); // Limpiar SecPreContrato al crear uno nuevo
        $("#btnGenerarPreContrato").prop("disabled", true); // Deshabilitar botón para nuevos registros
        validarSumaCompromisos();

        $.when(
            cargarDropdown("/PreContrato/ListaCotizacionesAprobadas", "#cboCotizacionesAceptadas"),
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
                        totalCotizacion = 0;
                        $("#totalCotizacionDisplay").text("0.00");
                        Swal.fire("Error", "La respuesta no contiene los datos esperados.", "error");
                        return;
                    }
                    totalCotizacion = parseFloat(response.subtotal) + parseFloat(response.valorImpuestos);
                    $("#totalCotizacionDisplay").text(isNaN(totalCotizacion) ? "0.00" : totalCotizacion.toFixed(2));
                    validarSumaCompromisos();
                },
                error: () => {
                    totalCotizacion = 0;
                    $("#totalCotizacionDisplay").text("0.00");
                    Swal.fire("Error", "No se pudieron obtener los detalles de la cotización.", "error");
                }
            });
        } else {
            totalCotizacion = 0;
            $("#totalCotizacionDisplay").text("0.00");
            validarSumaCompromisos();
        }
    });

    // --- Lógica para Compromisos de Pago ---
    $("#modalPreContrato").on('click', '#btnAnadirCompromiso', function() {
        const nuevaFila = $("#plantillaCompromiso").clone().removeAttr('id').removeAttr('style');
        $("#tablaCompromisos tbody").append(nuevaFila);
        reEnumerarCuotas();
    });

    $("#tablaCompromisos").on('click', '.btn-eliminar-compromiso', function() {
        $(this).closest('tr').remove();
        reEnumerarCuotas();
        validarSumaCompromisos();
        validarFechasEnConflicto();
    });

    $("#modalPreContrato").on('click', '#btnGenerarCuotas', generarCuotasInteligentes);
    $("#modalPreContrato").on('change', '#numCuotas', generarCuotasInteligentes);
    $("#modalPreContrato").on('change', '#montoCuota', generarCuotasInteligentes);

    $("#tablaCompromisos tbody").on('change', '.fecha-compromiso', validarFechasEnConflicto);
    $("#tablaCompromisos tbody").on('change', '.monto-compromiso', validarSumaCompromisos);
    // --- Fin Lógica ---

    let datosFormularioParaGuardar = null;

    function recolectarYValidarDatos() {
        if (!validarPrimerCompromiso()) {
            Swal.fire("Error", "El primer compromiso de pago debe ser de tipo 'Anticipo'.", "error");
            return null;
        }
        if (validarFechasEnConflicto()) {
            Swal.fire("Error", "Por favor, revise las fechas marcadas en rojo. Una fecha no puede ser anterior o igual a la precedente.", "error");
            return null;
        }

        const compromisos = [];
        $("#tablaCompromisos tbody tr").each(function() {
            const fila = $(this);
            const compromiso = {
                Tipo: fila.find(".tipo-compromiso").val(),
                Monto: parseFloat(fila.find(".monto-compromiso").val()),
                FechaVencimiento: fila.find(".fecha-compromiso").val()
            };
            if (!isNaN(compromiso.Monto) && compromiso.Monto > 0 && compromiso.FechaVencimiento) {
                compromisos.push(compromiso);
            }
        });

        const modelo = {
            SecPreContrato: parseInt($("#SecPreContrato").val()), // Incluir SecPreContrato
            SecCotizacion: parseInt($("#cboCotizacionesAceptadas").val()),
            Dias: parseInt($("#Dias").val()),
            TipoDias: $("#TipoDias").val(),
            PeriodoMantenimiento: $("#PeriodoMantenimiento").val(),
            AniosGarantia: parseInt($("#AniosGarantia").val()),
            MesesGarantia: parseInt($("#MesesGarantia").val()),
            PolizaGarantia: $("#PolizaGarantia").val(),
            CompromisosDePago: compromisos
        };

        if (!modelo.SecCotizacion) {
            Swal.fire("Atención", "Por favor, seleccione una cotización.", "warning");
            return null;
        }
        return modelo;
    }

    $("#btnGenerarPreContrato").click(function() {
        const modelo = recolectarYValidarDatos();
        if (!modelo) return;

        datosFormularioParaGuardar = modelo;

        $.ajax({
            url: "/PreContrato/GenerarVistaPreviaConPagos",
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

    $("#btnGuardarBorrador").click(function() {
        const modelo = recolectarYValidarDatos();
        if (!modelo) return;

        $.ajax({
            url: "/PreContrato/GuardarBorrador",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(modelo),
            beforeSend: () => Swal.fire({ title: 'Guardando Borrador...', allowOutsideClick: false, didOpen: () => Swal.showLoading() }),
            success: function(response) {
                Swal.close();
                if (response.estado) {
                    $('#modalPreContrato').modal('hide');
                    tablaPreContratos.ajax.reload(null, false);
                    Swal.fire("¡Guardado!", response.mensajes, "success");
                } else {
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: () => Swal.fire("Error", "No se pudo guardar el borrador.", "error")
        });
    });

    $("#btnGuardarDefinitivo").click(function() {
        if (!datosFormularioParaGuardar) {
            Swal.fire("Error", "No hay datos para guardar.", "error");
            return;
        }

        const contenidoHtml = $("#contenidoVistaPrevia").html();

        const payload = {
            ...datosFormularioParaGuardar,
            ContenidoHtml: contenidoHtml
        };

        $.ajax({
            url: "/PreContrato/CrearDesdeModalConPagos",
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
                    Swal.fire("Error", response.mensajes, "error");
                }
            },
            error: () => Swal.fire("Error", "No se pudo comunicar con el servidor.", "error")
        });
    });

    $("#tablaPreContratos tbody").on("click", ".btn-editar", function (e) {
        e.preventDefault();
        const id = $(this).data("id");

        // Limpiar el modal antes de cargar nuevos datos
        $("#cboCotizacionesAceptadas").val("");
        $("#totalCotizacionDisplay").text("0.00");
        totalCotizacion = 0;
        $("#Dias, #AniosGarantia, #MesesGarantia").val("");
        $("#PeriodoMantenimiento, #PolizaGarantia, #TipoDias").val("");
        $("#tablaCompromisos tbody").empty();
        $("#numCuotas, #montoCuota, #fechaPrimeraCuota").val("");
        $("#SecPreContrato").val("0"); // Limpiar SecPreContrato al abrir el modal
        validarSumaCompromisos();

        $.when(
            cargarDropdown("/PreContrato/ListaCotizacionesAprobadas", "#cboCotizacionesAceptadas"),
            cargarDropdown("/PolizaGarantia/ListaParaDropdown", "#PolizaGarantia")
        ).done(function() {
            $.ajax({
                url: `/PreContrato/DetallesParaEdicion/${id}`,
                type: "GET",
                beforeSend: () => Swal.fire({ title: 'Cargando Datos...', allowOutsideClick: false, didOpen: () => Swal.showLoading() }),
                success: function(response) {
                    Swal.close();
                    if (response.estado) {
                        const data = response.objeto;

                        // Poblar campos principales
                        $("#SecPreContrato").val(data.secPreContrato); // Establecer el ID del pre-contrato
                        $("#cboCotizacionesAceptadas").val(data.secCotizacion).trigger('change');
                        $("#Dias").val(data.dias);
                        // Corrección temporal para el desajuste 'Hábil' vs 'Hábiles'
                        $("#TipoDias").val(data.tipoDias === "Hábil" ? "Hábiles" : data.tipoDias);
                        $("#PeriodoMantenimiento").val(data.periodoMantenimiento);
                        $("#AniosGarantia").val(data.aniosGarantia);
                        $("#MesesGarantia").val(data.mesesGarantia);
                        $("#PolizaGarantia").val(data.polizaGarantia);

                        // Poblar tabla de compromisos
                        if (data.compromisosDePago && data.compromisosDePago.$values && data.compromisosDePago.$values.length > 0) {
                            data.compromisosDePago.$values.forEach(function(compromiso) {
                                const nuevaFila = $("#plantillaCompromiso").clone().removeAttr('id').removeAttr('style');
                                nuevaFila.find(".tipo-compromiso").val(compromiso.tipo);
                                nuevaFila.find(".monto-compromiso").val(compromiso.monto.toFixed(2));
                                const fecha = new Date(compromiso.fechaVencimiento).toISOString().split('T')[0];
                                nuevaFila.find(".fecha-compromiso").val(fecha);
                                $("#tablaCompromisos tbody").append(nuevaFila);
                            });
                        }

                        reEnumerarCuotas();
                        validarSumaCompromisos();
                        validarFechasEnConflicto();

                        $("#btnGenerarPreContrato").prop("disabled", false); // Habilitar botón para registros existentes
                        $('#modalPreContrato').modal('show');
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
        }).fail(() => Swal.fire("Error", "No se pudo obtener el contenido de la versión.", "error"));
    });

    $("#btnCerrarVerVersion").click(() => $('#modalVerVersion').modal('hide'));

    cargarDatos();
});
