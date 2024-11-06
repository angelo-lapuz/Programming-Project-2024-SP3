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


    //var rs = getComputedStyle(rootvar);
    //console.log(rs.getComputedStyle('--who-what-where'));

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

