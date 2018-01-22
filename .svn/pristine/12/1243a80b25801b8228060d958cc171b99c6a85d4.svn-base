<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Menu1.aspx.cs" Inherits="WebApp.Menu1" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link href="bootstrap-3.3.2-dist/css/bootstrap.css" rel="stylesheet" />
    <script type="text/javascript" src="Scripts/jquery-1.11.1.js"></script>
    <script type="text/javascript" src="bootstrap-3.3.2-dist/js/bootstrap.js"></script>
    <style type="text/css">
        #content
        {
            margin: 0px;
            width: 230px;
            overflow: auto; /*font-family:'Microsoft YaHei';*/
            font-family: Arial, sans-serif;
            font-size: 13px;
            margin-bottom: 5px;
        }
        
        .menuList
        {
            list-style: none;
        }
        
        .menuList li
        {
            margin-bottom: 4px; /*font-size:13px;*/
        }
        .menuList a
        {
            font-size: 13px;
            font-family: Arial, sans-serif;
            color: #183152;
            padding: 0.25px 0.5px;
        }
        
        .accordion-toggle
        {
            font-size: 13px;
            font-family: Arial, sans-serif;
            color: #183152;
            height: 25px;
        }
        .panel-heading:hover
        {
            background: aliceblue;
        }
    </style>
    <script type="text/javascript">
        function go(url) {
            var ifr = window.parent.document.getElementById("ifrContext");
            ifr.src = url;
            ifr.load;
        }
        $(function () {
            $("#labMenuList").delegate("div[name='goUrl']", "click", function () {
                var url = $(this).data("src");
                go(url);
            })
        })
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label ID="labMenuTitle" runat="server" Text=""></asp:Label>
    </div>
    <div id="content">
        <div class='panel-group' id='accordion1' style='max-height: 515px; overflow: auto; margin-bottom:8px;'>
            <div class='panel panel-default'>
                <div class='panel-heading' data-toggle='collapse' data-parent='#accordion1' href='#collapse1'
                    style='cursor: pointer;'>
                    <%--<a class='accordion-toggle'></a>--%>
                    <span style="font-size: 15px; font-weight: bold;">业务流程</span>
                </div>
                <div id='collapse1' class='panel-collapse'>
                    <div class='panel-body'>
                        <ul class='menuList'>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href="基本信息状态/List.htm"
                                    target='ifrContext'>基本信息状态</a> </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='促销员/List.htm' target='ifrContext'>促销员</a>
                            </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='销量记录/List.htm' target='ifrContext'>销量记录</a>
                            </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='灯箱片/List.htm' target='ifrContext'>灯箱片</a>
                            </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='道具/List.htm' target='ifrContext'>道具</a>
                            </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='柜台现状表/List.htm'
                                    target='ifrContext'>柜台现状表</a> </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='柜台投资/List.htm' target='ifrContext'>柜台投资</a>
                            </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='柜台投资明细/List.htm'
                                    target='ifrContext'>柜台投资明细</a> </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='活动支持/List.htm' target='ifrContext'>活动支持</a>
                            </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='汇总查询/List.htm' target='ifrContext'>汇总查询</a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <!--系统管理-->
        <div class='panel-group' id='accordion2' style='max-height: 515px; overflow: auto;margin-bottom:8px;'>
            <div class='panel panel-default'>
                <div class='panel-heading' data-toggle='collapse' data-parent='#accordion2' href='#collapse2'
                    style='cursor: pointer;'>
                    <span style="font-size: 15px; font-weight: bold;">系统管理</span>
                </div>
                <div id='collapse2' class='panel-collapse collapse'>
                    <div class='panel-body'>
                        <ul class='menuList'>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='Users/List.aspx' target='ifrContext'>用户信息</a>
                            </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='Role/List.aspx' target='ifrContext'>角色信息</a>
                            </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='Modules/Edit.aspx' target='ifrContext'>模块管理</a>
                            </li>
                            <li>
                                <img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='Modules/SetAuthority.aspx' target='ifrContext'>权限设置</a>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
