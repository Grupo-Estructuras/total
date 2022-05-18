
const ctx = document.getElementById('myChart').getContext('2d');
const myChart = new Chart(ctx, {
  type: 'bar',
  data: {
    labels: abcisa, //para la abcisa
    datasets: [{
      label: 'Apariciones', //titulo
      data: ordenada, //para la ordenada
      backgroundColor: ['rgba(255, 99, 132, 0.2)'], //colores de graficos
      borderColor: ['rgba(255, 99, 132, 1)'], //colores del borde
      borderWidth: 1
    }]
  },
  options: {scales: {y: {beginAtZero: true}}}
});//chart.js
