//buyingguideController
mogiApp.controller('buyingguideController',
    ['$scope', '$http', 'blockUI', 'suggest', 'cityServices',
    function ($scope, $http, blockUI, suggest, cityServices) {
        $scope.Toggle = (function () {
            var activeStep = "";
            var expands = {};
            var setActive = function (step) {
                var isExpand = expands[step];
                if (activeStep !== step && isExpand == true) {
                    activeStep = step;
                } else {
                    activeStep = "";
                }
            };
            var setExpand = function (step) {
                var isExist = expands[step];
                if (isExist == void (0)) {
                    expands[step] = true;
                } else {
                    expands[step] = !expands[step];
                }
                console.log(expands);
            };
            return {
                IsActive: function (step) {
                    return step === activeStep;
                    
                },
                IsExpand: function (step) {
                    var isExist = expands[step];
                    console.log('xxx');
                    if (isExist == true) {
                        return true;
                    }
                    return false;
                },
                OnClick: function (step) {
                    setExpand(step);
                    setActive(step);
                }

            };
        }());
        $scope.List = {
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
				cityServices.GetStreetById(sid).then(function (rp) {
					rp = rp.data;
                    if (rp.Status == true) {
                        //tham-khao-gia-{duong}-{quan}-d{districtId}-s{streetid}
                        var url = "/gia-nha-dat-";
                        url += rp.Data.CodeUrl + "-";
                        url += "-d" + objCity.i + "-s" + rp.Data.StreetId;
                        location.href = url;
                    }
                });
            },
            GetStreetSuggest: function (q) {
                return suggest.Street(q, null);
            },
            Values: [],

            Value: [],
            GotoDetail: function (item) {
                location.href = item.FrontEnd_SearchUrl;
            }
        };
        $scope.CheckList = (function () {
            var checklist = {};
            var key = "buyingguide";
            var saveLocalData = function (key, data) {
                if (typeof (Storage) !== "undefined") {
                    localStorage.setItem(key, data);
                } else {
                    console.log('Not support local storare');
                }
            };

            var getLocalData = function (key) {
                if (typeof (Storage) !== "undefined") {
                    return localStorage[key];

                } else {
                    console.log('Not support local storare');
                    return void (0);
                }
            };

            return {
                OnCheck: function (step) {

                    var v = checklist[step];
                    if (v === void (0)) {
                        checklist[step] = true;
                    } else {
                        checklist[step] = !checklist[step];
                    }
                    var localData = JSON.stringify(checklist);
                    saveLocalData(key, localData);
                },
                IsChecked: function (step) {
                    var v = checklist[step];
                    if (v === void (0)) {
                        return false;
                    } else {
                        return v;
                    }
                },
                Init: function () {
                    var localData = getLocalData(key);
                    if (localData !== void (0)) {
                        checklist = JSON.parse(localData);
                    }

                }
            };
        }());

        var init = (function () {
            $scope.CheckList.Init();
        }())
    }]);