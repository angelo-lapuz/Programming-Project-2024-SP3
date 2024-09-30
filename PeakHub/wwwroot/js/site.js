// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


document.getElementById('filter-toggle').addEventListener('click', function () {

    var filterOptions = document.getElementById('filter-options');
    if (filterOptions.style.display === 'none' || filterOptions.style.display === '') {
        filterOptions.style.display = 'grid';
    } else {
        filterOptions.style.display = 'none';
    }

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



