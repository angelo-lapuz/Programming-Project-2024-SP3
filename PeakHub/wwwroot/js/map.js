// Initialize map and layers
var map = L.map('map').setView([-41.5, 146.5], 7);

var tasmaniaLayer = L.tileLayer("https://services.thelist.tas.gov.au/arcgis/rest/services/Basemaps/TasmapRaster/MapServer/WMTS/tile/1.0.0/Basemaps_TasmapRaster/default/default028mm/{z}/{y}/{x}.png", {
    maxZoom: 16,
    minZoom: 6
}).addTo(map);

var markers = []; // used to show peaks on the map
var points = []; // used to store points for drawing a route
var polylines = []; //used when drawing on the map
var currentPolyline = null; // used when drawing on the map - editing the current polyline
var elevations = []; // used to store elevation data for a route
var sectionPolygons = {}; // used to store polygons for each section
var selectedSection = null; // used to store the currently selected section

try {
    var peaksData = JSON.parse(peaksData); // used to store the peaks data - pulled from Viewbag in index.cshtml
    var userRoutes = userRoutes; // used to store the user's saved routes - pulled from Viewbag in index.cshtml
    var userPeaks = JSON.parse(userPeaks); // used to store all the peaks a given user has completed
} catch (Exception) {

}

var markerLayer = L.layerGroup().addTo(map); // used to store the markers for the peaks
var drawnItems = new L.FeatureGroup(); // used to store the drawn routes
map.addLayer(drawnItems);

// defining buttons, boxes
var drawRouteBtn = document.getElementById('draw-route');
var undoBtn = document.getElementById('undo');
var finishBtn = document.getElementById('finish');
var clearBtn = document.getElementById('clear');
var routeBox = document.getElementById('route-box');



// Reference marker and slider elements
var placeMarkerBtn = document.getElementById('place-marker-btn');
var radiusSlider = document.getElementById('radius-slider');
//var radiusSliderContainer = document.getElementById('radius-slider-container');
var radiusValueDisplay = document.getElementById('radius-value');
var routeLengthSlider = document.getElementById('filter-length-slider');
var routeLengthDisplay = document.getElementById('filter-length-value');
var infoBtn = document.getElementById('info-button')

// used to determine if the user has placed a marker, or is in the process of placing one to prevent issues with other
// functions
var userMarker = null;
var radiusCircle = null;
var isPlacingMarker = false;

// event listener for the radius slider - adjusts depending on user input
radiusSlider.addEventListener('input', function () {
    // ddisplaying the value and drawing any peaks within range if a marker has been placed
    radiusValueDisplay.textContent = radiusSlider.value + ' km';
    if (userMarker) {
        var latlng = userMarker.getLatLng();
        filterPeaksWithinRadius(latlng, radiusSlider.value);
    }
});

//even listener for the route lengthh slider - used to determine the minimum length of peak routes
routeLengthSlider.addEventListener('input', function () {
    routeLengthDisplay.textContent = routeLengthSlider.value + ' km';
    applyFilters();
});


// Event listener for the "Place Marker" button - used when making the radius for 'pekas in range' feature
placeMarkerBtn.addEventListener('click', function () {

    if (isPlacingMarker === false) {
        placeMarkerBtn.classList.toggle('click');
    }
    // Enable marker placement mode
    isPlacingMarker = true;
    removePopups();

    // changing the cursor for the polygons to allow for more intuative placing of marker
    Object.values(sectionPolygons).forEach((polygon) => {
        polygon.on('mouseover', function () {
            polygon.getElement().style.cursor = 'crosshair';

        })
        polygon.on('mouseout', function () {
            polygon.getElement().style.cursor = '';
        });
    });
});

// Listen for map clicks to place a marker
map.on('click', function (e) {

    // if the user is placing a marker 
    if (isPlacingMarker) {

        placeMarkerBtn.classList.toggle('click');
        var latlng = e.latlng;
        
        // Remove any existing marker and circle
        if (userMarker) {
            map.removeLayer(userMarker);
        }
        if (radiusCircle) {
            map.removeLayer(radiusCircle);
        }

        // Place a new marker different from others so user can easily see 
        var Icon = L.icon({
            iconUrl: 'img/manIcon.png',
            iconSize: [45, 45]
        });

        // adding the icon to theh map
        userMarker = L.marker(latlng,{ icon: Icon}).addTo(map);

        // Show the slider for radius adjustment
        radiusSlider.style.visibility = 'visible';
        document.getElementById('radius-value').style.visibility = 'visible';

        // Initial filter for peaks within the default radius
        filterPeaksWithinRadius(latlng, radiusSlider.value);

        // Disable marker placement mode and remove popups
        isPlacingMarker = false;
        removePopups();

        // restting the polygon mouse hover effects
        Object.values(sectionPolygons).forEach((polygon) => {

            // resetting the mouseover effect
            polygon.on('mouseover', function () {
                polygon.getElement().style.cursor = '';

            })

            // resetting the mouseout effect
            polygon.on('mouseout', function () {
                polygon.getElement().style.cursor = '';
            });
        });

        // Reset cursor to default
        map.getContainer().style.cursor = '';
    }
});


// Function to filter peaks within the given radius (in kilometers)
function filterPeaksWithinRadius(latlng, radius) {
    var radiusInMeters = radius * 1000;  // Convert radius to meters

    // Clear previous radius circle and markers
    if (radiusCircle) {
        map.removeLayer(radiusCircle);
    }
    markerLayer.clearLayers();

    // Draw a circle around the marker representing the search radius
    radiusCircle = L.circle(latlng, { radius: radiusInMeters, color: 'blue', fillOpacity: 0.2, interactive: false }).addTo(map);

    // Filter peaks based on their distance from the marker
    var filteredPeaks = peaksData.filter(function (peak) {
        var peakCoords = peak.Coords.split(',');
        var peakLatLng = L.latLng(parseFloat(peakCoords[0]), parseFloat(peakCoords[1]));
        var distance = latlng.distanceTo(peakLatLng);  // Calculate distance between marker and peak

        return distance <= radiusInMeters;  // Only include peaks within the radius
    });

    // Show the filtered peaks on the map
    /*showSectionPeaks(filteredPeaks);*/
    applyFilters();
}

// removes all popups from polygons - used when placing marker
function removePopups() {

    // for every sectionpolygon
    Object.keys(sectionPolygons).forEach(sectionName => {

        var polygon = sectionPolygons[sectionName];
        // removing or adding the popup back depending on whether the user is placing a marker
        if (!isPlacingMarker) {

            polygon.bindPopup("Section: " + sectionName);
        } else {
            polygon.unbindPopup();
        }
    })
}

// Function to show section peaks
function showSectionPeaks(peaks) {
    // Clear any existing markers
    markerLayer.clearLayers();

    // For each peak, create a marker and add it to the map
    peaks.forEach(function (peak) {

        // parse the coordinates
        var coords = peak.Coords.split(',');
        var lat = parseFloat(coords[0]);
        var lng = parseFloat(coords[1]);

        /// checking that the values are valid 
        if (!isNaN(lat) && !isNaN(lng)) {

            // setting default color as blue
            var filterValue = 'hue-rotate(0deg)';
            var data = "";

            // checking that the user is logged in and that they have completed this current peak
            if (user && userPeaks) {
                let completedPeak = userPeaks.some(function (userPeak) {
                    return userPeak.PeakID === peak.PeakID;
                });
                // changing color to red or green if the user has / has not completed a peak
                if (completedPeak) {
                    // must use hue-rotate as leaflet dodes not support rgb / hex values
                    filterValue = 'hue-rotate(260deg)';
                    data = "You have completed this Peak";
                } else {
                    filterValue = 'hue-rotate(-220deg)';
                    data = "You have not completed this Peak";
                }
            }
            // adding the marker to the map - with the popup
            let marker = L.marker([lat, lng]).bindPopup(`
                <b>${peak.Name}</b><br> Elevation: ${peak.Elevation}m <br> Difficulty: ${peak.Difficulty} <br>
                ${data} <br>
                <a href="/Peak/Index/${peak.PeakID}" onclick="window.location='/Peak/Index/${peak.PeakID}'; return false;">View Peak</a>
            `).addTo(markerLayer);

            // Apply inline style to change marker color
            marker._icon.style.filter = filterValue;
        }
    });
}


// sections used for this map
var sectionsData = {
    "The North West": [{ "lat": -41.80919639152056, "lng": 145.62927246093753 }, { "lat": -41.70982942509965, "lng": 145.98770141601565 }, { "lat": -41.17451935556444, "lng": 146.56585693359378 }, { "lat": -41.169350717601425, "lng": 146.2046813964844 }, { "lat": -40.9591597721349, "lng": 145.63751220703128 }, { "lat": -40.820045086716505, "lng": 145.24200439453128 }, { "lat": -40.84706035607122, "lng": 145.09918212890628 }, { "lat": -40.70354580345142, "lng": 144.72564697265628 }, { "lat": -40.91766362458114, "lng": 144.71191406250003 }, { "lat": -41.050359519318874, "lng": 144.68170166015628 }, { "lat": -41.38711263243965, "lng": 144.79980468750003 }, { "lat": -41.720080555287105, "lng": 144.99206542968753 }, { "lat": -41.80970819375622, "lng": 145.0394439697266 }],
    "The West": [{ "lat": -41.813290694914215, "lng": 145.04287719726565 }, { "lat": -41.81021999190291, "lng": 145.62892913818362 }, { "lat": -41.79051281160145, "lng": 145.69759368896487 }, { "lat": -42.35346979349065, "lng": 145.71990966796878 }, { "lat": -42.24631026600387, "lng": 145.2042388916016 }, { "lat": -42.09007006868398, "lng": 145.28320312500003 }],
    "The Mid West": [{ "lat": -41.789744876718984, "lng": 145.69888114929202 }, { "lat": -42.35296235855689, "lng": 145.7205963134766 }, { "lat": -42.314893062816964, "lng": 146.2760925292969 }, { "lat": -42.20614200929955, "lng": 146.34887695312503 }, { "lat": -42.00644814707989, "lng": 145.97053527832034 }],
    "Pelion and Saint Clair": [{ "lat": -41.68829673005492, "lng": 146.0117340087891 }, { "lat": -41.74365194975237, "lng": 146.20262145996097 }, { "lat": -41.85779934552825, "lng": 146.13395690917972 }, { "lat": -42.019712622928495, "lng": 146.1655426025391 }, { "lat": -42.126238096064625, "lng": 146.1003112792969 }, { "lat": -42.06917490812321, "lng": 145.99937438964847 }, { "lat": -42.00785125130587, "lng": 145.97482681274417 }, { "lat": -41.789744876718984, "lng": 145.69828033447268 }],
    "The Central Plateau": [{ "lat": -41.88029768140287, "lng": 146.11885070800784 }, { "lat": -41.74365194975237, "lng": 146.20262145996097 }, { "lat": -41.6872711837914, "lng": 146.01104736328128 }, { "lat": -41.14867208811922, "lng": 146.58782958984378 }, { "lat": -41.07365737372555, "lng": 146.71897888183597 }, { "lat": -41.38711263243965, "lng": 147.07122802734378 }, { "lat": -41.979910896912344, "lng": 147.52716064453128 }, { "lat": -42.31387756616186, "lng": 146.28021240234378 }, { "lat": -42.20512475970615, "lng": 146.3475036621094 }, { "lat": -42.089050952191634, "lng": 146.12365722656253 }, { "lat": -42.0161416900189, "lng": 146.16725921630862 }],
    "The North East": [{ "lat": -41.075210270566636, "lng": 146.71691894531253 }, { "lat": -40.96538194577488, "lng": 147.32391357421878 }, { "lat": -41.017210578228436, "lng": 147.41180419921878 }, { "lat": -40.75766014997032, "lng": 147.98034667968753 }, { "lat": -40.99441099943952, "lng": 148.31542968750003 }, { "lat": -42.067645723795266, "lng": 148.23028564453128 }, { "lat": -41.983994270935625, "lng": 147.52441406250003 }, { "lat": -41.36031866306708, "lng": 147.05749511718753 }],
    "The Gordon": [{ "lat": -42.272228095985675, "lng": 146.44226074218753 }, { "lat": -42.676377880719635, "lng": 146.35711669921878 }, { "lat": -42.70060440808084, "lng": 145.3807067871094 }, { "lat": -42.245801966774025, "lng": 145.2049255371094 }, { "lat": -42.35346979349065, "lng": 145.71990966796878 }, { "lat": -42.316924006970325, "lng": 146.27471923828128 }],
    "The South East": [{ "lat": -42.90212460791275, "lng": 147.2483825683594 }, { "lat": -42.805980593526705, "lng": 146.63726806640628 }, { "lat": -42.675873060924566, "lng": 146.3578033447266 }, { "lat": -42.27121191118535, "lng": 146.4408874511719 }, { "lat": -41.98093176496801, "lng": 147.5285339355469 }, { "lat": -42.06866518410683, "lng": 148.23303222656253 }, { "lat": -42.86388628078584, "lng": 147.79357910156253 }],
    "The South": [{ "lat": -43.17613987737832, "lng": 146.47659301757815 }, { "lat": -42.72179486825571, "lng": 146.4566802978516 }, { "lat": -42.80849936032272, "lng": 146.63726806640628 }, { "lat": -42.90413649491734, "lng": 147.24151611328128 }, { "lat": -42.89709460316191, "lng": 147.3252868652344 }, { "lat": -43.05985799709891, "lng": 147.32803344726565 }, { "lat": -43.631105439358, "lng": 146.85699462890628 }, { "lat": -43.52166770320007, "lng": 146.50543212890628 }, { "lat": -43.271956049072365, "lng": 146.47693634033206 }, { "lat": -43.22894496172244, "lng": 146.44844055175784 }],
    "The South West": [{ "lat": -43.52465500687186, "lng": 146.49993896484378 }, { "lat": -43.53062917044242, "lng": 146.05773925781253 }, { "lat": -43.209179690393555, "lng": 145.98358154296878 }, { "lat": -43.28720268480439, "lng": 145.86547851562503 }, { "lat": -42.70060440808084, "lng": 145.37796020507815 }, { "lat": -42.67738750800697, "lng": 146.34887695312503 }, { "lat": -42.7278479565962, "lng": 146.4573669433594 }]
};

// used on individual peak page to center the map on the peak andd circle it
if (window.location.href.includes("Peak/Index/")) {

    // setting current peak and getting its coordinates
    var currentPeak = currentPeak;
    var coords = currentPeak.Coords.split(',');
    var lat = parseFloat(coords[0]);
    var lng = parseFloat(coords[1]);

    // setting the map to focus on the current peak
    if (!isNaN(lat) && !isNaN(lng)) {
        map.setView([lat, lng], 11);
        L.circle([lat, lng], { color: 'red', radius: 500 }).addTo(map);
        L.marker([lat, lng]).bindPopup(`<b>${currentPeak.Name}</b><br><a href="/Peak/Index/${currentPeak.PeakID}" onclick="window.location='/Peak/Index/${currentPeak.PeakID}'; return false;">View Peak</a>`).addTo(markerLayer);
    } else {
        console.log('Invalid coordinates for current peak: ', currentPeak.Name);
    }

    // removing the route drawing functionality
   
    routeBox.disabled = true;
    
    // removing the route drawing functionality

    document.querySelector('.filter-container').style.display = 'none';
    document.querySelector('.sections-container').style.display = 'none';
    document.querySelector('.routeList-container').style.display = 'none';
    document.querySelector('.info-button').style.display = 'none';
    document.querySelector('.plannerBtns').style.display = 'none';
    document.querySelector('.route-toggle').style.display = 'none';


    // ddrawing the peak/s for this particular peak
    drawRoute(currentPeak.Routes);
    let formattedCoords = [];
    let routeCoords = currentPeak.Routes;

    //  Parse routeCoords if it's a JSON string
    if (typeof routeCoords === 'string') {
        try {
            routeCoords = JSON.parse(routeCoords);
        } catch (error) {
            console.error("Error parsing routeCoords:", error);
        }
    }

    //  Check if routeCoords is an array of objects with lat/lng properties
    if (Array.isArray(routeCoords) && routeCoords.every(point => point.lat !== undefined && point.lng !== undefined)) {
        // Map each point to ensure it has lat and lng as numbers
        formattedCoords = routeCoords.map(point => ({
            lat: parseFloat(point.lat || point.Lat),
            lng: parseFloat(point.lng || point.Lng)
        }));
    } else {
        console.error("Unexpected structure for routeCoords after parsing:", routeCoords);
    }


    // Ensure the formatted coordinates are valid before proceeding
    if (formattedCoords.length > 0) {
        // Convert formatted coordinates to Leaflet LatLng objects
        const latLngCoords = formattedCoords.map(coord => L.latLng(coord.lat, coord.lng));

        // Draw the route on the map
        drawRoute([latLngCoords]);

        // Calculate distance and elevation for the route
        calculateDistanceAndElevation(latLngCoords);
    } else {
        console.error("No valid coordinates to process.");
    }


} else {

    // check the users routes and disabling the route box if they are not logged in
    checkRoutes();
    checkUser();
}
  
// defining the color and thickness of the polyline - used when drawing routes
var polylineDrawer = new L.Draw.Polyline(map,{
    shapeOptions: {
        color: '#FF0000',
        weight: 5
    }
});

// event listener for when the user clicks on the map - checks if the polyline drawer is enabled and adds the point to the points array
map.on('click', function (event) {
    /// if drawing route is enabled, add the point to the points array 
    if (polylineDrawer._enabled) {

        // /event.latlng is the point where the user clicked
        points.push(event.latlng);

        // drawing the polyline on the map
        drawPolyline();
    }
});

// disables the route box if user is not logged in
function checkUser() {
    if (!user) {
        document.getElementById('route-toggle-checkbox').disabled = 'true';
    }
    
}

// disable the routes box if userRoutes is not an array
function checkRoutes() {
    if (Array.isArray(userRoutes) && userRoutes.length > 0) {

        // if array is not empty, create a route box for each route
        userRoutes.forEach((route, index) => {
            createRouteBox(`Route ${index + 1}`, route, index);
        });
    } else {

        // disable the route box if user is not logged in
        if (!user) {
            document.querySelector('.routeList-container').style.display = 'none';
            document.getElementById('route-box').style.display = 'none';
        }
    }
}

// if the draw route button is clicked, clear the drawn items and enable the polyline drawer
// enabling only 1 route to be drawn at a time
drawRouteBtn.addEventListener('click', function () {

    // removing previously drawn routes from the map
    drawnItems.clearLayers();

    // clearing points of previous route
    points = [];

    // turning on the polyline drawer to draw a new route
    polylineDrawer.enable();
});

// if the finish button is clicked, disable the polyline drawer and draw the route on the map
finishBtn.addEventListener('click', async function () {

    // turn off the polyline drawer
    polylineDrawer.disable();

    if (points.length > 0) {
        // draw the polyline on the map - get the result to add to the polylines array
        var polyline = drawPolyline();

        // add the polyline to the polylines array
        polylines.push(polyline);

        // set the current polyline to null, ensures that next route does not contain last point of previous route
        currentPolyline = null;

        // calculate the distance and elevation of the route
        await calculateDistanceAndElevation(points);

        // reset points to empty array - ensures that the next route is properly cleared
        points = [];
    } else {
        console.log("No points were drawn, cannot finish route");
    }
});

// if the undo button is clicked, remove the last point from the points array and redraw the polyline
undoBtn.addEventListener('click', function () {
    if (points.length > 0) {
        points.pop();
        console.log("Point removed, remaining points:", points);

        // ensures thhere is a current polyline to remove - removes it from the map
        if (currentPolyline) {
            map.removeLayer(currentPolyline);
        }

        // re-draw polyLine with the updated points
        drawPolyline();
    } else {
        console.log("No points to undo");
    }
});

// clears all currently drawn routes - will not delete a route that has been saved but will remove it from view
clearBtn.addEventListener('click', function () {
    clear();
});
// if there are any drawn items, clear them, and reset the polylines and points arrays
function clear() {
    drawnItems.clearLayers();
    polylines = [];
    points = [];
    currentPolyline = null;
    markers = []; 
    elevations.length = 0;
    elevations = []; 
}
// saves the user route to the database - stored as JSON objects as string in user class to avoid another m:m table
saveBtn.addEventListener('click', function () {

    // checks that there is an existing polyline to save
    if (polylines.length > 0) {

        // getting the coordinates of the route to be saved
        var routeData = polylines.map(polyline => polyline.getLatLngs());

        // Send the route data to the Planner Controller (not webAPI)
        fetch('/Planner/SaveRoute', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            // as route data is an array of arrays, it needs to be stringified back into JSON 
            body: JSON.stringify({ route: routeData })
        })
            .then(response => {

                // checking if the response is ok (could be Unauthorized, Forbidden, etc.)
                if (!response.ok) {
                    // if not ok, throw an error with the response message
                    return response.json().then(data => {
                        throw new Error(data.error || 'Failed to save route');
                    });
                }
                // Parse the response as JSON
                return response.json();
            })
            .then(data => {
                console.log("Parsed data:", data);
                if (data.ok) {
                    alert("Route saved successfully");

                    // formatting the route so it can be displayed instantly 
                    const formattedRouteData = JSON.stringify(routeData.map(route =>
                        route.map(point => ({
                            Lat: point.lat,
                            Lng: point.lng
                        }))
                    ));

                    // Add the newly saved formatted route to the userRoutes array
                    userRoutes.push(formattedRouteData);

                    // Clear all existing route boxes/spans
                    clearRouteBoxes();

                    // Re-create route boxes with updated userRoutes
                    checkRoutes();
                } else {
                    alert("Failed to save route");
                }
            })
            .catch(error => {
                console.error('Error saving route:', error);
                // Check for the specific error message
                if (error.message === "You have reached the maximum number of saved routes.") {
                    alert("You can only save a maximum of 3 routes.");
                } else {
                    alert("An error occurred while saving the route");
                }
            });
    } else {
        alert("No route to save");
    }
    clear();
});

// called when deleting a route
function deleteRoute(index, routeBox) {
    // remove the route from the userRoutes array
    userRoutes.splice(index, 1);

    // delete the route box from the route-box div
    routeBox.remove();

    // clear the drawn items from the map
    drawnItems.clearLayers()

    // call the Planner delteRoute method passing the index of the route to delete
    fetch(`/Planner/DeleteRoute/${index}`, {
        method: 'POST',
    })
        // get the response of the call
        .then(response => response.json())
        .then(data => {
            // ifno errors in back display delete confirmation
            if (data.ok) {
                alert("Route deleted")
            } else {
                alert("Failed to delete Route");
            }
        })
        .catch(error => {
            console.log("error deleting route", error);
        });
    // Clear all existing route boxes/spans
    clearRouteBoxes();

    // Re-create route boxes with updated userRoutes
    checkRoutes();
}



// deletes all the routeBoxes
function clearRouteBoxes() {
    // Clear all the existing route boxes
    var routeBoxContainer = document.getElementById('route-box');
    routeBoxContainer.innerHTML = '';
}


// creates the RouteBoxes
function createRouteBox(routeName, routeCoords, index) {

    // creating new bbox div element
    var box = document.createElement('div');
    box.className = 'route-box';

    // used to click on the route name to draw the route
    var routeSpan = document.createElement('button');
    routeSpan.textContent = routeName;
    routeSpan.className = 'route-name';
    // adding to the box
    box.appendChild(routeSpan);

    // delete button to delete the route
    var deleteRouteBtn = document.createElement('button');
    deleteRouteBtn.innerHTML = '<i class="fa-solid fa-trash-can"></i>';
    deleteRouteBtn.className = 'delete-route-btn';
    box.appendChild(deleteRouteBtn);

    // adding event listeners to the routeSpan and deleteRouteBtn
    routeSpan.addEventListener('click', function () {
        drawRoute(routeCoords)

        // we will need to format the coordinates passed in here - namely because we are passing in a string of JSON objects
        // and we need to convert it back into an array so it can be used in the elevation method 
        let formattedCoords = [];

        if (typeof routeCoords === 'string') {
            try {
                routeCoords = JSON.parse(routeCoords);
            } catch (error) {
                return;
            }
        }

        // After parsing, process routeCoords as array of arrays
        if (Array.isArray(routeCoords) && Array.isArray(routeCoords[0])) {
            formattedCoords = routeCoords[0].map(point => ({
                lat: point.Lat,
                lng: point.Lng
            }));
        } else {
            console.error("Unexpected structure for routeCoords after parsing:", routeCoords);
        }

        // ensurign the formatted coordinates are valid 
        if (formattedCoords.length > 0) {

            // mapping the previously formatted coords
            const latLngCoords = formattedCoords.map(coord => L.latLng(coord.lat, coord.lng));

            // drawing the route and displaying the coordinates 
            drawRoute([latLngCoords]); 
            calculateDistanceAndElevation(latLngCoords);
        }
    });

    // delete route button declaration
    deleteRouteBtn.addEventListener('click', function () {
        deleteRoute(index, box);
    });

    // addding the box to the route-box div
    document.getElementById('route-box').appendChild(box);
}

// used to ddraw routes on the map - takes in a JSON object of coordinates
function drawRoute(routeData) {

    // clear the existing routes and clearing any items if they exist
    clear();
    if (drawnItems) {
        drawnItems.clearLayers();
    }

    try {
        // Parse routeData if it's a JSON string
        let routes = typeof routeData === 'string' ? JSON.parse(routeData) : routeData;

        // Check if the format is an array of single routes or multiple routes
        if (!Array.isArray(routes[0])) {
            // If it's a single route, wrap it in an array for consistency
            routes = [routes];
        }

        // Iterate through each route
        routes.forEach(routeSet => {
            let latlngs = routeSet.map(coord => ({
                lat: parseFloat(coord.lat || coord.Lat),
                lng: parseFloat(coord.lng || coord.Lng)
            })).filter(coord => !isNaN(coord.lat) && !isNaN(coord.lng));

            // Draw the polyline if we have valid coordinates
            if (latlngs.length > 0) {
                const polyline = L.polyline(latlngs, { color: '#FF0000', weight: 5 }).addTo(drawnItems);
                map.fitBounds(polyline.getBounds()); 
            }
        });
    } catch (error) {
        console.error("Error parsing route data:", error);
    }
}

// draws the routes / lines on the map
function drawPolyline() {

    // checking that there are valid points to draw the polyline
    if (points.length > 0) {
        if (currentPolyline) {
            // removing the currentPolyLine from the map
            map.removeLayer(currentPolyline);
        }
        // adding color, weight and adding to drawnitems layer (seperate Layer on map)
        currentPolyline = L.polyline(points, { color: '#FF0000', weight: 5 }).addTo(drawnItems);

        // returning the currentPolyline
        return currentPolyline;
    } else {
        console.log("No points to draw the polyline");
    }
}

// calculates the total distance and elevation over the course of the route
async function calculateDistanceAndElevation(points) {
    var totalDistance = 0;
    elevations = [];
    let elevationRequests = [];
    let distances = [];

    // Loop over each segment and calculate the direct distance between points
    for (var i = 0; i < points.length - 1; i++) {
        var segmentDistance = points[i].distanceTo(points[i + 1]);
        totalDistance += segmentDistance;

        // Number of interpolated points based on the length of the route
        let numPoints = (points.length >= 6) ? 2 : (points.length >= 4) ? 3 : 4;
        var interpolatedPoints = interpolatePoints(points[i], points[i + 1], numPoints);

        // Push each interpolated point's distance from the start, not scaled by request length
        for (var point of interpolatedPoints) {
            distances.push(totalDistance / 1000); // Add distance in kilometers
            elevationRequests.push({ lat: point.lat, lng: point.lng });
        }
    }

    try {
        // Fetch batch elevations and draw the chart
        let batchedElevations = await getElevationsBatch(elevationRequests);
        elevations = batchedElevations.map((elevation, idx) => ({
            distance: distances[idx],
            elevation: elevation
        }));
        drawElevationChart(elevations);
    } catch (error) {
        console.error('Error calculating distance and elevation:', error);
    }
}


// interpolates points between two latlng points
function interpolatePoints(startLatLng, endLatLng, numPoints) {
    var points = [];

    // foreach point between the start and end latlng, calculate the lat and long and add to the points array
    for (var i = 0; i <= numPoints; i++) {
        var lat = startLatLng.lat + (endLatLng.lat - startLatLng.lat) * (i / numPoints);
        var lng = startLatLng.lng + (endLatLng.lng - startLatLng.lng) * (i / numPoints);

        points.push({ lat: lat, lng: lng });
    }
    return points;
}

// gets the elevation from open-elevation in a batch (all data at once)
async function getElevationsBatch(locations) {
    const chunkSize = 10; // Adjust based on API limits, such as 10 coordinates per request
    let elevations = [];

    // Split locations into smaller chunks
    for (let i = 0; i < locations.length; i += chunkSize) {
        const locationChunk = locations.slice(i, i + chunkSize);
        const locationParams = locationChunk.map(({ lat, lng }) => `${lat},${lng}`).join('|');
        const url = `https://api.open-elevation.com/api/v1/lookup?locations=${locationParams}`;

        try {
            const response = await fetch(url);
            const data = await response.json();

            if (data && data.results) {
                elevations = elevations.concat(data.results.map(result => result.elevation));
            } else {
                console.error('No elevation data available for chunk:', i / chunkSize + 1);
                elevations = elevations.concat(Array(locationChunk.length).fill(null)); 
            }
        } catch (error) {
            console.error('Error fetching elevation data for chunk:', i / chunkSize + 1, error);
            elevations = elevations.concat(Array(locationChunk.length).fill(null)); 
        }
    }

    return elevations;
}

// function draws the elevation chart from an array of elevations
function drawElevationChart(elevations) {
    // Check if the URL includes "Peak/Index/"
    const isPeakIndexPage = window.location.href.includes("Peak/Index/");

    // Get the elevation chart canvas element
    var ctx = document.getElementById('elevationChart').getContext('2d');

    // If there is an existing elevation chart, destroy it
    if (window.elevationChart && typeof window.elevationChart.destroy === 'function') {
        window.elevationChart.destroy();
    }

    // Define the color based on the URL check
    const textColor = isPeakIndexPage ? 'white' : 'black'; 

    // Create a new elevation chart
    window.elevationChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: elevations.map(function (e) { return e.distance.toFixed(2) + ' km'; }),
            datasets: [{
                label: 'Elevation (m)',
                data: elevations.map(function (e) { return e.elevation; }),
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1,
                fill: false
            }]
        },
        options: {
            // makes the graph fill the canvas
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    labels: {
                        color: textColor 
                    }
                },
                tooltip: {
                    titleColor: textColor, 
                    bodyColor: textColor, 
                    footerColor: textColor 
                }
            },
            scales: {
                // x,y axis labelling
                x: {
                    title: {
                        display: true,
                        text: 'Distance (kilometers)',
                        color: textColor 
                    },
                    ticks: {
                        color: textColor
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Elevation (meters)',
                        color: textColor 
                    },
                    ticks: {
                        color: textColor 
                    }
                }
            }
        }
    });
}



function showSectionPeaks(peaks) {

    // Clear any existing markers
    markerLayer.clearLayers();

    // For every peak given, create a marker and add it to the marker layer
    peaks.forEach(function (peak) {
        // Get peak coords
        var coords = peak.Coords.split(',');
        var lat = parseFloat(coords[0]);
        var lng = parseFloat(coords[1]);

        // Check if the coordinates are valid
        if (!isNaN(lat) && !isNaN(lng)) {
            
            let filterValue = 'hue-rotate(0deg)'; 
            let data = "";
            // will only change color if user is logged in and userPeaks is not null
            if (user && userPeaks) {
                
                let completedPeak = userPeaks.some(function (userPeak) {
                    return userPeak.PeakID === peak.PeakID;
                });

                if (completedPeak) {
                    // set color to green
                    filterValue = 'hue-rotate(260deg)';
                    data = "You have completed this Peak";
                } else {
                    // set color to reddish
                    filterValue = 'hue-rotate(-220deg)';
                     data = "you have not completed this Peak"
                }
            } else {
           
            }
 
            // Create the marker and add it to the map
            let marker = L.marker([lat, lng]).bindPopup(`
                <b>${peak.Name}</b><br> Elevation: ${peak.Elevation}m <br> Difficulty: ${peak.Difficulty} <br>
                ${data} <br>
                <a href="/Peak/Index/${peak.PeakID}" onclick="window.location='/Peak/Index/${peak.PeakID}'; return false;">View Peak</a>`)
                .addTo(markerLayer);

            // Apply inline style to change color
            marker._icon.style.filter = filterValue;
        }
    });
}

var sectionBoxContainer = document.getElementById('section-boxes');

// shows the sectionData withing the sectionBoxContainer
Object.keys(sectionsData).forEach(function (sectionName) {
    createSectionBox(sectionName, sectionsData[sectionName]);
});

// creates the section box
function createSectionBox(sectionName, sectionCoords) {

    // create a box element for the section
    var box = createBoxElement(sectionName);

    // create a polygon for the section
    var polygon = createPolygon(sectionCoords, sectionName);

    // if the polygon is valid, add the hover effect and click event to the box
    if (polygon) {
        addHoverEffect(box, polygon);
        addClickEvent(box, polygon, sectionName);
        sectionPolygons[sectionName] = polygon;
    }
}

// creates a box element for the section
function createBoxElement(sectionName) {
    // create a div element with the class 'section-box' and the text content of the section name
    var box = document.createElement('div');
    box.className = 'section-box';
    box.textContent = sectionName;
    sectionBoxContainer.appendChild(box);
    return box;
}

// creates a polygon for the section 
function createPolygon(sectionCoords, sectionName) {

    // map the sectionCoords to an array of lat, lng pairs
    var polygonCoords = sectionCoords.map(coord => {
        var lat = parseFloat(coord.lat);
        var lng = parseFloat(coord.lng);

        // if coords are valid return the lat, lng pair, else return null
        if (!isNaN(lat) && !isNaN(lng)) {
            return [lat, lng];
        } else {
            return null;

        }
        // filter out any null values
    }).filter(coord => coord !== null);

    // if there are any valid coords, create a polygon with the coords and add it to the map
    if (polygonCoords.length > 0) {
        return L.polygon(polygonCoords, { color: "#0080fE", fillOpacity: 0 }).bindPopup("Section: " + sectionName).addTo(map);
    } else {
        return null;
    }
}

// adds a hover effect to the box and polygon
function addHoverEffect(box, polygon) {

    // when the mouse hovers over box effect added here
    box.addEventListener('mouseenter', () => polygon.setStyle({ fillOpacity: 0.3 }));
    box.addEventListener('mouseleave', () => polygon.setStyle({ fillOpacity: 0 }));
   
}

// click event for box, polygon and section
function addClickEvent(box, polygon, sectionName) {

    // event listener for box - shows peaks beloning to the box clickedo n
    box.addEventListener('click', () => {
        selectSection(polygon);
        var sectionPeaks = peaksData.filter(peak => peak.Region === sectionName);
        showSectionPeaks(sectionPeaks);
    });

    // event listener for the section on the map - shows peaks belonging to the section clicked on
    polygon.on('click', () => {
        selectSection(polygon);
        var sectionPeaks = peaksData.filter(peak => peak.Region === sectionName);
        showSectionPeaks(sectionPeaks);
    });
}

// used when a box / section is selected - used to set the current selected section anddto 'pan' to that section
function selectSection(polygon) {
    if (selectedSection) selectedSection.setStyle({ fillOpacity: 0 });
    selectedSection = polygon;
    map.fitBounds(polygon.getBounds());
}

// apply filters event listener - calls appy filters function when the button is clicked
document.getElementById('apply-filters').addEventListener('click', function () {
    applyFilters();
});

// auto filters when difficulty slider changes
document.getElementById('filter-difficultySlider').addEventListener('input', function () {

    document.getElementById('filter-difficulty').value = (difficultyMap[this.value]) === 'All' ? '' : difficultyMap[this.value].charAt(0);

    applyFilters();

});

// as above but with elevation slider
document.getElementById('filter-elevation').addEventListener('input', function () {

    document.getElementById('filter-elevation').value = this.value;

    applyFilters();

});

/// apply filters function filters peaks based off user input
function applyFilters() {

    // filter values that can hold values set by the user
    var nameFilter = document.getElementById('search-name').value.toLowerCase();
    var difficultyFilter = document.getElementById('filter-difficulty').value;
    var elevationFilter = document.getElementById('filter-elevation').value;
    var radius = parseFloat(radiusSlider.value) * 1000;
    var minRouteLength = parseFloat(routeLengthSlider.value) * 1000;

    // clearing anythign exisiting on the map
    markerLayer.clearLayers();

    var filteredPeaks = peaksData.filter(function (peak) {

        // Extract latitude and longitude of each peak
        var [lat, lng] = peak.Coords.split(',').map(coord => parseFloat(coord));
        var peakLatLng = L.latLng(lat, lng);

        // Check if the peak matches the name filter
        var withinRadius = !userMarker || userMarker.getLatLng().distanceTo(peakLatLng) <= radius;
        var matchesName = nameFilter === "" || peak.Name.toLowerCase().includes(nameFilter);
        var matchesDifficulty = difficultyFilter === "" || peak.Difficulty === difficultyFilter;
        var matchesElevation = elevationFilter === "" || peak.Elevation >= parseInt(elevationFilter);

        // Route length check, considering cases where Routes could be null
        var matchesRouteLength = true;
        if (minRouteLength > 0 && peak.Routes) {

            // 
            let routeData = peak.Routes;
            let routes = typeof routeData === 'string' ? JSON.parse(routeData) : routeData;
            routes = Array.isArray(routes[0]) ? routes : [routes];

            var totalLength = 0;
            routes.forEach(route => {
                for (var i = 0; i < route.length - 1; i++) {
                    var a = L.latLng(route[i].lat, route[i].lng);
                    var b = L.latLng(route[i + 1].lat, route[i + 1].lng);
                    totalLength += a.distanceTo(b);
                }
            });

            //setting the 
            matchesRouteLength = totalLength >= minRouteLength;
        } else if (minRouteLength > 0 && !peak.Routes) {
            // Exclude peaks without routes if a minimum route length is set
            matchesRouteLength = false; 
        }

        return withinRadius && matchesName && matchesDifficulty && matchesElevation && matchesRouteLength;
    });

    showSectionPeaks(filteredPeaks);


    const tableBody = document.querySelector('.filter-results-table').getElementsByTagName('tbody')[0];
    tableBody.innerHTML = '';

    filteredPeaks.forEach(function (peak) {
        var coords = peak.Coords.split(',');
        var lat = parseFloat(coords[0]);
        var lng = parseFloat(coords[1]);

        var newRow = tableBody.insertRow();
        var nameCell = newRow.insertCell(0);
        var sectionCell = newRow.insertCell(1);

        nameCell.textContent = peak.Name;
        sectionCell.textContent = peak.Region;

        newRow.addEventListener('click', function () {

            // Stolen from Max's code above. HEH
            map.setView([lat, lng], 12);
            // MAX are we able to make the pop up appear when it zooms into the peak. The one when a user clicks on the pin.
            markerLayer.eachLayer(function (marker) {
                var markerLatLng = marker.getLatLng();
                if (markerLatLng.lat === lat && markerLatLng.lng === lng) {
                    marker.openPopup();
                }
            });

        });

    });

}