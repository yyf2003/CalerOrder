<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuoteOperate.aspx.cs" Inherits="WebApp.QuoteOrderManager.QuoteOperate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        function Finish() {
            layer.closeAll();
            layer.msg("提交成功!");
            $("#btnRefresh").click();
        }
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
                活动报价提交
            </p>
    </div>
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
                        <td style="width: 120px;">
                            活动名称
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
                    <tr class="tr_bai">
                        <td>
                            项目类型
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
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
                        <td>
                            项目名称
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadSubject" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <div>
                                <asp:CheckBox ID="cbAllSubject" runat="server" /><span style="color: Blue;">全选</span>
                            </div>
                            <asp:CheckBoxList ID="cblSubjectName" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="10">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            区域
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:CheckBoxList ID="cblRegion" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="5">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                        </td>
                        <td style="text-align: left; padding-left: 5px; height: 35px;">
                            <input type="button" value="添加报价" id="btnAddQuote" class="layui-btn layui-btn-small" />
                        </td>
                    </tr>
                </table>
                <div class="tr" style="margin-top: 20px;">
                    >>报价列表
                </div>
                <asp:Repeater ID="gvList" runat="server" OnItemDataBound="gvList_ItemDataBound" OnItemCommand="gvList_ItemCommand">
                    <HeaderTemplate>
                        <table class="table">
                            <tr class="tr_hui">
                                <td style="width: 60px;">
                                    序号
                                </td>
                                <td>
                                    活动
                                </td>
                                <td>
                                    项目类型
                                </td>
                                <td>
                                    总面积
                                </td>
                                <td>
                                    总金额
                                </td>
                                <td>
                                    添加人
                                </td>
                                <td>
                                    添加时间
                                </td>
                                <td style="width: 150px;">
                                    操作
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="tr_hui">
                            <td>
                                <%#Container.ItemIndex + 1%>
                            </td>
                            <td>
                                <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="labSubjectCategory" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <%-- <asp:Label ID="labTotalArea" runat="server" Text=""></asp:Label>--%>
                                <%#Eval("order.TotalArea")%>
                            </td>
                            <td>
                                <%--<asp:Label ID="labTotalPrice" runat="server" Text=""></asp:Label>--%>
                                <%#Eval("order.TotalPrice")%>
                            </td>
                            <td>
                                <%#Eval("AddUserName")%>
                            </td>
                            <td>
                                <%#Eval("order.AddDate")%>
                            </td>
                            <td>
                                <span onclick="checkItem('<%#Eval("order.Id")%>')" style="cursor: pointer; color: Blue;">
                                    查看</span> &nbsp; |
                                <%--<span name="spanEditQuote" data-itemid='<%#Eval("order.Id")%>' style=" cursor:pointer;color:Blue;">编辑</span>--%>
                                <%--<asp:LinkButton ID="lbEdit" runat="server" ForeColor="Blue" OnClientClick="return editItem('<%#Eval("order.Id")%>')">删除</asp:LinkButton>--%>
                                <asp:LinkButton ID="lbEdit" runat="server">编辑</asp:LinkButton>
                                &nbsp; | &nbsp;
                                <asp:LinkButton ID="lbDelete" runat="server" CommandArgument='<%#Eval("order.Id")%>'>删除</asp:LinkButton>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%if (gvList.Items.Count == 0)
                          {%>
                        <tr class="tr_bai">
                            <td colspan="8" style="text-align: center;">
                                --无数据--
                            </td>
                        </tr>
                        <%} %>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Button ID="btnRefresh" runat="server" Text="Button" OnClick="btnRefresh_Click"
                    Style="display: none;" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
<script src="js/quoteOperate.js" type="text/javascript"></script>
