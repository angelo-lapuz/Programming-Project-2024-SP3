// -------------------------------------------------------------------------------- //
import { getPresignedURL } from './s3Upload.js';
let cropper;
let originalMIME;

function initalizeCropper() {
    const $image = $("#cropImage");
    if (cropper) { cropper.destroy(); }

    cropper = new Cropper($image[0], {
        aspectRatio: 1, 
        viewMode: 1,   
        responsive: true,
        autoCropArea: 1
    });

    $("#saveImage").removeClass("inactive");
}

$("#addImage").on("click", function () { $("#imageSelector").click(); })

$("#imageSelector").on("change", function () {
    const file = $(this)[0].files[0];

    if (file) {
        originalMIME = file.type;
        const reader = new FileReader();
        reader.onload = function (e) {
            $("#cropImage").attr("src", e.target.result);
            $("#mediaView").show();
            $("#addImage").hide();
            initalizeCropper();
        }

        reader.readAsDataURL(file);
    }
    else { $("#mediaView").hide().empty(); }
});

$("#saveImage").on("click", async function (e) {
    e.preventDefault();

    if (cropper) {
        console.log(`MIME == ${originalMIME}`);
        console.log("Canvas size:", cropper.getCroppedCanvas().width, cropper.getCroppedCanvas().height);

        cropper.getCroppedCanvas().toBlob(async function (blob) {
            if (blob) {
                console.log(`BLOB Success: ${blob}`);
                console.log("Blob type:", blob.type);
                console.log("Blob size:", blob.size);

                // Generate Display
                const reader = new FileReader();
                reader.onload = function (e) {
                    $("#mediaView").html(`<img src="${e.target.result}" alt="Cropped Preview" style="max-width: 100%;">`);
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
        $("#addImage").show();
        $("#mediaView").hide();
        $("#cropImage").empty();
    }
});

$("#cancelImage").on("click", function () {
    if (cropper) { cropper.destroy(); }

    $("#addImage").show();
    $("#mediaView").hide();
    $("#cropImage").empty();
})

// -------------------------------------------------------------------------------- //