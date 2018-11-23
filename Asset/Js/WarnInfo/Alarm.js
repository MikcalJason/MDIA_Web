require.config({
    baseUrl: "../Asset/Js",
    paths: {
        "jquery": "libs/jquery-3.3.1",
        "echart": "libs/Echart/echarts.min",
        "Mychart": "libs/Echart/MyChart",
        "myEchart": "libs/Echart/MyEChart",
        "cookie": "libs/jquery.cookie",
        "validRight": "validRight"
    }
})


//require(["jquery", "Mychart"], function ($, Myc) {
require(["validRight", "jquery", "Mychart"], function (valid, $, Myc) {

    //加载工厂下拉框
    $.getJSON("../Ajax/Handler.ashx", { Command: 1 }, function (Info) {
        var ddl = $("#Motorcycle");
        var data = Info.arrTable[0];
        var result = eval(data);
        $(result).each(function (key) {
            var opt = $("<option></option>").text(result[key].ProjectName).val(result[key].ProjectID);
            ddl.append(opt);
        })
        Station(result[0].ProjectID)
    });
    //获取报警
    function Station(nProjectID) {
        $.getJSON("../Ajax/Handler.ashx", { Command: 2, ProjectID: nProjectID }, function (Info) {
            var ddl = $("#Station");
            //清空选项
            ddl.empty();
            if (Info.Status == 1) {
                var data = Info.arrTable[0];
                var result = eval(data);
                var xAxis = [];
                var nPartID = "";
                for (var i = 0; i < result.length - 1; i++) {
                    //获取零件的id然后传入报警中
                    nPartID += result[i].PartID + ",";
                    xAxis.push(result[i].PartName + '');
                }
                nPartID += result[result.length - 1].PartID;
                xAxis.push(result[result.length - 1].PartName + '');
                $.post("../Ajax/Handler.ashx", { command: 212, ProjectID: 1, PartID: nPartID }, function (DataInfo) {
                    //讲报警统计信息取出
                    var Datas = [];
                    var datas = DataInfo.arrTable;
                    for (var i = 0; i < datas.length; i++) {
                        var series = [];
                        var Legend = "";
                        var results = eval(datas[i]);
                        if (results.length > 0) {
                            for (var j = 0; j < results.length; j++) {
                                var RuleDescs = results[j].RuleDesc.split(',');

                                var coum = 0;
                                for (var p = 0; p < RuleDescs.length; p++) {
                                    series = [];

                                    RuleDescs[p] = RuleDescs[p].replace(/\)/, "");

                                    var Rule = RuleDescs[p].split('(');

                                    for (var k = 0; k < datas.length; k++) {
                                        if (k == i) {
                                            series.push(parseInt(Rule[1]));
                                        }
                                        else {
                                            series.push(0);
                                        }
                                    }
                                    Legend = Rule[0];
                                    Datas.push({
                                        "legend": Legend,
                                        "xAxis": xAxis,
                                        "series": series
                                    })

                                    for (var f = 0; f < Datas.length - 1; f++) {
                                        //如果存在相同的系列名将系列合并
                                        if (Datas[f].legend == Legend) {
                                            for (var a = 0; a < Datas[f].series.length; a++) {
                                                //相同的参数合并在一起
                                                if (Datas[f].series[a] <= 0) {
                                                    series[a] = series[a];
                                                }
                                                else {
                                                    //如果参数不相同，将大的参数赋给新的
                                                    series[a] = Datas[f].series[a] + series[a];
                                                }
                                            }
                                            Datas[f].series = series;
                                            Datas.splice(Datas.length - 1, 1);
                                        }
                                    }
                                }

                            }
                        }

                    }
                    Myc.HeapBar("chartPassrate", "报警警告", Datas);
                    tables(datas);

                }, "json")
            }

        })
    }


    //通过点击工厂下报警信息
    $("#Motorcycle").change(function () {
        var nProjectID = $(this).val();
        Station(result[0].ProjectID)
    });

    function tables(data) {
        if (data.length > 0) {
            var arry = [];
            var Aveg = 0;
            var arrdata = [];
            var arrtirm = [];
            var sum = 0;
            for (var i = 0; i < data.length; i++) {
                var results = eval(data[i]);
                for (var j = 0; j < data[i].length; j++) {
                    Aveg += results[j].Count;
                    arrdata.push(results[j].Count);
                    arrtirm.push(/\d{4}-\d{1,2}-\d{1,2}/g.exec(results[j].MeasureTime));
                    sum++;
                }
            }
            for (var i = 0; i < arrtirm.length; i++) {
                for (var j = i + 1; j < arrtirm.length; j++) {
                    if (arrtirm[i] - arrtirm[j] > 0) {
                        var temp = arrtirm[j];
                        arrtirm[j] = arrtirm[i];
                        arrtirm[i] = temp;
                    }
                }
            }
            var tirm = arrtirm[0] + " ~ " + arrtirm[arrtirm.length - 1]
            arry.push(Math.round(Aveg / sum) + "次");
            arry.push(Math.round(Math.min.apply(null, arrdata)) + "次");
            arry.push(Math.round(Math.max.apply(null, arrdata)) + "次");
            arry.push(tirm);
            var arry1 = ["平均报警", "报警最少", "报警最多", "时间段"]
            var tb = $("#Parrtable tr").eq(-1);
            $("#Parrtable  tr:not(:first)").html("");
            var trHTML;
            for (var i = 0; i < arry.length; i++) {
                trHTML += "<tr><td  style='width: 180PX;'>" + arry1[i] + "</td><td  style='width: 240PX;'>" + arry[i] + "</td></tr>";
            }
            tb.after(trHTML);
        }
        else {
            $("#Parrtable  tr:not(:first)").html("");
        }
    }
    $(document).ready(function () {

    });

})


