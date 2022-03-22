bdsMap = {
    inited: false,
    highlightClass: 'maps-buttons-highlight',
    disabledClass: 'maps-buttons-inactive',
    msg: { system_error: 'Có lỗi xảy ra, hãy thử lại' },
    template: {
        "draw": '<div id="maps-status-message-draw" class="maps-status-message none"><div class="maps-status-message-bg"></div><span>Click và di chuyển chuột để vẽ khu vực tìm kiếm</span><a href="#" id="maps-buttons-draw-cancel" class="btn btn-default">Ngừng vẽ</a></div>',
        "palette": '<div id="maps-palette"><a href="#" id="maps-buttons-draw" class="btn-xs btn-default"><span></span><img src="/content/images/icons/loading-32x32.gif" width="26" height="26">Vẽ tìm kiếm</a><a href="#" id="maps-buttons-delete" class="btn-xs btn-default"><span></span></a><a class="btn-xs btn-default maps-buttons-inactive" href="#" id="maps-buttons-undo"><span></span></a><a href="#" id="maps-buttons-redo" class="btn-xs btn-default maps-buttons-inactive"><span></span></a></div>',
        "delete": '<div id="maps-status-message-delete" class="maps-status-message none"><div class="maps-status-message-bg"></div><span>Click vào khu vực hình vẽ muốn xóa.</span><a href="#" id="maps-buttons-delete-cancel" class="btn btn-default">Bỏ qua</a></div>',
        "loading": '<div id="maps-loading" class="maps-status-message none"><div class="maps-status-message-bg"></div><span><img src="/content/images/icons/loading-32x32.gif" width="24" height="24">Xin vui lòng chờ.</span></div>'
    }
};
bdsMap.user_alerts = function (a) {
    var b = {
        options: $.extend({
            action: "",
            href: "",
            isAlert: false,
            isAlertModified: false,
            pageSize: "",
            view: "list"
        }, a),
        init: function (d, c) {
            data = getSearchOptions();
            data.view_type = b.options.view;
            b._mapPolygon();
            if (d === "create") {
				b._create(c);
            } else {
				b._modify(c);
            }
        },
        _create: function (c) {
            delete data.section;
            data.page_size = b.options.pageSize;
            c = ZPG.util.appendParams(c, data);
            $.fancybox({
                href: c,
                titleShow: false,
                type: "ajax"
            })
        },
        _modify: function (c) {
            $.ajax({
                url: c,
                type: "POST",
                data: data,
                success: function (d) {
                    b.options.isAlertModified = false;
                    b._updateUI();
                    if (d.javascript) {
                        $.globalEval(d.javascript)
                    }
                }
            })
        },
        _mapPolygon: function () {
            if (b.options.view === "map" && isUserPolygon) {
                var c = map.getPolygons();
                if (!c.length) {
                    if (fancybox) {
						$.fancybox({
							content: "You need a defined search area to perform this action."
						});
                    }
					return false;
                }
				data.q = $('input[name="orig_q"]').attr("value");
            }
        },
        searchAlertUpdated: function () {
            if (b.options.isAlert) {
				b.options.isAlertModified = true;
            }
			b._updateUI();
        },
        _updateUI: function () {
            if (b.options.isAlert) {
                var e = $("#alerts-saved"),
                    d = $("#alerts-modified"),
                    f = $("#listings-tabs li:eq(2) a").attr("href"),
                    c = $("#alerts-map");
                if (b.options.isAlertModified) {
                    d.css("display", "block");
					e.css("display", "none");
                } else {
                    d.css("display", "none");
                    e.css("display", "block").delay(2000).fadeOut(200);
					c.attr("href", f);
                }
            }
        }
    };
	return {
		init: b.init,
		searchAlertUpdated: b.searchAlertUpdated
	};
};
bdsMap.getStaticMap = function (size, polys) {
	size = (size || '80x80');
	var u = '//maps.googleapis.com/maps/api/staticmap?size=' + size + '&path=color:0x5eab1f%7Cweight%3A3%7Cfillcolor:0xe16d07';
	for (i = 0; i < polys.length; i++) u += '|enc:' + polys[i];
	return u;
};

bdsMap.initialize = function (polyenc) {
    var self = this;
    self.inited = true;
    self.notifyChange = null;
    bdsMap.alerts = bdsMap.user_alerts({
        isAlert: isAlert,
        isAlertModified: false,
        pageSize: pageSize,
        view: view
    });

    self.map = map = new WebMap('map');

    map.setStrokeOption('strokeColor', '#5eab1f');
    map.setStrokeOption('fillColor', '#e16d07');
    map.canShowHeatmap(false);

    map.icon_favorite = map._getMarkerTypeImage(null, "favorite");
    map.icon_favorite_hover = map._getMarkerTypeImage(null, "favorite-hover");
    map.Markers = {
        contains: function (id) { return (this[id] != null); }
    };
    map.showMarkerGroupCallBack = function (el) {
        var childs = $(el).find("a[data-favourite-id]");
        var h = {};
        $(childs).each(function () {
            var id = $(this).attr("data-favourite-id");
            if (h[id] == null) {
                h[id] = true;
                if (bdsFavouritePartial.Favorites.contains(id) == true) {
                    bdsMap.user_favourites.add(id, el);
                }
            }
        });
    };
    
    var polys = [];
    if (polyenc) {
        for (var i = 0; i < polyenc.length; i++) {
            var g = google.maps.geometry.encoding.decodePath(polyenc[i]);
            var a = $.map(g, function (k, j) {
                return [[k.lat(), k.lng()]]
            });
            polys.push(a);
        }
    }
    map.initMapWithPolygons(polys);
    
    self.gmap = gmap = map.getMap();
    self.gmap.setCenter(new google.maps.LatLng(10.731287, 106.718871));
    var map_width = $('#map').width();
    var control_status_draw = document.createElement('DIV');
    var control_status_delete = document.createElement('DIV');
    //var control_status_loading = document.createElement('DIV');
    var controls = document.createElement('DIV');

    self.gmap.controls[google.maps.ControlPosition.TOP_RIGHT].push(control_status_draw);
    self.gmap.controls[google.maps.ControlPosition.TOP_RIGHT].push(control_status_delete);
    //self.gmap.controls[google.maps.ControlPosition.TOP_RIGHT].push(control_status_loading);
    self.gmap.controls[google.maps.ControlPosition.LEFT_TOP].push(controls);

    var undoRedoFunc = function (m, u, r) {
        if (!ignorePolygonChanges) {
            bdsMap.updateMapPolygon();
        }
    };
    map.addUndoListener(undoRedoFunc);
    map.addRedoListener(undoRedoFunc);

    map.addPolygonRemovedListener(function () { bdsMap.enableControls(); });
    map.addPolygonChangedListener(function () { bdsMap.enableControls(); });

    var callbackUpdateLocation = function () {
        if (isUserPolygon || ignorePolygonChanges) {
            return;
        }
        isUserPolygon = true;
        bdsMap.updateLocation();
    };
    map.addPolygonChangedListener(callbackUpdateLocation);
    map.addPolygonRemovedListener(callbackUpdateLocation);


    var callback = function () {
        if (!ignorePolygonChanges) {
            bdsMap.updateMapPolygon();
        }
    };

    map.addPolygonChangedListener(callback);
    map.addPolygonRemovedListener(callback);

    //map.addPolygonChangedListener(bdsMap.alerts.searchAlertUpdated);
    //map.addPolygonRemovedListener(bdsMap.alerts.searchAlertUpdated);

    $(control_status_draw).html(self.template.draw);
    $(control_status_delete).html(self.template.delete);
    //$(control_status_loading).html(self.template.loading);
    $(controls).html(self.template.palette);


    bdsMap.user_favourites.init();
}

bdsMap.UpdatePolygon = function (polyenc) {
	var polys = [];
	if (polyenc) {
		for (var i = 0; i < polyenc.length; i++) {
			var g = google.maps.geometry.encoding.decodePath(polyenc[i]);
			var a = $.map(g, function (k, j) {
				return [[k.lat(), k.lng()]]
			});
			polys.push(a);
		}
	}
	map.initMapWithPolygons(polys);
};

bdsMap.completeFreehand = function (changed) {
    if (changed) {
        isUserPolygon = true;
        bdsMap.updateLocation();
    }

    this.unhighlightControls();
    
    this.showMessages();
    this.updateMapPolygon();
};

bdsMap.completeDelete = function () {
    
    bdsMap.map.endDeleteSearchArea();
    bdsMap.unhighlightControls();
    bdsMap.showMessages();
};

bdsMap.cancelDelete = function () {
    if (this.map.isDeletingSearchArea()) {
        this.completeDelete();
        this.updateMapPolygon();
        this.enableControls();
    }
    return false;
};

bdsMap.cancelFreehand = function () {
    if (this.map.isDrawingFreehand()) {
        this.map.cancelFreehand();
        this.completeFreehand();
        this.enableControls();
    }
};

// remove all "active" control classes
bdsMap.unhighlightControls = function () {
	$('.' + this.highlightClass).removeClass(this.highlightClass);
};

// activate a single control
bdsMap.highlightControl = function (e) {
	this.unhighlightControls();
	$(e).addClass(this.highlightClass);
};

// disable known controls
bdsMap.disableControls = function (all) {
    var selector = '#maps-buttons-undo, #maps-buttons-redo';
    if (all) {
        selector = selector + ', #maps-buttons-draw, #maps-buttons-delete';
    }
    $(selector).addClass(this.disabledClass);
    $('#search-listings-button button').addClass('btn-disabled');
};

// enable all controls
bdsMap.enableControls = function () {
    $('.' + this.disabledClass).removeClass(this.disabledClass);

    // check undo
    if (!map.canUndo()) {
        $('#maps-buttons-undo').addClass(this.disabledClass);
    }

    // check redo
    if (!map.canRedo()) {
        $('#maps-buttons-redo').addClass(this.disabledClass);
    }

    // check delete
    if (!map.canDeleteSearchArea()) {
        $('#maps-buttons-delete').addClass(this.disabledClass);
    }

    if (!mostRecentPinCount) {
        $('#maps-buttons-email').addClass(this.disabledClass);
    }

    $('#search-listings-button button').removeClass('btn-disabled');

};

bdsMap.clearMap = function () {
    map.clearSearchArea();
    mostRecentPinCount = 0;
    bdsMap.enableControls();
    //_gaq.push(['_trackEvent', 'SmartMaps', 'Clear map', '/tracking/for-sale/results/map/']);
    return false;
};

// hide controls when status message shows
bdsMap.hideMessages = function () {
	$('#mapsMessage, .map-button').hide();
	$('.maps-status-message').addClass('none');
};

// show controls when status message removed
bdsMap.showMessages = function () {
	$('#mapsMessage, .map-button').show();
	$('.maps-status-message').addClass('none');
};

bdsMap.isDisabled = function (e) {
	return $(e).hasClass(this.disabledClass);
};

bdsMap.showLoading = function (msg) {
	// just in case
	this.hideLoading();
	$('#maps-buttons-draw').addClass('loading');
};

bdsMap.hideLoading = function () {
	$('#maps-buttons-draw').removeClass('loading');
};

bdsMap.getSearchPolygons = function () {
	if (isUserPolygon) {
		var polys = this.map.getPolygons();
		return this.map.encodePolygons(polys);
	}
	return '';
};

bdsMap.loadMarker = function (data, t, f) {
    for (var i in data) {
        var item = data[i];
        if (item && item.latlon) {
            var latlon = item.latlon.split(',');
            var forSale = true;//(bds.UrlParam.ch == 2);
            if (forSale) {
                map.addForSalePin(item.id, latlon[0], latlon[1], null, (f && item.IsFavorited ? "favorite" : item.ptid));
            } else {
                map.addToRentPin(item.id, latlon[0], latlon[1], null, (f && item.IsFavorited ? "favorite" : item.ptid));
            }
            data[item.id] = item;
        }
    }
    bdsMap.Data = data;
};
bdsMap.loadPins = function (resp) {
	var data = resp["docs"];
	if (!data) {
		mostRecentPinCount = 0;
		return;
	}
	var total = resp["numFound"];

	mostRecentPinCount = data.length;

	//bdsFavouritePartial.Init(bdsConf.Const.ReferType.PROPERTY).done(function (favorites) {
	//    $.each(data, function (i, o) {
	//        o.IsFavorited = favorites.contains(o.id);
	//    });
	//    bdsMap.loadMarker(data, total, true);
	//}).fail(function () {
	//    bdsMap.loadMarker(data, total);
	//});
	bdsMap.loadMarker(data, total);
	bdsMap.updateSearchResult(total);
};
bdsMap.updateSearchResult = function (t, i) {
	var c = bdsMap["notifySearchResult"];
	if (typeof (c) == "function") {
		c(t, i);
	}
};
bdsMap.updateLocation = function () {
	var c = bdsMap["notifyChange"];
	if (typeof (c) == "function") {
		var p = bdsMap.getSearchPolygons();
		c(p, isUserPolygon);
	}
};

bdsMap.updateMapPolygon = function () {
	var polys = this.map.getPolygons();
	// no polygons? clear results instead
	if (isUserPolygon && polys.length == 0) {
		this.map.clearMarkers(true);
		this.map.showMessage('');
	} else {
		this.disableControls(true);
		this.alerts.searchAlertUpdated();

		this.showLoading();
		var polyenc = this.getSearchPolygons();

		bds.SearchMap(polyenc).done(function (data) {
			if (data['redirect_url']) {
				top.location.href = data['redirect_url'];
			}
			bdsMap.map.clearMarkers(true);
			bdsMap.loadPins(data["response"]);
			bds.SetData(data["response"]);
			bds.LoadListView();

			var polys = null;
			if (data['draw_polyenc'] && typeof (data['draw_polyenc']) == 'object' && data['draw_polyenc'].length > 0) {
				polys = bdsMap.map.decodePolygons(data['draw_polyenc']);
			}

			if (isUserPolygon) {
				if (polys != null) {
					ignorePolygonChanges = true;
					bdsMap.map.replaceSearchArea(polys, false);
					ignorePolygonChanges = false;
				}
			} else {
				ignorePolygonChanges = true;

				if (polys != null) {
					bdsMap.map.replaceSearchArea(polys);
				}
				else if (data['bounding_box']) {
					bdsMap.map.clearSearchArea();
					bdsMap.map.focusMap(data['bounding_box']['lat_min'], data['bounding_box']['lon_min'], data['bounding_box']['lat_max'], data['bounding_box']['lon_max']);
				}
				bdsMap.updateLocation();

				ignorePolygonChanges = false;
			}

			bdsMap.enableControls();
			bdsMap.hideLoading();
		}).fail(function () {
			bdsMap.hideLoading();
			bdsMap.enableControls();
			bdsMap.showMessages(bdsMap.msg.system_error);
		});
	}
};

bdsMap.clearMap = function () {
    map.clearSearchArea();
    mostRecentPinCount = 0;
    enableControls();
    //_gaq.push(['_trackEvent', 'SmartMaps', 'Clear map', '/tracking/for-sale/results/map/']);
    return false;
};
// *********************************************************
// Map button controls
$('#maps').on('click', '#maps-buttons-draw-cancel', function () { bdsMap.cancelFreehand(); });

$('#maps').on('click', '#maps-buttons-draw', function () {

    if (bdsMap.isDisabled(this)) return;

    if (bdsMap.map.isDrawingFreehand()) {

        bdsMap.cancelFreehand();
        return;

    } else if (bdsMap.map.isDeletingSearchArea()) {
        bdsMap.completeDelete();
    }

    bdsMap.disableControls();
    bdsMap.highlightControl(this);
    bdsMap.hideMessages();

    $('#maps-status-message-draw').removeClass('none');

    bdsMap.map.clearMarkers();
    bdsMap.map.drawFreehand(function (changed) { bdsMap.completeFreehand(changed); });

    return false;
});

$('#maps').on('click', '#maps-buttons-delete', function () {

    if (bdsMap.isDisabled(this)) return;

    if (bdsMap.map.isDeletingSearchArea()) {
        bdsMap.cancelDelete();
        return false;
    } else if (bdsMap.map.isDrawingFreehand()) {
        bdsMap.map.cancelFreehand();
    }

    bdsMap.disableControls();
    bdsMap.highlightControl(this);
    bdsMap.hideMessages();

    $('#maps-status-message-delete').removeClass('none');

    bdsMap.map.clearMarkers();
    bdsMap.map.deleteSearchArea(function (a, b) {
        bdsMap.completeDelete(a,b);
    });

    return false;
});

$('#maps').on('click', '#maps-buttons-delete-cancel', function () { bdsMap.cancelDelete(); });

$('#maps').on('click', '#maps-buttons-undo', function () {

    if (bdsMap.isDisabled(this)) return;

    bdsMap.disableControls();
    bdsMap.map.undo();
    bdsMap.enableControls();

    return false;
});

$('#maps').on('click', '#maps-buttons-redo', function () {

    if (bdsMap.isDisabled(this)) return;

    bdsMap.disableControls();
    bdsMap.map.redo();
    bdsMap.enableControls();

    return false;
});

// End: Map button contols 

bdsMap.user_favourites = {
    init: function () {
        BDSUF = bdsMap.user_favourites;
		$(document).on("click", "a[data-favourite-action]", function () {
			BDSUF.$this = $(this);
			BDSUF.user = BDSUF.$this.attr("data-favourite-user");
			BDSUF.data = BDSUF.$this.attr("data-favourite-id");
			BDSUF.type = BDSUF.$this.attr("data-favourite-type");
			BDSUF.action = BDSUF.$this.attr("data-favourite-action");

			var c = bds["mapFavorite"];
			if (typeof (c) == "function") {
				c(BDSUF.action, BDSUF.data, BDSUF.type).done(function (resp) {
					if (BDSUF.action == 'add') BDSUF.add(); else BDSUF.remove();
				});
			}
			return false;
		});
    }, add: function (data, sender) {
        data = (data || BDSUF.data);
        var el = "#map-popup a[data-favourite-id=" + data + "]";
        if (sender) {
            el = $(sender).find("a[data-favourite-id=" + data + "]");
        }
        $(el).each(function () {
            var a = $(this).find("span").html(), b = $(this).attr("data-fav-copy");
            $(this).attr("data-favourite-action", "remove").attr("data-fav-copy", a).addClass("favourite-saved").find("span").html(b)

        });
        var m = map.Markers[data];
        if (m) {
            m.marker.icon1 = map.icon_favorite;
            m.marker.icon2 = map.icon_favorite_hover;
            m.marker.setIcon(map.icon_favorite);
        }
    }, remove: function (data, sender) {
        data = (data || BDSUF.data);
        var el = "#map-popup a[data-favourite-id=" + data + "]";
        $(el).each(function () {
            var a = $(this).find("span").html(), b = $(this).attr("data-fav-copy");
            $(this).attr("data-favourite-action", "add").attr("data-fav-copy", a).removeClass("favourite-saved").find("span").html(b)
        });

        el = $("#map-popup a[data-favourite-action=remove]");
        if (el.length == 0) {
            var m = map.Markers[data];
            if (m) {
                var item = bdsMap.Data[data];
                if (item) {
                    m.marker.icon1 = map._getMarkerTypeImage(null, item.pid);
                    m.marker.icon2 = map._getMarkerTypeImage(null, item.pid + "-hover");
                    m.marker.setIcon(m.marker.icon1);
                }
            }
        }
    }
};
