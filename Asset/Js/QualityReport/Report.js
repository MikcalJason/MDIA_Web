require.config({
    baseUrl: "../Asset/Js",
    paths: {
        "jquery": "libs/jquery-3.3.1",
        "bsp":"libs/Bootstrap/bootstrap"
    },
    shim: {
        "bsp": {
            deps: ['jquery'],
            exports:'bsp'
        }
    }
}) 

require(["jquery","bsp"], function ($,bsp) {

    $.post("../Ajax/Handler.ashx", { command: 31, FileUrl: "ReportFile" }, function (Info) {
        var data = Info.arrTable[0];
        loads(data);
    },"json")

    $("#btnSearch").click(function () {
        var strSearch = $("#txtSearch").val();
        if (strSearch == null || strSearch == undefined || strSearch == "") {
           
        } else {
            $.post("../Ajax/Handler.ashx", { command: 217, FileUrl: strSearch }, function (Info) {
                var data = Info.arrTable[0];
                loads(data);
            },"json")
        }
    })

    $("#ulOptions")[0].addEventListener("click", function (e) {
        if (e.target && e.target.nodeName.toLowerCase() == "a") {
            var option = e.target.innerText;
            $.post("../Ajax/Handler.ashx", { command: 218, FileUrl: option }, function (Info) {
                var data = Info.arrTable[0];
                loads(data);
            },"json")
        }
    })
    function loads(data) {
        $("#Main a").html("");
        var dl = $("#Main a").eq(-1);

        var trHTML = "";
        for (var i = 0; i < data.length; i++) {
            if (data[i].Type == 0) {
                trHTML += "<a href='javascript:' onclick='add(this)' data-url='" + data[i].FileUrl + "'> <dl><dt><img src='data:image/png;base64," + data[i].Thumbnail + "' /></dt><dd class='left'>" + data[i].Name + "</dd> <dd class='right'><span class='glyphicon glyphicon-menu-right' aria-hidden='true'></span></dd></dl></a>";
            }
            if (data[i].Type == 1) {
                trHTML += "<a href='javascript:'  onclick='addpdf(this)' data-url='" + data[i].FileUrl + "'> <dl><dt><img src='data:image/png;base64," + data[i].Thumbnail + "' /></dt><dd class='left'>" + data[i].Name + "</dd> <dd class='right'></dd></dl></a>";
            }
        }
        trHTML += "<div style='height:130px;float:left;'></div>";
        dl.after(trHTML);
    }
})
