
$(function () {
    $("#btnAdd").on("click", function () {
        Activity.edit("add");
    })

    $("span[name='edit']").on("click", function () {
        currId = $(this).data("activityid");
        var name = $(this).parent().prev("td").html();
        $("#txtActivityName").val(name);
        Activity.edit("update");
    })
})

var JsonStr = "";
var currId = 0;
var Activity = {
    edit: function (optype) {
        $("#editDiv").show().dialog({
            modal: true,
            width: 500,
            height: 200,
            iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
            title: optype == "add" ? '添加活动' : '编辑活动',
            resizable: false,
            buttons: [
           {
               text: '确定',
               iconCls: 'icon-ok',
               handler: function () {
                   if (CheckVal()) {
                       $.ajax({
                           type: "get",
                           url: "/Customer/Handler/ActivityList.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(JsonStr),
                           success: function (data) {
                               if (data == "exist") {
                                   alert("该活动已存在！");

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
    var name = $.trim($("#txtActivityName").val());
    if (name == "") {
        alert("请填写活动名称");
        return false;
    }
    JsonStr = '{"ActivityId":' + currId + ',"ActivityName":"' + name + '"}';
    return true;
}