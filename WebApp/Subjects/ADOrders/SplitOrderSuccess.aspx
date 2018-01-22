<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SplitOrderSuccess.aspx.cs"
    Inherits="WebApp.Subjects.ADOrders.SplitOrderSuccess" %>

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
    <script src="../../easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
    <link href="../../layer/skin/default/layer.css" rel="stylesheet" type="text/css" />
    <script src="../../layer/layer.js" type="text/javascript"></script>
    <style type="text/css">
        .style1
        {
            width: 120px;
            height: 32px;
        }
        .style2
        {
            width: 300px;
            height: 32px;
        }
        .style3
        {
            height: 32px;
        }
    </style>
    <script type="text/javascript">
        function SubmintFail() {
            layer.msg("提交失败：订单总面积为0");
        }
        function SubmintWarning() {
            layer.confirm("订单空尺寸数量占50%以上，确定提交吗？", {title:"警告", btn: ['继续提交', '取消'] }, function () {
                $("#Button1").click();
            }, function () {
                //layer.msg("取消");
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            订单拆分结果
        </p>
    </div>
    <div>
        <asp:Label ID="labState" Visible="false" runat="server" Text="订单拆分成功！" Style="color: Red;
            font-size: 20px; font-weight: bold; margin-left: 10px;"></asp:Label>
    </div>
    <div class="tr">
        >>项目信息
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td class="style1">
                内部项目编号
            </td>
            <td style="text-align: left; padding-left: 5px; " class="style2">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td class="style1">
            </td>
            <td style="text-align: left; padding-left: 5px;" class="style3">
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
    <br />
    <div class="tab" style="margin-top: 10px;">
        <span style="font-weight: bold;">最终订单信息</span> &nbsp;&nbsp;
        <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
    </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
       <ContentTemplate>
          
       
    <table style="width: 100%; margin-left: 5px;">
        <tr>
            <td style="width: 80px;">
                店铺编号：
            </td>
            <td style="width: 120px; text-align: left;">
                <asp:TextBox ID="txtShopNo" runat="server"></asp:TextBox>
            </td>
            <td style="width: 90px; text-align: center;">
                店铺名称：
            </td>
            <td style="width: 120px; text-align: left;">
                <asp:TextBox ID="txtShopName" runat="server" Style="width: 150px;"></asp:TextBox>
            </td>
            <td style="text-align: left;">
                <asp:Button ID="btnSearchFinal" runat="server" Text="查 询" class="easyui-linkbutton"
                    Style="width: 65px; height: 26px; margin-left: 10px;" OnClick="btnSearchFinal_Click" />
                <img id="loadSearch" src="/image/WaitImg/loadingA.gif" alt="" style="display: none;" />
                <asp:Button ID="btnExportAll" runat="server" Text="导出全部" class="easyui-linkbutton"
                    Style="width: 65px; height: 26px; margin-left: 10px;" OnClientClick="return check(this,0)" />
                <img id="loadExportAll" src="/image/WaitImg/loadingA.gif" alt="" style="display: none;" />
                <asp:Button ID="btnExportNotNull" runat="server" Text="导出非空(不含空尺寸)" class="easyui-linkbutton"
                    Style="width: 150px; height: 26px; margin-left: 10px;" OnClientClick="return check(this,1)" />
                <img id="Img1" alt="" src="/image/WaitImg/loadingA.gif" style="display: none;" />
            </td>
        </tr>
    </table>
    <div class="tab" style="overflow: auto;">
        <asp:Repeater ID="repeater_FinalOrder" runat="server" 
            onitemdatabound="repeater_FinalOrder_ItemDataBound">
            <HeaderTemplate>
                <%if (repeater_FinalOrder.Items.Count == 0)
                  { %>
                <table class="table">
                    <%}
                  else
                  { %>
                    <table class="table1" style="width: 1500px;">
                        <tr class="tr_hui">
                            <td style="width: 40px;">
                                序号
                            </td>
                            <td style="width: 50px;">
                                订单类型
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
                                城市级别
                            </td>
                            <td>
                                店铺规模大小
                            </td>
                            <td>
                                物料支持
                            </td>
                            <td>
                                位置
                            </td>
                            <td>
                                级别
                            </td>
                            <td>
                                位置描述
                            </td>
                            <td>
                                器架类型
                            </td>
                            <td>
                                性别
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                POP宽(mm)
                            </td>
                            <td>
                                POP高(mm)
                            </td>
                            <%--<td>
                                面积(M2)
                            </td>--%>
                            <td>
                                POP材质
                            </td>
                            <%--<td>
                                系列
                            </td>--%>
                            <td>
                                选图
                            </td>
                            <td>
                                备注
                            </td>
                            <td>
                                拆单状态
                            </td>
                        </tr>
                        <%} %>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="tr_bai">
                    <td style="width: 40px;">
                        <%#Container.ItemIndex + 1 + ((AspNetPagerFinal.CurrentPageIndex - 1) * AspNetPagerFinal.PageSize)%>
                    </td>
                    <td style="width: 50px;">
                       
                        <asp:Label ID="labOrderType" runat="server" Text=""></asp:Label>
                    </td>
                    <td>
                        <%--店铺编号--%>
                        <%#Eval("ShopNo")%>
                    </td>
                    <td>
                        <%--店铺名称--%>
                        <%#Eval("ShopName")%>
                    </td>
                    <td>
                        <%--区域--%>
                        <%#Eval("Region")%>
                    </td>
                    <td>
                        <%--省份--%>
                        <%#Eval("Province")%>
                    </td>
                    <td>
                        <%--城市--%>
                        <%#Eval("City")%>
                    </td>
                    <td>
                        <%--城市级别--%>
                        <%#Eval("CityTier")%>
                    </td>
                    <td>
                        <%#Eval("POSScale")%>
                    </td>
                    <td>
                        <%#Eval("MaterialSupport")%>
                    </td>
                    <td>
                        <%--位置--%>
                        <%#Eval("Sheet")%>
                    </td>
                    <td>
                        <%#Eval("LevelNum")%>
                    </td>
                    <td>
                        <%--位置描述--%>
                        <%#Eval("PositionDescription")%>
                    </td>
                    <td>
                        <%--器架类型--%>
                        <%#Eval("MachineFrame")%>
                    </td>
                    <td>
                        <%--性别--%>
                        <%#Eval("OrderGender") != null ? Eval("OrderGender") : Eval("Gender")%>
                    </td>
                    <td>
                        <%--数量--%>
                        <%#Eval("Quantity")%>
                    </td>
                    <td>
                        <%--POP宽(mm)--%>
                        <%#Eval("GraphicWidth")%>
                    </td>
                    <td>
                        <%-- POP高(mm)--%>
                        <%#Eval("GraphicLength")%>
                    </td>
                    <%--<td>
                       
                        <%#Eval("order.Area")%>
                    </td>--%>
                    <td>
                        <%--POP材质--%>
                        <%#Eval("GraphicMaterial")%>
                    </td>
                    <%--<td>
                       
                        <%#Eval("pop") != null ? Eval("pop.Category") : ""%>
                    </td>--%>
                    <td>
                        <%--选图--%>
                        <%#Eval("ChooseImg")%>
                    </td>
                    <td>
                        <%--备注--%>
                        <%#Eval("Remark")%>
                    </td>
                    <td>
                        <%--备注--%>
                        <%#Eval("SplitOrderRemark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <%if (repeater_FinalOrder.Items.Count == 0)
                  {%>
                <tr class="tr_bai">
                    <td style="text-align: center;">
                        --无数据信息--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div class="tab" style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPagerFinal" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPagerFinal_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>
    <table class="table">
      <tr class="tr_hui">
        <td style=" width:120px;">店铺数量：</td>
        <td style=" width:120px; text-align:left; padding-left:5px;">
            <asp:Label ID="labShopCount" runat="server" Text=""></asp:Label>
        </td>
        <td style=" width:120px;">总面积：</td>
        <td style=" width:150px; text-align:left; padding-left:5px;">
           <asp:Label ID="labTotalArea" runat="server" Text=""></asp:Label>
        </td>
        <td style=" width:120px;">POP总金额：</td>
        <td style=" text-align:left; padding-left:5px;">
           <asp:Label ID="labTotalPrice" runat="server" Text=""></asp:Label>
        </td>
      </tr>
    </table>
    <div style="text-align: center; margin-top :30px;">
        <asp:Button ID="btnSubmit" runat="server" Text="确定提交" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;" OnClick="btnSubmit_Click" />
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnReSplit" runat="server" Text="上一步" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;" OnClick="btnReSplit_Click" />
        &nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnGoBack" runat="server" Text="返 回" class="easyui-linkbutton" Style="width: 65px;
            height: 26px;" OnClick="btnGoBack_Click" />
    </div>
    <br />
    <br />
    <div style="display: none;">
        <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" />
        <iframe name="exportFrame" id="exportFrame" src=""></iframe>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    var subjectId = '<%=subjectId %>';
    function check(obj, isFilter) {
        $("#btnExportAll,#btnExportNotNull").attr("disabled", true);

        $(obj).next("img").show();
       
        $("#exportFrame").attr("src", "/Subjects/ShowOrderDetailExport.aspx?subjectId=" + subjectId + "&isFilter=" + isFilter);
        checkExport(obj);
        return false;
    }

    var timer;
    function checkExport(obj) {
        timer = setInterval(function () {
            $.ajax({
                type: "get",
                url: "/Subjects/Handler/CheckExport.ashx",
                cache: false,
                success: function (data) {
                    if (data == "empty") {
                        $("#btnExportAll,#btnExportNotNull").attr("disabled", false);
                        $("#exportFrame").attr("src", "");
                        $(obj).next("img").hide();
                        clearInterval(timer);
                        alert("没有数据可以导出！");
                    }
                    else if (data == "ok") {
                        $("#btnExportAll,#btnExportNotNull").attr("disabled", false);
                        $("#exportFrame").attr("src", "");
                        $(obj).next("img").hide();
                        clearInterval(timer);
                    }

                }
            })

        }, 1000);
    }


    
</script>
