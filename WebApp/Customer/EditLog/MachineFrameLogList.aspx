<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MachineFrameLogList.aspx.cs"
    Inherits="WebApp.Customer.EditLog.MachineFrameLogList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var shopId = '<%=ShopId %>';
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="layui-inline" style="padding-left: 10px; padding-top: 10px;">
            搜索： 从
            <div class="layui-input-inline" style="width: 130px;">
                <input type="text" id="txtBeginDate" placeholder="开始时间" onfocus="WdatePicker()"
                    class="layui-input" style="height: 25px;" />
            </div>
            至
            <div class="layui-input-inline" style="width: 130px;">
                <input type="text" id="txtEndDate" placeholder="结束时间" onfocus="WdatePicker()" class="layui-input"
                    style="height: 25px;" />
            </div>
            <span id="btnSearch" class="layui-btn layui-btn-danger layui-btn-small" style=" height:24px;">
                <i class="layui-icon">&#xe615;</i>
            </span>
        </div>
        <table class="layui-table">
            <thead>
                <tr>
                    <th style=" width:30px;">
                        序号
                    </th>
                    <th style=" width:80px;">
                        位置
                    </th>
                    <th>
                        器架名称
                    </th>
                    <th style=" width:90px;">
                        性别
                    </th>
                    <th style=" width:70px;">
                        数量
                    </th>
                    <th style=" width:120px;">
                        角落类型
                    </th>
                    <th style=" width:100px;">
                        是否生产
                    </th>
                    <th style=" width:120px;">
                        操作人
                    </th>
                    <th style=" width:120px;">
                        操作时间
                    </th>
                    <th style=" width:60px;">
                        备注
                    </th>
            </thead>
            <tbody id="tbodyEmpty">
                <tr>
                    <td colspan="10" style="text-align: center;">
                        --暂无数据--
                    </td>
                </tr>
            </tbody>
            <tbody id="tbodyData" style="display: none;">
            </tbody>
        </table>
        <div id="page1" style=" text-align:center; margin-top:10px;"></div>
    </div>
    </form>
</body>
</html>
<script src="js/MachineFrameLogList.js" type="text/javascript"></script>
