const MODELO_BASE = {
    secuencial: 0,
    parametro: "",
    descripcion: "",
    estaActivo: 1
}

let tablaData;

$(document).ready(function () {
    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/DiccionarioParametro/Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": "data.$values" // Adaptado para el serializador de .NET
        },
        "columns": [
            {
                data: "parametro", render: function (data) {
                    // Se quitan las llaves solo para la visualización en la tabla
                    return data.replace(/{{|}}/g, "");
                }
            },
            { data: "descripcion" },
            {
                data: "estaActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else
                        return '<span class="badge badge-danger">Inactivo</span>';
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                                  '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
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
    // Si el parámetro viene de la BD (ej: {{nombre}}), se quitan las llaves para mostrarlo en el input
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

    // Simple validación para campos no vacíos
    if (parametroSinFormato === "" || $("#txtDescripcion").val().trim() === "") {
        Swal.fire("Oops!", "Los campos Parámetro y Descripción no pueden estar vacíos.", "warning");
        return;
    }

    const modelo = {
        secuencial: parseInt($("#txtSecuencial").val()),
        parametro: `{{${parametroSinFormato}}}`, // Se añaden las llaves automáticamente
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
    .then(response => response.json())
    .then(responseJson => {
        if (responseJson.estado) {
            tablaData.ajax.reload(null, false);
            $("#modalData").modal("hide");
            Swal.fire("¡Listo!", `El parámetro fue ${esNuevo ? 'creado' : 'actualizado'} correctamente.`, "success");
        } else {
            Swal.fire("Error", "No se pudo guardar el parámetro.", "error");
        }
    })
    .catch(error => {
        console.error("Error al guardar:", error);
        Swal.fire("Error", "Ocurrió un error al intentar guardar.", "error");
    });
});

let filaSeleccionada;
$("#tbdata tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaData.row(fila).data();

    Swal.fire({
        title: '¿Está seguro?',
        text: `¿Desea eliminar el parámetro "${data.parametro}"?`,
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
            .then(response => response.json())
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw();
                    Swal.fire('¡Eliminado!', 'El parámetro ha sido eliminado.', 'success');
                } else {
                    Swal.fire('Error', 'No se pudo eliminar el parámetro.', 'error');
                }
            })
            .catch(error => {
                console.error("Error al eliminar:", error);
                Swal.fire("Error", "Ocurrió un error al intentar eliminar.", "error");
            });
        }
    });
});