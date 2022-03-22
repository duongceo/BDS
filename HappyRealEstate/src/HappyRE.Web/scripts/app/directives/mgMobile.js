mogiApp.directive('ngMobile', function () {
	return {
		restrict: 'E',
		replace: true,
		scope: {
			mobile: '@'
		},
		controller: function ($scope) {
			$scope.Show = function () {
				$scope.value = $scope.mobile;
			};
			var n = $scope.mobile;
			n = n.substring(0, n.length - 2) + 'xx';
			$scope.value = n;
			console.log(n);
		},
		template: '<span ng-bind="value" ng-click="Show()"></span>'
	};
});