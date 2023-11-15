var _myChart = null;
$(document).ready(function () {
    baoCaoLoiNhuan()
});
function baoCaoLoiNhuan() {
    var tuNgay = $('#tuNgay').val();
    var denNgay = $('#denNgay').val();
    $.ajax({
        url: '/baoCaoLoiNhuan',
        data: {
            TuNgay: tuNgay,
            DenNgay: denNgay,
        },
        method: 'POST',
        success: function (data) {
            console.log(data);
            _labels = [];
            _values = [];
            // Trích xuất dữ liệu từ phản hồi
            var doanhThuData = data.doanhThu;
            var giaVonData = data.giaVon;
            if (_myChart !== null) {
                _myChart.destroy();
            }
            // Tiếp tục xây dựng biểu đồ với dữ liệu này
            buildBarChart(doanhThuData, giaVonData);
        },
        error: function (error) {
            console.log(error);
        }
    });
}
function buildBarChart(doanhThuData, giaVonData) {
    var labels = doanhThuData.map(item => item.label);
    var doanhThuValues = doanhThuData.map(item => item.doanhthu);
    var giaVonValues = giaVonData.map(item => item.doanhthu);
    var loiNhuanValues = doanhThuValues.map((doanhThu, index) => doanhThu - giaVonValues[index]);


    var data = {
        labels: labels,
        datasets: [
            {
                label: 'Doanh Thu',
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1,
                data: doanhThuValues,
            },
            {
                label: 'Giá Vốn',
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgba(255, 99, 132, 1)',
                borderWidth: 1,
                data: giaVonValues,
            },
            {
                label: 'Lợi Nhuận',
                backgroundColor: 'rgba(255, 206, 86, 0.2)',
                borderColor: 'rgba(255, 206, 86, 1)',
                borderWidth: 1,
                data: loiNhuanValues,
            },
        ],
    };


    var options = {
        scales: {
            x: {
                beginAtZero: true,
            },
            y: {
                beginAtZero: true,
            },
        },
    };

    var ctx = document.getElementById('myBarChart').getContext('2d');
    _myChart = new Chart(ctx, {
        type: 'bar',
        data: data,
        options: options,
    });

}
