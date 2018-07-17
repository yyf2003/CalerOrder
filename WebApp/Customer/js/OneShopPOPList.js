



$(function () {
    CheckPrimission(url, null, $("#btnAdd"), $("#btnEdit"), $("#btnDelete"), null, $("#separator"), $("#btnCheckEditLog"), $("#toolbar"));
//    if ($("#hfPromission").val() != "") {
//        var arr = $("#hfPromission").val().split('|');
//        var count = 0;
//        $.each(arr, function (key, val) {
//            if (val == "add") {
//                $("#btnAdd").show();
//                count++;
//            }
//            if (val == "edit") {
//                $("#btnEdit").show();
//                
//                count++;
//            }
//            if (val == "delete") {
//                $("#btnDelete").show();
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
})


function addPOP() {
    var shopNo = $("#hfShopNo").val();

    if (shopNo != "") {
        POP.getSheetList();
        POP.getMaterialCategory();
        POP.add(shopNo);
    }
    else {
        alert("新增失败");
    }   
}



function editPOP() {
    var selectCount = 0;
    selectCount = $("input[name$='cbOne']:checked").length;
    if (selectCount == 0) {
        alert("请选择要编辑的POP");
        return false;
    }
    else if (selectCount > 1) {
        alert("只能选择一条POP");
        return false;
    }
    else {
        var popId = $("input[name$='cbOne']:checked:first").next("input").val() || 0;
        currPOPId = popId;
        POP.getSheetList();
        POP.edit();
        
        }
    
   
    
}




function deletePlan() {
    var selectCount = 0;
    selectCount = $("input[name$='cbOne']:checked").length;
    if (selectCount == 0) {
        alert("请选择要删除的POP");
        return false;
    }
    else {
        if (confirm("确定删除吗？")) {
            $("#btnDeletePOP").click();
        }
    }
}

function ShowEditLog() {
    layer.open({
        type: 2,
        time: 0,
        title: '查看POP修改记录',
        skin: 'layui-layer-rim', //加上边框
        area: ['95%', '95%'],
        content: 'EditLog/POPLogList.aspx?shopId=' + shopId,
        id: 'popLayer',
        cancel: function (index) {

        }

    });
}
