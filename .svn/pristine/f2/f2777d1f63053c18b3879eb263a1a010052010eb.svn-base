var RoleJsonStr = "";
var CurrRoleId=0;
var Role = {

    
    addRole: function (optype) {
        $("#editRoleDiv").show().dialog({
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
                            if (CheckUserVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/RoleHandler1.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(RoleJsonStr),
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该角色已存在！");

                                        }
                                        else if (data == "ok") {
                                            
                                            window.location.href = "List.aspx";
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
                            $("#editRoleDiv").dialog('close');
                        }
                    }
                ]
        });
    },
    editRole: function () {
        Role.addRole("update");
    }
}



$(function () {
    //添加用户
    $("#btnAdd").click(function () {
        ClearVal();
        Role.addRole("add");
    })


})
//编辑用户
function editRole(obj) {
    ClearVal();
    var name = $(obj).parent().siblings("td").eq(1).html();
    CurrRoleId = $(obj).data("roleid");
    $("#txtRoleName").val(name);

    Role.editRole();
}

function CheckUserVal() {
    RoleJsonStr = "";
    var name = $("#txtRoleName").val();
    
    if ($.trim(name) == "") {
        alert("请填写角色名称");
        return false;
    }
    RoleJsonStr = '{"RoleId":' + CurrRoleId + ',"RoleName":"' + name + '"}';
    return true;

}

function ClearVal() {
    $("#txtRoleName").val("");

    RoleJsonStr = "";
}