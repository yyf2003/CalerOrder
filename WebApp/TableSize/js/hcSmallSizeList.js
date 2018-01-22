

var pageIndex = 1;
var pageSize = 15;
var sizeId = 0;
var currFormat = "";
var addFormat = "";
var currSheet = "";
var currCornerType = "";
var currFrameName = "";
$(function () {

    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, null);
    Size.getFormat();
    //Size.getList(pageIndex, pageSize);
    Size.getList($(window));
    $("#btnAdd").click(function () {
        ClearVal();
        $("#selFormat").val(currFormat);
        //Size.getSheetList();
        Size.edit("add");
    })

    $("#btnEdit").click(function () {
        ClearVal();
        var canEdit = false;
        var rows = $("#tbList").datagrid("getSelections");
        if (rows.length == 0) {
            alert("请选择要编辑的行");
            return false;
        }
        else if (rows.length > 1) {
            alert("只能选择一行");
            return false;
        }
        if (rows[0].ParentId == 0) {
            canEdit = true;
        }
        //else {
        //            Size.model.Id = rows[0].Id;
        //            currSheet = rows[0].Sheet;
        //            //currCornerType = rows[0].CornerType;
        //            //currFrameName = rows[0].MachineFrameName;
        //            $("#selFormat").val(rows[0].Format);
        //            $("#txtRemark").val(rows[0].Remark);
        //            $("#txtBigWidth").val(rows[0].BigGraphicWidth);
        //            $("#txtBigLength").val(rows[0].BigGraphicLength);
        //            $("input[name='txtWidth']:first").val(rows[0].SmallGraphicWidth);
        //            $("input[name='txtLength']:first").val(rows[0].SmallGraphicLength);
        //            $("div.divAddSize:gt(0)").hide();
        //            $("#divAdd").hide();
        //            Size.getSheetList();

        // }
        if (canEdit) {
            Edit(rows[0]);
        }
    })

    $("#btnRefresh").click(function () {
        Size.getList(pageIndex, pageSize);
    })

    $("#btnDelete").click(function () {
        Size.deleteList();
    })

    $("#selSheet").on("change", function () {
        Size.getCornerType();
    })

    $("#selCornerType").on("change", function () {
        Size.getFrameList();
    })

    $("#spanAdd").click(function () {
        var div = '<div class="divAddSize">';
        div += '宽：<input type="text" name="txtWidth" style=" text-align:center; width:100px;"/>';
        div += '&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
        div += '高：<input type="text" name="txtLength" style=" text-align:center; width:100px;"/>';
        div += '<span name="deleteSmallSize" class="deleteSmall">移除</span></div>';
        $("#sizeContainer").append(div);

    })

    $("#sizeContainer").delegate("span[name='deleteSmallSize']","click",function () {

        $(this).parent().remove();
    })
})

var gridId;
var Size = {
    model: function () {
        this.Id = 0;
        this.ParentId = 0;
        this.Format = "";
        this.Sheet = "";
        this.BigGraphicWidth = "";
        this.BigGraphicLength = "";
        this.SmallGraphicWidth = "";
        this.SmallGraphicLength = "";
        this.Remark = "";

    },
    getFormat: function () {

        $("#tbShopFormat").datagrid({
            method: 'get',
            url: 'handler/HCSmallSizeList.ashx?type=getFormat',
            columns: [[

                    { field: 'Format', title: '店铺类型' }
                ]],
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onSelect: function (rowIndex, data) {
                go(data.Format);
            },
            onLoadSuccess: function () {

                if (addFormat) {
                    var row = null;
                    var rows = $(this).datagrid('getRows');

                    if (rows && rows.length > 0) {
                        for (var i = 0; i < rows.length; i++) {
                            if (rows[i].Format == addFormat) {
                                $(this).datagrid('selectRow', i);
                                addFormat = "";
                                break;
                            }
                        }
                    }

                }
            }
        })
    },
    getList1: function (pageIndex, pageSize) {
        $("#tbList").datagrid({
            queryParams: { type: "getList", format: currFormat, currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: 'handler/HCSmallSizeList.ashx',
            columns: [[
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'rowIndex', title: '序号' },
                        { field: 'checked', checkbox: true },
                        { field: 'Format', title: 'Format', width: 100 },
                        { field: 'Sheet', title: '位置', width: 100 },
                        { field: 'Remark', title: '备注', width: 120 },
                        { field: 'BigGraphicWidth', title: '宽（大）', width: 80 },
                        { field: 'BigGraphicLength', title: '高（大）', width: 80 },
                        { field: 'SmallGraphicWidth', title: '宽（小）', width: 80 },
                        { field: 'SmallGraphicLength', title: '高（小）', width: 80 }


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
                Size.getList(curIndex, curSize);
            }
        });
    },
    getSheetList: function () {
        document.getElementById("selSheet").length = 1;
        $.ajax({
            type: "get",
            url: "/Customer/Handler/MachineFrame.ashx?type=getSheetList",
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].Sheet == currSheet) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].Sheet + "' " + selected + ">" + json[i].Sheet + "</option>";
                        $("#selSheet").append(option);
                    }
                    //if (isSelected) {
                    //Size.getCornerType();
                    //}
                }
            }
        })
    },
    getCornerType: function () {
        var sheet = $("#selSheet").val();

        document.getElementById("selCornerType").length = 1;
        $.ajax({
            type: "get",
            url: "/Customer/Handler/MachineFrame.ashx?type=getCornerType&sheet=" + sheet,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].CornerType == currCornerType) {
                            selected = "selected='selected'";

                        }
                        var option = "<option value='" + json[i].CornerType + "' " + selected + ">" + json[i].CornerType + "</option>";
                        $("#selCornerType").append(option);
                    }

                }
                Size.getFrameList();
            }
        })
    },
    getFrameList: function () {
        var sheet = $("#selSheet").val();
        var cornerType = $("#selCornerType").val();

        document.getElementById("selFrameName").length = 1;
        $.ajax({
            type: "get",
            url: "/Customer/Handler/MachineFrame.ashx?type=getFrameList&sheet=" + sheet + "&cornerType=" + cornerType,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].FrameName == currFrameName) {
                            selected = "selected='selected'";

                        }
                        var option = "<option value='" + json[i].FrameName + "' " + selected + ">" + json[i].FrameName + "</option>";
                        $("#selFrameName").append(option);
                    }

                }
            }
        })
    },
    edit: function (optype) {
        $("#editDiv").show().dialog({
            modal: true,
            width: 600,
            height: 500,
            iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
            title: optype == "add" ? '添加' : '编辑',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            if (CheckVal()) {
                                var jsonStr = '{"Id":' + (Size.model.Id || 0) + ',"Sheet":"' + Size.model.Sheet + '","BigGraphicWidth":"' + Size.model.BigGraphicWidth + '","BigGraphicLength":"' + Size.model.BigGraphicLength + '","GraphicSizes":"' + Size.model.GraphicSizes + '","Remark":"' + Size.model.Remark + '","Format":"' + Size.model.Format + '"}';

                                $.ajax({
                                    type: "get",
                                    url: "handler/HCSmallSizeList.ashx",
                                    data: { type: "edit", jsonstr: escape(jsonStr) },
                                    success: function (data) {
                                        if (data == "ok") {
                                            $("#editDiv").dialog('close');
                                            $("#tbList").treegrid("reload");
                                            if (Size.model.Format != currFormat) {
                                                addFormat = Size.model.Format;
                                                $("#tbShopFormat").datagrid("reload");
                                            }
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
                url: "handler/HCSmallSizeList.ashx",
                data: { type: "delete", ids: ids },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbList").datagrid("reload");
                    }
                    else {
                        alert(data);
                    }
                }
            })
        }
    },
    getList: function (winsize) {

        gridId = $("#tbList").treegrid({
            //title: '模块管理',
            rownumbers: true,
            width: '100%',
            height: 'auto',
            nowrap: false,
            resizable: true,
            collapsible: false,
            //queryParams: { type: "getList", format: currFormat },
            url: 'handler/HCSmallSizeList.ashx?type=getList&format=' + currFormat,
            idField: 'id',
            treeField: 'text',
            animate: true,
            toolbar: '#toolbar',
            cache: false,
            //frozenColumns: [[
            //{ title: 'Format', field: 'Format', width: 200 }

            //]],
            columns: [[
                    { title: 'Id', field: 'id', hidden: true },
                    { title: 'ParentId', field: 'ParentId', hidden: true },
                    { field: 'text', title: 'Format', width: 300 },
                    { field: 'Sheet', title: '位置', width: 100 },
                    { field: 'Remark', title: '备注', width: 120 },
                    { field: 'GraphicWidth', title: '宽', width: 80 },
                    { field: 'GraphicLength', title: '高', width: 80 },

                ]],
            onLoadSuccess: function (row, data) {

            },
            onDblClickRow: function (row) {
                //var id = row.id;
                //moduleGrid.editModule(id);
                if (row.ParentId == 0) {
                    Edit(row);
                }
            }
        });
    }
};

function go(format) {
    
    currFormat = format || "";
    $("#machineFrameTitle").panel({
        title: ">>尺寸信息：" + format
    });
    //Size.getList(pageIndex, pageSize);
    Size.getList($(window));
}

function ClearVal() {
    currSheet = "";
    //currCornerType = "";
    //currFrameName = "";
    sizeId = 0;
    Size.model.Id = 0;
    Size.model.ParentId = 0;
    Size.model.Format = "";
    Size.model.Sheet = "";
    Size.model.Remark = "";
    Size.model.BigGraphicWidth = "";
    Size.model.BigGraphicLength = "";
    Size.model.SmallGraphicWidth = "";
    Size.model.SmallGraphicLength = "";
    $("#selFormat").val("");
    //$("#selSheet").val("");
    $("#txtRemark").val("");
    $("#txtBigWidth").val("");
    $("#txtBigLength").val("");
    $("input[name='txtWidth']").val("");
    $("input[name='txtLength']").val("");
    $("div.divAddSize:gt(0)").show();
    $("#divAdd").show();
}

function CheckVal() {
    var format = $("#selFormat").val();
    var sheet = $("#selSheet").val();
    var remark = $("#txtRemark").val();
    var bigWidth =$.trim($("#txtBigWidth").val());
    var bigLength = $.trim($("#txtBigLength").val());
    if (format == "") {
        alert("请选择Format");
        return false;
    }
    if (sheet == "") {
        alert("请选择POP位置");
        return false;
    }
    if (bigWidth == "") {
        alert("请填写宽（大）");
        return false;
    }
    else if (isNaN(bigWidth)) {
        alert("宽（大）填写不正确");
        return false;
    }
    if (bigLength == "") {
        alert("请填写高（大）");
        return false;
    }
    else if (isNaN(bigLength)) {
        alert("高（大）填写不正确");
        return false;
    }
    var graphicSizes = "";
    var sizeIsOk = true;
    $("div.divAddSize").each(function () {
        var width = $.trim($(this).find("input[name='txtWidth']").val());
        var length = $.trim($(this).find("input[name='txtLength']").val());
        if (width != "" && isNaN(width)) {
            sizeIsOk = false;
        }
        if (length != "" && isNaN(length)) {
            sizeIsOk = false;
        }
        if (width != "" && !isNaN(width) && length != "" && !isNaN(length)) {
            graphicSizes += width + "*" + length + "|";
        }
    })
    if (sizeIsOk == false) {
        alert("尺寸填写不正确");
        return false;
    }
    else if (graphicSizes == "") {
        alert("至少填写一个尺寸");
        return false;
    }
    Size.model.Format = format;
    Size.model.Sheet = sheet;
    Size.model.Remark = remark;
    Size.model.BigGraphicWidth = bigWidth;
    Size.model.BigGraphicLength = bigLength;
    Size.model.GraphicSizes = graphicSizes;

    return true;
}

function Edit(row) {
    if (row) {
        Size.model.Id = row.id;
        $("#selFormat").val(row.text);
        currSheet = row.Sheet;
        //Size.getSheetList();
        $("#txtRemark").val(row.Remark);
        $("#txtBigWidth").val(row.GraphicWidth);
        $("#txtBigLength").val(row.GraphicLength);
        var children = row.children;
        if (children) {
            var len = $(".divAddSize").length;
            if (children.length > len) {
                var count = children.length - len;
                for (var i = 0; i < count; i++) {
                    $("#spanAdd").click();
                }
            }
            len = children.length;
            for (var i = 0; i < len; i++) {
                
                var widthTxt = $(".divAddSize").eq(i).find("input[name='txtWidth']");
                var lengthTxt = $(".divAddSize").eq(i).find("input[name='txtLength']");
                widthTxt.val(children[i].GraphicWidth);
                lengthTxt.val(children[i].GraphicLength);
            }
        }
        Size.edit("update");
    }
}