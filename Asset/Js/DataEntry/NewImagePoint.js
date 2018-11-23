require.config({
    baseUrl: "../Asset/Js",
    paths: {
        "jquery": "libs/jquery-3.3.1",
        "zrender": "libs/Zrender/zrender",
        "imagePointCtrl": "libs/Zrender/ImagePointCtrl",
        "cookie": "libs/jquery.cookie",
        "validRight": "validRight",
        "bpjs": "libs/Bootstrap/bootstrap"
    }
})
require(["jquery", "imagePointCtrl", "bpjs"], function ( $, _, bpjs) {
//require(["validRight", "jquery", "imagePointCtrl", "bpjs"], function (valid, $, _, bpjs) {
    var imagePointCtrl = new _.ImagePointCtrl("main");
    var PointImage = {
        Data: {
            Dom: {
                head: null,
                main: null,
                form: null,
            },
            ProjectID: "",
            PartID: "",
            Image: {
                ImageID: "",
                ImageSource: null,
                Width: 0,
                Height: 0,
                ImagePoint: []
            },

            ProcessList: [],

            NominalList: [],

            PointIndex: 0
        },
        //入口
        Init: function (obj, dom) {
            if (dom != null) {
                this.Data.Dom.head = dom[0];
                this.Data.Dom.main = dom[1];
                this.Data.Dom.form = dom[2];
            }
            if (obj.proid != null)
                this.Data.ProjectID = obj.proid;
            if (obj.partid != null)
                this.Data.PartID = obj.partid;
            if (obj.imgid != null)
                this.Data.Image.ImageID = obj.imgid;
            this.Ajax();
        },

        Ajax: function () {
            var _self = this;
            var condition = _self.Data;
            var alertMsg = [];
            //获取工序、图片、图片上点的信息
            $.post("../Ajax/Handler.ashx", { Command: 100, ProjectID: condition.ProjectID, PartID: condition.PartID, ImageID: condition.Image.ImageID }, function (data) {
                if (data.ImageModel.Message == null && data.ImageModel.ImageModel.ImageData != null) {
                    _self.Data.Image.ImageSource = data.ImageModel.ImageModel.ImageData;
                    _self.Data.Image.ImageID = data.ImageModel.ImageModel.ImageID;
                    _self.Data.Image.Width = data.ImageModel.Width;
                    _self.Data.Image.Height = data.ImageModel.Height;
                }
                if (data.ProgressModel.Message == null && data.ProgressModel.ProgressArray.length != 0) {
                    _self.Data.ProcessList = data.ProgressModel.ProgressArray;
                } else {
                    alertMsg.push("缺少工序 ");
                }
                if (data.ImagePointModel.Message == null && data.ImagePointModel.ImagePointArray.length != 0) {
                    _self.Data.Image.ImagePoint = data.ImagePointModel.ImagePointArray;
                } else {
                    alertMsg.push("缺少测量点、测量点的位置信息 ");
                }
                if (data.NominalModel.Message == null && data.NominalModel.NominalArray.length != 0) {
                    _self.Data.NominalList = data.NominalModel.NominalArray;
                } else {
                    alertMsg.push("缺少名义值、上下公差信息 ");
                }
                _self.Render();
                if (alertMsg.length != 0)
                    alert(alertMsg.join("\n") + " ！");
            }, "json")
        },

        Render: function () {
            //绑定工序
            this.BindProcess();
            //绑定图片
            this.ShowImage();

            this.ShowPoint();

            this.BindForm();

            this.FirstTooltip();
        },

        BindProcess: function () {
            var dom = this.Data.Dom;
            var processList = this.Data.ProcessList;
            var selection = dom.head.find("select");
            var option = "";
            for (var i = 0; i < processList.length; i++) {
                option += "<option value='" + processList[i].WorkProgressID + "'>" + processList[i].WorkProgressName + "</option>";
            }
            selection.html(option);
            var submit = $("#submit")[0];
            submit.onclick = $.proxy(this.Submit, this);
        },

        BindForm: function () {
            this.ShowInfo();

            $("#buttonPrev")[0].onclick = $.proxy(this.ClickPrev, this);
            $("#buttonNext")[0].onclick = $.proxy(this.ClickNext, this);
            $("#btnSure")[0].onclick = $.proxy(this.Sure, this);
        },

        ShowInfo: function () {
            //输入框
            var txtInput = this.Data.Dom.form.find("input[type='tel']");

            //上一个
            var buttonPrev = $("#buttonPrev");
            //下一个
            var buttonNext = $("#buttonNext");
            //选中的点
            var selectIndex = this.Data.PointIndex;
            //点的个数
            var _length = this.Data.Image.ImagePoint.length;
            if (_length == 0) {
                buttonPrev.attr("disabled", "disabled");
                buttonNext.attr("disabled", "disabled");
            } else {
                //如果是第一个点 上一个点按钮禁用
                if (selectIndex == 0)
                    buttonPrev.attr("disabled", "disabled");
                else
                    buttonPrev.attr("disabled", false);
                //如果是最后点 下一个点按钮禁用
                if (selectIndex == _length - 1)
                    buttonNext.attr("disabled", "disabled");
                else
                    buttonNext.attr("disabled", false);
            }
            //如果不存在value或者value为空
            if (this.Data.Image.ImagePoint == null || this.Data.Image.ImagePoint[selectIndex] == null || !this.Data.Image.ImagePoint[selectIndex].hasOwnProperty('value') || this.Data.Image.ImagePoint[selectIndex].value == "")
                txtInput.val("");
            else
                txtInput.val(this.Data.Image.ImagePoint[selectIndex].value);
            if (this.Data.NominalList == null || this.Data.NominalList[selectIndex] == null) {
                $("#lblNominal").text("");
                $("#lblUpTol").text("");
                $("#lblLowTol").text("");
            }
            else {
                $("#lblNominal").text(this.Data.NominalList[selectIndex].Nominal);
                $("#lblUpTol").text(this.Data.NominalList[selectIndex].UpTol);
                $("#lblLowTol").text(this.Data.NominalList[selectIndex].LowTol);
            }
        },

        Submit: function () {
            var dom = this.Data.Dom;
            var selection = dom.head.find("select");
            var progressName = selection.find("option:selected").text();//工序
            var progressId = selection.find("option:selected").val();//工序ID
            var projectId = this.Data.ProjectID;//工厂ID
            var partId = this.Data.PartID;//零件ID
            var point = this.Data.Image.ImagePoint;
            var info = this.Data.NominalList;
            var pointcount = this.Data.Image.ImagePoint.length;
            var infocount = this.Data.NominalList.length;
            this.Post(progressId, progressName, projectId, partId, point, info, pointcount, infocount);
        },

        //上一个尺寸按钮
        ClickPrev: function () {
            var selectIndex = this.Data.PointIndex;
            if (this.Data.Dom.form.find("input[type='tel']").val() !== "")
                this.Data.Image.ImagePoint[selectIndex].value = this.Data.Dom.form.find("input[type='tel']").val();
            else {
                this.Data.Image.ImagePoint[selectIndex].value = "";
            }
            this.Data.PointIndex--;
            this.ShowInfo();
            this.ShowTooltip();
        },

        //下一个尺寸按钮
        ClickNext: function () {
            var selectIndex = this.Data.PointIndex;
            if (this.Data.Dom.form.find("input[type='tel']").val() !== "")
                this.Data.Image.ImagePoint[selectIndex].value = this.Data.Dom.form.find("input[type='tel']").val();
            else {
                this.Data.Image.ImagePoint[selectIndex].value = "";
            }
            this.Data.PointIndex++;
            this.ShowInfo();
            this.ShowTooltip();
        },

        Sure: function () {
            var selectIndex = this.Data.PointIndex;
            var _length = this.Data.Image.ImagePoint.length;
            if (this.Data.Dom.form.find("input[type='tel']").val() == "") {
                alert("指标数据不能为空！可以点击向下按钮跳过！");
            } else if (_length == 0) {
                alert("图片测点不能为空！");
            } else {
                this.Data.Image.ImagePoint[selectIndex].value = this.Data.Dom.form.find("input[type='tel']").val();
                if (selectIndex < _length - 1) {
                    this.Data.PointIndex++;
                }
                this.ShowInfo();
                this.ShowTooltip();
            }
        },
        //显示图片
        ShowImage: function () {
            imagePointCtrl.create(320, 320);

            imagePointCtrl.OnClickPoint = _clickPoint;
            imagePointCtrl.setImage(PointImage.Data.Image.ImageSource, PointImage.Data.Image.Width, PointImage.Data.Image.Height);//图片的base64,图片的宽度,图片的高度
        },

        //显示图片上的尺寸点
        ShowPoint: function () {
            var PointNXY = PointImage.Data.Image.ImagePoint;
            for (var index in PointNXY) {
                var pointInfo = new _.ImagePointCtrl.PointInfo(PointNXY[index].PointName, PointNXY[index].ImageX, PointNXY[index].ImageY);
                imagePointCtrl.addPoint(pointInfo);
            }
        },

        //显示图片上点的气泡
        ShowTooltip: function () {
            //图片点的位置、点的名字、点的数据
            var PointNXY = PointImage.Data.Image.ImagePoint;
            var index = this.Data.PointIndex;

            var _value = "";

            if (PointNXY == null || !PointNXY[index].hasOwnProperty("value") || PointNXY[index].value == "") _value = "";
            else _value = PointNXY[index].value;

            imagePointCtrl.removeTip();
            var tipInfo = new _.ImagePointCtrl.TipInfo(PointNXY[index].PointName, PointNXY[index].ImageX, PointNXY[index].ImageY, _value);
            imagePointCtrl.addTip(tipInfo);
        },

        FirstTooltip: function () {
            var PointNXY = PointImage.Data.Image.ImagePoint;
            var index = this.Data.PointIndex;
            var _value = "";
            if (PointNXY == null || PointNXY[index] == null || !PointNXY[index].hasOwnProperty('value') || PointNXY[index].value == "") _value = "";
            else _value = PointNXY[index].value;

            if (PointNXY[0] && PointNXY[0].PointName && PointNXY[0].ImageX && PointNXY[0].ImageY) {
                var tipInfo = new _.ImagePointCtrl.TipInfo(PointNXY[0].PointName, PointNXY[0].ImageX, PointNXY[0].ImageY, _value);
                imagePointCtrl.addTip(tipInfo);
            }
        },

        Post: function (progressId, progressName, projectId, partId, point, info, pointcount, infocount) {
            $.post("../Ajax/Handler.ashx", { Command: 32, ProgressId: progressId, ProgressName: progressName, ProjectId: projectId, PartId: partId, PointCount: pointcount, InfoCount: infocount, Point: point, Info: info }, function (data) {
                if (data.Status == "1")
                    alert("数据上传成功！");
                else
                    alert(data.Msg);
            }, "json");
        },

        GetValidValue: function () {
            var i = 0;
            var _ref = this.Data.Image.ImagePoint;
            for (var j = 0, _len = _ref.length; j < _len; j++) {
                if (_ref[j] && _ref[j].hasOwnProperty("value") && _ref[j].value != "") {
                    i++;
                }
            }
            return i;
        }
    }
    function GetRequest() {
        var url = location.search; //获取url中"?"符后的字串   
        var theRequest = new Object();
        if (url.indexOf("?") != -1) {
            var str = url.substr(1);
            strs = str.split("&");
            for (var i = 0; i < strs.length; i++) {
                theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
            }
        }
        return theRequest;
    }

    function _clickPoint(params) {
        var target = params.target;
        //alert(target.data.pointName);
        var div = document.getElementById("msg");
        div.innerHTML = target.data.pointName;
    }

    $(function () {
        $.ajaxSetup({
            async: false
        });

        var obj = GetRequest();
        var dom = [];

        dom.push($("#head"), $("#main"), $("#form"));
        PointImage.Init(obj, dom);
        PointImage.Render();

        $("#btnZoomIn")[0].onclick = imagePointCtrl.zoomIn;

        $("#btnZoomOut").bind("click", imagePointCtrl.zoomOut);

        $("#myModal").on('show.bs.modal', function (event) {
            var count = PointImage.GetValidValue();
            var modal = $(this);
            modal.find(".modal-body").html("您已经填写<strong>" + count + "</strong>个指标值，是否确定提交？");
        })
    })
})