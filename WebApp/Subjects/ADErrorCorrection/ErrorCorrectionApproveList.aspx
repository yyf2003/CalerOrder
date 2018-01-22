﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorCorrectionApproveList.aspx.cs" Inherits="WebApp.Subjects.ADErrorCorrection.ErrorCorrectionApproveList" %>

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
            订单纠错审批
        </p>
    </div>
    <br />
    <div class="tr">
        >>信息列表
    </div>
    <div>
        <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" 
        CellPadding="0" CssClass="table" BorderWidth="0" 
        HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--" 
            >
           <Columns>
              <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="ItemName" HeaderText="活动名称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="shopCount" HeaderText="纠错店铺数量" HeaderStyle-BorderColor="#dce0e9" HeaderStyle-Width="120"/>
            <asp:TemplateField  HeaderText="审批状态"  HeaderStyle-BorderColor="#dce0e9">
               <ItemTemplate>
                   <asp:Label ID="Label1" runat="server" Text="待审批"></asp:Label>
               </ItemTemplate>
               <HeaderStyle Width="90px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="查看"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                   <%--<asp:LinkButton ID="lbCheck" runat="server" CommandName="Check" CommandArgument='<%#Eval("GuidanceId") %>'>查看</asp:LinkButton>--%>
                   <a href="ErrorCorrectionApprove.aspx?guidanceId=<%#Eval("GuidanceId") %>">查看</a>
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
