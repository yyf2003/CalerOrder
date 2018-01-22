

var PlanJsonStr = "";
var CurrPlanId = 0;
var conditionRow = null;
$(function () {
    Condition.init();
    Plan.bindMaterialList();
    Plan.getList();
    $("#SheetDiv").delegate("input[name='radioSheet']", "change", function () {
        conditionRow = null;
        $("#SheetLoading").show();
        Condition.changeSheet();
    })
    $("#RegionDiv").delegate("input[name='cbRegion']", "change", function () {
        $("#RegionLoading").show();
        Condition.changeRegion();
    })
    $("#ProvinceDiv").delegate("input[name='cbProvince']", "change", function () {
        $("#ProvinceLoading").show();
        Condition.changeProvince();
    })
    $("#CityDiv").delegate("input[name='cbCity']", "change", function () {
        $("#CityLoading").show();
        Condition.changeCity();
    })
    $("#GenderDiv").delegate("input[name='cbGender']", "change", function () {
        $("#GenderLoading").show();
        Condition.changeGender();
    })
    $("#CornerTypeDiv").delegate("input[name='cbCornerType']", "change", function () {
        $("#CornerTypeLoading").show();
        Condition.changeCornerType();
    })
    $("#MachineFrameDiv").delegate("input[name='cbMachineFrame']", "change", function () {
        $("#MachineFrameLoading").show();
        Condition.changeMachineFrame();
    })
    $("#CityTierDiv").delegate("input[name='cbCityTier']", "change", function () {
        $("#CityTierLoading").show();
        Condition.changeCityTier();
    })
    $("#FormatDiv").delegate("input[name='cbFormat']", "change", function () {
        $("#FormatLoading").show();
        Condition.changeFormat();
    })
    $("#MaterialSupportDiv").delegate("input[name='cbMaterialSupport']", "change", function () {
        $("#MaterialSupportLoading").show();
        Condition.changeMaterialSupport();
    })
    $("#POSScaleDiv").delegate("input[name='cbPOSScale']", "change", function () {
        $("#POSScaleLoading").show();
        Condition.changePOSScale();
    })
    $("#IsInstallDiv").delegate("input[name='cbIsInstall']", "change", function () {
        $("#IsInstallLoading").show();
        Condition.changeIsInstall();
    })
    $("#QuantityDiv").delegate("input[name='cbQuantity']", "change", function () {
        $("#QuantityLoading").show();
        Condition.changeQuantity();
    })
    $("#POPSizeDiv").delegate("input[name='cbPOPSize']", "change", function () {
        $("#POPSizeLoading").show();
        Condition.changePOPSize();
    })
    $("#IsElectricityDiv").delegate("input[name='cbIsElectricity']", "change", function () {
        $("#IsElectricityLoading").show();
        Condition.changeIsElectricity();
    })

    //选择材质
    $("#container").delegate("span[name='btnSelectMaterial']", "click", function () {
        //SelectMaterialSpan = $(this);
        $(this).siblings("div").show();
    })

    //提交选择好的材质
    $("#container").delegate("span[name='btnSubmitMaterial']", "click", function () {

        var div = $(this).parent().parent().parent().parent();
        var mName = "";
        var price = "";
        $(div).find(".customerMaterials").find("input[name='cbMaterial']:checked").each(function () {

            mName = $(this).siblings("span").html();
            price = $(this).siblings("span").data("price");
        })
        if (mName.length > 0) {
            $("#txtMaterial").val(mName);
            $("#txtRackPrice").val(price);

        }
        $(".divMaterialList").hide();
    })

    //材质只能单选
    $("#container").delegate("input[name='cbMaterial']", "change", function () {
        if (this.checked) {
            $(".customerMaterials").find("input[name='cbMaterial']").not($(this)).attr("checked", false);
        }
    })

    $("#btnAddPlanContent").click(function () {
        Plan.addPlanContent();
    })

    //删除方案内容
    $("#addContent").delegate(".deletePlanDetail", "click", function () {
        $(this).parent().parent().remove();
    })
    //新增方案
    $("#btnSubmitPlan").click(function () {
       
        Plan.submitPlan("add");
    })
    //更新方案
    $("#btnUpdatePlan").on("click", function () {
        Plan.submitPlan("update");
    })

    //清空方案内容数据
    $("#spanClareDate").on("click", function () {
        if (confirm("确定清除吗？")) {
            $("#addContent").html("");
        }
    })

    //添加左/右窗贴，地铺，窗贴
    $("#btnAddChuChuang").click(function () {
        var sheet = $("input:radio[name='radioSheet']:checked").val() || "";

        if (sheet == "" || sheet.indexOf("橱窗") == -1) {
            return false;
        }
        //左侧贴
        var tr1 = "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtMaterial1' maxlength='100' value=''   style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50'  value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' readonly='readonly' data-windowtype='LeftSideStick' maxlength='50' value='左侧贴' style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";
        //右侧贴
        tr1 += "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' readonly='readonly' data-windowtype='RightSideStick' maxlength='50' value='右侧贴' style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";
        //地铺
        tr1 += "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' readonly='readonly' data-windowtype='Floor'  maxlength='50' value='地贴' style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";
        //窗贴
        tr1 += "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' readonly='readonly' data-windowtype='WindowStick' maxlength='50' value='窗贴' style='width: 90%; text-align: center;' /></td>";

        tr1 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";


        $("#addContent").append(tr1);
    })

    $("#cbSelectAllMachineFrame").change(function () {
        var checked = this.checked;
        $("input[name='cbMachineFrame']").each(function () {
            $(this).attr("checked", checked);

        })
    })
    $("#cbSelectAllPOPSize").change(function () {
        var checked = this.checked;
        $("input[name='cbPOPSize']").each(function () {
            $(this).attr("checked", checked);

        })
    })

    //删除方案
    $("#btnDelete").on("click", function () {

        var ids = "";
        var selectRow = $("#tbPlanList").datagrid("getSelected");
        if (selectRow != null) {
            ids = selectRow.Id;
        }

        if (ids != "") {
            $.ajax({
                type: "get",
                url: "/Subjects/Handler/SplitPlan.ashx?type=deletePlan&planIds=" + ids,
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        Plan.getList();
                        ClearConditionVal();
                        window.parent.HasChange();
                    }
                    else {
                        alert("删除失败：" + data);
                    }
                }
            })
        }
        else
            alert("请选择方案");


    })


    //全选每个省份的城市
    $("#CityDiv").delegate("input[name='citycbAll']", "click", function () {
        var checked = this.checked;
        $(this).parent().next("td").find("input[name='cbCity']").each(function () {
            $(this).attr("checked", checked);
        })
        Condition.changeCity();
    })

    //按店铺编号取条件
    var defaultShopNos = "";
    $("#txtShopNos").on("focus", function () {
        defaultShopNos = $.trim($(this).val());

    })
    $("#txtShopNos").on("blur", function () {
        var shopNos = $.trim($(this).val());
        if (defaultShopNos != shopNos) {
            $("#ShopNosLoading").show();
            Condition.init();
        }
    })

    //按店铺编号取条件
    var defaultShopNos1 = "";
    $("#txtNoInvolveShopNos").on("focus", function () {
        defaultShopNos1 = $.trim($(this).val());

    })
    //检查不参与拆单的店铺编号
    $("#txtNoInvolveShopNos").on("blur", function () {
        var shopNo = $.trim($(this).val());

        if (defaultShopNos1 != shopNo) {
            $.ajax({
                type: "get",
                url: "/Subjects/Handler/SplitPlan.ashx",
                data: { type: "checkShopNo", subjectId: subjectId, shopNo: shopNo },
                success: function (data) {

                    $("#NoInvolveShopNosMsg").html(data);
                }
            })
        }
    })

    //添加陈列桌尺寸(正常尺寸)
    $("#btnAddNormalSize").click(function () {
        AddTableSize(1)
    })

    //添加陈列桌尺寸(反包尺寸)
    $("#btnAddWithEdgelSize").click(function () {
        AddTableSize(2)
    })
})

var Condition = {
    init: function () {
        EmptyConditionVal();
        $("#SheetLoading").show();
        var shopNos = $.trim($("#txtShopNos").val());
        $.ajax({
            type: "get",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "initConditions", subjectId: subjectId, shopNos: shopNos },
            success: function (data) {

                if (data != "") {
                    if (data.indexOf("不存在") != -1) {
                        $("#ShopNosLoading").hide();
                        $("#SheetLoading").hide();
                        $("#shopMsg").html(data);
                    }
                    else {
                        $("#shopMsg").html("");
                        var json = eval(data);
                        if (json.length > 0) {
                            var sheetList = json[0].Sheet;
                            var regionList = json[0].RegionNames;

                            var genderList = json[0].Gender;
                            var cityTierList = json[0].CityTier;
                            var formatList = json[0].Format;
                            var materialSupportList = json[0].MaterialSupport;
                            var POSScaleList = json[0].POSScale;
                            var IsInstallList = json[0].IsInstall;
                            var quantityList = json[0].Quantity;
                            var POPSizeList = json[0].POPSize;
                            var isElectricityList = json[0].IsElectricity;
                            var chooseImgList = json[0].ChooseImg;
                            if (sheetList != "") {
                                var sheetArr = sheetList.split(',');
                                for (var i = 0; i < sheetArr.length; i++) {
                                    var input = "<input type='radio' name='radioSheet' value='" + sheetArr[i] + "' />" + sheetArr[i] + "&nbsp;&nbsp;";
                                    $("#SheetDiv").append(input);

                                }
                            }
                            if (regionList != "") {
                                var regionArr = regionList.split(',');
                                for (var i = 0; i < regionArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbRegion' value='" + regionArr[i] + "' />" + regionArr[i] + "&nbsp;&nbsp;";
                                    $("#RegionDiv").append(input);

                                }
                            }
                            $("#ProvinceDiv").html("");
                            $("#CityDiv").html("");
                            if (genderList != "") {
                                var genderArr = genderList.split(',');
                                for (var i = 0; i < genderArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbGender' value='" + genderArr[i] + "' />" + genderArr[i] + "&nbsp;&nbsp;";
                                    $("#GenderDiv").append(input);
                                }
                            }
                            if (cityTierList != "") {
                                var cityTierArr = cityTierList.split(',');
                                for (var i = 0; i < cityTierArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbCityTier' value='" + cityTierArr[i] + "' />" + cityTierArr[i] + "&nbsp;&nbsp;";
                                    $("#CityTierDiv").append(input);
                                }
                            }
                            if (formatList != "") {
                                var formatArr = formatList.split(',');
                                for (var i = 0; i < formatArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbFormat' value='" + formatArr[i] + "' />" + formatArr[i] + "&nbsp;&nbsp;";
                                    $("#FormatDiv").append(input);
                                }
                            }
                            if (materialSupportList != "") {
                                var materialSupportArr = materialSupportList.split(',');
                                for (var i = 0; i < materialSupportArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                    $("#MaterialSupportDiv").append(input);
                                }
                            }
                            if (POSScaleList != "") {
                                var POSScaleArr = POSScaleList.split(',');
                                for (var i = 0; i < POSScaleArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                    $("#POSScaleDiv").append(input);
                                }
                            }
                            if (IsInstallList != "") {
                                var IsInstallArr = IsInstallList.split(',');
                                for (var i = 0; i < IsInstallArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                    $("#IsInstallDiv").append(input);
                                }
                            }
                            if (quantityList != "") {
                                var quantityArr = quantityList.split(',');
                                for (var i = 0; i < quantityArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                    $("#QuantityDiv").append(input);
                                }
                            }
                            if (POPSizeList != "") {
                                var POPSizeArr = POPSizeList.split(',');
                                for (var i = 0; i < POPSizeArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                    $("#POPSizeDiv").append(input);
                                }
                                $("#SelectAllPOPSize").show();
                            }
                            else {
                                $("#SelectAllPOPSize").hide();
                            }
                            if (isElectricityList != "") {
                                var isElectricityArr = isElectricityList.split(',');
                                for (var i = 0; i < isElectricityArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                    $("#IsElectricityDiv").append(input);
                                }
                            }
                            if (chooseImgList != "") {
                                var chooseImgArr = chooseImgList.split(',');
                                for (var i = 0; i < chooseImgArr.length; i++) {
                                    var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                    $("#ChooseImgDiv").append(input);
                                }
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#ShopNosLoading").hide();
                $("#SheetLoading").hide();
            }
        })
    },
    changeSheet: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '"}';

        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {
                        var regionList = json[0].RegionNames;
                        var genderList = json[0].Gender;
                        var cornerTypeList = json[0].CornerType;
                        var machineFrameList = json[0].MachineFrameNames;
                        var cityTierList = json[0].CityTier;
                        var formatList = json[0].Format;
                        var materialSupportList = json[0].MaterialSupport;
                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;
                        $("#ProvinceDiv").html("");
                        $("#CityDiv").html("");
                        $("#RegionDiv").html("");
                        if (regionList != "") {
                            var regionArr = regionList.split(',');
                            for (var i = 0; i < regionArr.length; i++) {
                                var input = "<input type='checkbox' name='cbRegion' value='" + regionArr[i] + "' />" + regionArr[i] + "&nbsp;&nbsp;";
                                $("#RegionDiv").append(input);

                            }
                        }
                        $("#GenderDiv").html("");
                        if (genderList != "") {
                            var genderArr = genderList.split(',');
                            for (var i = 0; i < genderArr.length; i++) {
                                var input = "<input type='checkbox' name='cbGender' value='" + genderArr[i] + "' />" + genderArr[i] + "&nbsp;&nbsp;";
                                $("#GenderDiv").append(input);
                            }
                        }
                        $("#CornerTypeDiv").html("");
                        if (cornerTypeList != "") {

                            var cornerTypeArr = cornerTypeList.split(',');
                            for (var i = 0; i < cornerTypeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCornerType' value='" + cornerTypeArr[i] + "' />" + cornerTypeArr[i] + "&nbsp;&nbsp;";

                                $("#CornerTypeDiv").append(input);
                            }
                        }
                        $("#MachineFrameDiv").html("");
                        if (machineFrameList != "") {
                            var machineFrameArr = machineFrameList.split(',');
                            var selected = false;
                            for (var i = 0; i < machineFrameArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMachineFrame' value='" + machineFrameArr[i] + "' />" + machineFrameArr[i] + "&nbsp;&nbsp;";
                                $("#MachineFrameDiv").append(input);
                            }
                            $("#SelectAllMachineFrame").show();
                        }
                        else {
                            $("#SelectAllMachineFrame").hide();
                        }
                        $("#CityTierDiv").html("");
                        if (cityTierList != "") {
                            var cityTierArr = cityTierList.split(',');
                            for (var i = 0; i < cityTierArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCityTier' value='" + cityTierArr[i] + "' />" + cityTierArr[i] + "&nbsp;&nbsp;";
                                $("#CityTierDiv").append(input);
                            }
                        }
                        $("#FormatDiv").html("");
                        if (formatList != "") {
                            var formatArr = formatList.split(',');
                            for (var i = 0; i < formatArr.length; i++) {
                                var input = "<input type='checkbox' name='cbFormat' value='" + formatArr[i] + "' />" + formatArr[i] + "&nbsp;&nbsp;";
                                $("#FormatDiv").append(input);
                            }
                        }
                        $("#MaterialSupportDiv").html("");
                        if (materialSupportList != "") {
                            var materialSupportArr = materialSupportList.split(',');
                            for (var i = 0; i < materialSupportArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                $("#MaterialSupportDiv").append(input);
                            }
                        }
                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#SheetLoading").hide();
                if (conditionRow != null) {
                    var region = conditionRow.RegionNames || "";
                    var Gender = conditionRow.Gender || "";
                    var CornerType = conditionRow.CornerType || "";
                    var MachineFrame = conditionRow.MachineFrameNames || "";
                    var CityTier = conditionRow.CityTier || "";
                    var Format = conditionRow.Format || "";
                    var MaterialSupport = conditionRow.MaterialSupport || "";
                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";

                    if (region != "") {
                        var selected = false;
                        var arr = region.split(',');
                        $("input[name='cbRegion']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeRegion();
                        }
                    }

                    else if (Gender != "") {
                        var selected = false;
                        var arr = Gender.split(',');
                        $("input[name='cbGender']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeGender();
                        }
                    }
                    else if (CornerType != "") {
                        var selected = false;
                        var arr = CornerType.split(',');
                        $("input[name='cbCornerType']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCornerType();
                        }
                    }
                    else if (MachineFrame != "") {
                        var selected = false;
                        var arr = MachineFrame.split(',');
                        $("input[name='cbMachineFrame']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMachineFrame();
                        }
                    }
                    else if (CityTier != "") {
                        var selected = false;
                        var arr = CityTier.split(',');
                        $("input[name='cbCityTier']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCityTier();
                        }
                    }
                    else if (Format != "") {
                        var selected = false;
                        var arr = Format.split(',');
                        $("input[name='cbFormat']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeFormat();
                        }
                    }
                    else if (MaterialSupport != "") {
                        var selected = false;
                        var arr = MaterialSupport.split(',');
                        $("input[name='cbMaterialSupport']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMaterialSupport();
                        }
                    }
                    else if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeRegion: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {
                        var provinceList = json[0].ProvinceName;
                        var cityList = json[0].CityName;
                        var genderList = json[0].Gender;
                        var cornerTypeList = json[0].CornerType;
                        var machineFrameList = json[0].MachineFrameNames;
                        var cityTierList = json[0].CityTier;
                        var formatList = json[0].Format;
                        var materialSupportList = json[0].MaterialSupport;
                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;
                        $("#ProvinceDiv").html("");
                        $("#CityDiv").html("");
                        if (provinceList != "") {
                            var provinceArr = provinceList.split(',');
                            for (var i = 0; i < provinceArr.length; i++) {
                                var input = "<input type='checkbox' name='cbProvince' value='" + provinceArr[i] + "' />" + provinceArr[i] + "&nbsp;&nbsp;";
                                $("#ProvinceDiv").append(input);
                            }
                        }
                        $("#GenderDiv").html("");
                        if (genderList != "") {
                            var genderArr = genderList.split(',');
                            for (var i = 0; i < genderArr.length; i++) {
                                var input = "<input type='checkbox' name='cbGender' value='" + genderArr[i] + "' />" + genderArr[i] + "&nbsp;&nbsp;";
                                $("#GenderDiv").append(input);
                            }
                        }
                        $("#CornerTypeDiv").html("");
                        if (cornerTypeList != "") {

                            var cornerTypeArr = cornerTypeList.split(',');
                            for (var i = 0; i < cornerTypeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCornerType' value='" + cornerTypeArr[i] + "' />" + cornerTypeArr[i] + "&nbsp;&nbsp;";

                                $("#CornerTypeDiv").append(input);
                            }
                        }
                        $("#MachineFrameDiv").html("");
                        if (machineFrameList != "") {
                            var machineFrameArr = machineFrameList.split(',');
                            for (var i = 0; i < machineFrameArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMachineFrame' value='" + machineFrameArr[i] + "' />" + machineFrameArr[i] + "&nbsp;&nbsp;";
                                $("#MachineFrameDiv").append(input);
                            }
                            $("#SelectAllMachineFrame").show();
                        }
                        else {
                            $("#SelectAllMachineFrame").hide();
                        }
                        $("#CityTierDiv").html("");
                        if (cityTierList != "") {
                            var cityTierArr = cityTierList.split(',');
                            for (var i = 0; i < cityTierArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCityTier' value='" + cityTierArr[i] + "' />" + cityTierArr[i] + "&nbsp;&nbsp;";
                                $("#CityTierDiv").append(input);
                            }
                        }
                        $("#FormatDiv").html("");
                        if (formatList != "") {
                            var formatArr = formatList.split(',');
                            for (var i = 0; i < formatArr.length; i++) {
                                var input = "<input type='checkbox' name='cbFormat' value='" + formatArr[i] + "' />" + formatArr[i] + "&nbsp;&nbsp;";
                                $("#FormatDiv").append(input);
                            }
                        }
                        $("#MaterialSupportDiv").html("");
                        if (materialSupportList != "") {
                            var materialSupportArr = materialSupportList.split(',');
                            for (var i = 0; i < materialSupportArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                $("#MaterialSupportDiv").append(input);
                            }
                        }
                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#RegionLoading").hide();
                if (conditionRow != null) {
                    var Province = conditionRow.ProvinceName || "";

                    var Gender = conditionRow.Gender || "";
                    var CornerType = conditionRow.CornerType || "";
                    var MachineFrame = conditionRow.MachineFrameNames || "";
                    var CityTier = conditionRow.CityTier || "";
                    var Format = conditionRow.Format || "";
                    var MaterialSupport = conditionRow.MaterialSupport || "";
                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (Province != "") {
                        var selected = false;
                        var arr = Province.split(',');
                        $("input[name='cbProvince']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeProvince();
                        }
                    }
                    else if (Gender != "") {
                        var selected = false;
                        var arr = Gender.split(',');
                        $("input[name='cbGender']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeGender();
                        }
                    }
                    else if (CornerType != "") {
                        var selected = false;
                        var arr = CornerType.split(',');
                        $("input[name='cbCornerType']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCornerType();
                        }
                    }
                    else if (MachineFrame != "") {
                        var selected = false;
                        var arr = MachineFrame.split(',');
                        $("input[name='cbMachineFrame']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMachineFrame();
                        }
                    }
                    else if (CityTier != "") {
                        var selected = false;
                        var arr = CityTier.split(',');
                        $("input[name='cbCityTier']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCityTier();
                        }
                    }
                    else if (Format != "") {
                        var selected = false;
                        var arr = Format.split(',');
                        $("input[name='cbFormat']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeFormat();
                        }
                    }
                    else if (MaterialSupport != "") {
                        var selected = false;
                        var arr = MaterialSupport.split(',');
                        $("input[name='cbMaterialSupport']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMaterialSupport();
                        }
                    }
                    else if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeProvince: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {

                        var cityList = json[0].CityName;
                        var genderList = json[0].Gender;
                        var cornerTypeList = json[0].CornerType;
                        var machineFrameList = json[0].MachineFrameNames;
                        var cityTierList = json[0].CityTier;
                        var formatList = json[0].Format;
                        var materialSupportList = json[0].MaterialSupport;
                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;
                        $("#CityDiv").html("");
                        if (cityList != "") {
                            var cityArr = cityList.split(',');
                            var pName = "";
                            var table = "<table style='margin-left:0px;'>";
                            var count = cityArr.length;
                            for (var i = 0; i < count; i++) {

                                var province = cityArr[i].split('_')[0];
                                var city = cityArr[i].split('_')[1];

                                var div = "";
                                div += "<div style='float:left;padding-left: 5px;'>";
                                div += "<input type='checkbox' name='cbCity' value='" + city + "'/><span>" + city + "</span>&nbsp;";
                                div += "</div>";
                                if (pName != province) {
                                    if (i > 0)
                                        table += "</td></tr>";
                                    pName = province;
                                    table += "<tr>";
                                    table += "<td style='width:80px;border-right:1px solid #ccc; vertical-align:top; padding-top:5px;padding-left: 5px;'><input type='checkbox' name='citycbAll'/>" + pName + "</td><td style='vertical-align:top; padding-top:5px;'>";
                                }
                                table += div;
                                if (i == count - 1)
                                    table += "</td></tr>";


                            }
                            table += "</table>";
                            $("#CityDiv").html(table);
                        }
                        $("#GenderDiv").html("");
                        if (genderList != "") {
                            var genderArr = genderList.split(',');
                            for (var i = 0; i < genderArr.length; i++) {
                                var input = "<input type='checkbox' name='cbGender' value='" + genderArr[i] + "' />" + genderArr[i] + "&nbsp;&nbsp;";
                                $("#GenderDiv").append(input);
                            }
                        }
                        $("#CornerTypeDiv").html("");
                        if (cornerTypeList != "") {

                            var cornerTypeArr = cornerTypeList.split(',');
                            for (var i = 0; i < cornerTypeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCornerType' value='" + cornerTypeArr[i] + "' />" + cornerTypeArr[i] + "&nbsp;&nbsp;";

                                $("#CornerTypeDiv").append(input);
                            }
                        }
                        $("#MachineFrameDiv").html("");
                        if (machineFrameList != "") {
                            var machineFrameArr = machineFrameList.split(',');
                            for (var i = 0; i < machineFrameArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMachineFrame' value='" + machineFrameArr[i] + "' />" + machineFrameArr[i] + "&nbsp;&nbsp;";
                                $("#MachineFrameDiv").append(input);
                            }
                            $("#SelectAllMachineFrame").show();
                        }
                        else {
                            $("#SelectAllMachineFrame").hide();
                        }
                        $("#CityTierDiv").html("");
                        if (cityTierList != "") {
                            var cityTierArr = cityTierList.split(',');
                            for (var i = 0; i < cityTierArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCityTier' value='" + cityTierArr[i] + "' />" + cityTierArr[i] + "&nbsp;&nbsp;";
                                $("#CityTierDiv").append(input);
                            }
                        }
                        $("#FormatDiv").html("");
                        if (formatList != "") {
                            var formatArr = formatList.split(',');
                            for (var i = 0; i < formatArr.length; i++) {
                                var input = "<input type='checkbox' name='cbFormat' value='" + formatArr[i] + "' />" + formatArr[i] + "&nbsp;&nbsp;";
                                $("#FormatDiv").append(input);
                            }
                        }
                        $("#MaterialSupportDiv").html("");
                        if (materialSupportList != "") {
                            var materialSupportArr = materialSupportList.split(',');
                            for (var i = 0; i < materialSupportArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                $("#MaterialSupportDiv").append(input);
                            }
                        }
                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#ProvinceLoading").hide();

                if (conditionRow != null) {
                    var City = conditionRow.CityName || "";
                    var CornerType = conditionRow.CornerType || "";
                    var MachineFrame = conditionRow.MachineFrameNames || "";
                    var CityTier = conditionRow.CityTier || "";
                    var Format = conditionRow.Format || "";
                    var MaterialSupport = conditionRow.MaterialSupport || "";
                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (City != "") {
                        var selected = false;
                        var arr = City.split(',');
                        $("input[name='cbCity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCity();
                        }
                    }
                    else if (CornerType != "") {
                        var selected = false;
                        var arr = CornerType.split(',');
                        $("input[name='cbCornerType']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCornerType();
                        }
                    }
                    else if (MachineFrame != "") {
                        var selected = false;
                        var arr = MachineFrame.split(',');
                        $("input[name='cbMachineFrame']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMachineFrame();
                        }
                    }
                    else if (CityTier != "") {
                        var selected = false;
                        var arr = CityTier.split(',');
                        $("input[name='cbCityTier']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCityTier();
                        }
                    }
                    else if (Format != "") {
                        var selected = false;
                        var arr = Format.split(',');
                        $("input[name='cbFormat']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeFormat();
                        }
                    }
                    else if (MaterialSupport != "") {
                        var selected = false;
                        var arr = MaterialSupport.split(',');
                        $("input[name='cbMaterialSupport']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMaterialSupport();
                        }
                    }
                    else if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeCity: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {


                        var genderList = json[0].Gender;
                        var cornerTypeList = json[0].CornerType;
                        var machineFrameList = json[0].MachineFrameNames;
                        var cityTierList = json[0].CityTier;
                        var formatList = json[0].Format;
                        var materialSupportList = json[0].MaterialSupport;
                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;

                        $("#GenderDiv").html("");
                        if (genderList != "") {
                            var genderArr = genderList.split(',');
                            for (var i = 0; i < genderArr.length; i++) {
                                var input = "<input type='checkbox' name='cbGender' value='" + genderArr[i] + "' />" + genderArr[i] + "&nbsp;&nbsp;";
                                $("#GenderDiv").append(input);
                            }
                        }
                        $("#CornerTypeDiv").html("");
                        if (cornerTypeList != "") {

                            var cornerTypeArr = cornerTypeList.split(',');
                            for (var i = 0; i < cornerTypeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCornerType' value='" + cornerTypeArr[i] + "' />" + cornerTypeArr[i] + "&nbsp;&nbsp;";

                                $("#CornerTypeDiv").append(input);
                            }
                        }
                        $("#MachineFrameDiv").html("");
                        if (machineFrameList != "") {
                            var machineFrameArr = machineFrameList.split(',');
                            for (var i = 0; i < machineFrameArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMachineFrame' value='" + machineFrameArr[i] + "' />" + machineFrameArr[i] + "&nbsp;&nbsp;";
                                $("#MachineFrameDiv").append(input);
                            }
                            $("#SelectAllMachineFrame").show();
                        }
                        else {
                            $("#SelectAllMachineFrame").hide();
                        }
                        $("#CityTierDiv").html("");
                        if (cityTierList != "") {
                            var cityTierArr = cityTierList.split(',');
                            for (var i = 0; i < cityTierArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCityTier' value='" + cityTierArr[i] + "' />" + cityTierArr[i] + "&nbsp;&nbsp;";
                                $("#CityTierDiv").append(input);
                            }
                        }
                        $("#FormatDiv").html("");
                        if (formatList != "") {
                            var formatArr = formatList.split(',');
                            for (var i = 0; i < formatArr.length; i++) {
                                var input = "<input type='checkbox' name='cbFormat' value='" + formatArr[i] + "' />" + formatArr[i] + "&nbsp;&nbsp;";
                                $("#FormatDiv").append(input);
                            }
                        }
                        $("#MaterialSupportDiv").html("");
                        if (materialSupportList != "") {
                            var materialSupportArr = materialSupportList.split(',');
                            for (var i = 0; i < materialSupportArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                $("#MaterialSupportDiv").append(input);
                            }
                        }
                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#CityLoading").hide();

                if (conditionRow != null) {

                    var CornerType = conditionRow.CornerType || "";
                    var MachineFrame = conditionRow.MachineFrameNames || "";
                    var CityTier = conditionRow.CityTier || "";
                    var Format = conditionRow.Format || "";
                    var MaterialSupport = conditionRow.MaterialSupport || "";
                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";

                    if (CornerType != "") {
                        var selected = false;
                        var arr = CornerType.split(',');
                        $("input[name='cbCornerType']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCornerType();
                        }
                    }
                    else if (MachineFrame != "") {
                        var selected = false;
                        var arr = MachineFrame.split(',');
                        $("input[name='cbMachineFrame']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMachineFrame();
                        }
                    }
                    else if (CityTier != "") {
                        var selected = false;
                        var arr = CityTier.split(',');
                        $("input[name='cbCityTier']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCityTier();
                        }
                    }
                    else if (Format != "") {
                        var selected = false;
                        var arr = Format.split(',');
                        $("input[name='cbFormat']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeFormat();
                        }
                    }
                    else if (MaterialSupport != "") {
                        var selected = false;
                        var arr = MaterialSupport.split(',');
                        $("input[name='cbMaterialSupport']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMaterialSupport();
                        }
                    }
                    else if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeGender: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {


                        var cornerTypeList = json[0].CornerType;
                        var machineFrameList = json[0].MachineFrameNames;
                        var cityTierList = json[0].CityTier;
                        var formatList = json[0].Format;
                        var materialSupportList = json[0].MaterialSupport;
                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;


                        $("#CornerTypeDiv").html("");
                        if (cornerTypeList != "") {

                            var cornerTypeArr = cornerTypeList.split(',');
                            for (var i = 0; i < cornerTypeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCornerType' value='" + cornerTypeArr[i] + "' />" + cornerTypeArr[i] + "&nbsp;&nbsp;";

                                $("#CornerTypeDiv").append(input);
                            }
                        }
                        $("#MachineFrameDiv").html("");
                        if (machineFrameList != "") {
                            var machineFrameArr = machineFrameList.split(',');
                            for (var i = 0; i < machineFrameArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMachineFrame' value='" + machineFrameArr[i] + "' />" + machineFrameArr[i] + "&nbsp;&nbsp;";
                                $("#MachineFrameDiv").append(input);
                            }
                            $("#SelectAllMachineFrame").show();
                        }
                        else {
                            $("#SelectAllMachineFrame").hide();
                        }
                        $("#CityTierDiv").html("");
                        if (cityTierList != "") {
                            var cityTierArr = cityTierList.split(',');
                            for (var i = 0; i < cityTierArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCityTier' value='" + cityTierArr[i] + "' />" + cityTierArr[i] + "&nbsp;&nbsp;";
                                $("#CityTierDiv").append(input);
                            }
                        }
                        $("#FormatDiv").html("");
                        if (formatList != "") {
                            var formatArr = formatList.split(',');
                            for (var i = 0; i < formatArr.length; i++) {
                                var input = "<input type='checkbox' name='cbFormat' value='" + formatArr[i] + "' />" + formatArr[i] + "&nbsp;&nbsp;";
                                $("#FormatDiv").append(input);
                            }
                        }
                        $("#MaterialSupportDiv").html("");
                        if (materialSupportList != "") {
                            var materialSupportArr = materialSupportList.split(',');
                            for (var i = 0; i < materialSupportArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                $("#MaterialSupportDiv").append(input);
                            }
                        }
                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#GenderLoading").hide();
                if (conditionRow != null) {

                    var CornerType = conditionRow.CornerType || "";
                    var MachineFrame = conditionRow.MachineFrameNames || "";
                    var CityTier = conditionRow.CityTier || "";
                    var Format = conditionRow.Format || "";
                    var MaterialSupport = conditionRow.MaterialSupport || "";
                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";

                    if (CornerType != "") {
                        var selected = false;
                        var arr = CornerType.split(',');
                        $("input[name='cbCornerType']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCornerType();
                        }
                    }
                    else if (MachineFrame != "") {
                        var selected = false;
                        var arr = MachineFrame.split(',');
                        $("input[name='cbMachineFrame']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMachineFrame();
                        }
                    }
                    else if (CityTier != "") {
                        var selected = false;
                        var arr = CityTier.split(',');
                        $("input[name='cbCityTier']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCityTier();
                        }
                    }
                    else if (Format != "") {
                        var selected = false;
                        var arr = Format.split(',');
                        $("input[name='cbFormat']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeFormat();
                        }
                    }
                    else if (MaterialSupport != "") {
                        var selected = false;
                        var arr = MaterialSupport.split(',');
                        $("input[name='cbMaterialSupport']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMaterialSupport();
                        }
                    }
                    else if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeCornerType: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {
                        var machineFrameList = json[0].MachineFrameNames;
                        var cityTierList = json[0].CityTier;
                        var formatList = json[0].Format;
                        var materialSupportList = json[0].MaterialSupport;
                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;

                        $("#MachineFrameDiv").html("");
                        if (machineFrameList != "") {
                            var machineFrameArr = machineFrameList.split(',');
                            for (var i = 0; i < machineFrameArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMachineFrame' value='" + machineFrameArr[i] + "' />" + machineFrameArr[i] + "&nbsp;&nbsp;";
                                $("#MachineFrameDiv").append(input);
                            }
                            $("#SelectAllMachineFrame").show();
                        }
                        else {
                            $("#SelectAllMachineFrame").hide();
                        }
                        $("#CityTierDiv").html("");
                        if (cityTierList != "") {
                            var cityTierArr = cityTierList.split(',');
                            for (var i = 0; i < cityTierArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCityTier' value='" + cityTierArr[i] + "' />" + cityTierArr[i] + "&nbsp;&nbsp;";
                                $("#CityTierDiv").append(input);
                            }
                        }
                        $("#FormatDiv").html("");
                        if (formatList != "") {
                            var formatArr = formatList.split(',');
                            for (var i = 0; i < formatArr.length; i++) {
                                var input = "<input type='checkbox' name='cbFormat' value='" + formatArr[i] + "' />" + formatArr[i] + "&nbsp;&nbsp;";
                                $("#FormatDiv").append(input);
                            }
                        }
                        $("#MaterialSupportDiv").html("");
                        if (materialSupportList != "") {
                            var materialSupportArr = materialSupportList.split(',');
                            for (var i = 0; i < materialSupportArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                $("#MaterialSupportDiv").append(input);
                            }
                        }
                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#CornerTypeLoading").hide();
                if (conditionRow != null) {

                    var MachineFrame = conditionRow.MachineFrameNames || "";
                    var CityTier = conditionRow.CityTier || "";
                    var Format = conditionRow.Format || "";
                    var MaterialSupport = conditionRow.MaterialSupport || "";
                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";

                    if (MachineFrame != "") {
                        var selected = false;
                        var arr = MachineFrame.split(',');
                        $("input[name='cbMachineFrame']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMachineFrame();
                        }
                    }
                    else if (CityTier != "") {
                        var selected = false;
                        var arr = CityTier.split(',');
                        $("input[name='cbCityTier']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCityTier();
                        }
                    }
                    else if (Format != "") {
                        var selected = false;
                        var arr = Format.split(',');
                        $("input[name='cbFormat']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeFormat();
                        }
                    }
                    else if (MaterialSupport != "") {
                        var selected = false;
                        var arr = MaterialSupport.split(',');
                        $("input[name='cbMaterialSupport']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMaterialSupport();
                        }
                    }
                    else if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeMachineFrame: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var frame = "";
        $("input[name='cbMachineFrame']:checked").each(function () {
            frame += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '","MachineFrameNames":"' + frame + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {

                        var cityTierList = json[0].CityTier;
                        var formatList = json[0].Format;
                        var materialSupportList = json[0].MaterialSupport;
                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;
                        $("#CityTierDiv").html("");
                        if (cityTierList != "") {
                            var cityTierArr = cityTierList.split(',');
                            for (var i = 0; i < cityTierArr.length; i++) {
                                var input = "<input type='checkbox' name='cbCityTier' value='" + cityTierArr[i] + "' />" + cityTierArr[i] + "&nbsp;&nbsp;";
                                $("#CityTierDiv").append(input);
                            }
                        }
                        $("#FormatDiv").html("");
                        if (formatList != "") {
                            var formatArr = formatList.split(',');
                            for (var i = 0; i < formatArr.length; i++) {
                                var input = "<input type='checkbox' name='cbFormat' value='" + formatArr[i] + "' />" + formatArr[i] + "&nbsp;&nbsp;";
                                $("#FormatDiv").append(input);
                            }
                        }
                        $("#MaterialSupportDiv").html("");
                        if (materialSupportList != "") {
                            var materialSupportArr = materialSupportList.split(',');
                            for (var i = 0; i < materialSupportArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                $("#MaterialSupportDiv").append(input);
                            }
                        }
                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#MachineFrameLoading").hide();
                if (conditionRow != null) {

                    var CityTier = conditionRow.CityTier || "";
                    var Format = conditionRow.Format || "";
                    var MaterialSupport = conditionRow.MaterialSupport || "";
                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (CityTier != "") {
                        var selected = false;
                        var arr = CityTier.split(',');
                        $("input[name='cbCityTier']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeCityTier();
                        }
                    }
                    else if (Format != "") {
                        var selected = false;
                        var arr = Format.split(',');
                        $("input[name='cbFormat']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeFormat();
                        }
                    }
                    else if (MaterialSupport != "") {
                        var selected = false;
                        var arr = MaterialSupport.split(',');
                        $("input[name='cbMaterialSupport']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMaterialSupport();
                        }
                    }
                    else if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeCityTier: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var frame = "";
        $("input[name='cbMachineFrame']:checked").each(function () {
            frame += $(this).val() + ",";
        })
        var cityTier = "";
        $("input[name='cbCityTier']:checked").each(function () {
            cityTier += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '","MachineFrameNames":"' + frame + '","CityTier":"' + cityTier + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {

                        var formatList = json[0].Format;
                        var materialSupportList = json[0].MaterialSupport;
                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;

                        $("#FormatDiv").html("");
                        if (formatList != "") {
                            var formatArr = formatList.split(',');
                            for (var i = 0; i < formatArr.length; i++) {
                                var input = "<input type='checkbox' name='cbFormat' value='" + formatArr[i] + "' />" + formatArr[i] + "&nbsp;&nbsp;";
                                $("#FormatDiv").append(input);
                            }
                        }
                        $("#MaterialSupportDiv").html("");
                        if (materialSupportList != "") {
                            var materialSupportArr = materialSupportList.split(',');
                            for (var i = 0; i < materialSupportArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                $("#MaterialSupportDiv").append(input);
                            }
                        }
                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#CityTierLoading").hide();
                if (conditionRow != null) {

                    var Format = conditionRow.Format || "";
                    var MaterialSupport = conditionRow.MaterialSupport || "";
                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (Format != "") {
                        var selected = false;
                        var arr = Format.split(',');
                        $("input[name='cbFormat']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeFormat();
                        }
                    }
                    else if (MaterialSupport != "") {
                        var selected = false;
                        var arr = MaterialSupport.split(',');
                        $("input[name='cbMaterialSupport']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMaterialSupport();
                        }
                    }
                    else if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeFormat: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var frame = "";
        $("input[name='cbMachineFrame']:checked").each(function () {
            frame += $(this).val() + ",";
        })
        var cityTier = "";
        $("input[name='cbCityTier']:checked").each(function () {
            cityTier += $(this).val() + ",";
        })
        var format = "";
        $("input[name='cbFormat']:checked").each(function () {
            format += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '","MachineFrameNames":"' + frame + '","CityTier":"' + cityTier + '","Format":"' + format + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {

                        var materialSupportList = json[0].MaterialSupport;
                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;

                        $("#MaterialSupportDiv").html("");
                        if (materialSupportList != "") {
                            var materialSupportArr = materialSupportList.split(',');
                            for (var i = 0; i < materialSupportArr.length; i++) {
                                var input = "<input type='checkbox' name='cbMaterialSupport' value='" + materialSupportArr[i] + "' />" + materialSupportArr[i] + "&nbsp;&nbsp;";
                                $("#MaterialSupportDiv").append(input);
                            }
                        }
                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#FormatLoading").hide();
                if (conditionRow != null) {

                    var MaterialSupport = conditionRow.MaterialSupport || "";
                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (MaterialSupport != "") {
                        var selected = false;
                        var arr = MaterialSupport.split(',');
                        $("input[name='cbMaterialSupport']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeMaterialSupport();
                        }
                    }
                    else if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeMaterialSupport: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var frame = "";
        $("input[name='cbMachineFrame']:checked").each(function () {
            frame += $(this).val() + ",";
        })
        var cityTier = "";
        $("input[name='cbCityTier']:checked").each(function () {
            cityTier += $(this).val() + ",";
        })
        var format = "";
        $("input[name='cbFormat']:checked").each(function () {
            format += $(this).val() + ",";
        })
        var materialSupport = "";
        $("input[name='cbMaterialSupport']:checked").each(function () {
            materialSupport += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '","MachineFrameNames":"' + frame + '","CityTier":"' + cityTier + '","Format":"' + format + '","MaterialSupport":"' + materialSupport + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {

                        var POSScaleList = json[0].POSScale;
                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;

                        $("#POSScaleDiv").html("");
                        if (POSScaleList != "") {
                            var POSScaleArr = POSScaleList.split(',');
                            for (var i = 0; i < POSScaleArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOSScale' value='" + POSScaleArr[i] + "' />" + POSScaleArr[i] + "&nbsp;&nbsp;";
                                $("#POSScaleDiv").append(input);
                            }
                        }
                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#MaterialSupportLoading").hide();
                if (conditionRow != null) {

                    var POSScale = conditionRow.POSScale || "";
                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (POSScale != "") {
                        var selected = false;
                        var arr = POSScale.split(',');
                        $("input[name='cbPOSScale']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOSScale();
                        }
                    }
                    else if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changePOSScale: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var frame = "";
        $("input[name='cbMachineFrame']:checked").each(function () {
            frame += $(this).val() + ",";
        })
        var cityTier = "";
        $("input[name='cbCityTier']:checked").each(function () {
            cityTier += $(this).val() + ",";
        })
        var format = "";
        $("input[name='cbFormat']:checked").each(function () {
            format += $(this).val() + ",";
        })
        var materialSupport = "";
        $("input[name='cbMaterialSupport']:checked").each(function () {
            materialSupport += $(this).val() + ",";
        })
        var POSScale = "";
        $("input[name='cbPOSScale']:checked").each(function () {
            POSScale += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '","MachineFrameNames":"' + frame + '","CityTier":"' + cityTier + '","Format":"' + format + '","MaterialSupport":"' + materialSupport + '","POSScale":"' + POSScale + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {

                        var IsInstallList = json[0].IsInstall;
                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;

                        $("#IsInstallDiv").html("");
                        if (IsInstallList != "") {
                            var IsInstallArr = IsInstallList.split(',');
                            for (var i = 0; i < IsInstallArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsInstall' value='" + IsInstallArr[i] + "' />" + IsInstallArr[i] + "&nbsp;&nbsp;";
                                $("#IsInstallDiv").append(input);
                            }
                        }
                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#POSScaleLoading").hide();
                if (conditionRow != null) {

                    var IsInstall = conditionRow.IsInstall || "";
                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (IsInstall != "") {
                        var selected = false;
                        var arr = IsInstall.split(',');
                        $("input[name='cbIsInstall']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsInstall();
                        }
                    }
                    else if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeIsInstall: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var frame = "";
        $("input[name='cbMachineFrame']:checked").each(function () {
            frame += $(this).val() + ",";
        })
        var cityTier = "";
        $("input[name='cbCityTier']:checked").each(function () {
            cityTier += $(this).val() + ",";
        })
        var format = "";
        $("input[name='cbFormat']:checked").each(function () {
            format += $(this).val() + ",";
        })
        var materialSupport = "";
        $("input[name='cbMaterialSupport']:checked").each(function () {
            materialSupport += $(this).val() + ",";
        })
        var POSScale = "";
        $("input[name='cbPOSScale']:checked").each(function () {
            POSScale += $(this).val() + ",";
        })
        var IsInstall = "";
        $("input[name='cbIsInstall']:checked").each(function () {
            IsInstall += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '","MachineFrameNames":"' + frame + '","CityTier":"' + cityTier + '","Format":"' + format + '","MaterialSupport":"' + materialSupport + '","POSScale":"' + POSScale + '","IsInstall":"' + IsInstall + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {

                        var quantityList = json[0].Quantity;
                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;

                        $("#QuantityDiv").html("");
                        if (quantityList != "") {
                            var quantityArr = quantityList.split(',');
                            for (var i = 0; i < quantityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbQuantity' value='" + quantityArr[i] + "' />" + quantityArr[i] + "&nbsp;&nbsp;";
                                $("#QuantityDiv").append(input);
                            }
                        }
                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#IsInstallLoading").hide();
                if (conditionRow != null) {

                    var Quantity = conditionRow.Quantity || "";
                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (Quantity != "") {
                        var selected = false;
                        var arr = Quantity.split(',');
                        $("input[name='cbQuantity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeQuantity();
                        }
                    }
                    else if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeQuantity: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var frame = "";
        $("input[name='cbMachineFrame']:checked").each(function () {
            frame += $(this).val() + ",";
        })
        var cityTier = "";
        $("input[name='cbCityTier']:checked").each(function () {
            cityTier += $(this).val() + ",";
        })
        var format = "";
        $("input[name='cbFormat']:checked").each(function () {
            format += $(this).val() + ",";
        })
        var materialSupport = "";
        $("input[name='cbMaterialSupport']:checked").each(function () {
            materialSupport += $(this).val() + ",";
        })
        var POSScale = "";
        $("input[name='cbPOSScale']:checked").each(function () {
            POSScale += $(this).val() + ",";
        })
        var IsInstall = "";
        $("input[name='cbIsInstall']:checked").each(function () {
            IsInstall += $(this).val() + ",";
        })
        var quantity = "";
        $("input[name='cbQuantity']:checked").each(function () {
            quantity += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '","MachineFrameNames":"' + frame + '","CityTier":"' + cityTier + '","Format":"' + format + '","MaterialSupport":"' + materialSupport + '","POSScale":"' + POSScale + '","IsInstall":"' + IsInstall + '","Quantity":"' + quantity + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {

                        var POPSizeList = json[0].POPSize;
                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;

                        $("#POPSizeDiv").html("");
                        if (POPSizeList != "") {
                            var POPSizeArr = POPSizeList.split(',');
                            for (var i = 0; i < POPSizeArr.length; i++) {
                                var input = "<input type='checkbox' name='cbPOPSize' value='" + POPSizeArr[i] + "' />" + POPSizeArr[i] + "&nbsp;&nbsp;";
                                $("#POPSizeDiv").append(input);
                            }
                            $("#SelectAllPOPSize").show();
                        }
                        else {
                            $("#SelectAllPOPSize").hide();
                        }
                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#QuantityLoading").hide();
                if (conditionRow != null) {

                    var POPSize = conditionRow.POPSize || "";
                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (POPSize != "") {
                        var selected = false;
                        var arr = POPSize.split(',');
                        $("input[name='cbPOPSize']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changePOPSize();
                        }
                    }
                    else if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changePOPSize: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var frame = "";
        $("input[name='cbMachineFrame']:checked").each(function () {
            frame += $(this).val() + ",";
        })
        var cityTier = "";
        $("input[name='cbCityTier']:checked").each(function () {
            cityTier += $(this).val() + ",";
        })
        var format = "";
        $("input[name='cbFormat']:checked").each(function () {
            format += $(this).val() + ",";
        })
        var materialSupport = "";
        $("input[name='cbMaterialSupport']:checked").each(function () {
            materialSupport += $(this).val() + ",";
        })
        var POSScale = "";
        $("input[name='cbPOSScale']:checked").each(function () {
            POSScale += $(this).val() + ",";
        })
        var IsInstall = "";
        $("input[name='cbIsInstall']:checked").each(function () {
            IsInstall += $(this).val() + ",";
        })
        var quantity = "";
        $("input[name='cbQuantity']:checked").each(function () {
            quantity += $(this).val() + ",";
        })
        var POPSize = "";
        $("input[name='cbPOPSize']:checked").each(function () {
            POPSize += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '","MachineFrameNames":"' + frame + '","CityTier":"' + cityTier + '","Format":"' + format + '","MaterialSupport":"' + materialSupport + '","POSScale":"' + POSScale + '","IsInstall":"' + IsInstall + '","Quantity":"' + quantity + '","POPSize":"' + POPSize + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {

                        var isElectricityList = json[0].IsElectricity;
                        var chooseImgList = json[0].ChooseImg;

                        $("#IsElectricityDiv").html("");
                        if (isElectricityList != "") {
                            var isElectricityArr = isElectricityList.split(',');
                            for (var i = 0; i < isElectricityArr.length; i++) {
                                var input = "<input type='checkbox' name='cbIsElectricity' value='" + isElectricityArr[i] + "' />" + isElectricityArr[i] + "&nbsp;&nbsp;";
                                $("#IsElectricityDiv").append(input);
                            }
                        }
                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#POPSizeLoading").hide();
                if (conditionRow != null) {

                    var IsElectricity = conditionRow.IsElectricity || "";
                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (IsElectricity != "") {
                        var selected = false;
                        var arr = IsElectricity.split(',');
                        $("input[name='cbIsElectricity']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })
                        if (selected) {
                            Condition.changeIsElectricity();
                        }
                    }
                    else if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    },
    changeIsElectricity: function () {
        var shopNos = $.trim($("#txtShopNos").val());
        var sheet = $("input[name='radioSheet']:checked").val() || "";
        var regions = "";
        $("input[name='cbRegion']:checked").each(function () {
            regions += $(this).val() + ",";
        })
        var province = "";
        $("input[name='cbProvince']:checked").each(function () {
            province += $(this).val() + ",";
        })
        var city = "";
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
        var gender = "";
        $("input[name='cbGender']:checked").each(function () {
            gender += $(this).val() + ",";
        })
        var cornerType = "";
        $("input[name='cbCornerType']:checked").each(function () {
            cornerType += $(this).val() + ",";
        })
        var frame = "";
        $("input[name='cbMachineFrame']:checked").each(function () {
            frame += $(this).val() + ",";
        })
        var cityTier = "";
        $("input[name='cbCityTier']:checked").each(function () {
            cityTier += $(this).val() + ",";
        })
        var format = "";
        $("input[name='cbFormat']:checked").each(function () {
            format += $(this).val() + ",";
        })
        var materialSupport = "";
        $("input[name='cbMaterialSupport']:checked").each(function () {
            materialSupport += $(this).val() + ",";
        })
        var POSScale = "";
        $("input[name='cbPOSScale']:checked").each(function () {
            POSScale += $(this).val() + ",";
        })
        var IsInstall = "";
        $("input[name='cbIsInstall']:checked").each(function () {
            IsInstall += $(this).val() + ",";
        })
        var quantity = "";
        $("input[name='cbQuantity']:checked").each(function () {
            quantity += $(this).val() + ",";
        })
        var POPSize = "";
        $("input[name='cbPOPSize']:checked").each(function () {
            POPSize += $(this).val() + ",";
        })
        var isElectricity = "";
        $("input[name='cbIsElectricity']:checked").each(function () {
            isElectricity += $(this).val() + ",";
        })
        var jsonStr = '{"SubjectId":' + subjectId + ',"ShopNos":"' + shopNos + '","PositionName":"' + sheet + '","RegionNames":"' + regions + '","ProvinceId":"' + province + '","CityId":"' + city + '","Gender":"' + gender + '","CornerType":"' + cornerType + '","MachineFrameNames":"' + frame + '","CityTier":"' + cityTier + '","Format":"' + format + '","MaterialSupport":"' + materialSupport + '","POSScale":"' + POSScale + '","IsInstall":"' + IsInstall + '","Quantity":"' + quantity + '","POPSize":"' + POPSize + '","IsElectricity":"' + isElectricity + '"}';
        $.ajax({
            type: "post",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "changeCondition", jsonStr: escape(jsonStr) },
            success: function (data) {

                if (data != "") {
                    var json = eval(data);

                    if (json.length > 0) {
                        var chooseImgList = json[0].ChooseImg;

                        $("#ChooseImgDiv").html("");
                        if (chooseImgList != "") {
                            var chooseImgArr = chooseImgList.split(',');
                            for (var i = 0; i < chooseImgArr.length; i++) {
                                var input = "<input type='checkbox' name='cbChooseImg' value='" + chooseImgArr[i] + "' />" + chooseImgArr[i] + "&nbsp;&nbsp;";
                                $("#ChooseImgDiv").append(input);
                            }
                        }
                    }
                }
            },
            complete: function () {
                $("#IsElectricityLoading").hide();
                if (conditionRow != null) {

                    var ChooseImg = conditionRow.ChooseImg || "";
                    if (ChooseImg != "") {

                        var arr = ChooseImg.split(',');
                        $("input[name='cbChooseImg']").each(function () {

                            if ($.inArray($(this).val(), arr) != -1) {
                                $(this).attr("checked", "checked");
                                selected = true;
                            }

                        })

                    }
                }
            }
        })
    }
};

var Plan = {
    getList: function () {
        $("#tbPlanList").datagrid({
            queryParams: { type: 'getPlanList', customerId: customerId, subjectId: subjectId },
            method: 'get',
            url: '/Subjects/Handler/SplitPlan.ashx',
            cache: false,
            columns: [[
                            { field: 'Id', hidden: true },

                            { field: 'ShopNos1', title: '店铺编号',
                                formatter: function (value, rec) {
                                    var val = rec.ShopNos;
                                    val = val.length > 15 ? (val.substring(0, 15) + "...") : val;

                                    return val;
                                }

                            },
                            { field: 'PositionName', title: 'POP位置', sortable: true },
                            { field: 'RegionNames', title: '区域' },
                            {
                                field: 'ProvinceName1', title: '省份', formatter: function (value, row, index) {
                                    var provinceNames = row.ProvinceName;

                                    if (provinceNames.length > 20) {
                                        provinceNames = provinceNames.substring(0, 20) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                                    }
                                    return '<span>' + provinceNames + '</span>';
                                }
                            },
                            {
                                field: 'CityName1', title: '城市', formatter: function (value, row, index) {
                                    var cityNames = row.CityName;
                                    if (cityNames.length > 20) {
                                        cityNames = cityNames.substring(0, 20) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                                    }
                                    return '<span>' + cityNames + '</span>';
                                }
                            },
                            { field: 'Gender', title: '性别', sortable: true },
                            { field: 'CornerType', title: '角落类型', sortable: true },
                            { field: 'MachineFrameNames', title: '器架类型', sortable: true },
                            { field: 'CityTier', title: '城市级别', sortable: true },
                            { field: 'Format', title: '店铺类型', sortable: true },
                            { field: 'MaterialSupport', title: '物料支持类别', sortable: true },
                            { field: 'POSScale', title: '店铺规模', sortable: true },
                            { field: 'IsInstall', title: '是否安装', sortable: true },
                            { field: 'Quantity', title: '数量' },
                            { field: 'GraphicMaterial', title: 'POP材质' },
                            { field: 'POPSize', title: 'POP尺寸', sortable: true },
                            { field: 'WindowSize', title: 'Window尺寸' },
                            { field: 'IsElectricity', title: '通电否' },
                            { field: 'ChooseImg', title: '选图' },
                            { field: 'KeepPOPSize', title: '是否保留POP原尺寸' },
                            { field: 'NotInvolveShopNos', title: '不参与店铺' }
                ]],
            singleSelect: true,
            toolbar: '#toolbar',
            fit: false,
            collapsible: true,
            fitColumns: false,
            height: 'auto',
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            onLoadSuccess: function (data) {
                $(this).datagrid('clearSelections');
            },
            onSelect: function (rowIndex, rowData) {
                LoadCondition(rowData);
            },
            view: detailview,
            detailFormatter: function (index, row) {
                return '<div style="padding:2px;"> <table id="ddv_' + index + '"></table></div>';
            },
            onExpandRow: function (index, row) {
                commonExpandRow(index, row, this, 'ddv');
            }

        });
    },
    bindMaterialList: function () {
        $.ajax({
            type: "get",
            url: "/Subjects/Handler/SplitPlan.ashx",
            data: { type: "getCustomerMaterial", customerId: customerId, subjectId: subjectId },
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
    },
    addPlanContent: function () {
        if (CheckAddContentVal()) {
            $("#container input").each(function () {
                if ($(this).attr("Id") != "btnAddPlanContent" && $(this).attr("name") != "cbMaterial") {
                    $(this).val("");

                }
            });
            $("#txtNum").val("1");
        }
    },
    submitPlan: function (optype) {
        if (CheckSubmitPlanVal()) {
            
            $("#btnSubmitPlan").parent().hide().prev("div").show();
            $.ajax({
                type: "post",
                url: "/Subjects/Handler/SplitPlan.ashx",
                data: { type: "add", optype: optype, jsonStr: escape(PlanJsonStr) },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        Plan.getList();
                        ClearConditionVal();
                        Condition.init();
                        $("#btnSubmitPlan").parent().show().prev("div").hide();
                        window.parent.HasChange();
                    }
                    else {
                        alert("操作失败：" + data);
                    }
                }
            })
        }
    },
    loadDetailData: function () {
        $.ajax({
            type: "get",
            url: "/Subjects/Handler/SplitPlan.ashx?type=getPlanDetail&planId=" + CurrPlanId,
            cache: false,
            success: function (data) {
                $("#addContent").html("");
                var json = eval(data);
                if (json.length > 0) {
                    var tr1 = "";
                    for (var i = 0; i < json.length; i++) {


                        var windowType = "";
                        if (json[i].WindowType != "") {
                            windowType = "data-windowtype='" + json[i].WindowType + "'";
                        }
                        tr1 += "<tr class='tr_bai content'>";
                        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='" + json[i].OrderType + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
                        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='" + json[i].GraphicWidth + "' style='width: 90%; text-align: center;' /></td>";
                        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='" + json[i].GraphicLength + "' style='width: 90%; text-align: center;' /></td>";
                        tr1 += "<td><input type='text' name='txtMaterial1' maxlength='100' value='" + json[i].GraphicMaterial + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";


                        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value='" + json[i].RackSalePrice + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
                        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='" + json[i].Quantity + "' style='width: 90%; text-align: center;' /></td>";

                        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='" + json[i].NewChooseImg + "' style='width: 90%; text-align: center;' /></td>";
                        tr1 += "<td><input type='text' name='txtRemark1'  " + windowType + " maxlength='30' value='" + json[i].Remark + "' style='width: 90%; text-align: center;' /></td>";

                        tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
                        tr1 += "</tr>";
                    }
                    $("#addContent").append(tr1);
                }
            }
        })
    }
};

function commonExpandRow(index, row, target, nextName) {

    var curName = nextName + '_' + index;
    var id = row.Id;
    $('#' + curName).datagrid({
        method: 'get',
        url: '/Subjects/Handler/SplitPlan.ashx',
        cache: false,
        queryParams: { planId: id, type: "getPlanDetail", t: Math.random() * 1000 },
        fitColumns: false,
        height: 'auto',
        pagination: false,
        columns: [[
                    { field: 'Id', hidden: true },
                    { field: 'CustomerMaterialId', hidden: true },
                    { field: 'OrderType', title: '类型' },
                    { field: 'GraphicWidth', title: '宽' },
                    { field: 'GraphicLength', title: '高' },
                    { field: 'GraphicMaterial', title: '材质' },
                    { field: 'RackSalePrice', title: '销售价' },
                    { field: 'Quantity', title: '数量' },
                    { field: 'NewChooseImg', title: '新选图' },
                    { field: 'Remark', title: '备注' },
                    
            ]],
       
        onResize: function () {
            $(target).datagrid('fixDetailRowHeight', index);
        },
        onLoadSuccess: function (data) {
            $(target).datagrid('clearSelections');
            setTimeout(function () {
                $(target).datagrid('fixDetailRowHeight', index);
            }, 0);

        },
        onClickRow: function (rowIndex, rowData) {
            $(this).datagrid("unselectRow", rowIndex);

        }

    });
    $(target).datagrid('fixDetailRowHeight', index);

}


//添加方案内容
function CheckAddContentVal() {

    var planTypeId = $("#selPlanType").val();
    var planTypeName1 = $("#selPlanType option:selected").text();
    var width1 = $("#txtWidth").val();
    var length1 = $("#txtLength").val();

    if (planTypeId == "1") {
        if ($.trim(width1) == "") {
            alert("请填写宽");
            return false;
        }
        if ($.trim(length1) == "") {
            alert("请填写高");
            return false;
        }
    }
    if ($.trim(width1) != "" && isNaN(width1)) {
        alert("宽必须是数字");
        return false;
    }
    if ($.trim(length1) != "" && isNaN(length1)) {
        alert("高必须是数字");
        return false;
    }
    var material1 = $("#txtMaterial").val();
   
    var RackPrice1 = $("#txtRackPrice").val();

    var num1 = $("#txtNum").val();
    if ($.trim(num1) == "") {
        alert("请填写数量");
        return false;
    }
    else if (isNaN(num1)) {
        alert("数量必须是数字");
        return false;
    }
    var chooseImg1 = $("#txtChooseImg").val();
    var remark1 = $("#txtRemark").val();
    if ($.trim(remark1) == "") {
        alert("请填写备注");
        return false;
    }
    var tr1 = "<tr class='tr_bai content'>";
    tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='" + planTypeName1 + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='" + width1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='" + length1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtMaterial1' maxlength='100' readonly='readonly' value='" + material1 + "'  style='width: 90%; text-align: center;' /></td>";

    tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value='" + RackPrice1 + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='" + num1 + "' style='width: 90%; text-align: center;' /></td>";
    
    tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='" + chooseImg1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtRemark1' maxlength='50' value='" + remark1 + "' style='width: 90%; text-align: center;' /></td>";
    
    tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
    tr1 += "</tr>";
    $("#addContent").append(tr1);

    return true;
}

//新增方案
function CheckSubmitPlanVal() {

    PlanJsonStr = "";
    var canSubmit = true;
    
    var ShopNos = $.trim($("#txtShopNos").val());
    
    var sheet = $("input[name='radioSheet']:checked").val() || "";
    
    if (sheet == "") {
        alert("请选择pop位置");
        return false;
    }
    var txtNum = $.trim($("#txtNum").val());
    var txtRemark = $.trim($("#txtRemark").val());
    if (txtNum != "" && txtRemark != "") {
        alert("还有方案内容没添加，请添加后再提交");
        return false;
    }
    var region = "";
    $("input[name='cbRegion']:checked").each(function () {
        region += $(this).val() + ",";
    })

    var province = ""
    $("input[name='cbProvince']:checked").each(function () {
        province += $(this).val() + ",";

    })
    var city = "";
    if (city == "") {
        $("input[name='cbCity']:checked").each(function () {
            city += $(this).val() + ",";
        })
    }

    var cityTier = "";
    $("input[name='cbCityTier']:checked").each(function () {
        cityTier += $(this).val() + ",";
    })

    var Format = "";
    $("input[name='cbFormat']:checked").each(function () {
        Format += $(this).val() + ",";

    })

    var MaterialSupport = "";
    $("input[name='cbMaterialSupport']:checked").each(function () {
        MaterialSupport += $(this).val() + ",";

    })
    var Install = "";
    $("input[name='cbIsInstall']:checked").each(function () {
        Install += $(this).val() + ",";

    })
    var POSScale = "";
    $("input[name='cbPOSScale']:checked").each(function () {
        POSScale += $(this).val() + ",";
    })
    var CornerTypes = "";
    $("input[name='cbCornerType']:checked").each(function () {
        CornerTypes += $(this).val() + ",";
    })

    var MachineFrameNames = "";
    $("input[name='cbMachineFrame']:checked").each(function () {
        MachineFrameNames += $(this).val() + ",";
    })

    var Gender = "";
    $("input[name='cbGender']:checked").each(function () {
        Gender += $(this).val() + ",";
    })

    var Quantity = "";
    $("input[name='cbQuantity']:checked").each(function () {
        Quantity += $(this).val() + ",";
    })

    var POPSize = "";
    $("input[name='cbPOPSize']:checked").each(function () {
        POPSize += $(this).val() + ",";

    })

    var IsElectricity = "";
    $("input[name='cbIsElectricity']:checked").each(function () {
        IsElectricity += $(this).val() + ",";

    })

    var ChooseImg = "";
    $("input[name='cbChooseImg']:checked").each(function () {
        ChooseImg += $(this).val() + ",";
    })

    var KeepPOPSize = "";
    if ($("#cbKeepSize").attr("checked") == "checked") {
        KeepPOPSize = true;
    }

    var NotInvolveShopNos = $.trim($("#txtNoInvolveShopNos").val());

    if (region.length > 0)
        region = region.substring(0, region.length - 1);
    if (province.length > 0)
        province = province.substring(0, province.length - 1);
    if (city.length > 0)
        city = city.substring(0, city.length - 1);
    if (cityTier.length > 0)
        cityTier = cityTier.substring(0, cityTier.length - 1);
    if (Format.length > 0)
        Format = Format.substring(0, Format.length - 1);
   
    if (MaterialSupport.length > 0)
        MaterialSupport = MaterialSupport.substring(0, MaterialSupport.length - 1);
    if (Install.length > 0)
        Install = Install.substring(0, Install.length - 1);
    if (POSScale.length > 0)
        POSScale = POSScale.substring(0, POSScale.length - 1);

    if (CornerTypes.length > 0)
        CornerTypes = CornerTypes.substring(0, CornerTypes.length - 1);

    if (MachineFrameNames.length > 0)
        MachineFrameNames = MachineFrameNames.substring(0, MachineFrameNames.length - 1);
    if (Gender.length > 0)
        Gender = Gender.substring(0, Gender.length - 1);


    if (Quantity.length > 0)
        Quantity = Quantity.substring(0, Quantity.length - 1);
   
    if (POPSize.length > 0)
        POPSize = POPSize.substring(0, POPSize.length - 1);
    
    if (ChooseImg.length > 0)
        ChooseImg = ChooseImg.substring(0, ChooseImg.length - 1);
    if (IsElectricity.length > 0)
        IsElectricity = IsElectricity.substring(0, IsElectricity.length - 1);

    var addcontent = $("#addContent tr[class$='content']");
    //户外店项目可以不添加方案内容，
    if ($("#hfSubjectCategoryName").val() != "户外" && addcontent.length == 0) {
        alert("请先添加方案内容");
        return false;
    }
    
    var planDetailJson = "";
    for (var i = 0; i < addcontent.length; i++) {
        var td = $(addcontent[i]);
        var planType = $(td).find("td").eq(0).find("input[name='txtOrderType1']").val() || "";
        planType = planType != "" ? planType == "pop" ? "1" : "2" : "1";
        var width = $(td).find("input[name='txtWidth1']").val() || 0;
        var length = $(td).find("input[name='txtLength1']").val() || 0;
        var material = $(td).find("input[name='txtMaterial1']").val() || "";
        var RackPrice = $(td).find("input[name='txtPrice1']").val() || 0;
        var num = $(td).find("input[name='txtQuantity1']").val() || 0;
        var newChooseImg = $(td).find("input[name='txtChooseImg1']").val();
        var remark = $(td).find("input[name='txtRemark1']").val();
        var windowType = $(td).find("input[name='txtRemark1']").data("windowtype") || "";
        var isInSet = 0;
        var isPerShop = 0;
        planDetailJson += '{"OrderType":' + planType + ',"GraphicWidth":' + width + ',"GraphicLength":' + length + ',"Quantity":' + num + ',"GraphicMaterial":"' + material + '","RackSalePrice":' + RackPrice + ',"Remark":"' + remark + '","NewChooseImg":"' + newChooseImg + '","WindowType":"' + windowType + '"},';
    }
    
    if (planDetailJson.length > 0) {
        planDetailJson = planDetailJson.substring(0, planDetailJson.length - 1);

    }
    PlanJsonStr = '{"Id":' + CurrPlanId + ',"CustomerId":' + customerId + ',"SubjectId":' + subjectId + ',"ProvinceId":"' + province + '","CityId":"' + city + '","RegionNames":"' + region + '","CityTier":"' + cityTier + '","ShopNos":"' + ShopNos + '","IsInstall":"' + Install + '","Format":"' + Format + '","MaterialSupport":"' + MaterialSupport + '","POSScale":"' + POSScale + '","PositionName":"' + sheet + '","CornerType":"' + CornerTypes + '","MachineFrameNames":"' + MachineFrameNames + '","Quantity":"' + Quantity + '","Gender":"' + Gender + '",';

    PlanJsonStr += '"POPSize":"' + POPSize + '","ChooseImg":"' + ChooseImg + '","KeepPOPSize":"' + KeepPOPSize + '","IsElectricity":"' + IsElectricity + '","NotInvolveShopNos":"' + NotInvolveShopNos + '"';


    PlanJsonStr += ',"SplitOrderPlanDetail":[' + planDetailJson + ']}';
    PlanJsonStr = PlanJsonStr.replace(/×/g, "乘");
    
    return true;

}

//清空条件信息的选择
function ClearConditionVal() {
    CurrPlanId = 0;
    conditionRow = null;
    $("#ConditionTB input[type='text']").val("");
    $("#ConditionTB input[type='checkbox']").attr("checked", false);
    $("#ConditionTB input[type='radio']").attr("checked", false);
    $("#addContent").html("");
}

//清空条件信息里面的内容
function EmptyConditionVal() {
    $("#ConditionTB div").each(function () {
        if ($(this).prop("id").indexOf("Div") != -1) {
            $(this).html("");
        }
    })
}


//选择方案后，加载方案信息
function LoadCondition(row) {
    ClearConditionVal();
    if (row != null) {
       CurrPlanId = row.Id;
       conditionRow = row;
       var shopNos = row.ShopNos || "";
       if (shopNos != "") {
           $("#txtShopNos").val(shopNos);
       }
       var sheet = row.PositionName || "";
       var selected = false;
       $("input:radio[name='radioSheet']").each(function () {
           if ($(this).val() == sheet) {
               $(this).attr("checked", "checked");
               selected = true;
           }

       })
       if (selected) {
           Condition.changeSheet();
       }
       var KeepPOPSize = row.KeepPOPSize;
       if (KeepPOPSize == "是")
           $("#cbKeepSize").attr("checked", "checked");

       if (row.NotInvolveShopNos != "") {
           $("#txtNoInvolveShopNos").val(row.NotInvolveShopNos);
       }
       Plan.loadDetailData();
   }
}

//添加陈列桌尺寸 addType:1正常尺寸，2 反包尺寸
function AddTableSize(addType) {

    var sheet = $("input:radio[name='radioSheet']:checked").val() || "";
    var frameName = "";
    $("input[name = 'cbMachineFrame']:checked").each(function () {
        frameName += $(this).val() + ",";
    })
    if (sheet != "陈列桌") {
        return false;
    }
    if (frameName == "") {
        alert("请选择器架名称");
    }
    else {
        $.ajax({
            type: "get",
            url: "/Subjects/Handler/SplitPlan.ashx?type=getTableSize&sheet=" + sheet + "&frameName=" + frameName + "&addType=" + addType + "&subjectId=" + subjectId,
            success: function (data) {
                if (data != "") {

                    var json = eval(data);
                    var tr2 = "";
                    for (var i = 0; i < json.length; i++) {
                        var length = 0;
                        var width = 0;
                        var remark = json[i].Remark;
                        if (addType == 1) {
                            width = json[i].NormalWidth;
                            length = json[i].NormalLength;
                        }
                        else if (addType == 2) {
                            width = json[i].WithEdgeWidth;
                            length = json[i].WithEdgeLength;
                            if (remark.indexOf("反包") == -1)
                                remark = remark + "(反包)";
                        }
                        tr2 += "<tr class='tr_bai content'>";
                        tr2 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtWidth1' maxlength='20' value='" + width + "' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtLength1' maxlength='20' value='" + length + "' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtMaterial1' maxlength='100' readonly='readonly' value='" + json[i].MaterialName + "'  style='width: 90%; text-align: center;' /></td>";
                       
                        tr2 += "<td><input type='text' name='txtPrice1' maxlength='50'  value='" + json[i].Price + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='" + json[i].Quantity + "' style='width: 90%; text-align: center;' /></td>";
                        
                        tr2 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtRemark1' maxlength='50' value='" + remark + "' style='width: 90%; text-align: center;' /></td>";
                        
                        tr2 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
                        tr2 += "</tr>";
                    }
                    $("#addContent").append(tr2);
                }
            }
        })
    }
}
