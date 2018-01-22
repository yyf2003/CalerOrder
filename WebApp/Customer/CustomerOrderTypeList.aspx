<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerOrderTypeList.aspx.cs" Inherits="WebApp.Customer.CustomerOrderTypeList" %>

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
            客户订单类型设置
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    客户名称
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:DropDownList ID="ddlCustomerSearch" runat="server">
                    </asp:DropDownList>
                </td>
                <td style="width: 120px;">
                    
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    
                </td>
            </tr>
            
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnSearch_Click"/>&nbsp;&nbsp;
                    <asp:Button ID="btnAdd" runat="server" Text="添 加" OnClientClick="return false" 
                        Visible="false"  class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" onclick="btnAdd_Click" />
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="tr">
        >>信息列表
    </div>
    <div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" 
        CellPadding="0" CssClass="table" BorderWidth="0" 
        HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--" 
            onrowdatabound="gv_RowDataBound" onrowcommand="gv_RowCommand">
           <Columns>
              <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="CustomerName" HeaderText="客户名称" HeaderStyle-Width="200px" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="订单类型"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labOrderType" runat="server" Text=""></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="编辑" Visible="false"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                  
                   <span data-customerid='<%#Eval("Id") %>' name="spanEdit" style="color: blue;
                        cursor: pointer;">编辑</span>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="操作" Visible="false"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                   <asp:LinkButton ID="lbDelete"  CommandArgument='<%#Eval("Id") %>' CommandName="deleteItem" runat="server" OnClientClick="return confirm('确定删除吗？')" ForeColor="Red">删除</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
           </Columns>
            <AlternatingRowStyle CssClass="tr_bai" />
            <HeaderStyle CssClass="tr_hui" />
            <RowStyle CssClass="tr_bai" />
            <SelectedRowStyle CssClass="tr_hui" />
            <EmptyDataRowStyle CssClass="tr_bai" />
        </asp:GridView>
       
    </div>
    <br />
    <div style="text-align:center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="10" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>

     <div id="editDiv" title="订单类型设置" style="display:none;">
        <table style="width:600px;text-align:center;">
           <tr>
                <td style="width:120px;height:30px;">客户名称</td>
                <td style="text-align:left;padding-left:5px;">
                    <asp:DropDownList ID="ddlCustomer" runat="server">
                    </asp:DropDownList>
                    <span style="color:red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="width:120px;height:30px;">订单类型</td>
                <td style="text-align:left;padding-left:5px; line-height:25px; padding-top:15px;">
                    <asp:CheckBoxList ID="cblOrderType" runat="server" RepeatDirection="Horizontal" 
                        RepeatLayout="Flow" RepeatColumns="5">
                    </asp:CheckBoxList>
                </td>
            </tr>
           
            
        </table>
    </div>

    </form>
</body>
</html>
<script src="js/CustomerOrderTypeList.js" type="text/javascript"></script>
