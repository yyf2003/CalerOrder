<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpLoadUC.ascx.cs" Inherits="WebApp.FileUploadUC.UpLoadUC" %>

<script type="text/javascript">
    $(function () {
        var code = '<%=Code %>';
        var jsonStr = '<%=JosnStr %>';
        if (jsonStr != "") {

            var FileJson = eval("(" + jsonStr + ")");
            for (var i = 0; i < FileJson.length; i++) {
                ShowFiles(FileJson[i], code);
            }
        }
    })

    function ShowFiles(json, code) {

        if (json != null) {
            $("#tr").css({ display: "block" });
            var id = json.Id;
            var fileName = json.Title;
            var imgPath = json.SmallImgUrl;
            var SourcePath = json.FilePath;

            var exten = SourcePath.substring(SourcePath.lastIndexOf('.') + 1);
            SourcePath = SourcePath.replace("./", "../../");
            SourcePath = SourcePath.replace("~/", "../../");
            var imgSrc = "";
            imgSrc = SetImgSrc(exten);

            var div = "";
            if (imgSrc == "" && imgPath == "") {
                imgSrc = "../../image/UploadFileType/Others.png";
            }
            if (imgSrc == "") {

                imgSrc = imgPath;
                imgSrc = imgSrc.replace("./", "../../");
                imgSrc = imgSrc.replace("~/", "../../");

            }

            div = "<div style='width:130px;float:left;margin-right:8px; margin-top:20px; height:120px;'><img src='" + imgSrc + "' width='100px' height='80px' />";

            div += "<div style='font-size:12px; text-align:center;border:1px #ccc solid;'><a href='../../Handler/Files.ashx?id=" + id + "&type=download' rel='" + fileName + "' title='" + fileName + "'>" + fileName + "</a>&nbsp;<span name='DeleteFile' fileid='" + id + "' onclick='DeleteFile(this)' style='font-size:12px;color:red;cursor:pointer;'>删除</span></div>";


            div += "</div>";
            $("#show" + code).append(div);


        }
    }

    function SetImgSrc(fileType) {
        var src = "";
        switch (fileType) {
            case "xls":
            case "xlsx":
                src = "../../image/UpLoadFileType/EXCEL.png";
                break;
            case "doc":
            case "docx":
                src = "../../image/UpLoadFileType/WORD.png";
                break;
            case "ppt":
            case "pptx":
                src = "../../image/UpLoadFileType/PPT.png";
                break;
            case "rar":
            case "zip":
                src = "../../image/UpLoadFileType/yasuo.png";
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

                url: "../../Handler/Files.ashx?id=" + id + "&type=deletefile",
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
<table width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td style="height: 25px; vertical-align: top;">
            <iframe src="../../FileUploadUC/Upload.aspx?userid=<%=UserId %>&filetype=<%=FileType %>&filecode=<%=FileCode %>&code=<%=Code %>&subjectid=<%=SubjectId %>"
                id="upFrame" name="upFrame" width="100%" style="height: 30px;" frameborder="0"
                scrolling="no"></iframe>
        </td>
    </tr>
</table>