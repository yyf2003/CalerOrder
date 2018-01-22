<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MenuImgList.aspx.cs" Inherits="WebApp.Modules.MenuImgList" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="/CSS/Systen_style.css" rel="stylesheet" type="text/css" />
    <script src="/Scripts/jquery-1.7.2.js" type="text/javascript"></script>
    <script src="../uploadify/jquery.uploadify.js" type="text/javascript"></script>
    <link href="../uploadify/uploadify.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">

        var auth = "<% = Request.Cookies[FormsAuthentication.FormsCookieName]==null ? string.Empty : Request.Cookies[FormsAuthentication.FormsCookieName].Value %>";
        var ASPSESSID = "<%= Session.SessionID %>";
        $(function () {
            GetFiles();
            $("#upload1").uploadify({
                'swf': '../uploadify/uploadify.swf',
                'uploader': 'UploadHandler.ashx?type=upload',
                'fileSizeLimit': '5MB',
                'formData': { 'ASPSESSID': ASPSESSID, 'AUTHID': auth },
                'onQueueComplete': function (event, data) {
                    $("#showfiles").html("");
                    GetFiles();
                }
            });



        })


        function GetFiles() {
            $("#loadingDiv").show();
            $.ajax({
                type: 'get',
                url: 'UploadHandler.ashx?type=getfiles',
                cache: false,
                success: function (data) {
                    $("#loadingDiv").hide();
                    if (data != "") {
                        var FileJson = eval("(" + data + ")");
                        for (var i = 0; i < FileJson.length; i++) {
                            ShowFiles(FileJson[i]);
                        }
                    }
                }
            })
        }

        function ShowFiles(json) {

            if (json != null) {
                //$("#tr").css({ display: "block" });
                var id = json.Id;
                var SourcePath = json.Url;

                //var exten = SourcePath.substring(SourcePath.lastIndexOf('.') + 1);
                SourcePath = SourcePath.replace("./", "../");
                SourcePath = SourcePath.replace("~/", "../");
                var div;

                div = "<div style='float:left;margin-right:10px; margin-top:10px; width:100px; height:100px;text-align:center;'><img src='" + SourcePath + "' style='max-width:100px;max-height:100px;' />";

                div += "<div style='font-size:12px; text-align:center; margin-top:5px;'><span name='DeleteFile' fileid='" + id + "' onclick='DeleteFile(this)' style='font-size:12px;color:red;cursor:pointer;'>删 除</span></div>";

                div += "</div>";
                $("#showfiles").append(div);


            }
        }

        function DeleteFile(obj) {
            var id = $(obj).attr("fileid");
            var div = $(obj).parent().parent("div");
            $.ajax({
                type: "get",
                url: "UploadHandler.ashx?id=" + id + "&type=deletefile",
                success: function (data) {

                    if (data == "ok") {
                        div.remove();

                        return false;
                    }
                    else {
                        alert("删除失败");
                    }
                }
            })
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div class="nav_title">
        <a href="/index.aspx">
            <img src="/image/home.gif" width="47" height="44" style="float: left;" alt="" /></a><p
                class="nav_table_p">
                菜单图片管理
            </p>
    </div>
    <blockquote class="layui-elem-quote">
        <table class="table">
            <tr class="tr_bai">
                <td style="width: 100px;">
                    上传图片：
                </td>
                <td style="text-align: left; padding-left: 5px;">
                    <div style="margin-top: 10px;">
                        <input type="file" id="upload1" name="upload1" />
                    </div>
                </td>
            </tr>
        </table>
    </blockquote>
    <fieldset class="layui-elem-field layui-field-title" style="margin-top: 15px; font-size: 20px;
        font-weight: bold;">
        <legend>图片信息</legend>
    </fieldset>
    <div id="loadingDiv" style=" text-align:center; display:none;"><img src="../image/WaitImg/loading1.gif" /></div>
    
    <div id="showfiles" style=" margin-left:20px;">
    </div>
    </form>
</body>
</html>
<link href="/layui/css/layui.css" rel="stylesheet" type="text/css" media="all" />
<script src="/layui/layui.js" type="text/javascript"></script>
