



$(function () {
    //添加
    $("#btnAdd").click(function () {
        ClearVal();
        frame.add("add");
    })
})

function checkHCPOP(obj) {
    var shopId = $(obj).data("shopid");
    var frameName = $(obj).parent().siblings("td").eq(7).html();
    var gender = $(obj).parent().siblings("td").eq(8).html();
    frame.getHCPOP(shopId, frameName, gender);
}
var currId = 0;
var jsonStr = "";
var editCornerType = "";
var editFrameName = "";
function editItme(obj,frameId) {
    ClearVal();
    if (obj != null)
        currId = $(obj).data("frameid");
    else if (frameId)
        currId = frameId;
    $.ajax({
        type: "get",
        url: "./Handler/MachineFrame.ashx?type=getModel&frameId=" + currId,
        cache: false,
        success: function (data) {

            if (data != "") {

                var json = eval(data);
                if (json) {
                    $("#txtPOSCode").val(json[0].POSCode);
                    $("#selSheet").val(json[0].PositionName);
                    GetSheetList(json[0].PositionName);
                    editCornerType = json[0].CornerType;
                    editFrameName = json[0].MachineFrame;
                    //$("#txtFrameName").val(json[0].MachineFrame);
                    //$("#txtCornerType").val(json[0].CornerType);
                    $("#txtNum").val(json[0].Count);
                    $("#txtLevelNum").val(json[0].LevelNum);
                    var gender = json[0].Gender || "";
                    $("input:radio[name='tblSex']").each(function () {
                        if ($(this).val() == gender) {
                            //$(this).attr("checked", "checked");
                            this.checked = true;
                        }

                    })
                    var isValid = json[0].IsValid;
                   
                    $("input:radio[name='radioIsValid']").each(function () {

                        if ($(this).val() == isValid) {
                            this.checked = true;
                        }
                    })

                    frame.edit();
                }
            }
        }
    })
}


var frame = {
    add: function (optype) {
        
        $("#editDiv").show().dialog({
            modal: true,
            width: 450,
            height: 350,
            iconCls: optype == "add" ? 'icon-add' : 'icon-edit',
            title: optype == "add" ? "添加器架类型" : "编辑器架类型",
            resizable: false,
            buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {


                            if (CheckVal()) {

                                $.ajax({
                                    type: "get",
                                    url: "./Handler/MachineFrame.ashx?type=edit&optype=" + optype + "&jsonString=" + escape(jsonStr),
                                    cache: false,
                                    success: function (data) {
                                        if (data == "ok") {
                                           
                                            $("#btnSearch").click();
                                        }
                                        else if (data == "exist") {
                                            alert("该器架类型已经存在！");
                                        }
                                        else {
                                            alert(data);
                                        }
                                    }
                                })
                            }
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
    },
    edit: function () {
        this.add("update");
    },
    getHCPOP: function (shopId, frameName, gender) {

        $.ajax({
            type: "get",
            url: "./Handler/MachineFrame.ashx",
            data: { type: "getHCPOP", shopId: shopId, frameName: frameName, gender: gender },
            success: function (data) {
                $("#HCPOPTbBody").html("");
                if (data != "") {
                    var json = eval(data);
                    var tr = "";
                    for (var i = 0; i < json.length; i++) {
                        tr += "<tr class='tr_bai'>";
                        tr += "<td>" + (i + 1) + "</td>";
                        tr += "<td>" + json[i].MachineFrame + "</td>";
                        tr += "<td>" + json[i].MachineFrameGender + "</td>";
                        tr += "<td>" + json[i].POP + "</td>";
                        tr += "<td>" + json[i].GraphicWidth + "</td>";
                        tr += "<td>" + json[i].GraphicLength + "</td>";
                        tr += "<td>" + json[i].Count + "</td>";
                        tr += "</tr>";
                    }
                    $("#HCPOPTbBody").append(tr);
                }
                $("#HCPOPDiv").show().dialog({
                    modal: true,
                    width: 600,
                    height: 300,
                    iconCls: 'icon-add',
                    resizable: false,
                    buttons: [
                    {
                        text: '确定',
                        iconCls: 'icon-ok',
                        handler: function () {
                            $("#HCPOPDiv").dialog('close');
                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'icon-cancel',
                        handler: function () {
                            $("#HCPOPDiv").dialog('close');
                        }
                    }
                ]
                });
            }
        })
    }
}


function CheckVal() {
    var posCode = $.trim($("#txtPOSCode").val());
    var sheet = $("#selSheet").val();
    var cornerType = $("#selCornerType").val();
    var frameName = $("#selFrameName").val();
    var sex = $("input[name='tblSex']:checked").val() || "";
    var num = $.trim($("#txtNum").val());
    var levelNum = $.trim($("#txtLevelNum").val());
    var IsValid = $("input:radio[name='radioIsValid']:checked").val() || 1;
    if (posCode == "") {
        alert("请填写店铺编号");
        return false;
    }
    if (sheet == "0") {
        alert("请选择pop位置");
        return false;
    }
    if (frameName == "") {
        alert("请填写器架类型");
        return false;
    }
    if (sex == "") {
        alert("请选择性别");
        return false;
    }
    if (num == "") {
        alert("请填写数量");
        return false;
    }
    else if (isNaN(num)) {
        alert("数量必须是整数");
        return false;
    }
    if (levelNum != "" && isNaN(levelNum)) {
        alert("级别必须是整数");
        return false;
    }
    levelNum = levelNum == "" ? 1 : levelNum;
    jsonStr = '{"Id":' + currId + ',"POSCode":"' + posCode + '","PositionName":"' + sheet + '","MachineFrame":"' + frameName + '","Gender":"' + sex + '","Count":"' + num + '","CornerType":"' + cornerType + '","LevelNum":"' + levelNum + '","IsValid":' + IsValid + '}';
   
    return true;
   
}

function ClearVal() {
    jsonStr = "";
    currId = 0;
    $("#selSheet").val("0");
    $("#selCornerType").val("");
    $("#selFrameName").val("");
    //$("#editDiv").find("input").not("[name='tblSex',name='radioIsValid']").val("");
    //$("#editDiv").find("input").not("[name='radioIsValid']").val("");
    $("#txtNum").val("1");
    $("#txtLevelNum").val("1");
    editCornerType = "";
    editFrameName = "";

}

function loading() {
    $("#loadingImg").show();
    checkExport();
    return true;
}

var timer;
function checkExport() {
    timer = setInterval(function () {
        $.ajax({
            type: "get",
            //url: "handler/CheckExportDetail.ashx",
            url: "handler/CheckExportState.ashx?type=shopFrame",
            cache: false,
            success: function (data) {

                if (data == "ok") {
                    $("#loadingImg").hide();
                    clearInterval(timer);
                }

            }
        })

    }, 1000);
}

function loadSearch() {
    $("#loadingSearch").show();
    return true;
}