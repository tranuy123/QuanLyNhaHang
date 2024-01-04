var _myChart = null; // Biến để lưu đối tượng biểu đồ

$(document).ready(function () {
    // Gọi API để lấy dữ liệu doanh thu theo ngày
    doanhThuTheoNgay();
});
var _labels = [];
var _values = [];
var _tuNgay = null;
var _denNgay = null;
function doanhThuTheoThang() {
    $.ajax({
        url: '/doanhThuTheoThang',
        data: "fromDay=" + $('#fromDay').val() + "&toDay=" + $('#toDay').val(),
        method: 'POST',
        success: function (data) {
            _labels = [];
            _values = [];
            // Chuyển dữ liệu từ JSON sang mảng để cấu hình đồ thị
            data.forEach(function (item) {
                _labels.push(item.label); // Định dạng ngày tháng
                _values.push(item.doanhthu);
            });
            // Xóa biểu đồ cũ trước khi vẽ biểu đồ mới
            if (_myChart !== null) {
                _myChart.destroy();
            }
            renderDoThi(_labels, _values);
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function doanhThuTheoNgay() {
    $.ajax({
        url: '/doanhThuTheoNgay',
        method: 'POST',
        data: "fromDay=" + $('#fromDay').val() + "&toDay=" + $('#toDay').val(),
        success: function (data) {
            console.log(data);
            _labels = [];
            _values = [];
            // Chuyển dữ liệu từ JSON sang mảng để cấu hình đồ thị
            data.forEach(function (item) {
                _labels.push(moment(item.label).format('DD/MM/YYYY')); // Định dạng ngày tháng
                _values.push(item.doanhthu);
            });
            // Xóa biểu đồ cũ trước khi vẽ biểu đồ mới
            if (_myChart !== null) {
                _myChart.destroy();
            }
            renderDoThi(_labels, _values);
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function renderDoThi(labels, values) {
    var ctx = document.getElementById('doanhthu').getContext('2d');

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
    console.log(doanhThuData);
    _myChart = new Chart(ctx, {
        type: 'bar',
        data: doanhThuData,
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value, index, values) {
                            return formatCurrency(value); // Định dạng các giá trị trên trục y thành kiểu tiền tệ
                        }
                    }
                }
            }
        }
    });
}