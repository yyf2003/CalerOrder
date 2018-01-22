<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubjectDetail.aspx.cs"
    Inherits="WebApp.Subjects.OutsourceMaterialOrder.SubjectDetail" %>

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
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            外协物料订单
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 100px;">
                所属客户
            </td>
            <td style="width: 300px; text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 100px;">
                活动名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labGuidance" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td>
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                外协名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labOutsource" runat="server" Text=""></asp:Label>
            </td>
            <td>
                创建人
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddUserName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                订单类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
            </td>
            <td>
                创建时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                备注
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <div class="tr" style="margin-top: 10px;">
        >>物料信息</div>
    <asp:Repeater ID="tbMaterialList" runat="server">
        <HeaderTemplate>
            <table class="table" id="tbMaterialData">
                <tr class="tr_hui">
                    <td style="width: 60px;">
                        序号
                    </td>
                    <td>
                        物料名称
                    </td>
                    <td style="width: 150px;">
                        单价
                    </td>
                    <td style="width: 150px;">
                        数量
                    </td>
                    <td>
                        备注
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai materialData">
                <td style="width: 60px;">
                    <%#Container.ItemIndex+1 %>
                </td>
                <td>
                    <%#Eval("MaterialName")%>
                </td>
                <td style="width: 150px;">
                    <%#Eval("UnitPrice")%>
                </td>
                <td style="width: 150px;">
                    <%#Eval("Amount")%>
                </td>
                <td>
                    <%#Eval("Remark")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (tbMaterialList.Items.Count == 0)
              { %>
            <tr class="tr_bai">
                <td colspan="5" style="text-align: center;">
                    --暂无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <asp:Panel ID="Panel_ApproveInfo" runat="server" Visible="false">
        <div class="tr" style="margin-top: 10px;">
            >>审批记录</div>
        <div id="approveInfoDiv" runat="server" style="margin-top: -10px;">
        </div>
    </asp:Panel>
    <div style="margin-top: 10px; text-align: center; margin-bottom: 30px;">
        <span id="spanReturn" onclick="javascript:window.history.go(-1)" class="layui-btn layui-btn-small"
            style="margin-left: 30px;">返 回</span> <%--<span id="spanEdit" runat="server" class="layui-btn"
                style="margin-left: 30px; display: none;">继续提交</span>--%>
        <asp:Button ID="btnEdit" runat="server" Visible="false" Text="删除项目" class="layui-btn layui-btn-small"
            Style="margin-left: 30px;" OnClick="btnEdit_Click"/>
        <asp:Button ID="btnDelete" runat="server" Visible="false" Text="删除项目" class="layui-btn layui-btn-danger layui-btn-small"
            Style="margin-left: 30px;" OnClick="btnDelete_Click" OnClientClick="return ConfirmDelete()" />
    </div>
    </form>
</body>
</html>
