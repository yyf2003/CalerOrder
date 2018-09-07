<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.OutsourcingOrder.PayRecord.List" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="/layui230/css/layui.css" rel="stylesheet" type="text/css" media="all" />
    <script src="/layui230/layui.all.js" type="text/javascript"></script>
    <style type="text/css">
        .redFont{ color:Red;}
    </style>
    <script type="text/javascript">
        function submitSuccess() {
            layer.closeAll();
            layer.msg("添加成功");
            payRecord.getGuidanceList();
        }
        function updateSuccess() {
            $("#hfIsUpdate").val("1");
        }
    </script>
</head>
<body class="easyui-layout">
    <form id="form1" runat="server">
    <div data-options="region:'north',split:true," style="height: 44px; overflow: hidden;">
        <div class="nav_title">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
                class="nav_table_p">
                外协付款记录
            </p>
        </div>
    </div>
    <div data-options="region:'west',split:true,title:'外协信息'" style="width: 200px;">
        <div class="tr_hui" style="height: 30px; text-align: left; padding-left: 5px;">
            外协区域：</div>
        <div id="outsourceRegionDiv" style="padding-left: 5px; padding-top: 8px; padding-bottom: 8px;">
        </div>
        <table id="tbOutsource" style="width: 100%;">
        </table>
    </div>
    <div data-options="region:'center',title:'',height:'100%',iconCls:'icon-ok'" style="overflow: hidden;">
        <div id="orderTitle" class="easyui-panel" title=">>外协名称：" data-options="height:'100%',overflow:'auto'">
            <table class="table">
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
                        <asp:TextBox ID="txtMonth" runat="server" CssClass="Wdate" autocomplete="off" onfocus="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true,onpicked:getMonth})"
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
                        <input type="checkbox" id="guidanceCBALL" style="color: Blue;" />全选
                        <div id="guidanceDiv" style="width: 90%;">
                        </div>
                    </td>
                </tr>
                <tr class="tr_bai">
                    <td>
                    </td>
                    <td colspan="3" style="text-align: left; padding-left: 5px; height: 35px;">
                        <input type="button" id="btnSearch" value="查 询" class="easyui-linkbutton" style="width: 65px;
                            height: 26px;" />
                        <img id="loadingImg" src="../../image/WaitImg/loadingA.gif" style="display: none;" />
                    </td>
                </tr>
            </table>
            <div style="margin-top: 10px;">
                <div>
                    <div class="layui-btn-group demoTable" style="width: 100px; float: left; padding-left:5px;">
                        <a href="javascript:void(0)" class="layui-btn layui-btn-sm" id="btnSubmitMore">
                            批量添加</a>
                    </div>
                    <%--<div style="width: 400px; float: left; line-height:30px;">
                        所选活动应付总数：<span id="selectTotalPrice" style=" font-weight:bold;">0</span>
                    </div>--%>
                    <div style=" float: left; line-height:30px; text-align:left;">
                        应付金额：<span id="SpanAllShouldPay" style=" font-weight:bold;">0</span>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        已付金额：<span id="SpanAllPay" style=" font-weight:bold;">0</span>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                        欠款金额：<span id="SpanAllDebt" style=" font-weight:bold;">0</span>
                    </div>
                </div>
                <div style="clear: both; margin-top: 5px;">
                    <table id="tbGuidanceList" class="layui-hide" lay-filter="tbGuidanceList">
                    </table>
                </div>
            </div>
        </div>
    </div>
    <asp:HiddenField ID="hfIsUpdate" runat="server" />
    <asp:HiddenField ID="hfAddPromission" runat="server" />
    </form>
</body>
</html>
<script src="/Scripts/common.js" type="text/javascript"></script>
<script src="js/list.js" type="text/javascript"></script>
<script type="text/html" id="barDemo">
    <a class="layui-btn layui-btn-primary layui-btn-xs" lay-event="detail">查看</a>
    {{# if($("#hfAddPromission").val()=="1"){}}
    <a class="layui-btn layui-btn-xs" lay-event="add">添加销账</a>
    {{#}}}
</script>
