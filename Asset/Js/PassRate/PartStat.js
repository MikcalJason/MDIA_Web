require.config({
    baseUrl: "../Asset/Js",
    paths: {
        "jquery": "libs/jquery-3.3.1",
        "echart": "libs/Echart/echarts.min",
        "Mychart": "libs/Echart/MyChart",
        "myEchart": "libs/Echart/MyEChart",
        "bpjs": "libs/Bootstrap/bootstrap",
        "tmpl": "libs/jquery.tmpl"
    },
    shim: {
        "tmpl": {
            deps: ['jquery'],
            exports:'_'
        },
        "bpjs": {
            deps: ['jquery'],
            exports:'bpjs'
        }
    }
})

require(["jquery","tmpl","Mychart", "bpjs" ], function ($,_,Myc,bpjs) {
    var nodes = $("#Container>.row>.col-xs-10");//节点
    var parentNodes = $("#Container>.row");//父节点
    var nodeCur = 0;//子节点从1开始
    var parentIndex = 3;//父节点从3开始
    var ciiValue;
    var tmplFunc = {
        BeforeInit: function () {
            nodeCur = 0;
            parentIndex = 3;
        },

        Init: function (data) {
            var nModuleCount = WashedData.data.length;
            var nCurCount = nodes.length;
            if (nCurCount < nModuleCount) {
                for (var i = 0; i < nModuleCount - nCurCount; i++) {
                    parentNodes = $("#Container>.row");//父节点
                    var n = 3;
                    n += i;
                    var element = $("#tmpl").tmpl();
                    $(parentNodes[n]).after(element);
                }
            }
            nodes = $("#Container>.row>.col-xs-10");//节点
        },

        Add: function (name, data) {
            switch (name) {
                case "Label":
                    if ($("#tmplLabel").length > 0) {
                        for (var i = 0; i < data.Items.length; i++) {
                            if (data.Items[i].TagName == "CII指数") {
                                ciiValue = data.Items[i].Value[0];
                            }
                        }
                        $("#tmplLabel").tmpl(data).appendTo($(nodes[nodeCur]));
                    }
                    nodeCur++;
                    break;
                case "PassrateChart":
                    if ($("#tmplPassrateChart").length>0) {
                        $("#tmplPassrateChart").tmpl(data).appendTo($(nodes[nodeCur]));
                        $.each(data.Passrate, function (i, item) {
                            data.Passrate[i] = data.Passrate[i] * 100;
                        })
                        this.RenderBarChart("PassrateChart","合格率",data.AxisX,data.Passrate);
                    }
                    nodeCur++;
                    break;
                case "CIIChart":
                    if ($("#tmplCIIChart").length>0) {
                        $("#tmplCIIChart").tmpl(data).appendTo($(nodes[nodeCur]));
                        
                        this.RenderLineChart("CIIChart", "CII", data.AxisX, data.Passrate, ciiValue);
                    }
                    nodeCur++;
                    break;
                case "SortChart":
                    if ($("#tmplSortChart").length>0) {
                        $("#tmplSortChart").tmpl(data).appendTo($(nodes[nodeCur]));
                        $.each(data.Passrate, function (i, item) {
                            data.Passrate[i] = data.Passrate[i] * 100;
                        })
                        this.RenderHeapBar("SortChart", "质量排序", data.AxisX, data.Passrate);
                    }
                    nodeCur++;
                    break;
                case "Image":
                    if ($("#tmplImage")) {
                        $("#tmplImage").tmpl(data).appendTo($(nodes[nodeCur]));
                    }
                    nodeCur++;
                    break;
                case "MonthPassrateChart":
                    if ($("#tmplMonthPassrateChart").length>0) {
                        $("#tmplMonthPassrateChart").tmpl(data).appendTo($(nodes[nodeCur]));
                        $.each(data.Passrate, function (i, item) {
                            data.Passrate[i] = data.Passrate[i] * 100;
                        })
                        this.RenderBarChart("MonthPassrateChart", "月度合格率", data.AxisX, data.Passrate);
                    }
                    nodeCur++;
                    break;
                case "MonthCIIChart":
                    if ($("#tmplMonthCIIChart").length>0) {
                        $("#tmplMonthCIIChart").tmpl(data).appendTo($(nodes[nodeCur]));
                        $.each(data.CII, function (i, item) {
                            data.CII[i] = data.CII[i] * 100;
                        })
                        this.RenderBarChart("MonthCIIChart", "月度CII", data.AxisX, data.CII);
                    }
                    nodeCur++;
                    break;
            }
        },

        RenderBarChart: function (Id,Title,AxisX, FilePassrate) {
            var Datas = [{
                "legend": Title,
                "xAxis": AxisX,
                "series": FilePassrate
            }];
            Myc.MuchBar(Id, Title, Datas);
        },

        RenderLineChart: function (Id, Title, AxisX, FilePassrate,cii) {
            var Datas = [{
                "legend": Title,
                "xAxis": AxisX,
                "series": FilePassrate
            }];

            Myc.SingleLine(Id, Title, Datas,cii);
        },
        RenderHeapBar: function (Id, Title, AxisX, FilePassrate) {
            var Datas = [{
                "legend": Title,
                "xAxis": AxisX,
                "series": FilePassrate
            }];
            Myc.HeapBar(Id, Title, Datas);
        }
    }

    var WashedData = {
        data: [],
        Washed: function (Info) {
            var obj = {};
            obj.TagName = "Image";
            obj.URL = Info.URL;
            this.data.push(obj);
            for (var i = 0; i < Info.Tags.length; i++) {
                obj = {};
                obj = Info.Tags[i];
                this.data.push(obj);
            }
        }
    };

    var PullData = {
        Index: 0,
        ProjectList: null,
        DefaultAjax: function () {
            that = this;
            $.getJSON("../Ajax/Handler.ashx", { Command: 213 }, function (Info) {
                that.Render(Info);
            });
        },
        Ajax: function () {
            that = this;
            $.getJSON("../Ajax/Handler.ashx", { Command: 213, ProjectIndex: this.Index }, function (Info) {
                that.Render(Info);
            })
        },
        Render: function (Info) {
            if (Info.Status == 1) {
                WashedData.Washed(Info);//清洗数据
                tmplFunc.BeforeInit();
                tmplFunc.Init();//初始模板
                this.ShowArrow(Info);
                this.MarkIndex(Info);
                for (var i = 0; i < WashedData.data.length; i++) {
                    tmplFunc.Add(WashedData.data[i].TagName, WashedData.data[i]);
                }
                //this.RenderChart(Info.AxisX, Info.FilePassrate);
            }
        },

        InitProjectList: function () {
            that = this;
            $.getJSON("../Ajax/Handler.ashx", { Command: 216 }, function (Info) {
                if (Info.Status == 1) {
                    ModalData.Init(Info.Data);
                }
                else {
                    alert(Info.Msg);
                }
            })
        },
        ShowArrow: function (Info) {
            $("#arrowPrev").attr("src", "");
            $("#arrowNext").attr("src", "");
            if (Info.PrevProjectIndex != -1) {
                $("#arrowPrev").attr("src", "../Asset/Img/arrow_prev.png");
            }
            if (Info.NextProjectIndex != -1) {
                $("#arrowNext").attr("src", "../Asset/Img/arrow_next.png");
            }
        },
        MarkIndex: function (Info) {
            if (Info.ProjectIndex != -1) {
                $("#imgPart").attr("data-index", Info.ProjectIndex);
            }
        }
    }

    var ModalData = {
        ProjectTemplate: function (name, id) {
            var strBtnFac = "<div class='col-xs-4'><button id='" + name + "' name='project' data-index='" + id + "' type='button' style='background-color: #DEDEDE; width: 100%; height: 25px; border: none;margin-bottom:5px;'>" + name + "</button></div>";
            return strBtnFac;
        },

        Init: function (data) {
            $("#ProjectList").html("");//清空项目
            var ProjectList = data;
            var selectIndex = $("#imgIndex").attr("data-index");//图片是第几个
            var strProjectBtns = "";
            for (var i = 0; i < ProjectList.length; i++) {
                var ProjectName = ProjectList[i].ProjectName;
                strProjectBtns = this.ProjectTemplate(ProjectName, i);
                $("#ProjectList").append(strProjectBtns);
                if (i == selectIndex) {
                    $("#" + ProjectName).css("background-color", "#FF8400");
                }
            }
            this.AddEventListener();
        },

        AddEventListener: function () {
            var ProjectList = $("#ProjectList");
            ProjectList[0].addEventListener("click", function (e) {
                if (e.target && e.target.nodeName.toLowerCase() == "button") {
                    ProjectList.children().each(function (index) {
                        this.children[0].style.backgroundColor = "#DEDEDE";//清除颜色
                    })
                    e.target.style.backgroundColor = "#FF8400";//设置颜色
                }
            })
        }
    }

    $(document).ready(function () {
        //加载页面数据
        PullData.DefaultAjax();
        PullData.InitProjectList();
    })

    $("#arrowPrev").click(function () {
        var selectIndex = $("#imgIndex").attr("data-index");
        PullData.Index = selectIndex - 1;
        PullData.Ajax();
    })

    $("#arrowNext").click(function () {
        var selectIndex = $("#imgIndex").attr("data-index");
        PullData.Index = parseInt(selectIndex) + 1;
        PullData.Ajax();
    })

    $("#submit").click(function () {
        var SelectProject = "";
        $("#ProjectList div button").each(function () {
            if ($(this).css("background-color") == "rgb(255, 132, 0)") {
                SelectProject = $("#imgIndex").attr("data-index");
            }
        })
        PullData.Index = SelectProject;
        PullData.Ajax();
        $('#myModal').modal('hide')
    })
})