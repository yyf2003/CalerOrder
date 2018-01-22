<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerList.aspx.cs" Inherits="WebApp.Customer.CustomerList" %>

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
</head>
<body>
    <form id="form1" runat="server">
    
      <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            客户信息
        </p>
    </div>
    <div class="tr">
        >>搜索
    </div>
    <div>
     <table class="table">
            
            <tr class="tr_bai">
                <td style="width: 150px;">
                    客户名称
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
                <td colspan="4" style="padding-right: 10px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" 
                        onclick="btnSearch_Click"  class="easyui-linkbutton" style="width: 65px; height:26px;"/>&nbsp;&nbsp;
                    <asp:Button ID="btnAdd" runat="server" Text="添 加" Visible="false"  class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" onclick="btnAdd_Click" />
                </td>
            </tr>
        </table>
    </div>
     <br />
    <div class="tr">
        >>信息列表
    </div>
    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" 
        CellPadding="0" CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None"
        EmptyDataText="--无符合条件的信息--" onrowcommand="gv_RowCommand" 
        >
       <Columns>
          <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="CustomerCode" HeaderText="客户编码" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="CustomerName" HeaderText="客户名称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="CustomerShortName" HeaderText="简称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="Contact" HeaderText="联系人" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="Tel" HeaderText="联系电话" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="状态" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                   <%#Eval("IsDelete") != null && bool.Parse(Eval("IsDelete").ToString()) ? "<span style='color:red;'>已删除</span>" : "正常"%>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
           </asp:TemplateField>
             <asp:TemplateField HeaderText="区域信息" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <a href="/Regions/List.aspx?customerId=<%#Eval("Id") %>" style=" color:Blue;">查看</a>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="订单类型" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <a href="CustomerOrderTypeList.aspx?customerId=<%#Eval("Id") %>" style=" color:Blue;">查看</a>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="编辑" Visible="false" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton ID="lbEdit" CommandArgument='<%#Eval("Id") %>' CommandName="EditItem" runat="server">编辑</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="删除" Visible="false" HeaderStyle-Width="80px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton ID="lbDelete" runat="server" CommandName="DeleteItem" CommandArgument='<%#Eval("Id") %>' OnClientClick="return confirm('确定删除吗？')">删除</asp:LinkButton>
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
            CssClass="paginator"   CurrentPageButtonClass="cpb" AlwaysShow="True" 
FirstPageText="首页"  LastPageText="尾页" NextPageText="下一页" PrevPageText="上一页"  ShowCustomInfoSection="Left" 
ShowInputBox="Never" CustomInfoTextAlign="Left" LayoutType="Table" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div> 

    <div id="editDiv" title="添加客户" style="display:none;">
        <table style="width:350px;text-align:center;">
           <tr>
                <td style="width:120px;height:30px;">客户编号</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtCode" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width:120px;height:30px;">客户名称</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtCustomerName" runat="server" MaxLength="50"></asp:TextBox>
                    <span style="color:Red;">*</span>
                </td>
            </tr>
           <tr>
                <td style="width:120px;height:30px;">客户简称</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtShortName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width:120px;height:30px;">联系人</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtContact" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="width:120px;height:30px;">联系电话</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:TextBox ID="txtTel" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>


    </form>
   </body>
</html>
<script src="js/Customer.js" type="text/javascript"></script>
