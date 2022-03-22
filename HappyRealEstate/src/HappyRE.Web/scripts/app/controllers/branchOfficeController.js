function branchOfficeController($scope, mbnAgentServices) {
    $scope.City = {
        Options: [],
        Value: 0,
        Changed: function () {
            $scope.District.GetOptions(this.Value);
            GetMBNAgent(this.Value, 0);
        },
        Init: function () {
			mbnAgentServices.GetLocationAgent(0).then(function (rp) {
				rp = rp.data;
				$scope.City.Options = rp.Data;
			});
        }
    };
    $scope.District = {
        Options: [],
        Value: 0,
        Changed: function () {
            GetMBNAgent($scope.City.Value, this.Value);
        },
        GetOptions: function (id) {
			mbnAgentServices.GetLocationAgent(id).then(function (rp) {
				rp = rp.data;
                $scope.District.Options = rp.Data;
            });
        }
    };
    $scope.MBNAgents = [];

    function GetMBNAgent(cityId, districtId) {
        $scope.MBNAgents = [];
		mbnAgentServices.GetMBNAgents(cityId, districtId).then(function (rp) {
			rp = rp.data;
            $scope.MBNAgents = rp.Data;
        });
    }
	$scope.GoBack = function () {
		window.history.back();
	};

    $scope.ShowInfo = function (type, name, address, mobile, phone, ext) {
        var display = '';
        var typeId = type || 1;
        if (typeId == 2) {
            display = 'Đại lý';
        }
        if (typeId == 3) {
            display = 'Điểm NT'
        }
        var tmp = -1;
        if (name != undefined && address != undefined) {           
            var check = address.toLowerCase().startsWith(name.toLowerCase());
            if (check == true)
                tmp = 0;
            
        }

        if (name != undefined && name.length > 0 && tmp < 0)
            display += " " + name;
        if (address != undefined && address.length > 0) {
            if (tmp < 0) {
                display += ", " + address;
            }
            else {
                display += address;
            }
        }
        if (mobile != undefined && mobile.length > 0)
            display += ", ĐT: " + mobile;
        if (phone != undefined && phone.length > 0)
            display += ", " + phone;
        if (ext != undefined && ext.length > 0)
            display += " bấm " + ext;
        return display;
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
 
    $scope.BrandMaps = [];
    function GetBrandMaps(cityId, districtId) {
        cityId = cityId || 0;
        districtId = districtId || 0;

        $scope.BrandMaps=[];
		mbnAgentServices.GetMBNAgents(cityId, districtId).then(function (rp) {
			rp = rp.data;
            if (rp.Status == true) {
                $scope.BrandMaps = rp.Data;
            }                
        })
    };
    $scope.ShowMapByCity = function (CityId, DistrictId) {
        GetBrandMaps(CityId, DistrictId);

        $("#modalBrand").one('shown.bs.modal', function () {
            $scope.FullMaps($scope.BrandMaps);
        }).one('hidden.bs.modal', function () {
            //self.GetBrands(0, 0);
        }).modal("show");
    }

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

        var icons = ["/content/images/location-daily.png","/content/images/location-daily-hover.png","/content/images/location-vp.png","/content/images/location-vp-hover.png"];
        var points = [];
        for (i = 0; i < items.length; i++) {
            var item = items[i];
            if (item == null || item.IsHeader || (item.MapPosition == null) || item.MapPosition == "") {
                continue;
            }
            var point = getLatLng(item.MapPosition);
            if (point == null) continue;

            var icon = (item.IsBrand == true ? icons[2] : icons[0]);
            var title = item.MapTitle || '';
            if (title.length == 0) {
                title = $scope.ShowInfo(item.AgentTypeId, item.Name, item.Address, item.Mobile, item.Phone, item.PhoneExt);
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
        //var mapLabel = new MapLabel({
        //    text: 'Quần đảo Hoàng Sa (Việt Nam)',
        //    position: new google.maps.LatLng(16.328829535273602, 112.01797485351562),
        //    map: map,
        //    fontSize: 11,
        //    align: 'center'
        //});
        //var mapLabel2 = new MapLabel({
        //    text: 'Quần đảo Trường Sa (Việt Nam)',
        //    position: new google.maps.LatLng(8.676572640126338, 113.939208984375),
        //    map: map,
        //    fontSize: 11,
        //    align: 'center'
        //});
        //end
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
    $scope.City.Init();
    GetMBNAgent(0, 0);

};
branchOfficeController.$inject = ['$scope', 'mbnAgentServices'];
mogiApp.controller('branchOfficeController', branchOfficeController);