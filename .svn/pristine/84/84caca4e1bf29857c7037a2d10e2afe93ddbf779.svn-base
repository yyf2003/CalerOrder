<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckOrder.aspx.cs" Inherits="WebApp.Subjects.ADOrders.CheckOrder" %>

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
    <script src="../../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            核对订单
        </p>
    </div>
    <div class="tr">
        >>选择项目</div>
    <table class="table">
        <tr class="tr_bai">
            <td>
                客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:DropDownList ID="ddlCustomer" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                时间
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:TextBox ID="txtBeginDate" runat="server" MaxLength="15" onclick="WdatePicker()"
                    ContentEditable="false"></asp:TextBox>
                —
                <asp:TextBox ID="txtEndDate" runat="server" MaxLength="15" onclick="WdatePicker()"
                    ContentEditable="false"></asp:TextBox>
                &nbsp;
                <input type="button" id="btnSearchSubject" value="查  询" class="easyui-linkbutton"
                    style="width: 65px; height: 26px;" />
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目
            </td>
            <td style="text-align: left; padding-left: 5px; height: 80px; vertical-align: top;">
                <div style="width: 80%; margin: 5px;">
                   <input type="checkbox" id="cbALL"/><span style=" color:Blue;">全选</span>
                </div>
                <div id="projectsDiv" style="width: 80%; margin: 5px;">
                   
                </div>
            </td>
        </tr>
    </table>
    <br />
    <div class="tr">
        >>选择核单方案</div>
    <div>
        <table id="tbPlanList" style="width: 100%;">
        </table>
        <div id="toolbar">
            <a id="btnRefresh" onclick="refresh()" style="float: left;" class="easyui-linkbutton"
                plain="true" icon="icon-reload">刷新</a>
            <div class='datagrid-btn-separator'>
            </div>
            <a id="btnAdd" onclick="add()" style="float: left;" class="easyui-linkbutton" plain="true"
                icon="icon-add">新增</a> <a id="btnEdit" onclick="add()" style="float: left;" class="easyui-linkbutton"
                    plain="true" icon="icon-edit">编辑</a>
                    <a id="btnDelte" onclick="deletePlan()" style="float: left;" class="easyui-linkbutton"
                    plain="true" icon="icon-remove">删除</a>
            <div class='datagrid-btn-separator'>
            </div>
        </div>
    </div>
    
    <div style="text-align: center; margin-top:30px;">
        <div id="showButtons">
            <input type="button" id="btnCheckOrder" value="开始核单" class="easyui-linkbutton" style="width: 65px;
                height: 26px;" />
            &nbsp;&nbsp;&nbsp;
            <input type="button" id="btnGoBack" value="返 回" onclick="javascript:window.history.go(-1)"
                class="easyui-linkbutton" style="width: 65px; height: 26px;" />
        </div>
        <div id="showLoading" style="display: none;">
            <img src="../../image/WaitImg/loading1.gif" />
        </div>
    </div>
    <br />
    <br />
    </form>
</body>
</html>
<link href="../../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="../../fancyBox/source/jquery.fancybox.js" type="text/javascript"></script>
<script src="../../fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script src="../js/checkOrder.js" type="text/javascript"></script>
