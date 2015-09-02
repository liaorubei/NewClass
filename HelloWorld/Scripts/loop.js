
/*
简单图片轮循jQuery插件,因为多个页面都要到,所以制成插件,要求配合loop.css使用
liaorubei,20150729
*/
(function ($) {

    //默认属性
    var defaults = {
        srcs: [], urls: []
    };

    $.fn.extend({
        loop: function (options) {
            var opts = $.extend(defaults, options);
            var imgs = $("<ul class='imgs'></ul>");
            var ctrl = $("<ul class='ctrl'></ul>");
            var timer;
            var index = 0;
            var count = opts.srcs.length;

            //轮循图片地址
            for (var i = 0; i < opts.srcs.length; i++) {

                imgs.append("<li><a href='" + opts.urls[i] + "'><img src='" + opts.srcs[i] + "' alt='" + (i + 1) + "' /></a></li>");
                ctrl.append("<li>" + (i + 1) + "</li>");
            }
            imgs.appendTo(this);
            ctrl.appendTo(this);

            var show = function (i) {
                //先取得图片的高度,因为图片外面还有一个li,所以也包括li的高度,否则将会有一点点误差
                var h = $("li", imgs).height();

                //更改指定控制按钮的背景色
                $("li", ctrl).eq(i).addClass("on").siblings().removeClass("on");

                //同时根据指定控制按钮的索引来更改图片的显示和隐藏
                imgs.stop(true, false).animate({ "marginTop": -(h * i) + "px" }, 1000);
            };

            //鼠标悬停
            $("li", ctrl).mouseover(function () {
                var i = $(this).index();//不传递参数，返回这个元素在同辈中的索引位置。
                show(i);
            });

            //自动轮循,并支持链式反应
            return this.hover(function () { clearInterval(timer); }, function () { timer = setInterval(function () { show(index); index++; if (index == count) { index = 0; } }, 2500); }).trigger("mouseleave");
        }
    });
})(jQuery);