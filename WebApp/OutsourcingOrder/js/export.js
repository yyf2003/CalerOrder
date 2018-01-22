﻿
var subjectId = "";
var region = "";
var province = "";
var city = "";

$(function () {
    $("#ddlCustomer").change(function () {
        GetGuidance1();
    })

    $("#btnSearchSubject").on("click", function () {
        GetGuidance();
    })

    $("#guidanceDiv").delegate("input[name='guidancerd']", "click", function () {
        //GetProjectList();
        GetRegions();

    })

    //    $("#activityDiv").delegate("input[name='activityCB']", "change", function () {
    //        ScreenProject();
    //    })
    //    $("#typeDiv").delegate("input[name='typeCB']", "change", function () {
    //        ScreenProject();
    //    })

    //    $("#projectsDiv").delegate("input[name='projectCB']", "change", function () {
    //        GetRegions();
    //        GetProvince();
    //        GetCity();
    //    })

    $("#RegionDiv").delegate("input[name='regioncb']", "change", function () {
        //GetProvince();
        //GetCity();
        GetOutsource();
    })
    //    $("#ProvinceDiv").delegate("input[name='provincecb']", "change", function () {
    //        GetCity();
    //    })

    //查询
    $("#btnSearch").click(function () {
        //GetOrderDetail(currPage, pageSize);
        GetShopList(currPage, pageSize);
    })

    //项目全选
    $("#cbALL").on("click", function () {
        var check = this.checked;
        $("input[name='projectCB']").each(function () {
            this.checked = check;
        })
        GetRegions();
    })

    //导出350
    $("#btnExport350").click(function () {
        GetCondition();
        if (subjectId == "") {
            alert("请选择项目");
            return false;
        }

        $(this).attr("disabled", true).next("img").show();
        checkExport($(this), "350");
        //var url = "Handler/ExportOrders.ashx?type=exportotherphw&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&city=" + city;
        var url = "/Subjects/ExportHelper.aspx?type=exportOutsourcingOrder350&subjectids=" + subjectId + "&regions=" + region + "&province=" + province;

        $("#exportFrame").attr("src", url);
    })

    //导出竖版
    $("#btnExport").click(function () {
        GetCondition();
        if (subjectId == "") {
            alert("请选择项目");
            return false;
        }

        $(this).attr("disabled", true).next("img").show();
        checkExport($(this));
        //var url = "Handler/ExportOrders.ashx?type=exportotherphw&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&city=" + city;
        var url = "/Subjects/ExportHelper.aspx?type=exportOutsourcingOrder&subjectids=" + subjectId + "&regions=" + region + "&province=" + province;

        $("#exportFrame").attr("src", url);
    })
})

var timer;
function checkExport(obj, type) {
    type = type || "";
    timer = setInterval(function () {
        $.ajax({
            type: "get",
            url: "/Subjects/Handler/CheckExportOutsourcing.ashx?type="+type,
            cache: false,
            success: function (data) {
                
                if (data == "ok") {
                    $(obj).attr("disabled", false).next("img").hide();
                    clearInterval(timer);
                }

            }
        })

    }, 1000);
}


//获取查询条件
function GetCondition() {
    subjectId = "";
    $("#hfCityData").val("");
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
        $("#hfCityData").val(city);
    }
 
}

function GetGuidance1() {
    var customerId = $("#ddlCustomer").val() || "0";
    customerId = customerId == 0 ? -1 : customerId;
    $("#guidanceDiv").html("");
    $.ajax({
        type: "get",
        url: "../Subjects/Handler/CheckOrder.ashx",
        data: { type: "getGuidance", customerId: customerId },
        cache: false,
        success: function (data) {

            if (data != "") {
                var json = eval(data);

                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left; margin-right:10px;'><input type='radio' name='guidancerd' value='" + json[i].Id + "' />" + json[i].GuidanceName + "&nbsp;</div>";
                    $("#guidanceDiv").append(div);
                }
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
            url: "./handler/Export.ashx",
            data: { type: "getGuidance", customerId: customerId, beginDate: begin, endDate: end },
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    $("#imgLoading1").hide();

                    for (var i = 0; i < json.length; i++) {
                        //var option = "<option value='" + json[i].Id + "'>" + json[i].GuidanceName + "</option>";
                        //$("#selectGuidance").append(option);

                        var div = "<div style='float:left;'><input type='checkbox' name='guidanceCB' value='" + json[i].Id + "' /><span>" + json[i].GuidanceName + "</span>&nbsp;</div>";
                        $("#guidanceDiv").append(div);
                    }
                }
            }
        })

    }
}

function GetProjectList() {

    var guidanceIds = "";
    $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })

    $("#projectsDiv").html("");
    $("#activityDiv").html("");
    $("#typeDiv").html("");
    $(".trType").hide();
    $.ajax({
        type: "get",
        url: "./handler/Export.ashx",
        data: { type: "getProjectList", guidanceIds: guidanceIds },
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
    var guidanceIds = "";
    $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    var activityIds = "";
    var typeIds = "";
    $("input[name='activityCB']:checked").each(function () {
        activityIds += $(this).val() + ",";
    })
    $("input[name='typeCB']:checked").each(function () {
        typeIds += $(this).val() + ",";
    })

    $.ajax({
        type: "get",
        url: "./handler/Export.ashx",
        data: { type: "screenProject", guidanceIds: guidanceIds, activityId: activityIds, typeId: typeIds },
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


function GetRegions() {
    var guidanceId =$("input[name='guidancerd']:checked").val()|| "";

    if (guidanceId.length > 0) {

        $.ajax({
            type: "get",
            url: "./handler/Export.ashx?type=getregion&guidanceId=" + guidanceId,
            cache: false,
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
            },
            complete: function () {
                GetOutsource();
            }
        })
    }
    else
        $("#RegionDiv").html("");
}

function GetOutsource() {
    var customerId = $("#ddlCustomer").val();
    var guidanceId = $("input[name='guidancerd']:checked").val() || "";
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    if (regions == "") {
        $("input[name='regioncb']").each(function () {
            regions += $(this).val() + ",";
        })
    }
    document.getElementById("ddlOutsource").length = 1;
    $.ajax({
        type: "get",
        url: "./handler/Export.ashx",
        data: { type: "getoutsource", guidanceId: guidanceId, regions: regions, customerId: customerId },
        cache: false,
        success: function (data) {
           
            if (data != "") {
                var json = eval(data);

                for (var i = 0; i < json.length; i++) {
                    var option = "<option value='" + json[i].Id + "'>" + json[i].CompanyName + "</option>";
                    $("#ddlOutsource").append(option);
                }

            }
        }
    })
}

function GetProvince() {
    var sds = "";
    $("input[name='projectCB']:checked").each(function () {
        sds += $(this).val() + ",";
    })
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })

    if (regions.length > 0) {
        regions = regions.substring(0, regions.length - 1);
        sds = sds.substring(0, sds.length - 1);
        $.ajax({
            type: "get",
            url: "./handler/Export.ashx?type=getprovince&regions=" + regions + "&subjectids=" + sds,
            cache: false,
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
    var sds = "";
    $("input[name='projectCB']:checked").each(function () {
        sds += $(this).val() + ",";
    })
    var regions = "";
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    if (provinces.length > 0) {
        sds = sds.substring(0, sds.length - 1);
        regions = regions.substring(0, regions.length - 1);
        provinces = provinces.substring(0, provinces.length - 1);
        $.ajax({
            type: "get",
            url: "./handler/Export.ashx?type=getcity&regions=" + regions + "&subjectids=" + sds + "&province=" + escape(provinces),
            cache: false,
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

//分页
var pageSize = 2;
var currPage = 0;
function GetOrderDetail(pageindx, pagesize) {
    GetCondition();
    if (subjectId == "") {
        alert("请选择项目");
        return false;
    }
    $.ajax({
        type: "post",
        url: "./handler/Export.ashx",
        data: {type:"getlist",currpage:pageindx,pagesize:pagesize, subjectids: subjectId, regions: region, province: province, city: city },
        cache: false,
        beforeSend: function () { $("#divload").show(); $("#noData").hide(); }, //发送数据之前
        complete: function () { $("#divload").hide(); }, //接收数据完毕
        success: function (data) {
           
            if (data != "") {
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

                    tr += "<tr class='tr_bai'>";
                    tr += "<td>" + rowIndex + "</td>";
                    //tr += "<td><input type='checkbox' name='cbOne' value=" + json[i].Id + " " + selected + "/></td>";
                    tr += "<td>" + json[i].CompanyName + "</td>";
                    tr += "<td>" + json[i].SubjectName + "</td>";
                    tr += "<td>" + json[i].GuidanceName + "</td>";
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
                    
                    tr += "</tr>";

                }
                $("#listBody").html(tr);

                var strJS = "pageOperate(" + pageindx + "," + pagesize + "," + total + ")";
                eval(strJS);
            }
            else {
                $("#noData").show();
                $("#listBody").html("");
                $("#popListTB").hide(); $("#Pagination").hide();
                $("#spanShopNum").html("");
                $("#shopNumDiv").hide();
            }
        }
    })
}

var scollHeight = 0;
function GetShopList(pageindx, pagesize) {
    scollHeight = $(document).scrollTop();
    var guidanceId = $("input[name='guidancerd']:checked").val() || "";
    var outsourceId = $("#ddlOutsource").val();
    var orderType = $("input[name='rblOrderType']:checked").val() || "";
   
    if (guidanceId == "") {
        alert("请选择活动");
        return false;
    }
    if (outsourceId == 0) {
        alert("请选择外协");
        return false;
    }
    if (orderType == "") {
        alert("请选择安装还是发货");
        return false;
    }
    $.ajax({
        type: "get",
        url: "handler/Export.ashx",
        data: { type: "getshoplist", currpage: pageindx, pagesize: pagesize, guidanceId: guidanceId, outsourceId: outsourceId, orderType: orderType },
        cache: false,
        beforeSend: function () { $("#divload").show(); $("#noData").hide(); }, //发送数据之前
        complete: function () { $("#divload").hide(); $(document).scrollTop(scollHeight); }, //接收数据完毕
        success: function (data) {
           
            if (data != "") {

                $("#popListTB").show(); $("#Pagination").show();
                var tr = "";
                var json = eval(data);
                var total = json[0].total;
                //$("#labShopCount").html(total);
                //$("#shopNumDiv").show();
                for (var i = 0; i < json.length; i++) {
                    var rowIndex = pageindx * pagesize + i + 1;

                    tr += "<tr class='tr_bai'>";
                    tr += "<td>" + rowIndex + "</td>";
                    tr += "<td>" + json[i].ShopNo + "</td>";
                    tr += "<td>" + json[i].ShopName + "</td>";
                    tr += "<td>" + json[i].Region + "</td>";
                    tr += "<td>" + json[i].Province + "</td>";
                    tr += "<td>" + json[i].City + "</td>";
                    tr += "<td>" + json[i].CityTier + "</td>";
                    tr += "<td>" + json[i].Format + "</td>";
                    tr += "<td>" + json[i].Address + "</td>";

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
    //$("#labShopCount").html("0");
    //$("#divClean").hide();
    $("#noData").show();
    $("#listBody").html("");
    $("#popListTB").hide();
    $("#Pagination").hide();
    //$("#spanShopNum").html("");
    //$("#shopNumDiv").hide();
}