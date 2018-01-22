<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopLogList.aspx.cs" Inherits="WebApp.Customer.EditLog.ShopLogList" %>

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
            
            <div class="layui-input-inline" style="width: 130px;">
                <input type="text" id="txtShopNo" placeholder="店铺编号" class="layui-input"
                    style="height: 25px;" />
            </div>
            <span id="btnSearch" class="layui-btn layui-btn-danger layui-btn-small" style=" height:24px;">
                <i class="layui-icon">&#xe615;</i>
            </span>
        </div>
    </div>
    <div style=" width:100%; overflow:auto;">
         <table class="layui-table" style=" width:2500px;">
            <thead>
                <tr>
                    <th style=" width:30px;">
                        序号
                    </th>
                    <th style=" width:55px;">
                        修改类型
                    </th>
                    <th style=" width:80px;">
                        店铺编码
                    </th>
                    <th>
                        店铺名称
                    </th>
                    <th style=" width:50px;">
                        区域
                    </th>
                    <th style=" width:60px;">
                        省份
                    </th>
                    <th style=" width:60px;">
                        城市
                    </th>
                    <th style=" width:60px;">
                        城市级别
                    </th>
                    <th style=" width:60px;">
                        是否安装
                    </th>
                    <th style=" width:100px;">
                        SP特殊安装费
                    </th>
                    <th style=" width:110px;">
                        三叶草特殊安装费
                    </th>
                    <th style=" width:60px;">
                        Channel
                    </th>
                    <th style=" width:70px;">
                        Format
                    </th>
                    <th style=" width:70px;">
                        渠道
                    </th>
                    <th style=" width:70px;">
                        联系人
                    </th>
                    <th style=" width:70px;">
                        联系人电话
                    </th>
                    <th>
                        店铺地址
                    </th>
                    <th style=" width:60px;">
                        店铺状态
                    </th>
                    <th style=" width:60px;">
                        客服
                    </th>
                    <th>
                        备注
                    </th>
                    <th style=" width:60px;">
                        操作人
                    </th>
                    <th style=" width:150px;">
                        操作时间
                    </th>
                    
            </thead>
            <tbody id="tbodyEmpty">
                <tr>
                    <td colspan="21" style="text-align: center;">
                        --暂无数据--
                    </td>
                </tr>
            </tbody>
            <tbody id="tbodyData" style="display: none;">
            </tbody>
        </table>
        </div>
        <div id="page1" style=" text-align:center; margin-top:10px;"></div>
    </form>
</body>
</html>
<script src="js/ShopLogList.js" type="text/javascript"></script>
