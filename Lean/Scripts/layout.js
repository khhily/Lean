$(function () {
    BasePage = window.BasePage || {};

    (function (_) {
        _.Load = function () {
            bindButtonEvent = function () {
                
                //注销
                if ($("#btnLogout").length > 0) {
                    $("#btnLogout").click(function () {
                        $.ajax({
                            url: '/User/Logout',
                            data: '',
                            dataType: 'json',
                            type: 'post',
                            success: function (data) {
                                if (data.success) {
                                    $("#user_ul").html("").hide();
                                    $("#login_ul").show();
                                }
                            }
                        });
                    });
                }
            }
            bindButtonEvent();
            //登录
            $("#btnLogin").click(function () {
                var code = $("#UserCode").val();
                if ($("#UserCode").val() == "") {
                    alert("User Name can not be empty!");
                    return;
                }

                $.ajax({
                    url: '/User/Login',
                    data: $("#LoginForm").serialize(),
                    dataType: 'json',
                    type: 'post',
                    success: function (data) {
                        if (data.success) {
                            var backhtml = "";
                            if (data.IsAdmin) {
                                backhtml = '<li class="linkback"><div class="btn-group btn-group-xs"><a class="btn btn-link" href="/LeanManage/ManageHome/Index"><span>Back Manage</span></a></div></li>';
                            }
                            $("#login_ul").hide();
                            $("#user_ul").show().html('<li class="username_show">Hello, <a href="/Home/UserCenter/' + data.id + '">' + data.message + '</a></li>' +
                                backhtml +
                                '<li class="logout"><div class="btn-group btn-group-xs"><button class="btn btn-link" type="button" id="btnLogout" value="Logout"><span class="last-item">Logout</span></button></div></li>');
                            //'<li class="changepwd_show"><div class="btn-group btn-group-xs"><button class="btn btn-link" type="button" value="ChangePassword" id="btnChangePwd"><span>Change Password</span></button></div></li>'
                            bindButtonEvent();
                        } else {
                            alert(data.message);
                        }
                    },
                    error: function (data) {
                        alert("Server Error!");
                    }
                });
            });

            $("#ChangeRegCode").click(function () {
                WGX.ColorBox.AutoPage("注册", 600, 260, '/LeanManage/ManageHome/Registry');
            });

            $("#menu .menu_push").mouseover(function () {
                $(this).find(".menu_submenu").show();
            }).mouseout(function () {
                $(this).find(".menu_submenu").hide();
            });
        }();
    })(BasePage);
});