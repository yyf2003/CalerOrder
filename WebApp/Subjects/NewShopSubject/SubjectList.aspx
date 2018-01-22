<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubjectList.aspx.cs" Inherits="WebApp.Subjects.NewShopSubject.SubjectList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
     <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
           分区补单
        </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    客户
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:DropDownList ID="ddlCustomer" runat="server">
                    </asp:DropDownList>
                </td>
                <td style="width: 120px;">
                    区域
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:CheckBoxList ID="cblRegion" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    项目名称
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="width: 120px;">
                    项目编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtSubjectNo" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnSearch_Click"/>
                    <asp:Button ID="btnAdd" runat="server" Text="添 加" Visible="false" class="easyui-linkbutton" 
            style="width: 65px; height:26px; margin-left:20px;" onclick="btnAdd_Click"/>
                </td>
            </tr>
        </table>
    </div>
    <div class="tr" style=" margin-top:10px;">
        >>信息列表
    </div>
    <div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
            CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--"
            OnRowDataBound="gv_RowDataBound" OnRowCommand="gv_RowCommand">
            <Columns>
                <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="CustomerName" HeaderText="客户名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="subject.SubjectName" HeaderText="项目名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="subject.SupplementRegion" HeaderText="区域" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="RealName" HeaderText="创建人" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="subject.AddDate" HeaderText="创建时间" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="项目状态" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labStatus" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="审批状态" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labApprove" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="编辑" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("subject.Id") %>' ID="lbEdit" CommandName="EditItem"
                            runat="server" Style="color: Blue;">编辑</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="70px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="查看" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("subject.Id") %>' CommandName="Check" ID="lbCheck"
                            runat="server" Style="color: Blue;">查看</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="70px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="操作" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("subject.Id") %>' ID="lbDelete" CommandName="DeleteItem" 
                            runat="server" Style="color: red;">删除</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="70px"></HeaderStyle>
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
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
