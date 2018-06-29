<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Approve.aspx.cs" Inherits="WebApp.Subjects.RegionSubject.Approve" %>

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
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script type="text/javascript">
       
        function ApproveStae(msg, url) {
            if (msg == "ok") {
                layer.msg("审批成功");
                window.location.href = url;
            }
            else {
                layer.confirm(msg, { title: "提交失败", btn: ['确定'] });
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
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
    <div class="tab" style="margin-top: 15px;">
        <span>订单明细</span>
        <%--<span style=" margin-left:20px;">
            <asp:LinkButton ID="lbExport" runat="server" onclick="lbExport_Click" OnClientClick="return CheckExport()">导出</asp:LinkButton>
            <img id="exportLoadingImg" src="../../image/loadingA.gif" style="display:none;"/>
        </span>--%>
        <hr style="margin-top: 8px; margin-bottom: 5px; border: 1px solid; color: #000;" />
    </div>
    <asp:Panel ID="Panel1" runat="server">
    
    <div class="containerDiv">
        <asp:Repeater ID="orderListRepeater" runat="server">
            <HeaderTemplate>
                <table class="table1" style=" width:2000px;">
                    <tr class="tr_hui">
                        <td style="width: 30px;">
                            序号
                        </td>
                        <td>
                            项目
                        </td>
                        <td>
                            类型
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
                            店铺规模大小
                        </td>
                        <td>
                            物料支持级别
                        </td>
                        <td>
                            POP位置
                        </td>
                        <td>
                            GraphicNo
                        </td>
                        <td>
                            器架名称
                        </td>
                        <td>
                            POP位置明细
                        </td>
                        <td>
                            性别
                        </td>
                        <td>
                            数量
                        </td>
                        <td>
                            应收金额
                        </td>
                        <td>
                            应付金额
                        </td>
                        <td>
                            POP宽
                        </td>
                        <td>
                            POP高
                        </td>
                        <td>
                            材质
                        </td>
                        <td>
                            系列/选图
                        </td>
                        
                        <td>
                            安装位置描述
                        </td>
                        <td>
                            其他备注
                        </td>
                    </tr>
                    <tbody id="tbody1">
                    
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td style="width: 30px;">
                        <%#(AspNetPager1.CurrentPageIndex-1)*AspNetPager1.PageSize+ Container.ItemIndex + 1%>
                    </td>
                    <td>
                        <%#Eval("SubjectName")%>
                    </td>
                    <td>
                        <%#Eval("OrderType")%>
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
                        <%#Eval("POSScale")%>
                    </td>
                    <td>
                        <%#Eval("MaterialSupport")%>
                    </td>
                    <td>
                        <%#Eval("Sheet") %>
                    </td>
                    <td>
                        <%#Eval("GraphicNo")%>
                    </td>
                    <td>
                        <%#Eval("MachineFrame")%>
                    </td>
                    <td>
                        <%#Eval("PositionDescription")%>
                    </td>
                    <td>
                        <%#Eval("Gender")%>
                    </td>
                    <td>
                        <%#Eval("Quantity")%>
                    </td>
                    <td>
                        <%#Eval("ReceivePrice")%>
                    </td>
                    <td>
                        <%#Eval("PayPrice")%>
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
                        <%#Eval("ChooseImg")%>
                    </td>
                    
                    <td>
                        <%#Eval("PositionDescription")%>
                    </td>
                    <td>
                        <%#Eval("OtherRemark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                <%if (orderListRepeater.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="24" style="text-align: center;">
                        --暂无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div style="text-align: center; margin-top: 10px;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server" Visible="false">
        <div class="containerDiv">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
                <table class="table1" style=" width:2000px;">
                    <tr class="tr_hui">
                        <td style="width: 30px;">
                            序号
                        </td>
                        
                        <td>
                            类型
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
                            店铺规模大小
                        </td>
                        <td>
                            物料支持级别
                        </td>
                        <td>
                            POP位置
                        </td>
                        <td>
                            GraphicNo
                        </td>
                        <td>
                            器架名称
                        </td>
                        <td>
                            POP位置明细
                        </td>
                        <td>
                            性别
                        </td>
                        <td>
                            数量
                        </td>
                        <td>
                            应收金额
                        </td>
                        <td>
                            应付金额
                        </td>
                        <td>
                            POP宽
                        </td>
                        <td>
                            POP高
                        </td>
                        <td>
                            材质
                        </td>
                        <td>
                            系列/选图
                        </td>
                        
                        <td>
                            安装位置描述
                        </td>
                        <td>
                            其他备注
                        </td>
                    </tr>
                   
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td style="width: 30px;">
                        <%#(AspNetPager2.CurrentPageIndex-1)*AspNetPager2.PageSize+ Container.ItemIndex + 1%>
                    </td>
                   
                    <td>
                        <%#Eval("OrderType")%>
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
                        <%#Eval("POSScale")%>
                    </td>
                    <td>
                        <%#Eval("MaterialSupport")%>
                    </td>
                    <td>
                        <%#Eval("Sheet") %>
                    </td>
                    <td>
                        <%#Eval("GraphicNo")%>
                    </td>
                    <td>
                        <%#Eval("MachineFrame")%>
                    </td>
                    <td>
                        <%#Eval("PositionDescription")%>
                    </td>
                    <td>
                        <%#Eval("Gender")%>
                    </td>
                    <td>
                        <%#Eval("Quantity")%>
                    </td>
                    <td>
                        <%#Eval("ReceivePrice")%>
                    </td>
                    <td>
                        <%#Eval("PayPrice")%>
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
                        <%#Eval("ChooseImg")%>
                    </td>
                    
                    <td>
                        <%#Eval("PositionDescription")%>
                    </td>
                    <td>
                        <%#Eval("OtherRemark")%>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
               
                <%if (Repeater1.Items.Count == 0)
                  { %>
                <tr class="tr_bai">
                    <td colspan="23" style="text-align: center;">
                        --暂无数据--
                    </td>
                </tr>
                <%} %>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <div style="text-align: center; margin-top: 10px;">
        <webdiyer:AspNetPager ID="AspNetPager2" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" showinputbox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager2_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </asp:Panel>
     <br />
     <div class="tr">
        >>审批信息</div>
    <table class="table">
      <tr class="tr_bai">
        <td style="width:100px;">审批结果</td>
        <td style=" text-align:left; padding-left:5px;">
            <asp:RadioButtonList ID="rblApproveResult" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
              <asp:ListItem Value="1">通过 </asp:ListItem>
              <asp:ListItem Value="2">不通过 </asp:ListItem>
            </asp:RadioButtonList>
        </td>
      </tr>
      <tr class="tr_bai">
        <td>审批意见</td>
        <td style=" text-align:left; padding-left:5px; height:80px;">
            <asp:TextBox ID="txtRemark" runat="server" Columns="60" Rows="5" TextMode="MultiLine" MaxLength="100"></asp:TextBox>
            (100字以内)
        </td>
      </tr>
    </table>
    <br />
    <div id="btnDiv" style="text-align: center; margin-bottom:30px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return Check()" class="easyui-linkbutton" style="width: 65px;
            height: 26px; font-size:13px;" onclick="btnSubmit_Click"/>
       
         &nbsp;&nbsp;
      
            <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton" style="width: 65px;
            height: 26px;font-size:13px;"/>
    </div>
    <div id="loading" style="text-align: center; margin-bottom: 20px; display:none;">
        <img src="../../image/WaitImg/loading1.gif" />
    </div>
    <asp:HiddenField ID="hfSubjectType" runat="server" />
    <div id="approveLoading" style=" display:none;color:Red; font-size:18px; height:130px; width:420px; line-height:120px; text-align:center;">
       提示：正在审批，请稍等...
      
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
        //return confirm("确定提交吗？");
        if (confirm("确定提交吗？")) {
            $("#btnDiv").hide();
            //$("#loading").show();
            layer.open({
                type: 1,
                time: 0,
                title: '提示信息',
                skin: 'layui-layer-rim', //加上边框
                area: ['450px', '200px'],
                content: $("#approveLoading"),
                id: 'loadLayer',
                closeBtn: 0

            });
            return true;
        }
        else
            return false;
    }


</script>
