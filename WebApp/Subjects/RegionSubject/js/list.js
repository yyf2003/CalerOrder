



Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
    var eleId = e.get_postBackElement().id;
    if (eleId.indexOf("ddlCustomer") != -1 || eleId.indexOf("txtGuidanceMonth")!=-1) {
        $("#loadGuidance").show();
    }
})
Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
    
    $("#loadGuidance").hide();
    
})

$(function () {
    CheckPrimission(url, null, $("#btnAdd"),null, null, null, null);
})

function getMonth() {
    
    $("#txtGuidanceMonth").blur();
}