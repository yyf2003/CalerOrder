<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckDetail.aspx.cs" Inherits="WebApp.Subjects.Supplement.CheckDetail" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

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
    .table1
    {
        border-collapse: collapse;
        text-align: center;
        font-size: 12px;
        width:100%;
    }
    .table1 tr
    {
        height: 30px;
    }
    .table1 td
    {
        border: #dce0e9 solid 1px;
    }
    .center1
    {
        text-align: center;
    }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:Panel ID="PanelTitle" runat="server" Visible="false">
       <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            补单明细
        </p>
    </div>
    </asp:Panel>


    <div class="tr">
        >>补单项目信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                补单金额
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labPrice" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        
        <tr class="tr_bai">
            <td class="style1">
                提交人
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labUserName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                提交时间
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labAddDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <div class="tr">
        >>订单列表
    </div>
<%--    <div class="tab-content" style="height: 360px; overflow: auto;">
        <div>
          </div>
    </div>--%>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
      <ContentTemplate>
      
     
            <asp:Repeater ID="gvPOP" runat="server">
                <HeaderTemplate>
                    <table class="table" >
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
                                城市级别
                            </td>
                            <td>
                                位置
                            </td>
                            <td>
                                性别
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                POP宽(mm)
                            </td>
                            <td>
                                POP高(mm)
                            </td>
                            <td>
                                面积(M2)
                            </td>
                            <td>
                                POP材质
                            </td>
                            <td>
                                单价
                            </td>
                            <td>
                                选图
                            </td>
                            <td>
                                位置描述
                            </td>
                            
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="tr_bai">
                        <td style="width: 40px;">
                            <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                        </td>
                        <td>
                            <%#Eval("shop.ShopNo")%>
                        </td>
                        <td>
                            <%#Eval("shop.ShopName")%>
                        </td>
                        <td>
                            <%#Eval("shop.RegionName")%>
                        </td>
                        <td>
                            <%#Eval("shop.ProvinceName")%>
                        </td>
                        <td>
                            <%#Eval("shop.CityName")%>
                        </td>
                        <td>
                            <%#Eval("shop.CityTier")%>
                        </td>
                        <td>
                            <%#Eval("order.Sheet")%>
                        </td>
                        <td>
                            <%#Eval("order.Gender")%>
                        </td>
                        <td>
                            <%#Eval("order.Quantity")%>
                        </td>
                        <td>
                            <%#Eval("order.GraphicWidth")%>
                        </td>
                        <td>
                            <%#Eval("order.GraphicLength")%>
                        </td>
                        <td>
                            <%#Eval("order.Area")%>
                        </td>
                        <td>
                            <%#Eval("order.GraphicMaterial")%>
                        </td>
                        <td>
                            <%#Eval("order.UnitPrice")%>
                        </td>
                        <td>
                            <%#Eval("order.ChooseImg")%>
                        </td>
                        <td>
                            <%#Eval("order.PositionDescription")%>
                        </td>
                        
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <div style="text-align: center;">
                <webdiyer:AspNetPager ID="AspNetPager1" runat="server" pagesize="10" cssclass="paginator"
                    currentpagebuttonclass="cpb" alwaysshow="True" firstpagetext="首页" lastpagetext="尾页"
                    nextpagetext="下一页" prevpagetext="上一页" showcustominfosection="Left" showinputbox="Never"
                    custominfotextalign="Left" layouttype="Table" onpagechanged="AspNetPager1_PageChanged">
                </webdiyer:AspNetPager>
            </div>
 </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Panel ID="Panel1" runat="server" Visible="false">
       <br />
    <div class="tr">
        >>审批信息
    </div>
    <table class="table">
      <tr class="tr_bai">
        <td style=" width:120px;">审批结果</td>
        <td style=" text-align:left; padding-left:5px;">
            <asp:Label ID="labApproveResult" runat="server" Text=""></asp:Label>
        </td>
      </tr>
      <tr class="tr_bai">
        <td>审批意见</td>
        <td style=" text-align:left; padding-left:5px;">
           <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
        </td>
      </tr>
      <tr class="tr_bai">
        <td>审批人</td>
        <td style=" text-align:left; padding-left:5px;">
           <asp:Label ID="labApproveUserName" runat="server" Text=""></asp:Label>
        </td>
      </tr>
      <tr class="tr_bai">
        <td>审批时间</td>
        <td style=" text-align:left; padding-left:5px;">
           <asp:Label ID="labApproveDate" runat="server" Text=""></asp:Label>
        </td>
      </tr>
     
    </table>
    </asp:Panel>
    <asp:Panel ID="PanelBackButton" runat="server" Visible="false">
       
       <div style="text-align: center; margin-top:20px; margin-bottom:20px;">
           <%--<asp:Button ID="btnGoBack" runat="server" Text="返 回"  class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" onclick="btnGoBack_Click"/>--%>
           <input type="button" value="返 回" class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" onclick="javascript:window.history.go(-1)" />
        </div>
    </asp:Panel>
    
    </form>
</body>
</html>
