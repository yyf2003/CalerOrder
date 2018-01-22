<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmptyWarn.aspx.cs" Inherits="WebApp.Subjects.InstallPrice.EmptyWarn" %>

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
    <div>
    <table class="table">
          
          <tr class="tr_bai">
             <td colspan="2" style=" text-align:center;color:Blue;height:40px;">
               活动订单存在缺失信息，请补充后再进行归类！
             </td>
             
          </tr>
          <tr class="tr_bai">
             <td style=" width:120px;">缺失信息</td>
             <td style=" text-align:left; padding-left:5px;">
                 <asp:LinkButton ID="LinkButton1" runat="server" onclick="LinkButton1_Click">点击下载</asp:LinkButton>
             </td>
          </tr>
          <tr class="tr_bai">
             <td>更新数据</td>
             <td  style=" text-align:left; padding-left:5px;">
                 <asp:FileUpload ID="FileUpload1" runat="server" />
             </td>
          </tr>
          <tr class="tr_bai">
             <td></td>
             <td style=" text-align:left; padding-left:5px; height:30px;">
             <div id="showButton">
                 <asp:Button ID="btnUpdate" runat="server" Text="更 新"  class="easyui-linkbutton"
                    style="width: 80px; height: 26px; " onclick="btnUpdate_Click" OnClientClick="return Check()"/>
                    
              </div>
               <div id="showWaiting" style="color: Red; display: none;">
                    <img src='/Image/WaitImg/loadingA.gif' />正在导入，请稍等...
                </div>
                
             </td>
          </tr>
          <tr runat="server" id="updateResultTR" class="tr_bai" visible="false" >
             <td style=" width:120px;">更新结果</td>
             <td  style=" text-align:left; padding-left:5px;">
                 <asp:Label ID="labResult" runat="server" Text="" style="color:Red;"></asp:Label>
                 <span style=" margin-left:10px;">
                     <asp:LinkButton ID="lbDownLoadErrorMsg" runat="server" Visible="false"
                     onclick="lbDownLoadErrorMsg_Click">导出失败信息</asp:LinkButton>
                 </span>
             </td>
          </tr>
       </table>
       <div style=" margin:10px;">
         提示：缺失的信息包括：店铺规模大小、店铺级别(物料支持级别)
       </div>
    </div>
    </form>
</body>
</html>
<script type="text/javascript">
    function Check() {
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