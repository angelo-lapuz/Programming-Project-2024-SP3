// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let resizeTimer;
let screenwidth = window.innerWidth;

if (screenwidth < 601) {
    setLowRes();
}

if (screenwidth > 900 && screenwidth < 1201) {
    setMidRes();
}

if (screenwidth > 1200) {
    setHighRes();
}

var rootvar = document.querySelector(':root');
var regions = document.querySelectorAll('.info-region-select');
function setSmolRes() {
    rootvar.style.setProperty('--who-what-where', '4rem');
}

function setMidRes() {
    rootvar.style.setProperty('--who-what-where', '6rem');
    regions.forEach((element) => {
        element.classList.remove('buttonB');
    });
}
function setHighRes() {
    regions.forEach((element) => {
        element.classList.add('buttonB');
    });
}

window.addEventListener('resize', function () {

    screenwidth = window.innerWidth;

    document.body.classList.add('no-transition');

    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(function () {
        document.body.classList.remove('no-transition');
    }, 250); // delay in ms to wait for the resize to finish

    //var rs = getComputedStyle(rootvar);
    //console.log(rs.getComputedStyle('--who-what-where'));

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
var mapContainer = document.querySelector('.map-container');

if (filterToggle) {
    filterToggle.addEventListener('click', function () {
        filterContainer.classList.toggle("show");
        routeContainer.classList.toggle("showLRes");
        mapContainer.classList.toggle("show");

    });
}

var infoBtn = document.querySelector('.info-button');
var infoBox = document.querySelector('.info-data');

if (infoBtn) {
    infoBtn.addEventListener('click', function () {
        infoBox.classList.toggle("show");
    });
}

if (infoBox) {
    infoBox.addEventListener('click', function () {
        infoBox.classList.toggle("show");
    });
}

var routeToggle = document.querySelector('.route-toggle');
var routeContainer = document.querySelector('.route-container');

if (routeToggle) {
    routeToggle.addEventListener('click', function () {
        routeContainer.classList.toggle("show");
    });
}

var hamburgerBox = document.querySelector('.hamburger-menu input');
var navBar = document.querySelector('.navbar-links');

hamburgerBox.addEventListener('change', function () {
    navBar.classList.toggle('show');
})

var regionsToggle = document.querySelector('.region-links-toggle');
var regionsBox = document.querySelector('.sections-container');

if (regionsToggle) {
    regionsToggle.addEventListener('click', function () {
        regionsBox.classList.toggle('show');
    });
}


let vid = document.getElementById("bg-video");
vid.playbackRate = 0.75;

var regionSelect = document.getElementsByName('info-region');
var infoRegionBtnList = document.querySelector('.info-region-ul');
var whereRegions = document.querySelectorAll('.info-region-select label');


whereRegions.forEach((element, index) => {
    if (screenwidth > 1200) {
        element.addEventListener('click', function () {
            regionSelect[index].checked = true;
            moveMap(index + 1);
        });
    }
});




document.querySelector('.region-right').addEventListener('click', function () {
    var checkedRadioIndex = parseInt(regionSelect[findChecked()].value);

    if (window.innerWidth < 1201) {
        infoRegionBtnList.style.left = `calc(100vw * -${checkedRadioIndex})`
    }
    regionSelect[checkedRadioIndex].checked = true; 

    moveMap(checkedRadioIndex + 1);

    document.querySelector('.region-left').style.display = "block";

    if (checkedRadioIndex === 9) {
        document.querySelector('.region-right').style.display = "none";
    }
});

document.querySelector('.region-left').addEventListener('click', function () {
    var checkedRadioIndex = parseInt(regionSelect[findChecked()].value);

    var newCheck = checkedRadioIndex - 2;
    if (window.innerWidth < 1201) {
        infoRegionBtnList.style.left = `calc(100vw * -${newCheck})`
    }
    regionSelect[newCheck].checked = true;

    var moveMapIndex = checkedRadioIndex - 1;

    moveMap(moveMapIndex);

    document.querySelector('.region-right').style.display = "block";

    if (checkedRadioIndex === 2) {
        document.querySelector('.region-left').style.display = "none";
    }

});

function findChecked() {

    for ( i = 0; i < regionSelect.length; i++) {
        if (regionSelect[i].checked) {
            return i ;
        }
    }
}

function moveMap(index) {

    document.querySelector('.info-region-img-container img').src = `/img/tasmania-R${index}.png`;

    var image = document.querySelector('.info-region-img-base');
    if (screenwidth < 1201) {
        image.style.height = "200%";

        switch (index) {
            case 1:
                image.style.right = "-90%";
                image.style.bottom = "-100%";
                break;
            case 2:
                image.style.right = "10%";
                image.style.bottom = "-80%";
                break;
            case 3:
                image.style.right = "0%";
                image.style.bottom = "0%";
                break;
            case 4:
                image.style.right = "-30%";
                image.style.bottom = "-60%";
                break;
            case 5:
                image.style.right = "-70%";
                image.style.bottom = "-50%";
                break;
            case 6:
                image.style.right = "-70%";
                image.style.bottom = "-50%";
                break;
            case 7:
                image.style.right = "-90%";
                image.style.bottom = "-50%";
                break;
            case 8:
                image.style.right = "-60%";
                image.style.bottom = "-20%";
                break;
            case 9:
                image.style.right = "-70%";
                image.style.bottom = "20%";
                break;
            case 10:
                image.style.right = "-30%";
                image.style.bottom = "20%";
                break;
        } 
    } else {
        image.style.height = "100%";

        switch (index) {
            case 1:
                image.style.right = "-60%";
                image.style.bottom = "-40%";
                break;
            case 2:
                image.style.right = "-10%";
                image.style.bottom = "-40%";
                break;
            case 3:
                image.style.right = "-20%";
                image.style.bottom = "0%";
                break;
            case 4:
                image.style.right = "-40%";
                image.style.bottom = "-20%";
                break;
            case 5:
                image.style.right = "-60%";
                image.style.bottom = "-20%";
                break;
            case 6:
                image.style.right = "-60%";
                image.style.bottom = "-20%";
                break;
            case 7:
                image.style.right = "-75%";
                image.style.bottom = "-20%";
                break;
            case 8:
                image.style.right = "-55%";
                image.style.bottom = "0%";
                break;
            case 9:
                image.style.right = "-60%";
                image.style.bottom = "15%";
                break;
            case 10:
                image.style.right = "-40%";
                image.style.bottom = "20%";
                break;
        }
    }
}

document.querySelector('.info-regions-more').addEventListener('click', function () {
    document.querySelector('.info-regions-more').style.display = "none";
    document.querySelector('.info-region-selector').style.display = "block";
    document.querySelector('.info-region-selector').style.opacity = "100%";

    if (screenwidth > 1200) {

        regions.forEach((element) => {
            element.classList.add('buttonB');
        });

        document.getElementById('info-regions-container').style.overflow = "unset";

    } 
    moveMap(1);
});

let prevScrollPos = window.scrollY;
var header = document.getElementById('nav-area')


window.addEventListener('scroll', function () {

    var currentScrollPos = window.scrollY;

    //console.log(currentScrollPos);

    if (prevScrollPos < currentScrollPos) {
        header.classList.add('hide');
    } else {
        header.classList.remove('hide');
    }

    prevScrollPos = currentScrollPos;
});

