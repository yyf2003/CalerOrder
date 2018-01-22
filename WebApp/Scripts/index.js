var i = 0;
var timer;
var hasSubject = 0;
$(function () {
    GetSubjectList();
    timer = setInterval(GetSubjectList, 60000);
    $("#tbData").delegate("span[name='spanDeleteMsg']", "click", function () {
        var subjectId = $(this).data("subjectid");
        var span = $(this);
        $.ajax({
            type: "get",
            url: "Handler/index.ashx?type=deleteMsg&subjectId=" + subjectId,
            success: function (data) {
                if (data == "ok") {
                    $(span).parent().parent().hide(200);
                    $(span).parent().parent().remove();
                    if ($("#tbData tr").length == 0) {
                        $("#subjectContainer").slideUp(800);

                    }
                }
                else
                    alert(data);
            }
        })
    })

    $("#spanHideMsg").click(function () {
        var h = $("#subjectContainer").height();
        var state = $(this).data("state");

        if (state == "up") {

            $("#subjectContainer1").animate({ top: (h - 40) + "px" }, 400, function () {
                $("#spanHideMsg").data("state", "down");
                $("#spanHideMsg").html("<i class='layui-icon'>&#xe619;</i>");
                $(".spanBar").hide();
            });

        }
        else {

            $("#subjectContainer1").animate({ top: 0 + "px" }, 400, function () {
                $("#spanHideMsg").data("state", "up");
                $("#spanHideMsg").html("<i class='layui-icon'>&#xe61a;</i>");
                $(".spanBar").show();
            });

        }
    })

    $("#spanDeleteAll").click(function () {

        var ids = "";
        $("#tbData tr").each(function () {
            var id = $(this).find("span[name='spanDeleteMsg']").data("subjectid") || 0;
            ids += id + ",";
        })
        if (ids != "") {
            $.ajax({
                type: "form",
                url: "Handler/index.ashx",
                data: { type: "deleteAll", subjectIds: ids },
                success: function (data) {
                    if (data == "ok") {
                        clearInterval(timer);
                        $("#subjectContainer").slideUp(800);
                    }
                    else {
                        alert("操作失败");
                        //GetSubjectList();
                    }
                }
            })
        }
    })

    $("#spanClose").click(function () {
        clearInterval(timer);
        $("#subjectContainer").slideUp(800);
    })

    $("#spanRefresh").click(function () {
        GetSubjectList();
    })
})


function GetSubjectList() {
   
    $.ajax({
        type: "get",
        url: "Handler/index.ashx?type=getList",
        cache: false,
        success: function (data) {
            $("#tbData").html("");
            if (data != "") {
                $("#subjectContainer").show();
                var json = eval(data);
                if (json.length > 0) {
                    for (var i = 0; i < json.length; i++) {
                        var state = "";
                        if (json[i].ApproveState == 1) {
                            state = "<span style='color:green;'>通过</span>";
                        }
                        else {
                            state = "<span style='color:red;'>不通过</span>";
                        }
                        var tr = "<tr>";
                        tr += "<td>" + json[i].CustomerName + "</td>";
                        tr += "<td>" + json[i].GuidanceName + "</td>";
                        tr += "<td>" + json[i].SubjectName + "</td>";
                        tr += "<td>" + json[i].AddDate + "</td>";
                        tr += "<td>" + json[i].ApproveDate + "</td>";
                        tr += "<td>" + state + "</td>";
                        tr += "<td><a href='/Subjects/transfers.aspx?subjectId=" + json[i].Id + "'>查看</a></td>";
                        tr += "<td><span name='spanDeleteMsg' data-subjectid='" + json[i].Id + "' style='cursor:pointer; color:red;'>清除</span></td>";
                        tr += "</tr>";
                        $("#tbData").append(tr);
                    }
                }
                var h = $("#subjectContainer").height();
                $("#dataContent").css({ height: (h - 30) + "px" });
                
            }
            else {
                $("#subjectContainer").hide();
                clearInterval(timer);
            }
        }
    })
}

