

$(function () {
    $("#content").html("<div style='width:50px;margin-left:auto;margin-right:auto;margin-top:20px;'><img src='../image/loadingB.gif'/></div>");
    $.ajax({
        type: "get",
        url: "../Handler/MenusHandler.ashx?type=getTopMenu",
        cache: false,
        success: function (data) {

            var div = "";
            var index = 1;
            if (data != "") {
                var json = eval(data);

                $.each(json, function (key, val) {
                    div += "<div class='panel-group' id='accordion1' style='max-height: 515px; overflow: auto; margin-bottom:8px;'>";
                    div += "<div class='panel panel-default'>";
                    div += "<div class='panel-heading' data-toggle='collapse' data-parent='#accordion" + index + "' href='#collapse" + index + "' style='cursor: pointer;'>";
                    div += "<span style='font-size: 15px; font-weight: bold;'>" + val.ModuleName + "</span>";
                    div += "</div>";
                    var classStr = index == 1 ? "class='panel-collapse in'" : "class='panel-collapse collapse'";
                    //var classStr = "class='panel-collapse collapse'";
                    div += "<div id='collapse" + index + "' " + classStr + ">";
                    div += "<div id='child" + val.Id + "' class='panel-body'>";
                    div += GetChilds(val.Id);
                    div += "</div>";
                    div += "</div>";
                    div += "</div>";
                    div += "</div>";
                    index++;
                })
            }
            //var div1 = "<div style=' overflow:auto;'>" + div + "</div>";
            $("#content").html(div);
        }
    })


    

})



function GetChilds(parentId) {

    $.ajax({
        type: "get",
        url: "../Handler/MenusHandler.ashx?type=getChildMenu&parentId=" + parentId,
        cache:false,
        success: function (data) {
            var ahtml = '<ul class="menuList" style="max-height:300px;overflow:auto;">';
            if (data != "") {
                var json = eval(data);
                $.each(json, function (key, val) {
                    var url = val.Url;

                    ahtml += "<li><img src='zTree_v3/css/zTreeStyle/img/diy/1_open.png' /><a href='" + url + "' target='ifrContext'>" + val.ModuleName + "</a></li>";
                })
                ahtml += '</ul>';
            }
            $("#child" + parentId).html(ahtml);
        }
    })
}