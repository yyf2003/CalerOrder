<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportShops.aspx.cs" Inherits="WebApp.Customer.ImportShops" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="../easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/bootstrap/layout.css" rel="stylesheet" />
    <link href="../easyui1.4/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script type="text/javascript">
        function Finish() {
            window.parent.FinishImport();
        }
    </script>
    <style type="text/css">
       #failMsgDiv span{ float:left;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            批量导入店铺信息
        </p>
    </div>
    <div class="tr">
        >>选择文件
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                所属客户
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <%--<asp:Label ID="labCustomer" runat="server" Text=""></asp:Label>--%>
                <asp:DropDownList ID="ddlCustomer" runat="server">
                </asp:DropDownList>
                <span style="color:Red;">*</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                选择导入类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:RadioButtonList ID="rblImportType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                  <asp:ListItem Value="1">店铺信息 </asp:ListItem>
                  <asp:ListItem Value="2">POP信息 </asp:ListItem>
                  <asp:ListItem Value="3">器架信息 </asp:ListItem>
                  <asp:ListItem Value="4">特殊店铺基础安装费 </asp:ListItem>
                  <asp:ListItem Value="5">外协安装费 </asp:ListItem>
                  <asp:ListItem Value="6">更新客服 </asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                执行类型
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:CheckBox ID="cbDeleteOldPOP" runat="server" /><span style=" color:Red;">删除旧POP</span>
                &nbsp;&nbsp;
                <asp:CheckBox ID="cbDeleteOldFrame" runat="server" /><span style=" color:Red;">删除旧器架</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px;">
                类型检查
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:CheckBox ID="cbNoCheckType" runat="server" /><span style=" color:Red;">不用检查(Channel,Format,城市级别，是否安装)</span>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                模板下载
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:LinkButton ID="lbDownLoadShop" runat="server" 
                    onclick="lbDownLoadShop_Click">下载店铺模板</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lbDownLoadPOP" runat="server" onclick="lbDownLoadPOP_Click">下载POP模板</asp:LinkButton>
                 &nbsp;&nbsp;&nbsp;&nbsp;
                 <asp:LinkButton ID="lbDownLoadFrame" runat="server" onclick="lbDownLoadFrame_Click">下载器架模板</asp:LinkButton>
                 &nbsp;&nbsp;&nbsp;&nbsp;
                  <asp:LinkButton ID="lbDownLoadShopAndPOP" runat="server" onclick="lbDownLoadShopAndPOP_Click">下载店铺+POP模板</asp:LinkButton>
                 &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="lbDownLoadInstallPrice" runat="server" onclick="lbDownLoadInstallPrice_Click">下载特殊店铺基础安装费模板</asp:LinkButton>
            </td>
        </tr>
        <tr class="tr_bai">
            <td>
                选择文件
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:FileUpload ID="FileUpload1" runat="server" />
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
                   
                </div>
                <div id="showWaiting" style="color: Red; display: none;">
                    <img src='../Image/WaitImg/loadingA.gif' />正在导入，请稍等...
                </div>
            </td>
        </tr>
    </table>
    <div style=" height:300px;">
       <div id="ImportState" runat="server" style="height: 200px; margin-top:10px;text-align:left; display:none;">
        <table style=" width:100%;">
           <tr class="tr_bai">
              <td style="width: 120px;"> </td>
              <td style="text-align: left; padding-left: 5px;"> 
                  <asp:Label ID="labState" runat="server" Text="" style=" font-size:15px;color:Red;"></asp:Label>
              </td>
             
           </tr>
           <tr class="tr_bai">
              <td style="width: 120px;"> </td>
              <td style="text-align: left; padding-left: 5px;"> 
                  总数量：<asp:Label ID="labTotalNum" runat="server" Text=""></asp:Label>
              </td>
             
           </tr>
           <tr class="tr_bai">
              <td style="width: 120px;"> </td>
              <td style="text-align: left; padding-left: 5px;"> 
                  成功数量：<asp:Label ID="labSuccessNum" runat="server" Text=""></asp:Label>
              </td>
             
           </tr>
           <tr class="tr_bai">
              <td style="width: 120px;"> </td>
              <td style="text-align: left; padding-left: 5px;"> 
                  失败数量：<asp:Label ID="labFailNum" runat="server" Text=""></asp:Label>
                  <div runat="server" id="failMsgDiv" style=" display:none;"><span style="color:Red;">失败信息：</span>
                     <asp:Label ID="labFailMsg" runat="server" Text="" Visible="false"></asp:Label>
                      <asp:LinkButton ID="lbExportErrorMsg" runat="server" 
                          style="color:Blue;cursor:pointer;" onclick="lbExportErrorMsg_Click" Visible="false">导出失败信息</asp:LinkButton>
                  </div>
              </td>
             
           </tr>
        </table>
    </div>
    </div>
    
    </form>
</body>
</html>
<script type="text/javascript">
    function checkFile() {

        if ($("#ddlCustomer").val() == "0") {
            alert("请选择客户名称");
            return false;
        }
        var fileType = $("input:radio[name='rblImportType']:checked").val() || "0";
        if (fileType == "0") {
            alert("请选择导入类型");
            return false;
        }
        var val = $("#FileUpload1").val(); 
        if (val != "") { 
            var extent = val.substring(val.lastIndexOf('.') + 1); 
            if (extent != "xls" && extent != "xlsx") { 
                alert("只能上传Excel文件"); 
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
