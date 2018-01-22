<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="menu.aspx.cs" Inherits="WebApp.menu" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

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
            height:100%;
            overflow: auto; /*font-family:'Microsoft YaHei';*/
            font-family: Arial, sans-serif;
            font-size: 13px;
            margin-bottom: 5px;
        }
        
        .menuList
        {
            list-style: none;
           
           margin-left:-10px;
           
        }
        
        .menuList li
        {
            margin-bottom: 4px; /*font-size:13px; margin-left:-20px;*/
             margin-left:-10px;
           
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
</head>
<body>
    <form id="form1" runat="server">
    <div id="content" class="content">
    
    </div>
    </form>
</body>
</html>
<script src="Scripts/menu.js" type="text/javascript"></script>
