<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FinishAssignOrderList.aspx.cs"
    Inherits="WebApp.OutsourcingOrder.FinishAssignOrderList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <table class="layui-table">
       <tr>
          <th style=" width:60px; ">店铺编号</th>
          <td style=" width:100px; padding-left:5px; text-align:left;">
              <asp:Label ID="labShopNo" runat="server" Text=""></asp:Label>
          </td>
          <th style=" width:60px; ">店铺名称</th>
          <td style="padding-left:5px; text-align:left;">
              <asp:Label ID="labShopName" runat="server" Text=""></asp:Label>
          </td>
       </tr>
    </table>
    <div>
        <asp:Repeater ID="rp_orderList" runat="server" OnItemDataBound="rp_orderList_ItemDataBound">
            <HeaderTemplate>
                <table id="tbPOP" class="layui-table">
                    <tr>
                        <th style="width: 30px;">
                            序号
                        </th>
                        <th>
                            外协
                        </th>
                        <th>
                            订单类型
                        </th>
                        <th>
                            Sheet
                        </th>
                        <th>
                            POP编号
                        </th>
                        <th>
                            POP数量
                        </th>
                        <th>
                            费用金额
                        </th>
                        <th>
                            男/女
                        </th>
                        <th>
                            POP宽
                        </th>
                        <th>
                            POP高
                        </th>
                        <th>
                            POP材质
                        </th>
                        <th>
                            位置描述
                        </th>
                        <th>
                            选图
                        </th>
                        <th>
                            备注
                        </th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%#Container.ItemIndex+1 %>
                    </td>
                     <td>
                        <asp:Label ID="labOutsoure" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labOrderType" runat="server" Text=""></asp:Label>
                    </td>
                   
                    <td>
                        <%#Eval("order.Sheet")%>
                    </td>
                    <td>
                        <%#Eval("order.GraphicNo")%>
                    </td>
                    <td>
                        <%#Eval("order.Quantity")%>
                    </td>
                    <td>
                        <%#Eval("order.PayOrderPrice")%>
                    </td>
                    <td>
                       <%#(Eval("order.OrderGender") != null && Eval("order.OrderGender").ToString() != "") ? Eval("order.OrderGender") : Eval("order.Gender")%>
                            
                    </td>
                    <td>
                        <%#Eval("order.GraphicWidth")%>
                    </td>
                    <td>
                        <%#Eval("order.GraphicLength")%>
                    </td>
                    <td>
                        <%#Eval("order.GraphicMaterial")%>
                    </td>
                    <td>
                        <%#Eval("order.PositionDescription")%>
                    </td>
                    <td>
                        <%#Eval("order.ChooseImg")%>
                    </td>
                    <td>
                        <%#Eval("order.Remark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (rp_orderList.Items.Count == 0)
                  { %>
                <tr>
                    <td colspan="13" style="text-align: center;">
                        无数据显示
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
