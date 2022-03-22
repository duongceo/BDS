var mogiMapUtil = (function ($) {
    var map ;
    var infowindow;
    var coords = {};
    var makers = [];
    var icons = { "hospital": "hospital_H", "school": "school", "convenience_store": "shoppingcart", "gym": "https://www.google.com/maps/vt/icon/name=assets/icons/poi/tactile/pinlet_shadow-2-medium.png,assets/icons/poi/tactile/pinlet_outline-2-medium.png,assets/icons/poi/tactile/pinlet-2-medium.png&highlight=ff000000,ffffff,db4437,ffffff&color=ff000000&text=Gym&psize=8&ay=45?scale=1.25", "bank": "bank_intl", "restaurant": "restaurant", "supermarket": "shoppingcart", "shopping_mall": "shoppingbag" };
    var nearbys = {};
    function InitMap(div_canvas, mapPosition, title, brief, zoomLevel) {
        showMap(div_canvas, mapPosition, title, brief, zoomLevel);
    }

    function AddNearBy(type) {
        if (nearbys[type]) {
            for (var i = 0; i < makers.length; i++) {
                var m = makers[i];
                if (m.mogiTypes.indexOf(type) != -1) {
                    makers[i].setMap(map);
                }
            }
            return;
        }
        var service = new google.maps.places.PlacesService(map);
        service.nearbySearch({
            location: coords,
            radius: 1000,
            type: type
        }, callback);
        nearbys[type] = true;
    }

    function callback(results, status) {
        if (status === google.maps.places.PlacesServiceStatus.OK) {
            for (var i = 0; i < results.length; i++) {
                createMarker(results[i]);
            }
        }
    }
    function clearMarkers(type) {
        for (var i = 0; i < makers.length; i++) {
            var m = makers[i];
            if (m.mogiTypes.indexOf(type) != -1) {
                makers[i].setMap(null);
            }
        }
        //makers.length = 0;
    }

    function createMarker(place) {
        var n = icons[place.types[0]] || '';
        var icon = 'https://www.google.com/maps/vt/icon/name=assets/icons/poi/tactile/pinlet_shadow-2-medium.png,assets/icons/poi/tactile/pinlet_outline-2-medium.png,assets/icons/poi/tactile/pinlet-2-medium.png,assets/icons/poi/quantum/pinlet/' + n + '_pinlet-2-medium.png&highlight=ff000000,ffffff,db4437,ffffff&color=ff000000?scale=1.25';
        if (n == '') icon = 'https://www.google.com/maps/vt/icon/name=assets/icons/poi/tactile/pinlet_shadow-2-medium.png,assets/icons/poi/tactile/pinlet_outline-2-medium.png,assets/icons/poi/tactile/pinlet-2-medium.png&highlight=ff000000,ffffff,db4437,ffffff&color=ff000000?scale=1.25';
        if (n.substr(0, 4) == 'http') icon = n;
        var marker = new google.maps.Marker({
            map: map,
            position: place.geometry.location,
            icon: icon
        });
        marker.mogiTypes = place.types;
        makers.push(marker);
        google.maps.event.addListener(marker, 'click', function () {
            infowindow.setContent(place.name);
            infowindow.open(map, this);
        });
    }
    var showMap = function (div_canvas, mapPosition, title, brief, zoomLevel) {

        var n = mapPosition.indexOf(",");
        var lat = mapPosition.substring(0, n);
        var lng = mapPosition.substring(n + 1);
        coords.lat = parseFloat(lat);
        coords.lng = parseFloat(lng);
        initialize();

        infowindow = new google.maps.InfoWindow();
        

        map.setCenter(new google.maps.LatLng(coords.lat, coords.lng), 13);
        plotPoint(coords.lat, coords.lng, name, brief);
        google.maps.event.trigger(map, 'resize');

        function plotPoint(srcLat, srcLon, title, popUpContent, markerIcon) {
            var myLatlng = new google.maps.LatLng(srcLat, srcLon);
            var marker = new google.maps.Marker({
                position: myLatlng,
                map: map,
                title: title,
                icon: 'https://www.google.com/maps/vt/icon/name=assets/icons/poi/tactile/pinlet_shadow-2-medium.png,assets/icons/poi/tactile/pinlet_outline-2-medium.png,assets/icons/poi/tactile/pinlet-2-medium.png,assets/icons/poi/quantum/pinlet/dot_pinlet-2-medium.png&highlight=ff000000,ffffff,00deb6,ffffff&color=ff000000?scale=1.25'
            });            
            var infowindow = new google.maps.InfoWindow({
                content: '<div>' + popUpContent + '</div>'
            });

            infowindow.open(map, marker);

            google.maps.event.addListener(marker, 'click', function () {
                infowindow.open(map, marker);
            });
        }
        function initialize() {
            var latlng = new google.maps.LatLng(coords.lat, coords.lng);
            var myOptions = {
                zoom: zoomLevel || 14,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById(div_canvas), myOptions);
            google.maps.event.addListenerOnce(map, 'idle', function () {
                google.maps.event.trigger(map, 'resize');
                map.setCenter(new google.maps.LatLng(coords.lat, coords.lng), 13);
            });
            
        }
        //return map;
    };

    var searchLocationDialog = function (div_canvas, address, cb) {
        var map;
        var geocoder;

        initialize();
        google.maps.event.trigger(map, 'resize');


        function plotPoint(srcLat, srcLon, address) {
            var myLatlng = new google.maps.LatLng(srcLat, srcLon);
            var marker = new google.maps.Marker({
                position: myLatlng,
                map: map,
                draggable: true,
                animation: google.maps.Animation.DROP
            });
            var infowindow = new google.maps.InfoWindow({
                content: '<div>' + address + '</div>'
            });
            google.maps.event.addListener(marker, 'click', function () {
                infowindow.open(map, marker);
            });
            google.maps.event.addListener(marker, 'dragend', function () {
                var location = marker.getPosition();
                getAddress(location).then(function (address) {
                    map.setCenter(new google.maps.LatLng(location.lat(), location.lng()));
                    infowindow.setContent("<div style='max-width: 350px;'>" + address + "</div>");
                });
                if (cb != null)
                    cb(location.lat(), location.lng());
            });
        }

        function initialize() {
            var latlng = new google.maps.LatLng(10.838027, 106.628387);
            var myOptions = {
                zoom: 16,
                center: latlng,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById(div_canvas), myOptions);
            geocoder = new google.maps.Geocoder();
            if (address.length > 0) {
                getLocation(address).then(function (location) {
                    map.setCenter(new google.maps.LatLng(location.lat(), location.lng()));
                    plotPoint(location.lat(), location.lng(), address);
                    if (cb != null)
                        cb(location.lat(), location.lng());
                });
            }
        }

        function getLocation(address) {
            var defer = $.Deferred();
            geocoder.geocode({
                'address': address,
                'partialmatch': true

            }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    defer.resolve(results[0].geometry.location);
                } else {
                    defer.reject();
                }
            });

            return defer.promise();
        }

        function getAddress(position) {
            var defer = $.Deferred();
            geocoder.geocode({
                latLng: position
            }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    defer.resolve(results[0].formatted_address);
                } else {
                    defer.reject();
                }
            });

            return defer.promise();
        }

    };

    var getStaticMap = function (position, size, zoom) {
        var url = "http://maps.google.com/maps/api/staticmap?center={0}&markers={0}&zoom={1}&size={2}";
        return $.format(url, position, zoom, size);
    };

    return {
        InitMap: InitMap,
        AddNearBy: AddNearBy,
        clearMarkers:clearMarkers,
        //showMap: showMap,
        searchLocationDialog: searchLocationDialog,
        getStaticMap: getStaticMap
    };

})(jQuery);