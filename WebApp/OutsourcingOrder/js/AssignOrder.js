

var companyId = "";
var guidanceId = "";
var subjectId = "";
var region = "";
var province = "";
var city = "";
var customerServiceId = "";
var cityTier = "";
var isInstall = "";
var selectedArr = [];
var isSearch = 0;
var assignState = "";
var shopNo = "";
var ruanmo = "";
var materialAssign = "";
var materialPlan = "";
var otherMaterial = "";
var channel = "";
var format = "";
var sheet = "";
var noInstallPrice = 0;
$(function () {
    $("input[name^='cblMaterialPlan']").each(function () {
        $(this).attr("checked", false).attr("disabled", "disabled");
    })

    GetGuidance1();
    $("#ddlCustomer").change(function () {

        GetGuidance1();
    })

    $("#txtSearchShopNo").searchbox({
        width: 300,
        searcher: doSearch,
        prompt: '请输入店铺编号,可以输入多个,逗号分隔'
    })

    //查询
    $("#btnSearch").click(function () {
        isSearch = 1;
        GetShopList(currPage, pageSize);
        var val = $("#seleCompany option:last").val();
        if (val == 0) {
            GetOutsourcing();
        }
        $("#seleInstallCompany").val("0");
    })

    //项目全选
    $("#cbALL").on("click", function () {
        var check = this.checked;
        $("input[name='projectCB']").each(function () {
            this.checked = check;
        })
        GetRegions();
        //GetCustomerService();
        //GetCityTier();
        //GetIsInstall();
        //GetChannel();
        //GetFormat();

    })

    //明细全选
    $("#cbAll1").on("click", function () {

        var check = this.checked;
        $("input[name='cbOne']").each(function () {
            this.checked = check;
        })
        getSelected();
    })

    $("#listBody").delegate("input[name='cbOne']", "click", function () {
        getSelected();
    })


    $("#btnSearchSubject").on("click", function () {

        GetGuidance();
    })
    $("#guidanceDiv").delegate("input[name='guidancerd']", "click", function () {

        GetProjectList();

    })

    $("#activityDiv").delegate("input[name='activityCB']", "change", function () {
        ScreenProject();
    })
    $("#typeDiv").delegate("input[name='typeCB']", "change", function () {
        ScreenProject();
    })

    $("#projectsDiv").delegate("input[name='projectCB']", "change", function () {
        GetRegions();
        GetProvince();
        GetCity();
        GetCustomerService();
        GetCityTier();
        GetIsInstall();
        GetChannel();
        GetFormat();
        GetSheet();
    })
    $("#RegionDiv").delegate("input[name='regioncb']", "change", function () {
        GetProvince();
        GetCity();
        GetCustomerService();
        GetCityTier();
        GetIsInstall();
        GetChannel();
        GetFormat();
        GetSheet();
    })
    $("#ProvinceDiv").delegate("input[name='provincecb']", "change", function () {
        GetCity();
        GetCustomerService();
        GetCityTier();
        GetIsInstall();
        GetChannel();
        GetFormat();
        GetSheet();
    })

    $("#CityDiv").delegate("input[name='citycb']", "change", function () {
        GetCustomerService();
        GetCityTier();
        GetIsInstall();
        GetChannel();
        GetFormat();
        GetSheet();
    })
    $("#CustomerServiceNameDiv").delegate("input[name='cscb']", "change", function () {
        GetCityTier();
        GetIsInstall();
        GetChannel();
        GetFormat();
        GetSheet();
        //GetMaterial();
    })
    $("#cityTierDiv").delegate("input[name='cityTiercb']", "change", function () {

        GetIsInstall();
        GetChannel();
        GetFormat();
        GetSheet();
    })
    $("#ChannelDiv").delegate("input[name='channelcb']", "change", function () {
        GetFormat();
        GetSheet();
    })
    $("#FormatDiv").delegate("input[name='formatcb']", "change", function () {
        GetSheet();
    })
    $("#cbAllCity").change(function () {
        var checked = this.checked;
        $("input[name='citycb']").each(function () {
            this.checked = checked;
        })
        GetCustomerService();
        GetCityTier();
        GetIsInstall();
        GetChannel();
        GetFormat();
        GetSheet();
    })

    $("#cbAllSheet").change(function () {
        var checked = this.checked;
        $("input[name='sheetcb']").each(function () {
            this.checked = checked;
        })

    })

    //分配
    $("#btnSubmit").click(function () {
        if (selectedArr.length == 0) {
            alert("请选择店铺！");
            return false;
        }
        var companyId = $("#seleCompany").val();
        if (companyId == "0") {
            alert("请选择外协！");
            return false;
        }
        var installOutsourceId = $("#seleInstallCompany").val() || 0;

        var orderType = $("input[name='rblOrderType']:checked").val() || 0;
        if (orderType == 0) {
            alert("请选择订单类型！");
            return false;
        }
        $.ajax({
            type: "post",
            url: "./handler/AssignOrder.ashx",
            data: { type: "assign", guidanceId: guidanceId, subjectids: subjectId, regions: region, provinces: province, citys: city, customerServiceId: customerServiceId, cityTier: cityTier, isInstall: isInstall, shopIds: selectedArr.join(','), companyId: companyId, installOutsourceId: installOutsourceId, orderType: orderType, ruanmo: ruanmo, materialAssign: materialAssign, channel: channel, format: format, materialPlan: materialPlan, sheet: sheet, noInstallPrice: noInstallPrice },
            cache: false,
            beforeSend: function () { $("#imgSubmit1").show(); $("#btnSubmit").attr("disabled", "disabled"); }, //发送数据之前
            complete: function () { $("#imgSubmit1").hide(); $("#btnSubmit").attr("disabled", false); }, //接收数据完毕
            success: function (data) {
                $("#MsgTB").show();
                if (data != "") {
                    var state = data.split('|')[0];
                    var msg = data.split('|')[1];
                    if (state == "ok") {
                        var totalOrderCount = msg.split(',')[0];
                        var assignOrderCount = msg.split(',')[1];
                        var repeatOrderCount = msg.split(',')[2];
                        $("#AssignStateTB").show();
                        $("#spanTotalOrderCount").html(totalOrderCount);
                        $("#spanAssignOrderCount").html(assignOrderCount);
                        $("#spanRepeatOrderCount").html(repeatOrderCount);
                        $("#labState").text("分配成功");
                        selectedArr = [];
                        GetShopList(currPage, pageSize);

                    }
                    else {
                        //$("#MsgTB").show();
                        $("#labState").text("分配失败：");
                        $("#labTips").text(msg);
                        $("#AssignStateTB").hide();
                    }
                }

            }
        })
    })
    //分配全部
    $("#btnSubmitAll").click(function () {
        if (subjectId == "") {
            alert("请选择项目");
            return false;
        }
        var companyId = $("#seleCompany").val();
        if (companyId == "0") {
            alert("请选择外协！");
            return false;
        }
        var installOutsourceId = $("#seleInstallCompany").val() || 0;
        var orderType = $("input[name='rblOrderType']:checked").val() || 0;
        if (orderType == 0) {
            alert("请选择订单类型！");
            return false;
        }
        var len = $("#listBody tr").length;
        if (len == 0) {
            alert("没有店铺可以提交");
            return false;
        }
        var searchShopNo = $.trim($("#txtSearchShopNo").val());
        $.ajax({
            type: "post",
            url: "./handler/AssignOrder.ashx",
            data: { type: "assign", guidanceId: guidanceId, subjectids: subjectId, regions: region, provinces: province, citys: city, customerServiceId: customerServiceId, cityTier: cityTier, isInstall: isInstall, companyId: companyId, installOutsourceId: installOutsourceId, orderType: orderType, ruanmo: ruanmo, materialAssign: materialAssign, channel: channel, format: format, shopNo: searchShopNo, materialPlan: materialPlan, sheet: sheet, noInstallPrice: noInstallPrice },
            cache: false,
            beforeSend: function () { $("#imgSubmit2").show(); $("#btnSubmitAll").attr("disabled", "disabled"); }, //发送数据之前
            complete: function () { $("#imgSubmit2").hide(); $("#btnSubmitAll").attr("disabled", false); }, //接收数据完毕
            success: function (data) {

                $("#MsgTB").show();
                if (data != "") {
                    var state = data.split('|')[0];
                    var msg = data.split('|')[1];
                    if (state == "ok") {

                        $("#labState").text("分配成功");
                        var totalOrderCount = msg.split(',')[0];
                        var assignOrderCount = msg.split(',')[1];
                        var repeatOrderCount = msg.split(',')[2];
                        $("#AssignStateTB").show();
                        $("#spanTotalOrderCount").html(totalOrderCount);
                        $("#spanAssignOrderCount").html(assignOrderCount);
                        $("#spanRepeatOrderCount").html(repeatOrderCount);
                        selectedArr = [];
                        GetShopList(currPage, pageSize);

                    }
                    else {
                        $("#labState").text("分配失败：");
                        $("#labTips").text(msg);
                        $("#AssignStateTB").hide();
                    }
                }
            }
        })
    })

    //清空
    $("#btnClear").click(function () {
        GetCondition();
        if (selectedArr.length == 0) {
            alert("请选择店铺！");
            return false;
        }
        if (confirm("确定撤销吗？")) {
            $("#AssignStateTB").hide();
            $.ajax({
                type: "post",
                url: "./handler/AssignOrder.ashx",
                data: { type: "cancelAssign", guidanceId: guidanceId, subjectids: subjectId, regions: region, provinces: province, citys: city, customerServiceId: customerServiceId, cityTier: cityTier, isInstall: isInstall, shopIds: selectedArr.join(',') },
                cache: false,
                beforeSend: function () { $("#imgLoading").show(); $("#btnClear").attr("disabled", "disabled"); $("#btnClearAll").attr("disabled", "disabled"); }, //发送数据之前
                complete: function () { $("#imgLoading").hide(); $("#btnClear").attr("disabled", false); $("#btnClearAll").attr("disabled", false); }, //接收数据完毕
                success: function (data) {
                    $("#MsgTB").show();
                    if (data == "ok") {
                        $("#labState").text("撤销成功");
                        selectedArr = [];
                        GetShopList(currPage, pageSize);
                    }
                    else {
                        $("#labState").text("撤销失败：");
                        $("#labTips").text(data);
                    }
                }
            })
        }
    })
    //清空全部
    $("#btnClearAll").click(function () {

        var state = $("input[name='cbAssignState']:checked").val() || "";
        if (state == "0") {
            return false;
        }
        GetCondition();
        if (guidanceId == "") {
            alert("请选择活动");
            return false;
        }
        var len = $("#listBody tr").length;
        if (len == 0) {
            //alert("没有店铺可以提交");
            return false;
        }
        if (confirm("确定撤销吗？")) {
            $("#AssignStateTB").hide();
            $.ajax({
                type: "post",
                url: "./handler/AssignOrder.ashx",
                data: { type: "cancelAssign", guidanceId: guidanceId, subjectids: subjectId, regions: region, provinces: province, citys: city, customerServiceId: customerServiceId, cityTier: cityTier, isInstall: isInstall, materialAssign: materialAssign, channel: channel, format: format },
                cache: false,
                beforeSend: function () { $("#imgLoading").show(); $("#btnClear").attr("disabled", "disabled"); $("#btnClearAll").attr("disabled", "disabled"); }, //发送数据之前
                complete: function () { $("#imgLoading").hide(); $("#btnClear").attr("disabled", false); $("#btnClearAll").attr("disabled", false); }, //接收数据完毕
                success: function (data) {
                    $("#MsgTB").show();
                    if (data == "ok") {
                        $("#labState").text("撤销成功");
                        selectedArr = [];
                        GetShopList(currPage, pageSize);
                    }
                    else {
                        $("#labState").text("撤销失败：");
                        $("#labTips").text(data);
                    }
                }
            })
        }
    })

    //查看重复订单
    $("#spanCheckRepeat").click(function () {
        var url = "CheckRepeatOrder.aspx?guidanceId=" + (guidanceId || 0) + "&subjectIds=" + subjectId + "&regions=" + region + "&provinces=" + province + "&citys=" + city + "&customerService=" + customerServiceId + "&cityTier=" + cityTier + "&isInstall=" + isInstall + "&ruanmo=" + ruanmo + "&materialAssign=" + materialAssign + "&channel=" + channel + "&format=" + format + "&materialPlan=" + materialPlan + "&sheet=" + sheet + "&shopNo=" + shopNo;
        layer.open({
            type: 2,
            time: 0,
            title: '查看重复订单',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '92%'],
            content: url,
            cancel: function () {
                //Order.getMaterialList();
            }


        });
    })
    //查看订单明细
    $("#spanCheckOrderDetail").click(function () {
        var url = "CheckOrderDetail.aspx?guidanceId=" + (guidanceId || 0) + "&subjectIds=" + subjectId + "&regions=" + region + "&provinces=" + province + "&citys=" + city + "&customerService=" + customerServiceId + "&cityTier=" + cityTier + "&isInstall=" + isInstall + "&ruanmo=" + ruanmo + "&materialAssign=" + materialAssign + "&channel=" + channel + "&format=" + format + "&materialPlan=" + materialPlan + "&sheet=" + sheet + "&shopNo=" + shopNo;
        layer.open({
            type: 2,
            time: 0,
            title: '查看订单明细',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '92%'],
            content: url,
            cancel: function () {
                //Order.getMaterialList();
            }


        });
    })

    $("input[name='cbAssignState']").click(function () {
        var checked = this.checked;
        if (checked) {
            $("input[name='cbAssignState']").each(function () {
                $(this).attr("checked", false);
            })
            $(this).attr("checked", checked);
        }
        isSearch = 1;
        GetShopList(currPage, pageSize);
    })


    //查询上个月活动
    $("#spanUp").click(function () {
        var month1 = $.trim($("#txtMonth").val());

        if (month1 != "") {
            month1 = month1.replace(/-/g, "/");
            var date = new Date(month1);
            date.setMonth(date.getMonth() - 1);
            $("#txtMonth").val(date.Format("yyyy-MM"));
            GetGuidance1();
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
            GetGuidance1();
        }
    })

    $("input[name^='cblRuanMo']").change(function () {
        var checked = this.checked;
        if (checked) {
            $("input[name^='cblRuanMo']").each(function () {
                $(this).attr("checked", false);
            })
            $(this).attr("checked", true);
            var val = $(this).val();
            if (val == 1) {
                $("input[name^='cblMaterialAssign']").each(function () {
                    $(this).attr("checked", false).attr("disabled", "disabled");
                })
            }
            else {
                $("input[name^='cblMaterialAssign']").each(function () {
                    $(this).attr("checked", false).attr("disabled", false);
                })
            }
        }
        else {
            $("input[name^='cblMaterialAssign']").each(function () {
                $(this).attr("checked", false).attr("disabled", false);
            })
        }
    })
    $("input[name^='cblMaterialAssign']").change(function () {
        var checked = this.checked;
        if (checked) {
            $("input[name^='cblMaterialAssign']").each(function () {
                $(this).attr("checked", false);
            })
            $(this).attr("checked", true);
            var val = $(this).val();
            if (val == "背胶") {
                $("input[name^='cblMaterialPlan']").each(function () {
                    $(this).attr("checked", false).attr("disabled", false);
                })
            }
            else {

                $("input[name^='cblMaterialPlan']").each(function () {
                    $(this).attr("checked", false).attr("disabled", "disabled");
                })
            }
        }
        else {
            $("input[name^='cblMaterialPlan']").each(function () {
                $(this).attr("checked", false).attr("disabled", "disabled");
            })
        }
    })
    $("input[name^='cblMaterialPlan']").change(function () {
        var checked = this.checked;
        if (checked) {
            $("input[name^='cblMaterialPlan']").each(function () {
                $(this).attr("checked", false);
            })
            $(this).attr("checked", true);

        }

    })

    $("input[name^='rblOrderType']").change(function () {

        var val = $(this).val();
        if (val == "2") {
            $("#seleInstallCompany").attr("disabled", false);
        }
        else {
            $("#seleInstallCompany").val("0").attr("disabled", "disabled");
        }

    })
})


function getSelected() {
    //    var arr = [];
    //    var ids= getCookie("assignOrder") || "";
    //    if (ids != "") {
    //        arr = ids.split(',');
    //    }
    $("#listBody").find("input[name='cbOne']").each(function () {
        var id = $(this).val();

        if (this.checked) {
            var flag = false;
            for (i in selectedArr) {
                if (selectedArr[i] == id) {
                    flag = true;
                }
            }
            if (!flag)
                selectedArr.push(id);
        }
        else {
            var index = -1;
            for (i = 0; i < selectedArr.length; i++) {
                if (selectedArr[i] == id) {
                    index = i;
                }
            }
            if (index > -1)
                selectedArr.splice(index, 1);
        }
    })
    //    if (arr.length > 0) {
    //        setCookie(assignOrder, arr.join(','));
    //    }
}

//获取查询条件
function GetCondition() {
    
    guidanceId = $("#guidanceDiv").find("input[name='guidancerd']:checked").val() || "";
    subjectId = "";
    //$("#hfCityData").val("");
    $("input[name='projectCB']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    region = "";
    $("input[name='regioncb']:checked").each(function () {
        region += $(this).val() + ",";
    })
    province = "";
    $("input[name='provincecb']:checked").each(function () {
        province += $(this).val() + ",";
    })
    city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    customerServiceId = "";
    $("input[name='cscb']:checked").each(function () {
        customerServiceId += $(this).val() + ",";
    })
    cityTier = "";
    $("input[name='cityTiercb']:checked").each(function () {
        cityTier += $(this).val() + ",";
    })
    isInstall = "";
    $("input[name='isInstallcb']:checked").each(function () {
        isInstall += $(this).val() + ",";
    })
    channel = "";
    $("input[name='channelcb']:checked").each(function () {
        channel += $(this).val() + ",";
    })
    format = "";
    $("input[name='formatcb']:checked").each(function () {
        format += $(this).val() + ",";
    })
    sheet = "";
    $("input[name='sheetcb']:checked").each(function () {
        sheet += $(this).val() + ",";
    })
    assignState = "";
    shopNo = "";
    $("input[name='cbAssignState']:checked").each(function () {
        assignState += $(this).val() + ",";
    })
    shopNo = $.trim($("#txtSearchShopNo").val());
    otherMaterial = "";
    otherMaterial = $.trim($("#txtOtherMaterial").val());
    noInstallPrice = 0;
    if ($("#cbNoInstallPrice").attr("checked") == "checked") {
        noInstallPrice = 1;
    }
    if (subjectId.length > 0) {
        subjectId = subjectId.substring(0, subjectId.length - 1);
    }
    if (region.length > 0) {
        region = region.substring(0, region.length - 1);
    }
    if (province.length > 0) {
        province = province.substring(0, province.length - 1);
    }
    if (city.length > 0) {
        city = city.substring(0, city.length - 1);
        //$("#hfCityData").val(city);
    }
    if (customerServiceId.length > 0) {
        customerServiceId = customerServiceId.substring(0, customerServiceId.length - 1);

    }
    if (assignState.length > 0) {
        assignState = assignState.substring(0, assignState.length - 1);

    }
    if (channel.length > 0) {
        channel = channel.substring(0, channel.length - 1);

    }
    if (format.length > 0) {
        format = format.substring(0, format.length - 1);

    }
    if (sheet.length > 0) {
        sheet = sheet.substring(0, sheet.length - 1);

    }
    ruanmo = "";
    $("input[name^='cblRuanMo']:checked").each(function () {
        ruanmo += $(this).val() + ",";
    })
    if (ruanmo != "") {
        ruanmo = ruanmo.substring(0, ruanmo.length - 1);
    }
    materialAssign = "";
    $("input[name^='cblMaterialAssign']:checked").each(function () {
        materialAssign = $(this).val();
    })
    materialPlan = "";
    $("input[name^='cblMaterialPlan']:checked").each(function () {
        materialPlan = $(this).val();
    })
    searchType = $("input[name$='rblSearchType']:checked").val() || "";

}

function GetGuidance1() {
    var customerId = $("#ddlCustomer").val() || "0";
    customerId = customerId == 0 ? -1 : customerId;
    var guidanceMonth = $.trim($("#txtMonth").val());
    $("#guidanceDiv").html("");
    $("#projectsDiv").html("");
    $("#activityDiv").html("");
    $("#typeDiv").html("");
    $("#RegionDiv").html("");
    $("#ProvinceDiv").html("");
    $("#CityDiv").html("");
    $("#CustomerServiceNameDiv").html("");
    $("#cityTierDiv").html("");
    $("#IsInstallDiv").html("");
    $("#cbALL").attr("checked", false);

    $.ajax({
        type: "get",
        url: "../Subjects/Handler/CheckOrder.ashx",
        data: { type: "getGuidance", customerId: customerId, guidanceMonth: guidanceMonth },
        cache: false,
        success: function (data) {

            if (data != "") {
                var json = eval(data);

                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left; margin-right:10px;'><input type='radio' name='guidancerd' value='" + json[i].Id + "' />" + json[i].GuidanceName + "&nbsp;</div>";
                    $("#guidanceDiv").append(div);
                }
            }
            else {
                $("#guidanceDiv").html("<span style='color:red;'>无活动信息！</span>");
            }
        }
    })
}

function GetGuidance() {
    var customerId = $("#ddlCustomer").val() || "0";
    var begin = $.trim($("#txtBeginDate").val());
    var end = $.trim($("#txtEndDate").val());
    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    if (begin == "" || end == "") {
        alert("请选择开始时间和结束时间");
        return false;
    }
    if (begin != "" && end != "") {
        $("#guidanceDiv").html("");
        $("#projectsDiv").html("");
        $("#activityDiv").html("");
        $("#typeDiv").html("");
        $(".trType").hide();
        $("#imgLoading1").show();


        $.ajax({
            type: "get",
            url: "../Subjects/Handler/CheckOrder.ashx",
            data: { type: "getGuidance", customerId: customerId, beginDate: begin, endDate: end },
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    $("#imgLoading1").hide();

                    for (var i = 0; i < json.length; i++) {
                        //var option = "<option value='" + json[i].Id + "'>" + json[i].GuidanceName + "</option>";
                        //$("#selectGuidance").append(option);

                        var div = "<div style='float:left;'><input type='radio' name='guidancerd' value='" + json[i].Id + "' /><span>" + json[i].GuidanceName + "</span>&nbsp;</div>";
                        $("#guidanceDiv").append(div);
                    }
                }
            }
        })

    }
}

function GetProjectList() {

    var guidanceIds = $("#guidanceDiv").find("input[name='guidancerd']:checked").val() || "";


    $("#projectsDiv").html("");
    $("#activityDiv").html("");
    $("#typeDiv").html("");
    $("#RegionDiv").html("");
    $("#ProvinceDiv").html("");
    $("#CityDiv").html("");
    $("#CustomerServiceNameDiv").html("");
    $("#cityTierDiv").html("");
    $("#IsInstallDiv").html("");
    $("#cbALL").attr("checked", false);
    $("#projectLoadImg").show();
    $.ajax({
        type: "get",
        url: "../Subjects/Handler/CheckOrder.ashx",
        data: { type: "getProjectList", guidanceIds: guidanceIds },
        complete: function () { $("#projectLoadImg").hide(); },
        success: function (data) {

            if (data != "") {
                var projectStr = data.split('|')[0];

                var typeStr = data.split('|')[1];
                var activityStr = data.split('|')[2];
                if (activityStr != "") {
                    var json0 = eval(activityStr);
                    for (var i = 0; i < json0.length; i++) {
                        var div = "<div style='float:left;margin-right:10px;'><input type='checkbox' name='activityCB' value='" + json0[i].Id + "' /><span>" + json0[i].ActivityName + "</span>&nbsp;</div>";
                        $("#activityDiv").append(div);
                    }
                }
                if (typeStr != "") {
                    var json = eval(typeStr);
                    for (var i = 0; i < json.length; i++) {
                        var div = "<div style='float:left;margin-right:10px;'><input type='checkbox' name='typeCB' value='" + json[i].Id + "' /><span>" + json[i].TypeName + "</span>&nbsp;</div>";
                        $("#typeDiv").append(div);
                    }
                }
                if (projectStr != "") {
                    $(".trType").show();
                    var json1 = eval(projectStr);
                    for (var i = 0; i < json1.length; i++) {
                        var div = "<div style='float:left;margin-right:10px;'><input type='checkbox' name='projectCB' value='" + json1[i].Id + "' /><span>" + json1[i].SubjectName + "</span>&nbsp;</div>";
                        $("#projectsDiv").append(div);
                    }
                }
            }

        }
    })
}

function ScreenProject() {
    $("#projectsDiv").html("");
    //var guidanceId = $("#selectGuidance").val() || 0;
    var guidanceIds = $("#guidanceDiv").find("input[name='guidancerd']:checked").val() || "";
    //    $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
    //        guidanceIds += $(this).val() + ",";
    //    })
    var activityIds = "";
    var typeIds = "";
    $("input[name='activityCB']:checked").each(function () {
        activityIds += $(this).val() + ",";
    })
    $("input[name='typeCB']:checked").each(function () {
        typeIds += $(this).val() + ",";
    })
    $("#projectLoadImg").show();
    $.ajax({
        type: "get",
        url: "../Subjects/Handler/CheckOrder.ashx",
        data: { type: "screenProject", guidanceIds: guidanceIds, activityId: activityIds, typeId: typeIds },
        complete: function () { $("#projectLoadImg").hide(); },
        success: function (data) {

            if (data != "") {
                var json1 = eval(data);
                for (var i = 0; i < json1.length; i++) {
                    var div = "<div style='float:left;'><input type='checkbox' name='projectCB' value='" + json1[i].Id + "' /><span>" + json1[i].SubjectName + "</span>&nbsp;</div>";
                    $("#projectsDiv").append(div);
                }
            }
        }
    })
}


function GetProjects() {
    var customerId = $("#ddlCustomer").val() || "0";
    var begin = $.trim($("#txtBeginDate").val());
    var end = $.trim($("#txtEndDate").val());
    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    if (begin == "" || end == "") {
        alert("请选择开始时间和结束时间");
        return false;
    }
    if (begin != "" && end != "") {
        var loading = "<img src='../image/WaitImg/loading1.gif' />";
        $("#projectsDiv").html(loading);
        $.ajax({
            type: "get",
            url: "../Subjects/Handler/CheckOrder.ashx",
            data: { type: "getProjects", approveState: 1, customerId: customerId, beginDate: begin, endDate: end, export: 1 },
            cache: false,
            success: function (data) {
                $("#projectsDiv").html("");
                if (data != "") {

                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var div = "<div style='float:left;'><input type='checkbox' name='projectCB' value='" + json[i].Id + "' /><span>" + json[i].SubjectName + "</span>&nbsp;</div>";
                        $("#projectsDiv").append(div);
                    }
                }

            }

        })
    }

}

function GetRegions() {
    var guidanceId = $("#guidanceDiv").find("input[name='guidancerd']:checked").val() || "";
    var ids = "";
    $("input[name='projectCB']:checked").each(function () {
        ids += $(this).val() + ",";
    })
    if (ids.length > 0) {
        ids = ids.substring(0, ids.length - 1);
        $("#regionLoadImg").show();
        $.ajax({
            //type: "get",
            //url: "../Subjects/Handler/ExportOrders.ashx?type=getregion&subjectids=" + ids,
            //cache: false,
            //complete: function () { $("#regionLoadImg").hide(); },
            type: "post",
            url: "./handler/AssignOrder.ashx",
            data: { type: "getRegion", guidanceId: guidanceId, subjectIds: ids },
            complete: function () {
                $("#regionLoadImg").hide();
                GetCustomerService();
                GetCityTier();
                GetIsInstall();
                GetChannel();
                GetFormat();
                GetSheet();
            },
            success: function (data) {
                $("#RegionDiv").html("");
                if (data != "") {
                    var json = eval(data);
                    var div = "";
                    for (var i = 0; i < json.length; i++) {
                        div += "<div style='float:left;'><input type='checkbox' name='regioncb' value='" + json[i].RegionName + "' /><span>" + json[i].RegionName + "</span>&nbsp;</div>";

                    }
                    $("#RegionDiv").html(div);
                }
            }
        })
    }
    else
        $("#RegionDiv").html("");
}

function GetProvince() {
    //var sds = "";
    //$("input[name='projectCB']:checked").each(function () {
        //sds += $(this).val() + ",";
    //})
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })

    if (regions.length > 0) {
        regions = regions.substring(0, regions.length - 1);
        //sds = sds.substring(0, sds.length - 1);
        $("#provinceLoadImg").show();
        $.ajax({
            //type: "get",
            //url: "../Subjects/Handler/ExportOrders.ashx?type=getprovince&regions=" + regions + "&subjectids=" + sds,
            //cache: false,
            type: "post",
            url: "./handler/AssignOrder.ashx",
            data: { type: "getProvince", region: regions},
            complete: function () { $("#provinceLoadImg").hide(); },
            success: function (data) {
                $("#ProvinceDiv").html("");
                if (data != "") {
                    var json = eval(data);
                    var div = "";
                    for (var i = 0; i < json.length; i++) {
                        div += "<div style='float:left;'><input type='checkbox' name='provincecb' value='" + json[i].ProvinceName + "' /><span>" + json[i].ProvinceName + "</span>&nbsp;</div>";

                    }
                    $("#ProvinceDiv").html(div);
                }
            }
        })
    }
    else
        $("#ProvinceDiv").html("");
}

function GetCity() {
    $("#cbAllDiv").hide();
    $("#cbAllCity").attr("checked", false);
    //var sds = "";
    //$("input[name='projectCB']:checked").each(function () {
        //sds += $(this).val() + ",";
    //})
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    if (provinces.length > 0) {
        //sds = sds.substring(0, sds.length - 1);
        regions = regions.substring(0, regions.length - 1);
        provinces = provinces.substring(0, provinces.length - 1);
        $("#cityLoadImg").show();
        $.ajax({
            //type: "get",
            //url: "../Subjects/Handler/ExportOrders.ashx?type=getcity&regions=" + regions + "&subjectids=" + sds + "&province=" + escape(provinces),
            //cache: false,
            type: "post",
            url: "./handler/AssignOrder.ashx",
            data: { type: "getCity", region: regions, province: provinces },
            complete: function () { $("#cityLoadImg").hide(); },
            success: function (data) {

                $("#CityDiv").html("");
                if (data != "") {
                    var json = eval(data);
                    var div = "";
                    for (var i = 0; i < json.length; i++) {
                        div += "<div style='float:left;'><input type='checkbox' name='citycb' value='" + json[i].CityName + "' /><span>" + json[i].CityName + "</span>&nbsp;</div>";

                    }
                    $("#CityDiv").html(div);
                    $("#cbAllDiv").show();
                }
            }
        })
    }
    else {
        $("#CityDiv").html("");
    }
}


function GetCustomerService() {

    var subjectIds = "";
    $("input[name='projectCB']:checked").each(function () {
        subjectIds += $(this).val() + ",";
    })
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })

    if (subjectIds.length > 0) {
        //$("#csLoadImg").show();
        $("#csLoadImg").show();
        $.ajax({
            //type: "get",
            //url: "../Subjects/Handler/ExportOrders.ashx?type=getCS&regions=" + regions + "&subjectids=" + subjectIds + "&province=" + escape(provinces) + "&city=" + escape(city),
            //cache: false,
            type: "post",
            url: "./handler/AssignOrder.ashx",
            data: { type: "getCS", region: regions, province: provinces, city: city, subjectIds: subjectIds },
            complete: function () { $("#csLoadImg").hide(); },
            success: function (data) {
                //$("#csLoadImg").hide();
                $("#CustomerServiceNameDiv").html("");
                if (data != "") {
                    var json = eval(data);
                    var div = "";

                    for (var i = 0; i < json.length; i++) {

                        div += "<div style='float:left;'><input type='checkbox' name='cscb' value='" + json[i].UserId + "'/><span>" + json[i].UserName + "</span>&nbsp;</div>";

                    }

                    $("#CustomerServiceNameDiv").html(div);

                }
            }
        })
    }
    else {
        $("#CustomerServiceNameDiv").html("");
    }
}

function GetCityTier() {
    //var guidanceId = $("#guidanceDiv").find("input[name='guidancerd']:checked").val() || "";
    //var subjectIds = "";
    //$("input[name='projectCB']:checked").each(function () {
        //subjectIds += $(this).val() + ",";
    //})
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    var csid = "";
    $("input[name='cscb']:checked").each(function () {
        csid += $(this).val() + ",";
    })
    $("#cityTierLoadImg").show();
    $.ajax({
        type: "post",
        url: "./handler/AssignOrder.ashx",
        data: { type: "getCityTier", regions: regions, provinces: provinces, citys: city, customerServiceId: csid },
        complete: function () { $("#cityTierLoadImg").hide(); },
        success: function (data) {

            $("#cityTierDiv").html("");
            if (data != "") {

                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='cityTiercb' value='" + json[i].CityTier + "' /><span>" + json[i].CityTier + "</span>&nbsp;</div>";

                }
                $("#cityTierDiv").html(div);
            }

        }
    })
}

function GetIsInstall() {
    //var guidanceId = $("#guidanceDiv").find("input[name='guidancerd']:checked").val() || "";
    //var subjectIds = "";
    //$("input[name='projectCB']:checked").each(function () {
        //subjectIds += $(this).val() + ",";
    //})
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    var csid = "";
    $("input[name='cscb']:checked").each(function () {
        csid += $(this).val() + ",";
    })
    var cityTier = "";
    $("input[name='cityTiercb']:checked").each(function () {
        cityTier += $(this).val() + ",";
    })
    $("#IsInstallLoadImg").show();
    $.ajax({
        type: "post",
        url: "./handler/AssignOrder.ashx",
        data: { type: "getInstallLevel",regions: regions, provinces: provinces, citys: city, customerServiceId: csid, cityTier: cityTier },
        complete: function () { $("#IsInstallLoadImg").hide(); },
        success: function (data) {

            $("#IsInstallDiv").html("");
            if (data != "") {

                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='isInstallcb' value='" + json[i].IsInstall + "' /><span>" + json[i].IsInstall + "</span>&nbsp;</div>";

                }
                $("#IsInstallDiv").html(div);
            }

        }
    })
}

function GetMaterial() {
    var guidanceId = $("#guidanceDiv").find("input[name='guidancerd']:checked").val() || "";
    var subjectIds = "";
    $("input[name='projectCB']:checked").each(function () {
        subjectIds += $(this).val() + ",";
    })
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    var csid = "";
    $("input[name='cscb']:checked").each(function () {
        csid += $(this).val() + ",";
    })
    var cityTier = "";
    $("input[name='cityTiercb']:checked").each(function () {
        cityTier += $(this).val() + ",";
    })
    var isInstall = "";
    $("input[name='isInstallcb']:checked").each(function () {
        isInstall += $(this).val() + ",";
    })
    $("#MaterialLoadImg").show();
    $.ajax({
        type: "post",
        url: "./handler/AssignOrder.ashx",
        data: { type: "getMaterial", guidanceId: guidanceId, subjectIds: subjectIds, regions: regions, provinces: provinces, citys: city, customerServiceId: csid, cityTier: cityTier, isInstall: isInstall },
        complete: function () { $("#MaterialLoadImg").hide(); },
        success: function (data) {

            $("#MaterialDiv").html("");
            if (data != "") {

                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='materialcb' value='" + json[i].GraphicMaterial + "' /><span>" + json[i].GraphicMaterial + "</span>&nbsp;</div>";

                }
                $("#MaterialDiv").html(div);
            }

        }
    })
}


function GetChannel() {
    //var guidanceId = $("#guidanceDiv").find("input[name='guidancerd']:checked").val() || "";
    //var subjectIds = "";
    //$("input[name='projectCB']:checked").each(function () {
        //subjectIds += $(this).val() + ",";
    //})
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    var csid = "";
    $("input[name='cscb']:checked").each(function () {
        csid += $(this).val() + ",";
    })
    var cityTier = "";
    $("input[name='cityTiercb']:checked").each(function () {
        cityTier += $(this).val() + ",";
    })
    var isInstall = "";
    $("input[name='isInstallcb']:checked").each(function () {
        isInstall += $(this).val() + ",";
    })
    $("#ChannelLoadImg").show();
    $.ajax({
        type: "post",
        url: "./handler/AssignOrder.ashx",
        data: { type: "getChannel", regions: regions, provinces: provinces, citys: city, customerServiceId: csid, cityTier: cityTier, isInstall: isInstall },
        complete: function () { $("#ChannelLoadImg").hide(); },
        success: function (data) {

            $("#ChannelDiv").html("");
            if (data != "") {

                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='channelcb' value='" + json[i].Channel + "' /><span>" + json[i].Channel + "</span>&nbsp;</div>";

                }
                $("#ChannelDiv").html(div);
            }

        }
    })
}

function GetFormat() {
    //var guidanceId = $("#guidanceDiv").find("input[name='guidancerd']:checked").val() || "";
    //var subjectIds = "";
    //$("input[name='projectCB']:checked").each(function () {
        //subjectIds += $(this).val() + ",";
    //})
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    var csid = "";
    $("input[name='cscb']:checked").each(function () {
        csid += $(this).val() + ",";
    })
    var cityTier = "";
    $("input[name='cityTiercb']:checked").each(function () {
        cityTier += $(this).val() + ",";
    })
    var isInstall = "";
    $("input[name='isInstallcb']:checked").each(function () {
        isInstall += $(this).val() + ",";
    })
    var channel = "";
    $("input[name='channelcb']:checked").each(function () {
        channel += $(this).val() + ",";
    })
    $("#FormatLoadImg").show();
    $.ajax({
        type: "post",
        url: "./handler/AssignOrder.ashx",
        data: { type: "getFormat",regions: regions, provinces: provinces, citys: city, customerServiceId: csid, cityTier: cityTier, isInstall: isInstall,channel:channel },
        complete: function () { $("#FormatLoadImg").hide(); },
        success: function (data) {

            $("#FormatDiv").html("");
            if (data != "") {

                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='formatcb' value='" + json[i].Format + "' /><span>" + json[i].Format + "</span>&nbsp;</div>";

                }
                $("#FormatDiv").html(div);
            }

        }
    })
}

function GetSheet() {
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    var city = "";
    $("input[name='citycb']:checked").each(function () {
        city += $(this).val() + ",";
    })
    var csid = "";
    $("input[name='cscb']:checked").each(function () {
        csid += $(this).val() + ",";
    })
    var cityTier = "";
    $("input[name='cityTiercb']:checked").each(function () {
        cityTier += $(this).val() + ",";
    })
    var isInstall = "";
    $("input[name='isInstallcb']:checked").each(function () {
        isInstall += $(this).val() + ",";
    })
    var channel = "";
    $("input[name='channelcb']:checked").each(function () {
        channel += $(this).val() + ",";
    })
    var format = "";
    $("input[name='formatcb']:checked").each(function () {
        format += $(this).val() + ",";
    })
    $("#SheetLoadImg").show();
    $("#cbAllSheet").attr("checked", false);
    $.ajax({
        type: "post",
        url: "./handler/AssignOrder.ashx",
        data: { type: "getSheet", regions: regions, provinces: provinces, citys: city, customerServiceId: csid, cityTier: cityTier, isInstall: isInstall, channel: channel, format: format },
        complete: function () { $("#SheetLoadImg").hide(); },
        success: function (data) {

            $("#SheetDiv").html("");
            if (data != "") {
                $("#cbAllSheetDiv").show();
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='sheetcb' value='" + json[i].Sheet + "' /><span>" + json[i].Sheet + "</span>&nbsp;</div>";

                }
                $("#SheetDiv").html(div);
            }
            else
                $("#cbAllSheetDiv").hide();
        }
    })
}

function getMonth() {
    GetGuidance1();
}

//获取外协公司
function GetOutsourcing() {

    var customerId = $("#ddlCustomer").val() || "0";
    var region1 = "";
    $("input[name='regioncb']:checked").each(function () {
        region1 += $(this).val() + ",";
    })
    document.getElementById("seleCompany").length = 1;
    $.ajax({
        type: "get",
        url: "./handler/AssignOrder.ashx",
        data: { type: "GetOutsourcing", region: region1, customerId: customerId },
        success: function (data) {
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var option = "<option value='" + json[i].Id + "'>" + json[i].CompanyName + "</option>";
                    $("#seleCompany").append(option);
                    $("#seleInstallCompany").append(option);
                }
            }
        }
    })
}


//分页
var pageSize = 20;
var currPage = 0;
function GetOrderDetail(pageindx, pagesize) {
    GetCondition();
    if (subjectId == "") {
        alert("请选择项目");
        $("#listBody").html("");
        HideDetail();
        return false;
    }
    $.ajax({
        type: "get",
        url: "../Subjects/Handler/ExportOrders.ashx?type=getlist&currpage=" + pageindx + "&pagesize=" + pagesize,
        data: { subjectids: subjectId, regions: region, province: province, city: city, searchType: searchType },
        cache: false,
        beforeSend: function () { $("#divload").show(); $("#noData").hide(); }, //发送数据之前
        complete: function () { $("#divload").hide(); }, //接收数据完毕
        success: function (data) {

            if (data != "") {
                $("#divClean").show();
                $("#popListTB").show(); $("#Pagination").show();
                var data1 = data.split('$');
                var json = eval(data1[0]);
                var totalShopNum = data1[1];
                $("#labShopCount").html(totalShopNum);
                $("#shopNumDiv").show();
                var total = json[0].total;
                var tr = "";
                for (var i = 0; i < json.length; i++) {
                    var rowIndex = pageindx * pagesize + i + 1;
                    //var flag = false;
                    var selected = "";
                    if (selectedArr.length > 0) {
                        $.each(selectedArr, function (key, val) {
                            if (val == json[i].Id) {
                                selected = "checked='checked'";
                            }
                        })
                    }
                    tr += "<tr class='tr_bai'>";
                    tr += "<td>" + rowIndex + "</td>";
                    tr += "<td><input type='checkbox' name='cbOne' value=" + json[i].Id + " " + selected + "/></td>";
                    tr += "<td>" + json[i].CompanyName + "</td>";
                    tr += "<td>" + json[i].OrderType + "</td>";
                    tr += "<td>" + json[i].ShopNo + "</td>";
                    tr += "<td>" + json[i].ShopName + "</td>";
                    tr += "<td>" + json[i].Region + "</td>";
                    tr += "<td>" + json[i].Province + "</td>";
                    tr += "<td>" + json[i].City + "</td>";
                    tr += "<td>" + json[i].CityTier + "</td>";
                    tr += "<td>" + json[i].Format + "</td>";
                    tr += "<td>" + json[i].MaterialSupport + "</td>";
                    tr += "<td>" + json[i].POSScale + "</td>";

                    tr += "<td>" + json[i].Sheet + "</td>";
                    tr += "<td>" + json[i].LevelNum + "</td>";
                    tr += "<td>" + json[i].Gender + "</td>";
                    tr += "<td>" + json[i].Quantity + "</td>";
                    tr += "<td>" + json[i].MachineFrame + "</td>";
                    tr += "<td>" + json[i].PositionDescription + "</td>";
                    tr += "<td>" + json[i].GraphicWidth + "</td>";
                    tr += "<td>" + json[i].GraphicLength + "</td>";
                    tr += "<td>" + json[i].Area + "</td>";
                    tr += "<td>" + json[i].GraphicMaterial + "</td>";

                    tr += "<td>" + json[i].ChooseImg + "</td>";
                    //tr += "<td>" + json[i].Remark + "</td>";
                    tr += "</tr>";

                }
                $("#listBody").html(tr);

                var strJS = "pageOperate(" + pageindx + "," + pagesize + "," + total + ")";
                eval(strJS);
            }
            else {
                HideDetail();
            }
        }
    })
}

var scollHeight = 0;
function GetShopList(pageindx, pagesize) {
    scollHeight = $(document).scrollTop();


    $("#cbAll1").prop("checked", false);
    GetCondition();
    var subjectIdNew = subjectId;
    if (subjectId == "") {
        alert("请选择项目");
        $("#listBody").html("");
        HideDetail();
        return false;
    }
    else {
        var selectLen = $("input[name='projectCB']:checked").length;
        var totalLen = $("input[name='projectCB']").length;
        if (selectLen == totalLen) {
            subjectIdNew = "";
        }
    }
    $.ajax({
        type: "post",
        url: "handler/AssignOrder.ashx",
        data: { type: "getShopList", currpage: pageindx, pagesize: pagesize, guidanceId: guidanceId, subjectIds: subjectIdNew, regions: region, provinces: province, citys: city, isSearch: isSearch, assignState: assignState, shopNo: shopNo, customerServiceId: customerServiceId, cityTier: cityTier, isInstall: isInstall, ruanmo: ruanmo, materialAssign: materialAssign, channel: channel, format: format, materialPlan: materialPlan, sheet: sheet, otherMaterial: otherMaterial },
        cache: false,
        beforeSend: function () { $("#divload").show(); $("#noData").hide(); }, //发送数据之前
        complete: function () { $("#divload").hide(); $(document).scrollTop(scollHeight); if (isSearch == 1) { isSearch = 0; } }, //接收数据完毕
        success: function (data) {
            $("#spanCheckOrderDetail").show();

            if (data != "") {

                $("#divClean").show();
                $("#popListTB").show(); $("#Pagination").show();
                var tr = "";
                var jsonData = data.split('|')[0];
                var orderCount = data.split('|')[1];
                var repeatCount = data.split('|')[2];
                var isEditProduce = data.split('|')[3];
                var assignTotalOrderCount = data.split('|')[4];
                var notAssignTotalOrderCount = data.split('|')[5];
                var json = eval(jsonData);
                var total = json[0].total;
                $("#labShopCount").html(total);
                $("#labAssignShopCount").html(json[0].AssignShopCount);



                $("#labAssignTotalOrderCount").html(assignTotalOrderCount);
                $("#labNotAssignTotalOrderCount").html(notAssignTotalOrderCount);
                if (notAssignTotalOrderCount > 0) {
                    var span = "<span style='color:red'>" + notAssignTotalOrderCount + "</span>";
                    $("#labNotAssignTotalOrderCount").html(span);
                }


                if (json[0].NotAssignShopCount > 0) {
                    var span = "<span style='color:red'>" + json[0].NotAssignShopCount + "</span>";
                    $("#labNotAssignShopCount").html(span);
                }
                else {
                    $("#labNotAssignShopCount").html("0");
                }
                if (isSearch == 1) {
                    $("#labOrderCount").html(orderCount);
                    $("#labHasRepeated").html(repeatCount);

                    if (repeatCount > 0 || isEditProduce > 0) {
                        $("#spanCheckRepeat").show();
                    }
                    else {
                        $("#spanCheckRepeat").hide();
                    }
                }
                $("#shopNumDiv").show();
                for (var i = 0; i < json.length; i++) {
                    var rowIndex = pageindx * pagesize + i + 1;
                    var selected = "";
                    if (selectedArr.length > 0) {
                        $.each(selectedArr, function (key, val) {
                            if (val == json[i].ShopId) {
                                selected = "checked='checked'";
                            }
                        })
                    }
                    tr += "<tr class='tr_bai'>";
                    tr += "<td>" + rowIndex + "</td>";
                    tr += "<td><input type='checkbox' name='cbOne' value=" + json[i].ShopId + " " + selected + "/></td>";
                    tr += "<td>" + json[i].OutsourceName + "</td>";
                    //tr += "<td>" + json[i].OrderTypeName + "</td>";
                    tr += "<td>" + json[i].ShopNo + "</td>";
                    tr += "<td>" + json[i].ShopName + "</td>";
                    tr += "<td>" + json[i].Region + "</td>";
                    tr += "<td>" + json[i].Province + "</td>";
                    tr += "<td>" + json[i].City + "</td>";
                    tr += "<td>" + json[i].CityTier + "</td>";
                    tr += "<td>" + json[i].IsInstall + "</td>";
                    tr += "<td>" + json[i].Format + "</td>";


                    //tr += "<td>" + json[i].ShopOrderCount + "</td>";
                    tr += "<td><a href='javascript:void(0)' onclick='CheckOrignalOrder(" + json[i].ShopId + ")'>" + json[i].ShopOrderCount + "</a></td>";
                    var tr1 = "";
                    if (json[i].AssignOrderCount < json[i].ShopOrderCount || json[i].AssignOrderCount == 0) {
                        if (json[i].AssignOrderCount > 0)
                            tr1 = "<td style='color:red;'><a href='javascript:void(0)' onclick='CheckAssginOrder(" + json[i].ShopId + ")' style='color:red;text-decoration:underline;'>" + json[i].AssignOrderCount + "</a></td>";
                        else
                            tr1 = "<td style='color:red;'>0</td>";
                    }
                    else
                        tr1 = "<td><a href='javascript:void(0)' onclick='CheckAssginOrder(" + json[i].ShopId + ")' style='text-decoration:underline;'>" + json[i].AssignOrderCount + "</a></td>";
                    tr += tr1;
                    tr += "</tr>";
                }
                $("#listBody").html(tr);
                var strJS = "pageOperate(" + pageindx + "," + pagesize + "," + total + ")";
                eval(strJS);
            }
            else {
                HideDetail();
            }
        }
    })
}


function pageselectCallback(page_id) {

    GetShopList(page_id, pageSize);
}


function pageOperate(currPage, pageSize, total) {
    $("#Pagination").pagination(total, {
        callback: pageselectCallback,
        prev_text: '上一页',
        next_text: '下一页',
        items_per_page: pageSize,
        num_display_entries: 6,
        current_page: currPage,
        num_edge_entries: 2

    });
}

function HideDetail() {
    $("#labShopCount").html("0");
    $("#labOrderCount").html("0");
    $("#labHasRepeated").html("0");
    //$("#divClean").hide();
    $("#noData").show();
    $("#listBody").html("");
    $("#popListTB").hide();
    $("#Pagination").hide();
    $("#spanShopNum").html("");
    $("#shopNumDiv").hide();
    //$("#spanCheckOrderDetail").hide();
}


function doSearch() {
    isSearch = 1;
    GetShopList(currPage, pageSize);
    var val = $("#seleCompany option:last").val();
    if (val == 0) {
        GetOutsourcing();
    }
}


function CheckAssginOrder(shopId) {
    var subjectIdNew = subjectId;
    var selectLen = $("input[name='projectCB']:checked").length;
    var totalLen = $("input[name='projectCB']").length;
    if (selectLen == totalLen) {
        subjectIdNew = "";
    }
    var url = "FinishAssignOrderList.aspx?guidanceId=" + (guidanceId || 0) + "&shopId=" + shopId + "&subjectIds=" + subjectIdNew + "&ruanmo=" + ruanmo + "&materialAssign=" + materialAssign + "&materialPlan=" + materialPlan + "&sheet=" + sheet + "&otherMaterial=" + otherMaterial;
    layer.open({
        type: 2,
        time: 0,
        title: '查看已分配订单',
        skin: 'layui-layer-rim', //加上边框
        area: ['95%', '92%'],
        content: url,
        cancel: function () {
            //Order.getMaterialList();
        }


    });
}

function CheckOrignalOrder(shopId) {
    
    var url = "NoAssginOrderList.aspx?guidanceId=" + (guidanceId || 0) + "&shopId=" + shopId + "&subjectIds=" + subjectId + "&ruanmo=" + ruanmo + "&materialAssign=" + materialAssign + "&materialPlan=" + materialPlan + "&sheet=" + sheet + "&otherMaterial=" + otherMaterial; ;
    layer.open({
        type: 2,
        time: 0,
        title: '查看原始订单',
        skin: 'layui-layer-rim', //加上边框
        area: ['95%', '92%'],
        content: url,
        cancel: function () {
            //Order.getMaterialList();
        }


    });
}

