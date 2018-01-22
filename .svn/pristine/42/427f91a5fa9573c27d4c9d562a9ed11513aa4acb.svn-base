
var currId = 0;
//var editSheet = "";
var selectSheet = "";
var isEdit = 0;
$(function () {
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));
    frame.getSheetList();
    go();

    $("#btnAdd").click(function () {
        CleanVal();
        //selectSheet = currBindSheet;
        frame.bindSheet();
        $("#frameMsg").show();
        $("#editDiv").show().dialog({
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
                            frame.submit();

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
        })
    });

    $("#btnEdit").click(function () {

        CleanVal();
        var row = $("#tbMachineFrame").datagrid("getSelections");
        if (row.length == 0) {
            alert("请选择要编辑的行");
        }
        else if (row.length > 1) {
            alert("只能选择一行");
        }
        else {
            isEdit = 1;
            frame.model.Id = row[0].Id;
            //editSheet = row[0].Sheet;
            frame.bindSheet();
            $("#txtMachineFrameName").val(row[0].MachineFrame);
            $("#txtCornerType").val(row[0].CornerType);
            $("#frameMsg").hide();
            $("#editDiv").show().dialog({
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
                            frame.submit();

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
    })

    $("#btnDelete").click(function () {
        var row = $("#tbMachineFrame").datagrid("getSelections");
        if (row.length == 0) {
            alert("请选择要删除的行");
        }
        else {
            var ids = "";
            for (var i in row) {
                ids += row[i].Id + ",";
            }
            $.ajax({
                type: "get",
                url: "./Handler/BasicMachineFrameList.ashx",
                data: { type: 'delete', ids: ids, sheet: selectSheet },
                success: function (data) {

                    if (data != "") {
                        var dataArr = data.split('|');
                        if (dataArr[0] == "ok") {
                            if (dataArr[1] == "0")
                                frame.getSheetList();
                            $("#tbMachineFrame").datagrid("reload");
                        }
                        else {
                            alert(data);
                        }
                    }
                }
            })
        }
    })

    $("#btnExport").click(function () {
        
        $("#iframe1").attr("src", "ExportBasicMachineFrame.aspx");
    })
})

var pageIndex = 1;
var pageSize = 15;
var sheetGrid;
var frame = {
    model: function () {
        this.Id = 0;
        this.Sheet = "";
        this.MachineFrame = "";
        this.CornerType = "";
    },
    getSheetList: function () {
        sheetGrid = $("#tbSheet").datagrid({
            method: 'get',
            url: './Handler/BasicMachineFrameList.ashx?type=getSheet',
            columns: [[

                    { field: 'Sheet', title: '器架类型' }
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
                $('#tbSheet').datagrid("selectRow", 0);

            }
        })
    },
    bindSheet: function () {
        document.getElementById("selSheet").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/BasicMachineFrameList.ashx?type=bindSheet",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (selectSheet == json[i].Sheet) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].Sheet + "' " + selected + ">" + json[i].Sheet + "</option>";
                        $("#selSheet").append(option);
                    }

                    if (isSelected && isEdit == 1)
                        $("#selSheet").attr("disabled", "disabled");
                }

            }
        })
    },
    getFrameList: function (pageIndex, pageSize) {
        $("#tbMachineFrame").datagrid({
            queryParams: { type: "getList", currpage: pageIndex, pagesize: pageSize, sheet: selectSheet },
            method: 'get',
            url: './Handler/BasicMachineFrameList.ashx',
            columns: [[
                        { field: '1', title: '', checkbox: true },
                        { field: 'rowIndex', title: '序号' },
                        { field: 'Sheet', title: '器架类型', width: 200 },
                        { field: 'MachineFrame', title: '器架名称', width: 300 },
                        { field: 'CornerType', title: '角落类型', width: 300 }

            ]],
            height: "100%",
            toolbar: "#toolbar",
            pageList: [15, 20],
            striped: true,
            border: false,
            singleSelect: false,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onLoadSuccess: function (data) {

            }
        });
        var p = $("#tbMachineFrame").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                frame.getFrameList(curIndex, curSize);
            }
        });
    },
    submit: function () {
        if (checkVal()) {
            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"Sheet":"' + this.model.Sheet + '","MachineFrame":"' + this.model.MachineFrame + '","CornerType":"' + this.model.CornerType + '"}';
            $.ajax({
                type: "get",
                url: "./Handler/BasicMachineFrameList.ashx",
                data: { type: "edit", jsonstr: urlCodeStr(jsonStr) },
                success: function (data) {
                    var arr = data.split(':');
                    if (arr[0] == "1") {

                        if ((frame.model.Id || 0) == 0) {
                            var sheetRow = $("#tbSheet").datagrid('getRows');
                            var reloadSheet = false;
                            for (var i in sheetRow) {
                                if (sheetRow[i].Sheet == frame.model.Sheet)
                                    reloadSheet = true;
                            }

                            if (!reloadSheet)
                                frame.getSheetList();
                        }
                        $("#tbMachineFrame").datagrid("reload");
                    }
                    else {
                        if (data == "ok") {
                            $("#editDiv").dialog('close');
                            if ((frame.model.Id || 0) == 0) {
                                var sheetRow = $("#tbSheet").datagrid('getRows');
                                var reloadSheet = false;
                                for (var i in sheetRow) {
                                    if (sheetRow[i].Sheet == frame.model.Sheet)
                                        reloadSheet = true;
                                }

                                if (!reloadSheet)
                                    frame.getSheetList();
                            }
                            $("#tbMachineFrame").datagrid("reload");

                        }
                        else if (data == "exist") {
                            alert("该器架名称已经存在");
                        }
                        else {

                            alert(data);
                        }
                    }
                }
            })
        }
    }
}

function go() {
    var row = $("#tbSheet").datagrid('getSelected');
    if (row != null) {
        selectSheet = row.Sheet;
        $("#machineFrameTitle").panel({
            title: ">>器架类型：" + row.Sheet
        });
        frame.getFrameList(pageIndex, pageSize);
    }
}


function checkVal() {
    var sheet = $("#selSheet").val();
    if (sheet == "0") {
        alert("请选择器架类型");
        return false;
    }
    var frameName = $.trim($("#txtMachineFrameName").val());
    if (frameName == "") {
        alert("请填写器架名称");
        return false;
    }
    var cornerType = $.trim($("#txtCornerType").val());
    frame.model.Sheet = sheet;
    frame.model.MachineFrame = frameName;
    frame.model.CornerType = cornerType;
    return true;
}

function CleanVal() {
    frame.model.Id = 0;
    frame.model.Sheet = "";
    frame.model.MachineFrame = "";
    frame.model.CornerType = "";
    $("#selSheet").val("0").attr("disabled",false);
    $("#txtMachineFrameName").val("");
    $("#txtCornerType").val("");
    isEdit = 0;
}