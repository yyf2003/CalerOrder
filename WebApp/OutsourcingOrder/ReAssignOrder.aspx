<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReAssignOrder.aspx.cs"
    Inherits="WebApp.OutsourcingOrder.ReAssignOrder" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            外协特殊订单修改
        </p>
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="table">
                <tr class="tr_bai">
                    <td>
                        客户
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                        </asp:DropDownList>
                        <span style="color: Red;">*</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td style="width: 120px;">
                        活动月份
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onfocus="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                            Style="width: 80px;" OnTextChanged="txtMonth_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <asp:LinkButton ID="lbUp" runat="server" Style="margin-left: 20px; cursor: pointer;
                            color: Blue;" OnClick="lbUp_Click">上一个月</asp:LinkButton>
                        <asp:LinkButton ID="lbDown" runat="server" Style="margin-left: 20px; cursor: pointer;
                            color: Blue;" OnClick="lbDown_Click">下一个月</asp:LinkButton>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        活动名称
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <div id="seleAll" runat="server">
                            <input type="checkbox" id="guianceSeleAll" />全选
                        </div>
                        <asp:CheckBoxList ID="cblGuidanceList" runat="server" CssClass="cbl" CellSpacing="20"
                            RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                            OnSelectedIndexChanged="cblGuidanceList_SelectedIndexChanged">
                        </asp:CheckBoxList>
                        <asp:Panel ID="Panel_EmptyGuidance" runat="server" Visible="false">
                            <span style="color: Red;">无活动信息！</span>
                        </asp:Panel>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        区域
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblRegion" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                            RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="cblRegion_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        省份
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblProvince" runat="server" CssClass="cbl" CellSpacing="20"
                            RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="cblProvince_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        城市
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblCity" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                            RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="cblCity_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        城市级别
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblCityTier" runat="server" CssClass="cbl" CellSpacing="20"
                            RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="cblCityTier_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        SP安装级别
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblSPIsInstall" runat="server" CssClass="cbl" CellSpacing="20"
                            RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="cblSPIsInstall_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        三叶草安装级别
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblBCSIsInstall" runat="server" CssClass="cbl" CellSpacing="20"
                            RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" OnSelectedIndexChanged="cblBCSIsInstall_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        Sheet
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblSheet" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        材质
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblMaterial" runat="server" CssClass="cbl" CellSpacing="20"
                            RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="软膜">软膜&nbsp;</asp:ListItem>
                            <asp:ListItem Value="网格布">网格布&nbsp;</asp:ListItem>
                            <asp:ListItem Value="3mmPVC">PVC板</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        户外安装费
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:CheckBox ID="cbOOHInstallPrice" runat="server" />有OOH安装费
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        店铺编号
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtSearchShopNo" runat="server" MaxLength="50" Style="width: 300px;"></asp:TextBox>
                        (逗号分隔) <span id="spanClearShopNos" style="color: Blue; cursor: pointer;">清空</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                    </td>
                    <td style="text-align: left; padding-left: 5px; height: 40px;">
                        <asp:Button ID="btnResearch" runat="server" Text="查 询" Style="width: 65px; height: 26px;"
                            OnClientClick="return Check()" OnClick="btnResearch_Click" />
                        <img id="imgLoading" style="display: none;" src='../image/WaitImg/loadingA.gif' />
                    </td>
                </tr>
            </table>
            <div style="margin-top: 20px;">
                <table class="table">
                    <tr class="tr_hui">
                        <td style="width: 120px;">
                            店铺数量
                        </td>
                        <td style="text-align: left; padding-left: 5px; width: 100px;">
                            <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                        </td>
                        <td style="width: 120px;">
                            订单数量
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:Label ID="labOrderCount" runat="server" Text="0"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
            <div class="tr">
                >>店铺信息
            </div>
            <div style="margin-bottom: 50px;">
                <asp:Panel ID="Panel1" runat="server">
                    <asp:Repeater ID="Repeater_shopList" runat="server" OnItemDataBound="Repeater_shopList_ItemDataBound">
                        <HeaderTemplate>
                            <table class="table">
                                <tr class="tr_hui">
                                    <td style="width: 30px;">
                                        序号
                                    </td>
                                    <td style="width: 30px;">
                                        <input type="checkbox" id="cbShopAll" />
                                    </td>
                                    <td>
                                        外协名称
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
                                        安装级别
                                    </td>
                                    <td>
                                        店铺类型
                                    </td>
                                    <td>
                                        订单数量
                                    </td>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr class="tr_bai">
                                <td>
                                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.ItemIndex + 1%>
                                </td>
                                <td>
                                    <asp:CheckBox ID="cbOne" runat="server" />
                                    <asp:HiddenField ID="hfShopId" runat="server" Value='<%#Eval("ShopId")%>' />
                                </td>
                                <td>
                                    <%--外协名称--%>
                                    <asp:Label ID="labOutsource" runat="server" Text=""></asp:Label>
                                </td>
                                <td>
                                    <%--店铺编号--%>
                                    <%#Eval("ShopNo")%>
                                </td>
                                <td>
                                    <%--店铺名称--%>
                                    <%#Eval("ShopName")%>
                                </td>
                                <td>
                                    <%--区域--%>
                                    <%#Eval("Region")%>
                                </td>
                                <td>
                                    <%--省份--%>
                                    <%#Eval("Province")%>
                                </td>
                                <td>
                                    <%--城市--%>
                                    <%#Eval("City")%>
                                </td>
                                <td>
                                    <%--城市级别--%>
                                    <%#Eval("CityTier")%>
                                </td>
                                <td>
                                    <%--安装级别--%>
                                    <%#Eval("IsInstall")%>
                                </td>
                                <td>
                                    <%--店铺类型--%>
                                    <%#Eval("Format")%>
                                </td>
                                <td>
                                    <%--订单数量--%>
                                    <span>
                                        <%#Eval("OrderCount")%></span>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <%if (Repeater_shopList.Items.Count == 0)
                              { %>
                            <tr class="tr_bai">
                                <td colspan="12" style="text-align: center;">
                                    --无外协订单--
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
                </asp:Panel>
                <asp:Panel ID="Panel2" runat="server">
                    <table class="table">
                        <tr class="tr_hui">
                            <td style="width: 30px;">
                                序号
                            </td>
                            <td style="width: 30px;">
                                <input type="checkbox" id="cbShopAll" />
                            </td>
                            <td>
                                外协名称
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
                                安装级别
                            </td>
                            <td>
                                店铺类型
                            </td>
                            <td>
                                订单数量
                            </td>
                        </tr>
                        <tr class="tr_bai">
                            <td colspan="12" style="text-align: center;">
                                --无外协订单--
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
            <div class="tab" style="margin-top: 10px; margin-bottom: 50px;">
                <span style="font-weight: bold;">修改类型</span>
                <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
                <div style="font-size: small; margin-top: 10px; margin-bottom: 30px; display:none;">
                    <div style="font-weight: bold;">
                        1、 户外高空安装费变更：</div>
                    <div style="height: 30px; margin-top: 20px;">
                        请选择高空安装外协：
                        <asp:DropDownList ID="ddlType2InstallOutsource" runat="server">
                            <asp:ListItem Value="0">--请选择--</asp:ListItem>
                        </asp:DropDownList>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btnSubmitType2" runat="server" Text="保 存" Style="width: 65px; height: 26px;"
                            OnClientClick="return CheckType2()" OnClick="btnSubmitType2_Click" />
                        <img id="imgType2Waiting" style="display: none;" src='../image/WaitImg/loadingA.gif' />
                        <asp:Label ID="labType2State" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </div>
                </div>
                <div style="font-weight: bold;">
                    1、 雪弗板单独发：</div>
                <div style="margin-top: 10px;">
                    <%--<asp:CheckBox ID="cbCancelPVC" runat="server" />单独发外协雪弗板&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;--%>
                    <table class="table">
                        <tr class="tr_hui">
                            <td>
                                材料名称
                            </td>
                            <td>
                                外协名称
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                总金额
                            </td>
                            <td>
                                操作
                            </td>
                        </tr>
                        <tr class="tr_bai">
                            <td style="height: 40px;">
                                <asp:DropDownList ID="ddlMaterialName" runat="server">
                                    <asp:ListItem Value="3mmpvc">3mmPVC板</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlType1InstallOutsource" runat="server">
                                    <asp:ListItem Value="0">--请选择--</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                                <asp:TextBox ID="txtMaterialNum" runat="server" MaxLength="3" Style="width: 60px;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:TextBox ID="txtMaterialTotalPrice" runat="server" MaxLength="5" Style="width: 120px;
                                    text-align: center;"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Button ID="btnSubmitType1" runat="server" Text="保 存" Style="width: 65px; height: 26px;"
                                    OnClientClick="return CheckType1()" OnClick="btnSubmitType1_Click" />
                                <img id="imgType1Waiting" style="display: none;" src='../image/WaitImg/loadingA.gif' />
                                <asp:Label ID="labType1State" runat="server" Text="" ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <asp:Panel ID="PanelTypeDataList" runat="server" Visible="false">
                        <asp:Repeater ID="Repeater1" runat="server" onitemcommand="Repeater1_ItemCommand" 
                            >
                            <HeaderTemplate>
                                <table class="table">
                                    <tr class="tr_hui">
                                        <td>
                                            序号
                                        </td>
                                        <td>
                                            活动月份
                                        </td>
                                        <td>
                                            外协名称
                                        </td>
                                        <td>
                                            店铺编号
                                        </td>
                                        <td>
                                            店铺名称
                                        </td>
                                        <td>
                                            省份
                                        </td>
                                        <td>
                                            城市
                                        </td>
                                        <td>
                                            材质名称
                                        </td>
                                        <td>
                                            数量
                                        </td>
                                        <td>
                                            费用金额
                                        </td>
                                        <td>
                                            添加时间
                                        </td>
                                        <td>
                                            操作
                                        </td>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr class="tr_bai">
                                    <td>
                                        <%#Container.ItemIndex+1 %>
                                    </td>
                                    <td>
                                       <%#Eval("order.GuidanceYear") + "-" + Eval("order.GuidanceMonth")%>
                                    </td>
                                    <td>
                                        <%#Eval("CompanyName")%>
                                    </td>
                                    <td>
                                        <%#Eval("shop.ShopNo") %>
                                    </td>
                                    <td>
                                        <%#Eval("shop.ShopName")%>
                                    </td>
                                    <td>
                                        <%#Eval("shop.ProvinceName")%>
                                    </td>
                                    <td>
                                        <%#Eval("shop.CityName")%>
                                    </td>
                                    <td>
                                            <%#Eval("order.MaterialName")%>
                                        </td>
                                    <td>
                                        <%#Eval("order.Count")%>
                                    </td>
                                    <td>
                                        <%#Eval("order.TotalPrice")%>
                                    </td>
                                    <td>
                                        <%#Eval("order.AddDate")%>
                                    </td>
                                    <td>
                                        <asp:LinkButton ID="lbDelete" CommandArgument='<%#Eval("order.Id") %>' CommandName="deleteItem" runat="server" ForeColor="Red">删除</asp:LinkButton>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script src="js/reAssignOrder.js" type="text/javascript"></script>
