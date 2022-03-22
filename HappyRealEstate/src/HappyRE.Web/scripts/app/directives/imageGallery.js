mogiApp.directive('imageGallery', function () {
	return {
		scope: {
			topmedia: '=',
			listmedia: '='
		},
		link: function (scope, el, atts) {

			var lock = false;
			var s1 = el.find(scope.topmedia.element);
			//scope.topmedia.options['onInitialized'] = function () { reset_video_size(null, s1); };
			//scope.topmedia.options['onResized'] = function () { reset_video_size(null, s1); };

			s1.owlCarousel(scope.topmedia.options);

			var s2 = el.find(scope.listmedia.element);

			//scope.listmedia.options['onInitialized'] = function () { reset_video_size(null, s2); };
			//scope.listmedia.options['onResized'] = function () { reset_video_size(null, s2); };
			s2.owlCarousel(scope.listmedia.options);

			s2.on('click', '.owl-item', function () {
				s1.trigger('to.owl.carousel', [$(this).index(), 0, true]);
				addActivedMedia(this);

			});
			s2.on('changed.owl.carousel', function (e) {
				if (!lock) {
					lock = true;
					s1.trigger('to.owl.carousel', [e.item.index, 0, true]);
					var active = s2.find(".owl-item")[e.item.index];
					addActivedMedia(active);
					console.log(e.item.index);
					lock = false;
				}
			});

			var addActivedMedia = function (element) {
				$('.selected-media').removeClass('selected-media');
				$(element).addClass('selected-media');
			};
		}
	};

    /// Fix Video Thumbnail Image not display
    /// More Info: https://github.com/OwlCarousel2/OwlCarousel2/issues/112
    function reset_video_size(video_width, selector) {
        //better use jquery selectors: owl.items() and $(owl.items()) give problems, don't know why
        // var items = $('.owl-item:not([data-video])');
        // var videos = $('.owl-video-wrapper');
        var items = selector.find('.owl-item:not([data-video])');
        var videos = selector.find('.owl-video-wrapper');
        var v_height = 0;

        //user-defined width ELSE, width from inline css (when owl.autoWidth == false), 
        //ELSE, computed innerwidth of the first element.
        var v_width = (video_width) ? video_width : ((items.css('width') != 'auto') ? items.css('width') : items.innerWidth());

        items.each(function () {
            var h = $(this).innerHeight();
            if (h > v_height) v_height = h;
        });

        //set both width and height
        //videos.css({ 'height': v_height, 'width': v_width });
    }
});