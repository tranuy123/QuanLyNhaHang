"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(function () {
    console.log("Connection started");
}).catch(function (err) {
    return console.error(err.toString());
});

//------------------------------------------------------------------
connection.on("NhanYeuCauHoTro", function (item) {
    console.log(item);
    var idkhu = $("#idKhuOrderPhucVu").val();
    console.log(idkhu);
    var tbody = $("#tbody-ThongBao");

    var tr = $(`<tr>
                        <td>${item.tenBan} cần hỗ trợ</td>
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