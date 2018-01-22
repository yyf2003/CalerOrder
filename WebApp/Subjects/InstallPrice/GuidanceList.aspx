<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GuidanceList.aspx.cs" Inherits="WebApp.Subjects.InstallPrice.GuidanceList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="/My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
     <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <link href="/layer/skin/default/layer.css" rel="stylesheet" type="text/css" />
    <script src="/layer/layer.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
   <div class="nav_title">
        <a href="/index.aspx"><img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
            class="nav_table_p">
           安装费管理—活动指引信息
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
                    活动月份
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtGuidanceMonth" runat="server" MaxLength="20" CssClass="Wdate" onclick="WdatePicker({skin:'whyGreen',dateFmt:'yyyy-MM',isShowClear:false,readOnly:true})"
                                            Style="width: 80px;"></asp:TextBox>
                </td>
            </tr>
           <tr class="tr_bai">
                <td style="width: 120px;">
                    活动名称
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:TextBox ID="txtGuidanceName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="width: 120px;">
                    <%--提交状态--%>
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <%--<asp:RadioButtonList ID="rblState" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
                       <asp:ListItem Value="0" Selected="True">全部 </asp:ListItem>
                       <asp:ListItem Value="1">已提交 </asp:ListItem>
                       <asp:ListItem Value="2">未提交</asp:ListItem>
                    </asp:RadioButtonList>--%>
                </td>
            </tr>
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnSearch_Click" OnClientClick="return check();"/>
                  <img id="loadingImg" style=" display:none;" src="../../image/WaitImg/loadingA.gif" />
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
            onrowcommand="gv_RowCommand" onrowdatabound="gv_RowDataBound" 
            >
           <Columns>
              <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="CustomerName" HeaderText="客户名称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="ItemName" HeaderText="活动名称" HeaderStyle-BorderColor="#dce0e9"/>
           
            <asp:TemplateField HeaderText="活动类型"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labActivityName" runat="server" Text=""></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="BeginDate" DataFormatString={0:yyyy-MM-dd} HeaderText="开始时间" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="EndDate" DataFormatString={0:yyyy-MM-dd} HeaderText="结束时间" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="AddUserName" HeaderText="创建人" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="AddDate" HeaderText="创建时间" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="安装店铺数量"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labInstallShopCount" runat="server" Text=""></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="100px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="已提交安装费数量"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:Label ID="labFinishCount" runat="server" Text=""></asp:Label>
                </ItemTemplate>
                <HeaderStyle Width="100px"></HeaderStyle>
            </asp:TemplateField>
           
            <asp:TemplateField HeaderText="查看"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton CommandArgument='<%#Eval("ItemId") %>' CommandName="Check" ID="lbCheck" runat="server" style="color:Blue;">查看</asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
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
     <br />
     
    </form>
</body>
</html>
<script type="text/javascript">
    function check() {
        $("#loadingImg").show();
        return true;
    }
</script>