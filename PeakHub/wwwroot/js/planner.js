// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



let screenwidth = window.innerWidth;
setResStyles(screenwidth);

function setResStyles(width) {

    if (width < 901) {
        setMidRes();
        setLowRes();
    }

    if (width > 900 && width < 1201) {
        setMidRes();
    }

    if (width > 1200) {
        setMidRes();
        setHighRes();
    }
}



var rootvar = document.querySelector(':root');
function setLowRes() {
    var rootvar = document.querySelector(':root');
    rootvar.style.setProperty('--who-what-where', '4rem');
}

function setMidRes() {
    var rootvar = document.querySelector(':root');
    rootvar.style.setProperty('--who-what-where', '6rem');
}
function setHighRes() {

}

window.addEventListener('resize', function () {

    setResStyles(screenwidth);

});

const difficultyMap = {
    1: 'All',
    2: 'Easy',
    3: 'Moderate',
    4: 'Hard'
};

var difficultySlider = document.getElementById('filter-difficultySlider');
var difficultyValue = document.getElementById('difficulty-value');
var difficultyInput = document.getElementById('filter-difficulty');

if (difficultySlider) { 
    difficultySlider.addEventListener('input', function () {
        const difficultyLabel = difficultyMap[difficultySlider.value];
        difficultyValue.textContent = difficultyLabel;
        difficultyInput.value = (difficultyLabel === 'All') ? '' : difficultyLabel.charAt(0);
    });
}

var elevationSlider = document.getElementById('filter-elevation');
var elevationValue = document.getElementById('elevation-value');

if (elevationSlider) {
    elevationSlider.addEventListener('input', function () {
        elevationValue.textContent = `${elevationSlider.value} m`;
    });
}




var infoToggle = document.querySelector('.info-toggle');
var filterToggle = document.querySelector('.filter-toggle');
var filterToggleCheck = document.getElementById('filter-toggle-checkbox');
var filterContainer = document.querySelector('.filter-container');
var mapContainer = document.querySelector('.map-container');
var filterOverlay = document.querySelector('.filter-overlay-container');

var filterClosed = document.querySelector('.filter-closed-overlay');
var filterOpen = document.querySelector('.filter-open-overlay');


infoToggle.addEventListener('click', function () {
    filterOverlay.classList.toggle("hide");
});


filterToggleCheck.addEventListener('click', function () {

    filterContainer.classList.toggle("show");
    routeContainer.classList.toggle("showLRes");
    mapContainer.classList.toggle("show");
    filterClosed.classList.toggle('hide');
    filterOpen.classList.toggle('show');

});


var routeToggle = document.querySelector('.route-toggle');
var routeContainer = document.querySelector('.route-container');

if (routeToggle) {
    routeToggle.addEventListener('click', function () {
        routeContainer.classList.toggle("show");
    });
}

var regionsToggle = document.querySelector('.regions-toggle');
var regionsBox = document.querySelector('.regions-container');

regionsToggle.addEventListener('click', function () {
    regionsBox.classList.toggle('show');
});


var regionSelect = document.getElementsByName('info-region');
var infoRegionBtnList = document.querySelector('.info-region-ul');
var whereRegions = document.querySelectorAll('.info-region-select label');




