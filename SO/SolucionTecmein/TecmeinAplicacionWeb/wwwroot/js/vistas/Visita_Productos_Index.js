const MODELO_PRODUCTOS = {
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


$(document).ready( function () {

    fetch("EmpresaConstructora")
                .then(
                    respuesta => {
                        return respuesta.ok
                            ? respuesta.json()
                            : Promise.reject(respuesta);
                    }
                )
                .then(
                    respuestaJson => {
                        listaCompletaCanton = respuestaJson;
                        respuestaJson
                            .forEach(item => {
                                $("#cboEmpresa")
                                    .append(
                                        $("<option>")
                                            .val(item.secuencial)
                                            .text(item.nombre.trim())
                                    )
                            })
                        cmboConstructora.value = '-1';
                    }
                )
                .catch(error => {
                    console.error('Error al obtener la lista de Empresas Contructoras:', error);
                });

    $("#btnAgregarItem").click(
            function () {
                    limpiarFormularioModal();
                    esEdicion = false;

                    obtenerGeoubicacion()
                        .then((ubicacion) => {
                            var geo = ubicacion.toString();
                            $("#txtGeolocallizacion").val(geo)
                        })
                        .catch((error) => {
                            geo = "";
                            const mensaje = `Error al obtener la ubicación : "${error}"\n`;
                            toastr.warning("", mensaje);
                        });

                    mostrarModalVisita(MODELO_BASEVISITA)
    })

});


