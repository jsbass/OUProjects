MapUtils = (function () {
    var convertLocationGeoDataToGoogleMapsData = function(building) {
        building.marker.coordinates = {
            lng: building.marker.coordinates[0],
            lat: building.marker.coordinates[1]
        }
        building.shape.coordinates = building.shape.coordinates.map(function(coordSet) {
            return coordSet.map(function(coord, j, arr) {
                return {
                    lng: coord[0],
                    lat: coord[1]
                }
            });
        });
    };

    var convertLocationGoogleMapsDataToGeoData = function (building) {
            building.marker.coordinates = [building.marker.coordinates.lng, building.marker.coordinates.lat];
        building.shape.coordinates = building.shape.coordinates.map(function(coordSet) {
            return coordSet.map(function(coord) {
                return [coord.lng, coord.lat];
            });
        });
    }

    var locationPath = "api/map/locations";

    var OuCenter = {
        lat: 35.207621,
        lng: -97.444744
    }

    var getLocationAt = function(baseUrl, lng, lat, callback) {
        var query = PromiseHttp.get(`${baseUrl}${locationPath}?lng=${lng}&lat=${lat}`, { useCache: true });
        query.promise.then(function(response) {
            if (response === "") response = "null";
            callback(JSON.parse(response));
        }, console.error);
    }

    var getLocation = function (baseUrl, id, callback) {
        console.log(`Making request to: ${baseUrl}${locationPath}/${id}`);
        var query = PromiseHttp.get(`${baseUrl}${locationPath}/${id}`, { useCache: false });
        query.promise.then(function (response) {
                if (response === "") response = "null";
                callback(JSON.parse(response));
            }, console.error);
    }

    var postLocation = function (baseUrl, location, callback) {
        console.log(`Making request to: ${baseUrl}${locationPath}`);
        var query = PromiseHttp.post(`${baseUrl}${locationPath}`, location, { useCache: false });
        query.promise.then(function (response) {
                if (response === "") response = "null";
                callback(JSON.parse(response));
            }, console.error);
    }

    var searchQuery = { cancel: function () { console.warn('searchPromise Not Set');}};
    var searchLocations = function (baseUrl, searchString, callback) {
        searchQuery.cancel();
        var path = `${baseUrl}${locationPath}?search=${searchString}`;
        console.log(`Making request to: ${path}`);
        searchQuery = PromiseHttp.get(path, { useCache: true });
        searchQuery.promise.then(function (response) {
                if (response === "") response = "null";
                callback(JSON.parse(response));
            }, console.error);
    }

    return {
        Parking: {
            Types: []
        },
        getLocationsAt: getLocationAt,
        getLocation: getLocation,
        postLocation: postLocation,
        searchLocations: searchLocations,
        ouCenter: OuCenter,
        convertBuildingGoogleMapsDataToGeoData: convertLocationGoogleMapsDataToGeoData,
        convertBuildingGeoDataToGoogleMapsData: convertLocationGeoDataToGoogleMapsData
    }
})();

MapUtils.PanCenterControl = function(map) {
    var locateMeButton = iconButton('Locate Me',
        '',
        'glyphicon glyphicon-map-marker',
        function () {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function(pos) {
                    map.panTo({
                        lng: pos.coords.longitude,
                        lat: pos.coords.latitude
                    });
                }, console.error,
                    {
                        timeout: 1000,
                        maximumAge: Infinity
                    });
            } else {
                console.log('location services not supported for this browser');
            }
        });
    var centerOnOuButton = iconButton('Center On OU',
        '',
        'icon-OU-Logo',
        function() {
            map.panTo(MapUtils.ouCenter);
        });
    var parent = document.createElement('div');
    parent.className = 'btn-group-vertical';

    parent.appendChild(locateMeButton);
    parent.appendChild(centerOnOuButton);

    return parent;
}

MapUtils.PersonMarker = (function () {
    function PersonMarker() {
        this.div = createPersonDiv();
    }

    PersonMarker.prototype = new google.maps.OverlayView();
    PersonMarker.prototype.onAdd = function() {
        this.getPanes().markerLayer.appendChild(this.div);
    }

    PersonMarker.prototype.onRemove = function() {
        this.div.parentNode.removeChild(this.div);

        this.set('position');
    }

    PersonMarker.prototype.draw = function() {
        var position = this.get('position');
        var projection = this.getProjection();

        if (!position || !projection) {
            return;
        }

        var point = projection.fromLatLngToDivPixel(new google.maps.LatLng(position.lat, position.lng));
        this.div.style.top = point.y + 'px';
        this.div.style.left = point.x + 'px';
    }

    PersonMarker.prototype.move = function(position) {
        this.set('position', position);
        this.draw();
    }

    return PersonMarker;
})();

MapUtils.DeleteMenu = (function() {
    function DeleteMenu() {
        this.div_ = document.createElement('div');
        this.div_.className = 'map-utils_delete-menu';
        this.div_.innerHTML = 'Delete';

        var menu = this;
        google.maps.event.addDomListener(this.div_,
            'click',
            function () {
                menu.removeVertex();
            });
    }

    DeleteMenu.prototype = new google.maps.OverlayView();
    DeleteMenu.prototype.onAdd = function () {
        var deleteMenu = this;
        var map = this.getMap();
        this.getPanes().floatPane.appendChild(this.div_);

        this._divListener = google.maps.event.addDomListener(map.getDiv(), 'mousedown', function(e) {
            if (e.target != deleteMenu.div_) {
                deleteMenu.close();
            }
        }, true);
    };
    DeleteMenu.prototype.onRemove = function() {
        google.maps.event.removeListener(this._divListener);
        this.div_.parentNode.removeChild(this.div_);

        // clean up
        this.set('position');
        this.set('path');
        this.set('vertex');
    };

    DeleteMenu.prototype.close = function() {
        this.setMap(null);
    };

    DeleteMenu.prototype.draw = function() {
        var position = this.get('position');
        var projection = this.getProjection();

        if (!position || !projection) {
            return;
        }

        var point = projection.fromLatLngToDivPixel(position);
        this.div_.style.top = point.y + 'px';
        this.div_.style.left = point.x + 'px';
    };

    DeleteMenu.prototype.open = function(map, path, vertex) {
        this.set('position', path.getAt(vertex));
        this.set('path', path);
        this.set('vertex', vertex);
        this.setMap(map);
        this.draw();
    };

    DeleteMenu.prototype.removeVertex = function() {
        var path = this.get('path');
        var vertex = this.get('vertex');

        if (!path || vertex == undefined) {
            this.close();
            return;
        }

        path.removeAt(vertex);
        this.close();
    }

    return DeleteMenu;
})();

(function () {
    function ObjectType(name, labelFunc, onSelectFunc, filterFunc, createFunc, canEdit, containerClassName) {
        var bind = this;

        this.name = name;
        var objects = [];
        this.createFunc = createFunc;
        this.labelFunc = labelFunc;
        this.onSelectFunc = onSelectFunc;
        this.filterFunc = filterFunc;
        this.canEdit = canEdit;
        this.containerClassName = containerClassName;
        this.div = document.createElement('div');

        var filterElem = document.createElement('input');
        filterElem.type = 'text';
        filterElem.placeholder = "Search for " + name;

        var selectElem = document.createElement('select');
        selectElem.size = 10;
        selectElem.style["overflow-y"] = 'auto';

        var filter = function() {
            while (selectElem.hasChildNodes()) {
                selectElem.removeChild(selectElem.firstChild);
            }
            objects.forEach(function(item, i) {
                if (bind.filterFunc(item, filterElem.value)) {
                    var option = document.createElement('option');
                    option.label = bind.labelFunc(item);
                    option.value = i;
                    selectElem.appendChild(option);
                }
            });
        };

        filterElem.onchange = filter;

        selectElem.onchange = function() {
            if (this.value) {
                console.log('selected');
                console.log(objects[this.value]);
                onSelectFunc(objects[this.value]);
            }
        };

        var select = function (object) {
            var i = objects.indexOf(object);
            selectElem.selectedIndex = i;
            selectElem.dispatchEvent(new Event("change"));
        }

        this.select = function (object) {
            filterElem.value = "";
            filter();
            select(object);
        }

        this.deselect = function() {
            selectElem.selectedIndex = -1;
        }

        this.set = function(newObjects) {
            objects = newObjects;

            filterElem.value = "";
            filter();

            select(null);
        }

        this.add = function(object) {
            objects[objects.length] = object;
            this.select(null);
            filter();
        }

        this.remove = function(object) {
            objects.splice(bind.objects.indexOf(object));
            this.select(null);
            filter();
        }

        this.create = function() {
            createFunc(function(returnedObj) {
                if (returnedObj === null || returnedObj === undefined) return;
                bind.add(returnedObj);
                bind.select(returnedObj);
            });
        }

        this.div.appendChild(filterElem);
        this.div.appendChild(selectElem);
    }

    function constructor() {
        // ReSharper disable once InconsistentNaming

        this.div = document.createElement('div');
        this.div.className = 'map-utils_location-control';

        var infoWindow = document.createElement('div');
        infoWindow.className = 'location-control-pane info-window';

        var selectWindow = document.createElement('div');
        selectWindow.className = 'location-control-pane select-window';

        this.div.appendChild(selectWindow);
        this.div.appendChild(infoWindow);

        this.setInfoDiv = function(div) {
            while (infoWindow.hasChildNodes()) {
                infoWindow.removeChild(infoWindow.firstChild);
            }
            infoWindow.appendChild(div);
        }
        this.closeInfoDiv = function() {
            //TODO
        }
        this.openInfoDiv = function() {
            //TODO
        }
        var objectTypes = {};
        var panelGroups = {};

        this.setObjects = function(name, newObjects) {
            var objectType = objectTypes[name];
            objectType.set(newObjects);
        }

        this.addObject = function(name, object) {
            var objectType = objectTypes[name];
            objectType.add(object);
        }

        this.removeObject = function(name, object) {
            var objectType = objectTypes[name];
            objectType.remove(object);
        }

        this.addObjectType = function(name, labelFunc, onSelectFunc, filterFunc, createFunc, canEdit, containerClassName) {
            var objectType = new ObjectType(name, labelFunc, onSelectFunc, filterFunc, createFunc, canEdit, containerClassName);

            var panelGroup = panel();
            panelGroup.className += containerClassName;
            var panelHeading = panelGroup.getElementsByClassName('panel-heading')[0];
            var panelBody = panelGroup.getElementsByClassName('panel-body')[0];
            

            panelHeading.innerText = name;
            panelHeading.onclick = function () {
                if (panelBody.classList.contains('panel-body-hidden')) {
                    panelBody.classList.remove('panel-body-hidden');
                    panelBody.classList.add('panel-body-visible');
                } else {
                    panelBody.classList.add('panel-body-hidden');
                    panelBody.classList.remove('panel-body-visible');
                }
            }

            panelBody.appendChild(objectType.div);
            if (canEdit) {
                var createButton = document.createElement('div');
                createButton.className = 'create-btn btn btn-default';
                createButton.innerText = 'Add New';

                createButton.onclick = objectType.create;

                panelBody.appendChild(createButton);
            }

            panelGroups[name] = panelGroup;
            objectTypes[name] = objectType;
            selectWindow.appendChild(panelGroups[name]);
        }

        this.removeObjectType = function(name) {
            selectWindow.removeChild(panelGroups[name]);
            objectTypes[name] = null;
        }

        this.select = function(name, object)
        {
            objectTypes[name].select(object);
        }

        this.deselect = function(name) {
            objectTypes[name].deselect();
        }
    }
    MapUtils.LocationControl = constructor;
})();

(function() {
    var OmniBox = function(options) {
        var box = createOmniBox();
        var searchInput = box.getElementsByClassName('omnibox-search-input')[0];
        var searchSuggestions = box.getElementsByClassName('omnibox-suggestions')[0];
        var ul = searchSuggestions.getElementsByTagName('ul')[0];

        box.search = function(text) {
            searchInput.value = text;
            searchInput.dispatchEvent(new Event('change'));
        }

        box.setOnInput = function(oninput) {
            searchInput.oninput = oninput;
        }

        box.addSuggestion = function (rowElem) {
            searchSuggestions.classList.remove('hidden');
            ul.appendChild(rowElem);
        }

        box.clearSuggestions = function () {
            searchSuggestions.classList.add('hidden');
            while (ul.hasChildNodes()) {
                ul.removeChild(ul.firstChild);
            }
        }

        box.clearSuggestions();
        return box;
    }

    var InfoPane = function (options) {
        var box = createInfoPane();
        box.content = box.getElementsByClassName('content')[0];
        var toggleButton = box.getElementsByClassName('toggle-button')[0];

        toggleButton.onclick = function () {
            console.log('toggle');
            box.toggle();
        }

        box.close = function() {
            box.removeAttribute('visible');
        }

        box.open = function() {
            box.setAttribute('visible', '');
        }

        box.toggle = function() {
            if (box.getAttribute('visible') === null) {
                box.setAttribute('visible', '');
            } else {
                box.removeAttribute('visible');
            }
        }

        box.content.clear = function() {
            while (box.content.hasChildNodes()) {
                box.content.removeChild(box.content.firstChild);
            }
        }

        return box;
    }

    MapUtils.OmniBox = OmniBox;
    MapUtils.InfoPane = InfoPane;
})();

MapUtils.Types = {
    Building: 1,
    Parking: 2,
    None: 0,
};

(function() {
    var Marker = function () {
        var _map = null;
        var marker = new google.maps.Marker({});
        var setMap = marker.setMap;

        marker.setMap = function(map) {
            _map = map;
            setMap.call(marker, map);
        }

        marker.setLocation = function(location, canEdit) {
            if (location === undefined || location === null) {
                marker.setMap(null);
            } else {
                marker.setMap(_map);
                marker.setPosition(location.marker.coordinates);
                marker.setDraggable(canEdit);
                marker.addListener('drag', function () {
                    location.marker.coordinates = {
                        lng: marker.position.lng(),
                        lat: marker.position.lat()
                    };
                });
            }
        }

        return marker;
    }

    var Polygon = function () {
        var _map = null;
        var shape = new google.maps.Polygon({});
        var setMap = shape.setMap;

        shape.setMap = function (map) {
            _map = map;
            setMap.call(shape, map);
        }

        var updateLocationShape = function(location, paths) {
            location.shape.coordinates = paths.b.map(function(i) {
                return i.b.map(function(j) { return { lng: j.lng(), lat: j.lat() }; });
            });
        }

        shape.setLocation = function(location, canEdit) {
            if (location === undefined || location === null) {
                shape.setMap(null);
            } else {
                shape.setMap(_map);
                shape.setPaths(location.shape.coordinates);
                shape.setEditable(canEdit);
                if (canEdit) {
                    google.maps.event.addListener(shape.getPath(),
                        'set_at',
                        function(e) {
                            updateLocationShape(location, shape.getPaths());
                        });
                    google.maps.event.addListener(shape.getPath(),
                        'insert_at',
                        function(e) {
                            updateLocationShape(location, shape.getPaths());
                        });
                    google.maps.event.addListener(shape.getPath(),
                        'remove_at',
                        function(e) {
                            updateLocationShape(location, shape.getPaths());
                        });
                }
            }
        }

        return shape;
    }

    MapUtils.Polygon = Polygon;
    MapUtils.Marker = Marker;
})();