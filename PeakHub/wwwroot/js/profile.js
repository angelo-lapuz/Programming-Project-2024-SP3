
var awardsHeader = document.querySelector('.awards-header');
var awardsSection = document.querySelector('.awardsSection');
var awardsChevron = document.querySelector('.awards-header .fa-chevron-up');

var completedHeader = document.querySelector('.complete-header');
var completedSection = document.querySelector('.completedAbelsSection');
var completedChevron = document.querySelector('.complete-header .fa-chevron-up');

awardsHeader.addEventListener('click', function () {
    if (window.innerWidth < 901) {
        awardsSection.classList.toggle('show');
        awardsChevron.classList.toggle('flip');
    } else if (window.innerWidth < 1201 && window.innerWidth > 900) {

        let checkRotate = getComputedStyle(document.querySelector('.awards-header .fa-chevron-up')).transform;

        if (checkRotate === 'none') {
            awardsSection.classList.toggle('show');
            awardsChevron.classList.toggle('flip');
            awardsHeader.style.background = 'linear-gradient(to bottom, var(--darkgreen), var(--lightgreen) 25%, var(--lightgreen) 75%, var(--darkgreen))';
            completedSection.classList.toggle('show');
            completedChevron.classList.toggle('flip');
            completedHeader.style.background = 'linear-gradient(to bottom, silver, grey 25%, grey 75%, black)';
        }
    }

   
});

completedHeader.addEventListener('click', function () {
    if (window.innerWidth < 901) {
        completedSection.classList.toggle('show');
        completedChevron.classList.toggle('flip');
    } else if (window.innerWidth < 1201 && window.innerWidth > 900) {

        let checkRotate = getComputedStyle(document.querySelector('.awards-header .fa-chevron-up')).transform;

        if (checkRotate !== 'none') {
            awardsSection.classList.toggle('show');
            awardsChevron.classList.toggle('flip');
            awardsHeader.style.background = 'linear-gradient(to bottom, silver, grey 25%, grey 75%, black)';
            completedSection.classList.toggle('show');
            completedChevron.classList.toggle('flip');
            completedHeader.style.background = 'linear-gradient(to bottom, var(--darkgreen), var(--lightgreen) 25%, var(--lightgreen) 75%, var(--darkgreen))';
        }
    }

});

