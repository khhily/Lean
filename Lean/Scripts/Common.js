(function (_) {
    _.alert = function (message) {
        WGX.ColorBox.Alert("tips", "页面提示信息", message, null, false, null, null);
    };

    _.doFunction = function (fun) {
        var args = [], i;
        for (i = 1; i < arguments.length; i++) {
            args.push(arguments[i]);
        }
        return function () {
            fun.apply(null, args);
        };
    };
})(window);

var IndexPageHelper = {};
(function (i) {
    i.SetHistory = function() {
        $(function() {
            if ($("#historyIndex").val() == "") {
                //新页面会执行这里
                var str = history.length + "|" + location.pathname;
                $("#historyIndex").val(str);
                CookieHelper.set("historyIndex", str, null, "/");
            } else {
                //回退页面会执行这里
                //CookieHelper.set("historyIndex", $("#historyIndex").val(), null, "/");
            }
        });
    };


    i.GoToIndex = function (defaultHref) {
        var tmp = (CookieHelper.get("historyIndex") || "|").split("|");
        var idx = tmp[0];
        var path = tmp[1];
        var backCount = history.length - idx;
        if (backCount > 0 && path.indexOf(defaultHref) >= 0)
            history.go(-backCount);
        else {
            location.href = defaultHref;
        }
    };

    $("a[data-xxy-action]").on("click", function () {
        var action = $(this).attr("data-xxy-action");
        if (confirm("确认 " + action + " 吗?")) {
            location.href = this.href;
        }
        return false;
    });
})(IndexPageHelper);


var CookieHelper = {};
(function ($) {

    $.getExpires = function(y, m, d, h, i, s, ms) {
        var date = new Date();
        y = isNaN(y) ? date.getFullYear() : y;
        m = isNaN(m) ? date.getMonth() : m - 1;
        d = isNaN(d) ? date.getDate() : d;

        h = isNaN(h) ? date.getHours() : h;
        i = isNaN(i) ? date.getMinutes() : i;
        s = isNaN(s) ? date.getSeconds() : s;
        ms = isNaN(ms) ? date.getMilliseconds() : ms;

        return new Date(y, m, d, h, i, s, ms).toUTCString();
    };

    $.getExpiresByUTCString = function(UTCString) {
        var s = new Date(UTCString).toUTCString();
        if (s == 'NaN' || s == 'Invalid Date')
            return null; // IE,Opera NaN , FF,Safari Invalid Date;
        else
            return s;
    };


    $.set = function(k, v, expires, path, domain, secure) {
        var cookie = k + '=' + encodeURIComponent(v);

        if (expires) cookie += ";expires=" + expires;
        if (path) cookie += ";path=" + path;
        if (domain) cookie += ";domain=" + domain;
        if (secure) cookie += ";secure";
        document.cookie = cookie;
    };

    $.get = function(k) {
        var cks = document.cookie.split(';');
        var t;
        for (var i = 0; i < cks.length; i++) {
            t = cks[i].split('=');
            if (k == t[0].trim()) return decodeURIComponent(t[1]);
        }
    };

    $.remove = function(k) {
        $.set(k, '', $.getExpires(new Date().getFullYear() - 1));
    };

    $.empty = function() {
        var cks = document.cookie.split(';');
        var t;
        for (var i = 0; i < cks.length; i++) {
            $.remove(cks[i].split('=')[0].trim());
        }
    };
})(CookieHelper);

$.fn.WGXUploadFile = function (options) {
    me = $(this);

    var defaultOpts = {
        action: '',
        method: 'post',
        params: '',
        inputName: '',
        inputValue: '',
        beforesubmit: function () { return true },
        callback: function (fileId, data) { }
    };

    var fileId = me.attr("id");
    var inputName = me.attr("name");

    removeFile = function () {
        $("#ifrm_upload_" + fileId).remove();
        $("#form_upload_" + fileId).remove();
        return;
    }

    if (options == "remove") {
        removeFile();
        return;
    }

    var opts = $.extend(defaultOpts, options);

    if (opts.inputName == "") {
        opts.inputName = inputName;
        if (opts.inputName == "") {
            opts.inputName = "upload_input_name";
        }
    }

    var form = $("<form id='form_upload_" + fileId + "' name='form_upload_" + fileId + "' target='ifrm_upload_" + fileId + "' action='" + opts.action + "' method='" + opts.method + "' enctype='multipart/form-data' style='display:none;' ></form>");
    var ifrm = $('<iframe name="ifrm_upload_' + fileId + '" id="ifrm_upload_' + fileId + '" style="position:absolute;top:-999px;"></iframe>');
    var params = opts.params.split('&');
    for (var i = 0; i < params.length; i++) {
        var values = params[i].split('=');
        if (values.length > 0) {
            $(form).append("<input type='hidden' name='" + values[0] + "' value='" + values[1] + "' />");
        }
    }

    $(ifrm).load(function () {
        var content = $(this).contents().get(0);
        var data = $(content).find("body").text();
        if (data == "" || data == undefined) {
            return;
        }
        data = eval('(' + data + ')');
        opts.callback(fileId, data);
    });

    me.change(function () {
        var pObj = $(this).parent();
        if (!opts.beforesubmit()) {
            return;
        }

        $(form).append($(this));
        $(form).submit();
        $(pObj).append($(this));
    });

    $("body").append(form);
    $("body").append(ifrm);
};