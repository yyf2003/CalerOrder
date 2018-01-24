$(function () {
    CheckPrimission(url, null, $("#btnAddFrame"), $("#btnEditFrame"), $("#btnDeleteFrame"), null, $("#separator"), $("#btnCheckEditLog"), $("#toolbar"));
    //权限判断
//    if ($("#hfPromission").val() != "") {
//        var arr = $("#hfPromission").val().split('|');
//        var count = 0;
//        $.each(arr, function (key, val) {
//            if (val == "add") {
//                $("#btnAddFrame").show();
//                count++;
//            }
//            if (val == "edit") {
//                $("#btnEditFrame").show();
//               
//                count++;
//            }
//            if (val == "delete") {
//                $("#btnDeleteFrame").show();
//                count++;
//            }

//        })
//        if (count > 0)
//            $("#toolbar").show();
//    }

    $("input[name$='cbAll']").change(function () {
        var checked = this.checked;
        $("input[name$='cbOne']").each(function () {
            this.checked = checked;
        })
    })

    $("#selSheet").on("change", function () {
        GetCornerTypeList();
        //GetFrameList();
    })

    $("#selCornerType").change(function () {
        GetFrameList();
    })

    $("#txtFrameName").on("click", function () {
        showFrameList();
    })

    $("#txtCornerType").on("click", function () {
        showCornerTypeList();
    })

    $("#ddlFrameMenu").delegate("li", "click", function () {

        $("#txtFrameName").val($(this).html());
        hideMenu();
    })

    $("#ddlCornerType").delegate("li", "click", function () {

        $("#txtCornerType").val($(this).html());
        hideCornerTypeMenu();
    })
})



function addFrame() {
    ClearVal();
    var shopNo = $("#hfShopNo").val();
    $("#txtPOSCode").val(shopNo);
    GetSheetList();
    frame.add("add");
}

function editFrame() {
    var selectCount = 0;
    selectCount = $("input[name$='cbOne']:checked").length;
    if (selectCount == 0) {
        alert("请选择要编辑的器架");
        return false;
    }
    else if (selectCount > 1) {
        alert("只能选择一条");
        return false;
    }
    else {
        var frameId = $("input[name$='cbOne']:checked:first").next("input").val() || 0;
        GetSheetList();
        editItme(null, frameId);

    }



}

function deleteFrame() {
    var selectCount = 0;
    selectCount = $("input[name$='cbOne']:checked").length;
    
    if (selectCount == 0) {
        alert("请选择要删除的器架");
        return false;
    }
    else {
        if (confirm("确定删除吗？")) {
            $("#btnSubmitDelete").click();
        }
    }
}

function GetSheetList(sheet) {
    document.getElementById("selSheet").length = 1;
    $.ajax({
        type: "get",
        url: "Handler/MachineFrame.ashx?type=getSheetList",
        success: function (data) {

            if (data != "") {
                var json = eval(data);
                
                var isSelected = false;
                for (var i = 0; i < json.length; i++) {
                    var selected = "";
                    if (json[i].Sheet == sheet) {
                        selected = "selected='selected'";
                        isSelected = true;
                    }
                    var option = "<option value='" + json[i].Sheet + "' " + selected + ">" + json[i].Sheet + "</option>";
                    $("#selSheet").append(option);
                }
                if (isSelected) {
                    GetCornerTypeList();
                    //GetFrameList();
                }
            }
        }
    })
}

function GetFrameList() {
    var sheet = $("#selSheet").val();
    var cornerType = $("#selCornerType").val();
    //$("#ddlFrameMenu").html("");
    document.getElementById("selFrameName").length = 1;
    $.ajax({
        type: "get",
        url: "Handler/MachineFrame.ashx?type=getFrameList&sheet=" + sheet + "&cornerType=" + cornerType,
        success: function (data) {
            
            if (data != "") {
                var json = eval(data);

                for (var i = 0; i < json.length; i++) {
                    var selected = "";
                    if (json[i].FrameName == editFrameName) {
                        selected = "selected='selected'";
                        
                    }
                    var option = "<option value='" + json[i].FrameName + "' " + selected + ">" + json[i].FrameName + "</option>";
                    $("#selFrameName").append(option);
                }
               
            }
        }
    })
}

function GetCornerTypeList() {
    var sheet = $("#selSheet").val();
    //$("#ddlCornerType").html("");
    document.getElementById("selCornerType").length = 1;
    $.ajax({
        type: "get",
        url: "Handler/MachineFrame.ashx?type=getCornerType&sheet=" + sheet,
        success: function (data) {
           
            if (data != "") {
                var json = eval(data);
                
                for (var i = 0; i < json.length; i++) {
                    var selected = "";
                    if (json[i].CornerType == editCornerType) {
                        selected = "selected='selected'";
                       
                    }
                    var option = "<option value='" + json[i].CornerType + "' " + selected + ">" + json[i].CornerType + "</option>";
                    $("#selCornerType").append(option);
                }

            }
            GetFrameList();
        }
    })
}

function showFrameList() {

    $("#divFrameList").css({ left: 0 + "px", top: 20 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function showCornerTypeList() {

    $("#divCornerType").css({ left: 0 + "px", top: 20 + "px" }).slideDown("fast");
    $("body").bind("mousedown", onBodyDown);

}

function hideMenu() {
    $("#divFrameList").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function hideCornerTypeMenu() {
    $("#divCornerType").fadeOut("fast");
    $("body").unbind("mousedown", onBodyDown);
}

function onBodyDown(event) {
    if (!(event.target.id == "txtFrameName" || event.target.id == "divFrameList" || $(event.target).parents("#divFrameList").length > 0)) {
        hideMenu();
    }
    if (!(event.target.id == "txtCornerType" || event.target.id == "divCornerType" || $(event.target).parents("#divCornerType").length > 0)) {
        hideCornerTypeMenu();
    }
}

function ShowEditLog() {
    layer.open({
        type: 2,
        time: 0,
        title: '查看器架修改记录',
        skin: 'layui-layer-rim', //加上边框
        area: ['95%', '95%'],
        content: 'EditLog/MachineFrameLogList.aspx?shopId=' + shopId,
        id: 'popLayer',
        cancel: function (index) {
           
        }

    });
}
