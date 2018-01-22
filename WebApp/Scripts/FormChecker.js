///检验位数同时，检查是否满足需求
function checkAndCounter() {
    //B3
    var txtSalesPrepare = $.trim($("#txtSalesPrepare").val());
    if (!checknull(txtSalesPrepare)) {
        $("#lblSalesPrepare").text("销售金额预测必须填写！");
        $("#txtSalesPrepare").focus();
        return false;
    }
    else {
        if (!isnumber(txtSalesPrepare)) {
            $("#lblSalesPrepare").text("销售金额预测必须为数字！");
            $("#txtSalesPrepare").focus();
            return false;
        }
        else {
            if (parseFloat(txtSalesPrepare) < 0) {
                $("#lblSalesPrepare").text("销售金额预测必须为大于0！");
                $("#txtSalesPrepare").focus();
                return false;
            }
            else {
                $("#lblSalesPrepare").text("");
            }
        }
    }
    //B4
    var txtPurchasePresent = $.trim($("#txtPurchasePresent").val());
    if (!checknull(txtPurchasePresent)) {
        $("#lblPurchasePresent").text("进货折扣必须填写！");
        $("#txtPurchasePresent").focus();
        return false;
    }
    else {
        if (!isnumber(txtPurchasePresent)) {
            //alert(!isnumber(txtPurchasePresent));
            $("#lblPurchasePresent").text("进货折扣必须为0-100数字！");
            $("#txtPurchasePresent").focus();
            return false;
        }
        else {
            var txtPurchasePresentvalue = parseFloat(txtPurchasePresent);
            if (txtPurchasePresentvalue < 0 || txtPurchasePresentvalue > 100) {
                $("#lblPurchasePresent").text("进货折扣必须为0-100数字！");
                $("#txtPurchasePresent").focus();
                return false;
            }
            else {
                $("#lblPurchasePresent").text("");

            }
        }
    }
    //B5
    var txtSalePresent = $.trim($("#txtSalePresent").val());
    if (!checknull(txtSalePresent)) {
        $("#lblSalePresent").text("进货折扣必须填写！");
        $("#txtSalePresent").focus();
        return false;
    }
    else {
        if (!isnumber(txtSalePresent)) {
            $("#lblSalePresent").text("进货折扣必须为0-100数字！");
            $("#txtSalePresent").focus();
            return false;
        }
        else {
            var txtSalePresentvalue = parseFloat(txtSalePresent);
            if (txtSalePresentvalue < 0 || txtSalePresentvalue > 100) {
                $("#lblSalePresent").text("进货折扣必须为0-100数字！");
                $("#txtSalePresent").focus();
                return false;
            }
            else {
                $("#lblSalePresent").text("");
            }
        }
    }
    //B6
    var txtOverPresent = $.trim($("#txtOverPresent").val());
    if (!checknull(txtOverPresent)) {
        $("#lblOverPresent").text("售罄率必须填写！");
        $("#txtOverPresent").focus();
        return false;
    }
    else {
        if (!isnumber(txtOverPresent)) {
            $("#lblOverPresent").text("售罄率必须为0-100数字！");
            $("#txtOverPresent").focus();
            return false;
        }
        else {
            var txtOverPresentvalue = parseFloat(txtOverPresent);
            if (txtOverPresentvalue < 0 || txtOverPresentvalue > 100) {
                $("#lblOverPresent").text("售罄率必须为0-100数字！");
                $("#txtOverPresent").focus();
                return false;
            }
            else {
                $("#lblOverPresent").text("");
            }
        }
    }

    //计算进货成本---B7
    var txtCost = parseFloat(txtSalesPrepare) / (parseFloat(txtSalePresent) / 100) / (parseFloat(txtOverPresent) / 100) * (parseFloat(txtPurchasePresent) / 100);
    $("#txtCost").val(RoundNumber(txtCost, 0));
    //计算毛利额---B8
    var txtProfit = parseFloat(txtSalesPrepare) - parseFloat(txtCost);
    $("#txtProfit").val(RoundNumber(txtProfit, 0));
    //计算毛利率---B9
    var txtProfitPresent = parseFloat(txtProfit) / parseFloat(txtSalesPrepare) * 100;
    $("#txtProfitPresent").val(RoundNumber(txtProfitPresent, 0));



    //1.装修/家具道具采购---B11
    var txtFitmentPorp = $.trim($("#txtFitmentPorp").val());
    if (!checknull(txtFitmentPorp)) {
        $("#lblFitmentPorp").text("1.装修/家具道具采购必须填写！");
        $("#txtFitmentPorp").focus();
        return false;
    }
    else {
        if (!isnumber(txtFitmentPorp)) {
            $("#lblFitmentPorp").text("1.装修/家具道具采购必须为大于0的数字！");
            $("#txtFitmentPorp").focus();
            return false;
        }
        else {
            if (parseFloat(txtFitmentPorp) < 0) {
                $("#lblFitmentPorp").text("1.装修/家具道具采购必须为大于0的数字！");
                $("#txtFitmentPorp").focus();
                return false;
            }
            else {
                $("#lblFitmentPorp").text("");
            }
        }
    }

    //2.加盟费---B12
    var txtJoinMoney = $.trim($("#txtJoinMoney").val());
    if (!checknull(txtJoinMoney)) {
        $("#lblJoinMoney").text("");
        txtJoinMoney = 0;
    }
    else {
        if (!isnumber(txtJoinMoney)) {
            $("#lblJoinMoney").text("2.加盟费必须为大于0的数字！");
            $("#txtJoinMoney").focus();
            return false;
        }
        else {
            if (parseFloat(txtJoinMoney) < 0) {
                $("#lblJoinMoney").text("2.加盟费必须为大于0的数字！");
                $("#txtJoinMoney").focus();
                return false;
            }
            else {
                $("#lblJoinMoney").text("");
            }
        }
    }

    //3.固定资产采购---B13
    var txtFixedAssets = $.trim($("#txtFixedAssets").val());
    if (!checknull(txtFixedAssets)) {
        $("#lblFixedAssets").text("3.固定资产采购必须填！");
        $("#txtFixedAssets").focus();
        return false;
    }
    else {
        if (!isnumber(txtFixedAssets)) {
            $("#lblFixedAssets").text("3.固定资产采购必须为大于0的数字！");
            $("#txtFixedAssets").focus();
            return false;
        }
        else {
            if (parseFloat(txtFixedAssets) < 0) {
                $("#lblFixedAssets").text("3.固定资产采购必须为大于0的数字！");
                $("#txtFixedAssets").focus();
                return false;
            }
            else {
                $("#lblFixedAssets").text("");
            }
        }
    }

    //4.开业费用---B14
    var txtOpenMoney = $.trim($("#txtOpenMoney").val());
    if (!checknull(txtOpenMoney)) {
        $("#lblOpenMoney").text("");
        txtOpenMoney = 0;
    }
    else {
        if (!isnumber(txtOpenMoney)) {
            $("#lblOpenMoney").text("4.开业费用必须为大于0的数字！");
            $("#txtOpenMoney").focus();
            return false;
        }
        else {
            if (parseFloat(txtOpenMoney) < 0) {
                $("#lblOpenMoney").text("4.开业费用必须为大于0的数字！");
                $("#txtOpenMoney").focus();
                return false;
            }
            else {
                $("#lblOpenMoney").text("");
            }
        }
    }

    //5.转让费---B15
    var txtAssignMoney = $.trim($("#txtAssignMoney").val());
    if (!checknull(txtAssignMoney)) {
        $("#lblAssignMoney").text("");
        txtAssignMoney = 0;
    }
    else {
        if (!isnumber(txtAssignMoney)) {
            $("#lblAssignMoney").text("5.转让费必须为大于0的数字！");
            $("#txtAssignMoney").focus();
            return false;
        }
        else {
            if (parseFloat(txtAssignMoney) < 0) {
                $("#lblAssignMoney").text("5.转让费必须为大于0的数字！");
                $("#txtAssignMoney").focus();
                return false;
            }
            else {
                $("#lblAssignMoney").text("");
            }
        }
    }
    //开业投入---B10
    var txtOpenShopCost = parseFloat(txtFitmentPorp) + parseFloat(txtJoinMoney) + parseFloat(txtFixedAssets) + parseFloat(txtOpenMoney) + parseFloat(txtAssignMoney);
    $("#txtOpenShopCost").val(RoundNumber(txtOpenShopCost, 0));



    //1.租金---B17
    var txtRentMoney = $.trim($("#txtRentMoney").val())
    if (!checknull(txtRentMoney)) {
        $("#lblRentMoney").text("1.租金必须填写！");
        $("#txtRentMoney").focus();
        return false;
    }
    else {
        if (!isnumber(txtRentMoney)) {
            $("#lblRentMoney").text("1.租金必须为大于0的数字！");
            $("#txtRentMoney").focus();
            return false;
        }
        else {
            if (parseFloat(txtRentMoney) < 0) {
                $("#lblRentMoney").text("1.租金必须为大于0的数字！");
                $("#txtRentMoney").focus();
                return false;
            }
            else {
                $("#lblRentMoney").text("");
            }
        }
    }
    //2.装修/家具道具摊销---B18
    var txtFitmentPropShare = parseFloat(txtFitmentPorp) / 3;
    $("#txtFitmentPropShare").val(RoundNumber(txtFitmentPropShare, 0));
    //3.转让费摊销  ---B19 = B15/3
    var txtAssignShare = parseFloat(txtAssignMoney) / 3;
    $("#txtAssignShare").val(RoundNumber(txtAssignShare, 0));
    //4.固定资产摊销摊销---B20 = B13/3
    //alert("B13:" + txtFixedAssets);
    var txtFixedShare = parseFloat(txtFixedAssets) / 3;
    $("#txtFixedShare").val(RoundNumber(txtFixedShare, 0));

    //店铺租金费用 ---B16 =B17+B18+B19+B20
    var txtShopRentCost = parseFloat(txtRentMoney) + parseFloat(txtFitmentPropShare) + parseFloat(txtAssignShare) + parseFloat(txtFixedShare);
    $("#txtShopRentCost").val(RoundNumber(txtShopRentCost, 0));




    //1.1店长人数---B23
    var txtShopLeaderCount = $.trim($("#txtShopLeaderCount").val());
    if (!checknull(txtShopLeaderCount)) {
        $("#lblShopLeaderCount").text("");
        txtShopLeaderCount = 0;
    }
    else {
        if (!isnumber(txtShopLeaderCount)) {
            $("#lblShopLeaderCount").text("1.1店长人数必须为大于0的数字！");
            $("#txtShopLeaderCount").focus();
            return false;
        }
        else {
            if (parseFloat(txtShopLeaderCount) < 0) {
                $("#lblShopLeaderCount").text("1.1店长人数必须为大于0的数字！");
                $("#txtShopLeaderCount").focus();
                return false;
            }
            else {
                $("#lblShopLeaderCount").text("");
            }
        }
    }

    //1.2店长薪酬---B24
    var txtShopLeaderMoney = $.trim($("#txtShopLeaderMoney").val());
    if (!checknull(txtShopLeaderMoney)) {
        $("#lblShopLeaderMoney").text("");
        txtShopLeaderMoney = 0;
    }
    else {
        if (!isnumber(txtShopLeaderMoney)) {
            $("#lblShopLeaderMoney").text("1.2店长薪酬必须为大于0的数字！");
            $("#txtShopLeaderMoney").focus();
            return false;
        }
        else {
            if (parseFloat(txtShopLeaderMoney) < 0) {
                $("#lblShopLeaderMoney").text("1.2店长薪酬必须为大于0的数字！");
                $("#txtShopLeaderMoney").focus();
                return false;
            }
            else {
                $("#lblShopLeaderMoney").text("");
            }
        }
    }

    //1.3店员人数---B25
    var txtShopMemberCount = $.trim($("#txtShopMemberCount").val());
    if (!checknull(txtShopMemberCount)) {
        $("#lblShopMemberCount").text("1.3店员人数必须填写！");
        $("#txtShopMemberCount").focus();
        return false;
    }
    else {
        if (!isnumber(txtShopMemberCount)) {
            $("#lblShopMemberCount").text("1.3店员人数必须为大于0的数字！");
            $("#txtShopMemberCount").focus();
            return false;
        }
        else {
            if (parseFloat(txtShopMemberCount) < 0) {
                $("#lblShopMemberCount").text("1.3店员人数必须为大于0的数字！");
                $("#txtShopMemberCount").focus();
                return false;
            }
            else {
                $("#lblShopMemberCount").text("");
            }
        }
    }

    //1.4店员薪酬---B26
    var txtShopMemberMoney = $.trim($("#txtShopMemberMoney").val());
    if (!checknull(txtShopMemberMoney)) {
        $("#lblShopMemberMoney").text("1.4店员薪酬必须填写！");
        $("#txtShopMemberMoney").focus();
        return false;
    }
    else {
        if (!isnumber(txtShopMemberMoney)) {
            $("#lblShopMemberMoney").text("1.4店员薪酬必须为大于0的数字！");
            $("#txtShopMemberMoney").focus();
            return false;
        }
        else {
            if (parseFloat(txtShopMemberMoney) < 0) {
                $("#lblShopMemberMoney").text("1.4店员薪酬必须为大于0的数字！");
                $("#txtShopMemberMoney").focus();
                return false;
            }
            else {
                $("#lblShopMemberMoney").text("");
            }
        }
    }

    //1.5销售提成---B27
    var txtSaleUpper = $.trim($("#txtSaleUpper").val());
    if (!checknull(txtSaleUpper)) {
        $("#lblSaleUpper").text("1.5销售提成必须填写！");
        $("#txtSaleUpper").focus();
        return false;
    }
    else {
        if (!isnumber(txtSaleUpper)) {
            $("#lblSaleUpper").text("1.5销售提成必须为0-100数字！");
            $("#txtSaleUpper").focus();
            return false;
        }
        else {
            var txtSaleUppervalue = parseFloat(txtSaleUpper);
            if (txtSaleUppervalue < 0 || txtSaleUppervalue > 100) {
                $("#lblSaleUpper").text("1.5销售提成必须为0-100数字！");
                $("#txtSaleUpper").focus();
                return false;
            }
            else {
                $("#lblSaleUpper").text("");
            }
        }
    }

    //2.促销费---B28
    var txtSaleMoney = $.trim($("#txtSaleMoney").val());

    if (!checknull(txtSaleMoney)) {
        $("#lblSaleMoney").text("");
        txtSaleMoney = 0;
    }
    else {
        if (!isnumber(txtSaleMoney)) {
            $("#lblSaleMoney").text("1.4店员薪酬必须为大于0的数字！");
            $("#txtSaleMoney").focus();
            return false;
        }
        else {
            if (parseFloat(txtSaleMoney) < 0) {
                $("#lblSaleMoney").text("1.4店员薪酬必须为大于0的数字！");
                $("#txtSaleMoney").focus();
                return false;
            }
            else {
                $("#lblSaleMoney").text("");
            }
        }
    }

    //3.日常杂费---B29
    var txtDailyCostMoney = $.trim($("#txtDailyCostMoney").val());

    if (!checknull(txtDailyCostMoney)) {
        $("#lblDailyCostMoney").text("3.日常杂费必须填写！");
        $("#txtDailyCostMoney").focus();
        return false;
    }
    else {
        if (!isnumber(txtDailyCostMoney)) {
            $("#lblDailyCostMoney").text("3.日常杂费必须为大于0的数字！");
            $("#txtDailyCostMoney").focus();
            return false;
        }
        else {
            if (parseFloat(txtDailyCostMoney) < 0) {
                $("#lblDailyCostMoney").text("3.日常杂费必须为大于0的数字！");
                $("#txtDailyCostMoney").focus();
                return false;
            }
            else {
                $("#lblDailyCostMoney").text("");
            }
        }
    }

    //4.税金支出---B30
    var txtTaxes = $.trim($("#txtTaxes").val());
    if (!checknull(txtTaxes)) {
        $("#lblTaxes").text("4.税金支出必须填写！");
        $("#txtTaxes").focus();
        return false;
    }
    else {
        if (!isnumber(txtTaxes)) {
            $("#lblTaxes").text("4.税金支出必须为大于0的数字！");
            $("#txtTaxes").focus();
            return false;
        }
        else {
            if (parseFloat(txtTaxes) < 0) {
                $("#lblTaxes").text("4.税金支出必须为大于0的数字！");
                $("#txtTaxes").focus();
                return false;
            }
            else {
                $("#lblTaxes").text("");
            }
        }
    }


    //存货跌价损失率---B31
    var txtFallInPricePersent = $.trim($("#txtFallInPricePersent").val());
    if (!checknull(txtFallInPricePersent)) {
        $("#lblFallInPricePersent").text("存货跌价损失率必须填写！");
        $("#txtFallInPricePersent").focus();
        return false;
    }
    else {
        if (!isnumber(txtFallInPricePersent)) {
            $("#lblFallInPricePersent").text("存货跌价损失率必须为0-100数字！");
            $("#txtFallInPricePersent").focus();
            return false;
        }
        else {
            var txtFallInPricePersentvalue = parseFloat(txtFallInPricePersent);
            if (txtFallInPricePersentvalue < 0 || txtFallInPricePersentvalue > 100) {
                $("#lblFallInPricePersent").text("存货跌价损失率必须为0-100数字！");
                $("#txtFallInPricePersent").focus();
                return false;
            }
            else {
                $("#lblFallInPricePersent").text("");
            }
        }
    }


    //合计1.人员薪资福利---B22 =B23*B24*12+B25*B26*12+B3/B5*B27
    var txtWelfare = parseFloat(txtShopLeaderCount) * parseFloat(txtShopLeaderMoney) * 12
             + parseFloat(txtShopMemberCount) * parseFloat(txtShopMemberMoney) * 12 + parseFloat(txtSalesPrepare) / parseFloat(txtSalePresent) * parseFloat(txtSaleUpper);
    $("#txtWelfare").val(RoundNumber(txtWelfare, 0));


    //            alert("B22:" + txtWelfare);
    //            alert("B28:" + txtSaleMoney);
    //            alert("B29:" + txtDailyCostMoney);
    //            alert("B30:" + txtTaxes);
    //合计店铺管理费用---B21 = B22+B28+B29+B30
    var txtShopManageCost = parseFloat(txtWelfare) + parseFloat(txtSaleMoney) + parseFloat(txtDailyCostMoney) + parseFloat(txtTaxes);
    $("#txtShopManageCost").val(RoundNumber(txtShopManageCost, 0));


    //            alert("B3:" + txtSalesPrepare);
    //            alert("B5:" + txtSalePresent);
    //            alert("B6:" + txtOverPresent);
    //            alert("B4:" + txtPurchasePresent);
    //            alert("B31:" + txtTaxes)
    //存货跌价损失---B32 =B3/B5*(1-B6)*B4*B31
    var txtFallInPriceMoney = parseFloat(txtSalesPrepare) / (parseFloat(txtSalePresent) / 100) * (1 - (parseFloat(txtOverPresent) / 100)) * (parseFloat(txtPurchasePresent) / 100) * (parseFloat(txtFallInPricePersent) / 100);
    $("#txtFallInPriceMoney").val(RoundNumber(txtFallInPriceMoney, 0));
    //净利润----B33 =B8-B32-B16-B21
    var txtRatio = parseFloat(txtProfit) - parseFloat(txtFallInPriceMoney) - parseFloat(txtShopRentCost) - parseFloat(txtShopManageCost);
    $("#txtRatio").val(RoundNumber(txtRatio, 0));

    AddShopManagePlan();
}

function RoundNumber(num, pos) {
    return Math.round(num * Math.pow(10, pos)) / Math.pow(10, pos);
}

//添加更新
function AddShopManagePlan() {

    var ShopId = $.trim($("#hfShopId").val()); //店铺ID
    //alert(ShopId);
    var ShopManagePlanId = $.trim($("#hfShopManagePlanId").val()); //经营预算ID
    //alert(ShopManagePlanId);
    var ShopPlanYear = $.trim($("#hfShopPlanYear").val()); //经营预算ID
    //alert(ShopPlanYear);
    var SalesPrepare = $.trim($("#txtSalesPrepare").val()); //B3---销售金额预测
    //alert(SalesPrepare);
    var PurchasePresent = $.trim($("#txtPurchasePresent").val()); //B4---进货折扣
    //alert(PurchasePresent);
    var SalePresent = $.trim($("#txtSalePresent").val()); //B5---销售折扣
    //alert(SalePresent);
    var OverPresent = $.trim($("#txtOverPresent").val()); //B6---售罄率
    //alert(OverPresent);
    var Cost = $.trim($("#txtCost").val()); //B7---进货成本
    //alert(Cost);
    var Profit = $.trim($("#txtProfit").val()); //B8---毛利额
    //alert(Profit);
    var ProfitPresent = $.trim($("#txtProfitPresent").val()); //B9---毛利率
    //alert(ProfitPresent);
    var OpenShopCost = $.trim($("#txtOpenShopCost").val()); //B10---开业投入\
    //alert(OpenShopCost);
    var FitmentPorp = $.trim($("#txtFitmentPorp").val()); //B11---1.装修/家具道具采购
    //alert(FitmentPorp);
    var JoinMoney = $.trim($("#txtJoinMoney").val()); //B12---2.加盟费
    //alert(JoinMoney);
    var FixedAssets = $.trim($("#txtFixedAssets").val()); //B13---3.固定资产采购
    //alert(FixedAssets);
    var OpenMoney = $.trim($("#txtOpenMoney").val()); //B14---4.开业费用
    //alert(OpenMoney);
    var AssignMoney = $.trim($("#txtAssignMoney").val()); //B15---5.转让费
    //alert(AssignMoney);
    var ShopRentCost = $.trim($("#txtShopRentCost").val()); //B16---店铺租金费用
    //alert(ShopRentCost);
    var RentMoney = $.trim($("#txtRentMoney").val()); //B17---1.租金
    //alert(RentMoney);
    var FitmentPropShare = $.trim($("#txtFitmentPropShare").val()); //B18---2.装修/家具道具摊销
    //alert(FitmentPropShare);
    var AssignShare = $.trim($("#txtAssignShare").val()); //B19---3.转让费摊销
    //alert(AssignShare);
    var FixedShare = $.trim($("#txtFixedShare").val()); //B20---4.固定资产摊销摊销
    //alert(FixedShare);
    var ShopManageCost = $.trim($("#txtShopManageCost").val()); //B21---店铺管理费用
    //alert(ShopManageCost);
    var Welfare = $.trim($("#txtWelfare").val()); //B22---1.人员薪资福利
    //alert(Welfare);
    var ShopLeaderCount = $.trim($("#txtShopLeaderCount").val()); //B23---1.1店长人数
    //alert(ShopLeaderCount);
    var ShopLeaderMoney = $.trim($("#txtShopLeaderMoney").val()); //B24---1.2店长薪酬
    //alert(ShopLeaderMoney);
    var ShopMemberCount = $.trim($("#txtShopMemberCount").val()); //B25---1.3店员人数
    //alert(ShopMemberCount);
    var ShopMemberMoney = $.trim($("#txtShopMemberMoney").val()); //B26---1.4店员薪酬
    //alert(ShopMemberMoney);
    var SaleUpper = $.trim($("#txtSaleUpper").val()); //B27---1.5销售提成
    //alert(SaleUpper);
    var SaleMoney = $.trim($("#txtSaleMoney").val()); //B28---2.促销费
    //alert(SaleMoney);
    var DailyCostMoney = $.trim($("#txtDailyCostMoney").val()); //B29---3.日常杂费
    //alert(DailyCostMoney);
    var Taxes = $.trim($("#txtTaxes").val()); //B30---4.税金支出
    //alert(Taxes);
    var FallInPricePersent = $.trim($("#txtFallInPricePersent").val()); //B31---存货跌价损失率
    //alert(FallInPricePersent);
    var FallInPriceMoney = $.trim($("#txtFallInPriceMoney").val()); //B32---存货跌价损失
    //alert(FallInPriceMoney);
    var Ratio = $.trim($("#txtRatio").val()); //B33---净利润
    //alert(Ratio);
    //alert("test");

    //    $.ajax({
    //        type: "get",
    //        dataType: "text",
    //        url: "../Handler/ShopManagePlan_FormCheckerjs.ashx",
    //        data: { ShopId: ShopId, ShopManagePlanId: ShopManagePlanId, ShopPlanYear: ShopPlanYear, SalesPrepare: SalesPrepare, PurchasePresent: PurchasePresent, SalePresent: SalePresent,
    //            OverPresent: OverPresent, Cost: Cost, Profit: Profit, ProfitPresent: ProfitPresent,
    //            OpenShopCost: OpenShopCost, FitmentPorp: FitmentPorp, JoinMoney: JoinMoney, FixedAssets: FixedAssets,
    //            OpenMoney: OpenMoney, AssignMoney: AssignMoney, ShopRentCost: ShopRentCost, RentMoney: RentMoney,
    //            FitmentPropShare: FitmentPropShare, AssignShare: AssignShare, FixedShare: FixedShare,
    //            ShopManageCost: ShopManageCost, Welfare: Welfare, ShopLeaderCount: ShopLeaderCount, ShopLeaderMoney: ShopLeaderMoney,
    //            ShopMemberCount: ShopMemberCount, ShopMemberMoney: ShopMemberMoney, SaleUpper: SaleUpper, SaleMoney: SaleMoney,
    //            DailyCostMoney: DailyCostMoney, Taxes: Taxes, FallInPricePersent: FallInPricePersent, FallInPriceMoney: FallInPriceMoney,
    //            Ratio: Ratio, num: new Date()
    //        },
    //        beforeSend: function () { $("#divloading").css("display", "block"); $("#divShowUpdate").css("display", "none"); },
    //        complete: function () { $("#divloading").css("display", "none"); },
    //        success: function (data) {
    //            if (data != "0") {
    //                if (ShopManagePlanId == "-1") {
    //                    alert("添加成功！");
    //                    //document.location.reload();
    //                }
    //                else {
    //                    alert("修改成功！");
    //                    //document.location.reload();
    //                }
    //            }
    //            else {
    //                $("#divShowUpdate").css("display", "block");
    //                alert("服务器繁忙，稍后重试！");
    //            }
    //        }
    //    });

    $.post("../Handler/ShopManagePlan_FormCheckerjs.ashx",
      { ShopId: ShopId, ShopManagePlanId: ShopManagePlanId, ShopPlanYear: ShopPlanYear, SalesPrepare: SalesPrepare, PurchasePresent: PurchasePresent, SalePresent: SalePresent,
          OverPresent: OverPresent, Cost: Cost, Profit: Profit, ProfitPresent: ProfitPresent,
          OpenShopCost: OpenShopCost, FitmentPorp: FitmentPorp, JoinMoney: JoinMoney, FixedAssets: FixedAssets,
          OpenMoney: OpenMoney, AssignMoney: AssignMoney, ShopRentCost: ShopRentCost, RentMoney: RentMoney,
          FitmentPropShare: FitmentPropShare, AssignShare: AssignShare, FixedShare: FixedShare,
          ShopManageCost: ShopManageCost, Welfare: Welfare, ShopLeaderCount: ShopLeaderCount, ShopLeaderMoney: ShopLeaderMoney,
          ShopMemberCount: ShopMemberCount, ShopMemberMoney: ShopMemberMoney, SaleUpper: SaleUpper, SaleMoney: SaleMoney,
          DailyCostMoney: DailyCostMoney, Taxes: Taxes, FallInPricePersent: FallInPricePersent, FallInPriceMoney: FallInPriceMoney,
          Ratio: Ratio, num: new Date()
      },
      function (data) {
          if (data != "0") {
              if (ShopManagePlanId == "-1") {
                  alert("添加成功！");
                  window.location = window.location;
              }
              else {
                  alert("修改成功！");
                  window.location = window.location;
              }
          }
          else {
              $("#divShowUpdate").css("display", "block");
              alert("服务器繁忙，稍后重试！");
          }
      }, "text");
}

function ShowShopManagePlanInfo(obj) {
    $.ajax({
        //dataType: "text",
        url: "../Handler/ShowSingleShopManagePlan_FormCheckerjs.ashx?" + new Date(),
        data: { ShopManagePlanId: obj },
        success:
        function (list) {
            //alert(list);
            var jsonlist = eval("(" + list + ")");
            //alert(jsonlist);
            $("#divShowUpdate").css("display", "block"); //显示增加页面
            $("#divShowManagePlan").css("display", "none"); //列表页面隐藏
            //alert(jsonlist.ShopManagePlanId);
            $("#hfShopManagePlanId").val(jsonlist.ShopManagePlanId); //经营预算ID
            $("#hfShopPlanYear").val(jsonlist.ShopPlanYear); //经营预算ID
            $("#txtSalesPrepare").val(jsonlist.SalesPrepare); //B3---销售金额预测
            $("#txtPurchasePresent").val(jsonlist.PurchasePresent); ; //B4---进货折扣
            $("#txtSalePresent").val(jsonlist.SalePresent); ; //B5---销售折扣
            $("#txtOverPresent").val(jsonlist.OverPresent); ; //B6---售罄率
            $("#txtCost").val(jsonlist.Cost); //B7---进货成本
            $("#txtProfit").val(jsonlist.Profit); ; //B8---毛利额
            $("#txtProfitPresent").val(jsonlist.ProfitPresent); ; //B9---毛利率
            $("#txtOpenShopCost").val(jsonlist.OpenShopCost); ; //B10---开业投入
            $("#txtFitmentPorp").val(jsonlist.FitmentPorp); ; //B11---1.装修/家具道具采购
            $("#txtJoinMoney").val(jsonlist.JoinMoney); ; //B12---2.加盟费
            $("#txtFixedAssets").val(jsonlist.FixedAssets); ; //B13---3.固定资产采购
            $("#txtOpenMoney").val(jsonlist.OpenMoney); ; //B14---4.开业费用
            $("#txtAssignMoney").val(jsonlist.AssignMoney); ; //B15---5.转让费
            $("#txtShopRentCost").val(jsonlist.ShopRentCost); ; //B16---店铺租金费用
            $("#txtRentMoney").val(jsonlist.RentMoney); ; //B17---1.租金
            $("#txtFitmentPropShare").val(jsonlist.FitmentPropShare); ; //B18---2.装修/家具道具摊销
            $("#txtAssignShare").val(jsonlist.AssignShare); ; //B19---3.转让费摊销
            $("#txtFixedShare").val(jsonlist.FixedShare); ; //B20---4.固定资产摊销摊销
            $("#txtShopManageCost").val(jsonlist.ShopManageCost); ; //B21---店铺管理费用
            $("#txtWelfare").val(jsonlist.Welfare); ; //B22---1.人员薪资福利
            $("#txtShopLeaderCount").val(jsonlist.ShopLeaderCount); ; //B23---1.1店长人数
            $("#txtShopLeaderMoney").val(jsonlist.ShopLeaderMoney); ; //B24---1.2店长薪酬
            $("#txtShopMemberCount").val(jsonlist.ShopMemberCount); ; //B25---1.3店员人数
            $("#txtShopMemberMoney").val(jsonlist.ShopMemberMoney); ; //B26---1.4店员薪酬
            $("#txtSaleUpper").val(jsonlist.SaleUpper); ; //B27---1.5销售提成
            $("#txtSaleMoney").val(jsonlist.SaleMoney); ; //B28---2.促销费
            $("#txtDailyCostMoney").val(jsonlist.DailyCostMoney); ; //B29---3.日常杂费
            $("#txtTaxes").val(jsonlist.Taxes); ; //B30---4.税金支出
            $("#txtFallInPricePersent").val(jsonlist.FallInPricePersent); ; //B31---存货跌价损失率
            $("#txtFallInPriceMoney").val(jsonlist.FallInPriceMoney); ; //B32---存货跌价损失
            $("#txtRatio").val(jsonlist.Ratio); //B33---净利润

        }
    });
}


function DeleteShopManagePlanInfo(obj) {
    $.ajax({
        dataType: "text",
        url: "../Handler/ShopManagePlanDelete_FormCheckerjs.ashx?" + new Date(),
        data: { ShopManagePlanId: obj },
        success:
        function (data) {
            if (data != "0") {
                alert("删除成功！");
                document.location.reload();
            }
            else {
                alert("服务器忙，稍后重试！");
            }
        }
    });
}

function ShopAddDiv() {
    $("#divShowUpdate").css("display", "block"); //显示增加页面
    $("#divShowManagePlan").css("display", "none"); //列表页面隐藏

    $("#hfShopManagePlanId").val("-1"); //经营预算ID
    $("#hfShopPlanYear").val(""); //经营预算ID
    $("#txtSalesPrepare").val(""); //B3---销售金额预测
    $("#txtPurchasePresent").val(""); //B4---进货折扣
    $("#txtSalePresent").val(""); //B5---销售折扣
    $("#txtOverPresent").val(""); //B6---售罄率
    $("#txtCost").val("自动计算省略填写"); //B7---进货成本
    $("#txtProfit").val("自动计算省略填写"); //B8---毛利额
    $("#txtProfitPresent").val("自动计算省略填写"); //B9---毛利率
    $("#txtOpenShopCost").val("自动计算省略填写"); //B10---开业投入
    $("#txtFitmentPorp").val(""); //B11---1.装修/家具道具采购
    $("#txtJoinMoney").val(""); //B12---2.加盟费
    $("#txtFixedAssets").val(""); //B13---3.固定资产采购
    $("#txtOpenMoney").val(""); //B14---4.开业费用
    $("#txtAssignMoney").val(""); //B15---5.转让费
    $("#txtShopRentCost").val("自动计算省略填写"); //B16---店铺租金费用
    $("#txtRentMoney").val(""); //B17---1.租金
    $("#txtFitmentPropShare").val("自动计算省略填写"); //B18---2.装修/家具道具摊销
    $("#txtAssignShare").val("自动计算省略填写"); //B19---3.转让费摊销
    $("#txtFixedShare").val("自动计算省略填写"); //B20---4.固定资产摊销摊销
    $("#txtShopManageCost").val("自动计算省略填写"); //B21---店铺管理费用
    $("#txtWelfare").val("自动计算省略填写"); //B22---1.人员薪资福利
    $("#txtShopLeaderCount").val(""); //B23---1.1店长人数
    $("#txtShopLeaderMoney").val(""); //B24---1.2店长薪酬
    $("#txtShopMemberCount").val(""); //B25---1.3店员人数
    $("#txtShopMemberMoney").val(""); //B26---1.4店员薪酬
    $("#txtSaleUpper").val(""); //B27---1.5销售提成
    $("#txtSaleMoney").val(""); //B28---2.促销费
    $("#txtDailyCostMoney").val(""); //B29---3.日常杂费
    $("#txtTaxes").val(""); //B30---4.税金支出
    $("#txtFallInPricePersent").val(""); //B31---存货跌价损失率
    $("#txtFallInPriceMoney").val("自动计算省略填写"); //B32---存货跌价损失
    $("#txtRatio").val("自动计算省略填写"); //B33---净利润

    var titleDiv = $(".title div");
    if (titleDiv != null) {
        $("#hfShopPlanYear").val(titleDiv.length + 1);
    } else {
        $("#hfShopPlanYear").val("1");
    }
}

//取消编辑
function ShowPlanList() {
    $("#divShowUpdate").css("display", "none"); //显示增加页面
    $("#divShowManagePlan").css("display", "block"); //列表页面隐藏

}

function InitTable() {
    $(".table").each(
                function (i, item) {
                    $(item).children().each(
                        function (j, trs) {
                            $(trs).children().each(
                                function (k, tds) {
                                    $(tds).children().each(
                                        function (l, t) {
                                            $(t).attr("class", "nav_table_td");
                                        });
                                });
                        });
                });
}

function FormValidate2() {
    var hfIsContainCompeteGoods = $("#hfIsContainCompeteGoods").val();
    var hfHasShopManagePlan = $("#hfHasShopManagePlan").val();
    //alert(hfHasShopManagePlan);
    //alert(hfIsContainCompeteGoods);
    if (hfIsContainCompeteGoods == "false") {
        $("#lblIsContainCompeteGoods").text("竞品信息必填！！");
        return false;
    }
    else {
        $("#lblIsContainCompeteGoods").text("");
    }
    if (hfHasShopManagePlan == "0") {
        $("#lblShopManagePlan").text("1.店铺经营预算表(或财务评估书)必填！！");
        return false;
    }
    else {
        $("#lblShopManagePlan").text("");
    }
    var show1 = $.trim($("#show1").html());

    if (show1 == "") {
        //alert($("#lblshow1").text());
        $("#lblshow1").text("2.店铺照片必须上传！！");
        return false;
    }
    else {
        $("#lblshow1").text("");
    }
    var show2 = $.trim($("#show2").html());
    if (show2 == "") {
        $("#lblshow2").text("3.商圈图必须上传！！");
        return false;
    }
    else {
        $("#lblshow2").text("");
    }
    var show3 = $.trim($("#show3").html());
    if (show3 == "") {
        $("#lblshow3").text("4.竞品照片必须上传！！");
        return false;
    }
    else {
        $("#lblshow3").text("");
    }
    return true;
}

function FormValidate3() {
    alert("test");
    $("#form1").validate({
        rules: {
            hfHasShopManagePlan: { required: function (element) { return $("#hfHasShopManagePlan").val() != "0"; } },
            show1: { required: function (element) { return false } },
            show2: { required: function (element) { return $("#show2").html() != ""; } },
            show3: { required: function (element) { return $("#show3").html() != ""; } }
        },
        messages: {
            hfHasShopManagePlan: "<span style=\"color:red\"> 1.店铺经营预算表(或财务评估书)必填！！</span>",
            show1: "<span style=\"color:red\"> 2.店铺照片必须上传！！</span>",
            show2: "<span style=\"color:red\"> 3.商圈图必须上传！！</span>",
            show3: "<span style=\"color:red\"> 4.竞品照片必须上传！！</span>"
        },
        errorPlacement: function (error, element) {
            error.appendTo(element.parent());
            if (element.is(":radio")) {
                //alert("[name='" + $(element).attr("name") + "']:last");
                error.appendTo($("[name='" + $(element).attr("name") + "']:last").next());
            } else if (element.is(":checkbox")) {
                error.appendTo(element.next());
            }
        }
    });

}


function FormValidate() {
    $("#form1").validate({
        rules: {
            ddlRegion: { selectNone: true },
            ddlProvince: { selectNone: true },
            ddlCity: { selectNone: true },
            //ddlCounty: { selectNone: true },
            txtRoad: { required: true },
            txtShopAddress: { required: true },
            txtMap: { required: true },
            txtShopName: { required: true },
            txtClientName: { required: true },
            txtClientIDCode: { vailIDCode: true },
            txtClientTelephone: { vailTelephone: true },
            txtClientCellPhone: { vailCellphone: true },
            txtClientFax: { vailTelephone: true },
            rbtnlAdministrativeLevelId: {
                required: function (element) {
                    return $("input:radio[name='rbtnlAdministrativeLevelId']:checked").val() != "";
                }
            },
            rbtnlMarketLevelId: {
                required: function (element) {
                    return $("input:radio[name='rbtnlMarketLevelId']:checked").val() != "";
                }
            },
            rbtnlShopCategoryId: {
                required: function (element) {
                    return $("input:radio[name='rbtnlShopCategoryId']:checked").val() != "";
                }
            },
            rbtnlBussinessWayId: {
                required: function (element) {
                    return $("input:radio[name='rbtnlBussinessWayId']:checked").val() != "";
                }
            },
            rbtnBussinessCategoryId: {
                required: function (element) {
                    return $("input:radio[name='rbtnBussinessCategoryId']:checked").val() != "";
                }
            },
            rbtnShopSeriesId: {
                required: function (element) {
                    return $("input:radio[name='rbtnShopSeriesId']:checked").val() != "";
                }
            },
            txtShopArea: {
                required: true,
                number: true,
                range: [1, 10000]
            },
            txtShopFloor: {
                required: true,
                digits: true,
                range: [1, 100]
            },
            txtShopBlowArea: {
                required: true,
                number: true,
                range: [1, 10000]
            },
            txtShopStorage: {
                required: true,
                number: true,
                range: [1, 10000]
            },
            txtShopFloorHeight:
            {
                required: true,
                number: true,
                range: [1, 100]
            },
            txtShopWindowCount: {
                required: true,
                number: true,
                range: [1, 100]
            },
            txtShopDoorWidthSize: {
                required: true,
                number: true,
                range: [1, 10000]
            },
            txtShopDoorHeightSize: {
                required: true,
                number: true,
                range: [1, 10000]

            },
            txtPOSInitDate: {
                required: true
            },
            txtShopCharterMoney: {
                required: function () {
                    return $.trim($("#txtShopPoint").val()) == "";
                },
                number: true,
                range: [1, 10000]
            },
            txtShopPoint: {
                required: function () {
                    return $.trim($("#txtShopCharterMoney").val()) == "";
                },
                number: true,
                range: [1, 100]
            },
            txtShopRentBeginTime: {
                required: true
            },
            txtShopRentEndTime: {
                required: true
            },
            txtShopDoorHead: {
                required: true
            },
            txtShopShelf: {
                required: true
            },
            txtShopPlanSale: {
                required: true,
                number: true,
                range: [1, 10000]
            },
            txtShopPurchase: {
                required: true,
                number: true,
                range: [1, 10000]
            },
            isFrontMoney: {
                required: function (element) {
                    return $("input:radio[name='isFrontMoney']:checked").val() != "";
                }
            },
            txtFrontMoney: {
                required: function (element) {
                    return $("input:radio[name='isFrontMoney']:checked").val() == "rbtnIsFrontMoney1";
                },
                number: true,
                range: [1, 10000]
            },
            //            isGroup: {
            //                required: function (element) {
            //                    return $("input:radio[name='isGroup']:checked").val() != "";
            //                }
            //            },
            txtPartner: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsDirect";
                }
            },
            txtPartnerDuty: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsDirect";
                }
            },
            txtPartnerRake: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsDirect";
                }
            },
            txtShareholder: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsJoint";
                }
            },
            txtRequire: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsJoint";
                }
            },
            txtProportion: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsJoint";
                }
            },
            txtSalePresent: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsHasJoin";
                }
            },
            txtSaleBeginDate: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsHasJoin";
                }
            },
            txtSaleEndDate: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsHasJoin";
                }
            },
            txtSaleRequire: {
                required: function (element) {
                    return $("input:radio[name='isGroup']:checked").val() == "rbtnIsHasJoin";
                }
            },
            txtPlanOpeningDate: {
                required: true
            },
            txtSendOutDate: {
                required: true
            },
            ddlAssignCompany: {
                required: true
            },
            txtReceptInfo: {
                required: true
            },
            txtReceptAddress: {
                required: true
            }
        },
        messages: {
            ddlRegion: "<span style=\"color:red\">请选择区域！！</span>",
            ddlProvince: "<span style=\"color:red\">请选择省份！！</span>",
            ddlCity: "<span style=\"color:red\">请选择城市！！</span>",
            //ddlCounty: "<span style=\"color:red\">请选择区县！！</span>",
            txtRoad: "<span style=\"color:red\">镇/街道必填！！</span>",
            txtShopAddress: "<span style=\"color:red\">店铺详细地址必填！！</span>",
            txtMap: "<span style=\"color:red\">店铺所在地图位置必填！！</span>",
            txtShopName: "<span style=\"color:red\">店铺名称必填！！</span>",
            txtShopName: "<span style=\"color:red\">店铺名称必填！！</span>",
            txtClientName: "<span style=\"color:red\">客户名称必填！！</span>",
            txtClientIDCode: "<span style=\"color:red\">身份证号必填或格式错误！！</span>",
            txtClientTelephone: "<span style=\"color:red\">电话必填或X010-12345678格式！！</span>",
            txtClientCellPhone: "<span style=\"color:red\">手机必填或格式错误！！</span>",
            txtClientFax: "<span style=\"color:red\">传真必填或格式错误！！</span>",
            rbtnlAdministrativeLevelId: "<span style=\"color:red\">请选择行政级别！！</span>",
            rbtnlMarketLevelId: "<span style=\"color:red\">请选择市场级别！！</span>",
            rbtnlShopCategoryId: "<span style=\"color:red\">请选择店铺类型！！</span>",
            rbtnlBussinessWayId: "<span style=\"color:red\">请选择经营方式！！</span>",
            rbtnBussinessCategoryId: "<span style=\"color:red\">请选择经营类型！！</span>",
            rbtnShopSeriesId: "<span style=\"color:red\">请选择店铺系列！！</span>",
            txtShopArea: "<span style=\"color:red\">营业面积必填，且必须为1-10000之间数字！！</span>",
            txtShopFloor: "<span style=\"color:red\">层数必填，且必须为1-100之间数字！！</span>",
            txtShopBlowArea: "<span style=\"color:red\">底层面积必填，且必须为1-10000之间数字！！</span>",
            txtShopStorage: "<span style=\"color:red\">仓库面积必填，且必须为1-10000之间数字！！</span>",
            txtShopFloorHeight: "<span style=\"color:red\">层高必填，且必须为1-100之间数字！！</span>",
            txtShopWindowCount: "<span style=\"color:red\">橱窗数必填，且必须为1-100之间数字！！</span>",
            txtShopDoorWidthSize: "<span style=\"color:red\">门头宽必填,且必须为1-100之间数字！！</span>",
            txtShopDoorHeightSize: "<span style=\"color:red\">门头高必填,且必须为1-100之间数字！！</span>",
            txtPOSInitDate: "<span style=\"color:red\">POS安装日期必填，且为合法日期！！</span>",
            txtShopCharterMoney: "<span style=\"color:red\">租金必填，且必须为1-10000之间数字！！</span>",
            txtShopPoint: "<span style=\"color:red\">扣点必填，且必须为1-100之间数字！！</span>",
            txtShopRentBeginTime: "<span style=\"color:red\">租期开始时间必填，且为合法日期！！</span>",
            txtShopRentEndTime: "<span style=\"color:red\">租期结束时间必填，且为合法日期！！</span>",
            txtShopDoorHead: "<span style=\"color:red\">门头代数必填！！</span>",
            txtShopShelf: "<span style=\"color:red\">货架代数必填！！</span>",
            txtShopPlanSale: "<span style=\"color:red\">预计年销售额必填，且必须为1-10000之间数字！！</span>",
            txtShopPurchase: "<span style=\"color:red\">预计年进货额必填，且必须为1-10000之间数字！！</span>",
            isFrontMoney: "<span style=\"color:red\">请选择是否需要支付订金！！</span>",
            txtFrontMoney: "<span style=\"color:red\">申请支付订金必填，且必须为1-10000之间数字！！</span>",
            //isGroup: "<span style=\"color:red\">请选择合作方式！！</span>",
            txtPartner: "<span style=\"color:red\">合作方必填！！</span>",
            txtPartnerDuty: "<span style=\"color:red\">合作义务必填！！</span>",
            txtPartnerRake: "<span style=\"color:red\">合作抽成必填！！</span>",
            txtShareholder: "<span style=\"color:red\">股东必填！！</span>",
            txtRequire: "<span style=\"color:red\">出资要求必填！！</span>",
            txtProportion: "<span style=\"color:red\">出资比例必填！！</span>",
            txtSalePresent: "<span style=\"color:red\">代销折扣必填！！</span>",
            txtSaleBeginDate: "<span style=\"color:red\">代销开始时间必填，且为合法日期！！</span>",
            txtSaleEndDate: "<span style=\"color:red\">代销结束时间必填，且为合法日期！！</span>",
            txtSaleRequire: "<span style=\"color:red\">代销终期首售罄率要求必填！！</span>",
            txtPlanOpeningDate: "<span style=\"color:red\">预计开业时间必填，且为合法日期！！</span>",
            txtSendOutDate: "<span style=\"color:red\">指定发货时间必填，且为合法日期！！</span>",
            ddlAssignCompany: "<span style=\"color:red\">指定发货公司必选！！</span>",
            txtReceptInfo: "<span style=\"color:red\">收货人及联系方式必填！！</span>",
            txtReceptAddress: "<span style=\"color:red\">指定地址必填！！</span>"
        },
        errorPlacement: function (error, element) {
            error.appendTo(element.parent());
            if (element.is(":radio")) {
                //alert("[name='" + $(element).attr("name") + "']:last");
                error.appendTo($("[name='" + $(element).attr("name") + "']:last").next());
            } else if (element.is(":checkbox")) {
                error.appendTo(element.next());
            }
        }
    });
}

function ReplaceBy(obj, str) {
    var newobj = obj.replace("ibtnUpdate", str);
    return newobj;
}

function CheckValue(obj) {
    var client = obj.id;
    var txtTrademark = $("#" + ReplaceBy(client, "txtTrademark")).val();
    //alert("#" + ReplaceBy(client, "txtTrademark"));
    var txtShopCount = $("#" + ReplaceBy(client, "txtShopCount")).val();
    var txtShopArea = $("#" + ReplaceBy(client, "txtShopArea")).val();
    var txtShopPersent = $("#" + ReplaceBy(client, "txtShopPersent")).val();
    if (!checknull(txtTrademark)) {
        alert("品牌不能为空！！");
        $("#" + ReplaceBy(client, "txtTrademark")).focus();
        return false;
    }

    if (!checknull(txtShopCount)) {
        alert("店数不能为空！！");
        $("#" + ReplaceBy(client, "txtShopCount")).focus();
        return false;
    }
    else {
        if (!isint(txtShopCount)) {
            alert("店数必须为整数！！");
            $("#" + ReplaceBy(client, "txtShopCount")).focus();
            return false;
        } else {
            if (parseFloat(txtShopCount) <= 0) {
                alert("店数必须为大于0整数！");
                $("#" + ReplaceBy(client, "txtShopCount")).focus();
                return false;
            }
        }
    }

    if (!checknull(txtShopArea)) {
        alert("面积不能为空！！");
        $("#" + ReplaceBy(client, "txtShopArea")).focus();
        return false;
    }
    else {
        if (!isnumber(txtShopArea)) {
            alert("面积必须为数字！！");
            $("#" + ReplaceBy(client, "txtShopArea")).focus();
            return false;
        }
        else {
            if (parseFloat(txtShopArea) <= 0) {
                alert("面积必须为大于0整数！");
                $("#" + ReplaceBy(client, "txtShopArea")).focus();
                return false;
            }
        }
    }

    if (!checknull(txtShopPersent)) {
        alert("店效不能为空！！");
        $("#" + ReplaceBy(client, "txtShopPersent")).focus();
        return false;
    }
    return true;
}

function InitAddress() {

    var AddressDetail = "";
    var txtRoad = $.trim($("#txtRoad").val());
    var ddlProvince = $("#ddlProvince").val();
    var ddlCity = $("#ddlCity").val();
    var ddlCounty = $("#ddlCounty").val();
    //alert($("#ddlProvince option:selected").text());
    if (ddlProvince != "-1") {
        AddressDetail = $("#ddlProvince option:selected").text();
    }
    if (ddlCity != "-1") {
        AddressDetail = $("#ddlProvince option:selected").text();
        if ($("#ddlCity option:selected").text() == $("#ddlProvince option:selected").text().substr(0, 3)) {
            AddressDetail += $("#ddlCity option:selected").text();
        }
    }
    if (ddlCounty != "-1") {
        AddressDetail = $("#ddlProvince option:selected").text();
        if ($("#ddlCity option:selected").text() == $("#ddlProvince option:selected").text().substr(0, 3)) {
            AddressDetail += $("#ddlCity option:selected").text();
        }
        AddressDetail += $("#ddlCounty option:selected").text();
    }
    if (txtRoad != "") {
        AddressDetail += txtRoad;
        //$("#txtShopAddress").val("");
        $("#txtShopAddress").val(AddressDetail);
        $("#txtReceptAddress").val(AddressDetail);
    }

}