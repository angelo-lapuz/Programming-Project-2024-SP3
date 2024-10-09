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
var peaksData = JSON.parse(peaksData); // used to store the peaks data - pulled from Viewbag in index.cshtml
var userRoutes = userRoutes; // used to store the user's saved routes - pulled from Viewbag in index.cshtml
var userPeaks = JSON.parse(userPeaks);


var markerLayer = L.layerGroup().addTo(map); // used to store the markers for the peaks
var drawnItems = new L.FeatureGroup(); // used to store the drawn routes
map.addLayer(drawnItems);

// ddefining buttons, boxes
var drawRouteBtn = document.getElementById('draw-route');
var undoBtn = document.getElementById('undo');
var finishBtn = document.getElementById('finish');
var clearBtn = document.getElementById('clear');
var routeBox = document.getElementById('route-box');

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
    map = L.map('peakmap').setView([-41.5, 146.5], 7);
    var currentPeak = currentPeak;
    var coords = currentPeak.Coords.split(',');
    var lat = parseFloat(coords[0]);
    var lng = parseFloat(coords[1]);

    if (!isNaN(lat) && !isNaN(lng)) {
        map.setView([lat, lng], 11);
        L.circle([lat, lng], { color: 'red', radius: 500 }).addTo(map);
        L.marker([lat, lng]).bindPopup(`<b>${currentPeak.Name}</b><br><a href="/Peak/Index/${currentPeak.PeakID}" onclick="window.location='/Peak/Index/${currentPeak.PeakID}'; return false;">View Peak</a>`).addTo(markerLayer);
    } else {
        console.log('Invalid coordinates for current peak: ', currentPeak.Name);
    }
}
// checking if the user has routes and if they are logged in
checkRoutes();
checkUser();

// defining the color and thickness of the polyline - used when drawing routes
var polylineDrawer = new L.Draw.Polyline(map, {
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
        document.querySelector('.route-container').style.display = 'none';

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
    console.log("clearing");
    drawnItems.clearLayers();
    polylines = [];
    points = [];
    currentPolyline = null;
    markers = []; // used to show peaks on the map
    elevations = []; // used to store elevation data for a route

}
// saves the user route to the database - stored as JSON objects as string in user class to avoid another m:m table
saveBtn.addEventListener('click', function () {

    // checks that there is an existing polyline to save
    if (polylines.length > 0) {

        /// getting the coordinates of the route to be saved
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
            // getting the response from the /Planner/SaveRoute endpoint
            .then(response => {

                // checking if the response is ok (could be Unauthorized, Forbidden, etc.)
                if (!response.ok) {
                    // if not ok, throw an error with the response message
                    return response.json().then(data => {
                        throw new Error(data.error || 'Failed to save route');
                    });
                }// Parse the response as JSON
                return response.json();
            })
            .then(data => {
                console.log("Parsed data:", data);
                if (data.ok) {
                    alert("Route saved successfully");

                    // Add the newly saved route to the userRoutes array
                    userRoutes.push(JSON.stringify(routeData)); // Add the route data to userRoutes

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
    });

    deleteRouteBtn.addEventListener('click', function () {
        deleteRoute(index, box);
    });

    // addding the box to the route-box div
    document.getElementById('route-box').appendChild(box);
}

// used to ddraw routes on the map - takes in a JSON object of coordinates
function drawRoute(coordsJson) {
    clear();
    if (drawnItems) {
        drawnItems.clearLayers();
    }

    // will try to draw new coordinates on the map
    try {

        // splitting in the coordinates into an array
        const routes = JSON.parse(coordsJson);
        console.log(routes);
        // ror every route split into further array
        routes.forEach(routeSet => {
            var latlngs = [];

            // assign lat and long coorodiantes pulled from the object
            routeSet.forEach(coord => {
                var lat = parseFloat(coord.Lat);
                var lng = parseFloat(coord.Lng);

                // checking if coordiantes are valid
                if (!isNaN(lat) && !isNaN(lng)) {
                    latlngs.push([lat, lng]);
                }
            });

            // if there are any coordinates, draw the polyline on the map
            if (latlngs.length > 0) {
                var polyline = L.polyline(latlngs, { color: '#FF0000', weight: 5 }).addTo(drawnItems);

                // zoom to the position of the route on the map with the fitBounds built in method
                map.fitBounds(polyline.getBounds());
            }
        });
        // /rare incase of corrupted data
    } catch (error) {
        console.error("Error parsing the route JSON:", error);
    }
}



//////                                   END OF USER BASED ROUTE DRAWING FUNCTIONALITY                       //////

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

    // for every point in the points array, calculate the distance between the points
    for (var i = 0; i < points.length - 1; i++) {
        var segmentDistance = points[i].distanceTo(points[i + 1]);
        totalDistance += segmentDistance;

        // numPoints is the number of points to interpolate between the two points
        // used to make the graph smoother - however each point will slow the process down significantly 
        var numPoints = 4;
        var interpolatedPoints = interpolatePoints(points[i], points[i + 1], numPoints);

        // for each interpolated point, get the elevation and add it to the elevations array
        for (var point of interpolatedPoints) {
            var elevation = await getElevation(point.lat, point.lng);
            // distance is returned in meters, so divide by 1000 to get kilometers
            var currentDistance = (totalDistance * (elevations.length / interpolatedPoints.length)) / 1000;
            elevations.push({
                distance: currentDistance,
                elevation: elevation
            });
        }
    }
    // drawing the elevation chart
    drawElevationChart(elevations);
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

// gets the elevation from open-elevation API - takes in the latitude and longitude of a given point as parameters
async function getElevation(lat, lon) {
    const url = `https://api.open-elevation.com/api/v1/lookup?locations=${lat},${lon}`;

    try {
        /// call the api
        const response = await fetch(url);

        // parse response
        const data = await response.json();

        // if there is data and the results array is not empty, return the elevation
        if (data && data.results && data.results.length > 0) {
            return data.results[0].elevation;
            // not every lat, long has an elevation - can return null - especially if travesing lakes / water
        } else {
            console.error('Elevation data not available for this point.');
            return null;
        }
    } catch (error) {
        console.error('Error fetching elevation data:', error);
        return null;
    }
}

// function draws the elevation chart from an array of elevations
function drawElevationChart(elevations) {

    // get the elevation chart canvas element
    var ctx = document.getElementById('elevationChart').getContext('2d');
    // if there is an existing elevation chart, destroy it
    if (window.elevationChart && typeof window.elevationChart.destroy === 'function') {
        window.elevationChart.destroy();
    }
    // create a new elevation chart
    window.elevationChart = new Chart(ctx, {
        // type can be line, bar, ect
        type: 'line',
        data: {
            // getting the lables of the elevation - in this instance distance and elevation
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
            Response: true,
            maintainAspectRatio: false,
            scales: {
                // x,y axis labelling
                x: {
                    title: {
                        display: true,
                        text: 'Distance (kilometers)'
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: 'Elevation (meters)'
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

            // will only change color if user is logged in and userPeaks is not null
            if (user && userPeaks) {
                let completedPeak = userPeaks.some(function (userPeak) {
                    return userPeak.PeakID === peak.PeakID;
                });

                if (completedPeak) {
                    // set color to green
                    filterValue = 'hue-rotate(260deg)'; 
                } else {
                    // set color to reddish
                    filterValue = 'hue-rotate(-220deg)';
                }
            }
 
            // Create the marker and add it to the map
            let marker = L.marker([lat, lng]).bindPopup(`
                <b>${peak.Name}</b><br> Elevation: ${peak.Elevation}m <br> Difficulty: ${peak.Difficulty} <br>
                <a href="/Peak/Index/${peak.PeakID}" onclick="window.location='/Peak/Index/${peak.PeakID}'; return false;">View Peak</a>`)
                .addTo(markerLayer);

            // Apply inline style to change color
            marker._icon.style.filter = filterValue;
        }
    });
}



/// SECTION BOXES
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
        sectionPolygons[sectionName] = polygon;
        addHoverEffect(box, polygon);
        addClickEvent(box, polygon, sectionName);
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
        return L.polygon(polygonCoords, { color: "#0080fE", fillOpacity: 0 })
            .bindPopup("Section: " + sectionName)
            .addTo(map);
    } else {
        return null;
    }
}

// adds a hover effect to the box and polygon
function addHoverEffect(box, polygon) {

    // when the mouse hovers over box effect added here
    box.addEventListener('mouseenter', () => polygon.setStyle({ fillOpacity: 0.3 }));
    box.addEventListener('mouseleave', () => {
        if (selectedSection !== polygon) polygon.setStyle({ fillOpacity: 0 });
    });
}


function addClickEvent(box, polygon, sectionName) {

    box.addEventListener('click', () => {
        selectSection(polygon);
        var sectionPeaks = peaksData.filter(peak => peak.Section === sectionName);
        showSectionPeaks(sectionPeaks);
    });

    polygon.on('click', () => {
        selectSection(polygon);
        var sectionPeaks = peaksData.filter(peak => peak.Section === sectionName);
        showSectionPeaks(sectionPeaks);
    });
}

function selectSection(polygon) {
    if (selectedSection) selectedSection.setStyle({ fillOpacity: 0 });
    selectedSection = polygon;
    polygon.setStyle({ fillOpacity: 0.5 });
    map.fitBounds(polygon.getBounds());
}


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

function applyFilters() {
    var nameFilter = document.getElementById('search-name').value.toLowerCase();
    var difficultyFilter = document.getElementById('filter-difficulty').value;
    var elevationFilter = document.getElementById('filter-elevation').value;

    markerLayer.clearLayers();

    var filteredPeaks = peaksData.filter(function (peak) {
        var matchesName = nameFilter === "" || peak.Name.toLowerCase().includes(nameFilter);
        var matchesDifficulty = difficultyFilter === "" || peak.Difficulty === difficultyFilter;
        var matchesElevation = elevationFilter === "" || peak.Elevation >= parseInt(elevationFilter);
        return matchesName && matchesDifficulty && matchesElevation;
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
        sectionCell.textContent = peak.Section;

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