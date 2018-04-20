<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckPOPPriceDetail.aspx.cs"
    Inherits="WebApp.OutsourcingOrder.Statistics.CheckPOPPriceDetail" %>

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
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table class="table">
            <tr>
               <td>店铺编号：</td>
               <td colspan="5" style=" text-align:left; padding-left:5px; height:35px;">
                   <asp:TextBox ID="txtShopNo" runat="server"></asp:TextBox>
                   &nbsp;&nbsp;
                   <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" OnClientClick="return check()"
                        Style="width: 65px; height: 26px;"
                       onclick="btnSearch_Click" />

               </td>
               <td>
                   
               </td>
            </tr>
            <tr class="tr_bai">
                <td style="text-align: center; width: 120px;">
                    店铺数量：
                </td>
                <td style="text-align: left; width: 150px; padding-left: 5px;">
                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    应付合计：
                </td>
                <td style="text-align: left; padding-left: 5px; ">
                    <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                    <asp:Label ID="labMaterialPrice" runat="server" Text="0" Visible="false"></asp:Label>
                    <asp:Label ID="labPOPFinallPrice" runat="server" Text="0" Visible="false"></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    应收合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labRTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
                <td style="width: 120px;">
                   
                </td>
            </tr>
        </table>
    </div>
    <div class="tr" style="height: 25px; margin-top: 8px;">
        》店铺信息
    </div>
    <div class="containerDiv">
        <div id="loadingImg" style="display: none;">
            <img src="../../image/WaitImg/loading1.gif" />
        </div>
        <asp:Repeater ID="gvPOP" runat="server">
            <HeaderTemplate>
                <table class="table" style="width: 1200px; margin-right: 10px;">
                    <tr class="tr_hui">
                        <td style="width: 40px;">
                            序号
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
                            Sheet
                        </td>
                        <td>
                            POP编号
                        </td>
                        <td>
                            POP数量
                        </td>
                        <td>
                            男/女
                        </td>
                        <td>
                            POP宽
                        </td>
                        <td>
                            POP高
                        </td>
                        <td>
                            POP材质
                        </td>
                        <td>
                            应付单价
                        </td>
                        <td>
                            应收单价
                        </td>
                        <td>
                            应付小计
                        </td>
                        <td>
                            应收小计
                        </td>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td style="width: 40px;">
                        <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                    </td>
                    
                    <td>
                        <%#Eval("ShopNo") %>
                    </td>
                    <td>
                        <%#Eval("ShopName") %>
                    </td>
                    <td>
                        <%#Eval("Region") %>
                    </td>
                    <td>
                        <%#Eval("Province") %>
                    </td>
                    <td>
                        <%#Eval("City") %>
                    </td>
                    <td>
                        <%#Eval("Sheet")%>
                    </td>
                    <td>
                        <%#Eval("GraphicNo")%>
                    </td>
                    <td>
                        <%#Eval("Quantity")%>
                    </td>
                    <td>
                        <%#Eval("Gender")%>
                    </td>
                    <td>
                        <%#Eval("GraphicWidth")%>
                    </td>
                    <td>
                        <%#Eval("GraphicLength")%>
                    </td>
                    <td>
                        <%#Eval("GraphicMaterial")%>
                    </td>
                    <td>
                        <%#Eval("UnitPrice")%>
                    </td>
                    <td>
                        <%#Eval("ReceiveUnitPrice")%>
                    </td>
                    <td>
                        <%#Eval("TotalPrice")%>
                    </td>
                    <td>
                        <%#Eval("ReceiveTotalPrice")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (gvPOP.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="17" style="text-align: center;">
                        --无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        $("#spanCheckMaterialPrice").click(function () {
            var guidanceMonth = $(this).data("guidancemonth");
            var customerId = $(this).data("customerid");
            var outsourceId = $(this).data("outsourceid");
            var url = "CheckMaterialPriceOrder.aspx?outsourceId=" + outsourceId + "&guidanceMonth=" + guidanceMonth + "&customerId=" + customerId;
            layer.open({
                type: 2,
                time: 0,
                title: '应收外协费用',
                skin: 'layui-layer-rim', //加上边框
                area: ['90%', '90%'],
                content: url,
                cancel: function () {

                }
            });
        })
    })
</script>
