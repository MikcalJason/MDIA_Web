require(
	[
		'zrender',
		'zrender/shape/Circle',
		'zrender/shape/Rectangle',
		'zrender/shape/Image',
		'zrender/shape/Line'
	]);
function ImagePointCtrl(canvasID) {
    var CircleShape;
    var RectangleShape;
    var ImageShape;
    var BaseShape;
    var LineShape;
    var _zr;
    var _pageZoom = 1.0;
    var _pageWidth = 800;
    var _pageHeight = 600;
    var _imageInfo = null;
    var _pointInfo = [];

    this.OnClickPoint = null;
    this.OnClickImage = null;

    /*
		创建控件
		- pageWidth  页面宽度
		- pageHeight 页面高度
	*/
    this.create = function (pageWidth, pageHeight) {
        var zrender = require('zrender');
        CircleShape = require('zrender/shape/Circle');
        RectangleShape = require('zrender/shape/Rectangle');
        ImageShape = require('zrender/shape/Image');
        BaseShape = require('zrender/shape/Base');
        LineShape = require('zrender/shape/Line');

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

        //		 var pageShape = new RectangleShape({
        //			zlevel : 1,
        //			style: {
        //				x: 0,
        //				y: 0,
        //				width:pageWidth,
        //				height:pageHeight,
        //				color:'#FFFECC'
        //				},
        //			hoverable: false
        //		});
        //		_zr.addShape(pageShape);

        _zr.modLayer(1, {
            panable: true,
            zoomable: true,
            scale: pageScale,
            position: pagePos
        });

        _zr.render();
    }

    /*
		设置图片
		- imageUrl    图片路径
		- imageWidth  图片原始宽度
		- imageHeight 图片原始高度
	*/
    this.setImage = function (imageUrl, imageWidth, imageHeight) {
        //将图片在页面中居中
        var imageX = (_pageWidth - imageWidth) / 2.0;
        var imageY = (_pageHeight - imageHeight) / 2.0;

        var imagePart = new ImageShape({
            zlevel: 1,
            style: {
                image: imageUrl,
                x: imageX,
                y: imageY,
                width: imageWidth,
                height: imageHeight
            },
            hoverable: false
        });
        if (this.OnClickImage != null)
            imagePart.onclick = this.OnClickImage;
        _zr.addShape(imagePart);

        if (_imageInfo == null)
            _imageInfo = new ImagePointCtrl.ImageInfo(imageX, imageY, imageWidth, imageHeight);
    }

    /*
		增加测点
		- pointName    测点名称
		- x, y         点在图片上的坐标
	*/
    this.addPoint = function (pointInfo) {
        var pointX = _imageInfo.x + pointInfo.x;
        var pointY = _imageInfo.y + pointInfo.y;
        var chartX = _imageInfo.x + pointInfo.chartX;
        var chartY = _imageInfo.y + pointInfo.chartY;

        _pointInfo.push(pointInfo);

        var gridSize = [0, 0];
        var line = null;

        if (pointInfo.items.length > 0) {
            var itemHeight = 20;
            var itemKeyWidth = 70;
            var itemValueWidth = 100;

            gridSize[0] = itemKeyWidth + itemValueWidth;
            gridSize[1] = itemHeight * pointInfo.items.length;

            var gridCenter = [0, 0];

            gridCenter[0] = chartX + gridSize[0] / 2;
            gridCenter[1] = chartY + gridSize[1] / 2;

            line = new LineShape({
                zlevel: 1,
                style: {
                    xStart: pointX,
                    yStart: pointY,
                    xEnd: gridCenter[0],
                    yEnd: gridCenter[1],
                    strokeColor: '#000',
                    lineWidth: 1
                }
            });
            _zr.addShape(line);
        }

        var circle = new CircleShape({
            zlevel: 1,
            style: {
                x: pointX,
                y: pointY,
                r: 3,
                brushType: 'both',
                color: 'red',
                strokeColor: 'red',
                lineWidth: 1
            },
            hoverable: false,
            clickable: true,
            data: pointInfo
        });
        if (this.OnClickPoint != null)
            circle.onclick = this.OnClickPoint;
        _zr.addShape(circle);

        if (pointInfo.items.length > 0) {
            var curLayer = _zr.painter.getLayer(1);
            var imagePointGrid = new ImagePointCtrl.ImagePointGrid({
                zlevel: 1,
                style: {
                    x: chartX,
                    y: chartY,
                    width: gridSize[0],
                    height: gridSize[1],
                    color: 'red',
                    strokeColor: 'red',
                    lineWidth: 1
                },
                hoverable: false,
                draggable: true,
                data: line,
                layer: curLayer,
                ondragend: function (params) {
                    var line = this.data;
                    line.style.xEnd = this.style.x + this.style.width / 2;
                    line.style.yEnd = this.style.y + this.style.height / 2;
                    _zr.modShape(line.id, line);
                    _zr.refresh();
                }
            });
            imagePointGrid.pointInfo = pointInfo;
            /* var imagePointGrid = new RectangleShape({
					zlevel : 1,
					style: {
						x: chartX,
						y: chartY,
						width:200,
						height:100,
						color: 'red',
						strokeColor: 'red',
						lineWidth: 1
					},
					hoverable: false,
					draggable: true,
					data: pointInfo
			}); */
            _zr.addShape(imagePointGrid);
        }

        return pointInfo;
    }

    /*
		设置视图缩放比例
		-zoom       缩放比
		-centerView 是否居中
	*/
    this.setZoom = function (zoom, centerView) {
        var layer = _zr.painter.getLayer(1);
        if (layer.zoomable) {
            layer.__zoom = layer.__zoom || 1;
            var pagePos = layer.position;

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

            layer.position[0] = pagePos[0];
            layer.position[1] = pagePos[1];
            layer.scale[0] = zoom;
            layer.scale[1] = zoom;
            layer.dirty = true;
        }

        _pageZoom = zoom;

        _zr.painter.refresh();
    }

    //适合视图
    this.fitAll = function () {
        var canvasWidth = _zr.getWidth();
        var canvasHeight = _zr.getHeight();

        var zoom = _fitZoom(_pageWidth, _pageHeight, canvasWidth, canvasHeight) * 0.99;
        this.setZoom(zoom, true);
    }

    //缩小
    this.zoomIn = function () {
        var zoom = _pageZoom * 0.9;
        this.setZoom(zoom, false);
    }

    //放大
    this.zoomOut = function () {
        var zoom = _pageZoom * 1.1;
        this.setZoom(zoom, false);
    }

    //原始大小
    this.autoSize = function () {
        this.setZoom(1.0, true);
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

//Point类
var Point = function (x, y) {
    this.x = x;
    this.y = y;
}

//Rect类
var Rect = function (x, y, width, height) {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;

    this.getRight = function () {
        return this.x + this.width;
    }

    this.getBottom = function () {
        return this.y + this.height;
    }

    this.isIntersect = function (rect) {
        return !(rect.x >= this.getRight() ||
				 rect.getRight() <= this.x ||
				 rect.y >= this.getBottom() ||
				 rect.getBottom() <= this.y);
    }

    this.containsPoint = function (x, y) {
        return x >= this.x && x < this.getRight() && y >= this.y && y < this.getBottom();
    }
}

//图片信息类
ImagePointCtrl.ImageInfo = function (x, y, width, height) {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
}

ImagePointCtrl.PointItem = function (key, value, color) {
    this.key = key;
    this.value = value;
    this.color = color;
}

//测点信息类
ImagePointCtrl.PointInfo = function (pointName, x, y, chartX, chartY) {
    this.pointName = pointName;
    this.x = x;
    this.y = y;
    this.chartX = chartX;
    this.chartY = chartY;
    this.items = [];

    this.addItem = function (key, value, color) {
        var item = new ImagePointCtrl.PointItem(key, value, color);
        this.items.push(item);
    }
}

/*
	点表格控件
*/
ImagePointCtrl.ImagePointGrid = function (options) {
    Base = require('zrender/shape/Base');
    Base.call(this, options);

    this.type = 'ImagePointGrid';
    this.itemHeight = 20;
    this.itemKeyWidth = 70;
    this.itemValueWidth = 100;
    this.pointInfo = null;

    this.brush = function (ctx, isHighlight) {
        if (this.pointInfo == null)
            return;

        var style = this.style || {};

        ctx.save();
        ctx.beginPath();

        var charSize = ctx.measureText('A');
        var charWidth = charSize.width;
        var keyCharCount = this.itemKeyWidth / charWidth;
        var valueCharCount = this.itemValueWidth / charWidth;

        var left = this.style.x;
        var top = this.style.y;
        var y = top;

        ctx.strokeStyle = style.strokeColor || style.color;
        ctx.strokeStyle = 'black';
        //ctx.font = '12px 宋体';

        var width = this.itemKeyWidth + this.itemValueWidth;
        var textTop = y + this.itemHeight / 2 + charWidth / 2;

        var pointName = this.pointInfo.pointName;
        var pointNameCharCount = width / charWidth;
        if (pointName.length > pointNameCharCount)
            pointName = pointName.substring(0, pointNameCharCount);
        //pointName = _getText(ctx, pointName, width);

        //绘制测点名称
        ctx.fillStyle = 'white';
        ctx.fillRect(left, y, width, this.itemHeight);
        ctx.strokeRect(left, y, width, this.itemHeight);
        ctx.fillStyle = 'black';
        ctx.fillText(pointName, left + 1, textTop);
        y += this.itemHeight;

        for (var i = 0; i < this.pointInfo.items.length; i++) {
            var pointItem = this.pointInfo.items[i];
            var keyText = pointItem.key;
            var valueText = pointItem.value;
            textTop = y + this.itemHeight / 2 + charWidth / 2;

            if (keyText.length > keyCharCount)
                keyText = keyText.substring(0, keyCharCount);

            if (valueText.length > valueCharCount)
                valueText = valueText.substring(0, valueCharCount);

            //绘制关键字
            ctx.fillStyle = 'white';
            ctx.fillRect(left, y, this.itemKeyWidth, this.itemHeight);
            ctx.strokeRect(left, y, this.itemKeyWidth, this.itemHeight);
            ctx.fillStyle = 'black';
            ctx.fillText(keyText, left + 1, textTop);

            //绘制值
            ctx.fillStyle = pointItem.color;
            ctx.fillRect(left + this.itemKeyWidth, y, this.itemValueWidth, this.itemHeight);
            ctx.strokeRect(left + this.itemKeyWidth, y, this.itemValueWidth, this.itemHeight);
            ctx.fillStyle = 'black';
            ctx.fillText(valueText, left + this.itemKeyWidth + 1, textTop);

            y += this.itemHeight;
        }

        ctx.closePath();
        ctx.stroke();
        ctx.restore();
    }

    function _getText(ctx, text, maxWidth) {
        var totalWidth = 0;
        var charCount = 0;

        for (var i = 0; i < text.length; i++) {
            var charSize = ctx.measureText(text[i]);
            if (totalWidth < maxWidth) {
                charCount++;
                totalWidth += charSize.width;
            }
            else
                break;
        }

        if (charCount == text.length) {
            return text;
        }
        else {
            return text.substr(0, charCount);
        }
    }

    this.drift = function (dx, dy) {
        dx = dx / this.layer.scale[0];
        dy = dy / this.layer.scale[1];

        this.style.x += dx;
        this.style.y += dy;
    }

    this.isCover = function (x, y) {
        var originPos = this.transformCoordToLocal(x, y);
        x = originPos[0];
        y = originPos[1];

        if (x >= this.style.x
			&& x <= (this.style.x + this.style.width)
			&& y >= this.style.y
			&& y <= (this.style.y + this.style.height)
		) {
            return true;
        }
        return false;
    }
}

require('zrender/tool/util').inherits(ImagePointCtrl.ImagePointGrid, require('zrender/shape/Base'));