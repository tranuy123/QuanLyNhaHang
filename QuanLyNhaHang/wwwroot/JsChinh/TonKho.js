$(document).ready(function () {
    formatNumberInput()
});
function loadBaoCaoTH() {
    var idNhomHang = $('select[name="nhomHang"]').val();
    var idHangHoa = $('select[name="hangHoa"]').val();
    var tuNgay = $('#tuNgay').val();
    var denNgay = $('#denNgay').val();
    $.ajax({
        url: '/TonKho/BaoCaoTongHop', // Đường dẫn đến action xử lý form
        method: 'POST',
        data: {
            idNhomHang: idNhomHang,
            idHangHoa: idHangHoa,
            tuNgay: tuNgay,
            denNgay: denNgay,
        },
        success: function (response) {
            $('#tBody-BaoCaoTongHop').empty();
            response.forEach(function (data) {
                addRowBaoCaoTongHop(data);
            });
        }
    });
}
function addRowBaoCaoTongHop(data) {
    var newRow = `<tr>
        <td class="first-td-column text-center p-1 td-sticky">
            <input autocomplete="off" type="text" class="form-control form-table text-center stt" readonly value="${GanSTT()}" style="width:40px;z-index:2;" />
            <input type="hidden" name="idHangHoa" value="${data.id}" />
        </td>
        <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.maHang}
        </td>
        <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.tenHang}
        </td>
        <td class="p-1">
            <input autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value="${formatTotal(data.tongSL)}" name="soLuong" />
        </td>
        <td class="p-1">
            <input autocomplete="off" t type="text" class="w-100 form-control form-table formatted-number" style="width:80px;" value="${formatTotal(data.tongTien)}" name="donGia" />
        </td>
    </tr>`;
    $('#tBody-BaoCaoTongHop').append(newRow);
}
function GanSTT() {
    var stt = $('#tBody-BaoCaoTongHop tr').length;

    return Number(Number(stt) + 1);
}
function GanSTTCT() {
    var stt = $('#tBody-BaoCaoTongHop tr').length;

    return Number(Number(stt) + 1);
}
function loadBaoCaoCT() {
    var idNCC = $('select[name="nhaCungCap"]').val();
    var idNhomHang = $('select[name="nhomHangCT"]').val();
    var idHangHoa = $('select[name="hangHoaCT"]').val();
    var tuNgay = $('#tuNgayCT').val();
    var denNgay = $('#denNgayCT').val();
    $.ajax({
        url: '/TonKho/BaoCaoChiTiet', // Đường dẫn đến action xử lý form
        method: 'POST',
        data: {
            idNCC: idNCC,
            idNhomHang: idNhomHang,
            idHangHoa: idHangHoa,
            tuNgay: tuNgay,
            denNgay: denNgay,
        },
        success: function (response) {
            console.log(response);
            $('#tBody-BaoCaoChiTiet').empty();
            response.forEach(function (data) {
                addRowBaoCaoChiTiet(data);
            });
        }
    });
}
function addRowBaoCaoChiTiet(data) {
    var newRow = `<tr>
        <td class="first-td-column text-center p-1 td-sticky">
            <input autocomplete="off" type="text" class="form-control form-table text-center stt" readonly value="${GanSTTCT()}" style="width:40px;z-index:2;" />
            <input type="hidden" name="idHangHoa" value="${data.id}" />
        </td>
                <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.ngayNhap}
        </td>
             <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.nhaCungCap}
        </td>
        <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.maHang}
        </td>
        <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.tenHang}
        </td>
        <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.ngaySX}
        </td>
        <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.hanSD}
        </td>
   
        <td class="p-1">
            <input autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value="${formatTotal(data.soLuongNhap)}" name="soLuong" />
        </td>
        <td class="p-1">
            <input autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value="${formatTotal(data.soLuongXuat)}" name="soLuong" />
        </td>
        <td class="p-1">
            <input autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value="${formatTotal(data.soLuongTon)}" name="soLuong" />
        </td>
            <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.donViTinh}
        </td>
        <td class="p-1">
            <input autocomplete="off" t type="text" class="w-100 form-control form-table formatted-number" style="width:80px;" value="${formatTotal(data.giaNhap)}" name="donGia" />
        </td>
        <td class="p-1">
            <input autocomplete="off" t type="text" class="w-100 form-control form-table formatted-number" style="width:80px;" value="${formatTotal(data.thanhTien)}" name="donGia" />
        </td>
    </tr>`;
    $('#tBody-BaoCaoChiTiet').append(newRow);
}
function offTab1() {
    $('#tabXemPhieu').removeClass('active').addClass('hide');
    $('#tabs-dsPhieu').addClass('active').addClass('show');

}
function offTab2() {
    $('#tabXemPhieu').addClass('active').addClass('show');
    $('#tabs-dsPhieu').removeClass('active').addClass('hide');

}