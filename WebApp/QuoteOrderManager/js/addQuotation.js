
$(function () {
    $("input[name='btnOOHDec'],input[name='btnBasicDec']").click(function (event) {

        var val = $(this).next("input").val() || 0;
        if (val > 0) {
            val = parseInt(val) - 1;
        }
        else {
            val = 0;
        }
        $(this).next("input").val(val);

        var td = $(this).parent();
        var total = 0;
        td.find("input[type='text']").each(function () {
            var count = $(this).val() || 0;
            var price = $(this).data("val") || 0;
            total += (parseInt(count) * parseInt(price));
        })
        td.find("span").html(total);
        CountTotal();
    })

    $("input[name='btnOOHAdd'],input[name='btnBasicAdd']").click(function () {
        var val = $(this).prev("input").val() || 0;
        if (val < 100)
            val = parseInt(val) + 1;
        $(this).prev("input").val(val);


        var td = $(this).parent();
        var total = 0;
        td.find("input[type='text']").each(function () {
            var count = $(this).val() || 0;
            var price = $(this).data("val") || 0;
            total += (parseInt(count) * parseInt(price));
        })
        td.find("span").html(total);
        CountTotal();

    })

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

function CountTotal() {
    var total0 = 0;
    var total1 = $("#hfQuoteTotalPrice").val() || 0;
    $("#otherInstallPriceTable span").each(function () {
        if ($(this).prop("id").indexOf("labOOHTotal") != -1 || $(this).prop("id").indexOf("labBasicTotal") != -1) {
            var price = $(this).text() || 0;
            total0 += parseFloat(price);
        }
    })
    total0 += parseFloat(total1);
    $("#labQuoteTotalPrice").text(total0);
    $("#hfQuoteTotalPrice1").val(total0);
}

function Check() {
    var price = $("#hfQuoteTotalPrice1").val() || 0;
    if (parseFloat(price) == 0) {
        layer.msg("报价金额为0，提交失败！");
        return false;
    }
    return true;
}
