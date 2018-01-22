

var shopId = 0;
var addSubjectId = 0;
var guidanceList;
var popSheetListStr = "";
var subjectListStr = "";
var orderId = 0;
var materialId = 0;
var layerIndex = 0;
var checkOrderShopId = 0; //点查看的店铺Id
var checkOrderShopNo = ""; //点查看的店铺编号
var isChangeOrder = 0;
var isPickDate = 0; //是否选择完时间


$(function () {
    Order.getShopList();
    $("#btnAdd").click(function () {
        POP.getSubjectGuidanceList();
        layerIndex = 0;
        $("#txtShopNo").val("").prop("readonly", false);
        $("#getPOPMsg").html("");
        $("#tbodyPOPData").html("").hide();
        $("#tbodyPOPEmpty").show();
        layer.open({
            type: 1,
            time: 0,
            title: '添加POP订单明细',
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
            $("#getPOPLoading").show();
            POP.getList(shopNo);
        }

    })

    $("#tbodyPOPData").delegate("input[name='txtGuidanceMonth']", "blur", function () {
        if (isPickDate == 1) {
            isPickDate = 0;
            var seleGuidance = $(this).next("select");
            var val = $(this).val();
            $(seleGuidance).find("option:not(:first)").remove();
            $.ajax({
                type: "get",
                url: "handler/AddOrderDetail.ashx",
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

    //POP全选
    $("#cbAllPOP").click(function () {
        var checked = this.checked;
        $("input[name='cbOnePOP']").each(function () {
            this.checked = checked;
        })
    })

    $("#tbodyPOPData").delegate("input[name='cbOnePOP']", "change", function () {
        if (!this.checked) {
            $("#cbAllPOP").attr("checked", false);
        }
    })



    //查看店铺POP明细
    $("#tbodyOrderShopData").delegate("span[name='spanCheckOrderDetail']", "click", function () {
        isChangeOrder = 0;
        var shopId = $(this).data("shopid");
        checkOrderShopNo = $(this).data("shopno") || "";
        checkOrderShopId = shopId;
        Order.getList();
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
                        isChangeOrder = 1;
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

    //Order全选
    $("#cbAllOrder").click(function () {
        var checked = this.checked;
        $("input[name='cbOneOrder']").each(function () {
            this.checked = checked;
        })
    })

    $("#tbodyOrderData").delegate("input[name='cbOneOrder']", "change", function () {
        if (!this.checked) {
            $("#cbAllOrder").attr("checked", false);
        }
    })

    //在订单明细页面点击添加POP
    $("#btnAddFromDetail").click(function () {
        $("#txtShopNo").val(checkOrderShopNo).prop("readonly", true);
        POP.getSubjectGuidanceList();
        $("#btnGetPOP").click();
        layerIndex = layer.open({
            type: 1,
            time: 0,
            title: '添加POP订单明细',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '90%'],
            content: $("#popDetailDiv"),
            id: 'popLayer0',
            btn: ['提 交'],
            btnAlign: 'c',
            yes: function () {
                POP.submit();
            },
            cancel: function (index) {
                layer.close(index);
                $("#popDetailDiv").hide();

            }

        });

    })


    $("#seleAddGuidance").change(function () {
        var guidanceId = $(this).val();

        var seleSubject = $("#seleAddSubject");
        $(seleSubject).find("option:not(:first)").remove();
        POP.getSubjectList(guidanceId, seleSubject);
    })


    $("#txtAddSheet").on("click", function () {
        showAddSheetMenu();
    })

    $("#ddlAddSheetMenu").delegate("li", "click", function () {

        var sheetVal = $(this).html();
        $("#txtAddSheet").val(sheetVal);
        if (sheetVal.toLocaleLowerCase() == "hc鞋墙") {
            $("#txtAddWidth").val("1200");
            $("#txtAddLength").val("245");
        }
        //hideMenu();
        $("#divAddSheetMenu").hide();
    })

    $("#txtSheetEdit").on("click", function () {
        showEditSheetMenu();
    })

    $("#ddlEditSheetMenu").delegate("li", "click", function () {
        $("#txtSheetEdit").val($(this).html());
        $("#divEditSheetMenu").fadeOut("fast");
        //hideMenu();
    })

    $("#txtPropGender").on("click", function () {
        showAddPropGenderMenu();
    })

    $("#dllPropGenderMenu").delegate("li", "click", function () {
        $("#txtPropGender").val($(this).html());
        $("#divPropGenderMenu").fadeOut("fast");

    })

    //手动添加POP
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
            if (width != "" && isNaN(width)) {
                layer.msg('POP宽必须是数字');
                return false;
            }
            if (length != "" && isNaN(width)) {
                layer.msg('POP高必须是数字');
                return false;
            }

            var tr = "<tr>";
            tr += "<td><input type='checkbox' name='cbOnePOP' data-isvalid='1' checked='checked' value='0'></td>";
            tr += "<td>" + orderType + "</td>";
            tr += "<td style='text-align:left; padding-left :0px;'>活动：" + CreateGuidanceSelect(chooseImgGuidanceId);
            tr += "项目：" + CreateSujectSelect() + "</td>";
            tr += "<td><input type='text' name='txtChooseImg' value='" + chooseImg + "' maxlength='30' style='width:80px;'/></td>";
            tr += "<td>" + sheet + "</td>";
            tr += "<td></td>";
            tr += "<td>" + CreateGenderSelect(gender) + "</td>";
            tr += "<td>" + quantity + "</td>";
            tr += "<td>" + width + "</td>";
            tr += "<td>" + length + "</td>";
            tr += "<td>" + material + "</td>";
            tr += "<td>" + positionDescription + "</td>";

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

        }
    })

    $("#tbodyPOPData").delegate("span[name='spanDeletePOP']", "click", function () {
        $(this).parent("td").parent("tr").remove();
    })


    //设置道具信息
    $("#btnSetProp").click(function () {
        Prop.getList();
        clearPropVal();
        layer.open({
            type: 1,
            time: 0,
            title: '设置道具信息',
            skin: 'layui-layer-rim', //加上边框
            area: ['80%', '80%'],
            content: $("#editPropDiv"),
            id: 'popLayer11',
            cancel: function (index) {
                layer.close(index);
                $("#editPropDiv").hide();

            }

        });
    })


    //提交新道具信息
    $("#btnAddProp").click(function () {
        Prop.submit();
    })

    //修改道具
    $("#tbodyPropData").delegate("span[name='spanEditProp']", "click", function () {
        clearPropVal();
        var id = $(this).data("propid") || 0;
        var tr = $(this).parent().parent("tr");
        var materialSupport = $(tr).find("td").eq(1).html();
        var propName = $(tr).find("td").eq(2).html();
        var gender = $(tr).find("td").eq(3).html();
        var quantity = $(tr).find("td").eq(4).html();
        var graphicMaterial = $(tr).find("td").eq(5).html();
        var positionDescription = $(tr).find("td").eq(6).html();
        var remark = $(tr).find("td").eq(7).html();
        $("#selPropMaterialSupport").val(materialSupport);
        $("#txtPropName").val(propName);
        $("#txtPropGender").val(gender);
        $("#txtPropQuantity").val(quantity);
        $("#txtPropPositionDescription").val(positionDescription);
        $("#txtPropRemark").val(remark);
        Prop.model.Id = id;

    })

    //删除道具
    $("#tbodyPropData").delegate("span[name='spanDeleteProp']", "click", function () {
        var id = $(this).data("propid") || 0;
        Prop.deleteProp(id);
    })

    $("#seleMaterialSupport").on("change", function () {
        var popCount = $("#tbodyPOPData tr").length;

        if (popCount > 0) {
            var val = $(this).val();
            $.ajax({
                type: "get",
                url: "handler/AddOrderDetail.ashx",
                data: { type: "getProp", MaterialSupport: val },
                success: function (data) {
                    $("#tbodyPOPData tr").each(function () {
                        
                        var td = $(this).find("td:last");
                        var spanDelete = $(td).find("span");
                        if (spanDelete) {
                            var isFromdb = $(spanDelete).data("fromdb") || 0;
                            if (isFromdb == 1) {
                                $(this).remove();
                            }
                        }
                    })
                    if (data != "") {
                        var json = eval(data);
                        if (json.length > 0) {
                            var tr = "";
                            for (var i = 0; i < json.length; i++) {
                                tr += "<tr>";
                                tr += "<td><input type='checkbox' name='cbOnePOP' data-isvalid='1' value='" + json[i].Id + "' checked='checked'></td>";
                                tr += "<td>道具</td>";
                                tr += "<td style='padding-left:0px;'>活动：" + CreateGuidanceSelect();
                                tr += "项目：" + CreateSujectSelect() + "</td>";
                                tr += "<td>" + CreateChooseImgSelect(i) + "</td>";
                                tr += "<td>" + json[i].PropName + "</td>";
                                tr += "<td></td>";
                                tr += "<td>" + json[i].Gender + "</td>";
                                tr += "<td>" + json[i].Quantity + "</td>";
                                tr += "<td></td>";
                                tr += "<td></td>";
                                tr += "<td></td>";
                                tr += "<td>" + json[i].PositionDescription + "</td>";
                                tr += "<td><input type='text' name='txtPOPRemark' value='" + json[i].Remark + "' maxlength='30' style='width:100px;'/></td>";
                                tr += "<td><span name='spanDeletePOP' data-fromdb='1' style='color:red;cursor:pointer;'>删除</span></td>";
                                tr += "</tr>";
                            }
                            $("#tbodyPOPData").append(tr)
                        }
                    }
                }
            })
        }
    })


})

var Order = {
    getList: function () {
        var loadIndex = layer.load(0, { shade: false });

        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'getList', subjectId: subjectId, shopId: (checkOrderShopId || 0) },
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

                            tr += "<td>" + json[i].MaterialSupport + "</td>";
                            tr += "<td>" + json[i].POSScale + "</td>";

                            tr += "<td>" + json[i].Sheet + "</td>";
                            tr += "<td>" + json[i].GraphicNo + "</td>";
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
                if (layerIndex == 0) {
                    layer.open({
                        type: 1,
                        time: 0,
                        title: '查看POP订单明细',
                        skin: 'layui-layer-rim', //加上边框
                        area: ['95%', '90%'],
                        content: $("#showPOPOrderDetailDiv"),
                        id: 'popLayer',
                        cancel: function () {
                            layer.closeAll();
                            layerIndex = 0;
                            $("#showPOPOrderDetailDiv").hide();
                            if (isChangeOrder == 1) {
                                Order.getShopList();
                                isChangeOrder = 0;
                            }
                        }
                    });
                }
            }
        });

    },
    getShopList: function () {
        var loadIndex = layer.load(0, { shade: false });
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'getOrderShopList', subjectId: subjectId },
            success: function (data) {
                layer.close(loadIndex);
                if (data != "") {
                    var json = eval(data);
                    if (json.length > 0) {
                        var tr = "";
                        for (var i in json) {
                            tr += "<tr>";
                            tr += "<td>" + (parseInt(i) + 1) + "</td>";
                            if (json[i].ApproveState == 0)
                                tr += "<td><span style='color:red;'>待审批</span></td>";
                            else if (json[i].ApproveState == 1)
                                tr += "<td><span style='color:blue;'>未完成审批</span></td>";
                            else
                                tr += "<td><span style='color:green;'>完成审批</span></td>";
                            tr += "<td>" + json[i].ShopNo + "</td>";
                            tr += "<td>" + json[i].ShopName + "</td>";
                            tr += "<td>" + json[i].Region + "</td>";
                            tr += "<td>" + json[i].Province + "</td>";
                            tr += "<td>" + json[i].City + "</td>";
                            tr += "<td>" + json[i].Channel + "</td>";
                            tr += "<td>" + json[i].Format + "</td>";
                            tr += "<td>" + json[i].ShopType + "</td>";
                            tr += "<td>" + json[i].POPCount + "</td>";
                            tr += "<td><span name='spanCheckOrderDetail' data-shopid='" + json[i].ShopId + "' data-shopno='" + json[i].ShopNo + "' style='color:blue;cursor:pointer;'>查看</td>";
                            tr += "</tr>";
                        }
                        $("#tbodyOrderShopData").html(tr).show();
                        $("#tbodyOrderShopEmpty").hide();
                    }
                }
                else {
                    $("#tbodyOrderShopData").html("").hide();
                    $("#tbodyOrderShopEmpty").show();
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

        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'getOrder', id: orderId },
            success: function (data) {
                if (data != "") {
                    var orderJsonStr = data.split('|')[0];
                    var subjectJsonStr = data.split('|')[1];
                    var orderJson = eval(orderJsonStr);
                    if (orderJson.length > 0) {
                        $("#labOrderTypeEdit").html(orderJson[0].OrderType);
                        $("#labGraphicNoEdit").html(orderJson[0].GraphicNo);
                        $("#txtSheetEdit").val(orderJson[0].Sheet);
                        $("#ddlMaterialSupportEdit").val(orderJson[0].MaterialSupport);
                        $("#txtPOSScaleEdit").val(orderJson[0].POSScale);
                        $("#seleEditGender").val(orderJson[0].Gender);
                        $("#txtQuantityEdit").val(orderJson[0].Quantity);
                        $("#labWidthEdit").html(orderJson[0].GraphicWidth);
                        $("#labLengthEdit").html(orderJson[0].GraphicLength);
                        $("#labMaterialEdit").html(orderJson[0].GraphicMaterial);
                        $("#txtPositionDescriptionEdit").val(orderJson[0].PositionDescription);
                        $("#txtChooseImgEdit").val(orderJson[0].ChooseImg);
                        $("#txtRemarkEdit").val(orderJson[0].Remark);

                        if (popSheetListStr == "") {
                            POP.getPOPSheet();
                        }
                        layerIndex = layer.open({
                            type: 1,
                            time: 0,
                            title: '订单修改',
                            skin: 'layui-layer-rim', //加上边框
                            area: ['755px', '350px'],
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
                }

            }
        })
    },
    submitEdit: function () {

        var sheet = $.trim($("#txtSheetEdit").val());
        var posScale = $.trim($("#txtPOSScaleEdit").val());
        var materialSupport = $("#ddlMaterialSupportEdit").val();
        var gender = $("#seleEditGender").val();
        var quantity = $.trim($("#txtQuantityEdit").val());
        var positionDescription = $.trim($("#txtPositionDescriptionEdit").val());
        var chooseImg = $.trim($("#txtChooseImgEdit").val());
        var remark = $.trim($("#txtRemarkEdit").val());

        if (sheet == "") {
            layer.msg('请填写POP位置');
            return false;
        }
        if (materialSupport == "") {
            layer.msg("请选择物料支持级别");
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
        var orderJson = '{"Id":' + orderId + ',"MaterialSupport":"' + materialSupport + '","POSScale":"' + posScale + '","Sheet":"' + sheet + '","Gender":"' + gender + '","Quantity":' + quantity + ',"PositionDescription":"' + positionDescription + '","ChooseImg":"' + chooseImg + '","Remark":"' + remark + '"}';
        $.ajax({
            type: "post",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'editOrder', jsonStr: urlCodeStr(orderJson) },
            success: function (data) {
                if (data != "ok") {

                    layer.confirm(data);
                }
                else {
                    isChangeOrder = 1;
                    layer.close(layerIndex);
                    $("#editDiv").hide();
                    Order.getList();
                }
            }
        })

    }
};


var POP = {
    getList: function (shopNo) {

        shopId = 0;
        $("#getPOPMsg").html("").hide();
        document.getElementById("seleAddMaterial").length = 1;
        var materialSupport = $("#seleMaterialSupport").val();
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'getPOP', subjectId: subjectId, shopNo: shopNo, materialSupport: materialSupport },
            success: function (data) {
                if (data != "") {
                    var flag = data.split('|')[0];
                    var msg = data.split('|')[1];
                    $("#getPOPLoading").hide();
                    if (flag == "error") {

                        $("#getPOPMsg").html(msg).show();
                        $("#tbodyPOPData").html("").hide();
                        $("#tbodyPOPEmpty").show();
                    }
                    else {
                       
                        var json = eval(msg);
                        if (json.length > 0) {
                            var tr = "";
                            shopId = json[0].ShopId;

                            //$("#seleMaterialSupport").val(json[0].MaterialSupport);
                            //$("#txtPOSScale").val(json[0].POSScale);
                            for (var i in json) {
                                var orderType = "POP";
                                var checked = "";
                                if (json[i].OrderType == 2) {
                                    orderType = "道具";
                                    checked = "checked='checked'";
                                }
                                tr += "<tr>";
                                tr += "<td><input type='checkbox' name='cbOnePOP' data-isvalid='" + json[i].IsValid + "' value='" + json[i].Id + "' " + checked + "></td>";
                                tr += "<td>" + orderType + "</td>";
                                tr += "<td style='padding-left:0px;'>活动：" + CreateGuidanceSelect();
                                tr += "项目：" + CreateSujectSelect() + "</td>";
                                tr += "<td>" + CreateChooseImgSelect(i) + "</td>";
                                tr += "<td>" + json[i].Sheet + "</td>";
                                tr += "<td>" + json[i].GraphicNo + "</td>";
                                tr += "<td>" + json[i].Gender + "</td>";
                                tr += "<td>" + json[i].Quantity + "</td>";
                                tr += "<td>" + json[i].GraphicWidth + "</td>";
                                tr += "<td>" + json[i].GraphicLength + "</td>";
                                tr += "<td>" + json[i].GraphicMaterial + "</td>";
                                tr += "<td>" + json[i].PositionDescription + "</td>";
                                tr += "<td><input type='text' name='txtPOPRemark' value='" + json[i].Remark + "' maxlength='30' style='width:100px;'/></td>";
                                if (json[i].OrderType == 2) {
                                    tr += "<td><span name='spanDeletePOP' data-fromdb='1' style='color:red;cursor:pointer;'>删除</span></td>";
                                }
                                else {
                                    tr += "<td></td>";
                                }
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
                else {
                    $("#tbodyPOPData").html("").hide();
                    $("#tbodyPOPEmpty").show();
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
            url: "handler/AddOrderDetail.ashx",
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
            url: "handler/AddOrderDetail.ashx",
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
        if (materialSupport == "") {
            layer.msg("请选择物料支持级别");
            return false;
        }
        for (var i = 0; i < trs.length; i++) {
            var cb = $(trs[i]).find("td:first").find("input");
            if ($(cb).attr("checked") == "checked") {
                var popId = $(cb).val();
                var isValid = $(cb).data("isvalid");

                var orderType = $(trs[i]).find("td").eq(1).html();
                var chooseImgSubject = $(trs[i]).find("td").eq(2).find("select[name='seleSubject']");
                var chooseImg = "";
                if ($(chooseImgSubject).val() != "0")
                    chooseImg = $(chooseImgSubject).find("option:selected").text();
                var chooseImgRemark = $(trs[i]).find("td").eq(3).find("input").val();
                var sheet = $(trs[i]).find("td").eq(4).html();
                var graphicNo = $(trs[i]).find("td").eq(5).html();
                var gender = $(trs[i]).find("td").eq(6).html();
                var quantity = $(trs[i]).find("td").eq(7).html() || 1;
                var width = $(trs[i]).find("td").eq(8).html() || 0;
                var length = $(trs[i]).find("td").eq(9).html() || 0;
                var material = $(trs[i]).find("td").eq(10).html();
                var positionDescription = $(trs[i]).find("td").eq(11).html();
                var remark = $(trs[i]).find("td").eq(12).find("input").val();
                orderType = orderType == "POP" ? 1 : 2;
                if (chooseImg == "")
                    chooseImg = chooseImgRemark;
                else {
                    if (chooseImgRemark != "")
                        chooseImg = chooseImg + "(" + chooseImgRemark + ")";
                }
                popJson += '{"Id":' + popId + ',"OrderType":' + orderType + ',"SubjectId":' + subjectId + ',"ShopId":' + shopId + ',"MaterialSupport":"' + materialSupport + '","POSScale":"' + posScale + '","Sheet":"' + sheet + '","GraphicNo":"' + graphicNo + '","Gender":"' + gender + '","Quantity":' + quantity + ',"GraphicWidth":' + width + ',"GraphicLength":' + length + ',"GraphicMaterial":"' + material + '","PositionDescription":"' + positionDescription + '","ChooseImg":"' + chooseImg + '","Remark":"' + remark + '","IsValid":' + isValid + '},';
            }

        }

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
                        if (layerIndex > 0) {
                            layer.close(layerIndex);
                            $("#popDetailDiv").hide();
                            Order.getList();
                        }
                        else {
                            layer.closeAll();
                            $("#popDetailDiv").hide();
                            Order.getShopList();
                        }

                    }
                }
            })
        }

    }

};

//道具
var Prop = {
    model: function () {
        this.Id = 0;
        this.MaterialSupport = "";
        this.PropName = "";
        this.Quantity = "";
        this.Gender = "";
        this.GraphicMaterial = "";
        this.PositionDescription = "";
        this.Remark = "";
    },
    getList: function () {
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx?type=getPropList",
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var tr = "";
                    for (var i = 0; i < json.length; i++) {
                        tr += "<tr>";
                        tr += "<td>" + (i + 1) + "</td>";
                        tr += "<td>" + json[i].MaterialSupport + "</td>";
                        tr += "<td>" + json[i].PropName + "</td>";
                        tr += "<td>" + json[i].Gender + "</td>";
                        tr += "<td>" + json[i].Quantity + "</td>";
                        tr += "<td>" + json[i].GraphicMaterial + "</td>";
                        tr += "<td>" + json[i].PositionDescription + "</td>";
                        tr += "<td>" + json[i].Remark + "</td>";
                        tr += "<td><span name='spanEditProp' data-propid='" + json[i].Id + "' style='color:blue;cursor:pointer;'>修改</span> | <span name='spanDeleteProp' data-propid='" + json[i].Id + "' style='color:red;cursor:pointer;'>删除</span></td>";

                    }
                    $("#tbodyPropData").html(tr).show();
                    $("#tbodyPropEmpty").hide();
                }
                else {
                    $("#tbodyPropData").html("").hide();
                    $("#tbodyPropEmpty").show();
                }
            }
        })
    },
    submit: function () {
        if (checkPropVal()) {
            $("#imgPropLoading").show();
            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"MaterialSupport":"' + this.model.MaterialSupport + '","PropName":"' + this.model.PropName + '","Quantity":"' + this.model.Quantity + '","Gender":"' + this.model.Gender + '","GraphicMaterial":"' + this.model.GraphicMaterial + '","PositionDescription":"' + this.model.PositionDescription + '","Remark":"' + this.model.Remark + '"}';
            $.ajax({
                type: "post",
                url: "handler/AddOrderDetail.ashx",
                data: { type: 'editProp', jsonStr: urlCodeStr(jsonStr) },
                success: function (data) {
                    $("#imgPropLoading").hide();
                    if (data == "ok") {
                        Prop.getList();
                        clearPropVal();
                    }
                    else {
                        layer.msg(data);
                    }
                }
            })
        }
    },
    deleteProp: function (id) {
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'deleteProp', id: id },
            success: function (data) {
                if (data == "ok") {
                    Prop.getList();
                }
                else
                    layer.msg(data);
            }
        })
    }
};

function CreateGuidanceSelect(gid) {

    var select = '<input name="txtGuidanceMonth" class="Wdate" onclick="WdatePicker({dateFmt:\'yyyy年MM月\',readOnly:true,onpicked:getMonth})" style="width:90px;"/><select name="seleGuidance">';
    select += "<option value='0'>--选择活动--</option>";
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
    var option = "<option value='0'>--选择项目--</option>";
    select += option;
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

function CreateChooseImgSelect(index) {
    var input = "<div style='position: relative;'>";
    input += "<input type='text' name='txtChooseImg' onclick='showChooseImg(this," + index + ")' maxlength='20' style='width: 80px; text-align: center;'/>";
    input += "<div class='ChooseImgDiv' id='ChooseImgDiv" + index + "' name='ChooseImgDiv'  style='display:none; position: absolute; width: 100px;background-color: White; border: 1px solid #ccc; padding-top: 2px; z-index: 100;'>";
    input += "<ul style='margin-top: 0; width: 100px; margin-left: 0px; list-style: none;'>";
    input += "<li>summer男</li>";
    input += "<li>summer女</li>";
    input += "<li>bestseller男</li>";
    input += "<li>bestseller女</li>";
    input += "<li>哈登扣篮</li>";
    input += "<li>ESS女</li>";
    input += "<li>足球</li>";
    input += "</ul></div>";
    return input;

}

function showChooseImg(obj, index) {

    $(".ChooseImgDiv").hide();
    //$("divChooseImg'" + index+"'").css({ left: 2 + "px", top: -(higt + 4) + "px" }).show("fast");
    $(".ChooseImgDiv").eq(index).css({ left: 0 + "px", top: 19 + "px" }).show("fast");
    $("body").bind("mousedown", onBodyDown);
}

function showAddSheetMenu() {
    if (popSheetListStr.length > 0) {

        $("#divAddSheetMenu").css({ left: 12 + "px", top: -(155) + "px" }).show("fast");
        $("body").bind("mousedown", onBodyDown);
    }

}

function showEditSheetMenu() {
    if (popSheetListStr.length > 0) {

        $("#divEditSheetMenu").css({ left: 0 + "px", top: 18 + "px" }).show("fast");
        $("body").bind("mousedown", onBodyDown);
    }

}

function showAddPropGenderMenu() {
    $("#divPropGenderMenu").css({ left: 0 + "px", top: 18 + "px" }).show("fast");
    $("body").bind("mousedown", onBodyDown);

}

function hideMenu() {
    $("#divAddSheetMenu").fadeOut("fast");
    $("#divEditSheetMenu").fadeOut("fast");
    $("#divPropGenderMenu").fadeOut("fast");
    $(".ChooseImgDiv").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "txtAddSheet" || event.target.id == "divAddSheetMenu" || $(event.target).parents("#divAddSheetMenu").length > 0 || event.target.id == "txtSheetEdit" || event.target.id == "divEditSheetMenu" || $(event.target).parents("#divEditSheetMenu").length > 0 || event.target.id == "txtPropGender" || event.target.id == "divPropGenderMenu" || $(event.target).parents("#divdivPropGenderMenu").length > 0)) {
        hideMenu();
    }


}

//提交订单的时候检查
function CheckOrder() {
    var orderCount = $("#tbodyOrderShopData tr").length;

    if (orderCount == 0) {

        layer.msg('请先添加POP明细');
        return false;
    }

    $("#imgLoading").show();
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
        url: "handler/AddOrderDetail.ashx",
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

function checkPropVal() {
    var materialSupport = $("#selPropMaterialSupport").val();
    var propName = $.trim($("#txtPropName").val());
    var gender = $.trim($("#txtPropGender").val());
    var quantity = $.trim($("#txtPropQuantity").val());
    var positionDescription = $.trim($("#txtPropPositionDescription").val());
    var remark = $.trim($("#txtPropRemark").val());
    if (materialSupport == "") {
        layer.msg("请选择物料支持级别");
        return false;
    }
    if (propName == "") {
        layer.msg("请填写道具名称");
        return false;
    }
    if (quantity == "") {
        layer.msg("请填写数量");
        return false;
    }
    else if (isNaN(quantity)) {
        layer.msg("请数量填写不正确");
        return false;
    }
    Prop.model.MaterialSupport = materialSupport;
    Prop.model.PropName = propName;
    Prop.model.Quantity = quantity;
    Prop.model.Gender = gender;
    Prop.model.GraphicMaterial = "";
    Prop.model.PositionDescription = positionDescription;
    Prop.model.Remark = remark;
    return true;
}

function clearPropVal() {
    Prop.model.Id = 0;
    Prop.model.MaterialSupport = "";
    Prop.model.PropName = "";
    Prop.model.Quantity = "";
    Prop.model.Gender = "";
    Prop.model.GraphicMaterial = "";
    Prop.model.PositionDescription = "";
    Prop.model.Remark = "";
    $("#selPropMaterialSupport").val("");
    $("#txtPropName").val("");
    $("#txtPropGender").val("");
    $("#txtPropQuantity").val("1");
    $("#txtPropPositionDescription").val("");
    $("#txtPropRemark").val("");
}


