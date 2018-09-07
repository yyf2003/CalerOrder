<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddOrderDetail.aspx.cs"
    Inherits="WebApp.Subjects.PriceOrder.AddOrderDetail" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
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
    <style type="text/css">
        .divi
        {
            float: left;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            添加费用订单
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style1">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                订单类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <div class="tr" style=" margin-top :20px;">
        >>导入订单明细</div>
    <table class="table">
        <%-- <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
               费用类型：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:RadioButtonList ID="rblOrderType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                </asp:RadioButtonList>
            </td>
        </tr>--%>
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                导入订单：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <div class="divi">
                    <asp:FileUpload ID="FileUpload1" runat="server" />
                </div>
                <div class="divi" style="padding-left: 20px;">
                    <asp:CheckBox ID="cbAdd" runat="server" />追加订单（保留原数据）
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                下载订单模板：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:LinkButton ID="lbDownLoad" runat="server" OnClick="lbDownLoad_Click">下载模板</asp:LinkButton>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
            </td>
            <td style="text-align: left; padding-left: 5px; height: 30px;">
                <div id="showButton">
                    <asp:Button ID="btnImport" runat="server" Text="导 入" OnClientClick="return checkFile()"
                        class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnImport_Click" />
                </div>
                <div id="showWaiting" style="color: Red; display: none;">
                    <img src='../../Image/WaitImg/loadingA.gif' />正在导入，请稍等...
                </div>
            </td>
        </tr>
    </table>
    <div>
        <asp:Panel ID="Panel1" runat="server" Visible="false">
            <table>
                <tr class="tr_bai">
                    <td style="width: 120px; height: 50px;">
                    </td>
                    <td class="nav_table_tdleft" style="vertical-align: top;">
                        <asp:Label ID="labState" runat="server" Text="导入完成" Style="color: Red; font-weight: bold;
                            font-size: 16px;"></asp:Label>
                        <br />
                        <asp:Label ID="labTips" runat="server" Text="" Style="color: blue; font-size: 14px;"></asp:Label>
                        <div id="ExportFailMsg" runat="server" style="display: none;">
                            <asp:LinkButton ID="lbExportError" runat="server" OnClick="lbExportError_Click" Style="text-decoration: underline;">导出失败信息</asp:LinkButton>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <asp:Panel ID="Panel_OrderList" runat="server" Visible="false">
        <div class="tr" style=" margin-top :20px;">
            >>订单信息</div>
        <div class="containerDiv">
            <asp:Repeater ID="orderListRepeater" runat="server" 
                onitemdatabound="orderListRepeater_ItemDataBound">
                <HeaderTemplate>
                    <table class="table">
                        <tr class="tr_hui">
                            <td style="width: 50px;">
                                序号
                            </td>
                            <td>
                                费用类型
                            </td>
                            <td>
                                店铺编号
                            </td>
                            <td>
                                店铺名称
                            </td>
                            <td>
                                区域
                            </td>
                            <td>
                                省份
                            </td>
                            <td>
                                城市
                            </td>
                            <td>
                                应收费用金额
                            </td>
                            <td>
                                应付费用金额
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                费用内容
                            </td>
                            <td>
                                备注
                            </td>
                            <td>
                                外协
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="tr_bai">
                        <td>
                            <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                        </td>
                        <td>
                            <asp:Label ID="labOrderType" runat="server" Text=""></asp:Label>
                        </td>
                        <td>
                            <%#Eval("order.ShopNo")%>
                        </td>
                        <td>
                            <%#Eval("order.ShopName")%>
                        </td>
                        <td>
                            <%#Eval("order.Region")%>
                        </td>
                        <td>
                            <%#Eval("order.Province")%>
                        </td>
                        <td>
                            <%#Eval("order.City")%>
                        </td>
                        <td>
                            <%#Eval("order.Amount")%>
                        </td>
                        <td>
                            <%#Eval("order.PayAmount")%>
                        </td>
                        <td>
                            <%#Eval("order.Quantity")%>
                        </td>
                       
                        <td>
                            <%#Eval("order.Contents")%>
                        </td>
                        <td>
                            <%#Eval("order.Remark")%>
                        </td>
                        <td>
                           <%#Eval("CompanyName")%>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%if (orderListRepeater.Items.Count == 0)
                      { %>
                    <tr class="tr_bai">
                        <td colspan="13" style="text-align: center;">
                            --暂无数据--
                        </td>
                    </tr>
                    <%} %>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div style="text-align: center; margin-top: 5px;">
            <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
                CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
            </webdiyer:AspNetPager>
        </div>
    </asp:Panel>


    <div style="text-align: center; margin-bottom:30px; margin-top :30px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return Check()" class="easyui-linkbutton" style="width: 65px;
            height: 26px; font-size:13px;" onclick="btnSubmit_Click"/>
        <img id="loadingImg" src="../../image/WaitImg/loadingA.gif" style="display: none;" />
         &nbsp;&nbsp;
       <asp:Button ID="btnBack" runat="server" Text="返 回"  class="easyui-linkbutton" style="width: 65px;
            height: 26px; font-size:13px;" onclick="btnBack_Click"/>
           
    </div>


    <asp:HiddenField ID="hfGuidanceId" runat="server" Value="0" />
    </form>
</body>
</html>
<script type="text/javascript">
    function checkFile() {
        var val = $("#FileUpload1").val();
        if (val != "") {
            var extent = val.substring(val.lastIndexOf('.') + 1);
            if (extent != "xls" && extent != "xlsx") {

                alert('只能上传Excel文件');
                return false;
            }
        }
        else {

            alert('请选择文件');
            return false;
        }
        $("#showButton").hide();
        $("#showWaiting").show();
        return true;
    }

    function Check() {
        $("#loadingImg").show();
    }
</script>
