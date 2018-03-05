<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HandleSubjectList.aspx.cs"
    Inherits="WebApp.OrderChangeManage.HandleSubjectList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                项目变更—待处理项目
            </p>
    </div>
    <div class="tr">
        >>搜索</div>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    项目编号
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:TextBox ID="txtSubjectNo" runat="server" MaxLength="20"></asp:TextBox>
                </td>
                <td style="width: 120px;">
                    项目名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="btnSearch_Click" />
                </td>
            </tr>
        </table>
    </div>
    <br />
    <div class="tr">
        >>信息列表
    </div>
    <div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
            CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--"
            OnRowCommand="gv_RowCommand" OnRowDataBound="gv_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
                
                <asp:BoundField DataField="guidance.ItemName" HeaderText="活动名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="subject.SubjectName" HeaderText="项目名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="变更类型" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labChangeType" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="application.AddDate" HeaderText="申请时间" HeaderStyle-BorderColor="#dce0e9" HeaderStyle-Width="150"/>
                <asp:TemplateField HeaderText="处理状态" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labStatus" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="操作"  HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:LinkButton CommandArgument='<%#Eval("detail.Id") %>' CommandName="Excute" ID="lbCheck"
                            runat="server">立即处理</asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="100px"></HeaderStyle>
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
    <br />
    </form>
</body>
</html>
