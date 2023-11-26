var _tuNgay = null;
var _denNgay = null;
$(document).ready(function () {
    formatNumberInput()
});
function addChiTietPhieuXuat() {
    var tenHangHoa = $('#groupTTChiTiet select[name="idHangHoa"]').text();
    var idHangHoa = $('#groupTTChiTiet select[name="idHangHoa"]').val();
    var donViTinh = $('#groupTTChiTiet #dVT').val();;
    var soLuong = $('#groupTTChiTiet #SLHH').val();
    var donGia = $('#groupTTChiTiet #DonGia').val();
    var thanhTien = $('#groupTTChiTiet #ThanhTien').val();
    var soLuongTon = $('#groupTTChiTiet #sLCon').val();
    if (idHangHoa == '') {
        showToast("Vui lòng chọn hàng hóa", 500);
        return;
    }
    if (soLuong == '') {
        showToast("Vui lòng chọn số lượng", 500);
        return;
    }
    if (donGia == '') {
        showToast("Vui lòng chọn đơn giá", 500);
        return;
    }
    var newRow = $(`<tr data-idhh = "${idHangHoa}">
        <td class="first-td-column text-center p-1 td-sticky">
            <input autocomplete="off" type="text" class="form-control form-table text-center stt" readonly value="${GanSTT()}" style="width:40px;z-index:2;" />
            <input type="hidden" name="idHangHoa" value="${idHangHoa}" />
        </td>
        <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${tenHangHoa}
        </td>
        <td class="p-1">
            <input autocomplete="off" readonly type="text" class="w-100 form-control form-table input-date-short-mask" style="width:90px;" value="${donViTinh}" id="dVT" name="dVT" />
        </td>
        <td class="p-1">
            <input autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value="${soLuong}" name="soLuong" />
            <input autocomplete="off" type="hidden" class="w-100 form-control form-table formatted-number" style="width:55px;" value="${soLuongTon}" name="soLuongTon" />

        </td>
        <td class="p-1">
            <input autocomplete="off" t type="text" class="w-100 form-control form-table formatted-number" style="width:80px;" value="${donGia}" name="donGia" />
        </td>
        <td class="p-1">
            <input autocomplete="off" type="text" readonly class="w-100 form-control form-table formatted-number" style="width:100px;" value="${thanhTien}" name="thanhTien" />
        </td>
        <td class="text-center p-1 last-td-column">
            <button type="button" class="btn btn-icon btn-sm text-red remove-phieuXuatCt">
                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-x" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                    <path stroke="none" d="M0 0h24v24H0z" fill="none"></path>
                    <path d="M18 6l-12 12"></path>
                    <path d="M6 6l12 12"></path>
                </svg>
            </button>
        </td>
    </tr>
`)
    $('#tbodyChiTietPhieuXuat').append(newRow);
    TinhTongTienPhieuXuat();
    clearFormThemChiTietPhieuNhap();
}
function GanSTT() {
    var stt = $('#tbodyChiTietPhieuXuat tr').length;

    return Number(Number(stt) + 1);
}
function clearFormThemChiTietPhieuNhap() {
    $('#groupTTChiTiet #SLHH').val('');
    $('#groupTTChiTiet #DonGia').val('');
    $('#groupTTChiTiet #ThanhTien').val('');
    $('#groupTTChiTiet #dVT').val('');
    $('#groupTTChiTiet #sLCon').val('');
    $('#groupTTChiTiet select[name="idHangHoa"]')[0].selectize.clear();

}
function TinhTongTienPhieuXuat() {
    var tongTien = 0;
    $('#tbodyChiTietPhieuXuat tr').each(function () {
        var thanhTien = parseFloat($(this).find('input[name="thanhTien"]').val().replace(/,/g, ''));
        if (!isNaN(thanhTien)) {
            tongTien += thanhTien;
        }

    });
    $('#TienThanhToan').val(formatTotal(tongTien));
}
$('#tbodyChiTietPhieuXuat').on("click", ".remove-phieuXuatCt", function (event) {
    event.preventDefault();
    var $row = $(this).closest('tr');
    $row.remove();
    // Cập nhật số thứ tự trên các hàng cùng idToaThuoc
    var $rowsWithSameId = $('#tbodyChiTietPhieuXuat tr');
    $rowsWithSameId.each(function (index) {
        $(this).find('td input.stt').val(index + 1);
    });
    TinhTongTienPhieuXuat();
});
function themPhieuXuat() {
    var tableData = [];
    var dataPhieuNhapMaster = new FormData();
    var rows = $('#tbodyChiTietPhieuXuat tr');
    if (rows.length == 0) {
        showToast("Vui lòng thêm thông tin phiếu xuất", 500);
        return;
    }
    rows.each(function () {
        var row = $(this);
        var rowData = {};
        rowData.Idhh = row.find('input[name="idHangHoa"]').val();
        rowData.SoLuong = row.find('input[name="soLuong"]').val();
        rowData.Gia = row.find('input[name="donGia"]').val();
        rowData.ChenhLech = row.find('input[name="chenhLech"]').val();
        rowData.ThucXuat = row.find('input[name="thucXuat"]').val();

        tableData.push(rowData);
    });
    //var idNCC = $('select[name="nhaCungCap"]').val();
    //if (idNCC == '') {
    //    showToast("Vui lòng chọn nhà cung cấp", 500);
    //    return;
    //}
    var ngayNhap = $('input[name="ngayNhap"]').val();
    var ghiChu = $('input[name="ghiChu"]').val();
    dataPhieuNhapMaster.append("GhiChu", ghiChu);
    dataPhieuNhapMaster.append("NgayNhap", ngayNhap);
    //dataPhieuNhapMaster.append("Idncc", idNCC);
    var data = {
        PhieuXuat: queryStringToData(dataPhieuNhapMaster),
        ChiTietPhieuXuat: tableData,
        TuNgay: _tuNgay,
        DenNgay: _denNgay
    };
    $.ajax({
        url: '/XuatKho/ThemPhieuXuat', // Đường dẫn đến action xử lý form
        method: 'POST',
        data: JSON.stringify(data),
        contentType: "application/json",
        success: function (response) {
            if (response.statusCode == 200) {
                $('#tbodyChiTietPhieuXuat').empty();
                $('#TienThanhToan').val('');

            }
            showToast(response.message, response.statusCode);
        }
    });
}

function getDSNguyenLieu() {
     _tuNgay = $('#tuNgay').val();
     _denNgay = $('#denNgay').val();

    $.ajax({
        url: '/XuatKho/getDSXuatKho', // Đường dẫn đến action xử lý form
        method: 'POST',
        data: {
            TuNgay: _tuNgay,
            denNgay: _denNgay,
        },
        success: function (response) {
            $('#tbodyChiTietPhieuXuat').empty();
            $('#TienThanhToan').val('');
            response.forEach(function (data) {
                processDataNguyenLieu(data);
            });
            TinhTongTienPhieuXuat();
            //if (response.statusCode == 200) {
            //    $('#tbodyChiTietPhieuXuat').empty();
            //    $('#TienThanhToan').val('');

            //}
            //showToast(response.message, response.statusCode);
        }
    });
}
function processDataNguyenLieu(data) {
    var newRow = $(`<tr data-idhh = "${data.idhh}">
        <td class="first-td-column text-center p-1 td-sticky">
            <input autocomplete="off" type="text" class="form-control form-table text-center stt" readonly value="${GanSTT()}" style="width:40px;z-index:2;" />
            <input type="hidden" name="idHangHoa" value="${data.idhh}" />
        </td>
        <td class="p-1 td-sticky" style="position: sticky;left: 33px;background-color: #fff !important; z-index:2">
        ${data.tenHangHoa}
        </td>
        <td class="p-1">
            <input autocomplete="off" readonly type="text" class="w-100 form-control form-table input-date-short-mask" style="width:90px;" value="${data.donViTinh}" id="dVT" name="dVT" />
        </td>
          <td class="p-1">
            <input autocomplete="off" t type="text" class="w-100 form-control form-table formatted-number" style="width:80px;" value="${data.donGia}" name="donGia" />
        </td>
        <td class="p-1">
            <input autocomplete="off" t type="text" class="w-100 form-control form-table formatted-number" style="width:80px;" value="${data.tonKho}" name="tonKho" />
        </td>
        <td class="p-1">
            <input autocomplete="off" type="text" readonly class="w-100 form-control form-table formatted-number" style="width:55px;" value="${data.soLuong}" name="soLuong" />

        </td>
                <td class="p-1">
            <input autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value="${data.soLuong}" name="thucXuat" />

        </td>
        <td class="p-1">
            <input autocomplete="off" type="text" readonly class="w-100 form-control form-table formatted-number" style="width:100px;" value="${data.soLuong * data.donGia}" name="thanhTien" />
        </td>
                <td class="p-1">
            <input autocomplete="off" type="text" readonly class="w-100 form-control form-table formatted-number" style="width:100px;" value="0" name="chenhLech" />
        </td>
    </tr>`)
    if (data.soLuong > data.tonKho) {
        newRow.find('input[name="thucXuat"]').addClass('text-danger');
    }
    $('#tbodyChiTietPhieuXuat').append(newRow);
    formatNumberInput();
}
$('#tbodyChiTietPhieuXuat').on('input', 'input[name="thucXuat"]', function () {
    var tr = $(this).closest('tr');
    var tonKho = parseFloat(tr.find('input[name="tonKho"]').val().replace(/,/g, ''));
    var soLuong = parseFloat(tr.find('input[name="soLuong"]').val().replace(/,/g, ''));
    var thucXuat = parseFloat(tr.find('input[name="thucXuat"]').val().replace(/,/g, ''));
    var donGia = parseFloat(tr.find('input[name ="donGia"]').val().replace(/,/g, ''));
    if (!isNaN(thucXuat) && !isNaN(donGia)) { // Kiểm tra xem giá trị soLuong và donGia có phải là số hợp lệ
        if (thucXuat > tonKho) {
            tr.find('input[name="thucXuat"]').addClass('text-danger');
        } else {
            tr.find('input[name="thucXuat"]').removeClass('text-danger');
        }
        tr.find('input[name="thanhTien"]').val(formatTotal(thucXuat * donGia));
        tr.find('input[name="chenhLech"]').val(thucXuat - soLuong);
        TinhTongTienPhieuXuat();
    }
});
function offTab1() {
    $('#tabXemPhieu').removeClass('active').addClass('hide');
    $('#tabs-dsPhieu').addClass('active').addClass('show');

}
function offTab2() {
    $('#tabXemPhieu').addClass('active').addClass('show');
    $('#tabs-dsPhieu').removeClass('active').addClass('hide');

}
var _fromDay = null;
var _toDay = null;
var _soPhieu = null;
function getDSXKNL() {
    //for (var i = 1; i <= 7; i++) {
    //    addRowTableXKNL(i);
    //}
    _fromDay = $('#fromDay').val();
    _toDay = $('#toDay').val();
    _soPhieu = $('#soPhieuLS').val();

    $.ajax({
        url: '/XuatKho/LichSuXuat', // Đường dẫn đến action xử lý form
        method: 'POST',
        data: {
            TuNgay: _fromDay,
            DenNgay: _toDay,
            maPhieu: _soPhieu,
        },
        success: function (response) {
            $('#tbody-XemPhieuXuat').empty();
            $('#TienThanhToanTabLS').val('');
            response.forEach(function (data,i) {
                addRowTableXKNL(data, i);
            });
            /*TinhTongTienPhieuXuat();*/
            //if (response.statusCode == 200) {
            //    $('#tbodyChiTietPhieuXuat').empty();
            //    $('#TienThanhToan').val('');

            //}
            //showToast(response.message, response.statusCode);
        }
    });
}
function addRowTableXKNL(data, i) {
    var newRow = ` <tr class="accordion-toggle collapsed" id="c-2474${i}" data-toggle="collapse" data-parent="#c-2474${i}" href="#collap-2474${i}" aria-expanded="false">
                                <td>${formatDay(data.ngayTao)}</td>
                                <td>${data.soPx}</td>
                                <td>${data.soLuongHH} </td>
                                <td> <input readonly autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value=${data.tongTien} id="tongTienXuat" name="tongTienXuat"/></td>
                                <td>${data.tongChenhLech}</td>
                                <td>
                                    <button class="btn btn-sm dropdown-toggle more-horizontal" type="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        <span class="text-muted sr-only">Thao Tác</span>
                                    </button>
                                    <div class="dropdown-menu dropdown-menu-right">
                                        <a class="dropdown-item" href="#">In</a>
                                    </div>
                                </td>
                            </tr>
                            <tr id="collap-2474${i}" class="in p-3 bg-light collapse" style="">
                                <td colspan="8">
                            `;
    data.chiTietPhieuXuat.forEach(function (data, index) {
        var i = index + 1;
        newRow += `<dl class="row mb-0 mt-1">
                                        <dt class="col-sm-1">${i}</dt>
                                        <dd class="col-sm-1">${data.idhhNavigation.maHh}</dd>
                                        <dt class="col-sm-1">${data.idhhNavigation.tenHh}</dt>
                                        <dt class="col-sm-1">${data.dvt}</dt>
                                        <dd class="col-sm-1">${data.thucXuat}</dd>
                                        <dt class="col-sm-2"><input readonly autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value=${data.gia}/></dt>
                                        <dd class="col-sm-2"><input readonly autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value=${data.thucXuat * data.gia}/></dd>
                                        <dt class="col-sm-2 text-truncate">${data.chenhLech}</dt>
                                    </dl>
                                `;
    });
    newRow += `
    </td>
    </tr>`;
    $('#tbody-XemPhieuXuat').append(newRow);
    formatNumberInput();
    TinhTongTienPhieuXuatTabXem();

}
function TinhTongTienPhieuXuatTabXem() {
    var tongTien = 0;
    $('#tbody-XemPhieuXuat tr').each(function () {
        var check = $(this).find('input[name="tongTienXuat"]').val();
        if (check != undefined) {
            var thanhTien = parseFloat($(this).find('input[name="tongTienXuat"]').val().replace(/,/g, ''));
            if (!isNaN(thanhTien)) {
                tongTien += thanhTien;
            }
        }


    });
    $('#TienThanhToanTabLS').val(formatTotal(tongTien));
}