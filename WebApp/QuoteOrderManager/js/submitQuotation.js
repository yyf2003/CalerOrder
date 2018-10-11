

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

    $("input[name='btnAddPOPQuote']").click(function () {
        var sheet = $(this).siblings("select[name*='ddlSheet']").val() || "";
        var material = $(this).siblings("select[name*='ddlMaterial']").val();
        var area = $(this).siblings("input[name*='txtPOPArea']").val() || 0;
        var priceType = $(this).parent().parent().siblings("input[name*='hfChangeType']").val();
        $(this).siblings("input[name*='txtPOPArea']").val("");
        //alert(priceType);
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

        displayPOPQouteList($(this));
    })

    //删除pop折算报价
    $("#otherInstallPriceTable, #otherOrderPriceTable").delegate("span[name='spanDeletePOPQuote']", "click", function () {

        var priceType = $(this).data("pricetype");
        var sheet = $(this).parent().siblings("td").eq(0).html();
        var GraphicMaterial = $(this).parent().siblings("td").eq(1).html();
        var AddArea = $(this).parent().siblings("td").eq(2).html();
        AddArea = AddArea.replace("平米", "");
       
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

            }
        }
    });
})

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

function Check() {
    
    var quoteSubject = $("#ddlQuoteSubject").val();
    if (quoteSubject == "0") {
        layer.msg("请选择报价项目名称");
        return false;
    }
    if (POPQuoteJson.length > 0) {
        $("#hfPOPQuoteJson").val(JSON.stringify(POPQuoteJson));
    }
    return true;
}