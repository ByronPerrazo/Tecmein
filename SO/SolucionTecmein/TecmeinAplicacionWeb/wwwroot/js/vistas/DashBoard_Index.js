$(document).ready(function () {

    $("div.container-fluid").LoadingOverlay("show");

    // Guardará la información del resumen en el cliente para evitar consultas redundantes de red
    window.dashboardData = null;

    // #region MCP (Asistente Tecmein) Element Initializations y Flotante
    const mcpQueryInput = $('#mcpQueryInput');
    const mcpVoiceInputButton = $('#mcpVoiceInputButton');
    const mcpSendButton = $('#mcpSendButton');
    const mcpResponseArea = $('#mcpResponseArea');
    const mcpInitialMsg = $('#mcpInitialMsg');

    // Toggle de ventana de chat flotante
    $('.mcp-chat-trigger').on('click', function () {
        $('.mcp-chat-window').toggleClass('show');
    });

    $('.close-chat').on('click', function () {
        $('.mcp-chat-window').removeClass('show');
    });
    // #endregion

    // Paleta de colores premium para graficos
    const coloresPremium = ['#004A93', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b', '#6f42c1', '#fd7e14', '#20c997'];
    const coloresPremiumHover = ['#003366', '#148c60', '#26828f', '#cca130', '#b83b2e', '#5a32a3', '#ca6510', '#179c74'];

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
                    window.dashboardData = d; // Guardar copia global

                    // Actualizar contadores
                    $("#totalContratos").text(d.totalContratos || 0);
                    $("#ingresosMensuales").text(d.ingresosMensuales || '$0');
                    $("#pagosVencidos").text(d.pagosVencidos || 0);
                    $("#nuevosClientes").text(d.nuevosClientes || 0);

                    let barchart_labeles = d.listaVisitasUktimaSemanaVM.map(item => item.fecha);
                    let barchart_data = d.listaVisitasUktimaSemanaVM.map(item => item.total);

                    let piechart_labeles = d.listaMarcasMasVendidasVM.map(item => item.marca);
                    let piechart_data = d.listaMarcasMasVendidasVM.map(item => item.totalCantidad);

                    if (typeof Chart !== 'undefined') {
                        // Area Chart - Ventas de los ultimos 7 días
                        let controlVenta = document.getElementById("charVentas");
                        new Chart(controlVenta, {
                            type: 'bar',
                            data: {
                                labels: barchart_labeles.length > 0 ? barchart_labeles : ["Sin Resultados"],
                                datasets: [{
                                    label: "Visitas/Ventas",
                                    backgroundColor: "#36b9cc",
                                    hoverBackgroundColor: "#26828f",
                                    borderColor: "#36b9cc",
                                    data: barchart_data.length > 0 ? barchart_data : [0],
                                }],
                            },
                            options: {
                                maintainAspectRatio: false,
                                legend: { display: false },
                                scales: {
                                    xAxes: [{
                                        gridLines: { display: false, drawBorder: false },
                                        maxBarThickness: 40,
                                    }],
                                    yAxes: [{ ticks: { min: 0, maxTicksLimit: 5 } }],
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
                                    backgroundColor: coloresPremium.slice(0, Math.max(piechart_labeles.length, 1)),
                                    hoverBackgroundColor: coloresPremiumHover.slice(0, Math.max(piechart_labeles.length, 1)),
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
                                    xPadding: 12,
                                    yPadding: 12,
                                    displayColors: true,
                                    caretPadding: 10,
                                },
                                legend: { display: true, position: 'right', labels: { boxWidth: 12, fontSize: 10 } },
                                cutoutPercentage: 75,
                            },
                        });

                        // Nuevos Graficos
                        renderPieChart("chartVisitasPorEtapa", d.visitasPorEtapa, "Visitas por Etapa");
                        renderBarChart("chartContratosPorMes", d.contratosPorMes, "Contratos por Mes");
                        renderHorizontalBarChart("chartTopClientes", d.topClientesConMasContratos, "Top 5 Clientes");

                        Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
                        Chart.defaults.global.defaultFontColor = '#858796';
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
                    backgroundColor: coloresPremium.slice(0, Math.max(labels.length, 1)),
                    hoverBackgroundColor: coloresPremiumHover.slice(0, Math.max(labels.length, 1)),
                    hoverBorderColor: "rgba(234, 236, 244, 1)",
                }],
            },
            options: {
                maintainAspectRatio: false,
                legend: { display: true, position: 'right', labels: { boxWidth: 12, fontSize: 10 } },
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
                    backgroundColor: "#004A93",
                    hoverBackgroundColor: "#003366",
                    borderColor: "#004A93",
                    data: values.length > 0 ? values : [0],
                }],
            },
            options: {
                maintainAspectRatio: false,
                legend: { display: false },
                scales: {
                    xAxes: [{ gridLines: { display: false, drawBorder: false }, maxBarThickness: 40 }],
                    yAxes: [{ ticks: { min: 0, maxTicksLimit: 5 } }],
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
                    backgroundColor: coloresPremium.slice(0, Math.max(labels.length, 1)),
                    data: values.length > 0 ? values : [0],
                }],
            },
            options: {
                maintainAspectRatio: false,
                legend: { display: false },
                scales: {
                    xAxes: [{ ticks: { min: 0, maxTicksLimit: 5 } }],
                },
            }
        });
    }

    // #region Lógica para Ampliar Datos en Modales
    
    // Clic en botones de ampliación de métricas (tarjetas superiores)
    $(document).on('click', '.btn-ampliar-metric', function() {
        const metric = $(this).data('metric');
        const header = $('#tbDetalleHead');
        const body = $('#tbDetalleBody');
        
        header.empty();
        body.empty();
        $('#modalDetalleDashboard').modal('show');
        
        if (metric === 'pagosVencidos') {
            $('#modalDetalleTitulo').html('<i class="fas fa-calendar-times text-danger mr-2"></i> Desglose de Pagos Vencidos');
            body.html('<tr><td colspan="6" class="text-center"><i class="fas fa-spinner fa-spin mr-2"></i> Cargando pagos vencidos...</td></tr>');
            
            fetch('/Dashboard/ObtenerDetallePagosVencidos')
                .then(res => res.json())
                .then(response => {
                    body.empty();
                    header.html(`
                        <tr>
                            <th>Contrato (ID)</th>
                            <th>Cliente / Constructora</th>
                            <th>Cuota N°</th>
                            <th>Monto Pendiente</th>
                            <th>Vencimiento</th>
                            <th>Retraso (Días)</th>
                        </tr>
                    `);
                    
                    if (response.estado && response.objeto.length > 0) {
                        response.objeto.forEach(item => {
                            body.append(`
                                <tr>
                                    <td><strong>${item.numeroContrato}</strong></td>
                                    <td>${item.nombreCliente}</td>
                                    <td class="text-center">${item.numeroCuota}</td>
                                    <td class="text-right font-weight-bold text-danger">$${item.montoCuota.toFixed(2)}</td>
                                    <td>${item.fechaVencimiento}</td>
                                    <td class="text-center"><span class="badge badge-danger">${item.diasVencidos} días</span></td>
                                </tr>
                            `);
                        });
                    } else {
                        body.html('<tr><td colspan="6" class="text-center text-muted">No existen pagos vencidos actualmente.</td></tr>');
                    }
                })
                .catch(err => {
                    body.html('<tr><td colspan="6" class="text-center text-danger">Error al cargar la información.</td></tr>');
                });
        } 
        else if (metric === 'totalContratos') {
            $('#modalDetalleTitulo').html('<i class="fas fa-file-contract text-primary mr-2"></i> Información de Contratos');
            header.html('<tr><th>Indicador</th><th>Valor</th></tr>');
            const total = window.dashboardData ? window.dashboardData.totalContratos : 0;
            body.append(`<tr><td><strong>Total de Contratos Activos</strong></td><td>${total} contratos</td></tr>`);
            body.append(`<tr><td class="text-muted" colspan="2">Para ver el desglose completo y archivos de los contratos, dirígete al menú <strong>Ventas -> Contratos</strong>.</td></tr>`);
        } 
        else if (metric === 'ingresosMensuales') {
            $('#modalDetalleTitulo').html('<i class="fas fa-dollar-sign text-success mr-2"></i> Ingresos del Mes Actual');
            header.html('<tr><th>Métricas de Facturación</th><th>Monto</th></tr>');
            const ingresos = window.dashboardData ? window.dashboardData.ingresosMensuales : '$0.00';
            body.append(`<tr><td><strong>Valor Contratado Registrado en el Mes</strong></td><td class="font-weight-bold text-success">${ingresos}</td></tr>`);
            body.append(`<tr><td class="text-muted" colspan="2">Representa la suma de los valores totales de los planes de pago creados/registrados durante el mes actual. Para auditar los pagos recibidos y cuotas, navega a <strong>Financiero -> Plan de Pago</strong>.</td></tr>`);
        } 
        else if (metric === 'nuevosClientes') {
            $('#modalDetalleTitulo').html('<i class="fas fa-user-plus text-info mr-2"></i> Nuevos Clientes Registrados');
            header.html('<tr><th>Indicador</th><th>Valor</th></tr>');
            const nuevos = window.dashboardData ? window.dashboardData.nuevosClientes : 0;
            body.append(`<tr><td><strong>Clientes Incorporados (Últimos 30 días)</strong></td><td>${nuevos} clientes</td></tr>`);
            body.append(`<tr><td class="text-muted" colspan="2">Mide el total de clientes creados cuyo estado es activo dentro del último mes. Para ver y gestionar el listado completo, ingresa a <strong>Administración -> Clientes</strong>.</td></tr>`);
        }
    });

    // Clic en botones de ampliación de gráficos
    $(document).on('click', '.btn-ampliar-grafico', function() {
        const chartType = $(this).data('chart');
        const header = $('#tbDetalleHead');
        const body = $('#tbDetalleBody');
        
        header.empty();
        body.empty();
        $('#modalDetalleDashboard').modal('show');
        
        if (!window.dashboardData) {
            body.html('<tr><td class="text-center text-muted">No hay datos disponibles en memoria.</td></tr>');
            return;
        }

        if (chartType === 'visitasPorEtapa') {
            $('#modalDetalleTitulo').html('<i class="fas fa-filter text-primary mr-2"></i> Visitas por Etapa Comercial');
            header.html('<tr><th>Etapa Comercial</th><th class="text-center">Total Visitas</th></tr>');
            const data = window.dashboardData.visitasPorEtapa;
            if (Object.keys(data).length > 0) {
                Object.keys(data).forEach(key => {
                    body.append(`<tr><td><strong>${key}</strong></td><td class="text-center">${data[key]}</td></tr>`);
                });
            } else {
                body.html('<tr><td colspan="2" class="text-center text-muted">Sin datos.</td></tr>');
            }
        }
        else if (chartType === 'marcasMasVendidas') {
            $('#modalDetalleTitulo').html('<i class="fas fa-tags text-success mr-2"></i> Cantidad de Equipos por Marca');
            header.html('<tr><th>Marca / Fabricante</th><th class="text-center">Equipos Registrados</th></tr>');
            const list = window.dashboardData.listaMarcasMasVendidasVM;
            if (list && list.length > 0) {
                list.forEach(item => {
                    body.append(`<tr><td><strong>${item.marca}</strong></td><td class="text-center font-weight-bold text-success">${item.totalCantidad}</td></tr>`);
                });
            } else {
                body.html('<tr><td colspan="2" class="text-center text-muted">Sin datos.</td></tr>');
            }
        }
        else if (chartType === 'visitasUltimaSemana') {
            $('#modalDetalleTitulo').html('<i class="fas fa-calendar-day text-info mr-2"></i> Actividad de Visitas (Últimos Días)');
            header.html('<tr><th>Fecha de Registro</th><th class="text-center">Visitas Completadas</th></tr>');
            const list = window.dashboardData.listaVisitasUktimaSemanaVM;
            if (list && list.length > 0) {
                list.forEach(item => {
                    body.append(`<tr><td><strong>${item.fecha}</strong></td><td class="text-center">${item.total}</td></tr>`);
                });
            } else {
                body.html('<tr><td colspan="2" class="text-center text-muted">Sin datos.</td></tr>');
            }
        }
        else if (chartType === 'contratosPorMes') {
            $('#modalDetalleTitulo').html('<i class="fas fa-file-invoice-dollar text-warning mr-2"></i> Historial de Contratos por Mes');
            header.html('<tr><th>Mes / Periodo</th><th class="text-center">Contratos Firmados</th></tr>');
            const data = window.dashboardData.contratosPorMes;
            if (Object.keys(data).length > 0) {
                Object.keys(data).forEach(key => {
                    body.append(`<tr><td><strong>${key}</strong></td><td class="text-center font-weight-bold text-primary">${data[key]}</td></tr>`);
                });
            } else {
                body.html('<tr><td colspan="2" class="text-center text-muted">Sin datos.</td></tr>');
            }
        }
        else if (chartType === 'topClientes') {
            $('#modalDetalleTitulo').html('<i class="fas fa-trophy text-warning mr-2"></i> Ranking de Clientes con Mayor Volumen');
            header.html('<tr><th>Cliente / Constructora</th><th class="text-center">Contratos Firmados</th></tr>');
            const data = window.dashboardData.topClientesConMasContratos;
            if (Object.keys(data).length > 0) {
                Object.keys(data).forEach(key => {
                    body.append(`<tr><td><strong>${key}</strong></td><td class="text-center font-weight-bold text-warning">${data[key]}</td></tr>`);
                });
            } else {
                body.html('<tr><td colspan="2" class="text-center text-muted">Sin datos.</td></tr>');
            }
        }
    });

    // #endregion

    // #region MCP (Asistente Tecmein) Logic

    let isListening = false;
    let recognition; 

    function appendMessage(sender, message, isHtml = false) {
        // Asegurarse de que el asistente esté visible y desplegado
        $('.mcp-chat-window').addClass('show');
        mcpInitialMsg.remove(); // Quitar mensaje inicial si hay interaccion

        const messageContainer = $('<div class="mcp-message-container">');
        const messageCard = $('<div class="mcp-message-card">');

        if (sender === 'Tú') {
            messageContainer.addClass('user');
            messageCard.html(message);
        } else {
            messageContainer.addClass('assistant');
            if (isHtml) {
                messageCard.html(message);
            } else {
                messageCard.text(message);
            }
        }
        
        messageContainer.append(messageCard);
        mcpResponseArea.append(messageContainer);
        mcpResponseArea.scrollTop(mcpResponseArea[0].scrollHeight);
    }

    async function sendMcpQuery(query) {
        if (!query.trim()) {
            Swal.fire('Atención', 'Por favor, introduce una consulta.', 'warning');
            return;
        }

        appendMessage('Tú', query);
        mcpQueryInput.val('');
        
        const loader = $('<div id="mcpLoading" class="text-center text-muted small my-2"><i class="fas fa-spinner fa-spin mr-1"></i> Pensando...</div>');
        mcpResponseArea.append(loader);
        mcpResponseArea.scrollTop(mcpResponseArea[0].scrollHeight);

        fetch('/api/Mcp/query', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ naturalLanguageQuery: query })
        })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                return response.json().then(errorJson => {
                    return Promise.reject({ status: response.status, data: errorJson });
                });
            }
        })
        .then(responseJson => {
            $('#mcpLoading').remove();
            if (responseJson.estado) {
                appendMessage('Asistente', responseJson.objeto.respuesta, true);
            } else {
                appendMessage('Asistente', `Error: ${responseJson.mensajes || 'No se pudo procesar la consulta.'}`);
            }
        })
        .catch(error => {
            $('#mcpLoading').remove();
            if (error.status === 403) {
                Swal.fire("Acceso Denegado", "No tiene permisos para realizar esta consulta.", "error");
                appendMessage('Asistente', 'Acceso denegado. No tienes permisos para usar esta función.');
            } else {
                console.error('Error al comunicarse con el MCP:', error);
                Swal.fire("Error", "Hubo un error de comunicación con el asistente. Inténtalo de nuevo más tarde.", "error");
                appendMessage('Asistente', 'Hubo un error de comunicación con el asistente.');
            }
        });
    }

    // #endregion MCP Logic

    // #region Speech Recognition Logic

    if ('webkitSpeechRecognition' in window) {
        recognition = new webkitSpeechRecognition();
        recognition.continuous = false; 
        recognition.interimResults = false; 
        recognition.lang = 'es-ES'; 

        recognition.onstart = function () {
            isListening = true;
            mcpVoiceInputButton.addClass('btn-danger').removeClass('btn-primary');
            mcpVoiceInputButton.find('i').removeClass('fa-microphone').addClass('fa-stop-circle');
            mcpVoiceInputButton.prop('title', 'Detener dictado');
            
            $('.mcp-chat-window').addClass('show');
            mcpInitialMsg.remove();
            
            const esc = $('<div id="mcpEscuchando" class="text-center text-muted small my-2"><i class="fas fa-microphone fa-beat mr-1 text-danger"></i> Escuchando tu voz...</div>');
            mcpResponseArea.append(esc);
            mcpResponseArea.scrollTop(mcpResponseArea[0].scrollHeight);
        };

        recognition.onresult = function (event) {
            const transcript = event.results[0][0].transcript;
            mcpQueryInput.val(transcript); 
            $('#mcpEscuchando').remove();
            sendMcpQuery(transcript); 
        };

        recognition.onerror = function (event) {
            console.error('Error de reconocimiento de voz:', event.error);
            $('#mcpEscuchando').remove();
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
            $('#mcpEscuchando').remove();
        };
    } else {
        mcpVoiceInputButton.prop('disabled', true).attr('title', 'Reconocimiento de voz no soportado por tu navegador');
    }

    // #endregion Speech Recognition Logic

    // #region Event Listeners

    mcpSendButton.on('click', function () {
        sendMcpQuery(mcpQueryInput.val());
    });

    mcpQueryInput.on('keypress', function (e) {
        if (e.which == 13) { 
            sendMcpQuery(mcpQueryInput.val());
        }
    });

    mcpVoiceInputButton.on('click', function () {
        if (isListening) {
            recognition.stop();
        }
        else {
            $('.mcp-chat-window').addClass('show');
            mcpQueryInput.val(''); 
            mcpInitialMsg.remove();
            recognition.start();
        }
    });

    // #endregion Event Listeners

});