
var customerId = 0;
var guidanceId = 0;
var region = "";
var province = "";
var city = "";
var subjectName = "";
var subjectNo = "";
var crNumber = "";
var poNumber = "";
var isSubmitPrice = 0;
var isInvoice = 0;
var beginDate = "";
var endDate = "";
var adContact = "";

$(function () {
    GetCondition();
    
    $(window).resize(function () {
        $("#jqGrid").setGridWidth($(window).width() * 0.98);

    })
    $("#jqGrid").jqGrid({
        url: 'handler/list1.ashx',
        mtype: "post",
        postData: { type: "getlist", customerId: customerId, subjectName: subjectName, subjectNo: subjectNo, crNumber: crNumber, poNumber: poNumber, isSubmitPrice: isSubmitPrice, adContact: adContact, isInvoice: isInvoice, beginDate: beginDate, endDate: endDate,guidanceId:guidanceId },
        datatype: "json",
        page: 1,
        colModel: [
                    { label: '', name: 'Id', hidden: true, key: true },
                    { label: '所属客户', name: 'CustomerName', width: 70 },
                    { label: '项目名称', name: 'SubjectName' },
                  //{ label: '项目负责人', name: 'Contact', width: 80 },
                    {label: '项目时间', name: 'SubjectDate', width: 80 },
                    { label: 'AD负责人', name: 'ADContact', width: 80 },
//                    { label: 'CRNumber', name: 'CRNumber', width: 80 },
//                    { label: 'PONumber', name: 'PONumber', width: 80 },
                    { label: '参考金额', name: '',
                        formatter: function (cellValue, options, rowObject) {
                            var cellHtml = "<span onclick='CheckMaterialPrice(" + rowObject.Id + ")' style='color:blue;cursor:pointer;'>" + rowObject.MateialPrice + "</span>";
                            return cellHtml;
                        },
                        width: 80
                    },
                    { label: '实际报价金额', name: 'TotalMoney', width: 100 },
                    { label: '报价单', name: '',
                        formatter: function (cellValue, options, rowObject) {
                            var cellHtml = "<span onclick='CheckFile(" + rowObject.Id + ")' style='color:blue;cursor:pointer;'>查看</span>";
                            return cellHtml;
                        },
                        width: 50
                    },
                    { label: '上传报价单', name: '',
                        formatter: function (cellValue, options, rowObject) {
                            var cellHtml = "<span onclick='upload(" + rowObject.Id + ")' style='color:blue;cursor:pointer;'>上传</span>";
                            return cellHtml;
                        },
                        width: 80
                    },
                    { label: '报价明细', name: '',
                        formatter: function (cellValue, options, rowObject) {
                            var cellHtml = "<a href='PriceList.aspx?subjectid=" + rowObject.Id + "' style='color:blue;cursor:pointer;'>查看</a>";
                            return cellHtml;
                        },
                        width: 60
                    },
                    { label: '对账补充', name: '',
                        formatter: function (cellValue, options, rowObject) {
                            var cellHtml = "<span onclick='EditSupplement(" + rowObject.Id + ")' style='color:blue;cursor:pointer;'>编辑</span>";
                            return cellHtml;
                        },
                        width: 60
                    }//,
//                    { label: '是否已开票', name: 'IsInvoice', width: 80 },
//                    { label: '开票', name: '',
//                        formatter: function (cellValue, options, rowObject) {
//                            var cellHtml = "<span onclick='EditInvoice(" + rowObject.Id + ")' style='color:blue;cursor:pointer;'>编辑</span>";
//                            return cellHtml;
//                        },
//                        width: 50
//                    }
                ],
        rownumbers: true,
        //hoverrows: true,
        height: 520,
        rowNum: 15,
        //rowList: [5, 10, 20, 30],
        viewrecords: true,
        pager: "#jqGridPager",
        caption: "项目信息",
        autowidth: true,
        //hidegrid: false, 
        subGrid: true,
        subGridOptions: {

            expandOnLoad: true,
            selectOnExpand: false,
            reloadOnExpand: true
        },
        subGridRowExpanded: function (subgrid_id, row_id) {

            var subgrid_table_id, pager_id;
            subgrid_table_id = subgrid_id + "_t";
            pager_id = "p_" + subgrid_table_id;
            $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
            $("#" + subgrid_table_id).jqGrid(
            {
                url: "handler/list1.ashx?type=sublist&subjectid=" + row_id,
                datatype: "json",
                colModel: [
                      { label: "费用类型", name: "Category", width: 80 },
                      { label: "分摊金额名称", name: "FTPriceName", width: 120 },
                      { label: "分摊金额", name: "FTPrice", width: 80 },
                      { label: "并入金额名称", name: "BRPriceName", width: 120 },
                      { label: "并入金额", name: "BRPrice", width: 80 },
                 ],

                height: '100%',
                autowidth: true,
                footerrow: true,
                gridComplete: function () {

                    $(".ui-jqgrid-sdiv").show();
                    var ftTotal = $(this).getCol("FTPrice", false, "sum");
                    var brTotal = $(this).getCol("BRPrice", false, "sum");
                    $(this).footerData("set", { "FTPriceName": "合计：", "FTPrice": parseFloat(ftTotal).toFixed(2), "BRPrice": parseFloat(brTotal).toFixed(2) });

                }
            });
        },
        gridComplete: function () {
            //$(this).subGrid.show();

        }

    });

    $('#jqGrid').closest("div.ui-jqgrid-view")
               .children("div.ui-jqgrid-titlebar")
               .css("background-color", "goldenrod")
               .children("span.ui-jqgrid-title")
               .addClass("captiontitle");



})

function GetCondition() {
    region = "";
    province = "";
    city = "";
    customerId = $("#ddlCustomer").val() || 0;
    guidanceId = $("#ddlGuidance").val() || -1;
//    $("input[name^='cblRegion']:checked").each(function () {
//        region += $(this).val() + ",";
//    })
//    $("input[name^='cblProvince']:checked").each(function () {
//        province += $(this).val() + ",";
//    })
//    $("input[name^='cblCity']:checked").each(function () {
//        city += $(this).val() + ",";
//    })
//    if (region.length > 0)
//        region = region.substring(0, region.length - 1);
//    if (province.length > 0)
//        province = province.substring(0, province.length - 1);
//    if (city.length > 0)
//        city = city.substring(0, city.length - 1);
    subjectName = $("#txtSubjectName").val();
    subjectNo = $("#txtSubjectNo").val();
    crNumber = $("#txtCRNumber").val();
    poNumber = $("#txtPONumber").val();
    isSubmitPrice = $("input[name='rblSubmitPrice']:checked").val() || 0;
    isInvoice = $("input[name='rblIsInvoice']:checked").val() || 0;
    beginDate = $("#txtBeginDate").val();
    endDate = $("#txtEndDate").val();
    adContact = "";
    $("input[name^='cbADContact']:checked").each(function () {
        adContact += $(this).val() + ",";
    })
    return false;
}


function Reload() {
    GetCondition();
    
    var url1 = "handler/list1.ashx?type=getlist";
    var postData = { customerId: customerId, subjectName: subjectName, subjectNo: subjectNo, crNumber: crNumber, poNumber: poNumber, isSubmitPrice: isSubmitPrice, adContact: adContact, isInvoice: isInvoice, beginDate: beginDate, endDate: endDate, guidanceId: guidanceId };
    $('#jqGrid').setGridParam({ url: url1, postData: postData, page: 1 }).trigger("reloadGrid");
}

$(function () {
    $("#btnSearch").on("click", function () {
        
        Reload();
    })
})