require.config({
    baseUrl: "./Asset/Js",
    paths: {
        "jquery": "libs/jquery-3.3.1",
        "cookie": "libs/jquery.cookie",
        "validRight":"validRight"
    }
})
require(["jquery", "cookie","validRight"], function ($, _,valid) {
    $("#btn_Login").click(function () {
        var isChecked = $("input[type='checkbox']").is(':checked');
        var name = $("#UserName").val();
        var password = $("#Password").val();
        $.post("Ajax/Handler.ashx", { Command: 102, UserName: name, Password: password }, function (data) {
            if (data.Status == 1 && data.Msg == "OK") {
                if (isChecked) {
                    $.cookie('UserName', name, {expires:7, path: '/' });
                    $.cookie('Password', password, {expires:7, path: '/' });
                }
                window.location.href = "./PassRate/PartStat.html";
            }
            else {
                alert("用户名密码错误！");
            }
        }, 'json')
    })
})