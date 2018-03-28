
var currOutsourceId = "";
var currOutsourceName = "";
var guidanceIds = "";
var subjectIds = "";
var oType = "";
var activeTabIndex = 0;
var firstLoad = 1;
var materialCategoryId = "";
var orderShopNo = "";
var exportType = "";
var materialName = "";

$(function () {
    GetOutsourceList();
    //Order.getOrderList(orderPageIndex, orderPageSize);

    $("#guidanceDiv").delegate("input[name='guidanceCB']", "change", function () {
        GetSubjectList();
        GetMaterialCategory();
    })

    $("#guidanceCBALL").click(function () {
        var checked = this.checked;
        $("input[name='guidanceCB']").each(function () {
            this.checked = checked;
        })
        GetSubjectList();
        GetMaterialCategory();
    })

    $("#subjectCBALL").click(function () {
        var checked = this.checked;
        $("input[name='subjectcb']").each(function () {
            this.checked = checked;
        })

        GetMaterialCategory();
    })

    $("#subjectDiv").delegate("input[name='subjectcb']", "change", function () {

        GetMaterialCategory();
    })

    $("#btnSearch").click(function () {
        GetCondition();
        Order.getOrderList(orderPageIndex, orderPageSize);
    })
    layui.use('element', function () {
        var element = layui.element();

        //tab事件监听
        element.on('tab(order)', function (data) {
            activeTabIndex = data.index;

            if (activeTabIndex == 1 && firstLoad == 1) {
                //firstLoad = 0;
                //$("#Button1").click();
            }
        });

    });

    $("#txtSearchOrderShopNo").searchbox({
        width: 300,
        searcher: doSearchOrder,
        prompt: '请输入店铺编号,可以输入多个,逗号分隔'
    })

    $("#btnRefresh").click(function () {
        Order.getOrderList(orderPageIndex, orderPageSize);
    })

    //导出喷绘王模板-北京
    $("#btnExportBJPHW").click(function () {

        GetCondition();
        if (currOutsourceId == 0) {
            alert("请选择外协");
            return false;
        }
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }

        Export("exportbjphw");

    })
    //导出喷绘王模板-外协
    $("#btnExportOtherPHW").click(function () {
        GetCondition();
        if (currOutsourceId == 0) {
            alert("请选择外协");
            return false;
        }
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }

        Export("exportotherphw");

    })

    //导出350
    $("#btnExport350").click(function () {
        GetCondition();
        if (currOutsourceId == 0) {
            alert("请选择外协");
            return false;
        }
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }

        //$(this).attr("disabled", true).next("img").show();
        //checkExportBJPHW($(this));
        //var url = "Handler/ExportOrders.ashx?type=exportbjphw&subjectids=" + subjectId;
        Export("export350");

    })

    $("input[name^='cblExportType']").change(function () {

        if (this.checked) {

            $(this).siblings("input").each(function () {
                this.checked = false;
            });
        }
    })
    $("input[name^='cblMaterial']").change(function () {

        if (this.checked) {
            $(this).siblings("input").each(function () {
                this.checked = false;
            });
        }
    })
})


var orderPageIndex = 1;
var orderPageSize = 20;
var currMaterial = "";
var Order = {
    getOrderList: function (pageIndex, pageSize) {
        $("#divContent1").css({ height: "600px" });
        orderShopNo = $.trim($("#txtSearchOrderShopNo").val());
        $("#tbOrderList").datagrid({
            queryParams: { type: "getOrderList", outsourceId: currOutsourceId, guidanceIds: guidanceIds, subjectIds: subjectIds, materialCategoryId: materialCategoryId, outsourceType: oType, shopNo: orderShopNo, currpage: pageIndex, pagesize: pageSize, exportType: exportType, materialName: materialName },
            method: 'get',
            url: 'handler/OrderList.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'OrderType', title: '类型' },
                        { field: 'ShopNo', title: '店铺编号' },
                        { field: 'ShopName', title: '店铺名称' },
                        { field: 'Region', title: '区域' },
                        { field: 'Province', title: '省份' },
                        { field: 'City', title: '城市' },
                        { field: 'CityTier', title: '城市级别' },
                        { field: 'Format', title: '店铺类型' },
                        { field: 'OrderPrice', title: '费用金额' },
                        { field: 'Sheet', title: 'POP位置' },
                        { field: 'Gender', title: '男/女' },
                        { field: 'Quantity', title: '数量' },
                        { field: 'GraphicWidth', title: 'POP宽' },
                        { field: 'GraphicLength', title: 'POP高' },
                        { field: 'GraphicMaterial', title: '材质' },
                        { field: 'ChooseImg', title: '选图' },
                        { field: 'PositionDescription', title: '位置描述' }



            ]],
            height: "100%",
            toolbar: "#toolbar",
            pageList: [20],
            striped: true,
            border: false,
            singleSelect: true,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',

            emptyMsg: '没有相关记录',
            rowStyler: function (index, row) {
                if (row.IsDelete==1) {
                    return 'color:red';
                }
            }    

        });
        var p = $("#tbOrderList").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                Order.getOrderList(curIndex, curSize);
            }
        });


    },

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
        this.OrderGraphicMaterial = "";
        this.ChooseImg = "";
        this.Remark = "";
        this.Channel = "";
        this.Format = "";
        this.CityTier = "";
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
            jsonStr = '{"Id":' + (this.model.Id || 0) + ',"OrderType":' + this.model.OrderType + ',"ShopNo":"' + this.model.ShopNo + '","PositionDescription":"' + this.model.PositionDescription + '","Sheet":"' + this.model.Sheet + '","POSScale":"' + this.model.POSScale + '","MaterialSupport":"' + this.model.MaterialSupport + '","MachineFrame":"' + this.model.MachineFrame + '","Gender":"' + this.model.Gender + '","Quantity":' + this.model.Quantity + ',"GraphicWidth":"' + this.model.GraphicWidth + '","GraphicLength":"' + this.model.GraphicLength + '","OrderGraphicMaterial":"' + this.model.OrderGraphicMaterial + '","Remark":"' + this.model.Remark + '","ChooseImg":"' + this.model.ChooseImg + '","PayOrderPrice":' + this.model.PayOrderPrice + '}';
            var loadIndex = layer.load(0, { shade: false });
            $.ajax({
                type: "post",
                url: "handler/OrderList.ashx",
                data: { type: 'edit', jsonStr: jsonStr },
                success: function (data) {
                    if (data == "ok") {
                        layer.msg("提交成功！");
                        $("#editDiv").hide();
                        layer.closeAll();
                        Order.getOrderList(orderPageIndex, orderPageSize);
                    }
                    else {
                        layer.confirm(data);
                        layer.close(loadIndex);
                    }
                }
            })
        }
    }
};

function GetOutsourceList() {

    $("#tbOutsource").datagrid({
        method: 'get',
        url: 'handler/OrderList.ashx?type=getOutsource',
        columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'CompanyName', title: '外协名称' }
                ]],
        height: '100%',
        border: false,
        fitColumns: true,
        singleSelect: true,
        rownumbers: true,
        emptyMsg: '没有相关记录',
        onClickRow: function (rowIndex, data) {
            //selectOutsource(data.Id, data.CompanyName);
            selectOutsource();
        }
    })
}

//function selectOutsource(oId, oName) {
//    currOutsourceId = oId;
//    currOutsourceName = oName;
//    $("#orderTitle").panel({
//        title: ">>外协名称：" + oName
//    });
//    GetGuidacneList();
//    ClearDiv();
//}

function selectOutsource() {
    var rows = $("#tbOutsource").datagrid("getSelections");
    var oName = "";
    currOutsourceName = "";
    currOutsourceId = "";
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            currOutsourceId += (rows[i].Id + ",");
            oName += (rows[i].CompanyName + ",");
        }
    }
    if (oName.length > 0) {
        oName = oName.substring(0, oName.length - 1);
    }
    if (currOutsourceId.length > 0) {
        currOutsourceId = currOutsourceId.substring(0, currOutsourceId.length - 1);
    }
    currOutsourceName = oName;
    $("#orderTitle").panel({
        title: ">>外协名称：" + oName
    });
    GetGuidacneList();
    ClearDiv();
}

function GetGuidacneList() {
    var customerId = $("#ddlCustomer").val();
    var guidanceMonth = $.trim($("#txtMonth").val());

    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getGuidanceList", outsourceId: currOutsourceId, customerId: customerId, guidanceMonth: guidanceMonth },
        success: function (data) {
            $("#guidanceDiv").html("");
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left;'><input type='checkbox' name='guidanceCB' value='" + json[i].GuidanceId + "' />" + json[i].GuidanceName + "&nbsp;</div>";
                    $("#guidanceDiv").append(div);
                }
            }
            else {
                $("#guidanceDiv").html("<span style='color:red;'>无活动信息！</span>");
            }
        }
    });
}

function GetSubjectList() {
    GetCondition();
    $("#ImgLoadSubject").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getSubjectList", outsourceId: currOutsourceId, guidanceIds: guidanceIds },
        complete: function () { $("#ImgLoadSubject").hide(); },
        success: function (data) {
            $("#subjectDiv").html("");
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='subjectcb' value='" + json[i].SubjectId + "' /><span>" + json[i].SubjectName + "</span>&nbsp;</div>";

                }
                $("#subjectDiv").html(div);

            }
        }
    })
}

function GetMaterialCategory() {
    GetCondition();

    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getMaterialCategory", outsourceId: currOutsourceId, guidanceIds: guidanceIds, subjectIds: subjectIds },
        beforeSend: function () { $("#ImgLoadMC").show(); },
        complete: function () { $("#ImgLoadMC").hide(); },
        success: function (data) {
            $("#MaterialCategoryDiv").html("");

            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='mcategorycb' value='" + json[i].CategoryId + "' /><span>" + json[i].CategoryName + "</span>&nbsp;</div>";

                }
                $("#MaterialCategoryDiv").html(div);

            }
        }
    })
}

function getMonth() {
    GetGuidacneList();
}

function GetCondition() {
    guidanceIds = "";
    subjectIds = "";
    materialCategoryId = "";
    oType = "";
    orderShopNo = "";
    $("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    $("input[name='subjectcb']:checked").each(function () {
        subjectIds += $(this).val() + ",";
    })
    $("input[name='mcategorycb']:checked").each(function () {
        materialCategoryId += $(this).val() + ",";
    })
    $("input[name^='cblOutsourceType']:checked").each(function () {
        oType += $(this).val() + ",";
    })
    exportType = "";
    exportType = $("input[name^='cblExportType']:checked").val() || "";
    materialName = "";
    materialName = $("input[name^='cblMaterial']:checked").val() || "";
    orderShopNo = $.trim($("#txtSearchOrderShopNo").val());
    if (guidanceIds != "") {
        guidanceIds = guidanceIds.substring(0, guidanceIds.length - 1);
    }
    if (subjectIds != "") {
        subjectIds = subjectIds.substring(0, subjectIds.length - 1);
    }
    if (materialCategoryId != "") {
        materialCategoryId = materialCategoryId.substring(0, materialCategoryId.length - 1);
    }
    if (oType != "") {
        oType = oType.substring(0, oType.length - 1);
    }
}

function ClearDiv() {
    $("#subjectDiv").html("");
    $("#MaterialCategoryDiv").html("");
    guidanceIds = "";
    subjectIds = "";
    oType = "";
    materialCategoryId = "";
    orderShopNo = "";
    //$("#tbOrderList").datagrid('loadData',{ total: 0, rows: [] });
    var item = $('#tbOrderList').datagrid('getRows');
    for (var i = item.length - 1; i >= 0; i--) {
        var index = $('#tbOrderList').datagrid('getRowIndex', item[i]);
        $('#tbOrderList').datagrid('deleteRow', index);
    }
}

//查询上个月活动
$("#spanUp").click(function () {
    var month1 = $.trim($("#txtMonth").val());
    if (month1 != "") {
        month1 = month1.replace(/-/g, "/");
        var date = new Date(month1);
        date.setMonth(date.getMonth() - 1);
        $("#txtMonth").val(date.Format("yyyy-MM"));
        GetGuidacneList();
    }

})

//查询下个月活动
$("#spanDown").click(function () {
    var month1 = $.trim($("#txtMonth").val());
    if (month1 != "") {
        month1 = month1.replace(/-/g, "/");
        var date = new Date(month1);
        date.setMonth(date.getMonth() + 1);
        $("#txtMonth").val(date.Format("yyyy-MM"));
        GetGuidacneList();
    }
})


function doSearchOrder() {
    Order.getOrderList(orderPageIndex, orderPageSize);
}

function Export(type) {

    var typeName = "";
    $("input[name^='cblOutsourceType']:checked").each(function () {
        typeName += $(this).next().text() + ",";
    })
    var fileName = currOutsourceName;
    if (typeName != "") {
        typeName = typeName.substring(0, typeName.length - 1);
        fileName = fileName + "(" + typeName + ")";
    }
    //exportType: exportType, materialName: materialName
    var url = "ExportHelper.aspx?type=" + type + "&outsourceId=" + currOutsourceId + "&guidanceIds=" + guidanceIds + "&subjectIds=" + subjectIds + "&outsourceType=" + oType + "&shopNo=" + orderShopNo + "&materialCategoryId=" + materialCategoryId + "&fileName=" + fileName + "&exportType=" + exportType + "&materialName=" + materialName;

    $("#exportFrame").attr("src", url);
}




$(function () {




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
})

function editOrder() {
    //ClearVal();
    var row = $("#tbOrderList").datagrid("getSelected");
    if (row == null) {
        layer.msg("请选择要编辑的行");
    }
    else {
        var orderId = row.Id;
        Order.model.Id = orderId;
        Order.bindSheet();
        $.ajax({
            type: "get",
            url: "handler/OrderList.ashx",
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
                        if (json[0].OrderType > 3) {
                            $("tr.pop").hide();
                            $("tr.price").show();
                            $("#txtReceivePrice").val(json[0].OrderPrice);
                            $("#txtPayPrice").val(json[0].PayOrderPrice);
                            $("#txtSheet").val("").attr("disabled", "disabled");
                        }
                        else {
                            $("tr.pop").show();
                            $("tr.price").hide();
                            $("#txtReceivePrice").val("");
                            $("#txtPayPrice").val("");
                            $("#txtSheet").attr("disabled", false);
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
   
}

function deleteOrder() {
    var row = $("#tbOrderList").datagrid("getSelected");
    if (row == null) {
        layer.msg("请选择要编辑的行");
    }
    else if (row.IsDelete == 0) {
        var orderId = row.Id;
        $.ajax({
            type: "get",
            url: "handler/OrderList.ashx",
            data: { type: 'delete', orderId: orderId },
            success: function (data) {
                if (data == "ok") {
                    Order.getOrderList(orderPageIndex, orderPageSize);
                }
                else {
                    layer.msg(data);
                }
            }
        });
    }
    else {
        layer.msg("已删除");
    }
}

function recoverOrder() {
    var row = $("#tbOrderList").datagrid("getSelected");
    if (row == null) {
        layer.msg("请选择要编辑的行");
    }
    else {
      
        if (row.IsDelete == 1) {
            var orderId = row.Id;
            $.ajax({
                type: "get",
                url: "handler/OrderList.ashx",
                data: { type: 'recover', orderId: orderId },
                success: function (data) {
                    if (data == "ok") {
                        Order.getOrderList(orderPageIndex, orderPageSize);
                    }
                    else {
                        layer.msg(data);
                    }
                }
            });
        }
        else {
            layer.msg("该订单没删除，不能恢复");
        }
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
    Order.model.OrderGraphicMaterial = "";
    Order.model.ChooseImg = "";
    Order.model.Remark = "";

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
    var GraphicWidth = $.trim($("#txtGraphicWidth").val()) || 0
    var GraphicLength = $.trim($("#txtGraphicLength").val()) || 0;
    var GraphicMaterial = $("#ddlMaterial").val();

    var ChooseImg = $.trim($("#txtChooseImg").val());
    var Remark = $.trim($("#txtRemark").val());
    var PayPrice = $.trim($("#txtPayPrice").val()) || 0;

    if (ShopNo == "") {
        layer.msg("请填写店铺编号");
        return false;
    }
    if (orderType > 3) {
        if (PayPrice == "") {
            layer.msg("请填写应付费用");
            return false;
        }
        if (isNaN(PayPrice) || parseFloat(PayPrice) == 0) {
            layer.msg("应付费用填写不正确");
            return false;
        }
        Remark = $.trim($("#txtPriceRemark").val());
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
    Order.model.OrderGraphicMaterial = GraphicMaterial;
    Order.model.ChooseImg = ChooseImg;
    Order.model.Remark = Remark;
    Order.model.PayOrderPrice = PayPrice;

    return true;
}

