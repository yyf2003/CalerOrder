<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Quotation.List" %>

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
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
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
            项目报价信息
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
                    
                </td>
                <td style="text-align: left; padding-left: 5px;">
                   
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
                <td style="width: 120px;">
                    CR号
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:TextBox ID="txtCRNumber" runat="server" MaxLength="50"></asp:TextBox>
                </td>
                <td style="width: 120px;">
                    PO号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPONumber" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    是否提交报价
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:RadioButtonList ID="rblSubmitPrice" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="0" Selected="True">全部 </asp:ListItem>
                        <asp:ListItem Value="1">已提交 </asp:ListItem>
                        <asp:ListItem Value="2">未提交 </asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    AD负责人
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:CheckBoxList ID="cbADContact" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:CheckBoxList>
                </td>
            </tr>
            <tr class="tr_bai">
                <td colspan="4" style="padding-right: 20px; text-align: right; height: 30px;">
                    <asp:Button ID="Button1" runat="server" Text="查 询" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="Button1_Click" />
                    &nbsp;&nbsp;
                    <asp:Button ID="btnExport" runat="server" Text="导 出" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="btnExport_Click" />
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
            OnRowDataBound="gv_RowDataBound" ShowFooter="true">
            <Columns>
                <asp:TemplateField HeaderText="序号" HeaderStyle-Width="50px" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.DataItemIndex + 1%>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="CustomerName" HeaderText="所属客户" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="SubjectNo" HeaderText="项目编号" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="SubjectName" HeaderText="项目名称" HeaderStyle-BorderColor="#dce0e9" />
                <asp:BoundField DataField="Contact" HeaderText="项目负责人" HeaderStyle-BorderColor="#dce0e9" />
                <asp:TemplateField HeaderText="AD负责人" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labADContact" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px"></HeaderStyle>
                </asp:TemplateField>
                <asp:BoundField DataField="CRNumber" HeaderText="CR号" HeaderStyle-BorderColor="#dce0e9" />
                <%--<asp:BoundField DataField="AddDate" HeaderText="创建时间" HeaderStyle-BorderColor="#dce0e9"/>--%>
                <asp:TemplateField HeaderText="PO号" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <%#Eval("PONumber")%>
                    </ItemTemplate>
                    <FooterTemplate>
                        合计：
                    </FooterTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="汇总金额" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labTotalPrice" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="labTotal" runat="server" Text=""></asp:Label>
                    </FooterTemplate>
                    <HeaderStyle Width="80px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="文件数" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <asp:Label ID="labFileNum" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="50px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="查看报价单" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="btnCheckFile" data-subjectid='<%#Eval("Id") %>' style="color: Blue; cursor: pointer;">
                            查看</span>
                    </ItemTemplate>
                    <HeaderStyle Width="70px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="上传报价单" Visible="false" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="btnUpload" data-subjectid='<%#Eval("Id") %>' style="color: Blue; cursor: pointer;">
                            上传</span>
                    </ItemTemplate>
                    <HeaderStyle Width="70px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="报价明细" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <a href="PriceList.aspx?subjectid=<%#Eval("Id") %>">查看</a>
                    </ItemTemplate>
                    <HeaderStyle Width="60px"></HeaderStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="对账补充" HeaderStyle-BorderColor="#dce0e9">
                    <ItemTemplate>
                        <span name="btnSupplement" data-subjectid='<%#Eval("Id") %>' style="color: Blue;
                            cursor: pointer;">编辑</span>
                    </ItemTemplate>
                    <HeaderStyle Width="60px"></HeaderStyle>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle CssClass="tr_bai" />
            <HeaderStyle CssClass="tr_hui" />
            <RowStyle CssClass="tr_bai" />
            <FooterStyle CssClass="tr_bai" />
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
    <asp:HiddenField ID="hfIsFinishImport" runat="server" />
    </form>
</body>
</html>
<script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="../fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>
<script type="text/javascript">
    var $j = jQuery.noConflict();
    //上传
    $j(function () {
        $j("span[name='btnUpload']").on("click", function () {
            var subjctid = $(this).data("subjectid");
            var url = "UploadFiles.aspx?subjectid=" + subjctid;
            //$j("#hfIsFinishImport").val("");
            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%",
                hideOnOverlayClick: false

            })
        })

        $j("span[name='btnCheckFile']").on("click", function () {
            var subjctid = $(this).data("subjectid");
            var url = "CheckFiles.aspx?subjectid=" + subjctid;

            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "90%",
                hideOnOverlayClick: false

            })
        })


        $j("span[name='btnSupplement']").on("click", function () {
            var subjctid = $(this).data("subjectid");
            var url = "EditSupplement.aspx?subjectid=" + subjctid;

            $j.fancybox({
                href: url,
                type: 'iframe',
                padding: 5,
                width: "80%",
                hideOnOverlayClick: false,
                afterClose: function () {
                    if ($j("#hfIsFinishImport").val() == "1") {
                        $j("#Button1").click();
                        $j("#hfIsFinishImport").val("");
                    }
                }
            })
        })
    })
</script>
