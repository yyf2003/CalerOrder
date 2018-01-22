<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="WebApp.Subjects.ShopMaterialSupportManage.List" %>

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
<body style=" min-height:500px;">
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            导入店铺物料支持级别
        </p>
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                活动名称
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:Label ID="labGuidanceName" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                模板下载
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:LinkButton ID="lbDownLoadShop" runat="server" 
                    onclick="lbDownLoadShop_Click">下载店铺模板</asp:LinkButton>
               
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                选择文件
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:FileUpload ID="FileUpload1" runat="server" />
                <span style=" margin-left :30px;">
                    <asp:CheckBox ID="cbAdd" runat="server" />追加（保留之前导入的数据）
                </span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
            </td>
            <td style="text-align: left; padding-left: 5px; height: 35px;">
                <div id="showButton">
                    <asp:Button ID="btnImport" runat="server" Text="导 入" class="easyui-linkbutton" OnClientClick="return checkFile()"
                        Style="width: 65px; height: 26px;" OnClick="btnImport_Click" />
                    (<span style="color:Blue;">提示：要导入的Excel表格的Sheet名称必须为“Sheet1”</span>)
                    <asp:Label ID="labImportState" runat="server" Text="" ForeColor="Red"></asp:Label>
                    <asp:LinkButton ID="lbExportErrorMsg" runat="server" Visible="false" ForeColor="blue"
                        onclick="lbExportErrorMsg_Click">导出失败信息</asp:LinkButton>
                </div>
                <div id="showWaiting" style="color: Red; display: none;">
                    <img src='../../Image/WaitImg/loadingA.gif' />正在导入，请稍等...
                </div>

            </td>
        </tr>
    </table>

    <div style=" margin-top :20px;">
        <asp:Repeater ID="gv" runat="server">
           <HeaderTemplate>
              <table class="table">
                 <tr class="tr_hui">
                    <td style="width:40px;">序号</td>
                    <td>店铺编号</td>
                    <td>店铺名称</td>
                    <td>区域</td>
                    <td>省份</td>
                    <td>城市</td>
                    <td>物料支持级别</td>
                 </tr>
              
           </HeaderTemplate>
           <ItemTemplate>
              <tr class="tr_bai">
                    <td style="width:40px;">
                       <%# (AspNetPager1.CurrentPageIndex - 1) * AspNetPager1.PageSize + Container.ItemIndex + 1%>
                    </td>
                    <td><%#Eval("shop.ShopNo")%></td>
                    <td><%#Eval("shop.ShopName")%></td>
                    <td><%#Eval("shop.RegionName")%></td>
                    <td><%#Eval("shop.ProvinceName")%></td>
                    <td><%#Eval("shop.CityName")%></td>
                    <td><%#Eval("MaterialSupport")%></td>
                 </tr>
           </ItemTemplate>
           <FooterTemplate>
              <%if(gv.Items.Count==0){ %>
                <tr class="tr_bai">
                   <td colspan="7" style=" text-align:center;">
                      --暂无数据--
                   </td>
                </tr>
              <%} %>
           </table>
           </FooterTemplate>
        </asp:Repeater>
    </div>
    <div>
        <webdiyer:AspNetPager ID="AspNetPager1" runat="server" PageSize="20" CssClass="paginator"
                CurrentPageButtonClass="cpb" AlwaysShow="True" FirstPageText="首页" LastPageText="尾页"
                NextPageText="下一页" PrevPageText="上一页" ShowCustomInfoSection="Left" ShowInputBox="Never"
                CustomInfoTextAlign="Left" LayoutType="Table" onpagechanged="AspNetPager1_PageChanged">
        </webdiyer:AspNetPager>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function checkFile() {

        var val = $("#FileUpload1").val();
        if (val != "") {
            var extent = val.substring(val.lastIndexOf('.') + 1);
            if (extent != "xls" && extent != "xlsx") {
                alert("只能导入Excel文件");
                return false;
            }
        }
        else {
            alert("请选择文件");
            return false;
        }
        $("#showButton").css({ display: "none" });
        $("#showWaiting").css({ display: "" });
        return true;
    }
</script>
