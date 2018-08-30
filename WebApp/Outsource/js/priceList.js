

var pageIndex = 1;
var pageSize = 15;
var currBasicCategoryId = 0;
var currBasicMaterialId = 0;
$(function () {

    Price.bindUnit();
    Price.getCustomer();
    $("#btnAdd").click(function () {
        CleanVal();
        Price.bindBasicCategory();
        $("#editPriceDiv").show().dialog({
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
                            Price.submit();

                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editPriceDiv").dialog('close');
                        }
                    }
                ]
        });
    });

    $("#btnAddBatch").click(function () {
        $("#batchAddContainer").show().dialog({
            modal: true,
            width: 800,
            height: '90%',
            iconCls: 'icon-add',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            Price.submitBatchAdd();

                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#batchAddContainer").dialog('close');
                        }
                    }
                ]
        });
        Price.getBatchPriceList();
    });

    $("#btnEdit").click(function () {
        CleanVal();
        var rows = $("#tbList").datagrid("getSelections");
        if (rows.length == 0) {
            alert("请选择要编辑的行");
            return false;
        }
        else if (rows.length > 1) {
            alert("只能选择一行");
            return false;
        }
        else {
            var row = rows[0];
            currBasicCategoryId = row.BasicCategoryId;
            currBasicMaterialId = row.BasicMaterialId;
            Price.bindBasicCategory();
            Price.model.Id = row.Id;
            $("#txtInstallPrice").val(row.InstallPrice);
            $("#txtInstallAndProducePrice").val(row.InstallAndProductPrice);
            $("#txtSendPrice").val(row.SendPrice);
            $("#editPriceDiv").show().dialog({
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
                            Price.submit();

                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editPriceDiv").dialog('close');
                        }
                    }
                ]
            });
        }
    });

    $("#btnRefresh").click(function () {
        $("#tbList").datagrid("reload");
    });

    $("#btnDelete").click(function () {
        Price.deletePrice();
    });

    $("#selCategory").on("change", function () {
        Price.bindBasicMaterial();

    });

    $("#selBasicMaterial").on("change", function () {
        var unitId = $("#selBasicMaterial option:selected").data("unitid") || 0;
        $("#selUnit").val(unitId);
    });

    $("#seleCustomer").on("change", function () {
        Price.getList();
    });
    $("#selePriceItem").on("change", function () {
        Price.getList();
    });
})


var Price = {
    model: function () {
        this.Id = 0;
        this.OutsourctId = 0;
        this.BasicCategoryId = 0;
        this.BasicMaterialId = 0;
        this.InstallAndProductPrice = 0;
        this.InstallPrice = 0;
        this.SendPrice = 0;
        this.UnitId = 0;
    },
    getCustomer: function () {
        document.getElementById("seleCustomer").length = 0;
        $.ajax({
            type: 'get',
            url: '../../Outsource/MaterialInfo/ListHandler.ashx?type=getCustomer',
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var option = "<option value='" + json[i].CustomerId + "'>" + json[i].CustomerName + "</option>";
                        $("#seleCustomer").append(option);
                    }
                }
            },
            complete: function () {
                Price.getPriceItem();
            }
        })
    },
    getPriceItem: function () {
        document.getElementById("selePriceItem").length = 0;
        var customerId = $("#seleCustomer").val();
        $.ajax({
            type: 'get',
            url: '../../Outsource/MaterialInfo/ListHandler.ashx?type=getPriceItem&customerId=' + customerId,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var option = "<option value='" + json[i].ItemId + "'>" + json[i].ItemName + "</option>";
                        $("#selePriceItem").append(option);
                    }
                }
            },
            complete: function () {
                Price.getList(pageIndex, pageSize);
            }
        })
    },
    getList: function () {
        var customerId = $("#seleCustomer").val();
        var priceItemId = $("#selePriceItem").val();
        $("#tbList").datagrid({
            queryParams: { type: "getList", companyId: companyId, customerId: customerId, priceItemId: priceItemId },
            method: 'get',
            url: 'handler/PriceList.ashx',
            columns: [[

                        { field: 'rowIndex', title: '序号' },
                        { field: 'checked', checkbox: true },
                        { field: 'BasicCategoryName', title: '材质类型', width: 120 },
                        { field: 'BasicMaterialName', title: '材质名称', width: 300 },
                        { field: 'InstallPrice', title: '安装单价', width: 180 },
                        { field: 'InstallAndProductPrice', title: '安装+生产单价', width: 180 },
                        { field: 'SendPrice', title: '发货单价', width: 180 }

            ]],
            height: "85%",
            toolbar: "#toolbar",
            //pageList: [10, 15, 20],
            striped: true,
            border: false,
            singleSelect: false,
            //pagination: true,
            //pageNumber: pageIndex,
            //pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }



        });
        //        var p = $("#tbList").datagrid('getPager');
        //        $(p).pagination({
        //            showRefresh: false,
        //            onSelectPage: function (curIndex, curSize) {
        //                Price.getList(curIndex, curSize);
        //            }
        //        });
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
                        if (currBasicCategoryId == json[i].Id) {
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
                    var isSelected = false;
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currBasicMaterialId == json[i].Id) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].Id + "' data-unitid='" + json[i].UnitId + "' " + selected + ">" + json[i].MaterialName + "</option>";
                        $("#selBasicMaterial").append(option);
                    }
                    if (isSelected) {
                        $("#selBasicMaterial").change();
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
    submit: function () {
        if (CheckVal()) {
            var customerId = $("#seleCustomer").val();
            var priceItemId = $("#selePriceItem").val();
            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"OutsourctId":' + this.model.OutsourctId + ',"CustomerId":' + customerId + ',"PriceItemId":' + priceItemId + ',"InstallPrice":' + this.model.InstallPrice + ',"InstallAndProductPrice":' + this.model.InstallAndProductPrice + ',"SendPrice":' + this.model.SendPrice + ',"BasicMaterialId":' + this.model.BasicMaterialId + ',"BasicCategoryId":' + this.model.BasicCategoryId + ',"UnitId":' + this.model.UnitId + '}';
            
            $.ajax({
                type: "post",
                url: "handler/PriceList.ashx",
                data: { type: "edit", jsonStr: jsonStr },
                success: function (data) {
                    if (data == "ok") {
                        CleanVal();
                        $("#editPriceDiv").dialog('close');
                        $("#tbList").datagrid("reload");

                    }
                    else {
                        alert(data);
                    }
                }
            })
        }
    },
    deletePrice: function () {
        var rows = $("#tbList").datagrid("getSelections");
        if (rows.length == 0) {
            alert("请选择要删除的行");
            return false;
        } else {
            var ids = "";
            for (var i = 0; i < rows.length; i++) {
                ids += rows[i].Id + ",";
            }
            $.ajax({
                type: "post",
                url: "handler/PriceList.ashx",
                data: { type: "delete", ids: ids },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbList").datagrid("reload");
                    }
                    else {
                        alert("删除失败！");
                    }
                }
            })
        }
    },
    getBatchPriceList: function () {
        var customerId = $("#seleCustomer").val();
        var priceItemId = $("#selePriceItem").val();
        $.ajax({
            type: "get",
            url: '../../Outsource/MaterialInfo/ListHandler.ashx',
            data: { type: "getList", customerId: customerId, priceItemId: priceItemId, currpage: 1, pagesize: 1000 },
            success: function (data) {
                if (data != "") {

                    //var json = eval(data);
                    var json = JSON.parse(data);

                    var rows = json.rows;

                    if (json.total > 0) {
                        var tbData = "";
                        for (var i = 0; i < rows.length; i++) {
                            tbData += "<tr class='tr_bai'>";
                            tbData += "<td><span name='spanDelete' style='color:red;cursor:pointer;'>删除</span></td>";
                            tbData += "<td>" + (i + 1) + "</td>";
                            tbData += "<td>" + rows[i].MaterialName + "</td>";
                            tbData += "<td>" + rows[i].Unit + "</td>";
                            tbData += "<td><input name='txtNewInstallPrice' type='text' data-unitid='" + rows[i].UnitId + "' data-basiccategoryid='" + rows[i].BasicCategoryId + "' data-basicmaterialid='" + rows[i].BasicMaterialId + "' value='" + rows[i].InstallPrice + "' style='width:70%;text-align: center;'/></td>";
                            tbData += "<td><input name='txtNewInstallAndProducePrice' type='text' data-unitid='" + rows[i].UnitId + "' data-basiccategoryid='" + rows[i].BasicCategoryId + "' data-basicmaterialid='" + rows[i].BasicMaterialId + "' value='" + rows[i].InstallAndProductPrice + "' style='width:70%;text-align: center;'/></td>";

                            tbData += "<td><input name='txtNewSendPrice' type='text' data-unitid='" + rows[i].UnitId + "' data-basiccategoryid='" + rows[i].BasicCategoryId + "' data-basicmaterialid='" + rows[i].BasicMaterialId + "' value='" + rows[i].SendPrice + "' style='width:70%;text-align: center;'/></td>";

                            tbData += "</tr>";
                        }
                        $("#tbMaterialDetail").html(tbData);
                    }
                }
            }
        });
    },
    submitBatchAdd: function () {
        var customerId = $("#seleCustomer").val();
        var priceItemId = $("#selePriceItem").val();
        if ($("#tbMaterialDetail").html() != "") {
            var trs = $("#tbMaterialDetail tr");
            if (trs.length > 0) {
                var materialDetailJson = "";
                for (var i = 0; i < trs.length; i++) {
                    var tr = $(trs[i]);
                    var installPriceInput = $(tr).find("input[name$='txtNewInstallPrice']");
                    var installAndProducePriceInput = $(tr).find("input[name$='txtNewInstallAndProducePrice']");
                    var sendPriceInput = $(tr).find("input[name$='txtNewSendPrice']");
                    if (installPriceInput && installAndProducePriceInput && sendPriceInput) {
                        var installPrice = $(installPriceInput).val() || 0;
                        var installAndProducePrice = $(installAndProducePriceInput).val() || 0;
                        var sendPrice = $(sendPriceInput).val() || 0;
                        if (isNaN(installPrice)) {
                            installPrice = 0;
                        }
                        if (isNaN(installAndProducePrice)) {
                            installAndProducePrice = 0;
                        }
                        if (isNaN(sendPrice)) {
                            sendPrice = 0;
                        }
                        var unitId = $(installPriceInput).data("unitid") || 0;
                        var basicCategoryId = $(installPriceInput).data("basiccategoryid") || 0;
                        var basicMaterialId = $(installPriceInput).data("basicmaterialid") || 0;
                        materialDetailJson += '{"CustomerId":' + customerId + ',"PriceItemId":' + priceItemId + ',"OutsourctId":' + companyId + ',"UnitId":' + unitId + ',"InstallPrice":' + installPrice + ',"InstallAndProductPrice":' + installAndProducePrice + ',"SendPrice":' + sendPrice + ',"BasicCategoryId":' + basicCategoryId + ',"BasicMaterialId":' + basicMaterialId + '},';

                    }
                }
                if (materialDetailJson.length > 0) {
                    materialDetailJson = materialDetailJson.substring(0, materialDetailJson.length - 1);
                    var jsonStr = "[" + materialDetailJson + "]";
                    $.ajax({
                        type: "post",
                        url: "handler/PriceList.ashx",
                        data: { type: "addBatch", jsonStr: jsonStr },
                        success: function (data) {
                            if (data == "ok") {
                                $("#batchAddContainer").dialog('close');
                                $("#tbList").datagrid("reload");

                            }
                            else {
                                alert(data);
                            }
                        }
                    })
                }
            }
        }
    }
}



function CheckVal() {

    var basicCategoryId = $("#selCategory").val();
    var basicMaterialId = $("#selBasicMaterial").val();
    var installPrice = $.trim($("#txtInstallPrice").val());
    var installAndProducePrice = $.trim($("#txtInstallAndProducePrice").val());
    var sendPrice = $.trim($("#txtSendPrice").val());
    var unitId = $("#selUnit").val()||0;
    if (basicMaterialId == "0") {
        alert("请选择材质名称");
        return false;
    }
    if (installPrice != "" && isNaN(installPrice)) {
        alert("安装单价填写不正确");
        return false;
    }
    if (installAndProducePrice != "" && isNaN(installAndProducePrice)) {
        alert("安装+生产单价填写不正确");
        return false;
    }
    if (sendPrice != "" && isNaN(sendPrice)) {
        alert("发货单价填写不正确");
        return false;
    }

    Price.model.OutsourctId = companyId;
    Price.model.BasicCategoryId = basicCategoryId;
    Price.model.BasicMaterialId = basicMaterialId;
    if (installPrice > 0)
        Price.model.InstallPrice = installPrice;
    if (installAndProducePrice > 0)
        Price.model.InstallAndProductPrice = installAndProducePrice;
    if (sendPrice > 0)
        Price.model.SendPrice = sendPrice;
    Price.model.UnitId = unitId;
    return true;

}

function CleanVal() {
    Price.model.Id = 0;
    Price.model.OutsourctId = 0;
    Price.model.BasicCategoryId = 0;
    Price.model.BasicMaterialId = 0;
    Price.model.InstallPrice = 0;
    Price.model.InstallAndProductPrice = 0;
    Price.model.SendPrice = 0;
    Price.model.UnitId = 0;
    currBasicCategoryId = 0;
    currBasicMaterialId = 0;
    $("#selCategory").val("0");
    $("#selBasicMaterial").val("0");
    $("#txtPrice").val("");
    //document.getElementById("selUnit").length = 1;
}
