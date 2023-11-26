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
        $(this).inputmask({
            alias: "numeric",
            groupSeparator: ",",
            autoGroup: true,
            //autoDigits : true,
            digits: 4, // Số chữ số sau dấu thập phân
            allowMinus: true,
            radixPoint: ".", // Dấu thập phân
            rightAlign: false, // Căn giữa giá trị trong ô input
            // Định dạng đặc biệt nếu giá trị là 0
            onBeforeMask: function (value, opts) {
                if (value === "0") {
                    return "0\\";
                }
                return value;
            },
        });
        $(this).css("text-align", "right");
    });
}


function showToast(message, statusCode) {
    var backgrounColor;
    document.getElementById('toast').className = 'toast fade show';
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
function getMACAddress() {
    // Sử dụng chrome.runtime.getBackgroundPage để truy cập background page
    chrome.runtime.getBackgroundPage(function (backgroundPage) {
        // Lấy plugin Network Information
        const networkInformation = backgroundPage.chrome.webNavigation.getNavigation().getNetworkInformation();

        // Lấy địa chỉ MAC
        const macAddress = networkInformation.macAddress;

        // Hiển thị địa chỉ MAC
        console.log(macAddress);
    });
}

// Gọi hàm để lấy địa chỉ MAC
