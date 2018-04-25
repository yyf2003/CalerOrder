
$(function () {
    $("span[name='popSheetSpan']").click(function () {
        var sheet = $(this).html();
        var url = "CheckPOPOrderDetail.aspx?sheet=" + sheet;
        layer.open({
            type: 2,
            time: 0,
            title: '查看订单明细',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '90%'],
            content: url,
            id: 'layer1',

            cancel: function (index) {
                layer.closeAll();
            }

        });
    })
})