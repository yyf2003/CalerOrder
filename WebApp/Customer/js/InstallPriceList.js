
$(function () {

    CheckPrimission(url, null, null, $("#btnEdit"), $("#btnDelete"), null, $("#separator1"));
    Price.getList();
    Price.getMaterialSupport();
    $("#btnEdit").click(function () {

        Price.edit();

    })

    $("#btnAddNewRow").click(function () {
        var tr = "<tr class='tr_bai'>";
        tr += "<td>" + GetMaterialSupportDDL("") + "</td><td><input type='text' name='txtBasicInstallPrice' value='0' style='text-align:center;'/></td><td><input type='text' name='txtWindowInstallPrice' value='0' style='text-align:center;'/></td><td><span name='deleteSpan' data-id='0' style='color:red;cursor:pointer;'>删除</span></td>";
        tr += "</tr>";
        $("#editBody").append(tr);
    })

    $("#btnDelete").click(function () {
        Price.deleteItem();
    })

    $("#editBody").delegate("span[name='deleteSpan']", "click", function () {
        $(this).parent().parent().remove();
        //alert($("#editBody").html());
    })

    $("#editBody").delegate("input", "blur", function () {
        var val = $.trim($(this).val());

        if (val == "") {
            $(this).val("0");
        }
        else if (isNaN(val)) {
            $(this).val("0");
        }
    })

})
var MaterialSupportArr = [];
var dataJsonStr = "";
var Price = {
    model: function () {
        this.Id = 0;
        this.BasicMaterialSupportId = 0;
        this.MaterialSupport = "";
        this.BasicInstallPrice = 0;
        this.WindowInstallPrice = 0;
        this.OutsourceBasicInstallPrice = 0;
    },
    getMaterialSupport: function () {
        $.ajax({
            type: 'get',
            url: "./Handler/InstallPriceList.ashx?type=getMaterialSupportList",
            cache: true,
            success: function (data) {
                if (data != "") {
                    MaterialSupportArr = eval(data);
                }
            }
        })
    },
    getList: function () {
        $("#tbList").datagrid({
            method: 'get',
            url: './Handler/InstallPriceList.ashx?type=getList',
            columns: [[
               { field: 'Id', hidden: true },
               { field: 'RowIndex', title: "序号" },
               { field: 'checked', checkbox: true },
               { field: 'MaterialSupport', title: "级别", width: 120 },
               { field: 'BasicInstallPrice', title: "基础安装费", width: 200 },
               { field: 'WindowInstallPrice', title: "橱窗安装费", width: 300 },
               { field: 'OutsourceBasicInstallPrice', title: "外协基础安装费", width: 300 }

            ]],
            toolbar: '#toolbar',
            singleSelect: false,
            striped: true,
            border: false,
            pagination: false,
            fitColumns: false,
            height: 'auto',
            fit: false,
            iconCls: 'icon-save',
            emptyMsg: '没有相关记录',
            //selectOnCheck: false,
            //checkOnSelect:false,
            onClickRow: function (rowIndex, rowData) {
                $(this).datagrid("unselectRow", rowIndex);

            }
        })
    },
    edit: function () {
        $("#editBody").html("");
        var data = $("#tbList").datagrid("getData");

        var json = data.rows;
        if (json.length > 0) {
            var tr = "";
            for (var i = 0; i < json.length; i++) {
                
                tr += "<tr class='tr_bai'>";
                tr += "<td>" + GetMaterialSupportDDL(json[i].BasicMaterialSupportId) + "</td><td><input type='text' name='txtBasicInstallPrice' value='" + json[i].BasicInstallPrice + "' style='text-align:center;'/></td><td><input type='text'  name='txtWindowInstallPrice' value='" + json[i].WindowInstallPrice + "' style='text-align:center;'/></td><td><input type='text'  name='txtOSBasicInstallPrice' value='" + json[i].OutsourceBasicInstallPrice + "' style='text-align:center;'/></td><td><span name='deleteSpan' data-id='" + json[i].Id + "' style='color:red;cursor:pointer;'>删除</span></td>";
                tr += "</tr>";
            }
            $("#editBody").append(tr);
        }
        $("#editDiv").show().dialog({
            modal: true,
            width: 700,
            height: 450,
            iconCls: 'icon-add',
            resizable: false,
            buttons: [{
                text: '提交',
                iconCls: 'icon-ok',
                handler: function () {
                    if (CheckSubmitVal()) {

                        if (dataJsonStr != "") {
                            $.ajax({
                                type: 'post',
                                url: './Handler/InstallPriceList.ashx',
                                data: { type: "edit", jsonStr: escape(dataJsonStr) },
                                success: function (data) {
                                    if (data = "ok") {
                                        $("#editDiv").dialog('close');
                                        $("#tbList").datagrid("reload");
                                    }
                                    else
                                        alert(data);
                                }
                            })
                        }

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
    deleteItem: function () {
        var rows = $("#tbList").datagrid("getSelections");
        var ids = "";
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].Id + ",";
        }
        if (ids.length == 0) {
            alert("请选择要删除的行");
            return false;
        }
        if (confirm("确定删除吗？")) {
            $.ajax({
                type: 'post',
                url: './Handler/InstallPriceList.ashx',
                data: { type: "delete", ids: ids },
                success: function (data) {
                    if (data = "ok") {
                        $("#tbList").datagrid("reload");
                    }
                    else
                        alert(data);
                }
            })
        }
    }
}

//获取级别下拉框
function GetMaterialSupportDDL(id) {

    //var arr = ['Basic', 'VVIP', 'MCS', 'Premium', 'Others'];
    
    var select = "<select name='seleMaterialSupport'>";
    select += "<option value='0'>--请选择--</option>";
    for (var i = 0; i < MaterialSupportArr.length; i++) {
        var option = "";
        var selected = "";
        if (MaterialSupportArr[i].Id == id)
            selected = "selected='selected'";
        option = "<option value='" + MaterialSupportArr[i].Id + "' " + selected + ">" + MaterialSupportArr[i].MaterialSupportName + "</option>";
        select += option;
    }
    select += "</select>";

    return select;
}

function CheckSubmitVal() {
    var rows = $("#editBody tr");
    dataJsonStr = "";
   
    if (rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            var tr = rows[i];
            var id = $(tr).find("span[name='deleteSpan']").data("id")||0;
            var materialSupportId = $(tr).find("select[name='seleMaterialSupport']").val();
            var materialSupportName = $(tr).find("select[name='seleMaterialSupport'] option:selected").text();
            
            var basicInstallPrice = $(tr).find("input[name='txtBasicInstallPrice']").val();
            var windowInstallPrice = $(tr).find("input[name='txtWindowInstallPrice']").val();
            var osInstallPrice = $(tr).find("input[name='txtOSBasicInstallPrice']").val();
            if (materialSupportId != 0) {
                dataJsonStr += '{"Id":' + id + ',"BasicMaterialSupportId":' + materialSupportId + ',"MaterialSupport":"' + materialSupportName + '","BasicInstallPrice":' + basicInstallPrice + ',"WindowInstallPrice":' + windowInstallPrice + ',"OutsourceBasicInstallPrice":' + osInstallPrice + '},';
            }
        }
        dataJsonStr = "[" + dataJsonStr.substring(0, dataJsonStr.length - 1) + "]";
        return true;
    }
    else {
        alert("没有可提交的数据");
        return false;
    }
}