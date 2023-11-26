var _labelsTD = [];
var _valuesTD = [];
var _labelsK = [];
var _valuesK = [];
var _myChartTD = null;
var _myChartK = null;
$(document).ready(function () {
    baoCaoChiTieu();

});
function baoCaoChiTieu() {
    $.ajax({
        url: '/BaoCao/BaoCaoChiTieu',
        method: 'POST',
        data: "fromDay=" + $('#tuNgay').val() + "&toDay=" + $('#denNgay').val(),
        success: function (data) {
            console.log(data);
            _labelsTD = [];
            _valuesTD = [];
            _labelsK = [];
            _valuesK = [];
            // Chuyển dữ liệu từ JSON sang mảng để cấu hình đồ thị
            data.doThiThucDon.forEach(function (item,i) {
                _labelsTD.push(item.label); // Định dạng ngày tháng
                _valuesTD.push(item.soLuong);
                addRowTableTD(item,i);
            });
            data.doThiKhu.forEach(function (item,i) {
                _labelsK.push(item.label); // Định dạng ngày tháng
                _valuesK.push(item.soLuong);
                addRowTableK(item, i);
            });
            // Xóa biểu đồ cũ trước khi vẽ biểu đồ mới
            if (_myChartTD !== null) {
                _myChartTD.destroy();
            }
            if (_myChartK !== null) {
                _myChartK.destroy();
            }
            renderDoThiThucDon(_labelsTD, _valuesTD);
            renderDoThiKhu(_labelsK, _valuesK);

        },
        error: function (error) {
            console.log(error);
        }
    });
}
function addRowTableTD(data, i) {
    var stt = i + 1;
    var newRow = $(`<tr>
                <td>${stt}</td>
                <td>${data.label}</td>
                <td><input type="text" readonly class="form-control formatted-number" name="doanhThu" value="${data.soLuong}" /></td>
    </tr>`)
    $('#tbodyTD').append(newRow);
    formatNumberInput();
    TinhTongTD();
}
function addRowTableK(data, i) {
    var stt = i + 1;
    var newRow = $(`<tr>
                <td>${stt}</td>
                <td>${data.label}</td>
                <td><input type="text" readonly class="form-control formatted-number" name="doanhThu" value="${data.soLuong}" /></td>
    </tr>`)
    $('#tbodyK').append(newRow);
    formatNumberInput();
    TinhTongK();
}
function TinhTongTD() {
    var tongTien = 0;
    $('#tbodyTD tr').each(function () {
        var thanhTien = parseInt($(this).find('input[name="doanhThu"]').val().replace(/,/g, ''));
        if (!isNaN(thanhTien)) {
            tongTien += thanhTien;
        }

    });
    $('#tongTD').val(formatTotal(tongTien));
} function TinhTongK() {
    var tongTien = 0;
    $('#tbodyK tr').each(function () {
        var thanhTien = parseInt($(this).find('input[name="doanhThu"]').val().replace(/,/g, ''));
        if (!isNaN(thanhTien)) {
            tongTien += thanhTien;
        }

    });
    $('#tongK').val(formatTotal(tongTien));
}
function renderDoThiThucDon(labels, values) {
    var ctx = document.getElementById('doThiThucDon').getContext('2d');

    var doanhThuData = {
        labels: labels,
        datasets: [{
            label: 'Số Lượng',
            data: values,
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1
        }]
    };
    console.log(doanhThuData);
    _myChartTD = new Chart(ctx, {
        type: 'bar',
        data: doanhThuData,
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value, index, values) {
                            return value; // Định dạng các giá trị trên trục y thành kiểu tiền tệ
                        }
                    }
                }
            }
        }
    });
} function renderDoThiKhu(labels, values) {
    var ctx = document.getElementById('doThiKhu').getContext('2d');

    var doanhThuData = {
        labels: labels,
        datasets: [{
            label: 'Doanh thu',
            data: values,
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1
        }]
    };
    // Khởi tạo biểu đồ
    var _myChartK = new Chart(ctx, {
        type: 'bar',
        data: doanhThuData,
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value, index, values) {
                            return formatCurrencyVN(value); // Sử dụng hàm formatCurrency để định dạng giá trị
                        }
                    }
                }
            }
        }
    });
}
function formatCurrencyVN(value) {
    console.log(value);
    // Sử dụng toLocaleString để định dạng số thành tiền tệ
    return value.toLocaleString('vi-VN', {
        style: 'currency',
        currency: 'VND'
    });
}