var pageIndex = 1;
var pageSize = 15;
var currCustomerId = 0;
var currBasicMaterialId = 0;
var currCustomerMaterialId = 0;


$(function () {
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));
    Material.getCustomer();
    go();
    $("#txtSearchQuoteMaterialName").searchbox({
        width: 180,
        searcher: doSearch,
        prompt: '请输入报价材质名称'
    })

    $("#btnAdd").click(function () {
        CleanVal();
        Material.bindCustomer();
        Material.bindBasicCategory();
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
    })

    $("#btnEdit").click(function () {
        CleanVal();
        var row = $("#tbQuoteMaterial").datagrid("getSelected");
        if (row == null) {
            alert("请选择要编辑的行");
        }
        else {

            Material.model.Id = row.Id;
            currCustomerMaterialId = row.CustomerMaterialId;
            currCustomerId = row.CustomerId
            Material.bindCustomer();
            $("#txtQuoteMaterialName").val(row.QuoteMaterialName);
            Material.bindBasicCategory(row.BasicCategoryId);

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

    $("#btnDelete").click(function () {
        var row = $("#tbQuoteMaterial").datagrid("getSelected");
        if (row == null) {
            alert("请选择要编辑的行");
        }
        else {
            Material.deleteItem(row.Id);
        }
    })

    $("#selCategory").on("change", function () {
        Material.bindCustomerMaterial();
    });

    $("#selCustomerMaterial").on("change", function () {
        var unit = $("#selCustomerMaterial option:selected").data("unit") || 0;
        var price = $("#selCustomerMaterial option:selected").data("price") || 0;
        $("#labPrice").text(price);
        $("#labUnit").text(unit);

    })

    $("#btnRefresh").click(function () {
        $("#tbQuoteMaterial").datagrid("reload");
    })

    $("#btnExport").click(function () {
        
        $("#iframe1").attr("src", "ExportQuoteMaterial.aspx?customerId=" + currCustomerId);
    })
})

var Material = {
    model: function () {
        this.Id = 0;
        this.QuoteMaterialName = "";
        this.CustomerId = 0;
        this.BasicCategoryId = 0;
        this.CustomerMaterialId = 0;

    },
    getCustomer: function () {
        $("#tbCustomer").datagrid({
            method: 'get',
            url: 'Handler/OrderMaterialMapping.ashx?type=getCustomer&bind=1',
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
                go();
            },
            onLoadSuccess: function (data) {
                $('#tbCustomer').datagrid("selectRow", 0);
            }
        })
    },
    bindCustomer: function () {
        document.getElementById("selCustomer").length = 1;
        $.ajax({
            type: "get",
            url: "Handler/OrderMaterialMapping.ashx?type=getCustomer",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currCustomerId == json[i].CustomerId)
                            selected = "selected='selected'";
                        var option = "<option value='" + json[i].CustomerId + "' " + selected + ">" + json[i].CustomerName + "</option>";
                        $("#selCustomer").append(option);
                    }
                    $("#selCustomer").attr("disabled", "disabled");
                }
            }


        })
    },
    bindBasicCategory: function (categoryId) {

        document.getElementById("selCategory").length = 1;
        $.ajax({
            type: "get",
            url: "Handler/OrderMaterialMapping.ashx?type=getBasicCategory",
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
    bindCustomerMaterial: function () {
        document.getElementById("selCustomerMaterial").length = 1;
        var customerId1 = $("#selCustomer").val();
        customerId1 = customerId1 == 0 ? currCustomerId : customerId1;
        var categoryId1 = $("#selCategory").val();

        $.ajax({
            type: "get",
            url: "Handler/OrderMaterialMapping.ashx?type=getCustomerMaterial&customerId=" + customerId1 + "&categoryId=" + categoryId1,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var flag = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currCustomerMaterialId == json[i].Id) {
                            selected = "selected='selected'";
                            flag = true;
                        }
                        var option = "<option value='" + json[i].Id + "' data-unit='" + json[i].Unit + "' data-price='" + json[i].Price + "' " + selected + ">" + json[i].CustomerMaterialName + "</option>";
                        $("#selCustomerMaterial").append(option);
                    }
                    if (flag)
                        $("#selCustomerMaterial").change();
                }
            }
        })
    },
    getMaterialList: function (pageIndex, pageSize) {
        var searchQuoteMaterialName = $("#txtSearchQuoteMaterialName").val();
        
        $("#tbQuoteMaterial").datagrid({
            queryParams: { type: "getList", customerId: currCustomerId, currpage: pageIndex, pagesize: pageSize, searchQuoteMaterialName: searchQuoteMaterialName },
            method: 'get',
            url: './Handler/QuoteMaterialList.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },
                        { field: 'CustomerName', title: '客户名称' },
                        
                        { field: 'BasicCategoryName', title: '材质类型' },
                        { field: 'CustomerMaterialName', title: '客户材质名称' },
                        { field: 'QuoteMaterialName', title: '报价材质名称' },
                        { field: 'Price', title: '价格' },
                        { field: 'Unit', title: '单位' }


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
        var p = $("#tbQuoteMaterial").datagrid('getPager');
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
            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"CustomerId":' + this.model.CustomerId + ',"QuoteMaterialName":"' + this.model.QuoteMaterialName + '","BasicCategoryId":' + this.model.BasicCategoryId + ',"CustomerMaterialId":' + this.model.CustomerMaterialId + '}';

            $.ajax({
                type: "get",
                url: "Handler/QuoteMaterialList.ashx",
                data: { type: "edit", jsonstr: urlCodeStr(jsonStr) },
                success: function (data) {
                    if (data == "ok") {
                        $("#editMaterialDiv").dialog('close');
                        $("#tbQuoteMaterial").datagrid("reload");
                        CleanVal();
                    }
                    else if (data == "exist") {
                        alert("该材质名称已经存在");
                    }
                    else {
                        alert("提交失败：" + data);
                    }
                }
            })
        }
    },
    deleteItem: function (id) {
        $.ajax({
            type: "get",
            url: "Handler/QuoteMaterialList.ashx",
            data: { type: "delete", id: id },
            success: function (data) {
                if (data == "ok") {
                    $("#tbQuoteMaterial").datagrid("reload");
                }
                else {
                    alert("删除失败！");
                }
            }
        })
    }
};


function go() {
    var row = $("#tbCustomer").datagrid('getSelected');

    if (row != null) {
        currCustomerId = row.CustomerId || 0;
        $("#materialTitle").panel({
            title: ">>客户名称：" + row.CustomerName
        });
        Material.getMaterialList(pageIndex, pageSize);
    }
}

function doSearch() {
    
    Material.getMaterialList(pageIndex, pageSize);
}

function CheckVal() {
    var customerId = $("#selCustomer").val();
    var quoteMaterialName1 = $.trim($("#txtQuoteMaterialName").val());
    var categoryId = $("#selCategory").val();
    var customerMaterialId = $("#selCustomerMaterial").val();
    if (customerId == "0") {
        alert("请选择客户名称");
        return false;
    }
    if (quoteMaterialName = "") {
        alert("请填写报价材质名称");
        return false;
    }
    if (categoryId == "0") {
        alert("请选择材质类型");
        return false;
    }
    if (customerMaterialId == "0") {
        alert("请选择客户材质");
        return false;
    }

    Material.model.QuoteMaterialName = quoteMaterialName1;
    Material.model.CustomerId = customerId;
    Material.model.BasicCategoryId = categoryId;
    Material.model.CustomerMaterialId = customerMaterialId;

    return true;
}

function CleanVal() {
    $("#editMaterialDiv select").val("0");
    $("#editMaterialDiv span").html("");
    $("#editMaterialDiv input").val("");
    
    currBasicMaterialId = 0;
    currCustomerMaterialId = 0;
   
    Material.model.QuoteMaterialName = "";
    Material.model.CustomerId = 0;
    Material.model.BasicCategoryId = 0;
    Material.model.CustomerMaterialId = 0;


}