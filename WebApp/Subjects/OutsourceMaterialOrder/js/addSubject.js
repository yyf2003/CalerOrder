

function getMonth() {
    $("#txtGuidanceMonth").blur();
}

$(function () {

})

function CheckMaterialVal() {
    var material = $("#ddlMaterial").val();
    var amount = $.trim($("#txtMaterialAmount").val());
    if (material == "0") {
        layer.msg("请选择物料");
        return false;
    }
    if (amount == "") {
        layer.msg("请填写数量");
        return false;
    }
    else if (isNaN(amount)) {
        layer.msg("数量填写不正确");
        return false;
    }
    else if (parseFloat(amount)==0) {
        layer.msg("数量必须大于0");
        return false;
    }
    return true;
}

function CheckSubmitVal() {
    var customerId = $("#ddlCustomer").val();
    var guidanceId = $("#ddlGuidance").val();
    var subjectName = $.trim($("#txtSubjectName").val());
    var goutsourceId = $("#ddlOutsource").val();
    if (customerId == "0") {
        layer.msg("请选择客户");
        return false;
    }
    if (guidanceId == "0") {
        layer.msg("请选择活动");
        return false;
    }
    if (subjectName == "") {
        layer.msg("请填写项目名称");
        return false;
    }
    if (goutsourceId == "0") {
        layer.msg("请选择外协");
        return false;
    }
    var len = $("#tbMaterialData").find("tr.materialData").length;
    if (len == 0) {
        layer.msg("请添加物料信息");
        return false;
    }
    $("#ImgLoad").show();
    return true;
}