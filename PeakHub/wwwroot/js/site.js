// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


var filterOptions = document.querySelector('.filter-options');
var filterToggleArrow = document.getElementById('filter-toggle-bg-img');

document.getElementById('filter-toggle').addEventListener('click', function () {

    filterOptions.classList.toggle("show");
    filterToggleArrow.classList.toggle("rotate");
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

difficultySlider.addEventListener('input', function () {
    const difficultyLabel = difficultyMap[difficultySlider.value];
    difficultyValue.textContent = difficultyLabel;
    difficultyInput.value = (difficultyLabel === 'All') ? '' : difficultyLabel.charAt(0);
});

var elevationSlider = document.getElementById('filter-elevation');
var elevationValue = document.getElementById('elevation-value');
elevationSlider.addEventListener('input', function () {
    elevationValue.textContent = `${elevationSlider.value} m`;
});


var routingContainer = document.querySelector('.routing-container');

document.getElementById('routing-toggle').addEventListener('click', function () {
    routingContainer.classList.toggle("slide");
});


document.querySelector('.hamburger-menu').addEventListener('click', function () {
    document.querySelector('.navbar-collapse').classList.toggle("toggle");
    document.querySelector('.hamburger-menu-dash1').classList.toggle("toggle");
    document.querySelector('.hamburger-menu-dash2').classList.toggle("toggle");
    document.querySelector('.hamburger-menu-dash3').classList.toggle("toggle");
    document.querySelector('.hamburger-menu').classList.toggle("toggle");
    
});

