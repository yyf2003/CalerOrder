
Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
    var eleId = e.get_postBackElement().id;
    if (eleId.indexOf("ddlCustomer") != -1 || eleId.indexOf("txtGuidanceMonth") != -1 || eleId.indexOf("lbUp") != -1 || eleId.indexOf("lbDown") != -1) {
        $("#loadGuidance").show();

    }
    if (eleId.indexOf("cblGuidanceList") != -1) {
        $("#loadCategory").show();
        $("#loadSubject").show();
        $("#loadOutsource").show();
        
    }
    if (eleId.indexOf("cblSubjectCategory") != -1) {

        $("#loadSubject").show();
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cblRegion") != -1) {
        $("#loadProvince").show();
        $("#loadSubject").show();
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cblProvince") != -1) {
        $("#loadCity").show();
        $("#loadSubject").show();
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cblCity") != -1) {
        $("#loadSubject").show();
        $("#loadOutsource").show();
    }
    if (eleId.indexOf("cblSubjects") != -1) {
        $("#loadOutsource").show();
    }
})


Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
    $("#loadGuidance").hide();
    $("#loadCategory").hide();
    $("#loadProvince").hide();
    $("#loadCity").hide();
    $("#loadSubject").hide();
    //$("#loadOutsource").hide();
})


function getMonth() {
   
    $("#txtGuidanceMonth").blur();
}

function loading() {
    var outsource = $("input[name^='cblOutsource']:checked").length;
    if (outsource == 0) {
        alert("请选择外协名称");
        return false;
    }
    $("#loadingImg").show();
    return true;
}