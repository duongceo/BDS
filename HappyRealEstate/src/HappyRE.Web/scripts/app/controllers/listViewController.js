var bds = {
    Data: null,
    Paging: {},
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
        if (o == null) {
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
        o.UpdateText();
        $.ajax({
            type: 'POST',
            url: mogiRoutes.Property.ListViewByIds,
            data: { rent: filterData.Rent, total: o.Total, pageIndex: o.PageIndex, url: o.Url, ids: ids }
        }).done(function (resp, status, xhr) {
            bds.AppendHtml('list-view', resp);
            bdsMap.hideLoading();
        }).fail(function () { });
    },
    AppendHtml: function (id, html) {
        var target = (typeof id === 'string' ? document.getElementById(id) : id);
        var el = document.getElementById('list-view');
        $injector = angular.injector(['ng']),
        $compile = $injector.get('$compile'),
        $rootScope = $injector.get('$rootScope'),
        $scope = angular.element(el).scope();
        target.innerHTML = html;

        var f = $(target).find('#pagedata-favorite').html();
        $scope.Favorite.Init(JSON.parse(f || ''));

        $compile(target)($scope);
        $scope.$digest();

        $('#content-left').scrollTop(0);
    }
};
function ListViewController($scope, $http, blockUI, suggest, propertyServices, commonServices) {

    $scope.Screen = {
        Max640: mogiUtils.IsResponsive_Max640,
        Max768: mogiUtils.IsResponsive_Max768,
        Max991: mogiUtils.IsResponsive_Max991
    };

    $scope.Utils = {
        el: $('#keyword'),
        focus: function () {
            if ($scope.Screen.Max640()) {
                $scope.Utils.el.css('width', '100%');
            }
        },
        blur: function () {
            $scope.Utils.el.css('width', '');
        }
    }

    $scope.Favorite = {
        Items: {},
        Init: function (arr) {
            arr = (arr || []);
            for (var i = 0; i < arr.length; i++) {
                var v = arr[i];
                if (v != null) {
                    this.Items[parseInt(v)] = true;
                }
            }
        },
        Saved: function (id) {
            return (this.Items[id] || false);
        },
        AddRemove: function (id, e) {
            var self = this;
            var f = this.Saved(id);
            if (f == false) {
                this.Items[id] = true;
                propertyServices.AddFavorite(id).then(function () { });
            }
            else {
                delete this.Items[id];
                propertyServices.RemoveFavorite(id).then(function () { });
            }
            $(e.target).toggleClass('isfavorited');
            $(e.target).parent().toggleClass('favorited');
        }
    };

    // Keyword
    $scope.Keyword = null;
    $scope.GetMap = function (q) {
        return suggest.Map(q, null);
        //return suggest.Map(q, $scope.SetMapData).then(function (resp) {
        //    return suggest.Map_MapObject(q, resp);
        //});

    };

    $scope.OnKeyUp = function (e) {
        if (e.keyCode == 13) {
            $scope.CallSearch();
        }
    };
    $scope.KeywordOnSelected = function () {
        $scope.CallSearch();
    };

    // TransType - Loại giao dịch
    var filterTransType = mogiResUtils.filterUtils.getTransTypes();
    filterTransType.Range = mogiDatas.TransTypes;
    $scope.TransType = {
        Items: mogiDatas.TransTypes,
        Label: filterTransType.Title,
        Selected: mogiDatas.TransTypes[0],
        Changed: function (o) {
            this.Selected = o;
            $scope.PropType.Reset(o);
            $scope.Price.Reset();
        },
        IsSelected: function (o) {
            return (o.Name == this.Selected.Name);
        },
        GetPropTypes: function (o) {
            var res = null;
            if (this.Selected.Id == false) {
                res = mogiResUtils.getPropertyTypeForRents(0);
            } else {
                res = mogiResUtils.getPropertyTypeForSales(0);
            }
            res.splice(0, 0, o);
            return res;
        },
        GetPrices: function () {
            var res = [];
            if (this.Selected.Id == false) {
                res = mogiResUtils.filterUtils.getFilterPriceRent();
            } else {
                res = mogiResUtils.filterUtils.getFilterPriceSale();
            }
            return res;
        },
        GetValue: function () {
            return (this.Selected != null && this.Selected.Id);
        },
        SetValue: function (val) {
            var obj = _.find(this.Items, function (item) { return item.Id == val; });
            this.Changed(obj);
        }
    };

    // PropertyStyle - Kiểu bất động sản
    $scope.PropStyle = {
        Items: [],
        Label: '',
        Selected: [],
        HSelected: {},
        Changed: function (o) {
            if (this.HSelected[o.Id] == null) {
                this.Selected.push(o);
                this.HSelected[o.Id] = o;
            } else {
                delete this.HSelected[o.Id];
                var obj = _.reject(this.Selected, function (el) { return el.Id === o.Id; });
                this.Selected = obj;
            }
            $scope.PropType.UpdateDisplayName();
        },
        IsSelected: function (o) {
            var m = this.HSelected[o.Id];
            return (m != null && m.Id == o.Id);
        },
        Reset: function (p) {
            this.Selected = [];
            this.HSelected = {};
            this.Items = (p.Sale ? _.where(p.c, { Sale: true }) : _.where(p.c, { Rent: true }));
        },
        GetValue: function () {
            var arr = [];
            $.each(this.Selected, function (index, value) {
                arr.push(value.Id);
            });
            return arr;
        },
        SetValue: function (val) {
            if ((this.Items || null) == null) return;
            var arr = [];
            for (var i = 0; i <= this.Items.length - 1; i++) {
                var o = this.Items[i];
                var obj = _.find(val, function (item) {
                    return item == o.Id;
                });
                if (obj != null) {
                    this.Changed(o);
                }
            }

        }
    };

    // PropertyType - Loại bất động sản
    var filterPropType = mogiResUtils.filterUtils.getPropertyTypes();
    var filterPropTypeDefault = filterPropType.Range[0];
    filterPropType.Range = $scope.TransType.GetPropTypes(filterPropTypeDefault);
    $scope.PropType = {
        Items: filterPropType.Range,
        Label: filterPropType.Title,
        Selected: filterPropType.Range[0],
        Default: filterPropTypeDefault,
        DisplayName: filterPropType.Title,
        IsOpen: false,
        IsOpen2: false,
        Close: function () {
            this.IsOpen = false;
            this.IsOpen2 = false;
        },
        Changed: function (o) {
            this.Selected = o;
            $scope.PropStyle.Reset(o);
            this.UpdateDisplayName();
            if ($scope.Screen.Max768()) {
                $scope.FilterMore.ShowTotal();
            }
        },
        IsSelected: function (o) {
            return (o.Name == this.Selected.Name);
        },
        HasChild: function (o) {
            return (o.c != null && o.c.length > 0 && o.Id == this.Selected.Id);
        },
        HasFilter: function () {
            return (this.Selected != null && this.Selected.Id > 0);
        },
        UpdateDisplayName: function () {
            var m = $scope.Screen.Max768();
            if (this.Selected.Id == 0) {
                this.DisplayName = (m ? this.Selected.Name : this.Label);
                return;
            }
            var s = this.Selected.Name;
            var items = $scope.PropStyle.Selected;
            if (items.length > 0) {
                s += ' - ';
                for (var i = 0; i < items.length; i++) {
                    s += (i == 0 ? '' : ',') + items[i].Name;
                }
            }
            this.DisplayName = s;
            return s;
        },
        Reset: function (o) {
            if (o) {
                var items = [];
                if (o.Id == true) {
                    items = mogiResUtils.getPropertyTypeForSales(0);
                }
                else {
                    items = mogiResUtils.getPropertyTypeForRents(0);
                }
                items.splice(0, 0, this.Default);
                this.Items = items;
            }
            this.Selected = this.Items[0];
            $scope.PropStyle.Reset(this.Selected);
            this.UpdateDisplayName();
        },
        GetValue: function () {
            return this.Selected != null ? this.Selected.Id : 0;
        },
        SetValue: function (val) {
            if ((val || null) == null) return;
            var obj = _.find(this.Items, function (item) { return item.Id == val; });
            this.Changed(obj);
        }
    };
    $scope.PropStyle.Reset($scope.PropType.Selected);

    // Price - Khoảng giá
    var filterPrice = $scope.TransType.GetPrices();
    $scope.Price = {
        Items: filterPrice.Range,
        Label: filterPrice.Title,
        DisplayName: filterPrice.Title,
        SelectedFrom: filterPrice.Range[0],
        SelectedTo: filterPrice.Range[0],
        IsOpen: false,
        IsOpen2: false,
        ChangedFrom: function (o) {
            var t = this.SelectedTo;
            if (t.Value < o.Value && t.Value > 0) {
                this.SelectedFrom = t;
                this.SelectedTo = o;
            } else {
                this.SelectedFrom = o;
            }
            this.UpdateDisplayName();
            if ($scope.Screen.Max991()) {
                $scope.FilterMore.ShowTotal();
            }
        },
        ChangedTo: function (o) {
            this.IsOpen = false;
            this.IsOpen2 = false;
            var f = this.SelectedFrom;
            if (f.Value > o.Value && o.Value > 0) {
                this.SelectedFrom = o;
                this.SelectedTo = f;
            } else {
                this.SelectedTo = o;
            }
            this.UpdateDisplayName();
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
            var m = $scope.Screen.Max991();
            if (t.From == 0 && f.From == 0) {
                s = (m ? f.Name : this.Label);
            } else {
                if (f.From == 0) {
                    s = '< ' + t.Name;
                }
                else if (t.From == 0) {
                    s = '> ' + f.Name;
                } else {
                    s = f.Name + ' - ' + t.Name;
                }
                s = (m ? '' : this.Label + ': ') + s;
            }
            this.DisplayName = s;
        },
        Reset: function (p) {
            var res = $scope.TransType.GetPrices();
            this.Items = res.Range;
            this.Label = res.Title;
            this.DisplayName = res.Title;
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
                this.ChangedTo(obj);
            };
        }
    };

    // Filter
    $scope.FilterMore = {
        Label: '',
        HasFilter: false,
        IsCollapse: true,
        ShowTotal: function () {
            var t = 0;
            if ($scope.Area.HasFilter()) t += 1;
            if ($scope.Room.HasFilter()) t += 1;
            if ($scope.Direction.HasFilter()) t += 1;
            if ($scope.Legal.HasFilter()) t += 1;
            if ($scope.Time.HasFilter()) t += 1;
            if ($scope.Screen.Max991()) {
                if ($scope.Price.HasFilter()) t += 1;
            }
            if ($scope.Screen.Max768()) {
                if ($scope.PropType.HasFilter()) t += 1;
            }
            this.Label = (t == 0 ? '' : '(' + t + ')');
            this.HasFilter = (t > 0);
        },
        Collapse: function (event) {
            this.IsCollapse = !this.IsCollapse;
            $scope.Price.UpdateDisplayName();
        },
        OutsideClick: function (event) {
            event.stopPropagation();
        },
        Close: function () {
            this.IsCollapse = true;
        },
        Apply: function () {
            this.IsCollapse = true;
            this.ShowTotal();
            if ($scope.Mobile.IsOpen) {
                $scope.Mobile.Open();
            }
            if ($scope.Screen.Max640() == true) {
                $scope.CallSearch();
            }
        },
        Reset: function () {
            $scope.Area.Reset();
            $scope.Room.Reset();
            $scope.Direction.Reset();
            $scope.Legal.Reset();
            $scope.Time.Reset();
            if ($scope.Screen.Max991()) {
                $scope.Price.Reset();
            }
            if ($scope.Screen.Max768()) {
                $scope.PropType.Reset();
            }
            this.ShowTotal();
        }
    };
    // Mobile - Hiển thị lọc thêm
    $scope.Mobile = {
        IsOpen: false,
        Open: function (e) {
            this.IsOpen = !this.IsOpen;
            $('.command').toggleClass('open');
            $('body').toggleClass('overlay');
            $scope.PropType.UpdateDisplayName();
            $scope.Price.UpdateDisplayName();
        }
    };


    // Area
    var filterArea = mogiResUtils.filterUtils.getFilterArea();
    //angular.forEach(filterArea.Range, function (o) {
    //    if (o.From == 0 && o.To == 0) return;
    //    if (o.From == 0 && o.To > 0) {
    //        o.Name = "<= " + o.To + "m<sup>2</sup>";
    //        return;
    //    }
    //    o.Name = "" + o.From + " - " + o.To + "m<sup>2</sup>";
    //});
    $scope.Area = {
        Items: filterArea.Range,
        Label: filterArea.Title,
        Selected: filterArea.Range[0],
        Changed: function (o) {
            this.Selected = o;
            $scope.FilterMore.ShowTotal();
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
        }
    };

    // Room
    var filterRoom = mogiResUtils.filterUtils.getFilterRoom();
    $scope.Room = {
        Items: filterRoom.Range,
        Label: filterRoom.Title,
        Selected: filterRoom.Range[0],
        Changed: function (o) {
            this.Selected = o;
            $scope.FilterMore.ShowTotal();
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
        }
    };

    // Legal
    var filterLegal = mogiResUtils.filterUtils.getLegals();
    filterLegal.Range = mogiResUtils.getLegals();
    $scope.Legal = {
        Items: filterLegal.Range,
        Label: filterLegal.Title,
        Selected: filterLegal.Range[0],
        Changed: function (o) {
            this.Selected = o;
            $scope.FilterMore.ShowTotal();
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
        }
    };

    // Direction
    var filterDirection = mogiResUtils.filterUtils.getDirections();
    filterDirection.Range = mogiResUtils.getDirections();
    $scope.Direction = {
        Items: filterDirection.Range,
        Label: filterDirection.Title,
        Selected: filterDirection.Range[0],
        Changed: function (o) {
            this.Selected = o;
            $scope.FilterMore.ShowTotal();
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
        }
    };

    // Time
    var filterTime = mogiResUtils.filterUtils.getFilterTime();
    $scope.Time = {
        Items: filterTime.Range,
        Label: filterTime.Title,
        Selected: filterTime.Range[0],
        Changed: function (o) {
            this.Selected = o;
            $scope.FilterMore.ShowTotal();
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
        }
    };

    // Click Tìm kiếm 
    $scope.CallSearch = function () {
        $scope.FilterMore.IsCollapse = true;
        var p = {};
        p.Msg = mogiDatas.Msg;
        p.Rent = !$scope.TransType.GetValue();
        p.PropertyTypeId = $scope.PropType.GetValue();
        p.PropertyStyles = $scope.PropStyle.GetValue();
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
        p.Sort = $scope.SortView.GetSortValue(); // sort

        window.location.href = mogiUtils.friendlyUrl.getPropertySearchUrl(p);
    };

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
            if (o == null) o = this.Items[0];
            this.Selected = o;
        }
    };

    var owl = $('#owl-top-service');
    owl.pageIndex = 1;
    owl.pageSize = 3;
    owl.loading = false;
    owl.end = false;
    $scope.CarouselOptions = {
        margin: 10,
        loop: false,
        navText: ['&#x2039;', '&#x203a;'],
        items: 1,
        nav: true,
        dots: false,
        onChange: function (data) {
            var o = this;
            var m = o.maximum();
            var prev = (data.property.name == "position" && data.property.value < data.item.index);
            if (prev == true || owl.end == true || owl.loading == true || o._current < (m - 1)) {
                return;
            }
            owl.loading = true;
            if (m == 14) {
                owl.pageIndex = 2;
                owl.pageSize = 15;
            } else {
                owl.pageIndex += 1;
            }
            var f = filterData;
            var cityId = (f.did == 0 ? f.cid : f.did);
            var p = { id: cityId, rent: f.Rent, pageIndex: owl.pageIndex, pageSize: owl.pageSize };
			propertyServices.TopService(p).then(function (res) {
				res = res.data;
                if (res) {
                    var pos = o.maximum();
                    var items = $(res);
                    if (items.length > 0) {
                        for (var i = 0; i < items.length; i++) {
                            if (items[i].tagName == 'DIV') o.add(items[i]);
                        }
                        o.to(pos, 200, true);
                    }
                    owl.end = (items.length == 0);
                } else {
                    owl.end = true;
                }
                owl.loading = false;
            });
        }
    };

    $scope.IsSaveAlertSearch = function () {
        var o = pageData.AlertSearch;
        return (o != null && o.AlertSearchId > 0)
    }

    $scope.ShowAlertSearch = function () {
        var f = mogiUtils.friendlyUrl.mapSearchFilter(filterData);
        f = $.extend(f, filterData);
        f.json = JSON.stringify(filterData);
        var as = pageData.AlertSearch;
        if (as != null) {
            f.AlertSearchId = as.AlertSearchId;
            f.Title = as.Title;
            f.ReceiveEmailType = as.ReceiveEmailType;
            f.HashKey = as.HashKey;
        }
        commonServices.AlertSearch(f, function (resp) {
            location.reload();
        });
    }
    // Map
    $scope.Paging = {
        PageIndex: 1, PageSize: 10, Total: 0, Url: '',
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
            else if (p.Total == 0) {
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
        Init: function () {
            var f = mogiUtils.friendlyUrl.mapSearchFilter(filterData);
            f = $.extend(f, filterData);
            f["Msg"] = null; f["Map"] = null;
            bds.Search(f).success(function (resp) {
                var data = resp;
                var myTime = setInterval(function () {
                    if (bdsMap.inited == true) {
                        clearInterval(myTime);
                        bdsMap.UpdatePolygon(data["polyenc"]);
                        bdsMap.loadPins(data["response"]);
                        bds.SetData(data["response"]);
                        bds.LoadListView();
                    }
                }, 100);
                ignorePolygonChanges = false;
            });
        }
    };
    //phone number 
    $scope.Phone = (function () {
        var isClicks = {};
        var phone = '';
        return {
            GetPhoneNo: function (num) {
                if (!isClicks[num]) {
                    return num.substring(0, num.length - 2) + 'xx';
                }
                return num;
            },
            ShowPhone: function (num) { isClicks[num] = true; console.log(isClicks) }
        }
    }());
    // Active
    function Actived() {
        var fd = filterData;
        $scope.Favorite.Init(pageData.Favorites || []);
        $scope.TransType.SetValue(!fd.Rent);
        $scope.PropType.SetValue(fd.pid);
        $scope.PropStyle.SetValue(fd.psid);
        $scope.Price.SetValue(fd.fp, fd.tp);
        $scope.Area.SetValue(fd.fa, fd.ta);
        $scope.Room.SetValue(fd.tbr);
        $scope.Legal.SetValue(fd.lg);
        $scope.Direction.SetValue(fd.dt);
        $scope.Time.SetValue(fd.d);
        $scope.SortView.Init(fd);
        if ((fd.Map) != null) {
            var m = new suggest.MapField();
            var f = ["MapId", "ReferTypeId", "ReferId", "CityId", "DistrictId", "WardId", "StreetId", "ProjectId", "PlaceId", "CodeUrl", "Name"];
            for (var i = 0; i < f.length; i++) {
                m[f[i]] = fd.Map[f[i]];
            };
            $scope.Keyword = m;
        };
    };

    if (pageData.IsMapView == true) {
        bdsMap.notifySearchResult = function (t, i) {
            $scope.Paging.Set(i, t);
        };
        $scope.Paging.Init(pageData.Paging);
        $scope.Map.Init();
    }
    Actived();
};



ListViewController.$inject = ['$scope', '$http', 'blockUI', 'suggest', 'propertyServices', 'commonServices'];
mogiApp.controller('ListViewController', ListViewController);