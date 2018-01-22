<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddShop.aspx.cs" Inherits="WebApp.Subjects.NewShopSubject.AddShop" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
   <%-- <script src="../../Scripts/jquery-1.11.1.js" type="text/javascript"></script>--%>
    <script src="../../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="../../layui/css/layui.css" rel="stylesheet" type="text/css" />
    
    <script src="../../layui/lay/dest/layui.all.js" type="text/javascript"></script>
    <%--<script src="../../layer/layer.js" type="text/javascript"></script>--%>
    <script type="text/javascript">
        var subjectId = '<%=SubjectId %>';
        var region = '<%=Region %>';
        var regionId = '<%=RegionId %>';
        var customerId = '<%=CustomerId %>';
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            添加店铺信息
        </p>
    </div>
    <div>
        <div class="layui-btn-group">
            <span id="btnAdd" class="layui-btn layui-btn-small">
                <i class="layui-icon">&#xe654;</i>
            </span>
            <span id="btnEdit" class="layui-btn layui-btn-small">
                <i class="layui-icon">&#xe642;</i>
            </span>
            <span id="btnDelete" class="layui-btn layui-btn-small">
                <i class="layui-icon">&#xe640;</i>
            </span>
        </div>
    </div>
    <table class="layui-table">
        <thead>
            <tr>
                <th>
                    <input type="checkbox" id="cbAll" />
                </th>
                <th>
                    店铺编号
                </th>
                <th>
                    店铺名称
                </th>
                <th>
                    区域
                </th>
                <th>
                    省份
                </th>
                <th>
                    城市
                </th>
                <th>
                    城市级别
                </th>
                <th>
                    是否安装
                </th>
                <th>
                    Channel
                </th>
                <th>
                    Format
                </th>
                <th>
                    开店时间
                </th>
                <th>
                    订单信息
                </th>
                <th>
                    编辑
                </th>
                <th>
                    删除
                </th>
            </tr>
        </thead>
        <tbody id="tbodyEmpty">
            <tr>
                <td colspan="14" style="text-align: center;">
                    --暂无数据--
                </td>
            </tr>
        </tbody>
        <tbody id="tbodyData" style="display: none;"></tbody>
        
    </table>
    <div id="editShopDiv" style="display: none;">
        <table class="table" style="text-align: center;">
            <tr class="tr_bai">
                <td style="width: 120px; height: 30px;">
                    省份
                </td>
                <td style="text-align: left; width: 200px; padding-left: 5px;">
                    <select id="seleProvince">
                       <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px; width: 120px;">
                    城市
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleCity">
                       <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span> &nbsp;区/县：
                    <select id="seleArea">
                       <option value="0">--请选择--</option>
                    </select>
                    
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    店铺编码
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPOSCode" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    店铺名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPOSName" runat="server" MaxLength="50" Style="width: 220px;"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    经销商编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtAgentNo" runat="server" MaxLength="20"></asp:TextBox>
                </td>
                <td>
                    经销商名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtAgentName" runat="server" MaxLength="50" Style="width: 220px;"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    城市级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleCityTier">
                       <option value="0">--请选择--</option>
                       <option value="T1">T1</option>
                       <option value="T2">T2</option>
                       <option value="T3">T3</option>
                       <option value="T4">T4</option>
                       <option value="T5">T5</option>
                       <option value="T6">T6</option>
                       <option value="T7">T7</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
                <td>
                    安装级别
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:RadioButtonList ID="rblIsInstall" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="Y">Y&nbsp;&nbsp;</asp:ListItem>
                        <asp:ListItem Value="N">N</asp:ListItem>
                    </asp:RadioButtonList>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    联系人1
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtContact1" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    联系人电话1
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtTel1" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    联系人2
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtContact2" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    联系人电话2
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtTel2" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    Channel
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtChannel" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
                <td style="height: 30px;">
                    Format
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtFormat" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    LocationType
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtLocationType" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    BusinessModel
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtBusinessModel" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    开店日期
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtOpenDate" runat="server" onclick="WdatePicker()" MaxLength="20"></asp:TextBox>
                </td>
                <td style="height: 30px;">
                    客服
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlCSUser" runat="server">
                        <asp:ListItem Value="0">--请选择客服--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    店铺地址
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtAddress" runat="server" MaxLength="100" Style="width: 465px;"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    特殊安装费
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtBasicInstallPrice" runat="server" MaxLength="10"></asp:TextBox>
                    （特殊店铺的基础安装费）
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="height: 30px;">
                    备注
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRemark" runat="server" MaxLength="100" Style="width: 465px;"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
<script src="js/addShop.js" type="text/javascript"></script>
