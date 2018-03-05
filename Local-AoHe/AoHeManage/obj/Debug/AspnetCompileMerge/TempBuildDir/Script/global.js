Date.prototype.ToString = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(), //day
        "h+": this.getHours(), //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3), //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
     (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
        RegExp.$1.length == 1 ? o[k] :
        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}
Date.prototype.AddMonths = function (m) {
    var temp = new Date(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
    temp.setMonth(temp.getMonth() + m);
    return temp;
}
Date.prototype.AddDays = function (d) {
    var temp = new Date(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
    temp.setDate(temp.getDate() + d);
    return temp;
}
String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
}
Array.prototype.contains = function (needle) {
    for (i in this) {
        if (this[i] == needle) return true;
    }
    return false;
}
function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
function thistoString(json, char) {
    if (char == null) {
        char = "\"";
    }
    if (char != "'" && char != "\"") {
        throw new Error("参数 char 的值只能为 undefined、null、'、\" 其中之一。");
    }
    if (json == undefined || json == null) {
        return "null";
    }
    if (typeof (json) == "string") {
        return char + (json + "").replace("\"", "\\\"") + char;
    }

    var array = [];
    var keyValue = json;
    if (Object.prototype.toString.apply(json) === "[object Array]") {
        for (var i = 0; i < json.length; i++) {
            array.push(thistoString(json[i], char));
        }
        keyValue = "[" + array.join(", ") + "]";
    }
    else {
        if (Object.prototype.toString.apply(json) === "[object Date]") {
            keyValue = "new Date(" + json.getTime() + ")";
        }
        else {
            if (Object.prototype.toString.apply(json) === "[object RegExp]" || Object.prototype.toString.apply(json) === "[object Function]") {
                keyValue = json.toString();
            }
            else {
                if (Object.prototype.toString.apply(json) === "[object Object]") {
                    for (var i in json) {
                        array.push((typeof (i) == "string" ? char + i + char :
                            (typeof (i) === "object" ? thistoString(i, char) : i)) +
                            ":" +
                            (typeof (json[i]) == "string" ? char + (json[i] + "").replaceAll("\n", "\\n").replace("\"", "\\\"") + char : (typeof (json[i]) === "object" ? thistoString(json[i], char) : json[i])));
                    }
                    if (json == null) {
                        keyValue = "null";
                    }
                    else {
                        keyValue = "{" + array.join(", ") + "}";
                    }
                }
            }
        }
    }

    return keyValue;
}

function Loading(txt) {
    var loading = "";
    if (txt) {
        loading = "<div id='loading' onclick='ShowDiv()'><div id='txtLoading'>" + txt + "</div></div>";
    }
    else {
        loading = "<div id='loading' onclick='ShowDiv()'></div>";
    }
    $("body").prepend(loading);
    $("#loading").css("width", $(window).width());
    $("#loading").css("height", $(window).height());

    $("#txtLoading").css("top", ($(window).height() - 50) / 2);
    $("#txtLoading").css("left", ($(window).width() - 250) / 2);
}

function UnLoading() {
    $("#loading").css("display", "none");
}
function getweek(time) {
    /*显示星期*/
    /*time为date格式*/
    var str;
    var d = time.getDay();
    switch (d) {
        case 0:
            str = "星期日";
            break;
        case 1:
            str = "星期一";
            break;
        case 2:
            str = "星期二";
            break;
        case 3:
            str = "星期三";
            break;
        case 4:
            str = "星期四";
            break;
        case 5:
            str = "星期五";
            break;
        case 6:
            str = "星期六";
            break;
    }
    return str;
}