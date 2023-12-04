var _labelsTD = [];
var _valuesTD = [];
var _labelsK = [];
var _valuesK = [];
var _myChartTD = null;
var _myChartK = null;
$(document).ready(function () {
    baoCaoChenhLech();

});
function baoCaoChenhLech() {
    $.ajax({
        url: '/BaoCao/BaoCaoChenhLech',
        method: 'POST',
        data: "fromDay=" + $('#tuNgay').val() + "&toDay=" + $('#denNgay').val(),
        success: function (data) {
            console.log(data);
            _labelsTD = [];
            _valuesTD = [];
            _labelsK = [];
            _valuesK = [];
            $('#tbodyTD').empty();

            // Chuyển dữ liệu từ JSON sang mảng để cấu hình đồ thị
            data.dataDoThi.forEach(function (item, i) {
                _labelsTD.push(item.label); // Định dạng ngày tháng
                _valuesTD.push(item.soLuong);
            });
            data.dataTable.forEach(function (item, i) {
                console.log(item);
                addRowTableTD(item, i);
            });
            // Xóa biểu đồ cũ trước khi vẽ biểu đồ mới
            if (_myChartTD !== null) {
                _myChartTD.destroy();
            }

            renderDoThiThucDon(_labelsTD, _valuesTD);

        },
        error: function (error) {
            console.log(error);
        }
    });
}
function addRowTableTD(data, i) {
    var stt = i + 1;
    var newRow = ` <tr class="accordion-toggle collapsed" id="c-2474${i}" data-toggle="collapse" data-parent="#c-2474${i}" href="#collap-2474${i}" aria-expanded="false">
                                <td>${stt}</td>
                                <td colspan="2">${data.maTd}</td>
                                <td colspan="2">${data.ten}</td>
                                <td>
                                    <button class="btn btn-sm dropdown-toggle more-horizontal" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        <span class="text-muted sr-only">Thao Tác</span>
                                    </button>
                                    <div class="dropdown-menu dropdown-menu-right">
                                        <a class="dropdown-item" href="/DinhMuc/DinhMuc"> Sửa </a>
                                    </div>
                                </td>
                            </tr>
                            <tr id="collap-2474${i}" class="in p-3 bg-blue-lt collapse fw-bold table-primary">
                            <td class="text-center p-1">#</td>
                            <td class="text-center p-1">Mã HH</td>
                            <td class="text-center p-1 col-3">Tên HH</td>
                            <td class="text-center p-1">ĐVT</td>
                            <td class="text-center p-1">Số lượng</td>
                            <td class="text-center p-1">Chênh lệch</td>

                                </tr>
                            `;
    data.dinhMuc.forEach(function (data, index) {
        var j = index + 1;
        newRow += `
        <tr id="collap-2474${i}" class="in p-3 bg-light collapse table-secondary" style="">
                                        <td class="col-sm-1">${j}</td>
                                        <td class="col-sm-1">${data.maHh}</td>
                                        <td class="col-sm-1">${data.tenHh}</td>
                                        <td class="col-sm-1">${data.dvt}</td>
                                        <td class="col-sm-1">${data.soLuong}</td>
                                        <td class="col-sm-1">${data.chenhLech}</td>

     </tr>
                                `;
    });
    $('#tbodyTD').append(newRow);
    formatNumberInput();
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
}
function renderDoThiThucDon(labels, values) {
    var ctx = document.getElementById('doThiChenhLech').getContext('2d');

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
}
function formatCurrencyVN(value) {
    console.log(value);
    // Sử dụng toLocaleString để định dạng số thành tiền tệ
    return value.toLocaleString('vi-VN', {
        style: 'currency',
        currency: 'VND'
    });
}