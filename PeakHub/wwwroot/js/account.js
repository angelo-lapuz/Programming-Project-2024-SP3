// Get Screen Width Function
function getFormWidth() {
    var screenWidth = window.innerWidth;

    if (screenWidth <= 750) {
        return '90%';
    } else {
        return '50%';
    }
}

$(document).ready(function () {
    $("#forgotBtn").on('click', function () {
        $("#loginForm").animate({ opacity: 0, width: 0 }, 400, function () { $(this).css("display", "none"); });
        $("#forgotForm").delay(500).css("display", "flex").animate({ opacity: 1, width: getFormWidth() }, 500);
    })

    $("#cancelForgot").on('click', function () {
        $("#forgotForm").animate({ opacity: 0, width: 0 }, 400, function () { $(this).css("display", "none"); });
        $("#loginForm").delay(500).css("display", "flex").animate({ opacity: 1, width: getFormWidth() }, 500);
    })

    $(window).resize(function () {
        var newWidth = getFormWidth();
        if ($("#loginForm").is(":visible")) {
            $("#loginForm").css("width", newWidth);
        }
        if ($("#forgotForm").is(":visible")) {
            $("#forgotForm").css("width", newWidth);
        }
    });
});