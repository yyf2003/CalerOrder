<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List1.aspx.cs" Inherits="WebApp.Users.List1" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../easyui1.4/jquery.min.js"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
    <script src="../easyui1.4/plugins/jquery.treegrid.js"></script>
    <style type="text/css">
        .ul
        {
            list-style: none;
            margin: 0px;
            padding-left: 0px;
        }
        .ul li
        {
            float: left;
            margin-right: 10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            用户信息
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 150px;">
                    用户姓名
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRealName1" runat="server" MaxLength="20"></asp:TextBox>
                </td>
                <td style="width: 150px;">
                    登陆账号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtUserName1" runat="server" MaxLength="20"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 150px;">
                    角色
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:CheckBoxList ID="cblRole" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow"
                        RepeatColumns="8">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 10px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" OnClick="btnSearch_Click" class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" />&nbsp;&nbsp;
                    <asp:Button ID="btnAdd" runat="server" Text="添 加" OnClientClick="return false" Visible="false"
                        class="easyui-linkbutton" Style="width: 65px; height: 26px;" />
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="tr">
        >>信息列表</div>
    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
        CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--"
        OnRowDataBound="gv_RowDataBound" OnRowCommand="gv_RowCommand">
        <Columns>
            <asp:TemplateField HeaderText="序号" HeaderStyle-Width="60px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="RealName" HeaderText="姓名" HeaderStyle-BorderColor="#dce0e9" />
            <asp:BoundField DataField="UserName" HeaderText="登陆账号" HeaderStyle-BorderColor="#dce0e9" />
            <asp:TemplateField HeaderText="角色" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labRoles" runat="server" Text=""></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="CompanyName" HeaderText="所属公司" HeaderStyle-BorderColor="#dce0e9" />
            <asp:TemplateField HeaderText="负责客户" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labCustomers" runat="server" Text=""></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="LevelName" HeaderText="用户级别" HeaderStyle-BorderColor="#dce0e9" />
            <asp:TemplateField HeaderText="负责区域" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labRegions" runat="server" Text=""></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:BoundField DataField="AddDate" HeaderText="添加时间" HeaderStyle-BorderColor="#dce0e9" />--%>
            <asp:TemplateField HeaderText="状态" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%#Eval("IsDelete") != null && bool.Parse(Eval("IsDelete").ToString()) ?"<span style='color:red;'>已注销</span>":"正常"%>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="权限设置" Visible="false" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <span data-userid='<%#Eval("UserId") %>' onclick="editPermission(this)" style="color: blue;
                        cursor: pointer;">模块权限</span> | <span data-userid='<%#Eval("UserId") %>' onclick="editOrderApproveRight(this)"
                            style="color: blue; cursor: pointer;">订单审批权限</span>
                </ItemTemplate>
                <HeaderStyle Width="150px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="编辑" HeaderStyle-Width="80px" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%--<asp:HiddenField ID="hfRoleIds" runat="server" />
                    <asp:HiddenField ID="hfCustomerIds" runat="server" />
                    <asp:HiddenField ID="hfCompanyId" runat="server" Value='<%#Eval("CompanyId") %>' />
                    <asp:HiddenField ID="hfRegionIds" runat="server" />--%>
                    <span data-userid='<%#Eval("UserId") %>' onclick="editUser(this)" style="color: blue;
                        cursor: pointer;">编辑</span> | <span data-userid='<%#Eval("UserId") %>' onclick="editOutsource(this)"
                            style="color: blue; cursor: pointer;">负责外协</span> |
                    <asp:LinkButton ID="lbReSetPassord" CommandArgument='<%#Eval("UserId") %>' CommandName="ReSetPassord"
                        runat="server">重置密码</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="180px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="操作" Visible="false" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton ID="lbDelete" CommandArgument='<%#Eval("UserId") %>' CommandName="DeleteUser"
                        runat="server">删除</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
        </Columns>
        <AlternatingRowStyle CssClass="tr_bai" />
        <HeaderStyle CssClass="tr_hui" />
        <RowStyle CssClass="tr_bai" />
        <SelectedRowStyle CssClass="tr_hui" />
        <EmptyDataRowStyle CssClass="tr_bai" />
    </asp:GridView>
    <br />
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="10" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <div id="editUserDiv" title="添加用户" style="display: none;">
        <table style="width: 580px; text-align: center;">
            <tr>
                <td style="width: 120px; height: 30px;">
                    所属公司
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <input id="companyInput" style="width: 160px;" />
                </td>
            </tr>
            <tr>
                <td style="width: 120px; height: 30px;">
                    用户姓名
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtRealName" runat="server" MaxLength="40"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    登陆账号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtUserName" runat="server" MaxLength="40"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; padding-top: 8px;">
                    用户级别
                </td>
                <td style="text-align: left; padding-left: 5px; height: 30px; vertical-align: top;
                    padding-top: 8px;">
                    <select id="selUserLevel">
                        <option value="0">--请选择--</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; padding-top: 8px;">
                    负责客户
                </td>
                <td style="text-align: left; padding-left: 5px; height: 30px; vertical-align: top;
                    padding-top: 8px;">
                    <div id="customerContainer">
                    </div>
                </td>
            </tr>
            <tr>
                <td style="vertical-align: top; padding-top: 8px;">
                    选择角色
                </td>
                <td style="text-align: left; padding-left: 5px; height: 50px; vertical-align: top;
                    padding-top: 8px;">
                    <asp:HiddenField ID="sumbitRoles" runat="server" />
                    <div id="showRoles" style="height: 50px;">
                    </div>
                </td>
            </tr>
            <tr class="userRegionTr" style="display: none;">
                <td style="font-weight: bold; height: 20px;">
                    负责区域：
                </td>
                <td>
                </td>
            </tr>
            <tr class="userRegionTr" style="display: none;">
                <td>
                    区 域：
                </td>
                <td>
                    <div id="regionContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                    </div>
                </td>
            </tr>
            <tr class="userRegionTr" style="display: none;">
                <td style="vertical-align: top; padding-top: 8px;">
                    省 份：
                </td>
                <td>
                    <div id="provinceContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                    </div>
                </td>
            </tr>
            <tr class="userRegionTr" style="display: none;">
                <td style="vertical-align: top; padding-top: 8px;">
                    城 市：
                </td>
                <td>
                    <div id="cityContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                    </div>
                </td>
            </tr>
            <tr class="userRegionTr" style="display: none;">
                <td style="vertical-align: top; padding-top: 8px;">
                    区/县：
                </td>
                <td>
                    <div id="areaContainer" style="margin-left: 0px; padding-left: 5px; text-align: left;">
                    </div>
                </td>
            </tr>
        </table>
        <p>
        </p>
    </div>
    <div id="addOutsourceDiv" title="添加用户" style="display: none;">
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 80px;">
                    省份：
                </td>
                <td style="width: 150px; text-align: left; padding-left: 5px;">
                    <select id="seleProvince">
                        <option value="0">--请选择省份--</option>
                    </select>
                </td>
                <td style="width: 80px;">
                    城市：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="seleCity">
                        <option value="0">--请选择城市--</option>
                    </select>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: left; padding-left: 15px; height: 300px; vertical-align: top;">
                    <div id="outsourceListDiv" style="margin-top: 15px;">
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <div id="editUserPrimissionDiv" title="设置权限" style="display: none;">
        <div style="padding-left: 8px; height: 30px; background: #e5e2e2; padding-top: 5px;">
            角色：
            <select id="seleRole" class="easyui-combobox"></select>
            &nbsp;&nbsp;&nbsp;
            <input type="checkbox" id="cbExpand" />展开
        </div>
        <img id="loadModuleImg" style=" display:none;" src="../image/WaitImg/loading1.gif" />
        <table id="modulegrid">
        </table>
    </div>
    <div id="editOrderApprovePermission" title="设置订单审批权限" style="display: none;">
      <table class="table">
        <tr class="tr_bai">
           <td style=" width:120px;">请选择订单类型：</td>
           <td>
             <div id="orderTypeDiv"></div>
           </td>
        </tr>
      </table>
    </div>
    <asp:HiddenField ID="hfRoles" runat="server" />
    </form>
</body>
</html>
<script src="js/Users.js" type="text/javascript"></script>
