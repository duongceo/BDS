let wardReviewController = function ($scope) {
    $scope.CarouselOptions = {
        margin: 0,
        loop: false,
        navText: ['&#x2039;', '&#x203a;'],
        nav: true,
        dots: false,
        responsive: {
            0: { items: 1, margin: 10, stagePadding: 50, nav: false},
            425: { items: 2, margin: 10, nav: false },
            640: { items: 3, margin: 10, nav: false },
            768: { items: 4, margin: 20 }
        }
    };

    $scope.NeighborsCarouselOptions = {
        margin: 0,
        loop: false,
        navText: ['&#x2039;', '&#x203a;'],
        nav: true,
        dots: false,
        responsive: {
            0: { items: 1, margin: 12, stagePadding: 5, nav: false, loop: true },
            425: { items: 1, margin: 12, stagePadding: 20, nav: false, loop: true },
            600: { items: 1, margin: 12, stagePadding: 100, nav: false, loop: true },
            768: { items: 2, margin: 12, stagePadding: 30 },
            992: { items: 2, margin: 12, stagePadding: 100 },
            1070: { items: 3, margin: 12 }
        }
    };

    $scope.ToggleTab = function (value) {
        $scope.IsRent = value;
    };

    $scope.PropOptions = {
        items: 1,
        nav: true,
        navText: ['&#x2039;', '&#x203a;'],
        margin: 20,
        responsive: {
            0: { items: 1, stagePadding: 20, margin: 10, nav: true },
            375: { items: 1, stagePadding: 50, margin: 10, nav: true },
            425: { items: 1, stagePadding: 70, margin: 10, nav: true },
            640: { items: 2, stagePadding: 50, nav: true },
            768: { items: 3 },
            1024: { items: 4 }
        }
    };

    $scope.Gallery = (function () {

        return {
            Open: false,
            Close: function () { this.Open = false; },
            Options: {
                index: 0
            },
            Show: function (i) {
                this.Open = true;
                this.Options.index = i;
            },
            Data: [],
            Init: function (cover,images) {
                if (cover !== null) {
                    for (let i = 0; i < cover.length; i++) {
                        let obj = {
                            src: cover[i].url,
                            h: 700,
                            w: 1024,
                            title: cover[i].title
                        };
                        this.Data.push(obj);
                    }
                }
                if (images !== null) {
                    for (let i = 0; i < images.length; i++) {
                        let obj = {
                            src: images[i].url,
                            h: 700,
                            w: 1024,
                            title: images[i].title
                        };
                        this.Data.push(obj);
                    }
                }
            }
        };
    }());

    let init = (function () {
        $scope.Gallery.Init(cover,images);
    }());
};
wardReviewController.$inject = ['$scope'];
mogiApp.controller('wardReviewController', wardReviewController);
