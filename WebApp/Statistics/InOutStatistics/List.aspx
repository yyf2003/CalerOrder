<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Statistics.InOutStatistics.List" %>

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
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                项目收入/支出统计
            </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table class="table">
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            客户
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
                            <span style="color: Red; cursor: pointer;">*</span>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动时间
                        </td>
                        <td style="text-align: left;">
                            <div style="margin-left: 5px;">
                                <asp:TextBox ID="txtGuidanceBegin" runat="server" onclick="WdatePicker()" CssClass="Wdate"></asp:TextBox>
                                —
                                <asp:TextBox ID="txtGuidanceEnd" runat="server" onclick="WdatePicker()" CssClass="Wdate"></asp:TextBox>
                                &nbsp;&nbsp;
                                <asp:Button ID="btnGetGuidance" runat="server" Text="获取活动" class="easyui-linkbutton"
                                    Style="width: 65px; height: 26px;" OnClick="btnGetGuidance_Click" OnClientClick="return CheckGuidanceDate()" />
                            </div>
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
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动名称
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadGuidance" style="display: none;">
                                <img src="../../image/WaitImg/loadingA.gif" />
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
                    <tr class="tr_bai">
                        <td class="style1">
                            项目类型
                        </td>
                        <td style="text-align: left; padding-left: 5px;" class="style2">
                            <div id="loadCategory" style="display: none;">
                                <img src="../../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblSubjectCategory" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="10" AutoPostBack="true"
                                OnSelectedIndexChanged="cblSubjectCategory_SelectedIndexChanged">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td class="style1">
                            报价项目
                        </td>
                        <td style="text-align: left; padding-left: 5px;" class="style2">
                            <div id="loadQuoteSubject" style="display: none;">
                                <img src="../../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblQuoteSubjectList" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="10" OnSelectedIndexChanged="cblQuoteSubjectList_SelectedIndexChanged"
                                AutoPostBack="true">
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
                                <img src="../../image/WaitImg/loadingA.gif" />
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
                                <img src="../../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblCity" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="8">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
                <div class="tr" style="margin-top: 10px;">
                    》信息列表</div>
                <asp:Repeater ID="Repeater_List" runat="server" 
                    onitemdatabound="Repeater_List_ItemDataBound" 
                    >
                    <HeaderTemplate>
                        <table class="table">
                            <tr class="tr_hui">
                                <td rowspan="2" style="width: 40px; ">
                                    序号
                                </td>
                                <td rowspan="2">
                                    活动名称
                                </td>
                                <td rowspan="2">
                                    项目类型
                                </td>
                                <td rowspan="2">
                                    项目名称
                                </td>
                                <td rowspan="2">
                                    订单总面积
                                </td>
                                <td rowspan="2">
                                    闭店面积
                                </td>
                                <td colspan="6" style=" font-weight:bold;">
                                    收入(实际报价)
                                </td>
                                <td colspan="6" style=" font-weight:bold;">
                                    支出(外协)
                                </td>
                            </tr>
                            <tr class="tr_hui">
                                <td>
                                    POP面积
                                </td>
                                <td>
                                    POP制作
                                </td>
                                <td>
                                    安装费
                                </td>
                                <td>
                                    快递费
                                </td>
                                <td>
                                    物料
                                </td>
                                <td>
                                    合计
                                </td>
                                <td>
                                    POP面积
                                </td>
                                <td>
                                    POP制作
                                </td>
                                <td>
                                    安装费
                                </td>
                                <td>
                                    快递费
                                </td>
                                <td>
                                    其他
                                </td>
                                <td>
                                    合计
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="tr_bai">
                            <td>
                                <%#Container.ItemIndex+1 %>
                            </td>
                            <td>
                                <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="labSubjectCategory" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <%#Eval("QuoteSubjectName")%>
                            </td>
                            <td>
                                <%--<%#Eval("TotalArea")%>--%>
                                <asp:Label ID="labTotalArea" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                 <%--闭店面积--%>
                                 <asp:Label ID="labShutArea" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--报价POP面积--%>
                                <asp:Label ID="labQuoteArea" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--POP制作--%>
                                <asp:Label ID="labQuotePOPPrice" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--安装费--%>
                                <asp:Label ID="labQuoteInstallPrice" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--快递费--%>
                                <asp:Label ID="labQuoteExpressPrice" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--其他--%>
                                <asp:Label ID="labQuoteOtherPrice" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                               
                                <asp:Label ID="labQuoteTotalPrice" runat="server" Text="0" style=" color:blue;"></asp:Label>
                            </td>
                            <td>
                                <%--支出(实际生产)POP面积--%>
                                <asp:Label ID="labPayArea" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--POP制作--%>
                                <asp:Label ID="labPayPOPPrice" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--安装费--%>
                                <asp:Label ID="labPayInstallPrice" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--快递费--%>
                                <asp:Label ID="labPayExpressPrice" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--其他--%>
                                <asp:Label ID="labPayOtherPrice" runat="server" Text="0"></asp:Label>
                            </td>
                            <td>
                                <%--合计--%>
                                <asp:Label ID="labPayTotalPrice" runat="server" Text="0" style=" color:blue;"></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%if (Repeater_List.Items.Count == 0)
                          {%>
                        <tr class="tr_bai">
                            <td colspan="18" style="text-align: center;">
                                --无数据--
                            </td>
                        </tr>
                        <%} %>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
<script src="js/list.js" type="text/javascript"></script>
<script type="text/javascript">
    
</script>
