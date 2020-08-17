let dat = document.getElementById('DiagramPoints').value;
let years = document.getElementById('Years').value;

let chart = document.getElementById("lineChart");
let diagramData = {
    labels: years.split(","),
    datasets: [
        {
            label: "Money value",
            fill: false,
            lineTension: 0.1,
            backgroundColor: 'rgb(54, 162, 235)',
            data: dat.split("/")
        }
    ]
}

let lineChart = new Chart(chart,
    {
        type: 'line',
        data: diagramData,
    }
);