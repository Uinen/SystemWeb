$(function () {
    $("#from").ejDatePicker({
        locale: "it-IT",
        onClose: function (selectedDate) {
            $("#to").ejDatePicker("minDate", selectedDate);

        }
    });
    $("#to").ejDatePicker({
        locale: "it-IT",
        onClose: function (selectedDate) {
            $("#from").ejDatePicker("maxDate", selectedDate);

        },
    });
});