
function getMonth() {

    $("#txtGuidanceMonth").blur();
}

Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
    var eleId = e.get_postBackElement().id;
    if (eleId.indexOf("lbUp") != -1 || eleId.indexOf("lbDown") != -1) {
        $("#loadGuidance").show();

    }
    if (eleId.indexOf("cblGuidanceList") != -1) {
        $("#loadCategory").show();
        $("#loadSubject").show();
    }
    if (eleId.indexOf("btnCheckAllGuidance") != -1) {
        $("#loadCategory").show();
        $("#loadSubject").show();
    }
    if (eleId.indexOf("cblSubjectCategory") != -1) {
        $("#loadSubject").show();
    }

})

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
    $("#loadGuidance").hide();
    $("#loadCategory").hide();
    $("#loadSubject").hide();
    $("#cbCheckAllGuidance").change(function () {
        var checked = this.checked;
        $("input[name^='cblGuidanceList']").each(function () {
            this.checked = checked;
        });
        $("#btnCheckAllGuidance").click();
    })


    $("#cbAllSubject").change(function () {
        var checked = this.checked;
        $("input[name^='cblSubjectName']").each(function () {
            this.checked = checked;
        });
    })

    $("input[name^='cblSubjectName']").change(function () {
        if (!this.checked) {
            $("#cbAllSubject").prop("checked", false);
        }
        else {
            var checked = $("input[name^='cblSubjectName']:checked").length == $("input[name^='cblSubjectName']").length;
            $("#cbAllSubject").prop("checked", checked);
        }
    })



    $("#btnAddQuote").click(function () {
        var customerId = $("#ddlCustomer").val();
        var month = $("#txtGuidanceMonth").val();
        var guidanceId = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceId += $(this).val() + ',';
        })
        var subjectCategory = "";
        $("input[name^='cblSubjectCategory']:checked").each(function () {
            subjectCategory += $(this).val() + ',';
        })
        var subjectId = "";
        $("input[name^='cblSubjectName']:checked").each(function () {
            subjectId += $(this).val() + ',';
        })
        if (guidanceId == "") {
            layer.msg("请选择活动");
            return false;
        }
        if (subjectId == "") {
            layer.msg("请选择项目");
            return false;
        }
        var region = "";
        $("input[name^='cblRegion']:checked").each(function () {
            region += $(this).val() + ',';
        })
        $("#hfIsChange").val("");
        var url = "AddQuotation.aspx?customerId=" + customerId + "&month=" + month + "&guidanceId=" + guidanceId + "&subjectCategory=" + subjectCategory + "&subjectId=" + subjectId + "&region=" + region;
        layer.open({
            type: 2,
            time: 0,
            title: '添加报价单',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '90%'],
            content: url,
            id: 'layer1',
            cancel: function (index) {
                layer.closeAll();
                $("#btnRefreshGuidance").click();

            }

        });
    })


})


function editItem(id) {
    var url = "AddQuotation.aspx?itemId=" + id;
    layer.open({
        type: 2,
        time: 0,
        title: '编辑报价单',
        skin: 'layui-layer-rim', //加上边框
        area: ['95%', '90%'],
        content: url,
        id: 'layer1',
        cancel: function (index) {
            layer.closeAll();
            $("#btnRefreshOrder").click();
        }

    });
}

function checkItem(id) {
    var url = "CheckQuotation.aspx?itemId=" + id;
    layer.open({
        type: 2,
        time: 0,
        title: '查看报价单',
        skin: 'layui-layer-rim', //加上边框
        area: ['95%', '90%'],
        content: url,
        id: 'layer1',

        cancel: function (index) {
            layer.closeAll();
        }

    });
}

