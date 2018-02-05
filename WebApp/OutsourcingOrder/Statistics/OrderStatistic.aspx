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
                        <td style="width: 100px;">
                            客户
                        </td>
                        <td style="width: 200px; text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlCustomer" runat="server" Style="height: 23px;" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td style="width: 100px;">
                            活动月份
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                                Style="width: 80px;" OnTextChanged="txtGuidanceMonth_TextChanged" AutoPostBack="true"></asp:TextBox>
                            <asp:LinkButton ID="lbUp" runat="server" Style="margin-left: 20px; cursor: pointer;
                                color: Blue;" OnClick="lbUp_Click">上一个月</asp:LinkButton>
                            <asp:LinkButton ID="lbDown" runat="server" Style="margin-left: 20px; cursor: pointer;
                                color: Blue;" OnClick="lbDown_Click">下一个月</asp:LinkButton>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            时间范围
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtBegin" runat="server"></asp:TextBox>
                            -
                            <asp:TextBox ID="txtEnd" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            活动名称
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadGuidance" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
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
                            项目类型
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
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
                            区域
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;" class="style2">
                            <asp:CheckBoxList ID="cblRegion" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="5" OnSelectedIndexChanged="cblRegion_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            省份
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
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
                            城市
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
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
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
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
                    <tr class="tr_bai">
                        <td>
                            外协名称
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div runat="server" id="loadOutsource" style="display: none;">
                                <img src="/image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblOutsource" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                                OnSelectedIndexChanged="cblOutspurce_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            外协类型
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
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
                        <td colspan="3" style="text-align: left; padding-left: 5px; height: 35px;">
                            <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                                height: 26px;" OnClick="btnSearch_Click" OnClientClick="return loading()" />
                            <img id="loadingImg" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                        </td>
                    </tr>
                </table>
            </div>
            <div style=" margin-top:15px;">
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
                            <asp:Label ID="labInstall" runat="server" Text="0"></asp:Label>
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
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
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
            <div style="margin-top: 20px; margin-bottom :30px;">
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
                            <asp:Label ID="labRInstall" runat="server" Text="0"></asp:Label>
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
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
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
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
<script src="OrderStatistic.js" type="text/javascript"></script>
