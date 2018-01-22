<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderPrice.aspx.cs" Inherits="WebApp.Quotation.OrderPrice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <asp:Repeater ID="gvPrice" runat="server" 
                onitemdatabound="gvPrice_ItemDataBound">
                <HeaderTemplate>
                    <table class="table" >
                        <tr class="tr_hui">
                            <td style="width: 40px;">
                                序号
                            </td>
                            <td>
                                材质名称
                            </td>
                            <td>
                                单价
                            </td>
                            <td>
                                总面积
                            </td>
                            <td>
                                金额
                            </td>
                            
                            
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="tr_bai">
                        <td style="width: 40px;">
                            <%#Container.ItemIndex + 1%>
                        </td>
                        <td>
                            <%#Eval("GraphicMaterial")%>
                        </td>
                        <td>
                            <%#Eval("UnitPrice")%>
                        </td>
                        <td>
                            <%#Eval("Area")%>
                        </td>
                        <td>
                            <%--<%#Eval("Price")%>--%>
                            <asp:Label ID="labPrice" runat="server" Text=""></asp:Label>
                        </td>
                        
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%if (gvPrice.Items.Count == 0)
                      { %>
                      <tr class="tr_bai">
                        <td colspan="5" style=" text-align:center;">--无数据--</td>
                      </tr>
                    <%}
                      else
                      {%>
                        <tr class="tr_bai">
                        <td>
                           
                        </td>
                        <td>
                           
                        </td>
                        <td>
                           
                        </td>
                        <td>
                           合  计
                        </td>
                        <td>
                            <asp:Label ID="labTotalPrice" runat="server" Text=""></asp:Label>
                        </td>
                        
                    </tr>
                    <%} %>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
     
    </div>
    </form>
</body>
</html>
