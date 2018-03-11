
var currChannel = "";
var currFormat = "";
var currRegion = 0;
var currProvinceId = "";
var currCityId = "";
$(function () {
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));
    Confing.getList();

    $("#btnRefresh").click(function () {
        Confing.getList();
    })

    $("#btnAdd").click(function () {
        ClearVal();
        Confing.getChannel();
        Confing.getRegionList();

        layer.open({
            type: 1,
            time: 0,
            title: '新增设置',
            skin: 'layui-layer-rim', //加上边框
            area: ['800px', '500px'],
            content: $("#editDiv"),
            id: 'popLayer',
            btn: ['提 交'],
            btnAlign: 'c',
            yes: function () {
                Confing.submit();
            },
            cancel: function (index) {
                layer.closeAll();
                $("#editDiv").hide();

            }

        });
    })

    $("#btnEdit").click(function () {
        ClearVal();
        var rows = $("#tbList").datagrid("getSelections");
        if (rows.length == 0) {
            layer.msg("请选择要编辑的行");
            return false;
        }
        else if (rows.length > 1) {
            layer("只能选择一行");
            return false;
        }
        if (rows) {
            Confing.model.Id = rows[0].Id;
            currChannel = rows[0].Channel;
            currFormat = rows[0].Format;
            Confing.getChannel();
            currRegion = rows[0].RegionId;
            currProvinceId = rows[0].ProvinceId;
            currCityId = rows[0].CityId;
            $("#seleCustomer").val(rows[0].CustomerId);
            Confing.getRegionList();
            $("#seleConfigType").val(rows[0].TypeId);
            $("#txtMaterialName").val(rows[0].MaterialName);
            if (rows[0].OutsourctId > 0)
                $("#seleOutsource").val(rows[0].OutsourctId);
            //$("#txtChannel").val(rows[0].Channel);
            //$("#txtFormat").val(rows[0].Format);
            if (rows[0].TypeId != 1) {
                $("#txtMaterialName").val("").attr("disabled", "disabled");
                $("#seleOutsource").val("-1").attr("disabled", "disabled");
            }
            layer.open({
                type: 1,
                time: 0,
                title: '新增设置',
                skin: 'layui-layer-rim', //加上边框
                area: ['800px', '500px'],
                content: $("#editDiv"),
                id: 'popLayer',
                btn: ['提 交'],
                btnAlign: 'c',
                yes: function () {
                    Confing.submit();
                },
                cancel: function (index) {
                    layer.closeAll();
                    $("#editDiv").hide();

                }

            });
        }
    })

    $("#btnDelete").click(function () {
        var rows = $("#tbList").datagrid("getSelections");
        if (rows.length == 0) {
            layer.msg("请选择要删除的行");
            return false;
        }
        var ids = "";
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].Id + ",";
        }
        if (ids.length > 0) {
            layer.confirm('确定删除吗？', {
                btn: ['是', '否'] //按钮
            }, function () {
                $.ajax({
                    type: "post",
                    url: "AssignConfig.ashx",
                    data: { type: "delete", ids: ids },
                    success: function (data) {
                        if (data == "ok") {
                            layer.closeAll();
                            Confing.getList();

                        }
                        else {
                            layer.msg(data);
                        }
                    }
                })
            }, function () {

            });
        }
        else {
            layer.msg("请选择要删除的行");
        }
    })

    $("#seleCustomer").change(function () {
        Confing.getRegionList();
    })
    $("#regionContainer").delegate("input[name='rdRegion']", "change", function () {
        Confing.getProvinceList();
    })
    $("#provinceContainer").delegate("input[name='cbProvince']", "change", function () {
        Confing.getCityList();
    })

    $("#seleConfigType").change(function () {
        var val = $(this).val();
        if (val == "1") {
            $("#txtMaterialName").attr("disabled", false);
            $("#seleOutsource").val("-1").attr("disabled", false);

        }
        else {
            $("#txtMaterialName").val("").attr("disabled", "disabled");
            $("#seleOutsource").val("-1").attr("disabled", "disabled");
        }
    })



    //    $("#cbAllCity").change(function () {
    //        var checked = this.checked;
    //        $("input[name='cbCity']").each(function () {
    //            this.checked = checked;
    //        });
    //    })

    $("#cityContainer").delegate("input[name='cbAllCity']", "click", function () {
        var checked = this.checked;
        $(this).parent("td").next().find("input[name='cbCity']").each(function () {
            this.checked = checked;
        })
    })
    $("#cityContainer").delegate("input[name='cbCity']", "change", function () {

        if (!this.checked) {
            $(this).parent("td").prev().find("input[name='cbAllCity']").prop("checked", false);
            //$("#cbAllCity").prop("checked", false);
        }
        else {
            var checked = $("input[name='cbCity']:checked").length == $("input[name='cbCity']").length;
            //$("#cbAllCity").prop("checked", checked);
            $(this).parent("td").prev().find("input[name='cbAllCity']").prop("checked", checked);
        }
    })

    $("#cbAllChannel").click(function () {
        var checked = this.checked;
        $("input[name='cbChannel']").each(function () {
            this.checked = checked;
        })
        Confing.getFormat();
    })

    $("#channelContainer").delegate("input[name='cbChannel']", "change", function () {
        if (!this.checked) {
            $("#cbAllChannel").prop("checked", false);
        }
        else {
            var checked = $("input[name='cbAllChannel']:checked").length == $("input[name='cbAllChannel']").length;
            $("#cbAllChannel").prop("checked", checked);
        }
        Confing.getFormat();
    })

    $("#cbAllFormat").click(function () {
        var checked = this.checked;
        $("input[name='cbFormat']").each(function () {
            this.checked = checked;
        })
    })

    $("#formatContainer").delegate("input[name='cbFormat']", "change", function () {
        if (!this.checked) {
            $("#cbAllFormat").prop("checked", false);
        }
        else {
            var checked = $("input[name='cbFormat']:checked").length == $("input[name='cbFormat']").length;
            $("#cbAllFormat").prop("checked", checked);
        }
    })

})

var Confing = {
    model: function () {
        this.Id = 0;
        this.CustomerId = 0;
        this.ConfigTypeId = 0;
        this.ProductOutsourctId = 0;
        this.MaterialName = "";
        this.AssignType = 0;
        this.Region = "";
        this.ProvinceId = "";
        this.CityId = "";
        this.Channel = "";
        this.Format = "";

    },
    getList: function () {
        $("#tbList").datagrid({
            method: 'get',
            url: 'AssignConfig.ashx?type=getList',
            columns: [[
            //{ field: 'Id', hidden: true },
               {field: 'RowIndex', title: "序号" },
               { field: 'checked', checkbox: true },
               { field: 'TypeName', title: "类型" },
               { field: 'MaterialName', title: "材质名称" },
               { field: 'OutsourctName', title: "生产外协" },
               { field: 'Channel', title: "店铺Channel" },
               { field: 'Format', title: "店铺Format" },
               { field: 'RegionName', title: "应用区域" },
               { field: 'ProvinceName', title: "应用省份" },
               { field: 'CityName', title: "应用城市" }

            ]],
            toolbar: '#toolbar',
            singleSelect: false,
            striped: true,
            border: false,
            pagination: false,
            fitColumns: false,
            height: 'auto',
            fit: false,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            //selectOnCheck: false,
            //checkOnSelect:false,
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }
        })
    },
    getCustomerList: function () {
        document.getElementById("seleCustomer").length = 0;
        $.ajax({
            type: "get",
            url: "AssignConfig.ashx?type=getCustomer",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currCustomerId == json[i].CustomerId)
                            selected = "selected='selected'";
                        var option = "<option value='" + json[i].CustomerId + "' " + selected + ">" + json[i].CustomerName + "</option>";
                        $("#seleCustomer").append(option);
                    }

                }
            },
            complete: function () {
                $("#seleCustomer").change();
            }
        })
    },
    getConfigTypeList: function () {
        document.getElementById("seleConfigType").length = 1;
        $.ajax({
            type: "get",
            url: "AssignConfig.ashx?type=getConfigType",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currTypeId == json[i].TypeId)
                            selected = "selected='selected'";
                        var option = "<option value='" + json[i].TypeId + "' " + selected + ">" + json[i].TypeName + "</option>";
                        $("#seleConfigType").append(option);
                    }

                }
            }
        })
    },
    getRegionList: function () {
        var customerId = $("#seleCustomer").val() || 0;
        $("#regionContainer").html("");
        $("#provinceContainer").html("");
        $("#cityContainer").html("");
        $("#cbAllCity").prop("checked", false);

        $.ajax({
            type: "get",
            url: "AssignConfig.ashx?type=getRegion&customerId=" + customerId,
            cache: false,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var div = "";
                    var flag = false;
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        if (currRegion == json[i].Id) {
                            checked = "checked='checked'";
                            flag = true;
                        }
                        div += "<input type='radio' name='rdRegion' value='" + json[i].Id + "' " + checked + "/>" + json[i].RegionName + "&nbsp;&nbsp;";
                    }
                    $("#regionContainer").html(div);
                    if (flag) {
                        Confing.getProvinceList();
                    }
                }
            }
        })
    },
    getProvinceList: function () {
        var regionId = $("input[name='rdRegion']:checked").val() || 0;
        $("#provinceContainer").html("");
        $("#cityContainer").html("");
        $("#cbAllCity").prop("checked", false);
        $.ajax({
            type: "get",
            url: "AssignConfig.ashx?type=getProvince&regionId=" + regionId,
            cache: false,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var div = "";
                    var flag = false;
                    var arr = [];
                    if ($.trim(currProvinceId) != "") {
                        arr = currProvinceId.split(',');
                    }
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        $.each(arr, function (key, val) {
                            if (parseInt(val) == parseInt(json[i].ProvinceId)) {
                                checked = "checked='checked'";
                                flag = true;
                            }
                        })

                        div += "<input type='checkbox' name='cbProvince' value='" + json[i].ProvinceId + "' " + checked + "/>" + json[i].ProvinceName + "&nbsp;&nbsp;";
                    }
                    $("#provinceContainer").html(div);
                    if (flag) {
                        Confing.getCityList();
                    }
                }
            }
        })
    },
    getCityList: function () {
        $("#cityContainer").html("");
        $("#cbAllCity").prop("checked", false);
        var provinceId = "";
        $("input[name='cbProvince']:checked").each(function () {
            provinceId += $(this).val() + ",";
        })
        $.ajax({
            type: "get",
            url: "AssignConfig.ashx?type=getCity&provinceId=" + provinceId,
            cache: false,
            success: function (data) {

                if (data != "") {

                    var json = eval(data);
                    var provinceName = "";
                    var table = "<table>";
                    var count = json.length;
                    var flag = false;
                    var arr = [];
                    if ($.trim(currCityId) != "") {
                        arr = currCityId.split(',');
                    }
                    for (var i = 0; i < count; i++) {
                        var provinceId = json[i].ParentId;
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
                        div += "<input type='checkbox' name='cbCity' data-provinceid='" + provinceId + "' value='" + cityId + "' " + checked + "/><span>" + city + "</span>&nbsp;";
                        div += "</div>";

                        if (provinceName != province) {
                            if (i > 0)
                                table += "</td></tr>";
                            provinceName = province;
                            table += "<tr>";
                            table += "<td style='width:60px;border-right:1px solid #ccc; vertical-align:top; padding-top:5px;padding-left:5px;'>" + provinceName + "<input type='checkbox' name='cbAllCity'></td><td style='vertical-align:top; padding-top:5px;padding-left:5px;border-right:0px'>";
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
    getChannel: function () {
        $("#cbAllChannel").prop("checked", false);
        $("#formatContainer").html("");
        
        $.ajax({
            type: "get",
            url: "AssignConfig.ashx?type=getChannel",
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var div = "";
                    var flag = false;
                    var arr = [];
                    if ($.trim(currChannel) != "") {
                        arr = currChannel.split(',');
                    }
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        $.each(arr, function (key, val) {
                            if (val== json[i].Channel) {
                                checked = "checked='checked'";
                                flag = true;
                            }
                        })

                        div += "<div style='float:left;'><input type='checkbox' name='cbChannel' value='" + json[i].Channel + "' " + checked + "/>" + json[i].Channel + "&nbsp;&nbsp;</div>";
                    }
                    $("#channelContainer").html(div);
                    if (flag) {
                        Confing.getFormat();
                    }
                }
            }
        })
    },
    getFormat: function () {
        $("#formatContainer").html("");
        $("#cbAllFormat").prop("checked", false);
        var channels = "";
        $("input[name='cbChannel']:checked").each(function () {
            channels += $(this).val() + ",";
        })

        $.ajax({
            type: "get",
            url: "AssignConfig.ashx?type=getFormat&channel=" + channels,
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var div = "";

                    var arr = [];
                    if ($.trim(currFormat) != "") {
                        arr = currFormat.split(',');
                    }
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        $.each(arr, function (key, val) {
                            if (val == json[i].Format) {
                                checked = "checked='checked'";

                            }
                        })

                        div += "<div style='float:left;'><input type='checkbox' name='cbFormat' value='" + json[i].Format + "' " + checked + "/>" + json[i].Format + "&nbsp;&nbsp;</div>";
                    }
                    $("#formatContainer").html(div);

                }
            }
        })
    },
    submit: function () {
        if (CheckVal()) {
            var jsonStr = '{"Id":' + (this.model.Id || 0) + ',"CustomerId":' + this.model.CustomerId + ',"ConfigTypeId":' + this.model.ConfigTypeId + ',"MaterialName":"' + this.model.MaterialName + '","ProductOutsourctId":' + this.model.ProductOutsourctId + ',"Region":"' + this.model.Region + '","ProvinceId":"' + this.model.ProvinceId + '","CityId":"' + this.model.CityId + '","Channel":"' + this.model.Channel + '","Format":"' + this.model.Format + '"}';
            $.ajax({
                type: "post",
                url: "AssignConfig.ashx",
                data: { type: "edit", jsonStr: escape(jsonStr) },
                success: function (data) {
                    if (data == "ok") {
                        layer.closeAll();
                        $("#editDiv").hide();
                        Confing.getList();
                    }
                    else {
                        layer.msg(data);
                    }
                }
            })
        }
    }
};

function CheckVal() {
    var customerId = $("#seleCustomer").val();
    var typeId = $("#seleConfigType").val();
    var materialName = $.trim($("#txtMaterialName").val());
    var outsourceId = $("#seleOutsource").val();
    //var channel = $.trim($("#txtChannel").val());
    //var format = $.trim($("#txtFormat").val());
    var region = $("input[name='rdRegion']:checked").val() || "";

    var channel = "";
    $("input[name='cbChannel']:checked").each(function () {
        channel += $(this).val() + ",";
    })
    var format = "";
    $("input[name='cbFormat']:checked").each(function () {
        format += $(this).val() + ",";
    })

    var proviceId = "";
    $("input[name='cbProvince']:checked").each(function () {
        proviceId += $(this).val() + ",";
    })
    var cityId = "";
    $("input[name='cbCity']:checked").each(function () {
        var cid = $(this).val();
        var pid = $(this).data("provinceid");
        cityId += pid + ":" + cid + ",";
    })
    if (typeId == "0") {
        layer.msg("请选择类型");
        return false;
    }
    if (typeId == 1) {
        if (materialName == "") {
            layer.msg("请填写材质名称");
            return false;
        }
        if (outsourceId == "-1") {
            layer.msg("请选择生产外协");
            return false;
        }
    }
    if (region == "") {
        layer.msg("请选择区域");
        return false;
    }
    //if (proviceId == "") {
        //layer.msg("请选择省份");
       // return false;
   // }
    Confing.model.CustomerId = customerId;
    Confing.model.ConfigTypeId = typeId;
    Confing.model.MaterialName = materialName;
    Confing.model.ProductOutsourctId = outsourceId;
    Confing.model.Region = region;
    Confing.model.ProvinceId = proviceId;
    Confing.model.CityId = cityId;
    Confing.model.Channel = channel;
    Confing.model.Format = format;
   
    return true;
}

function ClearVal() {
    
    $("#seleConfigType").val("0");
    $("#seleOutsource").val("-1")
    $("#editDiv input").val("");
    $("#regionContainer,#provinceContainer,#cityContainer").html("");
    $("#txtMaterialName").attr("disabled", false);
    $("#seleOutsource").val("-1").attr("disabled", false);

    currRegion = 0;
    currProvinceId = "";
    currCityId = "";
    Confing.model.Id = 0;
    Confing.model.CustomerId = 0;
    Confing.model.ConfigTypeId = 0;
    Confing.model.MaterialName = "";
    Confing.model.ProductOutsourctId = -1;
    Confing.model.Region = 0;
    Confing.model.ProvinceId = "";
    Confing.model.CityId = "";
    Confing.model.Channel = "";
    Confing.model.Format = "";
}