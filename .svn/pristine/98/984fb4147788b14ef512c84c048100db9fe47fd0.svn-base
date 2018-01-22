var json;
var parentModuleJson;
var gridId;
var moduleGrid = {
    databind: function (winsize) {

        gridId = $("#datagrid").treegrid({

            title: '基础数据信息',
            rownumbers: true,
            width: '100%',
            height: 'auto',
            nowrap: false,
            resizable: true,
            collapsible: false,
            data: json,
            idField: 'id',
            treeField: 'text',
            animate: true,
            toolbar: '#toolbar',
            frozenColumns: [[
                    { title: '类型名称', field: 'text', width: 300 }

                ]],
            columns: [[
                    { title: 'Id', field: 'id', hidden: true },
                    { title: 'PId', field: 'ParentId', hidden: true },
                    { title: 'BaseCode', field: 'BaseCode', width: 300 }

                ]]
        });
    },
    refresh: function () {

        GetDataJson();
    },
    selected: function () {
        return gridId.treegrid('getSelected');
    },
    bindParentModule: function () {

        //        $("#parentModuleInput").combotree({
        //            url: './Handler/Handler1.ashx?type=getList',
        //            panelHeight: 150,
        //            overflow: 'auto',
        //            valueField: 'id',
        //            textField: 'text',
        //            method: 'get'
        //        })

    },
    addModule: function (optype) {
        var selectedNode = moduleGrid.selected();
        var id = 0;
        if (selectedNode)
            id = selectedNode.id;
        var parentId = $("#parentModuleInput").combotree('getValue');
        var moduleName = $("#txtName").val();
        var url = $("#txtUrl").val();
        var orderNum = $("#txtOrderNum").val();
        var json = '{"Id":"' + id + '","ModuleName":"' + moduleName + '","ParentId":"' + parentId + '","Url":"' + url + '","OrderNum":"' + orderNum + '"}';
        $.ajax({
            type: "get",
            url: "./Handler/Handler1.ashx?type=edit&jsonString=" + escape(json) + "&optype=" + optype,
            success: function (data) {
                var msg = optype == "add" ? "添加" : "更新";
                if (data == "ok") {
                    //alert(msg + "成功！");

                    $("#editModuleDiv").dialog('close');
                    moduleGrid.refresh();
                }
                else {
                    alert(msg + "失败！");
                }
            }
        })
    },
    editModule: function () {
        var selectedNode = moduleGrid.selected();
        if (selectedNode) {
            moduleGrid.bindParentModule();

            $("#parentModuleInput").combotree('setValue', selectedNode.ParentId);
            $("#txtName").val(selectedNode.text);
            $("#txtUrl").val(selectedNode.Url);
            $("#txtOrderNum").val(selectedNode.OrderNum);
            $("#editModuleDiv").show().dialog({
                modal: true,
                width: 400,
                height: 300,
                iconCls: 'icon-add',
                resizable: false,
                buttons: [
                        {
                            text: '确定',
                            iconCls: 'icon-ok',
                            handler: function () {
                                moduleGrid.addModule("edit");

                            }
                        },
                        {
                            text: '取消',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                $("#editModuleDiv").dialog('close');
                            }
                        }
                    ]
            });
        }
        else {
            alert("请先选择要编辑的行");
        }
    },
    deleteModule: function () {
        var selectedNode = moduleGrid.selected();
        if (selectedNode) {
            var moduleId = selectedNode.id;
            $.ajax({
                type: "get",
                url: "/Module/DeleteModule?id=" + moduleId + "&t=" + new Date().getTime(),
                success: function (data) {
                    if (data == "ok") {
                        //alert("删除成功");
                        moduleGrid.refresh();
                    }
                    else {
                        alert("删除失败");
                    }
                }
            })
        }
        else {
            alert("请先选择要删除的行");
        }

    }

};

function GetDataJson() {
  
    $.ajax({
        type: "get",
        url: "./Handler/Handler1.ashx?type=getList",
        cache: false,
        success: function (data) {
            
            if (data != "") {
                json = eval("(" + data + ")");
                moduleGrid.databind($(window));
            }
        }
    })
}

function ClearInput() {
    $("#txtName ,#txtUrl ,#txtOrderNum").val("");
}

$(function () {

    GetDataJson();


    $("#btnRefresh").click(function () {
        ClearInput();
        moduleGrid.refresh();
    })

    $("#btnAdd").click(function () {
        ClearInput();
        moduleGrid.bindParentModule();
        $("#editModuleDiv").show().dialog({
            modal: true,
            width: 400,
            height: 300,
            iconCls: 'icon-add',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            moduleGrid.addModule("add");

                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editModuleDiv").dialog('close');
                        }
                    }
                ]
        });

    })

    $("#btnEdit").click(function () {
        moduleGrid.editModule();
    })

    $("#btnDelete").click(function () {
        moduleGrid.deleteModule();
    })
})