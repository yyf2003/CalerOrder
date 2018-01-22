
Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, e) {
    $("#gvTable").find("span[name$='spanEdit']").on("click", function () {
        ClearVal();

        var id = $(this).data("detailid") || 0;
        $("#hfCurrDetailId").val(id);
        if (id > 0) {
            $("#btnUpdateOrderDetail").attr("disabled", false);
        }
        $.ajax({
            type: "get",
            url: "/Subjects/Supplement/handler/EditOrder.ashx?type=getModel&id=" + id,
            cache: false,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    if (json.length > 0) {
                        $("#txtPOSCode").val(json[0].ShopNo);
                        $("#ddlSheet").val(json[0].Sheet);
                        $("#ddlLevelNum").val(json[0].LevelNum);
                        $("#txtGraphicWidth").val(json[0].GraphicWidth);
                        $("#txtGraphicLength").val(json[0].GraphicLength);
                        $("#ddlGender").val(json[0].Gender);
                        $("#txtQuantity").val(json[0].Quantity);
                        $("#hfMaterial").val(json[0].GraphicMaterial);
                        $("#txtGraphicMaterial").val(json[0].GraphicMaterial);
                        $("#txtUnitPrice").val(json[0].UnitPrice);
                        $("#txtChooseImg").val(json[0].ChooseImg);
                        $("#txtPositionDescription").val(json[0].PositionDescription);

                    }
                }
            }
        })
    })

})


$(function () {
    $("#btnUpdateOrderDetail").attr("disabled", true);

    $("span[name='btnSelectMaterial']").click(function () {
        $(".divMaterialList").show();
    })
    BindCustomerMaterial();
    //材质只能单选
    $(".customerMaterials").delegate("input[name='cbMaterial']", "change", function () {
        if (this.checked) {
            $(".customerMaterials").find("input[name='cbMaterial']").not($(this)).attr("checked", false);
        }
    })
    //提交选择好的材质
    $("span[name='btnSubmitMaterial']").click(function () {

        var div = $(this).parent().parent().parent().parent();
        var mName = "";
        var price = "";
        $(div).find(".customerMaterials").find("input[name='cbMaterial']:checked").each(function () {

            mName = $(this).siblings("span").html();
            price = $(this).siblings("span").data("price");
        })
        if (mName.length > 0) {
            $("#hfMaterial").val(mName);
            $("#txtGraphicMaterial").val(mName);
            $("#txtUnitPrice").val(price);
           

        }
        $(".divMaterialList").hide();
    })


    //新增材质
    $("span[name='btnAddMaterial']").click(function () {
        Material.bindCustomer();
        Material.add();

    })
})



function checkVal() {
    var posCode = $.trim($("#txtPOSCode").val());

    if (posCode == "") {
        alert("请填写店铺编号");
        return false;
    }
    var sheet = $("#ddlSheet").val();
    if (sheet == "0") {
        alert("请选择POP位置");
        return false;
    }
    var GraphicWidth = $.trim($("#txtGraphicWidth").val());
    if (GraphicWidth != "" && isNaN(GraphicWidth)) {
        alert("宽必须为数字");
        return false;
    }
    var GraphicLength = $.trim($("#txtGraphicLength").val());
    if (GraphicLength != "" && isNaN(GraphicLength)) {
        alert("高必须为数字");
        return false;
    }
    var gender = $("#ddlGender").val();
    if (gender == "") {
        alert("请选择性别");
        return false;
    }

    var Quantity = $.trim($("#txtQuantity").val());
    if (Quantity == "") {
        alert("请填写数量");
        return false;
    }
    else if (isNaN(Quantity)) {
        alert("数量必须为数字");
        return false;
    }
    var UnitPrice = $.trim($("#txtUnitPrice").val());
    if (UnitPrice != "" && isNaN(UnitPrice)) {
        alert("单价必须为数字");
        return false;
    }
    return true;
}

function checkFile() {

    var val = $("#FileUpload1").val();
    if (val != "") {
        var extent = val.substring(val.lastIndexOf('.') + 1);
        if (extent != "xls" && extent != "xlsx") {
            alert("只能上传Excel文件");
            return false;
        }
    }
    else {
        alert("请选择文件");
        return false;
    }
    $("#showButton").css({ display: "none" });
    $("#showWaiting").css({ display: "" });
    return true;
}

function ClearVal() {
    $("#addTable input[type='text']").val("");
    $("#addTable select").val("");
    $("#txtQuantity").val("1");

}

//获取客户材质（报价）
function BindCustomerMaterial() {
    //var region = $("#selRegion").val();
    $.ajax({
        type: "get",

        url: "/Subjects/Handler/SplitOrder1.ashx?type=getCustomerMaterial&customerId=" + customerId,
        cache: false,
        success: function (data) {

            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style=' height:20px;'><input name='cbMaterial' type='checkbox' value='" + json[i].MaterialName + "'/>&nbsp;<span data-price='" + json[i].Price + "'>" + json[i].MaterialName + "</span></div>";
                }

                $(".customerMaterials").html(div);
            }
            else
                $(".customerMaterials").html("");
        }
    })
}


var MatrailjsonStr = "";
var Material = {
    bindCustomer: function () {
        $.ajax({
            type: "get",
            url: "/Materials/Handler/SetPrice.ashx?type=getCustomer",
            cache: false,
            success: function (data) {
                if (data != "") {

                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {

                        var option = "<option value='" + json[i].Id + "'>" + json[i].CustomerName + "</option>";
                        $("#selCustomer").append(option);

                    }

                }

            }

        })
    },
    add: function () {
        $("#editDiv").show().dialog({
            modal: true,
            width: 400,
            height: 300,
            iconCls: 'icon-add',
            title: '添加客户材质',
            resizable: false,
            buttons: [
                    {
                        text: '添加',
                        iconCls: 'icon-add',
                        visible: false,
                        handler: function () {
                            if (CheckMatrailVal()) {

                                $.ajax({
                                    type: "get",
                                    url: "/Materials/Handler/CustomerMaterialList.ashx?type=edit&opType=add&jsonString=" + urlCodeStr(MatrailjsonStr),
                                    cache: false,
                                    success: function (data) {

                                        if (data == "exist") {
                                            alert("该材质已存在！");
                                        }
                                        else if (data == "ok") {
                                            alert("添加成功");
                                            $("#editDiv input").val("");
                                            BindCustomerMaterial();
                                            //$("#editDiv").dialog('close');

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
    }
}

function CheckMatrailVal() {
    MatrailjsonStr = "";
    var customerId = $("#selCustomer").val();
    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }

    var materialName = $.trim($("#txtMaterialNameAdd").val());
    if (materialName == "") {
        alert("请填写材质名称");
        return false;
    }

    var unit = $.trim($("#txtUnit").val());
    var price = $.trim($("#txtPrice").val());
    if (price == "") {
        alert("请填写价格");
        return false;
    }
    if (isNaN(price)) {
        alert("价格必须是数字");
        return false;
    }

    MatrailjsonStr = '{"Id":0,"CustomerId":' + customerId + ',"MaterialName":"' + materialName + '","Unit":"' + unit + '","Price":' + price + '}';

    return true;
}
