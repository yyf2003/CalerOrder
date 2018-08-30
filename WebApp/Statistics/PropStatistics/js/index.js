

Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
    var eleId = e.get_postBackElement().id;
    if (eleId.indexOf("lbUp") != -1 || eleId.indexOf("lbDown") != -1 || eleId.indexOf("btnGetProject") != -1) {
        $("#loadPropGuidance").show();
        $("#loadPropSubject").show();
    }

    if (eleId.indexOf("cblPropGuidanceList") != -1 || eleId.indexOf("btnCheckAllPropGuidance") != -1) {

        $("#loadOutsource").show();
        $("#loadPropSubject").show();
        //$("#cbAllOutsource").prop("checked", false);
        //$("#cbAllPropSubject").prop("checked", false);
    }

    if (eleId.indexOf("cblOutsource") != -1 || eleId.indexOf("btnCheckAllOutsource") != -1) {
        $("#loadPropSubject").show();
        //$("#cbAllPropSubject").prop("checked", false);
    }
});

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
    $("#cbCheckAllPropGuidance").change(function () {
        var checked = this.checked;
        $("input[name^='cblPropGuidanceList']").each(function () {
            this.checked = checked;
        });
        $("#btnCheckAllPropGuidance").click();
    })

    $("#cbAllOutsource").change(function () {
        var checked = this.checked;
        $("input[name^='cblOutsource']").each(function () {
            this.checked = checked;
        });
        $("#btnCheckAllOutsource").click();
    })

    $("#cbAllPropSubject").change(function () {
        var checked = this.checked;
        $("input[name^='cblPropSubject']").each(function () {
            this.checked = checked;
        });

    });

    $("span[name='spanReceivePrice']").click(function () {
        getCondition();
        var url = "OrderList.aspx?guidanceId=" + guidanceId + "&subjectId=" + subjectId;
        $.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: '90%',
            hideOnOverlayClick: false
        });
    });

    $("span[name='spanOutsourcePrice']").click(function () {
        getCondition();
        var url = "OutsourceOrderList.aspx?guidanceId=" + guidanceId + "&subjectId=" + subjectId + "&outsourceName=" + outsourceName;
        $.fancybox({
            href: url,
            type: 'iframe',
            padding: 5,
            width: '90%',
            hideOnOverlayClick: false
        });
    });
});

function getMonth() {
    
    $("#txtGuidanceMonth").blur();
}

function loading() {
    $("#loadingImg").show();
    return true;
}

var guidanceId = "";
var subjectId = "";
var outsourceName = "";

function getCondition() {
     guidanceId = "";
     subjectId = "";
     outsourceName = "";
     $("input[name^='cblPropGuidanceList']:checked").each(function () {
         guidanceId += $(this).val() + ",";
     });
     if (guidanceId == "") {
         $("input[name^='cblPropGuidanceList']").each(function () {
             guidanceId += $(this).val() + ",";
         });
     }
     $("input[name^='cblOutsource']:checked").each(function () {
         outsourceName += $(this).val() + ",";
     });
     $("input[name^='cblPropSubject']:checked").each(function () {
         subjectId += $(this).val() + ",";
     });

}
