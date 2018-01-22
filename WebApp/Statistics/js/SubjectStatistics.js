

$(function () {
    region.getRegion();
})

var region = {
    getRegion: function () {
        $.ajax({
            type: "get",
            url: "handler/SubjectStatistics.ashx",
            data: { type: "getRegion", subjectId: subjectId },
            success: function (data) {
                var div = "";
                if (data != "") {
                    var json = eval(data);
                    
                    for (var i = 0; i < json.length; i++) {
                        div += "<div style='float:left;'>";
                        div += "<input type='checkbox' name='RegionDivcb' value='" + json[i].RegionName + "'/><span>" + json[i].RegionName + "</span>&nbsp;";
                        div += "</div>";
                    }

                }
                $("#regionDiv").html(div);
            }
        })
    }
};