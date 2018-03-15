



var postBackId;
Sys.WebForms.PageRequestManager.getInstance().add_initializeRequest(function (sender, e) {
    postBackId = e.get_postBackElement().id;
    if (postBackId.indexOf("btnSubmit") != -1) {
        var shopNum = $("#labShopCount").text() || 0;
        if (shopNum == 0) {
            alert("没有店铺信息");
            e.set_cancel(true);
            return false;
        }
        if ($("#ddlSubject").val() == "0") {
            alert("请选择归类项目");
            e.set_cancel(true);
            return false;
        }
        $("#imgWaitting").show();
    }

    if (postBackId.indexOf("btnSecondSubmit") != -1) {
        var subjectids = "";
        $("input[name^='cblSecondSubjectName']:checked").each(function () {
            subjectids += $(this).val() + ",";
        })
        
        if (subjectids == "") {
            alert("请选择项目");
            e.set_cancel(true);
            return false;
        }
        $("#imgWaitting1").show();
    }

    if (postBackId.indexOf("cblRegion") != -1) {

        $("#imgLoadSubjectType").show();
    }
    if (postBackId.indexOf("cblSubjectType") != -1) {

        $("#imgLoadSubjects").show();
    }
    if (postBackId.indexOf("cblSubjectName") != -1) {

        $("#imgLoadPOSScale").show();
    }
    if (postBackId.indexOf("cblPOSScale") != -1) {
        $("#imgLoadSelectShopCount").show();
    }

    if (postBackId.indexOf("cblSecondRegion") != -1) {

        $("#imgSecondLoadSubjectType").show();
    }
    if (postBackId.indexOf("cblSecondSubjectType") != -1) {

        $("#imgSecondLoadSubjects").show();
    }
})

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, e) {

    var val = $("#hfIsFinishEdit").val() || 0;
    var leftShopCount = $("#labTotalShopCount").html();
    //if (val == 0 && leftShopCount > 0) {
        //CheckEmpty();
        //$("#hfIsFinishEdit").val("1");
    //}


    $("img[name='loadImg']").hide();
    $("div[name='loadImg']").hide();
    $("span[name='spanCheckShop']").click(function () {
        //alert($(this).html());
        var detailId = $(this).data("detailid");
        var url = "ShopDetail.aspx?fromSubmit=1&InstallDetailId=" + detailId;
        var layer1 = layer.open({
            type: 2,
            title: '安装店铺明细',
            shadeClose: true,
            shade: 0.8,
            area: ['90%', '80%'],
            content: url
        });
    })

    $("span[name='spanSecondCheckShop']").click(function () {
        //alert($(this).html());
        var detailId = $(this).data("detailid");
        var url = "ShopDetail.aspx?InstallDetailId=" + detailId;
        var layer1 = layer.open({
            type: 2,
            title: '安装店铺明细',
            shadeClose: true,
            shade: 0.8,
            area: ['90%', '80%'],
            content: url
        });
    })


    $("span[name='spanCheckLeftShop']").click(function () {
        var region = "";
        var subjectTypeId = "";
        var subjectId = "";
        var posScale = "";
        $("input[name^='cblRegion']:checked").each(function () {
            region += $(this).val() + ",";
        })
        $("input[name^='cblSubjectType']:checked").each(function () {
            subjectTypeId += $(this).val() + ",";
        })
        $("input[name^='cblSubjectName']:checked").each(function () {
            subjectId += $(this).val() + ",";
        })
        $("input[name^='cblPOSScale']:checked").each(function () {
            posScale += $(this).val() + ",";
        })
       
        var url = "EditPOSScale.aspx?guidanceId=" + guidanceId + "&region=" + region + "&subjectTypeId=" + subjectTypeId + "&subjectId=" + subjectId + "&posScale=" + posScale;
        //var url = "EditPOSScale.aspx";
        var layer2 = layer.open({

            type: 2,
            title: '店铺信息',
            shadeClose: true,
            shade: 0.8,
            area: ['95%', '95%'],
            content: url,
            cancel: function () {

                if ($("#hfIsFinishEdit").val() == "1") {

                    window.location.reload();
                }
            }

        });

    })

    $("#cbPOSScale").change(function () {
        var checked = this.checked;
        $("input[name^='cblPOSScale']").each(function () {
            this.checked = checked;
        })
        if (checked)
            $("input[name^='cblPOSScale']").eq(0).attr("checked", false).click();
        else
            $("input[name^='cblPOSScale']").eq(0).attr("checked", true).click();
    })

    $("#cbSubjects").change(function () {
        var checked = this.checked;
        $("input[name^='cblSubjectName']").each(function () {
            this.checked = checked;
        })
        if (checked)
            $("input[name^='cblSubjectName']").eq(0).attr("checked", false).click();
        else
            $("input[name^='cblSubjectName']").eq(0).attr("checked", true).click();

    })

    $("#cbSecondSubjects").change(function () {
        var checked = this.checked;
        $("input[name^='cblSecondSubjectName']").each(function () {
            this.checked = checked;
        })
        if (checked)
            $("input[name^='cblSecondSubjectName']").eq(0).attr("checked", false).click();
        else
            $("input[name^='cblSecondSubjectName']").eq(0).attr("checked", true).click();

    })

    if (postBackId.indexOf("cblRegion") != -1) {
        $("#cbSubjects").attr("checked", false);
        $("#cbPOSScale").attr("checked", false);

    }
    if (postBackId.indexOf("cblSubjectType") != -1) {
        $("#cbSubjects").attr("checked", false);
        $("#cbPOSScale").attr("checked", false);

    }

    if (postBackId.indexOf("cblSecondRegion") != -1) {
        $("#cbSecondSubjects").attr("checked", false);

    }
    if (postBackId.indexOf("cblSecondSubjectType") != -1) {
        $("#cbSecondSubjects").attr("checked", false);

    }

    if (postBackId.indexOf("cblSubjectName") != -1) {
        $("#cbPOSScale").attr("checked", false);
    }

})


function CheckEmpty() {
    $.ajax({
        type: "get",
        url: "/Subjects/InstallPrice/handler/Handler1.ashx?type=checkEmpty&guidanceId=" + guidanceId,
        success: function (data) {

            if (data == "empty") {
                layer.open({
                    title: '警 告',
                    type: 2,
                    time: 0,
                    skin: 'layui-layer-rim', //加上边框
                    area: ['520px', '320px'], //宽高
                    content: '/Subjects/InstallPrice/EmptyWarn.aspx?guidanceId=' + guidanceId

                })
            }
        }
    })
}
var activeTabIndex = 0;
var firstLoad = 1;

$(function () {
    

    //layui事件监听
    layui.use('element', function () {
        var element = layui.element();

        //tab事件监听
        element.on('tab(order)', function (data) {
            activeTabIndex = data.index;
            
            if (activeTabIndex == 1 && firstLoad == 1) {
                firstLoad = 0;
                $("#Button1").click();
            }
        });
    });
})

function LoadDelete(obj) {
    if (confirm("确定删除吗？")) {
        $(obj).next("img").show();
        return true;
    }
    else
        return false;
}