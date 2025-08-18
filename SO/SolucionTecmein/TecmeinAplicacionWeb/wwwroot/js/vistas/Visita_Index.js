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

function cargarOperadores() {
    fetch("/Visita/Operadores")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(data => {
            const operadores = data.$values || data;
            $("#cboOperador").empty().append('<option value="">Seleccione Operador</option>');
            operadores.forEach(item => {
                $("#cboOperador").append($("<option>").val(item.secuencial).text(item.codigoOperador));
            });
        })
        .catch(error => console.error('Error al obtener la lista de Operadores:', error));
}

$(document).ready(function () {
    cargarOperadores(); // Call on document ready

    fetch("/Visita/Etapas")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(respuestaJson => {
            const data = respuestaJson.$values || respuestaJson;
            data.forEach(item => {
                $("#cboEtapaObra").append($("<option>").val(item.id).text(item.descripcion));
            });
        })
        .catch(error => console.error('Error al obtener la lista de Etapas:', error));

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
                // Acceder a la propiedad $values debido a ReferenceHandler.Preserve
                const data = respuestaJson.$values || respuestaJson;
                listaCompletaParroquia = data; // Asignar la data correcta
                data
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
                // Acceder a la propiedad $values debido a ReferenceHandler.Preserve
                const data = respuestaJson.$values || respuestaJson;
                listaCompletaCanton = data; // Asignar la data correcta
                data
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
                // Acceder a la propiedad $values debido a ReferenceHandler.Preserve
                const data = respuestaJson.$values || respuestaJson;
                listaCompletaProvincias = data; // Asignar la data correcta
                data
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
                // Acceder a la propiedad $values debido a ReferenceHandler.Preserve
                const data = respuestaJson.$values || respuestaJson;
                data
                    .forEach(item => {
                        $("#cboEmpresa")
                            .append(
                                $("<option>")
                                    .val(item.secuencial)
                                    .text(item.nombre)
                            );
                    })
            }
        )
        .catch(error => {
            console.error('Error al obtener la lista de Constructora:', error);
        });


     tablaData =
        $('#tbdata').DataTable({
            responsive: true,
            "ajax": {
                "url": 'Lista',
                "type": "GET",
                "datatype": "json",
                "dataSrc": function(json) {
                return json.data.$values;
            },
            },
            "columns": [
                { data: "secuencial", visible: false },
                { data: "nombre", searchable: true },
                { data: "nombreEmpresa", searchable: true },
                { data: "descripcionEtapa", searchable: true },
                { data: "nombreProvincia", searchable: true, width: "100px" },
                { data: "nombreCanton", searchable: true, width: "80px" },
                { data: "direccion", searchable: true },

                {
                    data: 'geoUbicacion', width: "20px",
                    render: function (data) {
                        return '<button onclick=\"initMap(\'' + data + '\')\" class=\"btn btn-success btn-mapa btn-sm mr-1\"><i class=\"fas fa-search-location\"></i></button>';
                    }
                },
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
                        '<button class="btn btn-primary btn-default btn-sm mr-2"><i class="fas fa-file-alt"></i></button>' +
                        '<button class="btn btn-primary btn-info btn-sm mr-2"><i class="fas fa-list-alt"></i></button>' +
                        '<button class="btn btn-danger btn-eliminar btn-sm mr-2"><i class="fas fa-trash-alt"></i></button>',
                    "orderable": true,
                    "searchable": false,
                    "width": "160px"
                },
                
            ],
            order: [[0, "desc"]],
            dom: "Bfrtip",
            buttons: [
                {
                    text: 'Exportar Excel',
                    extend: 'excelHtml5',
                    title: 'Reporte de Visitas',
                    filename: 'Reporte de Visitas',
                    exportOptions: {
                        columns: [1, 2, 3, 4]
                    }
                },
                {
                    text: 'Exportar PDF',
                    extend: 'pdfHtml5',
                    title: 'Reporte de Visitas',
                    filename: 'Reporte de Visitas',
                    exportOptions: {
                        columns: [1, 2, 3, 4]
                    }
                },
                'pageLength'
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
        var minDate = new Date(-8640000000000); 
            $("#dtpkFechaSigVisita").val('');
        fechaSiguienteVisita.style.display = 'none';
        validaFecha = false;
    }
});

let estadoVisita;
function mostrarDiv() {
    const contenedores = document.querySelectorAll('.DetalleVisita');
    contenedores.forEach(div => div.style.display = 'none');

    const seleccion = document.getElementById('cboEtapaObra').value;
    document.getElementById('detalleVisita_Vista').style.display = 'none';
    document.getElementById('Datos_ContratoObra').style.display = 'none'
    
    estadoVisita = seleccion;

    document.getElementById('div_fechaContrato').style.display = 'none';
    document.getElementById('div_fechaAnticipo').style.display = 'none';
    document.getElementById('txtDiasEntrega').style.display = 'none';

    if (seleccion == 'PRE' || seleccion == 'CON' || seleccion == 'POR') {
        document.getElementById('div_fechaContrato').style.display = 'block';
        document.getElementById('div_fechaAnticipo').style.display = 'block';
        document.getElementById('txtDiasEntrega').style.display = 'block';

    }
}

const cmboProvincia = document.getElementById('cboProvincia');
const cmboCanton = document.getElementById('cboCanton');
const cmboParroquia = document.getElementById('cboParroquia');

function cargarCantones(secProvincia) {
    cmboCanton.innerHTML = '<option value="" disabled selected>Seleccione Cantón</option>';
    cmboParroquia.innerHTML = '<option value="" disabled selected>Seleccione Parroquia</option>';

    const cantonesFiltrados = listaCompletaCanton.filter(c => c.secProvincia == secProvincia);
    cantonesFiltrados.forEach(item => {
        $("#cboCanton").append($("<option>").val(item.secuencial).text(item.nombre));
    });
}

function cargarParroquias(secCanton) {
    cmboParroquia.innerHTML = '<option value="" disabled selected>Seleccione Parroquia</option>';

    const parroquiasFiltradas = listaCompletaParroquia.filter(p => p.secCanton == secCanton);
    parroquiasFiltradas.forEach(item => {
        $("#cboParroquia").append($("<option>").val(item.secuencial).text(item.nombre));
    });
}

cmboProvincia.onchange = function () {
    cargarCantones(this.value);
};

cmboCanton.onchange = function () {
    cargarParroquias(this.value);
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
    document.getElementById('chkEsSigVisita').checked = false;
    $("#dtpkFechaSigVisita").val('');
}
function mostrarModalVisita(modeloVisita = MODELO_BASEVISITA) {
    limpiarFormularioModal();
    $("#txtId").val(modeloVisita.secuencial)
    $("#txtNombreObra").val(modeloVisita.nombre)
    $("#cboOperador").val(modeloVisita.secEmpresa);
    $("#cboEtapaObra").val(modeloVisita.idEtapa);
    
    // Carga y selección de combos en cascada
    if (modeloVisita.secProvincia) {
        $("#cboProvincia").val(modeloVisita.secProvincia);
        cargarCantones(modeloVisita.secProvincia);
        if (modeloVisita.secCanton) {
            $("#cboCanton").val(modeloVisita.secCanton);
            cargarParroquias(modeloVisita.secCanton);
            if (modeloVisita.secParroquia) {
                $("#cboParroquia").val(modeloVisita.secParroquia);
            }
        }
    } else {
        $("#cboProvincia").val($("#cboProvincia option:first").val());
        cargarCantones($("#cboProvincia option:first").val());
    }

    $("#txtDireccion").val(modeloVisita.direccion)
    $("#txtGeolocallizacion").val(modeloVisita.geoUbicacion)
    $("#cboEstado").val(modeloVisita.estaActivo)

    loadDateFromString(modeloVisita.fechaSiguienteVisita)
    
    $("#txtDescripcion").val(modeloVisita.detalle)
    //ejecutra llamada adicionales basados en el secuencial de la visita


    $("#modalData").modal("show")
};
function loadDateFromString(dateString) {
    if( !(!dateString || dateString.trim() === "")) {
    
    var dateParts = dateString.split('-'); 
    var day = parseInt(dateParts[2], 10);
    var month = parseInt(dateParts[1], 10) - 1; 
    var year = parseInt(dateParts[0], 10);

    var dateObject = new Date(year, month, day); 

    var minDate = new Date(-8640000000000); 
    if (dateObject <= minDate) {
        document.getElementById('chkEsSigVisita').checked = false;
        fechaSiguienteVisita.style.display = 'none';
    } else {
        
        document.getElementById('chkEsSigVisita').checked = true;
        fechaSiguienteVisita.style.display = 'block';
        var formattedDate = dateObject.toISOString().split('T')[0];
        
        $("#dtpkFechaSigVisita").val(formattedDate);
    }
    }
}

let esEdicion;
$("#btnNuevo").click(function () {
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

    const seleccion = document.getElementById('cboEtapaObra').value;
    if (seleccion != 'VIS') {
        var respuesta = validarFechaFormulario(validaFecha);
        respuesta += validarFormulario();

        if (respuesta != '') {
            toastr.warning("", respuesta);
            return;
        }

    }

    let secuencialVisita = $("#txtId").val().trim() == "" ? "0" : $("#txtId").val().trim();

    const modeloVisita = structuredClone(MODELO_BASEVISITA);
    modeloVisita["secuencial"] = secuencialVisita;
    modeloVisita["nombre"] = $("#txtNombreObra").val().trim();
    modeloVisita["secProvincia"] = parseInt($("#cboProvincia").val());
    modeloVisita["secCanton"] = parseInt($("#cboCanton").val());
    modeloVisita["secParroquia"] = parseInt($("#cboParroquia").val());
    modeloVisita["direccion"] = $("#txtDireccion").val().trim();
    modeloVisita["geoUbicacion"] = $("#txtGeolocallizacion").val().trim();
    modeloVisita["fechaSiguienteVisita"] = $("#dtpkFechaSigVisita").val();
    modeloVisita["detalle"] = $("#txtDescripcion").val();
    modeloVisita["esActivo"] = $("#cboEstado").val();
    modeloVisita["secEmpresa"] = $("#cboOperador").val();

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
            })
            .then(responseJson => {
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

        fetch("EditarVisita", {
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
            })
            .then(responseJson => {
                if (responseJson.estado) {

                    // Obtener los nombres desde los combos para actualizar la tabla
                    responseJson.objeto.nombreProvincia = $("#cboProvincia option:selected").text();
                    responseJson.objeto.nombreCanton = $("#cboCanton option:selected").text();

                    tablaData
                        .row(filaSeleccionada)
                        .data(responseJson.objeto)
                        .draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!",
                        "Visita a " + responseJson.objeto.nombre + " Editada ",
                        "success");
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

$("#tbdata tbody").on("click", ".btn-avanzar-etapa", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaData.row(fila).data();

    swal({
        title: "Avanzar Etapa",
        text: `¿Está seguro de avanzar la etapa de la visita "${data.nombre}"?`,
        type: "info",
        showCancelButton: true,
        confirmButtonClass: "btn-info",
        confirmButtonText: "Sí, avanzar",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    },
    function (respuesta) {
        if (respuesta) {
            $(".showSweetAlert").LoadingOverlay("show");

            const formData = new FormData();
            formData.append("secVisita", data.secuencial);
            formData.append("nuevoCodigoEtapa", ""); // Dejamos el código vacío para que el backend decida la siguiente etapa

            fetch("/Visita/CambiarEtapa", {
                method: "POST",
                body: formData
            })
            .then(response => {
                $(".showSweetAlert").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.ajax.reload(null, false); // Recargar la tabla sin resetear la paginación
                    swal("Listo!", "La etapa de la visita fue actualizada.", "success");
                } else {
                    swal("Error", responseJson.mensajes, "error");
                }
            })
            .catch(err => {
                 $(".showSweetAlert").LoadingOverlay("hide");
                 swal("Error", "No se pudo conectar con el servidor.", "error");
            });
        }
    });
});

$("#tbdata tbody").on("click", ".btn-mapa", function () {
    esEdicion = true;
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    $("#modalDataMapa").modal("show");
})


