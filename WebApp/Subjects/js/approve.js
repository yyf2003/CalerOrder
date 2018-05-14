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
//    if (confirm("确定提交吗？")) {
//        $("#btnDiv").hide();
//       
//        return true;
//    }
//    else
    //        return false;

    layer.open({
        type: 1,
        time: 0,
        title: '提示信息',
        skin: 'layui-layer-rim', //加上边框
        area: ['450px', '200px'],
        content: $("#approveLoading"),
        id: 'loadLayer',
        closeBtn: 0

    });

}