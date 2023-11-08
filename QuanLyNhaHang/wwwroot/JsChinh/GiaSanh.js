$(document).ready(function () {
    //formatNumberInput();
   
});
function changeHH() {
        var active = $('#active').val();
        var ids = $("#IDS").val();

        $.ajax({
            type: "post",
            url: "/loadViewGia",
            data: "active=" + active + "&ids=" + ids,
            success: function (result) {
                $('.table-responsive').replaceWith(result);
            },
            error: function () {
                alert("Fail to delete");
            }
        });
}
$('table').on('click', '.btnthem', function () {
    var $cell = $(this).closest('tr').find('.giacell');
    var idsanh = $cell.data('idsanh');
    var idtd = $cell.data('idtd');
    var $btnLuu = $(this).closest('tr').find('.btnLuu');
    var $btnHuy = $(this).closest('tr').find('.btnHuy1');
    var $btnxoa = $(this).closest('tr').find('.btnxoa');
    $cell.html('<input type="number" class="form-control formatted-number" value="" />');
    $btnLuu.removeClass("d-none");
    $btnHuy.removeClass("d-none");
    $btnxoa.addClass("d-none");

    $(this).addClass("d-none");
});
$('table').on('click', '.btnsua', function () {
    var $cell = $(this).closest('tr').find('.giacell');
    var value = $cell.text();
    console.log(value);
    var $btnLuu = $(this).closest('tr').find('.btnLuu');
    var $btnHuy = $(this).closest('tr').find('.btnHuy');
    var $btnxoa = $(this).closest('tr').find('.btnxoa');

    var initial_value = value;

    $cell.html('<input type="number" class="form-control formatted-number" value="' + value + '" />');
    $btnLuu.removeClass("d-none");
    $btnHuy.removeClass("d-none");
    $btnxoa.addClass("d-none");

    $(this).addClass("d-none");
    $cell.find('input').attr('data-initial-value', initial_value);

});
$('table').on('click', '.btnHuy', function () {
    var $cell = $(this).closest('tr').find('.giacell');
    // Đặt lại giá trị của ô input bằng giá trị ban đầu lưu trong data-initial-value
    var initial_value = $cell.find('input').attr('data-initial-value');
    $cell.html(initial_value);
    $(".btnsua").removeClass("d-none");
    $(".btnxoa").removeClass("d-none");

    $(".btnLuu").addClass("d-none");
    $(".btnHuy").addClass("d-none");
});
$('table').on('click', '.btnHuy1', function () {
    var $cell = $(this).closest('tr').find('.giacell');
    // Đặt lại giá trị của ô input bằng giá trị ban đầu lưu trong data-initial-value
    $cell.html('<td></td>');

    $(".btnthem").removeClass("d-none");
    $(".btnxoa").removeClass("d-none");

    $(".btnLuu").addClass("d-none");
    $(".btnHuy1").addClass("d-none");
});

$('table').on('click', '.btnLuu', function () {
    var idtd = $(this).closest('tr').find('input').val();
    var ids = $("#IDS").val();
    var $cell = $(this).closest('tr').find('.giacell');
    var gia = $cell.find('input').val().replace(/,/g, '');

    $.ajax({
        type: "post",
        url: "/UpdateGia",
        data: "idtd=" + idtd + "&ids=" + ids + "&gia=" + gia,
        success: function (result) {
            var new_value = $cell.find('input').val();
            $cell.html(new_value);
            $(".btnsua").removeClass("d-none");
            $(".btnthem").removeClass("d-none");

            $(".btnLuu").addClass("d-none");
            $(".btnHuy").addClass("d-none");
            $(".btnHuy1").addClass("d-none");
            $("#tbodygia").load(location.href + " #tbodygia>*", function () {
                //formatNumberInput();

            });

        },
        error: function () {
            alert("Fail to delete");
        }
    });

});
$('table').on('click', '.btnxoa', function () {
    var idtd = $(this).closest('tr').find('input').val();
    var ids = $("#IDS").val();

    // Hỏi người dùng xác nhận xóa
    var confirmDelete = confirm("Bạn có chắc muốn xóa?");

    if (confirmDelete) {
        $.ajax({
            type: "post",
            url: "/DeleteGia",
            data: "idtd=" + idtd + "&ids=" + ids,
            success: function (result) {
                $("#tbodygia").load(location.href + " #tbodygia>*", function () {
                    //formatNumberInput();
                });
            },
            error: function () {
                alert("Fail to delete");
            }
        });
    }
});
