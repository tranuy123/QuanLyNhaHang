document.addEventListener("DOMContentLoaded", function () {
    
    $(document).on('change', '.single-checkbox', function () {
        var checked = $(this).is(":checked");
        $('.single-checkbox').not(this).prop('checked', false);
        $(this).prop('checked', checked);
        var idProFile = $(this).closest('tr').data('id');
        if (checked) {
            showProfileCT(idProFile);
        } else {
            $('#tbodyDVKTDC').empty();
        }

    });
    // Lắng nghe sự kiện click cho checkbox trong các hàng <tr> trong tbody có id="tbodyDVKT"
    $(document).on('click', '#tbodyDVKT input[type="checkbox"]', function () {
        const isChecked = $(this).prop('checked');
        const dataId = $(this).closest('tr').data('id');
        if (isChecked) {
            const tdSecond = $(this).closest('tr').find('td').eq(1); // Lấy phần tử <td> thứ hai
            const col2Content = tdSecond.html(); // Lấy nội dung của phần tử <td> thứ hai
            var newRow = $('<tr data-id="' + dataId + '"></tr>');
            var col1 = $('<td></td>').text(tdSecond.text());
            var col2 = $('<td><input id="soLuong" class="form-control w-auto" /></td>');
            var col3 = $('<td class="text-center last-td-column"></td>').append('<a href="#" class="deleterowPrf list-group-item-actions"><svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-x text-red" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M18 6l-12 12"></path><path d="M6 6l12 12"></path></svg></a>');
            newRow.append(col1, col2,col3);

            $('#tbodyDVKTDC').append(newRow);
        } else {
            $('#tbodyDVKTDC tr[data-id="' + dataId + '"]').remove();
        }
        updateCheckAllDVKTStatus();
    });

}); //hết DOM
function updateCheckAllDVKTStatus() {
    const allChecked = $('#tbodyDVKT input[type="checkbox"]').length === $('#tbodyDVKT input[type="checkbox"]:checked').length;
    $('.check-all-DVKT').prop('checked', allChecked);
}
function kiemTraHangHoaDaCo() {
    var idDVKT = [];
    $('#tbodyDVKTDC tr').each(function () {
        var rowData = {};

        var dataId = $(this).data('id');
        rowData.Idhh = dataId;
        idDVKT.push(rowData);
    });
    // Kiểm tra và đánh dấu checkbox
    $('#tbodyDVKT tr').each(function () {
        var trDataId = $(this).data('id');

        // Kiểm tra xem trDataId có trong danh sách idDVKT không
        var isIdExist = idDVKT.some(function (item) {
            return item.Idhh === trDataId;
        });

        // Đánh dấu checkbox nếu trDataId có trong danh sách idDVKT
        if (isIdExist) {
            $(this).find('input[type="checkbox"]').prop('checked', true);
        } else {
            $(this).find('input[type="checkbox"]').prop('checked', false);

        }
    });
}
$('select[name=cbLoaiCLSTQ]').on('change', function () {
    var maCLS = $('select[name=cbLoaiCLSTQ]').val();
    var modelMap = {
        MaCls: maCLS,
    };
    showProgress();
    $.ajax({
        type: "post",
        url: "/CLS/CLS_Profile/api/searchWithCloumns",
        data: { modelMap: modelMap },
        success: function (response) {
            if (response.length > 100) {
                updateDataOfTableTQ1(response.slice(0, 99));
                dataResponse = response;
            } else {
                updateDataOfTableTQ1(response);
            }
        }
    });
});



function updateDataOfTableTQ1(datas) {
    $("#tbody-ProfileTQ").empty();
    datas.map((data) =>
        $("#tbody-ProfileTQ").append(getRowTable(data))
    );
}

function getRowTable(data) {
    return `<tr data-id="${data.id}">
                <td class="text-center w-1"><input type="checkbox" class="form-check-input"></td>
                <td>${data.tenPrf}</td>
            </tr>`;
}
function updateDataOfTableTQ2(datas) {
    $("#tbodyDVKT").empty();
    datas.map((data) =>
        $("#tbodyDVKT").append(getRowTableDVKT(data))
    );
}

function getRowTableDVKT(data) {
    return `<tr data-id="${data.id}">
    <td class="text-center w-1"><input type="checkbox" class="form-check-input"></td>
    <td>${data.tenDichVu}</td>
</tr>`;
}

$('#tbodyDVKTDC').on('click', '.deleterowPrf', function () {
    const dataId = $(this).closest('tr').data('id'); // Lấy giá trị của data-id trong hàng đang xóa
    $('#tbodyDVKT tr[data-id="' + dataId + '"] input[type="checkbox"]').prop('checked', false); // Bỏ chọn checkbox trong tbodyDVKT có data-id tương ứng
    $(this).closest('tr').remove(); // Xóa hàng trong tbodyDVKTDC
});


// Ví dụ khi người dùng click vào nút lưu
function luuProFileCt() {
        var idPRF;
        $('#tbody-ProfileTQ input[type="checkbox"]').each(function () {
            if ($(this).prop('checked')) {
                idPRF = $(this).closest('tr').data('id'); // Lấy giá trị data-id của hàng được chọn (trong tbody-ProfileTQ)
                return false; // Thoát khỏi vòng lặp nếu đã tìm thấy idPRF
            }
        });
        if (idPRF == null || idPRF == undefined) {
            showToast("Vui lòng chọn thực đơn", 500);
            return;
        }
        var idDVKT = []; // Khởi tạo mảng idDVKT để chứa các giá trị data-id của các tr trong tbodyDVKTDC

        // Duyệt qua từng hàng trong #tbodyDVKTDC và lấy giá trị data-id
    $('#tbodyDVKTDC tr').each(function () {
        var rowData = {};

            var dataId = $(this).data('id');
        var soLuong = $(this).find("#soLuong").val();
        rowData.Idhh = dataId;
        rowData.SoLuong = Number(soLuong);
        idDVKT.push(rowData);
    });
        if (idDVKT == "") {
            showToast("Vui lòng chọn hàng hóa", 500);
            return;
        }

        // Gọi hàm luuProfileCt để gửi dữ liệu lên server
        luuProfileCt(idPRF, idDVKT);
};


function luuProfileCt(idPRF, idDVKT) {
    var data = {
        IdPRF: idPRF,
        IdDVKT: idDVKT
    };
    console.log(data);
    $.ajax({
            url: "/DinhMuc/LuuDinhMuc", // Địa chỉ URL của action updateCt trong Controller
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (response) {
                // Xử lý dữ liệu trả về từ server nếu cần
                showToast(response.message, response.statusCode);
                $('#tbody-ProfileTQ input[type="checkbox"]').each(function () {
                    // Tìm hàng chứa checkbox được chọn và thêm style cho nó
                    $(this).closest('tr').addClass('text-red');
                    // Thêm sự kiện onclick vào ô input
                    $(this).click(function () {
                        // Gọi hàm showProfileCT với tham số là idPRF
                        showProfileCT(idPRF);
                    });
                    return false;
                });
            },
            error: function (xhr, status, error) {
                // Xử lý lỗi nếu có
                console.error(error);
            }
        });
    
}

function showProfileCT(id) {
    $.ajax({
        url: "/DinhMuc/showDinhMuc",
        type: "POST",
        data: {
            idPRF: id,
        },
        success: function (response) {
            if (response.length > 0) {
                updateDataOfTableDVKTDC(response)
            } else {
                $('#tbodyDVKTDC').empty();
            }
        },
        error: function (xhr, status, error) {
            // Xử lý lỗi nếu có
            console.error(error);
        }
    });
}

function updateDataOfTableDVKTDC(datas) {
    $("#tbodyDVKTDC").empty();
    datas.map((data) =>
        $("#tbodyDVKTDC").append(getRowTableDVKTDC(data))
    );
    kiemTraHangHoaDaCo();
}

function getRowTableDVKTDC(data) {
    console.log(data);
    return `<tr data-id="${data.idhhNavigation.idhh}">
    <td>${data.idhhNavigation.tenHh}</td>
    <td><input id="soLuong" class="form-control w-50" value="${data.soLuong}"/></td>
    <td class="text-center last-td-column"><a href="#" class="deleterowPrf list-group-item-actions"><svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-x text-red" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M18 6l-12 12"></path><path d="M6 6l12 12"></path></svg></a></td>
</tr>`;
}
$('.deleteallrow').on('click', function () {
    $('#tbodyDVKTDC').empty();
})
$('.cbDichVuKyThuat').on('change', function () {
    var selectedValue = $(this).val();
    if (selectedValue != '') {
        var selectedText = $(this).find("option:selected").text();
        var newRow = $('<tr data-id="' + selectedValue + '"></tr>');
        var col1 = $('<td></td>').text(selectedText);
        var col2 = $('<td class="text-center last-td-column"></td>').append('<a href="#" class="deleterowPrf list-group-item-actions"><svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-x text-red" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M18 6l-12 12"></path><path d="M6 6l12 12"></path></svg></a>');
        newRow.append(col1, col2);

        $('#tbodyDVKTDC').append(newRow);
        cleanSelect('.cbDichVuKyThuat');
    }
});
function cleanSelect(nameClass) {
    var selectElements = document.querySelectorAll(nameClass);
    selectElements.forEach(function (selectElement) {
        if (selectElement.tomselect) {
            selectElement.tomselect.clear();
        } else {
            selectElement.selectedIndex = 0;
        }
    });
}
$('.check-all-DVKT').on('click', function () {
    const isChecked = $(this).prop('checked');
    $('#tbodyDVKT input[type="checkbox"]').prop('checked', isChecked);

    if (isChecked) {
        // Thêm các dòng vào #tbodyDVKTDC nếu checkbox đã được check
        $('#tbodyDVKT input[type="checkbox"]:checked').each(function () {
            const dataId = $(this).closest('tr').data('id');
            const tdSecond = $(this).closest('tr').find('td').eq(1);
            const col1Content = tdSecond.text();
            const newRow = $('<tr data-id="' + dataId + '"></tr>');
            const col1 = $('<td></td>').text(col1Content);
            const col2 = $('<td class="text-center last-td-column"></td>').append('<a href="#" class="deleterowPrf list-group-item-actions"><svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-x text-red" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"></path><path d="M18 6l-12 12"></path><path d="M6 6l12 12"></path></svg></a>');
            newRow.append(col1, col2);
            $('#tbodyDVKTDC').append(newRow);
        });
    } else {
        // Xóa các dòng khỏi #tbodyDVKTDC nếu checkbox không được check
        $('#tbodyDVKTDC').empty();
    }
    updateCheckAllDVKTStatus();
});


function updateQuyenButtonLuu() {
    var isCheck = false;
    if (_isCapNhatProfile) {
        if (_qSua) {
            isCheck = true;
        }
    } else {
        if (_qThem) {
            isCheck = true;
        }
    }
    if (isCheck) {
        $('#btnLuuProfile').attr('onclick', 'update()');
        $('#btnLuuProfile').removeClass('disabled');
    } else {
        $('#btnLuuProfile').removeAttr('onclick');
        $('#btnLuuProfile').addClass('disabled');
    }
}