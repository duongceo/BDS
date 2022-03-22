var polygonUtil = (function ($, bootbox) {
    var root = this;
    root.data = { ReferId: 0, ReferTypeId: 0 };
    root.map = null;
    root.bootbox = bootbox;
    var Map = function (options) {
        var self = this;
        var geocoder =null;
        var drawingManager = null;
        root.map = self;
        self.setting = $.extend({}, { id: 'map-canvas', lat: 10.675990661444875, lng: 106.69943332672119, zoom: 10, polygon: null, address: "", markerOnly: false, markerDragend: null }, options);

        self.map = null;
        self.myshape = null;
        self.infoWindow = new google.maps.InfoWindow();
        self.colors = ['#1E90FF', '#FF1493', '#32CD32', '#FF8C00', '#4B0082'];
        self.selectedColor = '';
        self.selectedShape = null; // Polygon được chọn
        self.location = { lat: 10.675990661444875, lng: 106.69943332672119, zoom: 10, changed: false, toString: function () { return this.lat + ' ' + this.lng + ' ' + this.zoom;} }; // Vị trí marker trung tâm
        self.points = [];
        self.controls = document.createElement('DIV');
        self.top_center = document.createElement('DIV');
        self.template = {
            'toolbar': '<div id="maps-toolbar"><button id="maps-toolbar-save">Save</button><button id="maps-toolbar-center">Center</button><button id="maps-toolbar-import">Import</button><button id="maps-toolbar-delete">Delete</button></div>'
            //'import': '<div id="maps-import"><textarea id="maps-import-json"></textarea><button id="maps-import-apply">Apply</button><button id="maps-import-cancel">Cancel</button><div>'
        };
        self.marker = null;
        self.polygon = null;

        self.clearSelection = function () {
            if (self.selectedShape) {
                self.selectedShape.setEditable(false);
                self.selectedShape = null;
            }
        }

        self.setSelection = function (shape) {
            if (shape.type !== 'marker') {
                self.clearSelection();
                self.selectedShape = shape;
                self.selectedShape.setEditable(true);
                self.selectColor(shape.get('fillColor') || shape.get('strokeColor'));
            }
            if (shape.type == google.maps.drawing.OverlayType.POLYGON) {
                self.polygon = shape;
                self.getPolygonPoints(shape);
            }
            self.myshape = shape;
        }

        self.deleteSelectedShape = function () {
            if (self.selectedShape) {
                self.selectedShape.setMap(null);
                drawingManager.setOptions({
                    drawingControl: true,
                    polylineOptions: {
                        editable: true
                    },
                    drawingMode: google.maps.drawing.OverlayType.POLYGON,
                });
                self.points = [];
            } else {
                root.bootbox.alert({ message: "Phải chọn Polygon trước khi xóa!", size: "small" });
            }
        }

        self.selectColor = function (color) {
            self.selectedColor = color;
            // Retrieves the current options from the drawing manager and replaces the
            // stroke or fill color as appropriate.
            var polylineOptions = drawingManager.get('polylineOptions');
            polylineOptions.strokeColor = color;
            drawingManager.set('polylineOptions', polylineOptions);

            //var rectangleOptions = drawingManager.get('rectangleOptions');
            //rectangleOptions.fillColor = color;
            //drawingManager.set('rectangleOptions', rectangleOptions);

            //var circleOptions = drawingManager.get('circleOptions');
            //circleOptions.fillColor = color;
            //drawingManager.set('circleOptions', circleOptions);

            var polygonOptions = drawingManager.get('polygonOptions');
            polygonOptions.fillColor = color;
            drawingManager.set('polygonOptions', polygonOptions);
        }

        self.getPolygonPoints = function (shape) {
            if (shape.type !== google.maps.drawing.OverlayType.POLYGON) {
                return null;
            }
            var points = [];
            var vertices = shape.getPath();
            for (var i = 0; i < vertices.getLength() ; i++) {
                var xy = vertices.getAt(i);
                points.push(xy.lat(), xy.lng());
            }
            self.points = points;
        };

        self.setMakerCenter = function (shape) {
            if (!shape) return;
            var point = shape.getBounds().getCenter();
            self.location.lat = point.lat();
            self.location.lng = point.lng();
            self.location.changed = true;

            self.loadMarkerCenter(); // reload
        }
        self.setMarkerLocaiton = function (lat, lng) {
            self.map.setCenter(new google.maps.LatLng(lat, lng));
            self.location.lat = lat;
            self.location.lng = lng;
            self.location.changed = true;
            self.loadMarkerCenter(); // reload
        };
        self.setMarkerInfo = function (v) {
            self.infoWindow.setContent("<div style='max-width: 350px;'>" + v + "</div>");
            if (v) self.infoWindow.open(self.map, self.marker);
        };

        self.markerCallback = function () {
            if (self.setting.markerDragend) {
                self.setting.markerDragend(self.location.lat, self.location.lng);
            }
        };

        self.setAddress = function (v) {
            self.getLocationByAddress(v).then(function (p) {
                self.map.setCenter(new google.maps.LatLng(p.lat(), p.lng()));
                self.location.lat = p.lat();
                self.location.lng = p.lng();
                self.setMarkerInfo(v);
                self.markerCallback();
                self.setMarkerLocaiton(p.lat(), p.lng());
            });
        };

        self.getLocationByAddress = function (v) {
            var defer = $.Deferred();
            if (geocoder == null) {
                geocoder= new google.maps.Geocoder();
            }
            geocoder.geocode({
                'address': v,
                'partialmatch': true

            }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    defer.resolve(results[0].geometry.location);
                } else {
                    defer.reject();
                }
            });

            return defer.promise();
        };

        self.beforeSave = null;

        self.savePolygon = function () {
            if (self.polygon != null) {
                self.getPolygonPoints(self.polygon);
            }
            if (self.points == null || self.points.length < 5) {
                root.bootbox.alert({ message: "Chưa vẽ bản đồ!", size: "small" });
                return;
            }
            self.location.zoom = self.map.getZoom();

            var p = {
                referId: root.data.ReferId,
                referTypeId: root.data.ReferTypeId,
                location: self.location.toString(),
                points: self.points
            };
            
            if (typeof (self.beforeSave) == "function") {
                p = $.extend(p, self.beforeSave());
            }

            $.post("/api/map/update-polygon", p,
                function (resp) {
                    root.bootbox.alert("Đã lưu thành công!");
                });
        }

        self.initialize = function (lat, lng, zoom, polygon) {
            if (lat != null) {
                self.location.lat = lat;
                self.location.lng = lng;
                self.location.zoom = zoom;
                self.location.changed = true;
            }
            var point = new google.maps.LatLng(self.location.lat, self.location.lng);

            // Create Map
            self.map = new google.maps.Map(document.getElementById(self.setting.id), {
                zoom: zoom,
                center: point,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                disableDefaultUI: true,
                zoomControl: true,
                fullscreenControl: true
            });

            // Marker Only
            if (self.setting.markerOnly == true) {
                self.loadMarkerCenter();
                self.setMarkerInfo(self.setting.address);
                return;
            }

            // Creates a drawing manager attached to the map that allows the user to draw
            // markers, lines, and shapes.
            drawingManager = new google.maps.drawing.DrawingManager({
                //drawingMode: google.maps.drawing.OverlayType.POLYGON,
                drawingControlOptions: {
                    position: google.maps.ControlPosition.TOP_RIGHT,
                    drawingModes: [
                      //google.maps.drawing.OverlayType.MARKER,
                      //google.maps.drawing.OverlayType.CIRCLE,
                      google.maps.drawing.OverlayType.POLYGON,
                      //google.maps.drawing.OverlayType.POLYLINE,
                      //google.maps.drawing.OverlayType.RECTANGLE
                    ]
                },
                markerOptions: {
                    draggable: true
                },
                polylineOptions: {
                    editable: true,
                    draggable: true
                },
                polygonOptions: {
                    strokeColor: "#FF0000",
                    strokeOpacity: 0.4,
                    strokeWeight: 3,
                    fillColor: "#32CD32",
                    fillOpacity: 0.4,
                    editable: true,
                    draggable: true
                },
                map: self.map
            });

            google.maps.event.addListener(drawingManager, 'overlaycomplete', function (e) {
                var newShape = e.overlay;
                newShape.type = e.type;
                if (e.type !== google.maps.drawing.OverlayType.MARKER) {
                    drawingManager.setOptions({ drawingControl: false });
                    // Switch back to non-drawing mode after drawing a shape.
                    drawingManager.setDrawingMode(null);
                    // Add an event listener that selects the newly-drawn shape when the user mouses down on it.
                    google.maps.event.addListener(newShape, 'click', function (e) {
                        if (e.vertex !== undefined) {
                            if (newShape.type === google.maps.drawing.OverlayType.POLYGON) {
                                var path = newShape.getPaths().getAt(e.path);
                                path.removeAt(e.vertex);
                                if (path.length < 3) {
                                    newShape.setMap(null);
                                }
                            }
                        }
                        self.setSelection(newShape);
                    });
                    self.setSelection(newShape);
                    self.setMakerCenter(newShape);

                } else if (e.type == google.maps.drawing.OverlayType.MARKER) {
                    //var shape = e.overlay;
                    //shape.type = e.type;
                    //var point = shape.getPosition();
                }
            });

            // Clear the current selection when the drawing mode is changed, or when the map is clicked.
            google.maps.event.addListener(drawingManager, 'drawingmode_changed', self.clearSelection);
            google.maps.event.addListener(self.map, 'click', self.clearSelection);

            // toolbar
            self.map.controls[google.maps.ControlPosition.RIGHT_TOP].push(self.controls);
            self.controls.innerHTML = self.template.toolbar;
            google.maps.event.addDomListener(self.controls.querySelector('#maps-toolbar-save'), 'click', self.savePolygon);
            google.maps.event.addDomListener(self.controls.querySelector('#maps-toolbar-center'), 'click', function () { self.setMakerCenter(self.polygon); });
            google.maps.event.addDomListener(self.controls.querySelector('#maps-toolbar-import'), 'click', self.showImport);
            google.maps.event.addDomListener(self.controls.querySelector('#maps-toolbar-delete'), 'click', self.deleteSelectedShape);

            // load polygon
            google.maps.event.addListenerOnce(self.map, 'tilesloaded', function () {
                //this part runs when the mapobject is created and rendered
                self.loadPolygon(polygon);
            });
            //google.maps.event.addListener(self.map, 'zoom_changed', function () {});
            //google.maps.event.trigger(self.map, "resize");
        }

        self.loadMarkerCenter = function () {
            // marker
            if (self.marker) {
                self.marker.setMap(null);
            }
            self.marker = new google.maps.Marker({
                map: self.map,
                draggable: true,
                position: new google.maps.LatLng(self.location.lat, self.location.lng)
            });
            google.maps.event.addListener(self.marker, "dragend", function (evt) {
                self.location.lat = evt.latLng.lat();
                self.location.lng = evt.latLng.lng();
                self.location.changed = true;
                self.markerCallback();
            });
        }

        self.loadPolygon = function (obj) {
            // Marker Center
            self.loadMarkerCenter();

            if (obj == null) {
                // Polygon: Enable Draw
                drawingManager.setOptions({
                    drawingControl: true,
                    polylineOptions: { editable: true },
                    drawingMode: google.maps.drawing.OverlayType.POLYGON,
                });
                return;
            }
            drawingManager.setDrawingMode(null);
            drawingManager.setOptions({ drawingControl: false });

            var latlng = [], coordinate = obj.coordinates[0];
            for (i = 0; i < coordinate.length; i++) {
                latlng.push(new google.maps.LatLng(coordinate[i][0], coordinate[i][1]));
            }
            if (self.polygon != null) {
                self.polygon.setMap(null);
                self.points = [];
            }
            self.polygon = new google.maps.Polygon({
                paths: latlng,
                strokeColor: "#FF0000",
                strokeOpacity: 0.4,
                strokeWeight: 3,
                fillColor: "#32CD32",
                fillOpacity: 0.4,
                type: google.maps.drawing.OverlayType.POLYGON,
                draggable: true
                //editable: true
            });
            self.polygon.setMap(self.map);
            google.maps.event.addListener(self.polygon, 'click', function (e) {
                if (e.vertex !== undefined) {
                    if (this.type === google.maps.drawing.OverlayType.POLYGON) {
                        var path = this.getPaths().getAt(e.path);
                        path.removeAt(e.vertex);
                        if (path.length < 3) {
                            this.setMap(null);
                        }
                    }
                }
                this.setEditable(true);
                self.setSelection(this);
            });
            var point = self.polygon.getBounds().getCenter();
            self.map.fitBounds(self.polygon.getBounds());
            self.map.setZoom(self.map.getZoom());
            self.map.setCenter(point);
        }

        self.showImport = function () {
            root.bootbox.prompt({
                title: "Import Polygon",
                inputType: 'textarea',
                callback: function (result) {
                    if (result) {
                        self.loadPolygon(JSON.parse(result));
                        self.setMakerCenter(self.polygon);
                    }
                }
            });
        }

        self.initialize(self.setting.lat, self.setting.lng, self.setting.zoom, self.setting.polygon);

        return self;
    },
    GetPolygon = function (options, beforeSave) {
        var settings = $.extend({}, { referId: 0, referTypeId: 0, lat: null, lng: null, zoom: 10, polygon: null, address: '' }, options);
        $.post("/api/map/get-polygon", { referId: settings.referId, referTypeId: settings.referTypeId },
            function (resp) {
                if (resp) {
                    root.data = resp;
                    var items = [10.827997518904857, 106.63719884765624, 10];
                    if (resp.Location !== null && resp.Location != "") {
                        items = resp.Location.split(" ");
                    }
                    settings.lat = items[0];
                    settings.lng = items[1];
                    settings.zoom = parseInt(items[2]);
                    settings.polygon = JSON.parse(resp.JSON);
                    var obj = polygonUtil.Map(settings);
                    obj.beforeSave = beforeSave;
                } else {
                    root.data.ReferId = settings.referId;
                    root.data.ReferTypeId = settings.referTypeId;
                    var geocoderLocation = new google.maps.Geocoder();
                    geocoderLocation.geocode({ 'address': settings.address },
                        function (results, status) {
                            if (status == google.maps.GeocoderStatus.OK) {
                                settings.lat = results[0].geometry.location.lat();
                                settings.lng = results[0].geometry.location.lng();
                            }
                            obj = polygonUtil.Map(settings);
                            obj.beforeSave = beforeSave;
                        });
                }
            });
    };

    return {
        Map: Map,
        GetPolygon: GetPolygon
    };
})(jQuery, bootbox);
