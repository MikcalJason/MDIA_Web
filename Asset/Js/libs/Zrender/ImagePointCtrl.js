define(["zrender"], function (zrender) {
    function ImagePointCtrl(canvasID) {
        var _zr;
        var _group;
        var _pageZoom = 1.0;
        var _pageWidth = 800;
        var _pageHeight = 600;
        var _imageInfo = null;
        var _pointInfo = [];

        this.OnClickPoint = null;
        this.OnClickImage = null;
        var _self = this;
        this.create = function (pageWidth, pageHeight) {
            //页面大小
            _pageWidth = pageWidth;
            _pageHeight = pageHeight;
            _zr = zrender.init(document.getElementById(canvasID));

            var canvasWidth = _zr.getWidth();
            var canvasHeight = _zr.getHeight();
            var pagePos = [1, 1];
            var pageScale = [1, 1];
            var pageZoom = _fitZoom(_pageWidth, _pageHeight, canvasWidth, canvasHeight) * 0.99;

            pagePos[0] = (canvasWidth - _pageWidth * pageZoom) / 2.0;
            pagePos[1] = (canvasHeight - _pageHeight * pageZoom) / 2.0;

            pageScale[0] = pageZoom;
            pageScale[1] = pageZoom;

            _pageZoom = pageZoom;

            _group = new zrender.Group({
                position: pagePos
            });

            var deltPosition = null;
            var Draggroup = null;

            _zr.on('mouseup', function (e) {
                Draggroup = null;
            });

            _group.on('mousedown', function (e) {
                Draggroup = _group;
                deltPosition = [e.event.zrX - Draggroup.position[0], e.event.zrY - Draggroup.position[1]];
            });

            _zr.on('mousemove', function (e) {
                if (Draggroup != null) {
                    var new_pos = [e.event.zrX, e.event.zrY];
                    Draggroup.position = [new_pos[0] - deltPosition[0], new_pos[1] - deltPosition[1]];

                    Draggroup.dirty();
                }
            });

            _zr.add(_group);
        }

        this.setImage = function (imageURI, imageWidth, imageHeight) {
            var imageX = (_pageWidth - imageWidth) / 2.0;
            var imageY = (_pageHeight - imageHeight) / 2.0;

            var imagePart = new zrender.Image({
                scale: [1, 1],
                style: {
                    image: "data:image/png;base64," + imageURI,
                    x: imageX,
                    y: imageY,
                    width: imageWidth,
                    height: imageHeight
                },
                cursor: 'Move',
                draggable: false,
            });
            _group.add(imagePart);

            if (_imageInfo == null)
                _imageInfo = new ImagePointCtrl.ImageInfo(imageX, imageY, imageWidth, imageHeight);
        }

        this.addPoint = function (pointInfo) {
            var pointX = _imageInfo.x + pointInfo.x;
            var pointY = _imageInfo.y + pointInfo.y;
            var chartX = _imageInfo.x + pointInfo.chartX;
            var chartY = _imageInfo.y + pointInfo.chartY;
            _pointInfo.push(pointInfo);
            var gridSize = [0, 0];

            var circle = new zrender.Circle({
                scale: [1, 1],
                shape: {
                    cx: pointX,
                    cy: pointY,
                    r: 3
                },
                style: {
                    fill: 'red',
                    stroke: 'red'
                },
                cursor: 'pointer',
                data: pointInfo,
                draggable: false
            });
            if (this.OnClickPoint != null)
                circle.onclick = this.OnClickPoint;
            _group.add(circle);

        }

        this.setZoom = function (zoom, centerView) {
            var pagePos = _group.position;

            if (centerView) {
                //计算居中位置
                var canvasWidth = _zr.getWidth();
                var canvasHeight = _zr.getHeight();

                pagePos[0] = (canvasWidth - _pageWidth * zoom) / 2.0;
                pagePos[1] = (canvasHeight - _pageHeight * zoom) / 2.0;
            }
            else {
                //计算缩放后的位置
                var curPageWidth = _pageWidth * _pageZoom;
                var curPageHeight = _pageHeight * _pageZoom;

                var newPageWidth = _pageWidth * zoom;
                var newPageHeight = _pageHeight * zoom;

                pagePos[0] += (curPageWidth - newPageWidth) / 2.0;
                pagePos[1] += (curPageHeight - newPageHeight) / 2.0;
            }

            _group.attr('position', pagePos);
            _group.attr('scale', [zoom, zoom]);

            _pageZoom = zoom;

            _zr.painter.refresh();
        }

        this.fitAll = function () {
            var canvasWidth = _zr.getWidth();
            var canvasHeight = _zr.getHeight();

            var zoom = _fitZoom(_pageWidth, _pageHeight, canvasWidth, canvasHeight) * 0.99;
            this.setZoom(zoom, true);
        }

        //放大
        this.zoomIn = function () {
            var zoom = _pageZoom * 0.9;
            _self.setZoom(zoom, false);
        }

        //缩小
        this.zoomOut = function () {
            var zoom = _pageZoom * 1.1;
            _self.setZoom(zoom, false);
        }

        this.autoSize = function () {
            _self.setZoom(1.0, true);
        }

        this.addTip = function (tipInfo) {
            var tipX = _imageInfo.x + tipInfo.x;
            var tipY = _imageInfo.y + tipInfo.y;
            var tooltip = new ImagePointCtrl.ToolTip();
            var imagePointPin = new tooltip.Pin({
                shape: {
                    x: tipX,
                    y: tipY,
                    width: 50,
                    height: 46
                },
                style: {
                    text: tipInfo.value,
                    textFill: '#fff',
                    fill: '#18BE9B',//#00B7EE
                    stroke: '#000'
                },
                hoverable: true,
                draggable: false
            });
            imagePointPin.tipInfo = tipInfo;
            _group.add(imagePointPin);
        }

        this.removeTip = function () {
            var count;
            var el;
            var index;
            count = _group.childCount()
            if (count > 0) {
                index = count - 1;
                el = _group.childAt(index);
                _group.remove(el);
            }
        }

        function _fitZoom(width, height, maxWidth, maxHeight) {
            var fZoom = 1.0;

            if (width / height > maxWidth / maxHeight)
                fZoom = maxWidth / width;
            else
                fZoom = maxHeight / height;

            return fZoom;
        }
    }

    ImagePointCtrl.ImageInfo = function (x, y, width, height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    ImagePointCtrl.PointInfo = function (pointName, x, y) {
        this.pointName = pointName;
        this.x = x;
        this.y = y;
    }

    ImagePointCtrl.TipInfo = function (pointName, x, y, value)
    {
        this.pointName = pointName;
        this.x = x;
        this.y = y;
        this.value = value;
    }

    ImagePointCtrl.ToolTip = function () {
        this.Pin = zrender.Path.extend({
            type: 'pin',
            shape: {
                // x, y on the cusp
                x: 0,
                y: 0,
                width: 0,
                height: 0
            },
            buildPath: function (path, shape) {
                var x = shape.x;
                var y = shape.y;
                var w = shape.width / 5 * 3;
                // Height must be larger than width
                var h = Math.max(w, shape.height);
                var r = w / 2;
                // Dist on y with tangent point and circle center
                var dy = r * r / (h - r);
                var cy = y - h + r + dy;
                var angle = Math.asin(dy / r);
                // Dist on x with tangent point and circle center
                var dx = Math.cos(angle) * r;
                var tanX = Math.sin(angle);
                var tanY = Math.cos(angle);
                path.arc(
                    x, cy, r,
                    Math.PI - angle,
                    Math.PI * 2 + angle
                );
                var cpLen = r * 0.6;
                var cpLen2 = r * 0.7;
                path.bezierCurveTo(
                    x + dx - tanX * cpLen, cy + dy + tanY * cpLen,
                    x, y - cpLen2,
                    x, y
                );
                path.bezierCurveTo(
                    x, y - cpLen2,
                    x - dx + tanX * cpLen, cy + dy + tanY * cpLen,
                    x - dx, cy + dy
                );
                path.closePath();
            }
        })
    }

    return {
        ImagePointCtrl: ImagePointCtrl
    }
})