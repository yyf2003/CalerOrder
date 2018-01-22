
function urlCodeStr(str) {
    return escape(str).replace(/\+/g, '%2B');
}

function CheckPrimission(url, view, add, edit, dele,recover, separator,showLog) {
    $.ajax({
        type: "get",
        url: '/Handler/CheckPromission.ashx?url=' + url,
        success: function (data) {

            if (data != "") {
                var arr = data.split('|');
                var count = 0;
                $.each(arr, function (key, val) {
                    if (val == "view") {
                        if (view) {
                            $(view).show();
                            count++;
                        }
                    }
                    if (val == "add") {
                        
                        if (add) {
                           $(add).show();
                           count++;
                       }
                       if (showLog) {
                           $(showLog).show();
                           count++;
                       }
                    }

                    if (val == "edit") {
                        if (edit) {
                            $(edit).show();
                            count++;
                        }

                    }
                    if (val == "delete") {
                        if (dele) {
                            $(dele).show();
                            count++;
                        }
                        if (recover) {
                            $(recover).show();

                        }
                    }

                })
                if (count > 0) {
                    if (separator) {
                        $(separator).show();

                    }

                }
            }
        }
    })
}

function CheckPrimissionWithMoreBtn(url, viewArr, addArr, editArr, deleArr, recoverArr, separator) {
    $.ajax({
        type: "get",
        url: '/Handler/CheckPromission.ashx?url=' + url,
        success: function (data) {

            if (data != "") {
                var arr = data.split('|');
                var count = 0;
                $.each(arr, function (key, val) {
                    if (val == "view") {

                    }
                    if (val == "add") {
                        if (addArr) {
                            $.each(addArr, function (key0, val0) {
                                $(val0).show();
                                count++;
                            })

                        }
                    }
                    if (val == "edit") {
                        if (editArr) {
                            $.each(editArr, function (key1, val1) {
                                $(val1).show();
                                count++;
                            })

                        }
                    }
                    if (val == "delete") {
                        if (deleArr) {
                            $.each(deleArr, function (key2, val2) {
                                $(val2).show();
                                count++;
                            })
                        }
                        if (recoverArr) {
                            $.each(recoverArr, function (key3, val3) {
                                $(val3).show();
                                count++;
                            })
                        }
                    }

                })

                if (count > 0) {
                    if (separator) {
                        $(separator).show();

                    }

                }
                
            }
        }
    })
}

//写cookie
function setCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}

//读取cookies 
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}

//删除cookies 
function delCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null)
        document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}