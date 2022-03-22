mogiApp.directive('mgSocialShare', function () {
	return {
		restrict: 'E',
		scope: {
			svgUrl: '@'
		},
		controller: ['$scope', function ($scope) {
			$scope.Url = location.href;
			$scope.Lang = mogiDatas.Lang;
			$scope.facebook_id = mogiConst.Social.facebook_id;
			$scope.zalo_id = mogiConst.Social.zalo_id;
			$scope.svgFacebook = $scope.svgUrl + '#mi-facebook';
			$scope.svgMessenger = $scope.svgUrl + '#mi-messenger';
			function zalo() {
				var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
				po.src = 'https://sp.zalo.me/plugins/sdk.js';
				var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
			}
			(function activate() {
				if (window.zaloSDK) return;
				zalo();
				window.zaloSDK = true;
			})();
		}],
		replace: true,
		templateUrl: '/scripts/app/templates/social_share.html'
	};
});