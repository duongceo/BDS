mogiApp.controller('studentController', ['$scope', 'blockUI',
    function ($scope, blockUI) {
        $scope.Query = {
            keyword: '',
            cid: 0,
            did: 0,
            PageIndex: 1,
            PageSize: 20
        };

        $scope.Keyword = null;
        $scope.ResetKeyWord = function () {
            $scope.Keyword = null;
            $scope.Query.keyword = $scope.Keyword;
            $scope.School.Filter();
        };
        $scope.OnKeyUp = function (e) {
            $scope.Query.PageIndex = 1;
            $scope.Query.keyword = $scope.Keyword;
            $scope.School.Filter();
        };

        $scope.IsMobile = function () {
            var w = $(window).width();
            return w < 960;
        }

        $scope.Cities = (function () {
            var all_loc = {
                i: 0,
                n: 'Toàn quốc',
                p: 0,
                u: ''
            };
            var hcm = mogiCities.hash["c30"];
            var hn = mogiCities.hash["c24"];
            //var data = [all_loc].concat(mogiCities.data);
            var data = [all_loc, hcm, hn]

            var doSearch = function () {
                console.log($scope.Cities.Selected);
                if ($scope.Cities.Selected !== null) {
                    $scope.Query.cid = $scope.Cities.Selected.p == 0 ? $scope.Cities.Selected.i : $scope.Cities.Selected.p;
                    $scope.Query.did = $scope.Cities.Selected.p == 0 ? 0 : $scope.Cities.Selected.i;
                    $scope.Query.PageIndex = 1;
                    $scope.School.Filter();
                }
                else {
                    $scope.School.Filter();
                }
            }
            return {
                dialog: $('#filter-more'),
                OnClick: function (item, index) {
                    this.Selected = item;
                    this.DisplayText = item.n;
                    if (item.c !== void (0)) {
                        if (index == 0 && this.IsShowBackButton()) {
                            this.IsOpen = false;
                            doSearch();
                            this.dialog.modal("hide");
                            return;
                        }
                        this.Data = [item].concat(item.c);
                        this.ParentItem = item;
                        this.IsOpen = true;

                    } else {
                        this.IsOpen = false;
                        this.dialog.modal("hide");
                        doSearch();
                    }
                },
                OnClickBack: function (item) {
                    this.Data = data;
                    this.Selected = item;
                    this.DisplayText = item.n;
                    this.ParentItem = {};
                    this.IsOpen = true;

                },
                Data: data,
                Selected: {},
                IsSelected: function (id) {
                    if (this.Selected.i == void (0)) return false;
                    return this.Selected.i == id;
                }, GetValue: function () {
                    return this.Selected.i || 0;
                },
                ParentItem: {},
                DisplayText: all_loc.n,
                IsShowBackButton: function () {
                    return Object.keys(this.ParentItem).length !== 0;
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

                    var cid = 0;
                    var item = cid == 0 ? all_loc : mogiCities.hash['c' + cid];
                    if (item.p == 0) {
                        this.Data = data;


                    } else {
                        var pItem = mogiCities.hash['c' + item.p];
                        this.Data = [pItem].concat(pItem.c);
                        this.ParentItem = pItem;
                    }
                    this.Selected = item;
                    this.DisplayText = item.n;
                },
                GetValue: function () {
                    if (this.Selected.i != void (0)) {
                        return this.Selected;
                    } else {
                        return null;
                    }
                },
                Show: function (e) {
                    if ($scope.IsMobile() == false) return;
                    this.dialog.modal({
                        backdrop: true,
                        keyboard: false
                    });
                },
                GetSelectedText: function () {
                    if (this.Selected.i == void (0) || this.Selected.i == 0) return '';
                    return this.Selected.n;
                },
                GetName: function (o, i) {
                    if (i == 0 && o.i > 0) {
                        return mogiDatas.Msg.Listing_Filter_All;
                    }
                    return o.n;
                },
                GetTitle: function (s) {
                    if (this.ParentItem.n !== void (0)) return this.ParentItem.n;
                    return s;
                },
            };
        }());

        var schools = mogiColleges;

        $scope.School = (function () {
            //var schools = [];

            return {
                Items: schools,
                GetData: function () {
                    var startIndex = ($scope.Query.PageIndex - 1) * $scope.Query.PageSize;
                    var endIndex = startIndex + $scope.Query.PageSize;
                    endIndex = endIndex < $scope.School.Items.length ? endIndex : $scope.School.Items.length;
                    return $scope.School.Items.slice(startIndex, endIndex);
                },
                PageChanged: function () {
                    console.log($scope.Query.PageIndex);
                },
                Filter: function () {
                    var filter = {};
                    var data = schools;
                    if ($scope.Query.cid !== void (0) && $scope.Query.cid > 0) {
                        filter.cid = $scope.Query.cid;
                    }
                    if ($scope.Query.did !== void (0) && $scope.Query.did > 0) {
                        filter.did = $scope.Query.did;
                    }
                    if (filter.did !== void (0) || filter.cid !== void (0)) {
                        data = _.where(schools, filter);
                    }
                    console.log(filter);
                    console.log($scope.Query.keyword);
                    if ($scope.Query.keyword !== void (0) && $scope.Query.keyword !== null && $scope.Query.keyword != '') {

                        var escapedSearchText = $scope.Query.keyword;
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

                        data = _.filter(data, function (school) {
                            return regex.test(school.n);
                            //return school.n.toUpperCase().indexOf($scope.Query.keyword.toUpperCase()) >= 0;
                        });
                    }

                    $scope.School.Items = data;
                },
            }
        }());

    }]);