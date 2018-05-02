

Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
    var eleId = e.get_postBackElement().id;
    if (eleId.indexOf("lbUp") != -1 || eleId.indexOf("lbDown") != -1) {
        $("#loadGuidance").show();
       
    }
    if (eleId.indexOf("cblGuidanceList") != -1) {
        $("#loadShopType").show();
        $("#loadUser").show();
        $("#loadCategory").show();
        $("#loadSubject0").show();
        $("#loadSubject").show();
    }

    if (eleId.indexOf("cblSubjectChannel") != -1) {
        $("#loadShopType").show();
        $("#loadCategory").show();
        $("#loadSubject0").show();
        $("#loadSubject").show();
    }
    if (eleId.indexOf("cblShopType") != -1) {
        $("#loadUser").show();
        $("#loadCategory").show();
        $("#loadSubject").show();
    }
    if (eleId.indexOf("cblAddUser") != -1) {
        $("#loadCategory").show();
        $("#loadSubject0").show();
        $("#loadSubject").show();
    }
    if (eleId.indexOf("cblRegion") != -1) {
        $("#loadProvince").show();
        $("#loadSubject0").show();
        $("#loadSubject").show();
        $("#loadCustomerService").show();
    }
    if (eleId.indexOf("cblProvince") != -1) {
        $("#loadCity").show();
        $("#loadCustomerService").show();
        //$("#loadSubject0").show();
        //$("#loadSubject").show();
    }
    if (eleId.indexOf("btnCheckAllGuidance") != -1) {
        $("#loadShopType").show();
        $("#loadUser").show();
        $("#loadCategory").show();
        $("#loadSubject0").show();
        $("#loadSubject").show();
    }
    if (eleId.indexOf("cbShowSubjectNameList") != -1) {
        $("#loadSubjectNames").show();
       
    }
})

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
    $("#loadGuidance").hide();
    $("#loadShopType").hide();
    $("#loadUser").hide();
    $("#loadCategory").hide();
    $("#loadProvince").hide();
    $("#loadCity").hide();
    $("#loadSubject").hide();
    $("#loadCustomerService").hide();
    $("#loadSubjectNames").hide();
    $("#cbCheckAllGuidance").change(function () {
        var checked = this.checked;
        $("input[name^='cblGuidanceList']").each(function () {
            this.checked = checked;
        });
        $("#btnCheckAllGuidance").click();
    })

    $("#cbAll").change(function () {
        var checked = this.checked;
        $("input[name^='cblSubjects']").each(function () {
            this.checked = checked;
        });
    })

    $("input[name^='cblSubjects']").change(function () {
        if (!this.checked) {
            $("#cbAll").prop("checked", false);
        }
        else {
            var checked = $("input[name^='cblSubjects']:checked").length == $("input[name^='cblSubjects']").length;
            $("#cbAll").prop("checked", checked);
        }
    })

    $("#cbAll0").change(function () {
        var checked = this.checked;
        $("input[name^='cblPriceSubjects']").each(function () {
            this.checked = checked;
        });
    })

    $("input[name^='cblSubjectChannel']").change(function () {

        if (this.checked) {

            $(this).siblings("input").each(function () {
                this.checked = false;
            });
        }
    })

    $("span[name='spanCheckSubject']").click(function () {
        var subjectId = $(this).data("subjectid");
        //alert(subjectId);
        var url = "SubjectDetail.aspx?subjectId=" + subjectId;
        var layer1 = layer.open({
            type: 2,
            title: '项目信息',
            shadeClose: true,
            skin: 'layui-layer-rim', 
            shade: 0.8,
            area: ['500px', '450px'],
            content: url
        });
    })
})

//var $j = jQuery.noConflict();
$(function () {
    //查看店铺信息
    $("span[name='checkShop']").click(function () {
        var guidanceIds = "";
        var subjectid = "";
        var regions = "";
        var provinces = "";
        var citys = "";
        var customerServiceIds = "";
        var status = $(this).data("status") || "";
        var shopType = "";
        var subjectChannel = 0;
        var subjectCategory = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjects']:checked").each(function () {
            subjectid += $(this).val() + ",";
        })

        $("input[name^='cblRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        $("input[name^='cblProvince']:checked").each(function () {
            provinces += $(this).val() + ",";
        })
        $("input[name^='cblCity']:checked").each(function () {
            citys += $(this).val() + ",";
        })
        $("input[name^='cblSubjectChannel']:checked").each(function () {
            subjectChannel = $(this).val();
        })
        $("input[name^='cblShopType']:checked").each(function () {
            shopType += $(this).val() + ",";
        })
        $("input[name^='cblCustomerService']:checked").each(function () {
            customerServiceIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjectCategory']:checked").each(function () {
            subjectCategory += $(this).val() + ",";
        })
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }
        var url = "CheckShops.aspx?guidanceIds=" + guidanceIds + "&subjectIds=" + subjectid + "&regions=" + regions + "&provinces=" + provinces + "&citys=" + citys + "&status=" + status + "&subjectChannel=" + subjectChannel + "&shopType=" + shopType + "&customerServiceIds=" + customerServiceIds + "&subjectCategory=" + subjectCategory;
        $.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: '90%',
            //modal:true,
            hideOnOverlayClick: false
        })
    })

    //查看POP制作金额明细
    $("span[name='checkMaterial']").click(function () {
        var status = $(this).data("status") || "";
        CheckPOPPriceFUN(null, status);
    })

    //查看安装费明细
    $("span[name='checkInstallPrice']").click(function () {

        var status = $(this).data("status") || "";
        var guidanceIds = "";
        var subjectid = "";
        var regions = "";
        var provinces = "";
        var customerServiceIds = "";
        var subjectChannel = 0;
        var subjectCategory = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        var subjectCount = 0;
        var subjectSelectCount = $("input[name^='cblSubjects']").length;
        $("input[name^='cblSubjects']:checked").each(function () {
            subjectid += $(this).val() + ",";
            subjectSelectCount++;
        })
        if (subjectSelectCount == subjectCount) {
            subjectid = "";
        }
        $("input[name^='cblRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        $("input[name^='cblProvince']:checked").each(function () {
            provinces += $(this).val() + ",";
        })
        $("input[name^='cblSubjectChannel']:checked").each(function () {
            subjectChannel = $(this).val();
        })
        $("input[name^='cblCustomerService']:checked").each(function () {
            customerServiceIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjectCategory']:checked").each(function () {
            subjectCategory += $(this).val() + ",";
        })
        var url = "InstallPriceDetail.aspx?guidanceIds=" + guidanceIds + "&subjectIds=" + subjectid + "&regions=" + regions + "&provinces=" + provinces + "&subjectChannel=" + subjectChannel + "&customerServiceIds=" + customerServiceIds + "&subjectCategory=" + subjectCategory;
        //var url = "/Subjects/InstallPrice/ShopDetail.aspx?subjectIds=" + subjectid + "&region=" + regions + "&status=" + status + "&subjectChannel=" + subjectChannel + "&customerServiceIds=" + customerServiceIds;


        var layer1 = layer.open({
            type: 2,
            title: '安装店铺明细',
            shadeClose: true,
            shade: 0.8,
            area: ['96%', '95%'],
            content: url
        });
    })

    //查看发货费明细
    $("span[name='checkExpressPrice']").click(function () {
        var subjectid = "";
        var regions = "";
        var provinces = "";
        var citys = "";

        $("input[name^='cblSubjects']:checked").each(function () {
            subjectid += $(this).val() + ",";
        })
        if (subjectid == "") {
            $("input[name^='cblSubjects']").each(function () {
                subjectid += $(this).val() + ",";
            })
        }
        $("input[name^='cblRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        $("input[name^='cblProvince']:checked").each(function () {
            provinces += $(this).val() + ",";
        })
        $("input[name^='cblCity']:checked").each(function () {
            citys += $(this).val() + ",";
        })
        var url = "FreightDetail.aspx?subjectIds=" + subjectid + "&regions=" + regions + "&provinces=" + provinces;
        $.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: '90%',

            hideOnOverlayClick: false
        })
    })

    //查看物料明细
    $("span[name='checkMaterialOrderPrice']").click(function () {
        CheckMaterialOrderPriceFUN();
    })

    //查看其他费用明细
    $("span[name='checkOtherPrice']").click(function () {
        var subjectid = "";
        var regions = "";
        var provinces = "";
        var citys = "";
        $("input[name^='cblSubjects']:checked").each(function () {
            subjectid += $(this).val() + ",";
        })
        $("input[name^='cblPriceSubjects']:checked").each(function () {
            subjectid += $(this).val() + ",";
        })
        if (subjectid == "") {
            $("input[name^='cblSubjects']").each(function () {
                subjectid += $(this).val() + ",";
            })
            $("input[name^='cblPriceSubjects']").each(function () {
                subjectid += $(this).val() + ",";
            })
        }
        $("input[name^='cblRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        $("input[name^='cblProvince']:checked").each(function () {
            provinces += $(this).val() + ",";
        })
        $("input[name^='cblCity']:checked").each(function () {
            citys += $(this).val() + ",";
        })
        var url = "PriceSubjectStatistic.aspx?subjectIds=" + subjectid + "&region=" + regions + "&province=" + provinces + "&city=" + citys;
        $.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: '90%',
            hideOnOverlayClick: false
        })
    })

    //查看新开店安装费用明细
    $("span[name='checkNewShopInstallPrice']").click(function () {
        var subjectid = "";
        var regions = "";
        var provinces = "";
        var citys = "";

        $("input[name^='cblPriceSubjects']:checked").each(function () {
            subjectid += $(this).val() + ",";
        })
        if (subjectid == "") {
            $("input[name^='cblPriceSubjects']").each(function () {
                subjectid += $(this).val() + ",";
            })
        }
        $("input[name^='cblRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        $("input[name^='cblProvince']:checked").each(function () {
            provinces += $(this).val() + ",";
        })
        $("input[name^='cblCity']:checked").each(function () {
            citys += $(this).val() + ",";
        })
        var url = "NewShopInstallPriceDetail.aspx?subjectIds=" + subjectid + "&region=" + regions + "&province=" + provinces + "&city=" + citys;
        $.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: '90%',
            hideOnOverlayClick: false
        })
    })

    //导出
    $("#btnExport").click(function () {
        var guidanceIds = "";
        var subjectIds = "";
        var regions = "";
        var provinces = "";
        var citys = "";
        var subjectChannel = 0;
        var shopType = "";
        var customerServiceIds = "";
        var subjectCategory = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        $("input[name^='cblRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        $("input[name^='cblProvince']:checked").each(function () {
            provinces += $(this).val() + ",";
        })
        $("input[name^='cblCity']:checked").each(function () {
            citys += $(this).val() + ",";
        })

        $("input[name^='cblPriceSubjects']:checked").each(function () {
            subjectIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjects']:checked").each(function () {
            subjectIds += $(this).val() + ",";
        })
        if (subjectIds == "") {
            $("input[name^='cblPriceSubjects']").each(function () {
                subjectIds += $(this).val() + ",";
            })
            $("input[name^='cblSubjects']").each(function () {
                subjectIds += $(this).val() + ",";
            })
        }
        $("input[name^='cblSubjectChannel']:checked").each(function () {
            subjectChannel = $(this).val();
        })
        $("input[name^='cblShopType']:checked").each(function () {
            shopType += $(this).val() + ",";
        })
        $("input[name^='cblCustomerService']:checked").each(function () {
            customerServiceIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjectCategory']:checked").each(function () {
            subjectCategory += $(this).val() + ",";
        })
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }

        var isExportInstall = 0;
        var installPrice = "";
        var freight = "";
        if ($("#cbInstallPrice").attr("checked") == "checked") {
            isExportInstall = 1;
            installPrice = $("#labInstallPrice").html();
        }
        freight = $("#labExpressPrice").html();
        $("#exportWaiting").show();
        checkExport();
        var url = "handler/ExportDetail.ashx?guidanceIds=" + guidanceIds + "&subjectIds=" + subjectIds + "&regions=" + regions + "&provinces=" + provinces + "&citys=" + citys + "&isExportInstall=" + isExportInstall + "&installPrice=" + installPrice + "&freight=" + freight + "&subjectChannel=" + subjectChannel + "&shopType=" + shopType + "&customerServiceIds=" + customerServiceIds + "&subjectCategory=" + subjectCategory;
        $("#exportFrame").attr("src", url);
    })

    //按店统计导出
    $("#btnExportByShop").click(function () {
        var guidanceIds = "";
        var subjectIds = "";
        var regions = "";
        var provinces = "";
        var citys = "";
        var subjectChannel = 0;
        var shopType = "";
        var customerServiceIds = "";
        var subjectCategory = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        $("input[name^='cblRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        $("input[name^='cblProvince']:checked").each(function () {
            provinces += $(this).val() + ",";
        })
        $("input[name^='cblCity']:checked").each(function () {
            citys += $(this).val() + ",";
        })

        $("input[name^='cblPriceSubjects']:checked").each(function () {
            subjectIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjects']:checked").each(function () {
            subjectIds += $(this).val() + ",";
        })
        if (subjectIds == "") {
            $("input[name^='cblPriceSubjects']").each(function () {
                subjectIds += $(this).val() + ",";
            })
            $("input[name^='cblSubjects']").each(function () {
                subjectIds += $(this).val() + ",";
            })
        }
        $("input[name^='cblSubjectChannel']:checked").each(function () {
            subjectChannel = $(this).val();
        })
        $("input[name^='cblShopType']:checked").each(function () {
            shopType += $(this).val() + ",";
        })
        $("input[name^='cblCustomerService']:checked").each(function () {
            customerServiceIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjectCategory']:checked").each(function () {
            subjectCategory += $(this).val() + ",";
        })
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }

        $("#exportWaitingByShop").show();
        checkExportByShop();
        var url = "handler/ExportDetail.ashx?guidanceIds=" + guidanceIds + "&subjectIds=" + subjectIds + "&regions=" + regions + "&provinces=" + provinces + "&citys=" + citys + "&subjectChannel=" + subjectChannel + "&shopType=" + shopType + "&customerServiceIds=" + customerServiceIds + "&subjectCategory=" + subjectCategory + "&exportType=byShop";
        $("#exportFrame").attr("src", url);
    })
    //导出关闭店铺订单明细
    $("#btnExport1").click(function () {
        var guidanceIds = "";
        var subjectIds = "";
        var regions = "";
        var provinces = "";
        var citys = "";
        var subjectChannel = 0;
        var shopType = "";
        var subjectCategory = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        $("input[name^='cblRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        $("input[name^='cblProvince']:checked").each(function () {
            provinces += $(this).val() + ",";
        })
        $("input[name^='cblCity']:checked").each(function () {
            citys += $(this).val() + ",";
        })
        $("input[name^='cblPriceSubjects']:checked").each(function () {
            subjectIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjects']:checked").each(function () {
            subjectIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjectChannel']:checked").each(function () {
            subjectChannel = $(this).val();
        })
        $("input[name^='cblShopType']:checked").each(function () {
            shopType += $(this).val() + ",";
        })
        $("input[name^='cblSubjectCategory']:checked").each(function () {
            subjectCategory += $(this).val() + ",";
        })
        if (guidanceIds == "") {
            alert("请选择活动");
            return false;
        }
        $(this).next("img").show();
        checkExport();
        var url = "handler/ExportDetail.ashx?guidanceIds=" + guidanceIds + "&subjectIds=" + subjectIds + "&regions=" + regions + "&provinces=" + provinces + "&citys=" + citys + "&status=1&subjectChannel=" + subjectChannel + "&shopType=" + shopType + "&subjectCategory=" + subjectCategory;
        $("#exportFrame").attr("src", url);
    })

    //查看项目明细
    $("span[name='spanCheckDetail']").click(function () {
        var subjectId = $(this).data("subjectid");
        var subjectType = $(this).data("subjecttype") || 1;
        var regions = "";
        var provinces = "";
        var citys = "";
        var subjectChannel = 0;
        var shopType = "";
        var customerServiceIds = "";
        $("input[name^='cblRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        $("input[name^='cblProvince']:checked").each(function () {
            provinces += $(this).val() + ",";
        })
        $("input[name^='cblCity']:checked").each(function () {
            citys += $(this).val() + ",";
        })
        $("input[name^='cblSubjectChannel']:checked").each(function () {
            subjectChannel = $(this).val();
        })
        $("input[name^='cblShopType']:checked").each(function () {
            shopType += $(this).val() + ",";
        })
        $("input[name^='cblCustomerService']:checked").each(function () {
            customerServiceIds += $(this).val() + ",";
        })

        var url = "SubjectStatistics.aspx";
        if (subjectType == 2 || subjectType == 9) {
            //新开店安装费用订单明细
            url = "NewShopInstallPriceDetail.aspx";
        }
        if (subjectType == 12) {
            //费用订单明细
            url = "PriceSubjectStatistic.aspx";
        }
        if (subjectType == 4) {
            //二次安装费订单明细
            url = "/Subjects/SecondInstallFee/CheckOrderDetail.aspx";
        }
        url = url + "?subjectId=" + subjectId + "&region=" + regions + "&province=" + provinces + "&city=" + citys + "&subjectChannel=" + subjectChannel + "&shopType=" + shopType + "&customerServiceId=" + customerServiceIds;
        if (subjectType == 4) {
            //二次安装费订单明细
            url += "&isCheck=1";
        }
        $.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: '90%',
            //modal:true,
            hideOnOverlayClick: false
        })
    })
})

function CheckMaterialPrice(subjectId) {
    CheckPOPPriceFUN(subjectId,null);

}

function CheckMaterialOrderPrice(subjectId) {
    CheckMaterialOrderPriceFUN(subjectId);

}

function loading() {

    //$(".statisticsTable span").html("0").removeAttr("style").removeAttr("name");

    var customerId = $("#ddlCustomer").val();
    if (customerId == 0) {
        alert("请选择客户");
        return false;
    }
    var guidanceCount = 0;
    guidanceCount = $("input[name^='cblGuidanceList']:checked").length;
    var subjectCount = 0;
    //subjectCount += $("input[name^='cblSubjects']:checked").length;
    //subjectCount += $("input[name^='cblPriceSubjects']:checked").length;
    //subjectCount += $("input[name^='cblSecondInstallSubjects']:checked").length;
    if (guidanceCount == 0) {
        alert("请选择活动名称");
        return false;
    }



    $("#loadingImg").show();
    return true;
}

var timer;
function checkExport() {
    timer = setInterval(function () {
        $.ajax({
            type: "get",
            //url: "handler/CheckExportDetail.ashx",
            url: "handler/CheckExportState.ashx?type=allPrice",
            cache: false,
            success: function (data) {

                if (data == "ok") {
                    $("#exportWaiting").hide();
                    clearInterval(timer);
                }

            }
        })

    }, 1000);
}

var timer1;
function checkExportByShop() {
    timer1 = setInterval(function () {
        $.ajax({
            type: "get",
            url: "handler/CheckExportState.ashx?type=exportByShop",
            cache: false,
            success: function (data) {
                if (data == "ok") {
                    $("#exportWaitingByShop").hide();
                    clearInterval(timer1);
                }

            }
        })

    }, 1000);
}

function CheckPOPPriceFUN(subjectId1,status1) {
    var guidanceIds = "";
    var subjectid = subjectId1||"";
    var regions = "";
    var provinces = "";
    var citys = "";
    var status = status1 || "";
    var shopType = "";
    var customerServiceIds = "";
    var subjectChannel = 0;
    var subjectCategory = "";
    if (subjectid == "") {
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjects']:checked").each(function () {
            subjectid += $(this).val() + ",";
        })
    }
    $("input[name^='cblRegion']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    $("input[name^='cblProvince']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    $("input[name^='cblCity']:checked").each(function () {
        citys += $(this).val() + ",";
    })
    $("input[name^='cblSubjectChannel']:checked").each(function () {
        subjectChannel = $(this).val();
    })
    $("input[name^='cblShopType']:checked").each(function () {
        shopType += $(this).val() + ",";
    })
    $("input[name^='cblCustomerService']:checked").each(function () {
        customerServiceIds += $(this).val() + ",";
    })
    $("input[name^='cblSubjectCategory']:checked").each(function () {
        subjectCategory += $(this).val() + ",";
    })
    var url = "MaterialStatistics.aspx?guidanceIds=" + guidanceIds + "&subjectIds=" + subjectid + "&regions=" + regions + "&provinces=" + provinces + "&citys=" + citys + "&status=" + status + "&subjectChannel=" + subjectChannel + "&isSearch=1&shopType=" + shopType + "&customerServiceIds=" + customerServiceIds + "&subjectCategory=" + subjectCategory;
    $.fancybox({
        href: url,
        type: 'iframe',
        padding: 5,
        width: '90%',
        //modal:true,
        hideOnOverlayClick: false
    })
}

function CheckMaterialOrderPriceFUN(subjectId1) {
    var subjectid = subjectId1||"";
    var regions = "";
    var provinces = "";
    var citys = "";

    if (subjectid == "") {
        $("input[name^='cblSubjects']:checked").each(function () {
            subjectid += $(this).val() + ",";
        })
        if (subjectid == "") {
            $("input[name^='cblSubjects']").each(function () {
                subjectid += $(this).val() + ",";
            })
        }
    }
    
    $("input[name^='cblRegion']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    $("input[name^='cblProvince']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    $("input[name^='cblCity']:checked").each(function () {
        citys += $(this).val() + ",";
    })
    var url = "CheckMaterialOrderDetail.aspx?subjectIds=" + subjectid + "&regions=" + regions + "&provinces=" + provinces;
    $.fancybox({
        href: url,
        type: 'iframe',
        padding: 5,
        width: '90%',

        hideOnOverlayClick: false
    })
}

function CheckProjectDate() {
//    var beginDate = $("#txtSubjectBegin").val();
//    var endDate = $("#txtSubjectEnd").val();
//    if (beginDate == "") {
//        alert("请输入开始时间");
//        return false;
//    }
    $("#loadSubject").show();
    return true;
}

function CheckGuidanceDate() {
    $("#loadGuidance").show();
    return true;
}

function GetSelectProvinceAndCity() {
   
    var provinces = "";
    var citys = "";
    $("input[name^='cblProvince']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    $("input[name^='cblCity']:checked").each(function () {
        citys += $(this).val() + ",";
    })
    if (provinces != "") {
        provinces = provinces.substring(0, provinces.length - 1);
    }
    if (citys != "") {
        citys = citys.substring(0, citys.length - 1);
    }
    return provinces + "$" + citys;
}

function getMonth() {
    //alert($("#txtGuidanceMonth").val());
    $("#txtGuidanceMonth").blur();
}