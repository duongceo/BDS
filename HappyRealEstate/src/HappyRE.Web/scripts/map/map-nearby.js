var EventDispatcher = function ($, Log) {
    "use strict";
    var eventBag = {}
      , publicMethods = {};
    publicMethods.addListener = function (eventName, listener) {
        if (!eventBag.hasOwnProperty(eventName)) {
            eventBag[eventName] = $.Callbacks()
        }
        eventBag[eventName].add(listener);
    }
    ;
    publicMethods.dispatch = function (eventName, data) {
        if (eventBag.hasOwnProperty(eventName)) {
            data = typeof data !== "undefined" ? data : {};
            eventBag[eventName].fire(data);
        }
    };
    return publicMethods
}(jQuery);
(function ($) {
    "use strict";
    var MAP_KEY = "AIzaSyCvyvKiaQt4L6n1YPinGV93WBSJIK4GCsU";
    $.fn.mogiMap = function (options) {
        if (typeof google === "undefined" || typeof google.maps === "undefined") {
            $.error("$.mogiMap: google.maps is not loaded");
            return null
        }
        var M = {
            settings: $.extend(true, {}, $.fn.mogiMap.defaults, options),
            isDraggableStrategy: function (mapOptions) {
                if (typeof mapOptions.draggable !== "undefined") {
                    return
                }
                var isWithoutTouch = $("html").is(".no-touch")
                  , isTabletOrBigger = $(window).width() >= 768
                  , isDraggable = isWithoutTouch || isTabletOrBigger || !this.settings.disableDragIfTouchDevice;
                mapOptions.draggable = isDraggable;
                mapOptions.panControl = !isDraggable;
                mapOptions.mapTypeControl = false;
                mapOptions.styles = [{ featureType: "poi", elementType: "labels", stylers: [{ visibility: "off" }] }
                ,{
                featureType: 'administrative.locality',
                elementType: 'labels.text.fill',
                stylers: [{ color: '#d59563', visibility: "off" }]
            }];
            },
            transitLayerStatus: function (mapOptions) {
                if (typeof mapOptions.transitLayer !== "undefined") {
                    return
                }
                mapOptions.transitLayer = true;
            },
            mapConstructor: function ($domElement) {
                var mapOptions = this.settings.mapOptions;
                // quy.vu:begin
                mapOptions.disableDefaultUI = true;
                mapOptions.transitLayer = false;
                // quy.vu:end
                this.isDraggableStrategy(mapOptions);
                this.transitLayerStatus(mapOptions);
                var map = new google.maps.Map($domElement[0], mapOptions);
                //if (mapOptions.transitLayer !== false) {
                //    var transitLayer = new google.maps.TransitLayer;
                //    transitLayer.setMap(map)
                //}
                return map;
            }
        };
        if (this.length != 1) {
            $.error("$.mogiMap: Expects 1 DOM element, " + this.length + " given", this);
            return null
        }
        return M.mapConstructor(this)
    }
    ;
    $.fn.mogiMap.defaults = {
        disableDragIfTouchDevice: true,
        mapStyles: {
            media: {
                extension: "png",
                spritePath: "/scripts/map/sprite.png"
            },
            mapStyles: []
        },
        mapOptions: {
            zoom: 14,
            scrollwheel: false,
            mapTypeControl: false
        }
    };
    var _MogiMap = {
        pgIcons: null,
        IconMaker: {
            sprite: "/scripts/map/sprite.png",
            makeIcon: function (w, h, x, y, achorX, anchorY) {
                return {
                    icon: {
                        url: this.sprite,
                        size: new google.maps.Size(w, h),
                        origin: new google.maps.Point(x, y),
                        anchor: new google.maps.Point(achorX, anchorY)
                    }
                }
            },
            makePoi: function (iconIndex) {
                var w = 24
                  , h = 25
                  , xStart = 31
                  , padding = 10
                  , x = xStart + iconIndex * (w + padding)
                  , y = 83
                  , anchorX = Math.round(w / 2)
                  , anchorY = Math.round(h / 2);
                return [w, h, x, y, anchorX, anchorY]
            }
        },
        numberAbbreviation: function (number) {
            number = number.toString();
            if (number.length < 4) {
                return number
            }
            if (number.length > 6) {
                return number.substring(0, number.length - 6) + "M"
            }
            if (number.length > 3) {
                return number.substring(0, number.length - 3) + "K"
            }
        }
    };
    var MogiMapPublicMethods = {
        getDistanceFromLatLonInKm: function (lat1, lon1, lat2, lon2) {
            var R = 6371;
            var dLat = this.deg2rad(lat2 - lat1);
            var dLon = this.deg2rad(lon2 - lon1);
            var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos(this.deg2rad(lat1)) * Math.cos(this.deg2rad(lat2)) * Math.sin(dLon / 2) * Math.sin(dLon / 2);
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
            var d = R * c;
            return d
        },
        deg2rad: function (deg) {
            return deg * (Math.PI / 180)
        },
        injectGoogleMapsScript: function (callbackFunction) {
            if (!this.googleMapsIsLoaded()) {
                var script = document.createElement("script");
                script.type = "text/javascript";
                script.src = "https://maps.googleapis.com/maps/api/js?key=" + MAP_KEY + "&callback=" + callbackFunction + "&libraries=places";
                document.body.appendChild(script)
            } else if (typeof window[callbackFunction] === "function") {
                Log.log("$.mogiMap: Google Maps Script present already, calling the callback : " + callbackFunction);
                window[callbackFunction]()
            }
        },
        googleMapsIsLoaded: function () {
            return !(typeof google === "undefined" || typeof google.maps === "undefined")
        },
        getPgIcon: function (pgCategory) {
            var I = _MogiMap.IconMaker
              , hoverOffset = 110;
            if (!_MogiMap.pgIcons) {
                _MogiMap.pgIcons = {
                    FOOD: I.makeIcon.apply(I, I.makePoi(0)),
                    MEDICAL: I.makeIcon.apply(I, I.makePoi(1)),
                    SCHKDGINT: I.makeIcon.apply(I, I.makePoi(2)),
                    MALLSUPRETAI: I.makeIcon.apply(I, I.makePoi(3)),
                    MRTTRANSAIR: I.makeIcon.apply(I, I.makePoi(4)),
                    WORS: I.makeIcon.apply(I, I.makePoi(5)),
                    LISTING: I.makeIcon(20, 24, 0, 115, Math.round(20 / 2), 24),
                    PROPERTY: I.makeIcon(25, 30, 30, 115, Math.round(25 / 2), 30),
                    "LISTING-DETAIL-PROPERTY": I.makeIcon(32, 37, 219, 113, Math.round(32 / 2), 37),
                    CLUSTER: I.makeIcon(35, 35, 65, 115, Math.round(35 / 2), Math.round(35 / 2)),
                    LISTING_HOVER: I.makeIcon(20, 24, hoverOffset, 115, Math.round(20 / 2), 24),
                    PROPERTY_HOVER: I.makeIcon(25, 30, 30 + hoverOffset, 115, Math.round(25 / 2), 30),
                    CLUSTER_HOVER: I.makeIcon(35, 35, 65 + hoverOffset, 115, Math.round(35 / 2), Math.round(35 / 2))
                }
            }
            return _MogiMap.pgIcons[pgCategory]
        },
        makeClusterMarker: function (markerOptions) {
            var marker = null;
            markerOptions = markerOptions || {};
            var ClusterMarker = function (overlayValues) {
                this.hasBeenAdd = false;
                this.options = overlayValues;
                if (typeof this.options.icon === "undefined") {
                    if (typeof this.options.cssClass === "string" && this.options.cssClass !== "") {
                        switch (this.options.cssClass) {
                            case "icon-listing":
                            case "icon-station":
                            case "icon-cluster":
                            case "icon-property":
                                delete this.options.icon;
                                break;
                            default:
                                this.options.icon = MogiMapPublicMethods.getPgIcon("CLUSTER").icon;
                                break
                        }
                    } else {
                        this.options.icon = MogiMapPublicMethods.getPgIcon("CLUSTER").icon
                    }
                }
                this.$markerElement = $("<div />").addClass("pgmap-marker");
                this.$markerIcon = $("<div />").addClass("pgmap-marker-icon");
                this.$labelElementContent = $("<span />");
                this.$labelElement = $("<span />").addClass("pgmap-marker-label hide").append(this.$labelElementContent);
                if (this.options.cssClass == "icon-station") {
                    this.$labelArrowElement = $("<span />").addClass("pgmap-marker-label-arrow-container").append($("<span />").addClass("pgmap-marker-label-arrow"));
                    this.$labelArrowElement.insertBefore(this.$labelElementContent)
                }
                this.$badgeElement = $("<span />").addClass("pgmap-marker-badge hide");
                this.$markerElement.append(this.$markerIcon).append(this.$labelElement).append($("<span />").addClass("pgmap-marker-badge-wrapper").append(this.$badgeElement));
                this.setValues(this.options)
            };
            ClusterMarker.prototype = $.extend(new google.maps.OverlayView, {
                onAdd: function () {
                    this.getPanes().overlayMouseTarget.appendChild(this.$markerElement.get(0));
                    this.hasBeenAdd = true
                },
                draw: function () {
                    var position = this.getProjection().fromLatLngToDivPixel(this.get("position"))
                      , left = position.x
                      , top = position.y;
                    this.$markerElement.css({
                        top: top + "px",
                        left: left + "px"
                    })
                },
                onRemove: function () {
                    this.$markerElement.remove()
                },
                over: function () {
                    this.$markerElement.addClass("over")
                },
                out: function () {
                    this.$markerElement.removeClass("over")
                },
                setPosition: function (latLng) {
                    var p = null;
                    if (typeof latLng.lat === "number") {
                        p = new google.maps.LatLng(latLng.lat, latLng.lng)
                    } else {
                        p = latLng
                    }
                    this.set("position", p);
                    if (this.hasBeenAdd) {
                        this.draw()
                    }
                },
                getPosition: function () {
                    return this.get("position")
                },
                setLabel: function (label) {
                    if (label) {
                        if (typeof label === "object") {
                            label = label.text
                        }
                        if (typeof label == "number") {
                            label = _MogiMap.numberAbbreviation(label)
                        }
                        this.$labelElementContent.html(label);
                        this.$labelElement.removeClass("hide")
                    } else {
                        this.$labelElement.addClass("hide");
                        this.$labelElementContent.html("")
                    }
                },
                getLabel: function () {
                    return this.$labelElementContent.text()
                },
                setCssClass: function (cssClass) {
                    if (cssClass) {
                        this.$markerElement.addClass(cssClass)
                    }
                },
                removeCssClass: function (cssClass) {
                    if (cssClass) {
                        this.$markerElement.removeClass(cssClass)
                    }
                },
                setIcon: function (icon) {
                    if (!icon) {
                        return
                    }
                    var css = {
                        width: icon.size.width,
                        height: icon.size.height,
                        backgroundPosition: icon.origin.x * -1 + "px " + icon.origin.y * -1 + "px",
                        left: icon.anchor.x * -1 + "px",
                        bottom: (icon.size.height - icon.anchor.y) * -1 + "px"
                    };
                    if (icon.url) {
                        css.backgroundImage = 'url("' + icon.url + '")'
                    }
                    this.$markerIcon.css(css);
                    this.set("icon", icon)
                },
                setOpacity: function (opacity) {
                    this.$markerElement.css({
                        opacity: opacity
                    })
                },
                addListener: function (event, callback) {
                    var self = this;
                    self.$markerElement.on(event, function (e) {
                        callback.call(self, e)
                    })
                },
                setZIndex: function (zIndex) {
                    this.$markerElement.css({
                        zIndex: zIndex
                    })
                },
                setBadge: function (text) {
                    if (text) {
                        this.$badgeElement.html(text).removeClass("hide")
                    } else {
                        this.$badgeElement.addClass("hide").html("")
                    }
                }
            });
            marker = new ClusterMarker(markerOptions);
            return marker
        }
    };
    $.extend({
        mogiMap: function () {
            return MogiMapPublicMethods
        }
    })
})(jQuery);
function widgetMapNearbyCallback() {
    "use strict";
    var Map = {
        map: null,
        propertyPosition: null,
        infoWindow: null,
        googlePlacesMarkers: [],
        specificLocationMarker: null,
        $nearbyWidget: null,
        $panels: null,
        itemTemplate: '<div class="place {{pgCategory}}"><div class="pull-left place-name">{{name}}</div><div class="pull-right place-distance">{{distance}}</div></div>',
        noItemTemplate: '<div class="place {{pgCategory}} no-place">{{text}}</div>',
        specificLocationTemplate: '<div class="mode-of-travel"><div class="pull-left mode-name">{{name}}</div><div class="pull-right mode-distance">{{value}}</div></div>',
        init: function () {
            var $mapWidget = $(".map-nearby-widget")
              , $headerMap = $(".cover-img-box .gmap-noimage")
              , $mapCanvas = $(".map-canvas")
              , draggable = $("html").is(".no-touch") || $(window).width() >= 768;
            if ($headerMap.length !== 0) {
                $(window).trigger("googlemaps.loaded")
            }
            Map.$nearbyWidget = $mapWidget.find(".map-nearby-location .nav-tabs");
            Map.$panels = $(".nearby-locations").find(".place");
            if ($mapCanvas.length === 0) {
                return false
            }
            Map.propertyPosition = new google.maps.LatLng($mapCanvas.data("latitude"), $mapCanvas.data("longitude"));
            Map.map = $mapCanvas.mogiMap({
                mapOptions: {
                    center: Map.propertyPosition,
                    zoom: 15,
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
                }
            });
            var input = $mapWidget.find("#searchForLocation")[0];
            var options = {
                types: ["establishment"],
                componentRestrictions: {
                    country: $mapCanvas.data("region")
                }
            };
            var autocomplete = new google.maps.places.Autocomplete(input, options);
            autocomplete.bindTo("bounds", Map.map);
            Map.infoWindow = new google.maps.InfoWindow;
            google.maps.event.addListener(Map.infoWindow, "closeclick", function () {
                Map.handleInfoWindowClose()
            });
            var propertyMarker = new google.maps.Marker({
                map: Map.map,
                position: Map.propertyPosition,
                icon: Map.getGooglePlaceMarkerIconByType("LISTING-DETAIL-PROPERTY").icon
            });
            Map.addListenersToMarker(propertyMarker, $mapCanvas.data("title"));
            var directionsDisplay = new google.maps.DirectionsRenderer;
            directionsDisplay.setOptions({
                suppressMarkers: true
            });
            autocomplete.addListener("place_changed", function () {
                var place = autocomplete.getPlace();
                $(".specific-nearby-location").html("");
                if (Map.specificLocationMarker != null) {
                    Map.specificLocationMarker.setMap(null)
                }
                $.cookie("myLocation", "", { expires: 7, path: '/' });
                $.cookie("myLocationId", "", { expires: 7, path: '/' });
                directionsDisplay.setMap(null);
                if (!place.geometry) {
                    return
                }
                Map.specificLocationMarker = new google.maps.Marker({
                    map: Map.map,
                    position: place.geometry.location,
                    icon: Map.getGooglePlaceMarkerIconByType("PROPERTY_HOVER").icon
                });
                $.cookie("myLocation", $(input).val(), { expires: 7, path: '/' });
                $.cookie("myLocationId", place.place_id, { expires: 7, path: '/' });
                Map.addListenersToMarker(Map.specificLocationMarker, place.name);
                Map.adjustBounds();
                var origin = Map.propertyPosition;
                var destination = place.geometry.location.lat() + ", " + place.geometry.location.lng();
                var directionsService = new google.maps.DirectionsService;
                var request = {
                    origin: origin,
                    destination: destination,
                    travelMode: google.maps.DirectionsTravelMode.TRANSIT
                };
                directionsDisplay.setMap(Map.map);
                directionsService.route(request, function (response, status) {
                    if (status === "OK") {
                        var leg = response.routes[0].legs[0];
                        var duration = leg.duration.text;
                        if (leg.duration.value > 60 * 60) {
                            duration = (leg.duration.value / (60 * 60)).toFixed() + "+ hrs"
                        }
                        var value = leg.distance.text + " (" + duration + ")";
                        var template = Map.specificLocationTemplate.render({
                            name: "Buýt",
                            value: value
                        });
                        $(".specific-nearby-location").append(template);
                        var point = response.routes[0].legs[0]
                    }
                });
                request = {
                    origin: origin,
                    destination: destination,
                    travelMode: google.maps.DirectionsTravelMode.DRIVING
                };
                directionsService.route(request, function (response, status) {
                    if (status === "OK") {
                        var leg = response.routes[0].legs[0]
                          , duration = leg.duration.text
                          , value = null
                          , template = null;
                        if (leg.duration.value > 60 * 60) {
                            duration = (leg.duration.value / (60 * 60)).toFixed() + "+ hrs"
                        }
                        value = leg.distance.text + " (" + duration + ")";
                        template = Map.specificLocationTemplate.render({
                            name: "Ô tô",
                            value: value
                        });
                        $(".specific-nearby-location").append(template);
                        template = Map.specificLocationTemplate.render({
                            name: "Taxi",
                            value: value
                        });
                        $(".specific-nearby-location").append(template);
                        template = Map.specificLocationTemplate.render({
                            name: place.name,
                            value: leg.distance.text
                        });
                        $(".specific-nearby-location").prepend(template);
                        directionsDisplay.setDirections(response)
                    }
                })
            });
            $(input).val($.cookie("myLocation"));
            if ($(input).val().length > 0 && $.cookie("myLocationId").length > 0) {
                var service = new google.maps.places.PlacesService(Map.map);
                service.getDetails({
                    placeId: $.cookie("myLocationId")
                }, function (place, status) {
                    if (status === google.maps.places.PlacesServiceStatus.OK) {
                        autocomplete.set("place", place)
                    }
                })
            }
            Map.itemTemplate = Hogan.compile(Map.itemTemplate);
            Map.noItemTemplate = Hogan.compile(Map.noItemTemplate);
            Map.specificLocationTemplate = Hogan.compile(Map.specificLocationTemplate);
            Map.registerCategoryClickListener();
            Map.registerMarkerLinksClickListener();
            var defaultLocationType = Map.$nearbyWidget.find("li a").filter(":first");
            defaultLocationType.prop("checked", true);
            //defaultLocationType.click();
            return true
        },
        handleInfoWindowClose: function () {
            Map.clearPreviousSidebarSelections()
        },
        findPanelByCategory: function (pgCategory) {
            return Map.$panels.find(".panel-title[data-type=" + pgCategory + "]")
        },
        openCategory: function (pgCategory) {
            Map.findPanelByCategory(pgCategory).click()
        },
        markCategoryAsEmpty: function (pgCategory) {
            Map.findPanelByCategory(pgCategory).each(function () {
                var $panelTitle = $(this);
                $panelTitle.find(".emptyTitle").removeClass("hide");
                $panelTitle.find(".actualTitle").hide().closest("a").addClass("disabled")
            })
        },
        markCategoryAsSearched: function (pgCategory) {
            Map.findPanelByCategory(pgCategory).each(function () {
                var $actualTitle = $(this).find(".actualTitle");
                $actualTitle.html($actualTitle.attr("title"))
            })
        },
        registerCategoryClickListener: function () {
            Map.$nearbyWidget.on("click", "li a", function (e) {
                $(".nearby-locations .place").hide();
                var pgCategory = $(this).data("code")
                  , hasCalledPlaces = $(this).data("has-called-places-" + pgCategory);
                if (!hasCalledPlaces) {
                    $(this).data("has-called-places-" + pgCategory, true);
                    Map.nearbySearch(Map.getGooglePlacesTypesByCategory(pgCategory), pgCategory)
                } else {
                    $(".nearby-locations").find(".place." + pgCategory).show()
                }
                Map.infoWindow.close();
                Map.showMarkersForCategory(pgCategory);
                Map.adjustBounds()
            })
        },
        registerMarkerLinksClickListener: function () {
            $(".nearby-locations").on("click mouseover", ".place", function (e) {
                e.preventDefault();
                var marker = $(this).data("marker");
                if (marker != null) {
                    Map.clearPreviousSidebarSelections();
                    Map.openInfoWindow(marker);
                    Map.markSidebarItem(marker)
                }
                return false
            })
        },
        showMarkersForCategory: function (pgCategory) {
            for (var i = Map.googlePlacesMarkers.length - 1; i >= 0; i--) {
                var marker = Map.googlePlacesMarkers[i];
                marker.setVisible(typeof pgCategory === "undefined" || marker.pgCategory === pgCategory)
            }
        },
        adjustBounds: function () {
            var bounds = [];
            for (var i = Map.googlePlacesMarkers.length - 1; i >= 0; i--) {
                bounds.push(Map.googlePlacesMarkers[i].position)
            }
            bounds.push(Map.propertyPosition);
            if (Map.specificLocationMarker != null) {
                bounds.push(Map.specificLocationMarker.position)
            }
            var googleBounds = new google.maps.LatLngBounds;
            bounds.map(function (position) {
                googleBounds.extend(position)
            });
            if (!Map.googlePlacesMarkers.length) {
                return
            }
            Map.map.fitBounds(googleBounds);
            if ($("body").outerWidth() > 999) {
                if (Map.map.getBounds() != undefined) {
                    Map.map.fitBounds(Map.paddedBounds(0, 0, 0, 500))
                }
            }
        },
        paddedBounds: function (npad, spad, epad, wpad) {
            if (Map.map.getBounds() == undefined) {
                return
            }
            var SW = Map.map.getBounds().getSouthWest();
            var NE = Map.map.getBounds().getNorthEast();
            var topRight = Map.map.getProjection().fromLatLngToPoint(NE);
            var bottomLeft = Map.map.getProjection().fromLatLngToPoint(SW);
            var scale = Math.pow(2, Map.map.getZoom());
            var SWtopoint = Map.map.getProjection().fromLatLngToPoint(SW);
            var SWpoint = new google.maps.Point((SWtopoint.x - bottomLeft.x) * scale + wpad, (SWtopoint.y - topRight.y) * scale - spad);
            var SWworld = new google.maps.Point(SWpoint.x / scale + bottomLeft.x, SWpoint.y / scale + topRight.y);
            var pt1 = Map.map.getProjection().fromPointToLatLng(SWworld);
            var NEtopoint = Map.map.getProjection().fromLatLngToPoint(NE);
            var NEpoint = new google.maps.Point((NEtopoint.x - bottomLeft.x) * scale - epad, (NEtopoint.y - topRight.y) * scale + npad);
            var NEworld = new google.maps.Point(NEpoint.x / scale + bottomLeft.x, NEpoint.y / scale + topRight.y);
            var pt2 = Map.map.getProjection().fromPointToLatLng(NEworld);
            return new google.maps.LatLngBounds(pt1, pt2)
        },
        nearbySearch: function (googleTypes, pgCategory) {
            var nearbyPlacesRequest = {
                location: Map.propertyPosition,
                radius: 1500,
                language: 'vi',
                types: googleTypes
            }
              , placesService = new google.maps.places.PlacesService(Map.map)
              , callback = function (results, status) {
                  Map.nearbySearchCallback(results, status, pgCategory)
              };
            placesService.nearbySearch(nearbyPlacesRequest, callback)
        },
        convertNearbySearchResults: function (results, pgCategorySearched) {
            var markerData = []
              , place = null;
            if (typeof Map.existingMarkers === "undefined") {
                Map.existingMarkers = []
            }
            for (var i = 0; i < results.length; i++) {
                place = results[i];
                $.each(place.types, function (index, googlePlacesType) {
                    var pgCategory = Map.getCategoryByGooglePlacesType(googlePlacesType);
                    if (pgCategory === null || pgCategory != pgCategorySearched) {
                        return
                    }
                    var markerKey = "m" + place.id + "-" + pgCategory;
                    if (typeof Map.existingMarkers[markerKey] !== "undefined") {
                        return
                    }
                    var visible = typeof pgCategorySearched === "undefined" || pgCategory === pgCategorySearched
                      , marker = Map.createGooglePlaceMarker(place, pgCategory, visible);
                    markerData.push({
                        place: place,
                        marker: marker,
                        pgCategory: pgCategory
                    });
                    Map.existingMarkers[markerKey] = 1
                })
            }
            return markerData
        },
        nearbySearchCallback: function (results, status, pgCategorySearched) {
            if (status == google.maps.places.PlacesServiceStatus.ZERO_RESULTS) {
                var text = $(".map-nearby-location li a[data-code='" + pgCategorySearched + "']").data("empty");
                var li = $(Map.noItemTemplate.render({
                    pgCategory: pgCategorySearched,
                    text: text
                }));
                $(".nearby-locations").append(li);
                return
            }
            if (status == google.maps.places.PlacesServiceStatus.OK) {
                var markerData = this.convertNearbySearchResults(results, pgCategorySearched);
                if (markerData === [])
                    return;
                markerData.sort(function (a, b) {
                    var distanceA = a.marker.distance
                      , distanceB = b.marker.distance;
                    return distanceA < distanceB ? -1 : distanceA > distanceB ? 1 : 0
                });
                for (var i = 0; i < markerData.length; i++) {
                    var place = markerData[i].place
                      , marker = markerData[i].marker
                      , pgCategory = markerData[i].pgCategory
                      , $li = Map.createSidebarLineItem(place, marker.distance, pgCategory);
                    Map.googlePlacesMarkers.push(marker);
                    Map.bindMarkerAndLink(marker, $li)
                }
                Map.adjustBounds();
                if (pgCategorySearched) {
                    Map.markCategoryAsSearched(pgCategorySearched)
                }
                if (typeof pgCategorySearched === "undefined" && results.length < 20) {
                    Map.$panels.each(function () {
                        var $panel = $(this)
                          , pgCategory = $panel.find(".panel-title").data("type");
                        if (!$panel.find(".panel-body > li").length) {
                            Map.markCategoryAsEmpty(pgCategory)
                        } else {
                            Map.markCategoryAsSearched(pgCategory)
                        }
                    })
                }
            } else if (status == google.maps.places.PlacesServiceStatus.ZERO_RESULTS) {
                if (pgCategorySearched) {
                    Map.markCategoryAsEmpty(pgCategorySearched)
                } else {
                    Map.$panels.each(function () {
                        var pgCategory = $(this).find(".panel-title").data("type");
                        Map.markCategoryAsEmpty(pgCategory)
                    })
                }
            }
        },
        createSidebarLineItem: function (place, distance, pgCategory) {
            var $list = $(".nearby-locations")
              , distanceStr = distance < 1 ? distance * 1e3 + " m" : distance + " km"
              , data = {
                  id: place.id,
                  name: place.name,
                  distance: distanceStr,
                  distanceVal: distance,
                  pgCategory: pgCategory
              }
              , $li = $(Map.itemTemplate.render(data));
            $list.append($li);
            return $li
        },
        bindMarkerAndLink: function (marker, $a) {
            marker.link = $a;
            $a.data("marker", marker)
        },
        createGooglePlaceMarker: function (place, pgCategory, visible) {
            var marker = new google.maps.Marker({
                map: Map.map,
                position: place.geometry.location,
                icon: Map.getGooglePlaceMarkerIconByType(pgCategory).icon,
                visible: visible
            });
            marker.place = place;
            marker.pgCategory = pgCategory;
            marker.link = null;
            marker.distance = Map.getDistance(place, Map.propertyPosition).toFixed(2);
            google.maps.event.addListener(marker, "click", Map.clickGooglePlaceMarker);
            google.maps.event.addListener(marker, "mouseover", Map.clickGooglePlaceMarker);
            return marker
        },
        getGooglePlaceMarkerIconByType: function (pgCategory) {
            return $.mogiMap().getPgIcon(pgCategory)
        },
        openInfoWindow: function (marker) {
            var distanceStr = marker.distance < 1 ? marker.distance * 1e3 + " m" : marker.distance + " km";
            Map.infoWindow.setContent(marker.place.name + " - " + distanceStr);
            Map.infoWindow.open(Map.map, marker)
        },
        addListenersToMarker: function (marker, content) {
            marker.addListener("click", function () {
                Map.infoWindow.setContent(content);
                Map.infoWindow.open(Map.map, this)
            });
            marker.addListener("mouseover", function () {
                Map.infoWindow.setContent(content);
                Map.infoWindow.open(Map.map, this)
            })
        },
        clickGooglePlaceMarker: function () {
            var marker = this;
            Map.openInfoWindow(marker);
            Map.clearPreviousSidebarSelections();
            Map.markSidebarItem(marker)
        },
        clearPreviousSidebarSelections: function () {
            Map.$panels.find(".selected").removeClass("selected")
        },
        markSidebarItem: function (marker) {
            marker.link.addClass("selected")
        },
        getCategoryByGooglePlacesType: function (googlePlacesType) {
            var mappings = Map.getGooglePlaceTypeMappings().googleToPg;
            if (googlePlacesType in mappings) {
                return mappings[googlePlacesType]
            }
            return null
        },
        getGooglePlacesTypesByCategory: function (pgCategory) {
            var mappings = Map.getGooglePlaceTypeMappings().pgToGoogle;
            if (pgCategory in mappings) {
                return mappings[pgCategory]
            }
            return null
        },
        getGooglePlacesTypes: function () {
            return $.map(Map.getGooglePlaceTypeMappings().googleToPg, function (element, index) {
                return index
            })
        },
        getGooglePlaceTypeMappings: function () {
            var googleToPg = {}
              , pgToGoogle = {};
            pgToGoogle.MALLSUPRETAI = [];
            pgToGoogle.MALLSUPRETAI.push("bicycle_store");
            pgToGoogle.MALLSUPRETAI.push("book_store");
            pgToGoogle.MALLSUPRETAI.push("clothing_store");
            pgToGoogle.MALLSUPRETAI.push("convenience_store");
            pgToGoogle.MALLSUPRETAI.push("department_store");
            pgToGoogle.MALLSUPRETAI.push("furniture_store");
            pgToGoogle.MALLSUPRETAI.push("hardware_store");
            pgToGoogle.MALLSUPRETAI.push("home_goods_store");
            pgToGoogle.MALLSUPRETAI.push("jewelry_store");
            pgToGoogle.MALLSUPRETAI.push("pet_store");
            pgToGoogle.MALLSUPRETAI.push("shoe_store");
            pgToGoogle.MALLSUPRETAI.push("shopping_mall");
            pgToGoogle.MALLSUPRETAI.push("store");
            pgToGoogle.MRTTRANSAIR = [];
            pgToGoogle.MRTTRANSAIR.push("airport");
            pgToGoogle.MRTTRANSAIR.push("bus_station");
            pgToGoogle.MRTTRANSAIR.push("subway_station");
            pgToGoogle.MRTTRANSAIR.push("taxi_stand");
            pgToGoogle.MRTTRANSAIR.push("train_station");
            pgToGoogle.FOOD = [];
            pgToGoogle.FOOD.push("bakery");
            pgToGoogle.FOOD.push("bar");
            pgToGoogle.FOOD.push("cafe");
            pgToGoogle.FOOD.push("food");
            pgToGoogle.FOOD.push("grocery_or_supermarket");
            pgToGoogle.FOOD.push("meal_delivery");
            pgToGoogle.FOOD.push("meal_takeaway");
            pgToGoogle.FOOD.push("restaurant");
            pgToGoogle.MEDICAL = [];
            pgToGoogle.MEDICAL.push("dentist");
            pgToGoogle.MEDICAL.push("doctor");
            pgToGoogle.MEDICAL.push("fire_station");
            pgToGoogle.MEDICAL.push("health");
            pgToGoogle.MEDICAL.push("hospital");
            pgToGoogle.MEDICAL.push("pharmacy");
            pgToGoogle.MEDICAL.push("police");
            pgToGoogle.WORS = [];
            pgToGoogle.WORS.push("church");
            pgToGoogle.WORS.push("hindu_temple");
            pgToGoogle.WORS.push("mosque");
            pgToGoogle.WORS.push("place_of_WORS");
            pgToGoogle.WORS.push("synagogue");
            pgToGoogle.SCHKDGINT = [];
            pgToGoogle.SCHKDGINT.push("school");
            pgToGoogle.SCHKDGINT.push("university");
            $.each(pgToGoogle, function (pgKey, arrayGoogleNames) {
                $.each(arrayGoogleNames, function (key, googleName) {
                    googleToPg[googleName] = pgKey
                })
            });
            return {
                googleToPg: googleToPg,
                pgToGoogle: pgToGoogle
            }
        },
        getDistance: function (object1, object2) {
            var location1 = object1
              , location2 = object2;
            if ("geometry" in object1 && "location" in object1.geometry) {
                location1 = object1.geometry.location
            }
            if ("geometry" in object2 && "location" in object2.geometry) {
                location2 = object2.geometry.location
            }
            return $.mogiMap().getDistanceFromLatLonInKm(location1.lat(), location1.lng(), location2.lat(), location2.lng())
        }
    };
    Map.init()
}
(function ($) {
    $(document).ready(function () {
        var $map = $("#map-canvas")
          , $body = $("body")
          , $w = $(window)
          , forceLoad = $("[data-force-load-gmaps]").length !== 0;
        if (forceLoad) {
            $.mogiMap().injectGoogleMapsScript("forceLoadCallback")
        }
        if ($map.length) {
            var y = Math.round($map.offset().top) - $w.height()
              , showMap = function () {
                  if (!$.mogiMap().googleMapsIsLoaded()) {
                      $.mogiMap().injectGoogleMapsScript("widgetMapNearbyCallback")
                  } else {
                      widgetMapNearbyCallback()
                  }
                  $body.trigger("ga.listing.widgetMapNearby.show");
                  $w.off("scroll.widgetMapNearby")
              };
            f = function () {
                var showTime = $w.scrollTop() > y;
                if (showTime) {
                    $w.trigger("widgetMapNearby.show")
                }
            }
            ;
            f();
            EventDispatcher.addListener("height.changed", function () {
                y = Math.round($map.offset().top) - $w.height()
            });
            $w.on("scroll.widgetMapNearby", f);
            $w.on("widgetMapNearby.show", showMap);
            showMap();
        }
    })
})(jQuery);