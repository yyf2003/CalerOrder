


Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (s, e) {
    $(function () {
        $("#guianceSeleAll").change(function () {
            var checked = this.checked;
            $("input[name^='cblGuidanceList']").each(function () {
                this.checked = checked;
            });
        })

        $("#spanClearShopNos").click(function () {
            $("#txtSearchShopNo").val("");
        })

        $("input[name^='cblMaterial']").change(function () {

            if (this.checked) {
                $(this).siblings("input").each(function () {
                    this.checked = false;
                });
            }
        })

        $("#cbShopAll").click(function () {
            var checked = this.checked;
            $("input[name$='cbOne']").each(function () {
                this.checked = checked;
            });
        })

        $("input[name$='cbOne']").change(function () {
            if (!this.checked) {
                $("#cbShopAll").prop("checked", false);
            }
            else {
                var checked = $("input[name$='cbOne']:checked").length == $("input[name$='cbOne']").length;
                $("#cbShopAll").prop("checked", checked);
            }
        })
    })
})



function getMonth() {
    $("#txtGuidanceMonth").blur();
}

function Check() {
    var guidanceList = $("input[name^='cblGuidanceList']").length;
    if (guidanceList == 0) {
        alert("没有活动");
        return false;
    }
    else {
        $("#imgLoading").show();
        return true;
    }
}

function CheckType1() {
    $("#labType1State").text("");
    var len = $("input[name$='cbOne']:checked").length;
    if (len == 0) {
        layer.msg("请选择店铺");
        return false;
    }
//    if ($("#cbCancelPVC").attr("checked") != "checked") {
//        layer.msg("请选择修改类型");
//        return false;
    //    }
    var outsourceId = $("#ddlType1InstallOutsource").val();
    if (outsourceId == "0") {
        layer.msg("请选择外协");
        return false;
    }
    var num = $.trim($("#txtMaterialNum").val());
    if (num == "") {
        layer.msg("请填写数量");
        return false;
    }
    else if (isNaN(num)) {
        layer.msg("数量填写正确");
        return false;
    }
    var price = $.trim($("#txtMaterialTotalPrice").val());
    if (price == "") {
        layer.msg("请填写总金额");
        return false;
    }
    else if (isNaN(price)) {
        layer.msg("总金额填写正确");
        return false;
    }
    $("#imgType1Waiting").show();
    
    return true;
}

function CheckType2() {
    $("#labType2State").text("");
    var len = $("input[name$='cbOne']:checked").length;
    if (len == 0) {
        layer.msg("请选择店铺");
        return false;
    }
    if ($("#ddlInstallOutsource").val()=="0") {
        layer.msg("请选择高空安装外协");
        return false;
    }
    $("#imgType2Waiting").show();
    return true;
}