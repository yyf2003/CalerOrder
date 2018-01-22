<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Approve.aspx.cs" Inherits="WebApp.Subjects.HandMadeOrder.Approve" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../../easyui1.4/jquery.easyui.min.js"></script>
    <script src="../../easyui1.4/jquery.min.js" type="text/javascript"></script>
    <link href="../../bootstrap-3.3.2-dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script src="../../bootstrap-3.3.2-dist/js/bootstrap.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            项目审批
        </p>
    </div>
    <div class="tr">
        >>项目信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                内部项目编号
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
            </td>
            <td style="text-align: left; padding-left: 5px;">
            </td>
        </tr>
        <tr class="tr_bai">
            <td class="style1">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style2">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
            <td class="style1">
                外部项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style3">
                <asp:Label ID="labOutSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                开始时间
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labBeginDate" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                结束时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labEndDate" runat="server" Text=""></asp:Label>
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
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
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
    <div class="tab" style="margin-top: 10px;">
        订单信息
        <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
    </div>
    <ul id="myTab" class="nav nav-tabs" style="background-color: #dce0e9;">
        <li class="active"><a href="#pop" data-toggle="tab">POP订单</a></li>
        <%--<li><a href="#hc" data-toggle="tab">HC订单</a></li>--%>
    </ul>
    <div id="myTabContent" class="tab-content" style="overflow: auto;">
        <div class="tab-pane fade in active" id="pop" style="padding: 5px;">
            <table>
                <tr>
                    <td style="width: 60px; color: Blue;">
                        搜索：
                    </td>
                    <td style="text-align: left;">
                        店铺编号：<asp:TextBox ID="txtShopNo1" runat="server"></asp:TextBox>
                        &nbsp;
                        <asp:Button ID="btnSreach1" runat="server" Text="搜 索" OnClick="btnSreach1_Click"
                            class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Repeater ID="popList" runat="server">
                        <HeaderTemplate>
                            <table class="table1" style="width: 100%;">
                                <tr class="tr_hui">
                                    <td style="width: 30px;">
                                        序号
                                    </td>
                                    <td>
                                        店铺编号
                                    </td>
                                    <td>
                                        店铺名称
                                    </td>
                                    <td>
                                        物料支持级别
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
                                        位置
                                    </td>
                                    <td>
                                        位置描述
                                    </td>
                                    <td>
                                        级别
                                    </td>
                                    <td>
                                        性别
                                    </td>
                                    <td>
                                        数量
                                    </td>
                                    <td>
                                        长
                                    </td>
                                    <td>
                                        宽
                                    </td>
                                    <td>
                                        材质
                                    </td>
                                    <td>
                                        选图
                                    </td>
                                    <td>
                                        备注
                                    </td>
                                    <td>
                                        操作
                                    </td>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="width: 30px;">
                                    <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                                </td>
                                <td>
                                    <%#Eval("shop.ShopNo") %>
                                </td>
                                <td>
                                    <%#Eval("shop.ShopName") %>
                                </td>
                                <td>
                                    <%#Eval("order.MaterialSupport")%>
                                </td>
                                <td>
                                    <%#Eval("shop.RegionName") %>
                                </td>
                                <td>
                                    <%#Eval("shop.ProvinceName") %>
                                </td>
                                <td>
                                    <%#Eval("shop.CityName") %>
                                </td>
                                <td>
                                    <%#Eval("order.Sheet") %>
                                </td>
                                <td>
                                    <%#Eval("order.PositionDescription")%>
                                </td>
                                <td>
                                    <%#Eval("order.LevelNum")%>
                                </td>
                                <td>
                                    <%#Eval("order.Gender")%>
                                </td>
                                <td>
                                    <%#Eval("order.Quantity")%>
                                </td>
                                <td>
                                    <%#Eval("order.GraphicWidth")%>
                                </td>
                                <td>
                                    <%#Eval("order.GraphicLength")%>
                                </td>
                                <td>
                                    <%#Eval("order.GraphicMaterial")%>
                                </td>
                                <td>
                                    <%#Eval("order.ChooseImg")%>
                                </td>
                                <td>
                                    <%#Eval("order.Remark")%>
                                </td>
                                <td>
                                    <%#Eval("order.Operation")%>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div style="text-align: center; margin-top: 10px;">
                        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
                            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
                        </webdiyer:AspNetPager>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div class="tab-pane fade" id="hc" style="padding: 5px;">
            <table>
                <tr>
                    <td style="width: 60px; color: Blue;">
                        搜索：
                    </td>
                    <td style="text-align: left;">
                        店铺编号：<asp:TextBox ID="txtShopNo2" runat="server"></asp:TextBox>
                        &nbsp;
                        <asp:Button ID="btnSreach2" runat="server" Text="搜 索" OnClick="btnSreach2_Click"
                            class="easyui-linkbutton" Style="width: 65px; height: 26px; margin-bottom: 5px;" />
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <asp:Repeater ID="hcList" runat="server">
                        <HeaderTemplate>
                            <table class="table1" style="width: 100%;">
                                <tr class="tr_hui">
                                    <td style="width: 30px;">
                                        序号
                                    </td>
                                    <td>
                                        店铺编号
                                    </td>
                                    <td>
                                        店铺名称
                                    </td>
                                    <td>
                                        物料支持级别
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
                                        位置
                                    </td>
                                    <td>
                                        位置描述
                                    </td>
                                    <td>
                                        级别
                                    </td>
                                    <td>
                                        性别
                                    </td>
                                    <td>
                                        数量
                                    </td>
                                    <td>
                                        长
                                    </td>
                                    <td>
                                        宽
                                    </td>
                                    <td>
                                        材质
                                    </td>
                                    <td>
                                        选图
                                    </td>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="width: 30px;">
                                    <%#(AspNetPager2.CurrentPageIndex-1)*AspNetPager2.PageSize+ Container.ItemIndex + 1%>
                                </td>
                                <td>
                                    <%#Eval("shop.ShopNo") %>
                                </td>
                                <td>
                                    <%#Eval("shop.ShopName") %>
                                </td>
                                <td>
                                    <%#Eval("order.MaterialSupport")%>
                                </td>
                                <td>
                                    <%#Eval("shop.RegionName") %>
                                </td>
                                <td>
                                    <%#Eval("shop.ProvinceName") %>
                                </td>
                                <td>
                                    <%#Eval("shop.CityName") %>
                                </td>
                                <td>
                                    <%#Eval("order.Sheet") %>
                                </td>
                                <td>
                                    <%#Eval("pop.PositionDescription")%>
                                </td>
                                <td>
                                    <%#Eval("order.LevelNum")%>
                                </td>
                                <td>
                                    <%#Eval("order.Gender")%>
                                </td>
                                <td>
                                    <%#Eval("order.Quantity")%>
                                </td>
                                <td>
                                    <%#Eval("pop.GraphicWidth")%>
                                </td>
                                <td>
                                    <%#Eval("pop.GraphicLength")%>
                                </td>
                                <td>
                                    <%#Eval("pop.GraphicMaterial")%>
                                </td>
                                <td>
                                    <%#Eval("order.ChooseImg")%>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                    <div style="text-align: center; margin-top: 10px;">
                        <webdiyer:AspNetPager ID="AspNetPager2" runat="server" PageSize="20" CssClass="paginator"
                            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager2_PageChanged">
                        </webdiyer:AspNetPager>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <br />
    <div class="tr">
        >>审批信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 100px;">
                审批结果
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:RadioButtonList ID="rblApproveResult" runat="server" RepeatDirection="Horizontal"
                    RepeatLayout="Flow">
                    <asp:ListItem Value="1">通过 </asp:ListItem>
                    <asp:ListItem Value="2">不通过 </asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                审批意见
            </td>
            <td style="text-align: left; padding-left: 5px; height: 80px;">
                <asp:TextBox ID="txtRemark" runat="server" Columns="60" Rows="5" TextMode="MultiLine"
                    MaxLength="100"></asp:TextBox>
                (100字以内)
            </td>
        </tr>
    </table>
    <div id="btnDiv" style="text-align: center; margin-bottom: 20px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return Check()"
            class="easyui-linkbutton" Style="width: 65px; height: 26px; font-size: 13px;"
            OnClick="btnSubmit_Click" />
        &nbsp;&nbsp;
        <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton"
            style="width: 65px; height: 26px; font-size: 13px;" />
    </div>
    <div id="loadingApprove" style="text-align: center; margin-bottom: 20px; display:none;">
        <img src="../../image/WaitImg/loading1.gif" />
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    $(function () {


        $("#txtRemark").on("keyup", function () {
            var val = $(this).val();
            if (val.length > 100) {
                $(this).val(val.substring(0, 100));
            }
        })
    })


    function Check() {
        var result = $("input:radio[name='rblApproveResult']:checked").val() || 0;
        if (result == 0) {
            alert("请选择审批结果");
            return false;
        }
        if (result == 2) {
            if ($.trim($("#txtRemark").val()) == "") {
                alert("请填写审批意见");
                return false;
            }
        }
        if (confirm("确定提交吗？")) {
            $("#btnDiv").hide();
            $("#loadingApprove").show();
            return true;
        }
        else
            return false;
    }   
</script>
