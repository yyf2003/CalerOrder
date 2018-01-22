

var currCustomerId = 0;
var currPriceItemId = 0;
var currBasicMaterialId = 0;
$(function () {
    CheckPrimissionWithMoreBtn(url, null, new Array($("#btnAdd"), $("#btnAddType"), $("#btnChangeTypeState")), new Array($("#btnEdit"), $("#btnEditType")), new Array($("#btnDelete")), null, $("#separator1"));


    Material.getCustomer();
    selectCustomer();

    $("#txtSearchMaterialName").searchbox({
        width: 180,
        searcher: doSearch,
        prompt: '请输入材质名称'
    })

    //刷新
    $("#btnRefresh").click(function () {
        $("#tbMaterial").datagrid("reload");
    })
    //添加报价
    $("#btnAdd").click(function () {
        CleanVal();
        Material.bindCustomer();
        Material.bindBasicCategory();
        Material.bindUnit();
        $("#editMaterialDiv").show().dialog({
            modal: true,
            width: 480,
            height: 300,
            iconCls: 'icon-add',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            Material.submit();

                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editMaterialDiv").dialog('close');
                        }
                    }
                ]
        });

    });
    //编辑报价
    $("#btnEdit").click(function () {
        CleanVal();
        var row = $("#tbMaterial").datagrid("getSelected");
        if (row == null) {
            alert("请选择要编辑的行");
        }
        else {

            Material.model.Id = row.Id;
            currBasicMaterialId = row.BasicMaterialId;
            Material.bindCustomer();

            Material.bindBasicCategory(row.BasicCategoryId);
            $("#txtInstallPrice").val(row.InstallPrice);
            $("#txtSendPrice").val(row.SendPrice);
            Material.bindUnit(row.UnitId);
            $("#editMaterialDiv").show().dialog({
                modal: true,
                width: 480,
                height: 300,
                iconCls: 'icon-add',
                resizable: false,
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            Material.submit();

                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editMaterialDiv").dialog('close');
                        }
                    }
                ]
            });
        }

    });
    //删除报价
    $("#btnDelete").click(function () {
        var row = $("#tbMaterial").datagrid("getSelected");
        if (row == null) {
            alert("请选择要编辑的行");
        }
        else {
            if (confirm("确定删除吗？")) {
                Material.deleteMaterial(row.Id);
            }
        }
    })
    //选择材质类型
    $("#selCategory").on("change", function () {
        Material.bindBasicMaterial();

    });
    //选择材质
    $("#selBasicMaterial").on("change", function () {
        var unitId = $("#selBasicMaterial option:selected").data("unitid") || 0;
        $("#selUnit").val(unitId);
    });

    //新增报价类型
    $("#btnAddType").click(function () {
        $("#txtPriceItemName").val("");
        $("#txtPriceItemBeginDate").val("");
        $("#tbMaterialDetail").html("");
        Material.bindCustomer();
        //Material.bindPriceItem();
        $("#editPriceTypeDiv").show().dialog({
            modal: true,
            width: 700,
            height: 500,
            iconCls: 'icon-add',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            Material.submitNewPriceItem();
                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editPriceTypeDiv").dialog('close');

                        }
                    }
                ]
        });
    });

})

var pageIndex = 1;
var pageSize = 15;
var Material = {
    model: function () {
        this.Id = 0;
        this.CustomerId = 0;
        this.PriceItemId = 0;
        this.BasicCategoryId = 0;
        this.BasicMaterialId = 0;
        this.InstallPrice = 0;
        this.SendPrice = 0;
        this.UnitId = 0;

    },
    getCustomer: function () {
        $("#tbCustomer").datagrid({
            method: 'get',
            url: 'ListHandler.ashx?type=getCustomer',
            columns: [[
                    { field: 'CustomerId', hidden: true },
                    { field: 'CustomerName', title: '客户名称' }
                ]],
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onSelect: function (rowIndex, data) {
                selectCustomer();
            },
            onLoadSuccess: function (data) {
                $('#tbCustomer').datagrid("selectRow", 0);

            }
        })
    },
    getPriceItem: function () {

        $("#tbPriceType").datagrid({
            method: 'get',
            url: 'ListHandler.ashx?type=getPriceItem&customerId=' + currCustomerId,
            columns: [[
                    { field: 'ItemId', hidden: true },
                    { field: 'BeginDate', hidden: true },

                    { field: 'ItemName', title: '类型名称' },
                    { field: 'IsDelete', title: '状态', formatter: function (value, row) {
                        if (value == 1)
                            return "<span>禁用</span>";
                        else
                            return "<span>启用</span>"; ;
                    }
                    }

                ]],
            toolbar: "#toolbar1",
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onSelect: function (rowIndex, data) {
                SelectPriceItem();
            },
            onLoadSuccess: function (data) {

                $('#tbPriceType').datagrid("selectRow", 0);
                if (data.total == 0) {
                    currPriceItemId == 0;
                    SelectPriceItem();
                }
            }
        })
    },
    bindCustomer: function () {
        document.getElementById("selCustomer").length = 1;
        document.getElementById("selOtherCustomer").length = 1;
        $.ajax({
            type: "get",
            url: "ListHandler.ashx?type=getCustomer",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currCustomerId == json[i].CustomerId) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].CustomerId + "' " + selected + ">" + json[i].CustomerName + "</option>";
                        $("#selCustomer").append(option);
                        $("#selOtherCustomer").append(option);
                    }
                    if (isSelected)
                        Material.bindPriceItem();
                }
            }


        })
    },
    bindBasicCategory: function (categoryId) {
        document.getElementById("selCategory").length = 1;
        $.ajax({
            type: "get",
            url: "/Materials/Handler/CustomerMaterialList1.ashx?type=getBasicCategory",
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var flag = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (categoryId == json[i].Id) {
                            selected = "selected='selected'";
                            flag = true;
                        }
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].CategoryName + "</option>";
                        $("#selCategory").append(option);
                    }
                    if (flag)
                        $("#selCategory").change();
                }
            }
        })
    },
    bindBasicMaterial: function () {
        var categoryId = $("#selCategory").val();
        document.getElementById("selBasicMaterial").length = 1;
        $.ajax({
            type: "get",
            url: "/Materials/Handler/CustomerMaterialList1.ashx?type=getBasicMaterial&categoryId=" + categoryId,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currBasicMaterialId == json[i].Id)
                            selected = "selected='selected'";
                        var option = "<option value='" + json[i].Id + "' data-unitid='" + json[i].UnitId + "' " + selected + ">" + json[i].MaterialName + "</option>";
                        $("#selBasicMaterial").append(option);
                    }
                }
            }
        })
    },
    bindUnit: function (unitId) {
        document.getElementById("selUnit").length = 1
        $.ajax({
            type: "get",
            url: "/Materials/Handler/CustomerMaterialList1.ashx?type=getUnit",
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (unitId == json[i].Id)
                            selected = "selected='selected'";
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].UnitName + "</option>";
                        $("#selUnit").append(option);
                    }
                }
            }
        })
    },
    getMaterialList: function (pageIndex, pageSize) {
        var materialName = $("#txtSearchMaterialName").val();
        $("#tbMaterial").datagrid({
            queryParams: { type: "getList", customerId: currCustomerId, priceItemId: currPriceItemId, currpage: pageIndex, pagesize: pageSize, materialName: materialName },
            method: 'get',
            url: 'ListHandler.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },

                        { field: 'CustomerName', title: '客户名称' },
                        { field: 'MaterialName', title: '客户材料名称' },
                        { field: 'InstallPrice', title: '安装单价' },
                        { field: 'SendPrice', title: '发货单价' },
                        { field: 'Unit', title: '单位' },
                        { field: 'State', title: '状态', formatter: function (value, row) {
                            if (value == "已删除")
                                return "<span style='color:Red;'>" + value + "</span>";
                            else
                                return value;
                        }
                        }

            ]],
            height: "100%",
            toolbar: "#toolbar",
            pageList: [15, 20],
            striped: true,
            border: false,
            singleSelect: true,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onLoadSuccess: function (data) {

            }



        });
        var p = $("#tbMaterial").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                //GetSearchData();
                Material.getMaterialList(curIndex, curSize);
            }
        });
    },
    submit: function () {

        if (CheckVal()) {
            //var jsonStr = '{"Id":' + (this.model.MaterialId || 0) + ',"CustomerId":' + this.model.CustomerId + ',"MaterialName":"' + this.model.MaterialName + '","UnitId":' + this.model.UnitId + ',"Price":' + this.model.Price + ',"BasicMaterialId":' + this.model.BasicMaterialId + ',"BasicCategoryId":' + this.model.BasicCategoryId + '}';
            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"CustomerId":' + this.model.CustomerId + ',"UnitId":' + this.model.UnitId + ',"InstallPrice":' + this.model.InstallPrice + ',"SendPrice":' + this.model.SendPrice + ',"BasicMaterialId":' + this.model.BasicMaterialId + ',"BasicCategoryId":' + this.model.BasicCategoryId + ',"PriceItemId":' + currPriceItemId + '}';

            $.ajax({
                type: "get",
                url: "ListHandler.ashx",
                data: { type: "edit", jsonstr: urlCodeStr(jsonStr) },
                success: function (data) {

                    if (data == "ok") {
                        CleanVal();
                        $("#editMaterialDiv").dialog('close');
                        $("#tbMaterial").datagrid("reload");

                    }
                    else {
                        alert("提交失败：" + data);
                    }
                }
            })
        }
    },
    deleteMaterial: function (Id) {
        $.ajax({
            type: "get",
            url: "ListHandler.ashx",
            data: { type: "delete", Id: Id },
            success: function (data) {
                if (data == "ok") {
                    $("#tbMaterial").datagrid("reload");
                }
                else {
                    alert("删除失败！");
                }
            }
        })
    },
    bindPriceItem: function () {
        document.getElementById("selOldPriceItem").length = 1;
        var customerId1 = $("#selOtherCustomer").val();
        $.ajax({
            type: "get",
            url: 'ListHandler.ashx?type=getPriceItem&customerId=' + customerId1,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    if (json.length > 0) {

                        for (var i = 0; i < json.length; i++) {
                            var option = "<option value='" + json[i].ItemId + "'>" + json[i].ItemName + "</option>";
                            $("#selOldPriceItem").append(option);
                        }
                    }
                }
            }
        });
    },
    submitNewPriceItem: function () {
        var itemName = $.trim($("#txtPriceItemName").val());

        var beginDate = $.trim($("#txtPriceItemBeginDate").val());
        if (itemName == "") {
            layer.msg("请填写类型名称");
            return false;
        }
        if (beginDate == "") {
            layer.msg("请选择开始时间");
            return false;
        }
        var jsonStr = '{"CustomerId":"' + currCustomerId + '","ItemName":"' + itemName + '","BeginDate":"' + beginDate + '"';
        var materialDetailJson = "";
        if ($("#tbMaterialDetail").html() != "") {
            var trs = $("#tbMaterialDetail tr");
            if (trs.length > 0) {
                for (var i = 0; i < trs.length; i++) {
                    var tr = $(trs[i]);
                    var priceNumInput = $(tr).find("input[name$='txtNewMaterialPrice']");
                    if (priceNumInput) {
                        var priceNum = $(priceNumInput).val() || 0;
                        var unitId = $(priceNumInput).data("unitid") || 0;
                        var basicCategoryId = $(priceNumInput).data("basiccategoryid") || 0;
                        var basicMaterialId = $(priceNumInput).data("basicmaterialid") || 0;
                        materialDetailJson += '{"CustomerId":' + currCustomerId + ',"UnitId":' + unitId + ',"Price":' + priceNum + ',"BasicCategoryId":' + basicCategoryId + ',"BasicMaterialId":' + basicMaterialId + '},';

                    }
                }
            }
        }
        if (materialDetailJson.length > 0) {
            materialDetailJson = materialDetailJson.substring(0, materialDetailJson.length - 1);

        }
        jsonStr += ',"Materials":[' + materialDetailJson + ']}';

        $.ajax({
            type: "post",
            url: 'ListHandler?type=addPriceItem',
            data: { jsonStr: escape(jsonStr) },
            success: function (data) {
                if (data == "ok") {
                    $("#editPriceTypeDiv").dialog('close');
                    Material.getPriceItem();
                }
                else {
                    alert("提交失败：" + data);
                }
            }
        })
    }
};

function SelectPriceItem() {

    var row = $("#tbPriceType").datagrid('getSelected');
    if (row != null) {
        currPriceItemId = row.ItemId || 0;
        var state = row.IsDelete || 0;
        if (state == 0) {
            $("#spanChangeState").html("禁用");
        }
        else
            $("#spanChangeState").html("启用");
    }
    Material.getMaterialList(pageIndex, pageSize);
}

function selectCustomer() {
    var row = $("#tbCustomer").datagrid('getSelected');
    if (row != null) {

        currCustomerId = row.CustomerId || 0;
        $("#materialTitle").panel({
            title: ">>客户名称：" + row.CustomerName
        });
        Material.getPriceItem();
    }
}

function doSearch() {
    Material.getMaterialList(pageIndex, pageSize);
}

function CleanVal() {
    Material.model.Id = 0;
    currBasicMaterialId = 0;
    $("#selCustomer").val("0");
    //$("#txtCustomerMaterialName").val("");
    $("#selCategory").val("0");
    $("#selBasicMaterial").val("0");
    $("#txtInstallPrice").val("");
    $("#txtSendPrice").val("");
    $("#selUnit").val("0");
}

function CheckVal() {
    var customerId = $("#selCustomer").val();

    var basicCategoryId = $("#selCategory").val();
    var basicMaterialId = $("#selBasicMaterial").val();
    var installPrice = $.trim($("#txtInstallPrice").val());
    var sendPrice = $.trim($("#txtSendPrice").val());
    var unitId = $("#selUnit").val();
    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    if (basicMaterialId == "0") {
        alert("请选择材质名称");
        return false;
    }
    if (installPrice == "") {
        alert("请填写安装单价");
        return false;
    }
    else if (isNaN(installPrice)) {
        alert("安装单价必须为数字");
        return false;
    }
    if (sendPrice == "") {
        alert("请填写发货单价");
        return false;
    }
    else if (isNaN(sendPrice)) {
        alert("发货单价必须为数字");
        return false;
    }
    Material.model.CustomerId = customerId;
    Material.model.BasicCategoryId = basicCategoryId;
    Material.model.BasicMaterialId = basicMaterialId;
    Material.model.InstallPrice = installPrice;
    Material.model.SendPrice = sendPrice;
    Material.model.UnitId = unitId;

    return true;

}