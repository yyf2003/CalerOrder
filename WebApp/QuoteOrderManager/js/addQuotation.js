
//Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {

//    
//});
var POPQuoteJson = [];
var isClick = false;
$(function () {

    LoadPOPQuoteJson();

    $("input[name='btnOOHDec'],input[name='btnBasicDec']").click(function (event) {

        var val = $(this).next("input").val() || 0;
        if (val > 0) {
            val = parseInt(val) - 1;
        }
        else {
            val = 0;
        }
        $(this).next("input").val(val);
        if (val > 0) {
            $(this).next("input").css({ "color": "red" });
        }
        else {
            $(this).next("input").css({ "color": "" });
        }
        var td = $(this).parent();
        var total = 0;
        td.find("input[type='text']").each(function () {
            var count = $(this).val() || 0;
            var price = $(this).data("val") || 0;
            total += (parseInt(count) * parseInt(price));
        })
        td.find("span:last").html(total);
        if (total > 0) {
            td.find("span:last").css({ "color": "blue" });
        }
        else {
            td.find("span:last").css({ "color": "" });
        }
        //CountTotal();
    })

    $("input[name='btnOOHAdd'],input[name='btnBasicAdd']").click(function () {
        var val = $(this).prev("input").val() || 0;
        if (val < 100)
            val = parseInt(val) + 1;
        $(this).prev("input").val(val);
        if (val > 0) {
            $(this).prev("input").css({ "color": "red" });
        }
        else {
            $(this).prev("input").css({ "color": "" });
        }
        var td = $(this).parent();
        var total = 0;
        td.find("input[type='text']").each(function () {
            var count = $(this).val() || 0;
            var price = $(this).data("val") || 0;
            total += (parseInt(count) * parseInt(price));
        })
        td.find("span:last").html(total);
        if (total > 0) {
            td.find("span:last").css({ "color": "blue" });
        }
        else {
            td.find("span:last").css({ "color": "" });
        }
        //CountTotal();

    })

    $("span[name='popSheetSpan']").click(function () {
        var sheet = $(this).html();
        var url = "CheckPOPOrderDetail.aspx?sheet=" + sheet;
        layer.open({
            type: 2,
            time: 0,
            title: '查看订单明细',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '90%'],
            content: url,
            id: 'layer1',

            cancel: function (index) {
                layer.closeAll();
            }

        });
    })

    $("#popTable").delegate("input[name$='txtAddRate']", "click", function () {
        isClick = true;
        showAddRateMenu();
    })

    $("#txtQuoteSubjectName").click(function () {
        showQuoteNameListMenu();
    })

    $("#popTable").delegate("input[name$='txtAddRate']", "blur", function () {
        var val = $(this).val() || 0;
        if (isNaN(val)) {
            val = 0;
            $(this).val(val);
        }
        //ChangeRate();

    })
    $("#ddlAddRate").delegate("li", "click", function () {
        var val = $.trim($(this).html());
        $("input[name$='txtAddRate']").val(val);
        //alert($("input[name$='txtAddRate']").val());
        $("#divAddRate").hide();



    })

    $("#btnAddRate").click(function () {
        ChangeRate();
    })


    $("#otherInstallPriceTable").delegate("input[name$='txtPOPArea']", "blur", function () {
        var val = $.trim($(this).val());
        if (val != "") {
            if (isNaN(val)) {
                layer.msg("面积必须是数值");
                $(this).val("");
            }
        }
    })

    $("input[name='btnAddPOPQuote']").click(function () {
        var sheet = $(this).siblings("select[name$='ddlSheet']").val();
        var material = $(this).siblings("select[name$='ddlMaterial']").val();
        var area = $(this).siblings("input[name$='txtPOPArea']").val();
        var priceType = $(this).parent().parent().siblings("input[name$='hfChangeType']").val();
        $(this).siblings("input[name$='txtPOPArea']").val("");
        if (sheet == "") {
            layer.msg("请选择位置");
            return false;
        }
        if (material == "") {
            layer.msg("请选择材质");
            return false;
        }
        if (area == "" || parseFloat(area) == 0) {
            layer.msg("请填写面积");
            return false;
        }
        var materialName = material.substring(0, material.lastIndexOf('-'));
        var price = material.substring(material.lastIndexOf('-') + 1);

        POPQuoteModel.model.PriceType = priceType;
        POPQuoteModel.model.Sheet = sheet;
        POPQuoteModel.model.MaterialName = materialName;
        POPQuoteModel.model.Price = price;
        POPQuoteModel.model.Area = area;

        POPQuoteModel.AddToJson();

        POPQuoteModel.model.PriceType = 0;
        POPQuoteModel.model.Sheet = "";
        POPQuoteModel.model.MaterialName = "";
        POPQuoteModel.model.Price = 0;
        POPQuoteModel.model.Area = 0;

        /*
        if (POPQuoteJson.length > 0) {
        var table = "<table style='color:blue;'>";
        var hasRow = false;
        var totalPrice = 0;
        for (var i = 0; i < POPQuoteJson.length; i++) {
        if (POPQuoteJson[i].ChangeType == changeType) {
        var tr = "<tr>";
        tr += "<td style='padding:0 5px;'>" + POPQuoteJson[i].Sheet + "</td>";
        tr += "<td style='padding:0 5px;'>" + POPQuoteJson[i].GraphicMaterial + "</td>";
        tr += "<td style='padding:0 5px;'>" + POPQuoteJson[i].AddArea + "平米</td>";
        var subPrice = POPQuoteJson[i].AddArea * POPQuoteJson[i].GraphicMaterialUnitPrice;
        totalPrice += subPrice;
        tr += "<td style='padding:0 5px;'>金额:" + subPrice.toFixed(2) + "</td>";
        tr += "<td style='padding:0 15px;'><span data-changetype='" + changeType + "' name='spanDeletePOPQuote' style='cursor:pointer;color:red;'>×</span></td>";
        tr += "</tr>";
        table += tr;
        hasRow = true;
        }
        }
        table += "</table>";
        if (hasRow) {
        $(this).siblings("div").html(table);
        }
        $(this).next("span").html(totalPrice.toFixed(2));
        CountTotal();
        }
        */
        displayPOPQouteList($(this));
    })

    //删除pop折算报价
    $("#otherInstallPriceTable").delegate("span[name='spanDeletePOPQuote']", "click", function () {
        //var index = $(this).data("jsonindex");
        //alert(index);
        var priceType = $(this).data("pricetype");
        var sheet = $(this).parent().siblings("td").eq(0).html();
        var GraphicMaterial = $(this).parent().siblings("td").eq(1).html();
        var AddArea = $(this).parent().siblings("td").eq(2).html();
        AddArea = AddArea.replace("平米", "");
        //alert(AddArea);
        if (POPQuoteJson.length > 0) {
            var index = -1;
            for (var i = 0; i < POPQuoteJson.length; i++) {
                if (POPQuoteJson[i].PriceType == priceType && POPQuoteJson[i].Sheet == sheet && POPQuoteJson[i].GraphicMaterial == GraphicMaterial && POPQuoteJson[i].AddArea == AddArea) {
                    index = i;
                }
            }

            if (index > -1) {
                POPQuoteJson.splice(index, 1);
                var totalPrice = 0;
                for (var j = 0; j < POPQuoteJson.length; j++) {
                    if (POPQuoteJson[j].priceType == priceType) {
                        var subPrice = POPQuoteJson[j].AddArea * POPQuoteJson[j].GraphicMaterialUnitPrice;
                        totalPrice += subPrice;
                    }
                }
                var parentDiv = $(this).parent().parent().parent().parent().parent();
                parentDiv.prev("span").html(totalPrice.toFixed(2));
                $(this).parent().parent().remove();
                //CountTotal();
            }
        }
    });

    //页面初次加载的时候
    //$("input[name$='txtAddRate']").blur();

    $("input[name='btnAddExtendPOPPrice']").click(function () {
        var sheet = $(this).data("sheet");
        var td = $(this).parent();
        var table1 = $($(this).parent().parent().parent());
        var addTypeEle = $(td).siblings("td").eq(1).find("select");
        var materialEle = $(td).siblings("td").eq(2).find("select");
        var areaEle = $(td).siblings("td").eq(3).find("input");
        var remarkEle = $(td).siblings("td").eq(4).find("input");

        var addType = $(td).siblings("td").eq(1).find("select option:selected").val();
        var material = $(td).siblings("td").eq(2).find("select option:selected").val();
        var area = $(td).siblings("td").eq(3).find("input").val();

        //var addPrice = $(td).siblings("td").eq(2).find("input").val();
        var remark = $(td).siblings("td").eq(4).find("input").val();
        if (addType == "") {
            layer.msg("请选择类型");
            return false;
        }
        if (material == "") {
            layer.msg("请选择材质");
            return false;
        }

        if (area == "") {
            layer.msg("请填写面积");
            return false;
        }
        else if (isNaN(area)) {
            layer.msg("请金额填写不正确");
            return false;
        }
        var materialName = material.substring(0, material.lastIndexOf('-'));
        var unitPrice = material.substring(material.lastIndexOf('-') + 1);
        if (isNaN(unitPrice))
            unitPrice = 0;


        //var json = '{"QuoteItemId":' + quoteItemId + ',"OrderType":1,"Sheet":"' + sheet + '","AddPriceType":"' + addType + '","AddPrice":' + addPrice + ',"Remark":"' + remark + '"}';
        var json = '{"QuoteItemId":' + quoteItemId + ',"OrderType":1,"Sheet":"' + sheet + '","AddPriceType":"' + addType + '","MaterialName":"' + materialName + '","MaterialUnitPrice":' + unitPrice + ',"AddArea":' + area + ',"Remark":"' + remark + '"}';


        $.ajax({
            type: "get",
            url: "handler/AddQuotation.ashx",
            data: { type: 'addQuoteDifferenceDetail', jsonStr: json },
            success: function (data) {

                if (data.indexOf("error") != -1) {
                    var msg = data.split('|')[1];
                    layer.msg("添加失败：" + msg);
                }
                else {
                    $(addTypeEle).val("");
                    $(materialEle).val("");
                    $(areaEle).val("");
                    $(remarkEle).val("");
                    $(table1).find("tr:gt(0)").remove();
                    var json = eval(data);
                    if (json.length > 0) {
                        var totalPrice = 0;
                        for (var i = 0; i < json.length; i++) {
                            var subPrice = json[i].SubPrice || 0;

                            var tr = "<tr style='color:blue;'>";
                            tr += "<td>类型：</td><td style='text-align:left;padding-left:5px;'>" + json[i].AddPriceType + "</td>";
                            tr += "<td style='text-align:left;padding-left:5px;'>材质:" + json[i].MaterialName + "</td>";
                            tr += "<td style='text-align:left;padding-left:5px;'>面积:" + json[i].AddArea + "</td>";
                            tr += "<td style='text-align:left;padding-left:5px;'>备注:" + json[i].Remark + "</td>";
                            tr += "<td><span data-id='" + json[i].Id + "' name='spanDeleteDifference' style='cursor:pointer;color:red;'>×</span></td>";
                            tr += "</tr>";
                            $(table1).append(tr);
                            totalPrice = parseFloat(totalPrice) + parseFloat(subPrice);
                        }

                        var tr = "<tr>";
                        tr += "<td colspan='2' style='text-align:right;'>合计：</td>";
                        tr += "<td style='text-align:left;padding-left:5px;color:green;'>" + totalPrice.toFixed(2) + "</td>";
                        tr += "<td colspan='3'></td>";
                        tr += "</tr>";
                        $(table1).append(tr);

                    }
                }
            }
        })
    })

    $("#popTable").delegate("span[name='spanDeleteDifference']", "click", function () {
        var id = $(this).data("id") || 0;

        var table1 = $(this).parent().parent().parent();
        $.ajax({
            type: "get",
            url: "handler/AddQuotation.ashx",
            data: { type: "deleteQuoteDifferenceDetail", id: id },
            success: function (data) {

                if (data != "error") {
                    $(table1).find("tr:gt(0)").remove();
                    if (data != "") {
                        var json = eval(data);
                        if (json.length > 0) {

                            var totalPrice = 0;
                            for (var i = 0; i < json.length; i++) {
                                var subPrice = json[i].SubPrice || 0;
                                var tr = "<tr style='color:blue;'>";
                                tr += "<td>类型：</td><td style='text-align:left;padding-left:5px;'>" + json[i].AddPriceType + "</td>";
                                tr += "<td style='text-align:left;padding-left:5px;'>材质:" + json[i].MaterialName + "</td>";
                                tr += "<td style='text-align:left;padding-left:5px;'>面积:" + json[i].AddArea + "</td>";
                                tr += "<td style='text-align:left;padding-left:5px;'>备注:" + json[i].Remark + "</td>";
                                tr += "<td><span data-id='" + json[i].Id + "' name='spanDeleteDifference' style='cursor:pointer;color:red;'>×</span></td>";
                                tr += "</tr>";
                                $(table1).append(tr);
                                totalPrice = parseFloat(totalPrice) + parseFloat(subPrice);
                            }
                            var tr0 = "<tr>";
                            tr0 += "<td colspan='2' style='text-align:right;'>合计：</td>";
                            tr0 += "<td style='text-align:left;padding-left:5px;color:green;'>" + totalPrice.toFixed(2) + "</td>";
                            tr0 += "<td colspan='3'></td>";
                            tr0 += "</tr>";
                            $(table1).append(tr0);
                        }
                    }
                }
                else {
                    layer.msg("删除失败！");
                }
            }
        })
    })

    ChangeTextColor();


    $("#basicInstallPriceTB").delegate("span[name='checkQuoteInstallPriceSpan']", "click", function () {
        var levelType = $(this).data("leveltype");
        var price = $(this).data("price");
        var url = "CheckQuoteInstallPrice.aspx?itemId=" + quoteItemId + "&month=" + month + "&guidanceId=" + guidanceId + "&subjectCategory=" + subjectCategory + "&subjectId=" + subjectId + "&region=" + region + "&levelType=" + levelType + "&price=" + price;
        layer.open({
            type: 2,
            time: 0,
            title: '查看安装费信息',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '90%'],
            content: url,
            id: 'layer1',
            cancel: function (index) {
                layer.closeAll();
            }

        });
    })

})

function LoadPOPQuoteJson() {
    var jsonVal = $("#hfPOPQuoteJson").val();
   
    if (jsonVal != "") {
        POPQuoteJson = JSON.parse(jsonVal);
        
        $("input[name='btnAddPOPQuote']").each(function () {
            displayPOPQouteList($(this));
        })
    }
    //$("#otherInstallPriceTable").find("input[name]")
   
}

function displayPOPQouteList(btn) {
    if (POPQuoteJson.length > 0) {
        var priceType = $(btn).parent().parent().siblings("input[name$='hfChangeType']").val();
        
        var table = "<table style='color:blue;'>";
        var hasRow = false;
        var totalPrice = 0;
        for (var i = 0; i < POPQuoteJson.length; i++) {
            if (POPQuoteJson[i].PriceType == priceType) {
                var tr = "<tr>";
                tr += "<td style='padding:0 5px;'>" + POPQuoteJson[i].Sheet + "</td>";
                tr += "<td style='padding:0 5px;'>" + POPQuoteJson[i].GraphicMaterial + "</td>";
                tr += "<td style='padding:0 5px;'>" + POPQuoteJson[i].AddArea + "平米</td>";
                var subPrice = POPQuoteJson[i].AddArea * POPQuoteJson[i].GraphicMaterialUnitPrice;
                totalPrice += subPrice;
                tr += "<td style='padding:0 5px;'>金额:" + subPrice.toFixed(2) + "</td>";
                tr += "<td style='padding:0 15px;'><span data-pricetype='" + priceType + "' name='spanDeletePOPQuote' style='cursor:pointer;color:red;'>×</span></td>";
                tr += "</tr>";
                table += tr;
                hasRow = true;
            }
        }
        table += "</table>";
        if (hasRow) {
            $(btn).siblings("div").html(table);
        }
        //alert(table);
        $(btn).next("span").html(totalPrice.toFixed(2));
        //CountTotal();
    }
}

function ChangeRate() {
    var rate = $.trim($("input[name$='txtAddRate']").val()) || 0;
    var sheets = "";
    $("input[name='cbOneSheet']:checked").each(function () {
        sheets += $(this).val()+",";
    })
    if (sheets == "") {
        layer.msg("请选择位置");
        //$("input[name$='txtAddRate']").val("0");
        return false;
    }
    //$("#hfAddRate").val(rate);
    //$("#hfAddRateSheet").val(sheets);
    //alert(rate);
    //return false;
    $("#loadNewPriceImg").show();
    
    //return true;
    //$("input[name$='txtAddRate']").blur();
    $.ajax({
        type: "get",
        url: "handler/AddQuotation.ashx",
        data: {type:"addRate", month: month, guidanceId: guidanceId, subjectCategory: subjectCategory, subjectId: subjectId, customerId: customerId, sheet: sheets, rate: rate, quoteItemId: quoteItemId },
        success: function (data) {
            $("#loadNewPriceImg").hide();
            if (data == "ok") {
                $("#btnRefresh").click();
            }
            //$("#popList_labNewPOPTotalPrice").html(data);
            //$("#hfAddRatePrice").val(data);
            //CountTotal();
        }
    })
}

function CountTotal() {
    var total0 = 0;
    var total1 = $("#hfQuoteTotalPrice").val() || 0;
    var ratePrice = $("#popList_labNewPOPTotalPrice").html() || 0;
    $("#otherInstallPriceTable span").each(function () {
        if ($(this).prop("id").indexOf("labOOHTotal") != -1 || $(this).prop("id").indexOf("labBasicTotal") != -1 || $(this).prop("id").indexOf("labPOPTotal") != -1) {
            var price = $(this).text() || 0;
            total0 += parseFloat(price);
        }
    })
    total0 += parseFloat(total1);
    total0 += parseFloat(ratePrice);
    total0 = Math.round(total0 * 100) / 100;
    $("#labQuoteTotalPrice").text(total0);
    $("#hfQuoteTotalPrice1").val(total0);
}

function Check() {
    //var quoteSubjectName = $.trim($("#txtQuoteSubjectName").val());
    var quoteSubject=$("#ddlQuoteSubject").val();
    if (quoteSubject == "0") {
        layer.msg("请选择报价项目名称");
        return false;
    }
//    var price = $("#hfQuoteTotalPrice1").val() || 0;
//    if (parseFloat(price) == 0) {
//        layer.msg("报价金额为0，提交失败！");
//        return false;
//    }
//    if (POPQuoteJson.length > 0) {
//        $("#hfPOPQuoteJson").val(JSON.stringify(POPQuoteJson));
//    }
    return true;
}

function showAddRateMenu() {
    $("#divAddRate").css({ left: 10 + "px", top: 24 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function showQuoteNameListMenu() {
    $("#divQuoteSubjectNameList").css({ left: 0 + "px", top: 24 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}   


function hideMenu() {
    $("#divAddRate,#divQuoteSubjectNameList").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "txtAddRate" || event.target.id == "divAddRate" || $(event.target).parents("#divAddRate").length > 0 || event.target.id == "txtQuoteSubjectName" || event.target.id == "divQuoteSubjectNameList" || $(event.target).parents("#divQuoteSubjectNameList").length > 0)) {
        hideMenu();
    }

}


///pop折算报价类型
var POPQuoteModel = {
    model: function () {
        this.ChangeType = 0;
        this.PriceType = 0;
        this.Sheet = "";
        this.MaterialName = "";
        this.Price = 0;
        this.Area = 0;
        this.subPrice = 0;
    },
    AddToJson: function () {
        var json = { PriceType: this.model.PriceType, Sheet: this.model.Sheet, GraphicMaterial: this.model.MaterialName, GraphicMaterialUnitPrice: this.model.Price, AddArea: this.model.Area };

        POPQuoteJson.push(json);
    }
}



function checkfile() {
    var val = $("#FileUpload1").val();
    if (val != "") {
        var extent = val.substring(val.lastIndexOf('.') + 1);
        if (extent != "xls" && extent != "xlsx") {

            layer.msg('只能上传Excel文件');
            return false;
        }
    }
    else {

        layer.msg('请选择文件');
        return false;
    }
    var quoteSubject = $("#ddlQuoteSubject").val();
    if (quoteSubject == "0") {
        layer.msg("请选择报价项目名称");
        return false;
    }
    $("#ImportWaiting").show();
    return true;
}

function ChangeTextColor() {
    $(".output").each(function (e) {
        var td = $(this);
        var val = "";
        var span = $(this).find("span");
        if (span) {
            val = $(span).html();
        }
        else {
            val = $(this).html();
        }
        if (parseFloat(val) != 0) {
            $(td).addClass("numberColor");
        }
    });
    $(".differenceTClass").each(function (e) {
        var td = $(this);
        var val = "";
        var span = $(this).find("span");
        if (span) {
            val = $(span).html();
        }
        else {
            val = $(this).html();
        }
        if (parseFloat(val) > 0) {
            $(td).addClass("differenceColor1");
        }
        if (parseFloat(val) < 0) {
            $(td).addClass("differenceColor2");
        }
    })
}