<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderStatistic.aspx.cs"
    Inherits="WebApp.OutsourcingOrder.Statistics.OrderStatistic" %>

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
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                外协订单统计
            </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div>
                <table class="table">
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            客户
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlCustomer" runat="server" Style="height: 23px;" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                       
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            按活动月查询
                            <asp:RadioButton ID="rbOnGuidanceSearch" GroupName="rbSearchType" runat="server"
                                Checked="true" AutoPostBack="true" OnCheckedChanged="rbOnGuidanceSearch_CheckedChanged" />
                        </td>
                        <td style="text-align: left;">
                         
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 100px; text-align: center;">
                                        活动月份：
                                    </td>
                                    <td style="text-align: left; padding-left: 5px;">
                                        <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                                            Style="width: 80px;" OnTextChanged="txtGuidanceMonth_TextChanged" autocomplete="off"
                                            AutoPostBack="true"></asp:TextBox>
                                        <asp:LinkButton ID="lbUp" runat="server" Style="margin-left: 20px; cursor: pointer;
                                            color: Blue;" OnClick="lbUp_Click">上一个月</asp:LinkButton>
                                        <asp:LinkButton ID="lbDown" runat="server" Style="margin-left: 20px; cursor: pointer;
                                            color: Blue;" OnClick="lbDown_Click">下一个月</asp:LinkButton>
                                    </td>
                                </tr>
                            </table>
                           
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            按订单时间查询
                            <asp:RadioButton ID="rbOnOrderSubjectSearch" GroupName="rbSearchType" AutoPostBack="true"
                                runat="server" OnCheckedChanged="rbOnOrderSubjectSearch_CheckedChanged" />
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtBeginDate" runat="server" CssClass="Wdate" autocomplete="off"
                                onclick="WdatePicker()"></asp:TextBox>
                            —
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="Wdate" autocomplete="off" onclick="WdatePicker()"></asp:TextBox>
                            &nbsp;&nbsp;&nbsp;
                            <asp:Button ID="btnGuidanceSearchByDate" runat="server" Text="查 询" class="easyui-linkbutton"
                                Style="width: 65px; height: 26px;" OnClick="btnSearchByDate_Click" OnClientClick="return searchByDateloading()" />
                            <img id="searchByDateLoadingImg" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
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
                                            <img src="/image/WaitImg/loadingA.gif" />
                                        </div>
                                        <asp:CheckBox ID="cbAllGuidance" runat="server" AutoPostBack="true" />全选<br />
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
                                    <td style="width: 80px; text-align: center; font-weight: bold;">
                                        道具活动：
                                    </td>
                                    <td style="text-align: left; padding-left: 5px;">
                                        <div id="loadPropGuidance" style="display: none;">
                                            <img src="/image/WaitImg/loadingA.gif" />
                                        </div>
                                        <div>
                                            <asp:CheckBox ID="cbAllPropGuidance" runat="server" />
                                            <span style="color: Blue;">全选</span>
                                        </div>
                                        <asp:CheckBoxList ID="cblPropGuidanceList" runat="server" AutoPostBack="true" CellSpacing="20"
                                            CssClass="cbl" OnSelectedIndexChanged="cblPropGuidanceList_SelectedIndexChanged"
                                            RepeatColumns="5" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        </asp:CheckBoxList>
                                        <asp:Panel ID="Panel_EmptyPropGuidance" runat="server" Visible="false">
                                            <span style="color: Red;">无活动信息！</span>
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <%--<tr class="tr_bai">
                       <td>订单渠道</td>
                       <td style="text-align: left; padding-left: 5px;">
                           <div id="loadSubjectChannel" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblSubjectChannel" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="10" AutoPostBack="true"
                                OnSelectedIndexChanged="cblSubjectChannel_SelectedIndexChanged">
                                <asp:ListItem Value="1">上海订单&nbsp;</asp:ListItem>
                                <asp:ListItem Value="2">分区订单</asp:ListItem>
                            </asp:CheckBoxList>
                       </td>
                    </tr>--%>
                    <tr class="tr_bai">
                        <td>
                            外协区域
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadOutsourceRegion" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblOutsourceRegion" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="10" AutoPostBack="true" OnSelectedIndexChanged="cblOutsourceRegion_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            外协名称
                        </td>
                        <td style="text-align: left;">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 80px; text-align: center; font-weight: bold;">
                                        POP外协：
                                    </td>
                                    <td style="text-align: left; padding-left: 5px;">
                                        <div runat="server" id="loadOutsource" style="display: none;">
                                            <img src="/image/WaitImg/loadingA.gif" />
                                        </div>
                                        <asp:CheckBoxList ID="cblOutsourceId" runat="server" CssClass="cbl" CellSpacing="20"
                                            RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8" AutoPostBack="true"
                                            OnSelectedIndexChanged="cblOutspurce_SelectedIndexChanged">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 80px; text-align: center; font-weight: bold;">
                                        道具外协：
                                    </td>
                                    <td style="text-align: left; padding-left: 5px;">
                                        <div runat="server" id="loadPropOutsource" style="display: none;">
                                            <img src="/image/WaitImg/loadingA.gif" />
                                        </div>
                                        <asp:CheckBoxList ID="cblPropOutsourceId" runat="server" CssClass="cbl" CellSpacing="20"
                                            RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            项目类型
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadCategory" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblSubjectCategory" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="10" AutoPostBack="true"
                                OnSelectedIndexChanged="cblSubjectCategory_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            店铺区域
                        </td>
                        <td  style="text-align: left; padding-left: 5px;" class="style2">
                            <asp:CheckBoxList ID="cblRegion" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="5" OnSelectedIndexChanged="cblRegion_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            店铺省份
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadProvince" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblProvince" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8" OnSelectedIndexChanged="cblProvince_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            店铺城市
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadCity" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblCity" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="8" AutoPostBack="true" OnSelectedIndexChanged="cblCity_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            项目名称
                        </td>
                        <td style="text-align: left;">
                            <table style="width: 100%;">
                                <tr>
                                    <td style="width: 80px; text-align: center; font-weight: bold;">
                                        POP项目：
                                    </td>
                                    <td style="text-align: left; padding-left: 5px;">
                                        <div id="loadSubject" style="display: none;">
                                            <img src="/image/WaitImg/loadingA.gif" />
                                        </div>
                                        <div id="cbAllDiv" runat="server" style="display: none; text-align: left;">
                                            <asp:CheckBox ID="cbAll" runat="server" />全选
                                            <hr align="left" style="width: 100px; margin-bottom: 5px;" />
                                        </div>
                                        <asp:CheckBoxList ID="cblSubjects" runat="server" CssClass="cbl" CellSpacing="20"
                                            RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                                            OnSelectedIndexChanged="cblSubjects_SelectedIndexChanged">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 80px; text-align: center; font-weight: bold;">
                                        道具项目：
                                    </td>
                                    <td style="text-align: left; padding-left: 5px;">
                                        <div id="loadPropSubject" style="display: none;">
                                            <img src="/image/WaitImg/loadingA.gif" />
                                        </div>
                                        <div id="cbAllPropDiv" runat="server" style="display: none; text-align: left;">
                                            <asp:CheckBox ID="cbAllProp" runat="server" />全选
                                            <hr align="left" style="width: 100px; margin-bottom: 5px;" />
                                        </div>
                                        <asp:CheckBoxList ID="cblPropSubjects" runat="server" CssClass="cbl" CellSpacing="20"
                                            RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                                            OnSelectedIndexChanged="cblPropSubjects_SelectedIndexChanged">
                                        </asp:CheckBoxList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            材质名称
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div runat="server" id="loadMaterial" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblMaterial" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            类型
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:CheckBoxList ID="cblOutsourceType" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow">
                                <asp:ListItem Value="1">安装&nbsp;</asp:ListItem>
                                <asp:ListItem Value="2">发货&nbsp;</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                        </td>
                        <td style="text-align: left; padding-left: 5px; height: 35px;">
                            <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                                height: 26px;" OnClick="btnSearch_Click" OnClientClick="return loading()" />
                            <img id="loadingImg" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                        </td>
                    </tr>
                </table>
            </div>
            <div class="layui-tab layui-tab-card">
                <ul class="layui-tab-title">
                    <li class="layui-this" lay-id="1">统计信息</li>
                </ul>
                <div id="divContent1" class="layui-tab-content" style="padding: 0px 10px;">
                    <div id="toolbar" style="height: 28px; padding-top: 3px; font-size: 13px;">
                        <%-- <a id="btnExportDetail" style="float: left;" class="easyui-linkbutton l-btn l-btn-small l-btn-plain"  plain="true"
                            icon="icon-print">导出明细</a>--%>
                        <span id="btnExportDetail" class="layui-btn layui-btn-primary layui-btn-small"><i
                            class="layui-icon">&#xe61e;</i> 导出明细 </span>
                    </div>
                    <div style="margin-top: 15px;">
                        <table class="table">
                            <tr class="tr_hui">
                                <td style="width: 100px;">
                                    店铺数量：
                                </td>
                                <td style="width: 280px; text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                                </td>
                                <td style="width: 120px;">
                                    总面积：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labArea" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div style="margin-top: 10px;">
                        <span id="btnNo" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe629;</i>应付</span>
                        <table class="table">
                            <tr class="tr_bai">
                                <td style="width: 100px;">
                                    POP金额：
                                </td>
                                <td style="width: 280px; text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labPOPPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td style="width: 120px;">
                                    安装费：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labInstallPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    快递费：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labExpressPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    测量费：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labMeasurePrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    其他费用：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labOtherPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    道具费用：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labPropPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                </td>
                                <td>
                                </td>
                                <td>
                                    总金额：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div style="margin-top: 20px; margin-bottom: 30px;">
                        <span id="Span1" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe629;</i>应收</span>
                        <table class="table">
                            <tr class="tr_bai">
                                <td style="width: 100px;">
                                    POP金额：
                                </td>
                                <td style="width: 280px; text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labRPOPPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td style="width: 120px;">
                                    安装费：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labRInstallPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    快递费：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labRExpressPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    测量费：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labRMeasurePrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                    其他费用：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labROtherPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                   道具费用(总数)：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labRPropPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                            <tr class="tr_bai">
                                <td>
                                </td>
                                <td>
                                </td>
                                <td>
                                    总金额：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labRTotalPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Button ID="btnExport" runat="server" Text="" OnClick="btnExport_Click" Style="display: none;" />
    </form>
</body>
</html>
<script src="OrderStatistic.js" type="text/javascript"></script>
