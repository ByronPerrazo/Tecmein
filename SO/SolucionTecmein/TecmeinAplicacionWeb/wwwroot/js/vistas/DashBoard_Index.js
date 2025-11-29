$(document).ready(function () {

    $("div.container-fluid").LoadingOverlay("show");

    // #region MCP (Asistente Tecmein) Element Initializations
    const mcpQueryInput = $('#mcpQueryInput');
    const mcpVoiceInputButton = $('#mcpVoiceInputButton');
    const mcpSendButton = $('#mcpSendButton');
    const mcpResponseArea = $('#mcpResponseArea');
    // #endregion

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

                    // Update top cards
                    $("#totalContratos").text(d.totalContratos || 0); // New ID
                    $("#ingresosMensuales").text(d.ingresosMensuales || '$0'); // New ID
                    $("#pagosVencidos").text(d.pagosVencidos || 0); // New ID
                    $("#nuevosClientes").text(d.nuevosClientes || 0); // New ID

                    let barchart_labeles = d.listaVisitasUktimaSemanaVM.map(item => item.fecha);
                    let barchart_data = d.listaVisitasUktimaSemanaVM.map(item => item.total);

                    let piechart_labeles = d.listaMarcasMasVendidasVM.map(item => item.marca);
                    let piechart_data = d.listaMarcasMasVendidasVM.map(item => item.totalCantidad);

                    // Ensure Chart is defined before rendering
                    if (typeof Chart !== 'undefined') {
                        // Area Chart - Ventas de los ultimos 7 días
                        let controlVenta = document.getElementById("charVentas");
                        new Chart(controlVenta, {
                            type: 'bar',
                            data: {
                                labels: barchart_labeles.length > 0 ? barchart_labeles : ["Sin Resultados"],
                                datasets: [{
                                    label: "Cantidad",
                                    backgroundColor: "#5e93df",
                                    hoverBackgroundColor: "#2e59A9",
                                    borderColor: "#4e73df",
                                    data: barchart_data.length > 0 ? barchart_data : [0],
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

                        // Pie Chart - Marcas Más Vendidas
                        let controlProducto = document.getElementById("charProductos");
                        new Chart(controlProducto, {
                            type: 'doughnut',
                            data: {
                                labels: piechart_labeles.length > 0 ? piechart_labeles : ["Sin Resultados"],
                                datasets: [{
                                    data: piechart_data.length > 0 ? piechart_data : [0],
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

                        // New Charts
                        renderPieChart("chartVisitasPorEtapa", d.visitasPorEtapa, "Visitas por Etapa");
                        renderBarChart("chartContratosPorMes", d.contratosPorMes, "Contratos por Mes");
                        renderHorizontalBarChart("chartTopClientes", d.topClientesConMasContratos, "Top 5 Clientes");

                        Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
                        Chart.defaults.global.defaultFontColor = '#858796';
                    } else {
                        console.error("Chart.js no está definido. Asegúrate de que la librería Chart.js esté cargada antes de DashBoard_Index.js.");
                    }

                }
            }
        )
        .catch(error => {
            console.error('Error al obtener resumen :', error);
        });

    function renderPieChart(canvasId, data, label) {
        if (typeof Chart === 'undefined') return;
        const labels = Object.keys(data);
        const values = Object.values(data);
        const control = document.getElementById(canvasId);
        new Chart(control, {
            type: 'pie',
            data: {
                labels: labels.length > 0 ? labels : ["Sin Resultados"],
                datasets: [{
                    data: values.length > 0 ? values : [0],
                    backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b'],
                    hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf', '#dda20a', '#c73021'],
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
            },
        });
    }

    function renderBarChart(canvasId, data, label) {
        if (typeof Chart === 'undefined') return;
        const labels = Object.keys(data);
        const values = Object.values(data);
        const control = document.getElementById(canvasId);
        new Chart(control, {
            type: 'bar',
            data: {
                labels: labels.length > 0 ? labels : ["Sin Resultados"],
                datasets: [{
                    label: label,
                    backgroundColor: "#4e73df",
                    hoverBackgroundColor: "#2e59A9",
                    borderColor: "#4e73df",
                    data: values.length > 0 ? values : [0],
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
    }

    function renderHorizontalBarChart(canvasId, data, label) {
        if (typeof Chart === 'undefined') return;
        const labels = Object.keys(data);
        const values = Object.values(data);
        const control = document.getElementById(canvasId);
        new Chart(control, {
            type: 'horizontalBar',
            data: {
                labels: labels.length > 0 ? labels : ["Sin Resultados"],
                datasets: [{
                    label: label,
                    backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b'],
                    data: values.length > 0 ? values : [0],
                }],
            },
            options: {
                maintainAspectRatio: false,
                legend: {
                    display: false
                },
                scales: {
                    xAxes: [{
                        ticks: {
                            min: 0,
                            maxTicksLimit: 5
                        }
                    }],
                },
            }
        });
    }

    // #region MCP (Asistente Tecmein) Logic

    let isListening = false;
    let recognition; // Will hold the SpeechRecognition object

    function appendMessage(sender, message, isHtml = false) {
        const messageElement = $(`<div class="mb-2"><strong class="${sender === 'User' ? 'text-primary' : 'text-success'}">${sender}:</strong></div>`);
        if (isHtml) {
            messageElement.append(`<div class="alert alert-secondary">${message}</div>`);
        } else {
            messageElement.append(`<span class="ml-2">${message}</span>`);
        }
        mcpResponseArea.append(messageElement);
        mcpResponseArea.scrollTop(mcpResponseArea[0].scrollHeight);
    }

    async function sendMcpQuery(query) {
        if (!query.trim()) {
            Swal.fire('Atención', 'Por favor, introduce una consulta.', 'warning');
            return;
        }

        appendMessage('Tú', query);
        mcpQueryInput.val('');
        mcpResponseArea.append('<div id="mcpLoading" class="text-center text-muted"><i class="fas fa-spinner fa-spin"></i> Pensando...</div>');
        mcpResponseArea.scrollTop(mcpResponseArea[0].scrollHeight);

        console.log('Enviando consulta al MCP:', query);

        try {
            const response = await fetch('/api/Mcp/query', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ naturalLanguageQuery: query })
            });

            const responseJson = await response.json();

            $('#mcpLoading').remove();

            if (responseJson.estado) {
                appendMessage('Asistente', responseJson.objeto.respuesta, true); // Assuming response.objeto.respuesta contains HTML or rich text
            } else {
                appendMessage('Asistente', `Error: ${responseJson.mensajes || 'No se pudo procesar la consulta.'}`);
            }
        } catch (error) {
            $('#mcpLoading').remove();
            console.error('Error al comunicarse con el MCP:', error);
            appendMessage('Asistente', 'Hubo un error de comunicación con el asistente. Inténtalo de nuevo más tarde.');
        }
    }

    // #endregion MCP Logic

    // #region Speech Recognition Logic

    if ('webkitSpeechRecognition' in window) {
        recognition = new webkitSpeechRecognition();
        recognition.continuous = false; // Detener después de la primera pausa
        recognition.interimResults = false; // Solo resultados finales
        recognition.lang = 'es-ES'; // Establecer el idioma a español

        recognition.onstart = function () {
            isListening = true;
            mcpVoiceInputButton.addClass('btn-danger').removeClass('btn-primary');
            mcpVoiceInputButton.find('i').removeClass('fa-microphone').addClass('fa-stop-circle');
            mcpVoiceInputButton.prop('title', 'Detener dictado');
            appendMessage('Asistente', 'Escuchando...', true); // Indicar que está escuchando
        };

        recognition.onresult = function (event) {
            const transcript = event.results[0][0].transcript;
            mcpQueryInput.val(transcript); // Poner el texto reconocido en el input
            sendMcpQuery(transcript); // Enviar la consulta automáticamente
        };

        recognition.onerror = function (event) {
            console.error('Error de reconocimiento de voz:', event.error);
            $('#mcpLoading').remove();
            appendMessage('Asistente', 'No pude entender tu voz. Por favor, intenta de nuevo o escribe tu consulta.');
            isListening = false;
            mcpVoiceInputButton.removeClass('btn-danger').addClass('btn-primary');
            mcpVoiceInputButton.find('i').removeClass('fa-stop-circle').addClass('fa-microphone');
            mcpVoiceInputButton.prop('title', 'Dictar consulta');
        };

        recognition.onend = function () {
            isListening = false;
            mcpVoiceInputButton.removeClass('btn-danger').addClass('btn-primary');
            mcpVoiceInputButton.find('i').removeClass('fa-stop-circle').addClass('fa-microphone');
            mcpVoiceInputButton.prop('title', 'Dictar consulta');
            // Remove 'Escuchando...' message if no result was processed by onresult
            if (mcpResponseArea.find('div:last-child span:contains("Escuchando...")').length) {
                mcpResponseArea.find('div:last-child').remove();
            }
        };
    } else {
        mcpVoiceInputButton.prop('disabled', true).attr('title', 'Reconocimiento de voz no soportado por tu navegador');
        console.warn('webkitSpeechRecognition no es soportado por este navegador.');
    }

    // #endregion Speech Recognition Logic

    // #region Event Listeners

    mcpSendButton.on('click', function () {
        sendMcpQuery(mcpQueryInput.val());
    });

    mcpQueryInput.on('keypress', function (e) {
        if (e.which == 13) { // Enter key
            sendMcpQuery(mcpQueryInput.val());
        }
    });

    mcpVoiceInputButton.on('click', function () {
        if (isListening) {
            recognition.stop();
        }
        else {
            mcpQueryInput.val(''); // Clear input field before listening
            mcpResponseArea.find('p.text-muted').remove(); // Remove initial message
            recognition.start();
        }
    });

    // #endregion Event Listeners

});