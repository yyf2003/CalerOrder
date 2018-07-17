<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderStatistics.aspx.cs"
    Inherits="WebApp.Statistics.OrderStatistics" %>

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
    <style type="text/css">
        .cbl td
        {
            padding-right: 20px;
        }
        body
        {
            margin-bottom: 50px;
        }
    </style>
    <script type="text/javascript">
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                项目统计
            </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:HiddenField ID="hfPriceSubjectIds" runat="server" />
                <asp:HiddenField ID="hfSubjectIds" runat="server" />
                <table class="table">
                    <tr class="tr_bai">
                        <td class="style3">
                            客户
                        </td>
                        <td style="text-align: left; padding-left: 5px;" class="style4">
                            <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
                            <span style="color: Red; cursor: pointer;">*</span>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            按活动时间查询
                            <asp:RadioButton ID="rbOnGuidanceSearch" GroupName="rbSearchType" runat="server"
                                Checked="true" AutoPostBack="true" OnCheckedChanged="rbOnGuidanceSearch_CheckedChanged" />
                        </td>
                        <td style="text-align: left;">
                            <%--<div style="margin-left: 5px;">
                                <asp:TextBox ID="txtGuidanceBegin" runat="server" onclick="WdatePicker()" autocomplete="off"
                                    CssClass="Wdate"></asp:TextBox>
                                —
                                <asp:TextBox ID="txtGuidanceEnd" runat="server" onclick="WdatePicker()" autocomplete="off"
                                    CssClass="Wdate"></asp:TextBox>
                                &nbsp;&nbsp;
                                <asp:Button ID="btnGetGuidance" runat="server" Text="获取活动" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px;" OnClick="btnGetGuidance_Click" OnClientClick="return CheckGuidanceDate()" />
                            </div>--%>
                            <div>
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 120px; text-align: center;">
                                            按时间段显示：
                                        </td>
                                        <td style="width: 200px; text-align: left;">
                                            <asp:Label ID="labBeginDate" runat="server" Text=""></asp:Label>
                                            <asp:Label ID="labSeparator" runat="server" Text="—" Visible="false"></asp:Label>
                                            <asp:Label ID="labEndDate" runat="server" Text=""></asp:Label>
                                        </td>
                                        <td style="width: 120px; text-align: center;">
                                            按活动月显示：
                                        </td>
                                        <td style="text-align: left; padding-left: 8px;">
                                            <%--<asp:Label ID="labGuidanceMonth" runat="server" Text=""></asp:Label>--%>
                                            <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                                                Style="width: 80px;" OnTextChanged="txtGuidanceMonth_TextChanged" AutoPostBack="true"
                                                autocomplete="off"></asp:TextBox>
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
                    <tr class="tr_bai">
                        <td>
                            按时间查询
                            <asp:RadioButton ID="rbOnOrderSubjectSearch" GroupName="rbSearchType" AutoPostBack="true"
                                runat="server" OnCheckedChanged="rbOnOrderSubjectSearch_CheckedChanged" />
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtSubjectBegin" runat="server" onclick="WdatePicker()" autocomplete="off"></asp:TextBox>
                            —
                            <asp:TextBox ID="txtSubjectEnd" runat="server" onclick="WdatePicker()" autocomplete="off"></asp:TextBox>
                            &nbsp;&nbsp;
                            <asp:RadioButtonList ID="rblDateType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                              <asp:ListItem Value="1" Selected="True">按项目时间&nbsp;&nbsp;</asp:ListItem>
                              <asp:ListItem Value="2">按下单时间</asp:ListItem>
                            </asp:RadioButtonList>
                            &nbsp;&nbsp;
                            <asp:Button ID="btnGetProject" runat="server" Text="查 询" class="easyui-linkbutton"
                                Style="width: 65px; height: 26px;" OnClick="btnGetProject_Click" OnClientClick="return CheckProjectDate()" />
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动名称
                        </td>
                        <td style="text-align: left;">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 80px; text-align: center; font-weight: bold;">
                                        POP活动：
                                    </td>
                                    <td style="text-align: left; padding-left: 5px;">
                                        <div id="loadGuidance" style="display: none;">
                                            <img src="../image/WaitImg/loadingA.gif" />
                                        </div>
                                        <div>
                                            <asp:CheckBox ID="cbCheckAllGuidance" runat="server" /><span style="color: Blue;">全选</span>
                                            <asp:Button ID="btnCheckAllGuidance" runat="server" Text="Button" OnClick="btnCheckAllGuidance_Click"
                                                Style="display: none;" />
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
                                <tr>
                                    <td style="text-align: center; font-weight: bold;">
                                        道具活动：
                                    </td>
                                    <td style="text-align: left; padding-left: 5px;">
                                        <div id="loadPropGuidance" style="display: none;">
                                            <img src="../image/WaitImg/loadingA.gif" />
                                        </div>
                                        <div>
                                            <asp:CheckBox ID="cbCheckAllPropGuidance" runat="server" /><span style="color: Blue;">全选</span>
                                            <asp:Button ID="btnCheckAllPropGuidance" runat="server" Text="Button" OnClick="btnCheckAllPropGuidance_Click"
                                                Style="display: none;" />
                                        </div>
                                        <asp:CheckBoxList ID="cblPropGuidanceList" runat="server" CssClass="cbl" CellSpacing="20"
                                            RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                                            OnSelectedIndexChanged="cblPropGuidanceList_SelectedIndexChanged">
                                        </asp:CheckBoxList>
                                        <asp:Panel ID="Panel_EmptyPropGuidance" runat="server" Visible="false">
                                            <span style="color: Red;">无活动信息！</span>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td class="style1">
                            订单渠道
                        </td>
                        <td style="text-align: left; padding-left: 5px;" class="style2">
                            <div id="loadSubjectChannel" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblSubjectChannel" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="10" AutoPostBack="true"
                                OnSelectedIndexChanged="cblSubjectChannel_SelectedIndexChanged">
                                <asp:ListItem Value="1">系统单（上海）&nbsp;</asp:ListItem>
                                <asp:ListItem Value="2">分区订单</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td class="style1">
                            店铺类型
                        </td>
                        <td style="text-align: left; padding-left: 5px;" class="style2">
                            <div id="loadShopType" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblShopType" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="10" AutoPostBack="true"
                                OnSelectedIndexChanged="cblShopType_SelectedIndexChanged1">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td class="style1">
                            创建人
                        </td>
                        <td style="text-align: left; padding-left: 5px;" class="style2">
                            <div id="loadUser" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblAddUser" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="10" AutoPostBack="true"
                                OnSelectedIndexChanged="cblAddUser_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td class="style1">
                            项目类型
                        </td>
                        <td style="text-align: left; padding-left: 5px;" class="style2">
                            <div id="loadCategory" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblSubjectCategory" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="10" AutoPostBack="true"
                                OnSelectedIndexChanged="cblSubjectCategory_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td class="style1">
                            区域
                        </td>
                        <td style="text-align: left; padding-left: 5px;" class="style2">
                            <asp:CheckBoxList ID="cblRegion" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="5" OnSelectedIndexChanged="cblRegion_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
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
                        <td style="width: 120px;">
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
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            分区客服
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadCustomerService" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblCustomerService" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
                <div class="tr">
                    >>项目名称 &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:CheckBox ID="cbShowSubjectNameList" runat="server" AutoPostBack="true" OnCheckedChanged="cbShowSubjectNameList_CheckedChanged" />显示
                    <img id="loadSubjectNames" style="display: none;" src="../image/WaitImg/loadingA.gif" />
                </div>
                <%-- <asp:Panel ID="Panel_SubjectNameList" runat="server" Visible="false">
                   
                </asp:Panel>--%>
                <div id="Panel_SubjectNameList" runat="server" style="display: none;">
                    <table class="table">
                        <tr class="tr_bai">
                            <td style="width: 120px;">
                                POP订单项目
                            </td>
                            <td style="text-align: left; padding-left: 5px;">
                                <div id="loadSubject" style="display: none;">
                                    <img src="../image/WaitImg/loadingA.gif" />
                                </div>
                                <div id="cbAllDiv" runat="server" style="text-align: left;">
                                    <asp:CheckBox ID="cbAll" runat="server" />全选
                                    <hr align="left" style="width: 100px; margin-bottom: 5px;" />
                                </div>
                                <asp:CheckBoxList ID="cblSubjects" runat="server" CssClass="cbl" CellSpacing="20"
                                    RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                                    OnSelectedIndexChanged="cblSubjects_SelectedIndexChanged">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr class="tr_bai">
                            <td style="width: 120px;">
                                费用项目
                            </td>
                            <td style="text-align: left; padding-left: 5px;">
                                <div id="loadSubject0" style="display: none;">
                                    <img src="../image/WaitImg/loadingA.gif" />
                                </div>
                                <div id="cbAllDiv0" runat="server" style="display: none; text-align: left;">
                                    <asp:CheckBox ID="cbAll0" runat="server" />全选
                                    <hr align="left" style="width: 100px; margin-bottom: 5px;" />
                                </div>
                                <asp:CheckBoxList ID="cblPriceSubjects" runat="server" CssClass="cbl" CellSpacing="20"
                                    RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                                    OnSelectedIndexChanged="cblPriceSubjects_SelectedIndexChanged">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                        <tr class="tr_bai">
                            <td>
                                道具项目
                            </td>
                            <td style="text-align: left; padding-left: 5px;">
                                <div id="loadPropSubject" style="display: none;">
                                    <img src="../image/WaitImg/loadingA.gif" />
                                </div>
                                <div id="cbAllPropDiv" runat="server" style="display: none; text-align: left;">
                                    <asp:CheckBox ID="cbAllProp" runat="server" />全选
                                    <hr align="left" style="width: 100px; margin-bottom: 5px;" />
                                </div>
                                <asp:CheckBoxList ID="cblPropSubject" runat="server" CssClass="cbl" CellSpacing="20"
                                    RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                                    OnSelectedIndexChanged="cblPropSubject_SelectedIndexChanged">
                                </asp:CheckBoxList>
                            </td>
                        </tr>
                    </table>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style="text-align: right; padding-right: 25px; margin-top: 8px;">
            <asp:Button ID="Button1" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                height: 26px;" OnClick="Button1_Click" OnClientClick="return loading()" />
            <img id="loadingImg" style="display: none;" src="../image/WaitImg/loadingA.gif" />
        </div>
    </div>
    <div style="margin-top: 10px;">
        <table class="table">
            <tr class=" tr_hui">
                <td style="width: 120px; height: 30px;">
                    导出明细：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:CheckBox ID="cbInstallPrice" runat="server" />安装费 &nbsp;&nbsp;
                    <input type="button" id="btnExport" value="导出订单明细" class="easyui-linkbutton" style="width: 95px;
                        height: 26px;" />
                    <img id="exportWaiting" style="display: none;" src="../image/WaitImg/loadingA.gif" />
                    <input type="button" id="btnExportByShop" value="按店统计导出" class="easyui-linkbutton"
                        style="width: 95px; height: 26px; margin-left: 30px;" />
                    <img id="exportWaitingByShop" style="display: none;" src="../image/WaitImg/loadingA.gif" />
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div style="margin: 0px;">
        <table class="table statisticsTable">
            <tr class="tr_hui">
                <td style="width: 120px; text-align: right;">
                    项目数量：
                </td>
                <td style="text-align: left; width: 150px; padding-left: 5px;">
                    <asp:Label ID="labSubjectCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right; width: 150px;">
                    店铺数量：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 150px;">
                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right; width: 200px;">
                    总面积：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labArea" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    POP金额合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labPOPPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right;">
                    活动安装费合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labInstallPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right;">
                    快递费/运费合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labExpressPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    二次安装费合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSecondInstallPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right;">
                    物料费：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labMaterialPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right;">
                    新开店装修费：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labNewShopInstallPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    二次发货费合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSecondExpressPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right;">
                    新开店测量费：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labMeasurePrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right;">
                    分区增补/新开店安装费：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labRegionInstallPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    其他费用：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labFreight" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right;">
                    分区增补发货费：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labRegionExpressPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: right;">
                    分区增补其他费用：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labRegionOtherPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right;">
                    道具：
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labPropPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: right; font-weight:bold;">
                    总金额：
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="Panel1" runat="server" Visible="false">
        <table class="table statisticsTable">
            <tr class="tr_hui">
                <td style="width: 120px; color: Blue;">
                    闭店订单统计：
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <input type="button" id="btnExport1" value="导出明细" class="easyui-linkbutton" style="width: 65px;
                        height: 26px;" />
                    <img id="Img1" style="display: none;" src="../image/WaitImg/loadingA.gif" />
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: center; width: 120px;">
                    店铺数量：
                </td>
                <td style="text-align: left; width: 150px; padding-left: 5px;">
                    <asp:Label ID="labShopCount1" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 150px;">
                    面积合计：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 200px;">
                    <asp:Label ID="labArea1" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 120px;">
                    POP金额合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labPOPPrice1" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: center;">
                    快递费合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labExpressPrice1" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center;">
                    正常安装费合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labInstallPrice1" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center;">
                    物料费(道具)：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labMaterialPrice1" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td colspan="5" style="text-align: right; padding-right: 10px;">
                    总金额：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTotalPrice1" runat="server" Text=""></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div class="tr">
        >>信息列表 &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:CheckBox ID="cbShopSubjectList" runat="server" OnCheckedChanged="cbShopSubjectList_CheckedChanged"
            AutoPostBack="true" />显示项目明细
    </div>
    <div style="width: 100%; overflow: auto;">
        <asp:Repeater ID="gvList" Visible="false" runat="server" OnItemDataBound="gvList_ItemDataBound"
            OnItemCommand="gvList_ItemCommand">
            <HeaderTemplate>
                <table class="table1" style="width: 2100px;">
                    <tr class="tr_hui">
                        <td rowspan="2" style="width: 40px;">
                            序号
                        </td>
                        <td rowspan="2">
                            项目名称
                        </td>
                        <td rowspan="2">
                            项目类型
                        </td>
                        <td rowspan="2">
                            订单类型
                        </td>
                        <td rowspan="2">
                            所属客户
                        </td>
                        <td rowspan="2">
                            下单时间
                        </td>
                        <td colspan="3">
                            店铺数量
                        </td>
                        <td colspan="3">
                            POP总面积(平米)
                        </td>
                        <td colspan="3">
                            POP金额
                        </td>
                        <td colspan="3">
                            安装费
                        </td>
                        <td colspan="3">
                            物料费
                        </td>
                        <td colspan="3">
                            新开店安装费
                        </td>
                        <td colspan="3">
                            其他费用
                        </td>
                        <td rowspan="2">
                            道具费用
                        </td>
                        <td rowspan="2">
                            查看
                        </td>
                    </tr>
                    <tr class="tr_hui" style="color: #000; font-weight: bold;">
                        <td>
                            系统(上海)
                        </td>
                        <td>
                            分区
                        </td>
                        <td>
                            合计
                        </td>
                        <td>
                            系统(上海)
                        </td>
                        <td>
                            分区
                        </td>
                        <td>
                            合计
                        </td>
                        <td>
                            系统(上海)
                        </td>
                        <td>
                            分区
                        </td>
                        <td>
                            合计
                        </td>
                        <td>
                            系统(上海)
                        </td>
                        <td>
                            分区
                        </td>
                        <td>
                            合计
                        </td>
                        <td>
                            系统(上海)
                        </td>
                        <td>
                            分区
                        </td>
                        <td>
                            合计
                        </td>
                        <td>
                            系统(上海)
                        </td>
                        <td>
                            分区
                        </td>
                        <td>
                            合计
                        </td>
                        <td>
                            系统(上海)
                        </td>
                        <td>
                            分区
                        </td>
                        <td>
                            合计
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td style="width: 40px;">
                        <%--<%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>--%>
                        <%#Container.ItemIndex + 1%>
                    </td>
                    <td>
                        <%--<%#Eval("subject.SubjectName")%>--%>
                        <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
                    </td>
                    <%--<td>
                    <%#Eval("subject.SubjectNo")%>
                </td>--%>
                    <td>
                        <%#Eval("CategoryName")%>
                    </td>
                    <td>
                        <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <%#Eval("CustomerName")%>
                    </td>
                    <td>
                        <%#Eval("subject.AddDate")%>
                    </td>
                    <td>
                        <%--店铺数量--%>
                        <asp:Label ID="labShopCount" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <%--店铺数量--%>
                        <asp:Label ID="labShopCount1" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labShopCountSub" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <%--POP平米数--%>
                        <asp:Label ID="labArea" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <%--POP平米数--%>
                        <asp:Label ID="labArea1" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labAreaSub" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <%--POP金额--%>
                        <asp:Label ID="labPOPPrice" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <%--POP金额--%>
                        <asp:Label ID="labPOPPrice1" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labPOPPriceSub" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labInstallPrice" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labInstallPrice1" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labInstallPriceSub" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labMaterial" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labMaterial1" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labMaterialSub" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labNewShopInstallPrice" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labNewShopInstallPrice1" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labNewShopInstallPriceSub" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labOtherPrice" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labOtherPrice1" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labOtherPriceSub" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <asp:Label ID="labPropPrice" runat="server" Text="0"></asp:Label>
                    </td>
                    <td>
                        <span name="spanCheckDetail" data-subjectid='<%#Eval("Id") %>' data-subjecttype='<%#Eval("SubjectType") %>'
                            style="color: Blue; cursor: pointer;">查看</span>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (gvList.Items.Count == 0)
                  {%>
                <tr class="tr_bai">
                    <td colspan="22" style="text-align: center;">
                        --无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div style="display: none;">
        <iframe id="exportFrame" name="exportFrame" src=""></iframe>
    </div>
    </form>
</body>
</html>
<script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="/fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="/fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<link href="/layer/skin/default/layer.css" rel="stylesheet" type="text/css" />
<script src="/layer/layer.js" type="text/javascript"></script>
<script src="js/OrderStatistics.js" type="text/javascript"></script>
