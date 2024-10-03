// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


let resizeTimer;
window.addEventListener('resize', function () {
    document.body.classList.add('no-transition');

    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(function () {
        document.body.classList.remove('no-transition');
    }, 250); // delay in ms to wait for the resize to finish
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



var filterToggle = document.querySelector('.filter-toggle');
var filterContainer = document.querySelector('.filter-container');
filterToggle.addEventListener('click', function () {
    filterContainer.classList.toggle("show");
    routeContainer.classList.toggle("showLRes");

});

var infoBtn = document.querySelector('.info-button');
var infoBox = document.querySelector('.info-data');

infoBtn.addEventListener('click', function () {

    infoBox.classList.toggle("show");
});

infoBox.addEventListener('click', function () {
    infoBox.classList.toggle("show");
});

var routeToggle = document.querySelector('.route-toggle');
var routeContainer = document.querySelector('.route-container');

routeToggle.addEventListener('click', function () {
    routeContainer.classList.toggle("show");

});

var hamburgerBox = document.querySelector('.hamburger-menu input');
var navBar = document.querySelector('.navbar-links');

hamburgerBox.addEventListener('change', function () {
    navBar.classList.toggle('show');
})