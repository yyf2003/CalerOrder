
//var customerId = $("#hfCustomerId").val() || 0;
//选择方案列表中的行，加载省市，
var loadselectedCity = "";
var loadselectedProvince = "";
//在更多城市层按确定后，保存所选择的城市
var selectedCity = "";
var selectedCityNames = "";
var SelectMaterialSpan;
var txtMaterialInput;
var hfMaterialIdInput;
$(function () {
    $("#SheetLoading").show();
    GetRegoin();
    GetConditions();
    Plan.getList();





    //选择位置后获取器架类型
    $("#SheetDiv").delegate("input[name='SheetDivrd']", "change", function () {
        $(".showLoading").show();
        ChangeRegion();
        GetCornerType("");
        GetMachineFrame();
        GetConditions();
    })

    //选择角落类型后获取器架类型
    $("#CornerTypeDiv").delegate("input[name='CornerTypeDivcb']", "change", function () {
        $("#MachineFrameDiv").prev("div").show();
        GetMachineFrame();
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
    //导入
    $("#btnImport").on("click", function () {
        var url = "ImportSplitPlanDetail.aspx?customerId=" + customerId + "&subjectId=" + subjectId;
        $.fancybox.open({
            href: url,
            type: 'iframe',
            padding: 5,
            width: "98%"
        });
    })

    //刷新方案
    $("#btnRefresh").on("click", function () {
        Plan.refresh();
    })

    //删除方案
    $("#btnDelete").on("click", function () {
        var selectRow = $("#tbPlanList").datagrid("getSelected");
        if (selectRow != null) {
            var pid = selectRow.Id;
            $.ajax({
                type: "get",
                url: "../Handler/SplitOrder.ashx?type=deletePlan&planId=" + pid,
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

    })



    //检查输出是否为数值
    $("#txtGraphicWidth,#txtGraphicLength,#txtWindowWidth,#txtWindowHigh,#txtWindowDeep").on("blur", function () {
        var val = $.trim($(this).val());
        if ($.trim(val) != "") {
            if (isNaN(val)) {
                alert("必须填写大于0的数值");
                $(this).val("");
            }
            if (parseFloat(val) == 0) {
                alert("必须填写大于0的数值");
                $(this).val("");
            }
        }
    })


    //检查输出是否为数值
    $("#txtWidth,#txtLength").on("blur", function () {
        var val = $.trim($(this).val());
        if ($.trim(val) != "") {
            if (isNaN(val)) {
                $(this).val("0");
            }
        }
    })

    //检查数量的输入是否为整数
    var isInt = /^\d{1,}$/;
    $("#txtConditionNum").on("blur", function () {
        var val = $.trim($(this).val());
        if ($.trim(val) != "") {
            if (!isInt.exec(val)) {
                alert("必须填写大于0的整数");
                $(this).val("");
            }
            if (parseFloat(val) == 0) {
                alert("必须填写大于0的整数");
                $(this).val("");
            }
        }
    })
    $("#txtNum").on("blur", function () {
        var val = $(this).val();
        if ($.trim(val) != "") {
            if (!isInt.exec(val)) {
                $(this).val("1");
            }
            if (parseFloat(val) == 0) {
                $(this).val("1");
            }
        }
    })

    //删除方案内容
    $("#addContent").delegate(".deletePlanDetail", "click", function () {

        $(this).parent().parent().remove();
    })


    $("#RegionNameDiv").delegate("input[name='RegionNameDivcb']", "change", function () {

        ChangeRegion();
    })

    //选择省份
    $("#ProvinceNameDiv").delegate("input[name='ProvinceNameDivcb']", "change", function () {
        ChangeProvince();

    })

    //省份全选
    $("#provinceCBAll").on("click", function () {
        var checked = this.checked;
        $("#ProvinceNameDiv input[name='ProvinceNameDivcb']").each(function () {
            $(this).attr("checked", checked);
        })
        GetCity();
    })
    //城市全选
    $("#cityCBAll").on("click", function () {
        var checked = this.checked;
        $("#CityNameDiv input[name='CityNameDivcb']").each(function () {
            $(this).attr("checked", checked);
        })

    })

    //更多城市全选
    $("#moreCityCBAll").on("click", function () {
        var checked = this.checked;
        $("#showCityListDiv input[name='moreCityCB']").each(function () {
            $(this).attr("checked", checked);
        })
        ChangeMoreCitySelect();
    })


    //查找城市
    $("#btnSearchCity").on("click", function () {
        $("#showCityListDiv span").each(function () {
            $(this).css("color", "#010204");
        })
        var key = $("#txtSearchCity").val();
        //$(this).css("color", "#010204");
        if (key != "") {
            var arr = key.split(',');
            for (var i = 0; i < arr.length; i++) {
                $("#showCityListDiv span").each(function () {

                    if ($(this).html().indexOf(arr[i]) != -1) {
                        $(this).css("color", "#f00");
                    }

                })
            }

        }
    })

    //选择城市
    $("#CityNameDiv").delegate("input[name='CityNameDivcb']", "change", function () {

        var cbcount = $("#CityNameDiv input[name='CityNameDivcb']").length;
        var checkedcount = $("#CityNameDiv input[name='CityNameDivcb']:checked").length;
        if (cbcount == checkedcount) {
            $("#cityCBAll").attr("checked", "checked");
        }
        else
            $("#cityCBAll").attr("checked", false);


    })
    //选择城市（更多城市）
    $("#showCityListDiv").delegate("input[name='moreCityCB']", "change", function () {
        ChangeMoreCitySelect();
    })

    //打开客户材质选择层
    //    $("span[name='btnSelectMaterial']").on("click", function () {
    //        //alert("ddd");
    //        //$("#divMaterialList").show();
    //        $(this).siblings("div").show();
    //    })

    $("#addContent,#container").delegate("span[name='btnSelectMaterial']", "click", function () {
        SelectMaterialSpan = $(this);

        $(this).siblings("div").show();
    })

    //材质只能单选
    $("#addContent,#container").delegate("input[name='cbMaterial']", "change", function () {
        if (this.checked) {
            $(".customerMaterials").find("input[name='cbMaterial']").not($(this)).attr("checked", false);
        }
    })



    $("#selRegion").on("change", function () {
        BindCustomerMaterial();
    })


    //提交选择好的材质
    $("#addContent,#container").delegate("span[name='btnSubmitMaterial']", "click", function () {

        var div = $(this).parent().parent().parent().parent();
        //var div = $(table).parent();
        //var mId = "";
        var mName = "";
        var price = "";
        $(div).find(".customerMaterials").find("input[name='cbMaterial']:checked").each(function () {
            //mId = $(this).val();
            mName = $(this).siblings("span").html();
            price = $(this).siblings("span").data("price");
        })

        if (mName.length > 0) {
            $(SelectMaterialSpan).siblings("input[name='txtMaterial']").val(mName);
            //$(SelectMaterialSpan).siblings("input[name='hfMaterialId']").val(mId);
            //$(SelectMaterialSpan).siblings("input[name='txtMaterial1']").val(mName);
            //$(SelectMaterialSpan).siblings("input[name='hfMaterialId1']").val(mId);
            $(SelectMaterialSpan).parent().parent().parent().find("input[name='txtRackPrice']").val(price);
            //$(SelectMaterialSpan).parent().parent().parent().find("input[name='txtRackPrice1']").val(price);
        }


        $(".divMaterialList").hide();
    })


    $("#selRegion").on("change", function () {

        BindCustomerMaterial();
    })


    //按照条件获取pop订单基础数据
    $("#btnGetBaseInfo").on("click", function () {

        if (CheckSubmitVal()) {
            $("#spanWait").show();
            $.ajax({
                type: "post",
                url: "../Handler/SplitOrder.ashx?type=getPOPBaseInfo",
                data: { jsonStr: escape(PlanJsonStr) },
                cache: false,
                success: function (data) {
                    $("#spanWait").hide();
                    if (data != "") {
                        var json = eval(data);
                        for (var i = 0; i < json.length; i++) {
                            var tr1 = "<tr class='tr_bai content'>";
                            tr1 += "<td>pop</td>";
                            tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='" + json[i].Width + "' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='" + json[i].Length + "' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><div style='position:relative;'><input type='hidden' name='hfMaterialId1' /><input type='text' name='txtMaterial1' maxlength='100' value='" + json[i].GraphicMaterial + "' style='width: 70%; text-align: center;' /><span name='btnSelectMaterial' style='color:Blue; cursor:pointer;'>选择</span><div class='divMaterialList'></div></div></td>"; //

                            tr1 += "<td><input type='text' name='txtSupplier1' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value='" + json[i].UnitPrice + "' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='" + json[i].Quantity + "' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtRemark1' maxlength='50' value='" + json[i].Sheet + "' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
                            tr1 += "</tr>";
                            $("#addContent").append(tr1);
                        }
                        MaterialDiv();
                    }
                    else
                        alert("无pop基础数据");
                }
            })
        }

    })

    $("#spanClareDate").on("click", function () {
        if (confirm("确定清除吗？")) {
            $("#addContent").html("");
        }
    })


    $("#btnAddShopNos").on("click", function () {
        if ($("#divEditShopNos").is(":hidden")) {
            $("#divEditShopNos").show().animate({
                height: '259px',
                width: '220px',
                top: "-265px"
            }, 200);
        }
    })
    $("#closeEditShopNos").on("click", function () {
        $("#divEditShopNos").animate({
            height: '1px',
            width: '1px',
            top: "1px"
        }, 200, function () {
            $("#divEditShopNos").hide();
        });
    })

    $("#btnSubmitShopNos").on("click", function () {
        $("#txtShopNos").val($("#txtDivShopNos").val());
    })


    $("#CityNameDiv").delegate("input[name='CPDivcb']", "change", function () {
        var checked = this.checked;
        $(this).parent().siblings("td").find("div input").each(function () {
            this.checked = checked;
        })
    })

    $("#btnAddChuChuang").click(function () {
        var tr1 = "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='hidden' name='hfMaterialId1' value='' /><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtSupplier1' value='' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' maxlength='50' value='左侧贴' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='checkbox' name='cbInSet1'/></td>";
        tr1 += "<td><input type='checkbox' name='cbPerShop1'/><span name='spanPerShop' style='color:blue;cursor:pointer;display:none;'>设置</span></td>";
        tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";

        tr1 += "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='hidden' name='hfMaterialId1' value='' /><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtSupplier1' value='' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' maxlength='50' value='右侧贴' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='checkbox' name='cbInSet1'/></td>";
        tr1 += "<td><input type='checkbox' name='cbPerShop1'/><span name='spanPerShop' style='color:blue;cursor:pointer;display:none;'>设置</span></td>";
        tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";

        tr1 += "<tr class='tr_bai content'>";
        tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='pop' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='hidden' name='hfMaterialId1' value='' /><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtSupplier1' value='' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='1' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='text' name='txtRemark1' maxlength='50' value='地铺' style='width: 90%; text-align: center;' /></td>";
        tr1 += "<td><input type='checkbox' name='cbInSet1'/></td>";
        tr1 += "<td><input type='checkbox' name='cbPerShop1'/><span name='spanPerShop' style='color:blue;cursor:pointer;display:none;'>设置</span></td>";
        tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
        tr1 += "</tr>";


        $("#addContent").append(tr1);
    })

    // PlanJsonStr = ""url: "../Handler/SplitOrder.ashx?type=add&optype=" + optype,
    $("#addContent").delegate("input[name = 'cbPerShop1']", "change", function () {
        if (this.checked) {
            $(this).next("span").show();
        }
        else {
            $(this).next("span").hide();
        }
    })
    $("#addContent").delegate("span[name = 'spanPerShop']", "click", function () {

        PlanJsonStr = "";
        if (CheckSubmitVal()) {
            $("#TablePerShop").html("");
            var planDetailJson1 = "";

            var tr = $(this).parent().parent();
            var planType = $(tr).find("td").eq(0).find("input[name='txtOrderType1']").val() || "pop";
            planType = planType == "pop" ? "1" : "2"; ;


            var width = $(tr).find("input[name='txtWidth1']").val() || 0;
            var length = $(tr).find("input[name='txtLength1']").val() || 0;

            var materialId = $(tr).find("input[name='hfMaterialId1']").val() || "0";
            var material = $(tr).find("input[name='txtMaterial1']").val() || "";

            var Supplier = $(tr).find("input[name='txtSupplier1']").val() || "";
            var RackPrice = $(tr).find("input[name='txtPrice1']").val() || 0;
            var num = $(tr).find("input[name='txtQuantity1']").val() || 1;
            var newChooseImg = $(tr).find("input[name='txtChooseImg1']").val();
            var remark = $(tr).find("input[name='txtRemark1']").val();
            var isInSet = 0;
            if ($(tr).find("input[name='cbInSet1']").attr("checked") == "checked") {
                isInSet = 1;
            }
            var isPerShop = 0;
            if ($(tr).find("input[name='cbPerShop1']").attr("checked") == "checked") {
                isPerShop = 1;
            }
            planDetailJson1 = '{"OrderType":' + planType + ',"GraphicWidth":' + width + ',"GraphicLength":' + length + ',"Quantity":' + num + ',"CustomerMaterialId":' + materialId + ',"GraphicMaterial":"' + material + '","Supplier":"' + Supplier + '","RackSalePrice":' + RackPrice + ',"Remark":"' + remark + '","NewChooseImg":"' + newChooseImg + '","IsInSet":' + isInSet + ',"IsPerShop":' + isPerShop + '}';



            var newJson = JSON.parse(PlanJsonStr);
            newJson["SplitOrderPlanDetail"] = eval("[" + planDetailJson1 + "]");
            var newJsonStr = JSON.stringify(newJson);


            $.ajax({
                type: "post",
                url: "../Handler/SplitOrder.ashx?type=getShops",
                data: { jsonStr: escape(newJsonStr) },
                success: function (data) {

                    if (data != "") {

                        var json1 = eval(data);
                        for (var i = 0; i < json1.length; i++) {
                            var tr = "<tr class='tr_bai'>";
                            tr += "<td>" + (i + 1) + "</td>";
                            tr += "<td>" + json1[i].OrderType + "</td>";
                            tr += "<td><span data-shopid='" + json1[i].ShopId + "' data-orderid='" + json1[i].OrderId + "' data-newchooseimg='" + json1[i].NewChooseImg + "' data-machineframe='" + json1[i].MachineFrame + "'>" + json1[i].ShopNo + "</span></td>";
                            tr += "<td>" + json1[i].ShopName + "</td>";
                            tr += "<td>" + json1[i].Sheet + "</td>";
                            tr += "<td>" + json1[i].Gender + "</td>";
                            tr += "<td><input type='text' name='txtPerShopQuantity' value='" + json1[i].Quantity + "' style='width:90%;text-align: center;'/></td>";
                            tr += "<td><input type='text' name='txtPerShopGraphicMaterial' value='" + json1[i].GraphicMaterial + "' style='width:90%;text-align: center;'/></td>";
                            tr += "<td><input type='text' name='txtPerShopGraphicWidth' value='" + json1[i].GraphicWidth + "' style='width:90%;text-align: center;'/></td>";
                            tr += "<td><input type='text' name='txtPerShopGraphicLength' value='" + json1[i].GraphicLength + "' style='width:90%;text-align: center;'/></td>";
                            tr += "<td>" + json1[i].Remark + "</td>";
                            tr += "</tr>";
                            $("#TablePerShop").append(tr);
                        }

                        $("#editPerShopDiv").show().dialog({
                            modal: true,
                            width: '90%',
                            height: 500,
                            iconCls: 'icon-add',
                            resizable: false,
                            buttons: [
                                            {
                                                text: '提 交',
                                                iconCls: 'icon-ok',
                                                handler: function () {
                                                    var perShopJsonStr = "";
                                                    $("#TablePerShop tr").each(function () {
                                                        var span = $(this).find("td").eq(2).find("span");
                                                        var shopId = $(span).data("shopid");
                                                        var orderId = $(span).data("orderid");
                                                        var NewChooseImg = $(span).data("newchooseimg");
                                                        var MachineFrame = $(span).data("machineframe");

                                                        //subjectId
                                                        var orderType = $(this).find("td").eq(1).html();
                                                        var sheet = $(this).find("td").eq(4).html();
                                                        var gender = $(this).find("td").eq(5).html();
                                                        var quantity = $(this).find("input[name='txtPerShopQuantity']").val() || 0;
                                                        var graphicMaterial = $(this).find("input[name='txtPerShopGraphicMaterial']").val();
                                                        var width = $(this).find("input[name='txtPerShopGraphicWidth']").val() || 0;
                                                        var length = $(this).find("input[name='txtPerShopGraphicLength']").val() || 0;
                                                        var remark = $(this).find("td").eq(10).html();
                                                        orderType = orderType == "pop" ? "1" : "2";
                                                        if (parseInt(width) > 0 && parseInt(length) > 0)
                                                            perShopJsonStr += '{"OrderType":"' + orderType + '","ShopId":"' + shopId + '","SubjectId":"' + subjectId + '","Sheet":"' + sheet + '","GraphicWidth":"' + width + '","GraphicLength":"' + length + '","Gender":"' + gender + '","Quantity":"' + quantity + '","Remark":"' + remark + '","GraphicMaterial":"' + graphicMaterial + '","OrderId":"' + orderId + '","NewChooseImg":"' + NewChooseImg + '","MachineFrame":"' + MachineFrame + '"},';
                                                    })
                                                    if (perShopJsonStr.length > 0) {
                                                        perShopJsonStr = perShopJsonStr.substring(0, perShopJsonStr.length - 1);
                                                        perShopJsonStr = "[" + perShopJsonStr + "]";
                                                        $.ajax({
                                                            type: "post",
                                                            url: "../Handler/SplitOrder.ashx?type=addPerShop",
                                                            data: { jsonStr: escape(perShopJsonStr) },
                                                            success: function (data) {
                                                                if (data == "ok") {
                                                                    alert("提交成功");
                                                                    //$("#TablePerShop").html("");
                                                                    //$("#editPerShopDiv").dialog('close');
                                                                }
                                                                else {
                                                                    alert(data);
                                                                }
                                                            }
                                                        })
                                                    }
                                                    else {
                                                        alert("请填写POP尺寸");
                                                    }


                                                }
                                            },
                                            {
                                                text: '取 消',
                                                iconCls: 'icon-cancel',
                                                handler: function () {
                                                    $("#TablePerShop").html("");
                                                    $("#editPerShopDiv").dialog('close');
                                                }
                                            }
                                        ],
                            onClose: function () {

                                $("#TablePerShop").html("");
                            }
                        });
                    }
                }
            })
        }


    })

    //添加HC主KVPOP
    $("#btnAddHCPOP").on("click", function () {
        PlanJsonStr = "";
        if (CheckSubmitVal()) {
            $.ajax({
                type: "post",
                url: "../Handler/SplitOrder.ashx?type=addHCPOP",
                data: { jsonStr: escape(PlanJsonStr) },
                success: function (data) {

                    if (data != "") {
                        var json = eval(data);
                        for (var i = 0; i < json.length; i++) {

                            var tr1 = "<tr class='tr_bai content'>";
                            tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='" + json[i].POPType + "' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='hidden' name='hfMaterialId1' value='' /><input type='text' name='txtMaterial1' maxlength='100' value=''  style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtSupplier1' value='' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value=''  style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='" + json[i].POPGender + "' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='text' name='txtRemark1' data-hcpop='1' maxlength='50' value='" + json[i].POP + "' style='width: 90%; text-align: center;' /></td>";
                            tr1 += "<td><input type='checkbox' name='cbInSet1'/></td>";
                            tr1 += "<td><input type='checkbox' name='cbPerShop1'/><span name='spanPerShop' style='color:blue;cursor:pointer;display:none;'>设置</span></td>";
                            tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
                            tr1 += "</tr>";


                            $("#addContent").append(tr1);
                        }


                    }
                    else {
                        alert("没有数据可添加");
                    }
                }
            });
        }
    })


    $("#POPSizeCBAll").on("click", function () {
        var checked = this.checked;
        $(this).parent().next("div").find("input").each(function () {
            $(this).attr("checked", checked);
        })
    })


    $("#WindowSizCBAll").on("click", function () {
        var checked = this.checked;
        $(this).parent().next("div").find("input").each(function () {
            $(this).attr("checked", checked);
        })
    })

    //取消器架选择
    $("#MFCancelSelect").on("click", function () {
        $(this).parent().next("div").find("input").each(function () {
            $(this).attr("checked", false);
        })
    })
    //
})

//点击材质类型，筛选材质
//function getMaterial(obj) {
//    materialBigTypeId = $(obj).data("typeid") || 0;
//    
//    //获取客户材质
//    BindCustomerMaterial();
//}

function GetConditions(row) {
    var sheet = $("input:radio[name='SheetDivrd']:checked").val() || "0";
    var ShopNos = $.trim($("#txtShopNos").val());
    //获取条件
    $.ajax({
        type: "get",
        url: "../Handler/SplitOrder.ashx?type=getCondition&subjectIds=" + subjectId + "&positionName=" + escape(sheet) + "&shopNo=" + ShopNos,
        cache: false,
        success: function (data) {
            if (data != "") {

                var json = eval(data);
                if (json.length > 0) {
                    if (sheet == "0") {
                        $("#SheetDiv").html(InitCondition("SheetDiv", json[0].Sheet));
                    }
                    $("#CityTierDiv").html(InitCondition("CityTierDiv", json[0].CityTier));
                    $("#FormatDiv").html(InitCondition("FormatDiv", json[0].Format));
                    $("#MaterialSupportDiv").html(InitCondition("MaterialSupportDiv", json[0].MaterialSupport));
                    $("#IsInstallDiv").html(InitCondition("IsInstallDiv", json[0].IsInstall));
                    $("#POSScaleDiv").html(InitCondition("POSScaleDiv", json[0].POSScale));
                    $("#GenderDiv").html(InitCondition("GenderDiv", json[0].Gender));
                    $("#QuantityDiv").html(InitCondition("QuantityDiv", json[0].Quantity));
                    $("#GraphicMaterialDiv").html(InitCondition("GraphicMaterialDiv", json[0].GraphicMaterial));
                    $("#POPSizeDiv").html(InitCondition("POPSizeDiv", json[0].POPSize));
                    $("#WindowSizeDiv").html(InitCondition("WindowSizeDiv", json[0].WindowSize));
                    $("#ChooseImgDiv").html(InitCondition("ChooseImgDiv", json[0].ChooseImg));
                    $("#IsElectricityDiv").html(InitCondition("IsElectricityDiv", json[0].IsElectricity));
                }
                LoadConditions(row);
            }
        }

    })
}


function MaterialDiv() {
    $("#addContent").find(".divMaterialList").html($("#container").find(".divMaterialList").html());

}


function GetRegoin() {

    $.ajax({
        type: "get",
        url: "../Handler/Orders.ashx",
        data: { type: "getRegion", subjectIds: subjectId },
        cache: false,
        success: function (data) {
            $("#RegionDiv").html("");

            if (data != "") {

                var json = eval(data);
                for (var i = 0; i < json.length; i++) {
                    var div = "<input type='checkbox' name='RegionNameDivcb' value='" + json[i].Region + "' /><span>" + json[i].Region + "</span>&nbsp;";
                    $("#RegionNameDiv").append(div);
                    var option = "<option value='" + json[i].Region + "'>" + json[i].Region + "</option>";
                    $("#selRegion").append(option);
                }
                //获取客户材质大类
                //GetMaterialBigType();
                //获取客户材质
                BindCustomerMaterial();
            }
        }

    })
}

function ChangeRegion() {

    GetProvince();
    $("#CityNameDiv").html("");
    $("#cityAllDiv").hide();
    $("#cityCBAll").attr("checked", false);
    selectedCity = "";
    selectedCityNames = "";
}

function GetProvince() {
    var regions = "";
    $("#provinceCBAll").attr("checked", false);
    $("#RegionNameDiv input[name='RegionNameDivcb']:checked").each(function () {
        regions += $(this).val() + ",";
    })
    if (regions != "") {
        regions = regions.substring(0, regions.length - 1);
    }
    var positionName = $("input:radio[name='SheetDivrd']:checked").val() || "0";
    //alert(positionName);
    $.ajax({
        type: "get",
        url: "../Handler/Orders.ashx",
        data: { type: "getProvince", region: regions, subjectIds: subjectId, sheet: positionName },
        cache: false,
        success: function (data) {
            $("#ProvinceNameDiv").html("");
            if (data != "") {

                $("#provinceAllDiv").show();
                var json = eval(data);
                var div = "";
                var arr = [];
                if (loadselectedProvince != "") {
                    arr = loadselectedProvince.split(',');
                }
                for (var i = 0; i < json.length; i++) {
                    var select = "";
                    if (arr.length > 0) {
                        for (var a = 0; a < arr.length; a++) {
                            if (arr[a] == json[i].Province) {
                                select = "checked='checked'";
                            }
                        }
                    }
                    div += "<div style='width:100px;float:left;'>";
                    div += "<input type='checkbox' name='ProvinceNameDivcb' value='" + json[i].Province + "' " + select + "/><span>" + json[i].Province + "</span>&nbsp;";
                    div += "</div>";

                }
                $("#ProvinceNameDiv").html(div);
                ChangeProvince();
            }
            else {
                $("#provinceAllDiv").hide();

            }
        }

    })
}

function ChangeProvince() {
    GetCity();
    var cbcount = $("#ProvinceNameDiv input[name='ProvinceNameDivcb']").length;
    var checkedcount = $("#ProvinceNameDiv input[name='ProvinceNameDivcb']:checked").length;
    if (cbcount == checkedcount) {
        $("#provinceCBAll").attr("checked", "checked");
    }
    else
        $("#provinceCBAll").attr("checked", false);
}

function GetCity() {
    var provinces = "";
    $("#cityCBAll").attr("checked", false);
    selectedCity = "";
    selectedCityNames = "";
    $("#ProvinceNameDiv input[name='ProvinceNameDivcb']:checked").each(function () {
        provinces += $(this).val() + ",";
    })
    if (provinces != "") {
        provinces = provinces.substring(0, provinces.length - 1);
    }
    var positionName = $("input:radio[name='SheetDivrd']:checked").val() || "0";
    $.ajax({
        type: "get",
        url: "../Handler/Orders.ashx",
        data: { type: "getCity", province: provinces, subjectIds: subjectId, sheet: positionName },
        cache: false,
        success: function (data) {
            $("#CityDiv").html("");
            if (data != "") {

                $("#cityAllDiv").show();
                var json = eval(data);

                var count = json.length;
                //count = count > 50 ? 50 : count;
                var arr = [];

                if (loadselectedCity != "") {
                    arr = loadselectedCity.split(',');

                }
                var pName = "";
                var table = "<table>";
                for (var i = 0; i < count; i++) {
                    var check = "";
                    if (arr.length > 0) {
                        for (a = 0; a < arr.length; a++) {
                            if (arr[a] == json[i].City)
                                check = "checked='checked'";
                        }
                    }
                    var div = "";
                    div += "<div style='float:left;'>";
                    div += "<input type='checkbox' name='CityNameDivcb' value='" + json[i].City + "' " + check + "/><span>" + json[i].City + "</span>&nbsp;";
                    div += "</div>";
                    if (pName != json[i].Province) {
                        if (i > 0)
                            table += "</td></tr>";
                        pName = json[i].Province;
                        table += "<tr>";
                        table += "<td style='width:80px;border-right:1px solid #ccc; vertical-align:top; padding-top:5px;'><input type='checkbox' name='CPDivcb'/>" + pName + "</td><td style='vertical-align:top; padding-top:5px;'>";
                    }
                    table += div;
                    if (i == count - 1)
                        table += "</td></tr>";


                }
                table += "</table>";

                //if (json.length > 50) {
                //cityJson = json;
                //div += "<div style='float:left;'>";
                //div += "<span id='showMoreCitySpan' onclick='ShowMoreCity()' style='text-decoration:underline;cursor:pointer;color:blue;'>更多...</span>";
                //div += "</div>";
                //}
                $("#CityNameDiv").html(table);
                if (loadselectedCity == "-1") {
                    $("#cityCBAll").attr("checked", "checked");
                }
            }
            else {
                $("#cityAllDiv").hide();
                $("#CityNameDiv").html("");
                $("#cityCBAll").attr("checked", false);
            }
        }

    })
}


function ChangeMoreCitySelect() {
    var arr = [];
    if (loadselectedCity != "") {
        arr = loadselectedCity.split(',');

    }
    var cbcount = $("#showCityListDiv input[name='moreCityCB']").each(function () {
        var cid = $(this).val();
        var checked = this.checked;

        $("#CityNameDiv input[name='CityNameDivcb']").each(function () {
            if ($(this).val() == cid) {
                this.checked = checked;
            }
            for (var a = 0; a < arr.length; a++) {
                if (parseInt(arr[a]) == parseInt($(this).val()))
                    this.checked = checked;
            }
        })
    }).length;
    var checkedcount = $("#showCityListDiv input[name='moreCityCB']:checked").length;
    if (cbcount == checkedcount) {

        $("#cityCBAll,#moreCityCBAll").attr("checked", "checked");

    }
    else {
        $("#cityCBAll,#moreCityCBAll").attr("checked", false);
    }



}

//弹出更多城市

function ShowMoreCity() {

    if (cityJson != null && cityJson.length > 0) {
        var div = "";
        var checked = "";
        var isCheckAll = 0;
        var arr = [];
        if ($("#cityCBAll").attr("checked") == "checked") {
            checked = "checked='checked'";
            isCheckAll = 1;
            $("#moreCityCBAll").attr("checked", "checked");
        }
        else {
            if (selectedCity.length > 0) {
                selectedCity = selectedCity.substring(0, selectedCity.length - 1);
                arr = selectedCity.split(',');
            }
        }
        for (var i = 0; i < cityJson.length; i++) {
            var check1 = checked;
            if (isCheckAll == 0) {
                if (arr.length > 0) {
                    for (var j = 0; j < arr.length; j++) {
                        if (parseInt(cityJson[i].CityId) == parseInt(arr[j])) {
                            check1 = "checked='checked'";
                        }
                    }
                }
                else {
                    $("#CityNameDiv input[name='CityNameDivcb']:checked").each(function () {
                        if (parseInt(cityJson[i].CityId) == parseInt($(this).val())) {
                            check1 = "checked='checked'";
                        }
                    })
                }
            }

            div += "<div style='float:left; line-height:25px;'>";
            div += "<input type='checkbox' name='moreCityCB' value='" + cityJson[i].CityId + "' " + check1 + "/><span>" + cityJson[i].CityName + "</span>&nbsp;";
            div += "</div>";
        }
        $("#showCityListDiv").html(div);
    }
    else {
        $("#showCityListDiv").html("");
    }
    $("#showCityDiv").show().dialog({
        modal: true,
        width: 650,
        height: 400,
        iconCls: 'icon-search',
        resizable: false,
        buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            $("#showCityListDiv").find("input[name='moreCityCB']:checked").each(function () {
                                selectedCity += $(this).val() + ",";
                                selectedCityNames += $(this).next().html() + ",";
                                $("#showCityDiv").dialog('close');
                            })

                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#showCityDiv").dialog('close');
                        }
                    }
                ]
    });
}

//初始化条件值
function InitCondition(element, valStr) {
    var input = "";
    if ($.trim(valStr) != "") {
        var arr = valStr.split(',');
        if (arr.length > 0) {


            if (element == "SheetDiv") {

                for (var i = 0; i < arr.length; i++) {
                    input += "<input type='radio' name='SheetDivrd' value='" + arr[i] + "'/><span>" + arr[i] + "</span>&nbsp;";
                }
                $("#SheetLoading").hide();
            }
            else {
                var cbName = element + "cb";
                var checkedVal = "";
                $("input[name='" + cbName + "']:checked").each(function () {
                    checkedVal += $(this).val() + ",";
                })
                var selectedarr = [];
                if (checkedVal.length > 0) {
                    selectedarr = checkedVal.substring(0, checkedVal.length - 1).split(',');
                }
                var flag = false;
                for (var i = 0; i < arr.length; i++) {
                    var checked = "";
                    if (selectedarr.length > 0) {
                        for (var j = 0; j < selectedarr.length; j++) {
                            if (selectedarr[j] == arr[i])
                                checked = "checked='checked'";
                            flag = true;
                        }
                    }
                    input += "<div style='float:left;'>";
                    input += "<input type='checkbox' name='" + cbName + "' value='" + arr[i] + "' " + checked + "/>" + arr[i] + "&nbsp;";
                    input += "</div>";
                }
                if (flag) {
                    $("input[name='" + cbName + "']").change();
                }
            }

        }
    }
    return input;
}

//获取角落类型
function GetCornerType(corners) {
    var ShopNos = $.trim($("#txtShopNos").val());
    var gender = "";
    $("#GenderDiv").find("input[name='GenderDivcb']:checked").each(function () {
        gender += $(this).val() + ",";

    })
    $("#CornerTypeDiv").html("");
    var positionName = $("input:radio[name='SheetDivrd']:checked").val() || "0";
    var cornersArr = [];
    var checkedVal = "";
    if (corners.length > 0) {
        cornersArr = corners.split(',');

    }
    else {
        $("input[name='CornerTypeDivcb']:checked").each(function () {
            checkedVal += $(this).val() + ",";
        })
    }
   
    var selectedarr = [];
    if (checkedVal.length > 0) {
        selectedarr = checkedVal.substring(0, checkedVal.length - 1).split(',');
    }

    $.ajax({
        type: "post",
        //url: "../Handler/SplitOrder.ashx?type=getCornerType&positionName=" + escape(positionName) + "&subjectIds=" + subjectId + "&shopNo=" + ShopNos + "&gender=" + gender,
        url: "../Handler/SplitOrder.ashx",
        data: { type: "getCornerType", positionName: positionName, subjectIds: subjectId, shopNo: ShopNos, gender: gender },
        cache: false,
        success: function (data) {

            if (data != "") {
                var json = eval(data);
                if (json.length > 0) {
                    var input = "";
                    var flag = false;
                    for (var i = 0; i < json.length; i++) {
                        var check = "";
                        for (var j = 0; j < cornersArr.length; j++) {

                            if (json[i].CornerType == cornersArr[j]) {
                                check = "checked='checked'";
                                flag = true;
                            }
                        }

                        input += "<div style='float:left;'>";
                        input += "<input type='checkbox' name='CornerTypeDivcb' value='" + json[i].CornerType + "' " + check + "/><span>" + json[i].CornerType + "</span>&nbsp;";
                        input += "</div>";
                    }
                    $("#CornerTypeDiv").prev("div").hide();
                    $("#CornerTypeDiv").html(input);
                    if (flag) {

                        $("input[name='CornerTypeDivcb']").change();
                    }
                }

            }
            else {
                $("#CornerTypeDiv").prev("div").hide();
            }
        }
    })
}


//获取器架类型
var currentMachineFrames = "";
function GetMachineFrame() {
    var ShopNos = $.trim($("#txtShopNos").val());
    var gender = "";
    $("#GenderDiv").find("input[name='GenderDivcb']:checked").each(function () {
        gender += $(this).val() + ",";

    })
    $("#MachineFrameDiv").html("");
    var positionName = $("input:radio[name='SheetDivrd']:checked").val() || "0";
    var cornerType = "";
    $("#CornerTypeDiv").find("input[name='CornerTypeDivcb']:checked").each(function () {
        cornerType += $(this).val() + ",";

    })
    if (cornerType.length > 0)
        cornerType = cornerType.substring(0, cornerType.length - 1);
    var MachineFrameIdsArr = [];
    if (currentMachineFrames.length > 0) {
        MachineFrameIdsArr = currentMachineFrames.split(',');
        //currentMachineFrames = "";
    }
    var checkedVal = "";
    $("input[name='MachineFrameDivcb']:checked").each(function () {
        checkedVal += $(this).val() + ",";
    })
    var selectedarr = [];
    if (checkedVal.length > 0) {
        selectedarr = checkedVal.substring(0, checkedVal.length - 1).split(',');
    }
    $.ajax({
        type: "post",
        //url: "../Handler/SplitOrder.ashx?type=getMachineFrame&positionName=" + escape(positionName) + "&subjectIds=" + subjectId + "&cornerType=" + escape(cornerType) + "&shopNo=" + ShopNos + "&gender=" + gender,
        url: "../Handler/SplitOrder.ashx",
        data: { type: "getMachineFrame", positionName: positionName, subjectIds: subjectId, cornerType: cornerType, shopNo: ShopNos, gender: gender },
        cache: false,
        success: function (data) {

            if (data != "") {
                var json = eval(data);
                if (json.length > 0) {
                    var input = "";
                    var flag = false;
                    for (var i = 0; i < json.length; i++) {
                        var check = "";
                        for (var j = 0; j < MachineFrameIdsArr.length; j++) {
                            if (json[i].FrameName == MachineFrameIdsArr[j]) {
                                check = "checked='checked'";
                                flag = true;
                            }
                        }
                        if (check == "" && selectedarr.length > 0) {
                            for (var j = 0; j < selectedarr.length; j++) {
                                if (selectedarr[j] == json[i].FrameName) {
                                    check = "checked='checked'";
                                    flag = true;
                                }
                            }
                        }
                        input += "<div style='float:left;'>";
                        input += "<input type='checkbox' name='MachineFrameDivcb' value='" + json[i].FrameName + "' " + check + "/><span>" + json[i].FrameName + "</span>&nbsp;";
                        input += "</div>";
                    }
                    $("#MachineFrameDiv").prev("div").hide();
                    $("#MachineFrameDiv").html(input);
                    if (flag) {
                        $("input[name='MachineFrameDivcb']").change();
                    }
                }
                currentMachineFrames = "";
            }
        }
    })
}
//获取客户材质大类
//function GetMaterialBigType() {
//    var region = $("#selRegion").val();
//    $.ajax({
//        type: "get",
//        url: "../Handler/SplitOrder.ashx?type=getCustomerMaterialType&region=" + region + "&customerId=" + customerId,
//        cache:false,
//        success: function (data) {



//var materialBigTypeId = 0;
//获取客户材质（报价）
function BindCustomerMaterial() {
    var region = $("#selRegion").val();
    $.ajax({
        type: "get",
        //url: "../Handler/SplitOrder.ashx?type=getCustomerMaterial&region=" + region + "&bigTypeId=" + materialBigTypeId + "&customerId=" + customerId,
        url: "../Handler/SplitOrder.ashx?type=getCustomerMaterial&region=" + region + "&customerId=" + customerId,
        cache: false,
        success: function (data) {

            if (data != "") {
                var json = eval(data);
                var div = "";
                for (var i = 0; i < json.length; i++) {
                    div += "<div style=' height:20px;'><input name='cbMaterial' type='checkbox' value='" + json[i].MaterialName + "'/>&nbsp;<span data-price='" + json[i].Price + "'>" + json[i].MaterialName + "</span></div>";
                }
                //$("#customerMaterials").html(div);
                $(".customerMaterials").html(div);
            }
            else
                $(".customerMaterials").html("");
        }
    })
}



var PlanJsonStr = "";
var CurrPlanId = 0;
var Plan = {
    add: function () {
        CheckAddVal();
        $("#container input").each(function () {
            if ($(this).attr("Id") != "btnAddPlan" && $(this).attr("name") != "cbMaterial") {
                $(this).val("");
            }
        });
    },
    submit: function (optype) {

        if (CheckSubmitVal()) {
            $("#btnSubmitPlan").parent().hide().prev("div").show();
            //$("#btnSubmitPlan").parent().hide();
            $.ajax({
                type: "post",

                url: "../Handler/SplitOrder.ashx?type=add&optype=" + optype,
                data: { jsonStr: escape(PlanJsonStr) },
                cache: false,
                success: function (data) {
                    if (data == "ok") {
                        Plan.getList();
                        ClearConditionVal();
                        ClearVal();
                        $("#btnSubmitPlan").parent().show().prev("div").hide();
                        //$("#btnSubmitPlan").parent().prev("div").hide();
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
            queryParams: { type: 'getList', CustomerId: customerId, SubjectId: subjectId, planType: 1, t: Math.random() * 1000 },
            method: 'get',
            url: '../Handler/SplitOrder.ashx',
            cache: false,
            columns: [[
                            { field: 'Id', hidden: true },

                            { field: 'RegionNames', title: '区域' },

                            {
                                field: 'ProvinceName1', title: '省份', formatter: function (value, row, index) {
                                    var provinceNames = row.ProvinceName;

                                    if (provinceNames.length > 30) {
                                        provinceNames = provinceNames.substring(0, 30) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                                    }
                                    return '<span>' + provinceNames + '</span>';
                                }
                            },
                            {
                                field: 'CityName1', title: '城市', formatter: function (value, row, index) {
                                    var cityNames = row.CityName;
                                    if (cityNames.length > 50) {
                                        cityNames = cityNames.substring(0, 30) + "...<span style='color:blue;cursor:pointer;'>更多</span>";
                                    }
                                    return '<span>' + cityNames + '</span>';
                                }
                            },
                            { field: 'CityTier', title: '城市级别' },
                            { field: 'Gender', title: '性别' },
                            { field: 'PositionName', title: 'POP位置', sortable: true },
                            { field: 'CornerType', title: '角落类型', sortable: true },
                            { field: 'MachineFrameNames', title: '器架类型', sortable: true },
                            
                            { field: 'IsInstall', title: '是否安装' },
                            { field: 'Format', title: '店铺类型' },
                            { field: 'ShopNos1', title: '店铺编号',
                                formatter: function (value, rec) {
                                    var val = rec.ShopNos;
                                    val = val.length > 15 ? (val.substring(0, 15) + "...") : val;

                                    return val;
                                }

                            },
                            { field: 'MaterialSupport', title: '物料支持类别' },
                            { field: 'POSScale', title: '店铺规模' },
                            { field: 'Quantity', title: '数量' },
                            { field: 'GraphicNo', title: 'POP编号' },
                            { field: 'GraphicMaterial', title: 'POP材质' },
                            { field: 'POPSize', title: 'POP尺寸' },
                            { field: 'WindowSize', title: 'Window尺寸' },
                            { field: 'IsElectricity', title: '通电否' },
                            { field: 'ChooseImg', title: '选图' },
                            { field: 'KeepPOPSize', title: '是否保留POP原尺寸' }
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
function commonExpandRow(index, row, target, nextName) {

    var curName = nextName + '_' + index;
    var id = row.Id;
    $('#' + curName).datagrid({
        method: 'get',
        url: '../Handler/SplitOrder.ashx',
        cache: false,
        queryParams: { planId: id, type: "getDetail", t: Math.random() * 1000 },
        fitColumns: false,
        height: 'auto',
        pagination: false,
        //singleSelect: true,

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
                    { field: 'IsPerShop', title: '单店设置' }
            ]],
        frozenColumns: [[
                    {
                        field: 'edit', title: '删除', align: 'center',
                        formatter: function (value, rec) {
                            var btn = '<a href="javascript:void(0)" onclick="deleteDetail(' + rec.Id + ')">删除</a>';
                            return btn;
                        }
                    }
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




function CheckSubmitVal() {
    PlanJsonStr = "";
    var canSubmit = true;
    //var regionIds = "";
    var region = "";
    $("#RegionNameDiv").find("input[name='RegionNameDivcb']:checked").each(function () {
        region += $(this).val() + ",";
        //regionNames += $(this).next().html() + ",";

    })
    if (region == "") {
        alert("请选择区域");
        canSubmit = false;
    }

    var province = ""
    $("#ProvinceNameDiv").find("input[name='ProvinceNameDivcb']:checked").each(function () {
        province += $(this).val() + ",";

    })
    var city = selectedCity;

    if (city == "") {
        $("#CityNameDiv").find("input[name='CityNameDivcb']:checked").each(function () {
            city += $(this).val() + ",";

        })
    }

    var cityTier = "";
    $("#CityTierDiv").find("input[name='CityTierDivcb']:checked").each(function () {
        cityTier += $(this).val() + ",";

    })

    var ShopNos = $.trim($("#txtShopNos").val());
    var IsExcept = 0;
    if ($("#cbExcept").attr("checked") == "checked") {
        IsExcept = 1;
    }
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

    var PositionName = $("input:radio[name='SheetDivrd']:checked").val() || "";

    if (PositionName == "") {
        alert("请选择pop位置");
        canSubmit = false;
    }

    var CornerTypes = "";
    $("#CornerTypeDiv").find("input[name='CornerTypeDivcb']:checked").each(function () {
        CornerTypes += $(this).val() + ",";

    })

    var MachineFrameNames = "";
    $("#MachineFrameDiv").find("input[name='MachineFrameDivcb']:checked").each(function () {
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

    if (addcontent.length == 0) {
        //alert("请先添加方案内容");
        //return false;
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
        var num = $(td).find("input[name='txtQuantity1']").val()||0;
        var newGender = $(td).find("input[name='txtNewGender1']").val()||"";
        var newChooseImg = $(td).find("input[name='txtChooseImg1']").val();
        var remark = $(td).find("input[name='txtRemark1']").val();
        var isHCPOP = $(td).find("input[name='txtRemark1']").data("hcpop") || 0;
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
        planDetailJson += '{"OrderType":' + planType + ',"GraphicWidth":' + width + ',"GraphicLength":' + length + ',"Quantity":' + num + ',"CustomerMaterialId":' + materialId + ',"GraphicMaterial":"' + material + '","Supplier":"' + Supplier + '","RackSalePrice":' + RackPrice + ',"Remark":"' + remark + '","NewChooseImg":"' + newChooseImg + '","IsInSet":' + isInSet + ',"IsPerShop":' + isPerShop + ',"NewGender":"' + newGender + '","IsHCPOP":'+isHCPOP+'},';
    }
    
    if (planDetailJson.length > 0) {
        planDetailJson = planDetailJson.substring(0, planDetailJson.length - 1);

    }
    PlanJsonStr = '{"Id":' + CurrPlanId + ',"CustomerId":' + customerId + ',"SubjectId":' + subjectId + ',"ProvinceId":"' + province + '","CityId":"' + city + '","RegionNames":"' + region + '","CityTier":"' + cityTier + '","ShopNos":"' + ShopNos + '","IsExcept":' + IsExcept + ',"IsInstall":"' + Install + '","Format":"' + Format + '","MaterialSupport":"' + MaterialSupport + '","POSScale":"' + Scale + '","PositionName":"' + PositionName + '","CornerType":"' + CornerTypes + '","MachineFrameNames":"' + MachineFrameNames + '","Quantity":"' + Quantity + '","Gender":"' + Gender + '",';
    //PlanJsonStr += '"GraphicNo":"' + GraphicNo + '","GraphicMaterial":"' + GraphicMaterial + '","GraphicWidth":"' + GraphicWidth + '","GraphicLength":"' + GraphicLength + '","WindowWidth":"' + WindowWide + '","WindowHigh":"' + WindowHigh + '","WindowDeep":"' + WindowDeep + '","ChooseImg":"' + ChooseImg + '","KeepPOPSize":"' + KeepPOPSize + '"';
    PlanJsonStr += '"GraphicNo":"' + GraphicNo + '","GraphicMaterial":"' + GraphicMaterial + '","POPSize":"' + POPSize + '","WindowSize":"' + WindowSize + '","ChooseImg":"' + ChooseImg + '","KeepPOPSize":"' + KeepPOPSize + '","IsElectricity":"' + IsElectricity + '"';


    PlanJsonStr += ',"SplitOrderPlanDetail":[' + planDetailJson + ']}';
    PlanJsonStr = PlanJsonStr.replace(/×/g, "乘");
    //return false;
    return canSubmit;
}

function CheckAddVal() {

    var planTypeId = $("#selPlanType").val();
    var planTypeName1 = $("#selPlanType option:selected").text();
    var width1 = $("#txtWidth").val();
    //    if ($.trim(width1) == "") {
    //        alert("请填写宽");
    //        return false;
    //    }
    var length1 = $("#txtLength").val();
    //    if ($.trim(length1) == "") {
    //        alert("请填写高");
    //        return false;
    //    }
    var material1 = $("#container").find("input[name='txtMaterial']").val();
    var materialId1 = $("#container").find("input[name='hfMaterialId']").val();
    //    if ($.trim(material1) == "") {
    //        alert("请填写材质");
    //        return false;
    //    }
    var Supplier1 = $("#txtSupplier").val();
    var RackPrice1 = $("#txtRackPrice").val();
    //    if (planTypeId == "2" && $.trim(RackPrice1) == "") {
    //        alert("请填写道具销售价");
    //        return false;
    //    }
    //    if (planTypeId == "1")
    //        RackPrice1 = 0;
    var num1 = $("#txtNum").val();
    if ($.trim(num1) == "") {
        alert("请填写数量");
        return false;
    }
    var chooseImg1 = $("#txtChooseImg").val();


    var remark1 = $("#txtRemark").val();

    var isInSet = "";
    var isPerShop = "";
    if ($("#cbInSet").attr("checked") == "checked") {
        isInSet = "checked='checked'";
    }
    if ($("#cbPerShop").attr("checked") == "checked") {
        isPerShop = "checked='checked'";
    }

    //    var tr1 = "<tr class='tr_bai'>";
    //    tr1 += "<td>" + planTypeName1 + "</td>";
    //    tr1 += "<td>" + width1 + "</td>";
    //    tr1 += "<td>" + length1 + "</td>";
    //    tr1 += "<td><span name='spanMaterialSele' data-materialid='" + materialId1 + "'>" + material1 + "</span></td>";
    //   
    //    tr1 += "<td>" + Supplier1 + "</td>";
    //    tr1 += "<td>" + RackPrice1 + "</td>";
    //    tr1 += "<td>" + num1 + "</td>";
    //    tr1 += "<td>" + remark1 + "</td>";
    //    tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
    //    tr1 += "</tr>";

    var tr1 = "<tr class='tr_bai content'>";

    tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='" + planTypeName1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='" + width1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='" + length1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='hidden' name='hfMaterialId1' value='" + materialId1 + "' /><input type='text' name='txtMaterial1' maxlength='100' value='" + material1 + "'  style='width: 90%; text-align: center;' /></td>";

    tr1 += "<td><input type='text' name='txtSupplier1' value='" + Supplier1 + "' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value='" + RackPrice1 + "'  style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='" + num1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='" + chooseImg1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='text' name='txtRemark1' maxlength='50' value='" + remark1 + "' style='width: 90%; text-align: center;' /></td>";
    tr1 += "<td><input type='checkbox' name='cbInSet1' " + isInSet + "/></td>";
    tr1 += "<td><input type='checkbox' name='cbPerShop1' " + isPerShop + "/><span name='spanPerShop' style='color:blue;cursor:pointer;display:none;'>设置</span></td>";
    tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
    tr1 += "</tr>";



    $("#addContent").append(tr1);


}

function ClearVal() {
    PlanJsonStr = "";
    $("#addContent").html("");
    $("#container input").each(function () {
        if ($(this).attr("Id") != "btnAddPlan" && $(this).attr("name") != "cbMaterial") {
            $(this).val("");
        }
    });
}

function deleteDetail(detailId) {
    $.ajax({
        type: "get",
        url: "../Handler/SplitOrder.ashx?type=deleteDetail&detailId=" + detailId,
        cache: false,
        success: function (data) {
            if (data == "ok") {
                Plan.refresh();
            }
            else {
                alert("删除失败：" + data);
            }
        }
    })
}

function LoadCondition(row) {
    ClearConditionVal();
    if (row != null) {

        CurrPlanId = row.Id;
        loadselectedCity = row.CityName || "";
        loadselectedProvince = row.ProvinceName || "";

        var PositionId = row.PositionName || "";
        $("input:radio[name='SheetDivrd']").each(function () {
            if ($(this).val() == PositionId) {
                $(this).attr("checked", "checked");
            }

        })


        var Region = row.RegionNames || "";
        if (Region.length > 0) {
            var RegionIdArr = Region.split(',');
            for (var i = 0; i < RegionIdArr.length; i++) {
                $("input[name='RegionNameDivcb']").each(function () {
                    if ($(this).val() == RegionIdArr[i]) {
                        $(this).attr("checked", "checked");

                    }

                })
            }
            ChangeRegion();
        }
        var corners = row.CornerType || "";
        currentMachineFrames = row.MachineFrameNames || "";
        if (corners != "") {
            GetCornerType(corners);
        }
        else
            GetMachineFrame();
        
        GetConditions(row);



        var KeepPOPSize = row.KeepPOPSize;
        if (KeepPOPSize == "是")
            $("#cbKeepSize").attr("checked", "checked");

        LoadPlanDetail();
    }
}

function LoadConditions(row) {
    if (row != null) {
       

        var cityTier = row.CityTier || "";
        if (cityTier.length > 0) {
            var cityTierArr = cityTier.split(',');
            for (var i = 0; i < cityTierArr.length; i++) {
                $("input[name='CityTierDivcb']").each(function () {
                    if ($(this).val() == cityTierArr[i]) {
                        $(this).attr("checked", "checked");

                    }

                })
            }
        }


        $("#txtShopNos").val(row.ShopNos);
        if (row.IsExcept == 1) {
            $("#cbExcept").attr("checked", "checked");
        }
        $("#txtDivShopNos").val(row.ShopNos);
        var format = row.Format || "";

        if (format.length > 0) {
            var formatArr = format.split(',');
            for (var i = 0; i < formatArr.length; i++) {
                $("input[name='FormatDivcb']").each(function () {
                    if ($(this).val() == formatArr[i]) {
                        $(this).attr("checked", "checked");

                    }

                })
            }
        }

        var MaterialSupport = row.MaterialSupport || "";
        if (MaterialSupport.length > 0) {
            var MaterialSupportArr = MaterialSupport.split(',');
            for (var i = 0; i < MaterialSupportArr.length; i++) {
                $("input[name='MaterialSupportDivcb']").each(function () {
                    if ($(this).val() == MaterialSupportArr[i]) {
                        $(this).attr("checked", "checked");

                    }

                })
            }
            //$("input[name='MaterialSupportDivcb']").change();
        }

        var POSScale = row.POSScale || "";
        
        if (POSScale.length > 0) {
            var POSScaleArr = POSScale.split(',');
            
            for (var i = 0; i < POSScale.length; i++) {
                $("input[name='POSScaleDivcb']").each(function () {
                   
                    if ($(this).val() == POSScaleArr[i]) {
                        $(this).attr("checked", "checked");
                        //$("input[name='ScaleDivcb']").change();
                    }

                })
            }
        }


        var IsInstall = row.IsInstall || "";
        if (IsInstall.length > 0) {
            var IsInstallArr = IsInstall.split(',');
            for (var i = 0; i < IsInstallArr.length; i++) {
                $("input[name='InstallDivcb']").each(function () {
                    if ($(this).val() == IsInstallArr[i]) {
                        $(this).attr("checked", "checked");

                    }

                })
            }
        }



       
        
        //GetMachineFrame(MachineFrames);

        var Gender = row.Gender || "";
        if (Gender.length > 0) {
            var GenderArr = Gender.split(',');
            for (var i = 0; i < GenderArr.length; i++) {
                $("input[name='GenderDivcb']").each(function () {
                    if ($(this).val() == GenderArr[i]) {
                        $(this).attr("checked", "checked");

                    }

                })
            }
        }

        var Quantity = row.Quantity || "";
        if (Quantity.length > 0) {
            var QuantityArr = Quantity.split(',');
            for (var i = 0; i < QuantityArr.length; i++) {
                $("input[name='QuantityDivcb']").each(function () {
                    if ($(this).val() == QuantityArr[i]) {
                        $(this).attr("checked", "checked");

                    }

                })
            }
        }

        var GraphicMaterial = row.GraphicMaterial || "";
        if (GraphicMaterial.length > 0) {
            var GraphicMaterialArr = GraphicMaterial.split(',');
            for (var i = 0; i < GraphicMaterialArr.length; i++) {
                $("input[name='GraphicMaterialDivcb']").each(function () {
                    if ($(this).val() == GraphicMaterialArr[i]) {
                        $(this).attr("checked", "checked");

                    }

                })
            }
        }

        var POPSize = row.POPSize || "";
        if (POPSize.length > 0) {
            var popSizeArr = POPSize.split(',');
            for (var i = 0; i < popSizeArr.length; i++) {
                $("input[name='POPSizeDivcb']").each(function () {
                    if ($(this).val() == popSizeArr[i]) {
                        $(this).attr("checked", "checked");

                    }

                })
            }
        }

        var WindowSize = row.WindowSize || "";
        if (WindowSize.length > 0) {
            var WindowSizeArr = WindowSize.split(',');
            for (var i = 0; i < WindowSizeArr.length; i++) {
                $("input[name='WindowSizeDivcb']").each(function () {
                    if ($(this).val() == WindowSizeArr[i]) {
                        $(this).attr("checked", "checked");
                    }

                })
            }
        }

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
    }
}



function LoadPlanDetail() {
    $.ajax({
        type: "get",
        url: "../Handler/SplitOrder.ashx?type=getDetail&planId=" + CurrPlanId,
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
                    tr1 += "<tr class='tr_bai content'>";




                    tr1 += "<td><input type='text' name='txtOrderType1' maxlength='10' value='" + json[i].OrderType + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtWidth1' maxlength='20' value='" + json[i].GraphicWidth + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtLength1' maxlength='20' value='" + json[i].GraphicLength + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtMaterial1' maxlength='100' value='" + json[i].GraphicMaterial + "' style='width: 90%; text-align: center;' /></td>";

                    tr1 += "<td><input type='text' name='txtSupplier1' value='" + json[i].Supplier + "' maxlength='50'  style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtPrice1' maxlength='50' value='" + json[i].RackSalePrice + "'  style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtQuantity1' maxlength='10' value='" + json[i].Quantity + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtNewGender1' maxlength='10' value='" + json[i].NewGender + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtChooseImg1' maxlength='50' value='" + json[i].NewChooseImg + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='text' name='txtRemark1' " + isHCPOP + " maxlength='20' value='" + json[i].Remark + "' style='width: 90%; text-align: center;' /></td>";
                    tr1 += "<td><input type='checkbox' name='cbInSet1' " + isInSet + "/></td>";
                    tr1 += "<td><input type='checkbox' name='cbPerShop1' " + isPerShop + "/><span name='spanPerShop' " + style1 + ">设置</span></td>";
                    tr1 += "<td><span class='deletePlanDetail' style='color:red;cursor:pointer;'>删除</span></td>";
                    tr1 += "</tr>";
                }
                $("#addContent").append(tr1);
            }
        }
    })
}


function ClearConditionVal() {
    CurrPlanId = 0;
    $("#ConditionTB input[type='text']").val("");
    $("#ConditionTB input[type='checkbox']").attr("checked", false);
    $("#ConditionTB input[type='radio']").attr("checked", false);
    $("#addContent").html("");

}



//
var jsons = [];
var currControlIndex = 0; //触发“change”事件的空间在条件table的索引
var conditionControlArr = []; //有条件数据的控件名称数组
function GetSelectedConditions(cName) {
    //alert(JSON.stringify(jsons));
    var PlanJsonStr1 = "";
    var RegionName = "";
    var ProvinceName = ""
    var CityName = "";
    var CityTier = "";
    var ShopNos = $.trim($("#txtShopNos").val());
    var Format = "";
    var MaterialSupport = "";
    var IsInstall = "";
    var POSScale = "";
    var Sheet = "";
    var CornerType = "";
    var MachineFrame = "";
    var Gender = "";
    var Quantity = "";
    var GraphicMaterial = "";
    var POPSize = "";
    var WindowSize = "";
    var ChooseImg = "";
    var flag = false;
    currControlIndex = 0;
    conditionControlArr = [];
    $.each(jsons, function (key, val) {

        if (val.cName == cName) {
            currControlIndex = val.cIndex;
            flag = true;
        }
    })
    
    if (flag) {
        //把当前控件以及在前面的控件的名称保存起来
        $.each(jsons, function (key, val) {
            if (parseInt(val.cIndex) < (parseInt(currControlIndex) + 1)) {
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

    Sheet = $("input:radio[name='SheetDivrd']:checked").val() || "";
    if (RegionName.length > 0)
        RegionName = RegionName.substring(0, RegionName.length - 1);
    if (ProvinceName.length > 0)
        ProvinceName = ProvinceName.substring(0, ProvinceName.length - 1);
    if (CityName.length > 0)
        CityName = CityName.substring(0, CityName.length - 1);
    if (CityTier.length > 0)
        CityTier = CityTier.substring(0, CityTier.length - 1);
    if (Format.length > 0)
        Format = Format.substring(0, Format.length - 1);
    if (MaterialSupport.length > 0)
        MaterialSupport = MaterialSupport.substring(0, MaterialSupport.length - 1);
    if (IsInstall.length > 0)
        IsInstall = IsInstall.substring(0, IsInstall.length - 1);
    if (POSScale.length > 0)
        POSScale = POSScale.substring(0, POSScale.length - 1);
    if (CornerType.length > 0)
        CornerType = CornerType.substring(0, CornerType.length - 1);
    if (MachineFrame.length > 0)
        MachineFrame = MachineFrame.substring(0, MachineFrame.length - 1);
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
    var GraphicNo = $.trim($("#txtGraphicNo").val());
    PlanJsonStr1 = '{"CustomerId":' + customerId + ',"SubjectId":' + subjectId + ',"ProvinceId":"' + ProvinceName + '","CityId":"' + CityName + '","RegionNames":"' + RegionName + '","CityTier":"' + CityTier + '","ShopNos":"' + ShopNos + '","IsInstall":"' + IsInstall + '","Format":"' + Format + '","MaterialSupport":"' + MaterialSupport + '","POSScale":"' + POSScale + '","PositionName":"' + Sheet + '","CornerType":"' + CornerType + '","MachineFrameNames":"' + MachineFrame + '","Quantity":"' + Quantity + '","Gender":"' + Gender + '",';

    PlanJsonStr1 += '"GraphicNo":"' + GraphicNo + '","GraphicMaterial":"' + GraphicMaterial + '","POPSize":"' + POPSize + '","WindowSize":"' + WindowSize + '","ChooseImg":"' + ChooseImg + '"}';

    return PlanJsonStr1;
}


function UpdateControl() {
    var namesArr = [];
    jsons = [];
    var index = 1;
    $("#ConditionTB").find("input[type='checkbox']").each(function () {
        var name = $(this).attr("name");
        var flag = true;

        $.each(namesArr, function (key, val) {
            if (val == name)
                flag = false;
        })
        if (flag) {
            namesArr.push(name);
            jsons.push({ "cName": "" + name + "", "cIndex": "" + index + "" });
            index++;
        }
    });
   
    namesArr = null;
}



$(function () {


    $("#ConditionTB").delegate("input[type='checkbox']", "change", function () {
        if ($(this).attr("id") == "cbKeepSize" || $(this).attr("id") == "POPSizeCBAll" || $(this).attr("id") == "WindowSizCBAll") {
            return false;
        }
        if ($(this).attr("name") == "GenderDivcb") {
            $(".showLoading").show();
            GetCornerType("");
            GetMachineFrame();
        }
        
        UpdateControl();
        var name = $(this).attr("name");
        ChangeConditions(name);

    })

    $("#txtShopNos").on("blur", function () {
        var val = $(this).val();
        var name = $(this).attr("name");

        if (val != "") {

        }
        GetCornerType("");
        GetMachineFrame();
        ChangeConditions(name);
    })
})

function ChangeConditions(name) {
    var conditions = GetSelectedConditions(name);
    
    $.ajax({
        type: "post",
        url: "../Handler/SplitOrder.ashx?type=changeCondition",
        data: { jsonStr: escape(conditions) },
        cache: false,
        success: function (data) {
            
            if (data != "") {

                var jsonData = eval("(" + data + ")");
                $(".conditionDiv").each(function () {
                    var id = $(this).attr("id");

                    var field = id.replace("Div", "");
                    var canChange = true;
                    $.each(conditionControlArr, function (key, val) {

                        var agr1 = val.replace('cb', '');
                        if (agr1 == id)
                            canChange = false;
                    })
                    if (canChange) {
                        var js = '$("#' + id + '").html(InitCondition("' + id + '", jsonData[0].' + field + '));';
                        eval(js);
                    }
                })


            }
        }
    })
}

//查看漏拆单的订单数据
$(function () {
    $("#btnCheck").on("click", function () {
        $("#showButton").hide();
        $("#showWaiting").show();
        $.ajax({
            type: "get",
            url: "../Handler/SplitOrder.ashx?type=checkSplit&SubjectId=" + subjectId,
            success: function (data) {
                $("#showButton").show();
                $("#showWaiting").hide();
                if (data != "ok") {
                    $("#notSplitNumber").html(data + " 条");
                    $("#CheckDiv").show().dialog({
                        modal: true,
                        width: 250,
                        height: 200,
                        resizable: false

                    });
                }
            }
        })
    })
})