
function hideMenu() {
    $(".showDll").not($(this)).siblings("div").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}
function onBodyDown(event) {
    if (!(event.target.id == "txtCategory" || event.target.id == "divCategory" || $(event.target).parents("#divCategory").length > 0 || event.target.id == "txtBelongs" || event.target.id == "divBelongs" || $(event.target).parents("#divBelongs").length > 0 || event.target.id == "txtClassification" || event.target.id == "divClassification" || $(event.target).parents("#divClassification").length > 0 || event.target.id == "txtTaxRate" || event.target.id == "divTaxRate" || $(event.target).parents("#divTaxRate").length > 0 || event.target.id == "txtAccount" || event.target.id == "divAccount" || $(event.target).parents("#divAccount").length > 0)) {
        hideMenu();
    }


}
$(function () {
    GetQuotationCategory();
    GetQuotationBlongs();
    GetQuotationClassification();
    GetQuotationAccount();
    GetQuotationTaxRate();
    LoadPrices();

    $(".showDll").on("click", function () {
        $(".showDll").not($(this)).siblings("div").hide();
        $(this).siblings("div").css({ left: -1 + "px", top: 20 + "px", "z-index": 100 }).fadeIn("fast");
        $("body").bind("mousedown", onBodyDown);
    })

    $("#editTb").delegate("div[name='dllVal']", "click", function () {

        var tb = $(this).parent().parent().parent().siblings("input[type='text']");
        $(tb).val($(this).html());
    })
    $("#editTb").delegate("span[name='deleteDll']", "click", function () {
        var li = $(this).parent();
        var id = $(this).data("itemid");
        var type = $(this).data("itemtype");
        type = type.replace("ddl", "del");
        $.ajax({
            type: "get",
            url: "handler/Edit.ashx?type=" + type + "&id=" + id,
            cache: false,
            success: function (data) {
                if (data == "ok") {
                    li.remove();
                }
                else {
                    alert("删除失败！");
                }
            }
        })
    })

    //添加分摊金额
    $("#btnAddSharePrice").on("click", function () {
        var SharePriceName = $.trim($("#txtSharePriceName").val());
        var SharePrice = $.trim($("#txtSharePrice").val());
        if (SharePriceName == "") {
            alert("请填写费用名称");
            return false;
        }
        if (SharePrice == "") {
            alert("请填写费用金额");
            return false;
        }
        if (isNaN(SharePrice)) {
            alert("费用金额必须是数字");
            return false;
        }
        var tr = "<tr class='tr_bai'>";
        tr += "<td><input type='text' name='txtSharePriceName' value='" + SharePriceName + "' maxlength='20' style=' text-align:center; width:90%;'/></td>";
        tr += "<td><input type='text' name='txtSharePrice' value='" + SharePrice + "' maxlength='10' style=' text-align:center; width:80%;'/></td>";
        tr += "<td><span name='deleSharePrice' style=' color:Red; cursor:pointer;'>删除</span></td>";
        tr += "</tr>";
        $("#SharePrice").append(tr);
    })

    //删除分摊金额
    $("#SharePrice").delegate("span[name='deleSharePrice']", "click", function () {
        $(this).parent().parent().remove();
    })

    //添加并入金额
    $("#btnAddBingPrice").on("click", function () {
        var BingPriceName = $.trim($("#txtBingPriceName").val());
        //var BingPriceName = $("#ddlSubjects").val();
        var BingPrice = $.trim($("#txtBingPrice").val());
        
        if (BingPriceName == "") {
            alert("请选择费用名称");
            return false;
        }
        if (BingPrice == "") {
            alert("请填写费用金额");
            return false;
        }
        if (isNaN(BingPrice)) {
            alert("费用金额必须是数字");
            return false;
        }
        var tr = "<tr class='tr_bai'>";
        tr += "<td><input type='text' name='txtBingPriceName' value='" + BingPriceName + "' maxlength='20' style=' text-align:center; width:90%;'/></td>";
        tr += "<td><input type='text' name='txtBingPrice' value='" + BingPrice + "' maxlength='10' style=' text-align:center; width:80%;'/></td>";
        tr += "<td><span name='deleBingPrice' style=' color:Red; cursor:pointer;'>删除</span></td>";
        tr += "</tr>";
        $("#BingPrice").append(tr);
    })

    //删除并入金额
    $("#BingPrice").delegate("span[name='deleBingPrice']", "click", function () {
        $(this).parent().parent().remove();
    })
})

//编辑的时候加载分摊金额和并入金额
function LoadPrices() {
    var sharePrice = $("#hfSharePrice").val();
    if (sharePrice != "") {
        $("#SharePrice").html("");
        var json1 = eval(sharePrice);
        for (var i = 0; i < json1.length; i++) {
            var tr = "<tr class='tr_bai'>";
            tr += "<td><input type='text' name='txtSharePriceName' value='" + json1[i].PriceName + "' maxlength='20' style=' text-align:center; width:90%;'/></td>";
            tr += "<td><input type='text' name='txtSharePrice' value='" + json1[i].Price + "' maxlength='10' style=' text-align:center; width:80%;'/></td>";
            tr += "<td><span name='deleSharePrice' style=' color:Red; cursor:pointer;'>删除</span></td>";
            tr += "</tr>";
            $("#SharePrice").append(tr);
        }
    }
    var bingPrice = $("#hfBingPrice").val();

    if (bingPrice != "") {

        var json2 = eval(bingPrice);
        for (var k = 0; k < json2.length; k++) {
            var tr1 = "<tr class='tr_bai'>";
            tr1 += "<td><input type='text' name='txtBingPriceName' value='" + json2[k].PriceName + "' maxlength='20' style=' text-align:center; width:90%;'/></td>";
            tr1 += "<td><input type='text' name='txtBingPrice' value='" + json2[k].Price + "' maxlength='10' style=' text-align:center; width:80%;'/></td>";
            tr1 += "<td><span name='deleBingPrice' style=' color:Red; cursor:pointer;'>删除</span></td>";
            tr1 += "</tr>";
            $("#BingPrice").append(tr1);
        }
    }
    $("#hfSharePrice").val("");
    $("#hfBingPrice").val("");
}

function CheckPriceVal() {
    var Category = $("#ddlCategory").val();
    if (Category == "0") {
        alert("请选择类别");
        return false;
    }
    var Belongs = $("#ddlBelongs").val();
    if (Belongs == "0") {
        alert("请选择AD费用归属");
        return false;
    }
    var Classification = $.trim($("#txtClassification").val());
    if (Classification == "") {
        alert("请填写分类");
        return false;
    }
    var AdidasContact = $.trim($("#txtAdidasContact").val());
    var TaxRate = $("#ddlTaxRate").val();
    if (TaxRate == "0") {
        alert("请选择税率");
        return false;
    }
    var Account = $.trim($("#txtAccount").val());
    if (Account == "") {
        alert("请填写报价账户");
        return false;
    }
    var OfferPrice = $.trim($("#txtOfferPrice").val());
    if (OfferPrice == "") {
        alert("请填写报价金额");
        return false;
    }
    if (isNaN(OfferPrice)) {
        alert("报价金额必须为数字");
        return false;
    }
    var OtherPrice = $.trim($("#txtOtherPrice").val());
    var OtherPriceRemark = $.trim($("#txtOtherPriceRemark").val());
    if (OtherPrice != "") {
        if (isNaN(OtherPrice)) {
            alert("挂账金额必须为数字");
            return false;
        }
        if (OtherPriceRemark == "") {
            alert("请填写挂账说明");
            return false;
        }
    }
    var Remark = $.trim($("#txtRemark").val());
    if (confirm("确定提交吗？")) {
        CheckSharePrice();
        CheckBingPrice();
        return true;
    }
    else
        return false;
}

function CheckSharePrice() {
    var json = "";
    $("#SharePrice tr").each(function () {
        var name = $(this).find("input[name='txtSharePriceName']").val();
        var price = $(this).find("input[name='txtSharePrice']").val();
        var canSave = true;
        if ($.trim(name) == "") {
            canSave = false;
        }
        if ($.trim(price) == "") {
            canSave = false;
        }
        else if (isNaN(price)) {
            canSave = false;
        }
        if (canSave) {
            json += '{"PriceType":"1","PriceName":"' + name + '","Price":"' + price + '"},';
        }
    })
    if (json.length > 0) {
        json = json.substring(0, json.length - 1);
        $("#hfSharePrice").val("[" + json + "]");
    }
    else
        $("#hfSharePrice").val("");
}

function CheckBingPrice() {
    var json = "";
    $("#BingPrice tr").each(function () {
        var name = $(this).find("input[name='txtBingPriceName']").val();
        var price = $(this).find("input[name='txtBingPrice']").val();
        var canSave = true;
        if ($.trim(name) == "") {
            canSave = false;
        }
        if ($.trim(price) == "") {
            canSave = false;
        }
        else if (isNaN(price)) {
            canSave = false;
        }
        if (canSave) {
            json += '{"PriceType":"2","PriceName":"' + name + '","Price":"' + price + '"},';
        }
    })
    if (json.length > 0) {
        json = json.substring(0, json.length - 1);
        $("#hfBingPrice").val("[" + json + "]");
    }
    else
        $("#hfBingPrice").val("");
}

function InitDll(obj, jsonStr) {
    var json = eval(jsonStr);
    var id = $(obj).attr("id");
    for (var i = 0; i < json.length; i++) {
        var li = "<li><div name='dllVal' style='min-width:100px;float:left;'>" + json[i].Name + "</div><span name='deleteDll' data-itemtype='" + id + "' data-itemid='" + json[i].Id + "' style='color:red;cursor:pointer;float:right;'>×</span></li>";
        $(obj).append(li);
    }
}


function GetQuotationCategory() {
    $("#ddlCategory").html("");
    $.ajax({
        type: "get",
        url: "handler/Edit.ashx?type=category",
        cache: false,
        success: function (data) {
            if (data != "") {
                InitDll($("#ddlCategory"), data);
            }
        }
    })
}

function GetQuotationBlongs() {
    $("#ddlBelongs").html("");
    $.ajax({
        type: "get",
        url: "handler/Edit.ashx?type=blongs",
        cache: false,
        success: function (data) {
            if (data != "") {
                InitDll($("#ddlBelongs"), data);
            }
        }
    })
}

function GetQuotationClassification() {
    $("#ddlClassification").html("");
    $.ajax({
        type: "get",
        url: "handler/Edit.ashx?type=classification",
        cache: false,
        success: function (data) {
            if (data != "") {
                InitDll($("#ddlClassification"), data);
            }
        }
    })
}

function GetQuotationAccount() {
    $("#ddlAccount").html("");
    $.ajax({
        type: "get",
        url: "handler/Edit.ashx?type=account",
        cache: false,
        success: function (data) {
            if (data != "") {
                InitDll($("#ddlAccount"), data);
            }
        }
    })
}

function GetQuotationTaxRate() {
    $("#ddlTaxRate").html("");
    $.ajax({
        type: "get",
        url: "handler/Edit.ashx?type=taxrate",
        cache: false,
        success: function (data) {
            if (data != "") {
                InitDll($("#ddlTaxRate"), data);
            }
        }
    })
}


$(function () {
    GetSubjectList();
    $("#showBingPriceName").on("click", function () {
        if ($("#divBingPriceName").is(":hidden")) {
            $("#divBingPriceName").show().animate({
                height: '280px',
                width: '280px',
                top: "-265px"
            }, 200);
        }
    })

    $("#closeBingPriceName").on("click", function () {
        $("#divBingPriceName").animate({
            height: '1px',
            width: '1px',
            top: "1px"
        }, 200, function () {
            $("#divBingPriceName").hide();
        });
    })


    $("#btnSearchSubject").on("click", function () {
        $("#BingPriceNameList").html("");
        GetSubjectList();
    })

    

    $("#BingPriceNameList").delegate("li", "click", function () { 
      $("#txtBingPriceName").val($(this).html());
    })

})

function GetSubjectList() {
    var date = $("#txtDate").val();
    $.ajax({
        type: "get",
        url: "handler/Edit.ashx?type=getProjectList&subjectId=" + subjectId + "&dateStr=" + escape(date),
        cache: false,
        success: function (data) {
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var li = "<li>" + json[i].SubjectName + "</li>";
                    $("#BingPriceNameList").append(li);
                }
            }
        }
    })
}