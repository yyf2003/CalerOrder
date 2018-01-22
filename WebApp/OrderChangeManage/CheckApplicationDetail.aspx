<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckApplicationDetail.aspx.cs"
    Inherits="WebApp.OrderChangeManage.CheckApplicationDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            项目变更申请明细
        </p>
    </div>
    <table class="layui-table">
        <tr>
            <td style="width: 70px;">
                活动名称：
            </td>
            <td style="width: 250px; text-align: left; padding-left: 5px;">
                <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 80px;">
                申请人：
            </td>
            <td style="width: 120px; text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddUser" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 80px;">
                申请时间：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                审批状态：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labApproveState" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 80px;">
                审批人：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labApproveUserName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 80px;">
                审批时间：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labApproveDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                审批意见：
            </td>
            <td colspan="5" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labApproveRemark" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <blockquote class="layui-elem-quote" style="height: 15px; padding-top: 7px; font-weight: bold;">
        申请项目信息
    </blockquote>
    <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound"
        OnItemCommand="Repeater1_ItemCommand">
        <HeaderTemplate>
            <table class="layui-table" style="margin-top: -10px;">
                <thead>
                    <tr>
                        <th style="width: 30px;">
                            序号
                        </th>
                        <th>
                            项目名称
                        </th>
                        <th>
                            变更类型
                        </th>
                        <th>
                            变更说明
                        </th>
                        <th>
                            开始执行时间
                        </th>
                        <th style="width: 100px;">
                            状态
                        </th>
                        <th>
                            完成时间
                        </th>
                        <th style="width: 100px;">
                            操作
                        </th>
                    </tr>
                </thead>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <%#Container.ItemIndex+1 %>
                </td>
                <td>
                    <%#Eval("SubjectName")%>
                </td>
                <td>
                    <asp:Label ID="labChangeType" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <%#Eval("order.Remark")%>
                </td>
                <td>
                    <%#Eval("order.ActivityDate")%>
                </td>
                <td>
                    <asp:Label ID="labChangeState" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <%#Eval("order.FinishDate")%>
                </td>
                <td>
                    <%--<asp:Label ID="labOperateType" runat="server" Text=""></asp:Label>--%>
                    <asp:Panel ID="Panel1" runat="server">
                        <asp:LinkButton ID="lbExcute" runat="server" CommandArgument='<%#Eval("Id") %>' CommandName="Excute">执行</asp:LinkButton>
                        |
                        <asp:LinkButton ID="lbCancel" runat="server" CommandArgument='<%#Eval("Id") %>' CommandName="Cancel" OnClientClick="javascript:return confirm('确定撤销吗？')">撤销</asp:LinkButton>
                    </asp:Panel>
                    <asp:Panel ID="Panel2" runat="server" Visible="false">
                        完成
                    </asp:Panel>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (Repeater1.Items.Count == 0)
              {%>
            <tr>
                <td colspan="8" style="text-align: center;">
                    --无数据信息--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Panel ID="Panel_ApproveInfo" runat="server" Visible="false">
        <blockquote class="layui-elem-quote" style="height: 15px; padding-top: 7px; font-weight: bold;">
            审批记录</blockquote>
        <div id="approveInfoDiv" runat="server" style="margin-top: -10px;">
        </div>
    </asp:Panel>
    <div style="margin-top: 30px; text-align: center; margin-bottom: 30px;">
        <%--<span id="spanReturn" onclick="javascript:window.history.go(-1)" >返 回</span>--%>
        <asp:Button ID="btnGoBack" runat="server" Text="返 回" class="layui-btn layui-btn-primary"
            style="margin-left: 30px;" onclick="btnGoBack_Click"/>
    </div>
    </form>
</body>
</html>
