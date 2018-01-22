<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ByOrderStatistics.aspx.cs"
    Inherits="WebApp.Statistics.ByOrderStatistics" %>

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
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            按订单统计
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
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
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动名称
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlGuidance" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlGuidance_SelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            项目
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:CheckBox ID="cbAll" runat="server" />全选
                            <hr style="width: 100px; margin-bottom: 5px;" />
                            <asp:CheckBoxList ID="cblSubjects" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            时间段
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtBegin" runat="server" onclick="WdatePicker()"></asp:TextBox>
                            —
                            <asp:TextBox ID="txtEnd" runat="server" onclick="WdatePicker()"></asp:TextBox>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            区域
                        </td>
                        <td style="text-align: left; padding-left: 5px;">
                            <asp:CheckBoxList ID="cblRegion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    
                   <%-- <tr class="tr_bai">
                        <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                            
                        </td>
                    </tr>--%>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div style=" text-align:right; padding-right:25px; margin-top:8px;">
          <asp:Button ID="Button1" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                                height: 26px;" OnClick="Button1_Click" OnClientClick="return loading()"/>
            <img id="loadingImg" style=" display:none;" src="../image/WaitImg/loadingA.gif" />
        </div>
    </div>
    <br />
    <br />
    <div style="margin: 0px;">
        <table class="table">
            <tr class="tr_hui">
                <td style="text-align: center; width: 80px;">
                    店铺数量：
                </td>
                <td style="text-align: left; width: 120px; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labShopCount" runat="server" Text=""></asp:Label>
                </td>
                <td style="text-align: center; width: 80px;">
                    面积合计：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labArea" runat="server" Text=""></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    POP金额合计：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labPOPPrice" runat="server" Text=""></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    安装费合计：
                </td>
                <td style="text-align: left; padding-left: 5px; width: 100px;">
                    <asp:Label ID="labInstallPrice" runat="server" Text=""></asp:Label>
                </td>
                <td style="text-align: center; width: 90px;">
                    发货费合计：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labFahuoPrice" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_hui">
                <td colspan="9" style="text-align: right; padding-right: 10px;">
                    总金额：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labTotalPrice" runat="server" Text=""></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <div class="tr">
        >>信息列表
    </div>
    <asp:Repeater ID="gvList" runat="server" OnItemDataBound="gvList_ItemDataBound" 
        onitemcommand="gvList_ItemCommand">
        <HeaderTemplate>
            <table class="table1" style="width: 100%;">
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
                        POP总面积(平米)
                    </td>
                    <td>
                        POP金额(元)
                    </td>
                    <td>
                        安装费用(元)
                    </td>
                    <td>
                        发货费用(元)
                    </td>
                    <td>
                        统计金额(元)
                    </td>
                    <td>
                        订单明细
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
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
                    <%--POP平米数--%>
                    <asp:Label ID="labArea" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <%--POP金额--%>
                    <%--<a href="javascript:void(0);" onclick="CheckMaterialPrice(<%#Eval("ShopId") %>)"
                        style="text-decoration: underline;">
                        
                    </a>--%>
                    <asp:Label ID="labPOPPrice" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <%--安装费用--%>
                    <asp:Label ID="labInstallFee" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <%--发货费用--%>
                    <asp:Label ID="labFahuoFee" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <%--统计金额--%>
                    <asp:Label ID="labSubTotal" runat="server" Text=""></asp:Label>
                </td>
                <td>
                    <%--<a href="SubjectStatistics.aspx?subjectId=<%#Eval("ShopId") %>">查看</a>--%>
                    <asp:LinkButton ID="lbCheck" runat="server" CommandArgument='<%#Eval("ShopId") %>' CommandName="Check">查看</asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (gvList.Items.Count == 0)
              {%>
            <tr class="tr_bai">
                <td colspan="15" style="text-align: center;">
                    --无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <asp:HiddenField ID="hfSubjectIds" runat="server" />
    </form>
</body>
</html>
<script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="/fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="/fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
        $("#cbAll").change(function () {
            var checked = this.checked;
            $("input[name^='cblSubjects']").each(function () {
                this.checked = checked;
            });

        })
    })
    var $j = jQuery.noConflict();
    $("span[name='checkMaterial']").click(function () {
        var region = $(this).data("region");
        var subjectIds = $(this).data("subjectids");
        var url = "MaterialStatistics.aspx?subjectIds=" + subjectIds + "&regionName=" + region + "&isSearch=1";
        $j.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: 700,
            //modal:true,
            hideOnOverlayClick: false
        })
    })
    $("span[name='checkShopMaterial']").click(function () {
        var shopId = $(this).data("shopid");
        var subjectIds = $(this).data("subjectids");
        var url = "MaterialStatistics.aspx?subjectIds=" + subjectIds + "&shopId=" + shopId + "&isSearch=1";
        $j.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: 700,
            //modal:true,
            hideOnOverlayClick: false
        })
    })

    function loading() {
        var customerId = $("#ddlCustomer").val();
        if (customerId == "0") {
            alert("请选择客户");
            return false;
        }
        $("#loadingImg").show();
        return true;
    }
</script>
