const MODELO_BASE = {
    secuencial: 0,
    secRol: 0,
    secMenu: 0,
    esActivo: 1,
    fechaRegistro: ""
}

let tablaData;

$(document).ready(function () {
    Promise.all([
        fetch("/RolMenu/ObtenerRoles")
            .then(response => {
                if (!response.ok) throw new Error('Error al obtener roles');
                return response.json();
            })
            .then(responseJson => {
                console.log("Roles obtenidos:", responseJson);
                const roles = responseJson.$values;
                $("#cboRol").empty(); // Clear existing options
                if (roles && roles.length > 0) {
                    roles.forEach((item) => {
                        $("#cboRol").append(
                            $("<option>").val(item.secuencial).text(item.descripcion)
                        );
                    });
                }
            })
            .catch(error => {
                console.error("Error al cargar roles:", error);
            }),
        fetch("/RolMenu/ObtenerMenusHijos")
            .then(response => {
                if (!response.ok) throw new Error('Error al obtener menús');
                return response.json();
            })
            .then(responseJson => {
                console.log("Menús obtenidos:", responseJson);
                const menus = responseJson; // La respuesta ahora es un array JSON plano
                $("#cboMenu").empty(); // Clear existing options
                if (menus && menus.length > 0) {
                    menus.forEach((item) => {
                        $("#cboMenu").append(
                            $("<option>").val(item.secuencial).text(item.descripcion)
                        );
                    });
                }
            })
            .catch(error => {
                console.error("Error al cargar menús:", error);
            })
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
                        if (data == 1)
                            return '<span class="badge badge-info">Activo</span>';
                        else
                            return '<span class="badge badge-danger">Inactivo</span>';
                    }
                },
                {
                    "data": "fechaRegistro", render: function (data) {
                        return moment(data).format('DD/MM/YYYY');
                    }
                },
                {
                    "defaultContent": '<button class="btn btn-warning btn-editar btn-sm"><i class="fas fa-pencil-alt"></i></button>' +
                        '<button class="btn btn-danger btn-eliminar btn-sm ml-2"><i class="fas fa-trash-alt"></i></button>',
                    "orderable": false,
                    "searchable": false,
                    "width": "80px"
                }
            ],
            order: [[0, "desc"]],
            dom: "Bfrtip",
            buttons: [
                {
                    text: 'Exportar Excel',
                    extend: 'excelHtml5',
                    title: '',
                    filename: 'Reporte Roles Menus',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4]
                    }
                },
                {
                    text: 'Exportar Pdf',
                    extend: 'pdfHtml5',
                    title: '',
                    filename: 'Reporte Roles Menus',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4]
                    }
                }
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });
    });
})

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial)
    $("#cboRol").val(modelo.secRol)
    $("#cboMenu").val(modelo.secMenu)
    $("#cboEstado").val(modelo.esActivo)

    $("#modalData").modal("show")
}

$("#btnNuevo").on("click", function () {
    mostrarModal()
})

$("#btnGuardar").on("click", function () {

    const modelo = {
        secuencial: $("#txtId").val(),
        secRol: $("#cboRol").val(),
        secMenu: $("#cboMenu").val(),
        esActivo: $("#cboEstado").val()
    }

    fetch("/RolMenu/ProcesaGuardarRolMenu", {
        method: "POST",
        headers: { "Content-Type": "application/json; charset=utf-8" },
        body: JSON.stringify(modelo)
    })
        .then(response => {
            $("#modalData").modal("hide")
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            if (responseJson.estado) {
                tablaData.row.add(responseJson.objeto).draw(false)
                swal("Listo!", "Rol-Menú fue registrado", "success")
            } else {
                swal("Error", "No se pudo registrar el Rol-Menú", "error")
            }
        }).catch((error) => {
            console.error("Error en la solicitud:", error);
            swal("Error", "Ocurrió un error al procesar la solicitud", "error");
        });
})

$("#tbdata tbody").on("click", ".btn-editar", function () {
    let data = tablaData.row($(this).parents('tr')).data();

    mostrarModal(data);
})

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let data = tablaData.row($(this).parents('tr')).data();

    swal({
        title: "¿Está seguro?",
        text: `Eliminar Rol-Menú "${data.secuencial}"`, // Usar secuencial para identificar
        type: "warning",
        showCancelButton: true,
        confirmButtonText: "Si, eliminar",
        confirmButtonColor: "#DD6B55",
        cancelButtonText: "No, cancelar",
        closeOnConfirm: false,
        closeOnCancel: true
    }, function (isConfirm) {
        if (isConfirm) {
            fetch(`/RolMenu/Eliminar?secuencial=${data.secuencial}`, {
                method: "DELETE"
            })
                .then(response => {
                    return response.ok ? response.json() : Promise.reject(response);
                })
                .then(responseJson => {
                    if (responseJson.estado) {
                        tablaData.row($("#tbdata tbody .btn-eliminar").parents('tr')).remove().draw()
                        swal("Listo!", "Rol-Menú fue eliminado", "success")
                    } else {
                        swal("Error", "No se pudo eliminar el Rol-Menú", "error")
                    }
                }).catch((error) => {
                    console.error("Error en la solicitud:", error);
                    swal("Error", "Ocurrió un error al procesar la solicitud", "error");
                });
        }
    })
})
