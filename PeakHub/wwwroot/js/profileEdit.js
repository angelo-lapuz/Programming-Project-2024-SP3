// -------------------------------------------------------------------------------- //
import { getPresignedURL } from './s3Upload.js';
let cropper;
let originalMIME;

$("#cropImage").on("load", function () { 
    if (cropper) { cropper.destroy(); }

    cropper = new Cropper($("#cropImage")[0], {
        aspectRatio: 1,
        viewMode: 1,
        responsive: true,
        autoCropArea: 1
    });
});

$("#addImage").on("click", function () {
    $("#viewCropped").html("").hide();
    $("#imageSelector").click();
})

$("#imageSelector").on("change", function () {
    const file = $(this)[0].files[0];

    if (file) {
        originalMIME = file.type;
        const reader = new FileReader();
        reader.onload = function (e) {
            if ($("#cropImage").attr("src") !== e.target.result) {
                $("#cropImage").attr("src", e.target.result);
                $("#mediaView").show();
            }
        }

        reader.readAsDataURL(file);
    }
    else { $("#mediaView").hide().empty(); }
});

$("#saveImage").on("click", async function (e) {
    e.preventDefault();

    if (cropper) {
        cropper.getCroppedCanvas().toBlob(async function (blob) {
            if (blob) {
                // Generate Display
                const reader = new FileReader();
                reader.onload = function (e) {
                    $("#viewCropped").html(`<img src="${e.target.result}" alt="Cropped Preview">`).show();
                }
                reader.readAsDataURL(blob);

                // Upload File
                try {
                    const url = await getPresignedURL("image", blob, "Profile");
                    console.log(`URL: ${url}`);

                    if (url) {
                        console.log("Success! Image Uploaded");
                        $("#imgLink").val(url);
                    }
                }
                catch (error) {
                    console.log(`Upload Error: ${error}`);
                }
            }
        }, originalMIME);

        cropper.destroy();
        $("#cropImage").attr("src", "");
        $("#mediaView").hide();
    }
});

$("#cancelImage").on("click", function () {
    if (cropper) { cropper.destroy(); }
    $("#cropImage").attr("src", "");
    $("#mediaView").hide();
})

// -------------------------------------------------------------------------------- //