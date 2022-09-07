$(function () {
    $(".peakdate").datepicker();
    $(".newpeakdate").datepicker();
    $(".peakdate").focusin(function () {
        if ($(this).val() != '') {
            $(".ui-datepicker-prev").css("display", "none"); $(".ui-datepicker-next").css("display", "none");
        }
    });
    $(".peaktime").focusout(function () {
        var vTime = $(this).val();
        if (vTime == null || vTime == '') {
            $(this).val('0000');
            return;
        }
        else {
            if (vTime.length == 4) {
                var isnum = /^\d+$/.test(vTime);
                if (isnum == true) {
                    var hour = vTime.substring(0, 2);
                    var min = vTime.substring(2, 4);
                    if (parseInt(hour) > 23) {
                        alert("Invalid peak time.Value must be in the 24 hours format-HHMM!");
                        $(this).focus();
                        return false;
                    }
                    if (parseInt(min) > 59) {
                        alert("Invalid peak time.Value must be in the 24 hours format-HHMM!");
                        $(this).focus();
                        return false;
                    }
                }
                else {
                    alert("Invalid peak time.Value must be in the 24 hours format-HHMM!");
                    $(this).focus();
                    return false;
                }
            }
            else {
                alert("Invalid peak time.Value must be in the 24 hours format-HHMM!");
                $(this).focus();
                return false;
            }
        }
    });

    $(".neg-decimal-no").each(function () {
        if ($(this).val() != null || $.trim($(this).val()) != '') {
            var ctrlValue = parseFloat($(this).val());
            if (!isNaN(ctrlValue)) {
                $(this).val(ctrlValue.toString());
            }
        }
    });
    $(".decimal-no").each(function () {
        if ($(this).val() != null || $.trim($(this).val()) != '') {
            var ctrlValue = parseFloat($(this).val());
            if (!isNaN(ctrlValue)) {
                $(this).val(ctrlValue.toString());
            }
        }
    });
    $(".int-no").keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
        // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
        // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
    $(".neg-int-no").keydown(function (e) {
        //negative sign
        if (($(this).val().indexOf('-') !== -1 && (e.keyCode == 109 || e.keyCode == 189))) {
            e.preventDefault();
        }
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 109, 110, 189]) !== -1 ||
        // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
        // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
    $(".decimal-no").keydown(function (e) {
        if (($(this).val().indexOf('.') !== -1 && e.keyCode == 190)) {
            e.preventDefault();
        }
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
        // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
        // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }

    });

    $(".neg-decimal-no").keydown(function (e) {
        if (($(this).val().indexOf('.') !== -1 && e.keyCode == 190)) {
            e.preventDefault();
        }
        if (($(this).val().indexOf('-') !== -1 && (e.keyCode == 109 || e.keyCode == 189))) {
            e.preventDefault();
        }
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 109, 110, 189, 190]) !== -1 ||
        // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
        // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
});



    $("#upload").change(function () {
        alert('changed!');
    });

$(".OnScadaChange").change(function (e) {
    alert("test");
    $(flisrId).val("N");
    if ($(scadaId).val() == "Y")
        $(flisrId).prop("disabled", false);
    else
        $(flisrId).prop("disabled", true);
});

function CheckNumberOnly(e) {
    // Allow: backspace, delete, tab, escape, enter and .
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
    // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
    // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
        // let it happen, don't do anything
        return true;
    }
    // Ensure that it is a number and stop the keypress
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
        return false;
    }
}
 

 