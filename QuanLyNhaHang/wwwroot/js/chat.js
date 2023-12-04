"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
    console.log("Connection started");
}).catch(function (err) {
    return console.error(err.toString());
});



////----------------------------------------------------------------------------------------------
connection.on("HienThiCTHD", function (chitiethoadon, chitiethoadonHH, ipmac, tongtien, idban,tb) {
    var li = $("<li></li>").text(`says ${ipmac} and ${tongtien} and ${idban}`);
    $("#messagesList").append(li);
    var IPMAC = $("#IDMAC").val();
    var tbody = $("#tbodyNB");
    var tbodyHH = $("#tbodyNBHH");

    // Remove all existing rows from tbody
    tbody.empty();
    tbodyHH.empty();
    // Add new rows to tbody
    $.each(chitiethoadon, function (index, item) {
        var tr = $("<tr>");
        tr.append("<td>" + (index + 1) + "</td>");
        tr.append('<td style="display:none" class="idcthdNB">' + item.idcthd + '</td>');
        tr.append("<td>" + item.ten + "</td>");
        tr.append('<td><strong>' + item.tenBan + '</strong></td>')
        tr.append("<td></td>");
        tr.append("<td>" + item.sl + "</td>");
        tr.append('<td class="xacnhan2"><button id="xacnhan1" class="btn btn-primary" value=' + item.idcthd + '">Xác nhận</button></td > ');

        
        tbody.append(tr);
    });
    $.each(chitiethoadonHH, function (index, item) {

        var tr = $("<tr>");
        tr.append("<td>" + (index + 1) + "</td>");
        tr.append('<td style="display:none" class="idcthdNB">' + item.idcthd + '</td>');
        tr.append("<td>" + item.ten + "</td>");
        tr.append('<td><strong>' + item.tenBan + '</strong></td>')
        tr.append("<td></td>");
        tr.append("<td>" + item.sl + "</td>");
        tr.append('<td class="xacnhan2HH"><button id="xacnhan1" class="btn btn-primary" value=' + item.idcthd + '">Xác nhận</button></td > ');


        tbodyHH.append(tr);
    });
    $("#TableTGBEP").load(location.href + " #TableTGBEP>*", function () {
    });
    $("#TableTGBEPHH").load(location.href + " #TableTGBEPHH>*", function () {
    });
    if (tb == 1) {
        if (ipmac == IPMAC) {
            window.location.href = "/KhachHang/HoaDon";
            $('.toastGM').toast('show');

        }
    } else {
        if (ipmac == IPMAC) {
            window.location.href = "/KhachHang/HoaDon";
            $('.toastGMTB').toast('show');

        }
    }
    $(".DivViewOrderPhucVu").load(location.href + " .DivViewOrderPhucVu>*", function () {
    });
  
    

});


$(document).ready(function () {
    $(document).on('click','.btn-addcthd', function () {
        var tongtien = $('#total').val();
        var ipmac = $('#IDMAC').val();
        var idban = $('input[name="idban"]').val();
        connection.invoke("SaveCTHD", ipmac,tongtien,idban).catch(function (err) {
            console.log(err);

            return console.error(err.toString());
        });
    });
});

//-------------------------------------------------------------------------------------

connection.on("ReceiveMessage", function (id) {
    var li = $("<li></li>").text(`says ${id}`);
    $("#messagesList").append(li);
    $("#btnedit").prop("disabled", true);

    // Find the row with matching idcthdNB value and update its status and disable the xacnhan button
    $("td.idcthdNB").filter(function () {
        return $(this).text() === id;
    }).closest("tr").find(".trangthaicthd").text("Khách đang sửa yêu cầu");
    $("td.idcthdNB").filter(function () {
        return $(this).text() == id;
    }).find("#xacnhan1").prop("disabled", true);
});




$(document).ready(function () {
    $('.btn-sua').click(function () {

        var id = $(this).closest('tr').find('.td-idcthd').text();
        connection.invoke("SendMessage", id).catch(function (err) {
            return console.error(err.toString());
        });
    });
});

//----------------------------------------------------------------------------------




connection.on("ReceiveMessageNB", function (id) {
   

     //Find the row with matching idcthdNB value and update its status and disable the xacnhan button
    $("td.td-idcthd").filter(function () {
        return $(this).text() === id;
    }).closest("tr").find(".tdxacnhan1").text("Bếp đã xác nhận yêu cầu");
    $("td.td-idcthd").filter(function () {
        return $(this).text() === id;
    }).closest("tr").find(".btn-huy").prop("disabled", true);
    $("#TableTGHoanThanh").load(location.href + " #TableTGHoanThanh>*", function () {
    });
    $("#TableTGBEP").load(location.href + " #TableTGBEP>*", function () {
     });
    $("#TableTGHoanThanhHH").load(location.href + " #TableTGHoanThanhHH>*", function () {
    });
    $("#TableTGBEPHH").load(location.href + " #TableTGBEPHH>*", function () {
    });
});

$(document).ready(function () {
    $(document).on('click','.xacnhan2,.xacnhan2HH', function () {

        var id = $(this).find('#xacnhan1').val();
        connection.invoke("SendMessageNB", id).catch(function (err) {
            return console.error(err.toString());
        });
    });
});

//-----------------------------------------------------------------------
connection.on("ReceviPhucVu", function (listCTHDPV) {

    var idkhu = $("#IDKHU").val();

    var tbody = $("#tbodyTGNM");

    tbody.empty(); // Xóa bỏ các hàng hiện tại trong bảng

    for (var i = 0; i < listCTHDPV.length; i++) {
        var item = listCTHDPV[i];

        console.log(item.idcthd)
        if (item.idkhu == idkhu) {
            var tr = $(`<tr>
                        <td>${(i + 1)}</td>
                        <td>${item.tenBan}</td>
                        <td>${item.ten}</td>
                        <td>${item.sl}</td>
                        <td></td>
                        <td><button class="btn btn-primary btnTGPV" value="${item.idcthd}">Xác nhận</button></td>
                    </tr>`);
            tbody.append(tr);
        }
    }
    $("#TableTGHoanThanh").load(location.href + " #TableTGHoanThanh>*", function () {
    });
    $("#TableTGHoanThanhHH").load(location.href + " #TableTGHoanThanhHH>*", function () {
    });
});



$(document).ready(function () {
    $(document).on('click', '.tdtght, .tdtghtHH', function () {

        var id = $(this).find('#btntght').val();

        connection.invoke("SendPhucVu", id).catch(function (err) {
            return console.error(err.toString());
        });
    });
});


//----------------------------------------------
connection.on("HienThiCTHD1", function (chitiethoadon) {
   
    var tbody = $("#tbodyNB");

    // Remove all existing rows from tbody
    tbody.empty();

    // Add new rows to tbody
    $.each(chitiethoadon, function (index, item) {
        console.log(item.idtd);
        console.log(item.idcthd);
        console.log(chitiethoadon)
        var tr = $("<tr>");
        tr.append("<td>" + (index + 1) + "</td>");
        tr.append('<td style="display:none" class="idcthdNB">' + item.idcthd + '</td>');
        tr.append("<td>" + item.ten + "</td>");
        tr.append('<td><strong>' + item.tenBan + '</strong></td>')
        tr.append("<td>" + item.sl + "</td>");
        if (item.trangThaiOrder) {
            tr.append('<td class="trangthaicthd">Khách hàng đang sửa yêu cầu</td>');
        } else {
            tr.append('<td class="trangthaicthd"></td>');
        }
        if (item.trangThaiOrder) {
            tr.append('<td><button class="xacnhan btn btn-primary"  disabled>Xác nhận</button></td>');

        } else {
            tr.append('<td class="xacnhan2"><button id="xacnhan1" class="btn btn-primary" value=' + item.idcthd + '">Xác nhận</button></td > ');

        }
        tbody.append(tr);
    });
    $("#TableTGBEP").load(location.href + " #TableTGBEP>*", function () {
    });
    $("#TableTGBEPHH").load(location.href + " #TableTGBEPHH>*", function () {
    });
    $(".tableHD").load(location.href + " .tableHD>*", function () {
    });
    $('.toastHD').toast('show');


});

$(document).ready(function () {
    $(document).on('click', '.btn-huy', function () {

        var id = $(this).closest('tr').find('.td-idcthd').text();
        connection.invoke("DeleteCTHD", id).catch(function (err) {
            return console.error(err.toString());
        });
    });
});
//------------------------------------------------------------
connection.on("GiveHD", function (item,tt,ipmac) {
    var btnsendYCTT = $('.btn-sendYCTT');
    var IPMAC = $("#IDMAC").val();
    var tbody = $("#tbodyXNTT");
    var newRow = `<tr class="accordion-toggle collapsed" id="c-2474-${item.idhd}" data-toggle="collapse" data-parent="#c-2474-${item.idhd}" href="#collap-2474-${item.idhd}" aria-expanded="false">
    <td>${GanSTTXNTT()}</td>
    <td style="display:none" class=""></td> 
    <td>${item.tenBan}</td>
    <td>${item.tenKhu}</td>
    <td>${item.tenSanh}</td>
    <td name="tongTien">${item.tongTien.toLocaleString('vi-VN', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
    <td class="xacnhanTT">
        <input type="hidden" id="inputIDHDTT" value="${item.idhd}" />
        <a id="IDHDTT" class="btn btn-primary IDHDTT text-white"" value="${item.idhd}">Xác nhận</a>
    </td>
</tr>
<tr id="collap-2474-${item.idhd}" class="in p-3 bg-light collapse" style="">
    <td colspan="8">
        `;

    item.chiTietHoaDon.forEach(function (data, index) {
        var i = index + 1;
        newRow += `<dl class="row mb-0 mt-1">
        <dt class="col-sm-1">${i}
             <input readonly autocomplete="off" type="hidden" class="w-100 form-control form-table" value="${data.idCTHD}" name="IDCTHD" />
             <input readonly autocomplete="off" type="hidden" class="w-100 form-control form-table" value="${item.idhd}" name="IDHD"/>
        </dt>
        <dd class="col-sm-3">${data.ten}</dd>
        <dt class="col-sm-2"><input readonly autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" value="${data.soLuong}" name="soLuong" /></dt>
        <dt class="col-sm-2"><input readonly autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" value="${data.donGia}" name="donGia" /></dt>
        <dd class="col-sm-2"><input min="0" max="100" autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" style="width:55px;" min="0" max="100" name="giamGia" /></dd>
        <dt class="col-sm-2"><input readonly autocomplete="off" type="text" class="w-100 form-control form-table formatted-number" value="${data.thanhTien}" style="width:55px;" name="thanhTien" /></dt>
    </dl>`;
    });

    newRow += `</td></tr>`;

    tbody.append(newRow);
    formatNumberInput();
    if (ipmac == IPMAC) {
        if (tt == 1) {
            $('.toastTT').toast('show');
            btnsendYCTT.prop('disabled', true);
            $('.btn-sendHYCTT').prop('disabled', false);
        } else {
            $('.toastTTTB').toast('show');
        }
    }

});

function GanSTTXNTT() {
    var stt = $('#tbodyXNTT tr.accordion-toggle').length;

    return Number(Number(stt) + 1);
}
$(document).ready(function () {
    $(document).on('click', '.btn-sendYCTT', function () {
        var IDHD = $(this).val();
        var tbody = $(this).closest('tbody');
        var tr = tbody.find('tr.accordion-toggle');
        var trWithDisabledButton = tbody.find('tr.accordion-toggle').filter(function () {
            return $(this).find('td button.btn-huy:disabled').length > 0;
        });
        console.log(trWithDisabledButton.length , tr.length);
        if (trWithDisabledButton.length != tr.length) {
            showToast("Có món ăn chưa được hoàn thành, vui lòng chờ hoặc hủy món ăn để được thanh toán", 500)
            return;
        }

// Bây giờ `trWithDisabledButton` sẽ chứa tất cả các `tr` có button `disabled`.
        connection.invoke("SendHD", IDHD).catch(function (err) {
            console.log(err);

            return console.error(err.toString());
        });
    });
});
//------------------------------------------------------
connection.on("GiveHHD", function (data) {
    console.log(data);
    var trHHD = $('#tbodyXNTT tr#c-2474-' + data.idHD + '');
    trHHD.remove();
    var trHHDC = $('#tbodyXNTT tr#collap-2474-' + data.idHD + '');
    trHHDC.remove();
    var IPMAC = $("#IDMAC").val();
    console.log(data.ipmac);
    console.log(IPMAC);
    if ((data.ipmac).trim() == IPMAC.trim()) {
    $('.btn-sendHYCTT').prop('disabled', true);
    $('.btn-sendYCTT').prop('disabled', false);
        showToast("Thành công", 200);

    }
    
});
$(document).ready(function () {
    $(document).on('click', '.btn-sendHYCTT', function () {
        var IDHD = $(this).val();
        connection.invoke("SendHHD", IDHD).catch(function (err) {
            console.log(err);

            return console.error(err.toString());
        });
    });
});
//------------------------------------------------------
connection.on("NhanXNTT", function (ipmac) {
   
    var IPMAC = $("#IDMAC").val();
    if (IPMAC == ipmac) {
        $('.toastXNTT').toast('show');
        $('.btn-sendHYCTT').prop('disabled', true);
    }
   
    //$("#tbodyXNTT").load(location.href + " #tbodyXNTT>*", function () {
    //});
    //$("#tbodyXNNT").load(location.href + " #tbodyXNNT>*", function () {
    //});

});

$(document).ready(function () {
    $(document).on('click', 'a.IDHDTT', function () {

        var id = $(this).closest('tr').find('input#inputIDHDTT').val();
        console.log(id);
        connection.invoke("SendXNTT", id).catch(function (err) {
            return console.error(err.toString());
        });
    });
});
//--------------------------------------------------------------
connection.on("HuyHDXNNT", function (ipmac) {

    var IPMAC = $("#IDMAC").val();
    if (IPMAC == ipmac) {
        $('.btn-sendHYCTT').prop('disabled', false);
    }

    //$("#tbodyXNTT").load(location.href + " #tbodyXNTT>*", function () {
    //});
    //$("#tbodyXNNT").load(location.href + " #tbodyXNNT>*", function () {
    //});

});

$(document).ready(function () {
    $(document).on('click', 'a.huyHD', function () {

        var id = $(this).closest('tr').find('input#inputIDHDNT').val();
        console.log(id);
        connection.invoke("SendHuyHDXNNT", id).catch(function (err) {
            return console.error(err.toString());
        });
    });
});
//--------------------------------------------------------------
connection.on("NhanXNNT", function (ipmac) {
    var vaitro = $('#VAITRO').val();
    var IPMAC = $("#IDMAC").val();
    if (IPMAC == ipmac && vaitro == 'KHACHHANG') {
        console.log(123);
        $('.toastXNNT').toast('show');
        window.location.href = "/KhachHang/menu1";

    }

  
    $("#tbodyXNNT").load(location.href + " #tbodyXNNT>*", function () {
    });
    $(".DivViewOrderPhucVu").load(location.href + " .DivViewOrderPhucVu>*", function () {
    });

});

$(document).ready(function () {
    $(document).on('click', 'a.IDHDNT', function () {

        var id = $(this).closest('tr').find('input#inputIDHDNT').val();
        console.log(id);
        connection.invoke("SendXNNT", id).catch(function (err) {
            return console.error(err.toString());
        });
    });
});
//------------------------------------------------------------------
connection.on("NhanYeuCauHoTro", function (item) {
    console.log(item);
    var idkhu = $("#idKhuOrderPhucVu").val();
    console.log(idkhu);
    var tbody = $("#tbody-ThongBao");

    tbody.empty(); // Xóa bỏ các hàng hiện tại trong bảng

            var tr = $(`<tr>
                        <td>${item.tenBan}</td>
                        <td></td>
                    </tr>`);
            tbody.append(tr);
        
    function ganstt() {
        var stt = $('#tbody-ThongBao tr').val();
        return Number(stt.length) + 1;
        
    }
});

$(document).ready(function () {
    $(document).on('click', '#hoTro', function () {

        var mac = $('#MACMenu1').val();
        console.log(mac);
        connection.invoke("YeuCauHoTro", mac).catch(function (err) {
            return console.error(err.toString());
        });
    });
});
