
var jsonStr = "";
$(function () {
    BindSubjects();
    BindCustomerMaterial();
    $("#addEditOrder").click(function () {
        var shopId = $("#hfShopId").val() || 0;
        $.ajax({
            type: "get",
            url: "/Subjects/ADErrorCorrection/handler/ErrorCorrection.ashx?type=getOriginalOrders&guidanceId=" + itemId + "&shopId=" + shopId,
            success: function (data) {
                if (data != "") {
                    $("#editDiv").show();
                    var json = eval(data);
                    var tr = "";
                    for (var i = 0; i < json.length; i++) {
                        var state = json[i].State;

                        tr += "<tr class='tr_bai'>";
                        tr += "<td>" + (i + 1) + "</td>";
                        //tr += "<td><span name='spanShopId' data-shopid='" + json[i].ShopId + "'>" + json[i].ShopNo + "</span></td>";
                        tr += "<td><span name='spanSubjectId' data-subjectid='" + json[i].SubjectId + "'>" + json[i].SubjectName + "</span></td>";
                        tr += "<td>" + (json[i].OrderType == 1 ? "POP" : "道具") + "</td>";
                        tr += "<td><input type='text' value='" + json[i].Sheet + "' name='txtSheet' style='text-align:center;width:90%;'/></td>";
                        tr += "<td><input type='text' value='" + (json[i].LevelNum == 0 ? "" : json[i].LevelNum) + "' name='txtLevelNum' style='text-align:center;width:90%;'/></td>";
                        tr += "<td><input type='text' value='" + (json[i].GraphicWidth == 0 ? "" : json[i].GraphicWidth) + "' name='txtGraphicWidth' style='text-align:center;width:90%;'/></td>";
                        tr += "<td><input type='text' value='" + (json[i].GraphicLength == 0 ? "" : json[i].GraphicLength) + "' name='txtGraphicLength' style='text-align:center;width:90%;'/></td>";
                        tr += "<td><input type='text' value='" + json[i].Gender + "' name='txtGender' style='text-align:center;width:90%;'/></td>";
                        tr += "<td><input type='text' value='" + json[i].GraphicMaterial + "' name='txtGraphicMaterial' style='text-align:center;width:90%;'/></td>";
                        tr += "<td><input type='text' value='" + (json[i].UnitPrice == 0 ? "" : json[i].UnitPrice) + "' name='txtUnitPrice'style='text-align:center;width:90%;'/></td>";
                        tr += "<td><input type='text' value='" + json[i].Quantity + "' name='txtQuantity' style='text-align:center;width:90%;'/></td>";
                        tr += "<td><input type='text' value='" + json[i].ChooseImg + "' name='txtChooseImg' style='text-align:center;width:90%;'/></td>";
                        tr += "<td><input type='text' value='" + json[i].PositionDescription + "' name='txtPositionDescription' style='text-align:center;width:90%;'/></td>";
                        if (state == 0) {
                            tr += "<td><span name='spanState' data-state='" + state + "'>原订单</span></td>";
                            tr += "<td><input type='text' value='" + json[i].EditRemark + "' name='txtEditRemark' style='text-align:center;width:90%;'/></td>";
                            tr += "<td><span name='spanDelete' data-orderid='" + json[i].OrderId + "' style='color:red;cursor:pointer;'>删除</span></td>";
                        }
                        else if (state == 1) {
                            tr += "<td><span name='spanState' data-state='" + state + "' style='color:blue;'>新增</span></td>";
                            tr += "<td><input type='text' value='" + json[i].EditRemark + "' name='txtEditRemark' style='text-align:center;width:90%;'/></td>";
                            tr += "<td><span name='spanDelete' data-orderid='" + json[i].OrderId + "' style='color:red;cursor:pointer;'>删除</span></td>";
                        }
                        else if (state == 2) {
                            tr += "<td><span name='spanState' data-state='" + state + "' style='color:red;'>删除</span></td>";
                            tr += "<td><input type='text' value='" + json[i].EditRemark + "' name='txtEditRemark' style='text-align:center;width:90%;'/></td>";
                            tr += "<td><span name='spanDelete' data-orderid='" + json[i].OrderId + "' style='color:blue;cursor:pointer;'>恢复</span></td>";
                        }
                        
                        tr += "</tr>";
                    }
                    $("#orderContent").html(tr);
                }
            }
        })
    })
    $("#btnSubmit").click(function () {
        if (CheckVal()) {
            $.ajax({
                type: "post",
                url: "/Subjects/ADErrorCorrection/handler/ErrorCorrection.ashx",
                data: { type: "updateOrder", jsonStr: escape(jsonStr) },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        alert("提交成功！");
                    }
                    else {
                        alert(data);
                    }
                }
            })
        }
    })

    //选择材质
    //    $("#addContent").delegate("span[name='btnSelectMaterial']", "click", function () {
    //        SelectMaterialSpan = $(this);
    //        $(this).siblings("div").show();
    //    })

    $("#txtaddMaterial").click(function () {
        $(this).siblings("div").show();
    })

    //材质只能单选
    $("#addContent").delegate("input[name='cbMaterial']", "change", function () {
        if (this.checked) {
            $(".customerMaterials").find("input[name='cbMaterial']").not($(this)).attr("checked", false);
        }
    })
    //提交选择好的材质
    $("#addContent").delegate("span[name='btnSubmitMaterial']", "click", function () {

        var div = $(this).parent().parent().parent().parent();
        var mName = "";
        var price = "";
        $(div).find(".customerMaterials").find("input[name='cbMaterial']:checked").each(function () {

            mName = $(this).siblings("span").html();
            price = $(this).siblings("span").data("price");
        })
        if (mName.length > 0) {
            $("#txtaddMaterial").val(mName);
            $("#txtaddPrice").val(price);

        }
        $(".divMaterialList").hide();
    })

    $("#spanCloseMaterial").click(function () {
        $(".divMaterialList").hide();
    })

    //    $("#txtaddShopNo").blur(function () {
    //        var shopNo = $.trim($("#txtaddShopNo").val());
    //        if (shopNo != "") {
    //            $.ajax({
    //                type: "get",
    //                url: "/Subjects/ADErrorCorrection/handler/ErrorCorrection.ashx?type=getShopId&shopNo=" + shopNo,
    //                success: function (data) {
    //                    if (data != "error") {
    //                        $("#hfaddShopId").val(data);
    //                    }
    //                }
    //            });
    //        }
    //    })

    //新增订单
    $("#btnAddContent").click(function () {
        CheckAddContentVal();
    })

    $("#orderContent").delegate("span[name='spanDelete']", "click", function () {
        var stateSpan = $(this).parent("td").prev().find("span[name='spanState']");
        //alert($(this).parent("td").prev().html());
        var state = stateSpan.attr("data-state") || 0;
        //alert(state);
        if (state == 0) {
            //如果是原订单，把状态改成2 （已删除）
            stateSpan.attr("data-state", '2');
            stateSpan.html("删除");
            stateSpan.css("color", "red");
            $(this).html("恢复").css("color", "blue");
        }
        else if (state == 1) {
            //如果是新增，直接删除整行
            $(this).parent().parent().remove();

        }
        else if (state == 2) {
            //恢复
            stateSpan.attr("data-state", '0');
            stateSpan.html("原订单");
            stateSpan.removeAttr("style");
            $(this).html("删除").css("color", "red");
        }
    })

    $("#btnClearData").click(function () {
        if (confirm("确定清除吗？")) {
            var shopId = $("#hfShopId").val() || 0;
            $.ajax({
                type: "get",
                url: "/Subjects/ADErrorCorrection/handler/ErrorCorrection.ashx",
                data: { type: "clearData", guidanceId: itemId, shopId: shopId },
                success: function (data) {
                    if (data == "ok") {
                        $("#orderContent").html("");
                    }
                    else {
                        alert("操作失败！");
                    }
                }
            })
        }
    })

})


function CheckVal() {
    jsonStr = "";
    if ($("#orderContent tr").length == 0) {
        alert("没有可提交的数据");
        return false;
    }
    var flag = true;
    $("#orderContent tr").each(function () {
        var td = $(this).find("td");
        //var shopNo = td.eq(1).html();
        var subjectId = $(this).find("span[name='spanSubjectId']").data("subjectid") || 0;
        var orderType = td.eq(3).html();
        var sheet = $(this).find("input[name='txtSheet']").val();
        var levelNum = $(this).find("input[name='txtLevelNum']").val()||0;
        var width = $(this).find("input[name='txtGraphicWidth']").val()||0;
        var length = $(this).find("input[name='txtGraphicLength']").val()||0;
        var gender = $(this).find("input[name='txtGender']").val();
        var material = $(this).find("input[name='txtGraphicMaterial']").val();
        var unitPrice = $(this).find("input[name='txtUnitPrice']").val();
        var quantity = $(this).find("input[name='txtQuantity']").val()||0;
        var chooseImg = $(this).find("input[name='txtChooseImg']").val();
        var positionDescription = $(this).find("input[name='txtPositionDescription']").val();
        var state = $(this).find("span[name='spanState']").data("state") || 0;
        var orderId = $(this).find("span[name='spanDelete']").data("orderid") || 0;
        var shopId = $("#hfShopId").val() || 0;
        var editRemark = $(this).find("input[name='txtEditRemark']").val();
        if ($.trim(sheet) == "") {
            alert("请填写位置");
            flag= false;
        }
        if ($.trim(levelNum) != "" && isNaN(levelNum)) {
            alert("级别填写不正确");
            flag = false;
        }
        if ($.trim(width) != "" && isNaN(width)) {
            alert("宽填写不正确");
            flag = false;
        }
        if ($.trim(length) != "" && isNaN(length)) {
            alert("高填写不正确");
            flag = false;
        }
        if ($.trim(gender) == "") {
            alert("请填写性别");
            flag = false;
        }
        if ($.trim(quantity) == "") {
            alert("请填写数量");
            flag = false;
        }
        else if (isNaN(quantity)) {
            alert("数量填写不正确");
            flag = false;
        }
        orderType = orderType == "POP" ? 1 : 2;
        jsonStr += '{"OrderId":' + orderId + ',"GuidanceId":' + itemId + ',"SubjectId":' + subjectId + ',"OrderType":' + orderType + ',"ShopId":'+shopId+',"Sheet":"' + sheet + '","Gender":"' + gender + '","LevelNum":' + levelNum + ',"Quantity":' + quantity + ',"GraphicWidth":' + width + ',"GraphicLength":' + length + ',"GraphicMaterial":"' + material + '","UnitPrice":' + unitPrice + ',"ChooseImg":"' + chooseImg + '","Remark":"' + positionDescription + '","State":' + state + ',"EditRemark":"'+editRemark+'"},';
    })
    if (jsonStr != "") {
        jsonStr = jsonStr.substring(0, jsonStr.length - 1);
        jsonStr = "[" + jsonStr + "]";
    }
    
    return flag;
}

function CheckAddContentVal() {
    //var shopNo = $.trim($("#txtaddShopNo").val());
    //var shopId = $("#hfaddShopId").val()||0;
    var subjectId = $("#seleaddSubjectName").val();
    var subjectName = $("#seleaddSubjectName option:selected").text();
    var orderType = $("#seleAddOrderType").val();
    var sheet = $("#seleaddSheet").val();
    var levelNum = $.trim($("#txtaddLevelNum").val());
    var width = $.trim($("#txtaddWidth").val());
    var length = $.trim($("#txtaddLength").val());
    var gender = $("#seleaddGender").val();
    var material = $("#txtaddMaterial").val();
    var price = $.trim($("#txtaddPrice").val());
    var num = $.trim($("#txtaddNum").val());
    var chooseImg = $.trim($("#txtaddChooseImg").val());
    var remark = $.trim($("#txtaddRemark").val());
    var editRemark = $.trim($("#txtaddEditRemark").val());
    //var flag = true;
//    if (shopNo == "") {
//        alert("请填写店铺编号");
//        return false;
//    }
//    else if (shopId == 0) {
//        alert("该店铺编号不存在");
//        return false;
//    }
    if (subjectId == 0) {
        alert("请选择项目");
        return false;
    }
    if (price != "" && isNaN(price)) {
        alert("单价填写不正确");
        return false;
    }
    if (num == "") {
        alert("请填写数量");
        return false;
    }
    else if (isNaN(num)) {
        alert("数量填写不正确");
        return false;
    }
    if (remark == "") {
        alert("请填写POP描述");
        return false;
    }
    var rowNum = $("#orderContent tr").length;
    var tr = "";
    tr += "<tr class='tr_bai'>";
    tr += "<td>" + (rowNum + 1) + "</td>";
    //tr += "<td><span name='spanShopId' data-shopid='" + shopId + "'>" + shopNo + "</span></td>";
    tr += "<td><span name='spanSubjectId' data-subjectid='" + subjectId + "'>" + subjectName + "</span></td>";
    tr += "<td>" + orderType + "</td>";
    tr += "<td><input type='text' value='" + sheet + "' name='txtSheet' style='text-align:center;width:90%;'/></td>";
    tr += "<td><input type='text' value='" + levelNum + "' name='txtLevelNum' style='text-align:center;width:90%;'/></td>";
    tr += "<td><input type='text' value='" + width + "' name='txtGraphicWidth' style='text-align:center;width:90%;'/></td>";
    tr += "<td><input type='text' value='" + length + "' name='txtGraphicLength' style='text-align:center;width:90%;'/></td>";
    tr += "<td><input type='text' value='" + gender + "' name='txtGender' style='text-align:center;width:90%;'/></td>";
    tr += "<td><input type='text' value='" + material + "' name='txtGraphicMaterial' style='text-align:center;width:90%;'/></td>";
    tr += "<td><input type='text' value='" + price + "' name='txtUnitPrice'style='text-align:center;width:90%;'/></td>";
    tr += "<td><input type='text' value='" + num + "' name='txtQuantity' style='text-align:center;width:90%;'/></td>";
    tr += "<td><input type='text' value='" + chooseImg + "' name='txtChooseImg' style='text-align:center;width:90%;'/></td>";
    tr += "<td><input type='text' value='" + remark + "' name='txtPositionDescription' style='text-align:center;width:90%;'/></td>";
    tr += "<td><span name='spanState' data-state='1' style='color:blue;'>新增</span></td>";
    tr += "<td><input type='text' value='" + editRemark + "' name='txtEditRemark' style='text-align:center;width:90%;'/></td>";
    tr += "<td><span name='spanDelete' data-orderid='0' style='color:red;cursor:pointer;'>删除</span></td>";
    
    tr += "</tr>";
    
    $("#orderContent").append(tr);
    ClearVal();
    
}

//获取客户材质（报价）
function BindCustomerMaterial() {
    $.ajax({
        type: "get",

        url: "../Handler/SplitOrder1.ashx?type=getCustomerMaterial&customerId=" + customerId,
        cache: false,
        success: function (data) {
            //alert(data);
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style=' height:20px;'><input name='cbMaterial' type='checkbox' value='" + json[i].MaterialName + "'/>&nbsp;<span data-price='" + json[i].Price + "'>" + json[i].MaterialName + "</span></div>";
                }

                $(".customerMaterials").html(div);
            }
            else
                $(".customerMaterials").html("");
        }
    })
}

function BindSubjects() {
    document.getElementById("seleaddSubjectName").length = 1;
    $.ajax({
        type: "get",
        url: "/Subjects/ADErrorCorrection/handler/ErrorCorrection.ashx",
        data: { type: "getSubjects", guidanceId: itemId },
        cache: false,
        success: function (data) {
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var option = "<option value='" + json[i].Id + "'>" + json[i].SubjectName + "</option>";
                    $("#seleaddSubjectName").append(option);
                }
            }

        }
    })
}


function ClearVal() {
    $("#seleaddSubjectName").val("0");
    $("#addContent input:not(#btnAddContent)").val("");
    $("#txtaddNum").val("1");
 }



