<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Users.List" %>

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
      .ul{ list-style:none; margin:0px; padding-left:0px;}
      .ul li{ float:left; margin-right:10px;}
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
                <td colspan="4" style="padding-left: 10px; text-align: left; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" 
                        onclick="btnSearch_Click" class="easyui-linkbutton" style="width: 65px; height:26px;"/>&nbsp;&nbsp;
                    <input type="button" id="btnAdd" value="添 加" class="easyui-linkbutton" style="width: 65px; height:26px;"/>
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="tr">
        >>信息列表</div>
    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" 
        CellPadding="0" CssClass="table" BorderWidth="0" 
        HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--" 
        onrowdatabound="gv_RowDataBound" onrowcommand="gv_RowCommand">
       <Columns>
          <asp:TemplateField HeaderText="序号" HeaderStyle-Width="60px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="RealName" HeaderText="姓名" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="UserName" HeaderText="登陆账号" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="角色"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labRoles" runat="server" Text=""></asp:Label>
                </ItemTemplate>
                
            </asp:TemplateField>
            <asp:BoundField DataField="CompanyName" HeaderText="所属公司" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="负责客户"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labCustomers" runat="server" Text=""></asp:Label>
                </ItemTemplate>
                
            </asp:TemplateField>
            <asp:TemplateField HeaderText="负责活动"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labActivies" runat="server" Text=""></asp:Label>
                </ItemTemplate>
                
            </asp:TemplateField>
            <asp:BoundField DataField="AddDate" HeaderText="添加时间" HeaderStyle-BorderColor="#dce0e9"/>

            <asp:TemplateField HeaderText="状态" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%#Eval("IsDelete") != null && bool.Parse(Eval("IsDelete").ToString()) ?"<span style='color:red;'>已注销</span>":"正常"%>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="编辑" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:HiddenField ID="hfRoleIds" runat="server" />
                    <asp:HiddenField ID="hfCustomerIds" runat="server" />
                    <asp:HiddenField ID="hfAcivityIds" runat="server" />
                    <asp:HiddenField ID="hfCompanyId" runat="server" Value='<%#Eval("CompanyId") %>'/>
                    <span data-userid='<%#Eval("UserId") %>' onclick="editUser(this)" style="color:blue; cursor:pointer;">编辑</span>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="操作" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton ID="lbDelete"  CommandArgument='<%#Eval("UserId") %>' runat="server">删除</asp:LinkButton>
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
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="10" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>


    <div id="editUserDiv" title="添加用户" style="display:none;">
        <table style="width:450px;text-align:center;">
           <tr>
                <td style="width:120px;height:30px;">所属公司</td>
                <td style="text-align:left;padding-left:5px;">
                   <input id="companyInput"  style="width:160px;" />
                </td>
            </tr>
            <tr>
                <td style="width:120px;height:30px;">用户姓名</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtRealName" runat="server" MaxLength="20"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>登陆账号</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtUserName" runat="server" MaxLength="20"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style=" vertical-align:top; padding-top:8px;">选择角色</td>
                <td style="text-align:left;padding-left:5px; height:50px; vertical-align:top;padding-top:8px;">
                 <asp:HiddenField ID="hfRoles" runat="server" />
                 <asp:HiddenField ID="sumbitRoles" runat="server" />
                 <div id="showRoles" style=" height:50px; overflow:auto;"></div>
                 
                </td>
            </tr>
            <tr>
                <td style=" vertical-align:top; padding-top:8px;">负责客户</td>
                <td style="text-align:left;padding-left:5px;  vertical-align:top;padding-top:8px;">
                   <div id="customerContainer"></div>
                </td>
            </tr>
            <tr>
                <td style=" vertical-align:top; padding-top:8px;">负责活动</td>
                <td style="text-align:left;padding-left:5px; vertical-align:top;padding-top:8px;">
                   <div id="activityContainer"></div>
                </td>
            </tr>
        </table>
    </div>


    </form>
</body>
</html>
<script src="js/Users.js" type="text/javascript"></script>
