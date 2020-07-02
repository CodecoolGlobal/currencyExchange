var dat = document.getElementById('PassingToJavaScript1').value;
var years = document.getElementById('PassingToJavaScript2').value;
console.log(dat.split("/"));
var chart = document.getElementById("lineChart");
var data = {
    labels: years.split(","),
    datasets: [
        {
            label: "Money value",
            fill: false,
            lineTension: 0.1,
            data: dat.split("/")
        }
    ]
}

var lineChart = new Chart(chart,
    {
        type: 'line',
        data: data
    }
);