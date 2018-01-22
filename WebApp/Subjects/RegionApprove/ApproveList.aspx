<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveList.aspx.cs" Inherits="WebApp.Subjects.RegionApprove.ApproveList" %>

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
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a>
        <p class="nav_table_p">
            待审批的活动信息
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
                    活动名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtGuidanceName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    审批状态
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:RadioButtonList ID="rblApproveState" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="0" Selected="True">待审批 </asp:ListItem>
                        <asp:ListItem Value="1">已审批 </asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td style="width: 120px;">
                </td>
                <td style="text-align: left; padding-left: 5px;">
                </td>
            </tr>
            
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="Button2" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="Button2_Click" />
                </td>
            </tr>
        </table>
    </div>
     <div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" 
        CellPadding="0" CssClass="table" BorderWidth="0" 
        HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--" 
            onrowcommand="gv_RowCommand" onrowdatabound="gv_RowDataBound" 
            >
           <Columns>
              <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="ActivityName" HeaderText="活动类型" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="ItemName" HeaderText="活动名称" HeaderStyle-BorderColor="#dce0e9"/>
            
            <asp:BoundField DataField="BeginDate" DataFormatString={0:yyyy-MM-dd} HeaderText="开始时间" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="EndDate" DataFormatString={0:yyyy-MM-dd} HeaderText="结束时间" HeaderStyle-BorderColor="#dce0e9"/>
            <%--<asp:BoundField DataField="AddUserName" HeaderText="提交人" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="AddDate" HeaderText="提交时间" HeaderStyle-BorderColor="#dce0e9"/>--%>
           <asp:TemplateField HeaderText="分区客服"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labRegionCSName" runat="server" Text=""></asp:Label>
                    
                </ItemTemplate>
                <HeaderStyle Width="70px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="待核对店数"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labNotCheckShopCount" runat="server" Text="0"></asp:Label>
                    
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="已核对店数"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labCheckedShopCount" runat="server" Text="0"></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="查看"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton ID="lbCheck" runat="server">查看</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="操作"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton ID="lbApprove" runat="server" CommandName="Approve" CommandArgument='<%#Eval("ItemId") %>'>审核</asp:LinkButton>
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
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
