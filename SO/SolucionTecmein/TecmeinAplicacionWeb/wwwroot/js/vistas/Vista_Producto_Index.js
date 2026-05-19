const MODELO_VISITA_PRODUCTOS = {
    secuencial: 0,
    secuencialVisita: 0,
    tipoEquipo: "",
    sistema: "",
    marca: "",
    capacidad: "",
    velocidad: "",
    salaMaquinas: "",
    salaControl: "",
    numeroPersonas: 0,
    numeroParadas: 0,
    nombreParadas: "",
    embarque: "",
    tipoDucto: "",
    medidasDuctoAF: "",
    tipoMotor: "",
    foso: 0,
    recorrido: 0,
    ingresosFrontales: 0,
    ingresosPosteriores: 0,
    sobrerecorrido: 0,
    dimensionEntrada: 0,
    alturaEntrePisos: "",
    materialPuertas: "",
    energia: "",
    cantidad: "",
    estaActivo: 1,
}

let secVisitaProducto = 0;
let nombreProyecto = "";
let direccionProyecto = "";
let nombreProvincia = "";
let nombreCanton = "";
let nombreParroquia = "";
let contactoNombreCompleto = "";
let contactoCorreo = "";
let contactoTelefono = "";

function limpiarFormularioEquipos() {
    // Resetear todos los campos del formulario de equipos a sus valores por defecto
    $('#cboTipoEquipo').val('');
    $('#cboSistema').val('');
    $('#cboMarca').val('');
    $('#cboSalaMaquinas').val('');
    $('#cboTipoMotor').val('');
    $('#cboVelocidad').val('');
    $('#txtCapacidad').val('');
    $('#txtNumPersonas').val('');
    $('#txtNumParadas').val('');
    $('#txtNombresParadas').val('');
    $('#cboTipoEmbarque').val('');
    $('#cboTipoDucto').val('');
    $('#txtMedidasDuctoAF').val('');
    $('#txtFoso').val('');
    $('#txtRecorrido').val('');
    $('#txtEntradasFrontales').val('');
    $('#txtEntradasPosterior').val('');
    $('#txtSobreRecorrido').val('');
    $('#cboTipoEnergia').val('');
    $('#txtAlturaEntrePisos').val('');
    $('#txtDimencionEntrada').val('');
    $('#cboTipoMaterial').val('');
    $('#txtCantidad').val('');

    // Quitar clases de validación por si quedaron de una interacción anterior
    $('.is-invalid').removeClass('is-invalid');
}


$("#tbdata tbody").on("click", ".btn-detalles", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    secVisitaProducto = data.Secuencial;
    nombreProyecto = data.Nombre;
    direccionProyecto = data.Direccion;
    nombreProvincia = data.NombreProvincia;
    nombreCanton = data.NombreCanton;
    nombreParroquia = data.NombreParroquia;

    limpiarFormularioEquipos(); // Limpiar el formulario antes de cargar nuevos datos

    if (secVisitaProducto != 0)
        ProcesoCargaLista(secVisitaProducto)

    $("#modalDataDetalleVisita").modal("show")
})


$("#btnAgregarItem").click(function () {

    // Función auxiliar para manejar el fallo de validación
    const handleValidationError = (message, focusElement) => {
        toastr.warning("", message);
        $(focusElement).focus();
        // Asegurarse de que los acordeones permanezcan abiertos
        $('#collapseGeneralInfo').addClass('show');
        $('#collapseInstallationDetails').addClass('show');
        return true; // Indica que hubo un error
    };

    // --- VALIDACIÓN DE CAMPOS OBLIGATORIOS ---
    let hasError = false;

    // Validar Selects
    const requiredSelects = [
        { id: '#cboTipoEquipo', name: 'Tipo Equipo' },
        { id: '#cboSistema', name: 'Sistema' },
        { id: '#cboMarca', name: 'Marca' },
        { id: '#cboVelocidad', name: 'Velocidad' },
        { id: '#cboSalaMaquinas', name: 'Sala Máquinas' },
        { id: '#cboTipoEmbarque', name: 'Tipo de Embarque' },
        { id: '#cboTipoDucto', name: 'Ducto De' },
        { id: '#cboTipoMotor', name: 'Tipo Motor' },
        { id: '#cboTipoEnergia', name: 'Tipo Energía' },
        { id: '#cboTipoMaterial', name: 'Material de Puertas' }
    ];

    for (const select of requiredSelects) {
        if ($(select.id).val() === "" || $(select.id).val() === null) {
            hasError = handleValidationError(`Debe seleccionar un valor para "${select.name}".`, select.id);
            if (hasError) return;
        }
    }

    // Validar Inputs de texto/número
    const requiredInputs = [
        { id: '#txtCapacidad', name: 'Capacidad de Carga' },
        { id: '#txtNumPersonas', name: 'N° Personas' },
        { id: '#txtNumParadas', name: 'Número Paradas' },
        { id: '#txtFoso', name: 'Foso' },
        { id: '#txtRecorrido', name: 'Recorrido' },
        { id: '#txtEntradasFrontales', name: 'Entradas Frontales' },
        { id: '#txtEntradasPosterior', name: 'Entradas Posteriores' },
        { id: '#txtSobreRecorrido', name: 'Sobre Recorrido' },
        { id: '#txtAlturaEntrePisos', name: 'Altura Entre Pisos' },
        { id: '#txtDimencionEntrada', name: 'Dimensión Entrada' },
        { id: '#txtCantidad', name: 'Cantidad' }
    ];

    for (const input of requiredInputs) {
        if (($(input.id).val() || "").trim() === "") {
            hasError = handleValidationError(`Debe ingresar un valor para "${input.name}".`, input.id);
            if (hasError) return;
        }
    }
    // --- FIN VALIDACIÓN DE CAMPOS OBLIGATORIOS ---


    // Validación de formato para campos de dimensión
    const camposDimension = [
        { id: '#txtMedidasDuctoAF', name: 'Medidas Ducto A-F' },
        { id: '#txtDimencionEntrada', name: 'Dimensión Entrada' }
    ];

    for (const campo of camposDimension) {
        const valor = ($(campo.id).val() || "").trim();
        if (valor && !/^\d+\s*\*\s*\d+$/.test(valor)) {
            handleValidationError(`El formato para "${campo.name}" no es válido. Debe ser NÚMERO * NÚMERO.`, campo.id);
            return;
        }
    }

    // Validación para Nombres Paradas
    const nombresParadas = ($('#txtNombresParadas').val() || "").trim();
    const numParadas = parseInt($('#txtNumParadas').val() || "0");
    const nombresParadasInput = document.getElementById('txtNombresParadas');

    if (!validarNombresParadas(nombresParadas, numParadas, nombresParadasInput)) {
        handleValidationError("El número de nombres de paradas no coincide con el número de paradas.", nombresParadasInput);
        return;
    }

    // Validación para Entradas Frontales y Posteriores
    const entradasFrontales = parseInt($('#txtEntradasFrontales').val() || '0');
    const entradasPosteriores = parseInt($('#txtEntradasPosterior').val() || '0');
    const numParadasVal = parseInt($('#txtNumParadas').val() || '0');

    if (!validarSumaEntradas(entradasFrontales, entradasPosteriores, numParadasVal, document.getElementById('txtEntradasFrontales'), document.getElementById('txtEntradasPosterior'))) {
        handleValidationError("Verifique las entradas frontales y posteriores. La suma debe ser coherente con el número de paradas.", '#txtEntradasFrontales');
        return;
    }

    const modeloVisitaProductos = structuredClone(MODELO_VISITA_PRODUCTOS);
    modeloVisitaProductos["secVisita"] = secVisitaProducto;
    modeloVisitaProductos["tipoEquipo"] = ($("#cboTipoEquipo").val() || "").trim();
    modeloVisitaProductos["sistema"] = ($("#cboSistema").val() || "").trim();
    modeloVisitaProductos["marca"] = ($("#cboMarca").val() || "").trim();

    // Ajuste para campos numéricos de selects: null si es placeholder, no 0
    const capacidadVal = ($("#txtCapacidad").val() || "").trim();
    modeloVisitaProductos["capacidad"] = capacidadVal === "" ? null : parseInt(capacidadVal);

    const velocidadVal = ($("#cboVelocidad").val() || "").trim();
    modeloVisitaProductos["velocidad"] = velocidadVal === "" ? null : parseFloat(velocidadVal);

    modeloVisitaProductos["salaMaquinas"] = ($("#cboSalaMaquinas").val() || "").trim();
    modeloVisitaProductos["salaControl"] = ($("#cboSalaMaquinas").val() || "").trim(); // Mantengo la asignación actual

    const numPersonasVal = ($("#txtNumPersonas").val() || "").trim();
    modeloVisitaProductos["numeroPersonas"] = numPersonasVal === "" ? null : parseInt(numPersonasVal);

    const numParadasModelVal = ($("#txtNumParadas").val() || "").trim();
    modeloVisitaProductos["numeroParadas"] = numParadasModelVal === "" ? null : parseInt(numParadasModelVal);

    modeloVisitaProductos["nombreParadas"] = ($("#txtNombresParadas").val() || "").trim();
    modeloVisitaProductos["embarque"] = ($("#cboTipoEmbarque").val() || "").trim();
    modeloVisitaProductos["tipoDucto"] = ($("#cboTipoDucto").val() || "").trim();
    modeloVisitaProductos["medidasDuctoAF"] = ($("#txtMedidasDuctoAF").val() || "").trim();
    modeloVisitaProductos["tipoMotor"] = ($("#cboTipoMotor").val() || "").trim();

    const fosoVal = ($("#txtFoso").val() || "").trim();
    modeloVisitaProductos["foso"] = fosoVal === "" ? null : parseInt(fosoVal);

    const recorridoVal = ($("#txtRecorrido").val() || "").trim();
    modeloVisitaProductos["recorrido"] = recorridoVal === "" ? null : parseInt(recorridoVal);

    const entradasFrontalesVal = ($("#txtEntradasFrontales").val() || "").trim();
    modeloVisitaProductos["ingresosFrontales"] = entradasFrontalesVal === "" ? null : parseInt(entradasFrontalesVal);

    const entradasPosterioresVal = ($("#txtEntradasPosterior").val() || "").trim();
    modeloVisitaProductos["ingresosPosteriores"] = entradasPosterioresVal === "" ? null : parseInt(entradasPosterioresVal);

    const sobrerecorridoVal = ($("#txtSobreRecorrido").val() || "").trim();
    modeloVisitaProductos["sobrerecorrido"] = sobrerecorridoVal === "" ? null : parseInt(sobrerecorridoVal);

    const dimensionEntradaVal = ($("#txtDimencionEntrada").val() || "").trim();
    modeloVisitaProductos["dimensionEntrada"] = dimensionEntradaVal === "" ? null : parseInt(dimensionEntradaVal);

    const alturaEntrePisosVal = ($("#txtAlturaEntrePisos").val() || "").trim();
    modeloVisitaProductos["alturaEntrePisos"] = alturaEntrePisosVal === "" ? null : parseInt(alturaEntrePisosVal);

    modeloVisitaProductos["materialPuertas"] = ($("#cboTipoMaterial").val() || "").trim();
    modeloVisitaProductos["energia"] = ($("#cboTipoEnergia").val() || "").trim();

    const cantidadVal = ($("#txtCantidad").val() || "").trim();
    modeloVisitaProductos["cantidad"] = cantidadVal === "" ? null : parseInt(cantidadVal);

    modeloVisitaProductos["esActivo"] = $("#cboEstado").val();

    const datoProductoItem = new FormData();
    datoProductoItem.append("modelo", JSON.stringify(modeloVisitaProductos));


    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch("ProcesoGuardasEquipoVisita", {
        method: "POST",
        body: datoProductoItem
    })
        .then(response => {
            $("#modalData")
                .find("div.modal-content")
                .LoadingOverlay("hide");
            return response.ok
                ? response.json()
                : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                tablaDataPro.row.add(responseJson.objeto).draw(false);
                $("#modalData").modal("hide");
                Swal.fire("Listo!",
                    "Equipo Agregado Con Exito ",
                    "success");
            }
            else {
                Swal.fire("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(async error => {
            $("#modalData").find("div.modal-content").LoadingOverlay("hide");
            if (error.status === 403) {
                Swal.fire("Acceso Denegado", "No tiene permisos para actualizar equipos de visita.", "error");
            } else {
                // For other non-OK responses, try to get a more specific message if available
                const errorText = await error.text(); // error is the Response object here
                Swal.fire("Error!", `Error en la respuesta del servidor: ${errorText || error.statusText}`, "error");
            }
        });

});

let fechaMaquina = new Date();
async function ProcesoCargaLista(secuencialVisita) {
    try {
        if ($.fn.DataTable.isDataTable('#tbDataItems')) {
            $('#tbDataItems').DataTable().destroy();
        }

        const response = await $.ajax({
            url: `EquiposDeVisita?secuencialVisita=${secuencialVisita}`,
            type: "GET",
            dataType: "json"
        });

        // Acceder a la propiedad $values o data.$values
        const data = (response.data && response.data.$values) ? response.data.$values : (response.$values || response);

        if (Array.isArray(data)) {
            tablaDataPro = $('#tbDataItems').DataTable({
                responsive: true,
                data: data,
                columns: [
                    { data: "secuencial", visible: false },
                    { data: "cantidad", searchable: false, width: "10%", className: "text-center" },
                    { data: "detalleEspecifico", searchable: true, width: "80%" },
                    { data: "descripcionImpresa", visible: false },
                    {
                        "defaultContent":
                            '<div class="btn-group" role="group"><button class="btn btn-danger btn-eliminar-equipo btn-sm"><i class="fas fa-trash-alt"></i></button></div>',
                        "orderable": true,
                        "searchable": false,
                        "width": "10%",
                        "className": "text-center"
                    }
                ],
                "createdRow": function (row, data, dataIndex) {
                    $(row).find('th').addClass('th-celeste');
                },
                order: [[0, "desc"]],
                dom: '<"top"Bf>rt<"bottom"lip><"clear">',
                paging: false, // Deshabilitar paginación
                info: false,   // Deshabilitar información de paginación
                searching: false, // Deshabilitar búsqueda
                buttons: [
                    {
                        text: '<i class="fas fa-file-excel"></i>',
                        extend: 'excelHtml5',
                        title: 'Detalle Equipos Visita ' + nombreProyecto,
                        filename: 'Detalle Equipos Visita ' + nombreProyecto + ' ' + fechaMaquina.toLocaleDateString(),
                        exportOptions: {
                            columns: [1, 3]
                        },
                        className: 'btn-success',
                        titleAttr: 'Exportar a Excel'
                    },
                    {
                        text: '<i class="fas fa-file-pdf"></i>',
                        extend: 'pdfHtml5',
                        title: 'Detalle Equipos Visita ' + nombreProyecto,
                        filename: 'Detalle Equipos Visita ' + nombreProyecto + ' ' + fechaMaquina.toLocaleDateString(),
                        exportOptions: {
                            columns: [1, 3]
                        },
                        className: 'btn-danger',
                        titleAttr: 'Exportar a PDF',
                        customize: function (doc) {
                            let contactoData = null;
                            // Obtener el contacto principal de forma síncrona
                            $.ajax({
                                url: `/Contacto/ObtenerContactoPrincipal?secuencialVisita=${secVisitaProducto}`,
                                type: "GET",
                                dataType: "json",
                                async: false, // Importante para que los datos estén disponibles antes de generar el PDF
                                success: function (responseJson) {
                                    if (responseJson.estado && responseJson.objeto) {
                                        contactoData = responseJson.objeto;
                                    }
                                },
                                error: function (xhr, status, error) {
                                    console.error("Error al obtener el contacto principal:", error);
                                }
                            });

                            const now = new Date();
                            const printDate = `${now.toLocaleDateString()} ${now.toLocaleTimeString()} `;

                            // Construir el contenido del encabezado
                            const headerContent = [
                                { text: ' Propuesta ', bold: true, fontSize: 14, alignment: 'center', margin: [0, 0, 0, 10] },
                                {
                                    columns: [
                                        { width: 80, text: [{ text: 'Proyecto: ', bold: true }] },
                                        { width: '*', text: nombreProyecto }
                                    ]
                                },
                                {
                                    columns: [
                                        { width: 80, text: [{ text: 'Ubicación: ', bold: true }] },
                                        { width: '*', text: `${nombreProvincia} - ${nombreCanton} - ${nombreParroquia} ` }
                                    ]
                                },
                                {
                                    columns: [
                                        { width: 80, text: [{ text: 'Dirección: ', bold: true }] },
                                        { width: '*', text: direccionProyecto }
                                    ]
                                },
                                {
                                    columns: [
                                        { width: 80, text: [{ text: 'Contacto: ', bold: true }] },
                                        { width: '*', text: contactoData ? `${contactoData.nombres} ${contactoData.apellidos} ` : 'N/A' }
                                    ]
                                },
                                {
                                    columns: [
                                        { width: 80, text: [{ text: 'Correo: ', bold: true }] },
                                        { width: '*', text: contactoData ? contactoData.correo : 'N/A' }
                                    ]
                                },
                                {
                                    columns: [
                                        { width: 80, text: [{ text: 'Teléfono: ', bold: true }] },
                                        { width: '*', text: contactoData ? contactoData.telefono : 'N/A' }
                                    ]
                                },
                                { text: `Impreso: ${printDate} `, alignment: 'right', fontSize: 5, margin: [0, 10, 0, 0] }
                            ];

                            // Añadir el encabezado al documento
                            doc.content.splice(0, 0, { stack: headerContent, margin: [0, 0, 0, 12] });

                            const tableNode = doc.content[2];

                            if (tableNode && tableNode.table) {
                                // Definir estilos personalizados
                                doc.styles = doc.styles || {};
                                doc.styles.customGridHeader = {
                                    bold: true,
                                    fontSize: 12, // Tamaño de fuente ajustado
                                    fillColor: '#D7E1F5',
                                    alignment: 'center',
                                    color: '#000000', // Color de texto negro para mejor contraste
                                    margin: [0, 5, 0, 5]
                                };
                                doc.styles.cellStyle = {
                                    margin: [0, 5, 0, 5],
                                    alignment: 'center'
                                };

                                tableNode.table.body.forEach((row, i) => {
                                    if (i === 0) { // Fila de encabezado
                                        // Celda de Cantidad
                                        const headerCell1 = row[0];
                                        if (headerCell1) {
                                            headerCell1.text = 'Cantidad';
                                            headerCell1.style = 'customGridHeader';
                                            headerCell1.alignment = 'center';
                                        }

                                        // Celda de Descripción
                                        const headerCell2 = row[1];
                                        if (headerCell2) {
                                            headerCell2.text = 'Descripción del Equipo Solicitado';
                                            headerCell2.style = 'customGridHeader';
                                            headerCell2.alignment = 'left';
                                        }

                                        // Añadir nueva celda de encabezado 'Verificado'
                                        row.push({
                                            text: 'Verificado',
                                            style: 'customGridHeader',
                                            alignment: 'center'
                                        });
                                    } else {
                                        // Determinar el color de fondo para el efecto cebra (filas de datos impares son grises)
                                        const rowFillColor = (i % 2 !== 0) ? '#F5F5F5' : null;

                                        // Aplicar el color de fondo a todas las celdas existentes en la fila
                                        row.forEach(cell => {
                                            if (rowFillColor) {
                                                cell.fillColor = rowFillColor;
                                            } else {
                                                delete cell.fillColor; // Asegura que las filas pares sean blancas
                                            }
                                        });

                                        // Centrar la primera columna de datos
                                        if (row[0]) {
                                            row[0].alignment = 'center';
                                        }

                                        // Crear la nueva celda para la columna 'Verificado'
                                        const newCell = {
                                            canvas: [
                                                {
                                                    type: 'rect',
                                                    x: 45, // Ajustado para mejor centrado
                                                    y: 10,
                                                    w: 10,
                                                    h: 10,
                                                    r: 2,
                                                    lineColor: '#000000',
                                                    lineWidth: 1
                                                }
                                            ],
                                            style: 'cellStyle'
                                        };

                                        // Aplicar el color de fondo determinado a la nueva celda
                                        if (rowFillColor) {
                                            newCell.fillColor = rowFillColor;
                                        }

                                        row.push(newCell);
                                    }
                                });

                                // Ajustar el ancho de las columnas para incluir la nueva columna
                                // La suma debe ser '*' o un número fijo. Asumamos que el ancho total es 510.
                                // Ejemplo: ['10%', '70%', '20%']
                                tableNode.table.widths = ['15%', '65%', '20%'];
                            }

                        }
                    },
                    {
                        text: '<i class="fas fa-sync-alt"></i>', // Icono de sincronización
                        className: 'btn-info', // Clase para el estilo del botón
                        titleAttr: 'Sincronizar Equipos con Cotización', // Tooltip
                        action: function (e, dt, node, config) {
                            // Lógica para llamar al backend para sincronizar
                            Swal.fire({
                                title: "¿Sincronizar Equipos?",
                                text: "Esto añadirá los equipos de esta visita a la cotización activa si no están presentes.",
                                icon: "info",
                                showCancelButton: true,
                                confirmButtonColor: "btn-primary",
                                confirmButtonText: "Sí, sincronizar",
                                cancelButtonText: "No, cancelar",
                                reverseButtons: true
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    $(".showSweetAlert").LoadingOverlay("show");
                                    fetch(`/Visita/SincronizarEquipos?secuencialVisita=${secVisitaProducto}`, {
                                        method: "POST" // Usar POST para una acción que modifica datos
                                    })
                                        .then(response => {
                                            $(".showSweetAlert").LoadingOverlay("hide");
                                            return response.ok ? response.json() : Promise.reject(response);
                                        })
                                        .then(responseJson => {
                                            if (responseJson.estado) {
                                                Swal.fire("Listo!", "Equipos sincronizados exitosamente.", "success");
                                                // Opcional: recargar la tabla de equipos si la sincronización afecta su estado visual
                                                // tablaDataPro.ajax.reload();
                                            } else {
                                                Swal.fire("Error", responseJson.mensajes, "error");
                                            }
                                        })
                                        .catch(err => {
                                            $(".showSweetAlert").LoadingOverlay("hide");
                                            Swal.fire("Error", "No se pudo conectar con el servidor.", "error");
                                        });
                                }
                            });
                        }
                    }
                ],

                language: {
                    url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
                }
            });

            // Ajustar columnas de DataTables cuando el modal se muestra
            $('#modalDataDetalleVisita').on('shown.bs.modal', function () {
                tablaDataPro.columns.adjust().draw();
            });

        } else {
            console.error("La respuesta no es un array:", response);
            Swal.fire("Error!", "La respuesta del servidor no es válida.", "error");
        }

    } catch (error) {
        console.error("Error en el proceso:", error);
        Swal.fire("Error!", error.message, "error");
    }
}

// #region Validaciones de Entrada

// Función para validar que solo se ingresen números enteros
function validarEntradaEntero(event) {
    const input = event.target;
    input.value = input.value.replace(/[^0-9]/g, '');
}

// Función para validar el formato específico #*# (números y un asterisco)
function validarEntradaDimension(event) {
    const input = event.target;
    let value = input.value;

    // Permitir solo números y el carácter '*'
    value = value.replace(/[^0-9*]/g, '');

    // Asegurar que solo haya un '*' 
    const parts = value.split('*');
    if (parts.length > 2) {
        value = parts[0] + '*' + parts.slice(1).join('');
    }

    input.value = value;
}

// Función para validar el formato en el evento blur
function validarFormatoDimensionBlur(event) {
    const input = event.target;
    const valor = $(input).val().trim();
    const regex = /^\d+\s*\*\s*\d+$/;

    // Si el campo tiene valor pero no cumple el formato, marcar como inválido
    if (valor && !regex.test(valor)) {
        $(input).addClass('is-invalid');
        $(input).next('.invalid-feedback').css('display', 'block'); // Mostrar el mensaje de error
    } else {
        // Si está vacío o es válido, remover la marca
        $(input).removeClass('is-invalid');
        $(input).next('.invalid-feedback').css('display', 'none'); // Ocultar el mensaje de error
    }
}

// Función para validar la suma de entradas frontales y posteriores
function validarSumaEntradas() {
    const numParadas = parseInt($('#txtNumParadas').val() || '0');
    const entradasFrontales = parseInt($('#txtEntradasFrontales').val() || '0');
    const entradasPosteriores = parseInt($('#txtEntradasPosterior').val() || '0');

    const sumaEntradas = entradasFrontales + entradasPosteriores;
    const limiteSuperior = 2 * numParadas;
    const limiteInferior = numParadas;

    let isValid = true;
    let errorMessage = "";
    let esFrontal = false;
    let esPosteror = false;


    if (entradasFrontales > numParadas) {
        errorMessage = "Entradas Frontales no puede ser superior al Número de Paradas.";
        esFrontal = true;
        isValid = false;
    }

    if (entradasPosteriores > numParadas) {
        errorMessage = "Entradas Posteriores no puede ser superior al Número de Paradas.";
        esPosteror = true;
        isValid = false;
    }

    if (isValid && sumaEntradas > limiteSuperior) {
        esPosteror = esFrontal = true;
        errorMessage = "La suma de Entradas Frontales y Posteriores es superior al Número de Paradas.";
        isValid = false;
    }

    if (isValid && sumaEntradas < limiteInferior) {
        esPosteror = esFrontal = true;
        errorMessage = "La suma de Entradas Frontales y Posteriores debe ser menor o igual al Número de Paradas.";
        isValid = false;
    }


    if (isValid) {

        $('#txtEntradasFrontales').removeClass('is-invalid');
        $('#txtEntradasFrontales').next('.invalid-feedback').css('display', 'none');

        $('#txtEntradasPosterior').removeClass('is-invalid');
        $('#txtEntradasPosterior').next('.invalid-feedback').css('display', 'none');


    } else {
        if (esFrontal) {
            $('#txtEntradasFrontales').addClass('is-invalid');
            $('#txtEntradasFrontales').next('.invalid-feedback').text(errorMessage).css('display', 'block');
        }

        if (esPosteror) {
            $('#txtEntradasPosterior').addClass('is-invalid');
            $('#txtEntradasPosterior').next('.invalid-feedback').text(errorMessage).css('display', 'block');
        }

    }

    return isValid;
}


// Asignar los eventos de validación a los campos correspondientes
$(document).ready(function () {
    // Campos de solo enteros
    const camposEnteros = [
        '#txtCapacidad', '#txtNumPersonas', '#txtNumParadas', '#txtFoso',
        '#txtRecorrido', '#txtEntradasFrontales', '#txtEntradasPosterior',
        '#txtSobreRecorrido', '#txtCantidad', '#txtAlturaEntrePisos'
    ];
    camposEnteros.forEach(id => {
        $(document).on('input', id, validarEntradaEntero);
    });

    // Campos con formato #*#
    const camposDimension = ['#txtMedidasDuctoAF', '#txtDimencionEntrada'];
    camposDimension.forEach(id => {
        $(document).on('input', id, validarEntradaDimension); // Restricción de tipeo
        $(document).on('blur', id, validarFormatoDimensionBlur);  // Validación de formato al salir
    });

    // Validación para Nombre Paradas
    $(document).on('blur', '#txtNombresParadas', function () {
        const nombresParadas = $(this).val().trim();
        const numParadas = parseInt($('#txtNumParadas').val());
        validarNombresParadas(nombresParadas, numParadas, this);
    });

    // Validación para suma de entradas al perder el foco
    $(document).on('blur', '#txtNumParadas', validarSumaEntradas);
    $(document).on('blur', '#txtEntradasFrontales', validarSumaEntradas);
    $(document).on('blur', '#txtEntradasPosterior', validarSumaEntradas);
});

// #endregion

// Función de validación para Nombres Paradas
function validarNombresParadas(nombresParadas, numParadas, inputElement) {
    let isValid = true;
    if (nombresParadas && !isNaN(numParadas)) {
        const nombresArray = nombresParadas.split('-').map(item => item.trim()).filter(item => item !== '');
        if (nombresArray.length !== numParadas) {
            isValid = false;
        }
    } else if (nombresParadas && isNaN(numParadas)) {
        // Si hay nombres pero no hay número de paradas válido
        isValid = false;
    }

    if (!isValid) {
        $(inputElement).addClass('is-invalid');
        $(inputElement).next('.invalid-feedback').css('display', 'block'); // Mostrar el mensaje de error
    } else {
        $(inputElement).removeClass('is-invalid');
        $(inputElement).next('.invalid-feedback').css('display', 'none'); // Ocultar el mensaje de error
    }
    return isValid;
}

// Evento para el botón de eliminar equipo
$(document).on("click", ".btn-eliminar-equipo", function () {
    const fila = $(this).closest("tr");
    const data = tablaDataPro.row(fila).data();

    Swal.fire({
        title: "¿Está Seguro de Eliminar?",
        text: `Eliminar el equipo "${data.detalleEspecifico}"`, // Usar detalleEspecifico para el mensaje
        icon: "warning",
        showCancelButton: true,
        customClass: {
            confirmButton: 'btn-danger' // Correcto para aplicar clases CSS
        },
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "No, cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`/visita/ProcesoEliminarEquipoVisita?secuencialEquipoVisita=${data.secuencial}`,
                { method: "DELETE" }
            )
                .then(response => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        tablaDataPro.row(fila).remove().draw(false);
                        Swal.fire("Listo!", "El equipo fue eliminado.", "success");
                    } else {
                        Swal.fire("Error", responseJson.mensajes, "error");
                    }
                })
                .catch(async error => {
                    $(".showSweetAlert").LoadingOverlay("hide");
                    let errorMessage = "Ocurrió un error desconocido al eliminar el equipo.";
                    if (error.status === 403) {
                        errorMessage = "No tiene permisos para eliminar equipos de visita.";
                    } else if (error instanceof Response && typeof error.text === 'function') {
                        const errorText = await error.text();
                        errorMessage = `Error en la respuesta del servidor: ${errorText || error.statusText} `;
                    } else if (error.message) {
                        errorMessage = `Error: ${error.message} `;
                    }
                    Swal.fire("Error", errorMessage, "error");
                });
        }
    });
});