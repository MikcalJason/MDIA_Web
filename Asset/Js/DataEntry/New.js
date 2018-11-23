require.config({
    baseUrl: "../Asset/Js",
    paths: {
        "jquery": "libs/jquery-3.3.1",
        "cookie": "libs/jquery.cookie",
        "validRight": "validRight"
    }
})
//require(["jquery"], function ($) {
require(["validRight", "jquery"], function (valid, $) {
    var domProject = $("#ddlst_Project");
    var domPart = $("#ddlst_Part");
    var domImgList = $("#imgWrap");

    var Project = {
        Data: {
            Dom: null,
            Option: [],
            SelectId: 0,
            Events: {
                OnChange: null
            }
        },
        Init: function (dom, func) {
            this.Data.Dom = dom;
            if (func != null)
                this.Data.Events.OnChange = func;
            this.Ajax();
        },
        Ajax: function () {
            var _self = this;
            _self.Data.Option.length = 0;
            $.post("../Ajax/Handler.ashx", { Command: 1 }, function (Info) {
                if (Info.Status == "1") {
                    var data = Info.arrTable[0];
                    $.each(data, function (i, item) {
                        _self.Data.SelectId = data[0].ProjectID;
                        _self.Data.Option.push(item);
                    })
                    _self.Render();
                    if (_self.Data.Events.OnChange != null)
                        _self.Data.Dom.bind("change", _self.Data.Events.OnChange);
                }
                else {
                    alert(Info.Msg);
                }
            }, "json");
        },
        Render: function () {
            this.Data.Dom.html();
            var OptData = this.Data.Option;
            var option = "";
            for (var i in OptData) {
                option += "<option value='" + OptData[i].ProjectID + "'>" + OptData[i].ProjectName + "</option>";
            }
            this.Data.Dom.html(option);
        }
    };

    var Part = {
        Data: {
            Dom: null,
            Option: [],
            ParentId: "",
            SelectId: "",
            Events: {
                OnChange: null
            }
        },
        Init: function (dom, id, func) {
            if (func != null)
                this.Data.Events.OnChange = func;
            this.Data.ParentId = id;
            this.Data.Dom = dom;
            this.Ajax();
        },
        Ajax: function () {
            var _self = this;
            _self.Data.Option.length = 0;
            $.post("../Ajax/Handler.ashx", { Command: 2, ProjectID: _self.Data.ParentId }, function (Info) {
                if (Info.Status == "1") {
                    var data = Info.arrTable[0];
                    $.each(data, function (i, item) {
                        _self.Data.SelectId = data[0].PartID;
                        _self.Data.Option.push(item);
                    })
                    _self.Render();

                    if (_self.Data.Events.OnChange != null)
                        _self.Data.Dom.bind("change", _self.Data.Events.OnChange);//$.proxy(_self.OnChange(val), _self)
                }
                else {
                    alert(Info.Msg);
                }
            }, "json");
        },
        Render: function () {
            this.Data.Dom.html();
            var OptData = this.Data.Option;
            var option = "";
            for (var i in OptData) {
                option += "<option value='" + OptData[i].PartID + "'>" + OptData[i].PartName + "</option>";
            }
            this.Data.Dom.html(option);
        }
    };
    var Image = {
        Data: {
            Dom: null,
            ImgList: [],
            ProjectID: "",
            PartID: ""
        },

        Init: function (dom, proid, partid) {
            if (dom != null)
                this.Data.Dom = dom;
            if (proid != null)
                this.Data.ProjectID = proid;
            if (partid != null)
                this.Data.PartID = partid;
            this.Ajax();
        },
        Ajax: function () {
            var _self = this;
            _self.Data.ImgList.length = 0;
            $.post("../Ajax/Handler.ashx", { Command: 10, ProjectID: _self.Data.ProjectID, PartID: _self.Data.PartID }, function (Info) {
                if (Info.Status == "1") {
                    var data = Info.arrTable[0];
                    $.each(data, function (i, item) {
                        _self.Data.ImgList.push(item);
                    })
                    _self.Render();
                } else {
                    alert(Info.Msg);
                }
            }, "json");
        },
        Render: function () {
            this.Data.Dom.html();
            var imgList = this.Data.ImgList;
            var parent = this.Data;
            var html = "<div class='row' >";
            var i;
            for (i = 0; i < imgList.length;) {
                html += this.Templet.OneImg + "<a href='./NewImagePoint.html?proid=" + parent.ProjectID + "&partid=" + parent.PartID + "&imgid=" + imgList[i].ImageID + "'><img src='data:image/png;base64," + imgList[i].ImageData + "' class='img-thumbnail'></img></a>";
                i = i + 1;
                if (i < imgList.length)
                    html += this.Templet.TwoImg + "<a href='./NewImagePoint.html?proid=" + parent.ProjectID + "&partid=" + parent.PartID + "&imgid=" + imgList[i].ImageID + "'><img src='data:image/png;base64," + imgList[i].ImageData + "' class='img-thumbnail'></img></a>";
                i = i + 1;
                if (i < imgList.length)
                    html += this.Templet.ThreeImg + "<a href='./NewImagePoint.html?proid=" + parent.ProjectID + "&partid=" + parent.PartID + "&imgid=" + imgList[i].ImageID + "'><img src='data:image/png;base64," + imgList[i].ImageData + "' class='img-thumbnail'></img></a>" + this.Templet.Footer;
                i = i + 1;
            }
            html += "</div>"
            this.Data.Dom.html(html);
        },
        Templet: {
            OneImg: "<div class='col-xs-12' style='margin:5px;'><div class='col-xs-4' style='margin:0 -5px;'>",
            TwoImg: "</div><div class='col-xs-4' style='margin:0 -5px;'>",
            ThreeImg: "</div><div class='col-xs-4' style='margin:0 -5px;'>",
            Footer: "</div></div>"
        }
    };

    $(function () {
        $.ajaxSetup({
            async: false
        });

        $("#loading").ajaxSuccess(function () {
            $(this).html("");
        });

        function _partChange(params) {
            var target = params.target;
            Part.Data.SelectId = target.options[target.selectedIndex].value;//记录零件Id
            Image.Init(domImgList, Project.Data.SelectId, Part.Data.SelectId);
        }

        function _projectChange(params) {
            var target = params.target;
            Project.Data.SelectId = target.options[target.selectedIndex].value;//记录项目Id
            Part.Init(domPart, Project.Data.SelectId);
            Image.Init(domImgList, Project.Data.SelectId, Part.Data.SelectId);
        }

        Project.Init(domProject, _projectChange);//Dom,点击事件

        Part.Init(domPart, Project.Data.SelectId, _partChange);//Dom,项目ID,点击事件

        Image.Init(domImgList, Project.Data.SelectId, Part.Data.SelectId);//Dom,项目ID,零件ID
    })
})