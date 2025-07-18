

$(document).ready(function () {

    $("div.container-fluid").LoadingOverlay("show");

    fetch("ObtenerResumen")
        .then(
            respuesta => {
                $("div.container-fluid").LoadingOverlay("hide");
                return respuesta.ok
                    ? respuesta.json()
                    : Promise.reject(respuesta);
            }
        )
        .then(
            respuestaJson => {

                if (respuestaJson.estado) {
                    let d = respuestaJson.objeto;
                    $("#totalVisita").text(d.totalVisitasUltimaSemana)
                    $("#totalIngresos").text(d.totalIngresosUltimaSemana)
                    $("#totalProductos").text(d.totalMarcas)
                    $("#totalCategorias").text(d.totalEquipos)

                    let barchart_labeles;
                    let barchart_data;

                    if (d.listaVisitasUktimaSemanaVM.length > 0) {
                        barchart_labeles = d.listaVisitasUktimaSemanaVM.map(item => { return item.fecha })
                        barchart_data = d.listaVisitasUktimaSemanaVM.map(item => { return item.total })

                    } else {
                        barchart_labeles = ["Sin Resultados"]
                        barchart_data = [0]
                    }

                    let piechart_labeles;
                    let piechart_data;

                    if (d.listaMarcasMasVendidasVM.length > 0) {
                        piechart_labeles = d.listaMarcasMasVendidasVM.map(item => { return item.marca })
                        piechart_data = d.listaMarcasMasVendidasVM.map(item => { return item.totalCantidad })

                    } else {
                        piechart_labeles = ["Sin Resultados"]
                        piechart_data = [0]
                    }

                    /////////

                    let controlVenta = document.getElementById("charVentas");
                    let myBarChart = new Chart(controlVenta, {
                        type: 'bar',
                        data: {
                            labels: barchart_labeles,
                            datasets: [{
                                label: "Cantidad",
                                backgroundColor: "#5e93df",
                                hoverBackgroundColor: "#2e59A9",
                                borderColor: "#4e73df",
                                data: barchart_data,
                            }],
                        },
                        options: {
                            maintainAspectRatio: false,
                            legend: {
                                display: false
                            },
                            scales: {
                                xAxes: [{
                                    gridLines: {
                                        display: false,
                                        drawBorder: false
                                    },
                                    maxBarThickness: 50,
                                }],
                                yAxes: [{
                                    ticks: {
                                        min: 0,
                                        maxTicksLimit: 5
                                    }
                                }],
                            },
                        }
                    });


                    /////////

                    Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
                    Chart.defaults.global.defaultFontColor = '#858796';

                    // Pie Chart Example
                    let controlProducto = document.getElementById("charProductos");
                    let myPieChart = new Chart(controlProducto, {
                        type: 'doughnut',
                        data: {
                            labels: piechart_labeles,
                            datasets: [{
                                data: piechart_data,
                                backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', "#FF785B"],
                                hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', "#FF5733"],
                                hoverBorderColor: "rgba(234, 236, 244, 1)",
                            }],
                        },
                        options: {
                            maintainAspectRatio: false,
                            tooltips: {
                                backgroundColor: "rgb(255,255,255)",
                                bodyFontColor: "#858796",
                                borderColor: '#dddfeb',
                                borderWidth: 1,
                                xPadding: 15,
                                yPadding: 15,
                                displayColors: false,
                                caretPadding: 10,
                            },
                            legend: {
                                display: true
                            },
                            cutoutPercentage: 80,
                        },
                    });

                }
            }
        )
        .catch(error => {
            console.error('Error al obtener resumen :', error);
        });


});

