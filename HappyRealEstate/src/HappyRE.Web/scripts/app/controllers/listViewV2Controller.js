var bds = {
    Data: null,
	Map: {},
    Search: function (f) {
        return $.ajax({
            dataType: "json",
            type: 'POST',
            url: mogiRoutes.Property.MapViewData,
            data: f,
            traditional: true
        });
    },
    SearchMap: function (polyenc) {
        filterData.polyenc = polyenc;
        var f = mogiUtils.friendlyUrl.mapSearchFilter(filterData);
        f = $.extend(f, filterData);
        return this.Search(f);
    },
    notifyChange: function (polyenc, isUserPolygon) {

    },
    SetData: function (resp) {
        this.Data = resp.docs;
        this.Paging.Total = resp.numFound;
    },
    LoadListView: function (o) {
        if (bds.Data === null) return;
        
		var self = this;
        if (!o) {
            o = this.Paging;
            o.PageIndex = 1;
		}
		data = bds.Data;
        var startIndex = Math.max(0, o.PageIndex - 1) * o.PageSize;
        var endIndex = startIndex + o.PageSize;
        var ids = [];
        var t = o.Total;
        for (var i = startIndex; i < t && i < endIndex; i++) {
            ids.push(data[i].id);
		}
        if (ids.length === 0) {
            self.$scope.Map.Data = [];
            self.$scope.$apply();
            return;
        }
        o.UpdateText();
        $.ajax({
            type: 'POST',
            url: mogiRoutes.Property.ListingByIds,
            data: { rent: filterData.Rent, total: o.Total, pageIndex: o.PageIndex, url: o.Url, ids: ids }
		}).done(function (resp, status, xhr) {
			self.$scope.Map.Data = [];
			resp.Data.forEach(function (v) { self.$scope.Map.Data.push(v); });
			self.$scope.$apply();
            bdsMap.hideLoading();
        }).fail(function () { });
    },
	AppendHtml: function (id, html) {
		var target = (typeof id === 'string' ? document.getElementById(id) : id);
		var $injector = angular.injector(['ng', 'mogiApp']),
			$compile = $injector.get('$compile'),
			$rootScope = $injector.get('$rootScope'),
			$scope = angular.element(document.getElementById('main')).scope();
		target.innerHTML = html;
		$compile(target)($scope);
		$scope.$apply();
		$scope.$digest();
	}
};

function ListViewV2Controller($scope, $templateCache, $http, suggest, propertyServices, commonServices) {
	$scope.IsMobile = function () {
		var w = $(window).width();
		return w < 960;
	};

    // Transaction type Sale|Rent
    var TransType = true;
    // Keyword
    $scope.Keyword = null;
    $scope.GetMap = function (q) {
        return suggest.Map(q, null);
	};
	$scope.selectMatch = function (index, event) {
		var e = event;
	};
    $scope.OnKeyUp = function (e) {
        if (e.keyCode === 13) {
            $scope.CallSearch();
        }
    };
	$scope.KeywordOnSelected = function (item, model, label) {
		$scope.Keyword = item;
        $scope.CallSearch();
    };
	$scope.ResetKeyWord = function () {
		this.Keyword = $scope.Keyword = undefined;
    };
    $scope.FilterData = [
	        { i: 1, icon: 'icon-mogi-place', n: 'Khu vực', val: function () { return $scope.Cities.GetSelectedText(); } },
	        { i: 2, icon: 'icon-mogi-house', n: 'Loại bất động sản', val: function () { return $scope.PropertyType.GetSelectedText(); } },
	        { i: 3, icon: 'icon-mogi-dollar', n: 'Giá', val: function () { return $scope.Price.GetSelectedText(); } },
	        { i: 4, icon: 'icon-mogi-square', n: 'Diện tích', val: function () { return $scope.Area.GetSelectedText(); } },
            { i: 5, icon: 'icon-mogi-bed', n: 'Số phòng', val: function () { return $scope.Room.GetSelectedText(); } },
            { i: 6, icon: 'icon-mogi-direction', n: 'Hướng', val: function () { return $scope.Direction.GetSelectedText(); } },
            { i: 7, icon: 'icon-mogi-legal', n: 'Pháp lý', val: function () { return $scope.Legal.GetSelectedText(); } },
            { i: 8, icon: 'icon-mogi-clock', n: 'Thời gian đăng', val: function () { return $scope.Time.GetSelectedText(); } }
    ];
    for (var i = 0; i < $scope.FilterData.length; i++) {
        $scope.FilterData[i].n = i;
    }

    // Filter - Master Items
    $scope.FilterMaster = (function () {
        var selectedObj = {};
        return {
			dialog: function () { return $('#filter-more'); },
            Data: $scope.FilterData,
            IsOpen: false,
            IsShow: true,
            IsPopUp: false,
            IsSingle: true,
            IsSelected: function (id) {
                return selectedObj.i === id;
            },
            FilterCount: function () {
                var selectedItems = this.Data.filter(function (item, index) {
                    return (item.i > 3) && (item.val() !== '');
                });
                return selectedItems.length > 0 ? selectedItems.length : '';
            },
            ShowMaster: function () {
                selectedObj = {};
                this.IsShow = true;
            },
            OpenPopUp: function () {
				selectedObj = {};
				this.dialog().modal({
                    backdrop: true,
                    keyboard: false
                });
                this.IsPopUp = true;
                this.IsShow = true;
                this.IsSingle = false;
            },
            Open: function (i, e) {
                if ($scope.IsMobile() === false) return;

                selectedObj = this.Data[i];
                this.IsPopUp = true;
                this.IsShow = false;
                this.IsOpen = true;
                this.IsSingle = true;
                this.dialog().modal({
                    backdrop: true,
                    keyboard: false
                });
            },
            OnClick: function (item, index) {
                this.IsShow = false;
                selectedObj = item;
                this.IsOpen = true;
                return;
            },
            Init: function () {
                this.dialog().on('hide.bs.modal', function () {
                    $scope.FilterMaster.IsPopUp = false;
                });
            },
            Reset: function () {
                $scope.Area.Reset();
                $scope.Room.Reset();
                $scope.Direction.Reset();
                $scope.Time.Reset();
                $scope.Legal.Reset();
            },
            Closed: function () {
				this.Hide();
                $scope.CallSearch();
            },
            Hide: function () {
                this.IsPopUp = false;
                this.dialog().modal("hide");
            }
        };
    }());

    // Filter - Location
    $scope.Cities = (function () {
        var filterCity = mogiResUtils.filterUtils.getCities();
        $scope.FilterData[0].n = filterCity.Title;

        var all_loc = { i: 0, n: mogiDatas.Msg.Listing_Filter_Country, p: 0, u: '' };
        var data = [all_loc].concat(mogiCities.data);
        return {
            f: $scope.FilterMaster,
            DoSearch: function () {
                if (this.f.IsSingle === true) {
                    this.f.Closed();
                }
            },
            OnClick: function (item, index) {
                this.Selected = item;
                this.DisplayText = item.n;
                // if item has child
                if (item.c !== void (0)) {
                    // not select all
                    if (index !== 0) {
                        this.Data = [item].concat(item.c);
                        this.ParentItem = item;
                        return;
                    }
                }

                this.IsOpen = false; // dropdown

                $scope.Keyword = null;

                if (this.f.IsSingle === false) {
                    this.f.ShowMaster();
                } else {
                    this.DoSearch();
                }
            },
            OnClickBack: function (item) {
                if (this.f.IsSingle === false && this.ParentItem.n === void (0)) {
                    this.f.ShowMaster();
                    return;
                }

                if (this.f.IsSingle === true && this.IsLeaf() === false) {
                    this.f.Hide();
                    return;
                }

                this.Data = data;
                this.Selected = item;
                this.DisplayText = item.n;
                this.ParentItem = {};
            },
            Data: data,
            IsOpen: false,
            Selected: {},
            IsSelected: function (id) {
                if (this.Selected.i == void (0)) return false;
                return this.Selected.i == id;
            },
            GetValue: function () {
                if (this.Selected.i != void (0)) {
                    return this.Selected;
                } else {
                    return null;
                }
            },
            ParentItem: {},
            DisplayText: all_loc.n,
            IsShowBack: function () {
                return (this.f.IsSingle === false) || this.IsLeaf();
            },
            IsLeaf: function () {
                return Object.keys(this.ParentItem).length !== 0;
            },
            HasChild: function (item) {
                return (item.c !== void (0) && item.c.length > 0);
            },
            IsShowChild: function (item, index) {
                if (this.HasChild(item) == true) {
                    return (index != 0);
                }
                return false;
            },
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
            GetSelectedText: function () {
                if (this.Selected.i === void (0) || this.Selected.i === 0) return '';
                return this.Selected.n;
            },
            GetName: function (o, i) {
                if (i === 0 && o.i > 0) {
                    return mogiDatas.Msg.Listing_Filter_All;
                }
                return o.n;
            },
            GetTitle: function (s) {
                if (this.ParentItem.n !== void (0)) return this.ParentItem.n;
                return s;
            },
            Reset: function () {
                this.Init();
            },
            SetValue: function (cid, did) {
                var loc = did !== 0 ? mogiResUtils.GetCityById(did) : mogiResUtils.GetCityById(cid);
                this.Selected = loc;
                this.DisplayText = loc.n;

                if (did === 0) {
                    this.Data = [loc].concat(loc.c);
                    this.ParentItem = loc;
                } else {
                    this.ParentItem = mogiResUtils.GetCityById(cid);
                    this.Data = [this.ParentItem].concat(this.ParentItem.c);;
                }
            }
        };
    }());

    // Filter - Price
    $scope.Price = (function () {
        var data = null;
        return {
            f: $scope.FilterMaster,
            Items: [],
            Label: '',
            DisplayName: '',
            SelectedFrom: {},
            SelectedTo: {},
            IsOpen: false,
            IsOpen2: false,
            OnClickBack: function (item) {
                if (this.f.IsSingle == false) {
                    this.f.ShowMaster();
                    return;
                }

                if (this.f.IsSingle == true) {
                    this.f.Hide();
                    return;
                }
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

                IsOpen = true;
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
                $scope.FilterMaster.Closed();
            },
            IsSelectedFrom: function (o) {
                return (o.Id == this.SelectedFrom.Id);
            },
            IsSelectedTo: function (o) {
                return (o.Id == this.SelectedTo.Id);
            },
            HasFilter: function () {
                return ((this.SelectedFrom != null && this.SelectedFrom.Id > 0) || (this.SelectedTo != null && this.SelectedTo.Id > 0));
            },
            UpdateDisplayName: function () {
                var f = this.SelectedFrom;
                var t = this.SelectedTo;
                var s = '';
                if (t.From == 0 && f.From == 0) {
                    s = this.Label;
                } else {
                    if (f.From == 0) {
                        s = '< ' + t.Name;
                    }
                    else if (t.From == 0) {
                        s = '> ' + f.Name;
                    } else {
                        s = f.Name + ' - ' + t.Name;
                    }
                }
                this.DisplayName = s;
            },
            Reset: function () {
                this.SelectedFrom = this.Items[0];
                this.SelectedTo = this.Items[0];
                this.UpdateDisplayName();
            },
            GetFromValue: function () {
                return this.SelectedFrom != null ? this.SelectedFrom.From : 0;
            },
            GetToValue: function () {
                return this.SelectedTo != null ? this.SelectedTo.From : 0;
            },
            SetValue: function (from, to) {
                if ((from || null) != null) {
                    var obj = _.find(this.Items, function (item) { return item.From == from; });
                    this.ChangedFrom(obj);
                }
                if ((to || null) != null) {
                    var obj = _.find(this.Items, function (item) { return item.From == to; });
                    this.ChangedTo(obj, true);
                }
            },
			Init: function (isRent) {
                if (isRent) {
                    data = mogiResUtils.filterUtils.getFilterPriceRent();
                } else {
                    data = mogiResUtils.filterUtils.getFilterPriceSale();
                }
                this.Items = data.Range;
                this.Label = data.Title;
                this.DisplayName = data.Title;
                this.SelectedFrom = this.Items[0];
                this.SelectedTo = this.Items[0];
                this.UpdateDisplayName();
                $scope.FilterData[2].n = this.Label;
            },
            GetSelectedText: function () {
                return (this.DisplayName == this.Label || this.DisplayName == this.Items[0].Name) ? '' : this.DisplayName;
            }
        };
    }());

    // Filter - PropertyType
    $scope.PropertyType = (function () {
        var propertyType = mogiResUtils.filterUtils.getPropertyTypes();
        var data = propertyType.Range;
        return {
            f: $scope.FilterMaster,
            Label: '',
            DisplayText: '',
            Data: data,
            ParentItem: {},
            Selected: {},
            DoSearch: function () {
                if (this.f.IsSingle == true) {
                    this.f.Closed();
                }
            },
            GetValue: function () {
                return Selected.Id || 0;
            },
            GetPropertyTypeValue: function () {
                if (this.Selected.Id !== void (0))
                    return this.ParentItem.Id || this.Selected.Id;
                return 0;
            },
            GetPropertyStylesValue: function () {
                if (this.ParentItem.Id !== void (0))
                    return this.Selected.Id != void (0) ? [this.Selected.Id] : [];
                return [];
            },
            OnClick: function (item, index) {
                this.Selected = item;
                this.DisplayText = (item.Id == 0 ? propertyType.Title : item.Name);
                if (item.c !== void (0) && item.c.length > 0) {
                    if (index != 0) {
                        this.Data = [item].concat(item.c);
                        this.ParentItem = item;
                        return;
                    }
                }

                this.IsOpen = false; // dropdown

                if (this.f.IsSingle == false) {
                    this.f.ShowMaster();
                } else {
                    this.DoSearch();
                }
            },
            OnClickBack: function (item) {
                if (this.f.IsSingle == false && this.ParentItem.Name == void (0)) {
                    this.f.ShowMaster();
                    return;
                }

                if (this.f.IsSingle == true && this.IsLeaf() == false) {
                    this.f.Hide();
                    return;
                }

                this.Data = data;
                this.Selected = item;
                this.DisplayText = item.Name;
                this.ParentItem = {};
            },
            HasChild: function (item) {
                return (item.c !== void (0) && item.c.length > 0)
            },
            IsShowChild: function (item, index) {
                if (this.HasChild(item) == true) {
                    return (index != 0);
                }
                return false;
            },
            IsSelected: function (id) {
                if (this.Selected.Id == void (0)) return false;
                return this.Selected.Id == id;
            },
            IsShowBack: function () {
                return (this.f.IsSingle == false) || this.IsLeaf();
            },
            IsLeaf: function () {
                return Object.keys(this.ParentItem).length > 0;
            },
            IsOpen: false,
            Init: function (isRent) {
                if (isRent) {
                    data = data.concat(mogiResUtils.getPropertyTypeForRents(0));
                } else {
                    data = data.concat(mogiResUtils.getPropertyTypeForSales(0));
                }
                this.Data = data;
                this.Label = propertyType.Title;
                this.DisplayText = propertyType.Title;
                $scope.FilterData[1].n = this.Label;
            },
            GetSelectedText: function () {
                if (this.Selected.Id == void (0) || this.Selected.Id == 0) return '';
                return this.Selected.Name;
            },
            GetName: function (o, i, s) {
                if (i == 0 && o.Id > 0) return s;
                return o.Name;
            },
            GetTitle: function () {
                return (this.ParentItem.Name || this.Label);
            },
            Reset: function () {
                this.ParentItem = {};
                this.Selected = {};
                this.DisplayText = data[0].Name;
            },
            SetValue: function (pid, psid) {

                if ((pid || null) == null) return;

                var p = _.find(this.Data, function (item) { return item.Id == pid; });

                var ps = null;

                if ((psid || null) != null) {
                    ps = _.find(p.c, function (item) { return item.Id == psid; });
                }

                if (ps != void (0) && ps.Id !== 0) {
                    this.Selected = ps;
                    this.Data = [p].concat(p.c);
                    this.ParentItem = p;
                    this.DisplayText = ps.Name;
                } else {
                    this.Selected = p;
                    this.DisplayText = p.Name;
                }

                //   debugger;
            }
        };
    }());

    // Filter - Room
    $scope.Room = (function () {
        var filterRoom = mogiResUtils.filterUtils.getFilterRoom();
        $scope.FilterData[4].n = filterRoom.Title;
        return {
            Items: filterRoom.Range,
            Label: filterRoom.Title,
            Selected: filterRoom.Range[0],
            Changed: function (o) {
                this.Selected = o;

            },
            IsSelected: function (o) {
                return (o.Id == this.Selected.Id);
            },
            HasFilter: function () {
                return (this.Selected != null && this.Selected.Id > 0);
            },
            Reset: function () {
                this.Selected = this.Items[0];
            },
            GetFromValue: function () {
                return this.Selected != null ? this.Selected.From : 0;
            },
            GetToValue: function () {
                return this.Selected != null ? this.Selected.To : 0;
            },
            SetValue: function (val) {
                if ((val || null) == null) return;
                var obj = _.find(this.Items, function (item) { return item.From == val; });
                this.Changed(obj);
            },
            OnClick: function (item) {
                this.Selected = item;
                $scope.FilterMaster.ShowMaster();
            },
            OnClickBack: function () {
                $scope.FilterMaster.ShowMaster();
            },
            GetSelectedText: function () {
                if (this.Selected.Id == void (0) || this.Selected.Id == 0) return '';
                return this.Selected.Name;
            },
        };
    }());

    // Filter - Direction
    $scope.Direction = (function () {
        var filterDirection = mogiResUtils.filterUtils.getDirections();
        filterDirection.Range = mogiResUtils.getDirections();
        $scope.FilterData[5].n = filterDirection.Title;
        return {
            Items: filterDirection.Range,
            Label: filterDirection.Title,
            Selected: filterDirection.Range[0],
            Changed: function (o) {
                this.Selected = o;
            },
            IsSelected: function (o) {
                return (o.Id == this.Selected.Id);
            },
            HasFilter: function () {
                return (this.Selected != null && this.Selected.Id > 0);
            },
            Reset: function () {
                this.Selected = this.Items[0];
            },
            GetValue: function () {
                return this.Selected != null ? this.Selected.Id : 0;
            },
            SetValue: function (val) {
                if ((val || null) == null) return;
                var obj = _.find(this.Items, function (item) { return item.Id == val; });
                this.Changed(obj);
            },
            OnClick: function (item) {
                this.Selected = item;
                $scope.FilterMaster.ShowMaster();
            },
            OnClickBack: function () {
                $scope.FilterMaster.ShowMaster();
            },
            GetSelectedText: function () {
                if (this.Selected.Id == void (0) || this.Selected.Id == 0) return '';
                return this.Selected.Name;
            },
        };
    }());

    // Filter - Time
    $scope.Time = (function () {
        var filterTime = mogiResUtils.filterUtils.getFilterTime();
        $scope.FilterData[7].n = filterTime.Title;
        return {
            Items: filterTime.Range,
            Label: filterTime.Title,
            Selected: filterTime.Range[0],
            Changed: function (o) {
                this.Selected = o;
            },
            IsSelected: function (o) {
                return (o.Id == this.Selected.Id);
            },
            HasFilter: function () {
                return (this.Selected != null && this.Selected.Id > 0);
            },
            Reset: function () {
                this.Selected = this.Items[0];
            },
            GetValue: function () {
                return this.Selected != null ? this.Selected.Value : 0;
            },
            SetValue: function (val) {
                if ((val || null) == null) return;
                var obj = _.find(this.Items,
                    function (item) {
                        return item.Value == val;
                    });
                this.Changed(obj);
            },
            OnClick: function (item) {
                this.Selected = item;
                $scope.FilterMaster.ShowMaster();
            },
            OnClickBack: function () {
                $scope.FilterMaster.ShowMaster();
            }, GetSelectedText: function () {
                if (this.Selected.Id == void (0) || this.Selected.Id == 0) return '';
                return this.Selected.Name;
            },
        };
    }());

    // Filter - Area
    $scope.Area = (function () {
        var filterArea = mogiResUtils.filterUtils.getFilterArea();
        $scope.FilterData[3].n = filterArea.Title;

        return {
            Items: filterArea.Range,
            Label: filterArea.Title,
            Selected: filterArea.Range[0],
            Changed: function (o) {
                this.Selected = o;
            },
            IsSelected: function (o) {
                return (o.Id == this.Selected.Id);
            },
            HasFilter: function () {
                return (this.Selected != null && this.Selected.Id > 0);
            },
            Reset: function () {
                this.Selected = this.Items[0];
            },
            GetFromValue: function () {
                return this.Selected != null ? this.Selected.From : 0;
            },
            GetToValue: function () {
                return this.Selected != null ? this.Selected.To : 0;
            },
            SetValue: function (f, t) {
                f = (f || 0); t = (t || 0);
                var obj = _.find(this.Items, function (o) { return (o.From == f && o.To == t); });
                if (obj == null) return;
                this.Changed(obj);
            }, OnClick: function (item) {
                this.Selected = item;
                $scope.FilterMaster.ShowMaster();
            },
            OnClickBack: function () {
                $scope.FilterMaster.ShowMaster();
            }, GetSelectedText: function () {
                if (this.Selected.Id == void (0) || this.Selected.Id == 0) return '';
                return this.Selected.Name;
            },
        };
    }());

    // Filter - Legal
    $scope.Legal = (function () {
        var filterLegal = mogiResUtils.filterUtils.getLegals();
        filterLegal.Range = mogiResUtils.getLegals();
        $scope.FilterData[6].n = filterLegal.Title;
        return {
            Items: filterLegal.Range,
            Label: filterLegal.Title,
            Selected: filterLegal.Range[0],
            Changed: function (o) {
                this.Selected = o;
            },
            IsSelected: function (o) {
                return (o.Id == this.Selected.Id);
            },
            HasFilter: function () {
                return (this.Selected != null && this.Selected.Id > 0);
            },
            Reset: function () {
                this.Selected = this.Items[0];
            },
            GetValue: function () {
                return this.Selected != null ? this.Selected.Id : 0;
            },
            SetValue: function (val) {
                if ((val || null) == null) return;
                var obj = _.find(this.Items, function (item) { return item.Id == val; });
                this.Changed(obj);
            }, OnClick: function (item) {
                this.Selected = item;
                $scope.FilterMaster.ShowMaster();
            },
            OnClickBack: function () {
                $scope.FilterMaster.ShowMaster();
            }, GetSelectedText: function () {
                if (this.Selected.Id == void (0) || this.Selected.Id == 0) return '';
                return this.Selected.Name;
            },
        };
    }());

    // Sort
    var lstSort = mogiResUtils.syscodeUtils.getRefilterSort();
    lstSort.map(function (o) { o.Id = o.i; o.Name = o.n; o.Code = o.c; });
    $scope.SortView = {
        Items: lstSort,
        Selected: null,
        Changed: function (o) {
            this.Selected = o;
            $scope.CallSearch();
        },
        // View
        SelectedView: 'list',
        ChangedView: function (v) {
            this.SelectedView = v;
            $.cookie('view', '', { expires: -1, path: '/' });
            $.cookie('view', '', { expires: -1, path: location.pathname });
            if (v == 'map') {
                $.cookie('view', v, { path: '/' });
            }
            location.reload();
        },
        GetViewValue: function () {
            return this.SelectedView;
        },
        GetSortValue: function () {
            return (this.Selected == null ? '' : this.Selected.Code);
        },
        Init: function (fd) {
            var o = null;
            if (fd.s) {
                o = _.find(this.Items, function (i) { return i.Code == fd.s });
            }
            if (o === null) o = this.Items[0];
            this.Selected = o;
        }
    };

    // Click Tìm kiếm 
    $scope.CallSearch = function () {
        if ($scope.FilterMaster.IsPopUp == true) {

            $scope.FilterMaster.Closed();
        }
        var p = {};
        p.Msg = mogiDatas.Msg;
        p.Rent = TransType;
        p.PropertyTypeId = $scope.PropertyType.GetPropertyTypeValue();
        p.PropertyStyles = $scope.PropertyType.GetPropertyStylesValue();
        p.Map = $scope.Keyword;
        // price
        p.FromPrice = $scope.Price.GetFromValue();
        p.ToPrice = $scope.Price.GetToValue();
        // area
        p.FromArea = $scope.Area.GetFromValue();
        p.ToArea = $scope.Area.GetToValue();
        // direction
        p.DirectionId = $scope.Direction.GetValue();
        // legal
        p.LegalId = $scope.Legal.GetValue();
        // bed room
        p.FromBedRoom = $scope.Room.GetFromValue();
        p.ToBedRoom = $scope.Room.GetToValue();
        p.Days = $scope.Time.GetValue(); // day
        // Location
        p.Location = $scope.Cities.GetValue();

        p.Sort = $scope.SortView.GetSortValue(); // sort

        var url = mogiUtils.friendlyUrl.getPropertySearchUrl(p);
        window.location.href = url;
    };

   

    //phone number 
    $scope.Phone = (function () {
		var h = {};
		var total = 0;
		return {
			GetPhoneNo: function (n, e) {
				total++;
				console.log("GetPhoneNo:" + total);
				if (h[n]) return n;
				return n.substring(0, n.length - 2) + 'xx';
			},
			ShowPhone: function (n,e) {
				h[n] = true;
			}
		};
    }());

    var owl = $('#owl-top-service');
    owl.pageIndex = 1;
    owl.pageSize = 10;
    owl.loading = false;
    owl.end = false;
    $scope.CarouselOptions = {
        margin: 0,
        loop: false,
        navText: ['&#x2039;', '&#x203a;'],
        nav: true,
        dots: false,
        // items: 3,
        //slideBy: 2,
        responsive: {
			0: { items: 1, margin: 10, stagePadding: 50, nav: true },
			375: { items: 2, margin: 10, nav: true },
			425: { items: 2, margin: 10, stagePadding: 30, nav: true },
			640: { items: 3, margin: 10, stagePadding: 30, nav: false },
			768: { items: 3, margin: 20 }
        },
        onChange: function (data) {
            var o = this;
            var m = o.maximum();
            var prev = (data.property.name === "position" && data.property.value < data.item.index);
            if (prev === true || owl.end === true || owl.loading === true || o._current < (m - 1)) {
                return;
            }
            owl.loading = true;
            //if (m == 14) {
            //    owl.pageIndex = 2;
            //    owl.pageSize = 15;
            //} else {
            owl.pageIndex += 1;
            //}
            var f = filterData;
			var cityId = f.did === 0 ? f.cid : f.did;
            var p = { id: cityId, rent: f.Rent, pageIndex: owl.pageIndex, pageSize: owl.pageSize };
            propertyServices.TopService(p).then(function (res) {
                if (res) {
                    var pos = o.maximum();
                    var items = $(res.data);
                    if (items.length > 0) {
                        for (var i = 0; i < items.length; i++) {
                            if (items[i].tagName === 'DIV') o.add(items[i]);
                        }
                        o.to(pos, 200, true);
                    }
                    owl.end = items.length === 0;
                } else {
                    owl.end = true;
                }
                owl.loading = false;
            });
        }
    };

	$scope.ShowAlertSearch = function () {
		if (MOGI.IsAuth === false) {
			document.location.assign(mogiRoutes.loginUrl + '?returnUrl=' + encodeURIComponent(location.href));
			return;
		}
		var f = mogiUtils.friendlyUrl.mapSearchFilter(filterData);
		f = $.extend(f, filterData);
		f.json = JSON.stringify(filterData);
		var a = $scope.$parent.Profile.Data.Alert;
		if (a !== null) { $m.map(a, f); }
		commonServices.AlertSearch(f, function (resp) {
			if (resp) location.reload();
		});
	};
    // Map
    $scope.Paging = {
        PageIndex: 1, PageSize: 30, Total: 0, Url: '',
        Init: function (d) {
            this.Url = d.Url;
            this.Set(d.PageIndex, d.Total);
            bds.Paging = this; // export
        },
        Set: function (i, t) {
            this.PageIndex = (i || 1);
            this.Total = t;
            this.UpdateText();
        },
        UpdateText: function () {
            var p = this;
            var s = '';
            if (p.Total < 0) {
                s = '';
            }
            else if (p.Total === 0) {
                s = mogiDatas.Msg.Listing_Result_NotFound;
            } else {
                var v1 = 1 + Math.max(0, (p.PageIndex - 1)) * p.PageSize;
                var v2 = Math.min(p.Total, v1 + p.PageSize - 1);
                s = mogiDatas.Msg.Listing_ResultMap.format(v1, v2, p.Total);
            }
            $('#total-result').html(s);
        },
        Go: function (i, e) {
            this.PageIndex = i;
            bds.LoadListView(this, e);
            if (e) { e.preventDefault(); }
        }
    };
	$scope.Map = {
		Data: [],
		Init: function () {
			var f = mogiUtils.friendlyUrl.mapSearchFilter(filterData);
			f = $.extend(f, filterData);
			f["Msg"] = null; f["Map"] = null;
			bds.Search(f).success(function (resp) {
				var data = resp;
				bds.SetData(data["response"]);
				bds.LoadListView();
				var myTime = setInterval(function () {
					if (bdsMap.inited === true) {
						clearInterval(myTime);
						bdsMap.UpdatePolygon(data["polyenc"]);
						bdsMap.loadPins(data["response"]);
					}
				}, 10);
				ignorePolygonChanges = false;
			});
		},
		ShowMobile: function (o) { o.MobileView = o.User_Mobile; }
	};

	$scope.SearchPropType = {
		PropTypes: [],
		LocationText: '',
		Bottom: [],
		ShowBottom: false,
		Init: function (rent, pid, psid) {
			pid = pid || 0;
			psid = psid || [];
			this.PropTypes = rent ? mogiResUtils.getPropertyTypeForRents(0) : mogiResUtils.getPropertyTypeForSales(0);
			var c = $scope.Cities.GetValue();
			if ($m.object(c) && c.i !== 0) {
				if ($m.object($scope.Keyword) && $scope.Keyword.MapId > 0) {
					this.LocationText = $scope.Keyword.Name.split(',')[0];
				} else {
					this.LocationText = c.n;
				}
			}
			this.getBottom(pid, psid);
			this.getRight(pid, psid);
		},
		getUrl: function (o) {
			var p = {};
			p.Msg = mogiDatas.Msg;
			p.Rent = TransType;
			p.Location = $scope.Cities.GetValue();
			p.Map = $scope.Keyword;
			if (o.p > 0) {
				p.PropertyTypeId = p.p;
				p.PropertyStyles = [o.Id];
			} else {
				p.PropertyTypeId = o.Id;
			}
			return mogiUtils.friendlyUrl.getPropertySearchUrl(p);
		},
		getRight: function (pid, psid) {
			var items = [];
			if (pid === 0) {
				items = this.PropTypes;
			} else if (psid.length === 0) {
				var p = _.find(this.PropTypes, function (o) { return o.Id === pid; });
				items = p.c;
			}
			for (var i = 0, l = items.length; i < l; i++) {
				var url = this.getUrl(items[i]);
				this[items[i].Id] = { n: items[i].FName, u: url };
			}
		},
		getBottom: function (pid, psid) {
			var items = [];
			if (pid > 0) {
				if (psid.length === 0) {
					items = this.PropTypes;
				} else {
					var p = _.find(this.PropTypes, function (item) { return item.Id === pid; });
					items = p.c;
				}
			}
			var n = ' ' + this.LocationText;
			for (var i = 0, l = items.length; i < l; i++) {
				var o = items[i];
				var url = this.getUrl(o);
				this.Bottom.push({ n: o.FName + n, u: url });
			}
			this.ShowBottom = this.Bottom.length > 0;
		}
	};
	$scope.TemplateUrl = window.TemplateSearchBarUrl;
    (function () {
		var fd = filterData;
		var template = window.property_searchbar;
		if ($m.object(template) && $m.defined($templateCache.get(template.url)) === false) {
				$templateCache.put(template.url, template.data);
		}

		var searchresult = { Url: pageData.Url, Ids: pageData.Ids };
		$cache.setJson('mogi:searchresult', searchresult);

        //console.log(filterData);
        $scope.Price.Init(fd.Rent);
        $scope.Cities.Init();
        $scope.FilterMaster.Init();
        $scope.PropertyType.Init(fd.Rent);
		$scope.Favorite.init(pageData.Ids || []);
        TransType = fd.Rent;

        $scope.Price.SetValue(fd.fp, fd.tp);
        $scope.Area.SetValue(fd.fa, fd.ta);
        $scope.Room.SetValue(fd.tbr);
        $scope.Legal.SetValue(fd.lg);
        $scope.Direction.SetValue(fd.dt);
        $scope.Time.SetValue(fd.d);
        $scope.SortView.Init(fd);
        $scope.PropertyType.SetValue(fd.pid, fd.psid);

        if (fd.Map !== null) {
            var m = new suggest.MapField();
            var f = ["MapId", "ReferTypeId", "ReferId", "CityId", "DistrictId", "WardId", "StreetId", "ProjectId", "PlaceId", "CodeUrl", "Name"];
            for (var i = 0; i < f.length; i++) {
                m[f[i]] = fd.Map[f[i]];
            }
            $scope.Keyword = m;
            $scope.Cities.SetValue(m.CityId, m.DistrictId);

        } else {
            $scope.Keyword = fd.q;
        }
        $scope.FilterMaster.FilterCount();

        $scope.SearchPropType.Init(fd.Rent, fd.pid, fd.psid);
	}());

    // ward review
    $scope.WardReviewOptions = {
        margin: 0,
        loop: false,
        navText: ['&#x2039;', '&#x203a;'],
        nav: true,
        dots: false,
        // items: 3,
        //slideBy: 2,
        responsive: {
            0: { items: 1, margin: 10, stagePadding: 25, nav: true },
            375: { items: 1, margin: 16, nav: true, stagePadding: 55},
            425: { items: 1, margin: 16, stagePadding: 72, nav: true },
            640: { items: 2, margin: 16, stagePadding: 41, nav: true },
            768: { items: 2, margin: 16, stagePadding: 101},
            992: { items: 2, margin: 16, stagePadding: 49 },
            1170: { items: 2, margin: 16, stagePadding: 82 }
        }
    };

    if (pageData.IsMapView === true) {
        bdsMap.notifySearchResult = function (t, i) {
            $scope.Paging.Set(i, t);
        };
        $scope.Paging.Init(pageData.Paging);
        $scope.Map.Init();
	}
	bds.$scope = $scope;
}
ListViewV2Controller.$inject = ['$scope', '$templateCache', '$http', 'suggest', 'propertyServices', 'commonServices'];
mogiApp.controller('ListViewV2Controller', ListViewV2Controller)
.directive('searchBox', function ($templateCache) {
		return {
			restrict: 'E',
			replace: true,
			//controller: 'ListViewV2Controller',
			scope: {
				boxKeyword: '@',
				boxplaceHolder: '@',
				boxSelectAll: '@',
				boxFilterMore: '@',
				boxSearch: '@'
				//init: '&init',
				//Cities: '=cities',
				//PropertyType: '=propertyType'

			},
			link: function (scope, element, attrs) {
				//scope.init();
			},
			templateUrl: '/scripts/app/templates/property-listing-search.html'
		};
});
