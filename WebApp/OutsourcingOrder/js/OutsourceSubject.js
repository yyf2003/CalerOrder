


//var guidanceIds = "";
//var subjectIds = "";
//var oType = "";
//var activeTabIndex = 0;
//var firstLoad = 1;
//var materialCategoryId = "";
//var orderShopNo = "";
//var materialName = "";

var currMaterial = "";

$(function () {
    GetGuidacneList();

    Order.getOrderList(pageIndex, pageSize);

    $("#guidanceDiv").delegate("input[name='guidanceCB']", "change", function () {
        GetRegion();
    });
    $("#RegionDiv").delegate("input[name='regioncb']", "change", function () {
        GetOutsource();
        GetSubjectCategory();
    });
    $("#subjectCategoryDiv").delegate("input[name='categorycb']", "change", function () {
        GetOutsource();
        GetSubjectList();
    })
    $("#subjectDiv").delegate("input[name='subjectcb']", "change", function () {
        GetOutsource();
        GetProvince();
    })

    $("#ProvinceDiv").delegate("input[name='provincecb']", "change", function () {
        GetOutsource();
        GetCity();
    })

    $("#CityDiv").delegate("input[name='citycb']", "change", function () {
        GetOutsource();
        GetMaterialCategory();
    })

    $("#guidanceCBALL").click(function () {
        var checked = this.checked;
        $("input[name='guidanceCB']").each(function () {
            this.checked = checked;
        })
        GetRegion();

    })

    $("#subjectCBALL").click(function () {
        var checked = this.checked;
        $("input[name='subjectcb']").each(function () {
            this.checked = checked;
        })
        GetProvince();

    })

    $("#provinceCBAll").click(function () {
        var checked = this.checked;
        $("input[name='provincecb']").each(function () {
            this.checked = checked;
        })
        GetCity();

    })

    $("#cityCBAll").click(function () {
        var checked = this.checked;
        $("input[name='citycb']").each(function () {
            this.checked = checked;
        })
        GetMaterialCategory();
    })

    $("#txtSearchShopNo").searchbox({
        width: 300,
        searcher: doSearch,
        prompt: '请输入店铺编号,可以输入多个,逗号分隔'
    })


    $("#btnSearch").click(function () {
        Order.getOrderList(pageIndex, pageSize);
    })

    $("#btnIRefresh").click(function () {
        $("#tbOrderList").datagrid("reload");
    })

    //编辑
    $("#btnEdit").click(function () {
        editOrder();
    })

    //删除
    $("#btnDelete").click(function () {
        deleteOrder();
    })

    //恢复
    $("#btnRecover").click(function () {
        recoverOrder();
    })

    //修改外协
    $("#btnChangeOS").click(function () {

        changeOutsource();
    })

    //批量修改外协
    $("#btnBatchChangeOS").click(function () {
        var guidanceMonth = $("#txtMonth").val();
        if (guidanceMonth == "") {
            layer.msg("请选择活动月份");
            return false;
        }
        changeOutsourceBatch();
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
})


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



function GetGuidacneList() {
    var customerId = $("#ddlCustomer").val();
    var guidanceMonth = $.trim($("#txtMonth").val());
    $("#ImgLoadGuidance").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getGuidanceList", customerId: customerId, guidanceMonth: guidanceMonth },
        complete: function () { $("#ImgLoadGuidance").hide(); },
        success: function (data) {
            $("#guidanceDiv").html("");
            $("#guidanceCBALL").prop("checked", false);
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

function GetRegion() {
    var guidanceIds = "";
    $("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    $("#ImgLoadRegion").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getRegion", guidanceIds: guidanceIds },
        complete: function () { $("#ImgLoadRegion").hide(); },
        success: function (data) {
            $("#RegionDiv").html("");
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='regioncb' value='" + json[i].RegionName + "' /><span>" + json[i].RegionName + "</span>&nbsp;</div>";

                }
                $("#RegionDiv").html(div);

            }
            GetOutsource();
            GetSubjectCategory();

        }
    })
}

function GetSubjectCategory() {

    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })

    $("#ImgLoadSubjectCategory").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getSubjectCategory", region: region },
        complete: function () { $("#ImgLoadSubjectCategory").hide(); },
        success: function (data) {
            $("#subjectCategoryDiv").html("");
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='categorycb' value='" + json[i].CategoryId + "' /><span>" + json[i].CategoryName + "</span>&nbsp;</div>";

                }
                $("#subjectCategoryDiv").html(div);

            }

            GetSubjectList();
        }
    })
}

function GetSubjectList() {

    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    var category = "";
    $("input[name='categorycb']:checked").each(function () {
        category += $(this).val() + ",";
    })

    $("#ImgLoadSubject").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getSubjectList", region: region, category: category },
        cache: false,
        complete: function () { $("#ImgLoadSubject").hide(); },
        success: function (data) {
            $("#subjectDiv").html("");
            $("#subjectCBALL").prop("checked", false);
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='subjectcb' value='" + json[i].SubjectId + "' /><span>" + json[i].SubjectName + "</span>&nbsp;</div>";

                }
                $("#subjectDiv").html(div);

            }

            GetProvince();
        }
    })
}

function GetProvince() {
    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    var category = "";
    $("input[name='categorycb']:checked").each(function () {
        category += $(this).val() + ",";
    })
    var subjectId = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    $("#ImgLoadProvince").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getProvince", region: region, category: category, subjectIds: subjectId },
        cache: false,
        complete: function () { $("#ImgLoadProvince").hide(); },
        success: function (data) {
            $("#ProvinceDiv").html("");
            $("#provinceCBAll").prop("checked", false);
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='provincecb' value='" + json[i].Province + "' /><span>" + json[i].Province + "</span>&nbsp;</div>";

                }
                $("#ProvinceDiv").html(div);

            }

            GetCity();
        }
    })
}

function GetCity() {
    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    var category = "";
    $("input[name='categorycb']:checked").each(function () {
        category += $(this).val() + ",";
    })
    var subjectId = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    var province = "";
    $("input[name='provincecb']:checked").each(function () {
        province += $(this).val() + ",";
    })
    $("#ImgLoadCity").show();
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getCity", region: region, category: category, subjectIds: subjectId, province: province },
        cache: false,
        complete: function () { $("#ImgLoadCity").hide(); },
        success: function (data) {
            $("#CityDiv").html("");
            $("#cityCBAll").prop("checked", false);
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='citycb' value='" + json[i].City + "' /><span>" + json[i].City + "</span>&nbsp;</div>";

                }
                $("#CityDiv").html(div);

            }

            GetMaterialCategory();
        }
    })
}

function GetOutsource() {
    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    var category = "";
    $("input[name='categorycb']:checked").each(function () {
        category += $(this).val() + ",";
    })
    var subjectId = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    var province = "";
    $("input[name='provincecb']:checked").each(function () {
        province += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getOrderOutsource", subjectCategoryIds: category, subjectIds: subjectId, region: region, category: category, province: province, city: city },
        beforeSend: function () { $("#ImgLoadOutsource").show(); },
        complete: function () { $("#ImgLoadOutsource").hide(); },
        success: function (data) {
            $("#OutsourceDiv").html("");

            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='outsourcecb' value='" + json[i].OutsourceId + "' /><span>" + json[i].OutsourceName + "</span>&nbsp;</div>";

                }
                $("#OutsourceDiv").html(div);

            }
        }
    })
}


function GetMaterialCategory() {
    //GetCondition();
    var region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    var category = "";
    $("input[name='categorycb']:checked").each(function () {
        category += $(this).val() + ",";
    })
    var subjectId = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    var province = "";
    $("input[name='provincecb']:checked").each(function () {
        province += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    var outsourceId = "";
    $("input[name='outsourcecb']:checked").each(function () {
        outsourceId += $(this).val() + ",";
    })
    $.ajax({
        type: "get",
        url: "handler/OrderList.ashx",
        data: { type: "getMaterialCategory", subjectIds: subjectId, region: region, category: category, province: province, city: city, outsourceId: outsourceId },
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

//查询上个月活动
$("#spanUp").click(function () {
    var month1 = $.trim($("#txtMonth").val());
    if (month1 != "") {
        month1 = month1.replace(/-/g, "/");
        var date = new Date(month1);
        date.setMonth(date.getMonth() - 1);
        $("#txtMonth").val(date.getFullYear() + "-" + (date.getMonth() + 1));
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
        $("#txtMonth").val(date.getFullYear() + "-" + (date.getMonth() + 1));
        GetGuidacneList();
    }
})

function doSearch() {
    Order.getOrderList(pageIndex, pageSize);
}


var pageIndex = 1, pageSize = 15;
var Loadingindex;
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
        this.OrderGraphicMaterial = "";
        this.ChooseImg = "";
        this.Remark = "";
        this.Channel = "";
        this.Format = "";
        this.CityTier = "";
        this.PayOrderPrice = 0;
        this.ReceiveOrderPrice = 0;
    },
    getOrderList: function (pageIndex, pageSize) {
        //$("#divContent1").css({ height: "600px" });

        var region = "";
        $("input[name='regioncb']:checked").each(function () {
            region += $(this).val() + ",";
        })
        var category = "";
        $("input[name='categorycb']:checked").each(function () {
            category += $(this).val() + ",";
        })
        var subjectId = "";
        $("input[name='subjectcb']:checked").each(function () {
            subjectId += $(this).val() + ",";
        })
        var province = "";
        $("input[name='provincecb']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='citycb']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var outsourceId = "";
        $("input[name='outsourcecb']:checked").each(function () {
            outsourceId += $(this).val() + ",";
        })
        var materialCategoryId = "";
        $("input[name='mcategorycb']:checked").each(function () {
            materialCategoryId += $(this).val() + ",";
        })
        var oType = "";
        $("input[name^='cblOutsourceType']:checked").each(function () {
            oType += $(this).val() + ",";
        })
        var exportType = $("input[name^='cblExportType']:checked").val() || "";

        var materialName = $("input[name^='cblMaterial']:checked").val() || "";

        var shopNo = $.trim($("#txtSearchShopNo").val());

        $("#tbOrderList").datagrid({
            queryParams: { type: "getOrderList", region: region, subjectCategoryIds: category, subjectIds: subjectId, province: province, city: city, outsourceId: outsourceId, materialCategoryId: materialCategoryId, outsourceType: oType, shopNo: shopNo, exportType: exportType, materialName: materialName, currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: 'handler/OrderList.ashx',
            columns: [[
                        { field: 'checked', checkbox: true },
                        { field: 'rowIndex', title: '序号' },
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'OrderType', title: '订单类型' },
                        { field: 'OutsourceName', title: '外协' },
                        { field: 'AssignType', title: '类型' },
                        { field: 'ShopNo', title: '店铺编号' },
                        { field: 'ShopName', title: '店铺名称' },
                        { field: 'Region', title: '区域' },
                        { field: 'Province', title: '省份' },
                        { field: 'City', title: '城市' },
                        { field: 'CityTier', title: '城市级别' },
                        { field: 'IsInstall', title: '安装基本' },
                        { field: 'OrderPrice', title: '费用金额' },
                        { field: 'Sheet', title: 'POP位置' },
                        { field: 'Gender', title: '男/女' },
                        { field: 'Quantity', title: '数量' },
                        { field: 'GraphicWidth', title: 'POP宽' },
                        { field: 'GraphicLength', title: 'POP高' },
                        { field: 'GraphicMaterial', title: '材质' },
                        { field: 'UnitPrice', title: '单价' },
                        { field: 'ChooseImg', title: '选图' },
                        { field: 'PositionDescription', title: '位置描述' }



            ]],
            height: "100%",
            toolbar: "#toolbar",
            pageList: [15, 20],
            striped: true,
            border: false,
            singleSelect: false,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',

            emptyMsg: '没有相关记录',
            rowStyler: function (index, row) {
                if (row.IsDelete == 1) {
                    return 'color:red';
                }
            },
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

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
    submit: function () {
        if (CheckVal()) {
            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"OrderType":' + this.model.OrderType + ',"ShopNo":"' + this.model.ShopNo + '","PositionDescription":"' + this.model.PositionDescription + '","Sheet":"' + this.model.Sheet + '","POSScale":"' + this.model.POSScale + '","MaterialSupport":"' + this.model.MaterialSupport + '","MachineFrame":"' + this.model.MachineFrame + '","Gender":"' + this.model.Gender + '","Quantity":' + this.model.Quantity + ',"GraphicWidth":"' + this.model.GraphicWidth + '","GraphicLength":"' + this.model.GraphicLength + '","OrderGraphicMaterial":"' + this.model.OrderGraphicMaterial + '","Remark":"' + this.model.Remark + '","ChooseImg":"' + this.model.ChooseImg + '","PayOrderPrice":' + this.model.PayOrderPrice + ',"ReceiveOrderPrice":' + this.model.ReceiveOrderPrice + '}';
            var loadIndex = layer.load(0, { shade: false });
            $.ajax({
                type: "post",
                url: "handler/OrderList.ashx",
                data: { type: 'edit', jsonStr: jsonStr },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        layer.msg("提交成功！");
                        $("#editDiv").hide();
                        layer.closeAll();
                        Order.getOrderList(pageIndex, pageSize);
                    }
                    else {
                        layer.confirm(data);
                        layer.close(loadIndex);
                    }
                }
            })
        }
    },
    updateOutsource: function () {
        var newOutsourceId = $("#seleOutsource").val() || 0;
        var changeType = $("input[name$='rblChangeType']:checked").val() || 0;
        $.ajax({
            type: "get",
            url: "handler/OrderList.ashx",
            data: { type: 'changeOutsource', subjectId: subjectId, orderId: changeIds, newOutsourceId: newOutsourceId, changeType: changeType },
            complete: function () {
                layer.close(Loadingindex);
            },
            success: function (data) {

                if (data == "ok") {
                    Order.getOrderList(pageIndex, pageSize);
                    changeIds = "";
                    subjectId = "";
                    newOutsourceId = 0;
                    $("#divOutsource").hide();
                    layer.closeAll();
                    $("#seleOutsource").val("0");
                    //document.getElementById("seleOutsource").length = 1;
                }
                else {
                    layer.msg(data);
                }
            }
        });
    },
    updateOutsourceBatch: function () {
        //var formData = new FormData($("#form1"));  //重点：要用这种方法接收表单的参数

        var formData = new FormData();
        formData.append('file', $('#changeFile')[0].files[0]);
        var guidanceMonth = $("#txtMonth").val();
        var newOutsourceId = $("#seleOutsourceBatch").val();
        
        $.ajax({
            url: "handler/OrderList.ashx?type=changeOutsourceBatch&guidanceMonth=" + guidanceMonth + "&newOutsourceId=" + newOutsourceId,
            type: 'POST',
            data: formData,
            // 告诉jQuery不要去处理发送的数据
            processData: false,
            // 告诉jQuery不要去设置Content-Type请求头
            contentType: false,
            async: false,
            complete: function () {
                layer.close(Loadingindex);
            },
            success: function (data) {
                if (data == "ok") {
                    layer.closeAll();
                    $("#divOutsourceBatch").hide();
                    layer.msg("更新成功！");
                    Order.getOrderList(pageIndex, pageSize);
                }
                else {
                    layer.msg(data);
                }
            }
        });

    }
};


function editOrder() {
    //ClearVal();
    var row = $("#tbOrderList").datagrid("getSelections");
    if (row == null || row.length == 0) {
        layer.msg("请选择要编辑的行");
    }
    else if (row.length > 1) {
        layer.msg("只能选择一行");
    }
    else {
        var orderId = row[0].Id;
        //alert(orderId);
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
                            $("#txtPayPrice").val(json[0].PayOrderPrice);
                            $("#txtReceivePrice").val(json[0].ReceiveOrderPrice);
                            $("#txtPriceQuantity").val(json[0].Quantity);
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
    var row = $("#tbOrderList").datagrid("getSelections");
    var ids = "";
    if (row == null || row.length == 0) {
        layer.msg("请选择要编辑的行");
    }
    else {
        for (var i = 0; i < row.length; i++) {
            if (row[i].IsDelete == 0) {
                ids += row[i].Id + ",";
            }
        }
    }
    if (ids.length > 0) {

        $.ajax({
            type: "get",
            url: "handler/OrderList.ashx",
            data: { type: 'delete', orderId: ids },
            success: function (data) {
                if (data == "ok") {
                    Order.getOrderList(pageIndex, pageSize);
                }
                else {
                    layer.msg(data);
                }
            }
        });
    }

}

function recoverOrder() {
    var row = $("#tbOrderList").datagrid("getSelections");
    if (row == null || row.length == 0) {
        layer.msg("请选择要编辑的行");
    }
    var ids = "";
    if (row == null || row.length == 0) {
        layer.msg("请选择要编辑的行");
    }
    else {
        for (var i = 0; i < row.length; i++) {
            if (row[i].IsDelete == 1) {
                ids += row[i].Id + ",";
            }
        }
    }
    if (ids.length > 0) {
        var orderId = row[0].Id;
        $.ajax({
            type: "get",
            url: "handler/OrderList.ashx",
            data: { type: 'recover', orderId: ids },
            success: function (data) {
                if (data == "ok") {
                    Order.getOrderList(pageIndex, pageSize);
                }
                else {
                    layer.msg(data);
                }
            }
        });
    }

}

//修改外协
var subjectId = "";
var changeIds = "";
function changeOutsource() {
    $("input[name='rblChangeType']").prop("disabled", false);
    $("input[name='subjectcb']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    changeIds = "";
    var row = $("#tbOrderList").datagrid("getSelections");
    for (var i = 0; i < row.length; i++) {
        changeIds += row[i].Id + ",";
    }

    if (changeIds == "" && subjectId == "") {
        layer.msg("请选择项目或者直接选择要变更的POP");
        return false;
    }
    if (changeIds.length > 0) {
        //$("input[name='rblChangeType']").val("1")
        $("input:radio[name='rblChangeType']").each(function () {
            if ($(this).val() == "2") {
                $(this).attr("checked", "checked");
            }
        })
        $("input[name='rblChangeType']").prop("disabled", true);
    }
    else if (subjectId.length > 0) {
        $("input:radio[name='rblChangeType']").each(function () {
            if ($(this).val() == "1") {
                $(this).attr("checked", "checked");
            }
        })
        $("input[name='rblChangeType']").prop("disabled", true);
    }
    var outsourceLen = $("#seleOutsource option").length;
    if (outsourceLen == 1) {
       
        $.ajax({
            type: "get",
            url: "handler/OrderList.ashx?type=getOutsource",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    if (json.length > 0) {

                        for (var i = 0; i < json.length; i++) {
                            var option = "<option value='" + json[i].Id + "'>" + json[i].CompanyName + "</option>";
                            $("#seleOutsource").append(option);
                        }
                    }
                }
            }
        })
    }
    layer.open({
        type: 1,
        time: 0,
        title: '修改外协',
        skin: 'layui-layer-rim', //加上边框
        area: ['450px', '240px'],
        content: $("#divOutsource"),
        id: 'outsourceLayer',
        btn: ['提 交'],
        btnAlign: 'c',
        yes: function () {
            var newOutsourceId = $("#seleOutsource").val() || 0;
            var changeType = $("input[name$='rblChangeType']:checked").val() || 0;
            if (newOutsourceId == 0) {
                layer.msg("请选择新外协");
                return false;
            }
            if (changeType == 0) {
                layer.msg("请选择更新类型");
                return false;
            }
            var index2 = layer.confirm('确定修改外协吗？', {
                btn: ['确定', '取消'] //按钮
            }, function () {
                layer.close(index2);
                Loadingindex = layer.load(1, { shade: [0.5, '#fff'] }); //0代表加载的风格，支持0-2
                setTimeout(function () {
                    Order.updateOutsource();
                }, 1000);
            }, function () {
                //$("#divOutsource").hide();
                layer.close(index2);
            });
        },
        cancel: function () {
            $("#divOutsource").hide();
            layer.closeAll();
        }

    });

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
    var ReceivePrice = $.trim($("#txtReceivePrice").val()) || 0;
    var priceQuantity = $.trim($("#txtPriceQuantity").val());
    if (ShopNo == "") {
        layer.msg("请填写店铺编号");
        return false;
    }
    if (orderType > 3) {
        if (PayPrice == "") {
            layer.msg("请填写应付费用");
            return false;
        }
        if (isNaN(PayPrice)) {
            layer.msg("应付费用填写不正确");
            return false;
        }
        if (ReceivePrice == "") {
            layer.msg("请填写应收费用");
            return false;
        }
        if (priceQuantity == "") {
            layer.msg("请填写数量");
            return false;
        }
        if (isNaN(priceQuantity)) {
            layer.msg("数量填写不正确");
            return false;
        }
        if (isNaN(ReceivePrice)) {
            layer.msg("应收费用填写不正确");
            return false;
        }
        Remark = $.trim($("#txtPriceRemark").val());
        Order.model.Quantity = priceQuantity;
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
        Order.model.Quantity = Quantity;
    }
    Order.model.OrderType = orderType;
    Order.model.ShopNo = ShopNo;
    Order.model.Sheet = Sheet;
    Order.model.POSScale = POSScale;
    Order.model.MaterialSupport = MaterialSupport;
    Order.model.MachineFrame = MachineFrame;
    Order.model.PositionDescription = PositionDescription;
    Order.model.Gender = Gender;
    Order.model.GraphicWidth = GraphicWidth;
    Order.model.GraphicLength = GraphicLength;
    Order.model.OrderGraphicMaterial = GraphicMaterial;
    Order.model.ChooseImg = ChooseImg;
    Order.model.Remark = Remark;
    Order.model.PayOrderPrice = PayPrice;
    Order.model.ReceiveOrderPrice = ReceivePrice;
    return true;
}


function changeOutsourceBatch() {
    var outsourceLen = $("#seleOutsourceBatch option").length;
    if (outsourceLen == 1) {

        $.ajax({
            type: "get",
            url: "handler/OrderList.ashx?type=getOutsource",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    if (json.length > 0) {

                        for (var i = 0; i < json.length; i++) {
                            var option = "<option value='" + json[i].Id + "'>" + json[i].CompanyName + "</option>";
                            $("#seleOutsourceBatch").append(option);
                        }
                    }
                }
            }
        })
    }
    layer.open({
        type: 1,
        time: 0,
        title: '批量修改外协',
        skin: 'layui-layer-rim', //加上边框
        area: ['700px', '300px'],
        content: $("#divOutsourceBatch"),
        id: 'outsourceLayer',
        btn: ['提 交'],
        btnAlign: 'c',
        yes: function () {

            var newOutsourceId = $("#seleOutsourceBatch").val();
            if (newOutsourceId == "0") {
                layer.msg("请选择新外协名称");
                return false;
            }
            var val = $("#changeFile").val();
            if (val != "") {
                var extent = val.substring(val.lastIndexOf('.') + 1);
                if (extent != "xls" && extent != "xlsx") {
                    layer.msg("只能导入Excel文件");
                    return false;
                }
            }
            else {
                layer.msg("请选择文件");
                return false;
            }
            var index2 = layer.confirm('确定修改外协吗？', {
                btn: ['确定', '取消'] //按钮
            }, function () {
                layer.close(index2);
                Loadingindex = layer.load(1, { shade: [0.5, '#fff'] }); //0代表加载的风格，支持0-2
                setTimeout(function () {
                    Order.updateOutsourceBatch();
                }, 1000);
                
            }, function () {
                //$("#divOutsource").hide();
                layer.close(index2);
            });
        },
        cancel: function () {
            $("#divOutsourceBatch").hide();
            layer.closeAll();
        }

    });
}





