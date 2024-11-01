// Get Screen Width Function

// calculates a necessary change to styling when the window is re-sized
function getFormWidth() {

    var screenWidth = window.innerWidth;

    if (screenWidth <= 750) {
        return '90%';
    } else {
        return '50%';
    }
}
// this feature is an animation which is used on the page, is flips the active input field from login to forgot password
$(document).ready(function () {
    // forgot button will take the user to the forgotpassword page
    $("#forgotBtn").on('click', function () {
        $("#loginForm").animate({ opacity: 0, width: 0 }, 400, function () { $(this).css("display", "none"); });
        $("#forgotForm").delay(500).css("display", "flex").animate({ opacity: 1, width: getFormWidth() }, 500);
    })

    // cancel forgot will take the user back to the login 
    $("#cancelForgot").on('click', function () {
        $("#forgotForm").animate({ opacity: 0, width: 0 }, 400, function () { $(this).css("display", "none"); });
        $("#loginForm").delay(500).css("display", "flex").animate({ opacity: 1, width: getFormWidth() }, 500);
    })

    // this function changes the styling on the login and forgot password forms when the page is resizedd
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