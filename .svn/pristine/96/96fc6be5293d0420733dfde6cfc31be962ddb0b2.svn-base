var json;
var parentModuleJson;
var gridId;
var jsonStr = "";
var parentNodeId = 0;
var isAddNew = 0;
var moduleGrid = {
    databind: function (winsize) {

        gridId = $("#modulegrid").treegrid({

            title: '模块管理',
            rownumbers: true,
            width: '100%',
            height: 'auto',
            nowrap: false,
            resizable: true,
            collapsible: false,
            //data: json,
            url: './Handler/Handler1.ashx?type=getList&t=' + Math.random(),
            idField: 'id',
            treeField: 'text',
            animate: true,
            toolbar: '#toolbar',
            cache: false,
            frozenColumns: [[
                    { title: '模块名称', field: 'text', width: 200 }

                ]],
            columns: [[
                    { title: 'Id', field: 'id', hidden: true },
                    { title: 'PId', field: 'ParentId', hidden: true },
                    { title: 'Url', field: 'Url', width: 300 },
                    { title: '排序', field: 'OrderNum', width: 80 },
                    { title: '是否显示', field: 'IsShow', width: 80 },
                    { title: '是否显示在首页', field: 'IsShowOnHome', width: 100 },
                    { title: '图片', field: 'OrderNum1', width: 80,
                        formatter: function (value, row, index) {
                            if (row.ParentId > 0 && row.IsShowOnHome == "是") {
                                var imgUrl = row.ImgUrl || "";
                                //if (imgUrl == "") {
                                //imgUrl = "/image/close.gif";
                                //}
                                return "<div style='position:relative;'><img src='" + imgUrl + "' width='24' height='25'/><span onclick='UploadImg(" + row.id + ")' style='color:blue;cursor:pointer;margin-top:3px;margin-left:6px; position:absolute'>编辑</span></div>";
                                //return "<span>" + row.ParentId + "</span>";
                            }
                        }
                    },
                ]],
            onLoadSuccess: function (row, data) {

            },
            onDblClickRow: function (row) {
                var id = row.id;

                moduleGrid.editModule(id);

            }
        });
    },
    refresh: function () {
        $("#modulegrid").treegrid('reload');
        //GetModuleJson();
    },
    selected: function () {
        return gridId.treegrid('getSelected');
    },
    bindParentModule: function () {

        $("#parentModuleInput").combotree({
            url: "./Handler/Handler1.ashx?type=getParentList&t=" + new Date().getTime(),
            panelHeight: 150,
            overflow: 'auto',
            valueField: 'id',
            textField: 'text',
            method: 'get',
            onChange: function () {
                if (isAddNew==1) {
                    var parentId = $("#parentModuleInput").combotree('getValue');
                    $.ajax({
                        type: "get",
                        url: "./Handler/Handler1.ashx?type=getOrderNum&parentId=" + parentId,
                        success: function (data) {
                            if (data != "") {
                                $("#txtOrderNum").val(parseInt(data) + 1);
                            }
                        }
                    })
                }

            }
        })

    },
    addModule: function (optype) {

        if (CheckVal()) {
            $.ajax({
                type: "get",
                url: "./Handler/Handler1.ashx?type=edit&jsonString=" + escape(jsonStr) + "&optype=" + optype,
                cache: false,
                success: function (data) {
                    var msg = optype == "add" ? "添加" : "更新";
                    if (data == "error") {
                        alert(msg + "失败！");
                    }
                    else {
                        $("#editModuleDiv").dialog('close');
                        moduleGrid.refresh();
                        //GetModuleJson();
                        //$('#modulegrid').treegrid('reload', parentNodeId);
                        //window.frames["uploadframe"].upload(data);
                    }
                }
            })
        }

    },
    editModule: function (moduleId) {
        isAddNew = 0;
        var mid = moduleId || 0;
        if (mid > 0) {
            gridId.treegrid('select', mid);
        }
        var selectedNode = moduleGrid.selected();

        if (selectedNode) {
           
            moduleGrid.bindParentModule();
            //var parentNode = gridId.treegrid('getParent', mid);
            var parentId = selectedNode.ParentId == 0 ? -100 : selectedNode.ParentId;
            //var parentId = parentNode.Id;
            //parentId = parentId == 0 ? -100 : parentId;
            parentNodeId = parentId;
            $("#parentModuleInput").combotree('setValue', parentId);
            $("#txtName").val(selectedNode.text);
            $("#txtUrl").val(selectedNode.Url);
            $("#txtOrderNum").val(selectedNode.OrderNum);
            var isShow = selectedNode.IsShow;
            $("input:radio[name='rblIsShow']").each(function () {
                if ($(this).val() == isShow)
                    $(this).attr("checked", "checked");

            })
            var isShowOnHome = selectedNode.IsShowOnHome;
            //alert(isShowOnHome);
            $("input:radio[name='rblShowOnHome']").each(function () {
                if ($(this).val() == isShowOnHome)
                    $(this).attr("checked", "checked");

            })
            $("#editModuleDiv").show().dialog({
                modal: true,
                width: 450,
                height: 330,
                iconCls: 'icon-add',
                resizable: false,
                buttons: [
                        {
                            text: '确定',
                            iconCls: 'icon-ok',
                            handler: function () {
                                moduleGrid.addModule("edit");
                                //alert($("#parentModuleInput").combotree('getValue'));
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
                url: "/Modules/Handler/Handler1.ashx?type=deleteModule&moduleId=" + moduleId,
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        //alert("删除成功");
                        moduleGrid.refresh();
                    }
                    else if (data == "hasChildren") {
                        alert("该项目下存在子菜单，不能删除");
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


function GetModuleJson() {
    $.ajax({
        type: "get",
        url: "./Handler/Handler1.ashx?type=getList&t=" + Math.random(),
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

    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));

    //GetModuleJson();
    moduleGrid.databind($(window));

    $("#btnRefresh").click(function () {
        ClearInput();
        moduleGrid.refresh();
    })

    $("#btnAdd").click(function () {
        isAddNew = 1;
        ClearInput();
        moduleGrid.bindParentModule();
        $("#editModuleDiv").show().dialog({
            modal: true,
            width: 450,
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
        if (confirm("确定删除吗？")) {
            moduleGrid.deleteModule();
        }
    })

    //    $("#showfiles").delegate("img", "mouseover", function () {
    //        var div = $(this).parent();
    //        div.addClass('addBorder').siblings().removeClass('addBorder');
    //    })

    $("#showfiles").delegate("input[name='cbImg']", "change", function () {

        var div = $(this).parent().parent();
        div.siblings().find("input[name='cbImg']").attr("checked", false);

    })

})


function CheckVal() {
    var selectedNode = moduleGrid.selected();
    var id = 0;
    if (selectedNode)
        id = selectedNode.id;
    var parentId = $("#parentModuleInput").combotree('getValue');

    var moduleName = $("#txtName").val();
    var url = $("#txtUrl").val();
    var orderNum = $("#txtOrderNum").val() || "-1";
    var isShow = $("input:radio[name='rblIsShow']:checked").val() || "是";
    var isShowOnHome = $("input:radio[name='rblShowOnHome']:checked").val() || "否";
    isShow = isShow == "是" ? "1" : "0";
    isShowOnHome = isShowOnHome == "是" ? "1" : "0";
    if ($.trim(moduleName) == "") {
        alert("请输入模块名称");
        return false;
    }

    jsonStr = '{"Id":"' + id + '","ModuleName":"' + moduleName + '","ParentId":"' + parentId + '","Url":"' + url + '","OrderNum":"' + orderNum + '","IsShow":' + isShow + ',"IsShowOnHome":' + isShowOnHome + '}';
    return true;
}

function UploadImg(moduleId) {
    var selectedNode = moduleGrid.selected();
    if (selectedNode)
        parentNodeId = selectedNode.ParentId;

    GetFiles();
    $("#ImgDiv").show().dialog({
        modal: true,
        width: 620,
        height: 480,
        iconCls: 'icon-add',
        resizable: false,
        buttons: [
                        {
                            text: '确定',
                            iconCls: 'icon-ok',
                            handler: function () {
                                var imgId = $("input[name='cbImg']:checked").eq(0).val() || 0;
                                moduleId = moduleId || 0;
                                $.ajax({
                                    type: "get",
                                    url: "Handler/Handler1.ashx",
                                    data: { type: "editImg", moduleId: moduleId, imgId: imgId },
                                    success: function (data) {
                                        if (data == "ok") {
                                            moduleGrid.refresh();
                                            $("#ImgDiv").dialog('close');


                                        }
                                        else {
                                            alert("提交失败");
                                        }
                                    }
                                })

                            }
                        },
                        {
                            text: '取消',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                $("#ImgDiv").dialog('close');
                            }
                        }
                    ]
    });
}

function GetFiles() {
    $("#loadingImg").show();
    $("#showfiles").html("");
    $.ajax({
        type: 'get',
        url: './UploadHandler.ashx?type=getfiles',
        cache: false,
        success: function (data) {
            $("#loadingImg").hide();
            if (data != "") {
                var FileJson = eval("(" + data + ")");
                for (var i = 0; i < FileJson.length; i++) {
                    ShowFiles(FileJson[i]);
                }
            }
        }
    })
}

function ShowFiles(json) {

    if (json != null) {
        //$("#tr").css({ display: "block" });
        var id = json.Id;
        var SourcePath = json.Url;

        //var exten = SourcePath.substring(SourcePath.lastIndexOf('.') + 1);
        SourcePath = SourcePath.replace("./", "../");
        SourcePath = SourcePath.replace("~/", "../");
        var div;

        div = "<div style='float:left;margin-right:10px; margin-top:15px; width:70px; height:68px;text-align:center;'><img data-id='" + id + "' style='cursor:pointer;position:relative;' src='" + SourcePath + "' style='max-width:65px;max-height:65px;' />";
        div += "<div style='text-align:right;'><input type='checkbox' name='cbImg' value='" + id + "'/></div>";
        div += "</div>";
        $("#showfiles").append(div);


    }
}