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
    // Configuración de idioma local en español para DataTable
    const lenguajeEspanol = {
        processing:     "Procesando...",
        search:         "",
        searchPlaceholder: "Buscar...",
        lengthMenu:    "Mostrar _MENU_",
        info:           "Mostrando _START_ a _END_ de _TOTAL_ registros",
        infoEmpty:      "Mostrando 0 a 0 de 0 registros",
        infoFiltered:   "(filtrado de _MAX_ registros totales)",
        loadingRecords: "Cargando...",
        zeroRecords:    "No se encontraron resultados",
        emptyTable:     "Ningún dato disponible en esta tabla",
        paginate: {
            first:      "Primero",
            previous:   "Anterior",
            next:       "Siguiente",
            last:       "Último"
        }
    };

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
                data: "secuencial",
                render: function (data, type, row) {
                    return `<div class="dropdown">` +
                           `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #007bff; border-color: #007bff;">` +
                           `<i class="fas fa-cog text-warning mr-1"></i> Acciones` +
                           `</button>` +
                           `<div class="dropdown-menu">` +
                           `<a class="dropdown-item btn-editar" href="#"><i class="fas fa-pencil-alt text-primary mr-2"></i> Editar</a>` +
                           `<a class="dropdown-item btn-eliminar" href="#"><i class="fas fa-trash-alt text-danger mr-2"></i> Eliminar</a>` +
                           `</div>` +
                           `</div>`;
                },
                "orderable": false,
                "searchable": false,
                "width": "120px"
            }
        ],
        order: [[0, "asc"]],
        dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
        buttons: [
            {
                text: '<i class="fas fa-file-excel text-success fa-lg"></i>',
                extend: 'excelHtml5',
                title: 'Diccionario de Parámetros',
                filename: 'Reporte Diccionario Parámetros',
                exportOptions: {
                    columns: [0, 1, 2]
                },
                className: 'btn btn-link btn-sm p-1'
            }
        ],
        "language": lenguajeEspanol,
        initComplete: function() {
            $("#btnNuevo").appendTo(".toolbar-left");
            $("#btnNuevo").closest(".row").show();
        }
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
