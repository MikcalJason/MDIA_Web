require.config({
    baseUrl: "../Asset/Js",
    paths: {
        "jquery": "libs/jquery-3.3.1",
        "cookie": "libs/jquery.cookie",
        "validRight": "validRight"
    }
})

//require(["jquery", "cookie" ], function ($, _ ) {
require(["jquery", "cookie", "validRight"], function ($, _, valid) {
    $(function () {
        var UserName = $.cookie("UserName");
        $("#tbContainer").css("display", "block");
        $("#tbUser tr").each(function () {
            if (this.children[0].innerText == UserName) {
                $(this).remove();
            }
        })
  
    })
})