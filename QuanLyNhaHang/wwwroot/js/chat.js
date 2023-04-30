"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
    console.log("Connection started");
}).catch(function (err) {
    return console.error(err.toString());
});



////----------------------------------------------------------------------------------------------
connection.on("HienThiCTHD", function (chitiethoadon, ipmac, tongtien, idban) {
    var li = $("<li></li>").text(`says ${ipmac} and ${tongtien} and ${idban}`);
    $("#messagesList").append(li);
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
        tr.append('<td class="idcthdNB">' + item.idcthd + '</td>');
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
    $(".tableHD").load(location.href + " .tableHD>*", function () {
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
    var li = $("<li></li>").text(`says ${id}`);
    $("#messagesList").append(li);
    $("#btnedit").prop("disabled", true);

     //Find the row with matching idcthdNB value and update its status and disable the xacnhan button
    $("td.td-idcthd").filter(function () {
        return $(this).text() === id;
    }).closest("tr").find(".tdxacnhan1").text("Bếp đã xác nhận yêu cầu");
    $("td.td-idcthd").filter(function () {
        return $(this).text() === id;
    }).closest("tr").find(".btn-sua").prop("disabled", true);
    $("#TableTGHoanThanh").load(location.href + " #TableTGHoanThanh>*", function () {
    });
    $("#TableTGBEP").load(location.href + " #TableTGBEP>*", function () {
     });
     
});

$(document).ready(function () {
    $(document).on('click','.xacnhan2', function () {

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
                        <td>901-6206 Cras Av.</td>
                        <td><button class="btn btn-primary">Xác nhận</button></td>
                    </tr>`);
            tbody.append(tr);
        }
    }
    $("#TableTGHoanThanh").load(location.href + " #TableTGHoanThanh>*", function () {
    });
});



$(document).ready(function () {
    $(document).on('click', '.tdtght', function () {

        var id = $(this).find('#btntght').val();

        connection.invoke("SendPhucVu", id).catch(function (err) {
            return console.error(err.toString());
        });
    });
});



