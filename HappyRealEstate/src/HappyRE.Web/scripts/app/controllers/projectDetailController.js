function projectDetailController($scope) {
    $scope.IsRent = false;
    $scope.topMedia = {
        element: '.top-media',
        page: { index: 1, size: 5, total: 1, itemCount: 0 },
        options: {
            items: 2,
            rewind: true,
            nav: true,
            navText: ['&#x2039;', '&#x203a;'],
            video: true,
            dots: false,
            dotData: true,
            //stagePadding: 80,
            loop: true,
            margin: 2,
            responsive: {
                0: { items: 1, stagePadding: 30, nav: false },
                480: { items: 1, stagePadding: 50, nav: false },
                640: { items: 1, stagePadding: 50, nav: false },
                768: { items: 1, stagePadding: 120, nav: false },
                992: { items: 1, stagePadding: 250 }
            },
            onResize: function (e) {
            },
            onChanged: function (e) {
                //if (e.page.index < 0) return;
                //$scope.topMedia.go(e.page.index);
            },
			onInitialized: function (e) {
				return;
                //var o = $scope.topMedia.page;
                //var c = e.item.count;
                //o.total = (c - c % o.size) / o.size + (c % o.size > 0 ? 1 : 0);
                //o.itemCount = c;
            }
        },
        init: function () {
            $("#top-media").owlCarousel($scope.topMedia.options);
        },
		goPage: function (i) {
			return;
            //var o = $scope.topMedia.page;
            //var n = (o.index + i) * o.size - 1;
            //n = Math.min(o.itemCount - 1, Math.max(0, n));
            //$scope.topMedia.go(n);
        },
        go: function (index) {
            return;
            //var o = $scope.topMedia.page;
            //var i = index + 1;
            //var p = (i - i % 5) / 5 + ((i % 5) > 0 ? 1 : 0);
            //o.index = p;

        }
    };

    $scope.Init = function () {
        $scope.topMedia.init();
        $scope.SampleHouse.Init();
        initMap();
    };

    //$scope.title = 'ngPhotoswipe';
    //$scope.opts = {
    //    index: 0
    //};
    //$scope.showGallery = function (i) {
    //    console.log(i);
    //    if (angular.isDefined(i)) {
    //        $scope.opts.index = i;
    //    }
    //    $scope.open = true;
    //};

    //$scope.closeGallery = function () {
    //    $scope.open = false;
    //};
    $scope.PriceFormat = function (min, max, minArea) {

        if (min > 0) {
            if (max > 0) {
                var f1 = ((min % 1000000) > 0 ? 1 : 0);
                var f2 = ((max % 1000000) > 0 ? 1 : 0);
                min = min / 1000000;
                max = max / 1000000;
                var arr = [min.toFixed(f1), max.toFixed(f2)];
                return "<span>" + arr.join(' - ') + "</span> triệu/m<sup>2</sup></span>";
            } else if (minArea > 0) {
                var p = "Từ " + ((min * minArea) / 1000000);
                return "<span>" + p + "</span> triệu</span>";
            } else { return ""; }

        } else {
            return "";
        }
    };

    $scope.SampleHouse = (function () {
        return {
            Data: [],
            Open: [],
            Init: function () {
                if (SampleHouse !== void (0) && SampleHouse.length > 0) {
                    for (var i = 0; i < SampleHouse.length; i++) {
                        var obj = SampleHouse[i];
                        $scope.SampleHouse.Data.push(obj.Images);
                        $scope.SampleHouse.Open.push(false);
                        $scope.SampleHouse.Title.push(obj.Title);
                    }
                }
            },
            Show: function (i) {
                $scope.SampleHouse.Open[i] = true;
            },
            Close: function (i) {
                for (var j = 0; j < this.Open.length; j++) {
                    if (this.Open[j] === true) this.Open[j] = false;
                }
            },
            Options: {
                index: 0
            },
            Title: []
        };

    }());
    var initMap = function () {
        $scope.Location = prj_location || "0,0";
        $scope.Title = prj_title || "";
        $scope.ShowNearBy = true;
        $scope.ShowMap = true;
        $scope.IsResponsive = false;
    };
    $scope.ShowMobileMap = function () {
        $scope.IsResponsive = true;
        $scope.ShowMap = true;
    };

    $scope.viewmore = false;
	$scope.showmore = function () {
		$scope.viewmore = !$scope.viewmore;
	};
    $scope.activeMap = function () {
        $scope.ShowMap = true;
    };
}
projectDetailController.$inject = ['$scope'];
mogiApp.controller('projectDetailController', projectDetailController);