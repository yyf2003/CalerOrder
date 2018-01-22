<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="WebApp.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="CSS/global.css" rel="stylesheet" type="text/css" />
    <link href="CSS/tab.css" rel="stylesheet" type="text/css" />
    <%--<script src="Scripts/jquery-1.4.1.js" type="text/javascript"></script>--%>
   <%-- <script src="Scripts/jquery-1.11.1.js" type="text/javascript"></script>--%>
    <script src="Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="bootstrap-3.3.2-dist/css/bootstrap.css" rel="stylesheet" type="text/css" />
    <script src="bootstrap-3.3.2-dist/js/bootstrap.js" type="text/javascript"></script>
    <link href="layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="layui/layui.js" type="text/javascript"></script>
    <style type="text/css">
       ul
       {
        list-style:none;
        
       }
       ul li{ float:left; margin-right:50px; margin-bottom:50px;}
       #homeNav{ margin:0px;}
       #homeNav li{margin-right:5px; margin-bottom:0px; font-weight:bold;}
       .badge1
       {
        position:absolute;
       	height:22px;
       	width:22px;
       	padding-top:3px;
       	color:white;
       	background-color:red;
        margin-top:-78px;
       	margin-left:50px;
       	font-weight:bold;
       	/*
       	opacity:0.7;
       	filter:alpha(opacity=70);
       	-moz-opacity:0.7;  
        -khtml-opacity: 0.7;  
        */
        text-align:center;
        border-radius: 18px;
        font-size:11px;
       	}
       	#contentHeader div{ float:left;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="main_right" style="width: 100%;height:100%;">
       <div id="menuContainer" style=" height:50%; overflow:hidden;margin:0 auto">
        <div style=" height:30px; margin-left:5px; line-height:30px; color:Black; font-size:15px;">
            
         
         <ul id="homeNav">
            <li><img src="image/home.png" style=" height:25px; margin-top:-4px;"/></li>
            <li>首页</li>
         </ul>
        </div>
        <div class="main_right_tab">
            <div style="background: url(../image/nav_01.gif) repeat-x top; border: 1px solid #CCC; height: 60px;">
                <div class="main_right_tab_01_img">
                    <img src="image/user.jpg" width="40" height="40" /></div>
                <div class="main_right_tab_01_p" style=" font-size:13px; vertical-align:top;">
                    <div style="padding-top: 2px; padding-left: 15px;">
                        <a href="/Users/Edit1.aspx" target="ifrContext" title="修改个人信息"><asp:Label ID="labUserName" runat="server" Text="" ForeColor="Blue"></asp:Label></a>您好！欢迎您登陆！
                     </div>
                    <div style="padding-top: 5px; padding-left: 15px;">
                        当前时间:
                        <span id="time"></span>
                        </div>
                </div>
                
            </div>
            <div class="main_right_nav" style="font-size: 12px;word-break: break-all;word-wrap: break-word;">
                <asp:Label ID="labMenu" runat="server" Text=""></asp:Label>
            </div>
            
        </div>
        </div>
        <div id="subjectContainer" style=" height:50%; width:99%;  margin:0 auto;overflow:hidden; display:none;  position:relative;">
           
           <div id="subjectContainer1" style=" position:absolute; width:100%;background-color:White;">
           <div id="contentHeader">
             <div class="layui-btn layui-btn-normal">项目审批结果</div>
             <div class="spanBar" style=" margin-left:30px; margin-top :5px;">
                <span id="spanDeleteAll" class="layui-btn layui-btn-primary layui-btn-small">
                    <i class="layui-icon">&#xe640;</i>
                    全部清除
                </span>
                <span id="spanRefresh" class="layui-btn layui-btn-primary layui-btn-small">
                    <i class="layui-icon">&#x1002;</i>
                    刷新
                </span>
                <%--<span id="spanClose" class="layui-btn layui-btn-primary layui-btn-small">
                    <i class="layui-icon">&#x1006;</i>
                    关闭
                </span>--%>
             </div>
              <div id="spanHideMsg" data-state="up" style=" color:Blue;cursor:pointer; float:right; margin-right :20px; margin-top :10px;"><i class="layui-icon">&#xe61a;</i></div>
           </div>
           <div id="dataContent" style="overflow:auto; width:100%;" class="layui-collapse" lay-filter="test">
           <table class="layui-table" lay-skin="line" style=" margin-top :0px;">
               <thead>
                  <tr class="tr_hui">
                     <th>客户名称</th>
                     <th>活动名称</th>
                     <th>项目名称</th>
                     <th style="width:180px;">创建时间</th>
                     <th style="width:180px;">审批时间</th>
                     <th style="width:150px;">审批结果</th>
                     <th style="width:90px;">查看</th>
                     <th style="width:90px;">操作</th>
                  </tr>
               </thead>
               <tbody id="tbData" >
                  
               </tbody>
           </table>
           </div>
           </div>
        </div>
    </div>
     
    </form>
</body>
</html>

<script src="Scripts/index.js" type="text/javascript"></script>
<script type="text/javascript">
    $(function () {
        getTime();
        setInterval(getTime, 1000);
    })

    function getTime() {
        var t = new Date().toLocaleString();
        $("#time").html(t);
    }

</script>
