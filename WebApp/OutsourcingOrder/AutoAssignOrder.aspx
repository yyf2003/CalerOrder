<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoAssignOrder.aspx.cs"
    Inherits="WebApp.OutsourcingOrder.AutoAssignOrder" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <%--<link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />--%>
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
  <%--  <script src="../easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>--%>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    
    <script type="text/javascript">
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            外协订单初始化
        </p>
    </div>
    <div class="tr">
        >>选择活动</div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table class="table">
                <tr class="tr_bai">
                    <td>
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
                        活动月份
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <asp:TextBox ID="txtGuidanceMonth" runat="server" CssClass="Wdate" onfocus="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                            Style="width: 80px;" OnTextChanged="txtMonth_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <asp:LinkButton ID="lbUp" runat="server" Style="margin-left: 20px; cursor: pointer;
                            color: Blue;" OnClick="lbUp_Click">上一个月</asp:LinkButton>
                        <asp:LinkButton ID="lbDown" runat="server" Style="margin-left: 20px; cursor: pointer;
                            color: Blue;" OnClick="lbDown_Click">下一个月</asp:LinkButton>

                        <img id="loadGuidance" style="display: none; margin-left:20px;" src='../image/WaitImg/loadingA.gif' />
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        活动名称
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <div id="seleAll" runat="server">
                            <input type="checkbox" id="guianceSeleAll" />全选
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
                            RepeatLayout="Flow" AutoPostBack="true"
                            onselectedindexchanged="cblRegion_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </td>
                </tr>
            </table>
            <table style="width: 100%;">
                <tr>
                    <td style="width: 120px; height: 35px;">
                    </td>
                    <td>
                        <asp:Button ID="btnSubmit" runat="server" Text="开始初始化" class="layui-btn layui-btn-small"
                             OnClick="btnSubmit_Click" OnClientClick="return Check()" />
                        <div id="imgLoading" style="display: none; color:Red;">
                          <img id="imgLoading111"  src='../image/WaitImg/loadingA.gif' />
                          正在执行分单，请稍等...
                        </div>
                        
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td style="height: 30px;">
                        <asp:Label ID="labAssignState" runat="server" Text="" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
           <%-- <div id="loading" style="text-align: center; display: none;">
                <img src="../image/WaitImg/loading1.gif" />
            </div>--%>
            <asp:Panel ID="Panel1" runat="server" Visible="false" Style="margin-bottom: 30px; ">
                <asp:Button ID="btnRefresh" runat="server" Text="Button" 
                    onclick="btnRefresh_Click" style=" display:none;"/>
                <span id="spanRefresh" class="layui-btn layui-btn-primary layui-btn-small" style=" margin-left:5px; margin-bottom:6px;">
                    <i class="layui-icon">&#x1002;</i>
                    刷新
                </span>
                <img id="loading" style="display: none;" src='../image/WaitImg/loadingA.gif' />
                <div class="tr">
                    >>订单状态  
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    区域：
                    <asp:Label ID="labRegion" runat="server" Text="全部" style=" color:Blue;"></asp:Label>
                </div>
                <asp:Repeater ID="Repeater1" runat="server" 
                    onitemdatabound="Repeater1_ItemDataBound">
                    <HeaderTemplate>
                        <table class="table">
                            <tr class="tr_hui">
                                <td style="width: 60px;">
                                    序号
                                </td>
                                <td>
                                    活动名称
                                </td>
                                <td>
                                    店铺数量
                                </td>
                                <td>
                                    订单数量
                                </td>
                                <td>
                                    已分店铺
                                </td>
                                <td>
                                    已分订单
                                </td>
                                <td>
                                    重复订单
                                </td>
                                <td>
                                    未分订单
                                </td>
                            </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="tr_bai">
                            <td>
                                <%#Container.ItemIndex+1 %>
                            </td>
                            <td>
                                <%#Eval("ItemName") %>
                            </td>
                            <td>
                                <asp:Label ID="labTotalShopCount" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <%--订单数量--%>
                                <asp:Label ID="labTotalOrderCount" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <%--已分店铺--%>
                                <asp:Label ID="labAssignShopCount" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <%--已分订单--%>
                                <asp:Label ID="labAssignOrderCount" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                <%--重复订单--%>
                                <asp:Label ID="labRepeatOrderCount" runat="server" Text=""></asp:Label>
                            </td>
                            <td>
                                
                                <asp:Label ID="labNotAssignOrderCount" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
<script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script type="text/javascript">
    $("#loading").show();
    Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {

        
        var eleId = e.get_postBackElement().id;
        if (eleId.indexOf("lbUp") != -1 || eleId.indexOf("lbDown") != -1 || eleId.indexOf("txtGuidanceMonth")!=-1) {
            $("#loadGuidance").show();
            $("#loading").show();
        }
    })
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
        $(function () {
            $("#guianceSeleAll").change(function () {
                var checked = this.checked;
                $("input[name^='cblGuidanceList']").each(function () {
                    this.checked = checked;
                });
                $("#loading").show();
            })
            $("input[name^='cblRegion']").change(function () {
                $("#loading").show();
            })
            $("#btnSubmit").show()
            $("#loading").hide();
            $("#loadGuidance").hide();
            $("#spanRefresh").click(function () {
                Refresh();
            })

        })



    })



    function getMonth() {
        $("#txtGuidanceMonth").blur();
    }

    function Check() {
        var guidanceList = $("input[name^='cblGuidanceList']:checked").length;
        if (guidanceList == 0) {
            alert("请选择活动");
            return false;
        }
        else {
            if (confirm("确定要初始化吗？")) {
                $("#btnSubmit").hide();
                $("#imgLoading").show();
                return true;
            }
        }
    }


    function Refresh() {
        $("#loading").show();
        $("#btnRefresh").click();
    }


    function CheckNotAssignOrder(gid) {

        var region = "";
        $("input[name^='cblRegion']:checked").each(function () {
            region += $(this).val() + ",";
        })

        var url = "CheckNotAssignOrder.aspx?guidanceId=" + gid + "&region=" + region;
       
        layer.open({
            type: 2,
            time: 0,
            title: '没分配订单',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '92%'],
            content: url,
            cancel: function () {

            }
        });
    }
</script>
