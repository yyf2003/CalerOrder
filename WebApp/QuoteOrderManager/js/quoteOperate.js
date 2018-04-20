
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
       
    }

    
    if (eleId.indexOf("btnCheckAllGuidance") != -1) {
        $("#loadCategory").show();
      
    }


})

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
    $("#loadGuidance").hide();
    $("#loadCategory").hide();
    $("#cbCheckAllGuidance").change(function () {
        var checked = this.checked;
        $("input[name^='cblGuidanceList']").each(function () {
            this.checked = checked;
        });
        $("#btnCheckAllGuidance").click();
    })

    $("#btnAddQuote").click(function () {

        var month = $("#txtGuidanceMonth").val();
        var guidanceName = "";
        $("input[name^='cblGuidanceList']:checked").each(function () {
            guidanceName += $(this).val() + ',';
        })
        var subjectCategory = "";
        $("input[name^='cblSubjectCategory']:checked").each(function () {
            subjectCategory += $(this).val() + ',';
        })
        if (guidanceName == "") {
            layer.msg("请选择活动");
            return false;
        }
        var url = "AddQuotation.aspx?month=" + month + "&guidanceName=" + guidanceName + "&subjectCategory=" + subjectCategory;
        layer.open({
            type: 2,
            time: 0,
            title: '添加报价单',
            skin: 'layui-layer-rim', //加上边框
            area: ['95%', '90%'],
            content: url,
            id: 'layer1',
            //btn: ['提 交'],
            //btnAlign: 'c',
            //yes: function () {
            //POP.submit();
            //},
            cancel: function (index) {
                layer.closeAll();
            }

        });
    })


})

