mogiApp.controller('marketPriceController',
    ['$scope',
     'blockUI',
    function ($scope) {

        $scope.Search = (function () {
            var monthId = 0;
            var query = {
                FromMonth: moment().format('YYYYMM'),
                ToMonth: moment().format('YYYYMM'),
                CityId: null,
                DistrictId: null,
            };
            var cityId = [30, 24];
            var cityLabel = "";
            var districtLabel = "";
            var monthsLabel = "";

            
            var cities = (function () {
                var c = [];
                cityId.map(function (item) {
                    var key = 'c' + item;
                    c.push(mogiCities.hash[key]);
                });

                return c;
            }());

            var cityChange = function (item) {
                if (item.i !== query.CityId) {
                    query.CityId = item.i;
                    cityLabel = item.n;
                    districts = item.c;
                    districtChange(districts[0]);
                }
                console.log(query);
            }

            var districts = [];

            var districtChange = function (item) {
                if (query.DistrictId !== item.i) {
                    districtLabel = item.n;
                    query.DistrictId = item.i;
                }
            }

            var months = [
                {
                    i: 1,
                    n: '1 tháng gần đây',
                    From: moment().subtract(1, 'months').format('YYYYMM'),
                    To: moment().subtract(1, 'months').format('YYYYMM')
                },
                {
                    i: 3,
                    n: '3 tháng gần đây',
                    From: moment().subtract(3, 'months').format('YYYYMM'),
                    To: moment().subtract(1, 'months').format('YYYYMM')
                }];


            var monthChange = function (item) {
                if (monthId !== item.i) {
                    monthsLabel = item.n;
                    monthId = item.i;
                    query.FromMonth = item.From,
                    query.ToMonth = item.To
                }
            }

            var doSearch = function () {
                console.log(query);
                var param = [];

                Object.keys(query).map(function (key) {
                    param.push(key + '=' + query[key]);
                })
                var url = '/gia-nha-dat-chi-tiet?' + param.join('&');
                location.href = url;
            };


            var init = function () {
                cityChange(cities[0]);
                monthChange(months[0]);
                getQuery();
            };

            var getQuery = function () {
                //FromMonth=201706&ToMonth=201708&CityId=30&DistrictId=368
                var u = $.url();
                var from = parseInt(u.hparam('FromMonth') || 0);
                var to = parseInt(u.hparam('ToMonth') || 0);
                if (from === to) {
                    monthChange(months[0]);
                } else {
                    monthChange(months[1]);
                }
                var city = parseInt(u.hparam('CityId') || 0);
                var district = parseInt(u.hparam('DistrictId') || 0);

                cities.map(function (c, i) {
                    if (c.i == city) {
                        cityChange(cities[i]);
                        c.c.map(function (item) {
                            if (item.i == district) {
                                districtChange(item);
                            }
                        });
                    }
                });


            };

            return {
                Cities: cities,
                CityChange: cityChange,
                Init: init,
                CityLabel: function () { return cityLabel; },
                Districts: function () { return districts; },
                DistrictLabel: function () {
                    return districtLabel;
                },
                DistrictChange: districtChange,
                Months: months,
                MonthsLabel: function () { return monthsLabel; },
                MonthChange: monthChange,
                DoSearch: doSearch

            };

        }());

        $scope.MarketPrice = (function () {

            return {
                PriceToText: function (x) {
                    return mogiResUtils.currencyUtils.toShortTextv2(x);
                }
            }
        }());

        $scope.Search.Init();
    }]);