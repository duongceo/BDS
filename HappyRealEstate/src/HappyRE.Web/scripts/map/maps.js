var iconsBase = "/content/maps/images/markers/";
var defaultStrokeColor = "#5eab1f";
var defaultStrokeOpacity = 1;
var defaultStrokeWeight = 3;
var defaultFillColor = "#5c1862";
var defaultFillOpacity = 0.15;
var defaultRolloverStrokeColor = "#f05a28";
var defaultRolloverStrokeOpacity = 1;
var defaultRolloverStrokeWeight = 6;
var defaultRolloverFillColor = "#f05a28";
var defaultRolloverFillOpacity = 0.15;
var defaultPinHeight = 25;
var defaultPinWidth = 26;
var defaultPinShadow = "spacer.gif";
var defaultPinPointX = 9;
var defaultPinPointY = 34;
var defaultPinShadowX;
var defaultPinShadowY;

function WebMapCircle(b, a) {
    this.map = b;
    this.radiusMiles = undefined;
    this.changeListeners = [];
    this.circle = undefined;
    this.strokeOptions = $.extend({
        rolloverStrokeColor: defaultRolloverStrokeColor,
        rolloverStrokeOpacity: defaultRolloverStrokeOpacity,
        rolloverStrokeWeight: defaultRolloverStrokeWeight,
        rolloverFillColor: defaultRolloverFillColor,
        rolloverFillOpacity: defaultRolloverFillOpacity,
        strokeColor: defaultStrokeColor,
        strokeOpacity: defaultStrokeOpacity,
        strokeWeight: defaultStrokeWeight,
        fillColor: defaultFillColor,
        fillOpacity: defaultFillOpacity
    }, a);
    this._updateColours = function () {
        var c = this.getCircle();
        if (c) {
            c.setOptions(this.strokeOptions)
        }
    };
    this.setStrokeOption = function (c, d) {
        if (this.strokeOptions[c] != d) {
            this.strokeOptions[c] = d;
            this._updateColours()
        }
    };
    this.setStrokeOptions = function (c) {
        var d = 0;
        var e = this;
        $.each(c, function (f, g) {
            if (e.strokeOptions[f] != g) {
                e.strokeOptions[f] = g;
                d++
            }
        });
        if (d > 0) {
            this._updateColours()
        }
    };
    this.getCircle = function () {
        return this.circle
    };
    this.getRadiusMiles = function () {
        return this.radiusMiles
    };
    this.addCircleChangeListener = function (c) {
        this.changeListeners.push(c)
    };
    this._notifyCircleChangeListeners = function () {
        for (var c in this.changeListeners) {
            this.changeListeners[c](this)
        }
    };
    this.setCircle = function (e, f, c) {
        this.clear();
        this.radiusMiles = c;
        var d = 1609.344 * c;
        this.circle = new google.maps.Circle({
            center: new google.maps.LatLng(e, f),
            radius: d,
            map: this.map,
            strokeColor: this.strokeOptions.strokeColor,
            strokeOpacity: this.strokeOptions.strokeOpacity,
            strokeWeight: this.strokeOptions.strokeWeight,
            fillColor: this.strokeOptions.fillColor,
            fillOpacity: this.strokeOptions.fillOpacity,
            editable: false,
            clickable: false
        });
        var g = this;
        google.maps.event.addListener(this.circle, "center_changed", function () {
            g._notifyCircleChangeListeners()
        });
        google.maps.event.addListener(this.circle, "radius_changed", function () {
            g.radiusMiles = g.circle.getRadius() / 1609.344;
            g._notifyCircleChangeListeners()
        })
    };
    this.init = function (d, e, c) {
        this.setCircle(d, e, c)
    };
    this.clear = function () {
        if (this.circle) {
            this.circle.setMap(null)
        }
    }
}

function WebMapPolygon(b, a) {
    this.map = b;
    this.polygon = undefined;
    this.changeListeners = [];
    this.draggingPointIndex = -1;
    this.draggingPointIsNew = false;
    this.changesDisabled = false;
    //this.polygonMarker = new google.maps.MarkerImage(iconsBase + "polygon-marker.png", new google.maps.Size(14, 14), new google.maps.Point(0, 0), new google.maps.Point(7, 7));
    this.polygonMarker = { path: google.maps.SymbolPath.CIRCLE, strokeColor: defaultStrokeColor, fillColor: 'white', fillOpacity: 0.9, scale: 5, strokeWeight: 2 };
    this.polygonMarkerOver = { path: google.maps.SymbolPath.CIRCLE, strokeColor: defaultStrokeColor, fillColor: '#ef9240', fillOpacity: 0.9, scale: 5, strokeWeight: 2 };
    this.polygonMarkers = [];
    this.rolloverListeners = [];
    this.mouseListeners = [];
    this.dragStartListeners = [];
    this.dragEndListeners = [];
    this.strokeOptions = $.extend({
        rolloverStrokeColor: defaultRolloverStrokeColor,
        rolloverStrokeOpacity: defaultRolloverStrokeOpacity,
        rolloverStrokeWeight: defaultRolloverStrokeWeight,
        rolloverFillColor: defaultRolloverFillColor,
        rolloverFillOpacity: defaultRolloverFillOpacity,
        strokeColor: defaultStrokeColor,
        strokeOpacity: defaultStrokeOpacity,
        strokeWeight: defaultStrokeWeight,
        fillColor: defaultFillColor,
        fillOpacity: defaultFillOpacity
    }, a);
    this.addDragStartListener = function (c) {
        this.dragStartListeners.push(c)
    };
    this._notifyDragStartListeners = function () {
        for (var c in this.dragStartListeners) {
            this.dragStartListeners[c](this)
        }
    };
    this.addDragEndListener = function (c) {
        this.dragEndListeners.push(c)
    };
    this._notifyDragEndListeners = function () {
        for (var c in this.dragEndListeners) {
            this.dragEndListeners[c](this)
        }
    };
    this.enableRollOver = function () {
        var c = this.getPolygon();
        var d = this;
        if (!this.rolloverListeners.length) {
            this.rolloverListeners.push(google.maps.event.addListener(c, "mouseover", function () {
                d._rollOver()
            }), google.maps.event.addListener(c, "mouseout", function () {
                d._rollOut()
            }))
        }
    };
    this.disableRollOver = function () {
        this._rollOut();
        while (this.rolloverListeners.length) {
            google.maps.event.removeListener(this.rolloverListeners.pop())
        }
    };
    this.containsPoint = function (c, d) {
        return containsPoint(this.polygon.getPath().getArray(), c, d)
    };
    this._updateColours = function () {
        this._rollOut()
    };
    this.setStrokeOption = function (c, d) {
        if (this.strokeOptions[c] != d) {
            this.strokeOptions[c] = d;
            this._updateColours()
        }
    };
    this.setStrokeOptions = function (c) {
        var d = 0;
        var e = this;
        $.each(c, function (f, g) {
            if (e.strokeOptions[f] != g) {
                e.strokeOptions[f] = g;
                d++
            }
        });
        if (d > 0) {
            this._updateColours()
        }
    };
    this._rollOver = function () {
        this.setOptions({
            strokeColor: this.strokeOptions.rolloverStrokeColor,
            strokeOpacity: this.strokeOptions.rolloverStrokeOpacity,
            strokeWeight: this.strokeOptions.rolloverStrokeWeight,
            fillColor: this.strokeOptions.rolloverFillColor,
            fillOpacity: this.strokeOptions.rolloverFillOpacity
        })
    };
    this._rollOut = function () {
        this.setOptions({
            strokeColor: this.strokeOptions.strokeColor,
            strokeOpacity: this.strokeOptions.strokeOpacity,
            strokeWeight: this.strokeOptions.strokeWeight,
            fillColor: this.strokeOptions.fillColor,
            fillOpacity: this.strokeOptions.fillOpacity
        })
    };
    this.clear = function () {
        if (this.polygon) {
            this.polygon.setMap(null)
        }
        while (this.polygonMarkers.length > 0) {
            this.polygonMarkers.pop().setMap(null)
        }
        this._clearListeners()
    };
    this._initialiseListeners = function () {
        this._clearListeners();
        var f = this;
        var d = function (g) {
            if (f.draggingPointIndex != -1) {
                f.polygon.getPath().setAt(f.draggingPointIndex, g.latLng);
                f.polygonMarkers[f.draggingPointIndex].setPosition(g.latLng);
                f._notifyDragEndListeners();
                f.map.setOptions({
                    draggable: true
                });
                f.draggingPointIndex = -1;
                if (f.draggingPointIsNew) {
                    f.draggingPointIsNew = false;
                    f._notifyChangeListeners()
                }
            }
        };
        var e = function (g) {
            if (f.draggingPointIndex != -1) {
                f.polygon.getPath().setAt(f.draggingPointIndex, g.latLng);
                f.polygonMarkers[f.draggingPointIndex].setPosition(g.latLng)
            }
        };
        var c = function (x) {
            if (f.draggingPointIndex != -1 || f.changesDisabled) {
                return
            }
            var t = x.latLng;
            var o = f.polygon.getPath();
            var r;
            var p = o.getAt(0);
            var h = -1;
            var y = -1;
            var u = o.getLength() + 1;
            for (var v = 1; v < u; ++v) {
                r = p;
                p = o.getAt(v % o.getLength());
                var j = Math.min(r.lat(), p.lat());
                var s = Math.max(r.lat(), p.lat());
                if (j > t.lat() || s < t.lat()) {
                    continue
                }
                var w = Math.min(r.lng(), p.lng());
                var m = Math.max(r.lng(), p.lng());
                if (w > t.lng() || m < t.lng()) {
                    continue
                }
                var q = distanceFromPointToLine(t, r, p);
                if (h == -1 || q < h) {
                    h = q;
                    y = v
                }
            }
            var g = f.map.getZoom();
            var k = 5;
            var n = k / Math.pow(2, g);
            if (y != -1 && h <= n) {
                var l = f._addPolygonPointMarker(x.latLng);
                if (y >= o.length) {
                    o.push(x.latLng);
                    f.polygonMarkers.push(l)
                } else {
                    o.insertAt(y, x.latLng);
                    f.polygonMarkers.splice(y, 0, l)
                }
                f.draggingPointIndex = y;
                f.draggingPointIsNew = true;
                f.map.setOptions({
                    draggable: false
                });
                f._notifyDragStartListeners()
            }
        };
        if (!this.mouseListeners.length) {
            this.mouseListeners.push(google.maps.event.addListener(this.map, "mousedown", c), google.maps.event.addListener(this.map, "mousemove", e), google.maps.event.addListener(this.map, "mouseup", d), google.maps.event.addListener(this.polygon, "mousedown", c), google.maps.event.addListener(this.polygon, "mousemove", e), google.maps.event.addListener(this.polygon, "mouseup", d))
        }
    };
    this._clearListeners = function () {
        while (this.mouseListeners.length > 0) {
            google.maps.event.removeListener(this.mouseListeners.pop())
        }
    };
    this.init = function () {
        this.clear();
        this.polygon = new google.maps.Polyline({
            strokeColor: this.strokeOptions.strokeColor,
            strokeOpacity: this.strokeOptions.strokeOpacity,
            strokeWeight: this.strokeOptions.strokeWeight,
            map: this.map
        });
        this._initialiseListeners()
    };
    this.initWithPolygon = function (c) {
        this.init();
        this.setPolygon(c)
    };
    this.getPolygon = function () {
        return this.polygon
    };
    this.setOptions = function (c) {
        var d = this.getPolygon();
        if (d) {
            d.setOptions(c)
        }
    };
    this.disableChanges = function (c) {
        this.changesDisabled = true;
        this.disableMarkers();
        if (c) {
            this.setOptions({
                clickable: false
            })
        }
    };
    this.enableChanges = function () {
        this.changesDisabled = false;
        this.enableMarkers();
        this.setOptions({
            clickable: true
        })
    };
    this.disableMarkers = function () {
        $.each(this.polygonMarkers, function (c, d) {
            d.setVisible(false)
        })
    };
    this.enableMarkers = function () {
        $.each(this.polygonMarkers, function (c, d) {
            d.setVisible(true)
        })
    };
    this.setPolygon = function (e) {
        this.clear();
        this.polygon = new google.maps.Polygon({
            strokeColor: this.strokeOptions.strokeColor,
            strokeOpacity: this.strokeOptions.strokeOpacity,
            strokeWeight: this.strokeOptions.strokeWeight,
            fillColor: this.strokeOptions.fillColor,
            fillOpacity: this.strokeOptions.fillOpacity,
            clickable: true,
            map: b
        });
        this._initialiseListeners();
        e = forceClockwiseWinding(e);
        for (var d = 0; d < e.length; ++d) {
            var c = new google.maps.LatLng(e[d][0], e[d][1]);
            this._addPolygonPoint(c)
        }
    };
    this.getPath = function () {
        return this.polygon.getPath()
    };
    this.addChangeListener = function (c) {
        this.changeListeners.push(c)
    };
    this._addPolygonPointMarker = function (c) {
        var d = new google.maps.Marker({
            clickable: true,
            draggable: true,
            flat: true,
            icon: this.polygonMarker,
            position: c,
            title: "Di chuyển để thay đổi hình hoặc double-click để xóa điểm này.",
            raiseOnDrag: false,
            map: this.map
        });
        var i = this;
        var e = google.maps.event.addListener(d, "dragstart", function (k) {
            i.draggingPointIsNew = false;
            for (var j = 0; j < i.polygonMarkers.length; ++j) {
                if (i.polygonMarkers[j].getPosition().equals(d.getPosition())) {
                    i.draggingPointIndex = j;
                    i._notifyDragStartListeners();
                    break
                }
            }
        });
        var f = google.maps.event.addListener(d, "dragend", function (j) {
            i._notifyDragEndListeners();
            i.draggingPointIndex = -1;
            i._notifyChangeListeners()
        });
        var h = google.maps.event.addListener(d, "drag", function (j) {
            if (i.draggingPointIndex != -1) {
                i.polygon.getPath().setAt(i.draggingPointIndex, j.latLng)
            }
        });
        var g = google.maps.event.addListener(d, "dblclick", function (m) {
            var l = i.polygon.getPath();
            if (l.getLength() <= 3) {
                return
            }
            var j = -1;
            for (var k = 0; k < i.polygonMarkers.length; ++k) {
                if (i.polygonMarkers[k].getPosition().equals(d.getPosition())) {
                    j = k;
                    break
                }
            }
            if (j != -1) {
                l.removeAt(j);
                d.setMap(null);
                i.polygonMarkers.splice(j, 1);
                i._notifyChangeListeners()
            }
        });
        google.maps.event.addListener(d, 'mouseover', function () {
            this.setIcon(i.polygonMarkerOver);
        });
        google.maps.event.addListener(d, 'mouseout', function () {
            this.setIcon(i.polygonMarker);
        });
        return d
    };
    this._notifyChangeListeners = function () {
        for (var c in this.changeListeners) {
            this.changeListeners[c](this)
        }
    };
    this._addPolygonPoint = function (c) {
        this.polygon.getPath().push(c);
        var d = this._addPolygonPointMarker(c);
        this.polygonMarkers.push(d)
    }
}

function WebMap(a, b) {
    this.undoBufferSize = 10;
    this.undoBuffer = undefined;
    this.redoBuffer = undefined;
    this.currentSnapshot = undefined;
    this.snapshotFlux = false;
    this.strokeOptions = $.extend({
        rolloverStrokeColor: defaultRolloverStrokeColor,
        rolloverStrokeOpacity: defaultRolloverStrokeOpacity,
        rolloverStrokeWeight: defaultRolloverStrokeWeight,
        rolloverFillColor: defaultRolloverFillColor,
        rolloverFillOpacity: defaultRolloverFillOpacity,
        strokeColor: defaultStrokeColor,
        strokeOpacity: defaultStrokeOpacity,
        strokeWeight: defaultStrokeWeight,
        fillColor: defaultFillColor,
        fillOpacity: defaultFillOpacity,
        pinHeight: defaultPinHeight,
        pinWidth: defaultPinWidth,
        pinShadow: defaultPinShadow,
        pinPointX: defaultPinPointX,
        pinPointY: defaultPinPointY,
        pinShadowX: defaultPinShadowX,
        pinShadowY: defaultPinShadowY
    }, b);
    this.spacerMarker = new google.maps.MarkerImage("/content/images/maps/spacer.gif", new google.maps.Size(50, 25), new google.maps.Point(0, 0), new google.maps.Point(25, 13), new google.maps.Size(50, 25));
    this.shadowMarker = new google.maps.MarkerImage(iconsBase + this.strokeOptions.pinShadow, new google.maps.Size(31, 37), new google.maps.Point(0, 0), new google.maps.Point(this.strokeOptions.pinShadowX, this.strokeOptions.pinShadowY));
    this.pinPoint = new google.maps.Point(this.strokeOptions.pinPointX, this.strokeOptions.pinPointY);
    this.imageMarkerSize = new google.maps.Size(this.strokeOptions.pinWidth, this.strokeOptions.pinHeight);
    this.mapTypeControlClass = "map-button-type";
    this.heatMapOverlayIndex = 0;
    this.zedIndexOverlayIndex = 1;
    this.imageMarkers = {};
    this.mapHasMoved = false;
    this.drawingFreehand = false;
    this.activeMapControlId = undefined;
    this.buttonClass = "map-button";
    this.buttonActiveClass = "bg-primary text-white";
    this.buttonFirstClass = "map-button-first";
    this.buttonLastClass = "map-button-last";
    this.infoWindow = undefined;
    this.preInfoWindowPos = undefined;
    this.message = "";
    this.userMessage = "";
    this.showingUpdate = false;
    this.heatMapOverlay = undefined;
    this.pinShape = undefined;
    this.currentAjaxRequest = undefined;
    this.mapDomElement = document.getElementById(a);
    this.map = undefined;
    this.valueLayersEnabled = false;
    this.heatMapEnabled = false;
    this.zedIndexEnabled = false;
    this.nonValueLayerMessage = "";
    this.zedIndexSticky = false;
    this.tileSize = new google.maps.Size(256, 256);
    this.markers = [];
    this.zedIndexMarkers = {};
    this.markerGroups = undefined;
    this.markerTypes = {
        sale: {
            id: "listing_id",
            marker: "default",
            url: "/widgets/maps/listing-popup/?section=for-sale"
        },
        rent: {
            id: "listing_id",
            marker: "default",
            url: "/widgets/maps/listing-popup/?section=to-rent"
        },
        estimate: {
            id: "property_id",
            marker: "default",
            url: "/widgets/maps/property-popup/?section=home-values"
        },
        price: {
            id: "property_id",
            marker: "default",
            url: "/widgets/maps/property-popup/?section=house-prices"
        },
        temptme: {
            id: "property_id",
            marker: "default",
            url: "/widgets/maps/property-popup/?section=home-values&temptme=1"
        },
        agent: {
            id: "branch_id",
            marker: "default",
            url: "/widgets/maps/branch-popup"
        },
        developer: {
            id: "branch_id",
            marker: "default",
            url: "/widgets/maps/branch-popup"
        },
        "special-offers": {
            id: "branch_id",
            marker: "default",
            url: "/widgets/maps/plot-development-popup"
        },
        "government-incentives": {
            id: "branch_id",
            marker: "default",
            url: "/widgets/maps/plot-development-popup"
        },
        "favourite-sale": {
            id: "listing_id",
            marker: "favourite",
            url: "/widgets/maps/listing-popup/?section=for-sale&favourite=1"
        },
        "favourite-rent": {
            id: "listing_id",
            marker: "favourite",
            url: "/widgets/maps/listing-popup/?section=to-rent&favourite=1"
        },
        "favourite-estimate": {
            id: "property_id",
            marker: "favourite",
            url: "/widgets/maps/property-popup/?section=home-values&favourite=1"
        },
        "favourite-price": {
            id: "property_id",
            marker: "favourite",
            url: "/widgets/maps/property-popup/?section=house-prices&favourite=1"
        },
        "favourite-results-sale": {
            id: "listing_id",
            marker: "favourite",
            url: "/widgets/maps/listing-popup/?section=for-sale&favourite=1"
        },
        "favourite-results-rent": {
            id: "listing_id",
            marker: "favourite",
            url: "/widgets/maps/listing-popup/?section=to-rent&favourite=1"
        },
        "favourite-results-estimate": {
            id: "property_id",
            marker: "favourite",
            url: "/widgets/maps/property-popup/?section=home-values&favourite=1"
        },
        "favourite-results-price": {
            id: "property_id",
            marker: "favourite",
            url: "/widgets/maps/property-popup/?section=house-prices&favourite=1"
        }
    };
    this.polygons = [];
    this.circles = [];
    this.dragEndListeners = [];
    this.dragStartListeners = [];
    this.mapMovedListeners = [];
    this.polygonChangedListeners = [];
    this.polygonAddedListeners = [];
    this.polygonRemovedListeners = [];
    this.undoListeners = [];
    this.redoListeners = [];
    this.useMapControls = false;
    this.useHeatmap = true;
    this.initMapWithPolygons = function (c, d) {
        var e = this.getMap(d);
        this.snapshotFlux = true;
        if (c.length > 0) {
            this.deserializePolygons(c)
        }
        this.snapshotFlux = false;
        this._initUndoBuffer()
    };
    this.initMapWithPoint = function (d, e, c) {
        return this.initMapWithBounds(d, e, d, e)
    };
    this.initMapWithBounds = function (f, h, c, e, d) {
        var g = this.getMap(d);
        if (f != c && h != e) {
            this.focusMap(f, h, c, e)
        } else {
            g.setZoom(5);
            g.setCenter(new google.maps.LatLng(f, h))
        }
    };
    this.getMap = function (d) {
        if (!this.map) {
            if (!d) {
                d = "Map"
            }
            var c = {
                zoom: 14,
                minZoom: 5,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                mapTypeControl: true,
                mapTypeControlOptions: {
                    position: google.maps.ControlPosition.RIGHT_BOTTOM
                },
                panControl: false,
                fullscreenControl: true,
                fullscreenControlOptions: {
                    position: google.maps.ControlPosition.LEFT_BOTTOM
                },
                zoomControl: true,
                zoomControlOptions: {
                    style: google.maps.ZoomControlStyle.DEFAULT,
                    position: google.maps.ControlPosition.LEFT_BOTTOM
                },
                scaleControl: true,
                streetViewControl: true,
                streetViewControlOptions: {
                    position: google.maps.ControlPosition.LEFT_BOTTOM
                },
                styles: [{
                    featureType: "poi",
                    stylers: [{
                        visibility: "off"
                    }]
                }]                
            };
            var e = this;
            this.map = new google.maps.Map(this.mapDomElement, c);

            this.map.overlayMapTypes.setAt(this.zedIndexOverlayIndex, undefined);
            if (this.useHeatmap) {
                this.map.overlayMapTypes.setAt(this.heatMapOverlayIndex, undefined)
            }
            google.maps.event.addListener(this.map, "dragstart", function () {
                e._notifyDragStartListeners()
            });
            google.maps.event.addListener(this.map, "dragend", function () {
                e.mapHasMoved = true;
                e._notifyMapMovedListeners();
                e._notifyDragEndListeners()
            });
            if (this.useMapControls) {
                if (this.useHeatmap) {
                    this.addMapTypeControl("Heat", google.maps.MapTypeId.ROADMAP, true, (d == "Heat"))
                }
                this.addMapTypeControl("Hybrid", google.maps.MapTypeId.HYBRID, false, (d == "Hybrid"));
                this.addMapTypeControl("Map", google.maps.MapTypeId.ROADMAP, false, (d == "Map"))
            }
            google.maps.event.addListener(this.getMap(), "zoom_changed", function () {
                e._clearZedIndexMarkers()
            });
            this._initUndoBuffer()
        }
        return this.map
    };
    this.setStrokeOption = function (c, d) {
        if (this.strokeOptions[c] != d) {
            this.strokeOptions[c] = d;
            this._updateColours()
        }
    };
    this.setStrokeOptions = function (c) {
        var d = 0;
        $.each(c, function (f, g) {
            if (this.strokeOptions[f] != g) {
                this.strokeOptions[f] = g;
                d++
            }
        });
        if (d > 0) {
            this._updateColours()
        }
    };
    this._updateColours = function () {
        $.each(this.polygons, function (c, d) {
            d.setStrokeOptions(this.strokeOptions)
        });
        $.each(this.circles, function (c, d) {
            d.setStrokeOptions(this.strokeOptions)
        })
    };
    this.canShowHeatmap = function (c) {
        if (typeof (c) != "undefined") {
            this.useHeatmap = c
        }
        return this.useHeatmap
    };
    this.addUndoListener = function (c) {
        this.undoListeners.push(c)
    };
    this.addRedoListener = function (c) {
        this.redoListeners.push(c)
    };
    this.addMapMoveListener = function (c) {
        this.mapMovedListeners.push(c)
    };
    this.addDragStartListener = function (c) {
        this.dragStartListeners.push(c)
    };
    this.addDragEndListener = function (c) {
        this.dragEndListeners.push(c)
    };
    this.addPolygonChangedListener = function (c) {
        this.polygonChangedListeners.push(c)
    };
    this.addPolygonAddedListener = function (c) {
        this.polygonAddedListeners.push(c)
    };
    this.addPolygonRemovedListener = function (c) {
        this.polygonRemovedListeners.push(c)
    };
    this.getPolygons = function () {
        return this.polygons
    };
    this.deleteListeners = [];
    this.deletingSearchArea = false;
    this.canDeleteSearchArea = function () {
        return (this.polygons.length > 0 && !this.isDeletingSearchArea())
    };
    this.isDeletingSearchArea = function () {
        return this.deletingSearchArea
    };
    this.deleteSearchArea = function (c) {
        if (this.isDeletingSearchArea()) {
            return
        }
        this.deleteListeners = [];
        this.deletingSearchArea = true;
        var e = this;
        var d = function (g, f) {
            if (typeof (c) == "function") {
                c(g, f)
            }
            g.clear();
            e.polygons.splice(f, 1);
            e._snapShotForUndo();
            e._notifyPolygonRemovedListeners()
        };
        this._clearDeleteListeners();
        this.deleteListeners = $.map(this.polygons, function (h, f) {
            h.disableChanges(false);
            h.enableRollOver();
            var g = h.getPolygon();
            return google.maps.event.addListener(g, "click", function () {
                d(h, f)
            })
        })
    };
    this._clearDeleteListeners = function () {
        while (this.deleteListeners.length) {
            google.maps.event.removeListener(this.deleteListeners.pop())
        }
    };
    this.endDeleteSearchArea = function () {
        if (!this.isDeletingSearchArea()) {
            return
        }
        this.deletingSearchArea = false;
        this._clearDeleteListeners();
        $.each(this.polygons, function (c, d) {
            d.enableChanges();
            d.disableRollOver()
        })
    };
    this.isDrawingFreehand = function () {
        return this.drawingFreehand
    };
    this.tmpMapMouseUpListener = undefined;
    this.tmpPolyMouseUpListener = undefined;
    this.tmpMouseMoverListener = undefined;
    this.tmpMapPoly = undefined;
    this.tmpMapMouseDownListener = undefined;
    this.cancelFreehand = function () {
        if (!this.isDrawingFreehand()) {
            return
        }
        this._cleanUpDrawFreehand()
    };
    this.drawFreehand = function (i) {
        var h = this;
        if (this.isDrawingFreehand()) {
            return
        }
        this.drawingFreehand = true;
        var e = this.getMap();
        var d = 0;
        this.tmpMapPoly = new google.maps.Polyline({
            strokeColor: this.strokeOptions.strokeColor,
            strokeOpacity: this.strokeOptions.strokeOpacity,
            strokeWeight: this.strokeOptions.strokeWeight,
            map: e
        });
        var g = 10;
        var c = [];
        this.tmpMapMouseUpListener = undefined;
        this.tmpPolyMouseUpListener = undefined;
        e.setOptions({
            draggableCursor: "crosshair",
            draggable: false
        });
        $.each(this.polygons, function (j, k) {
            k.disableChanges(true)
        });
        this.tmpMouseMoverListener = google.maps.event.addListener(e, "mousemove", function (k) {
            if (d) {
                var j = new Date().valueOf();
                if (j - d >= g) {
                    d = j;
                    c.push(new Array(k.latLng.lat(), k.latLng.lng()))
                }
                h.tmpMapPoly.getPath().push(k.latLng)
            }
        });
        this.tmpMapMouseDownListener = google.maps.event.addListenerOnce(e, "mousedown", function (j) {
            d = new Date().valueOf()
        });
        var f = function (u) {
            h._cleanUpDrawFreehand();
            c = forceClockwiseWinding(c);
            for (var w = 0; w < h.polygons.length; ++w) {
                var x = h.polygons[w];
                var o = $.map(c, function (z, y) {
                    return (x.containsPoint(z[0], z[1]) ? y : null)
                });
                if (o.length == c.length) {
                    r = false;
                    continue
                }
                var q = splitChunks(o);
                var m = ((o[0] == 0 && o[o.length - 1] == (c.length - 1)) ? 2 : 1);
                if (q.length > m) {
                    continue
                }
                var t = x.getPolygon().getPath().getArray();
                var k = $.map(t, function (z, y) {
                    return (containsPoint(c, z.lat(), z.lng()) ? y : null)
                });
                if (!k.length && !o.length) {
                    continue
                }
                if (k.length == t.length) {
                    var j = h.polygons.splice(w, 1).pop();
                    j.clear();
                    w--;
                    continue
                }
                var l = splitChunks(k);
                var p = ((k[0] == 0 && k[k.length - 1] == (t.length - 1)) ? 2 : 1);
                if (l.length > 0 && l.length != p) {
                    continue
                }
                var j = h.polygons.splice(w, 1).pop();
                j.clear();
                w--;
                if (o.length == 0 && k.length == t.length) {
                    continue
                }
                if (q.length == 1) {
                    var v = c.splice(0, q[0][0]);
                    c.splice(0, q[0].length);
                    c = c.concat(v)
                } else {
                    $.each(q.reverse(), function (y, z) {
                        c.splice(z[0], z.length)
                    })
                } if (l.length == 1) {
                    var v = t.splice(0, l[0][0]);
                    t.splice(0, l[0].length);
                    t = t.concat(v)
                } else {
                    $.each(l.reverse(), function (y, z) {
                        t.splice(z[0], z.length)
                    })
                }
                var n = $.map(t, function (z, y) {
                    return [[z.lat(), z.lng()]]
                });
                c = c.concat(n)
            }
            var s = h._simplifyPolygon(c);
            var r = (s.length >= 3);
            if (r) {
                if (h.addPolygon(s, true)) {
                    h._notifyPolygonChangedListeners()
                } else {
                    h._notifyPolygonAddedListeners()
                }
            }
            if (typeof (i) == "function") {
                i(r)
            }
        };
        this.tmpMapMouseUpListener = google.maps.event.addListenerOnce(e, "mouseup", f);
        this.tmpPolyMouseUpListener = google.maps.event.addListenerOnce(this.tmpMapPoly, "mouseup", f)
    };
    this._cleanUpDrawFreehand = function () {
        if (this.tmpMapMouseUpListener != undefined) {
            google.maps.event.removeListener(this.tmpMapMouseUpListener);
            this.tmpMapMouseUpListener = undefined
        }
        if (this.tmpPolyMouseUpListener != undefined) {
            google.maps.event.removeListener(this.tmpPolyMouseUpListener);
            this.tmpPolyMouseUpListener = undefined
        }
        if (this.tmpMouseMoverListener != undefined) {
            google.maps.event.removeListener(this.tmpMouseMoverListener);
            this.tmpMouseMoverListener = undefined
        }
        if (this.tmpMapMouseDownListener != undefined) {
            google.maps.event.removeListener(this.tmpMapMouseDownListener);
            this.tmpMapMouseDownListener = undefined
        }
        if (this.tmpMapPoly != undefined) {
            this.tmpMapPoly.setMap(undefined);
            this.tmpMapPoly = undefined
        }
        this.drawingFreehand = false;
        var c = this.getMap();
        setTimeout(function () {
            c.setOptions({
                draggableCursor: "openhand",
                draggable: true
            })
        }, 100);
        $.each(this.polygons, function (d, f) {
            f.enableChanges()
        })
    };
    this.getInfoWindow = function () {
        if (!this.infoWindow) {
            this.infoWindow = new google.maps.InfoWindow();
            var c = this;
            google.maps.event.addListener(this.infoWindow, "closeclick", function () {
                if (c.preInfoWindowPos) {
                    c.getMap().panTo(c.preInfoWindowPos);
                    c.preInfoWindowPos = undefined
                }
            });
            google.maps.event.addListener(this.getMap(), "dragend", function () {
                c.preInfoWindowPos = undefined
            });
            google.maps.event.addListener(this.getMap(), "zoom_changed", function () {
                c.preInfoWindowPos = undefined;
                if (c.zedIndexEnabled) {
                    c.getInfoWindow().close()
                }
            });
            google.maps.event.addListener(this.getMap(), "click", function () {
                c.getInfoWindow().close()
            })
        }
        return this.infoWindow
    };
    this.closeInfoWindow = function () {
        var c = this.getInfoWindow();
        if (c) {
            c.close()
        }
    };
    this.getCenter = function () {
        return this.getMap().getCenter()
    };
    this.panTo = function (c) {
        return this.getMap().panTo(c)
    };
    this.resize = function () {
        google.maps.event.trigger(this.getMap(), "resize")
    };
    this.decodePolygons = function (d) {
        var c = [];
        $.each(d, function (f, h) {
            var g = google.maps.geometry.encoding.decodePath(h);
            var e = $.map(g, function (k, j) {
                return [[k.lat(), k.lng()]]
            });
            c.push(e)
        });
        return c
    };
    this.encodePolygons = function (d) {
        var c = $.map(d, function (g, e) {
            var f;
            if (g instanceof google.maps.Polygon) {
                f = g.getPath().getArray()
            } else {
                if (g instanceof WebMapPolygon) {
                    f = g.getPath()
                } else {
                    if (g[0] instanceof google.maps.LatLng) {
                        f = g
                    } else {
                        f = $.map(g, function (h, j) {
                            return new google.maps.LatLng(h[0], h[1])
                        })
                    }
                }
            }
            return google.maps.geometry.encoding.encodePath(f)
        });
        return c
    };
    this.serializeSearchable = function () {
        var c = {
            p: this.serializePolygons(),
            c: this.serializeCircles()
        };
        return c
    };
    this.deserializeSearchable = function (c) {
        this.deserializePolygons(c.p);
        this.deserializeCircles(c.c)
    };
    this.serializeCircles = function () {
        var c = [];
        $.each(this.circles, function (d, f) {
            var g = f.getCenter();
            c.push([g.lat(), g.lng(), f.getRadius()])
        });
        return c
    };
    this.deserializeCircles = function (c) {
        $.each(c, function (d, f) {
            this.addCircle(f[0], f[1], f[2])
        })
    };
    this.serializePolygons = function () {
        var c = [];
        $.each(this.polygons, function (d, j) {
            var h = j.getPath();
            var f = [];
            for (var d = 0; d < h.length; ++d) {
                var g = h.getAt(d);
                f.push([g.lat(), g.lng()])
            }
            c.push(f)
        });
        return c
    };
    this.deserializePolygons = function (c, e, d) {
        if (c.length == 0) {
            return
        }
        if (typeof (e) == "undefined") {
            e = true
        }
        var g = this;
        $.each(c, function (h, j) {
            g.addPolygon(j, d)
        });
        if (e) {
            var f = boundsForPolygons(c);
            this.focusMap(f.lat_min, f.lon_min, f.lat_max, f.lon_max, c)
        }
    };
    this.undo = function () {
        if (this.snapshotFlux) {
            return
        }
        this.snapshotFlux = true;
        if (this.canUndo()) {
            this.redoBuffer.push(this.currentSnapshot);
            var c = this.undoBuffer.pop();
            this._realizeSnapshot(c);
            this._notifyUndoListeners()
        }
        this.snapshotFlux = false
    };
    this.canUndo = function () {
        return (this.undoBuffer.length > 0)
    };
    this.redo = function () {
        if (this.snapshotFlux) {
            return
        }
        this.snapshotFlux = true;
        if (this.canRedo()) {
            this.undoBuffer.push(this.currentSnapshot);
            var c = this.redoBuffer.pop();
            this._realizeSnapshot(c);
            this._notifyRedoListeners()
        }
        this.snapshotFlux = false
    };
    this.canRedo = function () {
        return (this.redoBuffer.length > 0)
    };
    this.setZedIndexSticky = function (c) {
        this.zedIndexSticky = c
    };
    this.showUserMessage = function (c) {
        this.showMessage(this.message, c)
    };
    this.showMessage = function (f, e) {
        this.message = f;
        this.userMessage = e;
        var d = google.maps.ControlPosition.TOP_LEFT;
        var c = "";
        if (this.message && this.message != "") {
            c = this.message;
            if (this.userMessage && this.userMessage != "") {
                c += ": " + this.userMessage
            }
            c += "</div>";
            var g = document.createElement("div");
            $(g).attr("id", "mapsMessage").attr("class", "map-control").html(c);
            this._removeMapsMessage(google.maps.ControlPosition.TOP_LEFT, "mapsMessage");
            this.getMap().controls[d].push(g)
        } else {
            this._removeMapsMessage(d, "mapsMessage")
        }
    };
    this.focusMap = function (k, e, f, g, j) {
        var h = new google.maps.LatLng(k, e);
        var i = new google.maps.LatLng(f, g);
        var c = new google.maps.LatLngBounds(h, i);
        var d = this.getMap();
        d.setCenter(c.getCenter());
        d.fitBounds(c)
    };
    this.getBoundsUrlString = function (i) {
        var c = this.getMap().getBounds();
        if (i) {
            var e = c.getNorthEast();
            var l = c.getSouthWest();
            var h = e.lat() - l.lat();
            var f = e.lng() - l.lng();
            var k = $(this.mapDomElement).innerWidth() / f;
            var g = $(this.mapDomElement).innerHeight() / h;
            var j = this.imageMarkerSize.width / k;
            var d = this.imageMarkerSize.height / g;
            e = new google.maps.LatLng(e.lat() - j, e.lng() - d);
            c = new google.maps.LatLngBounds(l, e)
        }
        return c.toUrlValue()
    };
    this.pinCountForViewport = function () {
        var e = $(this.mapDomElement).innerWidth() / this.imageMarkerSize.width;
        var d = $(this.mapDomElement).innerHeight() / this.imageMarkerSize.height;
        var c = Math.round((e * d) * 0.3);
        return c
    };
    this.createPolygon = function () {
        var c = this.polygons.length;
        var d = new WebMapPolygon(this.map, this.strokeOptions);
        d.init();
        var e = this;
        d.addChangeListener(function () {
            e._snapShotForUndo();
            e._notifyPolygonChangedListeners()
        });
        d.addDragStartListener(function (f) {
            $.each(e.markers, function (g, h) {
                h.setOptions({
                    clickable: false
                })
            })
        });
        d.addDragEndListener(function (f) {
            $.each(e.markers, function (g, h) {
                h.setOptions({
                    clickable: true
                })
            })
        });
        this.polygons.push(d)
    };
    this.addCircle = function (e, f, c) {
        var d = new WebMapCircle(this.getMap(), this.strokeOptions);
        d.init(e, f, c);
        this.circles.splice(0, 0, d)
    };
    this.addPolygon = function (d, c) {
        var e = new WebMapPolygon(this.getMap(), this.strokeOptions);
        if (typeof (c) != "undefined") {
            e.setStrokeOptions(c)
        }
        e.initWithPolygon(d);
        var f = this;
        e.addChangeListener(function () {
            f._snapShotForUndo();
            f._notifyPolygonChangedListeners()
        });
        e.addDragStartListener(function (g) {
            $.each(f.markers, function (h, j) {
                j.setOptions({
                    clickable: false
                })
            })
        });
        e.addDragEndListener(function (g) {
            $.each(f.markers, function (h, j) {
                j.setOptions({
                    clickable: true
                })
            })
        });
        this.polygons.splice(0, 0, e);
        this._snapShotForUndo()
    };
    this.replaceSearchArea = function (c, e, d) {
        this.snapshotFlux = true;
        this.clearSearchArea();
        this.deserializePolygons(c, e, d);
        this.snapshotFlux = false
    };
    this.clearSearchArea = function () {
        this.clearPolygons();
        this.clearCircles();
        this._snapShotForUndo();
        this._notifyPolygonChangedListeners()
    };
    this.clearCircles = function () {
        while (this.circles.length) {
            this.circles.pop().clear()
        }
    };
    this.clearPolygons = function () {
        while (this.polygons.length) {
            this.polygons.pop().clear()
        }
    };
    this.getTile = function (g, e, c) {
        var d = c.createElement("DIV");
        d.style.width = "256px";
        d.style.height = "256px";
        d.style.position = "relative";
        var f = this;
        jQuery.ajax({
            url: "/ajax/maps/zedindex",
            data: {
                x: g.x,
                y: g.y,
                zoom: e
            },
            success: function (h) {
                if (typeof h == "object") {
                    while (h.length > 0) {
                        f._addZedIndexMarker(d, h.pop(), e)
                    }
                }
            }
        });
        return d
    };
    this._initUndoBuffer = function () {
        this.undoBuffer = new Array();
        this.redoBuffer = new Array();
        this.currentSnapshot = undefined;
        this._snapShotForUndo()
    };
    this._snapShotForUndo = function () {
        if (this.snapshotFlux) {
            return
        }
        this.redoBuffer.length = 0;
        if (typeof (this.currentSnapshot) != "undefined") {
            this.undoBuffer.push(this.currentSnapshot)
        }
        this.currentSnapshot = this.serializePolygons();
        if (this.undoBuffer.length > this.undoBufferSize) {
            this.undoBuffer.shift()
        }
    };
    this._realizeSnapshot = function (c) {
        if (typeof (c) != "undefined") {
            if (this.polygons.length > c.length) {
                while (this.polygons.length > c.length) {
                    var d = this.polygons.pop();
                    d.clear()
                }
            } else {
                while (this.polygons.length < c.length) {
                    this.createPolygon()
                }
            }
            $.each(this.polygons, function (f, h) {
                var g = c[f];
                h.setPolygon(g)
            });
            this.currentSnapshot = c
        } else {
            this.clearPolygons()
        }
    };
    this._notifyDragEndListeners = function () {
        for (var c in this.dragEndListeners) {
            this.dragEndListeners[c](this)
        }
    };
    this._notifyDragStartListeners = function () {
        for (var c in this.dragStartListeners) {
            this.dragStartListeners[c](this)
        }
    };
    this._notifyUndoListeners = function () {
        for (var c in this.undoListeners) {
            this.undoListeners[c](this, this.undoBuffer, this.redoBuffer)
        }
    };
    this._notifyRedoListeners = function () {
        for (var c in this.redoListeners) {
            this.redoListeners[c](this, this.undoBuffer, this.redoBuffer)
        }
    };
    this._notifyMapMovedListeners = function () {
        if (this.snapshotFlux) {
            return
        }
        for (var c in this.mapMovedListeners) {
            this.mapMovedListeners[c](this)
        }
    };
    this._notifyPolygonChangedListeners = function () {
        if (this.snapshotFlux) {
            return
        }
        for (var c in this.polygonChangedListeners) {
            this.polygonChangedListeners[c](this)
        }
    };
    this._notifyPolygonAddedListeners = function () {
        if (this.snapshotFlux) {
            return
        }
        for (var c in this.polygonAddedListeners) {
            this.polygonAddedListeners[c](this)
        }
    };
    this._notifyPolygonRemovedListeners = function () {
        if (this.snapshotFlux) {
            return
        }
        for (var c in this.polygonRemovedListeners) {
            this.polygonRemovedListeners[c](this)
        }
    };
    this._addZedIndexMarker = function (e, l, k) {
        var f = new google.maps.LatLng(l.lat, l.lng);
        var h = 256 * (l.x - Math.floor(l.x));
        var g = 256 * (l.y - Math.floor(l.y));
        var m = document.createElement("DIV");
        m.style.position = "absolute";
        $(m).addClass("map-zedindex");
        $(m).html(formatZedIndex(l.mean_estimate));
        var c = $(m).width();
        var j = $(m).height();
        if (!c) {
            c = 40
        }
        if (!j) {
            j = 15
        }
        m.style.top = (g - (j / 2)) + "px";
        m.style.left = (h - (c / 2)) + "px";
        var i = l.lat + "," + l.lng + "," + k;
        if (!this.zedIndexMarkers[i]) {
            this.zedIndexMarkers[i] = new google.maps.Marker({
                clickable: true,
                flat: true,
                icon: this.spacerMarker,
                position: f,
                title: l.location + " Zed-Index",
                map: this.getMap()
            });
            var d = this;
            google.maps.event.addListener(d.zedIndexMarkers[i], "click", function () {
                var o = d.getInfoWindow();
                var p = document.createElement("DIV");
                var n = '<p class="bottom-half medium">' + l.location + ' Zed-Index<br><strong class="brand-primary">&pound;' + ZPG.util.formatNumber(l.mean_estimate) + "</strong></p>";
                if (l.uri) {
                    n += '<p class="neither"><a href="/market/' + l.uri + '/" target="_parent">Area stats for ' + l.location + "</a></p>"
                }
                $(p).html(n);
                o.setContent(p);
                o.setPosition(f);
                o.open(d.getMap())
            })
        }
        $(e).append(m);
        return m
    };
    this._addControlButton = function (j, e, k, i, f) {
        var c = document.createElement("DIV");
        if (!f) {
            f = google.maps.ControlPosition.TOP_RIGHT
        }
        if (e) {
            c.id = e
        }
        $(c).addClass(this.buttonClass);
        $(c).html(j);
        $(c).click(k);
        var h = this;
        $(c).click(function () {
            h.setActiveMapTypeControlId(e)
        });
        var d = this.getMap();
        var g = d.controls[f].getLength();
        if (!g) {
            $(c).addClass(this.buttonLastClass);
            $(c).addClass(this.buttonFirstClass)
        } else {
            if (typeof i == "undefined" || i == g) {
                $(d.controls[f].getAt(g - 1)).removeClass(this.buttonFirstClass);
                $(c).addClass(this.buttonFirstClass)
            } else {
                if (typeof i != "undefined" && i == 0) {
                    $(d.controls[f].getAt(0)).removeClass(this.buttonLastClass);
                    $(c).addClass(this.buttonLastClass)
                }
            }
        } if (typeof i == "undefined") {
            d.controls[f].push(c)
        } else {
            d.controls[f].insertAt(i, c)
        }
        return c
    };
    this.addMapTypeControl = function (k, j, e, f, i) {
        var d = this._makeControlId(k);
        var h = this;
        var g = function () {
            $("." + h.mapTypeControlClass).removeClass(h.buttonActiveClass);
            $(this).addClass(h.buttonActiveClass);
            h.getMap().setMapTypeId(j);
            if (typeof e != "undefined") {
                if (typeof e == "function") {
                    e()
                } else {
                    h.showValueLayers(e);
                    if (!e && h.mapHasMoved) {
                        h._notifyMapMovedListeners()
                    }
                }
            }
        };
        var c = this._addControlButton(k, d, g, i);
        $(c).addClass(this.mapTypeControlClass);
        if ($(c).text() == "Heat") {
            $(c).attr("data-ga-category", "Google Map").attr("data-ga-action", "Heatmap").attr("data-ga-label", "/tracking" + window.location.pathname).click(function () {
                _gaq.push(["_trackEvent", $(c).attr("data-ga-category"), $(c).attr("data-ga-action"), $(c).attr("data-ga-label")])
            })
        }
        if (f) {
            this.setActiveMapTypeControlId(d);
            $(c).addClass(this.buttonActiveClass);
            g()
        }
        return c
    };
    this.disableStreetViewControl = function () {
        var c = this.getMap();
        c.setOptions({
            streetViewControl: false
        })
    };
    this.loadStreetViewTo = function (i, c, e, f, j) {
        if (!j) {
            j = 8
        }
        var d = this.getMap();
        var k = new google.maps.LatLng(i, c);
        var h = new google.maps.StreetViewService();
        var g = this;
        h.getPanoramaByLocation(k, j, function (l, m) {
            if (m == google.maps.StreetViewStatus.OK) {
                var n = {
                    position: l.location.latLng,
                    pov: {
                        heading: 0,
                        pitch: 0,
                        zoom: 1
                    }
                };
                var o = new google.maps.StreetViewPanorama(document.getElementById(e), n);
                o.setVisible(true)
            } else {
                if (j < 64) {
                    g.loadStreetViewTo(i, c, e, f, j * 2)
                } else {
                    if (typeof (f) == "function") {
                        f()
                    }
                }
            }
        })
    };
    this.addStreetViewControl = function (l, c, j, n, e) {
        if (!n) {
            n = "Street"
        }
        var d = this.getMap();
        var p = d.getStreetView();
        var i = this;
        var o = undefined;
        var q;
        var k = function () {
            if (typeof (o) != "undefined") {
                q = i.getActiveMapTypeControlId();
                p.setPosition(o);
                p.setVisible(true)
            }
        };
        var g = this.addMapTypeControl(n, google.maps.MapTypeId.ROADMAP, k, false, j);
        var f = "map-button-disabled";
        $(g).addClass(f);
        var m = new google.maps.LatLng(l, c);
        var h = new google.maps.StreetViewService();
        h.getPanoramaByLocation(m, 100, function (r, s) {
            if (s == google.maps.StreetViewStatus.OK) {
                $(g).removeClass(f);
                o = r.location.latLng;
                google.maps.event.addListener(p, "closeclick", function () {
                    i.activateMapTypeControlId(q)
                });
                if (e) {
                    k()
                }
            }
        })
    };
    this.addMockControl = function (d, f, c) {
        var e = this._makeControlId(d);
        this._addControlButton(d, e, f, c)
    };
    this.activateMapTypeControlId = function (c) {
        $("#" + c).click()
    };
    this.setActiveMapTypeControlId = function (c) {
        this.activeMapControlId = c
    };
    this.getActiveMapTypeControlId = function () {
        return this.activeMapControlId
    };
    this.setClearMarkersForValueLayer = function (c) {
        this.clearMarkersForValueLayer = c
    };
    this.shouldClearMarkersForValueLayer = function () {
        return this.clearMarkersForValueLayer
    };
    this.showValueLayers = function (c) {
        if (c != this.valueLayersEnabled) {
            this.valueLayersEnabled = c;
            if (c) {
                if (this.shouldClearMarkersForValueLayer()) {
                    this.clearMarkers(false)
                }
                this.nonValueLayerMessage = this.message;
                this.showMessage('<strong>Zoom in or out to see Zed-Index by area</strong><br><img src="/content/maps/images/zed-index-scale.png" alt="Zed-Index scale">');
                this.showZedIndex(true)
            } else {
                if (this.shouldClearMarkersForValueLayer()) {
                    this.showMarkers()
                }
                this.showMessage(this.nonValueLayerMessage);
                if (!this.zedIndexSticky) {
                    this.showZedIndex(false)
                }
            }
            this.showHeatMap(c)
        }
    };
    this.showZedIndex = function (c) {
        var d = this.getMap();
        if (c) {
            if (!this.zedIndexEnabled) {
                this.zedIndexEnabled = true;
                d.overlayMapTypes.setAt(this.zedIndexOverlayIndex, this)
            }
        } else {
            if (this.zedIndexEnabled) {
                this.zedIndexEnabled = false;
                d.overlayMapTypes.setAt(this.zedIndexOverlayIndex, undefined);
                this._clearZedIndexMarkers()
            }
        }
    };
    this.showHeatMap = function (c) {
        var e = this.getMap();
        if (!this.heatMapOverlay) {
            var d = {
                getTileUrl: function (h, g) {
                    var f = "/content/maps/images/heatmaps/" + g + "/" + h.x + "_" + h.y + ".png";
                    return f
                },
                tileSize: new google.maps.Size(256, 256),
                isPng: true,
                opacity: 0.3
            };
            this.heatMapOverlay = new google.maps.ImageMapType(d)
        }
        if (c) {
            if (!this.heatMapEnabled) {
                this.heatMapEnabled = true;
                e.overlayMapTypes.setAt(this.heatMapOverlayIndex, this.heatMapOverlay)
            }
        } else {
            if (this.heatMapEnabled) {
                this.heatMapEnabled = false;
                e.overlayMapTypes.setAt(this.heatMapOverlayIndex, undefined)
            }
        }
    };
    this.addForSalePin = function (e, c, d, f, p) {
        return this._addPinMarker(e, "sale", "sale", "sale", c, d, f, p)
    };
    this.addToRentPin = function (e, c, d, f, p) {
        return this._addPinMarker(e, "rent", "rent", "rent", c, d, f, p)
    };
    this.addEstimatePin = function (e, c, d, f) {
        return this._addPinMarker(e, "estimate", "estimate", "estimate", c, d, f)
    };
    this.addPricePin = function (e, c, d, f) {
        return this._addPinMarker(e, "price", "price", "price", c, d, f)
    };
    this.addTemptMePin = function (e, c, d, f) {
        return this._addPinMarker(e, "temptme", "temptme", "temptme", c, d, f)
    };
    this.addAgentPin = function (c, d, e, f) {
        return this._addPinMarker(c, "agent", "agent", "agent", d, e, f)
    };
    this.addDeveloperPin = function (c, d, e, f) {
        return this._addPinMarker(c, "developer", "developer", "developer", d, e, f)
    };
    this.addSpecialOffersPin = function (e, c, d, f) {
        return this._addPinMarker(e, "special-offers", "agent", "special-offers", c, d, f)
    };
    this.addIncentivesPin = function (e, c, d, f) {
        return this._addPinMarker(e, "government-incentives", "agent", "government-incentives", c, d, f)
    };
    this.addFavouriteForSalePin = function (e, c, d, f) {
        return this._addPinMarker(e, "sale", "favourite-sale", "sale", c, d, f)
    };
    this.addFavouriteToRentPin = function (e, c, d, f) {
        return this._addPinMarker(e, "rent", "favourite-rent", "rent", c, d, f)
    };
    this.addFavouriteEstimatePin = function (e, c, d, f) {
        return this._addPinMarker(e, "estimate", "favourite-estimate", "estimate", c, d, f)
    };
    this.addFavouritePricePin = function (e, c, d, f) {
        return this._addPinMarker(e, "price", "favourite-price", "price", c, d, f)
    };
    this.addFavouriteResultsForSalePin = function (e, c, d, f) {
        return this._addPinMarker(e, "sale", "favourite-results-sale", "sale", c, d, f)
    };
    this.addFavouriteResultsToRentPin = function (e, c, d, f) {
        return this._addPinMarker(e, "rent", "favourite-results-rent", "rent", c, d, f)
    };
    this.addFavouriteResultsEstimatePin = function (e, c, d, f) {
        return this._addPinMarker(e, "estimate", "favourite-results-estimate", "estimate", c, d, f)
    };
    this.addFavouriteResultsPricePin = function (e, c, d, f) {
        return this._addPinMarker(e, "price", "favourite-results-price", "price", c, d, f)
    };
    this.removeForSalePin = function (e, c, d) {
        return this._removePinMarker(e, "sale", c, d)
    };
    this.removeToRentPin = function (e, c, d) {
        return this._removePinMarker(e, "rent", c, d)
    };
    this.removeEstimatePin = function (e, c, d) {
        return this._removePinMarker(e, "estimate", c, d)
    };
    this.removePricePin = function (e, c, d) {
        return this._removePinMarker(e, "price", c, d)
    };
    this.removeTemptMePin = function (e, c, d) {
        return this._removePinMarker(e, "temptme", c, d)
    };
    this.removeAgentPin = function (c, d, e) {
        return this._removePinMarker(c, "agent", d, e)
    };
    this.removeDeveloperPin = function (c, d, e) {
        return this._removePinMarker(c, "developer", d, e)
    };
    this.removeFavouriteForSalePin = function (e, c, d) {
        return this._removePinMarker(e, "sale", c, d)
    };
    this.removeFavouriteToRentPin = function (e, c, d) {
        return this._removePinMarker(e, "rent", c, d)
    };
    this.removeFavouriteEstimatePin = function (e, c, d) {
        return this._removePinMarker(e, "estimate", c, d)
    };
    this.removeFavouritePricePin = function (e, c, d) {
        return this._removePinMarker(e, "price", c, d)
    };
    this.removeFavouriteResultsForSalePin = function (e, c, d) {
        return this._removePinMarker(e, "sale", c, d)
    };
    this.removeFavouriteResultsToRentPin = function (e, c, d) {
        return this._removePinMarker(e, "rent", c, d)
    };
    this.removeFavouriteResultsEstimatePin = function (e, c, d) {
        return this._removePinMarker(e, "estimate", c, d)
    };
    this.removeFavouriteResultsPricePin = function (e, c, d) {
        return this._removePinMarker(e, "price", c, d)
    };
    this.removeSpecialOffersPin = function (e, c, d) {
        return this._removePinMarker(branchId, "special-offers", c, d)
    };
    this.removeIncentivesPin = function (e, c, d) {
        return this._removePinMarker(branchId, "government-incentives", c, d)
    };
    this.addNumberedPin = function (d, i, g, h, e) {
        var j = new google.maps.LatLng(g, h);
        var f = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_letter_withshadow&chld=" + d + "|269dd7|ffffff", new google.maps.Size(40, 37), new google.maps.Point(0, 0), new google.maps.Point(11, 35));
        var c = this._addMarker({
            position: j,
            map: this.getMap(),
            title: i,
            icon: f
        });
        if (e) {
            google.maps.event.addListener(c, "click", function () {
                top.location.href = e
            })
        } else {
            c.setOptions({
                clickable: false
            })
        }
    };
    this.showMarkers = function () {
        var e = this.getMap();
        for (var d = 0; d < this.markers.length; ++d) {
            var c = this.markers[d];
            c.setMap(e)
        }
    };
    this.clearMarkers = function (c) {
        for (var e = 0; e < this.markers.length; ++e) {
            var d = this.markers[e];
            d.setMap(null)
        }
        if (c) {
            this.markers.length = 0;
            this._resetMarkerGroups()
        }
    };
    this.showMarkerGroupForEntity = function (f, h) {
        var l = this._getMarkerGroups();
        var e = l[f];
        for (var k in e) {
            for (var g in e[k]["entityIds"]) {
                var d = e[k]["entityIds"][g];
                if (d == h) {
                    var c = /^([\-\d\.]+),([\-\d\.]+)$/.exec(k);
                    if (c) {
                        var j = new google.maps.LatLng(c[1], c[2]);
                        return this.showMarkerGroup(f, k, j, h)
                    }
                }
            }
        }
    };
    this.showMarkerGroup = function (d, k, j, h) {
        var f = this._getMarkerTypeUrl(d);
        var i = this._getMarkerTypeIdParam(d);
        if (this.currentAjaxRequest) {
            this.currentAjaxRequest.abort()
        }
        var e = {};
        var c = this._getMarkerGroups();
        e[i] = c[d][k]["entityIds"];
        var g = this;
        this.currentAjaxRequest = jQuery.ajax({
            url: f,
            data: jQuery.param(e,true),
            success: function (n, l) {
                if (g.currentAjaxRequest.status == 200) {
                    var o = document.createElement("DIV");
                    //$(o).html(n);
                    var m = g.getInfoWindow();
                    $(o).find(".map-link-less-info").click(function () {
                        m.close();
                        var p = $(this).attr("data-entity-id");
                        $(o).find("div." + d + "-" + p).hide();
                        $(o).find("#map-popup-details").show();
                        m.setOptions({
                            content: o
                        });
                        m.open(g.getMap());
                        $("#map-popup-details ol").scrollTo($("li#map-popup-li-" + p), "none");
                        return false
                    });
                    $(o).find(".map-link-more-info").click(function () {
                        m.close();
                        $(o).find("#map-popup-details").hide();
                        var p = $(this).attr("data-entity-id");
                        $(o).find("div." + d + "-" + p).show();
                        m.setOptions({
                            content: o,
                            maxWidth: 550
                        });
                        m.open(g.getMap());
                        return false
                    });
                    $(o).find(".map-popup-full-details").hide();
                    m.setContent(o);
                    bds.AppendHtml(o, n);
                    m.setPosition(j);
                    g.preInfoWindowPos = g.getMap().getCenter();
                    m.open(g.getMap());
                    if (h) {
                        $(o).find("#map-popup-details ol").scrollTo($("li#map-popup-li-" + h), "none")
                    }

                    //quy.vu
                    var bdsCallBack = g["showMarkerGroupCallBack"];
                    if (typeof (bdsCallBack) == "function") {
                        bdsCallBack(o);
                    }
                } else { }
            },
            error: function () { },
            complete: function () {
                g.currentAjaxRequest = undefined
            }
        })
    };
    this._makeGroupKey = function (d) {
        var c = d.lat() + "," + d.lng();
        return c
    };
    this._getMarkerType = function (c) {
        return this.markerTypes[c]
    };
    this._getMarkerTypeIdParam = function (c) {
        var d = this._getMarkerType(c);
        return d.id
    };
    this._getMarkerTypeUrl = function (c) {
        var d = this._getMarkerType(c);
        return d.url
    };
    this._getMarkerTypeImage = function (c, m) {
        if (m) return this._getImageMarker(m);
        var d = this._getMarkerType(c);
        return this._getImageMarker(d.marker);
    };
    this._getImageMarker = function (d) {
        if (!this.imageMarkers[d]) {
            var c = new google.maps.MarkerImage(iconsBase + "pin-" + d + ".png", this.imageMarkerSize, new google.maps.Point(0, 0), this.pinPoint);
            this.imageMarkers[d] = c
        }
        return this.imageMarkers[d]
    };
    this._getMarkerGroups = function () {
        if (!this.markerGroups) {
            this._resetMarkerGroups()
        }
        return this.markerGroups
    };
    this._resetMarkerGroups = function () {
        this.markerGroups = {};
        for (var c in this.markerTypes) {
            this.markerGroups[c] = {}
        }
    };
    this._removePinMarker = function (f, e, h, j) {
        var k = new google.maps.LatLng(h, j);
        var d = this._makeGroupKey(k);
        var c = this._getMarkerGroups();
        if (!c[e][d]) {
            return
        }
        var g = $.inArray(f, c[e][d]["entityIds"]);
        if (g == -1) {
            return
        }
        c[e][d]["entityIds"].splice(g, 1);
        if (!c[e][d]["entityIds"].length) {
            if (typeof (c[e][d]["clickListener"]) != "undefined") {
                google.maps.event.removeListener(c[e][d]["clickListener"])
            }
            google.maps.event.removeListener(c[e][d]["mouseUpListener"]);
            c[e][d]["marker"].setMap(null);
            delete c[e][d]
        }
    };
    this._addPinMarker = function (i, h, d, o, l, f, j, z) {
        var isfavor = (z == "favorite");//quy.vu
        this.mapHasMoved = false;
        var m = new google.maps.LatLng(l, f);
        var p = this._makeGroupKey(m);
        var g = this._getMarkerGroups();
        if (g[h][p]) {
            g[h][p]["entityIds"].push(i);
            g[h][p].marker.count += 1;
            if (g[h][p].marker.markerGroup !== o) {
                g[h][p].marker.markerGroup = "other"
            }
            // quy.vu
            var markers = this["Markers"];
            if (markers) {
                markers[i] = g[h][p];
                var mk = markers[i].marker;
                if (isfavor && mk.isfavor == false) {
                    mk.icon1 = this._getMarkerTypeImage(null, z);
                    mk.icon2 = this._getMarkerTypeImage(null, z + "-hover");
                    mk.setIcon(mk.icon1);
                }
            }
            return
        }
        var c = this._getMarkerTypeImage(d, z);
        var c2 = this._getMarkerTypeImage(d, z + "-hover");
        var e = this.getMap();
        var n = this._addMarker({
            position: m,
            map: e,
            icon: c,
            shadow: this.shadowMarker,
            markerGroup: o,
            count: 1
        });
        g[h][p] = {
            entityIds: [i],
            marker: n,
            clickListener: undefined,
            mouseUpListener: undefined
        };
        if (typeof (j) != "undefined" && j) { } else {
            var k = this;
            g[h][p]["clickListener"] = google.maps.event.addListener(n, "click", function () {
                k.showMarkerGroup(h, p, m)
            })
        }
        g[h][p]["mouseUpListener"] = google.maps.event.addListener(n, "mouseup", function (q) {
            google.maps.event.trigger(e, "mouseup", q)
        });


        // quy.vu
        var markers = this["Markers"];
        if (markers) {
            markers[i] = g[h][p];
        }
        n.isfavor = isfavor;
        n.icon1 = c;
        n.icon2 = c2;
        google.maps.event.addListener(n, 'mouseover', function () {
            this.setIcon(this.icon2);
        });
        google.maps.event.addListener(n, 'mouseout', function () {
            this.setIcon(this.icon1);
        });
        return n
    };
    this._poi = [];
    this.findPointsOfInterest = function (k, l, h, j, c, i) {
        var g = new google.maps.LatLng(j, c);
        var d = this;
        var f = new google.maps.places.PlacesService(d.getMap());
        var e = [];
        $.each(l, function (m, n) {
            var o = {
                location: g,
                radius: h,
                types: [n]
            };
            f.radarSearch(o, function (q, p) {
                if (p == google.maps.places.PlacesServiceStatus.OK) {
                    $.each(q, function (r, s) {
                        if (!e[s.reference]) {
                            e[s.reference] = s;
                            d.addDeferredPointOfInterestMarker(k, s, f, i)
                        }
                    })
                } else {
                    console.log("BAD RESULT " + p)
                }
            })
        })
    };
    this.poiTypeMap = {
        imageMarkerSize: this.imageMarkerSize,
        nameMap: {
            uk_school: "school",
            uk_school_primary: "school",
            uk_school_secondary: "school",
            uk_school_primary_and_secondary: "school",
            national_rail_station: "rail",
            london_underground_station: "tube",
            london_dlr_station: "tube",
            poi_healthcare: "health",
            poi_food: "food",
            poi_restaurants_pubs_bars: "restaurant",
            poi_worship: "worship",
            uk_airport: "airport",
            uk_ferry_port: "port",
            uk_sports: "sports",
            poi_park_and_zoo: "parks"
        },
        markers: {},
        getMarker: function (c) {
            if (!this.markers[c]) {
                if (this.nameMap[c]) {
                    url = iconsBase + "pin-" + this.nameMap[c] + ".png";
                    this.markers[c] = new google.maps.MarkerImage(url, this.imageMarkerSize, new google.maps.Point(0, 0), this.pinPoint)
                } else {
                    this.markers[c] = new google.maps.MarkerImage("http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|81DAF5", new google.maps.Size(21, 34), new google.maps.Point(0, 0), new google.maps.Point(10, 34))
                }
            }
            return this.markers[c]
        }
    };
    this.addPointOfInterestMarker = function (j, k, i) {
        var f = this._poi[j];
        if (!f) {
            this._poi[j] = [];
            f = this._poi[j] = []
        }
        var g = this.poiTypeMap.getMarker(j);
        var h = this._addMarker({
            position: k,
            map: this.getMap(),
            icon: g,
            shadow: this.shadowMarker
        });
        f.push(h);
        var d = document.createElement("DIV");
        var e = this;
        $(d).html(i());
        var c = function () {
            var l = e.getInfoWindow();
            l.setContent(d);
            l.setPosition(k);
            e.preInfoWindowPos = e.getMap().getCenter();
            l.open(e.getMap())
        };
        google.maps.event.addListener(h, "click", c)
    };
    this.poiPopupContent = {
        cache: [],
        get: function (c) {
            return this.cache[c]
        },
        put: function (d, c) {
            var e = document.createElement("DIV");
            $(e).html(c);
            this.cache[d] = e;
            return e
        }
    };
    this.addDeferredPointOfInterestMarker = function (k, h, i, j) {
        var e = this._poi[k];
        if (!e) {
            this._poi[k] = [];
            e = this._poi[k] = []
        }
        var f = this.poiTypeMap.getMarker(k);
        var g = this._addMarker({
            position: h.geometry.location,
            map: this.getMap(),
            icon: f,
            shadow: this.shadowMarker
        });
        e.push(g);
        var d = this;
        var c = function () {
            var l = d.getInfoWindow();
            var m = d.poiPopupContent.get(h.reference);
            if (m) {
                l.setContent(m);
                l.setPosition(h.geometry.location);
                d.preInfoWindowPos = d.getMap().getCenter();
                l.open(d.getMap())
            } else {
                i.getDetails({
                    reference: h.reference
                }, function (o, n) {
                    if (n == google.maps.places.PlacesServiceStatus.OK) {
                        m = d.poiPopupContent.put(h.reference, j(o));
                        l.setContent(m);
                        l.setPosition(h.geometry.location);
                        d.preInfoWindowPos = d.getMap().getCenter();
                        l.open(d.getMap())
                    }
                })
            }
        };
        google.maps.event.addListener(g, "click", c)
    };
    this.hidePointsOfInterest = function (c) {
        var d = this._poi[c];
        if (!d) {
            return
        }
        $(d).each(function (e, f) {
            f.setMap(null)
        });
        this._poi[c] = []
    };
    this._addMarker = function (d) {
        var c = new google.maps.Marker(d);
        this.markers.push(c);
        return c
    };
    this._simplifyPolygon = function (h) {
        var j = boundsForPoints(h);
        var g = (j.lat_max - j.lat_min);
        var e = (j.lon_max - j.lon_min);
        var d = Math.max(g, e);
        var c = 0.01;
        var f = d * c;
        var i = simplifyPath(h, f);
        return i
    };
    this._removeMapsMessage = function (c, e) {
        var g = this.getMap();
        for (var f = 0; f < g.controls[c].getLength(); ++f) {
            var d = g.controls[c].getAt(f);
            if ($(d).attr("id") == e) {
                g.controls[c].removeAt(f);
                $("#" + e).remove();
                return
            }
        }
    };
    this._makeControlId = function (c) {
        var d = "map-control-" + c.toLowerCase().replace(/[^a-zA-Z0-9]/g, "-");
        return d
    };
    this._clearZedIndexMarkers = function () {
        for (var c in this.zedIndexMarkers) {
            this.zedIndexMarkers[c].setMap(undefined)
        }
        this.zedIndexMarkers = {}
    }
}
var simplifyPath = function (d, c) {
    var e = function (g, f) {
        this.p1 = g;
        this.p2 = f;
        this.distanceToPoint = function (j) {
            var i = (this.p2[1] - this.p1[1]) / (this.p2[0] - this.p1[0]),
                h = this.p1[1] - (i * this.p1[0]),
                k = [];
            k.push(Math.abs(j[1] - (i * j[0]) - h) / Math.sqrt(Math.pow(i, 2) + 1));
            k.push(Math.sqrt(Math.pow((j[0] - this.p1[0]), 2) + Math.pow((j[1] - this.p1[1]), 2)));
            k.push(Math.sqrt(Math.pow((j[0] - this.p2[0]), 2) + Math.pow((j[1] - this.p2[1]), 2)));
            return k.sort(function (m, l) {
                return (m - l)
            })[0]
        }
    };
    var b = function (n, m) {
        if (n.length <= 2) {
            return [n[0]]
        }
        var f = [],
            o = new e(n[0], n[n.length - 1]),
            h = 0,
            l = 0,
            j;
        for (var k = 1; k <= n.length - 2; k++) {
            var g = o.distanceToPoint(n[k]);
            if (g > h) {
                h = g;
                l = k
            }
        }
        if (h >= m) {
            j = n[l];
            o.distanceToPoint(j, true);
            f = f.concat(b(n.slice(0, l + 1), m));
            f = f.concat(b(n.slice(l, n.length), m))
        } else {
            j = n[l];
            o.distanceToPoint(j, true);
            f = [n[0]]
        }
        return f
    };
    var a = b(d, c);
    a.push(d[d.length - 1]);
    return a
};
var formatZedIndex = function (b) {
    var a = "&pound;";
    if (b > 999999) {
        a += (Math.round(b / 100000) / 10) + "m"
    } else {
        if (b > 999) {
            a += Math.round(b / 1000) + "k"
        } else {
            a += b
        }
    }
    return a
};
var distanceFromPointToLine = function (m, e, b) {
    var a = b.lat();
    var j = b.lng();
    var c = e.lat();
    var k = e.lng();
    var f = m.lat();
    var l = m.lng();
    var g = Math.abs(((a - c) * (k - l)) - ((c - f) * (j - k)));
    var h = Math.sqrt(Math.pow((a - c), 2) + Math.pow((j - k), 2));
    var i = g / h;
    return i
};
var boundsForPolygons = function (b) {
    var f;
    var d;
    var c;
    var a;
    $.each(b, function (g, j) {
        var h = boundsForPoints(j);
        if (parseFloat(h.lat_min) < f || f == undefined) {
            f = h.lat_min
        }
        if (parseFloat(h.lat_max) > d || d == undefined) {
            d = h.lat_max
        }
        if (parseFloat(h.lon_min) < c || c == undefined) {
            c = h.lon_min
        }
        if (parseFloat(h.lon_max) > a || a == undefined) {
            a = h.lon_max
        }
    });
    var e = [];
    e.lat_min = f;
    e.lat_max = d;
    e.lon_min = c;
    e.lon_max = a;
    return e
};
var boundsForPoints = function (i) {
    var e = function (k, j) {
        return k - j
    };
    var d = $.map(i, function (k, j) {
        return parseFloat(k instanceof google.maps.LatLng ? k.lat() : k[0])
    });
    d.sort(e);
    var g = d.shift();
    var b = d.pop();
    var f = $.map(i, function (k, j) {
        return parseFloat(k instanceof google.maps.LatLng ? k.lng() : k[1])
    });
    f.sort(e);
    var h = f.shift();
    var c = f.pop();
    var a = [];
    a.lat_min = g;
    a.lat_max = b;
    a.lon_min = h;
    a.lon_max = c;
    return a
};
var containsPoint = function (k, f, a) {
    var b = 0;
    var h = k.length;
    var c = h - 1;
    var e = $.map(k, function (l, j) {
        return (l instanceof google.maps.LatLng ? l.lat() : l[0])
    });
    var g = $.map(k, function (l, j) {
        return (l instanceof google.maps.LatLng ? l.lng() : l[1])
    });
    for (var d = 0; d < h; ++d) {
        if ((g[d] < a && g[c] >= a || g[c] < a && g[d] >= a) && (e[d] <= f || e[c] <= f)) {
            b ^= (e[d] + (a - g[d]) / (g[c] - g[d]) * (e[c] - e[d]) < f)
        }
        c = d
    }
    return b
};
var splitChunks = function (c) {
    var d = [];
    var a = [];
    var b = function (f, e) {
        return f - e
    };
    $.each(c.sort(b), function (f, g) {
        if (a.length == 0 || g == (a[a.length - 1] + 1)) {
            a.push(g)
        } else {
            d.push(a);
            a = [g]
        }
    });
    if (a.length) {
        d.push(a)
    }
    return d
};
var isClockwiseWinding = function (c) {
    var f = (c[0] instanceof google.maps.LatLng);
    var d = (f ? [c[c.length - 1].lat(), c[c.length - 1].lng()] : c[c.length - 1]);
    var b = 0;
    for (var a = 0; a < c.length; ++a) {
        var e = (f ? [c[a].lat(), c[a].lng()] : c[a]);
        b = b + (d[0] * e[1]) - (e[0] * d[1]);
        d = e
    }
    return (b < 0)
};
var forceClockwiseWinding = function (a) {
    if (!isClockwiseWinding(a)) {
        a.reverse()
    }
    return a
};
if (typeof _gaq != "undefined") {
    _gaq.push(["_trackEvent", "Google Map", "Viewed"])
};