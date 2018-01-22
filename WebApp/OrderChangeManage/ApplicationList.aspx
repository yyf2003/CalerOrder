<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplicationList.aspx.cs"
    Inherits="WebApp.OrderChangeManage.ApplicationList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <link href="/layui/css/layui.css" rel="stylesheet" type="text/css" />
    <script src="/layui/lay/dest/layui.all.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            项目变更申请信息
        </p>
    </div>
    <table style="width: 100%;">
        <tr>
            <%--<td style="width: 80px; text-align: left; padding-left: 10px;">
            </td>--%>
            <td style="text-align: left; padding-left: 10px;">
                <div class="layui-btn-group">
                    <span id="btnAdd" class="layui-btn layui-btn-small"><i class="layui-icon">&#xe654;</i>新增申请</span>
                    
                </div>
            </td>
        </tr>
    </table>
    <div style="margin-top: 10px;">
         <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" 
            CssClass="layui-table" HeaderStyle-BackColor="Gray" 
             EmptyDataText="--无符合条件的信息--" onrowcommand="gv_RowCommand" onrowdatabound="gv_RowDataBound"
            >
            <Columns>
                <asp:TemplateField HeaderText="序号">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="30px"></HeaderStyle>
                </asp:TemplateField>
                
                <asp:BoundField DataField="ItemName" HeaderText="活动名称"/>
                <asp:BoundField DataField="RealName" HeaderText="申请人" HeaderStyle-Width="80px">
<HeaderStyle Width="80px"></HeaderStyle>
                </asp:BoundField>
                <asp:BoundField DataField="AddDate" HeaderText="申请时间" HeaderStyle-Width="180px">

                </asp:BoundField>
                <asp:TemplateField HeaderText="状态">
                    <ItemTemplate>
                        <asp:Label ID="labState" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="70px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="审批状态">
                    <ItemTemplate>
                        <asp:Label ID="labApproveState" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="70px"></HeaderStyle>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="查看">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="Check" ID="lbCheck"
                            runat="server" Style="color: Blue;">查看</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="30px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="编辑" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' ID="lbEdit" CommandName="EditItem"
                            runat="server" Style="color: Blue;">编辑</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="30px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="操作" Visible="false">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("Id") %>' CommandName="DeleteItem" ID="lbDelete"
                            runat="server" Style="color: red;">删除</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="30px"></HeaderStyle>
                </asp:TemplateField>
            </Columns>
           
             <HeaderStyle BackColor="#EFEFEF" />
           
        </asp:GridView>
      
    </div>
    <div style="text-align: center; margin-top: 10px;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
   
    </form>
</body>
</html>
<script src="js/applicationList.js" type="text/javascript"></script>
