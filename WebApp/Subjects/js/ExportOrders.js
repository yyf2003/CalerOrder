﻿
var subjectId = "";
var region = "";
var province = "";
var city = "";
var csUserId = ""; //客服userId
var isInstall = "";
var materialCategoryId = "";
//var materialIds = "";
var searchType = "";
var exportType = "";
var materialName = "";
var exportShopNo = "";
var isNotInclude = "";
$(function () {
    GetGuidance1();

    $("#ddlCustomer").change(function () {

        GetGuidance1();
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


    //查询上个月活动
    //    $("#spanUp").click(function () {
    //        var begin = $("#labBeginDate").text();
    //        var end = $("#labEndDate").text();
    //        begin = begin.replace(/-/g, "/");
    //        end = end.replace(/-/g, "/");
    //        var date = new Date(begin);

    //        var year = date.getFullYear();
    //        var month = date.getMonth(); //上个月
    //        if (month == 0) {
    //            month = 12;
    //            year--;
    //        }
    //        var upFirstDate = year + "-" + month + "-1";
    //        var date2 = new Date(year, month, 0);
    //        var upLastDate = year + "-" + month + "-" + date2.getDate();
    //        
    //        $("#labBeginDate").html(upFirstDate);
    //        $("#labEndDate").html(upLastDate);
    //        GetGuidance(upFirstDate, upLastDate);
    //    })

    //查询下个月活动
    //$("#spanDown").click(function () {
    //        var begin = $("#labBeginDate").text();
    //        var end = $("#labEndDate").text();
    //        begin = begin.replace(/-/g, "/");
    //        end = end.replace(/-/g, "/");
    //        var date = new Date(end);
    //        var year = date.getFullYear();
    //        var month = date.getMonth() + 2; //下个月
    //        if (month >= 13) {
    //            month = 1;
    //            year++;
    //        }
    //        var upFirstDate = year + "-" + month + "-1";
    //        var date2 = new Date(year, month, 0);
    //        var upLastDate = year + "-" + month + "-" + date2.getDate();
    //        $("#labBeginDate").html(upFirstDate);
    //        $("#labEndDate").html(upLastDate);
    //        GetGuidance(upFirstDate, upLastDate);
    // })

    $("input[name='selectSubjectType']").change(function () {
        //var val = $("input[name='selectSubjectType']:checked").val();
        var val = $(this).val();
        if (val == 1) {
            $("#shanghaiSubjects").show();
            $("#regionSubjects").hide();
            $("input[name='projectRegionCB'],#cbALL1").attr("checked", false);
        }
        else {
            $("#shanghaiSubjects").hide();
            $("#regionSubjects").show();
            $("input[name='projectCB'],#cbALL").attr("checked", false);
        }
    })

    //查询
    $("#btnSearch").click(function () {
        GetOrderDetail(currPage, pageSize);
    })

    //全选
    $("#cbALL").on("click", function () {
        var check = this.checked;
        $("input[name='projectCB']").each(function () {
            this.checked = check;
        })
        if (check) {
            //$(".loadingImg").show();
            GetRegions();
            GetProvince();
            GetCity();
            GetCustomerService();
            GetInstallLevel();
            GetMaterialCategory();
        }
        else {
            $(".contentDiv").html("");
        }
    })

    //全选
    $("#cbALL1").on("click", function () {
        var check = this.checked;
        $("input[name='projectRegionCB']").each(function () {
            this.checked = check;
        })
        if (check) {
            GetRegions();
            GetProvince();
            GetCity();
            GetCustomerService();
            GetInstallLevel();
            GetMaterialCategory();
        }
        else {
            $(".contentDiv").html("");
        }
    })


    $("#btnSearchSubject").on("click", function () {
        var begin = $.trim($("#txtBeginDate").val());
        var end = $.trim($("#txtEndDate").val());
        if (begin != "" && end != "") {
            $("#labBeginDate").html(begin);
            $("#labEndDate").html(end);
        }
        GetGuidance(begin, end);
    })


    $("#guidanceDiv").delegate("input[name='guidanceCB']", "change", function () {
        GetProjectList();
        GetRegionProjectList();
    })
    //选择区域
    $("#regionDiv").delegate("input[name='regionCB']", "change", function () {
        ScreenProject();

    })
    //选择活动类型
    $("#activityDiv").delegate("input[name='activityCB']", "change", function () {
        ScreenProject();
    })
    //选择项目类型
    $("#typeDiv").delegate("input[name='typeCB']", "change", function () {
        ScreenProject();
    })
    //选择项目
    $("#projectsDiv").delegate("input[name='projectCB']", "change", function () {
        //GetRegions();
        GetProvince();
        //GetCity();
        GetCustomerService();
        GetInstallLevel();
        GetMaterialCategory();
    })

    $("#projectsDiv1").delegate("input[name='projectRegionCB']", "change", function () {
        //GetRegions();
        GetProvince();
        //GetCity();
        GetCustomerService();
        GetInstallLevel();
        GetMaterialCategory();
    })

    $("#ProvinceDiv").delegate("input[name='provincecb']", "change", function () {
        GetCity();
        GetCustomerService();
        GetInstallLevel();
        GetMaterialCategory();
    })
    $("#CityDiv").delegate("input[name='citycb']", "change", function () {
        GetCustomerService();
        GetInstallLevel();
        GetMaterialCategory();
    })
    $("#CustomerServiceNameDiv").delegate("input[name='cscb']", "change", function () {
        GetInstallLevel();
        GetMaterialCategory();
    })
    $("#IsInstallDiv").delegate("input[name='installcb']", "change", function () {
        GetMaterialCategory();
    })
    $("#cbAllCity").change(function () {
        var checked = this.checked;
        $("input[name='citycb']").each(function () {
            this.checked = checked;
        })
    })

    //导出
    $("#btnExport").click(function () {

        GetCondition();
        if (subjectId == "") {
            alert("请选择项目");
            return false;
        }

        $(this).attr("disabled", true).next("img").show();
        checkExport($(this));
        //var url = "Handler/ExportOrders.ashx?type=export&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&city=" + city + "&materialIds=" + materialIds;
        var url = "/Subjects/ExportHelper.aspx?type=export&subjectids=" + subjectId + "&regions=" + region + "&province=" + province;

        $("#exportFrame").attr("src", url);
    })

    //导出350（非空）
    $("#btnExport350").click(function () {
        GetCondition();
        var btn = $(this);
        if (subjectId == "") {
            alert("请选择项目");
            return false;
        }
        var guidanceIds = "";
        $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        btn.attr("disabled", true).next("img").show();
        checkExport350(btn);
        var url = "/Subjects/ExportHelper.aspx?type=export350&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&customerService=" + csUserId + "&isInstall=" + isInstall + "&materialCategory=" + materialCategoryId;
        $("#exportFrame").attr("src", url);



    })

    //导出350总表（含空）
    $("#btnExport350WithEmpty").click(function () {
        GetCondition();
        var btn = $(this);
        if (subjectId == "") {
            alert("请选择项目");
            return false;
        }
        var guidanceIds = "";
        $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        $.ajax({
            type: "get",
            url: "/Subjects/Handler/ExportOrders.ashx?type=checkEmptyInfo&guidanceId=" + guidanceIds + "&region=" + region,
            success: function (data) {

                if (data == "ok") {
                    btn.attr("disabled", true).next("img").show();
                    checkExport350(btn);
                    //var url = "Handler/ExportOrders.ashx?type=export350&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&city=" + city + "&materialIds=" + materialIds;
                    var url = "/Subjects/ExportHelper.aspx?type=export350withempty&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&customerService=" + csUserId + "&isInstall=" + isInstall + "&materialCategory=" + materialCategoryId;
                    $("#exportFrame").attr("src", url);
                }
                else {
                    layer.open({
                        title: '警 告',
                        type: 2,
                        time: 0,
                        skin: 'layui-layer-rim', //加上边框
                        area: ['520px', '320px'], //宽高
                        content: '/Subjects/EmptyDateWarn.aspx?guidanceId=' + guidanceIds
                    })
                }
            }
        })


    })

    //导出喷绘王模板-北京
    $("#btnPHWbj").click(function () {
        GetCondition();
        var selectSubjectType = $("input[name='selectSubjectType']:checked").val();
        var ids = "";
        if (selectSubjectType == 1) {
            $("input[name='projectCB']:checked").each(function () {
                ids += $(this).val() + ",";
            })
        }
        else {
            $("input[name='projectRegionCB']:checked").each(function () {
                ids += $(this).val() + ",";
            })
        }
        if (ids == "") {
            alert("请选择项目");
            return false;
        }

        $(this).attr("disabled", true).next("img").show();
        checkExportBJPHW($(this));
        //var url = "Handler/ExportOrders.ashx?type=exportbjphw&subjectids=" + subjectId;
        var url = "/Subjects/ExportHelper.aspx?type=exportbjphw&subjectids=" + ids + "&regions=" + region + "&province=" + province + "&customerService=" + csUserId + "&isInstall=" + isInstall + "&materialCategory=" + materialCategoryId + "&exportShopNo=" + exportShopNo + "&isNotInclude=" + isNotInclude + "&exportType=" + exportType + "&materialName=" + materialName + "&selecttype=" + selectSubjectType;

        $("#exportFrame").attr("src", url);

    })

    //导出喷绘王模板-外协
    $("#btnPHWwx").click(function () {
        GetCondition();
        var selectSubjectType = $("input[name='selectSubjectType']:checked").val();
        var ids = "";
        if (selectSubjectType == 1) {
            $("input[name='projectCB']:checked").each(function () {
                ids += $(this).val() + ",";
            })
        }
        else {
            $("input[name='projectRegionCB']:checked").each(function () {
                ids += $(this).val() + ",";
            })
        }
        if (ids == "") {
            alert("请选择项目");
            return false;
        }

        $(this).attr("disabled", true).next("img").show();
        checkExportOtherPHW($(this));
        //var url = "Handler/ExportOrders.ashx?type=exportotherphw&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&city=" + city;
        var url = "/Subjects/ExportHelper.aspx?type=exportotherphw&subjectids=" + ids + "&regions=" + region + "&province=" + province + "&customerService=" + csUserId + "&isInstall=" + isInstall + "&materialCategory=" + materialCategoryId + "&exportShopNo=" + exportShopNo + "&isNotInclude=" + isNotInclude + "&materialName=" + materialName + "&exportType=" + exportType + "&selecttype=" + selectSubjectType;

        $("#exportFrame").attr("src", url);
    })

    //导出报价单
    $("#btnExportQuote350").click(function () {
        GetCondition();
        var btn = $(this);
        if (subjectId == "") {
            alert("请选择项目");
            return false;
        }
        var guidanceIds = "";
        $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })

        btn.attr("disabled", true).next("img").show();
        checkExportQuote350(btn);
        //var url = "Handler/ExportOrders.ashx?type=export350&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&city=" + city + "&materialIds=" + materialIds;
        var url = "/Subjects/ExportHelper.aspx?type=exportQuote350&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&customerService=" + csUserId + "&isInstall=" + isInstall + "&materialCategory=" + materialCategoryId;
        $("#exportFrame").attr("src", url);
    })

    //导出装箱单
    $("#btnExportPackingList").click(function () {

        GetCondition();
        var btn = $(this);
        if (subjectId == "") {
            alert("请选择项目");
            return false;
        }
        var guidanceIds = "";
        $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        //btn.attr("disabled", true).next("img").show();
        //checkExport350(btn);
        //var url = "Handler/ExportOrders.ashx?type=export350&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&city=" + city + "&materialIds=" + materialIds;
        var url = "/Subjects/ExportHelper.aspx?type=exportPackingList&subjectids=" + subjectId + "&regions=" + region + "&province=" + province + "&customerService=" + csUserId + "&isInstall=" + isInstall + "&materialCategory=" + materialCategoryId;
        $("#exportFrame").attr("src", url);


    })

    //导出系统订单（350）
    $("#btnExportNew350").click(function () {
        GetCondition();
        var selectSubjectType = $("input[name='selectSubjectType']:checked").val();
        var ids = "";
        if (selectSubjectType == 1) {
            $("input[name='projectCB']:checked").each(function () {
                ids += $(this).val() + ",";
            })
        }
        else {
            $("input[name='projectRegionCB']:checked").each(function () {
                ids += $(this).val() + ",";
            })
        }
        var btn = $(this);
        if (ids == "") {
            alert("请选择项目");
            return false;
        }
        var guidanceIds = "";
        $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        btn.attr("disabled", true).next("img").show();
        checkExportNew350(btn);
        var url = "/Subjects/ExportHelper.aspx?type=exportNew350&subjectids=" + ids + "&regions=" + region + "&province=" + province + "&customerService=" + csUserId + "&isInstall=" + isInstall + "&materialCategory=" + materialCategoryId + "&exportType=" + exportType + "&exportShopNo=" + exportShopNo + "&isNotInclude=" + isNotInclude + "&materialName=" + materialName + "&selecttype=" + selectSubjectType;

        $("#exportFrame").attr("src", url);
    })

    $("input[name^='cblExportType']").change(function () {

        if (this.checked) {

            $(this).siblings("input").each(function () {
                this.checked = false;
            });
        }
    })
    $("input[name^='cblMaterial']").change(function () {

        if (this.checked) {
            $(this).siblings("input").each(function () {
                this.checked = false;
            });
        }
    })

})

var timer;
function checkExport(obj) {
    timer = setInterval(function () {
        $.ajax({
            type: "get",
            url: "Handler/CheckExport.ashx",
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

var timer1;
function checkExport350(obj) {
    timer1 = setInterval(function () {
        $.ajax({
            type: "get",
            url: "Handler/CheckExport350.ashx",
            cache: false,
            success: function (data) {
                if (data == "empty") {
                    alert("没有数据可以导出！");
                    $(obj).attr("disabled", false).next("img").hide();
                    clearInterval(timer1);
                    $("#exportFrame").attr("src", "");
                }
                else
                    if (data == "ok") {
                        $(obj).attr("disabled", false).next("img").hide();
                        clearInterval(timer1);
                        $("#exportFrame").attr("src", "");
                    }

            }
        })

    }, 1000);
}

var timer2;
function checkExportBJPHW(obj) {
    timer2 = setInterval(function () {
        $.ajax({
            type: "get",
            url: "Handler/CheckExportPHWBJ.ashx",
            cache: false,
            success: function (data) {

                if (data == "empty") {
                    clearInterval(timer2);
                    alert("没有数据可以导出！");
                    $(obj).attr("disabled", false).next("img").hide();

                    $("#exportFrame").attr("src", "");
                }
                else if (data == "notFinishInstallPrice") {
                    clearInterval(timer2);
                    alert("请提交完安装费后再导出！");
                    $(obj).attr("disabled", false).next("img").hide();
                    $("#exportFrame").attr("src", "");
                }
                else if (data == "ok") {
                    $(obj).attr("disabled", false).next("img").hide();
                    clearInterval(timer2);
                    $("#exportFrame").attr("src", "");
                }
            }
        })

    }, 1000);
}

var timer3;
function checkExportOtherPHW(obj) {
    timer3 = setInterval(function () {
        $.ajax({
            type: "get",
            url: "Handler/CheckExportPHWOther.ashx",
            cache: false,
            success: function (data) {
                if (data == "empty") {
                    clearInterval(timer3);
                    alert("没有数据可以导出！");
                    $(obj).attr("disabled", false).next("img").hide();
                    $("#exportFrame").attr("src", "");
                }
                else if (data == "notFinishInstallPrice") {
                    clearInterval(timer3);
                    alert("请提交完安装费后再导出！");
                    $(obj).attr("disabled", false).next("img").hide();
                    $("#exportFrame").attr("src", "");
                }
                else if (data == "ok") {
                    $(obj).attr("disabled", false).next("img").hide();
                    clearInterval(timer3);
                    $("#exportFrame").attr("src", "");
                }

            }
        })

    }, 1000);
}

var timer4;
function checkExportQuote350(obj) {
    timer4 = setInterval(function () {
        $.ajax({
            type: "get",
            url: "Handler/CheckExportQuote350.ashx",
            cache: false,
            success: function (data) {

                if (data == "empty") {
                    clearInterval(timer4);
                    alert("没有数据可以导出！");
                    $(obj).attr("disabled", false).next("img").hide();

                    $("#exportFrame").attr("src", "");
                }
                else if (data == "ok") {
                    $(obj).attr("disabled", false).next("img").hide();
                    clearInterval(timer4);
                    $("#exportFrame").attr("src", "");
                }
            }
        })

    }, 1000);
}


var timer5;
function checkExportNew350(obj) {
    timer5 = setInterval(function () {
        $.ajax({
            type: "get",
            url: "Handler/CheckExportNew350.ashx",
            cache: false,
            success: function (data) {

                if (data == "empty") {
                    clearInterval(timer5);
                    alert("没有数据可以导出！");
                    $(obj).attr("disabled", false).next("img").hide();

                    $("#exportFrame").attr("src", "");
                }
                else if (data == "ok") {
                    $(obj).attr("disabled", false).next("img").hide();
                    clearInterval(timer5);
                    $("#exportFrame").attr("src", "");
                }
            }
        })

    }, 1000);
}



function GetCondition() {
    subjectId = "";
    $("#hfCityData").val("");
    $("input[name='projectCB']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    region = "";
    $("input[name='regionCB']:checked").each(function () {
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



    csUserId = "";
    $("input[name='cscb']:checked").each(function () {
        csUserId += $(this).val() + ",";
    })
    isInstall = "";
    $("input[name='installcb']:checked").each(function () {
        isInstall += $(this).val() + ",";
    })
    materialCategoryId = "";
    $("input[name='mcategorycb']:checked").each(function () {
        materialCategoryId += $(this).val() + ",";
    })
    exportType = "";
    exportType = $("input[name^='cblExportType']:checked").val() || "";
    materialName = "";
    materialName = $("input[name^='cblMaterial']:checked").val() || "";
    exportShopNo = "";
    exportShopNo = $.trim($("#txtExportShopNo").val());
    isNotInclude = "";
    if ($("#cbNotInclude").attr("checked") == "checked") {
        isNotInclude = "1";
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
        $("#hfCityData").val(city);
    }
    if (csUserId.length > 0) {
        csUserId = csUserId.substring(0, csUserId.length - 1);
    }
    if (isInstall.length > 0) {
        isInstall = isInstall.substring(0, isInstall.length - 1);
    }
    if (materialCategoryId.length > 0) {
        materialCategoryId = materialCategoryId.substring(0, materialCategoryId.length - 1);
    }

}

function GetGuidance1() {
    var customerId = $("#ddlCustomer").val() || "0";
    customerId = customerId == 0 ? -1 : customerId;
    var guidanceMonth = $.trim($("#txtMonth").val());
    $("#guidanceDiv").html("");
    $("#regionDiv").html("");
    $("#projectsDiv").html("");
    $("#ProvinceDiv").html("");
    $("#CityDiv").html("");
    $("#cbAllDiv").hide();
    $("#activityDiv").html("");
    $("#typeDiv").html("");
    $(".trType").hide();
    $("#CustomerServiceNameDiv").html("");
    $("#IsInstallDiv").html("");
    $("#MaterialCategoryDiv").html("");
    $("#labOrderCount").html("0");
    $("#labShopCount").html("0");
    $("#loadGuidanceImg").show();
    $.ajax({
        type: "get",
        url: "Handler/CheckOrder.ashx",
        data: { type: "getGuidance", customerId: customerId,guidanceMonth:guidanceMonth },
        cache: false,
        success: function (data) {
            $("#guidanceDiv").html("");
            $("#loadGuidanceImg").hide();
            
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

function GetGuidance() {
    var customerId = $("#ddlCustomer").val() || "0";
//    if (!begin && !end) {
//         begin = $.trim($("#txtBeginDate").val());
//         end = $.trim($("#txtEndDate").val());
//    }

    var guidanceMonth = $.trim($("#txtMonth").val());

    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
//    if (begin == "" || end == "") {
//        alert("请选择开始时间和结束时间");
//        return false;
//    }
//    if (begin != "" && end != "") {

        $("#guidanceDiv").html("");
        $("#regionDiv").html("");
        $("#projectsDiv").html("");
        $("#ProvinceDiv").html("");
        $("#CityDiv").html("");
        $("#cbAllDiv").hide();
        $("#activityDiv").html("");
        $("#typeDiv").html("");
        $(".trType").hide();
        $("#CustomerServiceNameDiv").html("");
        $("#IsInstallDiv").html("");
        $("#MaterialCategoryDiv").html("");
        $("#labOrderCount").html("0");
        $("#labShopCount").html("0");
        $("#loadGuidanceImg").show();
        $.ajax({
            type: "get",
            url: "Handler/CheckOrder.ashx",
            //data: { type: "getGuidance", customerId: customerId, beginDate: begin, endDate: end },
            data: { type: "getGuidance", customerId: customerId, guidanceMonth:guidanceMonth },
            cache: false,
            success: function (data) {
                $("#loadGuidanceImg").hide();
                $("#guidanceDiv").html("");
                if (data != "") {
                    var json = eval(data);
                    $("#imgLoading1").hide();

                    for (var i = 0; i < json.length; i++) {

                        var div = "<div style='float:left;'><input type='checkbox' name='guidanceCB' value='" + json[i].Id + "' /><span>" + json[i].GuidanceName + "</span>&nbsp;</div>";
                        $("#guidanceDiv").append(div);
                    }
                }
                else {
                    $("#guidanceDiv").html("<span style='color:red;'>无活动信息！</span>");
                }
            }
        })

    //}
}

function GetProjectList() {
    $("#loadSubjectImg").show();
    //var guidanceId = $("#selectGuidance").val() || 0;
    var guidanceIds = "";
    $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    $("#regionDiv").html("");
    $("#projectsDiv").html("");
    $("#activityDiv").html("");
    $("#typeDiv").html("");
    $(".trType").hide();
    $(".contentDiv").html("");
    $("#cbALL").attr("checked", false);
    $.ajax({
        type: "get",
        url: "Handler/CheckOrder.ashx",
        data: { type: "getProjectList", guidanceIds: guidanceIds },
        success: function (data) {
            $("#loadSubjectImg").hide();
            if (data != "") {
                var projectStr = data.split('|')[0];
                var typeStr = data.split('|')[1];
                var activityStr = data.split('|')[2];
                var regionStr = data.split('|')[3];
                if (regionStr != "") {
                    var json01 = eval(regionStr);
                    for (var i = 0; i < json01.length; i++) {
                        var div = "<div style='float:left;'><input type='checkbox' name='regionCB' value='" + json01[i].RegionName + "' /><span>" + json01[i].RegionName + "</span>&nbsp;</div>";
                        $("#regionDiv").append(div);
                    }
                }
                if (activityStr != "") {
                    var json0 = eval(activityStr);
                    for (var i = 0; i < json0.length; i++) {
                        var div = "<div style='float:left;'><input type='checkbox' name='activityCB' value='" + json0[i].Id + "' /><span>" + json0[i].ActivityName + "</span>&nbsp;</div>";
                        $("#activityDiv").append(div);
                    }
                }
                if (typeStr != "") {
                    var json = eval(typeStr);
                    for (var i = 0; i < json.length; i++) {
                        var div = "<div style='float:left;'><input type='checkbox' name='typeCB' value='" + json[i].Id + "' /><span>" + json[i].TypeName + "</span>&nbsp;</div>";
                        $("#typeDiv").append(div);
                    }
                }
                if (projectStr != "") {
                    $(".trType").show();
                    var json1 = eval(projectStr);
                    for (var i = 0; i < json1.length; i++) {
                        var div = "<div style='float:left;'><input type='checkbox' name='projectCB' value='" + json1[i].Id + "' /><span>" + json1[i].SubjectName + "</span>&nbsp;</div>";
                        $("#projectsDiv").append(div);
                    }
                }
            }

        }
    })
}

function GetRegionProjectList() {
    var guidanceIds0 = "";
    $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
        guidanceIds0 += $(this).val() + ",";
    })
    $("#cbALL1").attr("checked", false);
    $("#projectsDiv1").html("");
    $(".trType1").hide();
    $.ajax({
        type: "get",
        url: "Handler/CheckOrder.ashx",
        data: { type: "getRegionProjectList", guidanceIds: guidanceIds0 },
        success: function (data) {

            if (data != "") {
                $(".trType1").show();
                var json1 = eval(data);
                for (var i = 0; i < json1.length; i++) {
                    var div = "<div style='float:left;'><input type='checkbox' name='projectRegionCB' value='" + json1[i].Id + "' /><span>" + json1[i].SubjectName + "</span>&nbsp;</div>";
                    $("#projectsDiv1").append(div);
                }
            }

        }
    })
}

function ScreenProject() {
    $("#projectsDiv").html("");
    $(".contentDiv").html("");
    $("#cbALL").attr("checked", false);
    //var guidanceId = $("#selectGuidance").val() || 0;
    var guidanceIds = "";
    $("#loadSubjectImg").show();
    $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    var regions = "";
    var activityIds = "";
    var typeIds = "";
    $("input[name='regionCB']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    $("input[name='activityCB']:checked").each(function () {
        activityIds += $(this).val() + ",";
    })
    $("input[name='typeCB']:checked").each(function () {
        typeIds += $(this).val() + ",";
    })

    $.ajax({
        type: "get",
        url: "Handler/CheckOrder.ashx",
        data: { type: "screenProject", guidanceIds: guidanceIds,region:regions, activityId: activityIds, typeId: typeIds },
        success: function (data) {
            $("#loadSubjectImg").hide();
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
            url: "Handler/CheckOrder.ashx",
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
    var selectSubjectType = $("input[name='selectSubjectType']:checked").val();
    var ids = "";
    if (selectSubjectType == 1) {
        $("input[name='projectCB']:checked").each(function () {
            ids += $(this).val() + ",";
        })
    }
    else {
        $("input[name='projectRegionCB']:checked").each(function () {
            ids += $(this).val() + ",";
        })
    }
    $("#RegionDiv").html("");
    if (ids.length > 0) {
        ids = ids.substring(0, ids.length - 1);
        $("#regionLoadImg").show();
        $.ajax({
            type: "get",
            url: "Handler/ExportOrders.ashx?type=getregion&subjectids=" + ids + "&selecttype=" + selectSubjectType,
            cache: false,
            success: function (data) {
                
                $("#regionLoadImg").hide();
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

    var regions = "";
    $("input[name='regionCB']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var sds = "";
    var selectSubjectType = $("input[name='selectSubjectType']:checked").val();
    if (selectSubjectType == 1) {
        $("input[name='projectCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    else {
        $("input[name='projectRegionCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    $("#provinceLoadImg").show();
    $("#ProvinceDiv").html("");
    $("#CityDiv").html("");
    $("#cbAllDiv").hide();
    $("#cbAllCity").attr("checked", false);
    regions = regions.substring(0, regions.length - 1);
    sds = sds.substring(0, sds.length - 1);
    $.ajax({
        type: "get",
        url: "Handler/ExportOrders.ashx?type=getprovince&regions=" + regions + "&subjectids=" + sds + "&selecttype=" + selectSubjectType,
        cache: false,
        success: function (data) {
            $("#provinceLoadImg").hide();
            
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



//    if (regions.length > 0) {
//       
//    }
//    else
//        $("#ProvinceDiv").html("");
}

function GetCity() {
    $("#cbAllDiv").hide();
    $("#cbAllCity").attr("checked", false);
    $("#CityDiv").html("");
    var sds = "";
    var selectSubjectType = $("input[name='selectSubjectType']:checked").val();
    if (selectSubjectType == 1) {
        $("input[name='projectCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    else {
        $("input[name='projectRegionCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    var regions = "";
    $("input[name='regionCB']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    var provinces = "";
    $("input[name='provincecb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    if (provinces.length > 0) {
        $("#cityLoadImg").show();
        sds = sds.substring(0, sds.length - 1);
        regions = regions.substring(0, regions.length - 1);
        provinces = provinces.substring(0, provinces.length - 1);
        $.ajax({
            type: "get",
            url: "Handler/ExportOrders.ashx?type=getcity&regions=" + regions + "&subjectids=" + sds + "&province=" + escape(provinces) + "&selecttype=" + selectSubjectType,
            cache: false,
            success: function (data) {
                $("#cityLoadImg").hide();
                
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

    GetCondition();
    var sds = "";
    var selectSubjectType = $("input[name='selectSubjectType']:checked").val();
    if (selectSubjectType == 1) {
        $("input[name='projectCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    else {
        $("input[name='projectRegionCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    if (sds.length > 0) {
        $("#csLoadImg").show();
        $.ajax({
            type: "get",
            url: "Handler/ExportOrders.ashx?type=getCS&regions=" + region + "&subjectids=" + sds + "&province=" + escape(province) + "&city=" + escape(city) + "&selecttype=" + selectSubjectType,
            cache: false,
            success: function (data) {
                $("#csLoadImg").hide();
                $("#CustomerServiceNameDiv").html("");
                if (data != "") {
                    var json = eval(data);
                    var div = "";

                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        if (json[i].UserId == userId)
                            checked = 'checked="checked"';
                        div += "<div style='float:left;'><input type='checkbox' name='cscb' value='" + json[i].UserId + "' " + checked + "/><span>" + json[i].UserName + "</span>&nbsp;</div>";

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

function GetInstallLevel() {
    GetCondition();
    var sds = "";
    var selectSubjectType = $("input[name='selectSubjectType']:checked").val();
    if (selectSubjectType == 1) {
        $("input[name='projectCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    else {
        $("input[name='projectRegionCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    if (sds.length > 0) {
        $("#installLoadImg").show();
        $.ajax({
            type: "get",
            url: "Handler/ExportOrders.ashx?type=getInstallLevel&regions=" + region + "&subjectids=" + sds + "&province=" + escape(province) + "&city=" + escape(city) + "&customerService=" + csUserId + "&selecttype=" + selectSubjectType,
            cache: false,
            success: function (data) {
                $("#installLoadImg").hide();
                $("#IsInstallDiv").html("");
                if (data != "") {
                    var json = eval(data);
                    var div = "";
                    for (var i = 0; i < json.length; i++) {
                        div += "<div style='float:left;'><input type='checkbox' name='installcb' value='" + json[i].IsInstall + "' /><span>" + json[i].IsInstall + "</span>&nbsp;</div>";

                    }
                    
                    $("#IsInstallDiv").html(div);

                }
            }
        })
    }
    else {
        $("#IsInstallDiv").html("");
    }
}

function GetMaterialCategory() {
    GetCondition();
    var sds = "";
    var selectSubjectType = $("input[name='selectSubjectType']:checked").val();
    if (selectSubjectType == 1) {
        $("input[name='projectCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    else {
        $("input[name='projectRegionCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    $("#labOrderCount").html("0");
    $("#labShopCount").html("0");
    if (sds.length > 0) {
        $("#mcLoadImg").show();
        $.ajax({
            type: "get",
            url: "Handler/ExportOrders.ashx?type=getMaterialCategory&regions=" + region + "&subjectids=" + sds + "&province=" + escape(province) + "&city=" + escape(city) + "&customerService=" + csUserId + "&isInstall=" + isInstall + "&selecttype=" + selectSubjectType,
            cache: false,
            success: function (data) {
                
                $("#mcLoadImg").hide();
                $("#MaterialCategoryDiv").html("");
                if (data != "") {
                    var json = eval(data);
                    var rows = json[0].rows;
                    $("#labOrderCount").html(json[0].OrderCount);
                    $("#labShopCount").html(json[0].ShopCount);
                    var div = "";
                    for (var i = 0; i < rows.length; i++) {
                        div += "<div style='float:left;'><input type='checkbox' name='mcategorycb' value='" + rows[i].CategoryId + "' /><span>" + rows[i].CategoryName + "</span>&nbsp;</div>";

                    }

                    $("#MaterialCategoryDiv").html(div);

                }
            }
        })
    }
    else {
        $("#MaterialCategoryDiv").html("");
    }
}

function GetOrderCount() { 
   
}

function getMonth() {
    GetGuidance1();
}

//分页
var pageSize = 20;
var currPage = 0;
function GetOrderDetail(pageindx, pagesize) {
    GetCondition();
    var sds = "";
    var selectSubjectType = $("input[name='selectSubjectType']:checked").val()||1;
    if (selectSubjectType == 1) {
        $("input[name='projectCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    else {
        $("input[name='projectRegionCB']:checked").each(function () {
            sds += $(this).val() + ",";
        })
    }
    if (sds == "") {
        alert("请选择项目");
        return false;
    }
    $.ajax({
        type: "get",
        url: "Handler/ExportOrders.ashx?type=getlist&currpage=" + pageindx + "&pagesize=" + pagesize,

        data: { subjectids: sds, regions: region, province: province, city: city, customerService: csUserId, isInstall: isInstall, materialCategory: materialCategoryId, selecttype: selectSubjectType },
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
                    tr += "<tr class='tr_bai'>";
                    tr += "<td>" + rowIndex + "</td>";
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
                    //tr += "<td>" + json[i].GraphicNo + "</td>";
                    //                    tr += "<td>" + json[i].POPName + "</td>";
                    //                    tr += "<td>" + json[i].POPType + "</td>";
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
                $("#noData").show();
                $("#listBody").html("");
                $("#popListTB").hide(); $("#Pagination").hide();
                $("#spanShopNum").html("");
                $("#shopNumDiv").hide();
            }
        }
    })
}

function pageselectCallback(page_id) {
    GetOrderDetail(page_id, pageSize);
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

function GetMaterials() {
    var begin = $.trim($("#txtBeginDate").val());
    var end = $.trim($("#txtEndDate").val());
    var customerId = $("#ddlCustomer").val() || "0";
    if (begin != "" && end != "") {

        $.ajax({
            type: "get",
            url: "Handler/CheckOrder.ashx",
            data: { type: "getMaterials", beginDate: begin, endDate: end, customerId: customerId },
            cache: false,
            success: function (data) {
                $("#MaterialsDiv").html("");

                if (data != "") {

                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var div = "<div style='float:left;'><input type='checkbox' name='materialCB' value='" + json[i].Id + "' /><span>" + json[i].Name + "</span>&nbsp;</div>";
                        $("#MaterialsDiv").append(div);
                    }
                }

            }

        })
    }
}

function CheckEmptyInfo() {
    var guidanceIds = "";
    var regions = "";
    $("#guidanceDiv").find("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    $("input[name='regioncb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
}