

var currPage = 1;
var isAdd = 0;
var currMaterial = "";
$(function () {
    Order.getSheetList();
    Order.getList(currPage);

    //POP全选
    $("#cbAllPOP").click(function () {
        var checked = this.checked;
        $("input[name='cbOnePOP']").each(function () {
            this.checked = checked;
        })
    })

    $("#tbodyOrderData").delegate("input[name='cbOnePOP']", "change", function () {
        if (!this.checked) {
            $("#cbAllPOP").attr("checked", false);
        }
    })

    $("#btnAdd").click(function () {
        ClearVal();
        isAdd = 1;
        Order.bindSheet();
        Order.bindMaterialCategory();
        if ($("#hfIsRegionSubject").val() == "1") {
            $("tr.showSubjectList").show();
            $("#ddlSubjectList").val("0");
        }
        else {
            $("tr.showSubjectList").hide();
            $("#ddlSubjectList").val("0");
        }
        var index0 = layer.open({
            type: 1,
            time: 0,
            title: '添加订单',
            skin: 'layui-layer-rim', //加上边框
            area: ['760', '500'],
            content: $("#editDiv"),
            id: 'popLayer',
            btn: ['提 交'],
            btnAlign: 'c',
            yes: function () {
                Order.submit();
            },
            cancel: function () {
                $("#editDiv").hide();
                layer.closeAll();
            }

        });
    })

    $("#btnEdit").click(function () {
        ClearVal();
        var selectCount = $("input[name='cbOnePOP']:checked").length;
        if (selectCount > 1) {
            layer.msg('一次只能修改一行');
            return false;
        }
        var orderId = $("input[name='cbOnePOP']:checked:first").val() || 0;
        if (orderId > 0) {
            Order.model.Id = orderId;
            Order.bindSheet();
            $.ajax({
                type: "get",
                url: "handler/List.ashx",
                data: { type: 'getOrder', orderId: orderId },
                success: function (data) {
                    if (data != "") {
                        var json = eval(data);
                        if (json.length > 0) {
                            $("input:radio[name^='rblOrderType']").each(function () {
                                if ($(this).val() == json[0].OrderType) {
                                    $(this).attr("checked", "checked");
                                    $("input:radio[name^='rblOrderType']").attr("disabled", "disabled");
                                }
                            })
                            $("#txtShopNo").val(json[0].ShopNo).attr("disabled", "disabled");
                            $("#txtSheet").val(json[0].Sheet);
                            Order.getMachineFrame(json[0].MachineFrame);
                            $("#txtPOSScale").val(json[0].POSScale);

                            $("#ddlMaterialSupport option").each(function () {

                                if ($(this).val().toLowerCase() == json[0].MaterialSupport.toLowerCase()) {

                                    $(this).attr("selected", "selected");
                                }
                            })
                            $("#txtPositionDescription").val(json[0].PositionDescription);
                            $("#txtGender").val(json[0].Gender);
                            $("#txtQuantity").val(json[0].Quantity);
                            $("#txtGraphicWidth").val(json[0].GraphicWidth);
                            $("#txtGraphicLength").val(json[0].GraphicLength);
                            currMaterial = json[0].GraphicMaterial;
                            Order.bindMaterialCategory(json[0].MaterialCategoryId);
                            $("#txtChooseImg").val(json[0].ChooseImg);
                            $("#txtRemark").val(json[0].Remark);
                            
                            if ($("#hfIsRegionSubject").val() == "1") {
                                $("tr.showSubjectList").show();
                                $("#ddlSubjectList").val(json[0].SubjectId);
                            }
                            else {
                                $("tr.showSubjectList").hide();
                                $("#ddlSubjectList").val("0");
                            }
                            if (json[0].OrderType > 3) {
                                $("tr.pop").hide();
                                $("tr.price").show();
                                $("#txtReceivePrice").val(json[0].OrderPrice);
                                $("#txtPayPrice").val(json[0].PayOrderPrice);

                            }
                            else {
                                $("tr.pop").show();
                                $("tr.price").hide();
                                $("#txtReceivePrice").val("");
                                $("#txtPayPrice").val("");
                            }
                            layer.open({
                                type: 1,
                                time: 0,
                                title: '修改订单',
                                skin: 'layui-layer-rim', //加上边框
                                area: ['760', '500'],
                                content: $("#editDiv"),
                                id: 'popLayer',
                                btn: ['提 交'],
                                btnAlign: 'c',
                                yes: function () {
                                    Order.submit();
                                },
                                cancel: function () {
                                    $("#editDiv").hide();
                                    layer.closeAll();
                                }

                            });
                        }
                    }
                }
            })

        }
    })

    $("#txtSheet").on("click", function () {
        showSheetMenu();
    })

    $("#txtGender").on("click", function () {
        showGenderMenu();
    })

    $("#ddlSheetMenu").delegate("li", "click", function () {
        $("#txtSheet").val($(this).html());
        hideMenu();
        Order.getMachineFrame();
    })
    $("#ddlGenderMenu").delegate("li", "click", function () {
        $("#txtGender").val($(this).html());
        hideMenu();
    })

    $("#ddlMaterialCategory").change(function () {
        Order.getMaterial();
    })

    $("#btnSearch").click(function () {
        Order.getList(currPage);
    })

    var shopNotxt = "";
    $("#txtShopNo").on({
        focusin: function () {
            shopNotxt = $.trim($(this).val());
        },
        focusout: function () {
            var shopNotxt1 = $.trim($(this).val());
            if (isAdd == 1 && shopNotxt != shopNotxt1 && shopNotxt1 != "") {
                $.ajax({
                    type: "get",
                    url: "handler/List.ashx",
                    data: { type: "checkShop", subjectId: subjectId, shopNo: shopNotxt1 },
                    success: function (data) {

                        if (data != "") {
                            //$("#ddlMaterialSupport").val(data.split('|')[0]);
                            $("#ddlMaterialSupport option").each(function () {
                                if ($(this).val().toLowerCase() == data.split('|')[0].toLowerCase()) {
                                    $(this).attr("selected", "selected");
                                }
                            })
                            $("#txtPOSScale").val(data.split('|')[1]);
                        }
                        else {
                            $("#ddlMaterialSupport").val("");
                            $("#txtPOSScale").val("");
                        }
                    }
                })
            }
        }
    })



    //删除
    $("#btnDelete").click(function () {
        var ids = "";
        $("input[name='cbOnePOP']:checked").each(function () {
            ids += $(this).val() + ",";
        })
        if (ids.length > 0) {
            var loadIndex = layer.load(0, { shade: false });
            $.ajax({
                type: "get",
                url: "handler/List.ashx",
                data: { type: "deleteOrder", ids: ids },
                success: function (data) {
                    if (data == "ok") {
                        Order.getList(currPage);
                    }
                    else {
                        layer.msg(data);
                    }
                }
            })
        }
    })

    //恢复
    $("#btnRecover").click(function () {
        var ids = "";
        $("input[name='cbOnePOP']:checked").each(function () {
            ids += $(this).val() + ",";
        })
        if (ids.length > 0) {
            var loadIndex = layer.load(0, { shade: false });
            $.ajax({
                type: "get",
                url: "handler/List.ashx",
                data: { type: "recoverOrder", ids: ids },
                success: function (data) {
                    if (data == "ok") {
                        Order.getList(currPage);
                    }
                    else {
                        layer.msg(data);
                    }
                }
            })
        }
    })

    $("#btnEditShop").click(function () {
        var selectCount = $("input[name='cbOnePOP']:checked").length;
        if (selectCount > 1) {
            layer.msg('一次只能修改一行');
            return false;
        }
        var orderId = $("input[name='cbOnePOP']:checked:first").val() || 0;
        if (orderId > 0) {
            Order.model.Id = orderId;

            $.ajax({
                type: "get",
                url: "handler/List.ashx",
                data: { type: 'getOrder', orderId: orderId },
                success: function (data) {
                    if (data != "") {
                        var json = eval(data);
                        if (json.length > 0) {
                            $("#labShopNo").text(json[0].ShopNo);
                            $("#labShopName").text(json[0].ShopName);
                            $("#txtPOSScale").val(json[0].POSScale);
                            $("#ddl_shopMaterialSupport option").each(function () {
                                if ($(this).val().toLowerCase() == json[0].MaterialSupport.toLowerCase()) {
                                    $(this).attr("selected", "selected");
                                }
                            })
                            $("#tb_ShopChannel").val(json[0].Channel);
                            $("#tb_ShopFormat").val(json[0].Format);
                            $("#ddl_ShopCityTier").val(json[0].CityTier);
                            $("#ddl_IsInstall").val(json[0].IsInstall);
                            $("input:radio[name$='rbl_ShopStatus']").each(function () {
                                if ($(this).val() == json[0].ShopStatus) {
                                    this.checked = selected = true;
                                }
                            })
                            layer.open({
                                type: 1,
                                time: 0,
                                title: '修改店铺信息',
                                skin: 'layui-layer-rim', //加上边框
                                area: ['760', '500'],
                                content: $("#editShopDiv"),
                                id: 'popLayer',
                                btn: ['提 交'],
                                btnAlign: 'c',
                                yes: function () {
                                    Order.submitShopEdit();
                                },
                                cancel: function () {
                                    $("#editShopDiv").hide();
                                    layer.closeAll();
                                }

                            });
                        }
                    }
                }
            })

        }



    })

    $("input[name='rblOrderType']").change(function () {
        var type = $(this).val();
        if (type > 3) {
            $("tr.pop").hide();
            $("tr.price").show();
        }
        else {
            $("tr.pop").show();
            $("tr.price").hide();
        }
    })
})
var Order = {
    model: function () {
        this.Id = 0;
        this.OrderType = 0;
        this.SubjectId = 0;
        this.RegionSupplementId = 0;
        this.ShopNo = "";
        this.Sheet = "";
        this.POSScale = "";
        this.MaterialSupport = "";
        this.MachineFrame = "";
        this.PositionDescription = "";
        this.Gender = "";
        this.Quantity = 0;
        this.GraphicWidth = "";
        this.GraphicLength = "";
        this.GraphicMaterial = "";
        this.ChooseImg = "";
        this.Remark = "";
        this.Channel = "";
        this.Format = "";
        this.CityTier = "";
        this.IsInstall = "";
        this.ShopStatus = "";
        this.OrderPrice = 0;
        this.PayOrderPrice = 0;


    },
    getSheetList: function () {
        var shopNo = $.trim($("#txtSearchShopNo").val());
        document.getElementById("seleSheet").length = 1;
        document.getElementById("seleGender").length = 1;
        $.ajax({
            type: "get",
            url: "handler/List.ashx",
            data: { type: 'getSheetList', subjectId: subjectId, shopNo: shopNo },
            success: function (data) {
                if (data != "") {

                    var json = JSON.parse(data);
                    var sheetJson = json[0].Sheet;
                    var genderJson = json[0].Gender;
                    for (var i = 0; i < sheetJson.length; i++) {
                        var option = "<option value='" + sheetJson[i].SheetName + "'>" + sheetJson[i].SheetName + "</option>";
                        $("#seleSheet").append(option);
                    }
                    for (var i = 0; i < genderJson.length; i++) {
                        var option = "<option value='" + genderJson[i].GenderName + "'>" + genderJson[i].GenderName + "</option>";
                        $("#seleGender").append(option);
                    }
                }
            }
        });
    },
    getList: function (curr) {
        var loadIndex = layer.load(0, { shade: false });
        var pageSize = 20;
        var shopNo = $.trim($("#txtSearchShopNo").val());
        var sheet = $("#seleSheet").val();
        var gender = $("#seleGender").val();
        
        $.getJSON('handler/List.ashx', {
            type: "getList",
            subjectId: subjectId,
            currPage: curr,
            pageSize: pageSize,
            shopNo: shopNo,
            sheet: sheet,
            gender: gender
        }, function (res) {
            //从第1页开始请求。返回的json格式可以任意定义 

            DisplayOrderList(res.rows);
            //layer.closeAll("loading");
            layer.close(loadIndex);
            layui.laypage({
                cont: 'page1', //容器。值支持id名、原生dom对象，jquery对象。【如该容器为】：<div id="page1"></div>  
                pages: Math.ceil(res.pageCount / pageSize),  //通过后台拿到的总页数  Math.ceil(res.pageCount / pageSize),
                curr: curr || 1, //初始化当前页  
                skin: '#1E9FFF', //皮肤颜色  
                groups: 6, //连续显示分页数  
                //skip: true, //是否开启跳页  
                first: '首页', //若不显示，设置false即可  
                last: '尾页', //若不显示，设置false即可  
                //prev: '<', //若不显示，设置false即可  
                //next: '>', //若不显示，设置false即可  
                jump: function (obj, first) { //触发分页后的回调 
                    if (!first) {
                        currPage = obj.curr;
                        Order.getList(obj.curr);
                    }

                }
            });

        });
    },
    bindSheet: function () {
        $("#ddlSheetMenu").html("");
        $.ajax({
            type: "get",
            url: "/Customer/Handler/POPList.ashx?type=getSheetList",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var li = "<li>" + json[i].SheetName + "</li>";
                        $("#ddlSheetMenu").append(li);
                    }
                }
            }
        })
    },
    bindMaterialCategory: function (cid) {
        document.getElementById("ddlMaterialCategory").length = 1;
        $.ajax({
            type: "get",
            url: "/Customer/Handler/POPList.ashx?type=getMaterialCategory",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (cid == json[i].Id) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var li = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].CategoryName + "</option>";
                        $("#ddlMaterialCategory").append(li);
                    }
                    if (isSelected) {
                        Order.getMaterial();
                    }
                }
            }
        })
    },
    getMaterial: function () {

        document.getElementById("ddlMaterial").length = 1;
        var categoryId = $("#ddlMaterialCategory").val();
        $.ajax({
            type: "get",
            url: "/Customer/Handler/POPList.ashx",
            data: { type: "getOrderMaterial", categoryId: categoryId },
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currMaterial == json[i].OrderMaterialName) {
                            selected = "selected='selected'";
                        }
                        var li = "<option value='" + json[i].OrderMaterialName + "' " + selected + ">" + json[i].OrderMaterialName + "</option>";
                        $("#ddlMaterial").append(li);
                    }
                }
            }
        })
    },
    getMachineFrame: function (Frame) {
        var sheet = $.trim($("#txtSheet").val());
        document.getElementById("ddlMachineFrame").length = 1;
        $.ajax({
            type: "get",
            url: "/Customer/Handler/MachineFrame.ashx?type=getFrameList&sheet=" + sheet,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].FrameName == Frame)
                            selected = "selected='selected'";
                        var option = "<option value='" + json[i].FrameName + "' " + selected + ">" + json[i].FrameName + "</option>";
                        $("#ddlMachineFrame").append(option);
                    }

                }
            }
        })
    },
    submit: function () {
        if (CheckVal()) {
            jsonStr = '{"Id":' + (this.model.Id || 0) + ',"SubjectId":' + this.model.SubjectId + ',"RegionSupplementId":' + this.model.RegionSupplementId + ',"OrderType":' + this.model.OrderType + ',"ShopNo":"' + this.model.ShopNo + '","PositionDescription":"' + this.model.PositionDescription + '","Sheet":"' + this.model.Sheet + '","POSScale":"' + this.model.POSScale + '","MaterialSupport":"' + this.model.MaterialSupport + '","MachineFrame":"' + this.model.MachineFrame + '","Gender":"' + this.model.Gender + '","Quantity":' + this.model.Quantity + ',"GraphicWidth":"' + this.model.GraphicWidth + '","GraphicLength":"' + this.model.GraphicLength + '","GraphicMaterial":"' + this.model.GraphicMaterial + '","Remark":"' + this.model.Remark + '","ChooseImg":"' + this.model.ChooseImg + '","OrderPrice":' + this.model.OrderPrice + ',"PayOrderPrice":' + this.model.PayOrderPrice + '}';
            var loadIndex = layer.load(0, { shade: false });
            $.ajax({
                type: "post",
                url: "handler/List.ashx",
                data: { type: 'edit', jsonStr: jsonStr },
                success: function (data) {
                    if (data == "ok") {
                        layer.msg("提交成功！");
                        $("#editDiv").hide();
                        layer.closeAll();
                        Order.getList(currPage);
                    }
                    else {
                        layer.confirm(data);
                    }
                }
            })
        }
    },
    submitShopEdit: function () {
        if (CheckShopEditVal()) {
            jsonStr = '{"Id":' + (this.model.Id || 0) + ',"SubjectId":' + subjectId + ',"Channel":"' + this.model.Channel + '","Format":"' + this.model.Format + '","CityTier":"' + this.model.CityTier + '","IsInstall":"' + this.model.IsInstall + '","POSScale":"' + this.model.POSScale + '","MaterialSupport":"' + this.model.MaterialSupport + '","ShopStatus":"' + this.model.ShopStatus + '"}';

            $.ajax({
                type: "post",
                url: "handler/List.ashx",
                data: { type: 'editShopInfo', jsonStr: jsonStr },
                success: function (data) {
                    if (data == "ok") {
                        layer.msg("更新成功！");
                        $("#editShopDiv").hide();
                        layer.closeAll();
                        Order.getList(currPage);
                    }
                    else {
                        layer.confirm(data);
                    }
                }
            })
        }
    }
};

function DisplayOrderList(json) {
   
    if (json.length > 0) {
        $("#tbodyOrderEmpty").hide();
        var tr = "";
        for (var i in json) {
            var state = "正常";
            if (json[i].IsDelete == 1) {
                tr += "<tr style='color:red'>";
                state = "已删除";
            }
            else
                tr += "<tr>";
            
            tr += "<td><input type='checkbox' name='cbOnePOP' value='" + json[i].Id + "'></td>";
            tr += "<td>" + json[i].rowIndex + "</td>";
            tr += "<td>" + json[i].SubjectName + "</td>";
            tr += "<td>" + json[i].OrderTypeName + "</td>";
            tr += "<td>" + json[i].ShopNo + "</td>";
            tr += "<td>" + json[i].ShopName + "</td>";
            tr += "<td>" + json[i].Region + "</td>";
            tr += "<td>" + json[i].Province + "</td>";
            tr += "<td>" + json[i].City + "</td>";
            
            tr += "<td>" + json[i].Format + "</td>";
            tr += "<td>" + json[i].MaterialSupport + "</td>";
            tr += "<td>" + json[i].POSScale + "</td>";

            tr += "<td>" + json[i].CityTier + "</td>";
            tr += "<td>" + json[i].IsInstall + "</td>";

            if (json[i].OrderType > 3) {
                tr += "<td></td>";
                tr += "<td></td>";
                tr += "<td></td>";
                tr += "<td>" + json[i].Quantity + "</td>";
                tr += "<td>" + json[i].ReceivePrice + "</td>";
                tr += "<td>" + json[i].PayPrice + "</td>";
                tr += "<td></td>";
                tr += "<td></td>";
                tr += "<td></td>";
                tr += "<td></td>";
                tr += "<td></td>";
            } else {
                tr += "<td>" + json[i].Sheet + "</td>";
                tr += "<td>" + json[i].MachineFrame + "</td>";
                tr += "<td>" + json[i].Gender + "</td>";
                tr += "<td>" + json[i].Quantity + "</td>";
                tr += "<td></td>";
                tr += "<td></td>";
                tr += "<td>" + json[i].GraphicWidth + "</td>";
                tr += "<td>" + json[i].GraphicLength + "</td>";
                tr += "<td>" + json[i].GraphicMaterial + "</td>";
                tr += "<td>" + json[i].PositionDescription + "</td>";
                tr += "<td>" + json[i].ChooseImg + "</td>";
            }
            
            tr += "<td>" + json[i].Remark + "</td>";
            tr += "<td>" + state + "</td>";
            if (json[i].ShopStatus == "关闭") {
                tr += "<td style='color:red;'>" + json[i].ShopStatus + "</td>";
            }
            else
                tr += "<td>" + json[i].ShopStatus + "</td>";
            tr += "</tr>";
        }
        $("#tbodyOrderData").html(tr).show();
    }
    else {
        $("#tbodyOrderEmpty").show();
        $("#tbodyOrderData").html("").hide();
    }
}

function showSheetMenu() {

    $("#divSheetMenu").css({ left: 0 + "px", top: 19 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function showGenderMenu() {

    $("#divGenderMenu").css({ left: 0 + "px", top: 19 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function hideMenu() {
    $("#divSheetMenu,#divGenderMenu").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "txtSheet" || event.target.id == "divSheetMenu" || $(event.target).parents("#divSheetMenu").length > 0 || event.target.id == "txtGender" || event.target.id == "divGenderMenu" || $(event.target).parents("#divGenderMenu").length > 0)) {
        hideMenu();
    }

}

function ClearVal() {
    isAdd = 0;
    currMaterial = "";
    $("#editDiv").find("input").each(function () {
        if ($(this).prop("name") != "rblOrderType") {
            $(this).val("");
        }
    })
    $("input:radio[name^='rblOrderType']:first").attr("checked", "checked");
    $("#txtShopNo").attr("disabled", false);
    $("input:radio[name^='rblOrderType']").attr("disabled", false);
    $("#txtQuantity").val("1");
    $("#ddlSubjectList").val("0");
    document.getElementById("ddlMachineFrame").length = 1;
    document.getElementById("ddlMaterial").length = 1;
    Order.model.Id = 0;
    Order.model.OrderType = 0;
    Order.model.ShopNo = "";
    Order.model.Sheet = "";
    Order.model.POSScale = "";
    Order.model.MaterialSupport = "";
    Order.model.MachineFrame = "";
    Order.model.PositionDescription = "";
    Order.model.Gender = "";
    Order.model.Quantity = 0;
    Order.model.GraphicWidth = 0;
    Order.model.GraphicLength = 0;
    Order.model.GraphicMaterial = "";
    Order.model.ChooseImg = "";
    Order.model.Remark = "";
    Order.model.OrderPrice = 0;
    Order.model.PayOrderPrice = 0;
    Order.model.SubjectId = 0;
    Order.model.RegionSupplementId = 0;
}

function CheckVal() {
    var orderType = $("input:radio[name^='rblOrderType']:checked").val() || 1;
    var ShopNo = $.trim($("#txtShopNo").val());
    var PositionDescription = $.trim($("#txtPositionDescription").val());
    var Sheet = $.trim($("#txtSheet").val());
    var POSScale = $.trim($("#txtPOSScale").val());
    var MaterialSupport = $("#ddlMaterialSupport").val();
    
    var MachineFrame = $("#ddlMachineFrame").val();
    var Gender = $.trim($("#txtGender").val());
    var Quantity = $.trim($("#txtQuantity").val());
    var GraphicWidth = $.trim($("#txtGraphicWidth").val())||0
    var GraphicLength = $.trim($("#txtGraphicLength").val())||0;
    var GraphicMaterial = $("#ddlMaterial").val();

    var ChooseImg = $.trim($("#txtChooseImg").val());
    var Remark = $.trim($("#txtRemark").val());
    var ReceivePrice = $.trim($("#txtReceivePrice").val())||0;
    var PayPrice = $.trim($("#txtPayPrice").val()) || 0;
    var realSubjectId = 0;
    if ($("#hfIsRegionSubject").val() == "1") {
        $("tr.showSubjectList").show();
        realSubjectId = $("#ddlSubjectList").val();
        if (realSubjectId == 0) {
            layer.msg("请选择所属项目");
            return false;
        }
        Order.model.SubjectId = realSubjectId;
        Order.model.RegionSupplementId = subjectId;
    }
    else {
        Order.model.SubjectId = subjectId;
        Order.model.RegionSupplementId = 0;
    }
    if (ShopNo == "") {
        layer.msg("请填写店铺编号");
        return false;
    }
    if (orderType > 3) {
        if (ReceivePrice == "") {
            layer.msg("请填写应收费用");
            return false;
        }
        if (isNaN(ReceivePrice) || parseFloat(ReceivePrice) == 0) {
            layer.msg("应收费用填写不正确");
            return false;
        }
        if (PayPrice == "") {
            layer.msg("请填写应付费用");
            return false;
        }
        if (isNaN(PayPrice) || parseFloat(PayPrice) == 0) {
            layer.msg("应付费用填写不正确");
            return false;
        }
    }
    else {
        if (Sheet == "") {
            layer.msg("请填写POP位置");
            return false;
        }
        if (Quantity == "") {
            layer.msg("请填写数量");
            return false;
        }
        if (isNaN(Quantity)) {
            layer.msg("数量填写不正确");
            return false;
        }
        if (orderType == 1) {
            if (Gender == "") {
                layer.msg("请填写性别");
                return false;
            }
            if (GraphicWidth == "") {
                layer.msg("请填写POP宽");
                return false;
            }
            if (GraphicWidth != "" && isNaN(GraphicWidth)) {
                layer.msg("POP宽填写不正确");
                return false;
            }
            if (GraphicLength == "") {
                layer.msg("请填写POP高");
                return false;
            }
            if (GraphicLength != "" && isNaN(GraphicLength)) {
                layer.msg("POP高填写不正确");
                return false;
            }
            if (GraphicMaterial == "") {
                layer.msg("请选择材质");
                return false;
            }
        }
        else {
            if (GraphicWidth != "" && isNaN(GraphicWidth)) {
                layer.msg("POP宽填写不正确");
                return false;
            }
            if (GraphicLength != "" && isNaN(GraphicLength)) {
                layer.msg("POP高填写不正确");
                return false;
            }
        }
    }
    Order.model.OrderType = orderType;
    Order.model.ShopNo = ShopNo;
    Order.model.Sheet = Sheet;
    Order.model.POSScale = POSScale;
    Order.model.MaterialSupport = MaterialSupport;
    Order.model.MachineFrame = MachineFrame;
    Order.model.PositionDescription = PositionDescription;
    Order.model.Gender = Gender;
    Order.model.Quantity = Quantity;
    Order.model.GraphicWidth = GraphicWidth;
    Order.model.GraphicLength = GraphicLength;
    Order.model.GraphicMaterial = GraphicMaterial;
    Order.model.ChooseImg = ChooseImg;
    Order.model.Remark = Remark;
    Order.model.OrderPrice = ReceivePrice;
    Order.model.PayOrderPrice = PayPrice;
    
    return true;
}



function CheckShopEditVal() {


    var POSScale = $("#tb_shopPOSScale").val();
    var materialSupport = $("#ddl_shopMaterialSupport").val();


    var channel = $.trim($("#tb_ShopChannel").val());
    var format = $.trim($("#tb_ShopFormat").val());
    var cityTier = $("#ddl_ShopCityTier").val();
    var isInstall = $("#ddl_IsInstall").val();

    var shopStatus = $("input:radio[name='rbl_ShopStatus']:checked").val()||"正常";

    if (materialSupport == "") {
        layer.msg("请选择物料支持级别");
        return false;
    }
    if (channel == "") {
        layer.msg("请填写channel");
        return false;
    }

    if (format == "") {
        layer.msg("请填写format");
        return false;
    }
    if (cityTier == "") {
        layer.msg("请选择城市级别");
        return false;
    }
    if (isInstall == "") {
        layer.msg("请选择是否安装");
        return false;
    }
   
   
    Order.model.POSScale = POSScale;
    Order.model.MaterialSupport = materialSupport;
    Order.model.Channel = channel;
    Order.model.Format = format;
    Order.model.CityTier = cityTier;
    Order.model.IsInstall = isInstall;
    Order.model.ShopStatus = shopStatus;
   
    return true;
}
