
var currPOPId = 0;
var currCornerType = "";
var currFrameName = "";
var jsonStr = "";
$(function () {
    $("span[name='editPOP']").on("click", function () {

        currPOPId = $(this).data("popid");
        POP.getSheetList();
        POP.edit();

    })



    $("#txtSheet").on("click", function () {
        showSheetMenu();
    })

    $("#txtGender").on("click", function () {
        showGenderMenu();
    })

    $("#ddlMaterialCategory").change(function () {
        POP.getMaterial();
    })

    $("#ddlSheetMenu").delegate("li", "click", function () {

        $("#txtSheet").val($(this).html());
        $("#divSheetMenu").hide();
        if (currPOPId==0)
           GetGraphicNoPrefix();
        POP.getCornerType();
    })
    $("#ddlGenderMenu").delegate("li", "click", function () {

        $("#txtGender").val($(this).html());
        $("#divGenderMenu").hide();
        POP.getCornerType();
    })

    $("#ddlCornerType").change(function () {
        POP.getFrameList();
    })
})
var currOrderGraphicMaterialId = 0;
var POP = {
    getSheetList: function () {
        $("#ddlSheetMenu").html("");
        $.ajax({
            type: "get",
            url: "/Customer/Handler/POPList.ashx?type=getSheetList",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var li = "<li>" + json[i].SheetName + "</li>";
                        $("#ddlSheetMenu").append(li);
                    }
                }
            }
        })
    },
    getMaterialCategory: function (cid) {
        document.getElementById("ddlMaterialCategory").length = 1;
        $.ajax({
            type: "get",
            url: "/Customer/Handler/POPList.ashx?type=getMaterialCategory",
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    var isSelected = false;
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (cid == json[i].Id) {
                            selected = "selected='selected'";
                            isSelected = true;
                        }
                        var li = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].CategoryName + "</option>";
                        $("#ddlMaterialCategory").append(li);
                    }
                    if (isSelected) {
                        POP.getMaterial();
                    }
                }
            }
        })
    },
    getMaterial: function () {
        document.getElementById("ddlMaterial").length = 1;
        var categoryId = $("#ddlMaterialCategory").val();
        $.ajax({
            type: "get",
            url: "/Customer/Handler/POPList.ashx",
            data: { type: "getOrderMaterial", categoryId: categoryId },
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (currOrderGraphicMaterialId == json[i].Id) {
                            selected = "selected='selected'";

                        }
                        var li = "<option value='" + json[i].Id + "' " + selected + ">" + json[i].OrderMaterialName + "</option>";
                        $("#ddlMaterial").append(li);
                    }
                }
            }
        })
    },
    getCornerType: function () {
        document.getElementById("ddlCornerType").length = 1;
        var sheet = $.trim($("#txtSheet").val());
        var gender = $.trim($("#txtGender").val());
        $.ajax({
            type: "get",
            url: "/Customer/Handler/POPList.ashx?type=getCornerType&sheet=" + sheet + "&shopId=" + shopId + "&gender=" + gender,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].CornerType == currCornerType) {
                            selected = "selected='selected'";
                        }
                        var option = "<option value='" + json[i].CornerType + "' " + selected + ">" + json[i].CornerType + "</option>";
                        $("#ddlCornerType").append(option);
                    }

                }
                POP.getFrameList();
            }
        })
    },
    getFrameList: function () {
        var sheet = $.trim($("#txtSheet").val());
        var cornerType = $("#ddlCornerType").val();
        var gender = $.trim($("#txtGender").val());
        document.getElementById("ddlFrameName").length = 1;
        $.ajax({
            type: "get",
            url: "/Customer/Handler/POPList.ashx?type=getFrameList&sheet=" + sheet + "&cornerType=" + cornerType+ "&shopId=" + shopId+ "&gender=" + gender,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    for (var i = 0; i < json.length; i++) {
                        var selected = "";
                        if (json[i].FrameName == currFrameName) {
                            selected = "selected='selected'";

                        }
                        var option = "<option value='" + json[i].FrameName + "' " + selected + ">" + json[i].FrameName + "</option>";
                        $("#ddlFrameName").append(option);
                    }

                }
            }
        })
    },
    edit: function () {
        $.ajax({
            type: "get",
            url: "/Customer/Handler/POPList.ashx?type=getPOP&id=" + currPOPId,
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    $("#txteditShopNo").val(json[0].ShopNo);

                    $("#txtPositionDescription").val(json[0].PositionDescription);
                    $("#txtGraphicNo").val(json[0].GraphicNo);
                    $("#txtSheet").val(json[0].Sheet);
                    $("#txtGender").val(json[0].Gender);
                    $("#txtQuantity").val(json[0].Quantity);
                    $("#txtGraphicWidth").val(json[0].GraphicWidth);
                    $("#txtGraphicLength").val(json[0].GraphicLength);
                    $("#txtWindowWide").val(json[0].WindowWide);
                    $("#txtWindowHigh").val(json[0].WindowHigh);
                    $("#txtWindowDeep").val(json[0].WindowDeep);
                    $("#txtWindowSize").val(json[0].WindowSize);

                    $("#seleWindowLeftSide").val(json[0].LeftSideStick);
                    $("#seleWindowRightSide").val(json[0].RightSideStick);
                    $("#seleWindowFloor").val(json[0].Floor);

                    currOrderGraphicMaterialId = json[0].OrderGraphicMaterialId;
                    POP.getMaterialCategory(json[0].MaterialCategoryId);
                    //                    $("#txtGlass").val(json[0].Glass);
                    //                    $("#txtBackdrop").val(json[0].Backdrop);
                    //                    $("#txtCategory").val(json[0].Category);
                    //                    $("#txtIsElectricity").val(json[0].IsElectricity);
                    //                    $("#txtIsHang").val(json[0].IsHang);
                    //                    $("#txtDoorPosition").val(json[0].DoorPosition);
                    $("#txtOOHInstallPrice").val(json[0].OOHInstallPrice);
                    $("#txtOSOOHInstallPrice").val(json[0].OSOOHInstallPrice);
                    $("#txtRemark").val(json[0].Remark);
                    currCornerType = json[0].CornerType;
                    currFrameName = json[0].FrameName;
                    POP.getCornerType();
                    $("input:radio[name='radioIsValid']").each(function () {
                        if ($(this).val() == json[0].IsValid) {
                            this.checked = true;
                        }
                    })
                }
                $("#editDiv").show().dialog({
                    modal: true,
                    width: 750,
                    height: 450,
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
                                        url: "/Customer/Handler/POPList.ashx",
                                        data: { type: "edit", jsonString: urlCodeStr(jsonStr) },
                                        success: function (data) {
                                            if (data == "ok") {
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
        })
    },
    add: function (ShopNo) {
        currPOPId = 0;
        ClearVal();
        if (ShopNo) {
            $("#txteditShopNo").val(ShopNo);
        }

        $("#editDiv").show().dialog({
            modal: true,
            width: 750,
            height: 450,
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
                                        url: "/Customer/Handler/POPList.ashx",
                                        data: { type: "edit", jsonString: urlCodeStr(jsonStr) },
                                        success: function (data) {
                                            if (data == "ok") {

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

function showSheetMenu() {

    $("#divSheetMenu").css({ left: -1 + "px", top: 20 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function showGenderMenu() {

    $("#divGenderMenu").css({ left: -1 + "px", top: 20 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function hideMenu() {
    $("#divSheetMenu,#divGenderMenu").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "txtSheet" || event.target.id == "divSheetMenu" || $(event.target).parents("#divSheetMenu").length > 0 || event.target.id == "txtGender" || event.target.id == "divGenderMenu" || $(event.target).parents("#divGenderMenu").length > 0)) {
        hideMenu();
    }

}

function CheckVal() {
    var ShopNo = $.trim($("#txteditShopNo").val());
    var PositionDescription = $.trim($("#txtPositionDescription").val());
    var GraphicNo = $.trim($("#txtGraphicNo").val());
    var Sheet = $.trim($("#txtSheet").val());
    var Gender = $.trim($("#txtGender").val());
    var Quantity = $.trim($("#txtQuantity").val());
    var GraphicWidth = $.trim($("#txtGraphicWidth").val());
    var GraphicLength = $.trim($("#txtGraphicLength").val());
    var WindowWide = $.trim($("#txtWindowWide").val()) || 0;
    var WindowHigh = $.trim($("#txtWindowHigh").val()) || 0;
    var WindowDeep = $.trim($("#txtWindowDeep").val())||0;
    var WindowSize = $.trim($("#txtWindowSize").val());
    //var GraphicMaterial = $.trim($("#txtGraphicMaterial").val());
    var MaterialCategoryId = $("#ddlMaterialCategory").val();
    var OrderGraphicMaterialId = $("#ddlMaterial").val();
    var GraphicMaterial = $("#ddlMaterial option:selected").text();
//    var DoubleFace = $.trim($("#txtDoubleFace").val());
//    var Glass = $.trim($("#txtGlass").val());
//    var Backdrop = $.trim($("#txtBackdrop").val());
//    var Category = $.trim($("#txtCategory").val());
//    var IsElectricity = $.trim($("#txtIsElectricity").val());
//    var IsHang = $.trim($("#txtIsHang").val());
    //    var DoorPosition = $.trim($("#txtDoorPosition").val());

    var CornerType = $("#ddlCornerType").val();
    var frameName = $("#ddlFrameName").val();
    var OOHInstallPrice = $.trim($("#txtOOHInstallPrice").val());
    var OSOOHInstallPrice = $.trim($("#txtOSOOHInstallPrice").val());
    var Remark = $.trim($("#txtRemark").val());
    var IsValid = $("input:radio[name='radioIsValid']:checked").val() || 1;

    var LeftSide = $("#seleWindowLeftSide").val();
    var RightSide = $("#seleWindowRightSide").val();
    var Floor = $("#seleWindowFloor").val();

    if (ShopNo == "") {
        alert("请填写店铺编号");
        return false;
    }
    if (Sheet == "") {
        alert("请填写POP位置");
        return false;
    }
    if (GraphicNo == "") {
        alert("请填写POP编号");
        return false;
    }
    if (Gender == "") {
        alert("请填写性别");
        return false;
    }
    if (Quantity == "") {
        alert("请填写数量");
        return false;
    }
    else if (isNaN(Quantity)) {
        alert("数量填写不正确");
        return false;
    }
    if (GraphicWidth == "") {
        alert("请填写POP宽");
        return false;
    }
    else if (isNaN(GraphicWidth)) {
        alert("POP宽填写不正确");
        return false;
    }
    if (GraphicLength == "") {
        alert("请填写POP高");
        return false;
    }
    else if (isNaN(GraphicLength)) {
        alert("POP高填写不正确");
        return false;
    }
    if (WindowWide != "" && isNaN(WindowWide)) {
        alert("位置宽填写不正确");
        return false;
    }
    if (WindowHigh != "" && isNaN(WindowHigh)) {
        alert("位置高填写不正确");
        return false;
    }
    if (WindowDeep != "" && isNaN(WindowDeep)) {
        alert("位置深填写不正确");
        return false;
    }
    if (OrderGraphicMaterialId == 0) {
        alert("请选择材质");
        return false;
    }
    if (OOHInstallPrice != "" && isNaN(OOHInstallPrice)) {
        alert("应收户外安装费填写不正确");
        return false;
    }
    if (OSOOHInstallPrice != "" && isNaN(OSOOHInstallPrice)) {
        alert("应付户外安装费填写不正确");
        return false;
    }
    OOHInstallPrice = OOHInstallPrice.length > 0 ? OOHInstallPrice : "0";
    OSOOHInstallPrice = OSOOHInstallPrice.length > 0 ? OSOOHInstallPrice : "0";
    //jsonStr = '{"Id":' + currPOPId + ',"ShopNo":"' + ShopNo + '","PositionDescription":"' + PositionDescription + '","GraphicNo":"' + GraphicNo + '","Sheet":"' + Sheet + '","Gender":"' + Gender + '","Quantity":' + Quantity + ',"GraphicWidth":"' + GraphicWidth + '","GraphicLength":"' + GraphicLength + '","WindowWide":"' + WindowWide + '","WindowHigh":"' + WindowHigh + '","WindowDeep":"' + WindowDeep + '","WindowSize":"' + WindowSize + '","GraphicMaterial":"' + GraphicMaterial + '","DoubleFace":"' + DoubleFace + '","Glass":"' + Glass + '","Backdrop":"' + Backdrop + '","Category":"' + Category + '","IsElectricity":"' + IsElectricity + '","IsHang":"' + IsHang + '","DoorPosition":"' + DoorPosition + '","Remark":"' + Remark + '","IsValid":' + IsValid + ',"OOHInstallPrice":"' + OOHInstallPrice + '","MaterialCategoryId":' + MaterialCategoryId + ',"OrderGraphicMaterialId":' + OrderGraphicMaterialId + ',"CornerType":"' + CornerType + '"}';

    jsonStr = '{"Id":' + currPOPId + ',"ShopId":' + shopId + ',"ShopNo":"' + ShopNo + '","PositionDescription":"' + PositionDescription + '","GraphicNo":"' + GraphicNo + '","Sheet":"' + Sheet + '","Gender":"' + Gender + '","Quantity":' + Quantity + ',"GraphicWidth":"' + GraphicWidth + '","GraphicLength":"' + GraphicLength + '","WindowWide":"' + WindowWide + '","WindowHigh":"' + WindowHigh + '","WindowDeep":"' + WindowDeep + '","WindowSize":"' + WindowSize + '","GraphicMaterial":"' + GraphicMaterial + '","Remark":"' + Remark + '","IsValid":' + IsValid + ',"OOHInstallPrice":"' + OOHInstallPrice + '","MaterialCategoryId":' + MaterialCategoryId + ',"OrderGraphicMaterialId":' + OrderGraphicMaterialId + ',"CornerType":"' + CornerType + '","MachineFrameName":"' + frameName + '","OSOOHInstallPrice":"' + OSOOHInstallPrice + '","LeftSideStick":"' + LeftSide + '","RightSideStick":"' + RightSide + '","Floor":"' + Floor + '"}';
    return true;
}

function ClearVal() {
    $("#editDiv").find("input").val("");
    $("#txtSheet").attr("disabled", false);
    document.getElementById("ddlCornerType").length = 1;
    document.getElementById("ddlFrameName").length = 1;
    document.getElementById("ddlMaterial").length = 1;

    $("#seleWindowLeftSide").val("");
    $("#seleWindowRightSide").val("");
    $("#seleWindowFloor").val("");
}

function loadingSearch() {
   
    $("#loadingSearch").show();
    return true;
}

function loading() {
    $("#loadingImg").show();
    checkExport();
    return true;
}

var timer;
function checkExport() {
    timer = setInterval(function () {
        $.ajax({
            type: "get",
            //url: "handler/CheckExportDetail.ashx",
            url: "handler/CheckExportState.ashx?type=pop",
            cache: false,
            success: function (data) {

                if (data == "ok") {
                    $("#loadingImg").hide();
                    clearInterval(timer);
                }

            }
        })

    }, 1000);
}

function loadSearch() {
    $("#loadingSearch").show();
   
    return true;
}

function GetGraphicNoPrefix() {
    var sheet = $.trim($("#txtSheet").val());
    $("#txtGraphicNo").val("");
    $.ajax({
        type: "get",
        url: "/Customer/Handler/POPList.ashx?type=getGraphicNoPrefix&sheet=" + sheet + "&shopId=" + shopId,
        success: function (data) {
            $("#txtGraphicNo").val(data);
        }
    })
}
