const MODELO_BASE = {
    id: 0,
    codigo: "",
    descripcion: "",
    porcentaje: 0,
    valorFijo: 0,
    codigoSri: "",
    secTipoImpuesto: 0,
    vigente: true
}

let tablaData;
let filaSeleccionada;

$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": 'Lista',
            "type": "GET",
            "datatype": "json",
            "dataSrc": function (json) {
                return json.data && json.data.$values ? json.data.$values : json.data;
            }
        },
        "columns": [
            { data: "id", visible: false, searchable: false },
            { data: "codigo" },
            { data: "descripcion" },
            { data: "porcentaje" },
            { data: "valorFijo" },
            { data: "codigoSri" },
            { data: "nombreTipoImpuesto" },
            {
                data: "vigente", render: function (data) {
                    if (data)
                        return '<span class="badge badge-info">Vigente</span>';
                    else
                        return '<span class="badge badge-danger">No Vigente</span>';
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
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: 'Impuestos',
                filename: 'Reporte Impuestos',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.id)
    $("#txtCodigo").val(modelo.codigo)
    $("#txtDescripcion").val(modelo.descripcion)
    $("#txtPorcentaje").val(modelo.porcentaje)
    $("#txtValorFijo").val(modelo.valorFijo)
    $("#txtCodigoSri").val(modelo.codigoSri)
    $("#cboVigente").val(modelo.vigente.toString())

    $("#modalData").modal("show")

    fetch("ListaTipoImpuesto")
        .then(response => response.ok ? response.json() : Promise.reject(response))
        .then(responseJson => {
            const cboTipoImpuesto = $("#cboTipoImpuesto");
            cboTipoImpuesto.empty();
            if (responseJson.length > 0) {
                responseJson.forEach(item => {
                    cboTipoImpuesto.append(
                        $("<option>").val(item.secuencial).text(item.nombre)
                    )
                });
            }
            cboTipoImpuesto.val(modelo.secTipoImpuesto);
        })
        .catch(error => {
            console.error('Error al obtener la lista de tipos de impuesto:', error);
        });
};

let esEdicion = false;

$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal();
})

$("#tbdata tbody").on("click", ".btn-editar", function () {
    esEdicion = true;

    if ($(this).closest("tr").hasClass("child")) {
        filaSeleccionada = $(this).closest("tr").prev();
    } else {
        filaSeleccionada = $(this).closest("tr");
    }

    const data = tablaData.row(filaSeleccionada).data();
    mostrarModal(data);
});

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_vacios = inputs.filter(item => item.value.trim() == "");

    if (inputs_vacios.length > 0) {
        const mensaje = `Debe llenar el campo: "${inputs_vacios[0].name}"`;
        toastr.warning("", mensaje);
        $(`input[name="${inputs_vacios[0].name}"]`).focus();
        return;
    }

    const modelo = structuredClone(MODELO_BASE);
    modelo["id"] = $("#txtId").val();
    modelo["codigo"] = $("#txtCodigo").val();
    modelo["descripcion"] = $("#txtDescripcion").val();
    modelo["porcentaje"] = parseFloat($("#txtPorcentaje").val());
    modelo["valorFijo"] = parseFloat($("#txtValorFijo").val());
    modelo["codigoSri"] = $("#txtCodigoSri").val();
    modelo["secTipoImpuesto"] = $("#cboTipoImpuesto").val();
    modelo["vigente"] = $("#cboVigente").val() === "true";

    const url = esEdicion ? "Editar" : "Crear";
    const method = esEdicion ? "PUT" : "POST";

    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    fetch(url, {
        method: method,
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(modelo)
    })
    .then(response => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        return response.ok ? response.json() : Promise.reject(response);
    })
    .then(responseJson => {
        if (responseJson.estado) {
            if(esEdicion) {
                tablaData.row(filaSeleccionada).data(responseJson.objeto).draw(false);
            } else {
                tablaData.row.add(responseJson.objeto).draw(false);
            }
            $("#modalData").modal("hide");
            Swal.fire("Listo!", `Impuesto ${esEdicion ? 'editado' : 'creado'} correctamente`, "success");
        } else {
            Swal.fire("Fallo!", responseJson.mensajes, "error");
        }
    })
    .catch(error => {
        $("#modalData").find("div.modal-content").LoadingOverlay("hide");
        console.error("Error al guardar:", error);
    });
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
        title: "Está Seguro de Eliminar?",
        text: `Eliminar el impuesto "${data.descripcion}"`, // Changed to descripcion
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Si, eliminar",
        cancelButtonText: "No, cancelar",
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            $(".showSweetAlert").LoadingOverlay("show");

            fetch(`Eliminar?id=${data.id}`, {
                method: "DELETE"
            })
            .then(response => {
                $(".showSweetAlert").LoadingOverlay("hide");
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(fila).remove().draw();
                    Swal.fire("Listo!", "El Impuesto fue eliminado", "success");
                } else {
                    Swal.fire("Fallo!", responseJson.mensajes, "error");
                }
            });
        }
    });
});