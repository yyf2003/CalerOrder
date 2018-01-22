<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckNotAssignOrder.aspx.cs" Inherits="WebApp.OutsourcingOrder.CheckNotAssignOrder" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

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
              <asp:TextBox ID="txtShopNo" runat="server" MaxLength="20" style=" height:23px;"></asp:TextBox>
          </td>
          <th style=" width:60px; ">店铺名称</th>
          <td style="padding-left:5px; text-align:left;">
              <asp:TextBox ID="txtShopName" runat="server" MaxLength="30" style=" height:23px;"></asp:TextBox>
              &nbsp;
              &nbsp;
              &nbsp;
              &nbsp;
              &nbsp;
              &nbsp;
              <asp:Button ID="btnSearch" runat="server" Text="查 询" 
                  class="layui-btn layui-btn-small" onclick="btnSearch_Click" OnClientClick="return check()"/>
                  <img id="imgLoading" style="display: none;" src='../image/WaitImg/loadingA.gif' />
          </td>
       </tr>
    </table>
    <div>
        <asp:Repeater ID="rp_orderList" runat="server" 
            OnItemDataBound="rp_orderList_ItemDataBound" 
            >
            <HeaderTemplate>
                <table id="tbPOP" class="layui-table">
                    <tr>
                        <th style="width: 30px;">
                            序号
                        </th>
                        <th>
                            项目名称
                        </th>
                        <th>
                            订单类型
                        </th>
                        <th>
                            店铺编号
                        </th>
                        <th>
                            店铺名称
                        </th>
                        <th>
                            省份
                        </th>
                        <th>
                            店铺类型
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
                       <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.ItemIndex + 1%>
                    </td>
                     <td>
                       <%#Eval("subject.SubjectName")%>
                    </td>
                    <td>
                        <asp:Label ID="labOrderType" runat="server" Text=""></asp:Label>
                    </td>
                   <td>
                        <%#Eval("order.ShopNo")%>
                    </td>
                    <td>
                        <%#Eval("order.ShopName")%>
                    </td>
                    <td>
                        <%#Eval("order.Province")%>
                    </td>
                    <td>
                        <%#Eval("order.Format")%>
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
                    <td colspan="16" style="text-align: center;">
                        无数据显示
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        
    </div>
    <div style=" text-align:center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function check() {
        $("#imgLoading").show();
        return true;
    }
</script>