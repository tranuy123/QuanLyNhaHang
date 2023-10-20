function formatDay(inputString) {
    if (inputString) {
        var inputDate = new Date(inputString);
        var day = inputDate.getDate();
        if (day < 10) {
            day = '0' + day;
        }
        var month = inputDate.getMonth() + 1;
        if (month < 10) {
            month = '0' + month;
        }
        var year = inputDate.getFullYear();
        return day + '-' + month + '-' + year;
    } else {
        return ""
    }
}
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(amount);
}
function formatTotal(number) {
    return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function formatNumberInput() {
    // Áp dụng inputmask cho các phần tử có lớp 'formatted-number' và giá trị khác 0
    $(".formatted-number").each(function () {
        var value = $(this).val();
        console.log(value);
        $(this).inputmask({
            alias: "numeric",
            groupSeparator: ",",
            autoGroup: true,
            digits: 0,
            allowMinus: false,
            digitsOptional: false,
            // Định dạng đặc biệt nếu giá trị là 0
            onBeforeMask: function (value, opts) {
                if (value === "0") {
                    return "0\\";
                }
                return value;
            },
        });

    });
}

function showToast(message, statusCode) {
    var backgrounColor;
    document.getElementById('toast').className = 'toast align-items-center text-white border-0 position-fixed top-0 end-0 p-3';
    $("#toastContent").text(message);
    if (statusCode === 200) {
        backgrounColor = "bg-success";
        $("#toast").addClass(backgrounColor);
    } else {
        backgrounColor = "bg-danger";
        $("#toast").addClass(backgrounColor);
    }
    $("#toast").show();
    setTimeout(function () {
        $("#toast").hide();
    }, 2000);
}
function queryStringToData(queryString) {
    // Phân tích cú pháp và xử lý chuỗi query string
    var params = new URLSearchParams(queryString);
    var jsonData = {};

    // Lặp qua các cặp key-value trong chuỗi query string
    for (var pair of params.entries()) {
        var key = pair[0];
        var value = pair[1];

        // Gán giá trị vào đối tượng JSON
        jsonData[key] = value;
    }
    return jsonData;
}