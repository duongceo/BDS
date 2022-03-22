function HomeController($scope, $templateCache, suggest) {
	$scope.Keyword = undefined;
	$scope.OnKeyUp = function (e) {
        if (e.keyCode === 13) {
            $scope.DoSearch();
        }
    };
	$scope.KeywordOnSelected = function (item, model, label) {
		$scope.Keyword = item;
        //$scope.DoSearch();
	};
    $scope.GetMap = function (q) {
        return suggest.Map(q, null);
    };
	$scope.ResetKeyword = function () {
		this.Keyword = $scope.Keyword = undefined;
    };
    // v2:begin
    $scope.Cities = (function () {
        var all_loc = { i: 0, n: msg.AllLocation, p: 0, u: "" };
        var data = [all_loc].concat(mogiCities.data);
        return {
            OnClick: function (item, index) {
                this.Selected = item;
                this.DisplayText = item.n;
                if (item.c !== void (0)) {
                    if (index === 0 && this.IsShowBackButton()) {
                        this.IsOpen = false;
                        return;
                    }
                    this.Data = [item].concat(item.c);
                    this.ParrentItem = item;
                    this.IsOpen = true;
                } else {
                    this.IsOpen = false;
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
            GetValue: function () {
                if (this.Selected.i !== void (0)) {
                    return this.Selected;
                } else {
                    return null;
                }
            },
            ParrentItem: {},
            DisplayText: all_loc.n,
            IsShowBackButton: function () {
                return Object.keys(this.ParrentItem).length !== 0;

            },
            HasChild: function (item) {
                return (item.c !== void 0) && item.c.length > 0;
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
            Init: function (cid) {
                cid = cid || 0;
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

    $scope.PropertyType = (function () {
        var propertyType = mogiResUtils.filterUtils.getPropertyTypes();
        var data = [propertyType.Range[0]].concat(mogiResUtils.getPropertyTypeForSales(0));
        return {
            Data: mogiResUtils.getPropertyTypeForSales(0),
            ParrentItem: {},
            Selected: {},
            GetValue: function () {
                return Selected.Id || 0;
            },
            SetValue: function (item) {
                if (item === null || item.Id === void (0)) return;
                this.OnClick(item, 0);
                this.IsOpen = false;
            },
            GetPropertyTypeValue: function () {
                if (this.Selected.Id !== void (0))
                    return this.ParrentItem.Id || this.elected.Id;
                return 0;
            },
            GetPropertyStylesValue: function () {
                if (this.ParrentItem.Id !== void (0))
                    return this.Selected.Id !== void (0) ? [this.Selected.Id] : [];
                return [];
            },
            DisplayText: '',
            OnClick: function (item, index) {
                this.Selected = item;
                this.DisplayText = (item.Id === 0 ? propertyType.Title : item.Name);
                if (item.c !== void (0) && item.c.length > 0) {
                    if (index === 0 && this.IsShowBackButton()) {
                        this.IsOpen = false;
                        return;
                    }
                    this.Data = [item].concat(item.c);
                    this.ParrentItem = item;
                    this.IsOpen = true;
                } else {
                    this.IsOpen = false;
                }
            },
            OnClickBack: function (item) {
                this.Data = data;
                this.Selected = item;
                this.DisplayText = item.Name;
                this.ParrentItem = {};
                this.IsOpen = true;

            },
            HasChild: function (item) {
                return (item.c !== void (0) && item.c.length > 0);
            },
            IsSelected: function (id) {
                if (this.Selected.Id === void (0)) return false;
                return this.Selected.Id === id;
            },
            IsShowBackButton: function () {
                return Object.keys(this.ParrentItem).length !== 0;

            },
            IsShowViewMore: function (item, index) {
                if ($scope.PropertyType.HasChild(item) === true) {
                    if (index === 0 && $scope.PropertyType.IsShowBackButton() === true) {
                        return false;
                    }
                    return true;
                }

                return false;
            },
            IsOpen: false,
            Init: function () {
                this.Data = data;
                this.Label = propertyType.Title;
                $scope.PropertyType.DisplayText = propertyType.Title;
            },
            IsShowItem: function (item) {
                if (item.Id === 0) return true;
                if ($scope.Transaction.IsRent()) {
                    return item.Rent;
                } else {
                    return item.Sale;
                }
            },
            Reset: function () {
                this.ParrentItem = {};
                this.Selected = {};
                this.DisplayText = propertyType.Title;
                if ($scope.Transaction.IsRent() === true) {
                    data = [propertyType.Range[0]].concat(mogiResUtils.getPropertyTypeForRents(0));
                } else {
                    data = [propertyType.Range[0]].concat(mogiResUtils.getPropertyTypeForSales(0));
                }
                this.Data = data;
            }
        };
    }());

    $scope.Transaction = (function () {
        var isRent = false;
        let cache = {
            rent: null,
            sale: null
        };

        let cachePrevValue = function () {
            let obj = {
                PropertyType: $scope.PropertyType.Selected,
                ParrentItem: $scope.PropertyType.ParrentItem,
                PropertyTypeData: $scope.PropertyType.Data,
                PriceFrom: $scope.Price.SelectedFrom,
                PriceTo: $scope.Price.SelectedTo
            };

            if (isRent) {
                cache.rent = obj;
            } else {
                cache.sale = obj;
            }
        };

        let getPrevValue = function () {
            if (isRent)
                return cache.rent;
            return cache.sale;
        };

        return {
            IsRent: function () {
                return isRent;
            },
            SetValue: function (val) {
                if (isRent === val) return;

                cachePrevValue();

                isRent = val;
                $scope.PropertyType.Reset();
                $scope.Price.Reset();

                // Set previous value if any
                let prevObj = getPrevValue();
                console.log(prevObj);
                if (prevObj === null) return;
                $scope.PropertyType.SetValue(prevObj.PropertyType);
                $scope.PropertyType.Data = prevObj.PropertyTypeData;
                $scope.PropertyType.ParrentItem = prevObj.ParrentItem;
                $scope.Price.ChangedFrom(prevObj.PriceFrom);
                $scope.Price.ChangedTo(prevObj.PriceTo);

            },
            GetValue: function () {
                return isRent;
            }
        };
    }());

    $scope.DoSearch = function () {
        var p = {};
        p.Msg = mogiDatas.Msg;
        p.Rent = $scope.Transaction.GetValue();
        p.PropertyTypeId = $scope.PropertyType.GetPropertyTypeValue();
        p.PropertyStyles = $scope.PropertyType.GetPropertyStylesValue();
        p.Map = $scope.Keyword;
        // price
        p.FromPrice = $scope.Price.GetFromValue();
        p.ToPrice = $scope.Price.GetToValue();
        // area
        p.FromArea = 0;
        p.ToArea = 0;
        // direction
        p.DirectionId = 0;
        // legal
        p.LegalId = 0;
        // bed room
        p.FromBedRoom = 0;
        p.ToBedRoom = 0;
        p.Days = 0; // day
        p.Sort = 0; // sort
        // Location
        p.Location = $scope.Cities.GetValue();

        window.location.href = mogiUtils.friendlyUrl.getPropertySearchUrl(p);
    };

    $scope.PriceFormat = function (min, max, minArea) {
        if (min > 0) {
            if (max > 0) {
                var f1 = ((min % 1000000) > 0 ? 1 : 0);
                var f2 = ((max % 1000000) > 0 ? 1 : 0);
                min = min / 1000000;
                max = max / 1000000;
                var arr = [min.toFixed(f1), max.toFixed(f2)];
                return "" + arr.join(' - ') + " triệu/m<sup>2</sup>";
            } else if (minArea > 0) {
                var p = mogiResUtils.currencyUtils.priceToText(min * minArea);
                return "Giá từ " + p;
            } else { return "0"; }

        } else {
            return "0";
        }
    };

    $scope.Price = (function () {
        let label = msg.Price ;
        let getData = function () {
            let isRent = $scope.Transaction.GetValue();
            if (isRent) {
                data = mogiResUtils.filterUtils.getFilterPriceRent();
            } else {
                data = mogiResUtils.filterUtils.getFilterPriceSale();
            }
            $scope.Price.Items = data.Range;
        };

        return {
            Items: [],
            Label: '',
            DisplayName: '',
            SelectedFrom: {},
            SelectedTo: {},
            IsOpen: false,
            IsOpen2: false,
            UpdateDisplayName: function () {
                var f = this.SelectedFrom;
                var t = this.SelectedTo;
                var s = '';
                if (t.From === 0 && f.From === 0) {
                    s = this.Label;
                } else {
                    if (f.From === 0) {
                        s = '< ' + t.Name;
                    }
                    else if (t.From === 0) {
                        s = '> ' + f.Name;
                    } else {
                        s = f.Name + ' - ' + t.Name;
                    }
                }
                this.DisplayName = s;
            },
            GetFromValue: function () {
                return this.SelectedFrom !== null ? this.SelectedFrom.From : 0;
            },
            GetToValue: function () {
                return this.SelectedTo !== null ? this.SelectedTo.From : 0;
            },
            ChangedFrom: function (o) {
                var t = this.SelectedTo;
                if (t.Value < o.Value && t.Value > 0) {
                    this.SelectedFrom = t;
                    this.SelectedTo = o;
                } else {
                    this.SelectedFrom = o;
                }
                this.UpdateDisplayName();

                this.IsOpen = true;
            },
            ChangedTo: function (o, l) {
                this.IsOpen = false;
                var f = this.SelectedFrom;
                if (f.Value > o.Value && o.Value > 0) {
                    this.SelectedFrom = o;
                    this.SelectedTo = f;
                } else {
                    this.SelectedTo = o;
                }
                this.UpdateDisplayName();

                if (l) return;
            },
            IsSelectedFrom: function (o) {
                return (o.Id === this.SelectedFrom.Id);
            },
            IsSelectedTo: function (o) {
                return (o.Id === this.SelectedTo.Id);
            },
            Reset: function () {
                this.SelectedFrom = this.Items[0];
                this.SelectedTo = this.Items[0];
                this.UpdateDisplayName();
                getData();
            },
            Init: function () {
                getData();
                this.Label = label;
                this.DisplayName = label;
                this.SelectedFrom = this.Items[0];
                this.SelectedTo = this.Items[0];
                this.UpdateDisplayName();
            },
            GetSelectedText: function () {
                return (this.DisplayName === this.Label || this.DisplayName === this.Items[0].Name)
                    ? '' : this.DisplayName;
            }
        };
    }());
    // v2:end
	(function () {
		if (window.IsMobile === false) {
			$scope.TemplateSearchBarUrl = window.TemplateSearchBarUrl;
		}
		var t1 = window.home_searchbar;
		if ($m.object(t1) && $m.defined($templateCache.get(t1.url)) === false) {
			$templateCache.put(t1.url, t1.data);
		}
        $scope.Cities.Init();
        $scope.PropertyType.Init();
        $scope.Price.Init();
    }());
}
HomeController.$inject = ['$scope', '$templateCache', 'suggest'];
mogiApp.controller('HomeController', HomeController);