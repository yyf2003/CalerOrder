<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubjectStatistics.aspx.cs"
    Inherits="WebApp.Statistics.SubjectStatistics" %>

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
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var subjectId = '<%=subjectId %>';
        var isSearch = '<%=isSearch %>';
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            项目统计—按店统计
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table class="table">
                    <tr class="tr_bai">
                        <td style="width: 100px;">
                            区域
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:CheckBoxList ID="cblRegion" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"
                                RepeatLayout="Flow" OnSelectedIndexChanged="cblRegion_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            省份
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadProvince" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblProvince" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8" OnSelectedIndexChanged="cblProvince_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            城市
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadCity" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblCity" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="8">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style="text-align: right; padding-right: 25px; margin-top: 8px;">
            <asp:Button ID="Button1" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                height: 26px;" OnClick="Button1_Click" OnClientClick="return loading()" />
            <img id="loadingImg" style="display: none;" src="../image/WaitImg/loadingA.gif" />
        </div>
    </div>
    <br />
    <div style="margin: 0px;">
        <table class="table">
            <tr class="tr_hui">
                <td>
                    项目名称：
                </td>
                <td colspan="7" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_hui">
                <td style="text-align: center; width: 100px;">
                    店铺数量：
                </td>
                <td style="text-align: left; width: 120px; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labShopCount" runat="server" Text=""></asp:Label>
                </td>
                <td style="text-align: center; width: 100px;">
                    面积合计：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 160px;">
                    <asp:Label ID="labArea" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 100px;">
                    POP金额合计：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 160px;">
                    <asp:Label ID="labPOPPrice" runat="server" Text="0"></asp:Label>
                </td>
                 <td style="text-align: center; width: 100px;">
                    物料费用合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labMaterialPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
          
        </table>
    </div>
    <div class="tr">
        >>信息列表
    </div>
    <asp:Repeater ID="gvList" runat="server" OnItemDataBound="gvList_ItemDataBound" OnItemCommand="gvList_ItemCommand">
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
                        城市级别
                    </td>
                    <td>
                        POP总面积(平米)
                    </td>
                    <td>
                        POP金额(元)
                    </td>
                    <td>
                        安装费(元)
                    </td>
                     <td>
                        测量费(元)
                    </td>
                     <td>
                        物料费用(元)
                    </td>
                    <td>
                        其他费用(元)
                    </td>
                    <td>
                        订单明细
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
                    <%#Eval("CityTier")%>
                </td>
                <td>
                    <%--POP平米数--%>
                    <asp:Label ID="labArea" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <%--POP金额
                    <a href="javascript:void(0);" onclick="CheckMaterialPrice(<%#Eval("Id") %>)" style="text-decoration: underline;">
                        
                    </a>--%>
                    <asp:Label ID="labPOPPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <%--安装费--%>
                    <asp:Label ID="labInstallPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <%--测量费--%>
                    <asp:Label ID="labMeasurePrice" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <%--物料费用--%>
                    <asp:Label ID="labMaterialPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                    <%--其他费用--%>
                    <asp:Label ID="labOtherPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td>
                   <asp:LinkButton ID="lbCheck" runat="server" CommandArgument='<%#Eval("Id") %>' CommandName="Check">查看</asp:LinkButton>
                   <%-- <span name="spanCheckDetail" data-subjectid='<%#Eval("Id") %>' data-subjecttype='<%#Eval("SubjectType") %>' style=" color:Blue; cursor:pointer;">查看</span>--%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (gvList.Items.Count == 0)
              {%>
            <tr class="tr_bai">
                <td colspan="14" style="text-align: center;">
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
    <div style="text-align: center; margin-top: 20px; margin-bottom: 20px; display: none;">
        <input type="button" value="返 回" onclick="javascript:window.history.go(-1);" class="easyui-linkbutton"
            style="width: 65px; height: 26px;" />
    </div>
    </form>
</body>
</html>
<script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="/fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="/fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script type="text/javascript">
    var $j = jQuery.noConflict();
    function CheckMaterialPrice(shopId) {

        var url = "MaterialStatistics.aspx?subjectId=" + subjectId + "&shopId=" + shopId;
        $j.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: 700,
            //modal:true,
            hideOnOverlayClick: false


        })
    }

    $(function () {
        $("span[name='checkMaterial']").click(function () {
            var region = $(this).data("region");
            var province = $(this).data("province");
            var city = $(this).data("city");
            var subjectChannel = $(this).data("subjectchannel");
            var shopType = $(this).data("shoptype");
            var url = "MaterialStatistics.aspx?subjectId=" + subjectId + "&regions=" + region + "&provinces=" + province + "&citys=" + city + "&isSearch=1&subjectChannel=" + subjectChannel + "&shopType=" + shopType;
            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: 700,
                //modal:true,
                hideOnOverlayClick: false
            })
        })
    })
</script>
