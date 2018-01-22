﻿

var pageIndex = 1;
var pageSize = 15;

$(function () {


    CheckPrimissionWithMoreBtn(url, null, new Array($("#btnAdd"), $("#btnImport")), new Array($("#btnEdit")), new Array($("#btnDelete")), null, $("#separator1"));


    $("#sheetSearch").combotree({
        data: [{ id: "", text: "POP位置" }, { id: "服装墙", text: "服装墙" }, { id: "陈列桌", text: "陈列桌" }, { id: "鞋墙", text: "鞋墙" }, { id: "SMU", text: "SMU" }, { id: "中岛", text: "中岛" }, { id: "橱窗", text: "橱窗"}],
        panelHeight: 150,
        overflow: 'auto',
        valueField: 'id',
        textField: 'text',
        method: 'get',
        onLoadSuccess: function (node, data) {
            $("#sheetSearch").combotree("tree").tree("expandAll");
        }
    })

    BlackList.getList(pageIndex, pageSize);
    $("#btnAdd").click(function () {
        ClearVal();
        BlackList.getCornerType();
        BlackList.edit("add");
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
            BlackList.model.Id = rows[0].Id;
            $("#tips").hide();
            $("#txtShopNo").val(rows[0].ShopNo).attr("readonly", "readonly");
            $("#ddlSheet").val(rows[0].Sheet);
            $("#ddlGender").val(rows[0].Gender);
            BlackList.getCornerType(rows[0].CornerType);
            BlackList.edit("update");
        }
    })
    $("#btnDelete").click(function () {
        BlackList.deleteList();
    })
    $("#btnRefresh").click(function () {
        $("#tbList").datagrid("reload");
    })
    $("#btnSearch").click(function () {
        BlackList.getList(pageIndex, pageSize);
    })

    $("#cbAll").click(function () {
        var checked = this.checked;
        $("input[name^='cblSheet']").each(function () {
            this.checked = checked;
        })
    })

    $("#btnImport").click(function () {
        $("#hfIsFinishImport").val("");
        //var url = "ImportShops.aspx?customerId=" + customerId;
        var url = "ImportFrameBlackList.aspx";

        layer.open({
            type: 2,

            title: '批量导入',
            skin: 'layui-layer-rim', //加上边框
            area: ['75%', '70%'],
            content: url,
            id: 'popLayer0',
            cancel: function (index) {
                layer.close(index);
                $("#popDetailDiv").hide();
                if ($("#hfIsFinishImport").val() == "1") {
                    $("#hfIsFinishImport").val("");
                    $("#btnRefresh").click();
                }
            }

        });
    })
})

var BlackList = {
    model: function () {
        this.Id = 0;
        this.ShopNo = "";
        this.Sheet = "";
        this.Gender = "";
        this.CornerType = "";
    },
    getCornerType: function (val) {
        document.getElementById("ddlCornerType").length = 1;
        $.ajax({
            type: "get",
            url: "Handler/FrameBlackListManage.ashx?type=getCornerType",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    if (json.length > 0) {
                        for (var i = 0; i < json.length; i++) {
                            var selected = "";
                            if (val != null && json[i].CornerType.toLocaleLowerCase() == val.toLocaleLowerCase()) {
                                selected = "selected='selected'";
                            }
                            var option = "<option value='" + json[i].CornerType + "' " + selected + ">" + json[i].CornerType + "</option>";
                            $("#ddlCornerType").append(option);
                        }
                    }
                }
            }
        })
    },
    getList: function (pageIndex, pageSize) {
        var sheet = $("#sheetSearch").combotree('getValue') || "";
        var shopNo = $.trim($("#shopNoSearch").val());
        var shopName = $.trim($("#shopNameSearch").val());
        $("#tbList").datagrid({
            queryParams: { type: "getList", currpage: pageIndex, pagesize: pageSize, sheet: sheet, shopNo: shopNo, shopName: shopName },
            method: 'get',
            url: 'Handler/FrameBlackListManage.ashx',
            columns: [[
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'rowIndex', title: '序号' },
                        { field: 'checked', checkbox: true },
                        { field: 'ShopNo', title: '店铺编号' },
                        { field: 'ShopName', title: '店铺名称' },
                        { field: 'RegionName', title: '区域' },
                        { field: 'ProvinceName', title: '省份' },
                        { field: 'CityName', title: '城市' },
                        { field: 'CountyName', title: '区/县' },
                        { field: 'Sheet', title: '无器架POP位置' },
                        { field: 'Gender', title: '性别' },
                        { field: 'CornerType', title: '角落类型' }

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
    edit: function (optype) {
        $("#editDiv").show().dialog({
            modal: true,
            width: 520,
            height: 300,
            iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
            title: optype == "add" ? '添加' : '编辑',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            if (CheckVal()) {
                                var jsonStr = '{"Id":' + (BlackList.model.Id || 0) + ',"ShopNo":"' + BlackList.model.ShopNo + '","Sheet":"' + BlackList.model.Sheet + '","Gender":"' + BlackList.model.Gender + '","CornerType":"' + BlackList.model.CornerType + '"}';
                                alert(jsonStr);
                                $.ajax({
                                    type: "get",
                                    url: "Handler/FrameBlackListManage.ashx",
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
                url: "Handler/FrameBlackListManage.ashx",
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

};

function CheckVal() {

    var shopNo = $.trim($("#txtShopNo").val());
    var sheet = $("#ddlSheet").val();
    var gender = $("#ddlGender").val();
//    $("input[name^='cblSheet']:checked").each(function () {
//        sheet += $(this).val()+",";
//    })
    if (shopNo == "") {
        alert("请填写店铺编号");
        return false;
    }
    if (sheet == "") {
        alert("请选择POP位置");
        return false;
    }

    BlackList.model.ShopNo = shopNo; 
    BlackList.model.Sheet = sheet;
    BlackList.model.Gender = gender;
    BlackList.model.CornerType = $("#ddlCornerType").val();
    return true;
}

function ClearVal() {
    BlackList.model.Id = 0;
    BlackList.model.ShopNo = "";
    BlackList.model.Sheet = "";
    BlackList.model.Gender = "";
    BlackList.model.CornerType = "";
    $("#tips").show();
    $("#txtShopNo").val("").attr("readonly", false);
    $("#ddlSheet").val("");
    //$("#ddlGender").val("")
    $("#ddlCornerType").val("");
}