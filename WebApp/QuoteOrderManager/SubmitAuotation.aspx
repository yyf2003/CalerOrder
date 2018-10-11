<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubmitAuotation.aspx.cs"
    Inherits="WebApp.QuoteOrderManager.SubmitAuotation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script type="text/javascript">
        function finish(msg) {
            if (msg == "ok") {
                window.parent.Finish();
            }
            else {
                layer.msg("提交失败!");
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    活动月份
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labGuidanceMonth" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    活动名称
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    项目类型
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSubjectCategory" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    项目名称
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labSubjectNames" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    VVIP店铺
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:Label ID="labVVIPShop" runat="server" Text="0"></asp:Label>
                </td>
                <td style="width: 120px;">
                    安装店铺
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:Label ID="labInstallShop" runat="server" Text="0"></asp:Label>
                </td>
                <td style="width: 120px;">
                    常规店铺
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labNormalShop" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    报价项目名称
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlQuoteSubject" runat="server">
                        <asp:ListItem Value="0">--请选择报价项目--</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td colspan="5" style="text-align: left; padding-left: 5px; height: 40px;">
                    <asp:Button ID="btnExport" runat="server" Text="导出报价单" class="layui-btn layui-btn-small"
                        OnClick="btnExport_Click" />
                </td>
            </tr>
        </table>
        <asp:Button ID="btnRefresh" runat="server" Text="Button" OnClick="btnRefresh_Click"
            Style="display: none;" />
    </div>
    <div class="tr">
        》POP订单统计
    </div>
    <asp:Repeater ID="popList" runat="server" 
        onitemdatabound="popList_ItemDataBound">
        <HeaderTemplate>
            <table class="table" id="popTable">
                <tr class="tr_hui">
                    <td rowspan="2">
                        画面位置
                    </td>
                    <td colspan="2" style="font-weight: bold;">
                        系统订单金额统计
                    </td>
                </tr>
                <tr class="tr_hui">
                    <td style="width: 200px;">
                        尺寸/数量
                    </td>
                    <td>
                        金额
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td runat="server" id="sheet" style="height: 30px;">
                   <%-- <span style="float: left; left: 10px; margin-left: 10px; margin-top: 10px;">
                        <input type="checkbox" name="cbOneSheet" value='<%#Eval("PositionDescription") %>' />
                    </span><span name="popSheetSpan" style="float: left; margin-left: 20px; cursor: pointer;
                        text-decoration: underline; color: Blue;">--%>
                        <%#Eval("Sheet") %></span>
                </td>
                <td>
                    <%--画面尺寸/物料数量--%>
                    <%-- <asp:Label ID="labQuantity" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("Amount") != null ? (Math.Round(decimal.Parse(Eval("Amount").ToString()), 2)).ToString() : ""%>
                </td>
                <td>
                    <%--金额--%>
                    <%--<asp:Label ID="labSubPrice" runat="server" Text=""></asp:Label>--%>
                    <%#Eval("TotalPrice")!=null?(Math.Round(decimal.Parse(Eval("TotalPrice").ToString()),2)).ToString():""%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (popList.Items.Count == 0)
              { %>
            <tr class="tr_bai">
                <td colspan="3" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%}
              else
              {
            %>
            <tr class="tr_bai">
                <td style="text-align: right; font-weight:bold;">
                    合计：
                </td>
                <td>
                    <asp:Label ID="labPOPTotalArea" runat="server" Text="0"></asp:Label>
                </td>
                <td style="text-align: center;">
                    <asp:Label ID="labPOPTotalPrice" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div class="tr">
        》T1-T3安装费统计
    </div>
    <table class="table" id="basicInstallPriceTB">
        <tr class="tr_hui">
            <td rowspan="2" style="width: 180px; ">
                费用名称
            </td>
            <td rowspan="2">
                类型
            </td>
            <td rowspan="2">
                单位
            </td>
            <td rowspan="2">
                费用说明
            </td>
            <td rowspan="2">
                单价
            </td>
            <td colspan="2" style=" font-weight:bold;">
                系统
            </td>
           
        </tr>
        <tr class="tr_hui">
            <td>
                发生数量
            </td>
            <td style=" width:120px;">
                金额
            </td>
            
        </tr>
        <tr class="tr_bai">
            <td rowspan="4">
                户外安装费
            </td>
            <td>
                <%--一层门头以上位置 （距水平地面高度在6M以上）--%>
            </td>
            <td>
                店
            </td>
            <td>
                <%--须提供设备租赁证明及操作人员高空作业证--%>
            </td>
            <td>
                5000
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel1Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel1" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td>
                <%--一层门头以上位置 （距水平地面高度在6M以内）--%>
            </td>
            <td>
                店
            </td>
            <td>
                <%--须提供吊兰租赁证明及操作人员高空作业证--%>
            </td>
            <td>
                2700
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel2Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel2" runat="server" Text="0"></asp:Label>
            </td>
          
        </tr>
        <tr class="tr_bai">
            <td>
               <%-- 一层门头以上位置 （距水平地面高度在3M以内）--%>
            </td>
            <td>
                店
            </td>
            <td>
                <%--须提供吊兰租赁证明及操作人员高空作业证--%>
            </td>
            <td>
                1800
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel3Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel3" runat="server" Text="0"></asp:Label>
            </td>
          
        </tr>
        <tr class="tr_bai">
            <td>
                <%--一层门头位置--%>
            </td>
            <td>
                店
            </td>
            <td>
            </td>
            <td>
                600
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel4Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labOOHLevel4" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td rowspan="8">
                店铺安装费
            </td>
            <td>
                Core-VVIP店铺
            </td>
            <td>
                店
            </td>
            <td>
                橱窗部分
            </td>
            <td>
                1000
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel1Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel1" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td>
                Core-VVIP店铺
            </td>
            <td>
                店
            </td>
            <td>
                店内其它
            </td>
            <td>
                800
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel1Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel1" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td>
                Core-Premium店铺
            </td>
            <td>
                店
            </td>
            <td>
                橱窗部分
            </td>
            <td>
                500
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel2Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel2" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td>
                Core-Premium店铺
            </td>
            <td>
                店
            </td>
            <td>
                店内其它
            </td>
            <td>
                400
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel2Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel2" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td>
                Core-Basic店铺
            </td>
            <td>
                店
            </td>
            <td>
                橱窗部分
            </td>
            <td>
                200
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel3Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labWindowLevel3" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td>
                Core-Basic店铺
            </td>
            <td>
                店
            </td>
            <td>
                店内其它
            </td>
            <td>
                150
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel3Count" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labBasicLevel3" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td>
                其它类型店铺-带橱窗
            </td>
            <td>
                店
            </td>
            <td>
                橱窗+店内
            </td>
            <td>
                500
            </td>
            <td class="output">
                <asp:Label ID="labKidsWindowLevelCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labKidsWindowLevel" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td>
                其它类型店铺-不带橱窗
            </td>
            <td>
                店
            </td>
            <td>
                橱窗+店内
            </td>
            <td>
                150
            </td>
            <td class="output">
                <asp:Label ID="labKidsBasicLevelCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labKidsBasicLevel" runat="server" Text="0"></asp:Label>
            </td>
           
           
        </tr>
        <tr class="tr_bai">
            <td>
                Generic POP 更换安装
            </td>
            <td>
            </td>
            <td>
                店
            </td>
            <td>
            </td>
            <td>
                150
            </td>
            <td class="output">
                <asp:Label ID="labGenericLevelCount" runat="server" Text="0"></asp:Label>
            </td>
            <td class="output">
                <asp:Label ID="labGenericLevel" runat="server" Text="0"></asp:Label>
            </td>
           
        </tr>
        <tr class="tr_bai">
            <td colspan="6" style="text-align: right;font-weight:bold;">
                合计：
            </td>
            <td>
                <asp:Label ID="labInstallPriceTotal" runat="server" Text="0"></asp:Label>
            </td>
           
           
        </tr>
    </table>

    <div class="tr">
        》快递费/道具费统计
    </div>
    <asp:Repeater ID="expressPriceListRepeater" runat="server" OnItemDataBound="expressPriceListRepeater_ItemDataBound">
        <HeaderTemplate>
            <table class="table">
                <tr class="tr_hui">
                    <td>
                        道具名称
                    </td>
                    <td>
                        类型
                    </td>
                    <td>
                        单价
                    </td>
                   <td>
                        数量
                    </td>
                    <td>
                        金额
                    </td>
                </tr>
               
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td runat="server" id="sheet">
                    <%--道具名称--%>
                    <%#Eval("PriceType")%>
                </td>
                <td>
                  <%#Eval("OrderType")%>
                </td>
                <td>
                    <%--单价--%>
                    <%#Eval("Price")%>
                </td>
                <td>
                    <%--数量--%>
                    <%#Eval("Count")%>
                </td>
                <td>
                    <%--金额--%>
                    <asp:Label ID="labSubPrice" runat="server" Text=""></asp:Label>
                </td>
               
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (expressPriceListRepeater.Items.Count == 0)
              { %>
            <tr class="tr_bai">
                <td colspan="5" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%}
              else
              {
            %>
            <tr class="tr_bai">
                <td colspan="4" style="text-align: right;font-weight:bold;">
                    合计：
                </td>
                <td style="text-align: center;">
                    <asp:Label ID="labExpressPriceTotal" runat="server" Text="0"></asp:Label>
                </td>
                
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>

     <div class="tr" style="margin-top: 20px;">
        》其他级别安装费统计
    </div>
    <asp:Repeater ID="otherInstallPriceRepeater" runat="server" OnItemDataBound="otherInstallPriceRepeater_ItemDataBound">
        <HeaderTemplate>
            <table class="table" id="otherInstallPriceTable">
                <tr class="tr_hui" style="font-weight: bold;">
                    <td>
                        费用名称
                    </td>
                    <td>
                        单位
                    </td>
                    <td>
                        单价
                    </td>
                    <td>
                        发生数量
                    </td>
                    <td>
                        金额
                    </td>
                    <td>
                        合计
                    </td>
                    <td>
                        剩余
                    </td>
                    <td style="color:Blue;">
                        报价折算明细
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td id="priceType" runat="server">
                    <%#Eval("PriceType")%>
                </td>
                <td>
                    店
                </td>
                <td>
                    <%#Eval("Price")%>
                </td>
                <td>
                    <%#Eval("Count")%>
                </td>
                <td>
                    <%--<%#decimal.Parse(Eval("Price").ToString()) * int.Parse(Eval("Count").ToString())%>--%>
                    <asp:Label ID="labPrice" runat="server" Text=""></asp:Label>
                </td>
                <td id="subPrice" runat="server">
                    <asp:Label ID="labSubPrice" runat="server" Text=""></asp:Label>
                </td>
                <td id="subPriceLeft" runat="server">
                    <asp:Label ID="labsubPriceLeft" runat="server" Text=""></asp:Label>
                </td>
                <td id="operatePrice" runat="server" style="text-align: left; padding-left: 10px;">
                    <asp:HiddenField ID="hfChangeType" runat="server" />
                    <asp:Panel ID="PanelPOP" runat="server">
                        <div>
                            <span style="font-weight: bolder;">POP</span>： 位置:
                            <asp:DropDownList ID="ddlSheet" runat="server">
                                <asp:ListItem Value="">--请选择--</asp:ListItem>
                               <%-- <asp:ListItem Value="橱窗">橱窗</asp:ListItem>
                                <asp:ListItem Value="陈列桌">陈列桌</asp:ListItem>
                                <asp:ListItem Value="服装墙">服装墙</asp:ListItem>
                                <asp:ListItem Value="鞋墙">鞋墙</asp:ListItem>
                                <asp:ListItem Value="SMU">SMU</asp:ListItem>
                                <asp:ListItem Value="中岛">中岛</asp:ListItem>
                                <asp:ListItem Value="收银台">收银台</asp:ListItem>
                                <asp:ListItem Value="OOH">OOH</asp:ListItem>--%>
                            </asp:DropDownList>
                            &nbsp;&nbsp;材质:
                            <asp:DropDownList ID="ddlMaterial" runat="server">
                                <asp:ListItem Value="">请选择材质</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;&nbsp;面积:
                            <asp:TextBox ID="txtPOPArea" runat="server" MaxLength="5" Style="width: 60px; text-align: center;"></asp:TextBox><span
                                style="color: Blue;">平米</span>
                            <input name="btnAddPOPQuote" type="button" value="添加" class="layui-btn layui-btn-small"
                                style="margin-left: 10px; height: 20px; line-height: 20px;" />
                            &nbsp;&nbsp; 合计：
                            <asp:Label ID="labPOPTotal" runat="server" Text="0"></asp:Label>
                            <div style="margin-left: 40px; text-align: left;">
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="PanelOOH" runat="server">
                        <span style="font-weight: bolder;">安装费</span>： 5000:
                        <input type="button" name="btnOOHDec" value="-" style="width: 20px;" />
                        <asp:TextBox ID="txtOOHPriceCount1" runat="server" data-val="5000" Text="0" MaxLength="3"
                            Style="width: 30px; text-align: center;"></asp:TextBox>
                        <input type="button" name="btnOOHAdd" value="+" style="width: 20px;" />
                        &nbsp;&nbsp;2700:
                        <input type="button" name="btnOOHDec" value="-" style="width: 20px;" />
                        <asp:TextBox ID="txtOOHPriceCount2" runat="server" data-val="2700" Text="0" MaxLength="3"
                            Style="width: 30px; text-align: center;"></asp:TextBox>
                        <input type="button" name="btnOOHAdd" value="+" style="width: 20px;" />
                        &nbsp;&nbsp;1800:
                        <input type="button" name="btnOOHDec" value="-" style="width: 20px;" />
                        <asp:TextBox ID="txtOOHPriceCount3" runat="server" data-val="1800" Text="0" MaxLength="3"
                            Style="width: 30px; text-align: center;"></asp:TextBox>
                        <input type="button" name="btnOOHAdd" value="+" style="width: 20px;" />
                        &nbsp;&nbsp;600:
                        <input type="button" name="btnOOHDec" value="-" style="width: 20px;" />
                        <asp:TextBox ID="txtOOHPriceCount4" runat="server" data-val="600" Text="0" MaxLength="3"
                            Style="width: 30px; text-align: center;"></asp:TextBox>
                        <input type="button" name="btnOOHAdd" value="+" style="width: 20px;" />
                        &nbsp;&nbsp; 合计：
                        <asp:Label ID="labOOHTotal" runat="server" Text="0"></asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="PanelBasic" runat="server">
                        <span style="font-weight: bolder;">安装费</span>： 800:
                        <input type="button" name="btnBasicDec" value="-" style="width: 20px;" />
                        <asp:TextBox ID="txtBasicPriceCount1" runat="server" Text="0" data-val="800" MaxLength="3"
                            Style="width: 30px; text-align: center;"></asp:TextBox>
                        <input type="button" name="btnBasicAdd" value="+" style="width: 20px;" />
                        &nbsp;&nbsp;400:
                        <input type="button" name="btnBasicDec" value="-" style="width: 20px;" />
                        <asp:TextBox ID="txtBasicPriceCount2" runat="server" Text="0" data-val="400" MaxLength="3"
                            Style="width: 30px; text-align: center;"></asp:TextBox>
                        <input type="button" name="btnBasicAdd" value="+" style="width: 20px;" />
                        &nbsp;&nbsp;150:
                        <input type="button" name="btnBasicDec" value="-" style="width: 20px;" />
                        <asp:TextBox ID="txtBasicPriceCount3" runat="server" Text="0" data-val="150" MaxLength="3"
                            Style="width: 30px; text-align: center;"></asp:TextBox>
                        <input type="button" name="btnBasicAdd" value="+" style="width: 20px;" />
                        &nbsp;&nbsp;合计：
                        <asp:Label ID="labBasicTotal" runat="server" Text="0"></asp:Label>
                    </asp:Panel>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (otherInstallPriceRepeater.Items.Count == 0)
              { %>
            <tr class="tr_bai">
                <td colspan="8" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>

     <div class="tr">
        》其他费用订单统计
    </div>
    <asp:Repeater ID="otherPriceOrderRepeater" runat="server" 
        onitemcreated="otherPriceOrderRepeater_ItemCreated">
        <HeaderTemplate>
            <table class="table" id="otherOrderPriceTable">
                <tr class="tr_hui" style="font-weight: bold;">
                    <td>
                        费用名称
                    </td>
                    <td>
                        单位
                    </td>
                    <td>
                        发生数量
                    </td>
                    <td>
                        金额
                    </td>
                    <td>
                        剩余
                    </td>
                    <td style="color:Blue;">
                        报价折算明细
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai">
                <td id="priceType" runat="server">
                    <%#Eval("PriceType")%>
                </td>
                <td>
                    店
                </td>
                <td>
                    <%#Eval("Count")%>
                </td>
                <td>
                    <%--<%#decimal.Parse(Eval("Price").ToString()) * int.Parse(Eval("Count").ToString())%>--%>
                    <%#Eval("TotalPrice")%>
                </td>
                <td id="subPriceLeft" runat="server">
                    <asp:Label ID="labsubPriceLeft" runat="server" Text=""></asp:Label>
                </td>
                <td style="text-align: left; padding-left: 10px;">
                    <asp:HiddenField ID="hfChangeType" runat="server" Value='<%#Eval("ChangeType") %>'/>
                    <asp:Panel ID="PanelPOP_OtherPriceOrder" runat="server">
                        <div>
                            <span style="font-weight: bolder;">POP</span>： 位置:
                            <asp:DropDownList ID="ddlSheet_OtherPriceOrder" runat="server">
                                <asp:ListItem Value="">--请选择--</asp:ListItem>
                               <%-- <asp:ListItem Value="橱窗">橱窗</asp:ListItem>
                                <asp:ListItem Value="陈列桌">陈列桌</asp:ListItem>
                                <asp:ListItem Value="服装墙">服装墙</asp:ListItem>
                                <asp:ListItem Value="鞋墙">鞋墙</asp:ListItem>
                                <asp:ListItem Value="SMU">SMU</asp:ListItem>
                                <asp:ListItem Value="中岛">中岛</asp:ListItem>
                                <asp:ListItem Value="收银台">收银台</asp:ListItem>
                                <asp:ListItem Value="OOH">OOH</asp:ListItem>--%>
                            </asp:DropDownList>
                            &nbsp;&nbsp;材质:
                            <asp:DropDownList ID="ddlMaterial_OtherPriceOrder" runat="server">
                                <asp:ListItem Value="">请选择材质</asp:ListItem>
                            </asp:DropDownList>
                            &nbsp;&nbsp;面积:
                            <asp:TextBox ID="txtPOPArea_OtherPriceOrder" runat="server" MaxLength="5" Style="width: 60px; text-align: center;"></asp:TextBox><span
                                style="color: Blue;">平米</span>
                            <input name="btnAddPOPQuote" type="button" value="添加" class="layui-btn layui-btn-small"
                                style="margin-left: 10px; height: 20px; line-height: 20px;" />
                            &nbsp;&nbsp; 合计：
                            <asp:Label ID="labPOPTotal_OtherPriceOrder" runat="server" Text="0"></asp:Label>
                            <div style="margin-left: 40px; text-align: left;">
                            </div>
                        </div>
                    </asp:Panel>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (otherPriceOrderRepeater.Items.Count == 0)
              { %>
            <tr class="tr_bai">
                <td colspan="6" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div style="text-align: center; height: 50px; margin-top: 30px; margin-bottom: 30px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" class="layui-btn layui-btn-normal"
            OnClick="btnSubmit_Click" OnClientClick="return Check()" />
        <asp:HiddenField ID="hfPOPQuoteJson" runat="server" />
      
    </div>
    </form>
</body>
</html>

<script src="js/submitQuotation.js" type="text/javascript"></script>
