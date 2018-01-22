
var currTypeId = 0;
var pageIndex = 1;
var pageSize = 15;

var provinceId = 0;
var cityId = 0;
var companyProvinceIds = "";
var companyCityIds = "";
$(function () {

    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), $("#btnRecover"), $("#separator1"));

    Company.getTypeList();
    Company.getCompanyList(pageIndex, pageSize);

    $("#btnCheck").click(function () {
        var row = $("#tbCompany").datagrid("getSelected");
        if (row == null) {
            alert("请选择要查看的行");
        }
        else {
            $("#labTypeName").text(row.TypeName);
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

    $("#btnAdd").click(function () {
        ClearVal();
        Company.bindTypeList();
        Company.getProvince();
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
                            Company.submit();
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
        var row = $("#tbCompany").datagrid("getSelected");
        if (row == null) {
            alert("请选择要编辑的行");
        }
        else {
            Company.model.Id = row.Id;
            Company.bindTypeList(row.TypeId);
            $("#txtCompanyName").val(row.CompanyName);
            $("#txtShortName").val(row.ShortName);
            provinceId = row.ProvinceId;
            cityId = row.CityId;
            Company.getProvince();
            $("#txtAddress").val(row.Address);
            $("#txtContact").val(row.Contact);
            $("#txtTel").val(row.Tel);
            $("#txtJoinDate").val(row.JoinDate);
            if (row.TypeId > 1) {
                companyProvinceIds = row.ProvinceIds;
                companyCityIds = row.CityIds;
            }
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
                            Company.submit();
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
    })

    $("#btnDelete").click(function () {
        Company.deleteCompany();
    });

    $("#btnRecover").click(function () {
        Company.recoverCompany();
    });


    $("#selProvince").change(function () {
        Company.getCity();
    });

    $("#selType").change(function () {
        var id = $(this).val();
        if (id > 1) {
            $("#regionDiv").show();
            Company.loadProvince();
        }
        else {
            $("#regionDiv").hide();
        }
    })
    $("#provinceContainer").delegate("input[name='cbProvince']", "change", function () {

        Company.loadCity();
    })
})


var Company = {
    model: function () {
        this.Id = 0;
        //this.ParentId = 0;
        this.TypeId = 0;
        this.CompanyName = "";
        this.ShortName = "";
        this.Contact = "";
        this.Tel = "";
        this.ProvinceId = 0;
        this.CityId = 0;
        this.Address = "";
        this.Regions = "";
        this.JoinDate = "";
    },
    getTypeList: function () {
        $("#tbCompanyType").datagrid({
            method: 'get',
            cache: false,
            url: './handler/List1.ashx?type=getTypeList',
            columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'TypeName', title: '类型名称' }
                ]],
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onSelect: function (rowIndex, data) {
                go(data.Id, data.TypeName);
            }
        })
    },
    bindTypeList: function (TypeId) {
        document.getElementById("selType").length = 1;
        $.ajax({
            type: "get",
            url: "./handler/List1.ashx?type=getTypeList",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 1; i < json.length; i++) {
                        var selected = "";
                        if (TypeId && TypeId == json[i].Id) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].TypeName + "</option>";
                        $("#selType").append(option);
                    }
                    if (isSelected)
                        $("#selType").change();
                }
            }
        })
    },
    getCompanyList: function (pageIndex, pageSize) {
        $("#tbCompany").datagrid({
            queryParams: { type: "getList", typeId: currTypeId, currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: './handler/List1.ashx',
            columns: [[
                        { field: 'rowIndex', title: '序号' },
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'CompanyName', title: '公司名称' },
                        { field: 'ShortName', title: '简称' },
                        { field: 'CompanyCode', title: '公司编码' },
                        { field: 'TypeName', title: '类型名称' },
                        { field: 'Province', title: '省份' },
                        { field: 'City', title: '城市' },
                        { field: 'Contact', title: '联系人' },
                        { field: 'Tel', title: '联系人电话' },
                        { field: 'Address', title: '公司地址' },
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
            pageList: [10, 15, 20],
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
        var p = $("#tbCompany").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {

                Company.getCompanyList(curIndex, curSize);
            }
        });
    },
    getProvince: function () {
        document.getElementById("selProvince").length = 1;
        $.ajax({
            type: "get",
            url: "./handler/List1.ashx",
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
            url: "./handler/List1.ashx",
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
                        $.each(arr, function (key, val) {
                            if (parseInt(val) == parseInt(json[i].PlaceId)) {
                                checked = "checked='checked'";
                                flag = true;
                            }
                        })
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
            url: "./handler/List1.ashx",
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
            url: "./handler/List1.ashx",
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
            var jsonStr = '{"Id":' + (Company.model.Id || 0) + ',"TypeId":' + (Company.model.TypeId || 0) + ',"CompanyName":"' + Company.model.CompanyName + '","ShortName":"' + Company.model.ShortName + '","Contact":"' + Company.model.Contact + '","Tel":"' + Company.model.Tel + '","ProvinceId":' + Company.model.ProvinceId + ',"CityId":' + Company.model.CityId + ',"Address":"' + Company.model.Address + '","Regions":"' + Company.model.Regions + '","JoinDate":"' + Company.model.JoinDate + '"}';
            $.ajax({
                type: "post",
                url: "./handler/List1.ashx",
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
                url: "./handler/List1.ashx",
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
                url: "./handler/List1.ashx",
                data: { type: "recover", ids: ids },
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
    }
}

function go(cId, cName) {
    currTypeId = cId || 0;

    $("#materialTitle").panel({
        title: ">>类型名称：" + cName
    });
    Company.getCompanyList(pageIndex, pageSize);
}

function CheckVal() {
    var typeId = $("#selType").val();
    var companyName = $.trim($("#txtCompanyName").val());
    var shortName = $.trim($("#txtShortName").val());
    var provinceId = $("#selProvince").val();
    var cityId = $("#selCity").val();
    var address = $.trim($("#txtAddress").val());
    var contact = $.trim($("#txtContact").val());
    var tel = $.trim($("#txtTel").val());
    var joinDate = $.trim($("#txtJoinDate").val());
    if (typeId == 0) {
        alert("请选择公司类型");
        return false;
    }
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

    var regionIdStr = "";
    var provinceIds = "";
    var cityIds = "";
    $("#provinceContainer").find("input[name='cbProvince']:checked").each(function () {
        var regionId = $(this).val();
        provinceIds += regionId + ",";

    })
    $("#cityContainer").find("input[name='cbCity']:checked").each(function () {
        var regionId = $(this).val();
        cityIds += regionId + ",";

    })
    if (provinceIds != "")
        provinceIds = provinceIds.substring(0, provinceIds.length - 1);
    if (cityIds != "")
        cityIds = cityIds.substring(0, cityIds.length - 1);
    regionIdStr = provinceIds + "|" + cityIds;

    Company.model.TypeId = typeId;
    Company.model.CompanyName = companyName;
    Company.model.ShortName = shortName;
    Company.model.Contact = contact;
    Company.model.Tel = tel;
    Company.model.ProvinceId = provinceId;
    Company.model.CityId = cityId;
    Company.model.Address = address;
    Company.model.Regions = regionIdStr;
    Company.model.JoinDate = joinDate;
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
    $("#regionDiv").hide();
    Company.model.Id = 0;
    Company.model.TypeId = 0;
    Company.model.CompanyName = "";
    Company.model.ShortName = "";
    Company.model.Contact = "";
    Company.model.Tel = "";
    Company.model.ProvinceId = 0;
    Company.model.CityId = 0;
    Company.model.Address = "";
    Company.model.Regions = "";
    Company.model.JoinDate = "";
}

