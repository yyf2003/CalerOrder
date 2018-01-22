<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HCPOPList.aspx.cs" Inherits="WebApp.Customer.HCPOPList" %>

<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../easyui1.4/jquery.min.js"></script>
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
    <script type="text/javascript">
        function FinishImport() {
            $("#hfIsFinishImport").val("1");
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            店铺器架信息
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
                    <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" 
                        onselectedindexchanged="ddlCustomer_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td style="width: 150px;">
                    
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   
                </td>
            </tr>
           <tr class="tr_bai">
                <td>
                    店铺编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtShopNo" runat="server"></asp:TextBox>
                </td>
                <td>
                    店铺名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtShopName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    区域
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlRegion" runat="server" AutoPostBack="true" 
                        onselectedindexchanged="ddlRegion_SelectedIndexChanged">
                       <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    省份
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlProvince" runat="server" 
                        >
                       <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
           
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 10px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" 
                        onclick="btnSearch_Click"  class="easyui-linkbutton" style="width: 65px; height:26px;"/>&nbsp;&nbsp;
                        <asp:Button ID="btnAdd" runat="server" Text="添 加" OnClientClick="return false" Visible="false" class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" />&nbsp;&nbsp;
                    <asp:Button ID="btnImport" runat="server" Text="批量导入" 
                        OnClientClick="return false" Visible="false"  class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;"  />
                        &nbsp;&nbsp;
                    <asp:Button ID="btnExportShop" runat="server" Text="导 出"  class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" onclick="btnExportShop_Click" />
                       
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
            CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--">
            <Columns>
                <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
               
            <asp:BoundField DataField="ShopNo" HeaderText="店铺编码" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="RegionName" HeaderText="区域" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="ProvinceName" HeaderText="省份" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="CityName" HeaderText="城市" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="MachineFrame" HeaderText="器架名称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="POP" HeaderText="POP名称" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="POPGender" HeaderText="性别" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="Count" HeaderText="数量" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="GraphicWidth" HeaderText="POP宽" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="GraphicLength" HeaderText="POP高" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="GraphicMaterial" HeaderText="材质" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="编辑"  HeaderStyle-BorderColor="#dce0e9" HeaderStyle-Width="60px">
                <ItemTemplate>
                    
                    <span data-id='<%#Eval("Id") %>'  style="color: blue;
                        cursor: pointer;">编辑</span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="操作" HeaderStyle-BorderColor="#dce0e9" HeaderStyle-Width="60px">
               <ItemTemplate>
                   <asp:LinkButton ID="lbDelete" runat="server" CommandArgument='<%=Id %>' CommandName="deleteItem">删除</asp:LinkButton>
               </ItemTemplate>
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
            CustomInfoTextAlign="Left" LayoutType="Table" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>





    <asp:HiddenField ID="hfIsFinishImport" runat="server" />
    </form>
</body>
</html>

<script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<script src="../fancyBox/source/jquery.fancybox.js" type="text/javascript"></script>
<script src="../fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script type="text/javascript">
    //解决jquery库的冲突，很重要！
    var $j = jQuery.noConflict();

    $j(function () {
        $j("#btnImport").on("click", function () {
            var customerId = $j("#ddlCustomer").val() || "0";
            if (customerId == "0") {
                alert("请选择客户");
                return false;

            }
            var url = "ImportHCPOP.aspx?customerId=" + customerId;
            $j("#hfIsFinishImport").val("");
            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%",
                afterClose: function () {
                    if ($j("#hfIsFinishImport").val() == "1") {
                        $j("#btnSearch").click();
                    }
                }
            })
        })
    })
</script>