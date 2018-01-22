
function GetFiles(subjectId, secItemId, fileCode, fileType, fileObj, showDelete) {
    $(fileObj).html("");
    $.ajax({
        type: 'get',
        url: '/Handler/Files.ashx?type=getfiles&subjectid=' + subjectId + "&secitemid=" + secItemId + "&filecode=" + fileCode + "&filetype=" + fileType,
        cache: false,
        success: function (data) {

            if (data != "") {
                var FileJson = eval("(" + data + ")");
                for (var i = 0; i < FileJson.length; i++) {
                    ShowFiles(FileJson[i],fileObj, showDelete);
                }
            }
        }
    })
}

function ShowFiles(json,fileObj, showDelete) {
    
    if (json != null) {
        //$("#tr").css({ display: "block" });
        var id = json.Id;
        var fileName = json.Title;
        var imgPath = json.SmallImgUrl;
        var SourcePath = json.FilePath;

        var exten = SourcePath.substring(SourcePath.lastIndexOf('.') + 1);
        //SourcePath = SourcePath.replace("./", "/");
        //SourcePath = SourcePath.replace("~/", "/");
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

        div = "<div style='width:130px;float:left;margin-right:8px; margin-top:20px; height:120px;'><img src='" + imgSrc + "' width='130px' height='100px' />";
        if (showDelete)
            div += "<div style='font-size:12px; text-align:center;border:1px #ccc solid;'><a href='/Handler/Files.ashx?id=" + id + "&type=download' rel='" + fileName + "' title='" + fileName + "'>" + fileName + "</a>&nbsp;<span name='DeleteFile' fileid='" + id + "' onclick='DeleteFile(this)' style='font-size:12px;color:red;cursor:pointer;'>删除</span></div>";
        else
            div += "<div style='font-size:12px; text-align:center;width:130px;'><a href='/Handler/Files.ashx?id=" + id + "&type=download' rel='" + fileName + "' title='" + fileName + "'>" + fileName + "</a></div>";

        div += "</div>";
        //$("#showfiles").append(div);
        $(fileObj).append(div);

    }
}

function SetImgSrc(fileType) {
    var src = "";
    switch (fileType) {
        case "xls":
        case "xlsx":
            src = "/image/UpLoadFileType/EXCEL.png";
            break;
        case "doc":
        case "docx":
            src = "/image/UpLoadFileType/WORD.png";
            break;
        case "ppt":
        case "pptx":
            src = "/image/UpLoadFileType/PPT.png";
            break;
        case "rar":
        case "zip":
            src = "/image/UpLoadFileType/yasuo.png";
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

            url: "/Handler/Files.ashx?id=" + id + "&type=deletefile",
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