
//Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {

//    
//});
var POPQuoteJson = [];
$(function () {
    $("input[name='btnOOHDec'],input[name='btnBasicDec']").click(function (event) {

        var val = $(this).next("input").val() || 0;
        if (val > 0) {
            val = parseInt(val) - 1;
        }
        else {
            val = 0;
        }
        $(this).next("input").val(val);

        var td = $(this).parent();
        var total = 0;
        td.find("input[type='text']").each(function () {
            var count = $(this).val() || 0;
            var price = $(this).data("val") || 0;
            total += (parseInt(count) * parseInt(price));
        })
        td.find("span:last").html(total);
        CountTotal();
    })

    $("input[name='btnOOHAdd'],input[name='btnBasicAdd']").click(function () {
        var val = $(this).prev("input").val() || 0;
        if (val < 100)
            val = parseInt(val) + 1;
        $(this).prev("input").val(val);


        var td = $(this).parent();
        var total = 0;
        td.find("input[type='text']").each(function () {
            var count = $(this).val() || 0;
            var price = $(this).data("val") || 0;
            total += (parseInt(count) * parseInt(price));
        })
        td.find("span:last").html(total);
        CountTotal();

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
        showAddRateMenu();
    })
    $("#popTable").delegate("input[name$='txtAddRate']", "blur", function () {
        var val = $(this).val() || 0;
        if (isNaN(val)) {
            val = 0;
            $(this).val(val);
        }
        ChangeRate();
    })
    $("#ddlAddRate").delegate("li", "click", function () {
        var val = $.trim($(this).html());
        $("input[name$='txtAddRate']").val(val);
        $("#divAddRate").hide();
        //$("input[name$='txtAddRate']").blur();
        $("#loadNewPriceImg").show();
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
        var changeType = $(this).parent().parent().siblings("input[name$='hfChangeType']").val();
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

        POPQuoteModel.model.ChangeType = changeType;
        POPQuoteModel.model.Sheet = sheet;
        POPQuoteModel.model.MaterialName = materialName;
        POPQuoteModel.model.Price = price;
        POPQuoteModel.model.Area = area;

        POPQuoteModel.AddToJson();

        POPQuoteModel.model.ChangeType = 0;
        POPQuoteModel.model.Sheet = "";
        POPQuoteModel.model.MaterialName = "";
        POPQuoteModel.model.Price = 0;
        POPQuoteModel.model.Area = 0;

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

    })

    //删除pop折算报价
    $("#otherInstallPriceTable").delegate("span[name='spanDeletePOPQuote']", "click", function () {
        //var index = $(this).data("jsonindex");
        //alert(index);
        var changeType = $(this).data("changetype");
        var sheet = $(this).parent().siblings("td").eq(0).html();
        var GraphicMaterial = $(this).parent().siblings("td").eq(1).html();
        var AddArea = $(this).parent().siblings("td").eq(2).html();
        AddArea = AddArea.replace("平米", "");
        //alert(AddArea);
        if (POPQuoteJson.length > 0) {
            var index = -1;
            for (var i = 0; i < POPQuoteJson.length; i++) {
                if (POPQuoteJson[i].ChangeType == changeType && POPQuoteJson[i].Sheet == sheet && POPQuoteJson[i].GraphicMaterial == GraphicMaterial && POPQuoteJson[i].AddArea == AddArea) {
                    index = i;
                }
            }

            if (index > -1) {
                POPQuoteJson.splice(index,1);
                var totalPrice = 0;
                for (var j = 0; j < POPQuoteJson.length; j++) {
                    if (POPQuoteJson[j].ChangeType == changeType) {
                        var subPrice = POPQuoteJson[j].AddArea * POPQuoteJson[j].GraphicMaterialUnitPrice;
                        totalPrice += subPrice;
                    }
                }
                var parentDiv = $(this).parent().parent().parent().parent().parent();
                parentDiv.prev("span").html(totalPrice.toFixed(2));
                $(this).parent().parent().remove();
                CountTotal();
            }
        }
    });

})

function ChangeRate() {
    var rate = $.trim($("input[name$='txtAddRate']").val()) || 0;
    //alert(rate);
    //return false;
    $.ajax({
        type: "get",
        url: "handler/AddQuotation.ashx",
        data: { month: month, guidanceId: guidanceId, subjectCategory: subjectCategory, subjectId: subjectId, customerId: customerId, rate: rate },
        success: function (data) {
            $("#loadNewPriceImg").hide();
            $("#popList_labNewPOPTotalPrice").html(data);
            CountTotal();
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
    var price = $("#hfQuoteTotalPrice1").val() || 0;
    if (parseFloat(price) == 0) {
        layer.msg("报价金额为0，提交失败！");
        return false;
    }
    if (POPQuoteJson.length > 0) {
        $("#hfPOPQuoteJson").val(JSON.stringify(POPQuoteJson));
    }
    return true;
}

function showAddRateMenu() {
    $("#divAddRate").css({ left: 32 + "px", top: 24 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function hideMenu() {
    $("#divAddRate").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "txtAddRate" || event.target.id == "divAddRate" || $(event.target).parents("#divAddRate").length > 0)) {
        hideMenu();
    }

}


///pop折算报价类型
var POPQuoteModel = {
    model: function () {
        this.ChangeType = 0;
        this.Sheet = "";
        this.MaterialName = "";
        this.Price = 0;
        this.Area = 0;
        this.subPrice = 0;
    },
    AddToJson: function () {
        var json = { ChangeType: this.model.ChangeType, Sheet: this.model.Sheet, GraphicMaterial: this.model.MaterialName, GraphicMaterialUnitPrice: this.model.Price, AddArea: this.model.Area };

        POPQuoteJson.push(json);
    }
}


