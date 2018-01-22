
$(function () {
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));
    Material.getCategory();
    Material.getMaterialList(pageIndex, pageSize);


    $("#btnAdd").click(function () {
        ClearVal();
        Material.bindCategory();
        Material.bindUnit();
        $("#editMaterialDiv").show().dialog({
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

    })

    $("#btnEdit").click(function () {
        ClearVal();

        var row = $("#tbMaterial").datagrid("getSelected");
        if (row == null) {
            alert("请选择要编辑的行");
        }
        else {
            Material.bindCategory();
            Material.bindUnit();
            var id = row.Id;
            Material.model.MaterialId = id;
            $.ajax({
                type: "get",
                url: "./Handler/BasicMaterialList.ashx?type=getModel&id=" + id,
                cache: false,
                success: function (data) {
                    if (data != "") {
                        var json = eval(data);
                        $("#selCategory").val(json[0].MaterialCategoryId);
                        $("#txtMaterialName").val(json[0].MaterialName);
                        $("#selUnit").val(json[0].UnitId);
                        $("#editMaterialDiv").show().dialog({
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
                }
            })
        }
    })

    $("#btnDelete").click(function () {
        Material.update(1);
    })
    $("#btnRecover").click(function () {
        Material.update(0);
    })

    $("#btnImport").click(function () {
        $("#iframe1").css({ height: 300 + "px" }).attr("src", "ImportBasicMaterial.aspx");
        $("#importMaterialDiv").show().dialog({
            modal: true,
            width: 700,
            height: 350,
            iconCls: 'icon-add',
            resizable: false,
            onClose: function () {
                if ($("#hfIsFinishImport").val() == "1") {
                    $("#hfIsFinishImport").val("0");
                    $("#tbMaterial").datagrid("reload");

                }
                $("#iframe1").attr("src", "");
            }
        });
    })

    $("#btnExport").click(function () {
        $("#iframe1").attr("src", "ExportBasicMaterial.aspx");
    })
})

var currCategoryId = 0;
var pageIndex = 1;
var pageSize = 15;
var Material = {
    model: function () {
        this.MaterialId = 0;
        this.CategoryId = 0;
        this.MaterialName = "";
        this.UnitId = 0;
    },
    getCategory: function () {
        $("#tbMaterialType").datagrid({
            method: 'get',
            url: './Handler/BasicMaterialList.ashx?type=getCategory',
            columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'CategoryName', title: '类别名称' }
                ]],
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onSelect: function (rowIndex, data) {
                go(data.Id, data.CategoryName);
            }
        })
    },
    getMaterialList: function (pageIndex, pageSize) {
        $("#tbMaterial").datagrid({
            queryParams: { type: "getList", categoryId: currCategoryId, currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: './Handler/BasicMaterialList.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'CategoryName', title: '类别名称' },
                        { field: 'MaterialName', title: '材料名称' },
                        { field: 'Unit', title: '单位' },
                        { field: 'State', title: '状态', formatter: function (value, row) {
                            if (value == "已删除")
                                return "<span style='color:Red;'>" + value + "</span>";
                            else
                                return value;
                        }
                        }

            ]],
            height: "100%",
            toolbar: "#toolbar",
            pageList: [10,15,20],
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
    bindCategory: function () {
        document.getElementById("selCategory").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/BasicMaterialList.ashx?type=getCategory",
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {

                        var option = "<option value='" + json[i].Id + "'>" + json[i].CategoryName + "</option>";
                        $("#selCategory").append(option);
                    }
                }
            }
        })
    },
    bindUnit: function () {
        document.getElementById("selUnit").length = 1
        $.ajax({
            type: "get",
            url: "./Handler/BasicMaterialList.ashx?type=getUnit",
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var option = "<option value='" + json[i].Id + "'>" + json[i].UnitName + "</option>";
                        $("#selUnit").append(option);
                    }
                }
            }
        })
    },
    submit: function () {
        if (CheckVal()) {

            var jsonStr = '{"Id":' + (this.model.MaterialId || 0) + ',"MaterialCategoryId":' + this.model.CategoryId + ',"MaterialName":"' + this.model.MaterialName + '","UnitId":' + this.model.UnitId + '}';

            $.ajax({
                type: "post",
                url: "./Handler/BasicMaterialList.ashx",
                data: { type: "edit", jsonstr: escape(jsonStr) },
                success: function (data) {
                    if (data == "ok") {
                        $("#editMaterialDiv").dialog('close');
                        $("#tbMaterial").datagrid("reload");

                    }
                    else if (data == "exist") {
                        alert("该材质名称已经存在");
                    }
                    else {
                        alert("提交失败：" + data);
                    }
                }
            })
        }
    },
    update: function (state) {
        var row = $("#tbMaterial").datagrid("getSelected");
        var msg = state == 1 ? "删除" : "恢复";
        if (row == null) {
            alert("请选择要" + msg + "的行");
        }
        else {
            var id = row.Id;
            $.ajax({
                type: "get",
                url: "./Handler/BasicMaterialList.ashx?type=update&id=" + id + "&state=" + state,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbMaterial").datagrid("reload");
                    }
                    else {
                        alert("提交失败！");
                    }
                }
            })
        }
    }
};

function go(cId, cName) {
    currCategoryId = cId || 0;

    $("#materialTitle").panel({
        title: ">>类别名称：" + cName
    });
    Material.getMaterialList(pageIndex, pageSize);
}

function CheckVal() {
    var categoryId = $("#selCategory").val();
    var materialName = $.trim($("#txtMaterialName").val());
    var unitId = $("#selUnit").val();
    if (categoryId == "0") {
        alert("请选择类别");
        return false;
    }
    if (materialName == "") {
        alert("请填写材质名称");
        return false;
    }
    if (unitId == "0") {
        alert("请选择单位");
        return false;
    }
    Material.model.CategoryId = categoryId;
    Material.model.MaterialName = materialName;
    Material.model.UnitId = unitId;
    return true;
}

function ClearVal() {
    currCategoryId = 0;
    Material.model.MaterialId = 0;
    $("#txtMaterialName").val("");
}

