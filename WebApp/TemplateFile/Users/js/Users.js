var UserJsonStr = "";
var CurrUserId = 0;
var User = {

    loadRoles: function () {
        var roleJson = $("#hfRoles").val() || "";
        if ($.trim(roleJson) != "") {
            var json = eval(roleJson);
            var div = "";
            for (var i = 0; i < json.length; i++) {
                div += "<div style='float:left;width:100px;'><input type='checkbox' name='cbRole' value='" + json[i].RoleId + "'>" + json[i].RoleName + "</div>";
            }
            $("#showRoles").html(div);
        }
    },
    bindCompanyList: function () {
        $("#companyInput").combotree({
            url: './Handler/UserHandler1.ashx?type=getCompany',
            panelHeight: 150,
            overflow: 'auto',
            valueField: 'id',
            textField: 'text',
            method: 'get'
        })
    },
    bindCustomerList: function (ids) {
        $.ajax({
            type: "get",
            url: "./Handler/UserHandler1.ashx?type=getCustomer",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var arr = [];
                    if ($.trim(ids) != "") {
                        arr = ids.split(',');
                    }
                    var json = eval(data);
                    var div = "<ul class='ul'>";
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";

                        if (arr.length > 0) {
                            $.each(arr, function (key, val) {
                                if (parseInt(val) == parseInt(json[i].CustomerId)) {
                                    checked = "checked='checked'";
                                }
                            })
                        }
                        div += "<li><input type='checkbox' name='cbCustomer' value='" + json[i].CustomerId + "' " + checked + ">" + json[i].CustomerName + "</li>";
                    }
                    div += "</ul>";
                    $("#customerContainer").html(div);
                }
            }
        })
    },
    bindActivity: function (aids) {
        $.ajax({
            type: "get",
            url: "./Handler/UserHandler1.ashx?type=getActivity",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var arr = [];
                    if ($.trim(aids) != "") {
                        arr = aids.split(',');
                    }
                    var json = eval(data);
                    var div = "<ul class='ul'>";
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";

                        if (arr.length > 0) {
                            $.each(arr, function (key, val) {
                                if (parseInt(val) == parseInt(json[i].ActivityId)) {
                                    checked = "checked='checked'";
                                }
                            })
                        }
                        div += "<li><input type='checkbox' name='cbActivity' value='" + json[i].ActivityId + "' " + checked + ">" + json[i].ActivityName + "</li>";
                    }
                    div += "</ul>";
                    $("#activityContainer").html(div);
                }
            }
        })
    },
    addUser: function (optype) {
        $("#editUserDiv").show().dialog({
            modal: true,
            width: 500,
            height: 400,
            iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
            title: optype == "add" ? '添加用户' : '编辑用户',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            if (CheckUserVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/UserHandler1.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(UserJsonStr),
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该登陆账号已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");

                                            //window.location.href = "List.aspx";
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
                            $("#editUserDiv").dialog('close');
                        }
                    }
                ]
        });
    },
    editUser: function () {
        User.addUser("update");
    }
}



$(function () {
    //添加用户
    $("#btnAdd").click(function () {
        ClearVal();
        User.bindCompanyList();
        User.loadRoles();
        User.bindCustomerList("");
        User.bindActivity();
        User.addUser("add");
    })


})
//编辑用户
function editUser(obj) {
    ClearVal();
    var name = $(obj).parent().siblings("td").eq(1).html();
    var username = $(obj).parent().siblings("td").eq(2).html();
    CurrUserId = $(obj).data("userid");
    var roleids = $(obj).siblings("input[name$='hfRoleIds']").val()||"";
    var customerids = $(obj).siblings("input[name$='hfCustomerIds']").val() || "";
    var activityids = $(obj).siblings("input[name$='hfAcivityIds']").val() || "";
    
    $("#txtRealName").val(name);
    $("#txtUserName").val(username);
    User.bindCompanyList();
    User.loadRoles();
    User.bindCustomerList(customerids);
    User.bindActivity(activityids);
    var companyId = $(obj).siblings("input[name$='hfCompanyId']").val()||"0";
    $("#companyInput").combotree('setValue', companyId);
    if ($.trim(roleids) != "") {
        var arr = roleids.split(',');
        $("#showRoles").find("input[name='cbRole']").each(function () {
            var f = false;
            var id = $(this).val();
            $.each(arr, function (key, val) {
                if (parseInt(val) == parseInt(id)) {
                    f = true;
                }
            })
            if (f) {
                $(this).attr("checked", true);
            }
        })
    }
    User.editUser();
}

function CheckUserVal() {
    UserJsonStr = "";
    var companyId = $("#companyInput").combotree('getValue')||"0";
    if (companyId == "0") {
        alert("请选择所属公司");
        return false;
    }
    var name = $("#txtRealName").val();
    var userName = $("#txtUserName").val();
    
    if ($.trim(name) == "") {
        alert("请填写用户姓名");
        return false;
    }
    if ($.trim(userName) == "") {
        alert("请填写登陆账号");
        return false;
    }
    var roles = "";
    $("#showRoles").find("input[name='cbRole']:checked").each(function () {
        roles += $(this).val() + ",";
    })
    if ($.trim(roles) == "") {
        alert("请选择角色");
        return false;

    }
    var roles = roles.substring(0, roles.length - 1);
    var customers = "";
    $("#customerContainer").find("input[name='cbCustomer']:checked").each(function () {
        customers += $(this).val() + ",";
    })
    var activies = "";
    $("#activityContainer").find("input[name='cbActivity']:checked").each(function () {
        activies += $(this).val() + ",";
    })
    UserJsonStr = '{"CompanyId":' + companyId + ',"UserId":' + CurrUserId + ',"UserName":"' + userName + '","RealName":"' + name + '","Roles":"' + roles + '","Customers":"' + customers + '","Activities":"' + activies + '"}';
    return true;

}

function ClearVal() {
    $("#txtRealName").val("");
    $("#txtUserName").val("");
    $("#showRoles").find("input[name='cbRole']").each(function () {
        $(this).checked = false;
    })
    UserJsonStr = "";
}