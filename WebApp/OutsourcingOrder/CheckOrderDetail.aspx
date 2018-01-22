<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOrderDetail.aspx.cs" Inherits="WebApp.OutsourcingOrder.CheckOrderDetail" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

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
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
       <ContentTemplate>
          <blockquote class="layui-elem-quote" style=" padding-left: 5px; padding-top:2px;">
                <div class="layui-inline" style="padding-left: 10px;">
                    <span style="font-weight: bold;">搜索：</span>
                    
                    <div class="layui-input-inline" style="width: 130px;">
                        <asp:TextBox ID="txtShopNo" runat="server" placeholder="店铺编号" class="layui-input"
                            Style="height: 25px;"></asp:TextBox>
                    </div>
                    <span id="btnSearch" class="layui-btn layui-btn-danger layui-btn-small" style="height: 24px;">
                        <i class="layui-icon">&#xe615;</i> </span>
                    <asp:Button ID="btnSearchServer" runat="server" Text="Button" OnClick="btnSearchServer_Click"
                        Style="display: none;" />
                </div>
                <div style=" margin-top:10px; padding-left:55px;">
                  <asp:CheckBoxList ID="cblSheet" runat="server" RepeatDirection="Horizontal" AutoPostBack="true"
                            RepeatLayout="Flow" OnSelectedIndexChanged="cblSheet_SelectedIndexChanged">
                        </asp:CheckBoxList>
                </div>
            </blockquote>
            <div class="layui-btn-group" style="margin-left: 5px;">
                 <span id="btnYes" class="layui-btn layui-btn-small" style="margin-left: 30px;">
                        <i class="layui-icon">&#xe605;</i>生产</span>
                 <span id="btnNo" class="layui-btn layui-btn-danger layui-btn-small"><i class="layui-icon">
                    &#x1006;</i>不生产</span>
                <asp:Button ID="btnNoProduce" runat="server" Text="Button" 
                    onclick="btnNoProduce_Click" style=" display:none;"/>
                <asp:Button ID="btnProduce" runat="server" Text="Button" 
                    onclick="btnProduce_Click" style=" display:none;"/>
            </div>
            <div style="overflow: auto;">
                <div id="divload" style="display: none; text-align: center;">
                    <img src="../image/WaitImg/loading1.gif" />
                </div>
                <asp:Repeater ID="rp_orderList" runat="server" OnItemDataBound="rp_orderList_ItemDataBound">
                    <HeaderTemplate>
                        <table id="tbPOP" class="layui-table" style="width: 2000px;">
                            <tr>
                                <th style="width: 20px;">
                                    <asp:CheckBox ID="cbAll" runat="server" />
                                </th>
                                <th style="width: 30px;">
                                    序号
                                </th>
                                <th style="width: 50px;">
                                    状态
                                </th>
                                <th style="width: 50px;">
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
                                    项目名称
                                </th>
                                <th>
                                    Sheet
                                </th>
                                <th>
                                    POP编号
                                </th>
                                <th>
                                    POP数量
                                </th>
                                <th>
                                    费用金额
                                </th>
                                <th>
                                    男/女
                                </th>
                                <th>
                                    POP宽
                                </th>
                                <th>
                                    POP高
                                </th>
                                <th>
                                    POP材质
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
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:CheckBox ID="cbOne" runat="server" />
                                <asp:HiddenField ID="hcOrderId" runat="server"  Value='<%#Eval("order.Id") %>'/>
                            </td>
                            <td>
                                <%#Container.ItemIndex+1 %>
                            </td>
                            <td>
                                <asp:Label ID="labState" runat="server" Text="正常"></asp:Label>
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
                                <%#Eval("order.Format")%>
                            </td>
                            <td>
                                <%#Eval("SubjectName")%>
                            </td>
                            <td>
                                <%#Eval("order.Sheet")%>
                            </td>
                            <td>
                                <%#Eval("order.GraphicNo")%>
                            </td>
                            <td>
                                <%#Eval("order.Quantity")%>
                            </td>
                            <td>
                                <%#Eval("order.OrderPrice")%>
                            </td>
                            <td>
                                <%#(Eval("order.OrderGender") != null && Eval("order.OrderGender").ToString() != "") ? Eval("order.OrderGender") : Eval("order.Gender")%>
                            
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
                                <%#Eval("order.PositionDescription")%>
                            </td>
                            <td>
                                <%#Eval("order.ChooseImg")%>
                            </td>
                            <td>
                                <%#Eval("order.Remark")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%if (rp_orderList.Items.Count == 0)
                          { %>
                        <tr>
                            <td colspan="21" style="text-align: center;">
                                无数据显示
                            </td>
                        </tr>
                        <%} %>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
                <div style=" text-align:center; margin-top :20px;">
                    <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
                       
                    </webdiyer:AspNetPager>
                </div>
            </div>
       </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
<script type="text/javascript">

    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (s, e) {
        var eleId = e.get_postBackElement().id;
        if (eleId.indexOf("btnSearchServer") != -1) {
            $("#divload").show();

        }
        if (eleId.indexOf("cblSheet") != -1) {
            $("#divload").show();

        }
    })

    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (s, e) {
        $("#divload").hide();
        $("#cbAll").change(function () {
            alert("ddd");
        })
        //POP全选
        $("input[name$='cbAll']").click(function () {
            var checked = this.checked;
            $("input[name$='cbOne']").each(function () {
                this.checked = checked;
            })
        })
        $("#tbPOP").delegate("input[name$='cbOne']", "change", function () {
            if (!this.checked) {
                $("input[name$='cbAll']").attr("checked", false);
            }
        })

        $("#btnSearch").click(function () {
            $("#btnSearchServer").click();
        })

        $("#btnNo").click(function () {
            var len = $("input[name$='cbOne']:checked").length;
            if (len > 0) {
                $("#btnNoProduce").click();
            }
        })
        $("#btnYes").click(function () {
            var len = $("input[name$='cbOne']:checked").length;
            if (len > 0) {
                $("#btnProduce").click();
            }
        })
    })

    
</script>