const MODELO_BASE = {
    secuencial: 0,
    parametro: "",
    descripcion: "",
    estaActivo: 1
}

let tablaData;

function manejarErrorFetch(error, operacion) {
    console.error(`Error en ${operacion}:`, error);
    if (error && error.mensajes) {
        Swal.fire("Error", error.mensajes, "error");
    } else {
        Swal.fire("Error", `Ocurrió un error inesperado durante: ${operacion}.`, "error");
    }
}

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/DiccionarioParametro/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function(json) { return json.data ? json.data.$values : []; },
            "error": function (jqXHR, textStatus, errorThrown) {
                 manejarErrorFetch(jqXHR.responseJSON || { mensajes: "No se pudo cargar la lista de parámetros." }, "Cargar Lista");
            }
        },
        "columns": [
            {
                data: "parametro", render: function (data) {
                    return data.replace(/{{|}}/g, "");
                }
            },
            { data: "descripcion" },
            {
                data: "estaActivo", render: function (data) {
                    return data == 1 ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                "defaultContent": '<div class="btn-group" role="group"><button class="btn btn-primary btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button>' +
                                  '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button></div>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "asc"]],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
});

function mostrarModal(modelo = MODELO_BASE) {
    const parametroParaMostrar = modelo.parametro.replace(/{{|}}/g, "");
    $("#txtSecuencial").val(modelo.secuencial);
    $("#txtParametro").val(parametroParaMostrar);
    $("#txtDescripcion").val(modelo.descripcion);
    $("#cboEstado").val(modelo.estaActivo ? 1 : 0);
    $("#modalData").modal("show");
}

$("#btnNuevo").click(function () {
    mostrarModal();
});

$("#btnGuardar").click(function () {
    const esNuevo = $("#txtSecuencial").val() == "0";
    const parametroSinFormato = $("#txtParametro").val().trim();

    if (parametroSinFormato === "" || $("#txtDescripcion").val().trim() === "") {
        Swal.fire("Oops!", "Los campos Parámetro y Descripción no pueden estar vacíos.", "warning");
        return;
    }

    const modelo = {
        secuencial: parseInt($("#txtSecuencial").val()),
        parametro: `{{${parametroSinFormato}}}`, 
        descripcion: $("#txtDescripcion").val(),
        estaActivo: $("#cboEstado").val() == "1"
    };

    const url = esNuevo ? "/DiccionarioParametro/Crear" : "/DiccionarioParametro/Editar";
    const method = esNuevo ? "POST" : "PUT";

    fetch(url, {
        method: method,
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        if (!response.ok) return response.json().then(err => Promise.reject(err));
        return response.json();
    })
    .then(responseJson => {
        if (responseJson.estado) {
            tablaData.ajax.reload(null, false);
            $("#modalData").modal("hide");
            Swal.fire("¡Listo!", `El parámetro fue ${esNuevo ? 'creado' : 'actualizado'} correctamente.`, "success");
        } else {
            Swal.fire("Error", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        manejarErrorFetch(error, "Guardar Parámetro");
    });
});

let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    filaSeleccionada = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila = $(this).closest("tr").hasClass("child") ? $(this).closest("tr").prev() : $(this).closest("tr");
    const data = tablaData.row(fila).data();

    Swal.fire({
        title: '¿Está seguro?',
        text: `¿Desea eliminar el parámetro "${data.parametro.replace(/{{|}}/g, "")}"?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`/DiccionarioParametro/Eliminar?secuencial=${data.secuencial}`, {
                method: "DELETE"
            })
            .then(response => {
                if (!response.ok) return response.json().then(err => Promise.reject(err));
                return response.json();
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw();
                    Swal.fire('¡Eliminado!', 'El parámetro ha sido eliminado.', 'success');
                } else {
                    Swal.fire('Error', responseJson.mensajes, 'error');
                }
            })
            .catch(error => {
                manejarErrorFetch(error, "Eliminar Parámetro");
            });
        }
    });
});
