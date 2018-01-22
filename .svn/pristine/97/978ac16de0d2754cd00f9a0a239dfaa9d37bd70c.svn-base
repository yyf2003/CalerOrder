

var currRoleId = 0;
var currModuleId = 0;
var moduleHash = new Array();
var setting = {
    check: {
        enable: true
    },
    data: {
        simpleData: {
            enable: true
        }
    }

};

var roles = {
    BindRole: function () {

        $("#tbRoles").datagrid({
            method: 'get',
            url: './Handler/AuthorityHandler1.ashx?type=getroles',
            columns: [[
                    { field: 'RoleId', hidden: true },
                    { field: 'RoleName', title: '角色名称' }
                ]],
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onSelect: function (rowIndex, data) {

                go(data.RoleId, data.RoleName);
            }
        })
    },
    BindModuleDDL: function () {

        $("#ModuleInput").combotree({
            url: './Handler/AuthorityHandler1.ashx?type=getFirstLevelModule',
            panelHeight: 150,
            overflow: 'auto',
            valueField: 'id',
            textField: 'text',
            method: 'get',
            onChange: function () {
                currModuleId = $("#ModuleInput").combotree("getValue") || 0;
                roles.BindModules();
            }
        })
    },
    BindModules: function () {

        $.ajax({
            type: "get",
            url: "./Handler/AuthorityHandler1.ashx?type=getModules&roleId=" + currRoleId + "&moduleId=" + currModuleId + "&t=" + new Date().getTime(),
            cache: false,
            success: function (data) {

                var zNodes = eval("(" + data + ")");
                $.fn.zTree.init($("#tree1"), setting, zNodes);
            }
        })
    },
    Submit: function () {
        if (currRoleId == 0) {
            alert("请选择角色");
            return;
        }
        var parentModuleId = $("#ModuleInput").combotree("getValue") || 0;
        var treeObj = $.fn.zTree.getZTreeObj("tree1");
        var nodes = treeObj.getCheckedNodes(true);
        $("#showWait").show();
        $("#btnSave").hide();
        var mids = "";
        for (var i = 0; i < nodes.length; i++) {
            var IsOperateType = nodes[i].IsOperateType || 0;
            if (IsOperateType == 1) {
                SaveIntoHash(nodes[i].pId, nodes[i].id);
            }

        }
        var jsonStr = "";
        for (key in moduleHash) {
            jsonStr += '{"RoleId":"' + currRoleId + '","ModuleId":"' + key + '","OperatePermission":"' + moduleHash[key] + '"},';

        }
        if (jsonStr.length > 0)
            jsonStr = "[" + jsonStr.substring(0, jsonStr.length - 1) + "]";
       
        $.ajax({
            type: "post",
            //url: "./Handler/AuthorityHandler1.ashx?type=updatePermission&parentModuleId=" + parentModuleId + "&roleId=" + currRoleId + "&jsonStr=" + jsonStr + "&t=" + new Date().getTime(),
            url: "./Handler/AuthorityHandler1.ashx",
            data: { type: "updatePermission", parentModuleId: parentModuleId, roleId: currRoleId, jsonStr: jsonStr },
            cache:false,
            success: function (data) {
                if (data == "ok") {
                    alert("提交成功！");
                    $("#showWait").hide();
                    $("#btnSave").show();
                }
                else
                    alert(data);
            }
        })


    }
}


$(function () {
    roles.BindRole();
    roles.BindModuleDDL();
    roles.BindModules();

    $("#btnSave").click(function () {
       
        moduleHash = new Array();
        roles.Submit();

    })

})

function SaveIntoHash(moduleid, operate) {
    var flag = false;
    for (key in moduleHash) {
        if (parseInt(key) == parseInt(moduleid)) {
            flag = true;
            break;
        }
    }
    if (flag) {
        moduleHash[moduleid] = moduleHash[moduleid] + "|" + operate;
    }
    else {
        moduleHash[moduleid] = operate;
    }
}

function go(rid, roleName) {
    currRoleId = rid||0;
    
    $("#moduleTitle").panel({
        title: ">>角色：" + roleName
    });
    roles.BindModules();
}