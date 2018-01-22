<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StatisticsList.aspx.cs"
    Inherits="WebApp.Statistics.StatisticsList" %>

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
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                项目统计报表
            </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div style=" margin-bottom:30px;">
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <table class="table">
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            客户
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged">
                            </asp:DropDownList>
                            <span style="color: Red;">*</span>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动时间
                        </td>
                        <td style="text-align: left;">
                            <div>
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 100px; text-align: center;">
                                            按活动月显示：
                                        </td>
                                        <td style="text-align: left; padding-left: 8px;">
                                            <%--<asp:Label ID="labGuidanceMonth" runat="server" Text=""></asp:Label>--%>
                                            <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                                                Style="width: 80px;" OnTextChanged="txtGuidanceMonth_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            <asp:LinkButton ID="lbUp" runat="server" Style="margin-left: 20px; cursor: pointer;
                                                color: Blue;" OnClick="lbUp_Click">上一个月</asp:LinkButton>
                                            <asp:LinkButton ID="lbDown" runat="server" Style="margin-left: 20px; cursor: pointer;
                                                color: Blue;" OnClick="lbDown_Click">下一个月</asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动名称
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <div id="loadGuidance" style="display: none;">
                                <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <div>
                              <input type="checkbox" id="cbAllGuidance"/>全选
                            </div>
                            <asp:CheckBoxList ID="cblGuidanceList" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" 
                                >
                            </asp:CheckBoxList>
                            <asp:Panel ID="Panel_EmptyGuidance" runat="server" Visible="false">
                                <span style="color: Red;">无活动信息！</span>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                            区域
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                          <asp:CheckBoxList ID="cblRegion" runat="server" CssClass="cbl" CellSpacing="20" RepeatDirection="Horizontal"
                                RepeatLayout="Flow" RepeatColumns="5"
                                >
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td>
                        </td>
                        <td style="text-align: left; padding-left: 5px; height: 35px;">
                            <asp:Button ID="btnSearch" runat="server" Text="查 询" Style="width: 65px;
                                height: 26px;" OnClick="btnSearch_Click" OnClientClick="return check()"/>
                            <img id="imgSearching" src="../image/WaitImg/loadingA.gif" style=" display:none;"/>
                        </td>
                    </tr>
                </table>
                <div class="tr" style="margin-top: 10px;">
                    >>统计信息
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Label ID="labSearchMonth" runat="server" Text="" ForeColor="Blue"></asp:Label>
                 </div>
                <asp:Repeater ID="Repeater1" runat="server" 
                    onitemdatabound="Repeater1_ItemDataBound">
                    <HeaderTemplate>
                        <table class="table" id="tbData">
                            <tr class="tr_hui">
                                <td style="width: 40px;">
                                    序号
                                </td>
                                <td>
                                    省份
                                </td>
                                <td>
                                    总店铺数量
                                </td>
                                <td>
                                    总面积
                                </td>
                                <td>
                                    POP金额
                                </td>
                                <td>
                                    安装费
                                </td>
                                <td>
                                    测量费
                                </td>
                                <td>
                                    快递费
                                </td>
                                <td>
                                    物料费
                                </td>
                                <td>
                                    费用合计
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                       <tr class="tr_bai">
                                <td>
                                    <%#Container.ItemIndex+1 %>
                                </td>
                                <td>
                                    <span name="spanRegion" data-region='<%#((string)Container.DataItem) %>' style=" cursor:pointer; color:Blue; text-decoration:underline;"><%#((string)Container.DataItem) %></span>  
                                </td>
                                <td>
                                    <asp:Label ID="labShopCount" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--总面积--%>
                                    <asp:Label ID="labArea" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--POP金额--%>
                                    <asp:Label ID="labPOPPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--安装费--%>
                                    <asp:Label ID="labInstallPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--测量费--%>
                                    <asp:Label ID="labMeasurePrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--快递费--%>
                                    <asp:Label ID="labExpressPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--物料费--%>
                                    <asp:Label ID="labMaterialPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="labSubPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <%if (Repeater1.Items.Count == 0)
                          {%>
                        <tr class="tr_bai">
                            <td colspan="10" style="text-align: center;">
                                --无数据--
                            </td>
                        </tr>
                        <%}
                          else
                          { %>
                          <tr class="tr_hui">
                                <td>
                                    
                                </td>
                                <td>
                                   合计
                                </td>
                                <td>
                                    <asp:Label ID="labTotalShopCount" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--总面积--%>
                                    <asp:Label ID="labTotalArea" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--POP金额--%>
                                    <asp:Label ID="labTotalPOPPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--安装费--%>
                                    <asp:Label ID="labTotalInstallPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--测量费--%>
                                    <asp:Label ID="labTotalMeasurePrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--快递费--%>
                                    <asp:Label ID="labTotalExpressPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <%--物料费--%>
                                    <asp:Label ID="labTotalMaterialPrice" runat="server" Text="0"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label ID="labTotalPrice" runat="server" Text="0"></asp:Label>
                                </td>
                            </tr>
                        <%} %>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
<script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="/layer/skin/default/layer.css" rel="stylesheet" type="text/css" />
<script src="/layer/layer.js" type="text/javascript"></script>
<script type="text/javascript">
    function getMonth() {
        
        $("#txtGuidanceMonth").blur();
    }

    function check() {
        $("#imgSearching").show();
        return true;
    }

    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (sender, s) {
        $("#tbData").delegate("span[name='spanRegion']", "click", function () {
            var guidanceIds = "";
            $("input[name^='cblGuidanceList']:checked").each(function () {
                guidanceIds += $(this).val() + ",";
            })
            var region = $(this).data("region") || "";
            var url = "StatisticsByRegion.aspx?guidanceIds=" + guidanceIds + "&region=" + region;
            layer.open({
                type: 2,
                title: '按区域统计-' + region,
                skin: 'layui-layer-rim', //加上边框
                shadeClose: true,
                shade: 0.8,
                area: ['95%', '95%'],
                content: url
            });
        })

        $("#cbAllGuidance").change(function () {
            var checked = this.checked;
            $("input[name^='cblGuidanceList']").each(function () {
                this.checked = checked;
            })
        })

        $("input[name^='cblGuidanceList']").change(function () {
            if (!this.checked) {
                $("#cbAllGuidance").prop("checked", false);
            }
            else {
                var checked = $("input[name^='cblGuidanceList']:checked").length == $("input[name^='cblGuidanceList']").length;
                $("#cbAllGuidance").prop("checked", checked);
            }
        })
    })
</script>
