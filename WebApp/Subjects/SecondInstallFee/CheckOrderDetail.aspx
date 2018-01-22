<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOrderDetail.aspx.cs"
    Inherits="WebApp.Subjects.SecondInstallFee.CheckOrderDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script type="text/javascript">
        var subjectId = '<%=SubjectId %>';
       
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            查看项目明细
        </p>
    </div>
    <blockquote class="layui-elem-quote" style="height: 20px; font-weight: bold;">
        项目信息</blockquote>
    <table class="layui-table" style="margin-top: -10px;">
        <tr>
            <td style="width: 100px;">
                项目编号：
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 100px;">
                项目名称：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                项目创建人：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddUserName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                所属客户：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomerName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                订单类型：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
            </td>
            <td>
                实施区域：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRegion" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                备注：
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRemark" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <blockquote class="layui-elem-quote" style="height: 20px; font-weight: bold; margin-top: 10px;">
        订单明细</blockquote>
    <div class="layui-tab layui-tab-brief" lay-filter="order" style="margin-top: -10px;">
        <ul class="layui-tab-title">
            <li class="layui-this" lay-id="1">POP信息</li>
            <%-- <li lay-id="2">物料信息</li>--%>
        </ul>
        <div class="layui-tab-content">
            <div class="layui-tab-item layui-show">
                <div class="layui-inline">
                    <div class="layui-input-inline" style="width: 120px;">
                        <input type="text" id="txtShopNo" placeholder="店铺编号" maxlength="25" class="layui-input"
                            style="height: 30px;" />
                    </div>
                    <div class="layui-input-inline" style="width: 180px;">
                        <input type="text" id="txtShopName" placeholder="店铺名称" maxlength="50" class="layui-input"
                            style="height: 30px;" />
                    </div>
                    <span id="btnSearch" class="layui-btn layui-btn-danger layui-btn-small"><i class="layui-icon">
                        &#xe615;</i> </span>
                    <img id="loadingImg" src="../../image/WaitImg/loadingA.gif" />
                </div>
                <span id="spanExportPOPOrder" style="margin-left: 30px;" class="layui-btn layui-btn-small">
                    <i class="layui-icon">&#xe601;</i>导 出</span>
                <asp:Button ID="btnExportPOPOrder" runat="server" Text="Button" Style="display: none;"
                    OnClick="btnExportPOPOrder_Click" OnClientClick="return checkPOP();" />
                <div style="overflow: auto;">
                    <table class="layui-table" style="width: 2000px;">
                        <thead>
                            <tr>
                                <th style="width: 30px;">
                                    序号
                                </th>
                                <th>
                                    订单类型
                                </th>
                                <th>
                                    店铺编号
                                </th>
                                <th>
                                    店铺名称
                                </th>
                                <th>
                                    区域
                                </th>
                                <th>
                                    省份
                                </th>
                                <th>
                                    城市
                                </th>
                                <th>
                                    Format
                                </th>
                                <th>
                                    物料支持级别
                                </th>
                                <th>
                                    应收费用
                                </th>
                                <th>
                                    应付费用
                                </th>
                                <th>
                                    Sheet
                                </th>
                                <th>
                                    男/女
                                </th>
                                <th>
                                    数量
                                </th>
                                <th>
                                    POP宽
                                </th>
                                <th>
                                    POP高
                                </th>
                                <th>
                                    材质
                                </th>
                                <th>
                                    位置描述
                                </th>
                                <th>
                                    选图
                                </th>
                                <th>
                                    备注
                                </th>
                                <th>
                                    添加时间
                                </th>
                            </tr>
                        </thead>
                        <tbody id="tbodyOrderEmpty">
                            <tr>
                                <td colspan="21" style="text-align: center;">
                                    --暂无数据--
                                </td>
                            </tr>
                        </tbody>
                        <tbody id="tbodyOrderData" style="display: none;">
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <asp:Panel ID="Panel1" runat="server">
        <asp:Panel ID="Panel_ApproveInfo" runat="server" Visible="false">
            <blockquote class="layui-elem-quote" style="height: 20px; font-weight: bold;">
                审批记录</blockquote>
            <div id="approveInfoDiv" runat="server" style="margin-top: -10px;">
            </div>
        </asp:Panel>
        <div style="margin-top: 10px; text-align: center; margin-bottom: 30px;">
            <%--<asp:Button ID="btnBack" runat="server" Text="返 回" CssClass="layui-btn layui-btn-primary"
            Style="margin-left: 30px;" OnClick="btnBack_Click" />--%>
            <input type="button" value="返 回" class="layui-btn layui-btn-primary" onclick="javascript:window.history.go(-1)" />
        </div>
    </asp:Panel>
    </form>
</body>
</html>
<script src="js/checkOrderDetail.js" type="text/javascript"></script>
