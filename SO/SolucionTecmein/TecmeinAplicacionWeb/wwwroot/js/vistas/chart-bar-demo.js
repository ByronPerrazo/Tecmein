// Set new default font family and font color to mimic Bootstrap's default styling
Chart.defaults.global.defaultFontFamily = 'Nunito', '-apple-system,system-ui,BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Arial,sans-serif';
Chart.defaults.global.defaultFontColor = '#858796';


// Bar Chart Example
let controlVenta = document.getElementById("charVentas");
let myBarChart = new Chart(controlVenta, {
  type: 'bar',
  data: {
    labels: ["06/07/2024", "07/07/2024", "08/07/2024", "09/07/2024","10/07/2024","11/07/2024", "12/07/2024"],
    datasets: [{
      label: "Cantidad",
      backgroundColor: "#5e93df",
      hoverBackgroundColor: "#2e59A9",
      borderColor: "#4e73df",
      data: [120,100,220,10,150,80,322],
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
