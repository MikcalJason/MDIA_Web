require.config({
    baseUrl: "../Asset/Js",
    paths: {
        "jquery": "libs/jquery-3.3.1",
        "validRight": "validRight"
    }
})

require(["jquery"], function ($) {
    var PullData = {
        Event:{
            OnClick:null
        },

        Init: function () {
            that = this;
            $.getJSON("../Ajax/Handler.ashx", { Command: 214 }, function (Info) {
                if (Info.Status == 1) {
                    that.Render(Info);
                }
                else {
                    alert(Info.Msg);
                }
            })
        },

        Setting: function () {
            var LastNum = $("#LastNum").val();
            var DateFrom = $("#DateFrom").val();
            var DateTo = $("#DateTo").val();
            if (($("#ckbOne").get(0).checked)) { DateFrom = ""; DateTo = "";}
            if (($("#ckbTwo").get(0).checked)) { LastNum = 0;}
            var SelectFactory = "";

            $("#FactoryList div button").each(function () {
                if ($(this).css("background-color") == "rgb(255, 132, 0)") {
                    SelectFactory = $(this).attr("id");
                }
            })
            
            $.getJSON("../Ajax/Handler.ashx", { Command: 215,LastNum:LastNum,DateFrom:DateFrom,DateTo:DateTo,SelectFactory:SelectFactory}, function (Info) {
                if (Info.Status == 1) {
                    alert(Info.Msg);
                }
                else {
                    alert(Info.Msg);
                }
            })
        },

        Render: function (data) {
            var divFac = $("#FactoryList");
            divFac.html("");

            var facList = data.ArrayFactory;
            var btnString = "";

            //渲染按钮
            for (var i = 0; i < facList.length; i++) {
                var name = facList[i].FactoryName;
                btnString = this.StringFacTemplate(name);
                divFac.append(btnString);
                if (name == data.SelectFactory)
                    $("#" + name).css("background-color", "#FF8400");
            }
            //最近几台车
            $("#LastNum").val(data.LastNum);

            //开始日期
            $("#DateFrom").val(data.DateFrom);
            //结束日期
            $("#DateTo").val(data.DateTo);

            if (data.LastNum > 0) {
                $("#ckbOne")[0].checked = true;
            }  //将输入框的状态设置为checked
            else
            {
                $("#ckbTwo")[0].checked = true;
            }
        },

        StringFacTemplate: function (name){
            var strBtnFac = "<div class='col-xs-4'><button id=" + name + " name='factory' type='button' style='background-color: #DEDEDE; width: 100%; height: 25px; border: none;margin-bottom:5px;'>" + name + "</button></div>";
            return strBtnFac;
        }
    }

    $(function () {
        PullData.Init();

        PullData.Event.OnClick = function () {
            this.css("background-color", "#FF8400");
        }

        $("#ckbOne").click(function () {
            if ($("#ckbOne").get(0).checked) {
                $("#ckbTwo")[0].checked = false;  //将输入框的状态设置为checked
            }
        })

        $("#ckbTwo").click(function () {
            if ($("#ckbTwo").get(0).checked) {
                $("#ckbOne")[0].checked = false; //将输入框的状态设置为checked         }
            }
        })
    })

    $("#btnSure").click(function () {
        PullData.Setting();
    })

    var facList = $("#FactoryList");

    //事件冒泡，点击子节点冒泡到父节点捕获
    facList[0].addEventListener("click", function (e) {
        if (e.target && e.target.nodeName.toLowerCase() == "button") {
            facList.children().each(function (index) {
                this.children[0].style.backgroundColor="#DEDEDE";//清除颜色
            })
            e.target.style.backgroundColor = "#FF8400";//设置颜色
        }
    })

})


