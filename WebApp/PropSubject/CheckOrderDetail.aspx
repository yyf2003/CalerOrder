<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOrderDetail.aspx.cs"
    Inherits="WebApp.PropSubject.CheckOrderDetail" %>

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
            查看项目明细
        </p>
    </div>
    <div class="tr">
        >>项目信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                活动名称
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目创建人
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labAddUserName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomerName" runat="server" Text=""></asp:Label>
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
                区域
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labRegion" runat="server" Text=""></asp:Label>
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
    <div class="tr" style="margin-top: 20px;">
        >>订单信息
    </div>
    <div style="width: 100%; overflow: auto;">
        <asp:Repeater ID="Repeater1" runat="server" 
            onitemdatabound="Repeater1_ItemDataBound">
            <HeaderTemplate>
                <table class="table" style="width: 2500px;">
                    <tr class="tr_hui" style="font-weight: bold;">
                        <td colspan="15">
                            应收
                        </td>
                        <td colspan="7" style="border-left-color: #000;">
                            应付
                        </td>
                    </tr>
                    <tr class="tr_hui">
                        <td>
                            序号
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
                            工艺/服务描述
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
                            材料成本
                        </td>
                        <td>
                            加工成本
                        </td>
                        <td>
                            包装费
                        </td>
                        <td>
                            运输费
                        </td>
                        <td>
                            单价小计
                        </td>
                        <td>
                            合计金额
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
                    <td>
                        <%--道具名称--%>
                        <%#Eval("MaterialName")%>
                    </td>
                    <td>
                        <%--应用位置--%>
                        <%#Eval("Sheet")%>
                    </td>
                    <td>
                        <%--材料类型--%>
                        <%#Eval("MaterialType")%>
                    </td>
                    <td>
                        <%--尺寸规格--%>
                        <%#Eval("Dimension")%>
                    </td>
                    <td>
                        <%--工艺/服务描述--%>
                        <%#Eval("ServiceType")%>
                    </td>
                    <td>
                        <%--包装方式--%>
                        <%#Eval("Packaging")%>
                    </td>
                    <td>
                        <%--单位--%>
                        <%#Eval("UnitName")%>
                    </td>
                    <td>
                        <%--数量--%>
                        <%#Eval("Quantity")%>
                    </td>
                    <td>
                        <%--材料成本--%>
                        <%#Eval("BOMCost")%>
                    </td>
                    <td>
                        <%--加工成本--%>
                        <%#Eval("ProcessingCost")%>
                    </td>
                    <td>
                        <%--包装费--%>
                        <%#Eval("PackingCost")%>
                    </td>
                    <td>
                        <%--运输费--%>
                        <%#Eval("TransportationCost")%>
                    </td>
                    <td>
                        <%--单价小计--%>
                        <%#Eval("UnitPrice")%>
                    </td>
                    <td>
                        <%--合计金额--%>
                        <asp:Label ID="labReceiveSub" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <%--外协名称--%>
                        <%#Eval("OutsourceName")%>
                    </td>
                    <td>
                        <%--包装方式--%>
                        <%#Eval("PayPackaging")%>
                    </td>
                    <td>
                        <%--单位--%>
                        <%#Eval("PayUnitName")%>
                    </td>
                    <td>
                        <%--数量--%>
                        <%#Eval("PayQuantity")%>
                    </td>
                    <td>
                        <%--单价小计--%>
                        <%#Eval("PayUnitPrice")%>
                    </td>
                    <td>
                        <%--合计金额--%>
                        <asp:Label ID="labPaySub" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <%--备注--%>
                        <%#Eval("PayRemark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (Repeater1.Items.Count == 0)
                  {%>
                <tr class="tr_bai">
                    <td colspan="22">
                        --暂无数据--
                    </td>
                </tr>
                <%}
                  else
                  {
                  %>
                  <tr class="tr_hui">
                    <td colspan="14" style=" text-align:right; font-weight:bold;">
                        合计金额：
                    </td>
                    <td style=" color:Blue;">
                       <asp:Label ID="labReceiveTotal" runat="server" Text="0"></asp:Label>
                    </td>
                    <td colspan="5" style=" text-align:right;font-weight:bold;">
                      合计金额：
                    </td>
                    <td style=" color:Blue;">
                       <asp:Label ID="labPayTotal" runat="server" Text="0"></asp:Label>
                    </td>
                    <td></td>
                </tr>
                  <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <asp:Panel ID="Panel_Container1" runat="server" Visible="false">
        <asp:Panel ID="Panel1" runat="server" Visible="false">
            <br />
            <div class="tr">
                >>审批记录</div>
            <div id="approveInfoDiv" runat="server">
            </div>
        </asp:Panel>
        <div style="text-align: center; margin-top: 20px; margin-bottom: 30px;">
            <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton"
                style="width: 65px; height: 26px;" />
        </div>
    </asp:Panel>
    </form>
</body>
</html>
