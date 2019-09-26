const buildingInfoTemplate = function(building, onCloseFunc, canEdit){
    var div = document.createElement('div');
    if (canEdit) {
        div.appendChild(infoField(
            'Name', //label
            building.name, //value
            FieldTypes.Text, //type
            function() {
                building.name = this.innerText;
            }, //onchange
            'building-name', //class
            true //canEdit
        ));
        div.appendChild(infoField(
            'Building Code', //label
            building.ouBuildingCode, //value
            FieldTypes.Text, //type
            function() {
                building.ouBuildingCode = this.innerText;
            }, //onchange
            'building-code', //class
            true //canEdit
        ));
    } else {
        div.appendChild(infoField(
            'Name',
            building.name + ' (' + building.ouBuildingCode + ')',
            FieldTypes.Text,
            function () { },
            'building-name',
            false
        ));
    }
    div.appendChild(infoField(
        'Description',
        building.description,
        FieldTypes.Text,
        function() {
            building.description = this.innerText;
        },
        'building-desc',
        canEdit
    ));
    return div;
};

const parkingInfoTemplate = function(parking, onCloseFunc, canEdit, parkingTypes) {
    var div = document.createElement('div');
    if (canEdit) {
        div.appendChild(
            infoField(
                'Name',
                parking.name,
                FieldTypes.Text,
                function() {
                    parking.name = this.innerText;
                },
                'parking-name',
                true
            ));
        div.appendChild(infoField(
            'Building Code',
            parking.ouParkingCode,
            FieldTypes.Text,
            function() {
                parking.ouParkingCode = this.innerText;
            },
            'parking-code',
            true
        ));
    } else {
        div.appendChild(
            infoField(
                'Name',
                parking.name + ' (' + parking.ouParkingCode + ')',
                FieldTypes.Text,
                function() {},
                'parking-name',
                false
            ));
    }
    div.appendChild(
        infoField(
            'Spaces',
            parking.spaceCount,
            FieldTypes.Text,
            function() {
                parking.spaceCount = this.innerText;
            },
            'parking-spacecount',
            canEdit
        ));
    div.appendChild(
        infoField(
            'Handicap Spaces',
            parking.handicappedCount,
            FieldTypes.Text,
            function() {
                parking.handicappedCount = this.innerText;
            },
            'parking-handicapcount',
            canEdit
        ));
    div.appendChild(infoField(
        'ParkingType',
        parking.parkingTypes.join(','),
        FieldTypes.Select,
        function(selected) {
            parking.parkingTypes = selected;
        },
        'parking-parking_types',
        canEdit,
        {
            isMulti: true,
            options: parkingTypes
        }
    ));
    div.appendChild(
        infoField(
            'Description',
            parking.description,
            FieldTypes.Text,
            function() {
                parking.description = this.innerText;
            },
            'parking-desc',
            canEdit
        ));
    return div;
}

//const markerCoordTemplate = function(x,y,setText,setFunc) {
//    var temp = document.createElement('div');
//    temp.innerHTML = `<div class="marker_coord_info_window">
//        <p>${x}, ${y}</p><input type="button" value="${setText}"/>
//    </div>`;
//    var result = temp.firstChild;
//    result.getElementsByTagName('input')[0].onclick = setFunc;
//    return result;
//}

const FieldTypes = {
    Text: 'text',
    Date: 'date',
    Range: 'range',
    Select: 'list',
    Radio: 'radio'
}

const infoField = function (label, value, type, onchange, className, editable, options) {
    var groupElem, labelElem, controlElem;

    groupElem = document.createElement('div');
    groupElem.className = 'form-group ' + className;

    labelElem = document.createElement('label');
    labelElem.innerText = label;
    groupElem.appendChild(labelElem);

    switch (type) {
        case FieldTypes.Text:
            controlElem = document.createElement('div');
            controlElem.innerText = value;
            if (editable) {
                controlElem.setAttribute('contentEditable', 'true');
                controlElem.oninput = onchange;
            }
            break;
        case FieldTypes.Date:
            throw 'Not Implemented';
            break;
        case FieldTypes.Range:
            throw 'Not Implemented';
            break;
        case FieldTypes.Select:
            if (editable) {
                controlElem = document.createElement('select');
                controlElem.value = value;
                if (options !== undefined && options !== null) {
                    var selected = value.split(',');
                    if (options.isMulti !== undefined && options.isMulti) controlElem.setAttribute('multiple', 'true');
                    if (options.options !== undefined) {
                        for (var i = 0; i < options.options.length; i++) {
                            var option = document.createElement('option');
                            option.value = options.options[i].value || options.options.label || options.options[i];
                            option.label = options.options[i].label || options.options.value || options.options[i];
                            for (var j = 0; j < selected.length; j++) {
                                if (selected[j] === option.value) option.setAttribute('selected', 'true');
                            }

                            controlElem.appendChild(option);
                        }
                    }
                }
                controlElem.onselect = function() {
                    var result = [];
                    var options = this && this.options;
                    var opt;

                    for (var i = 0, iLen = options.length; i < iLen; i++) {
                        opt = options[i];

                        if (opt.selected) {
                            result.push(opt.value);
                        }
                    }
                    onchange(result);
                }
            } else {
                controlElem = document.createElement('div');
                controlElem.innerText = value;
            }
            break;
        case FieldTypes.Radio:
            throw 'Not Implemented';
            break;
    }
    controlElem.className = 'form-control';
    groupElem.appendChild(controlElem);

    return groupElem;
}

const iconButton = function(title, className, iconClassName, onClick) {
    var button = document.createElement('div');
    button.className = 'btn btn-default ' + className;
    button.title = title;
    if (iconClassName != null) {
        var span = document.createElement('span');
        span.className = iconClassName;
        button.appendChild(span);
    }
    button.onclick = onClick;
    return button;
}

const panel = function() {
    var temp = document.createElement('div');
    temp.innerHTML = `<div class='panel panel-default'><div class='panel-heading'></div><div class='panel-body'></div></div>`;
    temp.firstChild.getElementsByTagName('p').onchange = onchange;
    return result = temp.firstChild;
}

const createOmniBox = function() {
    var temp = document.createElement('div');
    temp.innerHTML =
        `<div class='search-control'>
            <div class='omnibox'>
                <input class='omnibox-search-input' type='text' placeholder='Search OU Map'/>
                <button class ='omnibox-search-button glyphicon glyphicon-search' tooltip='Search'></button>
                <button class ='omnibox-directions-button glyphicon glyphicon-road'></button>
            </div>
            <div class ='omnibox-suggestions'>
                <ul>
                </ul>
            </div>
        </div>`;
    return temp.firstChild;
}

const createPersonDiv = function() {
    var result = document.createElement('div');
    result.className = 'person-marker';
    return result;
};

const createLoadingIcon = function(src) {
    var container = document.createElement('div');
    container.className = 'loading-icon-container';

    var img = document.createElement('img');

    container.appendChild(img);
    img.src = src;

    return container;
}

const createSearchRow = function(searchResult) {
    var li = document.createElement('li');
    li.innerHTML =
        `<span class='icon glyphicon'></span><span class='row-label'>${searchResult.title}</span>`;
    switch (searchResult.locationType) {
    case MapUtils.Types.Building:
        li.firstChild.classList.add('glyphicon-home');
        li.className = 'omnibox-search-row';
        break;
    case MapUtils.Types.Parking:
        li.firstChild.classList.add('glyphicon-road');
        li.className = 'omnibox-search-row';
        break;
    default:
    }
    return li;
};

const createInfoPane = function() {
    var temp = document.createElement('div');
    temp.innerHTML =
        `<div class='info-pane'>
            <div class ='content'>
                <div style='position: absolute; width: 100%; background-color: lightblue; bottom: 50%; text-align: center; font-size: 30px;'>
                    No Location Selected
                </div>
            </div>
            <div class ='toggle-button'></div>
        </div>`;
    return temp.firstChild;
}

const createAsteroids = function() {
    var temp = document.createElement('div');
    temp.innerHTML =
        `<div class='asteroids-window'><div class="asteroids-game"></div><div class="asteroids-close">x</div></div>`;
    return temp.firstChild;
}

const createInfoPaneContent = function(location, editable) {
    var temp = document.createElement('div');
    var errorImgUrl = baseUrl + 'Content/Images/NA.jpg';
    if (!editable) {
        temp.innerHTML =
        `<div class='location-img'>
            <img src='${location.imgUrl}' onerror='if (this.src != "${errorImgUrl}") this.src = "${errorImgUrl}";'>
         </div>
         <div class='location-info'>
            <div class='name'>${location.name} (${location.ouCode}) </div>
            <div class='description'>${location.description}</div>
         </div>`;
    } else {
        temp.innerHTML =
            `<div class='location-img'>
            <button class='upload-photo'></button>
         </div>
         <div class='location-info'>
            <div contenteditable='true' class ='name'>${location.name}</div>
            <div contenteditable='true' class ='code'>${location.ouCode}</div>
            <div contenteditable='true' class='description'>${location.description}</div>
         </div>`;
        temp.getElementsByClassName('name')[0].oninput = function () { location.name = this.innerText; };
        temp.getElementsByClassName('code')[0].oninput = function () { location.ouCode = this.innerText; };
        temp.getElementsByClassName('description')[0].oninput = function () { location.description = this.innerText; };
    }
    
    return temp;
}