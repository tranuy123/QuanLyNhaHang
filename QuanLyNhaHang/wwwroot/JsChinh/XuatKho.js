$(document).ready(function () {
    formatNumberInput()
});
$('#groupTTChiTiet').on('change', 'select[name = "idHangHoa"]', function () {
    var idHH = $(this).val();
    if (idHH != '') {
        $.ajax({
            url: '/XuatKho/getDonViTinhVaSL', // Đường dẫn đến action xử lý form
            method: 'POST',
            data: {
                idHH: idHH,
            },
            success: function (response) {
                $('#groupTTChiTiet #dVT').val(response.donViTinh);
                $('#groupTTChiTiet #DonGia').val(formatTotal(response.donGia));
                $('#groupTTChiTiet #sLCon').val(formatTotal(getSoLuongCon(idHH, response.soLuong)));
            }
        });
    }
});
$('#groupTTChiTiet').on('input', '#SLHH', function () {
    kiemTraSoLuong();
});
function getSoLuongCon(idHH, soLuong) {
    var $rowsWithSameId = $('#tbodyChiTietPhieuXuat tr[data-idhh="' + idHH + '"]');
    if ($rowsWithSameId.length == 0) {
        return soLuong;
    } else {
        var soluongdacap = 0;
        $rowsWithSameId.each(function (index) {
            soluongdacap += Number($(this).find('td input[name="soLuong"]').val());
        });
        return soLuong - Number(soluongdacap);
    }
}
function kiemTraSoLuong() {
    var sLCon = parseInt($('#groupTTChiTiet #sLCon').val().replace(/,/g, ''));
    var sl = parseInt($('#groupTTChiTiet #SLHH').val().replace(/,/g, ''));
    if (sl > sLCon) {
        alert("Số lượng vượt quá số lượng tồn");
        $('#groupTTChiTiet #SLHH').val("");
        return;
    } else {
        tinhThanhTien() 
    }
}
function tinhThanhTien() {
    var soLuong = parseInt($('#groupTTChiTiet #SLHH').val().replace(/,/g, ''));
    var donGia = parseInt($('#groupTTChiTiet #DonGia').val().replace(/,/g, ''));
    if (!isNaN(soLuong) && !isNaN(donGia)) { // Kiểm tra xem giá trị soLuong và donGia có phải là số hợp lệ
        $('#groupTTChiTiet #ThanhTien').val(formatTotal(soLuong * donGia));
    }
}
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
        var thanhTien = parseInt($(this).find('input[name="thanhTien"]').val().replace(/,/g, ''));
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
        showToast("Vui lòng thêm thông tin phiếu nhập", 500);
        return;
    }
    rows.each(function () {
        var row = $(this);
        var rowData = {};
        rowData.Idhh = row.find('input[name="idHangHoa"]').val();
        rowData.ThucXuat = row.find('input[name="soLuong"]').val();
        rowData.Gia = row.find('input[name="donGia"]').val();
        tableData.push(rowData);
    });
    //var idNCC = $('select[name="nhaCungCap"]').val();
    //if (idNCC == '') {
    //    showToast("Vui lòng chọn nhà cung cấp", 500);
    //    return;
    //}
    var ngayNhap = $('input[name="ngayNhap"]').val();
    var ghiChu = $('textarea[name="ghiChu"]').val();
    dataPhieuNhapMaster.append("GhiChu", ghiChu);
    dataPhieuNhapMaster.append("NgayNhap", ngayNhap);
    //dataPhieuNhapMaster.append("Idncc", idNCC);
    var data = {
        PhieuXuat: queryStringToData(dataPhieuNhapMaster),
        ChiTietPhieuXuat: tableData,
        TuNgay: null,
        DenNgay: null,
    };
    $.ajax({
        url: '/XuatKho/ThemPhieuXuatHangHoa', // Đường dẫn đến action xử lý form
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
function offTab1() {
    $('#borderedTabJustifiedContent').removeClass('active').addClass('hide');
    $('#tabs-dsPhieu').addClass('active').addClass('show');

}
function offTab2() {
    $('#borderedTabJustifiedContent').addClass('active').addClass('show');
    $('#tabs-dsPhieu').removeClass('active').addClass('hide');

}
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
            TieuHuy: $('#tieuHuyLS').prop('checked'),
        },
        success: function (response) {
            $('#tbody-XemPhieuXuat').empty();
            $('#TienThanhToanTabLS').val('');
            response.forEach(function (data, i) {
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
                                <td colspan="2">${data.soPx}</td>
                                <td>${data.soLuongHH} </td>
                                <td colspan="2"> <input readonly autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value=${data.tongTien} id="tongTienXuat" name="tongTienXuat"/></td>
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
                            <tr id="collap-2474${i}" class="in p-3 bg-blue-lt collapse fw-bold table-primary">
                            <td class="text-center p-1">#</td>
                            <td class="text-center p-1">Mã HH</td>
                            <td class="text-center p-1 col-3">Tên HH</td>
                            <td class="text-center p-1">ĐVT</td>
                            <td class="text-center p-1">Số lượng xuất</td>
                            <td class="text-center p-1">Giá</td>
                            <td class="text-center p-1">Thành tiền</td>
                            <td colspan="2" class="text-center p-1">Chênh lệch</td>
                                </tr>
                            `;
    data.chiTietPhieuXuat.forEach(function (data, index) {
        var j = index + 1;
        newRow += `
        <tr id="collap-2474${i}" class="in p-3 bg-light collapse table-secondary" style="">
                                        <td class="col-sm-1">${j}</td>
                                        <td class="col-sm-1">${data.idhhNavigation.maHh}</td>
                                        <td class="col-sm-1">${data.idhhNavigation.tenHh}</td>
                                        <td class="col-sm-1">${data.dvt}</td>
                                        <td class="col-sm-1">${data.thucXuat}</td>
                                        <td class="col-sm-2"><input readonly autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value=${data.gia}/></td>
                                        <td class="col-sm-2"><input readonly autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" value=${data.thucXuat * data.gia}/></td>
                                        <td class="col-sm-2 text-truncate">${data.chenhLech}</td>
     </tr>
                                `;
    });
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