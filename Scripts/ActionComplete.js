function ActionComplete(args, sender) {
    var gridObj = $("#FlatGrid").ejGrid("instance");
    this.getContent().addClass("e-widget");
    var browserDetails = gridObj.getBrowserDetails();
    if (browserDetails.browser == "msie" && parseInt(browserDetails.version, 10) <= 9)
        $("#FlatGrid").ejGrid("model.enableResponsiveRow", false);
    if (args.requestType == "filtering" || args.requestType == "searching") {
        var proxy = this;
        setTimeout(function () { proxy.windowonresize(); }, 30);
    }

    if (args.requestType == "BeginEdit") {

        var date = $("#" + sender._ID + "cData" + "rData").val();

        $("#" + sender._ID + "cData" + "rData").val($.ejDatePicker.formatDate("mm/dd/yy", new Date(date)));

    }
}

function ActionCompleteCali(args, sender) {
    var gridObj = $("#FlatGridCali").ejGrid("instance");
    this.getContent().addClass("e-widget");
    var browserDetails = gridObj.getBrowserDetails();
    if (browserDetails.browser == "msie" && parseInt(browserDetails.version, 10) <= 9)
        $("#FlatGridCali").ejGrid("model.enableResponsiveRow", false);
    if (args.requestType == "filtering" || args.requestType == "searching") {
        var proxy = this;
        setTimeout(function () { proxy.windowonresize(); }, 30);
    }

    if (args.requestType == "BeginEdit") {

        var date = $("#" + sender._ID + "FieldData").val();

        $("#" + sender._ID + "FieldData" ).val($.ejDatePicker.formatDate("mm/dd/yy", new Date(date)));

    }
}

function ActionCompleteErogatori(args, sender) {
    var gridObj = $("#FlatGridErogatori").ejGrid("instance");
    this.getContent().addClass("e-widget");
    var browserDetails = gridObj.getBrowserDetails();
    if (browserDetails.browser == "msie" && parseInt(browserDetails.version, 10) <= 9)
        $("#FlatGridErogatori").ejGrid("model.enableResponsiveRow", false);
    if (args.requestType == "filtering" || args.requestType == "searching") {
        var proxy = this;
        setTimeout(function () { proxy.windowonresize(); }, 30);
    }

    if (args.requestType == "BeginEdit") {

        var date = $("#" + sender._ID + "FieldData").val();

        $("#" + sender._ID + "FieldData").val($.ejDatePicker.formatDate("mm/dd/yy", new Date(date)));

    }
}

function ActionCompleteDispenser(args, sender) {
    var gridObj = $("#FlatGridDispenser").ejGrid("instance");
    this.getContent().addClass("e-widget");
    var browserDetails = gridObj.getBrowserDetails();
    if (browserDetails.browser == "msie" && parseInt(browserDetails.version, 10) <= 9)
        $("#FlatGridDispenser").ejGrid("model.enableResponsiveRow", false);
    if (args.requestType == "filtering" || args.requestType == "searching") {
        var proxy = this;
        setTimeout(function () { proxy.windowonresize(); }, 30);
    }
}

function ActionCompleteTank(args, sender) {
    var gridObj = $("#FlatGridTank").ejGrid("instance");
    this.getContent().addClass("e-widget");
    var browserDetails = gridObj.getBrowserDetails();
    if (browserDetails.browser == "msie" && parseInt(browserDetails.version, 10) <= 9)
        $("#FlatGridTank").ejGrid("model.enableResponsiveRow", false);
    if (args.requestType == "filtering" || args.requestType == "searching") {
        var proxy = this;
        setTimeout(function () { proxy.windowonresize(); }, 30);
    }

    if (args.requestType == "BeginEdit") {

        var date = $("#" + sender._ID + "LastDate").val();

        $("#" + sender._ID + "LastDate").val($.ejDatePicker.formatDate("mm/dd/yy", new Date(date)));

    }
}

function ActionCompleteDeficienze(args, sender) {
    var gridObj = $("#FlatGridDeficienze").ejGrid("instance");
    this.getContent().addClass("e-widget");
    var browserDetails = gridObj.getBrowserDetails();
    if (browserDetails.browser == "msie" && parseInt(browserDetails.version, 10) <= 9)
        $("#FlatGridDeficienze").ejGrid("model.enableResponsiveRow", false);
    if (args.requestType == "filtering" || args.requestType == "searching") {
        var proxy = this;
        setTimeout(function () { proxy.windowonresize(); }, 30);
    }

    if (args.requestType == "BeginEdit") {

        var date = $("#" + sender._ID + "LastDate").val();

        $("#" + sender._ID + "FieldData").val($.ejDatePicker.formatDate("mm/dd/yy", new Date(date)));

    }
}

function ActionCompleteProducts(args, sender) {
    var gridObj = $("#FlatGridProducts").ejGrid("instance");
    this.getContent().addClass("e-widget");
    var browserDetails = gridObj.getBrowserDetails();
    if (browserDetails.browser == "msie" && parseInt(browserDetails.version, 10) <= 9)
        $("#FlatGridProducts").ejGrid("model.enableResponsiveRow", false);
    if (args.requestType == "filtering" || args.requestType == "searching") {
        var proxy = this;
        setTimeout(function () { proxy.windowonresize(); }, 30);
    }
}

function ActionCompleteCartissima(args, sender) {
    var gridObj = $("#FlatGridCartissima").ejGrid("instance");
    this.getContent().addClass("e-widget");
    var browserDetails = gridObj.getBrowserDetails();
    if (browserDetails.browser == "msie" && parseInt(browserDetails.version, 10) <= 9)
        $("#FlatGridCartissima").ejGrid("model.enableResponsiveRow", false);
    if (args.requestType == "filtering" || args.requestType == "searching") {
        var proxy = this;
        setTimeout(function () { proxy.windowonresize(); }, 30);
    }
}