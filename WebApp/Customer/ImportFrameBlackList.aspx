<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportFrameBlackList.aspx.cs" Inherits="WebApp.Customer.ImportFrameBlackList" %>

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
</head>
<body>
    <form id="form1" runat="server">
    
    <div class="tr">
        >>选择文件
    </div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px;">
                模板下载
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:LinkButton ID="lbDownLoadTemplate" runat="server">下载模板</asp:LinkButton>
               
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
                    
                   
                </div>
                <div id="showWaiting" style="color: Red; display: none;">
                    <img src='../Image/WaitImg/loadingA.gif' />正在导入，请稍等...
                </div>
            </td>
        </tr>
        </table>
         <div style=" height:200px;">
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