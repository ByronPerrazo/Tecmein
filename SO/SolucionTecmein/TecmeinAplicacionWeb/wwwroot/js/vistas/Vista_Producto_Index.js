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

$("#tbdata tbody").on("click", ".btn-info", function () {

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    secVisitaProducto = data.secuencial;
    nombreProyecto = data.nombre;
    direccionProyecto = data.direccion;
    nombreProvincia = data.nombreProvincia;
    nombreCanton = data.nombreCanton;
    nombreParroquia = data.nombreParroquia;


    if (secVisitaProducto != 0)
        ProcesoCargaLista(secVisitaProducto)

    $("#modalDataDetalleVisita").modal("show")
})


$("#btnAgregarItem").click(function () {

    // Validación de formato para campos de dimensión
    const camposDimension = [
        { id: '#txtMedidasDuctoAF', name: 'Medidas Ducto A-F' },
        { id: '#txtDimencionEntrada', name: 'Dimensión Entrada' }
    ];

    for (const campo of camposDimension) {
        const valor = $(campo.id).val().trim();
        // Se valida solo si el campo tiene algún valor
        if (valor && !/^\d+\s*\*\s*\d+$/.test(valor)) {
            toastr.warning("", `El formato para "${campo.name}" no es válido. Debe ser NÚMERO * NÚMERO.`);
            $(campo.id).focus();
            return; // Detener la ejecución
        }
    }

    // Validación para Nombres Paradas al hacer clic en Agregar
    const nombresParadas = $('#txtNombresParadas').val().trim();
    const numParadas = parseInt($('#txtNumParadas').val());
    const nombresParadasInput = document.getElementById('txtNombresParadas');

    if (!validarNombresParadas(nombresParadas, numParadas, nombresParadasInput)) {
        toastr.warning("", "El número de nombres de paradas no coincide con el número de paradas.");
        $(nombresParadasInput).focus();
        return; // Detener la ejecución
    }

    // Validación para Entradas Frontales y Posteriores
    const entradasFrontales = parseInt($('#txtEntradasFrontales').val() || '0');
    const entradasPosteriores = parseInt($('#txtEntradasPosterior').val() || '0');
    const numParadasVal = parseInt($('#txtNumParadas').val() || '0');

    if (!validarSumaEntradas(entradasFrontales, entradasPosteriores, numParadasVal, document.getElementById('txtEntradasFrontales'), document.getElementById('txtEntradasPosterior'))) {
        toastr.warning("", "Verifique las entradas frontales y posteriores. Cada una no puede exceder el número de paradas, y la suma debe estar entre el número de paradas y el doble de este.");
        return; // Detener la ejecución
    }


    const modeloVisitaProductos = structuredClone(MODELO_VISITA_PRODUCTOS);
    modeloVisitaProductos["secVisita"] = secVisitaProducto;
    modeloVisitaProductos["tipoEquipo"] = $("#cboTipoEquipo").val().trim();
    modeloVisitaProductos["sistema"] = $("#cboSistema").val().trim();
    modeloVisitaProductos["marca"] = $("#cboMarca").val().trim();
    modeloVisitaProductos["capacidad"] = parseInt($("#txtCapacidad").val() || "0");
    modeloVisitaProductos["velocidad"] = parseFloat($("#cboVelocidad").val().trim());
    modeloVisitaProductos["salaMaquinas"] = $("#cboSalaMaquinas").val().trim();
    modeloVisitaProductos["salaControl"] = $("#cboSalaMaquinas").val().trim();
    modeloVisitaProductos["numeroPersonas"] = parseInt($("#txtNumPersonas").val());
    modeloVisitaProductos["numeroParadas"] = parseInt($("#txtNumParadas").val());
    modeloVisitaProductos["nombreParadas"] = $("#txtNombresParadas").val().trim();
    modeloVisitaProductos["embarque"] = $("#cboTipoEmbarque").val();
    modeloVisitaProductos["tipoDucto"] = $("#cboTipoDucto").val();
    modeloVisitaProductos["medidasDuctoAF"] = $("#txtMedidasDuctoAF").val().trim();
    modeloVisitaProductos["tipoMotor"] = $("#cboTipoMotor").val();
    modeloVisitaProductos["foso"] = parseInt($("#txtFoso").val());
    modeloVisitaProductos["recorrido"] = parseInt($("#txtRecorrido").val());
    modeloVisitaProductos["ingresosFrontales"] = parseInt($("#txtEntradasFrontales").val());
    modeloVisitaProductos["ingresosPosteriores"] = parseInt($("#txtEntradasPosterior").val());
    modeloVisitaProductos["sobrerecorrido"] = parseInt($("#txtSobreRecorrido").val());
    modeloVisitaProductos["dimensionEntrada"] = parseInt($("#txtDimencionEntrada").val());
    modeloVisitaProductos["alturaEntrePisos"] = parseInt($("#txtAlturaEntrePisos").val());
    modeloVisitaProductos["materialPuertas"] = $("#cboTipoMaterial").val();
    modeloVisitaProductos["energia"] = $("#cboTipoEnergia").val();
    modeloVisitaProductos["cantidad"] = parseInt($("#txtCantidad").val());
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
                swal("Listo!",
                    "Equipo Agregado Con Exito ",
                    "success");
            }
            else {
                swal("Fallo!", responseJson.mensajes, "error");
            }
        });

});

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

        // Acceder a la propiedad $values debido a ReferenceHandler.Preserve
        const data = response.$values || response;

        if (Array.isArray(data)) {
            tablaDataPro = $('#tbDataItems').DataTable({
                responsive: true,
                data: data,
                columns: [
                    { data: "secuencial", visible: false },
                    { data: "cantidad", searchable: false, width: "10%", className: "text-center" },
                    { data: "detalleEspecifico", searchable: true, width: "80%" },
                    {
                        "defaultContent":
                            '<button class="btn btn-danger btn-eliminar-equipo btn-sm mr-2"><i class="fas fa-trash-alt"></i></button>',
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
                dom: "Bfrtip",
                paging: false, // Deshabilitar paginación
                info: false,   // Deshabilitar información de paginación
                searching: false, // Deshabilitar búsqueda
                buttons: [
                    {
                        text: '<i class="fas fa-file-excel"></i>',
                        extend: 'excelHtml5',
                        title: 'Detalle Equipos Visita',
                        filename: 'Reporte Detalle Equipos',
                        exportOptions: {
                            columns: [1, 2]
                        },
                        className: 'btn-success',
                        titleAttr: 'Exportar a Excel'
                    },
                    {
                        text: '<i class="fas fa-file-pdf"></i>',
                        extend: 'pdfHtml5',
                        title: 'Detalle Equipos',
                        filename: 'Reporte Detalle Equipos',
                        exportOptions: {
                            columns: [1, 2]
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
                            const printDate = `${now.toLocaleDateString()} ${now.toLocaleTimeString()}`;

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
                                        { width: '*', text: `${nombreProvincia} - ${nombreCanton} - ${nombreParroquia}` }
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
                                        { width: '*', text: contactoData ? `${contactoData.nombres} ${contactoData.apellidos}` : 'N/A' }
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
                                { text: `Impreso: ${printDate}`, alignment: 'right', fontSize: 5, margin: [0, 10, 0, 0] }
                            ];

                            // Añadir el encabezado al documento
                            doc.content.splice(0, 0, { stack: headerContent, margin: [0, 0, 0, 12] });
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
            swal("Error!", "La respuesta del servidor no es válida.", "error");
        }

    } catch (error) {
        console.error("Error en el proceso:", error);
        swal("Error!", error.message, "error");
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