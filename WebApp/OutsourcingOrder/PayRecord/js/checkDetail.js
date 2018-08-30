
$(function () {
    GetDetailList();

    layui.use('table', function () {
        var table = layui.table;
        table.on('tool(tbPayRecordList)', function (obj) {
            var data = obj.data;
            var id = data.Id;
            if (obj.event == 'edit') {
                var addUserId = data.AddUserId;
                if (addUserId != currUserId) {
                    layer.msg("你无权操作！");
                    return false;
                }
                $("#txtPayDate").val(data.PayDate);
                $("#txtPay").val(data.PayAmount);
                $("#txtRemark").val(data.Remark);
                layer.open({
                    type: 1,
                    time: 0,
                    title: '修改付款记录',
                    skin: 'layui-layer-rim', //加上边框
                    area: ['500px', '300px'],
                    content: $("#editDiv"),
                    id: 'popLayer2',
                    btn: ['提 交'],
                    btnAlign: 'c',
                    yes: function () {
                        var payDate = $.trim($("#txtPayDate").val());
                        if (payDate == "") {
                            alert("请填写付款日期");
                            return false;
                        }
                        var pay = $.trim($("#txtPay").val());
                        if (pay == "") {
                            alert("请填写实付金额");
                            return false;
                        }
                        else if (isNaN(pay)) {
                            alert("实付金额填写不正确");
                            return false;
                        }
                        else if (parseFloat(pay) <= 0) {
                            alert("实付金额必须大于0");
                            return false;
                        }
                        var remark = $.trim($("#txtRemark").val());
                        var jsonStr = '{"Id":' + id + ',"PayDate":"' + payDate + '","PayAmount":' + pay + ',"Remark":"' + remark + '"}';
                        $.ajax({
                            type: "get",
                            url: "list.ashx",
                            data: { type: "editDetail", jsonStr: jsonStr },
                            success: function (data) {
                                if (data == "ok") {
                                    $("#Button1").click();
                                    layer.msg("修改成功");
                                    layer.closeAll();
                                    window.parent.updateSuccess();
                                }
                                else {
                                    layer.msg("修改失败！");
                                }
                            }
                        })

                    },
                    cancel: function () {
                        layer.closeAll();
                    }
                })
            }
            else if (obj.event == 'delete') {
                var addUserId = data.AddUserId;
                if (addUserId != currUserId) {
                    layer.msg("你无权操作！");
                    return false;
                }

                layer.confirm('确定删除吗？', function (index) {
                    $.ajax({
                        type: 'get',
                        url: 'list.ashx',
                        data: { type: 'deleteDetail', id: id },
                        success: function (data) {
                            if (data == "ok") {
                                $("#Button1").click();
                                layer.msg("删除成功");
                                layer.close(index);
                                obj.del();
                                window.parent.updateSuccess();
                            }
                            else {
                                layer.msg("删除失败！");
                                layer.close(index);
                            }
                        }
                    });
                })

            }
        });
    });
})


function GetDetailList() {
    layui.use('table', function () {
        var table = layui.table;
        table.render({
            elem: '#tbPayRecordList',
            url: 'list.ashx',
            where: {
                "type": "getPayDetail",
                "guidanceId": guidanceId,
                "outsourceId": outsourceId
            },
            method: "post",
            cols: [[
                      { field: 'RowIndex', width: 60, title: '序号', style: 'color: #000;' },
                      { field: 'PayDate', width: 150, title: '付款时间', style: 'color: #000;' },
                      { field: 'PayAmount', title: '付款金额', width: 150, style: 'color: #000;' },
                      { field: 'AddUserName', width: 100, title: '提交人', style: 'color: #000;' },
                      { field: 'Remark', title: '备注', style: 'color: #000;' },
                      { field: '', width: 150, title: '操作', style: 'color: #000;', toolbar: '#barDemo' }
                    ]],
            page: false
            //skin: 'row',
            //even: false

        });
    });
}