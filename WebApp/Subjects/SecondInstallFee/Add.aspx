﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Add.aspx.cs" Inherits="WebApp.Subjects.SecondInstallFee.Add" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            <asp:Label ID="labTitel" runat="server" Text="新增二次安装项目"></asp:Label>
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    
    <table class="table">
       <tr class="tr_bai">
            <td>
                活动名称：
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                
                <asp:DropDownList ID="ddlGuidance" runat="server"  AutoPostBack="true"
                    onselectedindexchanged="ddlGuidance_SelectedIndexChanged">
                    <asp:ListItem Value="0">请选择</asp:ListItem>
                </asp:DropDownList>
                <span style="color: Red;">*</span>
            </td>
            
        </tr>
       
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
               
                <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50" style=" width:200px;"></asp:TextBox>
                <span style="color: Red;">*</span>
            </td>
            <td style="width: 120px;">
                实施区域
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlRegion" runat="server">
                   <asp:ListItem Value="0">--请选择--</asp:ListItem>
                </asp:DropDownList>
                <span style="color: Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                费用
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
               <asp:TextBox ID="txtPrice" runat="server" MaxLength="10"></asp:TextBox><span style="color: Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                备注
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtRemark" runat="server" Style="width: 280px;"></asp:TextBox>
            </td>
        </tr>
    </table>
   
    <br />
    <div style="text-align: center; height: 35px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return CheckVal()"
            class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnSubmit_Click" />
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnNext" runat="server" Text="导入明细" OnClientClick="return CheckVal()"
            class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnNext_Click" />
        &nbsp;&nbsp;&nbsp;
       <%-- <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton"
            style="width: 65px; height: 26px;" />--%>
        <asp:Button ID="btnGoBack" runat="server" Text="返 回" class="easyui-linkbutton"
            style="width: 65px; height: 26px;" onclick="btnGoBack_Click"/>
    </div>
   
    <asp:HiddenField ID="hfCustomerId" runat="server" />
    </form>
</body>
</html>
<script type="text/javascript">
    function CheckVal() {
        if ($("#ddlGuidance").val() == "0") {
            alert("请选择活动名称");
            return false;
        }
        if ($.trim($("#txtSubjectName").val())=="") {
            alert("请填写项目名称");
            return false;
        }
        if ($("#ddlRegion").val() == "0") {
            alert("请选择区域");
            return false;
        }
        var price = $.trim($("#txtPrice").val());
        if (price == "") {
            alert("请填写合计费用");
            return false;
        }
        if (isNaN(price)) {
            alert("合计费用必须是数字");
            return false;
        }
        if (parseFloat(price) == 0) {
            alert("合计费用必须是大于0");
            return false;
        }
        return true;
    }
</script>
