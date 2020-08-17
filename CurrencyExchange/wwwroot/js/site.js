// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//function LoadData() {
//    let bc = document.getElementById("baseC");
//    let ec = document.getElementById("endC");
//    var conv = { BaseCurrency: bc, EndCurrency: ec};

//    $("#ConvertResolution tr").remove();
//    $.ajax({
//        type: 'POST',
//        url: '@Url.Action("Home/ExchangeRate")',
//        dataType: 'json',
//        data: { conversion: "" },
//        success: function (data) {
//            var item = '';
//            var row = "<tr>"
//                    + "<td class='prtoducttd'>" + item.Message + "</td>"
//                    + "</tr>";
//            $('#ConvertResolution').append(row);
//        },
//        error: function (ex) {
//            var r = jQuery.parseJSON(response.responseText);
//            alert("Message: " + r.Message);
//            alert("StackTrace: " + r.StackTrace);
//            alert("ExceptionType: " + r.ExceptionType);
//        }
//    });
//    return false;
//}

//function getResult() {
//    let bc = document.getElementById("baseC");
//    let ec = document.getElementById("endC");
//    fetch(`https://api.exchangeratesapi.io/latest?base=${bc}&symbols=${ec}`)
//        .then(response => response.json())
//        .then(data => console.log(data));
//}
