<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOrderSuccess.aspx.cs"
    Inherits="WebApp.Subjects.ADOrders.CheckOrderSuccess" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
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
  
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            核对订单
        </p>
    </div>
    <div style="color: Red; font-size: 20px; font-weight: bold; margin-left: 10px;">
        订单核对成功！
    </div>
    <br />
    <div class="tr">
        >>核单信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
            </td>
        </tr>
       <tr class="tr_bai">
            <td>
                起止时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                核单时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <div class="tab" style="margin-top: 10px;">
        核单结果 &nbsp;&nbsp;<asp:LinkButton ID="lbExport" runat="server" 
            onclick="lbExport_Click">导 出</asp:LinkButton>
        <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
    </div>
    <div style="overflow: auto;">
    <asp:Repeater ID="gvList" runat="server">
        <HeaderTemplate>
            <table class="table1" style="width: 1500px;">
                <tr class="tr_hui">
                    <td style="width: 40px;">
                        序号
                    </td>
                    <td>
                        项目名称
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
                        城市级别
                    </td>
                    <td>
                        是否安装
                    </td>
                    <td>
                        店铺类型
                    </td>
                    <td>
                        物料支持
                    </td>
                    <td>
                        店铺规模大小
                    </td>
                    <td>
                        位置
                    </td>
                    <td>
                        性别
                    </td>
                    <td>
                        系统数量
                    </td>
                    <td>
                        订单数量
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td style="width: 40px;">
                    <%#Container.ItemIndex + 1%>
                </td>
                <td>
                    <%#Eval("SubjectName")%>
                </td>
                <td>
                    <%#Eval("RegionNames")%>
                </td>
                <td>
                    <%#Eval("ProvinceId")%>
                </td>
                <td>
                    <%#Eval("CityId")%>
                </td>
                <td>
                    <%#Eval("CityTier")%>
                </td>
                <td>
                    <%#Eval("IsInstall")%>
                </td>
                <td>
                    <%#Eval("Format")%>
                </td>
                <td>
                    <%#Eval("MaterialSupport")%>
                </td>
                <td>
                    <%#Eval("POSScale")%>
                </td>
                <td>
                    <%#Eval("PositionName")%>
                </td>
                <td>
                    <%#Eval("Gender")%>
                </td>
                <td>
                    <%#Eval("BasicPositionCount")%>
                </td>
                <td>
                    <%#Eval("OrderPositionCount")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    </div>
    <br />
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <br />
    <div style=" text-align:center;">
      <input type="button" value="返 回" id="btnGoBack" onclick="window.history.go(-1)" class="easyui-linkbutton" style="width: 65px;
            height: 26px;"/>
    </div>
    <br />
    <br />

    </form>
</body>
</html>
