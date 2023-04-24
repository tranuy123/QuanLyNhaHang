"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


connection.on("ReceiveMessageNB", function (id) {
    var li = $("<li></li>").text(`says ${id}`);
    $("#messagesList").append(li);
    //    $("#btnedit").prop("disabled", true);

    //     //Find the row with matching idcthdNB value and update its status and disable the xacnhan button
    //    $("td.td-idcthd").filter(function () {
    //        return $(this).text() === id;
    //    }).closest("tr").find(".tdxacnhan1").text("Bếp đã xác nhận yêu cầu");
    //    $("td.td-idcthd").filter(function () {
    //        return $(this).text() === id;
    //    }).closest("tr").find(".btn-sua").prop("disabled", true);
    //    //$("#TableTGBEP").load(location.href + " #TableTGBEP>*", function () {
    //    // });
    //    // $("#TableTGHoanThanh").load(location.href + " #TableTGHoanThanh>*", function () {
    //    // });
});

$(document).ready(function () {
    $('.xacnhan').click(function () {

        var id = $(this).closest('tr').find('.idcthdNB').text();
        connection.invoke("SendMessageNB", id).catch(function (err) {
            return console.error(err.toString());
        });
    });
});

