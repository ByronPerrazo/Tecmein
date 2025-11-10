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
let listaCompletaEtapas;
let idEtapaVisita;
let userPermissions = []; // Moved to top

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

function cargarConstantesEquipos() {
    const selectsToLoad = [
        { id: "cboTipoEquipo", tipo: "tipoequipo", placeholder: "Tipo Equipo" },
        { id: "cboSistema", tipo: "sistema", placeholder: "Sistema" },
        { id: "cboMarca", tipo: "marca", placeholder: "Marca" },
        { id: "cboSalaMaquinas", tipo: "salamarquinas", placeholder: "Sala Maquinas" },
        { id: "cboTipoMotor", tipo: "tipomotor", placeholder: "Tipo Motor" },
        { id: "cboVelocidad", tipo: "velocidad", placeholder: "Velocidad m/s" },
        { id: "cboTipoEmbarque", tipo: "tipoembarque", placeholder: "Tipo de Embarque" },
        { id: "cboTipoDucto", tipo: "tipoducto", placeholder: "Ducto De" },
        { id: "cboTipoEnergia", tipo: "tipoenergia", placeholder: "Tipo Energía" },
        { id: "cboTipoMaterial", tipo: "materialpuertas", placeholder: "Material de Puertas" }
    ];

    selectsToLoad.forEach(selectInfo => {
        fetch(`/Visita/ObtenerConstantesEquipos?tipoConstante=${selectInfo.tipo}`)
            .then(response => response.ok ? response.json() : Promise.reject(response))
            .then(data => {
                const selectElement = $(`#${selectInfo.id}`);
                selectElement.empty().append(`<option value="" disabled selected>${selectInfo.placeholder}</option>`);
                
                // Iterar sobre las claves del objeto JSON, excluyendo la propiedad "$id"
                Object.keys(data).filter(key => key !== "$id").forEach(key => {
                    selectElement.append($("<option>").val(key).text(data[key]));
                });
            })
            .catch(error => console.error(`Error al cargar ${selectInfo.placeholder}:`, error));
    });
}

$(document).ready(function () {
    cargarOperadores(); // Call on document ready
    cargarConstantesEquipos(); // Call to load equipment constants

    fetch("/Visita/Etapas")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(respuestaJson => {
            const data = respuestaJson.$values || respuestaJson;
            listaCompletaEtapas = data;
            const etapaVisita = listaCompletaEtapas.find(e => e.codigo === "VIS");
            if (etapaVisita) {
                idEtapaVisita = etapaVisita.id;
            }

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


let userPermissions = []; // Global variable to store permissions

     Promise.all([
        fetch("Lista").then(response => response.text()).then(text => JSON.parse(text)),
        fetch("GetClaims").then(response => response.json())
    ]).then(([listaResponse, claimsResponse]) => {

        const claims = claimsResponse;
        userPermissions = claims.permisos || []; // Store permissions
        console.log("User Permissions after loading:", userPermissions); // Added console log
        let data = listaResponse.data;

        if (claims.rol.toLowerCase() !== "administrador") {
            data = data.filter(visita => visita.SecUsuario == claims.idUsuario);
        }

        tablaData = $('#tbdata').DataTable({
            responsive: true,
            data: data,
            "columns": [
                { data: "Secuencial", visible: false },
                { data: "Nombre", searchable: true },
                { data: "DescripcionEtapa", searchable: false },
                { data: "NombreCanton", searchable: true, width: "60px" },
                { data: "Direccion", searchable: true },
                {
                    data: 'GeoUbicacion', "orderable": false, "searchable": false, width: "20px",
                    render: function (data) {
                        return '<button onclick="initMap(\'" + data + "\')" class="btn btn-success btn-mapa btn-sm mr-1"><i class="fas fa-search-location"></i></button>';
                    }
                },
                {
                    data: "EstaActivo", visible: false, render: function (data) {
                        if (data == 1)
                            return '<span class="badge badge-info">Activo</span>';
                        else
                            return '<span class="badge badge-danger">Inactivo</span>';
                    }
                },

                {
                    "defaultContent":
                        '<div class="btn-group" role="group">' +
                        '<button class="btn btn-primary btn-editar btn-sm" title="Editar"><i class="fas fa-pencil-alt"></i></button>' +
                        '<button class="btn btn-primary btn-default btn-sm" title="Ver Archivo"><i class="fas fa-file-alt"></i></button>' +
                        '<button class="btn btn-primary btn-info btn-sm" title="Ver Detalles"><i class="fas fa-list-alt"></i></button>' +
                        '<button class="btn btn-danger btn-eliminar btn-sm" title="Eliminar"><i class="fas fa-trash-alt"></i></button>' +
                        '</div>',
                    "orderable": false, "searchable": false
                }
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


});

const tieneSigVisita = document.getElementById('chkEsSigVisita');
const fechaSiguienteVisita = document.getElementById('dtpkFechaSigVisita');
let validaFecha = false;

tieneSigVisita.addEventListener('change', function () {
    if (this.checked) {
        fechaSiguienteVisita.style.display = '';
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
    $("#cboProvincia").val("");
    $("#cboCanton").val("");
    $("#cboParroquia").val("");
    $("#txtDireccion").val('');
    $("#txtGeolocallizacion").val('');
    $("#cboEstado").val(1);
    document.getElementById('chkEsSigVisita').checked = false;
    $("#dtpkFechaSigVisita").val('');
    fechaSiguienteVisita.style.display = 'none';
}
function mostrarModalVisita(esEdicion, modeloVisita = MODELO_BASEVISITA) {
    limpiarFormularioModal();
    $("#txtId").val(modeloVisita.Secuencial)
    $("#txtNombreObra").val(modeloVisita.Nombre)
    $("#cboOperador").val(modeloVisita.SecEmpresa);
    
    if (esEdicion) {
        $("#cboEtapaObra").val(modeloVisita.IdEtapa).prop('disabled', true);
    } else {
        $("#cboEtapaObra").val(idEtapaVisita).prop('disabled', true);
    }
    
    // Carga y selección de combos en cascada
    if (modeloVisita.SecProvincia) {
        $("#cboProvincia").val(modeloVisita.SecProvincia);
        cargarCantones(modeloVisita.SecProvincia);
        if (modeloVisita.SecCanton) {
            $("#cboCanton").val(modeloVisita.SecCanton);
            cargarParroquias(modeloVisita.SecCanton);
            if (modeloVisita.SecParroquia) {
                $("#cboParroquia").val(modeloVisita.SecParroquia);
            }
        }
    } else {
        $("#cboProvincia").val($("#cboProvincia option:first").val());
        cargarCantones($("#cboProvincia option:first").val());
    }

    $("#txtDireccion").val(modeloVisita.Direccion)
    $("#txtGeolocallizacion").val(modeloVisita.GeoUbicacion)
    $("#cboEstado").val(modeloVisita.EstaActivo)

    loadDateFromString(modeloVisita.FechaSiguienteVisita)
    
    $("#txtDescripcion").val(modeloVisita.Detalle)
    
    // Forzar a la validación no intrusiva a parsear el formulario del modal
    $.validator.unobtrusive.parse("#formVisita");

    $("#modalData").modal("show")
};
function loadDateFromString(dateString) {
    if (!dateString || dateString.trim() === "" || dateString === "0001-01-01T00:00:00") {
        document.getElementById('chkEsSigVisita').checked = false;
        fechaSiguienteVisita.style.display = 'none';
        $("#dtpkFechaSigVisita").val('');
        return;
    }

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

let esEdicion;
    $("#btnNuevo").click(function () {
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

        mostrarModalVisita(false, MODELO_BASEVISITA)
    })
$("#btnGuardarVisitas").click(function () {

    // Validar el formulario usando jQuery Validate
    if (!$("#formVisita").valid()) {
        toastr.warning("", "Por favor, complete todos los campos requeridos.");
        return;
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
    if ($('#chkEsSigVisita').is(':checked')) {
        modeloVisita["fechaSiguienteVisita"] = $("#dtpkFechaSigVisita").val();
    }
    modeloVisita["detalle"] = $("#txtDescripcion").val();
    modeloVisita["estaActivo"] = $("#cboEstado").val();
    modeloVisita["secEmpresa"] = parseInt($("#cboOperador").val());
    modeloVisita["idEtapa"] = parseInt($("#cboEtapaObra").val());

    const datosFormulario = new FormData();
    datosFormulario.append("modelo", JSON.stringify(modeloVisita));


    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    const url = !esEdicion ? "CrearVisita" : "EditarVisita";
    const successMessage = !esEdicion ? "creada" : "editada";

    fetch(url, {
        method: "POST",
        body: datosFormulario
    })
    .then(response => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        if (response.ok) {
            return response.json();
        } else {
            // Si la respuesta no es OK, intenta leer el cuerpo del error
            return response.json().then(errorJson => {
                // Rechaza la promesa con el JSON del error para que sea capturado por el .catch
                return Promise.reject({ status: response.status, data: errorJson });
            });
        }
    })
    .then(responseJson => {
        if (responseJson.estado) {
            // Convertir propiedades a PascalCase para DataTables
            const dataForRow = {
                Secuencial: responseJson.objeto.secuencial,
                Nombre: responseJson.objeto.nombre,
                SecProvincia: responseJson.objeto.secProvincia,
                NombreProvincia: $("#cboProvincia option:selected").text(),
                SecCanton: responseJson.objeto.secCanton,
                NombreCanton: $("#cboCanton option:selected").text(),
                SecParroquia: responseJson.objeto.secParroquia,
                NombreParroquia: $("#cboParroquia option:selected").text(),
                Direccion: responseJson.objeto.direccion,
                FechaRegistro: responseJson.objeto.fechaRegistro,
                GeoUbicacion: responseJson.objeto.geoUbicacion,
                EstaActivo: responseJson.objeto.estaActivo,
                SecUsuario: responseJson.objeto.secUsuario,
                FechaSiguienteVisita: responseJson.objeto.fechaSiguienteVisita,
                Detalle: responseJson.objeto.detalle,
                IdEtapa: responseJson.objeto.idEtapa,
                DescripcionEtapa: $("#cboEtapaObra option:selected").text(),
                CodigoEtapa: responseJson.objeto.codigoEtapa,
                SecEmpresa: responseJson.objeto.secEmpresa,
                NombreEmpresa: $("#cboOperador option:selected").text(),
                SecConstructora: responseJson.objeto.secConstructora,
                NombreConstructora: responseJson.objeto.nombreConstructora
            };

            if (esEdicion) {
                tablaData.row(filaSeleccionada).data(dataForRow).draw(false);
            } else {
                tablaData.row.add(dataForRow).draw(false);
            }

            $("#modalData").modal("hide");
            Swal.fire("Listo!", `Visita a ${dataForRow.Nombre} ${successMessage}`, "success");
        } else {
            Swal.fire("Fallo!", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        if (error.status === 400) {
            // Error de validación del servidor
            Swal.fire("Datos Inválidos", error.data.mensajes, "error");
        } else if (error.status === 403) {
            Swal.fire("Acceso Denegado", `No tiene permisos para ${successMessage} visitas.`, "error");
        } else {
            Swal.fire("Error", `Ocurrió un error al ${successMessage} la visita.`, "error");
        }
    });
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

        mostrarModalVisita(true, data);
    })
    $("#tbdata tbody").on("click", ".btn-eliminar", function () {
        let fila
        if ($(this).closest("tr").hasClass("child")) {
            fila = $(this).closest("tr").prev();
        } else {
            fila = $(this).closest("tr");
        }

        const data = tablaData.row(fila).data();

        Swal.fire({
            title: "Está Seguro de Eliminar?",
            text: `Eliminar la visita "${data.Nombre}"`, 
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Si, eliminar",
            cancelButtonText: "No, cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`Eliminar?secuencial=${data.Secuencial}`, { 
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

                            Swal.fire("Listo!", " La Visita a " + data.Nombre + " fue Eliminada", "success");
                        }
                        else {
                            Swal.fire("Fallo!", responseJson.mensajes, "error");
                        }
                    })
                    .catch(error => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        if (error.status === 403) {
                            Swal.fire("Acceso Denegado", "No tiene permisos para eliminar visitas.", "error");
                        } else {
                            Swal.fire("Error", "Ocurrió un error al eliminar la visita.", "error");
                        }
                    });
            }
        })
    })
    $("#tbdata tbody").on("click", ".btn-avanzar-etapa", function () {
        // Check for "AVANZAR_ETAPA" permission
        if (!userPermissions.includes("AVANZAR_ETAPA")) { // Assuming "AVANZAR_ETAPA" is the permission name
            Swal.fire("Acceso Denegado", "No tiene permisos para avanzar la etapa de visitas.", "error");
            return; // Stop execution if no permission
        }

        let fila;
        if ($(this).closest("tr").hasClass("child")) {
            fila = $(this).closest("tr").prev();
        } else {
            fila = $(this).closest("tr");
        }
        const data = tablaData.row(fila).data();

        Swal.fire({
            title: "Avanzar Etapa",
            text: `¿Está seguro de avanzar la etapa de la visita "${data.Nombre}"?`,
            icon: "info",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "Sí, avanzar",
            cancelButtonText: "No, cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                $(".showSweetAlert").LoadingOverlay("show");

                const formData = new FormData();
                formData.append("secVisita", data.Secuencial);
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
                        Swal.fire("Listo!", "La etapa de la visita fue actualizada.", "success");
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


