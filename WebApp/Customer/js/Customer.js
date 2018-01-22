
var jsonStr = "";
var currCustomerId = 0;
var Customer = {
    addCustomer: function (optype) {
        $("#editDiv").show().dialog({
            modal: true,
            width: 400,
            height: 300,
            iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
            title: optype == "add" ? "添加客户" : "编辑客户",
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            if (CheckVal()) {
                                $.ajax({
                                    type: "get",
                                    url: "./Handler/Handler1.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(jsonStr),
                                    cache:false,
                                    success: function (data) {
                                        if (data == "exist") {
                                            alert("该客户已存在！");
                                        }
                                        else if (data == "ok") {
                                            window.location.href = "CustomerList.aspx";
                                        }
                                        else
                                            alert(data);
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
    editCustomer: function () {
        Customer.addCustomer("update");
    }
}
$(function () {
    //添加用户
//    $("#btnAdd").click(function () {
//        ClearVal();
//        Customer.addCustomer("add");
//    })


})
function editCustomer(obj)
{
    ClearVal();
    var td = $(obj).parent().siblings("td");
    $("#txtCode").val($.trim(td.eq(1).html()).replace("&nbsp;",""));
    $("#txtCustomerName").val($.trim(td.eq(2).html()).replace("&nbsp;", ""));
    $("#txtShortName").val($.trim(td.eq(3).html()).replace("&nbsp;", ""));
    $("#txtContact").val($.trim(td.eq(4).html()).replace("&nbsp;", ""));
    $("#txtTel").val($.trim(td.eq(5).html()).replace("&nbsp;", ""));
    currCustomerId = $(obj).data("customerid");
    Customer.editCustomer();
}


function CheckVal() {
    jsonStr = "";
    var customerCode = $("#txtCode").val();
    var customerName = $("#txtCustomerName").val();
    var shortName = $("#txtShortName").val();
    var contact = $("#txtContact").val();
    var tel = $("#txtTel").val();
    if ($.trim(customerName) == "") {
        alert("请填写客户名称");
        return false;
    }
    jsonStr = '{"Id":' + currCustomerId + ',"CustomerCode":"' + customerCode + '","CustomerName":"' + customerName + '","CustomerShortName":"' + shortName + '","Contact":"' + contact + '","Tel":"' + tel + '"}';
    return true;

}

function ClearVal() {
    $("#txtCode").val("");
    $("#txtCustomerName").val("");
    $("#txtShortName").val("");
    $("#txtContact").val("");
    $("#txtTel").val("");
    jsonStr = "";
}