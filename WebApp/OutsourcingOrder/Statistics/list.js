
var customerId = 0;
var guidanceMonth = "";
var currOutsourceId = "";
var currOutsourceName = "";
var guidanceId = "";
var subjectId = "";
var province = "";
var city = "";
var assignType = "";
$(function () {
    GetOutsourceList();

    $("#ddlCustomer").change(function () {
        GetGuidacneList();
    })

    $("#guidanceDiv").delegate("input[name='guidanceCB']", "change", function () {
        GetSubjectList();
        GetProvince();
    })

    $("#subjectDiv").delegate("input[name='subjectcb']", "change", function () {
        GetProvince();
    })

    $("#guidanceCBALL").click(function () {
        var checked = this.checked;
        $("input[name='guidanceCB']").each(function () {
            this.checked = checked;
        })
        GetSubjectList();
        GetProvince();
    })

    $("#subjectCBALL").click(function () {
        var checked = this.checked;
        $("input[name='subjectcb']").each(function () {
            this.checked = checked;
        })

        GetProvince();
    })



    $("#provinceDiv").delegate("input[name='provinceCB']", "change", function () {
        GetCity();
    })

    $("#btnSearch").click(function () {
        GetCondition();
        var subjectIds = subjectId;
        var selectedLen = $("input[name='subjectcb']:checked").length;
        var totalLen = $("input[name='subjectcb']").length;
        if (selectedLen == totalLen)
            subjectIds = "";
        $.ajax({
            type: "post",
            url: "ListHandler.ashx",
            data: { outsourceId: currOutsourceId, guidanceMonth: guidanceMonth, guidanceId: guidanceId, subjectId: subjectIds, province: province, city: city, assignType: assignType },
            beforeSend: function () { $("#loadingImg").show(); },
            complete: function () { $("#loadingImg").hide(); },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    $("#labShopCount").html(json[0].ShopCount);
                    var labArea = "0";
                    if (json[0].TotalArea > 0)
                        labArea = json[0].TotalArea;
                    $("#labArea").html(labArea + " 平米");

                    var labPOPPrice = "0";
                    if (json[0].POPPrice > 0) {
                        labPOPPrice = "<a href='javascript:void(0)' onclick='CheckPOPPrice()' style='text-decoration:underline;color:blue;'>" + json[0].POPPrice + "</a>";
                    } $("#labPOPPrice").html(labPOPPrice);

                    var labInstallPrice = "0";
                    if (json[0].InstallPrice > 0) {
                        labInstallPrice = "<a href='javascript:void(0)' onclick='CheckInstallPrice()' style='text-decoration:underline;color:blue;'>" + json[0].InstallPrice + "</a>";
                    }
                    $("#labInstall").html(labInstallPrice);
                    $("#labMeasurePrice").html(json[0].MeasurePrice);
                    var otherPrice = "0";
                    if (json[0].OtherPrice > 0) {

                        otherPrice = "<a href='javascript:void(0)' onclick='CheckOtherPrice()' style='text-decoration:underline;color:blue;'>" + json[0].OtherPrice + "</a>";
                    }
                    $("#labOtherPrice").html(otherPrice);

                    var rotherPrice = "0";
                    if (json[0].ReceiveOtherPrice > 0) {

                        rotherPrice = "<a href='javascript:void(0)' onclick='CheckOtherPrice()' style='text-decoration:underline;color:blue;'>" + json[0].ReceiveOtherPrice + "</a>";
                    }
                    $("#labROtherPrice").html(rotherPrice);


                    $("#labTotalPrice").html(json[0].TotalPrice);

                    var expressPriceTxt="0";
                    if(json[0].ExpressPrice>0)
                    {
                       expressPriceTxt="<a href='javascript:void(0)' onclick='CheckExpressPrice()' style='text-decoration:underline;color:blue;'>" + json[0].ExpressPrice + "</a>";
                    }
                    $("#labExpressPrice").html(expressPriceTxt);

                    $("#labRPOPPrice").html(json[0].ReceivePOPPrice);
                    var labRInstallPrice = "0";
                    if (json[0].ReceiveInstallPrice > 0) {
                        labRInstallPrice = "<a href='javascript:void(0)' onclick='CheckInstallPrice()' style='text-decoration:underline;color:blue;'>" + json[0].ReceiveInstallPrice + "</a>";
                    }
                    $("#labRInstall").html(labRInstallPrice);
                    $("#labRMeasurePrice").html(json[0].ReceiveMeasurePrice);

                    $("#labRTotalPrice").html(json[0].ReceiveTotalPrice);
                    $("#labRExpressPrice").html(json[0].ReceiveExpressPrice);
                }
                else {
                    init();
                }
            }
        });
    })


    $("#btnExportDetail").click(function () {
        if (currOutsourceId == 0) {
            alert("请选择外协");
            return false;
        }
        var guidanceId = "";
        $("input[name='guidanceCB']:checked").each(function () {
            guidanceId += $(this).val() + ",";
        })
        if (guidanceId == "") {
            $("input[name='guidanceCB']").each(function () {
                guidanceId += $(this).val() + ",";
            })
        }
        if (guidanceId == "") {
            alert("请选择活动");
            return false;
        }
        var subjectId = "";
        $("input[name='subjectcb']:checked").each(function () {
            subjectId += $(this).val() + ",";
        })

        var province = "";
        $("input[name='provinceCB']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cityCB']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var assignType = "";
        $("input[name^='cblOutsourceType']:checked").each(function () {
            assignType += $(this).val() + ",";
        })
        if (guidanceId != "") {
            guidanceId = guidanceId.substring(0, guidanceId.length - 1);
        }
        if (subjectId != "") {
            subjectId = subjectId.substring(0, subjectId.length - 1);
        }
        if (province != "") {
            province = province.substring(0, province.length - 1);
        }
        if (city != "") {
            city = city.substring(0, city.length - 1);
        }
        if (assignType != "") {
            assignType = assignType.substring(0, assignType.length - 1);
        }
        var selectedLen = $("input[name='subjectcb']:checked").length;
        var totalLen = $("input[name='subjectcb']").length;
        if (selectedLen == totalLen)
            subjectId = "";

        var url = "ExportHelper.aspx?outsourceId=" + currOutsourceId + "&guidanceIds=" + guidanceId + "&subjectIds=" + subjectId + "&outsourceType=" + assignType + "&province=" + province + "&city=" + city + "&outsourceName=" + currOutsourceName;
        $("#exportFrame").attr("src", url);
    })

    $("#btnGetProvince").click(function () {
        GetProvinceByDate();
    })

    $("#provinceDiv1").delegate("input[name='provinceCB1']", "change", function () {
        GetCityByDate();
    })

    $("#btnSearch1").click(function () {
        SearchByDate();
    })

    $("#btnExportDetail1").click(function () {
        var customerId = $("#ddlCustomer1").val();
        var beginDate = $.trim($("#txtBeginDate1").val());
        var endDate = $.trim($("#txtEndDate1").val());
        if (currOutsourceId == "") {
            layer.msg("请选择外协");
            return false;
        }
        if (beginDate == "") {
            layer.msg("请输入开始时间");
            return false;
        }
        if (endDate == "") {
            layer.msg("请输入结束时间");
            return false;
        }
        var province1 = "";
        $("input[name='provinceCB1']:checked").each(function () {
            province1 += $(this).val() + ",";
        })
        var city1 = "";
        $("input[name='cityCB1']:checked").each(function () {
            city1 += $(this).val() + ",";
        })
        var assignType1 = "";
        $("input[name^='cblOutsourceType1']:checked").each(function () {
            assignType1 += $(this).val() + ",";
        })
        var url = "ExportHelper.aspx?type=bydate&customerId=" + customerId + "&outsourceId=" + currOutsourceId + "&outsourceType=" + assignType + "&province=" + province1 + "&city=" + city1 + "&outsourceName=" + currOutsourceName + "&beginDate=" + beginDate + "&endDate=" + endDate;
        $("#exportFrame").attr("src", url);
    })
})

function init() {
    $("#labShopCount").html("0");
    $("#labArea").html("0");
    $("#labPOPPrice").html("0");
    $("#labInstall").html("0");
    $("#labMeasurePrice").html("0");
    $("#labOtherPrice").html("0");
    $("#labTotalPrice").html("0");
    $("#labExpressPrice").html("0");
    $("#labRPOPPrice").html("0");
    $("#labRInstall").html("0");
    $("#labRMeasurePrice").html("0");
    $("#labROtherPrice").html("0");
    $("#labRTotalPrice").html("0");
    $("#labRExpressPrice").html("0");
}

function init1() {
    $("#labShopCount1").html("0");
    $("#labArea1").html("0");
    $("#labPOPPrice1").html("0");
    $("#labInstall1").html("0");
    $("#labMeasurePrice1").html("0");
    $("#labOtherPrice1").html("0");
    $("#labTotalPrice1").html("0");
    $("#labExpressPrice1").html("0");
    $("#labRPOPPrice1").html("0");
    $("#labRInstall1").html("0");
    $("#labRMeasurePrice1").html("0");
    $("#labROtherPrice1").html("0");
    $("#labRTotalPrice1").html("0");
    $("#labRExpressPrice1").html("0");
}

function GetOutsourceList() {

    $("#tbOutsource").datagrid({
        method: 'get',
        url: '../handler/OrderList.ashx?type=getOutsource',
        columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'CompanyName', title: '外协名称' }
                ]],
        height: '100%',
        border: false,
        fitColumns: true,
        singleSelect: true,
        rownumbers: true,
        emptyMsg: '没有相关记录',
        onClickRow: function (rowIndex, data) {
            //selectOutsource(data.Id, data.CompanyName);
            selectOutsource();
        }
    })
}

function GetGuidacneList() {

    $("#provinceDiv").html("");
    $("#cityDiv").html("");

    var customerId = $("#ddlCustomer").val();
    var guidanceMonth = $.trim($("#txtMonth").val());

    $.ajax({
        type: "get",
        url: "../handler/OrderList.ashx",
        data: { type: "getGuidanceList", outsourceId: currOutsourceId, customerId: customerId, guidanceMonth: guidanceMonth },
        success: function (data) {
            $("#guidanceDiv").html("");
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left;'><input type='checkbox' name='guidanceCB' value='" + json[i].GuidanceId + "' />" + json[i].GuidanceName + "&nbsp;</div>";
                    $("#guidanceDiv").append(div);
                }
            }
            else {
                $("#guidanceDiv").html("<span style='color:red;'>无活动信息！</span>");
            }
        }
    });
}



function selectOutsource() {
    var rows = $("#tbOutsource").datagrid("getSelections");
    var oName = "";
    currOutsourceName = "";
    currOutsourceId = "";
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            currOutsourceId += (rows[i].Id + ",");
            oName += (rows[i].CompanyName + ",");
        }
    }
    if (oName.length > 0) {
        oName = oName.substring(0, oName.length - 1);
    }
    if (currOutsourceId.length > 0) {
        currOutsourceId = currOutsourceId.substring(0, currOutsourceId.length - 1);
    }
    currOutsourceName = oName;
    $("#orderTitle").panel({
        title: ">>外协名称：<span style='color:blue;'>" + oName + "</span>"
    });
    GetGuidacneList();
    $("#subjectDiv").html("");
}


function GetSubjectList() {
    var guidanceIds = "";
    $("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    if (guidanceIds != "") {
        guidanceIds = guidanceIds.substring(0, guidanceIds.length - 1);
    }
    $("#ImgLoadSubject").show();
    $.ajax({
        type: "get",
        url: "../handler/OrderList.ashx",
        data: { type: "getSubjectList", outsourceId: currOutsourceId, guidanceIds: guidanceIds },
        complete: function () { $("#ImgLoadSubject").hide(); },
        success: function (data) {
            $("#subjectDiv").html("");
            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style='float:left;'><input type='checkbox' name='subjectcb' value='" + json[i].SubjectId + "' /><span>" + json[i].SubjectName + "</span>&nbsp;</div>";

                }
                $("#subjectDiv").html(div);

            }
        }
    })
}



function GetProvince() {
    var guidanceIds = "";
    $("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    if (guidanceIds != "") {
        guidanceIds = guidanceIds.substring(0, guidanceIds.length - 1);
    }
    var subjectIds = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectIds += $(this).val() + ",";
    })
    if (subjectIds != "") {
        subjectIds = subjectIds.substring(0, subjectIds.length - 1);
    }
    $.ajax({
        type: "get",
        url: "../handler/OrderList.ashx",
        data: { type: "getProvince", outsourceId: currOutsourceId, guidanceIds: guidanceIds, subjectIds: subjectIds },
        beforeSend:function(){$("#ImgLoadProvince").show();},
        complete:function(){$("#ImgLoadProvince").hide();},
        success: function (data) {
            $("#provinceDiv").html("");
            $("#cityDiv").html("");
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left;'><input type='checkbox' name='provinceCB' value='" + json[i].Province + "' />" + json[i].Province + "&nbsp;</div>";
                    $("#provinceDiv").append(div);
                }
            }

        }
    });
}


function GetCity() {
    var guidanceIds = "";
    var provinces = "";
    $("input[name='guidanceCB']:checked").each(function () {
        guidanceIds += $(this).val() + ",";
    })
    $("input[name='provinceCB']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    var subjectIds = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectIds += $(this).val() + ",";
    })
    
    if (guidanceIds != "") {
        guidanceIds = guidanceIds.substring(0, guidanceIds.length - 1);
    }
    if (subjectIds != "") {
        subjectIds = subjectIds.substring(0, subjectIds.length - 1);
    }
    if (provinces != "") {
        provinces = provinces.substring(0, provinces.length - 1);
    }
    $.ajax({
        type: "get",
        url: "../handler/OrderList.ashx",
        data: { type: "getCity", outsourceId: currOutsourceId, guidanceIds: guidanceIds,subjectIds: subjectIds, provinces: provinces },
        beforeSend:function(){$("#ImgLoadCity").show();},
        complete:function(){$("#ImgLoadCity").hide();},
        success: function (data) {
            $("#cityDiv").html("");
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left;'><input type='checkbox' name='cityCB' value='" + json[i].City + "' />" + json[i].City + "&nbsp;</div>";
                    $("#cityDiv").append(div);
                }
            }

        }
    });
}

function GetCondition() {
    if (currOutsourceId =="") {
        alert("请选择外协");
        return false;
    }
    customerId = $("#ddlCustomer").val();
    guidanceMonth = $("#txtMonth").val();
    
    guidanceId = "";
    $("input[name='guidanceCB']:checked").each(function () {
        guidanceId += $(this).val() + ",";
    })
    if (guidanceId == "") {
        $("input[name='guidanceCB']").each(function () {
            guidanceId += $(this).val() + ",";
        })
    }
    if (guidanceId == "") {
        alert("请选择活动");
        return false;
    }
    subjectId = "";
    $("input[name='subjectcb']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
   
    province = "";
    $("input[name='provinceCB']:checked").each(function () {
        province += $(this).val() + ",";
    })
    city = "";
    $("input[name='cityCB']:checked").each(function () {
        city += $(this).val() + ",";
    })
    assignType = "";
    $("input[name^='cblOutsourceType']:checked").each(function () {
        assignType += $(this).val() + ",";
    })
    if (guidanceId != "") {
        guidanceId = guidanceId.substring(0, guidanceId.length - 1);
    }
    if (subjectId != "") {
        subjectId = subjectId.substring(0, subjectId.length - 1);
    }
    if (province != "") {
        province = province.substring(0, province.length - 1);
    }
    if (city != "") {
        city = city.substring(0, city.length - 1);
    }
    if (assignType != "") {
        assignType = assignType.substring(0, assignType.length - 1);
    }
}


function getMonth() {
    GetGuidacneList();
}

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

function CheckInstallPrice() {
    var url = "InstallOrderDetail.aspx?outsourceId=" + currOutsourceId + "&guidanceId=" + guidanceId + "&province=" + province + "&city=" + city + "&subjectId=" + subjectId;
    var layer1 = layer.open({
        type: 2,
        title: '安装费明细',
        shadeClose: true,
        shade: 0.8,
        area: ['95%', '90%'],
        content: url
    });
}

function CheckExpressPrice() {
    var url = "ExpressPriceDetail.aspx?outsourceId=" + currOutsourceId + "&guidanceId=" + guidanceId + "&province=" + province + "&city=" + city + "&subjectId=" + subjectId;
    var layer1 = layer.open({
        type: 2,
        title: "快递费明细",
        shadeClose: true,
        shade: 0.8,
        area: ['95%', '90%'],
        content: url
    });
}

function CheckOtherPrice() {
    //GetCondition();
    var url = "CheckOtherPriceDetail.aspx?outsourceId=" + currOutsourceId + "&guidanceId=" + guidanceId + "&province=" + province + "&city=" + city + "&assignType=" + assignType;
    layer.open({
        type: 2,
        time: 0,
        title: '其他费用信息',
        skin: 'layui-layer-rim', //加上边框
        area: ['95%', '90%'],
        content: url,
        cancel: function () {
            //Order.getMaterialList();
        }


    });
}

function CheckPOPPrice() {

    var url = "CheckPOPPriceDetail.aspx?outsourceId=" + currOutsourceId + "&guidanceId=" + guidanceId + "&province=" + province + "&city=" + city + "&assignType=" + assignType + "&guidanceMonth=" + guidanceMonth + "&customerId=" + customerId + "&subjectId=" + subjectId;
    layer.open({
        type: 2,
        time: 0,
        title: '其他费用信息',
        skin: 'layui-layer-rim', //加上边框
        area: ['95%', '90%'],
        content: url,
        cancel: function () {
           
        }
    });
}

function GetProvinceByDate() {
    
    var customerId = $("#ddlCustomer1").val();
    var beginDate = $.trim($("#txtBeginDate1").val());
    var endDate = $.trim($("#txtEndDate1").val());
    if (currOutsourceId == "") {
        layer.msg("请选择外协");
        return false;
    }
    if (beginDate == "") {
        layer.msg("请输入开始时间");
        return false;
    }
    if (endDate == "") {
        layer.msg("请输入结束时间");
        return false;
    }
    $.ajax({
        type: "post",
        url: "ListHandler.ashx",
        data: { type: "getProvinceByDate", customerId: customerId, outsourceId: currOutsourceId, beginDate: beginDate, endDate: endDate },
        beforeSend: function () { $("#Img1").show(); },
        complete: function () { $("#Img1").hide(); },
        success: function (data) {
            $("#provinceDiv1").html("");
            $("#cityDiv1").html("");
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {

                    var div = "<div style='float:left;'><input type='checkbox' name='provinceCB1' value='" + json[i].Province + "' />" + json[i].Province + "&nbsp;</div>";
                    $("#provinceDiv1").append(div);
                }
            }
        }
    })
}

function GetCityByDate() {

    var customerId = $("#ddlCustomer1").val();
    var beginDate = $.trim($("#txtBeginDate1").val());
    var endDate = $.trim($("#txtEndDate1").val());
    if (currOutsourceId == "") {
        layer.msg("请选择外协");
        return false;
    }
    if (beginDate == "") {
        layer.msg("请输入开始时间");
        return false;
    }
    if (endDate == "") {
        layer.msg("请输入结束时间");
        return false;
    }
    var province1 = "";
    $("input[name='provinceCB1']:checked").each(function () {
        province1 += $(this).val() + ",";
    })
    $.ajax({
        type: "post",
        url: "ListHandler.ashx",
        data: { type: "getCityByDate", customerId: customerId, outsourceId: currOutsourceId, beginDate: beginDate, endDate: endDate, province: province1 },
        beforeSend: function () { $("#ImgLoadCity1").show(); },
        complete: function () { $("#ImgLoadCity1").hide(); },
        success: function (data) {
            $("#cityDiv1").html("");
            if (data != "") {
                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var div = "<div style='float:left;'><input type='checkbox' name='cityCB1' value='" + json[i].City + "' />" + json[i].City + "&nbsp;</div>";
                    $("#cityDiv1").append(div);
                }
            }
        }
    })
}

function SearchByDate() {
    var customerId = $("#ddlCustomer1").val();
    var beginDate = $.trim($("#txtBeginDate1").val());
    var endDate = $.trim($("#txtEndDate1").val());
    if (currOutsourceId == "") {
        layer.msg("请选择外协");
        return false;
    }
    if (beginDate == "") {
        layer.msg("请输入开始时间");
        return false;
    }
    if (endDate == "") {
        layer.msg("请输入结束时间");
        return false;
    }
    var province1 = "";
    $("input[name='provinceCB1']:checked").each(function () {
        province1 += $(this).val() + ",";
    })
    var city1 = "";
    $("input[name='cityCB1']:checked").each(function () {
        city1 += $(this).val() + ",";
    })
    var assignType1 = "";
    $("input[name^='cblOutsourceType1']:checked").each(function () {
        assignType1 += $(this).val() + ",";
    })
    $.ajax({
        type: "post",
        url: "ListHandler.ashx",
        data: { type: "getSearchByDate", customerId: customerId, outsourceId: currOutsourceId, beginDate: beginDate, endDate: endDate, province: province1, city: city1, assignType: assignType1 },
        beforeSend: function () { $("#loadingImg1").show(); },
        complete: function () { $("#loadingImg1").hide(); },
        success: function (data) {

            if (data != "") {
                var json = eval(data);
                $("#labShopCount1").html(json[0].ShopCount);
                var labArea = "0";
                if (json[0].TotalArea > 0)
                    labArea = json[0].TotalArea;
                $("#labArea1").html(labArea + " 平米");

                var labPOPPrice = "0";
                if (json[0].POPPrice > 0) {
                    labPOPPrice = "<a href='javascript:void(0)' onclick='CheckPOPPrice()' style='text-decoration:underline;color:blue;'>" + json[0].POPPrice + "</a>";
                } $("#labPOPPrice1").html(labPOPPrice);

                var labInstallPrice = "0";
                if (json[0].InstallPrice > 0) {
                    labInstallPrice = "<a href='javascript:void(0)' onclick='CheckInstallPrice()' style='text-decoration:underline;color:blue;'>" + json[0].InstallPrice + "</a>";
                }
                $("#labInstall1").html(labInstallPrice);
                $("#labMeasurePrice1").html(json[0].MeasurePrice);
                var otherPrice = "0";
                if (json[0].OtherPrice > 0) {

                    otherPrice = "<a href='javascript:void(0)' onclick='CheckOtherPrice()' style='text-decoration:underline;color:blue;'>" + json[0].OtherPrice + "</a>";
                }
                $("#labOtherPrice1").html(otherPrice);

                var rotherPrice = "0";
                if (json[0].ReceiveOtherPrice > 0) {

                    rotherPrice = "<a href='javascript:void(0)' onclick='CheckOtherPrice()' style='text-decoration:underline;color:blue;'>" + json[0].ReceiveOtherPrice + "</a>";
                }
                $("#labROtherPrice1").html(rotherPrice);


                $("#labTotalPrice1").html(json[0].TotalPrice);
                $("#labExpressPrice1").html(json[0].ExpressPrice);

                $("#labRPOPPrice1").html(json[0].ReceivePOPPrice);
                var labRInstallPrice = "0";
                if (json[0].ReceiveInstallPrice > 0) {
                    labRInstallPrice = "<a href='javascript:void(0)' onclick='CheckInstallPrice()' style='text-decoration:underline;color:blue;'>" + json[0].ReceiveInstallPrice + "</a>";
                }
                $("#labRInstall1").html(labRInstallPrice);
                $("#labRMeasurePrice1").html(json[0].ReceiveMeasurePrice);

                $("#labRTotalPrice1").html(json[0].ReceiveTotalPrice);
                $("#labRExpressPrice1").html(json[0].ReceiveExpressPrice);
            }
            else {
                init1();
            }
        }
    })
}