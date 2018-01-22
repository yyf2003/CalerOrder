

var TypeJsonStr = "";
var CurrTypeId = 0;
var MaterialType = {
    bindBigTypeList: function () {
        $("#parentInput").combotree({
            url: './Handler/MaterialType.ashx?type=getParent',
            panelHeight: 150,
            overflow: 'auto',
            valueField: 'id',
            textField: 'text',
            method: 'get'
        })
    },
    addType: function (optype) {
        $("#editTypeDiv").show().dialog({
            modal: true,
            width: 400,
            height: 200,
            iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
            title: optype == "add" ? '添加类型' : '编辑类型',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            if (CheckTypeVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/MaterialType.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(TypeJsonStr),
                                    cache:false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该类型已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");

                                            window.location.href = "MaterialTypeList.aspx";
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
                            $("#editTypeDiv").dialog('close');
                        }
                    }
                ]
        });
    },
    editType: function () {
        MaterialType.addType("update");
    }
}



$(function () {
    //添加
    $("#btnAdd").click(function () {
        ClearVal();
        MaterialType.bindBigTypeList();
        MaterialType.addType("add");
    })


})
//编辑用户
function editType(obj) {
    ClearVal();
    var name = $(obj).parent().siblings("td").eq(1).html();
    CurrTypeId = $(obj).data("typeid");
    $("#txtTypeName").val(name);
    MaterialType.bindBigTypeList();
    var parentId = $(obj).siblings("input[name$='hfParentId']").val() || "0";
    $("#parentInput").combotree('setValue', parentId);
    MaterialType.editType();
}

function CheckTypeVal() {
    TypeJsonStr = "";
    var parentId = $("#parentInput").combotree('getValue') || "0";
    if (parentId == "0") {
        alert("请选择所属大类");
        return false;
    }
    var typeName = $("#txtTypeName").val();


    if ($.trim(typeName) == "") {
        alert("请填写类型名称");
        return false;
    }
    TypeJsonStr = '{"Id":' + CurrTypeId + ',"ParentId":' + parentId + ',"MaterialTypeName":"' + typeName + '"}';
    return true;

}

function ClearVal() {
    $("#txtTypeName").val("");
    TypeJsonStr = "";
}