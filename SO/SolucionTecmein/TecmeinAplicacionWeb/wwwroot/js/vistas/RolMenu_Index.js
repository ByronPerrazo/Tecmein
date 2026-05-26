const MODELO_BASE = {
    secuencial: 0,
    secRol: 0,
    secMenu: 0,
    esActivo: 1,
    fechaRegistro: ""
}

let tablaData;

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

$(document).ready(function () {
    Promise.all([
        fetch("/RolMenu/ObtenerRoles")
            .then(response => {
                if (!response.ok) throw new Error('Error al obtener roles');
                return response.json();
            })
            .then(responseJson => {
                const roles = responseJson.$values;
                $("#cboRol").empty();
                if (roles && roles.length > 0) {
                    roles.forEach((item) => {
                        $("#cboRol").append($("<option>").val(item.secuencial).text(item.descripcion));
                    });
                }
            })
            .catch(error => console.error("Error al cargar roles:", error)),
        fetch("/RolMenu/ObtenerMenusHijos")
            .then(response => {
                if (!response.ok) throw new Error('Error al obtener menús');
                return response.json();
            })
            .then(responseJson => {
                const menus = responseJson;
                $("#cboMenu").empty();
                if (menus && menus.length > 0) {
                    menus.forEach((item) => {
                        $("#cboMenu").append($("<option>").val(item.secuencial).text(item.descripcion));
                    });
                }
            })
            .catch(error => console.error("Error al cargar menús:", error))
    ]).then(() => {
        tablaData = $('#tbdata').DataTable({
            responsive: true,
            "ajax": {
                "url": '/RolMenu/ListaRolMenu',
                "type": "GET",
                "datatype": "json",
                "dataSrc": function (json) {
                    return json.data.$values;
                }
            },
            "columns": [
                { "data": "secuencial" },
                { "data": "descripcionRol" },
                { "data": "descripcionMenu" },
                {
                    "data": "esActivo", render: function (data) {
                        return data == 1 ? '<span class="badge badge-info">Activo</span>' : '<span class="badge badge-danger">Inactivo</span>';
                    }
                },
                {
                    "data": "fechaRegistro", render: function (data) {
                        if (!data) return "";
                        var date = new Date(data);
                        var day = ("0" + date.getDate()).slice(-2);
                        var month = ("0" + (date.getMonth() + 1)).slice(-2);
                        var year = date.getFullYear();
                        return `${day}/${month}/${year}`;
                    }
                },
                {
                    "data": "secuencial",
                    "render": function (data, type, row) {
                        return `<div class="dropdown">` +
                               `<button class="btn btn-primary btn-sm dropdown-toggle rounded-pill" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" style="background-color: #004A93; border-color: #004A93;">` +
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
            order: [[0, "desc"]],
            dom: '<"row mb-2 align-items-center"<"col-sm-12 col-md-6 d-flex align-items-center gap-2"<"toolbar-left">f><"col-sm-12 col-md-6 d-flex justify-content-end align-items-center gap-2"B l>>rtip',
            buttons: [
                { 
                    text: '<i class="fas fa-file-excel text-success fa-lg"></i>', 
                    extend: 'excelHtml5', 
                    title: 'Reporte Roles Menus', 
                    filename: 'Reporte Roles Menus', 
                    exportOptions: { columns: [0, 1, 2, 3, 4] },
                    className: 'btn btn-link btn-sm p-1'
                },
                { 
                    text: '<i class="fas fa-file-pdf text-danger fa-lg"></i>', 
                    extend: 'pdfHtml5', 
                    title: 'Reporte Roles Menus', 
                    filename: 'Reporte Roles Menus', 
                    exportOptions: { columns: [0, 1, 2, 3, 4] },
                    className: 'btn btn-link btn-sm p-1'
                }
            ],
            language: lenguajeEspanol,
            initComplete: function() {
                $("#btnNuevo").appendTo(".toolbar-left");
                $("#btnNuevo").closest(".row").show();
            }
        });
    });
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial);
    $("#cboRol").val(modelo.secRol);
    $("#cboMenu").val(modelo.secMenu);
    $("#cboEstado").val(modelo.esActivo);
    $("#modalData").modal("show");
}

$("#btnNuevo").on("click", function () {
    mostrarModal();
});

$("#btnGuardar").on("click", function () {
    const modelo = {
        secuencial: $("#txtId").val(),
        secRol: $("#cboRol").val(),
        secMenu: $("#cboMenu").val(),
        esActivo: $("#cboEstado").val()
    };

    fetch("/RolMenu/ProcesaGuardarRolMenu", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        $("#modalData").modal("hide");
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        if (responseJson.estado) {
            tablaData.ajax.reload();
            Swal.fire("Listo!", "Rol-Menú fue registrado", "success");
        } else {
            Swal.fire("Error", "No se pudo registrar el Rol-Menú", "error");
        }
    })
    .catch((error) => {
        console.error("Error en la solicitud:", error);
        Swal.fire("Error", "Ocurrió un error al procesar la solicitud", "error");
    });
});

$("#tbdata tbody").on("click", ".btn-editar", function () {
    let data = tablaData.row($(this).parents('tr')).data();
    mostrarModal(data);
});

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let data = tablaData.row($(this).parents('tr')).data();

    Swal.fire({
        title: "¿Está seguro?",
        text: `Eliminar Rol-Menú \"${data.secuencial}\"`, 
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`/RolMenu/Eliminar?secuencial=${data.secuencial}`, {
                method: "DELETE"
            })
            .then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row($(this).parents('tr')).remove().draw();
                    Swal.fire("Listo!", "Rol-Menú fue eliminado", "success");
                } else {
                    Swal.fire("Error", "No se pudo eliminar el Rol-Menú", "error");
                }
            }).catch((error) => {
                console.error("Error en la solicitud:", error);
                Swal.fire("Error", "Ocurrió un error al procesar la solicitud", "error");
            });
        }
    });
});
