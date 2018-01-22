

var currPage = 1;
$(function () {
    POPLog.getList(currPage);
    $("#btnSearch").click(function () {
        currPage = 1;
        POPLog.getList(currPage);
    })
})
var POPLog = {
    getList: function (curr) {
        var loadIndex = layer.load(0, { shade: false });
        var pageSize = 10;
        var begin = $.trim($("#txtBeginDate").val());
        var end = $.trim($("#txtEndDate").val());

        $.getJSON('/Customer/EditLog/handler/MachineFrameLogList.ashx', {
            type: "getPOPLogList",
            shopId: shopId,
            currPage: curr,
            pageSize: pageSize,
            beginDate: begin,
            endDate: end

        }, function (res) {
            //从第1页开始请求。返回的json格式可以任意定义 
            DisplayList(res.rows);
            layer.close(loadIndex);
            layui.laypage({
                cont: 'page1',
                pages: Math.ceil(res.pageCount / pageSize),
                curr: curr || 1, //初始化当前页  
                skin: '#1E9FFF', //皮肤颜色  
                groups: 6, //连续显示分页数  

                first: '首页', //若不显示，设置false即可  
                last: '尾页', //若不显示，设置false即可  

                jump: function (obj, first) { //触发分页后的回调 
                    if (!first) {
                        currPage = obj.curr;
                        POPLog.getList(obj.curr);
                    }

                }
            });

        });
    }
};

function DisplayList(json) {

    if (json.length > 0) {
        $("#tbodyEmpty").hide();
        var tr = "";
        for (var i in json) {

            tr += "<tr>";
            tr += "<td>" + json[i].rowIndex + "</td>";
            tr += "<td>" + json[i].ChangeType + "</td>";
            tr += "<td>" + json[i].Sheet + "</td>";
            tr += "<td>" + json[i].GraphicNo + "</td>";
            tr += "<td>" + json[i].OOHInstallPrice + "</td>";
            tr += "<td>" + json[i].Gender + "</td>";
            tr += "<td>" + json[i].Quantity + "</td>";
            tr += "<td>" + json[i].GraphicWidth + "</td>";
            tr += "<td>" + json[i].GraphicLength + "</td>";
            tr += "<td>" + json[i].GraphicMaterial + "</td>";
            tr += "<td>" + json[i].WindowWide + "</td>";
            tr += "<td>" + json[i].WindowHigh + "</td>";
            tr += "<td>" + json[i].WindowDeep + "</td>";
            tr += "<td>" + json[i].WindowSize + "</td>";
            tr += "<td>" + json[i].PositionDescription + "</td>";
            tr += "<td>" + json[i].Category + "</td>";
            tr += "<td>" + json[i].Remark + "</td>";
            tr += "<td>" + json[i].IsValid + "</td>";
            tr += "<td>" + json[i].AddUserName + "</td>";
            tr += "<td>" + json[i].AddDate + "</td>";
            
            
            tr += "</tr>";
        }
        $("#tbodyData").html(tr).show();
    }
    else {
        $("#tbodyEmpty").show();
        $("#tbodyData").html("").hide();
    }
}