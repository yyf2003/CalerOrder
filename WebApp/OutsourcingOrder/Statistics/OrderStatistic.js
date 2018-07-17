
Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
    var eleId = e.get_postBackElement().id;
    if (eleId.indexOf("ddlCustomer") != -1 || eleId.indexOf("txtGuidanceMonth") != -1 || eleId.indexOf("lbUp") != -1 || eleId.indexOf("lbDown") != -1) {
        $("#loadGuidance").show();
        $("#loadPropGuidance").show();
    }
    if (eleId.indexOf("cblGuidanceList") != -1) {
        $("#loadCategory").show();
        $("#loadSubject").show();
        $("#loadOutsource").show();

    }

    if (eleId.indexOf("cblPropGuidanceList") != -1) {
        $("#loadPropSubject").show();
        $("#loadPropOutsource").show();

    }

    if (eleId.indexOf("cblSubjectCategory") != -1) {

        $("#loadSubject").show();
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cblRegion") != -1) {
        $("#loadProvince").show();
        $("#loadSubject").show();
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cblProvince") != -1) {
        $("#loadCity").show();
        $("#loadSubject").show();
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cblCity") != -1) {
        $("#loadSubject").show();
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cblSubjects") != -1) {
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cblOutsourceId") != -1) {
        $("#loadMaterial").show();
    }
    if (eleId.indexOf("cbAllGuidance") != -1) {
        $("#loadCategory").show();
        $("#loadSubject").show();
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cbAllPropGuidance") != -1) {
        $("#loadPropSubject").show();
        $("#loadPropOutsource").show();
    }
})


Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
//    $("#loadGuidance").hide();
//    $("#loadPropGuidance").hide();
//    $("#loadCategory").hide();
//    $("#loadProvince").hide();
//    $("#loadCity").hide();
//    $("#loadSubject").hide();
//    $("#loadPropSubject").hide();
//    $("#loadMaterial").hide();
    //$("#loadOutsource").hide();
    $("span[name='checkPOPOrderPrice']").click(function () {

        GetCondition();
        var url = "CheckPOPPriceDetail.aspx?outsourceId=" + currOutsourceId + "&guidanceId=" + guidanceId + "&region=" + region + "&province=" + province + "&city=" + city + "&assignType=" + assignType + "&guidanceMonth=" + guidanceMonth + "&customerId=" + customerId + "&subjectId=" + subjectId;
        layer.open({
            type: 2,
            time: 0,
            title: 'POP费用明细',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '90%'],
            content: url,
            cancel: function () {

            }
        });
    });

    $("span[name='checkInstallPrice']").click(function () {
        GetCondition();
        var url = "InstallOrderDetail.aspx?outsourceId=" + currOutsourceId + "&guidanceId=" + guidanceId + "&region=" + region + "&province=" + province + "&city=" + city + "&subjectId=" + subjectId;
        var layer1 = layer.open({
            type: 2,
            title: '安装费明细',
            shadeClose: true,
            shade: 0.8,
            area: ['95%', '90%'],
            content: url
        });
    });

    $("span[name='checkExpressPrice']").click(function () {
        GetCondition();
        var url = "ExpressPriceDetail.aspx?outsourceId=" + currOutsourceId + "&guidanceId=" + guidanceId + "&region=" + region + "&province=" + province + "&city=" + city + "&subjectId=" + subjectId;
        var layer1 = layer.open({
            type: 2,
            title: "快递费明细",
            shadeClose: true,
            shade: 0.8,
            area: ['95%', '90%'],
            content: url
        });
    });

    $("span[name='checkOtherPrice']").click(function () {
        GetCondition();

        var url = "CheckOtherPriceDetail.aspx?outsourceId=" + currOutsourceId + "&guidanceId=" + guidanceId + "&region=" + region + "&province=" + province + "&city=" + city + "&assignType=" + assignType;
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
    });

    $("#cbAllGuidance").click(function () {
        var checked = this.checked;
        $("input[name^='cblGuidanceList']").each(function () {
            this.checked = checked;
        })

    });

    $("input[name^='cblGuidanceList']").change(function () {
        if (!this.checked) {
            $("#cbAllGuidance").prop("checked", false);
        }
        else {
            var checked = $("input[name^='cblGuidanceList']:checked").length == $("input[name^='cblGuidanceList']").length;
            $("#cbAllGuidance").prop("checked", checked);
        }
    })
   
    $("#btnExportDetail").click(function () {
        var guidanceId0 = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceId0 += $(this).val() + ",";
        })
        if (guidanceId0 == "") {
            layer.msg("请选择活动");
            return false;
        }
        $("#btnExport").click();
    })

})


function getMonth() {

    $("#txtGuidanceMonth").blur();
}

function loading() {
    //var outsource = $("input[name^='cblOutsource']:checked").length;
    //if (outsource == 0) {
    //alert("请选择外协名称");
    //return false;
    //}
    $("#loadingImg").show();
    return true;
}


var customerId = 0;
var currOutsourceId = "";
var guidanceMonth = "";
var guidanceId = "";
var subjectId = "";
var region = "";
var province = "";
var city = "";
var assignType = "";
function GetCondition() {

    customerId = $("#ddlCustomer").val();
    guidanceMonth = $("#txtMonth").val();
    guidanceId = "";
    $("input[name^='cblGuidanceList']:checked").each(function () {
        guidanceId += $(this).val() + ",";
    })
    if (guidanceId == "") {
        $("input[name^='cblGuidanceList']").each(function () {
            guidanceId += $(this).val() + ",";
        })
    }
    if (guidanceId == "") {
        alert("请选择活动");
        return false;
    }
    subjectId = "";
    $("input[name^='cblSubjects']:checked").each(function () {
        subjectId += $(this).val() + ",";
    })
    region = "";
    $("input[name^='cblRegion']:checked").each(function () {
        region += $(this).val() + ",";
    })
    province = "";
    $("input[name^='cblProvince']:checked").each(function () {
        province += $(this).val() + ",";
    })
    city = "";
    $("input[name^='cblCity']:checked").each(function () {
        city += $(this).val() + ",";
    })

    currOutsourceId = ""
    $("input[name^='cblOutsourceId']:checked").each(function () {
        currOutsourceId += $(this).val() + ",";
    })
    if (currOutsourceId == "") {
        $("input[name^='cblOutsourceId']").each(function () {
            currOutsourceId += $(this).val() + ",";
        })
    }
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
    if (region != "") {
        region = region.substring(0, region.length - 1);
    }
    if (province != "") {
        province = province.substring(0, province.length - 1);
    }
    if (city != "") {
        city = city.substring(0, city.length - 1);
    }
    if (currOutsourceId != "") {
        currOutsourceId = currOutsourceId.substring(0, currOutsourceId.length - 1);
    }
    if (assignType != "") {
        assignType = assignType.substring(0, assignType.length - 1);
    }
}


$(function () {

})

