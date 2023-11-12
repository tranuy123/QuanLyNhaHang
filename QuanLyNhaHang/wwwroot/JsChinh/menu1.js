    $(".closetoastMN").on("click", function () {

        $('.toastMN').toast('hide');
        });
    $(".closetoastHMN").on("click", function () {

        $('.toastHMN').toast('hide');
        });

    $(function () {
        $('.add-to-cart').click(function () {
            var button = $(this); // lưu lại context
            var idtd = button.data('idtd');
            var price = button.data('price');
            var ipmac = button.data('ipmac');
            $.ajax({
                type: "post",
                url: "/addHoaDon",
                data: "IPMAC=" + ipmac + "&IDTD=" + idtd + "&DonGia=" + price,
                success: function (result) {
                    if (result.statusCode == 200) {
                        button.addClass('d-none');

                        button.siblings('.remove-from-cart').removeClass('d-none');
                        $('.toastMN').toast('show');
                    }

                },
                error: function () {
                    alert("Fail");
                }
            });
        });


    $('.remove-from-cart').click(function () {
    var button = $(this); // lưu lại context
    var idtd = button.data('idtd');
    var ipmac = button.data('ipmac');


    $.ajax({
    type: "post",
    url: "/HuyHoaDonTam",
    data: "IPMAC=" + ipmac + "&IDTD=" + idtd,
        success: function (result) {
            console.log(ipmac, idtd);
            if (result.statusCode == 200) {
                console.log(123);
                button.addClass('d-none'); // sử dụng biến lưu lại context
                button.siblings('.add-to-cart').removeClass('d-none');
                $('.toastHMN').toast('show');

            }
        },
    error: function () {
        alert(ipmac);
    alert(idtd);
    alert("Fail");
                    }
                });
                
            });
        });
