
var provinceIds = "";
var cityIds = "";
var pageIndex = 1;
var pageSize = 15;
$(function () {
    
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));

    Host.getList(pageIndex, pageSize);
    $("#btnRefresh").click(function () {
        $("#tbList").datagrid("reload");
    })

    $("#btnAdd").click(function () {
        ClearVal();
        Host.LoadProvince();
        Host.edit("add");
    })

    $("#btnEdit").click(function () {
        ClearVal();
        var rows = $("#tbList").datagrid("getSelections");
        if (rows.length == 0) {
            alert("请选择要编辑的行");
            return false;
        }
        else if (rows.length > 1) {
            alert("只能选择一行");
            return false;
        }
        else {
            var id = rows[0].Id;
            Host.model.Id = id;
            $("#txtClientName").val(rows[0].ClientName);
            $("#txtClientNo").val(rows[0].ClientNo);
            $("#txtClientHost").val(rows[0].ClientHost);
            provinceIds = rows[0].ProvinceIds;
            cityIds = rows[0].CityIds;
            Host.LoadProvince();
            Host.edit("update");
        }
    })

    $("#btnDelete").click(function () {
        Host.deleteHost();
    })

    $("#provinceContainer").delegate("input[name='cbProvince']", "change", function () {
        Host.LoadCity();
    })

})



var Host = {
    model: function () {
        this.Id = 0;
        this.ClientName = "";
        this.ClientNo = "";
        this.ClientHost = "";
        this.Provinces = "";
        this.Cities = "";
    },
    getList: function (pageIndex, pageSize) {
        $("#tbList").datagrid({
            queryParams: { type: "getList", currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: 'Handler/ERPHostManage.ashx',
            columns: [[

                        { field: 'RowIndex', title: '序号' },
                        { field: 'checked', checkbox: true },
                        { field: 'Id', title: '序号', hidden: true },
                        { field: 'ClientName', title: '客户公司名称', width: 150 },
                        { field: 'ClientNo', title: '客户公司编号', width: 150 },
                        { field: 'ClientHost', title: '服务器域名', width: 150 },
                        { field: 'ProvinceName', title: '负责省份', width: 300 },
                        { field: 'CityName', title: '负责城市', width: 350 },

                        { field: 'State', title: '状态', formatter: function (value, row) {
                            if (value == "未激活")
                                return "<span style='color:Red;'>" + value + "</span>";
                            else
                                return value;
                        }
                        }

            ]],
            height: "520px",
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
        var p = $("#tbList").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                Host.getList(curIndex, curSize);
            }
        });

    },
    LoadProvince: function () {

        $("#provinceContainer").html("");
        $("#cityContainer").html("");
        $.ajax({
            type: "get",
            url: "/Users/Handler/Handler1.ashx?type=getProvince",
            cache: false,

            success: function (data) {

                if (data != "") {
                    //var div = "";
                    var json = eval(data);

                    var count = json.length;
                    var flag = false;
                    var arr = [];
                    if (provinceIds != "") {
                        arr = provinceIds.split(',');
                    }
                    var div = "";
                    for (var i = 0; i < count; i++) {

                        var province = json[i].ProvinceName;
                        var provinceId = json[i].ProvinceId;
                        var checked = "";
                        $.each(arr, function (key, val) {
                            if (parseInt(val) == parseInt(provinceId)) {
                                checked = "checked='checked'";
                                flag = true;
                            }
                        })

                        div += "<div style='float:left; height:20px;line-height:20px;'>";
                        div += "<input type='checkbox' name='cbProvince' value='" + provinceId + "' " + checked + "/><span>" + province + "</span>&nbsp;";
                        div += "</div>";



                    }
                    $("#provinceContainer").html(div);
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
                    if (cityIds != "") {
                        arr = cityIds.split(',');
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
                        div += "<div style='float:left;height:20px;line-height:20px;'>";
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
    edit: function (optype) {
        $("#editDiv").show().dialog({
            modal: true,
            width: 620,
            height: 400,
            iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
            title: optype == "add" ? '添加' : '编辑',
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            if (CheckVal()) {
                                var jsonStr = '{"Id":' + (Host.model.Id || 0) + ',"ClientName":"' + Host.model.ClientName + '","ClientNo":"' + Host.model.ClientNo + '","ClientHost":"' + Host.model.ClientHost + '","Provinces":"' + Host.model.Provinces + '","Cities":"' + Host.model.Cities + '"}';

                                $.ajax({
                                    type: "post",
                                    url: "Handler/ERPHostManage.ashx",
                                    data: { type: "edit", jsonString: escape(jsonStr) },
                                    success: function (data) {
                                        if (data == "ok") {
                                            $("#editDiv").dialog('close');
                                            $("#tbList").datagrid("reload");
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
    },
    deleteHost: function () {
        var rows = $("#tbList").datagrid("getSelections");
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
                url: "Handler/ERPHostManage.ashx",
                data: { type: "delete", ids: ids },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        
                        $("#tbList").datagrid("reload");
                    }
                    else {
                        alert("删除失败！");
                    }
                }
            })
        }
    }
};

function CheckVal() {
    var clientName = $.trim($("#txtClientName").val());
    var clientNo = $.trim($("#txtClientNo").val());
    var clientHost = $.trim($("#txtClientHost").val());
    var provinceIds = "";
    var cityIds = "";
    $("#provinceContainer").find("input[name='cbProvince']:checked").each(function () {
        provinceIds += $(this).val() + ",";
    })
    $("#cityContainer").find("input[name='cbCity']:checked").each(function () {
        
        cityIds += $(this).val() + ",";
    })
    if (clientName == "") {
        alert("请输入客户公司名称");
        return false;
    }
    if (clientNo == "") {
        alert("请输入客户公司编号");
        return false;
    }
    if (clientHost == "") {
        alert("请输入服务器域名");
        return false;
    }
    if (provinceIds != "")
        provinceIds = provinceIds.substring(0, provinceIds.length - 1);
    if (cityIds != "")
        cityIds = cityIds.substring(0, cityIds.length - 1);

    Host.model.ClientName = clientName;
    Host.model.ClientNo = clientNo;
    Host.model.ClientHost = clientHost;
    Host.model.Provinces = provinceIds;
    Host.model.Cities = cityIds;
    return true;
}

function ClearVal() {
    $("#txtClientName").val("");
    $("#txtClientNo").val("");
    $("#txtClientHost").val("");
    $("#provinceContainer").find("input[name='cbProvince']").each(function () {
        $(this).checked = false;
    })
    $("#provinceContainer").html("");
    Host.model.Id = 0;
    Host.model.ClientName = "";
    Host.model.ClientNo = "";
    Host.model.ClientHost = "";
    Host.model.Provinces = "";
    Host.model.Cities = "";
    provinceIds = "";
    cityIds = "";
}
