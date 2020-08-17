// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function LoadData() {
    let bc = document.getElementById("baseC");
    let ec = document.getElementById("endC");
    var conv = { BaseCurrency: bc, EndCurrency: ec};

    $("#ConvertResolution tr").remove();
    $.ajax({
        type: 'GET',
        url: '@Url.Action("ExchangeRate")',
        dataType: 'json',
        data: { conversion: conv },
        success: function (data) {
            var item = '';
            var row = "<tr>"
                    + "<td class='prtoducttd'>" + item.name + "</td>"
                    + "</tr>";
            $('#ConvertResolution').append(row);
        },
        error: function (ex) {
            var r = jQuery.parseJSON(response.responseText);
            alert("Message: " + r.Message);
            alert("StackTrace: " + r.StackTrace);
            alert("ExceptionType: " + r.ExceptionType);
        }
    });
    return false;
}  
