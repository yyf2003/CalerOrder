
$(function () {
    $("#btnAdd").click(function () {
        $("#projectsDiv").html("");
        ShowEdit("add");
    })

    $("#btnSearchSubject").on("click", function () {
        GetProjects();

    })

    $("#projectsDiv").delegate("input[name='projectCB']", "change", function () {
        if (this.checked) {
            $("#projectsDiv").find("input[name='projectCB']").not($(this)).attr("checked", false);
        }
    })

    $("span[name='spanEdit']").click(function () {
        $("#projectsDiv").html("");
        var begin = $(this).parent().siblings("td").eq(2).html(); ;
        var end = $(this).parent().siblings("td").eq(3).html();
        var itemId = $(this).data("itemid");
        var customerId = $(this).data("customerid");
        currSubjectId = $(this).data("subjectid");
        currId = itemId;
        $("#ddlCustomer").val(customerId);
        $("#txtBeginDate").val(begin);
        $("#txtEndDate").val(end);
        $("#btnSearchSubject").click();
        ShowEdit("update");
    })
})

var currSubjectId = "";
function GetProjects() {
    var customerId = $("#ddlCustomer").val() || "0";
    var begin = $.trim($("#txtBeginDate").val());
    var end = $.trim($("#txtEndDate").val());
    if (customerId == "0") {
        alert("请选择客户");
        return false;
    }
    if (begin == "" || end == "") {
        alert("请选择开始时间和结束时间");
        return false;
    }
    if (begin != "" && end != "") {
        var loading = "<img src='/image/WaitImg/loading1.gif' />";
        $("#projectsDiv").html(loading);
        $.ajax({
            type: "get",
            url: "/Subjects/Handler/CheckOrder.ashx",
            data: { type: "getProjects", customerId: customerId, beginDate: begin, endDate: end },
            cache: false,
            success: function (data) {
                $("#projectsDiv").html("");
                if (data != "") {

                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {
                        var checked = "";
                        if (currSubjectId != "" && currSubjectId == json[i].Id) {
                            checked = "checked='checked'";
                        }
                        var div = "<div style='float:left;margin-right:10px;margin-bottom:8px;'><input type='checkbox' name='projectCB' value='" + json[i].Id + "' " + checked + "/><span>" + json[i].SubjectName + "</span></div>";
                        $("#projectsDiv").append(div);
                    }
                }

            }

        })
    }

}








var currId = 0;
function ShowEdit(optype) {
    $("#editDiv").show().dialog({
        modal: true,
        width: 600,
        height: 350,
        iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
        title: optype == "add" ? '添加' : '编辑',
        resizable: false,
        buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            //$("#editDiv").dialog('close');
                            var subjectId = "";
                            $("input[name='projectCB']:checked").each(function () {
                                subjectId += $(this).val() + ",";
                            })
                            var begin = $("#txtBeginDate").val();
                            var end = $("#txtEndDate").val();
                            if ($.trim(begin) == "") {
                                alert("请选择开始时间");
                                return false;
                            }
                            if ($.trim(end) == "") {
                                alert("请选择结束时间");
                                return false;
                            }
                            if (subjectId == "") {
                                alert("请选择活动");
                                return false;
                            } else {
                                subjectId = subjectId.substring(0, subjectId.length - 1);
                            }
                            var json = '{"ItemId":"' + currId + '","BeginDate":"' + begin + '","EndDate":"' + end + '","SubjectId":"' + subjectId + '"}';
                            $.ajax({
                                type: "get",
                                url: "/Subjects/Material/handler/list.ashx?optype=" + optype + "&jsonstr=" + json,
                                success: function (data) {
                                    if (data == "ok") {
                                        //window.location = "List.aspx";
                                        $("#btnSearch").click();
                                    }
                                    else {
                                        alert(data);
                                    }
                                }
                            })
                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#editDiv").dialog('close');
                        }
                    }
                ]
    });
}

function importMaterial(itemId) {

    $("#iframe1").css({ height: 300 + "px" }).attr("src", "Import.aspx?itemid=" + itemId);
    $("#importDiv").show().dialog({
        modal: true,
        width: 600,
        height: 350,
        title: '导入物料信息',
        resizable: false,
        cache: false,
        onClose: function () {
            
            if ($("#hfIsFinishImport").val() == "1") {
                $("#hfIsFinishImport").val("");
                $("#btnSearch").click();

            }
            $("#iframe1").attr("src", "");
        }
    });
}


function checkMaterial(itemId) {
   
    $("#iframe1").css({height: 520 + "px" }).attr("src", "MaterialDetail.aspx?itemid=" + itemId);
    $("#importDiv").show().dialog({
        modal: true,
        width: '95%',
        height: 570,
        title: '物料信息',
        resizable: false,
        cache: false,
        onClose: function () {
            $("#iframe1").attr("src", "");
        }
    });
}


