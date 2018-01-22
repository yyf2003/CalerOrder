
$(function () {
    InitOrdrInfo();

    $(".showDll").on("click", function () {
        $(".showDll").not($(this)).siblings("div").hide();
        $(this).siblings("div").css({ left: 4 + "px", top: 23 + "px", "z-index": 100 }).fadeIn("fast");
        $("body").bind("mousedown", onBodyDown);
    })

    $("#myTabContent").delegate("li[name='dllVal']", "click", function () {

        var tb = $(this).parent().parent().siblings("input[type='text']");
        $(tb).val($(this).html());
//        if ($(tb).attr("id") == "txtAddListSheet") {
//            if ($(this).html().indexOf("桌") != -1) {
//                $("#ddlAddListLevelNum").attr("disabled", false);
//            }
//            else {
//                $("#ddlAddListLevelNum").val("0").attr("disabled", true);
//            }
//        }
    })

//    $("#txtAddListSheet").on("blur", function () {
//        var val = $(this).val();
//        if (val.indexOf("桌") != -1) {
//            $("#ddlAddListLevelNum").attr("disabled", false);
//        }
//        else {
//            $("#ddlAddListLevelNum").val("0").attr("disabled", true);
//        }
//    })

})

//初始化添加框的下拉菜单
function InitOrdrInfo() {
    $.ajax({
        type: "get",
        url: "../Handler/EditOrderDetails.ashx?type=getOrderInfo",
        success: function (data) {
            if (data != "") {
                var json = eval(data);
                var MaterialSupport = json[0].MaterialSupport;
                var POSScale = json[0].POSScale;
                var Sheet = json[0].Sheet;
                var Gender = json[0].Gender;
                var LevelNum=json[0].LevelNum;
                if (MaterialSupport.length > 0) {
                    InitDll($("ul[name='MaterialSupport']"), MaterialSupport,130);
                }
                if (POSScale.length > 0) {
                    InitDll($("ul[name='POSScale']"), POSScale,100);
                }
                if (Sheet.length > 0) {
                    InitDll($("ul[name='Sheet']"), Sheet);
                }
                if (Gender.length > 0) {
                    InitDll($("ul[name='Gender']"), Gender);
                }
                //if (LevelNum.length > 0) {
                    //InitDll($("ul[name='LevelNum']"), LevelNum);
                //}
            }
        }
    })
}

function InitDll(obj, jsonStr,width) {
    var arr = jsonStr.split(',');
    
    for (var i = 0; i < arr.length; i++) {
        var li = "<li name='dllVal'>" + arr[i] + "</li>";
        $(obj).append(li);
    }
    if (width!=null) {
        $(obj).parent("div").css({ width: (width+20) + "px" });
        $(obj).css({ width: width + "px" });
        $(obj).find("li").css({ width: width + "px" });
    }
}

function hideMenu() {
    $(".showDll").not($(this)).siblings("div").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "txtAddListMaterialSupport" || event.target.id == "divListMaterialSupport" || $(event.target).parents("#divListMaterialSupport").length > 0 || event.target.id == "txtAddListPOSScale" || event.target.id == "divAddListPOSScale" || $(event.target).parents("#divAddListPOSScale").length > 0 || event.target.id == "txtAddListSheet" || event.target.id == "divAddListSheet" || $(event.target).parents("#divAddListSheet").length > 0 || event.target.id == "txtAddListGender" || event.target.id == "divAddListGender" || $(event.target).parents("#divAddListGender").length > 0 || event.target.id == "txtAddPOPMaterialSupport" || event.target.id == "divPOPMaterialSupport" || $(event.target).parents("#divPOPMaterialSupport").length > 0 || event.target.id == "txtAddPOPPOSScale" || event.target.id == "divAddPOPPOSScale" || $(event.target).parents("#divAddPOPPOSScale").length > 0 || event.target.id == "txtAddPOPSheet" || event.target.id == "divAddPOPSheet" || $(event.target).parents("#divAddPOPSheet").length > 0 || event.target.id == "txtAddPOPGender" || event.target.id == "divAddPOPGender" || $(event.target).parents("#divAddPOPGender").length > 0 || event.target.id == "txtAddBCSheet" || event.target.id == "divAddBCSheet" || $(event.target).parents("#divAddBCSheet").length > 0 || event.target.id == "txtAddBCGender" || event.target.id == "divAddBCGender" || $(event.target).parents("#divAddBCGender").length > 0)) {
        hideMenu();
    }
}

//添加LIist检查
function CheckAddList() {
    var posCode = $.trim($("#txtAddListPOSCode").val());
    var Sheet = $.trim($("#txtAddListSheet").val());
    var Gender = $.trim($("#txtAddListGender").val());
    var Quantity = $.trim($("#txtAddListQuantity").val());
    var LevelNum = $("#ddlAddListLevelNum").val();
    
    if (posCode == "") {
        alert("请填写店铺编号");
        return false;
    }
    if (Sheet == "") {
        alert("请填写位置");
        return false;
    }
//    if (Sheet.indexOf("桌")!=-1 && LevelNum=="0") {
//        alert("请选择陈列桌级别");
//        return false;
//    }
    if (Gender == "") {
        alert("请填写性别");
        return false;
    }
    if (Quantity == "") {
        alert("请填写数量");
        return false;
    }
    if (isNaN(Quantity)) {
        alert("数量必须是数字");
        return false;
    }
    return true;
}

//添加LIist检查
function CheckAddPOP() {
    var posCode = $.trim($("#txtAddPOPPOSCode").val());
    var GraphicNo = $.trim($("#txtAddPOPGraphicNo").val());
    var Sheet = $.trim($("#txtAddPOPSheet").val());
    var Gender = $.trim($("#txtAddPOPGender").val());
    var Quantity = $.trim($("#txtAddPOPQuantity").val());
    if (posCode == "") {
        alert("请填写店铺编号");
        return false;
    }
    if (GraphicNo == "") {
        alert("请填写POP编号");
        return false;
    }
    if (Sheet == "") {
        alert("请填写位置");
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
    if (isNaN(Quantity)) {
        alert("数量必须是数字");
        return false;
    }
    return true;
}

//添加LIist检查
function CheckAddSupplement() {
    var posCode = $.trim($("#txtAddBCPOSCode").val());
    
    var Sheet = $.trim($("#txtAddBCSheet").val());
    var Gender = $.trim($("#txtAddBCGender").val());
    var Quantity = $.trim($("#txtAddBCQuantity").val());
    if (posCode == "") {
        alert("请填写店铺编号");
        return false;
    }
   
    if (Sheet == "") {
        alert("请填写位置");
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
    if (isNaN(Quantity)) {
        alert("数量必须是数字");
        return false;
    }
    return true;
}

//
function CheckEditFinallOrder() {

    var Sheet = $.trim($("#txtSheetF").val());
    //var LevelNumF = $.trim($("#txtLevelNumF").val());
    
    var Gender = $.trim($("#txtGenderF").val());
    var Quantity = $.trim($("#txtQuantityF").val());
    //var GraphicWidth = $.trim($("#txtGraphicWidthF").val());
    //var GraphicLength = $.trim($("#txtGraphicLengthF").val());
    
    if (Sheet == "") {
        alert("请填写位置");
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
    if (isNaN(Quantity)) {
        alert("数量必须是数字");
        return false;
    }
    return true;
}
