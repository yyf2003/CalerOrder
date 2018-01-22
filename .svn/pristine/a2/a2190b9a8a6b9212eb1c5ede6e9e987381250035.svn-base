<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Role.List" %>

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
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            角色信息
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
            
            <tr class="tr_bai">
                <td style="width: 150px;">
                    角色名称
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtName" runat="server" MaxLength="20"></asp:TextBox>
                </td>
                <td style="width: 150px;">
                    
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   
                </td>
            </tr>
           
            <tr class="tr_bai">
                <td colspan="4" style="padding-left: 10px; text-align: left; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" 
                        onclick="btnSearch_Click"  class="easyui-linkbutton" style="width: 65px; height:26px;"/>&nbsp;&nbsp;
                    <input type="button" id="btnAdd" value="添 加" class="easyui-linkbutton" style="width: 65px; height:26px;"/>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="tr">
        >>信息列表</div>
    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" 
        CellPadding="0" CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None"
        EmptyDataText="--无符合条件的信息--" onrowcommand="gv_RowCommand" 
        >
       <Columns>
          <asp:TemplateField HeaderText="序号" HeaderStyle-Width="60px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="RoleName" HeaderText="角色名称" HeaderStyle-BorderColor="#dce0e9"/>
            
            
            <asp:TemplateField HeaderText="编辑" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    
                    <span data-roleid='<%#Eval("RoleId") %>' onclick="editRole(this)" style="color:blue; cursor:pointer;">编辑</span>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="删除" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton ID="lbDelete" runat="server" CommandArgument='<%#Eval("RoleId") %>' OnClientClick="return confirm('确定删除吗？')">删除</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
       </Columns>
        <AlternatingRowStyle CssClass="tr_bai" />
        <HeaderStyle CssClass="tr_hui"/>
        <RowStyle CssClass="tr_bai" />
        <SelectedRowStyle CssClass="tr_hui" />
        <EmptyDataRowStyle CssClass="tr_bai" />
    </asp:GridView>
    <br />
    <div style="text-align:center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server">
        </webdiyer:AspNetPager>
    </div>


    <div id="editRoleDiv" title="添加角色" style="display:none;">
        <table style="width:350px;text-align:center;">
            <tr>
                <td style="width:120px;height:30px;">角色名称</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtRoleName" runat="server" MaxLength="20"></asp:TextBox>
                </td>
            </tr>
           
            
        </table>
    </div>


    </form>
</body>
</html>
<script src="js/role.js" type="text/javascript"></script>
