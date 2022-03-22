mogiApp.directive('lazyScroll', ['$rootScope', '$window', function ($rootScope, $window) {
	return {
		//restrict: 'E',
		//scope: {lazyDataUrl: '@'},
		//replace: true,
		link: function (scope, elem, attrs) {
			var lazyLoadData, scrollTrigger = .90, scrollEnabled = true;
			$window = angular.element($window);
			if (attrs.lazyNoScroll !== null) {
				scope.$watch(attrs.lazyNoScroll, function (value) {
					scrollEnabled = value === true ? false : true;
				});
			}

			if ((attrs.lazyScrollTrigger !== undefined) && (attrs.lazyScrollTrigger > 0 && attrs.lazyScrollTrigger < 100)) {
				scrollTrigger = attrs.lazyScrollTrigger / 100;
			}

			lazyLoadData = function () {
				var wintop = window.pageYOffset;
				var docHeight = window.document.body.clientHeight;
				var windowHeight = window.innerHeight;
				var triggered = (wintop / (docHeight - windowHeight));
				var elemtop = elem[0].offsetTop;
				if ((scrollEnabled) && (triggered >= scrollTrigger || elemtop <= wintop + 100)) {
					return scope.$apply(attrs.lazyScroll);
				}
			};

			$window.on('scroll', lazyLoadData);
			scope.$on('$destroy', function () {
				return $window.off('scroll', lazyLoadData);
			});
		}
	};
}]);