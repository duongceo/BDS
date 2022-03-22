mogiApp.directive('mgCarousel', function () {
	return {
		scope: {
			options: '='
		},
		link: function (scope, el, atts) {
			el.owlCarousel(scope.options);
		}
	};
});
