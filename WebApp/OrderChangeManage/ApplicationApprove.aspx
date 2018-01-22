﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplicationApprove.aspx.cs"
    Inherits="WebApp.OrderChangeManage.ApplicationApprove" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script type="text/javascript">
        function Success() {
            layer.msg("审批成功！");
            setTimeout(function () {
                window.location = "ApplicationApproveList.aspx";
            }, 1000);
        }

        function Fail(msg) {
            layer.confirm(msg);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            项目变更申请审批
        </p>
    </div>
    <blockquote class="layui-elem-quote" style="height: 15px; padding-top: 7px; font-weight: bold;">
        申请信息
    </blockquote>
    <table class="layui-table" style="margin-top: -10px;">
        <tr>
            <td style="width: 80px;">
                申请人：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labApplicationUser" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                申请时间：
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <asp:Label ID="labApplicationDate" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                活动名称：
            </td>
            <td style="text-align: left; padding-left: 5px;">
               <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>
    <asp:Repeater ID="Repeater1" runat="server" 
        onitemdatabound="Repeater1_ItemDataBound">
        <HeaderTemplate>
            <table class="layui-table" style="margin-top: -10px;">
                <thead>
                    <tr>
                        <th style="width:80px;">
                            项目类型
                        </th>
                        <th style="width:120px;">
                            项目编号
                        </th>
                        <th style="width: 300px;">
                            项目名称
                        </th>
                        <th style="width: 100px;">
                            变更类型
                        </th>
                        <th>
                            变更说明
                        </th>
                    </tr>
                </thead>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <asp:Label ID="labSubjectType" runat="server" Text=""></asp:Label>
                </td>
                 <td>
                  <%#Eval("SubjectNo")%>
                </td>
                <td>
                  <%#Eval("SubjectName")%>
                </td>
                <td>
                    <asp:Label ID="labChangeType" runat="server" Text=""></asp:Label>
                </td>
                <td>
                  <%#Eval("Remark")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <%if (Repeater1.Items.Count == 0)
              {%>
            <tr>
                <td colspan="3" style="text-align: center;">
                    --无数据信息--
                </td>
            </tr>
            <%} %>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <blockquote class="layui-elem-quote" style="height: 15px; padding-top: 7px; font-weight: bold;">
        审批
    </blockquote>
    <table class="layui-table" style="margin-top: -10px;">
      <tr class="tr_bai">
        <td style="width:80px;">审批结果</td>
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
   
    <div style="text-align: center; margin-bottom:20px;">
        <asp:Button ID="btnSubmit" runat="server" Text="提 交" OnClientClick="return Check()" CssClass="layui-btn layui-btn-normal" onclick="btnSubmit_Click"/>
        <img id="loadingImg"  src="../image/WaitImg/loadingA.gif" style=" display:none;"/>
        <input type="button" value="返 回" onclick="javascript:window.history.go(-1)" class="layui-btn layui-btn-primary" style="margin-left:30px;"/>
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
            layer.msg("请选择审批结果");
            return false;
        }
        if (result == 2) {
            if ($.trim($("#txtRemark").val()) == "") {
                layer.msg("请填写审批意见");
                return false;
            }
        }
        $("#loadingImg").show();
        return true;
    }

    
</script>
