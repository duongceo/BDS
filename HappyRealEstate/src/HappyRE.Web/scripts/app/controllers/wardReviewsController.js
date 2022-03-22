mogiApp.controller('wardReviewsController', ['$scope',
    function ($scope) {
        let wards = mogiWardReview || [];
        //
        $scope.Wards = (function () {
            let getWards = function (did) {
                let filter = { did: did };
                let data = _.where(wards, filter);
                for (let i = 0; i < data.length; i++) {
                    let img = '/Content/images/hcm-30.jpg';
                    if (data[i].Cover !== null) {
                        let cover = JSON.parse(data[i].Cover);
						img = 'https://cdn.batdongsanhanhphuc.vn/upload/review/thumb-medium/' + cover[0].MediaUrl;
                    }
                    data[i].image = img;
                }
                return data;
            };

            return {
                Items: [],
                Name: '',
                Init: function () {
                    if (did === 0) {
                        window.location.href = '/WardReview/Districts';
                        return;
                    }

                    let d = mogiResUtils.GetCityById(did);
                    this.Name = d.n;
                    this.Items = getWards(did);
                }
            };
        }());
        $scope.Wards.Init();
    }]);