﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PriceList.aspx.cs" Inherits="WebApp.Outsource.PriceList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/easyui1.4/jquery.min.js"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <script src="/easyui1.4/plugins/jquery.treegrid.js"></script>
    <script src="/easyui1.4/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <script type="text/javascript">
        var companyId = '<%=companyId %>';
        var isOperator = '<%=isOperator %>';
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',split:true," style="height: 90px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" />
             <p class="nav_table_p">
                外协材质报价
            </p>
        </div>
        <div style=" margin-top:0px; vertical-align:top;">
           <table class="table">
           <tr>
             <td style=" width:90px; height:30px;">外协名称：</td>
             <td style=" text-align:left; padding-left:5px;">
                 <asp:Label ID="labName" runat="server" Text=""></asp:Label>
             </td>
           </tr>
        </table>
        </div>
        
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div>
          <table class="table">
             <tr class=" tr_bai"">
                <td style=" width:90px;">客户：</td>
                <td style=" text-align:left; padding-left:5px;">
                   <select id="seleCustomer"></select>
                </td>
             </tr>
             <tr class=" tr_bai"">
                <td>报价：</td>
                <td style=" text-align:left; padding-left:5px;">
                   <select id="selePriceItem"></select>
                </td>
             </tr>
          </table>
        </div>
        <div id="materialTitle" class="easyui-panel" title=">>信息列表" data-options="height:'100%',overflow:'auto'">
           <table id="tbList" style="width: 100%;">
           </table>
            <div id="toolbar" style="height: 28px">
                <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                    刷新</a>
                <div class='datagrid-btn-separator'>
                </div>
                <a id="btnAdd" style="float: left; display:none;" class="easyui-linkbutton" plain="true"
                    icon="icon-add">新增</a> 
                    <a id="btnAddBatch" style="float: left; display:none;" class="easyui-linkbutton" plain="true"
                    icon="icon-add">批量新增</a>
                    <a id="btnEdit" style="float: left; display:none;" class="easyui-linkbutton"
                        plain="true" icon="icon-edit">编辑</a> <a id="btnDelete" style="float: left; display:none;"
                            class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
                 <div id="separator1" class='datagrid-btn-separator' style=" display:none;">
                </div>
            </div>
        </div>
    </div>

    <div id="editPriceDiv" title="添加材质报价" style="display: none;">
        <table style="width: 450px; text-align: center; margin-top: 10px;">
            
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    基础材质：
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <select id="selCategory">
                        <option value="0">类别</option>
                    </select>
                    <select id="selBasicMaterial">
                        <option value="0">材质名称</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    安装单价：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtInstallPrice" maxlength="10" style="width: 100px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    安装+生产单价：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtInstallAndProducePrice" maxlength="10" style="width: 100px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    发货单价：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtSendPrice" maxlength="10" style="width: 100px;" />
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 100px; height: 25px;">
                    单位：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selUnit" disabled="disabled">
                        <option value="0">--请选择--</option>
                    </select>
                    
                </td>
            </tr>
        </table>
    </div>
    <div id="batchAddContainer" title="批量添加材质报价" style="display: none;">
        
            <table class="table">
                <thead>
                    <tr class="tr_hui">
                        <td style="width: 40px;">
                            删除
                        </td>
                        <td style="width: 40px;">
                            序号
                        </td>
                        <td>
                            材质名称
                        </td>
                        <td style="width: 80px;">
                            单位
                        </td>
                        <td style="width: 120px;">
                            安装单价
                        </td>
                        <td style="width: 120px;">
                            安装+生产单价
                        </td>
                        <td style="width: 120px;">
                            发货单价
                        </td>
                    </tr>
                </thead>
                <tbody id="tbMaterialDetail">
                </tbody>
            </table>
       
    </div>
    </form>
</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/priceList.js" type="text/javascript"></script>
