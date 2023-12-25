$(document).ready(function () {
    formatNumberInput()
});
var _fromDay = null;
var _toDay = null;
var _soPhieu = null;
$('#tbody-ThucDon').on('change', 'input[name="soLuong"]', function () {
    getDSXKNL();
});
function getDSXKNL() {
    //for (var i = 1; i <= 7; i++) {
    //    addRowTableXKNL(i);
    //}
    _fromDay = $('#tuNgay').val();
    _toDay = $('#denNgay').val();
    var tableData = [];
    var rows = $('#tbody-ThucDon tr');
    rows.each(function () {
        var row = $(this);
        var rowData = {};
        rowData.Idtd = row.data('id');
        rowData.Sl = Number(row.find('input[name="soLuong"]').val().replace(/,/g, ''));
        tableData.push(rowData);
    });
    var data = {
        TuNgay: _fromDay,
        DenNgay: _toDay,
        ChiTietHoaDons: tableData,
    };
    $.ajax({
        url: '/CanhBaoNguyenLieu/getDSCanhBaoNguyenLieu', // Đường dẫn đến action xử lý form
        method: 'POST',
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (response) {
            $('#tbodyCBNL').empty();
            $('#TienThanhToan').val('');
            response.forEach(function (data) {
                addRowCBNL(data);
            });
        }
    });
}
function addRowCBNL(hh) {
    var newRow = $(`<tr data-id="${hh.idhh}">
                                                   
                <td>${hh.tenHangHoa}</td>
                <td>${hh.donViTinh}</td>
                <td><input type="text" readonly class="form-control formatted-number" value="${hh.tonKho}" /></td>
                <td><input type="text" readonly class="form-control formatted-number" name="soLuong" value="${hh.soLuong}" /></td>
                <td><input type="text" readonly class="form-control formatted-number" value="${Math.round(hh.donGia)}" /></td>
                <td><input type="text" readonly class="form-control formatted-number" value="${Math.round(hh.donGia * hh.soLuong)}" name="thanhTien" /></td>
    </tr>`)
    if (hh.soLuong > hh.tonKho) {
        newRow.find('input[name="soLuong"]').addClass('text-danger');
    }
    $('#tbodyCBNL').append(newRow);
    formatNumberInput();
    TinhTongTienPhieuXuat();
}
function TinhTongTienPhieuXuat() {
    var tongTien = 0;
    $('#tbodyCBNL tr').each(function () {
        var thanhTien = parseInt($(this).find('input[name="thanhTien"]').val().replace(/,/g, ''));
        console.log(thanhTien);
        if (!isNaN(thanhTien)) {
            tongTien += thanhTien;
        }

    });
    $('#TienThanhToan').val(formatTotal(tongTien));
}