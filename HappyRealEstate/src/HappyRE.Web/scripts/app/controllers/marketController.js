function MarketController($scope, $http, $filter, blockUI, suggest, propertyServices, cityServices,marketPriceServices) {

    $scope.List = {
        did:0,
        Keyword: null,
        SearchStreet: function () {
            if (this.Keyword == null || this.Keyword == undefined) {
                mogiUtils.boxAlert('Chưa nhập đường hoặc đường nhập vào không đúng');
                return;
            }
            if (typeof (this.Keyword) != 'object') {
                mogiUtils.boxAlert('Đường nhập vào không đúng');
                return;
            }
            var p = {};
            var did = this.Keyword.DistrictId;
            var sid = this.Keyword.StreetId;
            var objCity = mogiResUtils.GetCityById(did);
            if (sid > 0) {
				cityServices.GetStreetById(sid).then(function (rp) {
					rp = rp.data;
                    if (rp.Status == true) {
                        //gia-nha-dat-{duong}-d{districtId}-s{streetid}
                        var url = "gia-nha-dat-";
                        url += rp.Data.CodeUrl;
                        url += "-d" + objCity.i + "-s" + rp.Data.StreetId;
                        location.href = url;
                    }
                });
            } else {
                var url = "/gia-nha-dat-";
                url += objCity.u;
                url += "-qd" + objCity.i;
                location.href = url;
            }
        },
        GetStreetSuggest: function (q) {
            return suggest.Street(q, null);
        },
        Values: [],
        Data:[],
        GotoStreetList: function (did) {
            var objCity = mogiResUtils.GetCityById(did);
            var url = "/gia-nha-dat-";
            url += objCity.u;
            url += "-qd" + objCity.i;
            //location.href = url;
            return url;
        },
        GetDistrict: function (id) {
            $scope.List.did = id;
            var obj = this.Values[id];
            if (obj != null && obj != undefined) {
                this.Value = obj;
                $scope.List.Data = angular.copy($scope.List.Value);
            }
            else {
				propertyServices.MarketGetByDistrict(id).then(function (rp) {
					rp = rp.data;
                    if (rp.Status == true) {
                        $scope.List.Value = rp.Data;
                        $scope.List.Values[id] = rp.Data;
                        $scope.List.Data = angular.copy($scope.List.Value);
                    }
                    })
                $scope.List.GetMarketPriceSummary(id);
            }
            
        },
        Value: [],
        ResetKeyWord : function () {
            $scope.List.Keyword = null;
            $scope.List.Filter();
        },
        OnKeyUp: function (e) {
            $scope.List.Filter();
        },
        Filter: function () {
            var filter = {};
            var data = $scope.List.Data;
            if ($scope.List.Keyword !== void (0) && $scope.List.Keyword !== null && $scope.List.Keyword != '') {

                var escapedSearchText = $scope.List.Keyword;
                escapedSearchText = escapedSearchText.replace(/ă/ig, '[ăắằẳẵặ]');
                escapedSearchText = escapedSearchText.replace(/â/ig, '[âấầẩẫậ]');
                escapedSearchText = escapedSearchText.replace(/a/ig, '[aáàảãạăắằẳẵặâấầẩẫậ]');
                escapedSearchText = escapedSearchText.replace(/d/ig, '[dđ]');
                escapedSearchText = escapedSearchText.replace(/ê/ig, '[êếềểễệ]');
                escapedSearchText = escapedSearchText.replace(/e/ig, '[eéèẻẽẹêếềểễệ]');
                escapedSearchText = escapedSearchText.replace(/i/ig, '[iíìỉĩị]');
                escapedSearchText = escapedSearchText.replace(/ô/ig, '[ôốồổỗộ]');
                escapedSearchText = escapedSearchText.replace(/ơ/ig, '[ơớờởỡợ]');
                escapedSearchText = escapedSearchText.replace(/o/ig, '[oóòỏõọôốồổỗộơớờởỡợ]');
                escapedSearchText = escapedSearchText.replace(/ư/ig, '[ưứừửữự]');
                escapedSearchText = escapedSearchText.replace(/u/ig, '[uúùủũụưứừửữự]');
                escapedSearchText = escapedSearchText.replace(/y/ig, '[yýỳỷỹỵ]');

                var regex = new RegExp(escapedSearchText, 'i');

                data = _.filter(data, function (s) {
                    return regex.test(s.StreetName);
                });
            }

            $scope.List.Value = data;
        },
        GotoDetail: function (item) {
            location.href = item.FrontEnd_SearchUrl;
        },
        MonthRangeOptions: [{ i: 3, n: '3 tháng' }, { i: 6, n: '6 tháng' }, { i: 12, n: '12 tháng' }],
        MonthRange: 12,
        MonthRangeChanged: function () {
            $scope.List.GetMarketPriceSummary($scope.List.did);
        },
        GetMarketPriceSummary: function (did) {
            blockUI.start();
            marketPriceServices.GetHousePriceSummary_ByDistrict(did, $scope.List.MonthRange)
				.then(function (rp) {
					rp = rp.data;
                    blockUI.stop();
                    if (rp.Status == true) {
                        $scope.List.MarketPriceSummary = rp.Data;
                    }
                });
        },
    };
    //chi tiết

    function BuildProfileChartData(objs) {
        $scope.profileChart.labels = [];
        $scope.profileChart.datasets = [];
        var t1 = {
            label: "Thành viên",
            fill: false,
            backgroundColor: "#0000FF",
            borderColor: "#0000FF",
            data: []
        }
        var t2 = {
            label: "Môi giới",
            fill: false,
            backgroundColor: "#008000",
            borderColor: "#008000",
            data: []
        }
        var t3 = {
            label: "Duyệt",
            fill: false,
            backgroundColor: "#FF0000",
            borderColor: "#FF0000",
            data: []
        }

        angular.forEach(objs, function (value, key) {
            data.push(new { x: value.PropertyTypeName, y: value.AvgPrice });            
        });

        $scope.profileChart.datasets.push(member);
        $scope.profileChart.datasets.push(mogipro);
        $scope.profileChart.datasets.push(approved);
    }

    function getPercent(value, total, fixed) {
        fixed = fixed || 0;
        return (value * 100 / total).toFixed(fixed);
    }

    function toChangedPercent(now, before) {
        if (before == 0) return 0;
        return ((now - before) / before * 100).toFixed(2);
    }

    $scope.Detail = {
        Value: {s:0,d:0},
        MonthRangeOptions: [{ i: 3, n: '3 tháng' }, { i: 6, n: '6 tháng' }, { i: 12, n: '12 tháng' }],
        MonthRange: 12,
        MaxMonth:0,
        MarketPriceSummary: [],
        MTChartData: [],
        MTChartChartOptions: {
            responsive: true,
            scales: {
                xAxes: [{
                    display: true,
                    scaleLabel: {
                        display: false,
                        labelString: 'Month',
                        showLine: false
                    },
                    gridLines: {
                        display: false
                    }
                }],
                yAxes: [{
                    display: true,
                    scaleLabel: {
                        display: false,
                        labelString: 'Value',
                        showLine: false
                    }, ticks: {
                        autoSkip: false,
                        maxTicksLimit: 10

                    }, gridLines: {
                        display: true
                    }

                }]
            },
            maintainAspectRatio: false,
            elements: {
                line: {
                    tension: 0, // disables bezier curves
                }
            },
            tooltips: {
                enabled: false
            },
            legend: {
                display: false
            },

        },
        Init: function () {
            if (dataChart != null) {
                var arr = [];
                for (var i = 0; i <= items.length - 1; i++) {
                    var obj = (items[i] || null);
                    if (obj == null) continue;
                    var chartData = [];
                    for (var j = 0; j <= dataChart.length - 1; j++) {
                        var o = (dataChart[j] || null);
                        if (o == null) break;
                        if (o.PropertyStyle == obj.PropertyStyle) {
                            chartData.push(parseInt(o.AvgPrice));
                        }
                    }
                    var s = {
                        labels: months,
                        datasets: [{
                            label: "",
                            fill: false,
                            backgroundColor: "#45bfb7",
                            borderColor: "#45bfb7",
                            pointBackgroundColor: "#45bfb7",
                            pointBorderColor: "#45bfb7",
                            data: chartData
                        }]
                    };
                    arr.push(s);
                }
                this.MTChartData = arr;

            }
        },
        //MonthRangeChanged: function () {
        //    //$scope.Detail.GetMarketPriceSummary($scope.Detail.Value.s, $scope.Detail.Value.d);
        //},
    //    GetMarketPriceSummary: function (sid, did) {
    //        blockUI.start();
    //        //marketPriceServices.GetHousePriceSummary_ByStreet(did, sid, $scope.Detail.MonthRange)
    //        marketPriceServices.GetHousePriceSummary_ByStreet(did, sid)
				//.then(function (rp) {
				//	rp = rp.data;
    //                blockUI.stop();
    //                if (rp.Status == true) {
    //                    $scope.Detail.MarketPriceSummary = rp.Data;
    //                    $scope.Detail.AvgPriceChart.Init(rp.Data);
    //                }
    //            });
    //    },
        GetMarketPriceMonthly: function (sid, did) {
            blockUI.start();
            marketPriceServices.GetHousePriceMonthly_ByStreet(did, sid, 24)
				.then(function (rp) {
					rp = rp.data;
                    blockUI.stop();
                    if (rp.Status == true) {
                        $scope.Detail.PriceTrendChart.Init(rp.Data);
                        $scope.Detail.PriceMonthlyChange.Init(rp.Data);
                        $scope.Detail.CurrentMonthChart.Init(rp.Data);
                    }
                });
        },
        GetMarketPriceTopPrice: function (did) {
            blockUI.start();
            marketPriceServices.GetHousePrice_TopBy_AvgPrice(did)
				.then(function (rp) {
					rp = rp.data;
                    blockUI.stop();
                    if (rp.Status == true) {
                        $scope.Detail.MarketPriceTopPrice = rp.Data;
                    }
                });
        },
        GetMarketPriceTopTotal: function (did) {
            blockUI.start();
            marketPriceServices.GetHousePrice_TopBy_Total(did)
				.then(function (rp) {
					rp = rp.data;
                    blockUI.stop();
                    if (rp.Status == true) {
                        $scope.Detail.MarketPriceTopTotal = rp.Data;
                    }
                });
        },
        GetMarketPriceStreetVsDistrict: function (sid, did) {
            blockUI.start();
            marketPriceServices.GetHousePrice_Street_vs_District(did,sid)
				.then(function (rp) {
					rp = rp.data;
                    blockUI.stop();
                    if (rp.Status == true) {
                        $scope.Detail.StreetVsDistrictChart.Init(rp.Data);
                    }
                });
        },
        AvgPriceChart: {
            Data: [],
            Options: {
                legend: {
                    display: false
                },
                title: {
                    display: true,
                    text: 'Giá trung bình'
                },
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        gridLines: { display: false },
                        display: true,
                        barPercentage: 0.25,
                        categoryPercentage: 1,
                        barThickness: 30,
                        maxBarThickness: 30,
                        scaleLabel: {
                            display: false
                        }
                    }],
                    yAxes: [{
                        display: true,
                        scaleLabel: {
                            
                            display: true,
                            labelString: 'tr/m2'
                        },
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            },
            Init: function (objs) {
                //$scope.Detail.AvgPriceChart.Options.title.text = 'Giá trung bình ' + $scope.Detail.MonthRange +' tháng gần đây';
                var c = {
                    labels: [],
                    datasets:[]
                }
                var opts = [{ bg: '#32A8EA', d: [] }, { bg: '#FF9802', d: [] }, { bg: '#28D178', d: [] }, { bg: '#FF6362', d: [] }, { bg: '#0000FF', d: [] }];
                var k = 0;
                var max = 0;
                angular.forEach(objs, function (value, key) {                    
                    if (value.AvgPrice >=0) {
                        if (value.AvgPrice > max) max = value.AvgPrice;
                            c.labels.push(value.PropertyTypeName);
                        opts[k].d.push({ x: value.PropertyTypeName, y: Math.round(parseInt(value.AvgPrice / 1000000)) });
                        
                        c.datasets.push({
                            label: value.PropertyTypeName,
                            fill: false,                         
                            backgroundColor: opts[k].bg,
                            borderColor: opts[k].bg,
                            data: opts[k].d,
                        });
                    }
                    k = k + 1;
                });
                var size = $scope.Detail.CalcChartHeight(max);
                var ticks = $scope.Detail.AvgPriceChart.Options.scales.yAxes[0].ticks;
                ticks.max = 10 * size;
                ticks.stepSize = size;
                $scope.Detail.AvgPriceChart.Data = c;
            }
        },
        PriceTrendChart: {
            Data: [],
            MonthId:6,
            SourceData:[],
            Options: {
                legend: {
                    display: true,
                    position: 'bottom',
                },
                title: {
                    display: false,
                    text: 'Xu thế giá 12 tháng gần đây'
                },
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        display: true,
                        scaleLabel: {
                            display: false
                        },
                        gridLines: { display: false }
                    }],
                    yAxes: [{                                             
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'tr/m2'
                        },
                        ticks: {
                            beginAtZero: true,
                            max:0
                        }
                    }]
                }
            },
            Init: function (objs) {
                $scope.Detail.PriceTrendChart.SourceData = objs;               
                $scope.Detail.PriceTrendChart.LoadData(6);
            },
            LoadData: function (monthRange) {
                $scope.Detail.PriceRangeChart.MonthId = monthRange;
                var u = $scope.Detail.GetMonthRanges(monthRange);

                var c = {
                    labels: [],
                    datasets: []
                }
                var opts = [{ bg: '#32A8EA', d: [] }, { bg: '#FF9802', d: [] }, { bg: '#28D178', d: [] }, { bg: '#FF6362', d: [] }, { bg: '#0000FF', d: [] }, { bg: '#0000FF', d: [] }, { bg: '#0000FF', d: [] }, { bg: '#0000FF', d: [] }];
                var keys = [];
                var k = 0;
                var lb = [];
                var max = 0;
                var objs = $scope.Detail.PriceTrendChart.SourceData;

                angular.forEach(objs, function (value, key) {
                    if (value.AvgPrice > max) max = value.AvgPrice;
                    if ($.inArray(value.MonthId, lb) == -1 && value.MonthId > 0 && $.inArray(value.MonthId, u)>=0) {
                        lb.push(value.MonthId);
                    }
                });

                angular.forEach(objs, function (value, key) {
                    if ($.inArray(value.PropertyTypeName, keys) == -1 && $.inArray(value.MonthId, u) >= 0) {
                        if (value.MonthId > 0) keys.push(value.PropertyTypeName);

                        angular.forEach(u, function (c, d) {
                            var ou = $filter('filter')(objs, function (d) { return d.PropertyTypeName === value.PropertyTypeName && d.MonthId == c; });
                            if (ou.length > 0) opts[k].d.push(Math.round(parseInt(ou[0].AvgPrice / 1000000)));
                            else {
                                var o = opts[k].d[opts[k].d.length - 1];
                                if (o == undefined || o == null) {
                                    //Chon thang < thang hien tại gan nhat co du lieu
                                    var xx = $filter('filter')(objs, function (d) { return d.PropertyTypeName === value.PropertyTypeName && d.AvgPrice && d.AvgPrice != null && d.MonthId < c; });
                                    if (xx.length > 0) {
                                        xx = xx.sort(function (a, b) { return b.MonthId - a.MonthId });
                                        opts[k].d.push(Math.round(parseInt(xx[0].AvgPrice / 1000000)));
                                    } else {
                                        //Chon thang > thang hien tại gan nhat co du lieu
                                        xx = $filter('filter')(objs, function (d) { return d.PropertyTypeName === value.PropertyTypeName && d.AvgPrice && d.AvgPrice != null && d.MonthId > c; });
                                        if (xx.length > 0) {
                                            xx = xx.sort(function (a, b) { return a.MonthId - b.MonthId });
                                            opts[k].d.push(Math.round(parseInt(xx[0].AvgPrice / 1000000)));
                                        }
                                    }
                                }
                                else opts[k].d.push(o);
                            }
                        });

                        if (value.MonthId > 0) {
                            c.datasets.push({
                                label: value.PropertyTypeName,
                                fill: false,
                                lineTension: 0,
                                borderWidth: 2,
                                pointBorderWidth: 0,
                                backgroundColor: opts[k].bg,
                                borderColor: opts[k].bg,
                                data: opts[k].d,
                            });
                        }
                        k = k + 1;
                    }

                });

                angular.forEach(u, function (value, key) {
                    if (((key + 1) % 3 == 0 && key > 1) || key == 0) {
                        value = value + '';
                        c.labels.push(value.substring(4, 6) + '/' + value.substring(0, 4));
                    }
                    else c.labels.push('');
                });

                var size = $scope.Detail.CalcChartHeight(max);
                var ticks = $scope.Detail.PriceTrendChart.Options.scales.yAxes[0].ticks;
                ticks.max = 10 * size;
                ticks.stepSize = size;
                $scope.Detail.PriceTrendChart.Data = c;
                
            }
        },
        PriceMonthlyChange: {
            Month: {
                Range: [],
                Min: 0,
                Max: 0,
                Current: 0,
                Change: function (val) {

                    if ($scope.Detail.PriceMonthlyChange.Month.Current + val < $scope.Detail.PriceMonthlyChange.Month.Range.length && $scope.Detail.PriceMonthlyChange.Month.Current + val >5) {
                        $scope.Detail.PriceMonthlyChange.Month.Current += val;
                        $scope.Detail.PriceMonthlyChange.LoadData();
                    }
                }
            },
            Data: [],
            SourceData:[],
            Init: function (objs) {
                $scope.Detail.PriceMonthlyChange.SourceData = objs;

                var monthRanges = $scope.Detail.GetMonthRanges(24);
                $scope.Detail.PriceMonthlyChange.Month.Range = monthRanges;
                $scope.Detail.PriceMonthlyChange.Month.Max = Math.max.apply(Math, monthRanges); 
                $scope.Detail.PriceMonthlyChange.Month.Min =  Math.min.apply(Math, monthRanges);
                $scope.Detail.PriceMonthlyChange.Month.Current = $scope.Detail.PriceMonthlyChange.Month.Range.findIndex(x => x == $scope.Detail.PriceMonthlyChange.Month.Max);//$scope.Detail.PriceMonthlyChange.Month.Max;

                $scope.Detail.PriceMonthlyChange.LoadData();
            },   
            LoadData: function () {

                var idx = $scope.Detail.PriceMonthlyChange.Month.Current;// $scope.Detail.PriceMonthlyChange.Month.Range.findIndex(x => x == $scope.Detail.PriceMonthlyChange.Month.Current);
            var monthRanges = $.grep($scope.Detail.PriceMonthlyChange.Month.Range, function (v, i) {
                return i<=idx && i > (idx - 6);
            });

            var objs = $scope.Detail.PriceMonthlyChange.SourceData;
                $scope.Detail.PriceMonthlyChange.Data = [];
                var types = ['Mặt tiền, phố', 'Hẻm, ngõ', 'Căn hộ', 'Đất']
                angular.forEach(types, function (t) {
                    var im = {
                        Type: t,
                        M: [],
                        V: []
                    };
                    var vl = [];

                    angular.forEach(monthRanges, function (c, idx) {
                        var items = $.grep(objs, function (i) {
                            return i.MonthId == c && i.PropertyTypeName == t
                        });
                        c = c + '';
                        im.M.push(c.substring(4, 6) + '/' + c.substring(0, 4));
                        if (items.length > 0) {
                            vl.push(items[0].AvgPrice);
                            var changePercent = (idx > 0 && vl[idx] > 0 && vl[idx - 1] > 0) ? ((vl[idx] - vl[idx - 1]) / vl[idx - 1] * 100).toFixed(2) : 0;
                            changePercent = changePercent < 0 ? changePercent + '% <i class="down">&#9660;</i>' : changePercent > 0 ? changePercent + '% <i class="up">&#9650;</i>' : ''
                            im.V.push(Math.round(parseInt(items[0].AvgPrice / 1000000)) + ' tr/m<sup>2</sup><sup class="change">' + changePercent + '</sup>')
                        } else {
                            if (idx > 0) vl.push(vl[idx - 1]);
                            else vl.push(0);
                            im.V.push('-')
                        }
                    });
                    $scope.Detail.PriceMonthlyChange.Data.push(im);
                });
            }
        },
        StreetVsDistrictChart: {
            Data: [],
            Options: {
                title: {
                    display: true,
                    text: 'So sánh giá trung bình đường và quận'
                },
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    xAxes: [{
                        display: true,
                        scaleLabel: {
                            display: false
                        },
                        gridLines: { display: false }
                    }],
                    yAxes: [{
                        display: true,
                        scaleLabel: {
                            display: true,
                            labelString: 'tr/m2'
                        },
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                }
            },
            Init: function (objs) {
                var c = {
                    labels: [],
                    datasets: []
                }

                var street = {
                    label: "Đường",
                    fill: false,
                    lineTension: 0,
                    borderWidth: 2,
                    pointBorderWidth: 0,
                    backgroundColor: "#ef114b",
                    borderColor: "#ef114b",
                    data: []
                }
                var district = {
                    label: "Quận",
                    fill: false,
                    lineTension: 0,
                    borderWidth: 2,
                    pointBorderWidth: 0,
                    backgroundColor: "#28d178",
                    borderColor: "#28d178",
                    data: []
				}

				var monthRanges = $scope.Detail.GetMonthRanges(12);
				angular.forEach(monthRanges, function (value, key) {
					if (((key + 1) % 3 == 0 && key > 1) || key == 0) {
						var x = value + '';
						c.labels.push(x.substring(4, 6) + '/' + x.substring(0, 4));
					}
					else c.labels.push('');
				});

				var max = 0;


				angular.forEach(monthRanges, function (c, d) {
					var ou = $filter('filter')(objs, function (d) { return d.MonthId == c; });
					if (ou.length > 0) {
						var value = ou[0];
						if (value.StreetAvgPrice > max) max = value.StreetAvgPrice;
						if (value.DistrictAvgPrice > max) max = value.DistrictAvgPrice;
						if (value.StreetAvgPrice > 0) {
							street.data.push(Math.round(parseInt(value.StreetAvgPrice / 1000000)));
						} else {
							var o = street.data[street.data - 1];
							if (o == undefined || o == null) {
                                var xx = $filter('filter')(objs, function (d) { return d.StreetAvgPrice > 0 && d.MonthId < c});
                                if (xx.length > 0) {
                                    xx = xx.sort(function (a, b) { return b.MonthId - a.MonthId });
                                    street.data.push(Math.round(parseInt(xx[0].StreetAvgPrice / 1000000)));
                                } else {
                                    xx = $filter('filter')(objs, function (d) { return d.StreetAvgPrice > 0 && d.MonthId > c });
                                    if (xx.length > 0) {
                                        xx = xx.sort(function (a, b) { return a.MonthId - b.MonthId });
                                        street.data.push(Math.round(parseInt(xx[0].StreetAvgPrice / 1000000)));
                                    }
                                }
							}
						}


						if (value.DistrictAvgPrice > 0) {
							district.data.push(Math.round(parseInt(value.DistrictAvgPrice / 1000000)));
						} else {
							var o1 = district.data[district.data - 1];
                            if (o1 == undefined || o1 == null) {
                                var xx = $filter('filter')(objs, function (d) { return d.DistrictAvgPrice > 0 && d.MonthId < c });
								if (xx.length > 0) {
									xx = xx.sort(function (a, b) { return b.MonthId - a.MonthId });
									district.data.push(Math.round(parseInt(xx[0].DistrictAvgPrice / 1000000)));
                                } else {
                                    xx = $filter('filter')(objs, function (d) { return d.DistrictAvgPrice > 0 && d.MonthId > c });
                                    if (xx.length > 0) {
                                        xx = xx.sort(function (a, b) { return a.MonthId - b.MonthId });
                                        district.data.push(Math.round(parseInt(xx[0].DistrictAvgPrice / 1000000)));
                                    }
                                }
							}
						}

					} else {
						//street
                        var o = street.data[street.data - 1];
                        if (o == undefined || o == null) {
                            var xx = $filter('filter')(objs, function (d) { return d.StreetAvgPrice > 0 && d.MonthId < c });
                            if (xx.length > 0) {
                                xx = xx.sort(function (a, b) { return b.MonthId - a.MonthId });
                                street.data.push(Math.round(parseInt(xx[0].StreetAvgPrice / 1000000)));
                            } else {
                                xx = $filter('filter')(objs, function (d) { return d.StreetAvgPrice > 0 && d.MonthId > c });
                                if (xx.length > 0) {
                                    xx = xx.sort(function (a, b) { return a.MonthId - b.MonthId });
                                    street.data.push(Math.round(parseInt(xx[0].StreetAvgPrice / 1000000)));
                                }
                            }
                        }
						//district
                        var o1 = district.data[district.data - 1];
                        if (o1 == undefined || o1 == null) {
                            var xx = $filter('filter')(objs, function (d) { return d.DistrictAvgPrice > 0 && d.MonthId < c });
                            if (xx.length > 0) {
                                xx = xx.sort(function (a, b) { return b.MonthId - a.MonthId });
                                district.data.push(Math.round(parseInt(xx[0].DistrictAvgPrice / 1000000)));
                            } else {
                                xx = $filter('filter')(objs, function (d) { return d.DistrictAvgPrice > 0 && d.MonthId > c });
                                if (xx.length > 0) {
                                    xx = xx.sort(function (a, b) { return a.MonthId - b.MonthId });
                                    district.data.push(Math.round(parseInt(xx[0].DistrictAvgPrice / 1000000)));
                                }
                            }
                        }
					}

				});

                c.datasets.push(street);
                c.datasets.push(district);

                var size = $scope.Detail.CalcChartHeight(max);
                var ticks = $scope.Detail.StreetVsDistrictChart.Options.scales.yAxes[0].ticks;
                ticks.max = 10 * size;
                ticks.stepSize = size;
                $scope.Detail.StreetVsDistrictChart.Data = c;
                $scope.Detail.GetMonthRanges(12);
            }
        },
        CurrentMonthChart: {
            Data: [],
            DataTbl:[],
            Options: {
                legend: {
                    display: false
                },
                title: {
                    display: false
                },
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    datalabels: {
                        //formatter: function (value, ctx) {
                        //    return value + '%';
                        //},
                        align: 'top',
                        anchor: 'end',
                        offset: 1,
                        padding: {
                            top: 1
                        },
                        color: "#262637",
                        font: {
                            family: 'Open Sans',
                            size: 11
                        }
                    }
                },
                layout: {
                    padding: {
                        top: 20
                    }
                },
                scales: {
                    yAxes: [{
                        gridLines: { display: true },
                        display: true,
                        //scaleLabel: {
                        //    display: false
                        //},
                        ticks: {
                            beginAtZero: true
                        }
                    }],
                    xAxes: [{
                        gridLines: { display: false, offsetGridLines: true },
                        display: true,
                        barPercentage: 0.3,
                        categoryPercentage: 1,
                        barThickness: $scope.barWidth,
                        maxBarThickness: $scope.barWidth,
                        minBarLength: 2
                    }]
                }
            },
            Init: function (objs) {
                

                var c = {
                    labels: [],
                    datasets: []
                };
                c.datasets.push({
                    backgroundColor: [],
                    data: []
                });

                var opts = ['#32A8EA', '#FF9802', '#28D178', '#FF6362'];

                var crm = Math.max.apply(Math, objs.map(function (o) { return o.MonthId; }))
                var crObj = $.grep(objs, function (v) {
                    return v.MonthId == crm
                });
                var monthRanges = $scope.Detail.GetMonthRanges(24);
                var idx = monthRanges.findIndex(x => x == crm);
                

                
                angular.forEach(crObj, function (v, key) {

                    var tbl = {
                        PropertyTypeName: v.PropertyTypeName,
                        AvgPrice: Math.round(v.AvgPrice / 1000000) + ' tr/m<sup>2</sup>',
                        MinPrice: Math.round(v.MinPrice / 1000000) + ' tr/m<sup>2</sup>',
                        MaxPrice: Math.round(v.MaxPrice / 1000000) + ' tr/m<sup>2</sup>',
                        AvgPriceChanged: 0,
                        MinPriceChanged: 0,
                        MaxPriceChanged: 0
                    };

                    var crObj_prev = $.grep(objs, function (k) {
                        return k.MonthId == (monthRanges[idx - 1] || 0) && (k.PropertyTypeName == v.PropertyTypeName);
                    });

                    if (crObj_prev.length > 0) {
                        //debugger
                        tbl.AvgPriceChanged = toChangedPercent(v.AvgPrice, crObj_prev[0].AvgPrice);
                        tbl.MinPriceChanged = toChangedPercent(v.MinPrice, crObj_prev[0].MinPrice);
                        tbl.MaxPriceChanged = toChangedPercent(v.MaxPrice, crObj_prev[0].MaxPrice);
                    }

                    $scope.Detail.CurrentMonthChart.DataTbl.push(tbl);
                });

                angular.forEach(crObj, function (v, key) {
                    c.labels.push(v.PropertyTypeName);
                    c.datasets[0].data.push(Math.round(v.AvgPrice/1000000));
                    c.datasets[0].backgroundColor.push(opts[key]);
                });

                $scope.Detail.CurrentMonthChart.Data = c;
            }
        },
        PriceRangeChart: {
            Data: [],
            Fetch: function (sid) {
                var monthId = Math.max.apply(Math, $scope.Detail.GetMonthRanges(1));
                var f = {
                    monthId: monthId,
                    streetId:sid,
                    cityId: 0,
                    propertyTypeId: 1
                };
                marketPriceServices.PropertySummaryByPriceRange(f)
                    .then(function (rp) {
                        if (rp.data.Status === true) {
                            $scope.Detail.PriceRangeChart.Init(rp.data.Data);
                        }
                    });
                    
            },
            Options: {
                legend: {
                    display: false
                },
                title: {
                    display: false
                },
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    datalabels: {
                        formatter: function (value, ctx) {
                            return value + '%';
                        },
                        align: 'top',
                        anchor: 'end',
                        offset: 1,
                        padding: {
                            top: 1
                        },
                        color: "#262637",
                        font: {
                            family: 'Open Sans',
                            size: 11
                        }
                    }
                },
                layout: {
                    padding: {
                        top: 20
                    }
                },
                scales: {
                    yAxes: [{
                        gridLines: { display: false },
                        display: false,
                        scaleLabel: {
                            display: false
                        },
                        ticks: {
                            beginAtZero: true
                        }
                    }],
                    xAxes: [{
                        gridLines: { display: false, offsetGridLines: true },
                        display: true,
                        barPercentage: 0.3,
                        categoryPercentage: 1,
                        barThickness: $scope.barWidth,
                        maxBarThickness: $scope.barWidth,
                        minBarLength: 2
                    }]
                }
            },
            Init: function (objs) {
                var c = {
                    labels: [],
                    datasets: []
                };
                c.datasets.push({
                    backgroundColor: [],
                    data: []
                });
                var chart_colors = ['#00A285', '#03AF90', '#04BF9E', '#00CEA9', '#00DEB6', '#15EBC4', '#33F2D0', '#4EF6D8', '#72FEE5', '#94FBE8', '#94FBE8'];
                var sum = 0;
                objs.map(function (data) { sum += data.Value; return data; });

                angular.forEach(objs, function (v, key) {
                    c.labels.push(v.Name);
                    c.datasets[0].data.push(getPercent(v.Value, sum));
                    c.datasets[0].backgroundColor.push(chart_colors[key]);
                });
                $scope.Detail.PriceRangeChart.Data = c;
            }
        },
		CalcChartHeight: function (m) {
			var max = Math.round(parseInt((m / 1000000))) + 2;
			var size = Math.ceil(max / 10);
			var delta = size % 10;
			if (delta > 0) {
				size = size + 10 - delta;
			}
			//if (size * 10 < m) size = size + 5;
			return size;
		},
        GetMonthRanges: function (range) {
            var time = $scope.Detail.MaxMonth;
            var d = (time == undefined || time==null)? new Date(): new Date(time);
            var r = [];
            for (var i = 1; i <= range; i++) {
                var f = new Date(d.getFullYear(), d.getMonth());
                f.setMonth(f.getMonth() - i);
                var n = f.getFullYear() + '' + ((f.getMonth() + 1) < 10 ? '0' : '') + (f.getMonth() + 1);
                r.push(parseInt(n));
            }
            return r.sort();
        }
    };

    $scope.GotoDetail = function (s, d) {
        cityServices.GetStreetById(s)
			.then(function (rp) {
				rp = rp.data;
                if (rp.Status == true) {
                    var url = "gia-nha-dat-";
                    url += rp.Data.CodeUrl;
                    url += "-d" + d + "-s" + rp.Data.StreetId;
                    location.href = url;
                }
            });
    }

    $scope.HistoryPrice = {
        List: {
            Data: [],
            Total: 0
        },
        Query: {
            PageIndex: 1,
            PageSize: 10
        },
        GetByStreet: function (id) {
            propertyServices.GetPropertyByStreet(id, $scope.HistoryPrice.Query.PageIndex)
				.then(function (rp) {
					rp = rp.data;
                    if (rp.Status == true) {
                        $scope.HistoryPrice.List.Data = rp.Data.Data;
                        $scope.HistoryPrice.List.Total = rp.Data.Total;
                    }
                });
        },
        Search: function (id) {
            $scope.HistoryPrice.Query.PageIndex = 1;
            $scope.HistoryPrice.GetByStreet(id);
        },
        PageChanged: function (id) {
            $scope.HistoryPrice.GetByStreet(id);
        },
        GotoHistoryDetails: function (DetailUrl, loc, id) {
            var url = '/lich-su-gia-' + DetailUrl + "-" + loc + "-" + id;
            location.href = url;
        }
    }

    $scope.getPropertyByUniqueAddress = function (id) {
		propertyServices.GetPropertyByUniqueAddress(id)
			.then(function (rp) {
				rp = rp.data;
				if (rp.Status === true) {
					$scope.PropertyNews = rp.Data;
				}
			});
    };

    $scope.DetailInit = function (sid, did, time) {
        $scope.Detail.Value.s = sid;
        $scope.Detail.Value.d = did;
        $scope.Detail.MaxMonth = time;
        //$scope.Detail.GetMarketPriceSummary(sid, did);
        $scope.Detail.GetMarketPriceMonthly(sid, did);
        $scope.Detail.GetMarketPriceTopPrice(did);
        $scope.Detail.GetMarketPriceTopTotal(did);
        $scope.Detail.GetMarketPriceStreetVsDistrict(sid, did);
        //$scope.Detail.PriceRangeChart.Fetch(sid);
        $scope.HistoryPrice.Search(sid);
    }

    Chart.defaults.global.title.fontSize = 15;
    Chart.defaults.global.legend.position = 'bottom';
    Chart.defaults.global.legend.labels.boxWidth = 15;
    Chart.defaults.global.tooltips.backgroundColor = '#000';
    Chart.defaults.global.legend.labels.usePointStyle = true;
}
MarketController.$inject = ['$scope', '$http', '$filter','blockUI', 'suggest', 'propertyServices', 'cityServices','marketPriceServices'];
mogiApp.controller('marketController', MarketController);