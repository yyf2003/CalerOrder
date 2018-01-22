


var orderId = 0;
var shopId = 0;
var popSheetListStr = "";
var guidanceList;
var isPickDate = 0; //是否选择完时间
var addSubjectId = 0;
$(function () {

    Order.getList();

    $("#btnAdd").click(function () {
        POP.getSubjectGuidanceList();
        $("#txtShopNo").val("");
        $("#getPOPMsg").html("");
        $("#tbodyPOPData").html("").hide();
        $("#tbodyPOPEmpty").show();
        layer.open({
            type: 1,
            time: 0,
            title: '添加订单明细',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '90%'],
            content: $("#popDetailDiv"),
            id: 'popLayer',
            btn: ['提 交'],
            btnAlign: 'c',
            yes: function () {
                POP.submit();
            },
            cancel: function (index) {
                layer.closeAll();
                $("#popDetailDiv").hide();

            }

        });
    })

    $("#btnGetPOP").click(function () {
        var shopNo = $.trim($("#txtShopNo").val());
        if (shopNo != "") {
            $(this).next("img").show();
            POP.getList(shopNo);
        }

    })

    $("#btnImport").click(function () {
        layer.open({
            type: 2,
            time: 0,
            title: '批量导入',
            skin: 'layui-layer-rim', //加上边框
            area: ['800px', '400px'],
            content: 'ImportOrderDetail.aspx?subjectId=' + subjectId,
            cancel: function () {
                if ($("#hfIsChange").val() == "1") {
                    $("#hfIsChange").val("");
                    Order.getList();
                }
            }
        });
    })

    //POP全选
    $("#cbAllOrder").click(function () {
        var checked = this.checked;
        $("input[name='cbOneOrder']").each(function () {
            this.checked = checked;
        })
    })

    $("#tbodyOrderData").delegate("input[name='cbOneOrder']", "change", function () {

        if (!this.checked) {
            $("#cbAllOrder").prop("checked", false);
        }
        else {
            var checked = $("input[name='cbOneOrder']:checked").length == $("input[name='cbOneOrder']").length;
            $("#cbAllOrder").prop("checked", checked);
        }
    })

    $("#txtAddSheet").on("click", function () {
        showAddSheetMenu();
    })

    $("#ddlAddSheetMenu").delegate("li", "click", function () {

        var sheetVal = $(this).html();
        $("#txtAddSheet").val(sheetVal);
        $("#divAddSheetMenu").hide();
    })

    $("#txtSheetEdit").on("click", function () {
        showEditSheetMenu();
    })

    $("#ddlEditSheetMenu").delegate("li", "click", function () {
        $("#txtSheetEdit").val($(this).html());
        $("#divEditSheetMenu").fadeOut("fast");

    })

    //修改
    $("#btnEdit").click(function () {
        orderId = 0;
        var selectCount0 = $("input[name='cbOneOrder']:checked").length;
        if (selectCount0 == 0) {
            layer.msg('请选择要修改的行');
            return false;
        }
        if (selectCount0 > 1) {
            layer.msg('一次只能修改一条');
            return false;
        }
        $("input[name='cbOneOrder']:checked").each(function () {
            orderId = $(this).val();
        })
        if (orderId > 0) {
            Order.getOrder();

        }
    })

    //删除
    $("#btnDelete").click(function () {
        var ids0 = "";
        var selectCount0 = $("input[name='cbOneOrder']:checked").length;
        if (selectCount0 == 0) {
            layer.msg('请选择要删除的行');
            return false;
        }
        $("input[name='cbOneOrder']:checked").each(function () {
            ids0 += $(this).val() + ",";
        })
        if (ids0.length > 0) {
            $.ajax({
                type: "get",
                url: "handler/AddOrderDetail.ashx",
                data: { type: 'deleteOrder', ids: ids0 },
                success: function (data) {
                    if (data == "ok") {
                        $("#cbAllOrder").attr("checked", false);
                        layer.msg('删除成功');
                        Order.getList();
                    }
                    else {
                        //alert(data);
                        layer.open({
                            type: 1,
                            skin: 'layui-layer-demo', //样式类名
                            closeBtn: 0, //不显示关闭按钮
                            anim: 2,
                            shadeClose: true, //开启遮罩关闭
                            content: data
                        });
                    }
                }
            })
        }

    });

    $("body").keydown(function (event) {
        if (event.keyCode == 13) {
            return false;
        }
    });

    //选择活动月
    $("#tbodyPOPData").delegate("input[name='txtGuidanceMonth']", "blur", function () {
        if (isPickDate == 1) {
            isPickDate = 0;
            var seleGuidance = $(this).next("select");
            var val = $(this).val();
            $(seleGuidance).find("option:not(:first)").remove();
            $.ajax({
                type: "get",
                url: "/Subjects/RegionSubject/handler/AddNewShopOrder.ashx",
                data: { type: "getGuidanceList", month: val },
                success: function (data) {
                    if (data != "") {
                        var json = eval(data);
                        if (json.length > 0) {
                            for (var i = 0; i < json.length; i++) {
                                var option = "<option value='" + json[i].Id + "'>" + json[i].GuidanceName + "</option>";
                                $(seleGuidance).append(option);
                            }
                        }
                    }
                }
            })

        }
    })

    $("#tbodyPOPData").delegate("select[name='seleGuidance']", "change", function () {
        var guidanceId = $(this).val();
        var seleSubject = $(this).next("select");
        $(seleSubject).find("option:not(:first)").remove();
        POP.getSubjectList(guidanceId, seleSubject);
    })

    $("#seleAddGuidance").change(function () {
        var guidanceId = $(this).val();

        var seleSubject = $("#seleAddSubject");
        $(seleSubject).find("option:not(:first)").remove();
        POP.getSubjectList(guidanceId, seleSubject);
    })


    //添加其他订单
    $("#spanAddPOP").click(function () {
        if (shopId > 0) {
            var shopNo = $.trim($("#txtShopNo").val());
            var orderType = $("#seleAddOrderType").val();
            var sheet = $.trim($("#txtAddSheet").val());
            var gender = $("#seleAddGender").val();
            var quantity = $.trim($("#txtAddQuantity").val());
            var width = $.trim($("#txtAddWidth").val());
            var length = $.trim($("#txtAddLength").val());
            var material = $("#seleAddMaterial").val();
            var positionDescription = $.trim($("#txtAddPositionDescription").val());
            var chooseImg = $.trim($("#txtAddChooseImg").val());
            var remark = $.trim($("#txtAddRemark").val());
            var chooseImgGuidanceId = $("#seleAddGuidance").val();
            addSubjectId = $("#seleAddSubject").val();
            if (sheet == "") {
                layer.msg('请填写POP位置');
                return false;
            }
            if (quantity == "") {
                layer.msg('请填写数量');
                return false;
            }
            else if (isNaN(quantity)) {
                layer.msg('数量必须是数字');
                return false;
            }
            else if (parseFloat(quantity) == 0) {
                layer.msg('数量必须大于0');
                return false;
            }
            if (width == "") {
                layer.msg('请填写POP宽');
                return false;
            }
            else if (isNaN(width)) {
                layer.msg('POP宽必须是数字');
                return false;
            }
            else if (parseFloat(width) == 0) {
                layer.msg('POP宽必须大于0');
                return false;
            }
            if (length == "") {
                layer.msg('请填写POP高');
                return false;
            }
            else if (isNaN(length)) {
                layer.msg('POP高必须是数字');
                return false;
            }
            else if (parseFloat(length) == 0) {
                layer.msg('POP高必须大于0');
                return false;
            }

            if (material == "") {
                layer.msg('请选择材质');
                return false;
            }
            var tr = "<tr>";
            tr += "<td><input type='checkbox' name='cbOnePOP' data-isvalid='1' data-ordertype='" + orderType + "' checked='checked' value='0'></td>";
            tr += "<td>" + CreateOrderTypeSelect(orderType) + "</td>";
            tr += "<td style='text-align:left; padding-left :0px;'>" + CreateGuidanceSelect(chooseImgGuidanceId);
            tr += "" + CreateSujectSelect() + "</td>";
            tr += "<td><input type='text' name='txtChooseImg' value='" + chooseImg + "' maxlength='30' style='width:80px;'/></td>";
            tr += "<td>" + sheet + "</td>";
            tr += "<td></td>";
            tr += "<td>" + CreateGenderSelect(gender) + "</td>";
            tr += "<td><input type='text' name='txtQuantity' value='" + quantity + "' maxlength='2' style='width:80px;'/></td></td>";
            tr += "<td>" + width + "</td>";
            tr += "<td>" + length + "</td>";
            tr += "<td>" + material + "</td>";
            tr += "<td>" + positionDescription + "</td>";
            tr += "<td></td>";
            tr += "<td><input type='text' name='txtPOPRemark' value='" + remark + "' maxlength='30' style='width:100px;'/></td>";

            tr += "<td><span name='spanDeletePOP' style='color:red;cursor:pointer;'>删除</span></td>";
            tr += "</tr>";

            $("#tbodyPOPData").append(tr);
            $("#addPOPTable input").val("");
            //$("#addPOPTable select").val("");
            $("#txtAddQuantity").val("1");
            if (chooseImgGuidanceId > 0) {
                $("#tbodyPOPData").find("select[name='seleGuidance']:last").change();
            }
        }
        else {
            layer.msg('请先输入店铺编号');
        }
    })

    //添加费用
    $("#spanAddPrice").click(function () {
        if (shopId > 0) {
            var orderType = $("#selePriceType").val();

            var priceNum = $.trim($("#txtAddPriceNum").val());
            var payPriceNum = $.trim($("#txtAddPayPriceNum").val())||0;
            if (priceNum == "") {
                layer.msg('请填写应收费用');
                return false;
            }
            else if (isNaN(priceNum)) {
                layer.msg('应收费用必须是数字');
                return false;
            }
            if (payPriceNum != "" && isNaN(payPriceNum)) {
                layer.msg('应付费用必须是数字');
                return false;
            }
            var priceRemark = $.trim($("#txtAddPriceRemark").val());

            var tr = "<tr>";
            tr += "<td><input type='checkbox' name='cbOnePOP' data-isvalid='1' data-ordertype='" + orderType + "' checked='checked' value='0'></td>";
            tr += "<td>" + CreateOrderTypeSelect(orderType) + "</td>";
            tr += "<td></td>";
            tr += "<td></td>";
            tr += "<td></td>";
            tr += "<td></td>";
            tr += "<td></td>";
            tr += "<td></td>";
            tr += "<td></td>";
            tr += "<td></td>";
            tr += "<td></td>";
            tr += "<td></td>";
            tr += "<td>" + priceNum + "</td>";
            tr += "<td>" + payPriceNum + "</td>";
            tr += "<td><input type='text' name='txtPOPRemark' value='" + priceRemark + "' maxlength='50' style='width:100px;'/></td>";

            tr += "<td><span name='spanDeletePOP'  style='color:red;cursor:pointer;'>删除</span></td>";
            tr += "</tr>";

            $("#tbodyPOPData").append(tr);
            $("#addPriceTable input").val("");

        }
        else {
            layer.msg('请先输入店铺编号');
        }
    })

    //删除
    $("#tbodyPOPData").delegate("span[name='spanDeletePOP']", "click", function () {
        $(this).parent("td").parent("tr").remove();
    })
})

var Order = {
    getList: function () {
        var loadIndex = layer.load(0, { shade: false });
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'getList', subjectId: subjectId },
            success: function (data) {

                layer.close(loadIndex);
                if (data != "") {

                    var json = eval(data);

                    if (json.length > 0) {
                        var tr = "";
                        for (var i in json) {
                            tr += "<tr>";

                            if (json[i].IsApprove == 1) {
                                tr += "<td><input type='checkbox' name='0' disabled='disabled'></td>";
                                tr += "<td>" + (parseInt(i) + 1) + "</td>";
                                tr += "<td style='color:green'>审批通过</td>";
                            }
                            else {
                                tr += "<td><input type='checkbox' name='cbOneOrder' value='" + json[i].Id + "'></td>";
                                if (json[i].IsApprove == 2) {
                                    tr += "<td>" + (parseInt(i) + 1) + "</td>";
                                    tr += "<td style='color:red'>审批不通过</td>";
                                }
                                else {
                                    tr += "<td>" + (parseInt(i) + 1) + "</td>";
                                    tr += "<td>未审批</td>";
                                }

                            }
                            tr += "<td>" + json[i].OrderType + "</td>";

                            tr += "<td>" + json[i].ShopNo + "</td>";
                            tr += "<td>" + json[i].ShopName + "</td>";
                            tr += "<td>" + json[i].RegionName + "</td>";
                            tr += "<td>" + json[i].ProvinceName + "</td>";
                            tr += "<td>" + json[i].Price + "</td>";
                            tr += "<td>" + json[i].PayPrice + "</td>";
                            tr += "<td>" + json[i].Sheet + "</td>";
                            tr += "<td>" + json[i].Gender + "</td>";
                            tr += "<td>" + json[i].Quantity + "</td>";
                            tr += "<td>" + json[i].GraphicWidth + "</td>";
                            tr += "<td>" + json[i].GraphicLength + "</td>";
                            tr += "<td>" + json[i].GraphicMaterial + "</td>";
                            tr += "<td>" + json[i].PositionDescription + "</td>";
                            tr += "<td>" + json[i].ChooseImg + "</td>";
                            tr += "<td>" + json[i].Remark + "</td>";
                            tr += "</tr>";
                        }

                        $("#tbodyOrderData").html(tr).show();
                        $("#tbodyOrderEmpty").hide();
                    }
                }
                else {
                    $("#tbodyOrderData").html("").hide();
                    $("#tbodyOrderEmpty").show();
                }

            }
        });

    },
    getCustomerMaterialList: function () {
        document.getElementById("seleAddMaterial").length = 1;
        $.ajax({
            type: "get",
            url: "/Subjects/RegionSubject/handler/AddOrderDetail.ashx",
            data: { type: 'getCustomerMaterail', subjectId: subjectId },
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var option = "<option value='" + json[i].MaterialName + "'>" + json[i].MaterialName + "</option>";
                        $("#seleAddMaterial").append(option);
                    }
                }
            }
        });
    },
    getOrder: function () {
        //document.getElementById("seleSubjectEdit").length = 1;
        ClearEditVal();
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'getOrder', id: orderId },
            success: function (data) {
                if (data != "") {

                    var orderJson = eval(data);
                    if (orderJson.length > 0) {
                        var orderType = orderJson[0].OrderTypeName;
                        if (orderType.indexOf("费") == -1) {
                            $("#labOrderTypeEdit").html(orderType);
                            $("#labShopNoEdit").html(orderJson[0].ShopNo);
                            $("#labShopNameEdit").html(orderJson[0].ShopName);

                            $("#labGraphicNoEdit").html(orderJson[0].GraphicNo);
                            $("#txtSheetEdit").val(orderJson[0].Sheet);
                            $("#ddlMaterialSupportEdit").val(orderJson[0].MaterialSupport);
                            $("#txtPOSScaleEdit").val(orderJson[0].POSScale);
                            $("#seleEditGender").val(orderJson[0].Gender);
                            $("#txtQuantityEdit").val(orderJson[0].Quantity);
                            if (orderJson[0].GraphicWidth > 0) {
                                $("#txtWidthEdit").val(orderJson[0].GraphicWidth);
                            }
                            if (orderJson[0].GraphicLength > 0) {
                                $("#txtLengthEdit").val(orderJson[0].GraphicLength);
                            }

                            $("#labMaterialEdit").html(orderJson[0].GraphicMaterial);
                            $("#txtPositionDescriptionEdit").val(orderJson[0].PositionDescription);
                            $("#txtChooseImgEdit").val(orderJson[0].ChooseImg);
                            $("#txtRemarkEdit").val(orderJson[0].Remark);

                            if (popSheetListStr == "") {
                                POP.getPOPSheet();
                            }
                            if (orderJson[0].GraphicNo != "" || orderJson[0].OrderTypeName.indexOf("费") != -1) {
                                $("#txtSheetEdit").attr("disabled", "disabled");
                                $("#txtWidthEdit").attr("disabled", "disabled");
                                $("#txtLengthEdit").attr("disabled", "disabled");
                            }
                            layerIndex = layer.open({
                                type: 1,
                                time: 0,
                                title: '编辑POP订单',
                                skin: 'layui-layer-rim', //加上边框
                                area: ['854px', '400px'],
                                content: $("#editDiv"),
                                id: 'popLayer1',
                                btn: ['提 交'],
                                btnAlign: 'c',
                                yes: function () {
                                    Order.submitEdit();
                                },
                                cancel: function (index) {
                                    layer.close(index);
                                    $("#editDiv").hide();

                                }

                            });
                        }
                        else {
                            $("#seleAddPriceType").val(orderType);
                            $("#txtOrderPrice").val(orderJson[0].Price);
                            $("#txtPayOrderPrice").val(orderJson[0].PayPrice);
                            layerIndex = layer.open({
                                type: 1,
                                time: 0,
                                title: '编辑费用订单',
                                skin: 'layui-layer-rim', //加上边框
                                area: ['854px', '250px'],
                                content: $("#editPriceDiv"),
                                id: 'popLayer2',
                                btn: ['提 交'],
                                btnAlign: 'c',
                                yes: function () {
                                    Order.submitPriceEdit();
                                },
                                cancel: function (index) {
                                    layer.close(index);
                                    $("#editPriceDiv").hide();

                                }

                            });
                        }
                    }
                }

            }
        })
    },
    submitEdit: function () {
        var orderType = $("#labOrderTypeEdit").text();
        var sheet = $.trim($("#txtSheetEdit").val());
        var posScale = $.trim($("#txtPOSScaleEdit").val());
        var materialSupport = $("#ddlMaterialSupportEdit").val();
        var gender = $("#seleEditGender").val();
        var quantity = $.trim($("#txtQuantityEdit").val());
        var width = $.trim($("#txtWidthEdit").val());
        var length = $.trim($("#txtLengthEdit").val());
        var positionDescription = $.trim($("#txtPositionDescriptionEdit").val());
        var chooseImg = $.trim($("#txtChooseImgEdit").val());
        var remark = $.trim($("#txtRemarkEdit").val());
        if (sheet == "") {
            layer.msg('请填写POP位置');
            return false;
        }
        if (quantity == "") {
            layer.msg('请填写数量');
            return false;
        }
        else if (isNaN(quantity)) {
            layer.msg('数量必须是数字');
            return false;
        }
        else if (parseInt(quantity) == 0) {
            layer.msg('数量必须大于0');
            return false;
        }
        if (width == "") {
            layer.msg('请填写POP宽');
            return false;
        }
        else if (isNaN(width)) {
            layer.msg('POP宽必须是数字');
            return false;
        }
        else if (parseFloat(width) == 0) {
            layer.msg('POP宽必须大于0');
            return false;
        }
        if (length == "") {
            layer.msg('请填写POP高');
            return false;
        }
        else if (isNaN(length)) {
            layer.msg('POP高必须是数字');
            return false;
        }
        else if (parseFloat(length) == 0) {
            layer.msg('POP高必须大于0');
            return false;
        }
        var orderJson = '{"Id":' + orderId + ',"MaterialSupport":"' + materialSupport + '","POSScale":"' + posScale + '","Sheet":"' + sheet + '","Gender":"' + gender + '","Quantity":' + parseInt(quantity) + ',"GraphicWidth":' + width + ',"GraphicLength":' + length + ',"PositionDescription":"' + positionDescription + '","ChooseImg":"' + chooseImg + '","Remark":"' + remark + '"}';

        $.ajax({
            type: "post",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'editOrder', jsonStr: urlCodeStr(orderJson) },
            success: function (data) {
                if (data != "ok") {
                    layer.confirm(data);
                }
                else {
                    layer.closeAll();
                    $("#editDiv").hide();
                    Order.getList();
                }
            }
        })

    },
    submitPriceEdit: function () {
        var orderType = $("#seleAddPriceType").val();
        var orderPrice = $.trim($("#txtOrderPrice").val());
        var payOrderPrice = $.trim($("#txtPayOrderPrice").val())||0;
        var remark = $.trim($("#txtPriceRemarkEdit").val());
        if (orderPrice == "") {
            layer.msg('请填写应收金额');
            return false;
        }
        else if (isNaN(orderPrice)) {
            layer.msg('应收金额必须是数字');
            return false;
        }
        else if (parseFloat(orderPrice) == 0) {
            layer.msg('应收金额必须大于0');
            return false;
        }
        if (payOrderPrice != "" && isNaN(payOrderPrice)) {
            layer.msg('应付金额必须是数字');
            return false;
        }
        var orderJson = '{"Id":' + orderId + ',"OrderTypeName":"' + orderType + '","Price":' + orderPrice + ',"PayPrice":' + payOrderPrice + ',"Remark":"' + remark + '"}';

        $.ajax({
            type: "post",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'editOrder', jsonStr: urlCodeStr(orderJson) },
            success: function (data) {
                if (data != "ok") {
                    layer.confirm(data);
                }
                else {
                    layer.closeAll();
                    $("#editPriceDiv").hide();
                    Order.getList();
                }
            }
        })
    }
}

var POP = {
    getList: function (shopNo) {
        shopId = 0;
        $("#labShopName").val("");
        $("#labFormat").val("");
        $("#getPOPMsg").html("").hide();
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'getPOPList', subjectId: subjectId, shopNo: shopNo },
            success: function (data) {

                $("#getPOPLoading").hide();
                if (data.indexOf("error") != -1) {
                    $("#getPOPMsg").html(data.split('|')[1]).show();
                    $("#tbodyPOPData").html("").hide();
                    $("#tbodyPOPEmpty").show();
                }
                else {
                    var json = eval(data);
                    if (json.length > 0) {
                        var tr = "";
                        shopId = json[0].ShopId;
                        $("#labShopName").text(json[0].ShopName);
                        $("#labFormat").text(json[0].Format);
                        for (var i in json) {
                            tr += "<tr>";
                            tr += "<td><input type='checkbox' name='cbOnePOP' data-isvalid='" + json[i].IsValid + "' data-ordertype='" + json[i].OrderTypeName + "' value='" + json[i].Id + "'></td>";
                            tr += "<td>" + CreateOrderTypeSelect(json[i].OrderTypeName) + "</td>";
                            tr += "<td style='padding-left:0px;'>" + CreateGuidanceSelect();
                            tr += "" + CreateSujectSelect() + "</td>";
                            tr += "<td><input type='text' name='txtChooseImgRemark' value='' maxlength='30' style='width:100px;'/></td>";
                            tr += "<td>" + json[i].Sheet + "</td>";
                            tr += "<td>" + json[i].GraphicNo + "</td>";
                            tr += "<td>" + CreateGenderSelect(json[i].Gender) + "</td>";
                            tr += "<td><input type='text' name='txtQuantity' value='" + json[i].Quantity + "' maxlength='2' style='width:60px;'/></td>";
                            tr += "<td>" + json[i].GraphicWidth + "</td>";
                            tr += "<td>" + json[i].GraphicLength + "</td>";
                            tr += "<td>" + json[i].GraphicMaterial + "</td>";
                            tr += "<td>" + json[i].PositionDescription + "</td>";
                            tr += "<td></td>";
                            tr += "<td></td>";
                            tr += "<td><input type='text' name='txtPOPRemark' value='" + json[i].Remark + "' maxlength='30' style='width:100px;'/></td>";
                            tr += "<td></td>";
                            tr += "</tr>";
                        }
                        $("#tbodyPOPData").html(tr).show();
                        $("#tbodyPOPEmpty").hide();
                        Order.getCustomerMaterialList();
                        if (popSheetListStr == "") {
                            POP.getPOPSheet();
                        }
                    }
                    else {
                        $("#tbodyPOPData").html("").hide();
                        $("#tbodyPOPEmpty").show();
                    }
                }

            }
        })
    },
    getPOPSheet: function () {
        popSheetListStr = "";
        $("#ddlAddSheetMenu").html("");
        $("#ddlEditSheetMenu").html("");
        $.ajax({
            type: "get",
            url: "/Subjects/RegionSubject/handler/AddOrderDetail.ashx",
            data: { type: 'getSheetList' },
            success: function (data) {

                if (data != "") {
                    popSheetListStr = data;
                    var json = eval(data);
                    if (json.length > 0) {
                        for (var k = 0; k < json.length; k++) {
                            var li = "<li>" + json[k].SheetName + "</li>";
                            $("#ddlAddSheetMenu").append(li);
                            $("#ddlEditSheetMenu").append(li);
                        }
                    }
                }
            }
        });
    },
    getSubjectGuidanceList: function () {
        document.getElementById("seleAddGuidance").length = 1;
        $.ajax({
            type: "get",
            url: "/Subjects/RegionSubject/handler/AddNewShopOrder.ashx",
            data: { type: "getGuidanceList" },
            success: function (data) {

                if (data != "") {
                    guidanceList = eval(data);
                    if ((guidanceList || {}).length > 0) {
                        for (var i = 0; i < guidanceList.length; i++) {
                            var option = "<option value='" + guidanceList[i].Id + "'>" + guidanceList[i].GuidanceName + "</option>";
                            $("#seleAddGuidance").append(option);
                        }
                    }
                }
            }
        })
    },
    getSubjectList: function (guidanceId, obj) {
        $.ajax({
            type: "get",
            url: "/Subjects/RegionSubject/handler/AddNewShopOrder.ashx",
            data: { type: "getSubjectList", guidanceId: guidanceId, subjectId: subjectId },
            success: function (data) {

                if (data != "") {
                    var subjectjson = eval(data);
                    if (subjectjson.length > 0) {
                        for (var j = 0; j < subjectjson.length; j++) {
                            var selected = "";
                            if (subjectjson[j].SubjectId == addSubjectId)
                                selected = "selected='selected'";
                            var subjectOption = "<option value='" + subjectjson[j].SubjectId + "' " + selected + ">" + subjectjson[j].SubjectName + "</option>";
                            $(obj).append(subjectOption);
                        }
                    }
                }
                addSubjectId = 0;
            }
        })
    },
    submit: function () {
        var trs = $("#tbodyPOPData tr");
        var popJson = "";
        var materialSupport = $("#seleMaterialSupport").val();
        var posScale = $("#txtPOSScale").val();
        //if (materialSupport == "") {
        //layer.msg("请选择物料支持级别");
        //return false;
        //}
        var customerId = $("#hfCustomerId").val() || 0;

        for (var i = 0; i < trs.length; i++) {
            var cb = $(trs[i]).find("td:first").find("input");
            if ($(cb).attr("checked") == "checked") {
                var popId = $(cb).val();
                var isValid = $(cb).data("isvalid");
                var orderType = $(cb).data("ordertype") || "POP";
                var chooseImgSubject = $(trs[i]).find("td").eq(2).find("select[name='seleSubject']");
                var chooseImg = "";
                if (chooseImgSubject && $(chooseImgSubject).val() != "0")
                    chooseImg = $(chooseImgSubject).find("option:selected").text();
                var chooseImgRemark = $(trs[i]).find("td").eq(3).find("input").val() || "";
                var sheet = $(trs[i]).find("td").eq(4).html() || "";
                var graphicNo = $(trs[i]).find("td").eq(5).html() || "";
                var gender = $(trs[i]).find("td").eq(6).find("select").val() || "";
                var quantity = $(trs[i]).find("td").eq(7).find("input").val() || "";
                var width = $(trs[i]).find("td").eq(8).html() || 0;
                var length = $(trs[i]).find("td").eq(9).html() || 0;
                var material = $(trs[i]).find("td").eq(10).html();
                var positionDescription = $(trs[i]).find("td").eq(11).html();
                var orderPrice = $(trs[i]).find("td").eq(12).html() || 0;
                var payOrderPrice = $(trs[i]).find("td").eq(13).html() || 0;
                var remark = $(trs[i]).find("td").eq(14).find("input").val() || "";
                //orderType = orderType == "POP" ? 1 : 2;
                if (chooseImg == "")
                    chooseImg = chooseImgRemark;
                else {
                    if (chooseImgRemark != "")
                        chooseImg = chooseImg + "(" + chooseImgRemark + ")";
                }
                if (orderType.indexOf("费") == -1) {
                    if (quantity == "") {
                        layer.msg("请填写数量");
                        return false;
                        //break;
                    }
                    else if (isNaN(quantity)) {
                        layer.msg("数量填写不正确");
                        return false;
                    }
                    else if (parseInt(quantity) == 0) {
                        layer.msg("数量必须大于0");
                        return false;
                    }
                }
                else {
                    quantity = 1;
                }
                popJson += '{"Id":' + popId + ',"CustomerId":' + customerId + ',"Price":' + orderPrice + ',"PayPrice":' + payOrderPrice + ',"OrderTypeName":"' + orderType + '","SubjectId":' + subjectId + ',"ShopId":' + shopId + ',"MaterialSupport":"' + materialSupport + '","POSScale":"' + posScale + '","Sheet":"' + sheet + '","GraphicNo":"' + graphicNo + '","Gender":"' + gender + '","Quantity":' + parseInt(quantity) + ',"GraphicWidth":' + width + ',"GraphicLength":' + length + ',"GraphicMaterial":"' + material + '","PositionDescription":"' + positionDescription + '","ChooseImg":"' + chooseImg + '","Remark":"' + remark + '","IsValid":' + isValid + '},';
            }

        }
        //        alert(popJson);
        //        return false;
        if (popJson.length > 0) {
            popJson = popJson.substring(0, popJson.length - 1);
            popJson = "[" + popJson + "]";
            $.ajax({
                type: "post",
                url: "handler/AddOrderDetail.ashx",
                data: { type: 'addOrder', jsonStr: urlCodeStr(popJson) },
                success: function (data) {

                    if (data != "ok") {
                        layer.confirm(data, { title: "提交失败", btn: ['确定'] });
                    }
                    else {
                        isChangeOrder = 1;
                        layer.closeAll();
                        $("#popDetailDiv").hide();
                        Order.getList();

                    }
                }
            })
        }

    }
};


//提交订单的时候检查
function CheckOrder() {
    var orderCount = $("#tbodyOrderData tr").length;

    if (orderCount == 0) {

        layer.msg('请先添加订单');
        return false;
    }
    if (confirm("确认提交吗？")) {
        $("#imgLoading").show();
        return true;
    }
    else
        return false;
}

function CreateOrderTypeSelect(type) {
    var arr = ['POP', '道具', '安装费', '发货费'];
    var select = "<select name='seleOrderType'>";
    for (var i in arr) {
        var option = "";
        var selected = "";
        if (arr[i] == type)
            selected = "selected='selected'";
        option = "<option value='" + arr[i] + "' " + selected + ">" + arr[i] + "</option>";
        select += option;
    }
    select += "</select>";
    return select;

}

function CreateGenderSelect(gender) {
    var arr = ['男', '女', '男女不限'];
    var select = "<select name='seleGender'>";

    for (var i in arr) {
        var option = "";
        var selected = "";
        if (arr[i] == gender)
            selected = "selected='selected'";
        option = "<option value='" + arr[i] + "' " + selected + ">" + arr[i] + "</option>";
        select += option;
    }
    select += "</select>";

    return select;
}

function CreateGuidanceSelect(gid) {

    var select = '<input name="txtGuidanceMonth" class="Wdate" placeholder="活动月份" onclick="WdatePicker({dateFmt:\'yyyy年MM月\',readOnly:true,onpicked:getMonth})" style="width:90px;"/><select name="seleGuidance">';
    select += "<option value='0'>选择活动</option>";
    if ((guidanceList || {}).length > 0) {
        for (var i = 0; i < guidanceList.length; i++) {

            var selected = "";

            if (guidanceList[i].Id == gid) {
                selected = "selected='selected'";

            }
            var option = "<option value='" + guidanceList[i].Id + "' " + selected + ">" + guidanceList[i].GuidanceName + "</option>";
            select += option;
        }
    }

    select += "</select>";

    return select;
}

function CreateSujectSelect() {

    var select = "<select name='seleSubject'>";
    var option = "<option value='0'>选择项目</option>";
    select += option;
    select += "</select>";
    return select;
}

function getMonth() {
    var val = $(this).val();
    isPickDate = 1;
    $(this).blur();
}

function getMonthAdd() {
    var val = $(this).val();
    $("#seleAddGuidance option:not(:first)").remove();
    $.ajax({
        type: "get",
        url: "/Subjects/RegionSubject/handler/AddNewShopOrder.ashx",
        data: { type: "getGuidanceList", month: val },
        success: function (data) {
            if (data != "") {
                var json = eval(data);
                if (json.length > 0) {
                    for (var i = 0; i < json.length; i++) {
                        var option = "<option value='" + json[i].Id + "'>" + json[i].GuidanceName + "</option>";
                        $("#seleAddGuidance").append(option);
                    }
                }
            }
        }
    })
}

function showAddSheetMenu() {
    if (popSheetListStr.length > 0) {
        $("#divAddSheetMenu").css({ left: 15 + "px", top: -(155) + "px" }).show("fast");
        $("body").bind("mousedown", onBodyDown);
    }

}

function showEditSheetMenu() {
    if (popSheetListStr.length > 0) {

        $("#divEditSheetMenu").css({ left: 0 + "px", top: 18 + "px" }).show("fast");
        $("body").bind("mousedown", onBodyDown);
    }

}

function hideMenu() {
    $("#divAddSheetMenu").fadeOut("fast");
    //$("#divPropGenderMenu").fadeOut("fast");
    $("#divEditSheetMenu").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    //    if (!(event.target.id == "txtAddSheet" || event.target.id == "divAddSheetMenu" || $(event.target).parents("#divAddSheetMenu").length > 0 || event.target.id == "txtSheetEdit" || event.target.id == "divEditSheetMenu" || $(event.target).parents("#divEditSheetMenu").length > 0)) {
    //        hideMenu();
    //    }
    if (!(event.target.id == "txtAddSheet" || event.target.id == "divAddSheetMenu" || $(event.target).parents("#divAddSheetMenu").length > 0 || event.target.id == "txtSheetEdit" || event.target.id == "divEditSheetMenu" || $(event.target).parents("#divEditSheetMenu").length > 0 || event.target.id == "txtPropGender" || event.target.id == "divPropGenderMenu" || $(event.target).parents("#divdivPropGenderMenu").length > 0)) {
        hideMenu();
    }

}

function ClearEditVal() {
    $("#POPtable input[type='text']").val("").attr("disabled", false);
    //$("#txtSheetEdit").attr("disabled", "disabled");
    //$("#txtWidthEdit").attr("disabled", "disabled");
    //$("#txtLengthEdit").attr("disabled", "disabled");
}