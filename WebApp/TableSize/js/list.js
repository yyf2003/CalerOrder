

var pageIndex = 1;
var pageSize = 15;
var currFrameName = "";
var currOrderMaterialId = 0;
$(function () {
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, null);
    Table.getList(pageIndex, pageSize);



    $("#btnAdd").click(function () {
        ClearVal();
        Table.getFrameList();
        Table.bindMaterialCategory();
        Table.edit("add");
    })

    $("#btnEdit").click(function () {
        ClearVal();
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
            Table.model.Id = rows[0].Id;
            currFrameName = rows[0].MachineFrame;
            currOrderMaterialId = rows[0].OrderMaterialId;
            Table.getFrameList();
            Table.bindMaterialCategory(rows[0].BasicCategoryId);
            $("#txtRemark").val(rows[0].Remark);
            $("#txtNormalLength").val(rows[0].NormalLength);
            $("#txtNormalWidth").val(rows[0].NormalWidth);
            $("#txtWithEdgeLength").val(rows[0].WithEdgeLength);
            $("#txtWithEdgeWidth").val(rows[0].WithEdgeWidth);
            $("#txtQuantity").val(rows[0].Quantity);
            Table.edit("update");
        }
    })
    $("#btnRefresh").click(function () {
        $("#tbList").datagrid("reload");
    })

    $("#btnDelete").click(function () {
        Table.deleteList();
    })

    $("#selCategory").on("change", function () {
        Table.bindOrderMaterial();
    })

})

var Table = {
    model: function () {
        this.Id = 0;
        this.Sheet = "";
        this.MachineFrame = "";
        this.NormalLength = "";
        this.NormalWidth = "";
        this.WithEdgeLength = "";
        this.WithEdgeWidth = "";
        this.Quantity = "";
        this.Remark = "";
        this.BasicCategoryId = 0;
        this.OrderMaterialId = 0;
    },
    getList: function (pageIndex, pageSize) {
        $("#tbList").datagrid({
            queryParams: { type: "getList", currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: 'handler/List.ashx',
            columns: [[
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'rowIndex', title: '序号' },
                        { field: 'checked', checkbox: true },
                        { field: 'Sheet', title: '位置', width: 100 },
                        { field: 'MachineFrame', title: '器架名称', width:120 },
                        { field: 'Remark', title: '位置描述', width: 150 },
                        { field: 'NormalWidth', title: '正常宽', width: 80 },
                        { field: 'NormalLength', title: '正常高', width: 80 },
                        { field: 'WithEdgeWidth', title: '反包宽', width: 80 },
                        { field: 'WithEdgeLength', title: '反包高', width: 80 },
                        { field: 'Quantity', title: '数量', width: 80 },
                        { field: 'OrderMaterialName', title: '材质名称' }

            ]],
            height: "100%",
            toolbar: "#toolbar",
            pageList: [10, 15, 20],
            striped: true,
            border: false,
            singleSelect: false,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: false,
            fit: false,
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
                //GetSearchData();
                BlackList.getList(curIndex, curSize);
            }
        });
    },
    bindMaterialCategory: function (categoryId) {

        document.getElementById("selCategory").length = 1;
        $.ajax({
            type: "get",
            url: "handler/List.ashx?type=getMaterialCategory",
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
    bindOrderMaterial: function () {
        document.getElementById("selOrderMaterial").length = 1;
        var customerId1 = 1;
        var categoryId1 = $("#selCategory").val();

        $.ajax({
            type: "get",
            url: "handler/List.ashx?type=getOrderMaterial&customerId=" + customerId1 + "&categoryId=" + categoryId1,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var flag = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currOrderMaterialId == json[i].Id) {
                            selected = "selected='selected'";
                            flag = true;
                        }
                        var option = "<option value='" + json[i].Id + "' data-unit='" + json[i].UnitName + "' data-price='" + json[i].Price + "' " + selected + ">" + json[i].OrderMaterialName + "</option>";
                        $("#selOrderMaterial").append(option);
                    }
                    if (flag)
                        $("#selOrderMaterial").change();
                }
            }
        })
    },
    getFrameList: function () {
        var sheet = $("#selSheet").val();
        document.getElementById("selFrameName").length = 1;
        $.ajax({
            type: "get",
            url: "handler/List.ashx?type=getFrameList&sheet=" + sheet,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        if (json[i].FrameName == currFrameName) {
                            checked = "selected='selected'";
                        }
                        var option = "<option value='" + json[i].FrameName + "' " + checked + ">" + json[i].FrameName + "</option>";
                        $("#selFrameName").append(option);
                    }
                }
            }
        })
    },
    edit: function (optype) {
        $("#editDiv").show().dialog({
            modal: true,
            width: 520,
            height: 350,
            iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
            title: optype == "add" ? '添加' : '编辑',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            if (CheckVal()) {
                                var jsonStr = '{"Id":' + (Table.model.Id || 0) + ',"Sheet":"' + Table.model.Sheet + '","MachineFrame":"' + Table.model.MachineFrame + '","NormalLength":"' + Table.model.NormalLength + '","NormalWidth":"' + Table.model.NormalWidth + '","WithEdgeLength":"' + Table.model.WithEdgeLength + '","WithEdgeWidth":"' + Table.model.WithEdgeWidth + '","Remark":"' + Table.model.Remark + '","BasicCategoryId":"' + Table.model.BasicCategoryId + '","OrderMaterialId":"' + Table.model.OrderMaterialId + '","Quantity":"' + Table.model.Quantity + '"}';

                                $.ajax({
                                    type: "get",
                                    url: "handler/List.ashx",
                                    data: { type: "edit", jsonstr: escape(jsonStr) },
                                    success: function (data) {
                                        if (data == "ok") {
                                            $("#editDiv").dialog('close');
                                            $("#tbList").datagrid("reload");
                                        }
                                        else
                                            alert(data);
                                    }
                                })
                            }
                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editDiv").dialog('close');
                        }
                    }
                ]
        });
    },
    deleteList: function () {
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
                type: "get",
                url: "handler/List.ashx",
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


    var sheet = $("#selSheet").val();
    if (sheet == "0") {
        alert("请选择POP位置");
        return false;
    }
    var frameName = $("#selFrameName").val();
    if (frameName == "0") {
        alert("请选择器架名称");
        return false;
    }
    var remark = $.trim($("#txtRemark").val());
    if (remark == "") {
        alert("请填写位置说明");
        return false;
    }
    var normalLength = $.trim($("#txtNormalLength").val());
    if (normalLength == "") {
        alert("请填写正常长度");
        return false;
    }
    else if (isNaN(normalLength)) {
        alert("正常长度必须是数字");
        return false;
    }
    var normalWidth = $.trim($("#txtNormalWidth").val());
    if (normalWidth == "") {
        alert("请填写正常宽度");
        return false;
    }
    else if (isNaN(normalWidth)) {
        alert("正常宽度必须是数字");
        return false;
    }


    var withEdgeLength = $.trim($("#txtWithEdgeLength").val());
    if (withEdgeLength == "") {
        alert("请填写反包长度");
        return false;
    }
    else if (isNaN(withEdgeLength)) {
        alert("反包长度必须是数字");
        return false;
    }

    var withEdgeWidth = $.trim($("#txtWithEdgeWidth").val());
   
    if (withEdgeWidth == "") {
        alert("请填写反包宽度");
        return false;
    }
    else if (isNaN(withEdgeWidth)) {
        alert("反包宽度必须是数字");
        return false;
    }

    var quantity = $.trim($("#txtQuantity").val());
    if (quantity == "") {
        alert("请填写数量");
        return false;
    }
    else if (isNaN(quantity)) {
        alert("数量必须是数字");
        return false;
    }

    var categoryId = $("#selCategory").val();
    var materialId = $("#selOrderMaterial").val();
    if (materialId == "0") {
        alert("请选择材质");
        return false;
    }

    Table.model.Sheet = sheet;
    Table.model.MachineFrame = frameName;
    Table.model.Remark = remark;
    Table.model.NormalLength = normalLength;
    Table.model.NormalWidth = normalWidth;
    Table.model.WithEdgeLength = withEdgeLength;
    Table.model.WithEdgeWidth = withEdgeWidth;
    Table.model.BasicCategoryId = categoryId;
    Table.model.OrderMaterialId = materialId;
    Table.model.Quantity = quantity;
    return true;
}

function ClearVal() {
    currFrameName = "";
    Table.model.Id = 0;
    Table.model.Sheet = "";
    Table.model.MachineFrame = "";
    Table.model.Remark = "";
    Table.model.NormalLength = "";
    Table.model.NormalWidth = "";
    Table.model.WithEdgeLength = "";
    Table.model.WithEdgeWidth = "";
    Table.model.Quantity = "";
    Table.model.BasicCategoryId = 0;
    Table.model.OrderMaterialId = 0;
    $("#selFrameName").val("0");
    $("#txtRemark").val("");
    $("#txtNormalLength").val("");
    $("#txtNormalWidth").val("");
    $("#txtWithEdgeLength").val("");
    $("#txtWithEdgeWidth").val("");
    $("#txtQuantity").val("");
    $("#selCategory").val("0");
    document.getElementById("selOrderMaterial").length = 1;
}