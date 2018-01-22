<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatisticsByRegion.aspx.cs" Inherits="WebApp.Statistics.StatisticsByRegion" %>

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
</head>
<body>
    <form id="form1" runat="server">
      <table class="table">
         <tr class="tr_bai">
           <td style=" width:100px;">活动名称：</td>
           <td style=" text-align:left; padding-left:5px;">
               <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
           </td>
         </tr>
         <tr class="tr_bai">
            <td>区域：</td>
           <td style=" text-align:left; padding-left:5px;">
           <asp:Label ID="labRegion" runat="server" Text=""></asp:Label>
           </td>
         </tr>
         <tr class="tr_bai">
           <td>项目类型：</td>
           <td style=" text-align:left; padding-left:5px;">
               <asp:CheckBoxList ID="cblSubjectCategory" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
               </asp:CheckBoxList>
               <img id="loadingImg" src="../image/WaitImg/loadingA.gif" style=" margin-left :10px;display:none;"/>
           </td>
         </tr>
      </table>
      <div class="tr" style="margin-top: 10px;">
                    >>统计信息</div>
      <asp:Repeater ID="Repeater1" runat="server" 
                    onitemdatabound="Repeater1_ItemDataBound">
                    <HeaderTemplate>
                        <table class="table">
                            <tr class="tr_hui">
                                <td style="width: 40px;">
                                    序号
                                </td>
                                <td>
                                    项目类型
                                </td>
                                <td>
                                    店铺数量
                                </td>
                                <td>
                                    总面积
                                </td>
                                <td>
                                    POP金额
                                </td>
                                <td>
                                    安装费
                                </td>
                                <td>
                                    快递费
                                </td>
                               
                                <td>
                                    费用合计
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                       <tr class="tr_bai">
                                <td>
                                    <%#Container.ItemIndex+1 %>
                                </td>
                                <td>
                                    <%--<%#Eval("CategoryName")%>--%>
                                    <asp:Label ID="labSubjectCategoryName" runat="server" Text=""></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--总面积--%>
                                    <asp:Label ID="labArea" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--POP金额--%>
                                    <asp:Label ID="labPOPPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--安装费--%>
                                    <asp:Label ID="labInstallPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--快递费--%>
                                    <asp:Label ID="labExpressPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="labSubPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%if (Repeater1.Items.Count == 0)
                          {%>
                        <tr class="tr_bai">
                            <td colspan="7" style="text-align: center;">
                                --无数据--
                            </td>
                        </tr>
                        <%}
                          else
                          { %>
                          <tr class="tr_hui">
                                <td>
                                    
                                </td>
                                <td style=" font-weight">
                                   合计
                                </td>
                                <td>
                                   <%-- <asp:Label ID="labTotalShopCount" runat="server" Text="0"></asp:Label>--%>
                                </td>
                                <td>
                                    <%--总面积--%>
                                    <asp:Label ID="labTotalArea" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--POP金额--%>
                                    <asp:Label ID="labTotalPOPPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--安装费--%>
                                    <asp:Label ID="labTotalInstallPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--快递费--%>
                                    <asp:Label ID="labTotalExpressPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                
                                <td>
                                    <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                        <%} %>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
    </form>
</body>
</html>
