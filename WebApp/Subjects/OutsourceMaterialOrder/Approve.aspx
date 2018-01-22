<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Approve.aspx.cs" Inherits="WebApp.Subjects.OutsourceMaterialOrder.Approve" %>

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
     <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            外协物料订单审批
        </p>
    </div>
     <div class="tr">
        >>项目信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 100px;">
                所属客户
            </td>
            <td style="width: 300px; text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 100px;">
                活动名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labGuidance" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td>
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                外协名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labOutsource" runat="server" Text=""></asp:Label>
            </td>
            <td>
                创建人
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddUserName" runat="server" Text=""></asp:Label>
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
                创建时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labAddDate" runat="server" Text=""></asp:Label>
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
    <div class="tr" style="margin-top: 10px;">
        >>物料信息</div>
    <asp:Repeater ID="tbMaterialList" runat="server">
        <HeaderTemplate>
            <table class="table" id="tbMaterialData">
                <tr class="tr_hui">
                    <td style="width: 60px;">
                        序号
                    </td>
                    <td>
                        物料名称
                    </td>
                    <td style="width: 150px;">
                        单价
                    </td>
                    <td style="width: 150px;">
                        数量
                    </td>
                    <td>
                        备注
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="tr_bai materialData">
                <td style="width: 60px;">
                    <%#Container.ItemIndex+1 %>
                </td>
                <td>
                    <%#Eval("MaterialName")%>
                </td>
                <td style="width: 150px;">
                    <%#Eval("UnitPrice")%>
                </td>
                <td style="width: 150px;">
                    <%#Eval("Amount")%>
                </td>
                <td>
                    <%#Eval("Remark")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (tbMaterialList.Items.Count == 0)
              { %>
            <tr class="tr_bai">
                <td colspan="5" style="text-align: center;">
                    --暂无数据--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
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
            $("#loading").show();
            return true;
        }
        else
            return false;
    }


</script>
