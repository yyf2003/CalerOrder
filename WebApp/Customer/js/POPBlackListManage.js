﻿
var pageIndex = 1;
var pageSize = 15;

$(function () {

    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));


    BlackList.getList(pageIndex, pageSize);
    $("#btnAdd").click(function () {
        ClearVal();
        $("#txtShopNo").val("").attr("readonly", false);
        $("#seleSheet").val("").attr("disabled", false);
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
            BlackList.model.ShopId = rows[0].ShopId;
            $("#txtShopNo").val(rows[0].ShopNo).attr("readonly", "readonly");
            $("#seleSheet").val(rows[0].Sheet).attr("disabled", "disabled");
            $("#txtGraphicNo").val(rows[0].GraphicNo);
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

    
})



var BlackList = {
    model: function () {
        this.Id = 0;
        this.ShopId = 0;
        this.ShopNo = "";
        this.Sheet = "";
        this.GraphicNo = "";
    },
    getList: function (pageIndex, pageSize) {
        var shopNo = $.trim($("#shopNoSearch").val());
        var shopName = $.trim($("#shopNameSearch").val());
        $("#tbList").datagrid({
            queryParams: { type: "getList", currpage: pageIndex, pagesize: pageSize, shopNo: shopNo, shopName: shopName },
            method: 'get',
            url: 'Handler/POPBlackListManage.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },
                        { field: 'checked', checkbox: true },
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'ShopId', title: 'shopId', hidden: true },
                        { field: 'ShopNo', title: '店铺编号' },
                        { field: 'ShopName', title: '店铺名称' },
                        { field: 'RegionName', title: '区域' },
                        { field: 'ProvinceName', title: '省份' },
                        { field: 'CityName', title: '城市' },
                        { field: 'Sheet', title: 'POP位置' },
                        { field: 'GraphicNo', title: 'POP编号' }


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
                                var jsonStr = '{"Id":' + (BlackList.model.Id || 0) + ',"ShopId":' + (BlackList.model.ShopId || 0) + ',"ShopNo":"' + BlackList.model.ShopNo + '","Sheet":"' + BlackList.model.Sheet + '","GraphicNo":"' + BlackList.model.GraphicNo + '"}';
                                
                                $.ajax({
                                    type: "get",
                                    url: "Handler/POPBlackListManage.ashx",
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
                url: "Handler/POPBlackListManage.ashx",
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
    var shopNo=$.trim($("#txtShopNo").val());
    var sheet=$("#seleSheet").val();
    var popNo=$.trim($("#txtGraphicNo").val());
    if (shopNo == "") {
        alert("请填写店铺编号");
        return false;
    }
    if (sheet== "") {
        alert("请选择POP位置");
        return false;
    }
    if (popNo == "") {
        alert("请填写POP编号");
        return false;
    }
    BlackList.model.ShopNo = shopNo;
    BlackList.model.Sheet = sheet;
    BlackList.model.GraphicNo = popNo;
    return true;
}

function ClearVal() {
    BlackList.model.Id = 0;
    BlackList.model.ShopId = 0;
    BlackList.model.ShopNo = "";
    BlackList.model.Sheet = "";
    BlackList.model.GraphicNo = "";
    $("#txtShopNo").val("");
    $("#seleSheet").val("");
    $("#txtGraphicNo").val("");
}