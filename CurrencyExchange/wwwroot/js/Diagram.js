var dat = document.getElementById('DiagramPoints').value;
var years = document.getElementById('Years').value;

var chart = document.getElementById("lineChart");
var data = {
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

var lineChart = new Chart(chart,
    {
        type: 'line',
        data: data,
    }
);