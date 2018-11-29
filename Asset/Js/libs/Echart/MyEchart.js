

define(function(){
    var ChartOptions = {
        ChartMontage: function (text, arrType, strStack, Data,CII)
        {// 基于准备好的dom，初始化echarts实例

            var legendData = new Array();
            var strxAxis = "";
            var Tooformatter = "{b} <br />";
            for (var i = 0; i < Data.length; i++) {
                legendData[i] = Data[i].legend;
                strxAxis = Data[i].xAxis;
                Tooformatter += "{a" + i + "}:{c" + i + "}%<br />"
            }
            var CiiLength = strxAxis.length - 1;
            var Legend = [];
            var strSeries = "[";

            for (var i = 0; i < Data.length; i++) {
                if (Data.length > 1 && i + 1 != Data.length) {
                    if (legendData[i]!="") {
                        Legend.push(legendData[i] + "");
                    }
                    strSeries += "{\"name\":\"" + legendData[i] + "\",";
                    strSeries += "\"type\":\"" + arrType[i] + "\",";
                    strSeries += "\"stack\":\"" + strStack + "\",";
                    strSeries += "\"data\":[" + Data[i].series + "]},";
                }
                else if (i + 1 == Data.length) {
                    if (legendData[i] != "") {
                        Legend.push(legendData[i] + "");
                    }
                    strSeries += "{\"name\":\"" + legendData[i] + "\",";
                    strSeries += "\"type\":\"" + arrType[i] + "\",";
                    strSeries += "\"stack\":\"" + strStack + "\",";
                    strSeries += "\"data\":[" + Data[i].series + "]"
                    if (CII != null) {
                        strSeries += ",\"markLine\":{\"lineStyle\":{\"color\":\"#FF8400\"},\"data\":[[{\"coord\":[ 0 ," + CII + "]},{\"coord\":[" + CiiLength + "," + CII + "]}]]}";
                    }
                    strSeries+="}]";

                }
            }

            var json = JSON.parse(strSeries);
            return {
                'Legend':Legend,
                'strxAxis': strxAxis,
                'json': json,
                'formatter': Tooformatter
            }
        },
        ChartOption: function (text, arrType, strStack, Data,nLum,CII) {
            var DataMontage = ChartOptions.ChartMontage(text, arrType, strStack, Data,CII)
            if (nLum == 1) {
                option = {
                    title: {
                        text: "" + text + "",
                        x: 'center',
                    },
                    tooltip: {
                        trigger: 'axis',
                        axisPointer: {
                            type: 'shadow',
                        },
                        formatter: DataMontage.formatter//a=系列名，b表示数据名，c表示数据值
                    },
                    //legend: {
                    //    top: 30,
                    //    align: 'left',
                    //    data: DataMontage.Legend,
                    //},
                    xAxis: {
                        data: DataMontage.strxAxis,
                        nameTextStyle: {
                            width: '10%',
                        }
                    },
                    yAxis: {
                        type: 'value',
                        axisLabel: {
                            show: true,
                            textStyle: {
                                color: '#000',
                                fontSize: '50%',
                            },
                            interval: 0,
                            showMinLabel: true,
                            formatter: '{value} %'
                        },
                        splitNumber: 10
                    },
                    series: DataMontage.json,
                    barGap: 0,
                    color: ['#4E5AB0']
                };
            }
            else if(nLum == 2){
                option = {
                    title: {
                        text: "" + text + "",
                        x: 'center'
                    },
                    tooltip: {
                        trigger: 'axis',
                        axisPointer: {
                            type: 'shadow'
                        }
                    },
                    //legend: {
                    //    top: 30,
                    //    align: 'left',
                    //    data: DataMontage.Legend,
                    //},
                    xAxis: {
                        data: DataMontage.strxAxis,
                    },
                    yAxis: {
                    },
                    series: DataMontage.json,
                    barGap: 0,
                    color: ['#4E5AB0']
                    //color: ['red', 'blue', '#61a0a8', '#d48265', '#91c7ae', '#749f83', '#ca8622', '#bda29a', '#6e7074', '#546570', '#c4ccd3']
                };
            }
            else if (nLum == 3) {
                option = {
                    title: {
                        text: "" + text + "",
                        x: 'center'
                    },
                    tooltip: {
                        trigger: 'axis',
                        axisPointer: {
                            type: 'shadow'
                        }
                    },
                    xAxis: {
                    },
                    yAxis: {
                        data: DataMontage.strxAxis,
                    },
                    series: DataMontage.json,
                    barGap: 0,
                    color: ['#4E5AB0']
                    //color: ['red', 'yellow', 'green']
                };
            }
            return option;
        },

        PieOption:function (text, Data) {
            var legendData = new Array();
            for (var i = 0; i < Data.length; i++) {
                legendData[i] = Data[i].name;
            }

            var Legend = [];

            for (var i = 0; i < Data.length; i++) {
                if (Data.length > 1 && i + 1 != Data.length) {

                    Legend.push(legendData[i] + '');
                }
                else if (i + 1 == Data.length) {

                    Legend.push(legendData[i] + '');
                }
            }

            var option = {
                title: {
                    text: "" + text + "",
                    x: 'center'
                },
                tooltip: {
                    left:'',
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    left: 'right',
                    data: Legend
                },
                series: [
                    {
                        name: "" + text + "",
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '60%'],
                        data: Data,
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };
            return option;
        }
    }
    return {
        ChartOptions: ChartOptions
    }
})


