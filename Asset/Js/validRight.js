define(['jquery', 'cookie'], function ($) {
    var ValidRight = {
        Cookies: {},
        EnumType: null,
        _getLocationUrl: function () {
            var url = window.location.pathname;
            var urlArray = url.split('/');
            url = urlArray[2];
            //系统管理员用户
            if (url == "SystemManager") this.EnumType = 1;
                //一般用户
            else this.EnumType = 0;
        },
        _getCookie: function () {
            this.Cookies.UserName = $.cookie("UserName") ? $.cookie("UserName") : null;
            this.Cookies.Password = $.cookie("Password") ? $.cookie("Password") : null;
        },
        _validate: function () {
            _self = this;
            if (_self.Cookies.UserName == null || _self.Cookies.Password == null) {
                //window.location.href = "./Login.html";
            }
            else {
                $.post("./Ajax/Handler.ashx", { Command: 102, UserName: _self.Cookies.UserName, Password: _self.Cookies.Password }, function (data) {
                    if (data.Status == 1 && data.Msg == "OK") {
                        window.location.href = "./PassRate/PartStat.html";
                    }
                    else {
                        //window.location.href = "./Login.html";
                    }
                }, "json")
            }
        }
    }
    ValidRight._getLocationUrl();
    ValidRight._getCookie();
    ValidRight._validate();
})