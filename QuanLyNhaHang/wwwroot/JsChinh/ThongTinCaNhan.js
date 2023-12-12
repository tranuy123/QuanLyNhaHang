$(document).on('change', '#fileAvatar', function (e) {
    URL.revokeObjectURL($('#previewImage').prop('src'))
    const file = e.target.files[0];
    file.src = URL.createObjectURL(file);
    $('#previewImage').prop('src', file.src);
})