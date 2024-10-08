var map = L.map('map').setView([-41.5, 146.5], 7);

var tasmaniaLayer = L.tileLayer("https://services.thelist.tas.gov.au/arcgis/rest/services/Basemaps/TasmapRaster/MapServer/WMTS/tile/1.0.0/Basemaps_TasmapRaster/default/default028mm/{z}/{y}/{x}.png", {
    maxZoom: 16,
    minZoom: 6
}).addTo(map);

var markers = [];
var points = [];
var polylines = [];
var currentPolyline = null;
var elevations = [];
var sectionPolygons = {};
var selectedSection = null;
var peaksData = JSON.parse(peaksData);

var markerLayer = L.layerGroup().addTo(map);
var drawnItems = new L.FeatureGroup();
map.addLayer(drawnItems);

var drawRouteBtn = document.getElementById('draw-route');
var undoBtn = document.getElementById('undo');
var finishBtn = document.getElementById('finish');
var clearBtn = document.getElementById('clear');


var polylineDrawer = new L.Draw.Polyline(map, {
    shapeOptions: {
        color: '#FF0000',
        weight: 5
    }
});

map.on('click', function (event) {
    if (polylineDrawer._enabled) {
        points.push(event.latlng);
        console.log("Added point:", event.latlng);

        drawPolyline();
    }
});

drawRouteBtn.addEventListener('click', function () {
    console.log("Drawing route started");
    points = [];
    polylineDrawer.enable();
});

finishBtn.addEventListener('click', async function () {
    console.log("Drawing route finished");
    polylineDrawer.disable();

    if (points.length > 0) {
        console.log("Finished drawing with points:", points);
        var polyline = drawPolyline();

        polylines.push(polyline);
        currentPolyline = null;
        await calculateDistanceAndElevation(points);
        points = [];
    } else {
        console.log("No points were drawn, cannot finish route");
    }
});
undoBtn.addEventListener('click', function () {
    if (points.length > 0) {
        points.pop();
        console.log("Point removed, remaining points:", points);

        if (currentPolyline) {
            map.removeLayer(currentPolyline);
        }

        drawPolyline();
    } else {
        console.log("No points to undo");
    }
});

clearBtn.addEventListener('click', function () {
    console.log("Clearing all routes");
    if (drawnItems) {
        drawnItems.clearLayers();
        polylines = [];
        points = [];
        currentPolyline = null;
    }
});

function drawPolyline() {
    if (points.length > 0) {
        if (currentPolyline) {
            map.removeLayer(currentPolyline);
        }
        currentPolyline = L.polyline(points, { color: '#FF0000', weight: 5 }).addTo(drawnItems);
        return currentPolyline;
    } else {
        console.log("No points to draw the polyline");
    }
}

async function calculateDistanceAndElevation(points) {
    var totalDistance = 0;
    elevations = [];

    for (var i = 0; i < points.length - 1; i++) {
        var segmentDistance = points[i].distanceTo(points[i + 1]);
        totalDistance += segmentDistance;

        var numPoints = 4;
        var interpolatedPoints = interpolatePoints(points[i], points[i + 1], numPoints);

        for (var point of interpolatedPoints) {
            var elevation = await getElevation(point.lat, point.lng);
            var currentDistance = (totalDistance * (elevations.length / interpolatedPoints.length)) / 1000;
            elevations.push({
                distance: currentDistance,
                elevation: elevation
            });
        }
    }
    drawElevationChart(elevations);
}

function interpolatePoints(startLatLng, endLatLng, numPoints) {
    var points = [];
    for (var i = 0; i <= numPoints; i++) {
        var lat = startLatLng.lat + (endLatLng.lat - startLatLng.lat) * (i / numPoints);
        var lng = startLatLng.lng + (endLatLng.lng - startLatLng.lng) * (i / numPoints);
        points.push({ lat: lat, lng: lng });
    }
    return points;
}

async function getElevation(lat, lon) {
    const url = `https://api.open-elevation.com/api/v1/lookup?locations=${lat},${lon}`;
    try {
        const response = await fetch(url);
        const data = await response.json();
        if (data && data.results && data.results.length > 0) {
            return data.results[0].elevation;
        } else {
            console.error('Elevation data not available for this point.');
            return null;
        }
    } catch (error) {
        console.error('Error fetching elevation data:', error);
        return null;
    }
}
function drawElevationChart(elevations) {
    var ctx = document.getElementById('elevationChart').getContext('2d');

    if (window.elevationChart && typeof window.elevationChart.destroy === 'function') {
        window.elevationChart.destroy();
    }
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
            Response: true,
            maintainAspectRatio: false,
            scales: {
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




if (window.location.href.includes("Peak/Index/")) {

    var currentPeak = JSON.parse(currentPeak);
    var coords = currentPeak.Coords.split(',');
    var lat = parseFloat(coords[0]);
    var lng = parseFloat(coords[1]);

    if (!isNaN(lat) && !isNaN(lng)) {
        map.setView([lat, lng], 12);
        L.circle([lat, lng], { color: 'red', radius: 500 }).addTo(map);
        L.marker([lat, lng]).bindPopup(`<b>${currentPeak.Name}</b><br><a href="/Peak/Index/${currentPeak.PeakID}" onclick="window.location='/Peak/Index/${currentPeak.PeakID}'; return false;">View Peak</a>`).addTo(markerLayer);
    } else {
        console.log('Invalid coordinates for current peak: ', currentPeak.Name);
    }
}


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


function showSectionPeaks(peaks) {

    markerLayer.clearLayers();

    peaks.forEach(function (peak) {
        var coords = peak.Coords.split(',');
        var lat = parseFloat(coords[0]);
        var lng = parseFloat(coords[1]);

        if (!isNaN(lat) && !isNaN(lng)) {
            L.marker([lat, lng]).bindPopup(`<b>${peak.Name}</b><br> Elevation: ${peak.Details} <br> Difficulty: ${peak.Difficulty} <br><a href="/Peak/Index/${peak.PeakID}" onclick="window.location='/Peak/Index/${peak.PeakID}'; return false;">View Peak</a>`)
                .addTo(markerLayer)
        } else {
            console.log('Invalid coordinates for peak: ', peak.Name);
        }
    });
}


var sectionBoxContainer = document.getElementById('section-boxes');
Object.keys(sectionsData).forEach(function (sectionName) {
    createSectionBox(sectionName, sectionsData[sectionName]);
});

function createSectionBox(sectionName, sectionCoords) {

    var box = createBoxElement(sectionName);
    var polygon = createPolygon(sectionCoords, sectionName);

    if (polygon) {
        sectionPolygons[sectionName] = polygon;
        addHoverEffect(box, polygon);
        addClickEvent(box, polygon, sectionName);
    }
}

function createBoxElement(sectionName) {
    var box = document.createElement('div');
    box.className = 'section-box';
    box.textContent = sectionName;
    sectionBoxContainer.appendChild(box);
    return box;
}

function createPolygon(sectionCoords, sectionName) {

    var polygonCoords = sectionCoords.map(coord => {
        var lat = parseFloat(coord.lat);
        var lng = parseFloat(coord.lng);


        if (!isNaN(lat) && !isNaN(lng)) {
            return [lat, lng];
        } else {
            return null;

        }
    }).filter(coord => coord !== null);

    if (polygonCoords.length > 0) {
        return L.polygon(polygonCoords, { color: "#0080fE", fillOpacity: 0 })
            .bindPopup("Section: " + sectionName)
            .addTo(map);
    } else {
        return null;
    }
}

function addHoverEffect(box, polygon) {


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
    document.querySelector('.filter-results').style.display = "block";

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

            map.setView([lat, lng], 12);
            markerLayer.eachLayer(function (marker) {
                var markerLatLng = marker.getLatLng();
                if (markerLatLng.lat === lat && markerLatLng.lng === lng) {
                    marker.openPopup();
                }
            });

        });

    });

}