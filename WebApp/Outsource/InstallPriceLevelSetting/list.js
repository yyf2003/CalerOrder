

var currRegionName = "";
var currRegionId = 0;
var settingRowSelectedRow;
$(function () {
    setting.getRegion();
    setting.getSettingList();

    $("#btnAdd").click(function () {
        setting.bindRegion();
        layer.open({
            type: 1,
            title: '添加设置',
            skin: 'layui-layer-rim', //加上边框
            area: ['900px', '500px'],
            content: $("#editDiv"),
            id: 'settingLayer',
            btn: ['提 交'],
            btnAlign: 'c',
            yes: function () {
                setting.submit();
            },
            cancel: function () {
                $("#editDiv").hide();
                layer.closeAll();
            }
        });
    });

    $("#selRegion").change(function () {
        setting.bindProvice();
    })

    $("#cbAllProvince").change(function () {
        var checked = this.checked;
        $("input[name='cbProvince']").each(function () {
            this.checked = checked;
        });
        setting.bindCity();
    })

    $("#provinceDiv").delegate("input[name='cbProvince']", "change", function () {
        var totalLen = $("input[name='cbProvince']").length;
        var checkedLen = $("input[name='cbProvince']:checked").length;
        if (checkedLen < totalLen) {
            $("#cbAllProvince").prop("checked", false);
        }
        else if (totalLen > 0 && totalLen == checkedLen) {
            $("#cbAllProvince").prop("checked", true);
        }
        setting.bindCity();
    })

    $("input[name='rdSettingType']").change(function () {
        ChangeSettingType();
    })

    $("#citySettingData, #provinceSettingData").delegate("input", "blur", function () {
        var val = $(this).val();
        if (isNaN(val)) {
            $(this).val("");
        }
    })

    layui.use("table", function () {
        var table = layui.table;
        table.on('checkbox(tbSetList)', function (obj) {
            settingRowSelectedRow = null;
            var checkRow = table.checkStatus('tbSetList');
            settingRowSelectedRow = checkRow.data;
        });
        //监听编辑
        table.on('edit(tbSetList)', function (obj) {
            var editData = obj.data;
            var customerId = editData.CustomerId;
            var provinceId = editData.ProvinceId;
            var cityId = editData.CityId;
            var filed = obj.field;
            var val = obj.value||0;
            var materialSupport = filed.replace("Price", "");
            var jsonStr = '{"CustomerId":' + customerId + ',"ProvinceId":' + provinceId + ',"CityId":' + cityId + ',"MaterialSupport":"' + materialSupport + '","BasicInstallPrice":'+val+'}';
            if (jsonStr != "") {
               
                $.ajax({
                    type: 'post',
                    url: 'Handler1.ashx',
                    data: { type: 'edit', jsonStr: jsonStr },
                    success: function (data) {
                        if (data != "ok") {
                            layer.alert(data);
                        }
                    }
                })
            }
        })
    })

    $("#btnEdit").click(function () {
        var txt = $("#editSpan").html();
        if (txt == "编辑设置") {
            $(this).attr("class", "layui-btn layui-btn-danger layui-btn-sm");
            $("#editSpan").html("取消编辑");
            setting.getSettingListOnEdit();
        }
        else {
            $(this).attr("class", "layui-btn layui-btn-sm");
            $("#editSpan").html("编辑设置");
            setting.getSettingList();
        }
    })

    $("#btnDelete").click(function () {
        setting.deleteSetting();
    })


})



var setting = {
    getRegion: function () {
        $("#tbOutsourceRegion").datagrid({
            method: 'get',
            url: 'Handler1.ashx?type=getRegion',
            columns: [[
                    { field: 'RegionId', hidden: true },
                    { field: 'RegionName', title: '区域名称' }
                ]],
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onClickRow: function (rowIndex, data) {
                selectOutsourceRegion();
            }
        });
    },
    bindRegion: function () {
        document.getElementById("selRegion").length = 1;
        $("#provinceDiv").html("");
        $.ajax({
            type: 'get',
            url: 'Handler1.ashx',
            data: { type: 'getRegion' },
            success: function (data) {
                if (data != '') {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currRegionId > 0 && currRegionId == json[i].RegionId) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = '<option value="' + json[i].RegionId + '" ' + selected + '>' + json[i].RegionName + '</option>';
                        $("#selRegion").append(option);
                    }
                    if (isSelected) {
                        setting.bindProvice();
                    }
                }
            }
        })
    },
    bindProvice: function () {
        var rid = $("#selRegion").val();
        $("#citySettingData").html("");
        $.ajax({
            type: 'get',
            url: '/Customer/Handler/Shops.ashx',
            data: { type: 'bindProvince', regionId: rid },
            success: function (data) {
                if (data != '') {
                    var json = eval(data);
                    var div = "";
                    for (var i = 0; i < json.length; i++) {
                        div += '<div style="float:left;width:120px;"><input type="checkbox" name="cbProvince" value="' + json[i].Id + '"/>&nbsp;' + json[i].ProvinceName + '</div>';
                    }
                    $("#provinceDiv").html(div);
                }
            }
        })
    },
    bindCity: function () {
        $("#citySettingData").html("");
        var provinceId = "";
        $("input[name='cbProvince']:checked").each(function () {
            provinceId += $(this).val() + ',';
        })

        $.ajax({
            type: 'get',
            url: 'Handler1.ashx',
            data: { type: 'getCity', provinceId: provinceId },
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var tr = "";
                    for (var i = 0; i < json.length; i++) {
                        tr += "<tr class='tr_bai'>";
                        tr += "<td><span name='spanCityDate' data-parentid='" + json[i].ParentId + "' data-cityid='" + json[i].PlaceId + "'>" + json[i].PlaceName + "</span></td>";
                        tr += "<td><input type='text' name='txtCBasicPrice' maxlength='5' style='width:60px; text-align:center;'></td>";
                        tr += "<td><input type='text' name='txtCPremiumPrice' maxlength='5' style='width:60px; text-align:center;'></td>";
                        tr += "<td><input type='text' name='txtCVVIPPrice' maxlength='5' style='width:60px; text-align:center;'></td>";
                        tr += "<td><input type='text' name='txtCMCSPrice' maxlength='5' style='width:60px; text-align:center;'></td>";
                        tr += "<td><input type='text' name='txtCGenericPrice' maxlength='5' style='width:60px; text-align:center;'></td>";
                        tr += "<td><input type='text' name='txtCOthersPrice' maxlength='5' style='width:60px; text-align:center;'></td>";

                        tr += "</tr>";
                    }
                    $("#citySettingData").html(tr);
                }
            },
            complete: function () {
                ChangeSettingType();
            }
        })
    },
    getSettingList: function () {
        layui.use('table', function () {
            var table = layui.table;
            table.render({
                elem: '#tbSetList',
                url: 'Handler1.ashx',
                where: {
                    'type': 'getSettingList',
                    'regionId': currRegionId
                },
                method: 'get',
                cols: [[
                    { type: 'checkbox', fixed: 'left' },
                    { field: 'RowIndex', width: 60, title: '序号', style: 'color: #000;' },
                    { field: 'CustomerName', width: 100, title: '客户', style: 'color: #000;' },
                    { field: 'RegionName', width: 80, title: '区域', style: 'color: #000;' },
                    { field: 'ProvinceName', width: 100, title: '省份', style: 'color: #000;' },
                    { field: 'CityName', minWidth: 90, title: '城市', style: 'color: #000;' },
                    { field: 'BasicPrice', width: 80, title: 'Basic', style: 'color: #000;' },
                    { field: 'PremiumPrice', width: 90, title: 'Premium', style: 'color: #000;' },
                    { field: 'VVIPPrice', width: 80, title: 'VVIP', style: 'color: #000;' },
                    { field: 'MCSPrice', width: 80, title: 'MCS', style: 'color: #000;' },
                    { field: 'GenericPrice', width: 80, title: 'Generic', style: 'color: #000;' },
                    { field: 'OthersPrice', width: 80, title: 'Others', style: 'color: #000;' }

                ]],
                page: false
            })
        })
    },
    getSettingListOnEdit: function () {
        layui.use('table', function () {
            var table = layui.table;
            table.render({
                elem: '#tbSetList',
                url: 'Handler1.ashx',
                where: {
                    'type': 'getSettingList',
                    'regionId': currRegionId
                },
                method: 'get',
                cols: [[
                    { type: 'checkbox', fixed: 'left' },
                    { field: 'RowIndex', width: 60, title: '序号', style: 'color: #000;' },
                    { field: 'CustomerName', width: 100, title: '客户', style: 'color: #000;' },
                    { field: 'RegionName', width: 80, title: '区域', style: 'color: #000;' },
                    { field: 'ProvinceName', width: 100, title: '省份', style: 'color: #000;' },
                    { field: 'CityName', minWidth: 90, title: '城市', style: 'color: #000;' },
                    { field: 'BasicPrice', width: 80, title: 'Basic', edit: 'text', style: 'color: #000;' },
                    { field: 'PremiumPrice', width: 90, title: 'Premium', edit: 'text', style: 'color: #000;' },
                    { field: 'VVIPPrice', width: 80, title: 'VVIP', edit: 'text', style: 'color: #000;' },
                    { field: 'MCSPrice', width: 80, title: 'MCS', edit: 'text', style: 'color: #000;' },
                    { field: 'GenericPrice', width: 80, title: 'Generic', edit: 'text', style: 'color: #000;' },
                    { field: 'OthersPrice', width: 80, title: 'Others', edit: 'text', style: 'color: #000;' }

                ]],
                page: false
            })
        })
    },
    submit: function () {
        if (CheckVal()) {
            $.ajax({
                type: 'post',
                url: 'Handler1.ashx',
                data: { type: 'add', settingType: settingType, jsonStr: jsonStr },
                success: function (data) {
                    if (data == "ok") {
                        $("#editDiv").hide();
                        layer.closeAll();
                        setting.getSettingList();
                    }
                    else {
                        layer.alert(data);
                    }
                }
            })
        }
    },
    deleteSetting: function () {
        if (settingRowSelectedRow && settingRowSelectedRow.length > 0) {
            var jsonStr = "";
            var index2 = layer.confirm('确定删除吗？', {
                btn: ['确定', '取消'] //按钮
            }, function () {
                layer.close(index2);
                var Loadingindex = layer.load(1, { shade: [0.5, '#fff'] }); //0代表加载的风格，支持0-2
                setTimeout(function () {
                    for (var i = 0; i < settingRowSelectedRow.length; i++) {
                        var customerId = settingRowSelectedRow[i].CustomerId;
                        var provinceId = settingRowSelectedRow[i].ProvinceId;
                        var cityId = settingRowSelectedRow[i].CityId;
                        jsonStr += '{"CustomerId":' + customerId + ',"ProvinceId":' + provinceId + ',"CityId":' + cityId + '},';
                    }
                    
                    if (jsonStr != "") {
                        jsonStr = "[" + jsonStr.substring(0, jsonStr.length - 1) + "]";
                        
                        $.ajax({
                            type: 'post',
                            url: 'Handler1.ashx',
                            data: { type: 'delete', jsonStr: jsonStr },
                            success: function (data) {
                                if (data == "ok") {
                                    layer.msg("删除成功！");
                                    setting.getSettingList();
                                }
                                else {
                                    layer.alert(data);
                                }
                            },
                            complete: function () {
                                layer.close(Loadingindex);
                            }
                        })
                    }
                }, 1000);
            }, function () {
                layer.close(index2);
            })

        }
        else {
            layer.msg("请选择要删除的记录");
            return false;
        }

    }
}

function ChangeSettingType() {
    var settingType = $("input[name='rdSettingType']:checked").val() || 1;
    if (settingType == 1) {
        $("#citySettingData input").each(function () {
            $(this).val("").attr("disabled", "disabled");
        });
        $("#provinceSettingData input").each(function () {
            $(this).attr("disabled", false);
        });
    }
    else {
        $("#provinceSettingData input").each(function () {
            $(this).val("").attr("disabled", "disabled");
        });
        $("#citySettingData input").each(function () {
            $(this).attr("disabled", false);
        })
    }
}

function selectOutsourceRegion() {
    var row = $("#tbOutsourceRegion").datagrid("getSelected");
    currRegionName = "";
    currRegionId = 0;
    if (row) {
        currRegionName = row.RegionName;
        currRegionId = row.RegionId;
    }

    $("#orderTitle").panel({
        title: ">>区域：<span style='color:blue;'>" + currRegionName + "</span>"
    });
    setting.getSettingList();
}

var settingType = 1;
var jsonStr = "";
function CheckVal() {
    settingType = 1;
    jsonStr = "";
    var customerid = $("#ddlCustomer").val() || 0;
    if (customerid == 0) {
        layer.alert("请选择客户");
        return false;
    }
    var regionid = $("#selRegion").val();
    settingType = $("input[name='rdSettingType']:checked").val() || 1;

    if (settingType == 1) {
        //var proviceId = "";
        var proviceIdArr = [];
        $("input[name='cbProvince']:checked").each(function () {
            //proviceId += $(this).val() + ",";
            proviceIdArr.push($(this).val());
        })
        if (proviceIdArr.length == 0) {
            layer.alert("请选择省份");
            return false;
        }
        var basicPrice = $.trim($("#txtPBasicPrice").val());
        var premiumPrice = $.trim($("#txtPPremiumPrice").val());
        var VVIPPrice = $.trim($("#txtPVVIPPrice").val());
        var MCSPrice = $.trim($("#txtPMCSPrice").val());
        var genericPrice = $.trim($("#txtPGenericPrice").val());
        var othersPrice = $.trim($("#txtPOthersPrice").val());
        if (basicPrice == "") {
            layer.alert("请填写Basic级别的金额");
            return false;
        }
        if (premiumPrice == "") {
            layer.alert("请填写Premium级别的金额");
            return false;
        }
        if (VVIPPrice == "") {
            layer.alert("请填写VVIP级别的金额");
            return false;
        }
        if (MCSPrice == "") {
            layer.alert("请填写MCS级别的金额");
            return false;
        }
        if (genericPrice == "") {
            layer.alert("请填写Generic级别的金额");
            return false;
        }
        if (othersPrice == "") {
            layer.alert("请填写Others级别的金额");
            return false;
        }
        //jsonStr = '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"Province":"' + proviceId + '","BasicPrice":' + basicPrice + ',"PremiumPrice":' + premiumPrice + ',"VVIPPrice":' + VVIPPrice + ',"MCSPrice":' + MCSPrice + ',"OthersPrice":' + othersPrice + ',"GenericPrice":' + genericPrice + '}';
        //proviceId = proviceId.substring(0, proviceId.length - 1);
        $.each(proviceIdArr, function (key, val) {
            jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + val + ',"MaterialSupport":"Basic","BasicInstallPrice":' + basicPrice + '},';
            jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + val + ',"MaterialSupport":"Premium","BasicInstallPrice":' + premiumPrice + '},';
            jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + val + ',"MaterialSupport":"VVIP","BasicInstallPrice":' + VVIPPrice + '},';
            jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + val + ',"MaterialSupport":"MCS","BasicInstallPrice":' + MCSPrice + '},';
            jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + val + ',"MaterialSupport":"Generic","BasicInstallPrice":' + genericPrice + '},';
            jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + val + ',"MaterialSupport":"Others","BasicInstallPrice":' + othersPrice + '},';
        });
        jsonStr = "[" + jsonStr.substring(0, jsonStr.length - 1) + "]";

    }
    else {
        var trLen = $("#citySettingData tr").length;
        if (trLen > 0) {
            $("#citySettingData tr").each(function () {
                var cityTdSpan = $(this).find("span[name='spanCityDate']");
                var provinceId = $(cityTdSpan).data("parentid") || 0;
                var cityId = $(cityTdSpan).data("cityid") || 0;
                var basicPrice = $.trim($(this).find("input[name='txtCBasicPrice']").val()) || 0;
                var premiumPrice = $.trim($(this).find("input[name='txtCPremiumPrice']").val()) || 0;
                var VVIPPrice = $.trim($(this).find("input[name='txtCVVIPPrice']").val()) || 0;
                var MCSPrice = $.trim($(this).find("input[name='txtCMCSPrice']").val()) || 0;
                var genericPrice = $.trim($(this).find("input[name='txtCGenericPrice']").val()) || 0;
                var othersPrice = $.trim($(this).find("input[name='txtCOthersPrice']").val()) || 0;
                if (basicPrice > 0 && premiumPrice > 0 && VVIPPrice > 0 && MCSPrice > 0 && genericPrice > 0 && othersPrice > 0) {
                    jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + provinceId + ',"CityId":' + cityId + ',"MaterialSupport":"Basic","BasicInstallPrice":' + basicPrice + '},';
                    jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + provinceId + ',"CityId":' + cityId + ',"MaterialSupport":"Premium","BasicInstallPrice":' + premiumPrice + '},';
                    jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + provinceId + ',"CityId":' + cityId + ',"MaterialSupport":"VVIP","BasicInstallPrice":' + VVIPPrice + '},';
                    jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + provinceId + ',"CityId":' + cityId + ',"MaterialSupport":"MCS","BasicInstallPrice":' + MCSPrice + '},';
                    jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + provinceId + ',"CityId":' + cityId + ',"MaterialSupport":"Generic","BasicInstallPrice":' + genericPrice + '},';
                    jsonStr += '{"CustomerId":' + customerid + ',"RegionId":' + regionid + ',"ProvinceId":' + provinceId + ',"CityId":' + cityId + ',"MaterialSupport":"Others","BasicInstallPrice":' + othersPrice + '},';
                }
            });
            jsonStr = "[" + jsonStr.substring(0, jsonStr.length - 1) + "]";
        }
        else {
            layer.alert("没有可以提交的数据");
        }
    }

    return true;
}