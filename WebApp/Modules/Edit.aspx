<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="WebApp.Modules.Edit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../easyui1.4/jquery.min.js"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
    <script src="../easyui1.4/plugins/jquery.treegrid.js"></script>
    <style type="text/css">
      .addBorder
      {
      	 border:1px Gray solid;
      	 border-radius: 10px;
      	 background-color:Gray;
      	 /*opacity:0.3;
       	 filter:alpha(opacity=30);
       	-moz-opacity:0.3;  
        -khtml-opacity: 0.3; */
        
      }
    </style>
    <script type="text/javascript">
        var url = '<%=url %>';
        function Reload() {
            window.location = "Edit.aspx";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <a href="../index.aspx">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
            class="nav_table_p">
            模块管理
        </p>
    </div>
    <table id="modulegrid">
    </table>
    <div id="toolbar">
        <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
            刷新</a>
        <div class='datagrid-btn-separator'>
        </div>
        <a id="btnAdd" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-add">新增</a>
        <a id="btnEdit" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a>
        <a id="btnDelete" style="float: left;display:none;" class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
        <div id="separator1" style="display:none;" class='datagrid-btn-separator'>
        </div>
    </div>
    <div id="editModuleDiv" title="添加模块" style="display: none;">
        <table style="width: 430px; text-align: center;">
            <tr class="tr_bai">
                <td style="width: 120px; height: 30px;">
                    上级模块
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input id="parentModuleInput" style="width: 160px;" />
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    模块名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtName" maxlength="20"/>
                    
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    Url
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtUrl" maxlength="100"/>
                   
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    排序
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtOrderNum" maxlength="20"/>
                    
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    是否显示
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:RadioButtonList ID="rblIsShow" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="是" Selected="True">是 </asp:ListItem>
                        <asp:ListItem Value="否">否 </asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    是否首页显示
                </td>
                <td style="text-align: left; padding-left: 5px; ">
                    <asp:RadioButtonList ID="rblShowOnHome" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="是">是 </asp:ListItem>
                        <asp:ListItem Value="否" Selected="True">否 </asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            
        </table>
    </div>
  
    <div id="ImgDiv" title="选择图片" style="display:none; text-align:center;">
       <img id="loadingImg" style="display:none;" src="../image/WaitImg/loading1.gif" />
       <div id="showfiles" style=" margin-left:10px;"></div>
    </div>

    </form>
</body>
</html>
<link href="/layer/skin/default/layer.css" rel="stylesheet" type="text/css" />
<script src="/layer/layer.js" type="text/javascript"></script> 
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/module.js" type="text/javascript"></script>
