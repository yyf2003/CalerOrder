<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddOrderDetail.aspx.cs"
    Inherits="WebApp.Subjects.NewShopSubject.AddOrderDetail" %>

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
    <style type="text/css">
        .style1
        {
            height: 30px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            添加订单信息
        </p>
    </div>
    <div class="tr" style="margin-top: 20px;">
        >>店铺信息</div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="table" style="text-align: center;">
                <tr>
                    <td style="width: 120px; height: 30px;">
                        省份
                    </td>
                    <td style="text-align: left; width: 250px; padding-left: 5px;">
                        <asp:DropDownList ID="ddlProvince" runat="server">
                            <asp:ListItem Value="0">--请选择--</asp:ListItem>
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                    <td style="height: 30px; width: 120px;">
                        城市
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlCity" runat="server">
                            <asp:ListItem Value="0">--请选择--</asp:ListItem>
                        </asp:DropDownList>
                        <span style="color: Red;">*</span> &nbsp;区/县：
                        <asp:DropDownList ID="ddlArea" runat="server">
                            <asp:ListItem Value="0">--请选择--</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        店铺编码
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtPOSCode" runat="server" MaxLength="50"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                    <td style="height: 30px;">
                        店铺名称
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtPOSName" runat="server" MaxLength="50" Style="width: 220px;"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        经销商编号
                    </td>
                    <td style="text-align: left; padding-left: 5px;" class="style1">
                        <asp:TextBox ID="txtAgentNo" runat="server" MaxLength="20"></asp:TextBox>
                    </td>
                    <td class="style1">
                        经销商名称
                    </td>
                    <td style="text-align: left; padding-left: 5px;" class="style1">
                        <asp:TextBox ID="txtAgentName" runat="server" MaxLength="50" Style="width: 220px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="style1">
                        城市级别
                    </td>
                    <td style="text-align: left; padding-left: 5px;" class="style1">
                        <asp:DropDownList ID="ddlCityTier" runat="server">
                            <asp:ListItem Value="">--请选择--</asp:ListItem>
                            <asp:ListItem Value="T1">T1</asp:ListItem>
                            <asp:ListItem Value="T2">T2</asp:ListItem>
                            <asp:ListItem Value="T3">T3</asp:ListItem>
                            <asp:ListItem Value="T4">T4</asp:ListItem>
                            <asp:ListItem Value="T5">T5</asp:ListItem>
                            <asp:ListItem Value="T6">T6</asp:ListItem>
                            <asp:ListItem Value="T7">T7</asp:ListItem>
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                    <td class="style1">
                        安装级别
                    </td>
                    <td style="text-align: left; padding-left: 5px;" class="style1">
                        <asp:RadioButtonList ID="rblIsInstall" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                            <asp:ListItem Value="Y">Y&nbsp;</asp:ListItem>
                            <asp:ListItem Value="N">N</asp:ListItem>
                        </asp:RadioButtonList>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        联系人1
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtContact1" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                    <td style="height: 30px;">
                        联系人电话1
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtTel1" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        联系人2
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtContact2" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                    <td style="height: 30px;">
                        联系人电话2
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtTel2" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        Channel
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtChannel" runat="server" MaxLength="50"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                    <td style="height: 30px;">
                        Format
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtFormat" runat="server" MaxLength="50"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        LocationType
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtLocationType" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                    <td style="height: 30px;">
                        BusinessModel
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtBusinessModel" runat="server" MaxLength="50"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        开店日期
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtOpenDate" runat="server" onclick="WdatePicker()" MaxLength="20"></asp:TextBox>
                    </td>
                    <td style="height: 30px;">
                        客服
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlCSUser" runat="server">
                            <asp:ListItem Value="0">--请选择客服--</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        店铺地址
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtAddress" runat="server" MaxLength="100" Style="width: 465px;"></asp:TextBox>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        特殊安装费
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtBasicInstallPrice" runat="server" MaxLength="10"></asp:TextBox>
                        （特殊店铺的基础安装费）
                    </td>
                </tr>
                <tr>
                    <td style="height: 30px;">
                        备注
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtRemark" runat="server" MaxLength="100" Style="width: 465px;"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="tr" style="margin-top: 20px;">
        >>店铺信息</div>
    </form>
</body>
</html>
