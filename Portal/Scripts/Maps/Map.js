var map;
var buildings = [], parking = [];
var buildingShapes = [], parkingShapes = [];
var omniBox = new MapUtils.OmniBox(), infoPane = new MapUtils.InfoPane();
var marker = new MapUtils.Marker(), shape = new MapUtils.Polygon();
var deleteMenu = new MapUtils.DeleteMenu();
var appRoot = document.getElementById('body-content');
var currentLoc = new MapUtils.PersonMarker();

function initMap() {
    //refreshLocations(function () {
    //});
    map = new google.maps.Map(document.getElementById('map'),
    {
        center: MapUtils.ouCenter,
        zoom: 17,
        styles: [
            {
                featureType: 'poi.business',
                stylers: [{ visibility: 'off' }]
            }
        ],
        mapTypeControl: false
    });
    map.addListener('click', function (event) {
        console.log(event);
        if (event.latLng != undefined) {
            console.log(event.latLng.lng(), event.latLng.lat());
            MapUtils.getLocationsAt(baseUrl, event.latLng.lng(), event.latLng.lat(), function(result) {
                if (result.locs != undefined && result.locs != null) {
                    console.log(result.locs.map(function (l) { return l.name }));
                    if (result.locs.length > 0) {
                        MapUtils.convertBuildingGeoDataToGoogleMapsData(result.locs[0]);
                        setLocation(result.locs[0], false);
                    }
                }
                console.log(result);
            });
        } else {
            console.log('no latLng');
        }
    });
    navigator.geolocation.getCurrentPosition(function (geopos) {
            currentLoc.setMap(map);
            currentLoc.move({
                lng: geopos.coords.longitude,
                lat: geopos.coords.latitude
            });
            window.geowatcher = navigator.geolocation.watchPosition(function(geo) {
                currentLoc.move({
                    lng: geo.coords.longitude,
                    lat: geo.coords.latitude
                });
            }, console.error, {timeout: 10000});
        },
        function() { console.warn('GeoLocation Disabled'); });
    marker.setMap(map);
    shape.setMap(map);
    google.maps.event.addListener(shape, 'rightclick', function(e) {
        if (e.vertex != undefined) {
            console.log('vertex');
            deleteMenu.open(map, this.getPath(), e.vertex);
        } else {
            console.log('not vertex');
        }
    });
    var panCenterControl = MapUtils.PanCenterControl(map);
    panCenterControl.style['padding-right'] = '10px';

    map.controls[google.maps.ControlPosition.RIGHT_BOTTOM].push(panCenterControl);

    appRoot.appendChild(omniBox);
    appRoot.appendChild(infoPane);

    var inputTimer = -1;
    var defaultLoc = null;
    omniBox.setOnInput(function () {
        clearTimeout(inputTimer);
        inputTimer = setTimeout(
        (function (text) {
            return function() {
                omniBox.clearSuggestions();

                MapUtils.searchLocations(baseUrl,
                    text,
                    function (response) {
                        var scores = response.scores;
                        for (var i = 0; i < 5; i++) {
                            if (i === scores.length) break;
                            MapUtils.convertBuildingGeoDataToGoogleMapsData(scores[i]);
                            var row = createSearchRow(scores[i]);
                            row.onclick = function (loc) {
                                return function () {
                                    console.log(loc);
                                    omniBox.clearSuggestions();
                                    getAndSetLocation(loc, false);
                                }
                            }(scores[i]);
                            omniBox.addSuggestion(row);
                        }
                    });
            }
        })(this.value), 500);
    });
    if (defaultLocCode != null && defaultLocCode != "") {
        MapUtils.getLocation(baseUrl, defaultLocCode,
            function (location) {
                MapUtils.convertBuildingGeoDataToGoogleMapsData(location);
                setLocation(location, false);
            });
    }
}

function getAndSetLocation(overlayShape, editable) {
    map.panTo(overlayShape.marker.coordinates);
    MapUtils.getLocation(baseUrl, overlayShape.id,
        function (location) {
            MapUtils.convertBuildingGeoDataToGoogleMapsData(location);
            setLocation(location, editable);
        });
}

function showBounds(left, top, right, bottom, map) {
    return new google.maps.Rectangle({
        strokeColor: '#FF0000',
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillColor: '#FF0000',
        fillOpacity: 0.35,
        map: map,
        bounds: new google.maps.LatLngBounds(
            new google.maps.LatLng(bottom, left),
            new google.maps.LatLng(top, right))
    });
}

function setLocation(location, editable) {
    parkingShapes.forEach(function (s) {
        s.setMap(editable ? null : map);
    });
    buildingShapes.forEach(function (s) {
        s.setMap(editable ? null : map);
    });
    map.panTo(location.marker.coordinates);

    //set marker
    marker.setLocation(location, editable && canEdit);
    shape.setLocation(location, editable && canEdit);

    //set infoPane content and open
    infoPane.content.clear();
    infoPane.content.appendChild(createInfoPaneContent(location, editable && canEdit));

    if (canEdit) {
        var buttonGroup = document.createElement('div');
        buttonGroup.className = 'button-group';
        if (editable) {
            var save = document.createElement('button');
            var cancel = document.createElement('button');
            save.className = 'save-button';
            save.onclick = function() {
                var response = function(result) {
                    showSnackbar(`Successfully update ${location.name}`);
                    setLocation(location, false);
                }

                infoPane.content.clear();
                infoPane.content.appendChild(createLoadingIcon(loadingIcon));
                MapUtils.convertBuildingGoogleMapsDataToGeoData(location);
                MapUtils.postLocation(baseUrl, location, response);
                MapUtils.convertBuildingGeoDataToGoogleMapsData(location);
            }

            cancel.className = 'cancel-button';
            cancel.onclick = function() {
                setLocation(location, false);
            }

            buttonGroup.appendChild(cancel);
            buttonGroup.appendChild(save);
        } else {
            var edit = document.createElement('button');
            edit.onclick = function() {
                setLocation(location, true);
            }
            edit.className = 'edit-button';
            buttonGroup.appendChild(edit);
        }
        infoPane.content.getElementsByClassName('location-info')[0].appendChild(buttonGroup);
    }

    shape.setVisible(editable && canEdit);

    infoPane.open();
}

google.maps.event.addDomListener(window, 'load', initMap);