mogiApp.directive('mogiNearBy', function () {
	return {
		restrict: 'E',
		scope: {
			ShowNearBy: '=showNearBy',
			Location: '=location',
			Title: '=title',
			ShowMap: '=showMap',
			IsResponsive: '=isResponsive'
			//DisplayMap: '&displayMap',
			//HidenMap: '&hidenMap'
		},
		link: function ($scope, $element, $attrs) {
			$scope.DisplayMap = $scope.ShowMap;
			//$scope.IsResponsive = mogiUtils.IsResponsive_Max768();
			$scope.NearBy = {
				Items: mogiResUtils.syscodeUtils.getGoogleNearBy(),
				AddNearBy: function (type) {
					for (var i = 0; i <= this.Items.length - 1; i++) {
						var obj = this.Items[i];
						if (obj.Checked == true) {
							if (obj.c == type) {
								mogiMapUtil.AddNearBy(obj.c);
							}
						} else {
							mogiMapUtil.clearMarkers(obj.c);
						}
					}
				}
			};
			$scope.Lables = {
				Close: mogiDatas.Msg.GoogleMap_Return,
				Header: mogiDatas.Msg.GoogleMap_Header,
				Footer: mogiDatas.Msg.GoogleMap_Footer,
			};
			//$scope.DisplayMap = function () {
			//    alert('display');
			//    $scope.ShowMap = true;
			//    Active();
			//};
			//$scope.HidenMap = function () {
			//    alert('hide');
			//    $scope.ShowMap = false;
			//};
			function Active() {
				mogiMapUtil.InitMap("map-cavas", $scope.Location, $scope.Title, $scope.Title, 16);
			}
			$scope.NearByRespon = {
				Show: false,
				Open: function () {
					this.Show = !this.Show;
				},
				Css: function () {
					if (this.Show == true) {
						return "fa fa-arrow-circle-up";
					}
					return "fa fa-arrow-circle-down";
				}
			};
			var mapElementId = $('#mgGoogleMap');
			var mapavas = $('#map-cavas');
			$scope.$watch("IsResponsive", function (newVal, oldVal) {
				if (newVal == true) {
					mapElementId.addClass('map-fullscreen');
					mapavas.addClass('');
				}
				else {
					mapElementId.removeClass('map-fullscreen');
				}
			});
			$scope.CloseMap = function () {
				if ($scope.IsResponsive == true) {
					$scope.ShowMap = false;
				}
			}
			//var width = $(window).width();
			//if (width < 768) {
			//    Active();
			//}
			$scope.$watch('ShowMap', function (nVal, oVal) {
				if (nVal == true) {
					Active();
					console.log("MapInit");
				}
			});
			// ng-show="{{ShowNearBy && IsResponsive==true}}"
		},
		template: ''
			+ '<div class="clearfix" style=" width:100%" ng-show="ShowMap" id="mgGoogleMap">'
			+ '<div class="map-header clearfix" ng-show="IsResponsive"><span class="close-map" ng-click="CloseMap()">{{Lables.Close}}</span> {{Lables.Header}}</div>'

			+ '<div id="map-cavas" class="map-display" class="clearfix"></div>'
			+ '<div class="map-bottom clearfix" ng-show="IsResponsive"> <div class="open-nearby" >{{Lables.Footer}} <span ng-click="NearByRespon.Open()"><i ng-class="NearByRespon.Css()"></i></span></div>'
			+ '<div class="near-items clearfix" ng-show="NearByRespon.Show">'
			+ '<ul><li ng-repeat="item in NearBy.Items" class="checkbox near-item"><label><input type="checkbox" ng-click="NearBy.AddNearBy(item.c)" ng-model="item.Checked" />{{item.n}}</label></li>'
			+ '</ul></div>'
			+ '</div>'
			+ '<div class="near-items clearfix" ng-show="ShowNearBy && IsResponsive ==false">'
			+ '<ul>'
			+ '<li ng-repeat="item in NearBy.Items" class="checkbox near-item">'
			+ '<label><input type="checkbox" ng-click="NearBy.AddNearBy(item.c)" ng-model="item.Checked" />{{item.n}}</label>'
			+ '</li>'
			+ '</ul>'
			+ '</div>'
			+ '</div>'
	};
});