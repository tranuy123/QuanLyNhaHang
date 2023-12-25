$(document).ready(function () {
    formatNumberInput();
});
$('#tbodyXNTT').on('keyup', 'input[name ="giamGia"]', function () {
    var dl = $(this).closest('dl');
    var eThanhTien = dl.find('input[name="thanhTien"]');
    var idhd = dl.find('input[name="IDHD"]').val();

    var soLuong = dl.find('input[name="soLuong"]').val();
    var donGia = dl.find('input[name="donGia"]').val();
    var soLuong = parseInt(soLuong.replace(/,/g, ''));
    var donGia = parseInt(donGia.replace(/,/g, ''));
    var giamGia = $(this).val();
    var thanhTien = soLuong * donGia;
    var thanhTienMoi = thanhTien - thanhTien * (giamGia / 100);
    eThanhTien.val(formatTotal(thanhTienMoi));
    var dls = dl.closest('tr').find('dl')
    var tongThanhTien = 0;

    // Lặp qua tất cả các dl trong cùng một colspan
    dls.each(function () {
        // Tìm input có thuộc tính name là "thanhTien" trong mỗi dl và thêm giá trị vào tổng
        var thanhTien = parseFloat($(this).find('input[name="thanhTien"]').val().replace(/,/g, '')) || 0;
        tongThanhTien += thanhTien;
    });
    var eTongTien = $('#tbodyXNTT').find('tr#c-2474-' + idhd + '');
    eTongTien.find('td[name="tongTien"]').text(formatTotal(tongThanhTien));

});
//$('#tbodyXNTT').on('click', 'a.IDHDTT', function () {
//    var idCTHD = $(this).closest('tr').find('input#inputIDHDTT').val();
//    console.log(idCTHD);
//    UpdateHH(idCTHD);
//});
//function UpdateHH(idHH) {
//    var dls = $('#tbodyXNTT tr#collap-2474-' + idHH + ' td dl');
//    var tableData = [];
//    dls.each(function () {
//        var row = $(this);
//        var rowData = {};
//        rowData.Idcthd = parseInt(row.find('input[name="IDCTHD"]').val());
//        rowData.TyLeGiam = row.find('input[name="giamGia"]').val();
//        if (rowData.TyLeGiam == '') {
//            rowData.TyLeGiam = 0;
//        } else {
//            rowData.TyLeGiam = parseInt(rowData.TyLeGiam);
//        }        rowData.ThanhTien = parseFloat(row.find('input[name="thanhTien"]').val().replace(/,/g, ''));
//        tableData.push(rowData);
//    })
//    var data = {
//        IdHD: idHH,
//        ChiTietHoaDon: tableData
//    };
//    console.log(data);
//    $.ajax({
//        url: '/ThuNgan/UpdateHH', // Đường dẫn đến action xử lý form
//        method: 'POST',
//        data: JSON.stringify(data),
//        contentType: "application/json",
//        success: function (response) {
//            $("#tbodyXNTT").load(location.href + " #tbodyXNTT>*", function () {
//            });
//            $("#tbodyXNNT").load(location.href + " #tbodyXNNT>*", function () {
//            });
//        }
//    });
//    formatNumberInput();
//}
$('#tbodyXNNT').on('click', 'a.huyHD', function () {
    var idHD = $(this).closest('td').find('#inputIDHDNT').val();
    huyXNTT(idHD);
});
function huyXNTT(idHD) {
    $.ajax({
        url: '/ThuNgan/HuyXNTT', // Đường dẫn đến action xử lý form
        method: 'POST',
        data: {
            idHD: idHD
        },
        success: function (response) {
            showToast(response.message, response.statusCode);
            if (response.statusCode == 200) {
                $("#tbodyXNTT").load(location.href + " #tbodyXNTT>*", function () {
                    formatNumberInput();
                });
                $("#tbodyXNNT").load(location.href + " #tbodyXNNT>*", function () {
                    formatNumberInput();
                });

            }
        }
    })
}