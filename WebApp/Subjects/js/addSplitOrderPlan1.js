
var SelectMaterialSpan;


$(function () {
    //    $("#btnAddChuChuang").attr("disabled", "disabled");
    //    $("#btnAddTableSize").attr("disabled", "disabled");
    GetConditions();
    Plan.getList();
    BindCustomerMaterial();
    //,input[type='radio']
    $("#ConditionTB").delegate("input[type='checkbox']", "click", function () {

        planModel = null;
        if (!clickToChange)
            clickToChange = true;
        var name = $(this).attr("name");

        ExecuteChange(name);

    })
    $("#ConditionTB").delegate("input[type='radio']", "change", function () {

        planModel = null;
        if (!clickToChange)
            clickToChange = true;
        var name = $(this).attr("name");
        if (name == "PositionNameDivrd") {//pop位置选择
            //            $("#btnAddChuChuang").attr("disabled", "disabled");
            //            $("#btnAddTableSize").attr("disabled", "disabled");
            $(".showLoading").show();
            $("input[name='MachineFrameNamesDivcb']").change();
            //            var val = $(this).val();
            //            if (val == "橱窗") {
            //                $("#btnAddChuChuang").attr("disabled", false);
            //            }
            //            if (val == "陈列桌") {
            //                $("#btnAddTableSize").attr("disabled", false);
            //            }
        }
        ExecuteChange(name);



    })
    //各个条件的全选
    $("input[name='cbAll']").on("click", function () {
        var checked = this.checked;

        $(this).parent().next("div").find("input[type='checkbox']").each(function () {
            $(this).attr("checked", checked);

        })
        //SetConditionControl();
        //ChangeConditions(name);
        if ($(this).attr("id") == "provinceCBAll") {
            SetConditionControl();
            ChangeConditions("ProvinceNameDivcb");
        }
    })

    //全选每个省份的城市
    $("#CityNameDiv").delegate("input[name='citycbAll']", "click", function () {
        var checked = this.checked;

        $(this).parent().next("td").find("input[type='checkbox']").each(function () {
            $(this).attr("checked", checked);
        })
        SetConditionControl();
        ChangeConditions("CityNameDivcb");
    })
    //取消器架的选择
    $("#MFCancelSelect").on("click", function () {
        $("#MachineFrameNamesDiv").find("input").each(function () {
            $(this).attr("checked", false);
        })
        SetConditionControl();
        ChangeConditions("MachineFrameNamesDivcb");
    })

    //按店铺编号取条件
    $("#txtShopNos").on("blur", function () {
        var val = $(this).val();
        GetConditions(val);
    })

    $("#container").delegate("span[name='btnSelectMaterial']", "click", function () {
        SelectMaterialSpan = $(this);
        $(this).siblings("div").show();
    })

    //材质只能单选
    $("#container").delegate("input[name='cbMaterial']", "change", function () {
        if (this.checked) {
            $(".customerMaterials").find("input[name='cbMaterial']").not($(this)).attr("checked", false);
        }
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
            $(SelectMaterialSpan).siblings("input[name='txtMaterial']").val(mName);
            $(SelectMaterialSpan).parent().parent().parent().find("input[name='txtRackPrice']").val(price);

        }
        $(".divMaterialList").hide();
    })

    //新增材质
    $("#container").delegate("span[name='btnAddMaterial']", "click", function () {
        Material.bindCustomer();
        Material.add();

    })

    //添加方案内容
    $("#btnAddPlan").on("click", function () {
        Plan.add();
    })
    //新增方案
    $("#btnSubmitPlan").on("click", function () {
        Plan.submit("add");
    })
    //更新方案
    $("#btnUpdatePlan").on("click", function () {
        Plan.update();
    })


    //刷新方案
    $("#btnRefresh").on("click", function () {
        Plan.refresh();
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
                url: "/Subjects/Handler/SplitOrder1.ashx?type=deletePlan&planIds=" + ids,
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        Plan.getList();
                        ClearConditionVal();
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

    //删除方案内容
    $("#addContent").delegate(".deletePlanDetail", "click", function () {
        $(this).parent().parent().remove();
    })

    //添加左/右窗贴，地铺，窗贴
    $("#btnAddChuChuang").click(function () {
        var sheet = $("input:radio[name='PositionNameDivrd']:checked").val() || "";

        if (sheet == "" || sheet.indexOf("橱窗") == -1) {
            return false;
        }
        //左侧贴
        var tr1 = "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='hidden' name='hfMaterialId1' value='' /><input type='text' name='txtMaterial1' maxlength='100' value=''   style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtSupplier1' value='' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50'  value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' readonly='readonly' data-windowtype='LeftSideStick' maxlength='50' value='左侧贴' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='checkbox' name='cbInSet1'/></td>";

        tr1 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";
        //右侧贴
        tr1 += "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='hidden' name='hfMaterialId1' value='' /><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtSupplier1' value='' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' readonly='readonly' data-windowtype='RightSideStick' maxlength='50' value='右侧贴' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='checkbox' name='cbInSet1'/></td>";

        tr1 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";
        //地铺
        tr1 += "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='hidden' name='hfMaterialId1' value='' /><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtSupplier1' value='' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' readonly='readonly' data-windowtype='Floor'  maxlength='50' value='地贴' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='checkbox' name='cbInSet1'/></td>";

        tr1 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";
        //窗贴
        tr1 += "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='hidden' name='hfMaterialId1' value='' /><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtSupplier1' value='' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' readonly='readonly' data-windowtype='WindowStick' maxlength='50' value='窗贴' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='checkbox' name='cbInSet1'/></td>";

        tr1 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";


        $("#addContent").append(tr1);
    })


    //添加陈列桌尺寸(正常尺寸)
    $("#btnAddNormalSize").click(function () {
        AddTableSize(1)
    })

    //添加陈列桌尺寸(反包尺寸)
    $("#btnAddWithEdgelSize").click(function () {
        AddTableSize(2)
    })


    //清空方案内容数据
    $("#spanClareDate").on("click", function () {
        if (confirm("确定清除吗？")) {
            $("#addContent").html("");
        }
    })

    var sheetName1 = "";
    $("#MachineFrameNamesDiv").delegate("input[name='MachineFrameNamesDivcb']", "change", function () {
        var flag = false;
        $("#MachineFrameNamesDiv").find("input[name='MachineFrameNamesDivcb']:checked").each(function () {
            flag = $(this).val() == "空";
        })
        if (flag) {
            $("#EmptyFrameShopTr").show();
            var sheet = $("input:radio[name='PositionNameDivrd']:checked").val() || "";
            if (sheet != sheetName1) {
                sheetName1 = sheet;
                $.ajax({
                    type: "get",
                    url: "/Subjects/Handler/SplitOrder1.ashx",
                    data: { type: "getEmptyFrameShopCount", subjectId: subjectId, sheet: sheet },
                    success: function (data) {
                        if (data != "") {

                            $("#emptyFrameShopCount").html(data);

                        }
                    }

                })
            }
        }
        else
            $("#EmptyFrameShopTr").hide();
    })

    $("#exportEmptyFrameShop").click(function () {
        var sheet = $("input:radio[name='PositionNameDivrd']:checked").val() || "";
        $("#exportFrame").attr("src", "ExportEmptyFrameShop.aspx?subjectId=" + subjectId + "&sheet=" + sheet);
    })
})
var controls = [];
var currControlIndex = 0; //触发“change”事件的空间在条件table的索引
var conditionControlArr = []; //有条件数据的控件名称数组
function GetConditions(shopNos) {
    $("#SheetLoading").show();
    var shopNo = shopNos || "";
    //获取条件
    $.ajax({
        type: "get",
        url: "/Subjects/Handler/SplitOrder1.ashx",
        data: { type: "getCondition", subjectId: subjectId, shopNos: shopNo },
        cache: false,
        success: function (data) {

            if (data != "") {
                if (data.indexOf("不存在") != -1) {
                    $("#SheetLoading").hide();
                    alert(data);
                }
                else {
                    var json = eval(data);

                    if (json.length > 0) {
                        if (json[0].Sheet == "") {
                            alert("该项目不含此店铺订单");
                            $("#SheetLoading").hide();
                        }

                        InitCondition("PositionNameDiv", json[0].Sheet);
                        InitCondition("RegionNamesDiv", json[0].RegionNames);
                        InitCondition("CityTierDiv", json[0].CityTier);
                        InitCondition("FormatDiv", json[0].Format);
                        //InitCondition("ShopLevelDiv", json[0].ShopLevel);
                        InitCondition("MaterialSupportDiv", json[0].MaterialSupport);
                        InitCondition("IsInstallDiv", json[0].IsInstall);
                        InitCondition("POSScaleDiv", json[0].POSScale);
                        InitCondition("GenderDiv", json[0].Gender);
                        InitCondition("QuantityDiv", json[0].Quantity);
                        InitCondition("GraphicMaterialDiv", json[0].GraphicMaterial);
                        InitCondition("POPSizeDiv", json[0].WindowSize);
                        InitCondition("WindowSizeDiv", json[0].WindowSize);
                        InitCondition("ChooseImgDiv", json[0].ChooseImg);
                        InitCondition("IsElectricityDiv", json[0].IsElectricity);


                    }
                }

            }
        }

    })
}

function InitCondition(element, valStr) {
    var input = "";
    var cbName = element + "cb";
    var hasSelect = false;
    
    if ($.trim(valStr) != "") {
        var selectedarr = [];
        var oldArr = [];
        if (planModel != null) {

            var val = eval("planModel." + element.replace("Div", ""));

            if (val != "") {
                oldArr = val.split(',');
                hasSelect = true;

            }

        }
        else {
            var checkedVal = "";
            $("input[name='" + cbName + "']:checked").each(function () {
                checkedVal += $(this).val() + ",";
            })

            if (checkedVal.length > 0) {
                selectedarr = checkedVal.substring(0, checkedVal.length - 1).split(',');
            }
        }

        $("#" + element).siblings(".divSelect").show();
        var arr = valStr.split(',');
        if (arr.length > 0) {

            if (element == "PositionNameDiv") {

                for (var i = 0; i < arr.length; i++) {

                    input += "<input type='radio' name='PositionNameDivrd' value='" + arr[i] + "'/><span>" + arr[i] + "</span>&nbsp;";
                }
                $("#SheetLoading").hide();
            }
            else if (element == "CityNameDiv") {

                var pName = "";
                var table = "<table>";
                var count = arr.length;
                for (var i = 0; i < count; i++) {

                    var province = arr[i].split('_')[0];
                    var city = arr[i].split('_')[1];
                    var checked = "";
                    if (oldArr.length > 0) {
                        for (var k = 0; k < oldArr.length; k++) {
                            if (oldArr[k] == city) {
                                checked = "checked='checked'";
                                hasSelect = true;
                            }
                        }
                    }
                    else if (selectedarr.length > 0) {
                        for (var j = 0; j < selectedarr.length; j++) {
                            if (selectedarr[j] == city) {
                                checked = "checked='checked'";
                                hasSelect = true;
                            }
                        }
                    }
                    var div = "";
                    div += "<div style='float:left;'>";
                    div += "<input type='checkbox' name='CityNameDivcb' value='" + city + "' " + checked + "/><span>" + city + "</span>&nbsp;";
                    div += "</div>";
                    if (pName != province) {
                        if (i > 0)
                            table += "</td></tr>";
                        pName = province;
                        table += "<tr>";
                        table += "<td style='width:80px;border-right:1px solid #ccc; vertical-align:top; padding-top:5px;'><input type='checkbox' name='citycbAll'/>" + pName + "</td><td style='vertical-align:top; padding-top:5px;'>";
                    }
                    table += div;
                    if (i == count - 1)
                        table += "</td></tr>";


                }
                table += "</table>";
                input = table;

            }
            else {


                for (var i = 0; i < arr.length; i++) {

                    if (element == "RegionNamesDiv") {
                        var option = "<option value='" + arr[i] + "'>" + arr[i] + "</option>";
                        $("#selRegion").append(option);
                    }
                    var checked = "";

                    if (oldArr.length > 0) {
                        for (var kk = 0; kk < oldArr.length; kk++) {
                            if (oldArr[kk] == arr[i]) {
                                checked = "checked='checked'";
                                hasSelect = true;
                            }
                        }
                    }
                    else if (selectedarr.length > 0) {
                        for (var j = 0; j < selectedarr.length; j++) {
                            if (selectedarr[j] == arr[i]) {
                                checked = "checked='checked'";
                                hasSelect = true;
                            }
                        }
                    }
                    
                    input += '<div style="float:left;">';
                    input += '<input type="checkbox" name="' + cbName + '" value="' + arr[i] + '" ' + checked + '/>' + arr[i] + '&nbsp;';
                    input += '</div>';
                    
                }


            }

        }


    }
    $("#" + element).html(input);
    if (hasSelect) {

        ExecuteChange(cbName);

    }
    //return input;
}

//条件中所有的checkbox和radio
function SetConditionControl() {
    var namesArr = [];
    controls = [];
    var index = 1;
    //", "input[type='radio']
    $("#ConditionTB").find("input[type='checkbox']").each(function () {
        var name = $(this).attr("name");
        var flag = true;

        $.each(namesArr, function (key, val) {
            if (val == name)
                flag = false;
        })
        if (flag) {
            namesArr.push(name);
            controls.push({ "cName": "" + name + "", "cIndex": "" + index + "" });
            index++;
        }
    });

    namesArr = null;
}
//获取有选择中条件的控件的值
function GetSelectedConditions(cName) {

    var PlanJsonStr1 = "";
    var ShopNos = $.trim($("#txtShopNos").val());
    var Sheet = "";
    var RegionNames = "";
    var ProvinceName = ""
    var CityName = "";
    var Gender = "";
    var CornerType = "";
    var MachineFrameNames = "";
    var CityTier = "";
    var Format = "";
    //var ShopLevel = "";
    var MaterialSupport = "";
    var IsInstall = "";
    var POSScale = "";
    var Quantity = "";
    var GraphicMaterial = "";
    var POPSize = "";
    var WindowSize = "";
    var ChooseImg = "";
    var flag = false;
    currControlIndex = 0;
    conditionControlArr = [];
    $.each(controls, function (key, val) {
        if (val.cName == cName) {
            currControlIndex = val.cIndex;
            flag = true;

        }
    })

    if (flag) {
        //把当前控件以及在前面的控件的名称保存起来
        $.each(controls, function (key, val) {
            if (parseInt(val.cIndex) < (parseInt(currControlIndex) + 1)) {
                if (val.cName != "undefined")
                    conditionControlArr.push(val.cName);
            }
        })

        if (conditionControlArr.length > 0) {
            $.each(conditionControlArr, function (key, val) {

                var agr1 = val.replace('Divcb', '');
                var js = '$("#' + agr1 + 'Div").find("input[name=\'' + agr1 + 'Divcb\']:checked").each(function () {' + agr1 + '+= $(this).val() + ",";})';
                eval(js);

            })
        }

    }

    Sheet = $("input:radio[name='PositionNameDivrd']:checked").val() || "";
    if (RegionNames.length > 0)
        RegionNames = RegionNames.substring(0, RegionNames.length - 1);
    if (ProvinceName.length > 0)
        ProvinceName = ProvinceName.substring(0, ProvinceName.length - 1);
    if (CityName.length > 0)
        CityName = CityName.substring(0, CityName.length - 1);
    if (Gender.length > 0)
        Gender = Gender.substring(0, Gender.length - 1);
    if (CornerType.length > 0)
        CornerType = CornerType.substring(0, CornerType.length - 1);
    if (MachineFrameNames.length > 0)
        MachineFrameNames = MachineFrameNames.substring(0, MachineFrameNames.length - 1);
    if (CityTier.length > 0)
        CityTier = CityTier.substring(0, CityTier.length - 1);
    if (Format.length > 0)
        Format = Format.substring(0, Format.length - 1);
//    if (ShopLevel.length > 0)
//        ShopLevel = ShopLevel.substring(0, ShopLevel.length - 1);
    if (MaterialSupport.length > 0)
        MaterialSupport = MaterialSupport.substring(0, MaterialSupport.length - 1);
    if (IsInstall.length > 0)
        IsInstall = IsInstall.substring(0, IsInstall.length - 1);
    if (POSScale.length > 0)
        POSScale = POSScale.substring(0, POSScale.length - 1);


    if (Quantity.length > 0)
        Quantity = Quantity.substring(0, Quantity.length - 1);
    if (GraphicMaterial.length > 0)
        GraphicMaterial = GraphicMaterial.substring(0, GraphicMaterial.length - 1);
    if (POPSize.length > 0)
        POPSize = POPSize.substring(0, POPSize.length - 1);
    if (WindowSize.length > 0)
        WindowSize = WindowSize.substring(0, WindowSize.length - 1);
    if (ChooseImg.length > 0)
        ChooseImg = ChooseImg.substring(0, ChooseImg.length - 1);
    //var GraphicNo = $.trim($("#txtGraphicNo").val());
    PlanJsonStr1 = '{"CustomerId":' + customerId + ',"SubjectId":' + subjectId + ',"ProvinceId":"' + ProvinceName + '","CityId":"' + CityName + '","RegionNames":"' + RegionNames + '","CityTier":"' + CityTier + '","ShopNos":"' + ShopNos + '","IsInstall":"' + IsInstall + '","Format":"' + Format + '","MaterialSupport":"' + MaterialSupport + '","POSScale":"' + POSScale + '","PositionName":"' + Sheet + '","CornerType":"' + CornerType + '","MachineFrameNames":"' + MachineFrameNames + '","Quantity":"' + Quantity + '","Gender":"' + Gender + '",';

    PlanJsonStr1 += '"GraphicMaterial":"' + GraphicMaterial + '","POPSize":"' + POPSize + '","WindowSize":"' + WindowSize + '","ChooseImg":"' + ChooseImg + '"}';

    return PlanJsonStr1;
}
function ExecuteChange(name) {

    if (name.indexOf("All") != -1)
        return false;
    ChangeConditions(name);
}

function ChangeConditions(name) {
    SetConditionControl();
    var conditions = GetSelectedConditions(name);

    $.ajax({
        type: "post",
        url: "/Subjects/Handler/SplitOrder1.ashx",
        data: { type: "changeCondition", jsonStr: escape(conditions) },
        cache: false,
        async: clickToChange,
        success: function (data) {

            if (data != "") {

                var jsonData = eval("(" + data + ")");
                $(".showLoading").hide();
                $(".conditionDiv").each(function () {
                    var id = $(this).attr("id");

                    var field = id.replace("Div", "");
                    var canChange = true;

                    $.each(conditionControlArr, function (key, val) {
                        //alert(val);
                        var agr1 = val.replace('cb', '');

                        if (agr1 == id)
                            canChange = false;
                    })

                    if (canChange) {

                        //var js = '$("#' + id + '").html(InitCondition("' + id + '", jsonData[0].' + field + '));';
                        var js = 'InitCondition("' + id + '", jsonData[0].' + field + ');';
                        eval(js);

                    }
                })


            }
            if ($("#ProvinceNameDiv").html() == "")
                $("#provinceAllDiv").hide();
            if ($("#CityNameDiv").html() == "")
                $("#cityAllDiv").hide();
            if ($("#MachineFrameNamesDiv").html() == "")
                $("#divMFCancelSelect").hide();
        }
    })
}
//获取客户材质（报价）
function BindCustomerMaterial() {
    //var region = $("#selRegion").val();
    $.ajax({
        type: "get",

        url: "../Handler/SplitOrder1.ashx?type=getCustomerMaterial&customerId=" + customerId + "&SubjectId=" + subjectId,
        cache: false,
        success: function (data) {
            //alert(data);
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


//方案类
var PlanJsonStr = "";
var CurrPlanId = 0;
var planModel = null;
var Plan = {
    add: function () {
        if (CheckAddContentVal()) {
            $("#container input").each(function () {
                if ($(this).attr("Id") != "btnAddPlan" && $(this).attr("name") != "cbMaterial") {
                    $(this).val("");
                }
            });
        }
    },
    submit: function (optype) {

        if (CheckSubmitVal()) {
           
            $("#btnSubmitPlan").parent().hide().prev("div").show();
            $.ajax({
                type: "post",
                url: "/Subjects/Handler/SplitOrder1.ashx",
                data: { type: "add", optype: optype, jsonStr: escape(PlanJsonStr) },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        Plan.getList();
                        //ClearConditionVal();
                        ClearAddContentVal();
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
    update: function () {
        Plan.submit("update");
    },
    getList: function () {
        $("#tbPlanList").datagrid({
            queryParams: { type: 'getList', CustomerId: customerId, SubjectId: subjectId, planType: 1 },
            method: 'get',
            url: '/Subjects/Handler/SplitOrder1.ashx',
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
            //{ field: 'ShopLevel', title: '店铺级别', sortable: true },
                            {field: 'MaterialSupport', title: '物料支持类别', sortable: true },
                            { field: 'POSScale', title: '店铺规模', sortable: true },
                            { field: 'IsInstall', title: '是否安装', sortable: true },
                            { field: 'Quantity', title: '数量' },
                            { field: 'GraphicMaterial', title: 'POP材质' },
                            { field: 'POPSize', title: 'POP尺寸', sortable: true },
                            { field: 'WindowSize', title: 'Window尺寸' },
                            { field: 'IsElectricity', title: '通电否' },
                            { field: 'ChooseImg', title: '选图' },
                            { field: 'KeepPOPSize', title: '是否保留POP原尺寸' },
                            { field: 'NoMainKV', title: '不含主KV' },
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
    refresh: function () {

        this.getList();
    }
}
var selected = false;
var clickToChange = true;
function commonExpandRow(index, row, target, nextName) {

    var curName = nextName + '_' + index;
    var id = row.Id;
    $('#' + curName).datagrid({
        method: 'get',
        url: '/Subjects/Handler/SplitOrder.ashx',
        cache: false,
        queryParams: { planId: id, type: "getDetail", t: Math.random() * 1000 },
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
                    { field: 'Supplier', title: '供货方' },
                    { field: 'RackSalePrice', title: '销售价' },
                    { field: 'Quantity', title: '数量' },
                    { field: 'NewGender', title: '性别' },
                    { field: 'NewChooseImg', title: '新选图' },
                    { field: 'Remark', title: '备注' },
                    { field: 'IsInSet', title: '男女共一套' },
        //{ field: 'IsPerShop', title: '单店设置' }
            ]],
        //        frozenColumns: [[
        //                    {
        //                        field: 'edit', title: '删除', align: 'center',
        //                        formatter: function (value, rec) {
        //                            var btn = '<a href="javascript:void(0)" onclick="deleteDetail(' + rec.Id + ')">删除</a>';
        //                            return btn;
        //                        }
        //                    }
        //                     ]],
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
    var material1 = $("#container").find("input[name='txtMaterial']").val();
    var materialId1 = $("#container").find("input[name='hfMaterialId']").val();

    var Supplier1 = $("#txtSupplier").val();
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
    var isInSet = "";
    var isPerShop = "";
    if ($("#cbInSet").attr("checked") == "checked") {
        isInSet = "checked='checked'";
    }
    if ($("#cbPerShop").attr("checked") == "checked") {
        isPerShop = "checked='checked'";
    }



    var tr1 = "<tr class='tr_bai content'>";

    tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='" + planTypeName1 + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='" + width1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='" + length1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='hidden' name='hfMaterialId1' value='" + materialId1 + "' /><input type='text' name='txtMaterial1' maxlength='100' readonly='readonly' value='" + material1 + "'  style='width: 90%; text-align: center;' /></td>";

    tr1 += "<td><input type='text' name='txtSupplier1' value='" + Supplier1 + "' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value='" + RackPrice1 + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='" + num1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='" + chooseImg1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtRemark1' maxlength='50' value='" + remark1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='checkbox' name='cbInSet1' " + isInSet + "/></td>";
    //tr1 += "<td><input type='checkbox' name='cbPerShop1' " + isPerShop + "/><span name='spanPerShop' style='color:blue;cursor:pointer;display:none;'>设置</span></td>";
    tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
    tr1 += "</tr>";
    $("#addContent").append(tr1);

    return true;
}
//清空方案内容输入
function ClearAddContentVal() {
    PlanJsonStr = "";
    $("#addContent").html("");
    $("#container input").each(function () {
        if ($(this).attr("Id") != "btnAddPlan" && $(this).attr("name") != "cbMaterial") {
            $(this).val("");
        }
    });
}
//清空条件信息的选择
function ClearConditionVal() {
    CurrPlanId = 0;
    $("#ConditionTB input[type='text']").val("");
    $("#ConditionTB input[type='checkbox']").attr("checked", false);
    $("#ConditionTB input[type='radio']").attr("checked", false);
    $("#addContent").html("");
    $("#txtNum").val("1");

}

function CheckSubmitVal() {

    PlanJsonStr = "";
    var canSubmit = true;
    var ShopNos = $.trim($("#txtShopNos").val());
    var PositionName = $("input:radio[name='PositionNameDivrd']:checked").val() || "";
    if (PositionName == "") {
        alert("请选择pop位置");
        //canSubmit = false;
        return false;
    }

    var txtNum = $.trim($("#txtNum").val());
    var txtRemark = $.trim($("#txtRemark").val());
    if (txtNum != "" && txtRemark != "") {
        alert("还有方案内容没添加，请添加后再提交");
        return false;
    }

    var region = "";
    $("#RegionNamesDiv").find("input[name='RegionNamesDivcb']:checked").each(function () {
        region += $(this).val() + ",";


    })
    

    var province = ""
    $("#ProvinceNameDiv").find("input[name='ProvinceNameDivcb']:checked").each(function () {
        province += $(this).val() + ",";

    })
    var city = "";

    if (city == "") {
        $("#CityNameDiv").find("input[name='CityNameDivcb']:checked").each(function () {
            city += $(this).val() + ",";

        })
    }

    var cityTier = "";
    $("#CityTierDiv").find("input[name='CityTierDivcb']:checked").each(function () {
        cityTier += $(this).val() + ",";

    })

    var Format = "";
    $("#FormatDiv").find("input[name='FormatDivcb']:checked").each(function () {
        Format += $(this).val() + ",";

    })

    var MaterialSupport = "";
    $("#MaterialSupportDiv").find("input[name='MaterialSupportDivcb']:checked").each(function () {
        MaterialSupport += $(this).val() + ",";

    })
    var Install = "";
    $("#IsInstallDiv").find("input[name='IsInstallDivcb']:checked").each(function () {
        Install += $(this).val() + ",";

    })
    var Scale = "";
    $("#POSScaleDiv").find("input[name='POSScaleDivcb']:checked").each(function () {
        Scale += $(this).val() + ",";

    })
    var CornerTypes = "";
    $("#CornerTypeDiv").find("input[name='CornerTypeDivcb']:checked").each(function () {
        CornerTypes += $(this).val() + ",";

    })

    var MachineFrameNames = "";
    $("#MachineFrameNamesDiv").find("input[name='MachineFrameNamesDivcb']:checked").each(function () {
        MachineFrameNames += $(this).val() + ",";

    })

    var Gender = "";
    $("#GenderDiv").find("input[name='GenderDivcb']:checked").each(function () {
        Gender += $(this).val() + ",";

    })

    var Quantity = "";
    $("#QuantityDiv").find("input[name='QuantityDivcb']:checked").each(function () {
        Quantity += $(this).val() + ",";

    })

    var GraphicMaterial = "";
    $("#GraphicMaterialDiv").find("input[name='GraphicMaterialDivcb']:checked").each(function () {
        GraphicMaterial += $(this).val() + ",";

    })

    var POPSize = "";
    $("#POPSizeDiv").find("input[name='POPSizeDivcb']:checked").each(function () {
        POPSize += $(this).val() + ",";

    })

    var WindowSize = "";
    $("#WindowSizeDiv").find("input[name='WindowSizeDivcb']:checked").each(function () {
        WindowSize += $(this).val() + ",";

    })


    var IsElectricity = "";
    $("#IsElectricityDiv").find("input[name='IsElectricityDivcb']:checked").each(function () {
        IsElectricity += $(this).val() + ",";

    })

    var ChooseImg = "";
    $("#ChooseImgDiv").find("input[name='ChooseImgDivcb']:checked").each(function () {
        ChooseImg += $(this).val() + ",";

    })
    
    var KeepPOPSize = "";
    if ($("#cbKeepSize").attr("checked") == "checked") {
        KeepPOPSize = true;
    }

    var NoMainKV = "";
    if ($("#cbNoMainKV").attr("checked") == "checked") {
        NoMainKV = true;
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
    if (Scale.length > 0)
        Scale = Scale.substring(0, Scale.length - 1);

    if (CornerTypes.length > 0)
        CornerTypes = CornerTypes.substring(0, CornerTypes.length - 1);

    if (MachineFrameNames.length > 0)
        MachineFrameNames = MachineFrameNames.substring(0, MachineFrameNames.length - 1);
    if (Gender.length > 0)
        Gender = Gender.substring(0, Gender.length - 1);


    if (Quantity.length > 0)
        Quantity = Quantity.substring(0, Quantity.length - 1);
    if (GraphicMaterial.length > 0)
        GraphicMaterial = GraphicMaterial.substring(0, GraphicMaterial.length - 1);

    if (POPSize.length > 0)
        POPSize = POPSize.substring(0, POPSize.length - 1);
    if (WindowSize.length > 0)
        WindowSize = WindowSize.substring(0, WindowSize.length - 1);


    if (ChooseImg.length > 0)
        ChooseImg = ChooseImg.substring(0, ChooseImg.length - 1);
    if (IsElectricity.length > 0)
        IsElectricity = IsElectricity.substring(0, IsElectricity.length - 1);

    var GraphicNo = $.trim($("#txtGraphicNo").val());

    var addcontent = $("#addContent tr[class$='content']");
    //户外店项目可以不添加方案内容，
    if ($("#hfSubjectCategoryName").val()!="户外" && addcontent.length == 0) {
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

        var materialId = $(td).find("input[name='hfMaterialId1']").val() || "0";
        var material = $(td).find("input[name='txtMaterial1']").val() || "";

        var Supplier = $(td).find("input[name='txtSupplier1']").val() || "";
        var RackPrice = $(td).find("input[name='txtPrice1']").val() || 0;
        var num = $(td).find("input[name='txtQuantity1']").val() || 0;
        var newGender = $(td).find("input[name='txtNewGender1']").val() || "";
        var newChooseImg = $(td).find("input[name='txtChooseImg1']").val();
        var remark = $(td).find("input[name='txtRemark1']").val();
        var isHCPOP = $(td).find("input[name='txtRemark1']").data("hcpop") || 0;
        var windowType = $(td).find("input[name='txtRemark1']").data("windowtype") || "";
        if (isHCPOP == 0) {
            num = num || 1;
        }
        var isInSet = 0;
        if ($(td).find("input[name='cbInSet1']").attr("checked") == "checked") {
            isInSet = 1;
        }
        var isPerShop = 0;
        if ($(td).find("input[name='cbPerShop1']").attr("checked") == "checked") {
            isPerShop = 1;
        }
        planDetailJson += '{"OrderType":' + planType + ',"GraphicWidth":' + width + ',"GraphicLength":' + length + ',"Quantity":' + num + ',"CustomerMaterialId":' + materialId + ',"GraphicMaterial":"' + material + '","Supplier":"' + Supplier + '","RackSalePrice":' + RackPrice + ',"Remark":"' + remark + '","NewChooseImg":"' + newChooseImg + '","IsInSet":' + isInSet + ',"IsPerShop":' + isPerShop + ',"NewGender":"' + newGender + '","IsHCPOP":' + isHCPOP + ',"WindowType":"' + windowType + '"},';
    }

    if (planDetailJson.length > 0) {
        planDetailJson = planDetailJson.substring(0, planDetailJson.length - 1);

    }
    PlanJsonStr = '{"Id":' + CurrPlanId + ',"CustomerId":' + customerId + ',"SubjectId":' + subjectId + ',"ProvinceId":"' + province + '","CityId":"' + city + '","RegionNames":"' + region + '","CityTier":"' + cityTier + '","ShopNos":"' + ShopNos + '","IsInstall":"' + Install + '","Format":"' + Format + '","MaterialSupport":"' + MaterialSupport + '","POSScale":"' + Scale + '","PositionName":"' + PositionName + '","CornerType":"' + CornerTypes + '","MachineFrameNames":"' + MachineFrameNames + '","Quantity":"' + Quantity + '","Gender":"' + Gender + '",';

    PlanJsonStr += '"GraphicMaterial":"' + GraphicMaterial + '","POPSize":"' + POPSize + '","WindowSize":"' + WindowSize + '","ChooseImg":"' + ChooseImg + '","KeepPOPSize":"' + KeepPOPSize + '","IsElectricity":"' + IsElectricity + '","NotInvolveShopNos":"' + NotInvolveShopNos + '","NoMainKV":"' + NoMainKV + '"';


    PlanJsonStr += ',"SplitOrderPlanDetail":[' + planDetailJson + ']}';
    PlanJsonStr = PlanJsonStr.replace(/×/g, "乘");

    return true;

}

function LoadCondition(row) {
    ClearConditionVal();
    if (row != null) {
        clickToChange = false;
        planModel = row;
        CurrPlanId = row.Id;
        if (row.ShopNos != "") {
            $("#txtShopNos").val(row.ShopNos);
        }

        var sheet = row.PositionName || "";
        var selected = false;
        $("input:radio[name='PositionNameDivrd']").each(function () {
            if ($(this).val() == sheet) {
                $(this).attr("checked", "checked");
                selected = true;
            }

        })
        if (selected)
            ChangeConditions("PositionNameDivrd");

        var ChooseImg = row.ChooseImg || "";
        if (ChooseImg.length > 0) {
            var ChooseImgArr = ChooseImg.split(',');
            for (var i = 0; i < ChooseImgArr.length; i++) {
                $("input[name='ChooseImgDivcb']").each(function () {
                    if ($(this).val() == ChooseImgArr[i]) {
                        $(this).attr("checked", "checked");
                    }

                })
            }
        }

        var KeepPOPSize = row.KeepPOPSize;
        if (KeepPOPSize == "是")
            $("#cbKeepSize").attr("checked", "checked");

        if (row.NotInvolveShopNos != "") {
            $("#txtNoInvolveShopNos").val(row.NotInvolveShopNos);
        }

        LoadPlanDetail();
    }
}

function LoadPlanDetail() {
    $.ajax({
        type: "get",
        url: "/Subjects/Handler/SplitOrder1.ashx?type=getDetail&planId=" + CurrPlanId,
        cache: false,
        success: function (data) {
            $("#addContent").html("");
            var json = eval(data);
            if (json.length > 0) {
                var tr1 = "";
                for (var i = 0; i < json.length; i++) {

                    var isInSet = json[i].IsInSet == "是" ? "checked='checked'" : "";
                    var isPerShop = "";
                    var style1 = "style='color:blue;cursor:pointer;display:none;'";
                    if (json[i].IsPerShop == "是") {
                        isPerShop = "checked='checked'";
                        style1 = "style='color:blue;cursor:pointer;'";
                    }
                    var isHCPOP = "";
                    if (json[i].IsHCPOP == 1) {
                        isHCPOP = "data-hcpop='1'";
                    }
                    var windowType = "";
                    if (json[i].WindowType != "") {
                        windowType = "data-windowtype='" + json[i].WindowType + "'";
                    }
                    tr1 += "<tr class='tr_bai content'>";




                    tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='" + json[i].OrderType + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='" + json[i].GraphicWidth + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='" + json[i].GraphicLength + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtMaterial1' maxlength='100' value='" + json[i].GraphicMaterial + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";

                    tr1 += "<td><input type='text' name='txtSupplier1' value='" + json[i].Supplier + "' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value='" + json[i].RackSalePrice + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='" + json[i].Quantity + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='" + json[i].NewGender + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='" + json[i].NewChooseImg + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtRemark1' " + isHCPOP + " " + windowType + " maxlength='30' value='" + json[i].Remark + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='checkbox' name='cbInSet1' " + isInSet + "/></td>";
                    //tr1 += "<td><input type='checkbox' name='cbPerShop1' " + isPerShop + "/><span name='spanPerShop' " + style1 + ">设置</span></td>";
                    tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
                    tr1 += "</tr>";
                }
                $("#addContent").append(tr1);
            }
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

//添加陈列桌尺寸 addType:1正常尺寸，2 反包尺寸
function AddTableSize(addType) {
    
    var sheet = $("input:radio[name='PositionNameDivrd']:checked").val() || "";
    var frameName = "";
    $("input[name = 'MachineFrameNamesDivcb']:checked").each(function () {
        frameName += $(this).val() + ",";
    })
    if (frameName == "") {
        alert("请选择器架名称");
    }
    else {
        $.ajax({
            type: "get",
            url: "/Subjects/Handler/SplitOrder1.ashx?type=getTableSize&sheet=" + sheet + "&frameName=" + frameName + "&addType=" + addType + "&SubjectId=" + subjectId,
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
                            if (remark.indexOf("反包")==-1)
                              remark = remark + "(反包)";
                        }
                        tr2 += "<tr class='tr_bai content'>";
                        tr2 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtWidth1' maxlength='20' value='" + width + "' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtLength1' maxlength='20' value='" + length + "' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='hidden' name='hfMaterialId1' value='' /><input type='text' name='txtMaterial1' maxlength='100' readonly='readonly' value='" + json[i].MaterialName + "'  style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtSupplier1' value='' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtPrice1' maxlength='50'  value='" + json[i].Price + "' readonly='readonly' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='" + json[i].Quantity + "' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='text' name='txtRemark1' maxlength='50' value='" + remark + "' style='width: 90%; text-align: center;' /></td>";
                        tr2 += "<td><input type='checkbox' name='cbInSet1'/></td>";
                        tr2 += "<td><span class='deletePlanDetail'  style='color:red;cursor:pointer;'>删除</span></td>";
                        tr2 += "</tr>";
                    }
                    $("#addContent").append(tr2);
                }
            }
        })
    }
}

