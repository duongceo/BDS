var projectsController = function ($scope, suggest) {
	// variables
	$scope.Keyword = '';
	$scope.Id = '';

	// public functions
	$scope.getProjectSuggest = function (q) {
		var url = mogiRoutes.Common.Suggest_Project;
		return suggest.GetSuggestion(q, url);
	};
	$scope.KeywordOnSelected = function () {
		$scope.searchProject();
	};
	$scope.searchProject = function () {
		var url = getKeyword();
		window.location.href = url;
	};
	$scope.PhoneFormat = function (num) {
		var phone = num.replace(/(\d{1,100})(\d{3})(\d{3})/, '$1 $2 $3');
		return phone;
	};
	$scope.PriceFormat = function (min, max, minArea) {
		if (min > 0) {
			if (max > 0) {
				var f1 = ((min % 1000000) > 0 ? 1 : 0);
				var f2 = ((max % 1000000) > 0 ? 1 : 0);
				min = min / 1000000;
				max = max / 1000000;
				var arr = [min.toFixed(f1), max.toFixed(f2)];
				return "<span>" + arr.join(' - ') + "</span> triệu/m<sup>2</sup></span>"
			} else if (minArea > 0) {
				var p = "Giá từ " + ((min * minArea) / 1000000);
				return "<span>" + p + "</span> triệu</span>"
			} else { return ""; }

		} else {
			return "";
		}
	};

	$scope.Cities = (function () {
		var all_loc = {
			i: 0,
			n: 'Toàn quốc',
			p: 0,
			u: ''
		};

		var data = [all_loc].concat(mogiCities.data);
		var doSearch = function () {
			var p = $scope.Cities.Selected;
			var url = '';
			if (p.i > 0) {
				url = p.u + '-cid' + p.i;
			}
			window.location.assign(mogiRoutes.Project.SearchProject + url);
		};
		return {
			OnClick: function (item, index) {
				this.Selected = item;
				this.DisplayText = item.n;
				if (item.c !== void (0)) {
					if (index == 0 && this.IsShowBackButton()) {
						this.IsOpen = false;
						doSearch();
						return;
					}
					this.Data = [item].concat(item.c);
					this.ParrentItem = item;
					this.IsOpen = true;

				} else {
					this.IsOpen = false;
					doSearch();
				}
			},
			OnClickBack: function (item) {
				this.Data = data;
				this.Selected = item;
				this.DisplayText = item.n;
				this.ParrentItem = {};
				this.IsOpen = true;

			},
			Data: data,
			Selected: {},
			ParrentItem: {},
			DisplayText: all_loc.n,
			IsShowBackButton: function () {
				return Object.keys(this.ParrentItem).length !== 0;

			},
			IsSelected: function (id) {

				if (this.Selected.i == void (0)) return false;
				return this.Selected.i == id;
			},
			HasChild: function (item) {
				return (item.c !== void (0) && item.c.length > 0)
			},
			IsShowViewMore: function (item, index) {
				if ($scope.Cities.HasChild(item) == true) {
					if (index == 0 && $scope.Cities.IsShowBackButton() == true) {
						return false;
					}
					return true;
				}

				return false;
			},
			IsOpen: false,
			Init: function () {
				var item = (cid == 0) ? all_loc : mogiCities.hash['c' + cid];
				if (item === void (0)) return;
				if (item.p == 0) {
					this.Data = data;


				} else {
					var pItem = mogiCities.hash['c' + item.p];
					this.Data = [pItem].concat(pItem.c);
					this.ParrentItem = pItem;
				}
				this.Selected = item;
				this.DisplayText = item.n;
			}
		};
	}());

	function getKeyword() {
		// Keyword is object
		if ($scope.Keyword != null && $scope.Keyword.Name != void (0)) {
			if ($scope.Keyword.Url !== '') {
				return mogiRoutes.Project.SearchProject + $scope.Keyword.Url;
			} else {
				return mogiRoutes.Project.SearchProjectKeyword + $scope.Keyword.Name;
			}
		} else {
			if ($scope.Keyword == void (0)) {
				return mogiRoutes.Project.SearchProject;
			}
			return mogiRoutes.Project.SearchProjectKeyword + $scope.Keyword;
		}
	}

	var init = (function () {
		if (query != null) {
			var f = ["Type", "Id", "Url"]
			var o = new suggest.SuggestionField();
			o.Name = query.Keyword;
			for (var i = 0; i < f.length; i++) {
				o[f[i]] = query[f[i]];
			}
			$scope.Keyword = o;
			//$scope.Keyword = { Id: 0, Name: "OK", FullName: function () { return "OK";}};
		}
		$scope.Cities.Init();
	}());

};
projectsController.$inject = ['$scope', 'suggest'];
mogiApp.controller('projectsController', projectsController);