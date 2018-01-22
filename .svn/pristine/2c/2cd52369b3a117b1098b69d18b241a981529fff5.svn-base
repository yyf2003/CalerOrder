<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List1.aspx.cs" Inherits="WebApp.Quotation.List1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%--<link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    --%>
    <link href="../CSS/StyleSheet1.css" rel="stylesheet" type="text/css" />
    <script src="../jqGrid/js/jquery-1.11.0.min.js" type="text/javascript"></script>
    <script src="../jqGrid/js/jquery.jqGrid.min.js" type="text/javascript"></script>
    <script src="../My97DatePicker/WdatePicker.js" type="text/javascript"></script>
    <link href="/easyui1.4/themes/icon.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="/easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../jqGrid/css/ui.jqgrid-bootstrap.css" rel="stylesheet" type="text/css" />
    <script src="../bootstrap-3.3.2-dist/js/bootstrap.min.js" type="text/javascript"></script>
    <link href="../bootstrap-3.3.2-dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script src="../jqGrid/js/i18n/grid.locale-cn.js" type="text/javascript"></script>
    <script src="/easyui1.4/jquery.easyui.min.js"></script>
    <script type="text/javascript">
        $.jgrid.defaults.width = "100%";
        $.jgrid.defaults.styleUI = 'Bootstrap';
        function FinishImport() {
            $("#hfIsFinishImport").val("1");
        }
    </script>
    <script src="js/list1.js" type="text/javascript"></script>
    <style type="text/css">
        .captiontitle
        {
            color: White;
            font-weight: bold;
        }
        .table11
        {
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            项目报价信息
        </p>
    </div>
    <div class="tr1">
        >>搜索</div>
    <div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
           <ContentTemplate>
           
           
        <table class="table1">
            <tr class="tr_bai">
                <td style="width: 120px;">
                    客户
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:DropDownList ID="ddlCustomer" runat="server"  AutoPostBack="true"
                        onselectedindexchanged="ddlCustomer_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td style="width: 120px;">
                   
                </td>
                <td style="text-align: left; padding-left: 5px;">
                </td>
            </tr>
            <tr class="tr_bai">
                        <td style="width: 120px;">
                            活动名称
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <asp:DropDownList ID="ddlGuidance" runat="server">
                             <asp:ListItem Value="-1">--请选择--</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            项目时间
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <asp:TextBox ID="txtBeginDate" runat="server" onclick="WdatePicker()" style=" height:21px;"></asp:TextBox>
                            —
                            <asp:TextBox ID="txtEndDate" runat="server" onclick="WdatePicker()" style=" height:21px;"></asp:TextBox>
                           
                        </td>
                    </tr>
                    <%--<tr class="tr_bai">
                        <td class="style1">
                            区域
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;" class="style2">
                           
                            <asp:CheckBoxList ID="cblRegion" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="5" 
                                onselectedindexchanged="cblRegion_SelectedIndexChanged" AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            省份
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadProvince" style=" display:none;">
                               <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblProvince" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8" 
                                onselectedindexchanged="cblProvince_SelectedIndexChanged" AutoPostBack="true">
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr class="tr_bai">
                        <td style="width: 120px;">
                            城市
                        </td>
                        <td colspan="3" style="text-align: left; padding-left: 5px;">
                            <div id="loadCity" style=" display:none;">
                               <img src="../image/WaitImg/loadingA.gif" />
                            </div>
                            <asp:CheckBoxList ID="cblCity" runat="server" CssClass="cbl" CellSpacing="20"
                                RepeatDirection="Horizontal" RepeatLayout="Flow" RepeatColumns="8" >
                            </asp:CheckBoxList>
                        </td>
                    </tr>--%>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    项目名称
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:TextBox ID="txtSubjectName" runat="server" MaxLength="50" style=" height:21px;"></asp:TextBox>
                </td>
                <td style="width: 120px;">
                    项目编号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtSubjectNo" runat="server" MaxLength="50" style=" height:21px;"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    CR号
                </td>
                <td style="text-align: left; padding-left: 5px; width: 250px;">
                    <asp:TextBox ID="txtCRNumber" runat="server" MaxLength="50" style=" height:21px;"></asp:TextBox>
                </td>
                <td style="width: 120px;">
                    PO号
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtPONumber" runat="server" MaxLength="50" style=" height:21px;"></asp:TextBox>
                </td>
            </tr>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    是否提交报价
                </td>
                <td style="text-align: left; padding-left: 5px;width: 250px;">
                    <asp:RadioButtonList ID="rblSubmitPrice" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="0" Selected="True">全部 </asp:ListItem>
                        <asp:ListItem Value="1">已提交 </asp:ListItem>
                        <asp:ListItem Value="2">未提交 </asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td style="width: 120px;">
                    是否已开票
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <asp:RadioButtonList ID="rblIsInvoice" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="0" Selected="True">全部 </asp:ListItem>
                        <asp:ListItem Value="1">已开票 </asp:ListItem>
                        <asp:ListItem Value="2">未开票 </asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <%--<tr class="tr_bai">
                <td style="width: 120px;">
                    项目时间
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:TextBox ID="txtBeginDate" runat="server" onfocus="WdatePicker()" style=" width:120px; height:21px;"></asp:TextBox>
                    —
                    <asp:TextBox ID="txtEndDate" runat="server" onfocus="WdatePicker()" style=" width:120px;height:21px;"></asp:TextBox>
                </td>
            </tr>--%>
            <tr class="tr_bai">
                <td style="width: 120px;">
                    AD负责人
                </td>
                <td colspan="3" style="text-align: left; padding-left: 5px;">
                    <asp:CheckBoxList ID="cbADContact" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    </asp:CheckBoxList>
                </td>
            </tr>
            
        </table>
        </ContentTemplate>
        </asp:UpdatePanel>
        <div style="text-align: right; padding-right: 25px; margin-top: 8px;">
            <input type="button" id="btnSearch" value="查 询" class="easyui-linkbutton"  Style="width: 65px;
                        height: 26px;"/>
                <img id="loadingImg" style=" display:none;" src="../image/WaitImg/loadingA.gif" />
                &nbsp;&nbsp;
             <asp:Button ID="btnExport" runat="server" Text="导 出" class="easyui-linkbutton" Style="width: 65px;
                        height: 26px;" OnClick="btnExport_Click" />
             <img id="exportWaiting" style=" display:none;" src="../image/WaitImg/loadingA.gif" />
        </div>
    </div>
    <br />
    <div style="padding-top: 0px; width: 98%;">
        <table id="jqGrid" class="table11">
        </table>
        <div id="jqGridPager">
        </div>
        <br />
    </div>
    <asp:HiddenField ID="hfIsFinishImport" runat="server" />
    <%--<div id="divUpload" style=" display:none;">
       <iframe id="iframeUpload" name="iframeUpload" width="100%" frameborder="0" scrolling="auto"></iframe>
    </div>--%>
    </form>
</body>
</html>
<script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
<link href="../fancyBox/source/jquery.fancybox.css" rel="stylesheet" type="text/css" />
<script src="../fancyBox/source/jquery.fancybox.pack.js" type="text/javascript"></script>

<script type="text/javascript">
    var $j = jQuery.noConflict();

    function upload(subjctid) {
        var url = "UploadFiles.aspx?subjectid=" + subjctid;

        $j.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: "90%",
            hideOnOverlayClick: false,
            afterClose: function () {
                Reload();
            }
        })
    }

    function CheckFile(subjctid) {
        var url = "CheckFiles.aspx?subjectid=" + subjctid;

        $j.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: "90%",
            hideOnOverlayClick: false

        })
    }

    

    function EditSupplement(subjctid) {
        var url = "EditSupplement.aspx?subjectid=" + subjctid;

        $j.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: "80%",
            hideOnOverlayClick: false,
            afterClose: function () {
                if ($j("#hfIsFinishImport").val() == "1") {
                    Reload();
                    $j("#hfIsFinishImport").val("");
                }
            }
        })
    }

    function EditInvoice(subjctid) {
        var url = "Invoice.aspx?subjectid=" + subjctid;

        $j.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: "80%",
            hideOnOverlayClick: false,
            afterClose: function () {
                if ($j("#hfIsFinishImport").val() == "1") {
                    Reload();
                    $j("#hfIsFinishImport").val("");
                }
            }
        })
    }

    function CheckMaterialPrice(subjctid) {
        var url = "OrderPrice.aspx?subjectid=" + subjctid;

        $j.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: 700,
            //modal:true,
            hideOnOverlayClick: false
           
            
        })
    }
</script>

<script type="text/javascript">
   
</script>