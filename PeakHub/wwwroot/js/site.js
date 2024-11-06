// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Nav List On/Off
$(function () {
    let showingProfileNav = false;
    $("#profile-toggle").on("click", function () {
        showingProfileNav = !showingProfileNav;
        $("#profile-links").toggle(); 
        $("#feature-links").toggle();
    });

    $("#hamburger-toggle").on("click", function () {
        $(this).find("i").toggleClass("fa-bars fa-times");
        $("#mobile-nav .navbar-list").toggleClass("open"); 
    });
});

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
window.addEventListener('scroll', function () {
    var currentScrollPos = window.scrollY;

    if (prevScrollPos < currentScrollPos) {
        $("#nav-area").addClass("hide");
    } else {
        $("#nav-area").removeClass("hide");
    }

    prevScrollPos = currentScrollPos;
});

