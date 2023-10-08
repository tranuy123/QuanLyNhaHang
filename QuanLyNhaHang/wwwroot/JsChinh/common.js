function formatDay(inputString) {
    if (inputString) {
        var inputDate = new Date(inputString);
        var day = inputDate.getDate();
        if (day < 10) {
            day = '0' + day;
        }
        var month = inputDate.getMonth() + 1;
        if (month < 10) {
            month = '0' + month;
        }
        var year = inputDate.getFullYear();
        return day + '-' + month + '-' + year;
    } else {
        return ""
    }
}
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD' }).format(amount);
}
