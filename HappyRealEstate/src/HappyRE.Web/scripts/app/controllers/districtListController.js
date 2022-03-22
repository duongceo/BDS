//districtListController
mogiApp.controller('districtListController', ['$scope',
    function ($scope) {
        let wards = mogiWardReview || [];

        $scope.Location = (function () {
            let cities = [];
            let districts = {};
            let initData = function () {
                cities = getCities();
                districts = getDistricts(cities);
            };

            let getCities = function () {
                let data = [];
                let availableCities = _.countBy(wards, "cid");
                let cityIds = Object.keys(availableCities);
                for (let i = 0; i < cityIds.length; i++) {
                    let c = mogiResUtils.GetCityById(cityIds[i]);
                    c.image = imagePath + 'districts/thumb-medium/' + c.u + '.jpg';
                    data.push(c);
                }
                return data;
            };

            let getDistricts = function (cities) {
                let data = {};
                for (let i = 0; i < cities.length; i++) {
                    let districtInCity = _.where(wards, { cid: cities[i].i });
                    let avalableDistrictIds = _.countBy(districtInCity, "did");
					let districtIds = Object.keys(avalableDistrictIds);
					let cityid = cities[i].i;
					let districts = cities[i].c;
					data[cityid] = [];
					for (let j = 0; j < districts.length; j++) {
						let d = districts[j];
						if (_.indexOf(districtIds, d.i.toString()) !== -1) {
							d.image = imagePath + 'districts/thumb-medium/' + d.u + '.jpg';
							d.url = '/review-khu-vuc/' + d.u + '-did' + d.i;
							data[cityid].push(d);
						}
                    }
                }

                return data;
            };

            return {
                City: [],
                Districts: [],
                IsSelected: false,
                OnCitySelected: function (item) {
                    this.IsSelected = true;
                    this.Districts = districts[item.i];
                    console.log(this.Districts);
                },
                Init: function () {
                    initData();
                    this.City = cities;
                    console.log(cities);
                    this.Districts = districts;
                }
            };
        }());

        $scope.Location.Init();
    }]);