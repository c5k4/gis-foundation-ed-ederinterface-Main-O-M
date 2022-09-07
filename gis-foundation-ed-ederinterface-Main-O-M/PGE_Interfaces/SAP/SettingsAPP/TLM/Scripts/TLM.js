jQuery(document).ready(function () {
    if (window.location.href.toLowerCase().indexOf("/specialload") > -1) {
        $("li.specialload-li").addClass("selected");
    }
    if (window.location.href.toLowerCase().indexOf("/history") > -1) {
        $("li.history-li").addClass("selected");
    }
    if (window.location.href.toLowerCase().indexOf("/gis") > -1) {
        $("li.gis-li").addClass("selected");
    }
    if (window.location.href.toLowerCase().indexOf("/inputload") > -1) {
        $("li.loading-li").addClass("selected");
    }
    if (window.location.href == "http://" + window.location.host + "/") {
        $("li.loading-li").addClass("selected");
    }

    if (window.location.href.toLowerCase().indexOf("/outputload") > -1) {
        $("li.generation-li").addClass("selected");
        //        $("span.negSign").each(function () {
        //            if ($.trim($(this).closest("div.Cell").text()).length > 0) {
        //                $(this).addClass("showSign");
        //            }
        //        });
    }

    //set up hidden start month from start month
});


