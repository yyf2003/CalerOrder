<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderList.aspx.cs" Inherits="WebApp.OutsourcingOrder.OrderList" %>

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
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/easyui1.4/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <style type="text/css">
        .inputClass
        {
            width: 180px;
        }
        #editDiv li
        {
            margin-bottom: 2px;
            height: 20px;
            font-size: 14px;
            cursor: pointer;
            padding-left: 5px;
            color: Blue;
        }
        #editDiv li:hover
        {
            background-color: #f0f1f2;
            text-decoration: underline;
        }
        </style>
    <script type="text/javascript">
        var url = '<%=url %>';  
    </script>
</head>
<body class="easyui-layout" data-options="fit:true">
    <form id="form1" runat="server">
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                外协订单查询
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'外协信息'" style="width: 200px;">
        <table id="tbOutsource" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: auto;">
        <div id="orderTitle" class="easyui-panel" title=">>外协名称：" data-options="height:'100%',overflow:'auto'">
            <table id="conditionTB" class="table">
                <tr class="tr_bai">
                    <td style="width: 100px;">
                        客户
                    </td>
                    <td style="width: 200px; text-align: left; padding-left: 5px;">
                        <asp:DropDownList ID="ddlCustomer" runat="server" Style="height: 23px;">
                        </asp:DropDownList>
                    </td>
                    <td style="width: 100px;">
                        活动月份
                    </td>
                    <td style="text-align: left; padding-left: 5px;">
                        <%-- <input type="text" id="txtMonth" value="" class="Wdate"  />--%>
                        <asp:TextBox ID="txtMonth" runat="server" CssClass="Wdate" onfocus="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
                            Style="width: 80px;"></asp:TextBox>
                        <span id="spanUp" style="margin-left: 10px; cursor: pointer; color: Blue;">上一个月</span>
                        <span id="spanDown" style="margin-left: 20px; cursor: pointer; color: Blue;">下一个月</span>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        活动名称
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <input type="checkbox" id="guidanceCBALL" style="color: Blue;" />全选
                        <div id="guidanceDiv" style="width: 90%;">
                        </div>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        项目名称
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <img id="ImgLoadSubject" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                        <input type="checkbox" id="subjectCBALL" style="color: Blue;" />全选
                        <div id="subjectDiv" style="width: 90%;">
                        </div>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        材质类型
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <img id="ImgLoadMC" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                        <div id="MaterialCategoryDiv" style="width: 90%;">
                        </div>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        材质名称
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblMaterial" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="软膜">软膜&nbsp;&nbsp;&nbsp;</asp:ListItem>
                            <asp:ListItem Value="非软膜">非软膜&nbsp;&nbsp;&nbsp;</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        店铺类型
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblExportType" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                            <asp:ListItem Value="nohc">非HC订单&nbsp;</asp:ListItem>
                            <asp:ListItem Value="hc">HC订单&nbsp;</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                        类型
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px;">
                        <asp:CheckBoxList ID="cblOutsourceType" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                            <asp:ListItem Value="1">安装&nbsp;</asp:ListItem>
                            <asp:ListItem Value="2">发货&nbsp;</asp:ListItem>
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px; height: 35px;">
                        <input type="button" id="btnSearch" value="查 询" class="easyui-linkbutton" style="width: 65px;
                            height: 26px;" />
                        <img id="loadingImg" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                    </td>
                </tr>
            </table>
            <div>
                <div style=" margin-top:10px; color:Blue; font-weight:bolder; padding-left:10px;">
                提示：红色行已删除
                </div>
                <div class="layui-tab layui-tab-card">
                    <ul class="layui-tab-title">
                        <li class="layui-this" lay-id="1">订单信息</li>
                        <%--<li lay-id="2">店铺信息</li>--%>
                    </ul>
                    <div id="divContent1" class="layui-tab-content" data-options="fit:false" style="padding: 0px;
                        height: 100%;">
                        <div class="layui-tab-item layui-show" style="height: 100%;">
                            <table id="tbOrderList" style="width: 100%;">
                            </table>
                            <div id="toolbar" style="height: 28px; padding-top: 3px;">
                                <a id="btnRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                                    刷新</a> <a id="btnEditOrder" onclick="editOrder()" style="float: left; display:none ;"
                                        class="easyui-linkbutton" plain="true" icon="icon-edit">编辑</a> <a id="btnDeleteOrder"
                                            onclick="deleteOrder()" style="float: left; display:none ;" class="easyui-linkbutton"
                                            plain="true" icon="icon-remove">删除</a>
                                            <a id="btnRecoverOrder"
                                            onclick="recoverOrder()" style="float: left; display:none ;" class="easyui-linkbutton"
                                            plain="true" icon="icon-redo">恢复</a>
                                             <a id="btnChangeOS" onclick="changeOutsource()" style="float: left; display:none ;"
                                        class="easyui-linkbutton" plain="true" icon="icon-edit">修改外协</a>
                                <div class='datagrid-btn-separator'>
                                </div>
                                <a id="btnExport350" style="float: left;" class="easyui-linkbutton" plain="true"
                                    icon="icon-print">导出350</a> <a id="btnExportBJPHW" style="float: left;" class="easyui-linkbutton"
                                        plain="true" icon="icon-print">导出北京模板</a> <a id="btnExportOtherPHW" style="float: left;"
                                            class="easyui-linkbutton" plain="true" icon="icon-print">导出外协模板</a>
                                <div class='datagrid-btn-separator'>
                                </div>
                                <input type="text" id="txtSearchOrderShopNo" />
                            </div>
                        </div>
                       <%-- <div id="divContent2" class="layui-tab-item">
                        </div>--%>
                    </div>
                </div>
            </div>
        </div>
        <div style="display: none;">
            <iframe id="exportFrame" name="exportFrame" src=""></iframe>
        </div>
    </div>




     <div id="editDiv" title="编辑POP信息" style="display: none;">
        <table id="POPtable" class="table" style="width: 750px; text-align: center; margin-bottom: 50px;">
            <tr>
                <td style="height: 30px;width: 100px; ">
                    订单类型
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                     <asp:RadioButtonList ID="rblOrderType" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 30px;">
                    店铺编号
                </td>
                <td style="text-align: left; width: 250px; padding-left: 5px;">
                    <asp:TextBox ID="txtShopNo" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 100px; height: 30px;">
                    POP位置
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <input type="text" id="txtSheet" class="inputClass" />
                        <div id="divSheetMenu" class="inputClass" style="display: none; position: absolute;
                            height: 150px; overflow: auto; background-color: White; border: 1px solid #ccc;
                            padding-top: 2px; z-index: 100;">
                            <ul id="ddlSheetMenu" class="inputClass" style="margin-top: 5; margin-left: 0px;
                                list-style: none;">
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    店铺规模大小
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPOSScale" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    物料支持级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlMaterialSupport" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                        <asp:ListItem Value="Basic">Basic</asp:ListItem>
                        <asp:ListItem Value="Premium">Premium</asp:ListItem>
                        <asp:ListItem Value="VVIP">VVIP</asp:ListItem>
                        <asp:ListItem Value="MCS">MCS</asp:ListItem>
                        <asp:ListItem Value="Generic">Generic</asp:ListItem>
                        <asp:ListItem Value="Others">其他</asp:ListItem>
                    </asp:DropDownList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    器架名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlMachineFrame" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td style="height: 30px;">
                    位置描述
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPositionDescription" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    性别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="position: relative;">
                        <input type="text" id="txtGender" style="width: 120px;" />
                        <div id="divGenderMenu" style="display: none; position: absolute; width: 125px; background-color: White;
                            border: 1px solid #ccc; padding-top: 2px; z-index: 100;">
                            <ul id="ddlGenderMenu" style="margin-top: 0; width: 125px; margin-left: 0px; list-style: none;">
                                <li>男</li>
                                <li>女</li>
                                <li>男女不限</li>
                                <li>无</li>
                            </ul>
                        </div>
                        <span style="color: Red;">*</span>
                    </div>
                </td>
                <td style="height: 30px;">
                    数量
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtQuantity" runat="server" MaxLength="3"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    POP宽
                </td>
                <td style="text-align: left; padding-left: 5px;width: 250px;">
                    <asp:TextBox ID="txtGraphicWidth" runat="server" MaxLength="8" CssClass="inputClass"></asp:TextBox>(mm)
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    POP高
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtGraphicLength" runat="server" MaxLength="8" CssClass="inputClass"></asp:TextBox>(mm)
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    POP材质
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlMaterialCategory" runat="server">
                        <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddlMaterial" runat="server">
                        <asp:ListItem Value="">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="pop">
                <td style="height: 30px;">
                    选图
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtChooseImg" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                </td>
                <td>
                    备注
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" MaxLength="50" Style="width: 200px;"></asp:TextBox>
                </td>
            </tr>
            <tr class="price" style=" display:none;">
                <td style="height: 30px;width: 100px;">
                    应付费用
                </td>
                <td style="text-align: left;width: 250px; padding-left: 5px;">
                    <asp:TextBox ID="txtPayPrice" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 100px;">
                    应收费用
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtReceivePrice" runat="server" MaxLength="20" CssClass="inputClass"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="price" style=" display:none;">
                <td style="height: 30px;width: 100px;">
                    费用备注
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPriceRemark" runat="server" MaxLength="100" Style="width: 200px;"></asp:TextBox>
                </td>
                
            </tr>
        </table>
    </div>

     <div id="divOutsource" style="display:none; width:380px; height:90px; margin:0px;">
       <table class="table">
           <tr>
             <td style=" width:100px; height:35px;">
               请选择新外协：
             </td>
             <td style=" text-align:left; padding-left:10px;">
                <select id="seleOutsource">
                   <option value="0">--请选择--</option>
                </select>
             </td>
           </tr>
           
       </table>
     </div>

    </form>
</body>
</html>
<script src="/Scripts/common.js" type="text/javascript"></script>
<script src="js/orderList.js" type="text/javascript"></script>
