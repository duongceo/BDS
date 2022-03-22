//agentListingController

var agentListingController = function ($scope, agentServices, suggest) {
    // variables
    $scope.Keyword = null;

    // public functions
    $scope.ResetKeyWord = function () {
        $scope.Keyword = null;
    };
    $scope.getAgentSuggest = function (q) {
        var url = mogiRoutes.Common.Suggest_Agent;
        return suggest.GetSuggestion(q, url);
    };

    $scope.searchAgent = function () {
        var url = getKeyword();
        window.location.href = url;
    };

    $scope.PhoneFormat = function (num) {
        return num.replace(/(\d{4})(\d{3})(\d+)/, '$1 $2 $3');
    };

    $scope.Cities = (function () {
        var all_loc = {
            i: 0,
            n: msg.AllLocation,
            p: 0,
            u: ''
        };

        var data = [all_loc].concat(mogiCities.data);

        var doSearch = function () {
            console.log($scope.Cities.Selected);
            var param = '';
            if ($scope.Cities.Selected.i > 0) {
                param = $scope.Cities.Selected.u + '-cid' + $scope.Cities.Selected.i;
            }
            var url = mogiRoutes.Agent.SearchAgent + param;
            window.location.href = url;;
        };
        return {
            OnClick: function (item, index) {
                this.Selected = item;
                this.DisplayText = item.n;
                if (item.c !== void (0)) {
                    if (index === 0 && this.IsShowBackButton()) {
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
            IsSelected: function (id) {

                if (this.Selected.i === void (0)) return false;
                return this.Selected.i === id;
            },
            ParrentItem: {},
            DisplayText: all_loc.n,
            IsShowBackButton: function () {
                return Object.keys(this.ParrentItem).length !== 0;

            },
            HasChild: function (item) {
                return (item.c !== void (0) && item.c.length > 0)
            },
            IsShowViewMore: function (item, index) {
                if ($scope.Cities.HasChild(item) === true) {
                    if (index === 0 && $scope.Cities.IsShowBackButton() === true) {
                        return false;
                    }
                    return true;
                }

                return false;
            },
            IsOpen: false,
            Init: function () {
                var item = cid === 0 ? all_loc : mogiCities.hash['c' + cid];
                if (item.p === 0) {
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

    // private functions

    function getKeyword() {
        console.log($scope.Keyword);
        // Keyword is object
        if ($scope.Keyword !== null && $scope.Keyword.Name !== void (0)) {
            if ($scope.Keyword.Url !== '') {
                return mogiRoutes.Agent.SearchAgent + $scope.Keyword.Url;
            } else {
                return mogiRoutes.Agent.SearchAgentKeyword + $scope.Keyword.Name;
            }

        } else {
            if ($scope.Keyword === void (0)) {
                return mogiRoutes.Agent.SearchAgent;
            }
            return mogiRoutes.Agent.SearchAgentKeyword + $scope.Keyword;
        }
    }


    var init = (function () {
        if (query !== null) {
            var f = ["Type", "Id", "Url"];
            var o = new suggest.SuggestionField();
            o.Name = query.Keyword;
            for (var i = 0; i < f.length; i++) {
                o[f[i]] = query[f[i]];
            }
            $scope.Keyword = o;
        }
        $scope.Cities.Init();
    }());

}
agentListingController.$inject = ['$scope', 'agentServices', 'suggest'];
mogiApp.controller('agentListingController', agentListingController);