





function CheckVal() {
    var subjectType = $("input[name='rblSubjectType']:checked").val()||0;
    if (subjectType == 0) {
        alert("请选择订单类型");
        return false;
    }
    
    var guidanceId = $("#ddlGuidance").val();
    if (guidanceId == 0) {
        alert("请选择活动名称");
        return false;
    }
    if (subjectType == 10) {
        var subjectId = $("#ddlSubjectName").val();
        if (subjectId == "0") {
            alert("请选择项目名称");
            return false;
        }
    }
    else {
        var subjectName = "";
        subjectName = $.trim($("#txtSubjectName").val());
        if (subjectName == "") {
            alert("请填写项目名称");
            return false;
        }
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
    var region = $("input[name='rblRegion']:checked").val() || "";
    if (region == "") {
        alert("请选择区域");
        return false;
    }
    var remark = $.trim($("#txtRemark").val());
    if (subjectType == 10 && remark=="") {
        alert("请填写备注");
        return false;
    }
    $("#loadingImg").show();
   
    return true;
}
