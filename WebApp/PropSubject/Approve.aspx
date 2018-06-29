<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Approve.aspx.cs" Inherits="WebApp.PropSubject.Approve" %>

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
    <div class="tr" style=" margin-top:20px;">
        >>订单信息
    </div>
    <div style="width: 100%; overflow: auto;">
       <asp:Repeater ID="Repeater1" runat="server">
                <HeaderTemplate>
                    <table class="table" style="width: 2000px;">
                        <tr class="tr_hui" style="font-weight: bold;">
                            <td colspan="14">
                                应收
                            </td>
                            <td colspan="16" style="border-left-color: #000;">
                                应付
                            </td>
                        </tr>
                        <tr class="tr_hui">
                            <td>
                                序号
                            </td>
                            <td>
                                道具名称
                            </td>
                            <td>
                                应用位置
                            </td>
                            <td>
                                材料类型
                            </td>
                            <td>
                                尺寸规格
                            </td>
                            <td>
                                工艺/服务描述
                            </td>
                            <td>
                                包装方式
                            </td>
                            <td>
                                单位
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                材料成本
                            </td>
                            <td>
                                加工成本
                            </td>
                            <td>
                                包装费
                            </td>
                            <td>
                                运输费
                            </td>
                            <td>
                                单价小计
                            </td>
                            <td>
                                外协名称
                            </td>
                            <td>
                                包装方式
                            </td>
                            <td>
                                单位
                            </td>
                            <td>
                                数量
                            </td>
                            <td>
                                单价小计
                            </td>
                            <td>
                                备注
                            </td>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="tr_bai">
                        <td>
                            <%--序号--%>
                            <%#Container.ItemIndex+1 %>
                        </td>
                        <td>
                            <%--道具名称--%>
                            <%#Eval("MaterialName")%>
                        </td>
                        <td>
                            <%--应用位置--%>
                            <%#Eval("Sheet")%>
                        </td>
                        <td>
                            <%--材料类型--%>
                            <%#Eval("MaterialType")%>
                        </td>
                        <td>
                            <%--尺寸规格--%>
                            <%#Eval("Dimension")%>
                        </td>
                        <td>
                            <%--工艺/服务描述--%>
                            <%#Eval("ServiceType")%>
                        </td>
                        <td>
                            <%--包装方式--%>
                            <%#Eval("Packaging")%>
                        </td>
                        <td>
                            <%--单位--%>
                            <%#Eval("UnitName")%>
                        </td>
                        <td>
                            <%--数量--%>
                            <%#Eval("Quantity")%>
                        </td>
                        <td>
                            <%--材料成本--%>
                            <%#Eval("BOMCost")%>
                        </td>
                        <td>
                            <%--加工成本--%>
                            <%#Eval("ProcessingCost")%>
                        </td>
                        <td>
                            <%--包装费--%>
                            <%#Eval("PackingCost")%>
                        </td>
                        <td>
                            <%--运输费--%>
                            <%#Eval("TransportationCost")%>
                        </td>
                        <td>
                            <%--单价小计--%>
                            <%#Eval("UnitPrice")%>
                        </td>
                        <td>
                            <%--外协名称--%>
                            <%#Eval("OutsourceName")%>
                        </td>
                        <td>
                            <%--包装方式--%>
                            <%#Eval("PayPackaging")%>
                        </td>
                        <td>
                            <%--单位--%>
                            <%#Eval("PayUnitName")%>
                        </td>
                        <td>
                            <%--数量--%>
                            <%#Eval("PayQuantity")%>
                        </td>
                        <td>
                            <%--单价小计--%>
                            <%#Eval("PayUnitPrice")%>
                        </td>
                        <td>
                            <%--备注--%>
                            <%#Eval("PayRemark")%>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <%if (Repeater1.Items.Count == 0)
                      {%>
                    <tr class="tr_bai">
                        <td colspan="20">
                            --暂无数据--
                        </td>
                    </tr>
                    <%} %>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
    </div>
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
       
        if (confirm("确定提交吗？")) {
            $("#btnDiv").hide();
           
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