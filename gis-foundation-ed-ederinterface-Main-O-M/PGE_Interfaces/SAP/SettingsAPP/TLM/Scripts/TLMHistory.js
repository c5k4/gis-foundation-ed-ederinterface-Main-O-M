jQuery(document).ready(function () {
    SetHiddenStartMonthFromStartMonth();
    SetMonthPicker();
    SetHistoryUI();
    SetRetrieveDataButtonClick();

    //ME Q2 2020
    SetPreviousMonthBtnClick();
    SetNextMonthBtnClick();
    EnableDisablePrevMonth();
    EnableDisableNextMonth();
});

function SetHiddenStartMonthFromStartMonth() {
    //debugger;
    var dt = $("#StartMonth").val();
    if (dt != null) {
        var newDtMt = dt.substr(0, 2);
        var newDtYr = dt.substr(3, 4);
        $("#StartMonthHid").val(newDtMt + "/01/" + newDtYr);
    }
}

function SetMonthPicker() {
    $('#StartMonthHid').datepicker({
        constrainInput: true,
        showOn: 'button',
        buttonImage: "/Content/images/date-picker.png",
        buttonImageOnly: true,
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'mm/dd/yy',
        onClose: function (dateText, inst) {
            //debugger;
            var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
            var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();

            if (!(VerifyDate(new Date(year, month, 1))))
                return;
            $(this).datepicker('setDate', new Date(year, month, 1));
            var dt = $(this).val();
            var mt = dt.substr(0, 2); //get month
            var yr = dt.substr(6, 4); //get year
            $("#StartMonth").val(mt + "/" + yr);
            EnableDisablePrevMonth();
            EnableDisableNextMonth();
        },
        yearRange: "-4:+0"
    });

}

//ME Q2 2020 - START
function SetNextMonthBtnClick() {
    $("#NextMonthBtnId").click(function () {
        if (document.getElementById("StartMonth").value.length > 0) {
            var dataLastDate = new Date(document.getElementById("DataLastDate").value.split("/")[1], document.getElementById("DataLastDate").value.split("/")[0] - 1, 1);
            var selectedStartDate = new Date(document.getElementById("StartMonth").value.split("/")[1], document.getElementById("StartMonth").value.split("/")[0] - 1, 1);
            if (selectedStartDate < dataLastDate) {
                SetNextMonth();
                EnableDisablePrevMonth();
                EnableDisableNextMonth();
                $("#RetrieveData").click();
            }
        }
    });
}

function SetNextMonth() {

    if (document.getElementById("StartMonth").value.length > 0) {
        var startMonthYear = document.getElementById("StartMonth").value.split("/");
        var oldStartDate = new Date(startMonthYear[1], startMonthYear[0]-1, 1);
        var newStartDate = new Date(oldStartDate.setMonth(oldStartDate.getMonth() + 1));
        var newMonth = newStartDate.getMonth() + 1;   //convert month 0-11 to 1-12
        $("#StartMonth").val(('0' + newMonth).slice(-2) + "/" + newStartDate.getFullYear());
        SetHiddenStartMonthFromStartMonth();
    }
}

function EnableDisableNextMonth() {
    if (document.getElementById("StartMonth").value.length > 0) {
        var dataLastDate = new Date(document.getElementById("DataLastDate").value.split("/")[1], document.getElementById("DataLastDate").value.split("/")[0] - 1, 1);
        var selectedStartDate = new Date(document.getElementById("StartMonth").value.split("/")[1], document.getElementById("StartMonth").value.split("/")[0] - 1, 1);
        if (selectedStartDate < dataLastDate) {
            $("#NextMonthBtnId").removeClass("next-month-img-disabled");
            $("#NextMonthBtnId").addClass("next-month-img");
            return;
        }
    }
    $("#NextMonthBtnId").removeClass("next-month-img");
    $("#NextMonthBtnId").addClass("next-month-img-disabled");
}

function SetPreviousMonthBtnClick() {
    $("#PreviousMonthBtnId").click(function () {
        if (document.getElementById("StartMonth").value.length > 0) {
            var dataStartDate = new Date(document.getElementById("DataStartDate").value.split("/")[1], document.getElementById("DataStartDate").value.split("/")[0] - 1, 1);
            var selectedStartDate = new Date(document.getElementById("StartMonth").value.split("/")[1], document.getElementById("StartMonth").value.split("/")[0] - 1, 1);
            if (selectedStartDate > dataStartDate) {
                SetPreviousMonth();
                EnableDisablePrevMonth();
                EnableDisableNextMonth();
                $("#RetrieveData").click();
            }
        }
    });
}

function SetPreviousMonth() {
    // debugger;
    if (document.getElementById("StartMonth").value.length > 0) {
        var startMonthYear = document.getElementById("StartMonth").value.split("/");
        var oldStartDate = new Date(startMonthYear[1], startMonthYear[0] - 1, 1);
        var newStartDate = new Date(oldStartDate.setMonth(oldStartDate.getMonth() - 1));
        var newMonth = newStartDate.getMonth() + 1;   //convert month 0-11 to 1-12
        $("#StartMonth").val(('0' + newMonth).slice(-2) + "/" + newStartDate.getFullYear());
        SetHiddenStartMonthFromStartMonth();
    }
}

function EnableDisablePrevMonth() {
    if (document.getElementById("StartMonth").value.length > 0) {
        var dataStartDate = new Date(document.getElementById("DataStartDate").value.split("/")[1], document.getElementById("DataStartDate").value.split("/")[0] - 1, 1);
        var selectedStartDate = new Date(document.getElementById("StartMonth").value.split("/")[1], document.getElementById("StartMonth").value.split("/")[0] - 1, 1);
        if (selectedStartDate > dataStartDate) {
            $("#PreviousMonthBtnId").removeClass("prev-month-img-disabled");
            $("#PreviousMonthBtnId").addClass("prev-month-img");
            return;
        }
    }
    $("#PreviousMonthBtnId").removeClass("prev-month-img");
    $("#PreviousMonthBtnId").addClass("prev-month-img-disabled");
}
//ME Q2 2020 - END

function SetRetrieveDataButtonClick() {
    $("#RetrieveData").click(function () {

        var alertMsg = "";
        if (!CheckIfDatePresent("DataStartDate")) {
            alertMsg = "Data Start Date is invalid or empty! Please verify Global Id!";
        }
        if (!CheckIfDatePresent("StartMonth")) {
            alertMsg = alertMsg + "\n" + "Start Month is invalid or empty!";
        }

        if (alertMsg.length > 0) {
            alert(alertMsg);
            return false;
        }
        //$("a.csv-link").css("display", "inline-block");
        if (document.getElementById("StartMonth").value)
            return ValidateStartDate("StartMonth");

    });
}

function SetHistoryUI() {
    $("div#load-history tr:even").css("background-color", "#F8F7FC");
    $("div#load-history tr:odd").css("background-color", "#FFF");
    $("#divProgress").css("display", "none");
    if ($("#DataStartDate").val().length <= 0) {
        $('#RetrieveData').attr("disabled", true);
    }
    else if ($("#DataStartDate").val().length > 0) {
        $('#RetrieveData').attr("disabled", false);
    }

    if ($("#ShowRecOrGen option:selected").text() == "Generation") {
        $("span.neg-sign").addClass("show-neg-sign");
    }

    if ($("#ShowRecOrGen option:selected").text() == "Delivered") {
        $("span.neg-sign").removeClass("show-neg-sign");
    }
}

function VerifyDate(SelectedDate) {
    //debugger;
    if (SelectedDate) {
        if (document.getElementById("EarliestLoadDate").value) {
            var startDate = new Date(document.getElementById("EarliestLoadDate").value);
            var endDate = new Date(document.getElementById("LatestLoadDate").value);

            if (startDate > SelectedDate) {
                alert("No Data available before " + (startDate.getMonth() + 1) + "/" + startDate.getFullYear() + ". Please choose another valid date.");
                return false;
            }
            if (SelectedDate > endDate) {
                alert("No Data available after " + (endDate.getMonth() + 1) + "/" + endDate.getFullYear() + ". Please choose another valid date.");
                return false;
            }
            var monthsDiff = 24 * 60 * 60 * 1000 * 365 * 3;
            var totalDiff = (endDate - SelectedDate) / monthsDiff;
            if (!(totalDiff <= 1)) {
                alert("Invalid start month! It can't be earlier than 36 months!");
                return false;
            }
            return true;
        }
        else {
            return false;
        }
    }
}
    




function ValidateStartDate(startDateId) {
    var txtVal = document.getElementById(startDateId).value;
    var filter = new RegExp("(0[123456789]|10|11|12)([/])([1-2][0-9][0-9][0-9])");
    if (filter.test(txtVal)) {
        var monthYear = txtVal.split("/");
        var month = monthYear[0];
        var year = monthYear[1];
        var startDate = new Date(year, month, 1);
        var todayDate = new Date();
        var todayDateYear = todayDate.getYear();
        var todayDateMonth = todayDate.getMonth();
        var newTodayDate = new Date(todayDateYear, todayDateMonth, 1);
        var monthsDiff = 24 * 60 * 60 * 1000 * 365 * 3;
        var totalDiff = (newTodayDate - startDate) / monthsDiff;
        if (totalDiff <= 1) {
            return true;
        }
        else {
            alert("Invalid start month! It can't be earlier than 36 months!");
            return false;
        }
    }
    else {
        alert("Invalid start date!");
        return false;
    }
}