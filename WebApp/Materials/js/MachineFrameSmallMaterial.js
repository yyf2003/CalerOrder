
var currCustomerId = 0;
var currMachineFrameName = 0;
$(function () {

    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), $("#btnRecover"), $("#separator1"));

    Material.getCustomer();
    Material.getMaterialList();

    $("#btnAdd").click(function () {
        CleanVal();
        Material.bindCustomer();
        Material.bindSmallMaterial();
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
        var row = $("#tbMaterial").datagrid("getSelected");
        if (row == null) {
            alert("请选择要编辑的行");
        }
        else {
            Material.model.MaterialId = row.Id;
            Material.bindCustomer();
            $("#selSheet").val(row.Sheet).change();
            currMachineFrameName = row.MachineFrame;
            Material.bindSmallMaterial(row.SmallMaterialId);
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
    })

    $("#selSheet").change(function () {
        Material.bindMachineFrame();
    })


    $("#btnRefresh").click(function () {
        $("#tbMaterial").datagrid("reload");
    })

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
})


var pageIndex = 1;
var pageSize = 15;
var Material = {
    model: function () {
        this.MaterialId = 0;
        this.CustomerId = 0;
        this.Sheet = "";
        this.MachineFrame = "";
        this.SmallMaterialId = 0;

    },
    getCustomer: function () {
        $("#tbCustomer").datagrid({
            method: 'get',
            url: './Handler/CustomerMaterialList1.ashx?type=getCustomer',
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
            url: "./Handler/CustomerMaterialList1.ashx?type=getCustomer",
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
                }
            }


        })
    },
    bindMachineFrame: function () {
        document.getElementById("selMachineFrame").length = 1;
        var sheet = $("#selSheet").val();
        $.ajax({
            type: "get",
            url: "./Handler/MachineFrameSmallMaterial.ashx",
            data: { type: "getMachineFrame", customerId: currCustomerId, sheet: sheet },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        if (currMachineFrameName == json[i].FrameName)
                            checked = "selected='selected'";
                        var option = "<option value='" + json[i].FrameName + "' " + checked + ">" + json[i].FrameName + "</option>";
                        $("#selMachineFrame").append(option);
                    }
                }
            }
        })
    },
    bindSmallMaterial: function (SmallMaterialId) {
        document.getElementById("selSmallMaterial").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/MachineFrameSmallMaterial.ashx",
            data: { type: "getSmallMaterial" },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        if (SmallMaterialId == json[i].Id)
                            checked = "selected='selected'";
                        var option = "<option value='" + json[i].Id + "' " + checked + ">" + json[i].SmallMaterialName + "</option>";
                        $("#selSmallMaterial").append(option);
                    }
                }
            }
        })
    },
    getMaterialList: function (pageIndex, pageSize) {
        $("#tbMaterial").datagrid({
            queryParams: { type: "getList", customerId: currCustomerId, currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: './Handler/MachineFrameSmallMaterial.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },
                        { field: 'CustomerName', title: '客户名称' },
                        { field: 'Sheet', title: '位置' },
                        { field: 'MachineFrame', title: '器架名称' },
                        { field: 'SmallMaterialName', title: '辅料' }

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
            var jsonStr = '{"Id":' + (this.model.MaterialId || 0) + ',"CustomerId":' + this.model.CustomerId + ',"Sheet":"' + this.model.Sheet + '","MachineFrame":"' + this.model.MachineFrame + '","SmallMaterialId":' + this.model.SmallMaterialId + '}';

            $.ajax({
                type: "get",
                url: "./Handler/MachineFrameSmallMaterial.ashx",
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
            url: "./Handler/MachineFrameSmallMaterial.ashx",
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

function CleanVal() {
    Material.model.MaterialId = 0;
    //currBasicMaterialId = 0;
    $("#selCustomer").val("0");
    //$("#txtCustomerMaterialName").val("");
    $("#selSheet").val("0");
    $("#selMachineFrame").val("0");
    $("#selSmallMaterial").val("0");

}

function CheckVal() {
    var customerId = $("#selCustomer").val();
    var sheet = $("#selSheet").val();
    var machineFrame = $("#selMachineFrame").val();
    var smallMaterialId = $("#selSmallMaterial").val();

    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    if (machineFrame == "0") {
        alert("请选择器架名称");
        return false;
    }
    if (smallMaterialId == "0") {
        alert("请选择辅料");
        return false;
    }
   
    Material.model.CustomerId = customerId;
    Material.model.Sheet = sheet;
    Material.model.MachineFrame = machineFrame;
    Material.model.SmallMaterialId = smallMaterialId;

    return true;
}