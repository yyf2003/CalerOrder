﻿
var shopId = 0;
var activeTabIndex = 0;
var subjectList;
var popSheetListStr = "";
var subjectListStr = "";
var orderId = 0;
var materialId = 0;
var layerIndex = 0;
var checkOrderShopId = 0;//点查看的店铺Id
var checkOrderShopNo = ""; //点查看的店铺编号
var isChangeOrder = 0;
$(function () {
    Order.getShopList();
    Order.getMaterialList();
    //首页添加POP
    $("#btnAdd").click(function () {
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

    //查询和获取pop信息
    $("#btnGetPOP").click(function () {
        var shopNo = $.trim($("#txtShopNo").val());
        if (shopNo != "") {
            $(this).next("img").show();
            POP.getList(shopNo);
        }

    })

    //修改
    $("#btnEdit").click(function () {
        orderId = 0;
        if (activeTabIndex == 0) {
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

    //导入订单
    $("#btnImport").click(function () {
        layer.open({
            type: 2,
            time: 0,
            title: '导入订单',
            skin: 'layui-layer-rim', //加上边框
            area: ['800px', '400px'],
            content: 'ImportHCOrder.aspx?subjectId=' + subjectId,
            cancel: function () {
                
                if ($("#hfIsChange").val() == "1") {
                    $("#hfIsChange").val("");
                    Order.getShopList();
                }
            }


        });
    })

    //提交
    $("#spanSubmit").click(function () {
        $("#btnSubmit").click();
    })

    //layui事件监听
    layui.use('element', function () {
        var element = layui.element();

        //tab事件监听
        element.on('tab(order)', function (data) {
            activeTabIndex = data.index;
        });
    });

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
            var subjectId = $("#seleAddSubject").val();
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
            if (subjectId == "0") {
                layer.msg('请选择项目');
                return false;
            }
            var tr = "<tr>";
            tr += "<td><input type='checkbox' name='cbOnePOP' checked='checked' value='0'></td>";
            tr += "<td>" + CreateSujectSelect(subjectId) + "</td>";
            tr += "<td>" + orderType + "</td>";
            //tr += "<td>" + shopNo + "</td>";
            tr += "<td>" + sheet + "</td>";
            tr += "<td></td>";
            tr += "<td>" + CreateGenderSelect(gender) + "</td>";
            tr += "<td>" + quantity + "</td>";
            tr += "<td>" + width + "</td>";
            tr += "<td>" + length + "</td>";
            tr += "<td>" + material + "</td>";
            tr += "<td>" + positionDescription + "</td>";
            tr += "<td><input type='text' name='txtChooseImg' value='" + chooseImg + "' maxlength='30' style='width:80px;'/></td>";
            tr += "<td><input type='text' name='txtPOPRemark' value='" + remark + "' maxlength='30' style='width:100px;'/></td>";

            tr += "<td><span name='spanDeletePOP' style='color:red;cursor:pointer;'>删除</span></td>";
            tr += "</tr>";
            $("#tbodyPOPEmpty").hide();
            $("#tbodyPOPData").append(tr).show();
            $("#addPOPTable input").val("");
            $("#addPOPTable select").val("");
            $("#txtAddQuantity").val("1");
            
        }
        else {

        }
    })

    $("#tbodyPOPData").delegate("span[name='spanDeletePOP']", "click", function () {
        $(this).parent("td").parent("tr").remove();
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
        hideMenu();
    })


    $("#tbodyPOPData").delegate("li", "click", function () {
        //alert($(this).html());
        var div = $(this).parent().parent("div");
        $(div).prev("input").val($(this).html());
        $(div).hide();
        //alert($(div).prop("id"));
    })

    //查看店铺POP明细
    $("#tbodyOrderShopData").delegate("span[name='spanCheckOrderDetail']", "click", function () {
        isChangeOrder = 0;
        var shopId = $(this).data("shopid");
        checkOrderShopNo = $(this).data("shopno") || "";
        checkOrderShopId = shopId;
        Order.getList();
    })

    //在订单明细页面点击添加POP
    $("#btnAddFromDetail").click(function () {
        $("#txtShopNo").val(checkOrderShopNo).prop("readonly", true);
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

    $("body").keydown(function (event) {
        if (event.keyCode == 13) {
            return false;
        }
    });
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
                            tr += "<td><input type='checkbox' name='cbOneOrder' value='" + json[i].Id + "'></td>";
                            if (json[i].IsApprove == 1) {
                                //tr += "<td><input type='checkbox' name='0' disabled='disabled'></td>";
                                tr += "<td>" + (parseInt(i) + 1) + "</td>";
                                tr += "<td style='color:green'>审批通过</td>";
                            }
                            else {
                                //tr += "<td><input type='checkbox' name='cbOneOrder' value='" + json[i].Id + "'></td>";
                                if (json[i].IsApprove == 2) {
                                    tr += "<td>" + (parseInt(i) + 1) + "</td>";
                                    tr += "<td style='color:red'>审批不通过</td>";
                                }
                                else {
                                    tr += "<td>" + (parseInt(i) + 1) + "</td>";
                                    tr += "<td>未审批</td>";
                                }

                            }

                            tr += "<td>" + json[i].SubjectName + "</td>";
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
    getOrder: function () {
        document.getElementById("seleSubjectEdit").length = 1;
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
                        if (subjectJsonStr != "") {
                            var subjectJson = eval(subjectJsonStr);
                            if (subjectJson.length > 0) {
                                for (var j = 0; j < subjectJson.length; j++) {
                                    var selected = "";
                                    if (orderJson[0].HandMakeSubjectId == subjectJson[j].SubjectId)
                                        selected = "selected='selected'";
                                    var subjectOption = "<option value='" + subjectJson[j].SubjectId + "' " + selected + ">" + subjectJson[j].SubjectName + "</option>";
                                    $("#seleSubjectEdit").append(subjectOption);
                                }
                            }
                        }
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
    getMaterialList: function () {
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'getMaterialList', subjectId: subjectId },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {
                        var tr = "";
                        for (var i in json) {
                            tr += "<tr>";
                            tr += "<td><input type='checkbox' name='cbOneMaterial' value='" + json[i].Id + "'></td>";
                            tr += "<td>" + json[i].SubjectName + "</td>";
                            tr += "<td>" + json[i].ShopNo + "</td>";
                            tr += "<td>" + json[i].ShopName + "</td>";
                            tr += "<td>" + json[i].RegionName + "</td>";
                            tr += "<td>" + json[i].ProvinceName + "</td>";
                            tr += "<td>" + json[i].CityName + "</td>";
                            tr += "<td>" + json[i].Sheet + "</td>";
                            tr += "<td>" + json[i].MaterialName + "</td>";
                            tr += "<td>" + json[i].MaterialCount + "</td>";
                            tr += "<td>" + json[i].MaterialLength + "</td>";
                            tr += "<td>" + json[i].MaterialWidth + "</td>";
                            tr += "<td>" + json[i].MaterialHigh + "</td>";
                            tr += "<td>" + json[i].Price + "</td>";
                            tr += "<td>" + json[i].Remark + "</td>";

                            tr += "</tr>";
                        }
                        $("#tbodyMaterialData").html(tr).show();
                        $("#tbodyMaterialEmpty").hide();
                    }
                }
                else {
                    $("#tbodyMaterialData").html("").hide();
                    $("#tbodyMaterialEmpty").show();
                }
            }
        })
    },
    getCustomerMaterialList: function () {
        document.getElementById("seleAddMaterial").length = 1;
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
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
    submitEdit: function () {
        var handMakeSubjectId = $("#seleSubjectEdit").val();
        var sheet = $.trim($("#txtSheetEdit").val());
        var posScale = $.trim($("#txtPOSScaleEdit").val());
        var materialSupport = $("#ddlMaterialSupportEdit").val();
        var gender = $("#seleEditGender").val();
        var quantity = $.trim($("#txtQuantityEdit").val());
        var positionDescription = $.trim($("#txtPositionDescriptionEdit").val());
        var chooseImg = $.trim($("#txtChooseImgEdit").val());
        var remark = $.trim($("#txtRemarkEdit").val());
        if (subjectId == 0) {
            layer.msg('请选择项目');
            return false;
        }
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
        var orderJson = '{"Id":' + orderId + ',"MaterialSupport":"' + materialSupport + '","POSScale":"' + posScale + '","Sheet":"' + sheet + '","Gender":"' + gender + '","Quantity":' + quantity + ',"PositionDescription":"' + positionDescription + '","ChooseImg":"' + chooseImg + '","Remark":"' + remark + '","HandMakeSubjectId":' + handMakeSubjectId + '}';
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
        $.ajax({
            type: "get",
            url: "handler/AddOrderDetail.ashx",
            data: { type: 'getPOP', subjectId: subjectId, shopNo: shopNo },
            success: function (data) {

                $("#getPOPLoading").hide();
                if (data.indexOf("error") != -1) {

                    $("#getPOPMsg").html(data.split('|')[1]).show();
                    $("#tbodyPOPData").html("").hide();
                    $("#tbodyPOPEmpty").show();
                }
                else {
                    var popData = data.split('|')[0];
                    var subjectData = data.split('|')[1];
                    if (subjectData) {
                        document.getElementById("seleAddSubject").length = 1;
                        subjectList = eval(subjectData);
                        if (subjectList.length > 0) {
                            shopId = subjectList[0].ShopId;
                            for (var j = 0; j < subjectList.length; j++) {
                                var subjectOption = "<option value='" + subjectList[j].SubjectId + "'>" + subjectList[j].SubjectName + "</option>";
                                $("#seleAddSubject").append(subjectOption);
                            }
                        }
                        Order.getCustomerMaterialList();
                        if (popSheetListStr == "") {
                            POP.getPOPSheet();
                        }
                    }
                    if (popData) {
                        var json = eval(popData);
                        if (json.length > 0) {
                            var tr = "";
                            shopId = json[0].ShopId;
                            
                            if (json[0].MaterialSupport != "")
                                $("#seleMaterialSupport").val(json[0].MaterialSupport).attr("disabled", true);
                            else
                                $("#seleMaterialSupport").val("").attr("disabled", false);
                            $("#txtPOSScale").val(json[0].POSScale);
                            for (var i in json) {

                                tr += "<tr>";
                                tr += "<td><input type='checkbox' name='cbOnePOP' value='" + json[i].Id + "'></td>";
                                tr += "<td>" + CreateSujectSelect() + "</td>";
                                tr += "<td>POP</td>";

                                tr += "<td>" + json[i].Sheet + "</td>";
                                tr += "<td>" + json[i].GraphicNo + "</td>";
                                tr += "<td>" + CreateGenderSelect(json[i].Gender) + "</td>";
                                //tr += "<td>" + json[i].Gender + "</td>";
                                tr += "<td>" + json[i].Quantity + "</td>";
                                tr += "<td>" + json[i].GraphicWidth + "</td>";
                                tr += "<td>" + json[i].GraphicLength + "</td>";
                                tr += "<td>" + json[i].GraphicMaterial + "</td>";
                                tr += "<td>" + json[i].PositionDescription + "</td>";
                                //tr += "<td><input type='text' name='txtChooseImg' value='' maxlength='30' style='width:80px;'/></td>";
                                tr += "<td>" + CreateChooseImgSelect(i) + "</td>";
                                tr += "<td><input type='text' name='txtPOPRemark' value='" + json[i].Remark + "' maxlength='30' style='width:100px;'/></td>";

                                tr += "<td></td>";
                                tr += "</tr>";
                            }
                            //tr += "<tr>";
                            //tr += "<td colspan='14' style='height:50px;'></td>";
                            //tr += "</tr>";
                            $("#tbodyPOPData").html(tr).show();
                            $("#tbodyPOPEmpty").hide();
                            
                            
                        }
                        else {
                            $("#tbodyPOPData").html("").hide();
                            $("#tbodyPOPEmpty").show();
                        }
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
            url: "handler/AddOrderDetail.ashx",
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
                var handMakeSubjectId = $(trs[i]).find("td").eq(1).find("select").val() || 0;
                var orderType = $(trs[i]).find("td").eq(2).html();
                var sheet = $(trs[i]).find("td").eq(3).html();
                var graphicNo = $(trs[i]).find("td").eq(4).html();
                var gender = $(trs[i]).find("td").eq(5).find("select").val()||"";
                var quantity = $(trs[i]).find("td").eq(6).html() || 1;
                var width = $(trs[i]).find("td").eq(7).html() || 0;
                var length = $(trs[i]).find("td").eq(8).html() || 0;
                var material = $(trs[i]).find("td").eq(9).html();
                var positionDescription = $(trs[i]).find("td").eq(10).html();
                var chooseImg = $(trs[i]).find("td").eq(11).find("input").val()||"";
                var remark = $(trs[i]).find("td").eq(12).find("input").val();
                if (handMakeSubjectId == 0) {
                    layer.msg("请选择项目");
                    return false;
                }
                orderType = orderType == "POP" ? 1 : 2;
                popJson += '{"Id":' + popId + ',"OrderType":' + orderType + ',"SubjectId":' + subjectId + ',"ShopId":' + shopId + ',"MaterialSupport":"' + materialSupport + '","POSScale":"' + posScale + '","Sheet":"' + sheet + '","GraphicNo":"' + graphicNo + '","Gender":"' + gender + '","Quantity":' + quantity + ',"GraphicWidth":' + width + ',"GraphicLength":' + length + ',"GraphicMaterial":"' + material + '","PositionDescription":"' + positionDescription + '","ChooseImg":"' + chooseImg + '","Remark":"' + remark + '","HandMakeSubjectId":' + handMakeSubjectId + '},';
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
                        layer.confirm(data);
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

function CreateSujectSelect(sId) {
    
    var select = "<select name='seleSubject'>";
    if ((subjectList || {}).length > 0) {
        for (var i = 0; i < subjectList.length; i++) {
            var option = "";
            var selected = "";
            var selected = "";
            if (subjectList[i].SubjectId == sId)
                selected = "selected='selected'";
            option = "<option value='" + subjectList[i].SubjectId + "' " + selected + ">" + subjectList[i].SubjectName + "</option>";
            select += option;
        }
    }
    else {
        option = "<option value='0'>--请选择项目--</option>";
        select += option;
    }
    select += "</select>";

    return select;
}

function CreateChooseImgSelect(index) { 
   var input="<div style='position: relative;'>";
   input+="<input type='text' name='txtChooseImg' onclick='showChooseImg(this,"+index+")' maxlength='20' style='width: 80px; text-align: center;'/>";
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

//提交订单的时候检查
function CheckOrder() {
    var orderCount = $("#tbodyOrderShopData tr").length;
   
    if (orderCount == 0) {
       
        layer.msg('请先添加POP明细');
        return false;
    }
    if (confirm("确认提交吗？")) {
        $("#imgLoading").show();
        return true;
    }
    else
        return false;
}

function showAddSheetMenu() {
    if (popSheetListStr.length>0) {
        //var higt = $("#divAddSheetMenu").height() || 120;
        //var higt = $("#divAddSheetMenu").height() || 120;
        $("#divAddSheetMenu").css({ left: 2 + "px", top: -(153) + "px" }).show("fast");
        $("body").bind("mousedown", onBodyDown);
    }

}

function showEditSheetMenu() {
    if (popSheetListStr.length > 0) {
        //var higt = $("#divEditSheetMenu").height() || 120;
        $("#divEditSheetMenu").css({ left: 0 + "px", top: 18+ "px" }).show("fast");
        $("body").bind("mousedown", onBodyDown);
    }

}

function hideMenu() {
    $("#divAddSheetMenu").fadeOut("fast");
    $("#divEditSheetMenu").fadeOut("fast");
    $(".ChooseImgDiv").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "txtAddSheet" || event.target.id == "divAddSheetMenu" || $(event.target).parents("#divAddSheetMenu").length > 0 || event.target.id == "txtSheetEdit" || event.target.id == "divEditSheetMenu" || $(event.target).parents("#divEditSheetMenu").length > 0)) {
        hideMenu();
    }
    
    
}