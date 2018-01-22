<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="WebApp.Register" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8">
    <title>会员注册</title>
    <meta name="renderer" content="webkit">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">
    <link href="layui/css/layui.css" rel="stylesheet" type="text/css" media="all" />
    <script src="Scripts/jquery-1.11.1.js" type="text/javascript"></script>
</head>
<body>
    <form runat="server" class="layui-form layui-form-pane">
    <div style="margin: 20px; padding-top: 20px; width: 60%; margin-left: auto; margin-right: auto;">
        <fieldset class="layui-elem-field layui-field-title" style="margin-top: 10px; font-size: 20px;
            font-weight: bold;">
            <legend>会员注册</legend>
        </fieldset>
        <blockquote class="layui-elem-quote">
        注册成功后就可以在本系统下订单了！
        </blockquote>
        <div class="layui-form-item">
            <label class="layui-form-label">
                姓 名</label>
            <div class="layui-input-inline">
                <input type="text" name="title" autocomplete="off" placeholder="请输入姓名" class="layui-input"
                    style="width: 380px;" />
            </div>
        </div>
        <div class="layui-form-item">
            <label class="layui-form-label">
                性 别</label>
            <div class="layui-input-block">
                <input type="radio" name="sex" value="男" title="男">
                <input type="radio" name="sex" value="女" title="女">
            </div>
        </div>
        <div class="layui-form-item">
            <label class="layui-form-label">
                登录账号</label>
            <div class="layui-input-inline">
                <input type="text" name="title" autocomplete="off" placeholder="请输入登录账号" class="layui-input"
                    style="width: 380px;" />
            </div>
        </div>
        <div class="layui-form-item">
            <label class="layui-form-label">
                密 码</label>
            <div class="layui-input-inline">
                <input type="password" name="title" autocomplete="off" placeholder="请输入密码" class="layui-input"
                    style="width: 380px;" />
            </div>
        </div>
        <div class="layui-form-item">
            <label class="layui-form-label">
                密码确认</label>
            <div class="layui-input-inline">
                <input type="password" name="title" autocomplete="off" placeholder="请再一次输入密码" class="layui-input"
                    style="width: 380px;" />
            </div>
        </div>
        <div class="layui-form-item">
            <div class="layui-inline">
                <label class="layui-form-label">
                    手机号码</label>
                <div class="layui-input-inline">
                    <input type="tel" name="tel" class="layui-input" lay-verify="tel" />
                </div>
            </div>
            <div class="layui-inline">
                <label class="layui-form-label">
                    邮箱地址</label>
                <div class="layui-input-inline">
                    <input type="email" name="email" class="layui-input" lay-verify="email" style="width: 266px;" />
                </div>
            </div>
        </div>
        <div class="layui-form-item">
            <label class="layui-form-label">
                所在区域</label>
            <div class="layui-input-inline">
                <select name="a1">
                    <option value="">请选择省</option>
                    <option value="浙江省">浙江省</option>
                    <option value="江西省">江西省</option>
                    <option value="福建省">福建省</option>
                </select>
            </div>
            <div class="layui-input-inline">
                <select name="a2">
                    <option value="">请选择市</option>
                    <option value="杭州">杭州</option>
                    <option value="宁波">宁波</option>
                    <option value="温州">温州</option>
                    <option value="温州">台州</option>
                    <option value="温州">绍兴</option>
                </select>
            </div>
            <div class="layui-input-inline">
                <select name="a3">
                    <option value="">请选择县/区</option>
                    <option value="西湖区">西湖区</option>
                    <option value="余杭区">余杭区</option>
                    <option value="拱墅区">临安市</option>
                </select>
            </div>
        </div>
        
        <div class="layui-form-item">
            <label class="layui-form-label">
                详细地址</label>
            <div class="layui-input-block">
                <input type="text" name="title" autocomplete="off" class="layui-input" />
            </div>
        </div>
    </div>
    <div style=" height:50px; margin-top:30px; margin-bottom:20px; text-align:center;">
        <asp:Button ID="Button1" runat="server" Text="提  交" class="layui-btn" style="width:80px;"/>
        <asp:Button ID="Button2" runat="server" Text="取  消" class="layui-btn layui-btn-warm" style="width:80px; margin-left:50px;"/>
    </div>
    </form>
</body>
</html>
<script src="layui/layui.js" type="text/javascript"></script>
<script src="layui/lay/dest/layui.all.js" type="text/javascript"></script>
<script type="text/javascript">
    layui.use('form', function () {
        var form = layui.form();
        var layer = layui.layer;
        var layedit = layui.layedit;
        var laydate = layui.laydate;
        form.verify({
            tel: [/^1[3|4|5|7|8]\d{9}$/, '手机必须11位，只能是数字！'],
            email: [/^[a-z0-9._%-]+@([a-z0-9-]+\.)+[a-z]{2,4}$|^1[3|4|5|7|8]\d{9}$/, '邮箱格式不对']
        });
    });

</script>
