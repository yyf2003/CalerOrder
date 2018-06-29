$(function () {
    //ChangeSubjectType();
})

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, e) {
    $("input[name^='cblOrderType'").change(function () {

        if (this.checked) {
            $(this).siblings("input").attr("checked", false);
        }
        
    })

    //    $("#rblSubjectType").change(function () {
    //        ChangeSubjectType();
    //    })
})

function CheckVal() {
    if ($("#ddlCustomer").val() == "0") {
        layer.msg("请选择所属客户");
        return false;
    }
    var guidanceId = $("#ddlGuidance").val();
    if (guidanceId == 0) {
        layer.msg("请选择活动名称");
        return false;
    }
    var subjectType = $("input:radio[name='rblSubjectType']:checked").val() || -1;

    if (subjectType == -1) {
        layer.msg("请选择订单类型");
        return false;
    }

    if ($.trim($("#txtSubjectName").val()) == "") {
        layer.msg("请填写项目名称");
        return false;
    }

    if ($.trim($("#txtBeginDate").val()) == "") {
        layer.msg("请填写开始时间");
        return false;
    }
    if ($.trim($("#txtEndDate").val()) == "") {
        layer.msg("请填写结束时间");
        return false;
    }

    

    var subjectType = $("#ddlSubjectType").val();
    var subjectCategory = $("#ddlSubjectCategory").val();
    if (subjectType == "0") {
        layer.msg("请选择项目类型");
        return false;
    }
    if (subjectCategory == "0") {
        layer.msg("请选择项目分类");
        return false;
    }

    //if (subjectType != 2) {
        
    //}

    $("#loadingImg").show();
    return true;
}

function CheckVal0() {
    var guidanceId = $("#ddlGuidance").val();
    if (guidanceId == 0) {
        alert("请选择活动名称");
        return false;
    }
    var subjectType = $("input:radio[name='rblSubjectType']:checked").val() || -1;

    if (subjectType == -1) {
        alert("请选择订单类型");
        return false;
    }

    if ($.trim($("#txtSubjectName").val()) == "") {
        alert("请填写项目名称");
        return false;
    }

    if ($.trim($("#txtBeginDate").val()) == "") {
        alert("请填写开始时间");
        return false;
    }
    if ($.trim($("#txtEndDate").val()) == "") {
        alert("请填写结束时间");
        return false;
    }

    if ($("#ddlCustomer").val() == "0") {
        alert("请选择所属客户");
        return false;
    }

    if (subjectType != 2) {
        var subjectType = $("#ddlSubjectType").val();
        var subjectCategory = $("#ddlSubjectCategory").val();
        if (subjectType == "0") {
            alert("请选择项目类型");
            return false;
        }
        if (subjectCategory == "0") {
            alert("请选择项目分类");
            return false;
        }
    }
    
    if ($("#cbNoOrderList").attr("checked") == "checked") {
        var region = $("input:radio[name='rblRegion']:checked").val() || "";
        if (region == "") {
            alert("请选择补单区域");
            return false;
        }
    }
    $("#Img0").show();
    return true;
}

function ChangeSubjectType() {
    var val = $("input[name^='rblSubjectType']:checked").val();
    if (val == 1) {
        $("#subjectTypeTr").show();
    }
    else {
        $("#subjectTypeTr").hide();
    }
}