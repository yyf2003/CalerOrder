<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckShops.aspx.cs" Inherits="WebApp.Statistics.CheckShops" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
   
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
   
</head>
<body>
    <form id="form1" runat="server">
    <div class="tr">
        >>搜索</div>
    <table class="table">
        <tr class="tr_bai">
           
        <tr class="tr_bai">
            <td style="width: 100px;">
               店铺编号
            </td>
            <td style="text-align: left; padding-left: 5px; width:300px;">
                <asp:TextBox ID="txtShopNo" runat="server"></asp:TextBox>
            </td>
           <td style="width: 100px;">
               安装级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:CheckBoxList ID="cblIsInstall" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                </asp:CheckBoxList>
            </td>
        </tr>
    </table>
    <div style="text-align: right; padding-right: 25px; margin-top: 8px;">
            <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                height: 26px;"   
                onclick="btnSearch_Click"/>
                <img id="loadingImg" style=" display:none;" src="/image/WaitImg/loadingA.gif" />
                &nbsp;&nbsp;
          
           <asp:Button ID="btnExport" runat="server" Text="导 出"  class="easyui-linkbutton" Style="width: 65px;
                 height: 26px;" onclick="btnExport_Click" />
             <img id="exportWaiting" style=" display:none;" src="/image/WaitImg/loadingA.gif" />
        </div>
    <br />
    <table style=" font-size:12px;">
      <tr class="tr_bai">
         <td style=" width:70px;">店铺数量：</td>
         <td style=" width:200px; text-align:left; padding-left:5px;">
             <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
         </td>
      </tr>
    </table>
    <div class="tr">
        >>店铺信息
    </div>
    
    <asp:Repeater ID="gvList" runat="server">
        <HeaderTemplate>
            
            <table class="table1" style="width: 100%;">
                <tr class="tr_hui">
                    <td style="width: 40px;">
                        序号
                    </td>
                    <td>
                        店铺编号
                    </td>
                    <td>
                        店铺名称
                    </td>
                    <td>
                        区域
                    </td>
                    <td>
                        省份
                    </td>
                    <td>
                        城市
                    </td>
                    <td>
                        店铺地址
                    </td>
                    <td>
                        城市级别
                    </td>
                    <td>
                        是否安装
                    </td>
                    <td>
                        Channel
                    </td>
                    <td>
                        Format
                    </td>
                    <td>
                        店铺规模大小
                    </td>
                    <td>
                        物料支持级别
                    </td>
                    <td>
                        店铺状态
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td style="width: 40px;">
                    <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                </td>
                <td>
                    <%#Eval("ShopNo") %>
                </td>
                <td>
                    <%#Eval("ShopName") %>
                </td>
                <td>
                    <%#Eval("RegionName")%>
                </td>
                <td>
                    <%#Eval("ProvinceName")%>
                </td>
                <td>
                    <%#Eval("CityName")%>
                </td>
                <td>
                    <%#Eval("POPAddress")%>
                </td>
                <td>
                    <%#Eval("CityTier")%>
                </td>
                <td>
                    <%#Eval("IsInstall")%>
                </td>
                <td>
                        <%#Eval("Channel")%>
                    </td>
                    <td>
                        <%#Eval("Format")%>
                    </td>
                <td>
                     <%#Eval("POSScale")%>
                </td>
                <td>
                     <%#Eval("MaterialSupport")%>
                </td>
               <td>
                     <%#Eval("Status")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (gvList.Items.Count == 0)
              {%>
            <tr class="tr_bai">
                <td colspan="15" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
