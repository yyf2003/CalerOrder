
$(function () {

    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"),null, $("#separator1"));

    MaterialSupport.getBasicList();
    MaterialSupport.getMaterialSupportList();


    $("#btnAdd").click(function () {
        CleanVal();
        MaterialSupport.bindBasicList();
        $("#editDiv").show().dialog({
            modal: true,
            width: 450,
            height: 280,
            iconCls: 'icon-add',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            MaterialSupport.submit();

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

    })
    $("#btnEdit").click(function () {
        CleanVal();
        var rows = $("#tbMaterialSupport").datagrid("getSelections");

        if (rows.length == 0) {
            alert("请选择要编辑的行");
            return false;
        }
        else if (rows.length > 1) {
            alert("只能选择一行");
            return false;
        }
        else {

            var id = rows[0].Id;
            MaterialSupport.model.Id = id;
            $.ajax({
                type: "get",
                url: "./handler/List.ashx?type=getModel&id=" + id,
                cache: false,
                success: function (data) {
                    if (data != "") {
                        var json = eval(data);

                        $("#txtName").val(json[0].NewMaterialSupportName);
                        MaterialSupport.bindBasicList(json[0].BasicMaterialSupportId);

                        $("#editDiv").show().dialog({
                            modal: true,
                            width: 450,
                            height: 280,
                            iconCls: 'icon-add',
                            resizable: false,
                            buttons: [
                                        {
                                            text: '确定',
                                            iconCls: 'icon-ok',
                                            handler: function () {
                                                MaterialSupport.submit();

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
                    }
                }
            })
        }
    })
    $("#btnDelete").click(function () {
        MaterialSupport.deleteItem();
    })
    $("#btnRefresh").click(function () {
        $("#tbMaterialSupport").datagrid("reload");
    })
})
var currBasicId = 0;
var MaterialSupport = {
    model: function () {
        this.Id = 0;
        this.BasicMaterialSupportId = 0;
        this.NewMaterialSupportName = "";
    },
    getBasicList: function () {
        $("#tbBasicList").datagrid({
            method: 'get',
            url: './handler/List.ashx?type=getBasic',
            columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'MaterialSupportName', title: '名称', width: 200 }
                ]],
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onSelect: function (rowIndex, data) {
                go(data.Id, data.MaterialSupportName);
            }
        })
    },
    bindBasicList: function (basicId) {
        document.getElementById("selBasic").length = 1;
        $.ajax({
            type: "get",
            url: "./handler/List.ashx?type=getBasic",
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (basicId && json[i].Id == basicId) {
                            selected = "selected='selected'";
                        }
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].MaterialSupportName + "</option>";
                        $("#selBasic").append(option);
                    }
                }
            }
        })
    },
    getMaterialSupportList: function () {
        $("#tbMaterialSupport").datagrid({
            queryParams: { type: "getList", basicId: currBasicId },
            method: 'get',
            url: './handler/List.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'checked', checkbox: true },
                        { field: 'MaterialSupportName', title: '名称' },
                        { field: 'BasicMaterialSupportName', title: '基础类型名称' }


            ]],
            height: "100%",
            toolbar: "#toolbar",
            singleSelect: false,
            striped: true,
            border: false,
            pagination: false,
            fitColumns: false,
            fit: false,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            //selectOnCheck: false,
            //checkOnSelect: false,
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }



        })
    },
    submit: function () {
        if (CheckVal()) {

            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"BasicMaterialSupportId":' + this.model.BasicMaterialSupportId + ',"NewMaterialSupportName":"' + this.model.NewMaterialSupportName + '"}';

            $.ajax({
                type: "post",
                url: "./handler/List.ashx",
                data: { type: "edit", jsonstr: escape(jsonStr) },
                success: function (data) {
                    if (data == "ok") {
                        $("#editDiv").dialog('close');
                        $("#tbMaterialSupport").datagrid("reload");

                    }
                    else if (data == "exist") {
                        alert("该级别已经存在");
                    }
                    else {
                        alert("提交失败：" + data);
                    }
                }
            })
        }
    },
    deleteItem: function () {
        
        var rows = $("#tbMaterialSupport").datagrid("getSelections");
        var ids = "";
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].Id + ",";
        }
        if (ids.length == 0) {
            alert("请选择要删除的行");
            return false;
        }
        else {
           
            $.ajax({
                type: "get",
                url: "./handler/List.ashx?type=delete&ids=" + ids,
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        $("#editDiv").dialog('close');
                        $("#tbMaterialSupport").datagrid("reload");

                    }
                    else {
                        alert("删除失败！");
                    }
                }
            })
        }
    }
};

function go(Id, Name) {
    currBasicId = Id || 0;

    $("#materialTitle").panel({
        title: ">>类别名称：" + Name
    });
    MaterialSupport.getMaterialSupportList();
}

function CheckVal() {
    var name = $.trim($("#txtName").val());
    var basicId = $("#selBasic").val();
    if (name == "") {
        alert("请填写物料支持级别");
        return false;
    }
    if (basicId == 0) {
        alert("请选择基础类型");
        return false;
    }
    MaterialSupport.model.BasicMaterialSupportId = basicId;
    MaterialSupport.model.NewMaterialSupportName = name;
    return true;
}

function CleanVal() {
    //currBasicId = 0;
    MaterialSupport.model.Id = 0;
    $("#txtName").val("");
}