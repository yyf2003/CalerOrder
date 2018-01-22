<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PriceList.aspx.cs" Inherits="WebApp.Quotation.PriceList" %>

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
    <script type="text/javascript">
        var subjectId = '<%=subjectId %>';
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
            报价信息
        </p>
    </div>
    <div class="tr">
        >>项目信息</div>
    <div>
        <table class="table">
            <tr class="tr_bai">
            <td style="width: 120px;">
                内部项目编号
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectNo" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
            </td>
            <td style="text-align: left; padding-left: 5px;">
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                项目名称
            </td>
            <td style="text-align: left; padding-left: 5px; width: 300px;">
                <asp:Label ID="labSubjectName" runat="server" Text=""></asp:Label>
            </td>
            <td style="width: 120px;">
                项目负责人
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labContact" runat="server" Text=""></asp:Label>
            </td>
        </tr>
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px;  text-align: right; height: 30px;">
                    <asp:Button ID="btnSearch" runat="server" Text="查 询" class="easyui-linkbutton" 
            style="width: 65px; height:26px;display:none;" onclick="btnSearch_Click"/>
                   <asp:Button ID="btnAdd" runat="server" Text="添 加" Visible="false" OnClientClick="return false" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" />
                    &nbsp;&nbsp;
               
            <asp:Button ID="btnImport" runat="server" Text="批量导入" Visible="false" OnClientClick="return false" class="easyui-linkbutton" 
                 style="width: 65px; height:26px;" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnExport" runat="server" Text="导 出"  class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnExport_Click" />
                </td>
            </tr>
        </table>
        <br />
        <div class="tr">
        >>信息列表</div>
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
                <HeaderStyle Width="40px"></HeaderStyle>
            </asp:TemplateField>
            <%--<asp:BoundField DataField="CRNumber" HeaderText="CR号" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="AccountCheckDate" HeaderText="AD对账日期" HeaderStyle-BorderColor="#dce0e9"/>
            --%>
            <asp:BoundField DataField="Category" HeaderText="类别" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="Classification" HeaderText="分类" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="AdidasContact" HeaderText="Adidas负责人" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="Blongs" HeaderText="AD费用归属" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="Account" HeaderText="报价账户" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="OfferPrice" HeaderText="报价金额" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="OtherPrice" HeaderText="挂账金额" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="OtherPriceRemark" HeaderText="挂账说明" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="合计金额" HeaderStyle-BorderColor="#dce0e9">
               <ItemTemplate>
                   <asp:Label ID="labTotal" runat="server" Text=""></asp:Label>
               </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="TaxRate" HeaderText="税率" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:BoundField DataField="AddDate" HeaderText="添加日期" HeaderStyle-BorderColor="#dce0e9"/>
            <asp:TemplateField HeaderText="查看"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <span name="btnCheck" data-quotationid='<%#Eval("Id") %>' style=" color:Blue; cursor:pointer;">查看</span>
                </ItemTemplate>
                <HeaderStyle Width="80px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="编辑" Visible="false"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <span name="btnEdit" data-quotationid='<%#Eval("Id") %>' style=" color:Blue; cursor:pointer;">编辑</span>
                </ItemTemplate>
                <HeaderStyle Width="60px"></HeaderStyle>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="操作" Visible="false"  HeaderStyle-BorderColor="#dce0e9">
                <ItemTemplate>
                    <asp:LinkButton ID="lbDelete" CommandArgument='<%#Eval("Id") %>' CommandName="DeleteItem" runat="server" OnClientClick="return confirm('确定删除吗？')">删除</asp:LinkButton>
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
         <br />
    <div style="text-align:center;">
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
            CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
            NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
            CustomInfoTextAlign="Left" LayoutType="Table" 
            onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </div>
    <asp:HiddenField ID="hfIsFinishImport" runat="server" />

    <div style=" text-align:center;">
       <asp:Button ID="btnBack" runat="server" Text="返 回"  class="easyui-linkbutton" 
            style="width: 65px; height:26px;" onclick="btnBack_Click"  />
      <%-- <input type="button" onclick="window.history.go(-1);" value="返 回" class="easyui-linkbutton" 
            style="width: 65px; height:26px;" />--%>
    </div>

    </form>
</body>
</html>
<script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="../fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script type="text/javascript">
    var $j = jQuery.noConflict();
    $j(function () {
        $j("#btnAdd").on("click", function () {

            var url = "Edit.aspx?subjectid=" + subjectId;
            $j("#hfIsFinishImport").val("");
            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%",
                hideOnOverlayClick: false,
                afterClose: function () {
                    if ($j("#hfIsFinishImport").val() == "1") {
                        $j("#btnSearch").click();
                        $j("#hfIsFinishImport").val("");
                    }
                }
            })
        })

        //编辑
        $j("span[name='btnEdit']").on("click", function () {
            $j("#hfIsFinishImport").val("");
            var quotationid = $(this).data("quotationid")||0;
            var url = "Edit.aspx?subjectid=" + subjectId + "&quotationid=" + quotationid;
            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%",
                hideOnOverlayClick: false,
                afterClose: function () {
                    if ($j("#hfIsFinishImport").val() == "1") {
                        $j("#btnSearch").click();
                        $j("#hfIsFinishImport").val("");
                    }
                }
            })
        })

        //查看
        $j("span[name='btnCheck']").on("click", function () {
            var quotationid = $(this).data("quotationid") || 0;
            var url = "QuotationDetail.aspx?quotationid=" + quotationid;
            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%",
                hideOnOverlayClick: false
                
            })
        })

        //导入
        $j("#btnImport").on("click", function () {

            var url = "Import.aspx?subjectid=" + subjectId;
            $j("#hfIsFinishImport").val("");
            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%",
                hideOnOverlayClick: false,
               
                afterClose: function () {
                    if ($j("#hfIsFinishImport").val() == "1") {
                        $j("#btnSearch").click();
                        $j("#hfIsFinishImport").val("");
                    }
                }
            })
        })
    })
</script>