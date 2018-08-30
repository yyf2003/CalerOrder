<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutsourceOrderList.aspx.cs"
    Inherits="WebApp.Statistics.PropStatistics.OutsourceOrderList" %>

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
</head>
<body>
    <form id="form1" runat="server">
    <div class="tr">
        》道具订单明细
    </div>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    总金额：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 150px;">
                    <asp:Button ID="btnExport" runat="server" Text="导 出" class="easyui-linkbutton" OnClientClick="return check()"
                        Style="width: 65px; height: 26px;" OnClick="btnExport_Click" />
                    <img id="exportWaiting" style="display: none;" src="/image/WaitImg/loadingA.gif" />
                </td>
            </tr>
        </table>
    </div>
    <div class="tr" style="margin-top: 20px;">
        >>订单信息
    </div>
    <div style="width: 100%; overflow: auto;">
        <asp:Repeater ID="Repeater1" runat="server" OnItemDataBound="Repeater1_ItemDataBound">
            <HeaderTemplate>
                <table class="table" style="width: 1800px;">
                    <tr class="tr_hui">
                        <td>
                            序号
                        </td>
                        <td>
                            活动名称
                        </td>
                        <td>
                            项目名称
                        </td>
                        <td>
                            道具名称
                        </td>
                        <td>
                            应用位置
                        </td>
                        <td>
                            材料类型
                        </td>
                        <td>
                            尺寸规格
                        </td>
                       
                        <td>
                            外协名称
                        </td>
                        <td>
                            包装方式
                        </td>
                        <td>
                            单位
                        </td>
                        <td>
                            数量
                        </td>
                        <td>
                            单价小计
                        </td>
                        
                        <td>
                            合计金额
                        </td>
                        <td>
                            备注
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td>
                        <%--序号--%>
                        <%#Container.ItemIndex+1 %>
                    </td>
                    <td runat="server" id="tbGuidanceName">
                        <%--活动名称--%>
                        <%#Eval("guidance.ItemName")%>
                    </td>
                    <td runat="server" id="tbSubjectName">
                        <%--项目名称--%>
                        <%#Eval("subject.SubjectName")%>
                    </td>
                    <td runat="server" id="tbMaterialName">
                        <%--道具名称--%>
                        <%#Eval("rOrder.MaterialName")%>
                    </td>
                    <td runat="server" id="tbSheet">
                        <%--应用位置--%>
                        <%#Eval("rOrder.Sheet")%>
                    </td>
                    <td runat="server" id="tbMaterialType">
                        <%--材料类型--%>
                        <%#Eval("rOrder.MaterialType")%>
                    </td>
                    <td runat="server" id="tbDimension">
                        <%--尺寸规格--%>
                        <%#Eval("rOrder.Dimension")%>
                    </td>
                  
                    <td>
                        <%--外协名称--%>
                        <%#Eval("oOrder.OutsourceName")%>
                    </td>
                    <td>
                        <%--包装方式--%>
                        <%#Eval("oOrder.Packaging")%>
                    </td>
                    <td>
                        <%--单位--%>
                        <%#Eval("oOrder.UnitName")%>
                    </td>
                    <td>
                        <%--数量--%>
                        <%#Eval("oOrder.Quantity")%>
                    </td>
                    <td>
                        <%--单价小计--%>
                        <%#Eval("oOrder.UnitPrice")%>
                    </td>
                    
                    
                    <td>
                        <%--合计金额--%>
                        <asp:Label ID="labPaySub" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <%--备注--%>
                        <%#Eval("oOrder.Remark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (Repeater1.Items.Count == 0)
                  {%>
                <tr class="tr_bai">
                    <td colspan="14">
                        --暂无数据--
                    </td>
                </tr>
                <%}
                  else
                  {
                %>
                <tr class="tr_hui">
                    <td colspan="12" style="text-align: right; font-weight: bold;">
                        合计金额：
                    </td>
                    <td style="color: Blue;">
                        <asp:Label ID="labPayTotal" runat="server" Text="0"></asp:Label>
                    </td>
                    <td></td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
