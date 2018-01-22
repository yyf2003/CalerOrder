﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckDetail.aspx.cs" Inherits="WebApp.Subjects.InstallManagement.CheckDetail" %>

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
    <script type="text/javascript">
        var subjectId = '<%=guidanceId %>';
        var secItemId = '<%=shopId %>';
        var fileType = '<%=FileType %>';
        var fileCode = '<%=FileCode %>';
    </script>
</head>
<body>
    <form id="form1" runat="server">
     <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            POP安装信息
        </p>
    </div>
    <div class="tr">
        >>店铺信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 100px;">
                店铺编号
            </td>
            <td style="width: 200px; text-align: left; padding-left: 5px;">
                <asp:Label ID="labShopNo" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 100px;">
                店铺名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labShopName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                区域
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRegion" runat="server" Text=""></asp:Label>
            </td>
            <td>
                省份
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labProvince" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                城市
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCity" runat="server" Text=""></asp:Label>
            </td>
            <td>
                城市级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCityTier" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                安装级别
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labIsInstall" runat="server" Text=""></asp:Label>
            </td>
            <td>
                地址
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddress" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <div class="tr" style="margin-top: 10px;">
        >>订单信息</div>
    <asp:Repeater ID="rep_OrderList" runat="server" 
        onitemdatabound="rep_OrderList_ItemDataBound">
        <HeaderTemplate>
            <table class="table">
                <tr class="tr_hui">
                    <td style=" width:40px;">
                        序号
                    </td>
                    <td style=" width:60px;">
                        订单类型
                    </td>
                    <td>
                        位置
                    </td>
                    <td>
                        位置描述
                    </td>
                    <td>
                        选图
                    </td>
                    <td>
                        级别
                    </td>
                    <td>
                        性别
                    </td>
                    <td>
                        数量
                    </td>
                    <td>
                        POP宽
                    </td>
                    <td>
                        POP高
                    </td>
                    <td>
                        面积
                    </td>
                    <td>
                        材质
                    </td>
                    <td>
                        备注
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td>
                    <%#Container.ItemIndex+1 %>
                </td>
                <td>
                    <asp:Label ID="labOrderType" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <%#Eval("Sheet") %>
                </td>
                <td>
                    <%#Eval("PositionDescription")%>
                </td>
                <td>
                    <%#Eval("ChooseImg")%>
                </td>
                <td>
                    <%#Eval("LevelNum")%>
                </td>
                <td>
                    <%#Eval("Gender")%>
                </td>
                <td>
                    <%#Eval("Quantity")%>
                </td>
                <td>
                    <%#Eval("GraphicWidth")%>
                </td>
                <td>
                   <%#Eval("GraphicLength")%>
                </td>
                <td>
                    <%#Eval("Area")%>
                </td>
                <td>
                    <%#Eval("GraphicMaterial")%>
                </td>
                <td>
                    <%#Eval("Remark")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (rep_OrderList.Items.Count == 0)
              {%>
               <tr class="tr_bai">
                <td colspan="15" style=" text-align:center;">
                    --暂无数据--
                </td>
                </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <br />
    <div class="tr" style="margin-top: 10px;">
        >>安装信息</div>
     <table class="table">
        <tr class="tr_bai">
           <td style=" width:100px;">
             安装时间
           </td>
           <td style=" text-align:left; padding-left:5px;">
               <asp:Label ID="labInstallDate" runat="server" Text=""></asp:Label>
           </td>
        </tr>
        <tr class="tr_bai">
           <td>
             完工时间
           </td>
           <td style=" text-align:left; padding-left:5px;">
              <asp:Label ID="labFinishDate" runat="server" Text=""></asp:Label>
           </td>
        </tr>
        <tr class="tr_bai">
           <td>
             安装人员
           </td>
           <td style=" text-align:left; padding-left:5px;">
              <asp:Label ID="labInstallUserName" runat="server" Text=""></asp:Label>
           </td>
        </tr>
        <tr class="tr_bai">
           <td>
             默认安装费用
           </td>
           <td style=" text-align:left; padding-left:5px;">
              <asp:Label ID="labDefaultPrice" runat="server" Text=""></asp:Label>
           </td>
        </tr>
        <tr class="tr_bai">
           <td>
             其他费用
           </td>
           <td style=" text-align:left; padding-left:5px;">
              <asp:Label ID="labOtherPrice" runat="server" Text=""></asp:Label>
           </td>
        </tr>
        <tr class="tr_bai">
           <td>
             其他费用说明
           </td>
           <td style=" text-align:left; padding-left:5px;">
              <asp:Label ID="labOtherPriceRemark" runat="server" Text=""></asp:Label>
           </td>
        </tr>
        <tr class="tr_bai">
           <td>
             安装照片
           </td>
           <td style=" text-align:left; padding-left:5px;">
               <div id="InstallFiles"></div>
           </td>
        </tr>
        <tr class="tr_bai">
           <td>
             备注
           </td>
           <td style=" text-align:left; padding:5px;">
              <asp:Label ID="labInstallRemark" runat="server" Text=""></asp:Label>
           </td>
        </tr>
     </table>
    
     <div style="text-align: center; height: 35px; margin-top:20px; margin-bottom:20px;">
            <asp:Button ID="Button1" runat="server" Text="返 回" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="Button1_Click"/>
           
    </div>
    </form>
</body>
</html>
<script src="/Common/Js/GetFiles.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        GetFiles(subjectId, secItemId, fileCode, fileType, $("#InstallFiles"));

    })
   
</script>