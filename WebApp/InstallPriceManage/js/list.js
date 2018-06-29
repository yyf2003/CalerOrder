
var i_pageIndex = 1;
var i_pageSize = 15;
var e_pageIndex = 1;
var e_pageSize = 15;
var activeTabIndex = 0;
var firstLoad = 1;
var currInstallSubjectId = 0;
var currInstallGuidanceId = 0;
$(function () {
    GetGuidacneList();
    GetRegion();
    shopInstall.getInstallShopList(i_pageIndex, i_pageSize);
    $("#ddlCustomer").change(function () {
        GetGuidacneList();
        GetRegion();
    })
    //查询上个月活动
    $("#spanUp").click(function () {

        var month1 = $.trim($("#txtMonth").val());
        if (month1 != "") {
            month1 = month1.replace(/-/g, "/");
            var date = new Date(month1);
            date.setMonth(date.getMonth() - 1);
            $("#txtMonth").val(date.Format("yyyy-MM"));
            GetGuidacneList();
        }

    })

    //查询下个月活动
    $("#spanDown").click(function () {
        var month1 = $.trim($("#txtMonth").val());
        if (month1 != "") {
            month1 = month1.replace(/-/g, "/");
            var date = new Date(month1);
            date.setMonth(date.getMonth() + 1);
            $("#txtMonth").val(date.Format("yyyy-MM"));
            GetGuidacneList();
        }
    })

    $("#guidanceCBALL").click(function () {
        var checked = this.checked;
        $("input[name='guidanceCB']").each(function () {
            this.checked = checked;
        })

    })

    $("#guidanceDiv").delegate("input[name='guidanceCB']", "change", function () {
        if (!this.checked) {
            $("#guidanceCBALL").prop("checked", false);
        }
        else {
            var checked = $("input[name='guidanceCB']:checked").length == $("input[name='guidanceCB']").length;
            $("#guidanceCBALL").prop("checked", checked);
        }
    })

    //查询
    $("#btnSearch").click(function () {
        var guidanceId = "";
        $("input[name$='guidanceCB']:checked").each(function () {
            guidanceId += $(this).val() + ",";
        });
        if (guidanceId == "") {
            layer.msg("请选择活动");
            return false;
        }
        else {
            if (activeTabIndex == 0)
                shopInstall.getInstallShopList(i_pageIndex, i_pageSize);
            else
                shopInstall.getExpressShopList(e_pageIndex, e_pageSize);
        }
    })

    $("#txtISearchShopNo").searchbox({
        width: 500,
        searcher: doISearch,
        prompt: '请输入店铺编号查询'
    })

    $("#txtESearchShopNo").searchbox({
        width: 500,
        searcher: doESearch,
        prompt: '请输入店铺编号查询'
    });

    //编辑安装费
    $("#btnIEdit").click(function () {
        ClearInstallVal();
        var rows = $("#tbInstallShopList").datagrid('getSelections');
        if (rows == null || rows.length == 0) {
            layer.msg("请选择要编辑的安装费");
            return false;
        }
        else if (rows.length > 1) {
            layer.msg("一次只能编辑一行");
            return false;
        }
        else {
            var row = rows[0];
            shopInstall.model.Id = row.Id;
            currInstallSubjectId = row.SubjectId;
            currInstallGuidanceId = row.GuidanceId;
            shopInstall.getInstallSubjectList();
            $("#labShopNo").text(row.ShopNo);
            $("#txtBasicPrice").val(row.BasicInstallPrice);
            $("#txtWindowPrice").val(row.WindowInstallPrice);
            $("#txtOOHPrice").val(row.OOHInstallPrice);
            $("#txtPayPrice").val(row.PayPrice);
            $("#ddlMaterialSupport").val(row.MaterialSupport);
            $("#txtRemark").val(row.Remark);
            $("#editInstallDiv").show().dialog({
                modal: true,
                width: 720,
                height: 400,
                iconCls: 'icon-edit',
                resizable: false,
                buttons: [
                        {
                            text: '确定',
                            iconCls: 'icon-ok',
                            handler: function () {
                                shopInstall.submitInstallPrice();

                            }
                        },
                        {
                            text: '取消',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                $("#editInstallDiv").dialog('close');
                            }
                        }
                    ]
            });
        }
    });

    $("#btnEEdit").click(function () {
        $("#txtExpressPrice").val("");
        $("#txtPayExpressPrice").val("");
        var rows = $("#tbExpressShopList").datagrid('getSelections');
        if (rows == null || rows.length == 0) {
            layer.msg("请选择要编辑的快递费");
            return false;
        }
        else if (rows.length > 1) {
            layer.msg("一次只能编辑一行");
            return false;
        }
        else {
            var id = rows[0].Id;
            $("#labShopNo1").text(rows[0].ShopNo);
            $("#txtExpressPrice").val(rows[0].ReceivePrice);
            $("#txtPayExpressPrice").val(rows[0].PayPrice);
            $("#editExpressDiv").show().dialog({
                modal: true,
                width: 720,
                height: 300,
                iconCls: 'icon-edit',
                resizable: false,
                buttons: [
                        {
                            text: '确定',
                            iconCls: 'icon-ok',
                            handler: function () {
                                var expressPrice = $.trim($("#txtExpressPrice").val()) || 0;
                                var payPrice = $.trim($("#txtPayExpressPrice").val()) || 0;
                                if (expressPrice != "" && isNaN(expressPrice)) {
                                    layer.msg("应收快递费必须填数字");
                                    return false;
                                }
                                if (parseFloat(expressPrice) == 0) {
                                    layer.msg("请填写应收快递费");
                                    return false;
                                }
                                if (payPrice != "" && isNaN(payPrice)) {
                                    layer.msg("应付快递费必须填数字");
                                    return false;
                                }
                                var jsonStr = '{"Id":' + id + ',"ExpressPrice":' + expressPrice + ',"PayPrice":' + payPrice + '}';
                                alert(jsonStr);
                                $.ajax({
                                    type: "get",
                                    url: "handler/List.ashx",
                                    data: { type: "editExpressPrice", jsonStr: urlCodeStr(jsonStr) },
                                    success: function (data) {
                                        if (data == "ok") {
                                            $("#tbExpressShopList").datagrid("reload");
                                            $("#editExpressDiv").dialog('close');
                                        }
                                        else {
                                            layer.msg(data);
                                        }
                                    }
                                })
                            }
                        },
                        {
                            text: '取消',
                            iconCls: 'icon-cancel',
                            handler: function () {
                                $("#editExpressDiv").dialog('close');
                            }
                        }
                    ]
            });
        }
    });

    //删除安装费
    $("#btnIDelete").click(function () {
        var rows = $("#tbInstallShopList").datagrid('getSelections');
        if (rows == null || rows.length == 0) {
            layer.msg("请选择要删除的安装费");
        }
        else {
            var ids = "";
            for (var i = 0; i < rows.length; i++) {
                ids += rows[i].Id + ",";
            }
            $.ajax({
                type: "get",
                url: "handler/List.ashx?type=deleteInstallPrice&ids=" + ids,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbInstallShopList").datagrid("reload");
                    }
                    else {
                        layer.msg(data);
                    }
                }
            })
        }
    });

    //删除快递费
    $("#btnEDelete").click(function () {
        var rows = $("#tbExpressShopList").datagrid('getSelections');
        if (rows == null || rows.length == 0) {
            layer.msg("请选择要删除的快递费");
        }
        else {
            var ids = "";
            for (var i = 0; i < rows.length; i++) {
                ids += rows[i].Id + ",";
            }
            $.ajax({
                type: "get",
                url: "handler/List.ashx?type=deleteExpressPrice&ids=" + ids,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbExpressShopList").datagrid("reload");
                    }
                    else {
                        layer.msg(data);
                    }
                }
            })
        }
    });

    //恢复安装费
    $("#btnIRecover").click(function () {
        var rows = $("#tbInstallShopList").datagrid('getSelections');
        if (rows == null || rows.length == 0) {
            layer.msg("请选择要恢复的安装费");
        }
        else {
            var ids = "";
            for (var i = 0; i < rows.length; i++) {
                ids += rows[i].Id + ",";
            }
            $.ajax({
                type: "get",
                url: "handler/List.ashx?type=recoverInstallPrice&ids=" + ids,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbInstallShopList").datagrid("reload");
                    }
                    else {
                        layer.msg(data);
                    }
                }
            })
        }
    });

    //恢复快递费
    $("#btnERecover").click(function () {
        var rows = $("#tbExpressShopList").datagrid('getSelections');
        if (rows == null || rows.length == 0) {
            layer.msg("请选择要恢复的快递费");
        }
        else {
            var ids = "";
            for (var i = 0; i < rows.length; i++) {
                ids += rows[i].Id + ",";
            }
            $.ajax({
                type: "get",
                url: "handler/List.ashx?type=recoverExpressPrice&ids=" + ids,
                success: function (data) {
                    if (data == "ok") {
                        $("#tbExpressShopList").datagrid("reload");
                    }
                    else {
                        layer.msg(data);
                    }
                }
            })
        }
    });

    $("#guidanceDiv").delegate("input[name$='guidanceCB']", "change", function () {
        //$("#ProvinceDiv").html("");
        //$("#CityDiv").html("");
        GetProvince();
        GetCity();
    });

    $("#regionDiv").delegate("input[name$='regionCB']", "change", function () {
        GetProvince();
        GetCity();
        //$("#CityDiv").html("");
    });

    $("#ProvinceDiv").delegate("input[name$='provinceCB']", "change", function () {
        GetCity();
    });

    //layui事件监听
    layui.use('element', function () {
        var element = layui.element();

        //tab事件监听
        element.on('tab(order)', function (data) {
            activeTabIndex = data.index;
            if (activeTabIndex == 1 && firstLoad == 1) {
                firstLoad = 0;
                shopInstall.getExpressShopList(e_pageIndex, e_pageSize);
            }
        });
    });
})

function GetGuidacneList() {
    var month = $.trim($("#txtMonth").val());
    var customerId = $("#ddlCustomer").val();
    $.ajax({
        type: "get",
        url: "/Subjects/Handler/CheckOrder.ashx",
        data: { type: "getGuidance", guidanceMonth: month, customerId: customerId },
        success: function (data) {
            $("#guidanceDiv").html("");
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left;'><input type='checkbox' name='guidanceCB' value='" + json[i].Id + "' />" + json[i].GuidanceName + "&nbsp;</div>";
                    $("#guidanceDiv").append(div);
                }
            }
            else {
                $("#guidanceDiv").html("<span style='color:red;'>无活动信息！</span>");
            }
        }
    })
}

function GetRegion() {
    var customerId = $("#ddlCustomer").val();
    $("#regionDiv").html("");
    $.ajax({
        type: "get",
        url: "handler/List.ashx",
        data: { type: "getRegion", customerId: customerId },
        success: function (data) {
            
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left;'><input type='checkbox' name='regionCB' value='" + json[i].RegionName + "' />" + json[i].RegionName + "&nbsp;&nbsp;</div>";
                    $("#regionDiv").append(div);
                }
            }

        }
    })
}

function GetProvince() {
    var guidanceId = "";
    $("input[name$='guidanceCB']:checked").each(function () {
        guidanceId += $(this).val() + ",";
    });
    var region = "";
    $("input[name$='regionCB']:checked").each(function () {
        region += $(this).val() + ",";
    });
    $("#ProvinceDiv").html("");
    $.ajax({
        type: "get",
        url: "handler/List.ashx",
        data: { type: "getProvince", guidanceId: guidanceId, region: region },
        success: function (data) {
           
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var div = "<div style='float:left;'><input type='checkbox' name='provinceCB' value='" + json[i].ProvinceName + "' />" + json[i].ProvinceName + "&nbsp;&nbsp;</div>";
                    $("#ProvinceDiv").append(div);
                }
            }
        }

    })
}


function GetCity() {
    var guidanceId = "";
    $("input[name$='guidanceCB']:checked").each(function () {
        guidanceId += $(this).val() + ",";
    });
    var province = "";
    $("input[name$='provinceCB']:checked").each(function () {
        province += $(this).val() + ",";
    });
    $("#CityDiv").html("");
    $.ajax({
        type: "get",
        url: "handler/List.ashx",
        data: { type: "getCity", guidanceId: guidanceId, province: province },
        success: function (data) {
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var div = "<div style='float:left;'><input type='checkbox' name='cityCB' value='" + json[i].CityName + "' />" + json[i].CityName + "&nbsp;&nbsp;</div>";
                    $("#CityDiv").append(div);
                }
            }
        }

    })
}

function getMonth() {
    GetGuidacneList();
}

var shopInstall = {
    model: function () {
        this.Id = 0;
        this.GuidanceId = 0;
        this.ShopId = 0;
        this.SubjectId = 0;
        this.BasicInstallPrice = 0;
        this.WindowInstallPrice = 0;
        this.OOHInstallPrice = 0;
        this.PayPrice = 0;
        this.MaterialSupport = "";
        this.Remark = "";
    },
    getInstallShopList: function (pageIndex, pageSize) {
        var guidanceId = "";
        $("input[name$='guidanceCB']:checked").each(function () {
            guidanceId += $(this).val() + ",";
        });

        var region = "";
        var shopNo = $.trim($("#txtISearchShopNo").val());
        $("#tbInstallShopList").datagrid({
            queryParams: { type: "getInstallShopList", guidanceId: guidanceId, region: region, shopNo: shopNo, currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: "handler/List.ashx",
            columns: [[
                        { field: 'Id', title: 'id', hidden: true },
                        { field: 'checked', checkbox: true },
                        { field: 'rowIndex', title: '序号' },
                        { field: 'GuidanceName', title: '活动名称' },
                        { field: 'ShopNo', title: '店铺编号' },
                        { field: 'ShopName', title: '店铺名称' },
                        { field: 'Region', title: '区域' },
                        { field: 'Province', title: '省份' },
                        { field: 'MaterialSupport', title: '物料级别' },
                        { field: 'BasicInstallPrice', title: '店内安装费' },
                        { field: 'WindowInstallPrice', title: '橱窗安装费' },
                        { field: 'OOHInstallPrice', title: '高空(OOH)安装费' },
                        { field: 'TotalPrice', title: '应收合计' },
                        { field: 'PayPrice', title: '应付合计' },
                        { field: 'Remark', title: '备注' },
                        { field: 'IsDelete', title: '状态', formatter: function (value, row) {
                            if (value == "1") {
                                return "<span style='color:red;'>已删除</span>";
                            }
                            else {
                                return "<span>正常</span>";
                            }
                        }
                        }

            ]],
            height: "100%",
            toolbar: "#iToolbar",
            pageList: [15, 20],
            striped: true,
            border: false,
            singleSelect: false,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onLoadSuccess: function (data) {

            },
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }
        });
        var p = $("#tbInstallShopList").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                //GetSearchData();
                shopInstall.getShopList(curIndex, curSize);
            }
        });
    },
    getExpressShopList: function (pageIndex, pageSize) {
        var guidanceId = "";
        $("input[name$='guidanceCB']:checked").each(function () {
            guidanceId += $(this).val() + ",";
        });

        var region = "";
        var shopNo = $.trim($("#txtESearchShopNo").val());

        $("#tbExpressShopList").datagrid({
            queryParams: { type: "getExpressShopList", guidanceId: guidanceId, region: region, shopNo: shopNo, currpage: pageIndex, pagesize: pageSize },
            method: 'get',
            url: "handler/List.ashx",
            columns: [[
                        { field: 'checked', checkbox: true },
                        { field: 'rowIndex', title: '序号' },
                        { field: 'GuidanceName', title: '活动名称' },
                        { field: 'ShopNo', title: '店铺编号' },
                        { field: 'ShopName', title: '店铺名称' },
                        { field: 'Region', title: '区域' },
                        { field: 'Province', title: '省份' },
                        { field: 'ReceivePrice', title: '应收安装费' },
                        { field: 'PayPrice', title: '应付安装费' },
                        { field: 'IsDelete', title: '状态', formatter: function (value, row) {
                            if (value == "1") {
                                return "<span style='color:red;'>已删除</span>";
                            }
                            else {
                                return "<span>正常</span>";
                            }
                        }
                        }

            ]],
            height: "100%",
            toolbar: "#eToolbar",
            pageList: [15, 20],
            striped: true,
            border: false,
            singleSelect: false,
            pagination: true,
            pageNumber: pageIndex,
            pageSize: pageSize,
            fitColumns: true,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onLoadSuccess: function (data) {

            },
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }
        });
        var p = $("#tbExpressShopList").datagrid('getPager');
        $(p).pagination({
            showRefresh: false,
            onSelectPage: function (curIndex, curSize) {
                //GetSearchData();
                shopInstall.getExpressShopList(curIndex, curSize);
            }
        });
    },
    getInstallSubjectList: function () {
        document.getElementById("ddlSubjectList").length = 1;
        $.ajax({
            type: "get",
            url: "handler/List.ashx",
            data: { type: "getInstallSubject", guidanceId: currInstallGuidanceId },
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currInstallSubjectId == json[i].Id) {
                            selected = "selected='selected'";
                        }
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].SubjectName + "</option>";
                        $("#ddlSubjectList").append(option);
                    }
                }
            }

        })
    },
    submitInstallPrice: function () {

        if (CheckInstallVal()) {

            var jsonStr = '{"Id":' + this.model.Id + ',"SubjectId":' + this.model.SubjectId + ',"BasicPrice":' + this.model.BasicInstallPrice + ',"WindowPrice":' + this.model.WindowInstallPrice + ',"OOHPrice":' + this.model.OOHInstallPrice + ',"PayPrice":' + this.model.PayPrice + ',"MaterialSupport":"' + this.model.MaterialSupport + '","Remark":"' + this.model.Remark + '"}';

            $.ajax({
                type: "get",
                url: "handler/List.ashx",
                data: { type: "editInstallPrice", jsonStr: urlCodeStr(jsonStr) },
                success: function (data) {
                    if (data == "ok") {
                        $("#tbInstallShopList").datagrid("reload");
                        $("#editInstallDiv").dialog('close');
                    }
                    else {
                        layer.msg(data);
                    }
                }
            })
        }
    }
    
};

function doISearch() {
    var guidanceId = "";
    $("input[name$='guidanceCB']:checked").each(function () {
        guidanceId += $(this).val() + ",";
    });
    if (guidanceId == "") {
        layer.msg("请选择活动");
        return false;
    }
    else
        shopInstall.getInstallShopList(i_pageIndex, i_pageSize);
}

function doESearch() {
    var guidanceId = "";
    $("input[name$='guidanceCB']:checked").each(function () {
        guidanceId += $(this).val() + ",";
    });
    if (guidanceId == "") {
        layer.msg("请选择活动");
        return false;
    }
    else
        shopInstall.getExpressShopList(e_pageIndex, e_pageSize);
}

function CheckInstallVal() {
    var subjectId = $("#ddlSubjectList").val() || 0;
    var basicPrice = $.trim($("#txtBasicPrice").val()) || 0;
    var winPrice = $.trim($("#txtWindowPrice").val()) || 0;
    var oohPrice = $.trim($("#txtOOHPrice").val()) || 0;
    var payPrice = $.trim($("#txtPayPrice").val()) || 0;
    var materialSupport = $("#ddlMaterialSupport").val();
    var remark = $.trim($("#txtRemark").val());
    if (subjectId == 0) {
        layer.msg("请选择安装费项目");
        return false;
    }
    if ((basicPrice != "" && isNaN(basicPrice)) || (winPrice != "" && isNaN(winPrice)) || (oohPrice != "" && isNaN(oohPrice)) || (payPrice != "" && isNaN(payPrice))) {
        layer.msg("安装费必选填写数字");
        return false;
    }
    if ((parseFloat(basicPrice) + parseFloat(winPrice) + parseFloat(oohPrice)) == 0) {
        layer.msg("请填写应收安装费");
        return false;
    }
    if (materialSupport == "") {
        layer.msg("请选择物料级别");
        return false;
    }
    shopInstall.model.SubjectId = subjectId;
    shopInstall.model.BasicInstallPrice = basicPrice;
    shopInstall.model.WindowInstallPrice = winPrice;
    shopInstall.model.OOHInstallPrice = oohPrice;
    shopInstall.model.PayPrice = payPrice;
    shopInstall.model.MaterialSupport = materialSupport;
    shopInstall.model.Remark = remark;
    return true;
}

function ClearInstallVal() {
    document.getElementById("ddlSubjectList").length = 1;
    $("#txtBasicPrice").val("");
    $("#txtWindowPrice").val("");
    $("#txtOOHPrice").val("");
    $("#txtPayPrice").val("");

    $("#ddlMaterialSupport").val("");
    $("#txtRemark").val("");
    currInstallSubjectId = 0;
    currInstallGuidanceId = 0;
    shopInstall.model.Id = 0;
    shopInstall.model.GuidanceId = 0;
    shopInstall.model.ShopId = 0;
    shopInstall.model.SubjectId = 0;
    shopInstall.model.BasicInstallPrice = 0;
    shopInstall.model.WindowInstallPrice = 0;
    shopInstall.model.OOHInstallPrice = 0;
    shopInstall.model.PayPrice = 0;
    shopInstall.model.MaterialSupport = "";
    shopInstall.model.Remark = "";
}


