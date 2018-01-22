<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckData.aspx.cs" Inherits="WebApp.Subjects.CheckDataBase.CheckData" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <link href="/easyui1.4/themes/bootstrap/easyui.css" rel="stylesheet" />
    <script src="/easyui1.4/jquery.min.js" type="text/javascript"></script>
    <link href="/bootstrap-3.3.2-dist/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script src="/bootstrap-3.3.2-dist/js/bootstrap.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /><p
            class="nav_table_p">
            数据检查
        </p>
    </div>
    <div class="tr">
        >>导入</div>
    <table class="table">
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                选择文件：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                
                <asp:FileUpload ID="FileUpload1" runat="server" />
               
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                模板：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:LinkButton ID="lbDownLoad" runat="server" OnClick="lbDownLoad_Click">下载模板</asp:LinkButton>
            </td>
        </tr>
         <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
                检查类型：
            </td>
            <td style="text-align: left; padding-left: 5px;">
                <asp:RadioButtonList ID="rblType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                   <asp:ListItem Value="1">店铺信息&nbsp;&nbsp;</asp:ListItem>
                   <asp:ListItem Value="2">POP信息&nbsp;&nbsp;</asp:ListItem>
                   <asp:ListItem Value="3">器架信息&nbsp;&nbsp;</asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr class="tr_bai">
            <td style="width: 120px; text-align: right;">
            </td>
            <td style="text-align: left; padding-left: 5px; height: 50px;">
                <div id="showButton">
                    <asp:Button ID="btnImport" runat="server" Text="导 入" OnClientClick="return checkFile()"
                        class="easyui-linkbutton" Style="width: 65px; height: 26px;" OnClick="btnImport_Click" />
                </div>
                <div id="showWaiting" style="color: Red; display: none;">
                    <img src='../../Image/WaitImg/loadingA.gif' />正在导入，请稍等...
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
                  <asp:Label ID="labResult" runat="server" Text="" style=" font-size:15px;color:Red;"></asp:Label>
                  <div runat="server" id="failMsgDiv" style=" display:none;">
                      
                      <asp:LinkButton ID="lbExportErrorMsg" runat="server" 
                          style="color:Blue;cursor:pointer;" onclick="lbExportErrorMsg_Click" Visible="false">导出</asp:LinkButton>
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
        var fileType = $("input:radio[name='rblType']:checked").val() || "0";
        if (fileType == "0") {
            alert("请选择检查类型");
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
        //return true;
    }
</script>