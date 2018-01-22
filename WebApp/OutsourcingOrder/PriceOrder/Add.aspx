<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.OutsourcingOrder.PriceOrder.Add" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../../easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            新增外协费用订单
        </p>
    </div>
    <div class="tr">
        >>订单信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td>
                客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlCustomer" runat="server">
                </asp:DropDownList>
                <span style="color: Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                活动月份
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtMonth" runat="server" CssClass="Wdate" onfocus="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true})"
                    Style="width: 80px;"></asp:TextBox>
                <span style="color: Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="30" style=" width:300px;"></asp:TextBox>
                <span style="color: Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                外协名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlOutsource" runat="server">
                  <asp:ListItem Value="0">--请选择--</asp:ListItem>
                </asp:DropDownList>
                <span style="color: Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                订单类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <asp:DropDownList ID="ddlOrderType" runat="server">
                  <asp:ListItem Value="0">--请选择--</asp:ListItem>
                </asp:DropDownList>
                <span style="color: Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                费用金额
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <asp:TextBox ID="txtPrice" runat="server" MaxLength="30"></asp:TextBox>
                <span style="color: Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                备注
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <asp:TextBox ID="txtRemark" runat="server" MaxLength="30" style=" width:300px;"></asp:TextBox>
            </td>
        </tr>
    </table>
     <div style="text-align: center; height: 35px; margin-top:30px;">
       <asp:Button ID="btnNext" runat="server" Text="提 交" 
            OnClientClick="return CheckVal()" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnNext_Click"/>
            <img id="loadingImg" src="/image/WaitImg/loadingA.gif" style=" display:none;"/>
            &nbsp;&nbsp;&nbsp;
           <asp:Button ID="btnReturn" runat="server" Text="返 回" 
            class="easyui-linkbutton" style="width: 65px; height:26px;" 
            onclick="btnReturn_Click"/>
    </div>
    </form>
</body>
</html>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script type="text/javascript">
    function CheckVal() {
        var month = $.trim($("#txtMonth").val());
        var subjectName = $.trim($("#txtSubjectName").val());
        var outsourceId = $("#ddlOutsource").val();
        var orderType = $("#ddlOrderType").val();
        var price = $.trim($("#txtPrice").val());
        if (month == "") {
            layer.msg("请选择活动月份");
            return false;
        }
        if (subjectName == "") {
            layer.msg("请填写项目名称");
            return false;
        }
        if (outsourceId == "0") {
            layer.msg("请选择外协名称");
            return false;
        }
        if (orderType == "0") {
            layer.msg("请选择订单类型");
            return false;
        }
        if (price == "") {
            layer.msg("请填写费用金额");
            return false;
        }
        else if (isNaN(price)) {
            layer.msg("费用金额填写不正确");
            return false;
        }
        else if (parseFloat(price)==0) {
            layer.msg("费用金额必须大于0");
            return false;
        }
        $("#loadingImg").show();
        return true;
    }
</script>