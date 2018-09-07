var customerId = 0;
var guidanceMonth = "";
var currOutsourceId = "";
var currOutsourceName = "";
var loadingImg;
var selectedGuidanceId = "";

var totalShouldPayPrice = 0;
var totalPayPrice = 0;
var totalDebtPrice = 0;

$(function () {


    payRecord.getOutsourceRegion();
    payRecord.getOutsource();
    payRecord.getGuidanceList();
    $("#outsourceRegionDiv").delegate("input[name='cbosRegion']", "change", function () {
        payRecord.getOutsource();
    })

    //查询上个月活动
    $("#spanUp").click(function () {
        var month1 = $.trim($("#txtMonth").val());
        if (month1 != "") {
            month1 = month1.replace(/-/g, "/");
            var date = new Date(month1);
            date.setMonth(date.getMonth() - 1);
            $("#txtMonth").val(date.Format("yyyy-MM"));
            payRecord.getGuidance();
        }

    })

    //查询下个月活动
    $("#spanDown").click(function () {
        var month1 = $.trim($("#txtMonth").val());
        if (month1 != "") {
            month1 = month1.replace(/-/g, "/");
            var date = new Date(month1);
            date.setMonth(date.getMonth() + 1);
            $("#txtMonth").val(date.Format("yyyy-MM"));
            payRecord.getGuidance();
        }
    })

    $("#guidanceCBALL").click(function () {
        var checked = this.checked;
        $("input[name='guidanceCB']").each(function () {
            this.checked = checked;
        })
    })

    $("#guidanceDiv").delegate("input[name='guidanceCB']", "change", function () {
        var totalLen = $("input[name='guidanceCB']").length;
        var chckedLen = $("input[name='guidanceCB']:checked").length;
        if (chckedLen<totalLen) {
            $("#guidanceCBALL").prop("checked", false);
        }
        else if (totalLen > 0 && chckedLen == totalLen) {
            $("#guidanceCBALL").prop("checked", true);
        }
    })


    $("#btnSearch").click(function () {

        $("#loadingImg").show();
        setTimeout(function () {
            payRecord.getGuidanceList();
        }, 2000);

    })

    $("#btnSubmitMore").click(function () {
        //var price = $("#selectTotalPrice").html() || 0;
        if (selectedGuidanceId == "") {
            layer.alert("请选择活动");
            return false;
        }
        layer.open({
            type: 2,
            time: 0,
            title: '添加付款',
            skin: 'layui-layer-rim', //加上边框
            area: ['85%', '80%'],
            content: "AddPayment.aspx?guidanceId=" + selectedGuidanceId + "&outsourceId=" + currOutsourceId,
            id: 'popLayer',
            cancel: function () {
                layer.closeAll();
            }

        });
    })

    layui.use("table", function () {
        var table = layui.table;
        table.on('tool(tbGuidanceList)', function (obj) {
            var row = obj.data;
            var guidanceId = row.GuidanceId;
            if (obj.event == "add") {
                layer.open({
                    type: 2,
                    time: 0,
                    title: '添加付款',
                    skin: 'layui-layer-rim', //加上边框
                    area: ['85%', '80%'],
                    content: "AddPayment.aspx?guidanceId=" + guidanceId + "&outsourceId=" + currOutsourceId,
                    id: 'popLayer',
                    cancel: function () {
                        layer.closeAll();
                    }

                });
            }
            else if (obj.event == "detail") {
                layer.open({
                    type: 2,
                    time: 0,
                    title: '查看付款记录',
                    skin: 'layui-layer-rim', //加上边框
                    area: ['90%', '80%'],
                    content: "CheckPayDetail.aspx?guidanceId=" + guidanceId + "&outsourceId=" + currOutsourceId,
                    id: 'popLayer1',
                    cancel: function () {
                        if ($("#hfIsUpdate").val() == "1") {
                            payRecord.getGuidanceList();
                            $("#hfIsUpdate").val("");
                        }
                        layer.closeAll();
                    }

                });
            }
        });
        table.on('checkbox(tbGuidanceList)', function (obj) {
            selectedGuidanceId = "";
            var checkRow = table.checkStatus('tbGuidanceList');
            var checkData = checkRow.data;
            var selectShouldPayPrice = 0;
            var selectPayPrice = 0;
            var selectDebtPrice = 0;
            if (checkData.length > 0) {
                for (var i = 0; i < checkData.length; i++) {
                    selectShouldPayPrice = parseFloat(selectShouldPayPrice) + parseFloat(checkData[i].ShouldPayPrice);
                    selectPayPrice = parseFloat(selectPayPrice) + parseFloat(checkData[i].PayPrice);
                    selectDebtPrice = parseFloat(selectDebtPrice) + parseFloat(checkData[i].DebtPrice);
                    selectedGuidanceId += checkData[i].GuidanceId + ",";
                }
                $("#SpanAllShouldPay").html(selectShouldPayPrice.toFixed(2));
                $("#SpanAllPay").html(selectPayPrice.toFixed(2));
                $("#SpanAllDebt").html(selectDebtPrice.toFixed(2));
                if (selectDebtPrice > 0) {
                    $("#SpanAllDebt").addClass("redFont");
                }
                else {
                    $("#SpanAllDebt").removeClass("redFont");
                }
            }
            else {
                $("#SpanAllShouldPay").html(totalShouldPayPrice.toFixed(2));
                $("#SpanAllPay").html(totalPayPrice.toFixed(2));
                $("#SpanAllDebt").html(totalDebtPrice.toFixed(2));
                if (totalDebtPrice > 0) {
                    $("#SpanAllDebt").addClass("redFont");
                }
                else {
                    $("#SpanAllDebt").removeClass("redFont");
                }
            }

            //$("#selectTotalPrice").html(totalPrice.toFixed(2));

        });

    });
})

var payRecord = {

    getOutsourceRegion: function () {
        $("#outsourceRegionDiv").html("");
        $.ajax({
            type: "get",
            url: "list.ashx",
            data: { type: "getOutsourceRegion" },
            success: function (data) {
                if (data != "") {
                    var json = eval(data);
                    //var cbox = "";
                    for (var i = 0; i < json.length; i++) {
                        var cbox = "<input type='checkbox' name='cbosRegion' value='" + json[i].RegionId + "'>" + json[i].RegionName + "&nbsp;&nbsp;";
                        $("#outsourceRegionDiv").append(cbox);
                    }
                }
            }
        })
    },
    getOutsource: function () {
        var regionId = "";
        $("input[name='cbosRegion']:checked").each(function () {
            regionId += $(this).val() + ",";
        })
        $("#tbOutsource").datagrid({
            method: 'get',
            url: 'list.ashx?type=getOutsource&regionId=' + regionId,
            columns: [[
                    { field: 'CompanyId', hidden: true },
                    { field: 'CompanyName', title: '外协名称' }
                ]],
            height: '100%',
            border: false,
            fitColumns: true,
            singleSelect: true,
            rownumbers: true,
            emptyMsg: '没有相关记录',
            onClickRow: function (rowIndex, data) {
                selectOutsource();
            }
        });
    },
    getGuidance: function () {
        var customerId = $("#ddlCustomer").val();
        var guidanceMonth = $.trim($("#txtMonth").val());
        var outsourceId = currOutsourceId || -1;

        $.ajax({
            type: "get",
            url: "../handler/OrderList.ashx",
            data: { type: "getGuidanceList", outsourceId: outsourceId, customerId: customerId, guidanceMonth: guidanceMonth },
            beforeSend: function () {
                $("#ImgLoadGuidance").show();
            },
            complete: function () {
                $("#ImgLoadGuidance").hide();
                $("#btnSearch").click();
            },
            success: function (data) {

                $("#guidanceDiv").html("");
                if (data != "") {
                    var json = eval(data);
                    for (var i = 0; i < json.length; i++) {

                        var div = "<div style='float:left;'><input type='checkbox' name='guidanceCB' value='" + json[i].GuidanceId + "' />" + json[i].GuidanceName + "&nbsp;</div>";
                        $("#guidanceDiv").append(div);
                    }
                }
                else {
                    $("#guidanceDiv").html("<span style='color:red;'>无活动信息！</span>");
                }
            }
        });
    },
    getGuidanceList: function () {
        var customerId = $("#ddlCustomer").val();
        var guidanceMonth = $.trim($("#txtMonth").val());
        var outsourceId = currOutsourceId || -1;
        var guidanceId = "";
        $("input[name='guidanceCB']:checked").each(function () {
            guidanceId += $(this).val() + ",";
        })
        layui.use('table', function () {
            var table = layui.table;
            table.render({
                elem: '#tbGuidanceList',
                url: 'list.ashx',
                where: {
                    "type": "getGuidanceList",
                    "customerId": customerId,
                    "guidanceMonth": guidanceMonth,
                    "outsourceId": outsourceId,
                    "guidanceId": guidanceId
                },
                method: "post",
                cols: [[
                      { type: 'checkbox', fixed: 'left' },
                      { field: 'RowIndex', width: 60, title: '序号', style: 'color: #000;' },
                      { field: 'GuidanceMonth', width: 90, title: '活动月份', style: 'color: #000;' },
                      { field: 'GuidanceName', title: '活动名称', minWidth: 200, style: 'color: #000;' },
                      { field: 'ShouldPayPrice', width: 120, title: '应付金额', style: 'color: #000;' },
                      { field: 'PayPrice', title: '实付金额', width: 120, style: 'color: #000;' },
                      { field: 'DebtPrice', title: '欠款金额', width: 120, style: 'color: #000;', templet: function (d) {
                          if (d.DebtPrice > 0) {
                              return '<span style="color:red;">' + d.DebtPrice + '</span>';
                          }
                          else
                              return d.DebtPrice;
                      }
                      },
                      { field: 'PayRecordCount', width: 90, title: '付款记录', style: 'color: #000;' },
                      { field: 'LastPayDate', title: '最后付款时间', width: 120, style: 'color: #000;' },
                      { field: '', width: 150, title: '操作', style: 'color: #000;', toolbar: '#barDemo' }
                    ]],
                page: true,
                limit: 20,
                done: function (res, curr, count) {
                    totalShouldPayPrice = res.AllShouldPay;
                    totalPayPrice = res.AllPay;
                    totalDebtPrice = res.AllDebt;
                    $("#SpanAllShouldPay").html(totalShouldPayPrice.toFixed(2));
                    $("#SpanAllPay").html(totalPayPrice.toFixed(2));
                    $("#SpanAllDebt").html(totalDebtPrice.toFixed(2));
                    if (res.AllDebt > 0) {
                        $("#SpanAllDebt").addClass("redFont");
                    }
                    else {
                        $("#SpanAllDebt").removeClass("redFont");
                    }
                }

            });

        });
        $("#loadingImg").hide();

    }
};

function selectOutsource() {
    var rows = $("#tbOutsource").datagrid("getSelections");
    var oName = "";
    currOutsourceName = "";
    currOutsourceId = "";
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            currOutsourceId += (rows[i].CompanyId + ",");
            oName += (rows[i].CompanyName + ",");
        }
    }
    if (oName.length > 0) {
        oName = oName.substring(0, oName.length - 1);
    }
    if (currOutsourceId.length > 0) {
        currOutsourceId = currOutsourceId.substring(0, currOutsourceId.length - 1);
    }
    currOutsourceName = oName;
    $("#orderTitle").panel({
        title: ">>外协名称：<span style='color:blue;'>" + oName + "</span>"
    });
    payRecord.getGuidance();
    $("#subjectDiv").html("");
}



function getMonth() {
    payRecord.getGuidance();
}

