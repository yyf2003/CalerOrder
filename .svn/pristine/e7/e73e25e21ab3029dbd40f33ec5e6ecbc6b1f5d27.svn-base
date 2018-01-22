<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetPrice.aspx.cs" Inherits="WebApp.Materials.SetPrice" %>

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
    <script src="../Scripts/common.js" type="text/javascript"></script>
   
    <script src="../easyui1.4/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery.pagination.js" type="text/javascript"></script>
    <link href="../CSS/pagination.css" rel="stylesheet" type="text/css" />
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
            材料报价
        </p>
    </div>
    <div class="tr">
        >>搜索
    </div>
    <div>
        <asp:Panel ID="Panel1" runat="server">
            <table id="searchTab" class="table">
            <tr class="tr_bai">
                <td style="width: 150px;">
                    选择客户
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" 
                        onselectedindexchanged="ddlCustomer_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td style="width: 150px;">
                    区域
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlRegion" runat="server">
                       <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
           <%-- <tr class="tr_bai">
                <td style="width: 150px;">
                    大类
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                    <asp:DropDownList ID="ddlBigType" runat="server" AutoPostBack="true" 
                        onselectedindexchanged="ddlBigType_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td style="width: 150px;">
                    小类
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    
                    <asp:DropDownList ID="ddlSmallType" runat="server">
                      <asp:ListItem Value="0">--请选择--</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>--%>
           <tr class="tr_bai">
                <td style="width: 150px;">
                    客户材质名称
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                     <asp:TextBox ID="txtMaterialName" runat="server" MaxLength="50"></asp:TextBox>
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
                    <asp:Button ID="btnAdd" runat="server" Text="添 加" OnClientClick="return false" Visible="false"  class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" />
                        &nbsp;&nbsp;
                    <asp:Button ID="btnImport" runat="server" Text="批量导入" OnClientClick="return false" Visible="false"  class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" />
                </td>
            </tr>
        </table>
        </asp:Panel>
     
    </div>
     <br />
    <div class="tr">
        >>信息列表
    </div>
    <asp:GridView ID="gv" runat="server" AutoGenerateColumns="false" CellPadding="0"
        CssClass="table" BorderWidth="0" HeaderStyle-BorderStyle="None" EmptyDataText="--无符合条件的信息--"
        OnRowCommand="gv_RowCommand" 
        onrowdatabound="gv_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                </ItemTemplate>
                <HeaderStyle Width="50px"></HeaderStyle>
            </asp:TemplateField>
            <asp:BoundField DataField="CustomerName" HeaderText="客户名称" HeaderStyle-BorderColor="#dce0e9" />
            <asp:BoundField DataField="RegionName" HeaderText="区域" HeaderStyle-BorderColor="#dce0e9" />
            <%--<asp:BoundField DataField="BigTypeName" HeaderText="所属大类" HeaderStyle-BorderColor="#dce0e9" />--%>
            <%--<asp:BoundField DataField="SmallTypeName" HeaderText="所属小类" HeaderStyle-BorderColor="#dce0e9" />--%>
            <asp:BoundField DataField="CustomerMaterialName" HeaderText="客户材质名称" HeaderStyle-BorderColor="#dce0e9" />
            
            <asp:BoundField DataField="CostPrice" HeaderText="成本价" HeaderStyle-BorderColor="#dce0e9" />
            <asp:BoundField DataField="SalePrice" HeaderText="销售价" HeaderStyle-BorderColor="#dce0e9" />
            <asp:TemplateField HeaderText="状态" HeaderStyle-BorderColor="#dce0e9" HeaderStyle-Width="60px">
                <ItemTemplate>
                   <%#Eval("IsDelete") != null && bool.Parse(Eval("IsDelete").ToString())?"<span style='color:red;'>已删除</span>":"正常"%>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="编辑" Visible="false" HeaderStyle-BorderColor="#dce0e9" HeaderStyle-Width="60px">
                <ItemTemplate>
                    
                    <span data-priceid='<%#Eval("Id") %>' onclick="editPrice(this)" style="color: blue;
                        cursor: pointer;">编辑</span>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="操作" Visible="false" HeaderStyle-BorderColor="#dce0e9" HeaderStyle-Width="60px">
                <ItemTemplate>
                    <asp:LinkButton ID="lbDelete" CommandArgument='<%#Eval("Id") %>' CommandName="deleteItem"
                        runat="server" OnClientClick="return confirm('确定删除吗？')" Style="color: Blue;">删除</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <AlternatingRowStyle CssClass="tr_bai" />
        <HeaderStyle CssClass="tr_hui" />
        <RowStyle CssClass="tr_bai" />
        <SelectedRowStyle CssClass="tr_hui" />
        <EmptyDataRowStyle CssClass="tr_bai" />
    </asp:GridView>
    <br />
    <div style="text-align: center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="15" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>



    <div id="editPriceDiv" title="添加报价" style="display: none;">
        <table style="width: 780px; text-align: center;">
            <tr>
                <td style="width: 100px; height: 25px;">
                    客户
                </td>
                <td style="text-align: left;width: 180px; padding-left: 5px;">
                    <select id="selCustomer">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
                <td style="width: 100px;">
                    区域
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selRegion">
                        <option value="0">--请选择--</option>
                    </select>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr>
                <td style="width: 100px; height: 25px;">
                    客户材质名称
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtCustomerMaterial" runat="server" MaxLength="50" style=" width:200px;"></asp:TextBox>
                   <span style="color: Red;">*</span>
                </td>
                
            </tr>
            <tr>
                <td style="width: 100px; height: 25px;">
                    成本价
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtCostPrice" runat="server" MaxLength="10"></asp:TextBox>
                  
                </td>
                <td style="width: 100px;">
                    销售价
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtSalePrice" runat="server" MaxLength="10"></asp:TextBox>
                    <span style="color: Red;">*</span>
                </td>
            </tr>
            <tr class="tr">
              <td colspan="4" style=" text-align:left;">>>材料</td>
            </tr>
            <%--<tr>
                <td style="width: 100px; height: 22px;">
                    所属大类
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selBigType">
                        <option value="0">--请选择--</option>
                    </select>
                   
                </td>
                <td style="width: 100px;">
                    所属小类
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <select id="selSmallType">
                        <option value="0">--请选择--</option>
                    </select>
                    
                </td>
            </tr>--%>
           <tr>
             <td style="  min-height:30px;">
               材料信息：
               
             </td>
              <td colspan="3" style=" text-align:left; padding-left:5px;">
                 <div id="materialContentx"></div>
             </td>
           </tr>
           <tr>
             <td colspan="4">
                <div id="DataTable">
                <div id="divload" style=" display:none;">
                    <img src="../image/WaitImg/loading1.gif" />
                </div>
                  <table id="materialData" class="table" style=" display:none;">
                    <thead>
                    <tr class="tr_hui">
                      
                       <%--<td style=" width:30px;">
                         <input type="checkbox" id="cbAll" />
                       </td>
                       <td style=" width:30px;">序号</td>
                       <td>材料名称</td>
                       <td>大类</td>
                       <td>小类</td>
                       <td>品牌</td>
                       <td>长</td>
                       <td>宽</td>
                       <td>面积</td>
                       <td>单位</td>--%>
                       
                     </tr>
                    </thead>
                     <tbody id="materialBody">
                       
                     </tbody>
                  </table>
                </div>
                <br />
                <div id="Pagination" class="sabrosus"  style=" display:none;">
                
                </div>
             </td>
           </tr>
        </table>
    </div>
    <asp:HiddenField ID="hfIsFinishImport" runat="server" />
    </form>
</body>
</html>

<script src="js/SetPrice.js" type="text/javascript"></script>
<script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="../fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script type="text/javascript">
    //解决jquery库的冲突，很重要！
    var $j = jQuery.noConflict();
    //批量导入
    $j(function () {
        $j("#btnImport").on("click", function () {
            
            var url = "ImportCustomerMaterial.aspx";
            $j("#hfIsFinishImport").val("");
            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%",
                hideOnOverlayClick: false,
                //modal: true,
                //showCloseButton:true,
                afterClose: function () {
                    if ($j("#hfIsFinishImport").val() == "1") {
                        $j("#btnSearch").click();
                    }
                }
            })
        })
    })
</script>

