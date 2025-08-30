let tablaData;
// Corregido: Apunta a modalData, el id estándar en la nueva vista.
const modalData = new bootstrap.Modal(document.getElementById('modalData'));

const modeloBase = {
    idPermiso: "",
    descripcion: ""
};

$(document).ready(function () {
    tablaData = $('#tablaPermisos').DataTable({
        responsive: true,
        "ajax": {
            "url": "/Permiso/Lista",
            "type": "GET",
            "datatype": "json",
            "dataSrc": "listaObjeto.$values"
        },
        "columns": [
            { "data": "idPermiso" },
            { "data": "descripcion" },
            {
                "defaultContent": '<button class="btn btn-primary btn-sm btn-editar"><i class="fas fa-pencil-alt"></i></button>' +
                                  '<button class="btn btn-danger btn-sm ms-2 btn-eliminar"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "asc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Permisos',
                exportOptions: {
                    columns: [0, 1]
                }
            },
            'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });
});


function mostrarModal(modelo = modeloBase) {
    $("#txtIdPermiso").val(modelo.idPermiso);
    $("#txtIdPermisoInput").val(modelo.idPermiso);
    $("#txtDescripcion").val(modelo.descripcion);

    // Si es un nuevo permiso, el ID es editable. Si se edita, no.
    $("#txtIdPermisoInput").prop("disabled", modelo.idPermiso !== "");

    modalData.show(); // Corregido
}

$("#btnNuevo").click(function () {
    mostrarModal();
});

$("#btnGuardar").click(function () {
    const idOriginal = $("#txtIdPermiso").val(); // El ID antes de la edición
    const esNuevo = idOriginal === "";

    const modelo = {
        idPermiso: $("#txtIdPermisoInput").val(),
        descripcion: $("#txtDescripcion").val()
    };

    const url = esNuevo ? "/Permiso/Crear" : "/Permiso/Editar";
    const method = esNuevo ? "POST" : "PUT";

    fetch(url, {
        method: method,
        headers: {
            "Content-Type": "application/json; charset=utf-8"
        },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        if (responseJson.estado) {
            modalData.hide(); // Corregido
            Swal.fire("Listo!", "El permiso fue guardado", "success");
            tablaData.ajax.reload();
        } else {
            Swal.fire("Error", responseJson.mensaje, "error");
        }
    })
    .catch((error) => {
        Swal.fire("Error", "No se pudo guardar el permiso", "error");
    });
});

let filaSeleccionada;
$("#tablaPermisos tbody").on("click", ".btn-editar", function () {
    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }
    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$("#tablaPermisos tbody").on("click", ".btn-eliminar", function () {
    let fila;
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }
    const data = tablaData.row(fila).data();

    Swal.fire({
        title: "¿Está seguro?",
        text: `¿Eliminar el permiso "${data.idPermiso}"?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Sí, eliminar",
        cancelButtonText: "No, cancelar"
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(`/Permiso/Eliminar?idPermiso=${data.idPermiso}`, {
                method: "DELETE"
            })
            .then(response => {
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    Swal.fire("Listo!", "El permiso fue eliminado", "success");
                    tablaData.row(fila).remove().draw();
                } else {
                    Swal.fire("Error", responseJson.mensaje, "error");
                }
            })
            .catch((error) => {
                Swal.fire("Error", "No se pudo eliminar el permiso", "error");
            });
        }
    });
});