const MODELO_BASE = {
    secuencial: "",
    secTipoProducto: 0,
    nombre: "",
    marca: "",
    sistema: "",
    capacidad: "",
    motor: "",
    stock: 0,
    urlImagen: "",
    precio: 0,
    descripcion: "",
    estaActivo: 1,
}

let tablaData;
let esEdicion = false;
let filaSeleccionada;

function manejarErrorFetch(error, operacion, overlayElement) {
    if (overlayElement) $(overlayElement).LoadingOverlay("hide");
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

$(document).ready(function () {
    fetch("ListaTipoProducto")
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(responseJson => {
            responseJson.forEach(item => {
                $("#cboTipoProducto").append($("<option>").val(item.secuencial).text(item.nombre));
            });
        })
        .catch(error => {
            manejarErrorFetch(error, "Cargar Tipos de Producto");
        });

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": 'Lista',
            "type": "GET",
            "datatype": "json",
            "error": function(jqXHR) { manejarErrorFetch(jqXHR.responseJSON, "Cargar Productos"); }
        },
        "columns": [
            { data: "secuencial", visible: false },
            { data: "urlImagen", render: data => `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>` },
            { data: "nombre" },
            { data: "marca" },
            { data: "sistema" },
            { data: "descripcion" },
            { data: "nombreTipoProducto" },
            { data: "stock" },
            { data: "precio" },
            { data: "estaActivo", render: data => data == 1 ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>' },
            {
                "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button>' +
                                  '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button></div>',
                "orderable": false, "searchable": false, "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: ['excelHtml5', 'pageLength'],
        language: { url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json" },
    });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial);
    $("#txtNombre").val(modelo.nombre);
    $("#txtMarca").val(modelo.marca);
    $("#cboSistema").val(modelo.sistema == "" ? $("#cboSistema option:first").val() : modelo.sistema);
    $("#txtCapacidad").val(modelo.capacidad);
    $("#cboMotor").val(modelo.motor == "" ? $("#cboMotor option:first").val() : modelo.motor);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboTipoProducto").val(modelo.secTipoProducto == 0 ? $("#cboTipoProducto option:first").val() : modelo.secTipoProducto);
    $("#txtStock").val(modelo.stock);
    $("#txtPrecio").val(modelo.precio);
    $("#cboEstado").val(modelo.estaActivo);
    $("#txtImagen").val("");
    $("#imgProducto").attr("src", modelo.urlImagen);
    $("#modalData").modal("show");
}

$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal();
});

$("#btnGuardar").click(function () {
    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");
    if (inputs_vacios.length > 0) {
        toastr.warning(`Debe llenar el campo: "${inputs_vacios[0].name}"`);
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["secuencial"] = $("#txtId").val();
    modelo["nombre"] = $("#txtNombre").val();
    modelo["marca"] = $("#txtMarca").val();
    modelo["sistema"] = $("#cboSistema").val();
    modelo["capacidad"] = $("#txtCapacidad").val();
    modelo["motor"] = $("#cboMotor").val();
    modelo["descripcion"] = $("#txtDescripcion").val();
    modelo["secTipoProducto"] = parseInt($("#cboTipoProducto").val());
    modelo["stock"] = $("#txtStock").val();
    modelo["estaActivo"] = $("#cboEstado").val();
    modelo["precio"] = parseFloat($("#txtPrecio").val());

    const inputImagen = document.getElementById("txtImagen");
    const datosFormulario = new FormData();
    datosFormulario.append("imagen", inputImagen.files[0]);
    datosFormulario.append("modelo", JSON.stringify(modelo));

    const url = esEdicion ? "Editar" : "Crear";
    const method = esEdicion ? "PUT" : "POST";
    const modalContent = $("#modalData .modal-content");

    modalContent.LoadingOverlay("show");

    fetch(url, { method: method, body: datosFormulario })
        .then(response => {
            if (!response.ok) return response.json().then(err => Promise.reject(err));
            return response.json();
        })
        .then(responseJson => {
            modalContent.LoadingOverlay("hide");
            if (responseJson.estado) {
                if (esEdicion) {
                    tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
                } else {
                    tablaData.row.add(responseJson.objeto).draw(false);
                }
                $("#modalData").modal("hide");
                Swal.fire("Listo!", `Producto ${esEdicion ? 'editado' : 'creado'} correctamente`, "success");
            } else {
                Swal.fire("Fallo!", responseJson.mensajes, "error");
            }
        })
        .catch(error => {
            manejarErrorFetch(error, "Guardar Producto", modalContent);
        });
});

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;
    filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(fila).data();

    Swal.fire({ /* ... */ }).then((result) => {
        if (result.isConfirmed) {
            const sweetAlertOverlay = $(".swal2-container");
            sweetAlertOverlay.LoadingOverlay("show");

            fetch(`Eliminar?secuencial=${data.secuencial}`, { method: "DELETE" })
                .then(response => {
                    if (!response.ok) return response.json().then(err => Promise.reject(err));
                    return response.json();
                })
                .then(responseJson => {
                    sweetAlertOverlay.LoadingOverlay("hide");
                    if (responseJson.estado) {
                        tablaData.row(fila).remove().draw();
                        Swal.fire("Listo!", `El producto "${data.nombre}" fue eliminado`, "success");
                    } else {
                        Swal.fire("Fallo!", responseJson.mensajes, "error");
                    }
                })
                .catch(error => {
                    manejarErrorFetch(error, "Eliminar Producto", sweetAlertOverlay);
                });
        }
    });
});