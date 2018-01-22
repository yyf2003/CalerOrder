
var currCategoryId = 0;
var currCustomerId = 0;
var currBasicMaterialId = 0;
var currCustomerMaterialId = 0;
var pageIndex = 1;
var pageSize = 15;
$(function () {
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));

    Material.getCategory();
    Material.getMaterialList(pageIndex, pageSize);
    //go();

    $("#txtSearchOrderMaterialName").searchbox({
        width: 180,
        searcher: doSearch,
        prompt: '请输入订单材质名称'
    })

    $("#btnAdd").click(function () {
        CleanVal();
        //Material.bindCustomer();
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

    });

    $("#btnEdit").click(function () {
        CleanVal();
        var row = $("#tbOrderMaterial").datagrid("getSelected");
        if (row == null) {
            alert("请选择要编辑的行");
        }
        else {

            Material.model.Id = row.Id;
            currBasicMaterialId = row.BasicMaterialId;
            Material.bindCustomer(row.CustomerId);
            $("#txtOrderMaterialName").val(row.OrderMaterialName);
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

    $("#btnRefresh").click(function () {
        $("#tbOrderMaterial").datagrid("reload");
    })
    $("#selCustomer").on("change", function () {
        Material.bindBasicCategory();

    });
    $("#selCategory").on("change", function () {
        Material.bindBasicMaterial();

    });

    $("#btnDelete").on("click", function () {
        var row = $("#tbOrderMaterial").datagrid("getSelected");
        if (row == null) {
            alert("请选择要删除的行");
        }
        else
            if (confirm("确定删除吗？")) {
                var id = row.Id;
                $.ajax({
                    type: "get",
                    url: "./Handler/OrderMaterialMapping.ashx?type=delete&id=" + id,
                    success: function (data) {
                        if (data == "ok") {
                            $("#tbOrderMaterial").datagrid("reload");
                        }
                        else {
                            alert("删除失败！");
                        }
                    }

                })
            }
    })

    $("#btnImport").click(function () {
        $("#iframe1").css({ height: 300 + "px" }).attr("src", "ImportCustomerMaterialInfo.aspx");
        $("#importMaterialDiv").show().dialog({
            modal: true,
            width: 700,
            height: 350,
            iconCls: 'icon-add',
            resizable: false,
            onClose: function () {
                if ($("#hfIsFinishImport").val() == "1") {
                    $("#hfIsFinishImport").val("0");
                    $("#tbMaterial").datagrid("reload");

                }
                $("#iframe1").attr("src", "");
            }
        });
    })

    $("#btnExport").click(function () {
        $("#iframe1").attr("src", "ExportOrderMaterial.aspx?customerId=" + currCustomerId);
    })
})



function doSearch() {
    Material.getMaterialList(pageIndex, pageSize);

}
var Material = {
    model: function () {
        this.Id = 0;
        this.OrderMaterialName = "";
        this.CustomerId = 0;
        this.BasicMaterialId = 0;
        this.CustomerMaterialId = 0;
        this.BasicCategoryId = 0;

    },
    getCustomer: function () {
        $("#tbCustomer").datagrid({
            method: 'get',
            url: './Handler/OrderMaterialMapping.ashx?type=getCustomer&bind=1',
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
//        document.getElementById("selCustomer").length = 1;
//        $.ajax({
//            type: "get",
//            url: "./Handler/OrderMaterialMapping.ashx?type=getCustomer",
//            success: function (data) {
//                if (data != "") {
//                    var json = eval(data);
//                    for (var i = 0; i < json.length; i++) {
//                        var selected = "";
//                        if (currCustomerId == json[i].CustomerId)
//                            selected = "selected='selected'";
//                        var option = "<option value='" + json[i].CustomerId + "' " + selected + ">" + json[i].CustomerName + "</option>";
//                        $("#selCustomer").append(option);
//                    }
//                }
//            }


//        })
    },
    bindCustomerMaterial: function () {
        //        document.getElementById("selCustomerMaterial").length = 1;
        //        var customerId = $("#selCustomer").val();
        //        var categoryId = $("#selCategory").val();
        //        $.ajax({
        //            type: "get",
        //            url: "./Handler/OrderMaterialMapping.ashx?type=getCustomerMaterial&customerId=" + customerId + "&categoryId=" + categoryId,
        //            success: function (data) {

        //                if (data != "") {
        //                    var json = eval(data);
        //                    for (var i = 0; i < json.length; i++) {
        //                        var selected = "";
        //                        if (currCustomerMaterialId == json[i].Id)
        //                            selected = "selected='selected'";
        //                        var option = "<option value='" + json[i].Id + "' " + selected + " data-basicmaterialid='" + json[i].BasicMaterialId + "'>" + json[i].CustomerMaterialName + "</option>";
        //                        $("#selCustomerMaterial").append(option);
        //                    }
        //                }
        //            }
        //        })

    },
    bindBasicMaterial: function () {
        document.getElementById("selBasicMaterial").length = 1;
        var customerId = $("#selCustomer").val();
        var categoryId = $("#selCategory").val();
        $.ajax({
            type: "get",
            url: "./Handler/OrderMaterialMapping.ashx?type=getBasicMaterial&categoryId=" + categoryId,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currBasicMaterialId == json[i].Id)
                            selected = "selected='selected'";
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].BasicMaterialName + "</option>";
                        $("#selBasicMaterial").append(option);
                    }
                }
            }
        })
    },
    getCategory: function () {
        $("#tbMaterialType").datagrid({
            method: 'get',
            url: './Handler/OrderMaterialMapping.ashx?type=getBasicCategory',
            columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'CategoryName', title: '类别名称' }
                ]],
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onSelect: function (rowIndex, data) {
                go(data.Id, data.CategoryName);
            }
        })
    },
    bindBasicCategory: function (categoryId) {
        document.getElementById("selCategory").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/OrderMaterialMapping.ashx?type=getBasicCategory",
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
    getMaterialList: function (pageIndex, pageSize) {
        var orderMaterialName = $("#txtSearchOrderMaterialName").val();

        $("#tbOrderMaterial").datagrid({
            //queryParams: { type: "getList", customerId: currCustomerId, currpage: pageIndex, pagesize: pageSize, orderMaterialName: orderMaterialName },
            queryParams: { type: "getList", categoryId: currCategoryId, currpage: pageIndex, pagesize: pageSize, orderMaterialName: orderMaterialName },
            method: 'get',
            url: './Handler/OrderMaterialMapping.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },
            //{ field: 'BasicMaterialId', title: '单价',hidden:true },
            //{ field: 'CustomerName', title: '客户名称' },
                        {field: 'CategoryName', title: '材质类型' },
                        {field: 'OrderMaterialName', title: '订单材质名称' },
                        
                        {field: 'BasicMaterialName', title: '对应基础材质' },
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
        var p = $("#tbOrderMaterial").datagrid('getPager');
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
            //var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"CustomerId":' + this.model.CustomerId + ',"OrderMaterialName":"' + this.model.OrderMaterialName + '","BasicCategoryId":' + this.model.BasicCategoryId + ',"BasicMaterialId":' + this.model.BasicMaterialId + '}';

            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"OrderMaterialName":"' + this.model.OrderMaterialName + '","BasicCategoryId":' + this.model.BasicCategoryId + ',"BasicMaterialId":' + this.model.BasicMaterialId + '}';

            $.ajax({
                type: "get",
                url: "./Handler/OrderMaterialMapping.ashx",
                data: { type: "edit", jsonstr: urlCodeStr(jsonStr) },
                success: function (data) {
                    if (data == "ok") {
                        $("#editMaterialDiv").dialog('close');
                        $("#tbOrderMaterial").datagrid("reload");
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
    }
};


function go(cId, cName) {

    currCategoryId = cId || 0;
   
    $("#materialTitle").panel({
        title: ">>类别名称：" + cName
    });
    Material.getMaterialList(pageIndex, pageSize);
}


function CheckVal() {
    //var customerId = $("#selCustomer").val();
    var materialName = $.trim($("#txtOrderMaterialName").val());
    var basicCategoryId = $("#selCategory").val();
    var basicMaterialId = $("#selBasicMaterial").val();
//    if (customerId == "0") {
//        alert("请选择客户");
//        return false;
//    }
    if (materialName == "") {
        alert("请填写材质名称");
        return false;
    }
    if (basicMaterialId == 0) {
        alert("请选择对应客户材质");
        return false;
    }
   
    //Material.model.CustomerId = customerId;
    Material.model.OrderMaterialName = materialName;
    Material.model.BasicMaterialId = basicMaterialId;
    Material.model.BasicCategoryId = basicCategoryId;
   

    return true;

}

function CleanVal() {
    Material.model.Id =0;
    currCustomerMaterialId = 0;
    currBasicMaterialId = 0;
   
    //$("#selCustomer").val("0");
    $("#txtOrderMaterialName").val("");
    $("#selCategory").val("0");
    $("#selBasicMaterial").val("0");

}



