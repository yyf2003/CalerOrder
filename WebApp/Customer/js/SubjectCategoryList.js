


$(function () {
    $("#btnAdd").on("click", function () {
        ClearVal();
        SubjectCategory.edit("add");
    })

    $("span[name='edit']").on("click", function () {
        ClearVal();
        currId = $(this).data("categoryid");
        var customerId = $(this).data("customerid");
        $("#ddlCustomer").val(customerId);
        var name = $(this).parent().prev("td").html();
        $("#txtCategoryName").val(name);
        SubjectCategory.edit("update");
    })
})

var JsonStr = "";
var currId = 0;
var SubjectCategory = {
    edit: function (optype) {
        $("#editDiv").show().dialog({
            modal: true,
            width: 500,
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
                           url: "/Customer/Handler/SubjectCategoryList.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(JsonStr),
                           success: function (data) {
                               if (data == "exist") {
                                   alert("该类型已存在！");

                               }
                               else if (data == "ok") {
                                   //alert("提交成功！");
                                   $("#btnSearch").click();
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
    }
};

function CheckVal() {
    var customerId = $("#ddlCustomer").val();
    var name = $.trim($("#txtCategoryName").val());
    if (name == "") {
        alert("请填写类型名称");
        return false;
    }
    JsonStr = '{"Id":' + currId + ',"CustomerID":' + customerId + ',"CategoryName":"' + name + '"}';
    return true;
}

function ClearVal() {
    $("#ddlCustomer").val("0");
    $("#txtCategoryName").val("");
}