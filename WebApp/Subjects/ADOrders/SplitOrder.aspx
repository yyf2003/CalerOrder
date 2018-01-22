﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SplitOrder.aspx.cs" Inherits="WebApp.Subjects.ADOrders.SplitOrder" %>

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
    <script src="../../easyui1.4/datagrid-detailview.js" type="text/javascript"></script>
    
    
      <script type="text/javascript"> 
        function HasChange() {
            $("#hfIsChange").val("1");
        }
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            拆分订单
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目编号
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>
                <asp:HiddenField ID="hfCustomerId" runat="server" />
                <asp:HiddenField ID="hfSubjectId" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <div class="tr">
        >>选择拆单方案</div>
    <div >
    <table id="tbPlanList" style="width: 100%;">
    </table>
    <div id="toolbar">
        <a id="btnRefresh" onclick="refresh()" style="float:left;" class="easyui-linkbutton" plain="true" icon="icon-reload">刷新</a>
        <div class='datagrid-btn-separator'></div>
        <a id="btnAdd" onclick="add()" style="float:left;" class="easyui-linkbutton" plain="true" icon="icon-add">新增</a>
        <a id="btnEdit" onclick="add()" style="float:left;" class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
        <a id="btnDelete" onclick="deletePlan()" style="float:left;" class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
        <div class='datagrid-btn-separator'></div>
    </div>
    </div>
    <br />
    <br />
    <div style="text-align: center;">
     <div id="showButtons">
        <input type="button" id="btnSplitPlan" value="执行方案" class="easyui-linkbutton" style="width: 65px;
            height: 26px;"/>
        &nbsp;&nbsp;&nbsp;
        <%--<input type="button" id="btnGoBack" value="返 回" onclick="javascript:window.history.go(-1)" class="easyui-linkbutton" style="width: 65px;
            height: 26px;" />--%>
         <asp:Button ID="btnGoBack" runat="server" Text="上一步" class="easyui-linkbutton" style="width: 65px;
            height: 26px;" onclick="btnGoBack_Click"/>
            &nbsp;&nbsp;&nbsp;
         <asp:Button ID="btnNext" runat="server" Text="下一步" class="easyui-linkbutton" style="width: 65px;
            height: 26px;" onclick="btnNext_Click" />
           
     </div>
     <div id="showLoading" style="display:none;">
         <img src="../../image/WaitImg/loading1.gif" />
     </div>
    </div>
    <br />
    <br />
    <asp:HiddenField ID="hfIsChange" runat="server" />
    </form>
</body>
</html>

<link href="../../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="../../fancyBox/source/jquery.fancybox.js" type="text/javascript"></script>
<script src="../../fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>


<script src="../../Subjects/js/splitOrder.js" type="text/javascript"></script>
