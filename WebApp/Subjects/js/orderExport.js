

function getMonth() {

    $("#txtGuidanceMonth").blur();

    
}

Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
    var eleId = e.get_postBackElement().id;
    if (eleId.indexOf("lbUp") != -1 || eleId.indexOf("lbDown") != -1 || eleId.indexOf("txtGuidanceMonth") != -1) {
        $("#loadGuidanceImg").show();
    }
    if (eleId.indexOf("cblGuidanceList") != -1) {
        $("#loadSubjectTypeImg").show();

    }
    if (eleId.indexOf("cblSubjectType") != -1) {
        $("#loadActivityImg").show();
    }
    if (eleId.indexOf("cblActivity") != -1) {
        $("#loadRegionImg").show();
    }
    if (eleId.indexOf("cblRegion") != -1) {
        $("#loadSubjectImg").show();
        $("#loadlCustomerServiceImg").show();
    }
    if (eleId.indexOf("cblSubjects") != -1 || eleId.indexOf("cbSubjectAll") != -1) {
        $("#loadProvinceImg").show();
        $("#loadlCustomerServiceImg").show();
    }
    if (eleId.indexOf("cblProvince") != -1 || eleId.indexOf("cbProvinceAll") != -1) {
        $("#loadCityImg").show();
        $("#loadlCustomerServiceImg").show();
    }
})

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
    $("#loadGuidanceImg").hide();
    $("#loadSubjectTypeImg").hide();
    $("#loadActivityImg").hide();
    $("#loadRegionImg").hide();
    $("#loadSubjectImg").hide();
    $("#loadProvinceImg").hide();
    $("#loadCityImg").hide();
    $("#loadlCustomerServiceImg").hide();

    $("#cbSubjectAll").change(function () {
        var checked = this.checked;
        $("input[name^='cblSubjects']").each(function () {
            this.checked = checked;
        });
    })

    $("input[name^='cblSubjects']").change(function () {
        if (!this.checked) {
            $("#cbSubjectAll").prop("checked", false);
        }
        else {
            var checked = $("input[name^='cblSubjects']:checked").length == $("input[name^='cblSubjects']").length;
            $("#cbSubjectAll").prop("checked", checked);
        }
    })

    $("#cbProvinceAll").change(function () {
        var checked = this.checked;
        $("input[name^='cblProvince']").each(function () {
            this.checked = checked;
        });
    })

    $("input[name^='cblProvince']").change(function () {
        if (!this.checked) {
            $("#cbProvinceAll").prop("checked", false);
        }
        else {
            var checked = $("input[name^='cblProvince']:checked").length == $("input[name^='cblProvince']").length;
            $("#cbProvinceAll").prop("checked", checked);
        }
    })

    $("#cbCityAll").change(function () {
        var checked = this.checked;
        $("input[name^='cblCity']").each(function () {
            this.checked = checked;
        });
    })

    $("input[name^='cblCity']").change(function () {
        if (!this.checked) {
            $("#cbCityAll").prop("checked", false);
        }
        else {
            var checked = $("input[name^='cblCity']:checked").length == $("input[name^='cblCity']").length;
            $("#cbCityAll").prop("checked", checked);
        }
    })
})