

define(['myEchart', 'echart'], function (MyEChart, echarts) {

//单个柱状图
    function SingleBar(id, text, Data,CII, clickmain) {
    var myChart = echarts.init(document.getElementById("" + id + ""));
    // 指定图表的配置项和数据
    var arrType = new Array();
    arrType[0] = "bar";
    var option;
    if (Data == null)
    {
        option = myChart.getOption();
    }
    else {

        option = MyEChart.ChartOptions.ChartOption(text, arrType, "", Data, 2, CII);
    }
    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
    Events.ClickMain(myChart, clickmain);

}
//多个柱状图
    function MuchBar(id, text, Data, CII, clickmain) {
    var myChart = echarts.init(document.getElementById("" + id + ""));
    // 指定图表的配置项和数据
    var arrType = new Array();
    for (var i = 0; i < Data.length; i++) {
        arrType[i] = "bar";
    }
    var option;
    if (Data==null) {
        option = myChart.getOption();
    }
    else{
        option = MyEChart.ChartOptions.ChartOption(text, arrType, "", Data, 1, CII);
    }
    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
    Events.ClickMain(myChart, clickmain);

}
//堆叠条形图
    function HeapBar(id, text, Data, CII, clickmain) {
    var myChart = echarts.init(document.getElementById("" + id + ""));
    // 指定图表的配置项和数据
    var arrType = new Array();
    for (var i = 0; i < Data.length; i++) {
        arrType[i] = "bar";
    }
    var option;
    if (Data == null) {
        option = myChart.getOption();
    }
    else {

        option = MyEChart.ChartOptions.ChartOption(text, arrType, "总量", Data, 3, CII);
    }
    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
    Events.ClickMain(myChart, clickmain);

}
//折线图
    function SingleLine(id, text, Data, CII, clickmain) {
    var myChart = echarts.init(document.getElementById("" + id + ""));
    // 指定图表的配置项和数据
    var arrType = new Array();
    arrType[0] = "line";
    var option;
    if (Data == null) {
        option = myChart.getOption();
    }
    else {

        option = MyEChart.ChartOptions.ChartOption(text, arrType, "", Data, 2, CII);
    }
    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
    Events.ClickMain(myChart, clickmain);
}
//多折线图
    function MuchLine(id, text, Data, CII, clickmain) {
    var myChart = echarts.init(document.getElementById("" + id + ""));
    // 指定图表的配置项和数据
    var arrType = new Array();
    for (var i = 0; i < Data.length; i++) {
        arrType[i] = "line";
    }
    var option;
    if (Data == null) {
        option = myChart.getOption();
    }
    else {

        option = MyEChart.ChartOptions.ChartOption(text, arrType, "", Data, 2, CII);
    }
    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
    Events.ClickMain(myChart, clickmain);

}

//柱折图
    function BarLine(id, text, Data, CII, clickmain) {
    var myChart = echarts.init(document.getElementById("" + id + ""));
    // 指定图表的配置项和数据
    var arrType = new Array();

    arrType[0] = "bar";
    arrType[1] = "line";
    var option;
    if (Data == null) {
        option = myChart.getOption();
    }
    else {
        option = MyEChart.ChartOptions.ChartOption(text, arrType, "", Data, 2, CII);
    }
    // 使用刚指定的配置项和数据显示图表。
    myChart.setOption(option);
    Events.ClickMain(myChart, clickmain);

}

//饼型图
    function Pie(id, text, Data, CII, clickmain) {
    var myChart = echarts.init(document.getElementById("" + id + ""));
    var option;
    if (Data == null) {
        option = myChart.getOption();
    }
    else {

        option = MyEChart.ChartOptions.PieOption(text, Data, CII);
    }
 
    myChart.setOption(option);
    Events.ClickMain(myChart, clickmain);
}

//
var Events = {
    ClickMain: function (myChart, clickmain) {
        myChart.on('click', function (params) {
            clickmain();
        });
    }
}
    return {
        SingleBar: SingleBar,
        MuchBar: MuchBar,
        HeapBar: HeapBar,
        SingleLine: SingleLine,
        MuchLine: MuchLine,
        BarLine: BarLine,
        Pie: Pie,
        Events:Events
    }
})