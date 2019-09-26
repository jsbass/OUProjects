google.maps.event.addDomListener(window, 'load', initMap);

var mapElem = document.getElementById("map");

var activeLocation, activeShape,
    buildingList, buildingShapes,
    parkingList, parkingShapes,
    marker, deleteMenu, locationControl = new MapUtils.LocationControl(),centerControl;

function SaveInfo(building) {
    MapUtils.convertBuildingGoogleMapsDataToGeoData(building);
    console.log("normally will send bld to server");
    console.log(JSON.stringify(building));
    MapUtils.postBuilding(baseUrl, building, function(i) {
        if (i == 1) {
            alert("success");
        } else {
            alert('something happened. Result: ' + i);
        }
    });
    MapUtils.convertBuildingGeoDataToGoogleMapsData(building);
}

function SaveParking(parking) {
    MapUtils.convertBuildingGoogleMapsDataToGeoData(parking);
    MapUtils.postParking(baseUrl,
        parking,
        function(i) {
            if (i == 1) {
                alert("success");
            } else {
                alert('something happened. Result: ' + i);
            }
        });
    MapUtils.convertBuildingGeoDataToGoogleMapsData(parking);
}

function ClearShape(building) {
    building.GeoData = {
        type: "Polygon",
        coordinates: [[]]
    }
}

function setActiveLocation(location, type) {
    activeLocation = location;
    var infoDiv, saveFunc, stokeColor, fillColor;
    switch (type) {
        case 'building':
            stokeColor = '#FFF';
            fillColor = '#555';
            infoDiv = buildingInfoTemplate(location, locationControl.closeInfoDiv, true);
            saveFunc = SaveInfo;
            break;
        case 'parking':
            strokeColor = '#00F';
            fillColor = '#77F';
            infoDiv = parkingInfoTemplate(location, locationControl.closeInfoDiv, true, MapUtils.Parking.Types);
            saveFunc = SaveParking;
            break;
        default:
            console.error(type + ' is an unrecognized location type');
            return;
    }
    console.log(location);
    var saveButton = document.createElement('input');
    saveButton.className = 'save-button';
    saveButton.type = 'button';
    saveButton.value = 'Save';
    saveButton.onclick = function () {
        saveFunc(location);
    };
    infoDiv.appendChild(saveButton);
    locationControl.setInfoDiv(infoDiv);
    if (activeLocation == null) {
        activeShape.setMap(null);
        marker.setMap(null);
        map.panTo(center);
    } else {
        var shape = new google.maps.Polygon({
            strokeColor: stokeColor,
            strokeWeight: 1,
            fillColor: fillColor,
            fillOpacity: .2,
            paths: activeLocation.shape.coordinates,
            editable: true,
            map: map
        });
        activeShape.setMap(null);
        activeShape = shape;
        marker.setPosition(activeLocation.marker.coordinates);
        marker.setMap(map);
        google.maps.event.addListener(shape.getPath(), 'set_at', function (e) {
            activeLocation.shape.coordinates = activeShape.getPaths().b.map(function (i) {
                return i.b.map(function (j) { return { lng: j.lng(), lat: j.lat() }; });
            });
            console.log('set_at');
            console.log(e);
        });
        google.maps.event.addListener(shape.getPath(), 'insert_at', function (e) {
            activeLocation.shape.coordinates = activeShape.getPaths().b.map(function (i) {
                return i.b.map(function (j) { return { lng: j.lng(), lat: j.lat() }; });
            });
            console.log('insert_at');
            console.log(e);
        });
        google.maps.event.addListener(shape.getPath(), 'remove_at', function (e) {
            activeLocation.shape.coordinates = activeShape.getPaths().b.map(function (i) {
                return i.b.map(function (j) { return { lng: j.lng(), lat: j.lat() }; });
            });
            console.log('remove_at');
            console.log(e);
        });
        google.maps.event.addListener(shape, 'rightclick', function(e) {
            if (e.vertex != undefined) {
                console.log('vertex');
                deleteMenu.open(map, shape.getPath(), e.vertex);
            } else {
                console.log('not vertex');
            }
        });
        map.panTo(activeLocation.marker.coordinates);
    }

}

function refreshBuildingList() {
    MapUtils.getBuildings(baseUrl,
        function(response) {
            buildingList = response.buildings;
            buildingList.forEach(function(b) {
                MapUtils.convertBuildingGeoDataToGoogleMapsData(b);
            });
            locationControl.addObjectType(
                        'Buildings',
                        function (b) { return b.name + ' (' + b.ouBuildingCode + ')'; },
                        function (b) {
                            setActiveLocation(b, 'building');
                        },
                        function (b, text) {
                            if (text === "") return true;
                            return b.name.toLowerCase().indexOf(text.toLowerCase()) > -1 || b.ouBuildingCode.toLowerCase().indexOf(text.toLowerCase()) > -1;
                        },
                        function (successFunc) {
                            var name = window.prompt('Building Name');
                            if (name === null) return;
                            if (name === "") {
                                alert('Name cannot be empty');
                                return;
                            }
                            var code = '';
                            var getCode = true;
                            while (getCode) {
                                var code = window.prompt("Building Code");
                                if (code === null) return;
                                var buildingsWithCode = buildingList.filter(function (i) { return i.ouBuildingCode === code });
                                getCode = code !== '' && buildingsWithCode.length > 0;
                                if (getCode) {
                                    var response = confirm(buildingsWithCode[0].name + ' already uses the code: ' + code + '\nRetry?');
                                    if (!response) return;
                                }
                            }
                            alert('drag marker to starting spot and click on it to set the building location');
                            var marker = new google.maps.Marker({
                                position: MapUtils.ouCenter,
                                map: map,
                                draggable: true
                            });
                            map.panTo(marker.position);
                            marker.addListener('click',
                                function () {
                                    var polyCoordOffset = 0.001;
                                    var response = confirm('set building here?');
                                    if (response) {
                                        marker.setMap(null);
                                        successFunc({
                                            name: name,
                                            ouBuildingCode: code,
                                            marker: {
                                                type: "Point",
                                                coordinates: {
                                                    lat: marker.position.lat(),
                                                    lng: marker.position.lng()
                                                }
                                            },
                                            shape: {
                                                type: "Polygon",
                                                coordinates: [
                                                    [
                                                        {
                                                            lat: marker.position.lat(),
                                                            lng: marker.position.lng()
                                                        },
                                                        {
                                                            lat: marker.position.lat() + polyCoordOffset,
                                                            lng: marker.position.lng()
                                                        },
                                                        {
                                                            lat: marker.position.lat(),
                                                            lng: marker.position.lng() + polyCoordOffset
                                                        }
                                                    ]
                                                ]
                                            },
                                            description: ""
                                        });
                                    }
                                });
                        },
                        true,
                        '');
            locationControl.setObjects('Buildings', buildingList);
            if (defaultLocCode !== '' && defaultLocCode[0] === 'b') {
                var defaultLoc = buildingList
                    .filter(function (b) { return b.ouBuildingCode.toLowerCase() == defaultLocCode.substring(2).toLowerCase() })[0];
                if (defaultLoc != null) {
                    setActiveLocation(defaultLoc, 'building');
                }
            }
        });
}

function refreshParkingList() {
    MapUtils.getParking(baseUrl,
        function (response) {
            parkingList = response.parking;
            MapUtils.Parking.Types = response.parkingTypes;
            parkingList.forEach(function (b) {
                MapUtils.convertBuildingGeoDataToGoogleMapsData(b);
            });
            locationControl.addObjectType(
                        'Parking',
                        function (p) { return p.name + ' (' + p.ouParkingCode + ')'; },
                        function (p) {
                            setActiveLocation(p, 'parking');
                        },
                        function (p, text) {
                            if (text === "") return true;
                            return p.name.toLowerCase().indexOf(text.toLowerCase()) > -1 || p.ouParkingCode.toLowerCase().indexOf(text.toLowerCase()) > -1;
                        },
                        function (successFunc) {
                            var name = window.prompt('Parking Name');
                            if (name === null) return;
                            if (name === "") {
                                alert('Name cannot be empty');
                                return;
                            }
                            var code = '';
                            var getCode = true;
                            while (getCode) {
                                var code = window.prompt("Parking Code");
                                if (code === null) return;
                                var parkingsWithCode = parkingList.filter(function (i) { return i.ouParkingCode === code });
                                getCode = code !== '' && buildingsWithCode.length > 0;
                                if (getCode) {
                                    var response = confirm(parkingsWithCode[0].name + ' already uses the code: ' + code + '\nRetry?');
                                    if (!response) return;
                                }
                            }
                            alert('drag marker to starting spot and click on it to set the building location');
                            var marker = new google.maps.Marker({
                                position: MapUtils.ouCenter,
                                map: map,
                                draggable: true
                            });
                            map.panTo(marker.position);
                            marker.addListener('click',
                                function () {
                                    var polyCoordOffset = 0.001;
                                    var response = confirm('set parking here?');
                                    if (response) {
                                        marker.setMap(null);
                                        successFunc({
                                            name: name,
                                            ouParkingCode: code,
                                            marker: {
                                                type: "Point",
                                                coordinates: {
                                                    lat: marker.position.lat(),
                                                    lng: marker.position.lng()
                                                }
                                            },
                                            spaceCount: 0,
                                            handicappedCount: 0,
                                            shape: {
                                                type: "Polygon",
                                                coordinates: [
                                                    [
                                                        {
                                                            lat: marker.position.lat(),
                                                            lng: marker.position.lng()
                                                        },
                                                        {
                                                            lat: marker.position.lat() + polyCoordOffset,
                                                            lng: marker.position.lng()
                                                        },
                                                        {
                                                            lat: marker.position.lat(),
                                                            lng: marker.position.lng() + polyCoordOffset
                                                        }
                                                    ]
                                                ]
                                            },
                                            description: ""
                                        });
                                    }
                                });
                        },
                        true,
                        '');
            locationControl.setObjects('Parking', parkingList);
            if (defaultLocCode !== '' && defaultLocCode[0] === 'p') {
                var defaultLoc = parkingList
                    .filter(function (p) { return p.ouParkingCode.toLowerCase() == defaultLocCode.substring(2).toLowerCase() })[0];
                if (defaultLoc != null) {
                    setActiveLocation(defaultLoc, 'parking');
                }
            }
        });
}

function onMarkerMove(e) {
    activeBuilding.marker.coordinates = {
        lng: marker.position.lng(),
        lat: marker.position.lat()
    };
}

function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: MapUtils.ouCenter,
        zoom: 17
    });
    deleteMenu = new MapUtils.DeleteMenu();

    map.controls[google.maps.ControlPosition.LEFT_CENTER].push(locationControl.div);

    var centerControlParent = MapUtils.PanCenterControl(map);
    centerControlParent.index = 1;
    centerControlParent.style['padding-right'] = '10px';
    map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(centerControlParent);

    marker = new google.maps.Marker({
        map: null,
        position: map.center,
        draggable: true
    });
    activeShape = new google.maps.Polygon({
        strokeColor: '#FFF',
        strokeWeight: 1,
        fillColor: "#555",
        fillOpacity: .2
    });
    marker.addListener("drag", onMarkerMove);
    refreshBuildingList();
    refreshParkingList();
}