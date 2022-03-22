mogiApp.directive('mgToggleText', function(){
	return {
		scope: {
			showchars: '=',
			readmoretext: '=',
			readlesstext: '=',
			ellipsis: '='
		},
		replace: true,
		transclude: true,
		link: function (scope, el, atts, ctrl, transclude) {
		},
		controller: function ($scope, $transclude) {
			$scope.linkLable = $scope.readmoretext;
			$scope.isShow = false;
			$scope.showText = '';
			$scope.hiddentext = '';
			$scope.isTruncate = false;

			$transclude(function (clone, scope) {

				$scope.text = clone.text().trim();
			});

			(function truncateText(showChar, text) {
				if (text.length <= showChar) {
					$scope.showText = text;
				} else {
					var lastShowIndex = text.substring(0, showChar).lastIndexOf(" ");
					$scope.showText = text.substring(0, lastShowIndex);
					$scope.hiddentext = text.substring(lastShowIndex);
					$scope.isTruncate = true;
				}

			})($scope.showchars, $scope.text);

			$scope.toggleText = function () {
				if ($scope.isShow) {
					$scope.linkLable = $scope.readmoretext;

				} else {
					$scope.linkLable = $scope.readlesstext;
				}
				$scope.isShow = !$scope.isShow;
			};

		},
		template: '<span>{{showText}}<span ng-show="!isShow && $scope.isTruncate">{{ellipsis}}</span><span ng-show="isShow" class="animate-show-hide">{{hiddentext}}</span> <a  ng-show="isTruncate" href="" ng-click="toggleText()"><b>{{ linkLable}}</b></a></span>'
	};
});