<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckPayDetail.aspx.cs"
    Inherits="WebApp.OutsourcingOrder.PayRecord.CheckPayDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="/layui230/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui230/layui.js" type="text/javascript"></script>
    <script type="text/javascript">
        var guidanceId = '<%=guidanceId %>';
        var outsourceId = '<%=outsourceId %>';
        var currUserId = '<%=currUserId %>';
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" style=" display:none;"/>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 100px;">
                    外协名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labOutsourceName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    活动名称：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    应付金额：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labShouldPay" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    已付金额：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:Label ID="labPay" runat="server" Text="0"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <div>
       <table class="layui-hide" id="tbPayRecordList" lay-filter="tbPayRecordList" style="color: #000;"></table>
    </div>

    <div id="editDiv" style=" width:500px; display:none;">
      <table class="table">
         <tr class="tr_bai">
            <td style=" width:100px;">付款日期：</td>
            <td style=" text-align:left; padding-left:5px;">
               <asp:TextBox ID="txtPayDate" runat="server" CssClass="Wdate" onclick="WdatePicker()"></asp:TextBox>
            </td>
         </tr>
         <tr class="tr_bai">
            <td>付款金额：</td>
            <td style=" text-align:left; padding-left:5px;">
               <asp:TextBox ID="txtPay" runat="server" MaxLength="10" autocomplete="off"></asp:TextBox>
            </td>
         </tr>
         <tr class="tr_bai">
            <td>备注：</td>
            <td style=" text-align:left; padding-left:5px;">
               <asp:TextBox ID="txtRemark" runat="server" autocomplete="off" Style="width: 350px;"></asp:TextBox>
            </td>
         </tr>
      </table>
    </div>

    </form>
</body>
</html>
<script src="js/checkDetail.js" type="text/javascript"></script>
 <script type="text/html" id="barDemo">
    <a class="layui-btn layui-btn-primary layui-btn-xs" lay-event="edit">修改</a>
    <a class="layui-btn layui-btn-danger layui-btn-xs" lay-event="delete">删除</a>
 </script>