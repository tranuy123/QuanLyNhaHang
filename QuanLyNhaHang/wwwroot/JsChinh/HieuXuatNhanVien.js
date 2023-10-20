var _myChart = null; // Biến để lưu đối tượng biểu đồ
var _labels = [];
var _values = [];
var _tuNgay = null;
var _denNgay = null;
$(document).ready(function () {
    // Gọi API để lấy dữ liệu doanh thu theo ngày
    HieuSuatNhanVien();
});
function HieuSuatNhanVien() {
    $.ajax({
        url: '/BaoCao/getDuLieuHieuSuatNhanVien',
        method: 'POST',
        data: "fromDay=" + $('#fromDay').val() + "&toDay=" + $('#toDay').val(),
        success: function (data) {
            console.log(data);
            _labels = [];
            _values = [];
            // Chuyển dữ liệu từ JSON sang mảng để cấu hình đồ thị
            data.forEach(function (item) {
                _labels.push(item.ten); // Định dạng ngày tháng
                _values.push(item.diem);
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
        // Dữ liệu mẫu
function renderDoThi(labels, values) {
    // Màu sắc tương ứng cho từng nhân viên
    var employeeColors = [
        'rgba(75, 192, 192, 0.2)',
        'rgba(255, 99, 132, 0.2)',
        'rgba(54, 162, 235, 0.2)',
        'rgba(255, 206, 86, 0.2)',
        // Thêm màu cho các nhân viên khác
    ];

    var ctx = document.getElementById('employeePerformanceChart').getContext('2d');
    var employeePerformanceChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Điểm',
                data: values,
                backgroundColor: employeeColors,
                borderColor: employeeColors,
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Điểm'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Nhân viên'
                    }
                }
            }
        }
    });
}

   
