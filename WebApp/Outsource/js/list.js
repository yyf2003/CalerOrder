

var pageIndex = 1;
var pageSize = 15;
var provinceId = 0;
var cityId = 0;
var companyProvinceIds = "";
var companyCityIds = "";
$(function () {
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), $("#btnRecover"), $("#separator1"));

    Outsource.getList(pageIndex, pageSize);


    $("#btnAdd").click(function () {
        ClearVal();

        Outsource.getProvince();
        Outsource.loadProvince();
        Outsource.getCustomerService();
        $("#editDiv").show().dialog({
            modal: true,
            width: 720,
            height: 500,
            iconCls: 'icon-add',
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
                            $("#editDiv").dialog('close');
                        }
                    }
                ]
        });
    });

    $("#btnEdit").click(function () {
        ClearVal();
        var rows = $("#tbCompany").datagrid("getSelections");
        if (rows.length == 0) {
            alert("请选择要编辑的行");
            return false;
        }
        else if (rows.length > 1) {
            alert("只能选择一行");
            return false;
        }
        else {
            var row = rows[0];
            Outsource.model.Id = row.Id;
            $("#txtCompanyCode").val(row.CompanyCode);
            $("#txtCompanyName").val(row.CompanyName);
            $("#txtShortName").val(row.ShortName);
            provinceId = row.ProvinceId;
            cityId = row.CityId;
            Outsource.getProvince();
            $("#txtAddress").val(row.Address);
            $("#txtContact").val(row.Contact);
            $("#txtTel").val(row.Tel);
            $("#txtJoinDate").val(row.JoinDate);
            Outsource.getCustomerService(row.CustomerServiceId);
            companyProvinceIds = row.ProvinceIds;
            companyCityIds = row.CityIds;
            Outsource.loadProvince();
            $("#editDiv").show().dialog({
                modal: true,
                width: 720,
                height: 500,
                iconCls: 'icon-add',
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
                            $("#editDiv").dialog('close');
                        }
                    }
                ]
            });
        }
    });

    $("#btnRefresh").click(function () {
        $("#tbCompany").datagrid("reload");
    });

    $("#btnDelete").click(function () {
        Outsource.deleteCompany();
    });

    $("#btnRecover").click(function () {
        Outsource.recoverCompany();
    });

    $("#btnCheck").click(function () {
        var rows = $("#tbCompany").datagrid("getSelections");
        if (rows.length == 0) {
            alert("请选择要查看的行");
            return false;
        }
        else if (rows.length > 1) {
            alert("只能选择一行");
            return false;
        }
        else {
            var row = rows[0];
            $("#labCompanyCode").text(row.CompanyCode);
            $("#labCompanyName").text(row.CompanyName);
            $("#labShortName").text(row.ShortName);
            $("#labProvinceName").text(row.Province);
            $("#labCityName").text(row.City);
            $("#labContacts").text(row.Contact);
            $("#labTel").text(row.Tel);
            $("#labAddress").text(row.Address);
            $("#labJoinDate").text(row.JoinDate);
            $("#labInchargeProvince").text(row.InchargeProvince);
            $("#labInchargeCity").text(row.InchargeCity);
            $("#labSCUserName").text(row.CustomerServiceName);
            $("#checkDiv").show().dialog({
                modal: true,
                width: 720,
                height: 330,
                iconCls: 'icon-tip',
                resizable: false,
                buttons: [
                    {
                        text: '关闭',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#checkDiv").dialog('close');
                        }
                    }
                ]
            });
        }
    });

    $("#selProvince").change(function () {
        Outsource.getCity();
    });

    $("#provinceContainer").delegate("input[name='cbProvince']", "change", function () {

        Outsource.loadCity();
    });
})


var Outsource = {
    model: function () {
        this.Id = 0;
        this.CompanyName = "";
        this.ShortName = "";
        //this.CompanyCode = "";
        this.ProvinceId = 0;
        this.CityId = 0;
        this.Address = "";
        this.JoinDate = "";
        //this.ERPHost = "";
        this.Contact = "";
        this.Tel = "";
        this.Regions = "";

    },
    getList: function (pageIndex, pageSize) {
        $("#tbCompany").datagrid({
            queryParams: { type: "getList", currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: 'handler/List.ashx',
            columns: [[

                        { field: 'rowIndex', title: '序号' },
                        { field: 'checked', checkbox: true },

                        { field: 'CompanyName', title: '外协名称', width: 200 },
                        { field: 'ShortName', title: '简称', width: 200 },
                        { field: 'CompanyCode', title: '公司编码', width: 100 },
                        { field: 'Province', title: '省份', width: 100 },
                        { field: 'City', title: '城市', width: 150 },
                        { field: 'Contact', title: '联系人', width: 100 },
                        { field: 'Tel', title: '联系人电话', width: 150 },
                        { field: 'Address', title: '地址', width: 350 },
                        { field: 'JoinDate', title: '加盟时间', width: 180 },
                        { field: 'InchargeProvince', title: '负责省份', width: 150 },
                        { field: 'InchargeCity', title: '负责城市', width: 350 },

                        { field: 'State', title: '状态', formatter: function (value, row) {
                            if (value == 0)
                                return "<span style='color:Red;'>已删除</span>";
                            else
                                return "<span>正常</span>";
                        }
                        },
                    { field: 'price', title: '查看报价', align: 'center', formatter: function (value, row) {
                        return "<span onclick='CheckPrice(" + row.Id + ")' style='color:blue;cursor:pointer;'>查看</span>";

                    }
                    }

            ]],
            height: "99%",
            toolbar: "#toolbar",
            pageList: [10, 15, 20],
            striped: true,
            border: false,
            singleSelect: false,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }



        });
        var p = $("#tbCompany").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                Outsource.getList(curIndex, curSize);
            }
        });
    },
    getProvince: function () {
        document.getElementById("selProvince").length = 1;
        $.ajax({
            type: "get",
            url: "/CompanyManage/handler/List1.ashx",
            data: { type: "getPlace", parentId: 0 },
            cache: true,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (provinceId == json[i].PlaceId) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].PlaceId + "' " + selected + ">" + json[i].PlaceName + "</option>";
                        $("#selProvince").append(option);
                    }
                    if (isSelected)
                        $("#selProvince").change();
                }
            }
        })
    },
    loadProvince: function () {
        $.ajax({
            type: "get",
            url: "/CompanyManage/handler/List1.ashx",
            data: { type: "getPlace", parentId: 0 },
            cache: true,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var div = "";
                    var flag = false;
                    var arr = [];
                    if ($.trim(companyProvinceIds) != "") {
                        arr = companyProvinceIds.split(',');
                    }
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        if (arr.length > 0) {
                            $.each(arr, function (key, val) {
                                if (parseInt(val) == parseInt(json[i].PlaceId)) {
                                    checked = "checked='checked'";
                                    flag = true;
                                }
                            })
                        }
                        div += "<div style='float:left;width:70px; margin-bottom:5px;'><input type='checkbox' name='cbProvince' value='" + json[i].PlaceId + "' " + checked + ">&nbsp;" + json[i].PlaceName + "</div>";
                    }
                    $("#provinceContainer").html(div);
                    if (flag) {
                        $("#provinceContainer").find("input[name='cbProvince']").change();
                    }
                }
            }
        })
    },
    getCity: function () {
        document.getElementById("selCity").length = 1;
        var parentId = $("#selProvince").val() || 0;
        $.ajax({
            type: "get",
            url: "/CompanyManage/handler/List1.ashx",
            data: { type: "getPlace", parentId: parentId },
            cache: true,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (cityId == json[i].PlaceId) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].PlaceId + "' " + selected + ">" + json[i].PlaceName + "</option>";
                        $("#selCity").append(option);
                    }
                }
            }
        })
    },
    loadCity: function () {
        $("#cityContainer").html("");
        var provinceIds = "";
        $("#provinceContainer").find("input[name='cbProvince']:checked").each(function () {
            var provinceId = $(this).val();
            provinceIds += provinceId + ",";

        })

        $.ajax({
            type: "get",
            url: "/CompanyManage/handler/List1.ashx",
            cache: false,
            data: { type: "getPlace", parentIds: provinceIds },
            success: function (data) {

                if (data != "") {

                    var json = eval(data);
                    var proinceName = "";
                    var table = "<table>";
                    var count = json.length;
                    var flag = false;
                    var arr = [];
                    if ($.trim(companyCityIds) != "") {
                        arr = companyCityIds.split(',');

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

                }
            }
        })
    },
    submit: function () {
        if (CheckVal()) {
            var jsonStr = '{"Id":' + (Outsource.model.Id || 0) + ',"CompanyName":"' + Outsource.model.CompanyName + '","ShortName":"' + Outsource.model.ShortName + '","Contact":"' + Outsource.model.Contact + '","Tel":"' + Outsource.model.Tel + '","ProvinceId":' + Outsource.model.ProvinceId + ',"CityId":' + Outsource.model.CityId + ',"Address":"' + Outsource.model.Address + '","Regions":"' + Outsource.model.Regions + '","JoinDate":"' + Outsource.model.JoinDate + '","CustomerServiceId":"' + Outsource.model.CustomerServiceId + '"}';

            $.ajax({
                type: "post",
                url: "handler/List.ashx",
                data: { type: "edit", jsonStr: escape(jsonStr) },
                success: function (data) {
                    if (data == "ok") {
                        $("#editDiv").dialog('close');
                        $("#tbCompany").datagrid("reload");

                    }
                    else if (data == "exist") {
                        alert("该公司名称已经存在");
                    }
                    else {
                        alert("提交失败：" + data);
                    }
                }
            })
        }
    },
    deleteCompany: function () {
        var rows = $("#tbCompany").datagrid("getSelections");
        if (rows.length == 0) {
            alert("请选择要删除的行");
            return false;
        } else {
            var ids = "";
            for (var i = 0; i < rows.length; i++) {
                ids += rows[i].Id + ",";
            }
            $.ajax({
                type: "post",
                url: "handler/List.ashx",
                data: { type: "delete", ids: ids },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbCompany").datagrid("reload");
                    }
                    else {
                        alert("删除失败！");
                    }
                }
            })
        }
    },
    recoverCompany: function () {
        var rows = $("#tbCompany").datagrid("getSelections");
        if (rows.length == 0) {
            alert("请选择要恢复的行");
            return false;
        } else {
            var ids = "";
            for (var i = 0; i < rows.length; i++) {
                ids += rows[i].Id + ",";
            }
            $.ajax({
                type: "post",
                url: "handler/List.ashx",
                data: { type: "recover", ids: ids },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbCompany").datagrid("reload");
                    }
                    else {
                        alert("恢复失败！");
                    }
                }
            })
        }
    },
    getCustomerService: function (uid) {

        document.getElementById("seleCustomerService").length = 1;
        $.ajax({
            type: "get",
            url: "handler/List.ashx?type=getCustomerService",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].UserId == uid) {
                            selected = "selected='selected'";
                        }
                        var option = "<option value='" + json[i].UserId + "' " + selected + ">" + json[i].UserName + "</option>";
                        $("#seleCustomerService").append(option);
                    }
                }
            }
        });
    }
}

function CheckVal() {
    var companyCode = $.trim($("#txtCompanyCode").val());
    var companyName = $.trim($("#txtCompanyName").val());
    var shortName = $.trim($("#txtShortName").val());
    var provinceId = $("#selProvince").val();
    var cityId = $("#selCity").val();
    var address = $.trim($("#txtAddress").val());
    var contact = $.trim($("#txtContact").val());
    var tel = $.trim($("#txtTel").val());
    var joinDate = $.trim($("#txtJoinDate").val());
    var csId = $.trim($("#seleCustomerService").val());
    if (companyName == "") {
        alert("请填写公司名称");
        return false;
    }
    if (provinceId == 0) {
        alert("请选择省份");
        return false;
    }
    if (cityId == 0) {
        alert("请选择城市");
        return false;
    }
    if (csId == 0) {
        alert("请选择负责客服");
        return false;
    }
    var regionIdStr = "";
    var provinceIds = "";
    var cityIds = "";
    $("#provinceContainer").find("input[name='cbProvince']:checked").each(function () {

        provinceIds += $(this).val() + ",";

    })
    $("#cityContainer").find("input[name='cbCity']:checked").each(function () {
        var regionId = $(this).val();
        cityIds += regionId + ",";

    })
    if (provinceIds != "")
        provinceIds = provinceIds.substring(0, provinceIds.length - 1);
    else {
        alert("请选择负责省份");
        return false;
    }
    if (cityIds != "")
        cityIds = cityIds.substring(0, cityIds.length - 1);
    regionIdStr = provinceIds + "|" + cityIds;

    Outsource.model.CompanyCode = companyCode;
    Outsource.model.CompanyName = companyName;
    Outsource.model.ShortName = shortName;
    Outsource.model.Contact = contact;
    Outsource.model.Tel = tel;
    Outsource.model.ProvinceId = provinceId;
    Outsource.model.CityId = cityId;
    Outsource.model.Address = address;
    Outsource.model.Regions = regionIdStr;
    Outsource.model.JoinDate = joinDate;
    Outsource.model.CustomerServiceId = csId;
    return true;
}

function ClearVal() {
    provinceId = 0;
    cityId = 0;
    companyProvinceIds = "";
    companyCityIds = "";
    $("#provinceContainer").html("");
    $("#cityContainer").html("");
    $("#editDiv").find("input").val("");
    $("select").val("0");

    Outsource.model.Id = 0;

    Outsource.model.CompanyName = "";
    //Outsource.model.CompanyCode = "";
    Outsource.model.ShortName = "";
    Outsource.model.Contact = "";
    Outsource.model.Tel = "";
    Outsource.model.ProvinceId = 0;
    Outsource.model.CityId = 0;
    Outsource.model.Address = "";
    Outsource.model.Regions = "";
    Outsource.model.JoinDate = "";
    Outsource.model.CustomerServiceId = 0;
}