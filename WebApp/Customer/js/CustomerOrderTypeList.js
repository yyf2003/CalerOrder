
var JsonStr = "";

$(function () {
    //添加
    $("#btnAdd").click(function () {
        ClearVal();
        $("#ddlCustomer").val($("#ddlCustomerSearch").val());
        OrderType.add("add");

    })

    $("#gv").delegate("span[name='spanEdit']", "click", function () {
        var customerId = $(this).data("customerid");
        $("#ddlCustomer").val(customerId);
        OrderType.edit();
    })
})

var OrderType = {
    add: function (optype) {
        $("#editDiv").show().dialog({
            modal: true,
            width: 620,
            height: 350,
            iconCls: 'icon-add',
            title: '订单类型设置',
            resizable: false,
            buttons: [

                    {
                        text: optype == "add" ? '添加' : '更新',
                        iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
                        visible: false,
                        handler: function () {
                            if (CheckVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/CustomerOrderTypeList.ashx",
                                    data: { type: "edit", jsonString: JsonStr },
                                    cache: false,
                                    success: function (data) {
                                        if (data == "ok") {
                                            //$("#editDiv").dialog('close');
                                            $("#btnSearch").click();
                                        }
                                        else
                                            alert("提交失败：" + data);
                                    }
                                })
                            }
                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editDiv").dialog('close');
                        }
                    }
                ]
        });
    },
    edit: function () {
        var customerId = $("#ddlCustomer").val();
        this.add("update");
        $.ajax({
            type: "get",
            url: "./Handler/CustomerOrderTypeList.ashx",
            data: { type: "getOrderType", customerId: customerId },
            success: function (data) {
                if (data != "") {
                    var arr = data.split(',');
                    if (arr.length > 0) {
                        $("#cblOrderType input").each(function () {
                            for (var i = 0; i < arr.length; i++) {
                                if ($(this).val() == arr[i])
                                    $(this).attr("checked", true);
                            }
                        })
                    }
                }
            }
        })
    }
};


function CheckVal() {
    var customerId = $("#ddlCustomer").val();
    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    var orderTypeIds = "";
    $("#cblOrderType input:checked").each(function () {
        orderTypeIds += $(this).val() + ",";
    })
    if (orderTypeIds.length > 0)
        orderTypeIds = orderTypeIds.substring(0, orderTypeIds.length - 1);
    else {
        alert("请选择订单类型");
        return false;
    }
    JsonStr = '{"CustomerId":' + customerId + ',"OrderTypeIds":"' + orderTypeIds + '"}';
    
    return true;
}

function ClearVal() {
    JsonStr = "";
    $("#cblOrderType input:checked").each(function () {
        this.checked = false;
    })
}