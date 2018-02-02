
var currCustomerId = 0; //选中的报价的客户ID
var CSUserId = 0;
var outsourceId = 0;
var oohInstallOutsourceId = 0
var bcsOutsourceId = 0;
var currShopId = 0;
var currRegion = "";
var currProvinceId = 0;
var currCityId = 0;
var currAreaId = 0;
var jsonStr = "";
$(function () {
    CheckPrimission(url, null, $("#btnAddNewShop"), null, null, null, null, $("#btnCheckEditLog"));
    // CheckPrimission(url, null, new Array($("#btnAddNewShop"),$("#btnCheckEditLog")), null, null, null, null);
    shop.bindCustomer();
    $("span[name='checkShop']").on("click", function () {
        ClearVal();
        currShopId = $(this).data("shopid");
        shop.check();
    })

    $("span[name='editShop']").on("click", function () {
        ClearVal();
        currShopId = $(this).data("shopid");
        shop.bindChannel();
        shop.edit();
    })

    $("#selCustomer").change(function () {
        shop.bindRegion();

    })

    $("#seleRegion").change(function () {
        shop.bindProvince();
        shop.getCustomerService();
    })
    $("#seleProvince").change(function () {
        shop.bindCity();
        shop.getCustomerService();
    })
    $("#seleCity").change(function () {
        shop.bindArea();

    })


})

function AddNewShop() {
    ClearVal();
    shop.getOutsourceList();
    //shop.getOOHInstallOutsourceList();
    shop.bindChannel();
    shop.add();
}


var shop = {
    bindCustomer: function () {
        $.ajax({
            type: "get",
            url: "../../Materials/Handler/SetPrice.ashx?type=getCustomer",
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
    bindRegionMenu: function () {
        var customerid = $("#selCustomer").val();

        $("#ddlRegionMenu").html("");
        if (customerid > 0) {
            $.ajax({
                type: "get",
                url: "../../Materials/Handler/SetPrice.ashx?type=getRegion&customerId=" + customerid,
                cache: false,
                success: function (data) {

                    if (data != "") {

                        var json = eval(data);
                        for (var i = 0; i < json.length; i++) {
                            var li = "<li>" + json[i].RegionName + "</li>";
                            $("#ddlRegionMenu").append(li);
                        }

                    }

                },
                complete: function () {
                    shop.bindProvinceMenu();
                }
            })
        }

    },
    bindProvinceMenu: function () {
        var region = $("#txtRegion").val();
        $("#ddlProvinceMenu").html("");
        if (region != "") {
            $.ajax({
                type: "get",
                url: "./Handler/Shops.ashx?type=GetProvince&region=" + region,
                cache: false,
                success: function (data) {
                    if (data != "") {

                        var json = eval(data);
                        for (var i = 0; i < json.length; i++) {

                            var li = "<li>" + json[i].ProvinceName + "</li>";
                            $("#ddlProvinceMenu").append(li);
                        }

                    }

                },
                complete: function () {
                    shop.bindCityMenu();
                }
            })
        }
    },
    bindCityMenu: function () {
        var province = $("#txtProvince").val();
        $("#ddlCityMenu").html("");
        if (province != "") {
            $.ajax({
                type: "get",
                url: "./Handler/Shops.ashx?type=GetCity&province=" + province,
                cache: false,
                success: function (data) {
                    if (data != "") {

                        var json = eval(data);
                        for (var i = 0; i < json.length; i++) {

                            var li = "<li>" + json[i].CityName + "</li>";
                            $("#ddlCityMenu").append(li);
                        }

                    }

                }
            })
        }
    },
    getCustomerService: function () {//获取客服
        var customerid = $("#selCustomer").val();
        //var region = $.trim($("#txtRegion").val());
        //var province = $.trim($("#txtProvince").val());
        var region = $("#seleRegion").find("option:selected").text();
        var province = $("#seleProvince").find("option:selected").text();
        document.getElementById("selCSUser").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=GetCSUser&customerId=" + customerid + "&region=" + region + "&province=" + province,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var obj = $("#selCSUser").find("option[value='" + json[i].UserId + "']");
                        if (($(obj).val() || "") == "") {
                            var selected = "";
                            if (CSUserId == json[i].UserId)
                                selected = "selected='selected'";
                            var option = "<option value='" + json[i].UserId + "' " + selected + ">" + json[i].RealName + "</option>";
                            $("#selCSUser").append(option);
                        }

                    }
                }
            },
            complate: function () {

            }
        })

    },
    getOutsourceList: function () {
        document.getElementById("seleOutsource").length = 1;
        document.getElementById("seleBCSOutsource").length = 1;
        document.getElementById("seleOOHInstallOutsource").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=getOutsource",
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var obj = $("#seleOutsource").find("option[value='" + json[i].Id + "']");
                        if (($(obj).val() || "") == "") {
                            var selected = "";
                            if (outsourceId == json[i].Id)
                                selected = "selected='selected'";
                            var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].OutsourceName + "</option>";
                            $("#seleOutsource").append(option);


                        }

                        var obj0 = $("#seleOOHInstallOutsource").find("option[value='" + json[i].Id + "']");
                        if (($(obj0).val() || "") == "") {
                            var selected0 = "";
                            if (oohInstallOutsourceId == json[i].Id)
                                selected0 = "selected='selected'";
                            var option = "<option value='" + json[i].Id + "' " + selected0 + ">" + json[i].OutsourceName + "</option>";
                            $("#seleOOHInstallOutsource").append(option);
                        }

                        var obj1 = $("#seleBCSOutsource").find("option[value='" + json[i].Id + "']");
                        if (($(obj1).val() || "") == "") {
                            var selected1 = "";
                            if (bcsOutsourceId == json[i].Id)
                                selected1 = "selected='selected'";
                            var option = "<option value='" + json[i].Id + "' " + selected1 + ">" + json[i].OutsourceName + "</option>";
                            $("#seleBCSOutsource").append(option);

                           

                        }
                    }


                }
            },
            complate: function () {

            }
        })
    },
    getOOHInstallOutsourceList: function () {
        document.getElementById("seleOOHInstallOutsource").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=getOutsource",
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var obj = $("#seleOOHInstallOutsource").find("option[value='" + json[i].Id + "']");
                        if (($(obj).val() || "") == "") {
                            var selected = "";
                            if (oohInstallOutsourceId == json[i].Id)
                                selected = "selected='selected'";
                            var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].OutsourceName + "</option>";
                            $("#seleOOHInstallOutsource").append(option);
                        }

                    }
                }
            },
            complate: function () {

            }
        })
    },
    bindChannel: function () {

        $("#ddlChannelEditMenu").html("");
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=getChannel",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var li = "<li>" + json[i].Channel + "</li>";
                        $("#ddlChannelEditMenu").append(li);
                    }
                }
            }
        })
    },
    bindFormat: function () {
        var channel1 = $.trim($("#txtChannel").val());
        $("#ddlFormatEditMenu").html("");
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=getFormat&channel=" + channel1,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var li = "<li>" + json[i].Format + "</li>";
                        $("#ddlFormatEditMenu").append(li);
                    }
                }
            }
        })
    },
    check: function () {
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=checkshop&shopid=" + currShopId,
            success: function (data) {

                if (data != "") {

                    var shop = eval("(" + data + ")");
                    $("#labClientName").html(shop[0].Customer);
                    $("#labRegion").html(shop[0].RegionName);
                    $("#labProvince").html(shop[0].ProvinceName);
                    $("#labCity").html(shop[0].CityName);
                    $("#labArea").html(shop[0].AreaName);
                    $("#labPOSCode").html(shop[0].ShopNo);
                    $("#labPOSName").html(shop[0].ShopName);
                    $("#labCustomerCode").html(shop[0].AgentCode);
                    $("#labCustomerName").html(shop[0].AgentName);
                    $("#labCityTier").html(shop[0].CityTier);
                    $("#labInstall").html(shop[0].IsInstall);
                    $("#labBCSInstall").html(shop[0].BCSIsInstall);
                    $("#labContact1").html(shop[0].Contact1);
                    $("#labTel1").html(shop[0].Tel1);
                    $("#labContact2").html(shop[0].Contact2);
                    $("#labTel2").html(shop[0].Tel2);
                    $("#labChannel").html(shop[0].Channel);
                    $("#labFormat").html(shop[0].Format);
                    $("#labLocationType").html(shop[0].LocationType);
                    $("#labBusinessModel").html(shop[0].BusinessModel);
                    $("#labOpenDate").html(shop[0].OpeningDate);
                    $("#labStatus").html(shop[0].Status);
                    $("#labShopLevel").html(shop[0].POSScale);
                    $("#labPOSAddress").html(shop[0].POPAddress);
                    $("#labCSUserName").html(shop[0].CSUserName);
                    $("#labRemark").html(shop[0].Remark);
                    $("#labBasicInstallPrice").html(shop[0].BasicInstallPrice);
                    $("#labOutsourceInstallPrice").html(shop[0].OutsourceInstallPrice);
                    $("#labBCSInstallPrice").html(shop[0].BCSInstallPrice);
                    $("#labOutsourceBCSInstallPrice").html(shop[0].OutsourceBCSInstallPrice);
                    $("#labShopType").html(shop[0].ShopType);
                    $("#labOutsourceName").html(shop[0].OutsourceName);
                    $("#labOOHOutsourceName").html(shop[0].OOHOutsourceName);
                    $("#labBCSOutsourceName").html(shop[0].BCSOutsourctName);
                    $("#checkDiv").show().dialog({
                        modal: true,
                        width: 820,
                        height: 564,
                        iconCls: 'icon-add',
                        resizable: false
                    })
                }
            }
        })

    },
    edit: function () {

        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=checkshop&shopid=" + currShopId,
            success: function (data) {

                if (data != "") {

                    var shopJson = eval("(" + data + ")");

                    $("#selCustomer").val(shopJson[0].CustomerId);
                    if ($("#selCustomer").val() != "0") {
                        $("#selCustomer").attr("disabled", "disabled");
                    }
                    currRegion = shopJson[0].RegionName;
                    currProvinceId = shopJson[0].ProvinceId;
                    currCityId = shopJson[0].CityId;
                    currAreaId = shopJson[0].AreaId;

                    $("#txtPOSCode").val(shopJson[0].ShopNo);
                    $("#txtPOSName").val(shopJson[0].ShopName);
                    $("#txtAgentNo").val(shopJson[0].AgentCode);
                    $("#txtAgentName").val(shopJson[0].AgentName);
                    //$("#txtCityTier").val(shop[0].CityTier);
                    $("#seleCityTier").val(shopJson[0].CityTier);
                    $("#seleIsInstall").val(shopJson[0].IsInstall);
                    $("#seleBCSIsInstall").val(shopJson[0].BCSIsInstall);
                    $("#txtContact1").val(shopJson[0].Contact1);
                    $("#txtTel1").val(shopJson[0].Tel1);
                    $("#txtContact2").val(shopJson[0].Contact2);
                    $("#txtTel2").val(shopJson[0].Tel2);
                    $("#txtChannel").val(shopJson[0].Channel);
                    $("#txtFormat").val(shopJson[0].Format);
                    $("#txtLocationType").val(shopJson[0].LocationType);
                    $("#txtBusinessModel").val(shopJson[0].BusinessModel);
                    $("#txtOpenDate").val(shopJson[0].OpeningDate);

                    var selected = false;
                    $("input:radio[name$='rblStatus']").each(function () {
                        if ($(this).val() == shopJson[0].Status) {
                            this.checked = selected = true;
                        }
                    })
                    if (!selected) {
                        $("input:radio[name$='rblStatus']").eq(0).checked = true;
                    }
                    $("#txtShopLevel").val(shopJson[0].POSScale);
                    $("#txtAddress").val(shopJson[0].POPAddress);
                    $("#txtRemark").val(shopJson[0].Remark);
                    $("#txtBasicInstallPrice").val(shopJson[0].BasicInstallPrice);
                    $("#txtBCSInstallPrice").val(shopJson[0].BCSInstallPrice);
                    $("#txtOutsourceInstallPrice").val(shopJson[0].OutsourceInstallPrice);
                    $("#txtOutsourceBCSInstallPrice").val(shopJson[0].OutsourceBCSInstallPrice);
                    $("#txtShopType").val(shopJson[0].ShopType);
                    CSUserId = shopJson[0].CSUserId || 0;
                    currCustomerId = shopJson[0].CustomerId;
                    outsourceId = shopJson[0].OutsourceId;
                    oohInstallOutsourceId = shopJson[0].OOHInstallOutsourceId;
                    bcsOutsourceId = shopJson[0].BCSOutsourceId;
                    shop.getOutsourceList();
                    //shop.getOOHInstallOutsourceList();
                    $("#editDiv").show().dialog({
                        modal: true,
                        width: 870,
                        height: 571,
                        iconCls: 'icon-add',
                        resizable: false,
                        buttons: [
                        {
                            text: '确定',
                            iconCls: 'icon-ok',
                            handler: function () {
                                if (CheckVal()) {
                                    $.ajax({
                                        type: "get",
                                        url: "./Handler/Shops.ashx?type=edit&jsonString=" + escape(jsonStr),
                                        success: function (data) {
                                            if (data == "exist") {
                                                alert("店铺编号重复！");

                                            }
                                            else if (data == "ok") {
                                                //window.location.href = "ShopList.aspx";
                                                $("#editDiv").dialog('close');
                                                $("#btnSearch").click();

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
                    })

                }
            },
            complete: function () {
                shop.bindRegion();
                shop.bindFormat();
                //shop.getCustomerService();
            }
        })
    },
    bindRegion: function () {
        document.getElementById("seleRegion").length = 1;
        document.getElementById("seleProvince").length = 1;
        document.getElementById("seleCity").length = 1;
        document.getElementById("seleArea").length = 1;
        var customerid = $("#selCustomer").val();
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=bindRegion&customerId=" + customerid,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].RegionName == currRegion) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].RegionName + "</option>";
                        $("#seleRegion").append(option);
                    }
                    if (isSelected) {
                        shop.bindProvince();
                        shop.getCustomerService();
                    }
                }
            }
        })
    },
    bindProvince: function () {
        var regionId = $("#seleRegion").val();
        document.getElementById("seleProvince").length = 1;
        document.getElementById("seleCity").length = 1;
        document.getElementById("seleArea").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=bindProvince&regionId=" + regionId,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].Id == currProvinceId) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].ProvinceName + "</option>";
                        $("#seleProvince").append(option);
                    }
                    if (isSelected) {
                        shop.bindCity();
                        shop.getCustomerService();
                    }
                }
            }
        })
    },
    bindCity: function () {
        var provinceId = $("#seleProvince").val();
        document.getElementById("seleCity").length = 1;
        document.getElementById("seleArea").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=bindCity&provinceId=" + provinceId,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].Id == currCityId) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].PlaceName + "</option>";
                        $("#seleCity").append(option);
                    }
                    if (isSelected) {
                        shop.bindArea();
                    }
                }
            }
        })
    },
    bindArea: function () {
        var cityId = $("#seleCity").val();
        document.getElementById("seleArea").length = 1;
        $.ajax({
            type: "get",
            url: "./Handler/Shops.ashx?type=bindArea&cityId=" + cityId,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);

                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].Id == currAreaId) {
                            selected = "selected='selected'";

                        }
                        var option = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].PlaceName + "</option>";
                        $("#seleArea").append(option);
                    }

                }
            }
        })
    },
    add: function () {

        $("#editDiv").show().dialog({
            modal: true,
            width: 870,
            height: 571,
            iconCls: 'icon-add',
            resizable: false,
            buttons: [
                        {
                            text: '确定',
                            iconCls: 'icon-ok',
                            handler: function () {
                                if (CheckVal()) {
                                    $.ajax({
                                        type: "get",
                                        url: "./Handler/Shops.ashx?type=add&jsonString=" + escape(jsonStr),
                                        success: function (data) {
                                            if (data == "exist") {
                                                alert("店铺编号重复！");

                                            }
                                            else if (data == "ok") {
                                                //window.location.href = "ShopList.aspx";
                                                $("#editDiv").dialog('close');
                                                $("#btnSearch").click();

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
        })
    }
};

function showChannelMenu() {

    $("#divChannelEditMenu").css({ left: -1 + "px", top: 20 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function showFormatMenu() {

    $("#divFormatEditMenu").css({ left: -1 + "px", top: 20 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function showRegionMenu() {

    $("#divRegionMenu").css({ left: -1 + "px", top: 20 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function showProvinceMenu() {

    $("#divProvinceMenu").css({ left: -1 + "px", top: 20 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function showCityMenu() {

    $("#divCityMenu").css({ left: -1 + "px", top: 20 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function hideMenu() {
    $("#divChannelEditMenu,#divFormatEditMenu").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "txtChannel" || event.target.id == "divChannelEditMenu" || $(event.target).parents("#divChannelEditMenu").length > 0 || event.target.id == "txtFormat" || event.target.id == "divFormatEditMenu" || $(event.target).parents("#divFormatEditMenu").length > 0 || event.target.id == "txtCity" || event.target.id == "divCityMenu" || $(event.target).parents("#divCityMenu").length > 0)) {
        hideMenu();
    }

}


$(function () {

    $("#txtChannel").on("click", function () {
        showChannelMenu()();
    })

    $("#txtFormat").on("click", function () {
        showFormatMenu();
    })

    $("#txtRegion").on("click", function () {

        showRegionMenu();
    })

    $("#txtProvince").on("click", function () {

        showProvinceMenu();
    })

    $("#txtCity").on("click", function () {

        showCityMenu();
    })

    $("#ddlChannelEditMenu").delegate("li", "click", function () {
        $("#txtChannel").val($(this).html());
        $("#divChannelEditMenu").hide();
        shop.bindFormat();
    })

    $("#ddlFormatEditMenu").delegate("li", "click", function () {
        $("#txtFormat").val($(this).html());
        $("#divFormatEditMenu").hide();
    })


    $("#ddlRegionMenu").delegate("li", "click", function () {
        $("#txtRegion").val($(this).html());
        shop.bindProvinceMenu();
    })
    $("#ddlProvinceMenu").delegate("li", "click", function () {
        $("#txtProvince").val($(this).html());
        shop.bindCityMenu();
    })

    $("#ddlCityMenu").delegate("li", "click", function () {
        $("#txtCity").val($(this).html());
    })
})


function CheckVal() {
    var customerId = $("#selCustomer").val();
    var RegionId = $("#seleRegion").val();
    var RegionName = $("#seleRegion").find("option:selected").text();
    var ProvinceId = $("#seleProvince").val();
    var ProvinceName = "";
    if (ProvinceId != "0")
        ProvinceName = $("#seleProvince").find("option:selected").text();
    var CityId = $("#seleCity").val();
    var CityName = "";
    if (CityId != "")
        CityName = $("#seleCity").find("option:selected").text();
    var AreaId = $("#seleArea").val();
    var AreaName = "";
    if (AreaId != "0")
        AreaName = $("#seleArea").find("option:selected").text();
    var POSCode = $.trim($("#txtPOSCode").val());
    var POSName = $.trim($("#txtPOSName").val());
    var AgentNo = $.trim($("#txtAgentNo").val());
    var AgentName = $.trim($("#txtAgentName").val());
    //var CityTier = $.trim($("#txtCityTier").val());
    var CityTier = $("#seleCityTier").val();
    var IsInstall = $("#seleIsInstall").val();
    var BCSIsInstall = $("#seleBCSIsInstall").val();
    var Contact1 = $.trim($("#txtContact1").val());
    var Tel1 = $.trim($("#txtTel1").val());
    var Contact2 = $.trim($("#txtContact2").val());
    var Tel2 = $.trim($("#txtTel2").val());
    var Channel = $.trim($("#txtChannel").val());
    var Format = $.trim($("#txtFormat").val());
    var LocationType = $.trim($("#txtLocationType").val());
    var BusinessModel = $.trim($("#txtBusinessModel").val());
    var OpeningDate = $.trim($("#txtOpenDate").val());
    //var Status = $.trim($("#txtStatus").val());

    var Status = $("input:radio[name='rblStatus']:checked").val();
    var POPAddress = $.trim($("#txtAddress").val());
    var csUserId = $("#selCSUser").val();
    var remark = $.trim($("#txtRemark").val());

    var shopLevel = $.trim($("#txtShopLevel").val());
    var basicInstallPrice = $.trim($("#txtBasicInstallPrice").val());
    var bcsInstallPrice = $.trim($("#txtBCSInstallPrice").val());
    var osInstallPrice = $.trim($("#txtOutsourceInstallPrice").val());
    var osBCSInstallPrice = $.trim($("#txtOutsourceBCSInstallPrice").val());
    var shopType = $.trim($("#txtShopType").val());
    var outsourceId = $("#seleOutsource").val();
    var oohOutsourceId = $("#seleOOHInstallOutsource").val();
    var bcsOutsourceId = $("#seleBCSOutsource").val();
    jsonStr = "";
    if (RegionId == "0") {
        alert("请填写客户");
        return false;
    }
    if (RegionId == "0") {
        alert("请填写区域");
        return false;
    }
    if (ProvinceId == "0") {
        alert("请填写省份");
        return false;
    }
    if (CityId == "0") {
        alert("请填写城市");
        return false;
    }
    if (POSCode == "") {
        alert("请填写店铺编码");
        return false;
    }
    if (POSName == "") {
        alert("请填写店铺名称");
        return false;
    }
    if (CityTier == "") {
        alert("请填写城市级别");
        return false;
    }
    if (Format == "") {
        alert("请填写Format");
        return false;
    }
    if (basicInstallPrice != "" && isNaN(basicInstallPrice)) {
        alert("基础安装费必须是数字");
        return false;
    }
    if (bcsInstallPrice != "" && isNaN(bcsInstallPrice)) {
        alert("三叶草特殊安装费必须是数字");
        return false;
    }
    if (osInstallPrice != "" && isNaN(osInstallPrice)) {
        alert("外协安装费必须是数字");
        return false;
    }
    basicInstallPrice = basicInstallPrice.length > 0 ? basicInstallPrice : 0;
    bcsInstallPrice = bcsInstallPrice.length > 0 ? bcsInstallPrice : 0;
    osInstallPrice = osInstallPrice.length > 0 ? osInstallPrice : 0;
    osBCSInstallPrice = osBCSInstallPrice.length > 0 ? osBCSInstallPrice : 0;
    jsonStr = '{"Id":' + currShopId + ',"CustomerId":' + customerId + ',"ShopName":"' + POSName + '","ShopNo":"' + POSCode + '","RegionId":' + RegionId + ',"RegionName":"' + RegionName + '","ProvinceId":' + ProvinceId + ',"ProvinceName":"' + ProvinceName + '","CityId":' + CityId + ',"CityName":"' + CityName + '","AreaId":' + AreaId + ',"AreaName":"' + AreaName + '","CityTier":"' + CityTier + '","IsInstall":"' + IsInstall + '","AgentCode":"' + AgentNo + '","AgentName":"' + AgentName + '","POPAddress":"' + POPAddress + '","Contact1":"' + Contact1 + '","Tel1":"' + Tel1 + '","Contact2":"' + Contact2 + '","Tel2":"' + Tel2 + '","Channel":"' + Channel + '","Format":"' + Format + '","LocationType":"' + LocationType + '","BusinessModel":"' + BusinessModel + '","OpeningDate":"' + OpeningDate + '","Status":"' + Status + '","CSUserId":' + csUserId + ',"Remark":"' + remark + '","BasicInstallPrice":' + basicInstallPrice + ',"ShopType":"' + shopType + '","BCSInstallPrice":' + bcsInstallPrice + ',"OutsourceInstallPrice":' + osInstallPrice + ',"OutsourceBCSInstallPrice":' + osBCSInstallPrice + ',"OutsourceId":' + outsourceId + ',"OOHInstallOutsourceId":' + oohOutsourceId + ',"BCSIsInstall":"' + BCSIsInstall + '","BCSOutsourceId":"' + bcsOutsourceId + '"}';

    return true;
}

function loadingSearch() {
    $("#loadingSearch").show();
    return true;
}


function loading1() {
    $("#loadingImg1").show();
    checkExport1();
    return true;
}

var timer1;
function checkExport1() {
    timer1 = setInterval(function () {
        $.ajax({
            type: "get",
            //url: "handler/CheckExportDetail.ashx",
            url: "handler/CheckExportState.ashx?type=shop",
            cache: false,
            success: function (data) {

                if (data == "ok") {
                    $("#loadingImg1").hide();
                    clearInterval(timer1);
                }

            }
        })

    }, 1000);
}

function loading2() {
    $("#loadingImg2").show();
    checkExport2();
    return true;
}

var timer2;
function checkExport2() {
    timer2 = setInterval(function () {
        $.ajax({
            type: "get",
            //url: "handler/CheckExportDetail.ashx",
            url: "handler/CheckExportState.ashx?type=shopPop",
            cache: false,
            success: function (data) {

                if (data == "ok") {
                    $("#loadingImg2").hide();
                    clearInterval(timer2);
                }

            }
        })

    }, 1000);
}


function loading3() {

    $("#loadingImg3").show();
    checkExport3();
    return true;
}

var timer3;
function checkExport3() {
    timer3 = setInterval(function () {
        $.ajax({
            type: "get",
            //url: "handler/CheckExportDetail.ashx",
            url: "handler/CheckExportState.ashx?type=shopFrame",
            cache: false,
            success: function (data) {

                if (data == "ok") {
                    $("#loadingImg3").hide();
                    clearInterval(timer3);
                }

            }
        })

    }, 1000);
}

function loadSearch() {
    $("#loadingSearch").show();

    return true;
}

function ClearVal() {
    currShopId = 0;
    CSUserId = 0;
    outsourceId = 0;
    oohInstallOutsourceId = 0;
    $("#editDiv input").not("input:radio[name='rblStatus']").val("");
    $("#ddlChannelEditMenu,#ddlFormatEditMenu").html("");
}