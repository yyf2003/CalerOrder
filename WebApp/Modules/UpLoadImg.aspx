<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpLoadImg.aspx.cs" Inherits="WebApp.Modules.UpLoadImg" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="/uploadify/jquery.uploadify.js" type="text/javascript"></script>
    <link href="/uploadify/uploadify.css" rel="stylesheet" type="text/css" /> 
    <style type="text/css">
       body{ margin:0px;}
    </style>
    <script type="text/javascript">
//        function upload(mid) {
//            $("#hfModuleId").val(mid);
//            $("#btnUpload").click();
//        }

//        function FinishUpload() {
//            parent.Reload();
//        }
    </script>
     <script type="text/javascript">
         var itemId = '<%=ItemId %>';
         var fileCode = '<%=FileCode %>';
         var fileType = '<%=FileType %>';
         var auth = "<% = Request.Cookies[FormsAuthentication.FormsCookieName]==null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value %>";
         var ASPSESSID = "<%= Session.SessionID %>";
         $(function () {
             GetFiles();

             $("#upload1").uploadify({
                 'swf': '../uploadify/uploadify.swf',
                 'uploader': '../uploadify/Handler1.ashx?subjectid=' + itemId + "&filecode=" + fileCode + "&filetype=" + fileType,
                 'fileSizeLimit': '200MB',
                 'formData': { 'ASPSESSID': ASPSESSID, 'AUTHID': auth },

                 'onQueueComplete': function (event, data) {
                     $("#showfiles").html("");
                     GetFiles();
                 }
             });


             function GetFiles() {
                 $.ajax({
                     type: 'get',
                     url: '../Handler/Files.ashx?type=getfiles&subjectid=' + itemId + "&filecode=" + fileCode + "&filetype=" + fileType,
                     cache: false,
                     success: function (data) {

                         if (data != "") {
                             var FileJson = eval("(" + data + ")");
                             for (var i = 0; i < FileJson.length; i++) {
                                 ShowFiles(FileJson[i]);
                             }
                         }
                     }
                 })
             }
         })




         function ShowFiles(json) {

             if (json != null) {
                 //$("#tr").css({ display: "block" });
                 var id = json.Id;
                 var fileName = json.Title;
                 var imgPath = json.SmallImgUrl;
                 var SourcePath = json.FilePath;

                 var exten = SourcePath.substring(SourcePath.lastIndexOf('.') + 1);
                 SourcePath = SourcePath.replace("./", "../");
                 SourcePath = SourcePath.replace("~/", "../");
                 var imgSrc = "";
                 imgSrc = SetImgSrc(exten);

                 var div = "";
                 if (imgSrc == "" && imgPath == "") {
                     imgSrc = "/image/UploadFileType/Others.png";
                 }
                 if (imgSrc == "") {

                     imgSrc = imgPath;
                     imgSrc = imgSrc.replace("./", "../");
                     imgSrc = imgSrc.replace("~/", "../");

                 }

                 div = "<div style='width:130px;float:left;margin-right:8px; margin-top:20px; height:120px;'><img src='" + imgSrc + "' width='100px' height='80px' />";

                 div += "<div style='font-size:12px; text-align:center;border:1px #ccc solid;'><a href='../../Handler/Files.ashx?id=" + id + "&type=download' rel='" + fileName + "' title='" + fileName + "'>" + fileName + "</a>&nbsp;<span name='DeleteFile' fileid='" + id + "' onclick='DeleteFile(this)' style='font-size:12px;color:red;cursor:pointer;'>删除</span></div>";


                 div += "</div>";
                 $("#showfiles").append(div);


             }
         }

         function SetImgSrc(fileType) {
             var src = "";
             switch (fileType) {
                 case "xls":
                 case "xlsx":
                     src = "../image/UpLoadFileType/EXCEL.png";
                     break;
                 case "doc":
                 case "docx":
                     src = "../image/UpLoadFileType/WORD.png";
                     break;
                 case "ppt":
                 case "pptx":
                     src = "../image/UpLoadFileType/PPT.png";
                     break;
                 case "rar":
                 case "zip":
                     src = "../image/UpLoadFileType/yasuo.png";
                     break;
             }
             return src;
         }



         function DeleteFile(obj) {
             if (confirm("确定删除吗？")) {
                 var id = $(obj).attr("fileid");

                 var div = $(obj).parent().parent("div");
                 $.ajax({
                     type: "get",

                     url: "../Handler/Files.ashx?id=" + id + "&type=deletefile",
                     success: function (data) {

                         if (data == "ok") {
                             div.remove();

                             if ($(".ShowFileInfo").html() == "") {
                                 $("#tr").css({ display: "none" });
                             }
                             return false;
                         }
                         else {
                             alert("删除失败");
                         }
                     }
                 })
             }
         }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <%--<asp:FileUpload ID="FileUpload1" runat="server" />
    <asp:Button ID="btnUpload" runat="server" Text="上传" 
         Style="display: none;" onclick="btnUpload_Click" />
         <asp:HiddenField ID="hfModuleId" runat="server" />--%>


    </form>
</body>
</html>
