

var pageIndex = 1;
var pageSize = 15;
var currBasicCategoryId = 0;
var currBasicMaterialId = 0;
$(function () {
    Price.getList(pageIndex, pageSize);
    Price.bindUnit();
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
})


var Price = {
    model: function () {
        this.Id = 0;
        this.OutsourceId = 0;
        this.BasicCategoryId = 0;
        this.BasicMaterialId = 0;
        this.InstallPrice = 0;
        this.SendPrice = 0;
    },
    getList: function (pageIndex, pageSize) {
        $("#tbList").datagrid({
            queryParams: { type: "getList", companyId: companyId, currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: 'handler/PriceList.ashx',
            columns: [[

                        { field: 'rowIndex', title: '序号' },
                        { field: 'checked', checkbox: true },

                        { field: 'BasicCategoryName', title: '材质类型', width: 120 },
                        { field: 'BasicMaterialName', title: '材质名称', width: 300 },
                        { field: 'InstallPrice', title: '安装单价', width: 180 },
                        { field: 'SendPrice', title: '发货单价', width: 180 }

            ]],
            height: "99%",
            toolbar: "#toolbar",
            pageList: [10, 15, 20],
            striped: true,
            border: false,
            singleSelect: false,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }



        });
        var p = $("#tbList").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                Price.getList(curIndex, curSize);
            }
        });
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

            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"OutsourceId":' + this.model.OutsourceId + ',"InstallPrice":' + this.model.InstallPrice + ',"SendPrice":' + this.model.SendPrice + ',"BasicMaterialId":' + this.model.BasicMaterialId + ',"BasicCategoryId":' + this.model.BasicCategoryId + '}';

            $.ajax({
                type: "get",
                url: "handler/PriceList.ashx",
                data: { type: "edit", jsonstr: urlCodeStr(jsonStr) },
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
    }
}



function CheckVal() {
    
    var basicCategoryId = $("#selCategory").val();
    var basicMaterialId = $("#selBasicMaterial").val();
    var installPrice = $.trim($("#txtInstallPrice").val());
    var sendPrice = $.trim($("#txtSendPrice").val());
    if (basicMaterialId == "0") {
        alert("请选择材质名称");
        return false;
    }
    if (installPrice != "" && isNaN(installPrice)) {
        alert("安装单价填写不正确");
        return false;
    }
    if (sendPrice != "" && isNaN(sendPrice)) {
        alert("发货单价填写不正确");
        return false;
    }
    Price.model.OutsourceId = companyId;
    Price.model.BasicCategoryId = basicCategoryId;
    Price.model.BasicMaterialId = basicMaterialId;
    if (installPrice>0)
        Price.model.InstallPrice = installPrice;
    if (sendPrice > 0)
        Price.model.SendPrice = sendPrice;
    
    return true;

}

function CleanVal() {
    Price.model.Id = 0;
    Price.model.OutsourceId = 0;
    Price.model.BasicCategoryId = 0;
    Price.model.BasicMaterialId = 0;
    Price.model.InstallPrice = 0;
    Price.model.SendPrice = 0;
    currBasicCategoryId = 0;
    currBasicMaterialId = 0;
    $("#selCategory").val("0");
    $("#selBasicMaterial").val("0");
    $("#txtPrice").val("");
    //document.getElementById("selUnit").length = 1;
}
