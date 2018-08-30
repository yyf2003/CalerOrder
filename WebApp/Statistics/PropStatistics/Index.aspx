<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="WebApp.Statistics.PropStatistics.Index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="/layui230/css/layui.css" rel="stylesheet" type="text/css" media="all" />
    <script src="/layui230/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                道具项目统计
            </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
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
                            按活动月查询
                            <asp:RadioButton ID="rbOnGuidanceSearch" GroupName="rbSearchType" runat="server"
                                Checked="true" AutoPostBack="true" OnCheckedChanged="rbOnGuidanceSearch_CheckedChanged" />
                        </td>
                        <td style="text-align: left;">
                            <div>
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 120px; text-align: center;">
                                            活动月份：
                                        </td>
                                        <td style="text-align: left; padding-left: 8px;">
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
                            <asp:TextBox ID="txtSubjectBegin" runat="server" CssClass="Wdate" onclick="WdatePicker()" autocomplete="off"></asp:TextBox>
                            —
                            <asp:TextBox ID="txtSubjectEnd" runat="server" CssClass="Wdate" onclick="WdatePicker()" autocomplete="off"></asp:TextBox>
                           <%-- &nbsp;&nbsp;
                            <asp:RadioButtonList ID="rblDateType" runat="server" RepeatDirection="Horizontal"
                                RepeatLayout="Flow">
                                <asp:ListItem Value="1" Selected="True">按项目时间&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="2">按下单时间</asp:ListItem>
                            </asp:RadioButtonList>--%>
                            &nbsp;&nbsp;
                            <asp:Button ID="btnGetProject" runat="server" Text="查 询" class="easyui-linkbutton"
                                Style="width: 65px; height: 26px;" OnClick="btnGetProject_Click" OnClientClick="return CheckProjectDate()" />
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动名称
                        </td>
                        <td style="text-align: left; padding-left:5px;">
                            <div id="loadPropGuidance" style="display: none;">
                                <img src="../../image/WaitImg/loadingA.gif" />
                            </div>
                            <div id="guidanceSelectAllDiv" runat="server" style="display: none; text-align: left;">
                                <asp:CheckBox ID="cbCheckAllPropGuidance" runat="server" /><span style="color: Blue;">全选</span>
                                <hr align="left" style="width: 100px; margin-bottom: 5px;" />
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
                    <tr class="tr_bai">
                        <td>
                            外协
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadOutsource" style="display: none;">
                                <img src="../../image/WaitImg/loadingA.gif" />
                            </div>
                            <div id="OutsourceSelectAllDiv" runat="server" style="display: none; text-align: left;">
                                <asp:CheckBox ID="cbAllOutsource" runat="server" /><span style="color: Blue;">全选</span>
                               
                                <hr align="left" style="width: 100px; margin-bottom: 5px;" />
                                <asp:Button ID="btnCheckAllOutsource" runat="server" Text="Button" OnClick="btnCheckAllOutsource_Click"
                                    Style="display: none;" />
                            </div>
                            <asp:CheckBoxList ID="cblOutsource" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" AutoPostBack="true"
                                OnSelectedIndexChanged="cblOutsource_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            项目
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadPropSubject" style="display: none;">
                                <img src="../../image/WaitImg/loadingA.gif" />
                            </div>
                            <div id="SubjectSelectAllDiv" runat="server" style="display: none; text-align: left;">
                               
                                <asp:CheckBox ID="cbAllPropSubject" runat="server" /><span style="color: Blue;">全选</span>
                                <hr align="left" style="width: 100px; margin-bottom: 5px;" />
                            </div>
                            <asp:CheckBoxList ID="cblPropSubject" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                      <td></td>
                      <td style="text-align: left; padding-left: 5px; height:40px;">
                          <asp:Button ID="btnSearch" runat="server" Text="查&nbsp;&nbsp;&nbsp;询" 
                              class="layui-btn layui-btn-sm" onclick="btnSearch_Click" OnClientClick="return loading()"/>
                          <img id="loadingImg" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                      </td>
                    </tr>
                </table>

                 <div style="margin-top: 20px;">
                        <span id="btnNo" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe629;</i>应付</span>
                        <table class="table">
                            <tr class="tr_bai">
                                <td style="width: 100px;">
                                    应付金额：
                                </td>
                                <td style="text-align: left; padding-left: 5px;">
                                    <asp:Label ID="labPayPrice" runat="server" Text="0"></asp:Label>
                                </td>
                               
                            </tr>
                        </table>
                    </div>
                    <div style="margin-top: 20px; margin-bottom: 30px;">
                        <span id="Span1" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe629;</i>应收</span>
                        <table class="table">
                            <tr class="tr_bai">
                                <td style="width: 100px;">
                                    应收金额：
                                </td>
                                <td style="text-align: left; padding-left: 5px; ">
                                    <asp:Label ID="labReceivePrice" runat="server" Text="0"></asp:Label>
                                </td>
                               
                            </tr>
                        </table>
                    </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
<script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="/fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="/fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script src="js/index.js" type="text/javascript"></script>
