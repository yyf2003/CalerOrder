<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OutsourceSubject.aspx.cs"
    Inherits="WebApp.OutsourcingOrder.OutsourceSubject" %>

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
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/easyui1.4/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            外协项目查询
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
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
                <img id="ImgLoadGuidance" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                <input type="checkbox" id="guidanceCBALL" /><span style="color: Blue;">全选</span>
                <div id="guidanceDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                区域
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <img id="ImgLoadRegion" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                <div id="RegionDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目类型
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <img id="ImgLoadSubjectCategory" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                <div id="subjectCategoryDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                项目名称
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <img id="ImgLoadSubject" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                <input type="checkbox" id="subjectCBALL" /><span style="color: Blue;">全选</span>
                <div id="subjectDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                省份
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <img id="ImgLoadProvince" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                <input type="checkbox" id="provinceCBAll" /><span style="color: Blue;">全选</span>
                <div id="ProvinceDiv" style="width: 90%;">
                </div>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                城市
            </td>
            <td colspan="3" style="text-align: left; padding-left: 5px;">
                <img id="ImgLoadCity" src="/image/WaitImg/loadingA.gif" style="display: none;" />
                <input type="checkbox" id="cityCBAll" /><span style="color: Blue;">全选</span>
                <div id="CityDiv" style="width: 90%;">
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
    <div style=" margin-top:10px;">
        <table id="tbOrderList">
        </table>
        <div id="toolbar" style="height: 28px">
            <a id="btnIRefresh" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-reload">
                刷新</a>
            <div class='datagrid-btn-separator'>
            </div>
            <a id="btnIAdd" style="float: left;" class="easyui-linkbutton" plain="true" icon="icon-add">
                新增</a> <a id="btnIEdit" style="float: left;" class="easyui-linkbutton" plain="true"
                    icon="icon-edit">编辑</a> <a id="btnIDelete" style="float: left;" class="easyui-linkbutton"
                        plain="true" icon="icon-remove">删除</a> <a id="btnIRecover" style="float: left;" class="easyui-linkbutton"
                            plain="true" icon="icon-redo">恢复</a>
            <div class='datagrid-btn-separator'>
            </div>
            <input type="text" id="txtISearchShopNo" />
        </div>
    </div>
    </form>
</body>
</html>
<script src="js/OutsourceSubject.js" type="text/javascript"></script>
