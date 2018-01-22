<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderList.aspx.cs" Inherits="WebApp.Subjects.RegionApprove.OrderList" %>

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
     <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a>
        <p class="nav_table_p">
            订单核对信息
        </p>
    </div>
    <table class="table">
            <tr class="tr_bai">
                <td style="text-align: right; width: 120px;">
                    活动名称：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 300px;">
                    <asp:Label ID="labItemName" runat="server" Text=""></asp:Label>
                </td>
                <td style="width: 120px;">
                   活动类型：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:Label ID="labActivityType" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    分区客服：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labCSName" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    店铺范围：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labShopArea" runat="server" Text=""></asp:Label>
                </td>
            </tr>
           
            </table>

    <blockquote class="layui-elem-quote" style=" height:15px;font-weight:bold; margin-top :15px; padding-top:8px; padding-left :5px;">
        项目信息</blockquote>
    <div style=" margin-top :-10px;">
        <asp:Repeater ID="Repeater1" runat="server">
           <HeaderTemplate>
              <table class="table">
                 <tr class="tr_hui">
                    <td style=" width:40px;">序号</td>
                    <td>店铺编号</td>
                    <td>店铺名称</td>
                    <td>区域</td>
                    <td>省份</td>
                    <td>城市</td>
                    <td>项目名称</td>
                    <td>Sheet</td>
                    <td>器架类型</td>
                    <td>男/女</td>
                    <td>数量</td>
                    <td>POP宽</td>
                    <td>POP高</td>
                    <td>材质</td>
                    <td>位置描述</td>
                    <td>选图</td>
                    <td>备注</td>
                    
                 </tr>
             
           </HeaderTemplate>
           <ItemTemplate>
              <tr class="tr_bai">
                    <td>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.ItemIndex + 1%>
                    </td>
                    <td><%#Eval("order.ShopNo")%></td>
                    <td><%#Eval("order.ShopName")%></td>
                    <td><%#Eval("order.Region")%></td>
                    <td><%#Eval("order.Province")%></td>
                    <td><%#Eval("order.City")%></td>
                    <td><%#Eval("subject.SubjectName")%></td>
                    <td><%#Eval("order.Sheet")%></td>
                    <td><%#Eval("order.MachineFrame")%></td>
                    <td><%#Eval("order.Gender")%></td>
                    <td><%#Eval("order.Quantity")%></td>
                    <td><%#Eval("order.GraphicWidth")%></td>
                    <td><%#Eval("order.GraphicLength")%></td>
                    <td><%#Eval("order.GraphicMaterial")%></td>
                    <td><%#Eval("order.PositionDescription")%></td>
                    <td><%#Eval("order.ChooseImg")%></td>
                    <td><%#Eval("order.Remark")%></td>
                    
                 </tr>
           </ItemTemplate>
           <FooterTemplate>
              <%if (Repeater1.Items.Count == 0)
                { %>
                 <tr>
                   <td colspan="17" style=" text-align:center;">
                       --无数据--
                   </td>
                 </tr>
              <%} %>
               </table>
           </FooterTemplate>
        </asp:Repeater>
    </div>
    <div style="text-align:center; margin-top :10px;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
