

var pageIndex = 1;
var pageSize = 15;
$(function () {
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), $("#btnRecover"), $("#separator1"));
    Materail.getList(pageIndex, pageSize);
    Materail.getUnitList();
    $("#btnAdd").click(function () {
        ClearVal();
        layer.open({
            type: 1,
            time: 0,
            title: '添加物料',
            skin: 'layui-layer-rim', //加上边框
            area: ['500px', '200px'],
            content: $("#editDiv"),
            id: 'popLayer',
            btn: ['提 交'],
            btnAlign: 'c',
            yes: function () {
                Materail.submit();
            },
            cancel: function (index) {
                layer.closeAll();
                $("#editDiv").hide();

            }

        });
    })

    $("#btnEdit").click(function () {
        ClearVal();
        var rows = $("#tbMaterial").datagrid("getSelections");
        if (rows.length == 0) {
            layer.msg("请选择要编辑的行");
            return false;
        }
        else if (rows.length > 1) {
            layer.msg("只能选择一行");
            return false;
        }
        var row = rows[0];
        Materail.model.Id = row.Id;
        $("#txtMaterialName").val(row.MaterialName);
        $("#seleUnit").val(row.UnitId);
        $("#txtPrice").val(row.UnitPrice);
        layer.open({
            type: 1,
            time: 0,
            title: '添加物料',
            skin: 'layui-layer-rim', //加上边框
            area: ['500px', '200px'],
            content: $("#editDiv"),
            id: 'popLayer',
            btn: ['提 交'],
            btnAlign: 'c',
            yes: function () {
                Materail.submit();
            },
            cancel: function (index) {
                layer.closeAll();
                $("#editDiv").hide();

            }

        });
    })

    $("#btnRefresh").click(function () {
        $("#tbMaterial").datagrid("reload");
    });

    $("#btnDelete").click(function () {
        Materail.deleteItem();
    });

    $("#btnRecover").click(function () {
        Materail.recoverItem();
    });
})



var Materail = {
    model: function () {
        this.Id = 0;
        this.MaterialName = "";
        this.UnitId = 0;
        this.UnitPrice = 0;

    },
    getUnitList: function () {
        $.ajax({
            type: "get",
            url: "Handler1.ashx?type=getUnitList",
            cache: true,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var option = "<option value='" + json[i].Id + "'>" + json[i].UnitName + "</option>";
                        $("#seleUnit").append(option);
                    }
                }
            }
        })
    },
    getList: function (pageIndex, pageSize) {
        $("#tbMaterial").datagrid({
            queryParams: { type: "getList", currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: 'Handler1.ashx',
            columns: [[

                        { field: 'rowIndex', title: '序号' },
                        { field: 'checked', checkbox: true },
                        { field: 'MaterialName', title: '物料名称', width: 200 },
                        { field: 'UnitName', title: '单位', width: 200 },
                        { field: 'UnitPrice', title: '单价', width: 100 },
                        { field: 'AddDate', title: '添加时间', width: 100 },

                        { field: 'State', title: '状态', formatter: function (value, row) {
                            if (value == 0)
                                return "<span style='color:Red;'>已删除</span>";
                            else
                                return "<span>正常</span>";
                        }
                        }

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
        var p = $("#tbMaterial").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                Materail.getList(curIndex, curSize);
            }
        });
    },
    submit: function () {
        if (CheckVal()) {
            var json = '{"Id":' + (Materail.model.Id || 0) + ',"MaterialName":"' + Materail.model.MaterialName + '","UnitId":' + (Materail.model.UnitId || 0) + ',"UnitPrice":' + (Materail.model.UnitPrice || 0) + '}';
            $.ajax({
                type: "get",
                url: "Handler1.ashx",
                data: { type: "edit", jsonStr: urlCodeStr(json) },
                success: function (data) {
                   
                    if (data == "ok") {
                        layer.closeAll();
                        $("#editDiv").hide();
                        $("#tbMaterial").datagrid("reload");

                    }
                    else if (data == "exist") {
                        layer.msg("该物料名称已经存在");
                    }
                    else {
                        layer.msg("提交失败：" + data);
                    }
                }
            })
        }
    },
    deleteItem: function () {
        var rows = $("#tbMaterial").datagrid("getSelections");
        if (rows.length == 0) {
            layer.msg("请选择要删除的行");
            return false;
        } else {
            var ids = "";
            for (var i = 0; i < rows.length; i++) {
                ids += rows[i].Id + ",";
            }
            $.ajax({
                type: "post",
                url: "Handler1.ashx",
                data: { type: "delete", ids: ids },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbMaterial").datagrid("reload");
                    }
                    else {
                        layer.msg("删除失败！");
                    }
                }
            })
        }
    },
    recoverItem: function () {
        var rows = $("#tbMaterial").datagrid("getSelections");
        if (rows.length == 0) {
            layer.msg("请选择要恢复的行");
            return false;
        } else {
            var ids = "";
            for (var i = 0; i < rows.length; i++) {
                ids += rows[i].Id + ",";
            }
            $.ajax({
                type: "post",
                url: "Handler1.ashx",
                data: { type: "recover", ids: ids },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbMaterial").datagrid("reload");
                    }
                    else {
                        layer.msg("恢复失败！");
                    }
                }
            })
        }
    }
}

function CheckVal() {
    var materialName = $.trim($("#txtMaterialName").val());
    var unitId = $("#seleUnit").val();
    var price = $.trim($("#txtPrice").val());
    if (materialName == "") {
        layer.msg('请填写物料名称');
        return false;
    }
    if (unitId == "0") {
        layer.msg('请选择单位');
        return false;
    }
    if (price == "") {
        layer.msg('请填写单价');
        return false;
    }
    else if (isNaN(price)) {
        layer.msg('单价填写不正确');
        return false;
    }
    Materail.model.MaterialName = materialName;
    Materail.model.UnitId = unitId;
    Materail.model.UnitPrice = price;
    return true;
}

function ClearVal() {
    Materail.model.Id = 0;
    $("#txtMaterialName").val("");
    $("#seleUnit").val("0");
    $("#txtPrice").val("");
}