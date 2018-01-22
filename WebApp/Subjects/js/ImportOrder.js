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
    //return true;
}




