

var JsonStr = "";
var CurrId = 0;
var MaterialCategory = {
    
    add: function (optype) {
        $("#editCategoryDiv").show().dialog({
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
                            if (CheckVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/MaterialCategory.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(TypeJsonStr),
                                    cache: false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该类型已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");

                                            window.location.href = "MaterialCategoryList.aspx";
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
    edit: function () {
        MaterialCategory.add("update");
    }
}



$(function () {
    //添加
    $("#btnAdd").click(function () {
        ClearVal();
        MaterialCategory.add("add");
    })


})
//编辑用户
function editCategory(obj) {
    ClearVal();
    var name = $(obj).parent().siblings("td").eq(1).html();
    CurrId = $(obj).data("categoryid");
    $("#txtCategoryName").val(name);
    MaterialCategory.edit();
}

function CheckVal() {
    JsonStr = "";

    var categoryName = $("#txtCategoryName").val();


    if ($.trim(categoryName) == "") {
        alert("请填写类型名称");
        return false;
    }
    TypeJsonStr = '{"Id":' + CurrId + ',"CategoryName":"' + categoryName + '"}';
    return true;

}

function ClearVal() {
    $("#txtCategoryName").val("");
    JsonStr = "";
}