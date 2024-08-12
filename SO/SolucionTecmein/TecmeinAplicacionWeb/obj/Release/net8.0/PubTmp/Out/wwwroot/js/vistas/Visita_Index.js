const MODELO_BASEVISITA = {
    secuencial: "",
    nombre: "",
    secProvincia: "",
    nombreProvincia: "",
    secCanton: "",
    nombreCanton: "",
    secParroquia: "",
    nombreParroquia: "",
    direccion: "",
    fechaRegistro: "",
    geoUbicacion: "",
    secuencialUsuario: 0,
    fechaSiguienteVisita: "",
    detalle: "",
    estaActivo: 1,
}




let tablaData;
let listaCompletaProvincias;
let listaCompletaCanton;
let listaCompletaParroquia;
$(document).ready(function () {

    fetch("Parroquias")
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
                        $("#cboParroquia")
                            .append(
                                $("<option>")
                                    .val(item.secuencial)
                                    .text(item.nombre)
                            )
                    })

            }
        )
        .catch(error => {
            console.error('Error al obtener la lista de Parroquias:', error);
        });

    fetch("Cantones")
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

                        $("#cboCanton")
                            .append(
                                $("<option>")
                                    .val(item.secuencial)
                                    .text(item.nombre)
                            )
                    })

            }
        )
        .catch(error => {
            console.error('Error al obtener la lista de Cantones:', error);
        });

    fetch("Provincias")
        .then(
            respuesta => {
                return respuesta.ok
                    ? respuesta.json()
                    : Promise.reject(respuesta);
            }
        )
        .then(
            respuestaJson => {
                listaCompletaProvincias = respuestaJson;
                respuestaJson
                    .forEach(item => {
                        $("#cboProvincia")
                            .append(
                                $("<option>")
                                    .val(item.secuencial)
                                    .text(item.nombre)
                            )
                    })

            }
        )
        .catch(error => {
            console.error('Error al obtener la lista de Provincias:', error);
        });


    tablaData =
        $('#tbdata').DataTable({
            responsive: true,
            "ajax": {
                "url": 'Lista',
                "type": "GET",
                "datatype": "json"
            },
            "columns": [
                { data: "secuencial", visible: false },
                { data: "nombre", searchable: true },
                { data: "nombreProvincia", searchable: true, width: "100px" },
                { data: "nombreCanton", searchable: true, width: "8px" },
                { data: "direccion", searchable: true },
               
                {
                    data: "estaActivo", render: function (data) {
                        if (data == 1)
                            return '<span class="badge badge-info">Activo</span>';
                        else
                            return '<span class="badge badge-danger">Inactivo</span>';
                    }
                },

                {
                    "defaultContent":
                        '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                        '<button class="btn btn-danger btn-eliminar btn-sm mr-2"><i class="fas fa-trash-alt"></i></button>'+
                        '<button onclick="iniciarMap()" class="btn btn-success btn-mapa btn-sm mr-1"><i class="fas fa-eye"></i></button>',
                    "orderable": false,
                    "searchable": false,
                    "width": "120px"
                }
            ],
            order: [[0, "desc"]],
            dom: "Bfrtip",
            buttons: [
                {
                    text: 'Exportar Excel',
                    extend: 'excelHtml5',
                    title: 'Productos',
                    filename: 'Reporte de Productos',
                    exportOptions: {
                        columns: [0, 2, 3, 4, 5, 6]
                    }
                }, 'pageLength'
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });


});

const tieneSigVisita = document.getElementById('chkEsSigVisita');
const fechaSiguienteVisita = document.getElementById('dtpkFechaSigVisita');
let validaFecha = false;

tieneSigVisita.addEventListener('change', function () {
    if (this.checked) {
        fechaSiguienteVisita.style.display = 'block';
        validaFecha = true;
    } else {
        fechaSiguienteVisita.style.display = 'none';
        validaFecha = false;
    }
});

const cmboProvincia = document.getElementById('cboProvincia');
const cmboCanton = document.getElementById('cboCanton');
const cmboParroquia = document.getElementById('cboParroquia');
cmboProvincia.onchange = function () {

    cmboCanton.innerHTML = '';
    cmboParroquia.innerHTML = '';

    var provSelect = cmboProvincia.value;

    fetch("Cantones")
        .then(
            respuesta => {
                return respuesta.ok
                    ? respuesta.json()
                    : Promise.reject(respuesta);
            }
        )
        .then(
            respuestaJson => {
                respuestaJson
                    .forEach(item => {
                        if (item.secProvincia == provSelect)
                            $("#cboCanton")
                                .append(
                                    $("<option>")
                                        .val(item.secuencial)
                                        .text(item.nombre)
                                )
                    })

            }
        )
        .catch(error => {
            console.error('Error al obtener la lista de Provincias:', error);
        });


};
cmboCanton.onchange = function () {

    cmboParroquia.innerHTML = '';

    var cantonSelect = cmboCanton.value;

    fetch("Parroquias")
        .then(
            respuesta => {
                return respuesta.ok
                    ? respuesta.json()
                    : Promise.reject(respuesta);
            }
        )
        .then(
            respuestaJson => {
                respuestaJson
                    .forEach(item => {
                        if (item.secCanton == cantonSelect)
                            $("#cboParroquia")
                                .append(
                                    $("<option>")
                                        .val(item.secuencial)
                                        .text(item.nombre)
                                )
                    })

            }
        )

        .catch(error => {
            console.error('Error al obtener la lista de Provincias:', error);
        });


};

function obtenerGeoubicacion() {
    if (navigator.geolocation) {
        return new Promise((resolve, reject) => {
            navigator.geolocation.getCurrentPosition(function (position) {
                const latitud = position.coords.latitude;
                const longitud = position.coords.longitude;
                const ubicacionString = `${latitud},${longitud}`;
                resolve(ubicacionString.toString());
            }, function (error) {
                reject('Error al obtener la ubicación: ' + error.message);
            });
        });
    } else {
        return Promise.reject('Tu navegador no admite la geolocalización.');
    }
}

function limpiarFormularioModal() {
    $("#txtId").val('');
    $("#txtNombreObra").val('');
    $("#cboProvincia").val($("#cboProvincia option:first").val());
    $("#cboCanton").val($("#cboCanton option:first").val());
    $("#cboParroquia").val($("#cboParroquia option:first").val());
    $("#txtDireccion").val('');
    $("#txtGeolocallizacion").val('');
    $("#cboEstado").val(1);
    $("#dtpkFechaSigVisita").val('');
}
function mostrarModalVisita(modeloVisita = MODELO_BASEVISITA) {
    limpiarFormularioModal();
    $("#txtId").val(modeloVisita.secuencial)
    $("#txtNombreObra").val(modeloVisita.nombre)
    $("#cboProvincia").val(modeloVisita.secProvincia == "" ? $("#cboProvincia option:first").val() : modeloVisita.secProvincia)
    $("#cboCanton").val(modeloVisita.secCanton == "" ? $("#cboCanton option:first").val() : modeloVisita.secCanton)
    $("#cboParroquia").val(modeloVisita.secParroquia == "" ? $("#cboParroquia option:first").val() : modeloVisita.secParroquia)
    $("#txtDireccion").val(modeloVisita.direccion)
    $("#txtGeolocallizacion").val(modeloVisita.geoUbicacion)
    $("#cboEstado").val(modeloVisita.estaActivo)
    $("#dtpkFechaSigVisita").val(modeloVisita.fechaSiguienteVisita)
    $("#txtDescripcion").val(modeloVisita.detalle)
    $("#modalData").modal("show")
};

let esEdicion;
$("#btnNuevo").click(function () {
    limpiarFormularioModal();
    esEdicion = false;
    mostrarModalVisita(MODELO_BASEVISITA)
})
$("#btnGuardarVisitas").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");

    inputs_vacios.forEach(x => {
        const mensaje = `Debe llenar el campo: "${x.name}"\n`;
        toastr.warning("", mensaje);
        return;
    });

    if (inputs_vacios.length > 0) {
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    const selects = document.querySelectorAll("select.input-validar");

    const selectsConValorDeshabilitado = Array.from(selects).filter(select => {
        const selectedOption = select.options[select.selectedIndex];
        return selectedOption.disabled && selectedOption.selected;
    });

    selectsConValorDeshabilitado.map(select => {
        const mensaje = `Debe seleccionar una opción valida en : "${select.name}"\n`;
        toastr.warning("", mensaje);
        selectsConValorDeshabilitado[0].focus();
        return;
    });

    var respuesta = validarFechaFormulario(validaFecha);
    respuesta += validarFormulario();

    if (respuesta != '') {
        toastr.warning("", respuesta);
        return;
    }



    let geo = "";
    debugger;

    obtenerGeoubicacion()
        .then((ubicacion) => {
            //console.log('Ubicación obtenida:', ubicacion);
            geo = ubicacion.toString();
        })
        .catch((error) => {
            geo = "";
            const mensaje = `Error al obtener la ubicación : "${error}"\n`;
            toastr.warning("", mensaje);
        })

    debugger;
    $("#txtGeolocallizacion").val(geo);

    let secuencialVisita = $("#txtId").val().trim() == "" ? "0" : $("#txtId").val().trim();

    const modeloVisita = structuredClone(MODELO_BASEVISITA);
          modeloVisita["secuencial"] = secuencialVisita;
          modeloVisita["nombre"] = $("#txtNombreObra").val().trim();
          modeloVisita["secProvincia"] = parseInt($("#cboProvincia").val());
          modeloVisita["secCanton"] = parseInt($("#cboCanton").val());
          modeloVisita["secParroquia"] = parseInt($("#cboParroquia").val());
          modeloVisita["direccion"] = $("#txtDireccion").val().trim();
          modeloVisita["geoUbicacion"] = geo; 
          modeloVisita["fechaSiguienteVisita"] = $("#dtpkFechaSigVisita").val();
          modeloVisita["detalle"] = $("#txtDescripcion").val();
          modeloVisita["esActivo"] = $("#cboEstado").val();

    const datosFormulario = new FormData();
    datosFormulario.append("modelo", JSON.stringify(modeloVisita));
    

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (!esEdicion) {

        fetch("CrearVisita", {
            method: "POST",
            body: datosFormulario
        })
            .then(response => {
                $("#modalData")
                    .find("div.modal-content")
                    .LoadingOverlay("hide");
                return response.ok
                    ? response.json()
                    : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!",
                        "Visita a " + responseJson.objeto.nombre + " Creada ",
                        "success");
                }
                else {
                    swal("Fallo!", responseJson.mensajes, "error");
                }
            });
    }
    else {

        fetch("Editar", {
            method: "PUT",
            body: datosFormulario
        })
            .then(response => {
                $("#modalData")
                    .find("div.modal-content")
                    .LoadingOverlay("hide");
                return response.ok
                    ? response.json()
                    : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {

                    tablaData
                        .row(filaSeleccionada)
                        .data(responseJson.objeto)
                        .draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!",
                        "Visita a " + responseJson.objeto.nombre + " Editada ",
                        "success");
                }
                else {
                    swal("Fallo!", responseJson.mensajes, "error");
                }
            });

    }

});

let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    debugger;
    mostrarModalVisita(data);

})

$("#tbdata tbody").on("click", ".btn-eliminar", function () {

    let fila
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    swal({
        title: "Está Seguro de Eliminar?",
        text: `Eliminar la visita "${data.nombre}"`,
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

                fetch(`Eliminar?secuencial=${data.secuencial}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok
                            ? response.json()
                            : Promise.reject(response);
                    }).then(responseJson => {
                        debugger;
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw(false);

                            swal("Listo!", " La Visita a " + data.nombre + " fue Eliminada", "success");
                        }
                        else {
                            swal("Fallo!", responseJson.mensajes, "error");
                        }
                    });
            }
        }
    )

})

$("#tbdata tbody").on("click", ".btn-mapa", function () {
    esEdicion = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    debugger;
    //mostrarModalVisita(data);
    $("#modalDataMapa").modal("show");
})


