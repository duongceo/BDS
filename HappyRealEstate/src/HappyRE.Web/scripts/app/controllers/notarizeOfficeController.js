function notarizeOfficeController($scope, cmsServices) {
    $scope.Data = {};
    $scope.TEST = "test";
    var loadData = function () {
        cmsServices.GetNotarizeOffices().then(function (rp) {
			rp = rp.data;
            parseData(rp.Data);
        });
    };

    var init = (function () {
        loadData();
    }());

    var parseData = function (data) {
        for (var i = 0; i < data.length; i++) {
            var obj = data[i];
            if ($scope.Data[obj.City] == void (0))
                $scope.Data[obj.City] = {};
            if ($scope.Data[obj.City][obj.District] == void (0)) {
                $scope.Data[obj.City][obj.District] = [];
            }
            $scope.Data[obj.City][obj.District].push(obj);
        }
        console.log($scope.Data);
    }

    $scope.ShowInfo = function (name, address, street, phone) {
        var display = '';
        var typeId = 1;

        var tmp = [];
        if (name != undefined && name.length > 0)
            tmp.push(name);
        if (address != undefined && address.length > 0) {
            tmp.push(address);
        }
        if (street != undefined && street.length > 0) {
            tmp.push(street);
        }
        if (phone != undefined && phone.length > 0) {
            tmp.push("ĐT: " + phone);
        }

        return tmp.join(", ");
    }

    $scope.ShowMapByCity = function (city) {
        console.log(city);
        var cityItems = [];
        for (var key in $scope.Data[city]) {
            for (var i = 0; i < $scope.Data[city][key].length; i++) {
                cityItems.push($scope.Data[city][key][i]);
            }
        }

        $("#modalBrand").one('shown.bs.modal', function () {
            $scope.FullMaps(cityItems);
        }).one('hidden.bs.modal', function () {
            //self.GetBrands(0, 0);
        }).modal("show");
    }

    $scope.ShowMapByDistrict = function (city, district) {
        console.log(city + ' ' + district);

        $("#modalBrand").one('shown.bs.modal', function () {
            $scope.FullMaps($scope.Data[city][district]);
        }).one('hidden.bs.modal', function () {
            //self.GetBrands(0, 0);
        }).modal("show");
    }
    $scope.ShowMap = function (item) {
        $("#modalBrand").one('shown.bs.modal', function () {
            var markers = $scope.FullMaps([item]);
            if (markers.length > 0) {
                markers[0].ShowInfo();
            }
        }).one('hidden.bs.modal', function () {
            //self.GetBrands(0, 0);
        }).modal("show");
    };
    function getLatLng(v) {
        v = v.replace(/[\(\s\)]/g, "");
        var items = v.split(",");
        if (items.length != 2) return null;
        return new google.maps.LatLng(items[0], items[1]);
    }

    // show full map
    $scope.FullMaps = function (items) {
        document.getElementById("map_canvas").innerHTML = "";
        if (items.length == 0) {
            return;
        }

        var icons = ["/content/images/location-daily.png", "/content/images/location-daily-hover.png", "/content/images/location-vp.png", "/content/images/location-vp-hover.png"];
        var points = [];
        for (i = 0; i < items.length; i++) {
            var item = items[i];
            if (item == null || (item.Location == null) || item.Location == "") {
                continue;
            }
            var point = getLatLng(item.Location);
            if (point == null) continue;

            var icon = icons[2];
            var title = '';
            if (title.length == 0) {
                title = $scope.ShowInfo(item.OrgName, item.Address, item.Street, item.Phone);
            }
            points.push({ point: point, name: name, title: title, icon: icon });
        }

        //hoàng sa & trường sa            
        var customMapType = new google.maps.StyledMapType([
          {
              featureType: 'water',
              elementType: 'labels',
              stylers: [{ visibility: 'off' }]
          },
          {
              featureType: 'landscape',
              elementType: 'labels',
              stylers: [{ visibility: 'off' }]
          }
        ], {
            name: 'Custom Style'
        });
        var customMapTypeId = 'custom_style';
        //end

        var myOptions = {
            zoom: 13,
            center: points[0].point,
            zoomControl: true,
            fullscreenControl: true,
            mapTypeControl: false,
            mapTypeControlOptions: {
                mapTypeIds: [google.maps.MapTypeId.ROADMAP, customMapTypeId]
            }
        };
        var markersArray = [];
        var map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
        //hoàng sa & trường sa
        map.mapTypes.set(customMapTypeId, customMapType);
        map.setMapTypeId(customMapTypeId);

        for (i = 0; i < points.length; i++) {
            var item = points[i];
            addMarker(item.point, item.name, item.title, item.icon);
        }

        function addMarker(point, title, content, markerIcon) {
            var marker = new google.maps.Marker({
                position: point,
                map: map,
                title: title,
                icon: markerIcon
            });
            markersArray.push(marker);
            marker.infowindow = new google.maps.InfoWindow({
                content: '<div style="max-width: 350px;">' + content + '</div>'
            });

            self.openedWindow = null;
            google.maps.event.addListener(marker, 'click', function () {
                if (self.openedWindow != null) {
                    self.openedWindow.close();
                }
                marker.infowindow.open(map, marker);
                self.openedWindow = marker.infowindow;
            });

            google.maps.event.addListener(map, 'click', function () {
                if (self.openedWindow != null) {
                    self.openedWindow.close();
                }
            });

            marker.ShowInfo = function () {
                this.infowindow.open(map, marker);
                self.openedWindow = marker.infowindow;
            }
        }
        return markersArray;
    };


};
notarizeOfficeController.$inject = ['$scope', 'cmsServices'];
mogiApp.controller('notarizeOfficeController', notarizeOfficeController);