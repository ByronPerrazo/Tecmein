
const MODELO_BASE = {
    secuencial: "",
    nombre: "",
    urlCatalogo: "",
    fechaRegistro: "",
    estaActivo: 1
}

$(document).ready(function () {


        tablaData =
            $('#tbdataCatalogo').DataTable({
            responsive: true,
            "ajax": {
                "url": 'Lista',
                "type": "GET",
                "datatype": "json",
                "dataSrc": "data.$values",
            },
            "columns": [
                { data: "secuencial", visible: false, searchable: true },
                { data: "nombre", searchable: true },
                {
                    data: "urlCatalogo", render: function (data) {
                        return `<a href="${data}" target="_blank" class="btn-link btn-sm mr-2"> Abrir Documento <i class="fas fa-external-link-alt"></i></a>`;
                    },
                    "width": "150px"

                },

                { data: "fechaRegistro", searchable: true, "width": "100px" },

                {
                    data: "estaActivo", render: function (data) {
                        if (data == 1)
                            return '<span class="badge badge-info">Activo</span>';
                        else
                            return '<span class="badge badge-danger">Inactivo</span>';
                    },
                    "width": "50px"
                },

                {
                    "defaultContent":
                        /* '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +*/
                        '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',

                    "orderable": false,
                    "searchable": false,
                    "width": "50px"
                }
            ],
            order: [[0, "desc"]],
            dom: "Bfrtip",
            buttons: [
                {
                    text: 'Exportar Excel',
                    extend: 'excelHtml5',
                    title: 'Catalogos',
                    filename: 'Reporte de Catalogos',
                    exportOptions: {
                        columns: [0, 2, 3, 4]
                    }
                }, 'pageLength'
            ],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
            },
        });

    
});

function mostrarModal(modelo = MODELO_BASE) {
    $("#txtId").val(modelo.secuencial)
    $("#txtNombre").val(modelo.nombre)
    $("#txtUrlCatalogo").val(modelo.urlCatalogo)
    $("#txtFechaRegistro").val(modelo.fechaRegistro)
    $("#cboEstado").val(modelo.estaActivo)
    $("#txtDocumento").val("")
    $("#modalData").modal("show")
};

let esEdicion;
$("#btnNuevo").click(function () {
    esEdicion = false;
    mostrarModal();
})

$("#btnGuardarCat").click(function () {

    const modelo = structuredClone(MODELO_BASE);
    modelo["secuencial"] = 0;
    modelo["nombre"] = $("#txtNombre").val().trim();
    modelo["urlCatalogo"] = $("#txtUrlCatalogo").val().trim();
    modelo["fechaRegistro"] = $("#txtFechaRegistro").val().trim();
    modelo["estaActivo"] = $("#cboEstado").val();

    const inputDocumento = document.getElementById("txtDocumento");
    const datosFormulario = new FormData();
    datosFormulario.append("archivoPDF", inputDocumento.files[0]);
    datosFormulario.append("modelo", JSON.stringify(modelo));



    $("#modalData").find("div.modal-content").LoadingOverlay("show");

    if (!esEdicion) {

        fetch("CrearCatalogo", {
            method: "POST",
            body: datosFormulario
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide");

                if (!response.ok) {
                    throw new Error('Network response was not ok: ' + response.statusText);
                }

                return response.ok
                    ? response.json()
                    : Promise.reject(response);
            }).then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false);
                    $("#modalData").modal("hide");
                    swal("Listo!", "Catalogo " + responseJson.objeto.nombre + " Creado ", "success");
                }
                else {
                    swal("Fallo!", responseJson.mensajes, "error");
                }
            })
            .catch(error => {
                console.error('Hubo un problema con la solicitud Fetch:', error);
            });
        ;
    } 

});

$("#tbdataCatalogo tbody").on("click", ".btn-eliminar", function () {

    let fila
    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const data = tablaData.row(fila).data();

    swal({
        title: "Está Seguro de Eliminar?",
        text: `Eliminar el Tipo Producto "${data.nombre}"`,
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
                        debugger;
                        if (responseJson.estado) {
                            tablaData.row(fila).remove().draw(false);

                            swal("Listo!", " El Tipo Producto " + data.nombre + " fue Eliminado", "success");
                        }
                        else {
                            swal("Fallo!", responseJson.mensajes, "error");
                        }
                    });

            }
        }

    )

})