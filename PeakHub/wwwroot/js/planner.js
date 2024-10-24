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
    document.querySelector('.regions-overlay-container').classList.toggle('hide');
    document.querySelector('.regions-overlay-circle').classList.toggle('hide');
    document.querySelector('.route-overlay-container').classList.toggle('hide');
    document.querySelector('.route-overlay-buttons').classList.toggle('infohide');
    document.querySelector('.route-overlay-A').classList.toggle('infohide');
    document.querySelector('.route-overlay-B').classList.toggle('infohide');
    document.querySelector('.route-overlay-route-save').classList.toggle('infohide');
});


var routeOverlayBtn = document.querySelector('.route-overlay-buttons');

filterToggleCheck.addEventListener('click', function () {

    filterContainer.classList.toggle("show");
    routeContainer.classList.toggle("showLRes");
    mapContainer.classList.toggle("show");
    filterClosed.classList.toggle('hide');
    filterOpen.classList.toggle('show');
    routeOverlayBtn.classList.toggle('slide');
    document.querySelector('.route-closed-overlay .fa-arrow-down').classList.toggle('slide');
    

});


var routeToggleCheck = document.getElementById('route-toggle-checkbox');
var routeContainer = document.querySelector('.route-container');


routeToggleCheck.addEventListener('click', function () {
    routeContainer.classList.toggle("show");
    document.querySelector('.route-closed-overlay').classList.toggle('hide');
    document.querySelector('.route-open-overlay').classList.toggle('show');
    document.querySelector('.route-overlay-buttons').classList.toggle('show');
});


var regionsToggleCheck = document.getElementById('regions-toggle-checkbox');
var regionsBox = document.querySelector('.regions-container');

regionsToggleCheck.addEventListener('click', function () {
    regionsBox.classList.toggle('show');
    document.querySelector('.regions-closed-overlay').classList.toggle('hide');
    document.querySelector('.regions-open-overlay').classList.toggle('show');
    
});


var regionSelect = document.getElementsByName('info-region');
var infoRegionBtnList = document.querySelector('.info-region-ul');
var whereRegions = document.querySelectorAll('.info-region-select label');



document.querySelector('.fa-user').addEventListener('click', function () {

    document.querySelector('.nav-login-list').classList.toggle('show');
});