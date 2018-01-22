<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderExport.aspx.cs" Inherits="WebApp.Subjects.OrderExport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <style type="text/css">
        .table1 td
        {
            height: 10px;
        }
        .tbBold
        {
            font-weight: bold;
        }
        .tdContent
        {
            text-align: left;
            line-height: 25px;
            padding-left: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                订单导出
            </p>
    </div>
    <blockquote class="layui-elem-quote" style="height: 15px; padding-top: 7px; font-weight: bold;">
        选择项目
    </blockquote>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="layui-table table1" style="margin-top: -10px;">
                <tr>
                    <td class="tbBold" style="width: 100px;">
                        客户
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlCustomer" runat="server" 
                            onselectedindexchanged="ddlCustomer_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="tbBold">
                        活动时间
                    </td>
                    <td style="text-align: left; padding: 0px;">
                        <div style="padding-top: 5px; padding-bottom: 5px;">
                            <asp:TextBox ID="txtGuidanceBegin" runat="server" onclick="WdatePicker()" CssClass="Wdate" style=" width:100px;"></asp:TextBox>
                            —
                            <asp:TextBox ID="txtGuidanceEnd" runat="server" onclick="WdatePicker()" CssClass="Wdate" style=" width:100px;"></asp:TextBox>
                            &nbsp;&nbsp;
                            <asp:Button ID="btnGetGuidance" runat="server" Text="查  询" class="layui-btn layui-btn-small" style=" height:25px; padding-top :0px;"
                                OnClick="btnGetGuidance_Click" OnClientClick="return CheckGuidanceDate()" />
                        </div>
                        <div>
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 80px; text-align: center;">
                                        按时间显示：
                                    </td>
                                    <td style="width: 150px; text-align: left;">
                                        <asp:Label ID="labBeginDate" runat="server" Text=""></asp:Label>
                                        <asp:Label ID="labSeparator" runat="server" Text="—" Visible="false"></asp:Label>
                                        <asp:Label ID="labEndDate" runat="server" Text=""></asp:Label>
                                    </td>
                                    <td style="width: 100px; text-align: center;">
                                        按活动月显示：
                                    </td>
                                    <td style="text-align: left; padding-left: 8px;">
                                        <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                                            Style="width: 80px;" OnTextChanged="txtGuidanceMonth_TextChanged" AutoPostBack="true"></asp:TextBox>
                                        <asp:LinkButton ID="lbUp" runat="server" Style="margin-left: 20px; cursor: pointer;
                                            color: Blue;" OnClick="lbUp_Click">上一个月</asp:LinkButton>
                                        <asp:LinkButton ID="lbDown" runat="server" Style="margin-left: 20px; cursor: pointer;
                                            color: Blue;" OnClick="lbDown_Click">下一个月</asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="tbBold">
                        项目
                    </td>
                    <td style="padding: 0px;">
                        <table style="width: 100%;">
                            <tr>
                                <td style="width: 80px;">
                                    活动名称：
                                </td>
                                <td style="text-align: left; padding-left: 10px;">
                                    <div id="loadGuidanceImg" style="display: none;">
                                        <img src="../image/WaitImg/loadingA.gif" />
                                    </div>
                                    <asp:CheckBoxList ID="cblGuidanceList" runat="server" RepeatDirection="Horizontal"
                                        RepeatLayout="Flow" RepeatColumns="6" AutoPostBack="true" OnSelectedIndexChanged="cblGuidanceList_SelectedIndexChanged">
                                    </asp:CheckBoxList>
                                    <asp:Label ID="labEmptyGuidance" Visible="false" runat="server" Text="无活动信息" Style="color: Red;"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    活动类型：
                                </td>
                                <td style="text-align: left; padding-left: 10px;">
                                    <div id="loadSubjectTypeImg" style="display: none;">
                                        <img src="../image/WaitImg/loadingA.gif" />
                                    </div>
                                    <asp:CheckBoxList ID="cblSubjectType" runat="server" RepeatDirection="Horizontal"
                                        RepeatLayout="Flow" RepeatColumns="6" AutoPostBack="true" OnSelectedIndexChanged="cblSubjectType_SelectedIndexChanged">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    活动分类：
                                </td>
                                <td style="text-align: left; padding-left: 10px;">
                                    <div id="loadActivityImg" style="display: none;">
                                        <img src="../image/WaitImg/loadingA.gif" />
                                    </div>
                                    <asp:CheckBoxList ID="cblActivity" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                                        RepeatColumns="6" AutoPostBack="true" OnSelectedIndexChanged="cblActivity_SelectedIndexChanged">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    区 域：
                                </td>
                                <td style="text-align: left; padding-left: 10px;">
                                    <div id="loadRegionImg" style="display: none;">
                                        <img src="../image/WaitImg/loadingA.gif" />
                                    </div>
                                    <asp:CheckBoxList ID="cblRegion" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"
                                        RepeatLayout="Flow" OnSelectedIndexChanged="cblRegion_SelectedIndexChanged">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                        </table>
                        <div style="width: 100%; margin: 10px; text-align: left; line-height: 25px;">
                            <div id="loadSubjectImg" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:Panel ID="PanelSubject" runat="server" Visible="false">
                                <div>
                                    <asp:CheckBox ID="cbSubjectAll" runat="server" AutoPostBack="true" OnCheckedChanged="cbSubjectAll_CheckedChanged" /><span
                                        style="color: Blue;">全选</span>
                                </div>
                                <asp:CheckBoxList ID="cblSubjects" runat="server" RepeatDirection="Horizontal" 
                                    RepeatLayout="Flow" AutoPostBack="true"
                                    RepeatColumns="6" 
                                    onselectedindexchanged="cblSubjects_SelectedIndexChanged">
                                </asp:CheckBoxList>
                            </asp:Panel>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="tbBold">
                        省份
                    </td>
                    <td style="text-align: left; padding-left: 10px;">
                        <div id="loadProvinceImg" style="display: none;">
                            <img src="../image/WaitImg/loadingA.gif" />
                        </div>
                        <asp:Panel ID="PanelProvince" runat="server" Visible="false">
                            <div>
                                <asp:CheckBox ID="cbProvinceAll" runat="server" AutoPostBack="true" 
                                    oncheckedchanged="cbProvinceAll_CheckedChanged" /><span style="color: Blue;">全选</span>
                            </div>
                            <div style="line-height:25px;">
                            <asp:CheckBoxList ID="cblProvince" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true"
                                RepeatColumns="6" onselectedindexchanged="cblProvince_SelectedIndexChanged">
                            </asp:CheckBoxList>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="tbBold">
                        城市
                    </td>
                    <td style="text-align: left; padding-left: 10px;">
                        <div id="loadCityImg" style="display: none;">
                            <img src="../image/WaitImg/loadingA.gif" />
                        </div>
                        <asp:Panel ID="PanelCity" runat="server" Visible="false">
                            <div>
                                <asp:CheckBox ID="cbCityAll" runat="server" AutoPostBack="true" 
                                    oncheckedchanged="cbCityAll_CheckedChanged" /><span style="color: Blue;">全选</span>
                            </div>
                            <div style="line-height:25px;">
                            <asp:CheckBoxList ID="cblCity" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true"
                                RepeatColumns="6" onselectedindexchanged="cblCity_SelectedIndexChanged">
                            </asp:CheckBoxList>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td class="tbBold">
                        分区客服
                    </td>
                    <td style="text-align: left; padding-left: 10px;">
                         <div id="loadlCustomerServiceImg" style="display: none;">
                            <img src="../image/WaitImg/loadingA.gif" />
                        </div>
                        <asp:CheckBoxList ID="cblCustomerService" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow" RepeatColumns="6">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="tbBold">
                        安装级别
                    </td>
                    <td style="text-align: left; padding-left: 10px;">
                        <asp:CheckBoxList ID="cblIsInstall" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                            RepeatColumns="6">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="tbBold">
                        材质类型
                    </td>
                    <td style="text-align: left; padding-left: 10px;">
                        <asp:CheckBoxList ID="cblMaterialCategory" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow" RepeatColumns="6">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_hui">
                    <td class="tbBold">
                    </td>
                    <td style="text-align: left;">
                        <span>总店铺数量：</span><asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                        <span style=" margin-left:80px;">总订单数量：</span><asp:Label ID="labOrderCount" runat="server" Text="0"></asp:Label>
                        
                        <span style=" margin-left:80px;">闭店数量：</span><asp:Label ID="labShutShopCout" runat="server" Text="0"></asp:Label>
                        <span style=" margin-left:80px;">闭店订单数量：</span><asp:Label ID="labShutShopOrderCout" runat="server" Text="0"></asp:Label>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
    <blockquote class="layui-elem-quote" style="height: 15px; padding-top: 7px; font-weight: bold;">
        选择导出类型
    </blockquote>
    <table class="layui-table table1" style="margin-top: -10px; margin-bottom: 30px;">
        <tr>
            <td class="tbBold" style="width: 100px;">
                导出类型
            </td>
            <td colspan="3" style="text-align: left;">
                <asp:CheckBoxList ID="cblExportType" runat="server" RepeatDirection="Horizontal"
                    RepeatLayout="Flow">
                    <asp:ListItem Value="nohc">非HC订单&nbsp;</asp:ListItem>
                    <asp:ListItem Value="hc">HC订单&nbsp;</asp:ListItem>
                </asp:CheckBoxList>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="tbBold">
                导出订单
            </td>
            <td style="text-align: left; width: 350px;">
                <asp:Button ID="Button1" runat="server" Text="导出系统单" CssClass="layui-btn layui-btn-small" />
                <asp:Button ID="Button2" runat="server" Text="导出报价单" CssClass="layui-btn layui-btn-small"
                    Style="margin-left: 30px;" />
            </td>
            <td class="tbBold" style="width: 100px;">
                导出喷绘王模板
            </td>
            <td style="text-align: left;">
                <asp:Button ID="Button3" runat="server" Text="导出北京订单" CssClass="layui-btn layui-btn-small" />
                <asp:Button ID="Button4" runat="server" Text="导出外协订单" CssClass="layui-btn layui-btn-small"
                    Style="margin-left: 30px;" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
<script src="js/orderExport.js" type="text/javascript"></script>
