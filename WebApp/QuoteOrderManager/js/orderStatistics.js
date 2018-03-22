function getMonth() {

    $("#txtGuidanceMonth").blur();
}


function loading() {

    var customerId = $("#ddlCustomer").val();
    if (customerId == 0) {
        alert("请选择客户");
        return false;
    }
    var guidanceCount = 0;
    guidanceCount = $("input[name^='cblGuidanceList']:checked").length;
    var subjectCount = 0;
    if (guidanceCount == 0) {
        alert("请选择活动名称");
        return false;
    }



    $("#loadingImg").show();
    return true;
}

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

$(function () {

    $("#btnExportQuote").click(function () {
        var guidanceIds = "";
        var subjectIds = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceIds += $(this).val() + ",";
        })
        $("input[name^='cblSubjects']:checked").each(function () {
            subjectIds += $(this).val() + ",";
        })
        if (subjectIds == "") {
            $("input[name^='cblSubjects']").each(function () {
                subjectIds += $(this).val() + ",";
            })
        }
        var templateType = $("input[name='rblExportType']:checked").val() || 1;
        var url = "handler/ExportDetail.ashx?guidanceIds=" + guidanceIds + "&subjectIds=" + subjectIds + "&templateType=" + templateType;
        $("#exportFrame").attr("src", url);
    })

    $("#cbShopSubjectList").change(function () {
        $("#loadSubjectList").show();
    })
})