$(function () {

   
    $("#txtRemark").on("keyup", function () {
        var val = $(this).val();
        if (val.length > 100) {
            $(this).val(val.substring(0, 100));
        }
    })
})


function Check() {
    var result = $("input:radio[name='rblApproveResult']:checked").val() || 0;
    if (result == 0) {
        alert("请选择审批结果");
        return false;
    }
    if (result == 2) {
        if ($.trim($("#txtRemark").val()) == "") {
            alert("请填写审批意见");
            return false;
        }
    }
    //return confirm("确定提交吗？");
    if (confirm("确定提交吗？")) {
        $("#btnDiv").hide();
        $("#loadingApprove").show();
        return true;
    }
    else
        return false;
}