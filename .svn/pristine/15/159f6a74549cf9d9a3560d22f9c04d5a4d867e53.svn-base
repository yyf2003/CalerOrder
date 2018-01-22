<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MaterialDetail.aspx.cs" Inherits="WebApp.Subjects.Material.MaterialDetail" %>

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
</head>
<body>
    <form id="form1" runat="server">
    <div class="tr">
        >>搜索
    </div>
    <div>
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 150px;">
                    店铺编号
                </td>
                <td style="width: 200px; text-align: left; padding-left: 5px;">
                  <asp:TextBox ID="txtShopNo" runat="server"></asp:TextBox>
                </td>
                <td style="width: 150px;">
                   店铺名称
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   <asp:TextBox ID="txtShopName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td>
                    物料名称
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                     <asp:CheckBoxList ID="cblMaterialName" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:CheckBoxList>
                </td>
               
            </tr>
           
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" OnClick="btnSearch_Click" class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" />&nbsp;&nbsp;
                    
                    <asp:Button ID="btnExport" runat="server" Text="导 出" class="easyui-linkbutton"
                        Style="width: 65px; height: 26px;" onclick="btnExport_Click" />
                   &nbsp;&nbsp;
                   <asp:Button ID="btnDelete" runat="server" Text="删 除" class="easyui-linkbutton" OnClientClick="return confiemCheck()"
                        Style="width: 65px; height: 26px;" onclick="btnDelete_Click" />
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
                <asp:TemplateField HeaderStyle-Width="40px" HeaderStyle-BorderColor="#dce0e9">
                   <HeaderTemplate>
                      <asp:CheckBox ID="cbAll" name="cbAll" runat="server"/>
                   </HeaderTemplate>
                   <ItemTemplate>
                       <asp:CheckBox ID="cbOne" name="cbOne" runat="server" />
                       <asp:HiddenField ID="hfMaterialId" runat="server" Value='<%#Eval("MaterialId") %>'/>
                   </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
               
                <asp:BoundField DataField="ShopNo" HeaderText="店铺编码" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="ShopName" HeaderText="店铺名称" HeaderStyle-BorderColor="#dce0e9" />
                
                <asp:BoundField DataField="MaterialName" HeaderText="物料名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="MaterialCount" HeaderText="数量" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="Price" HeaderText="销售价格" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="编辑" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="editShop" data-shopid='<%#Eval("MaterialId") %>' style="color: Blue; cursor: pointer;">
                            编辑</span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="操作" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        删除
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
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="10" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" OnPageChanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    <p></p>
    <p></p>
    </form>
</body>
</html>
<script type="text/javascript">
    $(function () {

        $("input[name$='cbAll']").on("click", function () {

            var checked = this.checked;
            $("input[name$='cbOne']").each(function () {
                this.checked = checked;
            })
        })
    })

    function confiemCheck() {

        var len = $("input[name$='cbOne']:checked").length;
        if (len == 0) {
            alert("请选择要删除的记录");
            return false;
        }
        return confirm("确定删除吗？");
    }
</script>
