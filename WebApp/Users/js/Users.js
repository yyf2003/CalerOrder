var UserJsonStr = "";
var CurrUserId = 0;
var UserRegionIds = "";
var UserProvinceIds = "";
var UserCityIds = "";
var UserAreaIds = "";
var moduleJson;
var gridId;
var moduleHash = [];
var expandId = [];
var User = {

    loadRoles: function (roles) {
        var roleJson = $("#hfRoles").val() || "";
        roles = roles || "";
        if ($.trim(roleJson) != "") {
            var arr = [];
            if ($.trim(roles) != "") {
                arr = roles.split(',');
            }
            var json = eval(roleJson);
            var div = "";
            var flag = false;
            for (var i = 0; i < json.length; i++) {
                var checked = "";
                if (arr.length > 0) {
                    $.each(arr, function (key, val) {
                        if (parseInt(val) == parseInt(json[i].RoleId)) {
                            checked = "checked='checked'";
                            flag = true;
                        }
                    })
                }
                div += "<div style='float:left;width:100px; margin-bottom:5px;'><input type='checkbox' name='cbRole' value='" + json[i].RoleId + "' " + checked + ">&nbsp;" + json[i].RoleName + "</div>";
            }
            $("#showRoles").html(div);
            if (flag) {
                $("#showRoles").find("input[name='cbRole']").change();
            }
        }

    },
    bindUserLevel: function (userLevelId) {
        userLevelId = userLevelId || 0;

        document.getElementById("selUserLevel").length = 1;
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx?type=getUserLevel",
            cache: false,
            success: function (data) {
                if (data != "") {

                    var json = eval(data);

                    for (var i = 0; i < json.length; i++) {

                        var checked = "";
                        if (parseInt(userLevelId) == parseInt(json[i].LevelId)) {
                            checked = "selected='selected'";
                        }

                        var option = "<option value='" + json[i].LevelId + "' " + checked + ">" + json[i].LevelName + "</option>";
                        $("#selUserLevel").append(option);
                    }

                }
            },
            complete: function () {
                $("#selUserLevel").change();
            }
        })
    },
    loadRegions: function () {
        //var flag = false;

        $("#regionContainer").html("");
        $("#provinceContainer").html("");
        $("#areaContainer").html("");
        $("#cityContainer").html("");
        var customerId = 0;
        $("input[name='cbCustomer']:checked").each(function () {
            customerId = $(this).val();
        })
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx?type=getRegion&customerId=" + customerId,
            cache: false,
            //async:false,
            success: function (data) {

                if (data != "") {
                    var div = "";
                    var json = eval(data);
                    //var regionids = regionIds || "";
                    var arr = [];
                    if ($.trim(UserRegionIds) != "") {
                        arr = UserRegionIds.split(',');

                    }
                    var flag = false;
                    for (var i = 0; i < json.length; i++) {

                        var checked = "";
                        $.each(arr, function (key, val) {
                            if (parseInt(val) == parseInt(json[i].Id)) {
                                checked = "checked='checked'";
                                flag = true;
                            }
                        })

                        div += "<div style='float:left;width:100px; margin-bottom:5px;'><input type='checkbox' name='cbRegion' value='" + json[i].Id + "' " + checked + ">&nbsp;" + json[i].RegionName + "</div>";

                    }
                    $("#regionContainer").html(div);
                    if (flag) {
                        $("#regionContainer").find("input[name='cbRegion']").change();
                    }
                }
            }
        })
    },
    LoadProvince: function () {
        var regionIds = "";
        $("#provinceContainer").html("");
        $("#areaContainer").html("");
        $("#cityContainer").html("");
        $("#regionContainer").find("input[name='cbRegion']:checked").each(function () {
            var regionId = $(this).val();
            regionIds += regionId + ",";

        })
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx?type=getProvince&regionId=" + regionIds,
            cache: false,
            //async: false,
            success: function (data) {

                if (data != "") {
                    //var div = "";
                    var json = eval(data);
                    var regionName = "";
                    var table = "<table>";
                    var count = json.length;
                    var flag = false;
                    var arr = [];
                    if ($.trim(UserProvinceIds) != "") {
                        arr = UserProvinceIds.split(',');

                    }
                    for (var i = 0; i < count; i++) {
                        var region = json[i].RegionName;
                        var province = json[i].ProvinceName;
                        var provinceId = json[i].ProvinceId;
                        var checked = "";
                        $.each(arr, function (key, val) {
                            if (parseInt(val) == parseInt(provinceId)) {
                                checked = "checked='checked'";
                                flag = true;
                            }
                        })
                        var div = "";
                        div += "<div style='float:left;'>";
                        div += "<input type='checkbox' name='cbProvince' value='" + provinceId + "' " + checked + "/><span>" + province + "</span>&nbsp;";
                        div += "</div>";

                        if (regionName != region) {
                            if (i > 0)
                                table += "</td></tr>";
                            regionName = region;
                            table += "<tr>";
                            table += "<td style='width:60px;border-right:1px solid #ccc; vertical-align:top; padding-top:5px;'>" + regionName + "</td><td style='vertical-align:top; padding-top:5px;'>";
                        }
                        table += div;
                        if (i == count - 1)
                            table += "</td></tr>";

                    }
                    table += "</table>";
                    $("#provinceContainer").html(table);
                    if (flag) {
                        $("#provinceContainer").find("input[name='cbProvince']").change();
                    }
                }
            }
        })
    },
    LoadCity: function () {
        $("#areaContainer").html("");
        $("#cityContainer").html("");
        var provinceIds = "";
        $("#provinceContainer").find("input[name='cbProvince']:checked").each(function () {
            var provinceId = $(this).val();
            provinceIds += provinceId + ",";

        })
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx?type=getPlace&parentId=" + provinceIds,
            cache: false,
            //async: false,
            success: function (data) {

                if (data != "") {

                    var json = eval(data);
                    var proinceName = "";
                    var table = "<table>";
                    var count = json.length;
                    var flag = false;
                    var arr = [];
                    if ($.trim(UserCityIds) != "") {
                        arr = UserCityIds.split(',');

                    }
                    for (var i = 0; i < count; i++) {

                        var province = json[i].ParentName;
                        var city = json[i].PlaceName;
                        var cityId = json[i].PlaceId;
                        var checked = "";
                        $.each(arr, function (key, val) {
                            if (parseInt(val) == parseInt(cityId)) {
                                checked = "checked='checked'";
                                flag = true;
                            }
                        })
                        var div = "";
                        div += "<div style='float:left;'>";
                        div += "<input type='checkbox' name='cbCity' value='" + cityId + "' " + checked + "/><span>" + city + "</span>&nbsp;";
                        div += "</div>";

                        if (proinceName != province) {
                            if (i > 0)
                                table += "</td></tr>";
                            proinceName = province;
                            table += "<tr>";
                            table += "<td style='width:60px;border-right:1px solid #ccc; vertical-align:top; padding-top:5px;'>" + proinceName + "</td><td style='vertical-align:top; padding-top:5px;'>";
                        }
                        table += div;
                        if (i == count - 1)
                            table += "</td></tr>";
                    }
                    table += "</table>";
                    $("#cityContainer").html(table);
                    if (flag) {
                        $("#cityContainer").find("input[name='cbCity']").change();
                    }
                }
            }
        })
    },
    LoadArea: function () {
        var cityIds = "";
        $("#areaContainer").html("");
        $("#cityContainer").find("input[name='cbCity']:checked").each(function () {
            var cityId = $(this).val();
            cityIds += cityId + ",";

        })

        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx?type=getPlace&parentId=" + cityIds,
            cache: false,
            //async: false,
            success: function (data) {

                if (data != "") {

                    var json = eval(data);
                    var cityName = "";
                    var table = "<table>";
                    var count = json.length;
                    var arr = [];
                    if ($.trim(UserAreaIds) != "") {
                        arr = UserAreaIds.split(',');

                    }
                    for (var i = 0; i < count; i++) {

                        var city = json[i].ParentName;
                        var area = json[i].PlaceName;
                        var areaId = json[i].PlaceId;
                        var checked = "";
                        $.each(arr, function (key, val) {
                            if (parseInt(val) == parseInt(areaId)) {
                                checked = "checked='checked'";

                            }
                        })
                        var div = "";
                        div += "<div style='float:left;'>";
                        div += "<input type='checkbox' name='cbArea' value='" + areaId + "' " + checked + "/><span>" + area + "</span>&nbsp;";
                        div += "</div>";

                        if (cityName != city) {
                            if (i > 0)
                                table += "</td></tr>";
                            cityName = city;
                            table += "<tr>";
                            table += "<td style='width:60px;border-right:1px solid #ccc; vertical-align:top; padding-top:5px;'>" + cityName + "</td><td style='vertical-align:top; padding-top:5px;'>";
                        }
                        table += div;
                        if (i == count - 1)
                            table += "</td></tr>";
                    }
                    table += "</table>";
                    $("#areaContainer").html(table);
                }
            }
        })
    },
    bindCompanyList: function () {
        $("#companyInput").combotree({
            url: '/Users/Handler/Handler1.ashx?type=getCompany',
            panelHeight: 150,
            overflow: 'auto',
            valueField: 'id',
            textField: 'text',
            method: 'get',
            onLoadSuccess: function (node, data) {
                $("#companyInput").combotree("tree").tree("expandAll");
            }
        })
    },
    bindCustomerList: function (ids) {
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx?type=getCustomer",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var arr = [];
                    if ($.trim(ids) != "") {
                        arr = ids.split(',');
                    }
                    var json = eval(data);
                    var div = "<ul class='ul'>";
                    var flag = false;
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";

                        if (arr.length > 0) {
                            $.each(arr, function (key, val) {
                                if (parseInt(val) == parseInt(json[i].CustomerId)) {
                                    checked = "checked='checked'";
                                    flag = true;
                                }
                            })
                        }
                        div += "<li><input type='checkbox' name='cbCustomer' value='" + json[i].CustomerId + "' " + checked + ">&nbsp;" + json[i].CustomerName + "</li>";
                    }
                    div += "</ul>";
                    $("#customerContainer").html(div);
                    //                    if (flag) {
                    //                        $("#customerContainer").find("input[name='cbCustomer']").change();
                    //                    }
                }
            },
            complete: function () {
                $("#customerContainer").find("input[name='cbCustomer']").change();
            }
        })
    },
    addUser: function (optype) {
        $("#editUserDiv").show().dialog({
            modal: true,
            width: 650,
            height: 500,
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
                                    url: "/Users/Handler/Handler1.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(UserJsonStr),
                                    success: function (data) {
                                        if (data == "exist") {
                                            layer.msg("该登陆账号已存在！");

                                        }
                                        else if (data == "ok") {
                                            //alert("提交成功！");
                                            $("#btnSearch").click();
                                            //window.location.href = "List.aspx";
                                        }
                                        else
                                            layer.msg(data);
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
    },
    getUserRole: function () {
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx",
            data: { type: "getUserRole", userId: CurrUserId },

            success: function (data) {

                if (data != "") {
                    var roleJson = eval(data);

                    $("#seleRole").combobox({
                        data: roleJson,
                        valueField: 'RoleId',
                        textField: 'RoleName',
                        onSelect: function (item) {
                            User.getModules();
                        }
                    });
                    User.getModules();
                }
            }
        })

    },
    getModules: function () {
        var roleId = $("#seleRole").combobox('getValue') || 0;

        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx",
            data: { type: "getModules", userId: CurrUserId, roleId: roleId },
            cache: false,
            beforeSend: function () {
                $("#loadModuleImg").show();
            },
            complete: function () {
                $("#loadModuleImg").hide();
            },
            success: function (data) {

                if (data != "") {
                    moduleJson = eval("(" + data + ")");
                    User.bindModules($(window));
                }
            }
        })
    },
    bindModules: function (winsize) {
        gridId = $("#modulegrid").treegrid({
            title: '模块信息',
            rownumbers: true,
            width: '99%',
            height: 'auto',
            nowrap: false,
            resizable: true,
            collapsible: true,
            data: moduleJson,
            idField: 'id',
            treeField: 'text',
            animate: false,
            allowCellEdit: true,
            frozenColumns: [[
                    { title: '模块名称', field: 'text', width: 200 }

            ]],
            columns: [[
                    { title: 'Id', field: 'id', hidden: true },
                    { title: 'PId', field: 'ParentId', hidden: true },
                    {
                        title: '操作权限', field: 'OperateStr', width: 500, renderer: function (value, record) {
                            return value;
                        }
                    }

            ]],
            onLoadSuccess: function (row, data) {

                if ($("#cbExpand").attr("checked") == "checked")
                    gridId.treegrid("expandAll");
                else if (expandId.length > 0) {

                    $.each(expandId, function (key, val) {

                        gridId.treegrid("expand", val);
                    })
                }
            },
            onExpand: function (row) {

                var canPush = true;
                for (var i = 0; i < expandId.length; i++) {
                    if (expandId[i] == row.id) {
                        canPush = false;
                    }
                }
                if (canPush) {
                    expandId.push(row.id);
                }
            },
            onCollapse: function (row) {
                var canPOP = false;
                for (var i = 0; i < expandId.length; i++) {
                    if (expandId[i] == row.id) {
                        canPOP = true;
                    }
                }
                if (canPOP) {
                    expandId.pop(row.id);
                }
            }
        });
    },
    editOrderPermission: function () {

    },
    submitUserPermission: function () {
        var rows = gridId.treegrid('getRoots');
        $(".datagrid-wrap").find("div.divPermission").each(function () {
            var moduleId = $(this).data("moduleid");
            var operateStr = "";
            $(this).find("input.cbPermission:checked").each(function () {
                SaveIntoHash(moduleId, $(this).val());
            })
        })

        var jsonStr = "";
        var roleId = $("#seleRole").combobox('getValue') || 0;
        if (moduleHash.length == 0) {
            jsonStr += '{"UserId":"' + CurrUserId + '","RoleId":"' + roleId + '","ModuleId":"0","OperatePermission":""},';
        }
        else {
            for (key in moduleHash) {
                jsonStr += '{"UserId":"' + CurrUserId + '","RoleId":"' + roleId + '","ModuleId":"' + key + '","OperatePermission":"' + moduleHash[key] + '"},';
            }
        }
        if (jsonStr.length > 0)
            jsonStr = "[" + jsonStr.substring(0, jsonStr.length - 1) + "]";
        var exportChannelJson = "";
        $("input[name='channelPermission']:checked").each(function () {
            var channel = $(this).val();
            var format = "";
            $(this).parent().next().find("input[name='formatPermission']:checked").each(function () {
                format += $(this).val() + ",";
            })
            if (format != "") {
                format = format.substring(0, format.length - 1);
            }
            exportChannelJson += '{"UserId":' + CurrUserId + ',"Channel":"' + channel + '","Format":"' + format + '"},';
        })
        if (exportChannelJson.length > 0) {
            exportChannelJson = "[" + exportChannelJson.substring(0, exportChannelJson.length - 1) + "]";
            //exportChannelJson = exportChannelJson.substring(0, exportChannelJson.length - 1);
        }

        //alert(exportChannelJson);
        //return false;
        $.ajax({
            type: "post",
            url: "/Users/Handler/Handler1.ashx",
            data: { type: "updatePermission", jsonStr: jsonStr, exportChannelJsonStr: exportChannelJson },
            cache: false,
            success: function (data) {

                if (data == "ok") {
                    moduleHash = [];
                    $("#editUserPrimissionDiv").dialog('close');
                    expandId = [];
                    layer.msg("提交成功");
                }
                else
                    layer.msg(data);
            }
        })
    },
    getShopChannels: function () {
        $("#editPermissionChannelTable").html("");
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx",
            data: { type: "GetChannel", userId: CurrUserId },
            beforeSend: function () {
                $("#loadExportChannelImg").show();
            },
            complete: function () {
                $("#loadExportChannelImg").hide();
            },
            cache: false,
            success: function (data) {
                var data1 = data;
                if (data != "") {

                    var shopChannelJsonData = "";
                    var exportPermissionJson = [];
                    if (data.indexOf('|') != -1) {
                        shopChannelJsonData = data.split('|')[0];
                        var exportPermissionJsonData = data.split('|')[1];

                        exportPermissionJson = JSON.parse(exportPermissionJsonData);
                    }
                    else {
                        shopChannelJsonData = data;
                    }

                    var shopChannelJson = JSON.parse(shopChannelJsonData);

                    if (shopChannelJson.length > 0) {

                        for (var i = 0; i < shopChannelJson.length; i++) {
                            var channel = shopChannelJson[i].Channel;
                            var checked = "";
                            var disabled = "disabled='disabled'";
                            if (exportPermissionJson.length > 0) {
                                for (var ii = 0; ii < exportPermissionJson.length; ii++) {
                                    if (exportPermissionJson[ii].Channel == channel) {
                                        checked = "checked='checked'";
                                        disabled = "";
                                    }
                                }
                            }
                            var tr = "<tr class='tr_bai'>";
                            tr += "<td style='text-align:left;padding-left:20px;'><input type='checkbox' name='channelPermission' value='" + channel + "' " + checked + "/>" + channel + "</td>";
                            tr += "<td style='text-align:left; padding-left:5px;'>";
                            var formats = shopChannelJson[i].Formats;
                            for (var j = 0; j < formats.length; j++) {
                                var divFormat = "<div style='width:120px;'>";
                                divFormat += "<input type='checkbox' name='formatPermission'  value='" + formats[j].Format + "' " + disabled + "/>" + formats[j].Format;
                                divFormat += "</div>";
                                tr += divFormat;
                            }
                            tr += "</td>";
                            tr += "</tr>";
                            $("#editPermissionChannelTable").append(tr);
                        }
                    }
                }
            }
        })
    }

}

var Outsource = {
    getProvince: function () {
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx?type=getProvince",
            cache: false,
            //async: false,
            success: function (data) {
                document.getElementById("seleProvince").length = 1;
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var option = "<option value='" + json[i].ProvinceId + "'>" + json[i].ProvinceName + "</option>";
                        $("#seleProvince").append(option);
                    }
                }
            }
        })
    },
    getCity: function () {
        var provinceId = "";
        provinceId = $("#seleProvince").val();
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx?type=getPlace&parentId=" + provinceId,
            cache: false,
            //async: false,
            success: function (data) {
                document.getElementById("seleCity").length = 1;
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var option = "<option value='" + json[i].PlaceId + "'>" + json[i].PlaceName + "</option>";
                        $("#seleCity").append(option);
                    }
                }
            }
        })
    },
    getOutsource: function () {
        var provinceid = $("#seleProvince").val();
        var cityid = $("#seleCity").val();
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx",
            data: { type: "getOutsource", userId: CurrUserId, provinceId: provinceid, cityId: cityid },
            success: function (data) {
                $("#outsourceListDiv").html("");

                if (data != "") {
                    var json = eval(data);
                    var div = "";
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        if (json[i].Selected == 1) {
                            checked = "checked='checked'";
                        }
                        div += "<div style='margin-bottom:8px;'>";
                        div += "<input type='checkbox' name='outsourcecb' value='" + json[i].OutsourceId + "' " + checked + "/><span>" + json[i].OutsourceName + "</span>&nbsp;";
                        div += "</div>";
                    }
                    $("#outsourceListDiv").html(div);
                }
            }
        })
    },
    submit: function () {
        var jsonStr = "";
        $("input[name='outsourcecb']:checked").each(function () {
            jsonStr += '{"UserId":' + CurrUserId + ',"OutsourceId":"' + $(this).val() + '"},';
        })
        if (jsonStr != "") {
            jsonStr = "[" + jsonStr.substring(0, jsonStr.length - 1) + "]";
        }
        else {
            alert("请选择外协");
            return false;
        }
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx",
            data: { type: "editOutsource", jsonStr: jsonStr },
            success: function (data) {
                if (data == "ok") {
                    $("#seleProvince").val("0");
                    $("#seleCity").val("0");
                    document.getElementById("seleCity").length = 1;
                    alert("提交成功！");
                    $("#addOutsourceDiv").dialog('close');
                }
                else {
                    alert(data);
                }
            }
        })
    }
}

$(function () {
    //添加用户
    $("#btnAdd").click(function () {
        ClearVal();
        User.bindCompanyList();
        User.loadRoles();
        User.bindUserLevel();
        User.bindCustomerList("");
        User.addUser("add");
    })


    $("#selUserLevel").on("change", function () {
        DisplayRegion();
    })
    $("#customerContainer").delegate("input[name='cbRole'],input[name='cbCustomer']", "change", function () {
        //User.loadRegions();
        DisplayRegion();
    })

    $("#regionContainer").delegate("input[name='cbRegion']", "change", function () {
        User.LoadProvince();
    })
    $("#provinceContainer").delegate("input[name='cbProvince']", "change", function () {
        User.LoadCity();
    })
    $("#cityContainer").delegate("input[name='cbCity']", "change", function () {
        User.LoadArea();
    })

    $("#seleProvince").change(function () {
        Outsource.getCity();
        Outsource.getOutsource();
    })

    $("#seleCity").change(function () {
        Outsource.getOutsource();
    })

    $("#cbExpand").on("click", function () {
        if (this.checked)
            gridId.treegrid("expandAll");
        else {
            gridId.treegrid("collapseAll");
        }
        expandId = [];
    })

    $("#editPermissionChannelTable").delegate("input[name='channelPermission']", "change", function () {
        var checked = this.checked;
        if (checked) {
            $(this).parent().next().find("input[name='formatPermission']").each(function () {
                $(this).prop("disabled",false);
            })
        }
        else {
            $(this).parent().next().find("input[name='formatPermission']").each(function () {
                $(this).prop("checked", false).prop("disabled", true);
            })
        }
    })

})
//编辑用户
function editUser(obj) {
    ClearVal();
    CurrUserId = $(obj).data("userid");
    $.ajax({
        type: "get",
        url: "/Users/Handler/Handler1.ashx?type=getModel&userId=" + CurrUserId,
        cache: false,
        success: function (data) {

            if (data != "") {
                User.bindCompanyList();
                var json = eval(data);
                var companyId = json[0].CompanyId;
                $("#companyInput").combotree('setValue', companyId);
                $("#txtRealName").val(json[0].RealName);
                $("#txtUserName").val(json[0].UserName);
                var userLevelId = json[0].UserLevelId || 0;

                User.bindUserLevel(userLevelId);
                var customers = json[0].Customers || "";
                User.bindCustomerList(customers);

                var roles = json[0].Roles || "";
                User.loadRoles(roles);
                var regions = json[0].Regions || "";
                if (regions != "") {
                    UserRegionIds = regions.split('|')[0];
                    UserProvinceIds = regions.split('|')[1];
                    UserCityIds = regions.split('|')[2];
                    UserAreaIds = regions.split('|')[3];
                }
                User.editUser();
            }
        }
    })
}

function CheckUserVal() {
    UserJsonStr = "";
    var companyId = $("#companyInput").combotree('getValue') || "0";
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

    var userLevel = $("#selUserLevel").val();
    if (userLevel == 0) {
        alert("请选择用户级别");
        return false;

    }

    var roles = "";
    var selectRegion = false;
    $("#showRoles").find("input[name='cbRole']:checked").each(function () {
        var roleId = $(this).val();
        roles += roleId + ",";
        //        if(roleId==5 || roleId==6)
        //        {
        //           selectRegion=true;
        //        }
    })
    if ($.trim(roles) == "") {
        alert("请选择角色");
        return false;

    }
    if (roles.length > 0)
        roles = roles.substring(0, roles.length - 1);
    var customers = "";
    $("#customerContainer").find("input[name='cbCustomer']:checked").each(function () {

        customers += $(this).val() + ",";
    })
    var regionIds = "";
    var provinceIds = "";
    var cityIds = "";
    var areaIds = "";
    var regionIdStr = "";
    if (userLevel > 1) {

        //分区
        //if (userLevel == 2) {

        //}
        //else if (userLevel == 3) {//省级

        //}
        //else if (userLevel == 4) {//市级

        //}
        //else if (userLevel == 5) {//县级

        //}
        $("#regionContainer").find("input[name='cbRegion']:checked").each(function () {
            var regionId = $(this).val();
            regionIds += regionId + ",";

        })
        $("#provinceContainer").find("input[name='cbProvince']:checked").each(function () {
            var regionId = $(this).val();
            provinceIds += regionId + ",";

        })
        $("#cityContainer").find("input[name='cbCity']:checked").each(function () {
            var regionId = $(this).val();
            cityIds += regionId + ",";

        })
        $("#areaContainer").find("input[name='cbArea']:checked").each(function () {
            var regionId = $(this).val();
            areaIds += regionId + ",";

        })
        //        if (regionIds == "" && provinceIds == "" && cityIds == "" && areaIds=="") {
        //            alert("请选择负责区域");
        //            return false;
        //        }
        if (customers != "")
            customers = customers.substring(0, customers.length - 1);
        if (regionIds != "")
            regionIds = regionIds.substring(0, regionIds.length - 1);
        if (provinceIds != "")
            provinceIds = provinceIds.substring(0, provinceIds.length - 1);
        if (cityIds != "")
            cityIds = cityIds.substring(0, cityIds.length - 1);
        if (areaIds != "")
            areaIds = areaIds.substring(0, areaIds.length - 1);
        regionIdStr = regionIds + "|" + provinceIds + "|" + cityIds + "|" + areaIds;
    }

    UserJsonStr = '{"CompanyId":' + companyId + ',"UserId":' + CurrUserId + ',"UserName":"' + userName + '","RealName":"' + name + '","Roles":"' + roles + '","Customers":"' + customers + '","UserLevelId":"' + userLevel + '","Regions":"' + regionIdStr + '"}';

    return true;

}

function ClearVal() {
    $("#txtRealName").val("");
    $("#txtUserName").val("");
    $("#showRoles").find("input[name='cbRole']").each(function () {
        $(this).checked = false;
    })
    $(".userRegionTr").hide();
    UserJsonStr = "";
    UserRegionIds = "";
    UserProvinceIds = "";
    UserCityIds = "";
    UserAreaIds = "";
}

function DisplayRegion() {
    var flag = false;
   
    var levelId = $("#selUserLevel").val();
    var customerId = 0;
    $("input[name='cbCustomer']:checked").each(function () {
        customerId++;
    })
    if (levelId > 1 && customerId > 0) {
        $(".userRegionTr").show();
        User.loadRegions();
    }
    else
        $(".userRegionTr").hide();
}

function editOutsource(obj) {
    CurrUserId = $(obj).data("userid");
    Outsource.getOutsource();
    Outsource.getProvince();
    $("#addOutsourceDiv").show().dialog({
        modal: true,
        width: 650,
        height: 425,
        iconCls: 'icon-add',
        title: '添加外协',
        resizable: false,
        buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            Outsource.submit();
                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#addOutsourceDiv").dialog('close');
                        }
                    }
                ]
    });
}

function editPermission(obj) {
    expandId = [];
    CurrUserId = $(obj).data("userid");
    var userName = $(obj).data("username");
    $("#editPrimissionUserName").html(userName);
    User.getUserRole();
    User.getShopChannels();
    $("#editUserPrimissionDiv").show().dialog({
        modal: true,
        width: '90%',
        height: '80%',
        iconCls: 'icon-edit',
        title: '设置用户权限',
        resizable: false,
        buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            User.submitUserPermission();
                        }

                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editUserPrimissionDiv").dialog('close');
                        }
                    }
                ]
                });
    
}

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

function editOrderApproveRight(obj) {
    CurrUserId = $(obj).data("userid");
    $("#editOrderApprovePermission").show().dialog({
        modal: true,
        width: 500,
        height: 300,
        iconCls: 'icon-add',
        title: '设置订单审批类型',
        resizable: false,
        buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            
                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editOrderApprovePermission").dialog('close');
                        }
                    }
                ]
    });
}