<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HCSmallSizeList.aspx.cs" Inherits="WebApp.TableSize.HCSmallSizeList" %>

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
        var url = '<%=url %>';
    </script>
    <style type="text/css">
      
      .deleteSmall{ color:red; cursor:pointer;}
    </style>
</head>
<body class="easyui-layout">
    
     <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                HC小尺寸信息
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'店铺类型'" style="width: 150px;">
        <table id="tbShopFormat" style="width: 100%;">
        </table>
    </div>
   <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="machineFrameTitle" class="easyui-panel" title=">>尺寸信息" data-options="height:'100%',overflow:'auto'">
            <table id="tbList" style="width: 100%;">
            </table>
            <div id="toolbar" style="height: 25px; margin-top: 0px;">
                <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                    刷新</a>
                <div class='datagrid-btn-separator'>
                </div>
                <a id="btnAdd" style="float: left; display: none;" class="easyui-linkbutton" plain="true"
                    icon="icon-add">添加</a> <a id="btnEdit" style="float: left; display: none;" class="easyui-linkbutton"
                        plain="true" icon="icon-edit">编辑</a> <a id="btnDelete" style="float: left; display: none;"
                            class="easyui-linkbutton" plain="true" icon="icon-remove">删除</a>
            </div>
        </div>
    </div>

    <div id="editDiv" title="添加" style="display: none;">
        <table style="width: 500px; text-align: center;">
            <tr class="tr_bai">
                <td style="width: 150px;">
                    Format：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selFormat">
                        <option value="">--请选择--</option>
                        <option value="HCE">HCE</option>
                        <option value="YA">YA</option>
                        <option value="TERREX">TERREX</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 150px;">
                    POP位置：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selSheet">
                        <option value="鞋墙">鞋墙</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    备注说明：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtRemark" maxlength="20"/>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    宽（大）：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtBigWidth" maxlength="7"/>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    高（大）：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input type="text" id="txtBigLength" maxlength="7"/>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
               <td style=" height:30px;vertical-align:top; padding-top:5px;">小画面尺寸：</td>
               <td style="text-align: left; padding-left: 5px; vertical-align:top; padding-top:5px;">
                 <div id="divAdd" style=" width:100px; margin-bottom :5px;">
                   <%-- <span id="spanAdd" style="color:blue;cursor:pointer; text-decoration:underline;">+添加</span>--%>
                   <a id="spanAdd"  class="easyui-linkbutton" plain="true" icon="icon-add"></a>
                 </div>
                 <div id="sizeContainer">
                 <div class="divAddSize">
                 宽：<input type="text" name="txtWidth" style=" text-align:center; width:100px;"/>
                 &nbsp;&nbsp;&nbsp;&nbsp;
                 高：<input type="text" name="txtLength" style=" text-align:center; width:100px;"/>
                 
                 </div>
                 <div class="divAddSize">
                 宽：<input type="text" name="txtWidth" style=" text-align:center; width:100px;"/>
                 &nbsp;&nbsp;&nbsp;&nbsp;
                 高：<input type="text" name="txtLength" style=" text-align:center; width:100px;"/>
                  <span name="deleteSmallSize" class="deleteSmall">移除</span>
                 </div>
                 <div class="divAddSize">
                 宽：<input type="text" name="txtWidth" style=" text-align:center; width:100px;"/>
                 &nbsp;&nbsp;&nbsp;&nbsp;
                 高：<input type="text" name="txtLength" style=" text-align:center; width:100px;"/>
                 <span name="deleteSmallSize" class="deleteSmall">移除</span>
                 </div>
                 <div class="divAddSize">
                 宽：<input type="text" name="txtWidth" style=" text-align:center; width:100px;"/>
                 &nbsp;&nbsp;&nbsp;&nbsp;
                 高：<input type="text" name="txtLength" style=" text-align:center; width:100px;"/>
                 <span name="deleteSmallSize" class="deleteSmall">移除</span>
                 </div>
                 <div class="divAddSize">
                 宽：<input type="text" name="txtWidth" style=" text-align:center; width:100px;"/>
                 &nbsp;&nbsp;&nbsp;&nbsp;
                 高：<input type="text" name="txtLength" style=" text-align:center; width:100px;"/>
                 <span name="deleteSmallSize" class="deleteSmall">移除</span>
                 </div>
                 </div>
               </td>
            </tr>
            
        </table>
    </div>


</body>
</html>
<script src="../Scripts/common.js" type="text/javascript"></script>
<script src="js/hcSmallSizeList.js" type="text/javascript"></script>
