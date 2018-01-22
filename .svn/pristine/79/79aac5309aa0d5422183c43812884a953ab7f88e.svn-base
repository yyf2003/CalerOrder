<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuotationDetail.aspx.cs"
    Inherits="WebApp.Quotation.QuotationDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../easyui1.4/jquery.min.js"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            查看报价信息
        </p>
    </div>
    <div class="tr">
        >>费用信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                类别
            </td>
            <td style="width: 260px; text-align: left; padding-left: 5px;">
                <asp:Label ID="labCategory" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                AD费用归属
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labBelongs" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                分类
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labClassification" runat="server" Text=""></asp:Label>
            </td>
            <td>
                Adidas负责人
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAdidasContact" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                税率
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labTaxRate" runat="server" Text=""></asp:Label>
            </td>
            <td>
                报价账户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAccount" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                报价金额
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labOfferPrice" runat="server" Text=""></asp:Label>
            </td>
            <td>
                挂账金额
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labOtherPrice" runat="server" Text=""></asp:Label>
            </td>
        </tr>
         <tr class="tr_bai">
            <td>
                挂账说明
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labOtherPriceRemark" runat="server" Text=""></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td>
                备注
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
            </td>
           
        </tr>
    </table>
    <br />
    <div class="tr">
        >>分摊金额
    </div>
    <asp:Repeater ID="gvShareList" runat="server" OnItemDataBound="gvList_ItemDataBound">
        <HeaderTemplate>
            <table class="table1" id="gvShareList">
                <tr class="tr_hui">
                    <td style="width: 150px;">
                        费用名称
                    </td>
                    <td style="width: 130px;">
                        费用金额
                    </td>
                    <td style="width: 100px;">
                        合计
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td>
                    <%#Eval("PriceName")%>
                </td>
                <td>
                    <%#Eval("Price")%>
                </td>
                <td runat="server" id="shareTotalTd">
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (gvShareList.Items.Count == 0)
              { %>
            <tr>
                <td colspan="3" style="text-align: center; height: 30px;">
                    --无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <br />
    <div class="tr">
        >>并入金额
    </div>
    <asp:Repeater ID="gvBingList" runat="server" OnItemDataBound="gvBingList_ItemDataBound">
        <HeaderTemplate>
            <table class="table1" id="gvShareList">
                <tr class="tr_hui">
                    <td style="width: 150px;">
                        费用名称
                    </td>
                    <td style="width: 130px;">
                        费用金额
                    </td>
                    <td style="width: 100px;">
                        合计
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td>
                    <%#Eval("PriceName")%>
                </td>
                <td>
                    <%#Eval("Price")%>
                </td>
                <td runat="server" id="bingTotalTd">
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (gvBingList.Items.Count == 0)
              { %>
            <tr>
                <td colspan="3" style="text-align: center; height: 30px;">
                    --无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    </form>
</body>
</html>
