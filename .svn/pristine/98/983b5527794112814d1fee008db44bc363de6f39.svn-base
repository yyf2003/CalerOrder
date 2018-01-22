<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpLoad.aspx.cs" Inherits="WebApp.FileUploadUC.UpLoad" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <style type="text/css">
       body{ margin:0px;}
    </style>
    <script type="text/javascript">
        var IsImg = /(jpg|jpeg|gif|png|bmp|tga|tif|ico|dwg|dwj)$/;
        var IsExcel = /(xlsx|xls)$/;
        var IsWord = /(doc|docx)$/;
        var IsZip = /(zip|rar)$/;
        var IsPPT = /(ppt|pptx)$/;

        function CheckFile() {

            var type = '<%=fileType %>';

            var val = document.getElementById("<%=FileUpload1.ClientID %>").value;

            if (val == "") {
                alert("请选择要上传的文件");
                return false;
            }
            else {
                var exten = val.substring(val.lastIndexOf('.') + 1).toLowerCase();
                if (type == "Image") {
                    if (!IsImg.exec(exten)) {
                        alert("只能上传图片文件");
                        ClearVal();
                        return false;
                    }
                }
                if (type == "Excel") {
                    if (!IsExcel.exec(exten)) {
                        alert("只能上传Excel文件");
                        ClearVal();
                        return false;
                    }
                }
                if (type == "Word") {
                    if (!IsWord.exec(exten)) {
                        alert("只能上传Word文件");
                        ClearVal();
                        return false;
                    }
                }
                if (type == "PPT") {
                    if (!IsPPT.exec(exten)) {
                        alert("只能上传PPT文件");
                        ClearVal();
                        return false;
                    }
                }
                $("#fileUploadId").hide();
                $("#showMsg").show();
                return true;
            }
            
        }

        function ClearVal() {

            document.getElementById("<%=form1.ClientID %>").reset();
        }



        function callback(jsonstr, code) {
            window.parent.ShowFiles(jsonstr, code);
            $("#showMsg").hide();
            $("#fileUploadId").show();
        }

        function AlertError() {
            alert("上传文件大小不能超过30M");
            $("#showMsg").hide();
            $("#fileUploadId").show();
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="fileUploadId">
        <asp:FileUpload ID="FileUpload1" runat="server" Style="width: 150px;" />
        <asp:Button ID="btn_UpLoad" runat="server" Text="上 传" OnClick="btn_UpLoad_Click"
            OnClientClick="return CheckFile()" />
    </div>
    <div id="showMsg" style="padding-left: 10px;  display: none; font-size: 13px; color: Red;
        line-height: 40px; background: #fff; height: 40px; width: 100%;">
        <img src="../image/WaitImg/loadingA.gif" />正在上传...
    </div>
    
    </form>
</body>
</html>
