;(function (tabs) {
    if (typeof define === "function" && define.amd) {
        //AMD模式
        define(["jquery"], tabs);
    } else {
        tabs(jQuery);
    }
}(function ($) {
    $.fn.tabs = function () {
        var element = $(this);

        element.find("li").bind("click", function () {
            var imgEle = element.find("li.nav_active img");
            var imgUrl = toggleUrl(imgEle.attr("src"), false);
            imgEle.attr("src", imgUrl);//把白色图片替换为黑色

            element.find("li").removeClass("nav_active");//移除选中样式

            $(this).addClass("nav_active");
            var selImgUrl = toggleUrl($(this).find("img").attr("src"), true);
            $(this).find("img").attr("src", selImgUrl);//黑色替换为白色
        });

        function toggleUrl(url, white) {
            if (white) {
                //黑色替换为白色
                if (url.indexOf('_White') == -1) {
                    var index = url.lastIndexOf('.');
                    url = insertStr(url, index, "_White");
                    return url;
                }
            }
            else {
                //白色替换为黑色
                if (!(url.indexOf('_White') == -1)) {
                    url = url.replace('_White', '');
                    return url;
                }
            }
        }

        function insertStr(str1, n, str2) {
            if (str1.length < n) {
                return str1 + str2;
            } else {
                s1 = str1.substring(0, n);
                s2 = str1.substring(n, str1.length);
                return s1 + str2 + s2;
            }
        }

        return this;
    }
}));