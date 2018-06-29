
var postBackEleId;
Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(function (sender, e) {
    postBackEleId = e.get_postBackElement().id;
    if (postBackEleId.indexOf('ddlCustomer') != -1 || postBackEleId.indexOf('txtGuidanceMonth') != -1 || postBackEleId.indexOf('lbUp') != -1 || postBackEleId.indexOf('lbDown') != -1) {
        $("#loadGuidance").show();
    }
})

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {
    $("#loadGuidance").hide();
})


function getMonth() {
    $("#txtGuidanceMonth").blur();
}