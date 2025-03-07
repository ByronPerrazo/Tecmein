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
$("#tbdata tbody").on("click", ".btn-info", function () {
    //esEdicion = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    secVisitaProducto = data.secuencial
    mostralModalDetalleProductos();

    if (secVisitaProducto!=0)
        ProcesoCargaLista(secVisitaProducto)

    $("#modalDataDetalleVisita").modal("show")
})

function mostralModalDetalleProductos() {

    
}


//$(document).ready(function () {

//    ProcesoCargaLista(secVisitaProducto)

//});



$("#btnAgregarItem").click(function () {

   
    const modeloVisitaProductos = structuredClone(MODELO_VISITA_PRODUCTOS);
    modeloVisitaProductos["secVisita"] = secVisitaProducto;
    modeloVisitaProductos["tipoEquipo"] = $("#cboTipoEquipo").val().trim();
    modeloVisitaProductos["sistema"] = $("#cboSistema").val().trim();
    modeloVisitaProductos["marca"] = $("#cboMarca").val().trim();
    modeloVisitaProductos["capacidad"] = parseInt($("#txtCapacidad").val());
    modeloVisitaProductos["velocidad"] = parseFloat($("#cboVelocidad").val().trim());
    modeloVisitaProductos["salaMaquinas"] =  $("#cboSalaMaquinas").val().trim();
    modeloVisitaProductos["salaControl"] = $("#cboSalaMaquinas").val().trim();
    modeloVisitaProductos["numeroPersonas"] = parseInt($("#txtNumPersonas").val());
    modeloVisitaProductos["numeroParadas"] =  parseInt($("#txtNumParadas").val());
    modeloVisitaProductos["nombreParadas"] =  $("#txtNombresParadas").val().trim();
    modeloVisitaProductos["embarque"] = $("#cboTipoEmbarque").val();
    modeloVisitaProductos["tipoDucto"] = $("#cboTipoDucto").val();
    modeloVisitaProductos["medidasDuctoAF"] = $("#txtMedidasDuctoAF").val().trim();
    modeloVisitaProductos["tipoMotor"] = $("#cboTipoMotor").val();
    modeloVisitaProductos["foso"] = parseInt($("#txtFoso").val());
    modeloVisitaProductos["recorrido"] = parseInt($("#txtRecorrido").val());
    modeloVisitaProductos["ingresosFrontales"] = parseInt($("#txtEntradasFrontales").val());
    modeloVisitaProductos["ingresosPosteriores"] = parseInt($("#txtEntradasPosterior").val());
    modeloVisitaProductos["sobrerecorrido"]   = parseInt($("#txtSobreRecorrido").val());
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

        console.log(response); // Verifica la respuesta aquí

        if (Array.isArray(response)) {
            tablaDataPro = $('#tbDataItems').DataTable({
                responsive: true,
                data: response,
                columns: [
                    { data: "secuencial", visible: false, width: "5%" },
                    { data: "cantidad", searchable: false, width: "5%" },
                    { data: "detalleEspecifico", searchable: true, width: "80%" },
                    {
                        "defaultContent":
                            '<button class="btn btn-danger btn-eliminar-equipo btn-sm mr-2"><i class="fas fa-trash-alt"></i></button>',
                        "orderable": true,
                        "searchable": false,
                        "width": "10%"
                    }
                ],
                order: [[0, "desc"]],
                dom: "Bfrtip",
                buttons: [
                    {
                        text: 'Exportar Excel',
                        extend: 'excelHtml5',
                        title: 'Detalle Equipos Visita',
                        filename: 'Reporte Detalle Equipos',
                        exportOptions: {
                            columns: [0, 1, 2]
                        }
                    }
                    
                    , 'pageLength'
                ],
                language: {
                    url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
                },
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

$("#tbDataItems tbody").on("click", ".btn-eliminar-equipo", function () {
    let fila;
    const $this = $(this); 

    if ($this.closest("tr").hasClass("child")) {
        fila = $this.closest("tr").prev();
    } else {
        fila = $this.closest("tr");
    }

    const data = tablaDataPro.row(fila).data();

    swal({
        title: "Está Seguro de Eliminar?",
        text: `Eliminar El Equipo "${data.marca} - ${data.sistema} - ${data.tipoEquipo}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (respuesta) {
            if (respuesta) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`ProcesoEliminarEquipoVisita?secuencialEquipoVisita=${data.secuencial}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok
                            ? response.json()
                            : Promise.reject(response);
                    }).then(responseJson => {
                        if (responseJson.estado) {
                            tablaDataPro.row(fila).remove().draw(false);
                            swal("Listo!", " Equipo Fue Eliminado", "success");
                        } else {
                            swal("Fallo!", responseJson.mensajes, "error");
                        }
                    });
            }
        });
});
