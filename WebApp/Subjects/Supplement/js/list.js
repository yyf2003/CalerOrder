
$(function () {

    $("#btnAdd").click(function () {
        $("#projectsDiv").html("");
        ShowEdit("add");
    })

    $("#btnSearchSubject").on("click", function () {
        GetProjects();

    })

    //活动只能单选
    $("#projectsDiv").delegate("input[name='projectCB']", "change", function () {
        if (this.checked) {
            $("#projectsDiv").find("input[name='projectCB']").not($(this)).attr("checked", false);
        }
    })

    $("#txtReason").on("keyup", function () {
        var val = $(this).val();
        if (val.length > 50)
            $(this).val(val.substring(0, 50));
    })


    $("span[name='spanEdit']").click(function () {
        $("#projectsDiv").html("");
        var itemId = $(this).data("itemid");

        currId = itemId;
        var customerId = $(this).data("customerid");
        $.ajax({
            type: "get",
            url: "/Subjects/Supplement/handler/List.ashx?type=getModel&id=" + itemId,
            success: function (data) {

                if (data != "") {
                    var json = eval(data);
                    if (json.length > 0) {

                        currSubjectId = json[0].SubjectId;
                        $("#ddlAddCustomer").val(customerId);
                        $("#txtBeginDate").val(json[0].BeginDate);
                        $("#txtEndDate").val(json[0].EndDate);
                        $("#txtPrice").val(json[0].Price);
                        $("#txtReason").val(json[0].Reason);
                        $("#btnSearchSubject").click();
                        ShowEdit("update");
                    }
                }
            }
        })

    });
})



var currSubjectId = "";
function GetProjects() {
    var customerId = $("#ddlAddCustomer").val() || "0";
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

                            var subjectId = "";
                            $("input[name='projectCB']:checked").each(function () {
                                subjectId = $(this).val();
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
                            }
                            var price = $.trim($("#txtPrice").val());
                            if (price != "" && isNaN(price)) {
                                alert("补单金额只能填写数字");
                                return false;
                            }
                            var reason = $.trim($("#txtReason").val());
                            if (reason == "") {
                                alert("请填写补单原因");
                                return false;
                            }
                            var json = '{"SupplementId":"' + currId + '","BeginDate":"' + begin + '","EndDate":"' + end + '","SubjectId":"' + subjectId + '","Price":"' + price + '","Reason":"' + reason + '"}';
                            $.ajax({
                                type: "get",
                                url: "/Subjects/Supplement/handler/List.ashx?type=add&optype=" + optype + "&jsonstr=" + json,
                                cache: false,
                                success: function (data) {
                                    if (data == "ok") {
                                        //window.location = "List.aspx";
                                        $("#editDiv").dialog('close');
                                        cleanVal();
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

function editOrder(itemId) {

    $("#iframe1").css({ height: 550 + "px" }).attr("src", "EditOrder.aspx?supplementId=" + itemId);
    $("#editOrderDiv").show().dialog({
        modal: true,
        width: '95%',
        height: 600,
        title: '编辑补单明细',
        resizable: false,
        cache: false,
        onClose: function () {
            $("#iframe1").attr("src", "");
           
        }
    });
}



function checkOrder(itemId) {

    $("#iframe2").css({ height: 550 + "px" }).attr("src", "CheckDetail.aspx?supplementId=" + itemId);
    $("#checkOrderDiv").show().dialog({
        modal: true,
        width: '95%',
        height: 600,
        title: '查看补单明细',
        resizable: false,
        onClose: function () {
            $("#iframe2").attr("src", "");
           
        }
        
    });
}

function ClearIframe() {
    $("#editOrderDiv").dialog('close');
    $("#iframe1").attr("src", "");
    $("#iframe2").attr("src", "");
    $("#btnSearch").click();
}

function cleanVal() {
    $("#addTable").find("input[type='text']").val("");

}

           