// -------------------------------------------------------------------------------- //
import { getPresignedURL, filePreview } from '../s3Upload.js';

// Setup Quill.JS
var quill = new Quill('#editor', {
    theme: 'snow',
    modules: {
        toolbar: [
            ['bold', 'italic', 'underline', 'strike'],
            ['blockquote', 'code-block'],
            [{ 'header': 1 }, { 'header': 2 }],
            [{ 'list': 'ordered' }, { 'list': 'bullet' }],
            [{ 'align': [] }],
            [{ 'color': [] }, { 'background': [] }]
        ]
    }
});

quill.on('text-change', function () {
    toggleSubmitBtn();
    var text = quill.getText().trim();
    $('#charCount').text(`Characters: [${text.length}]`);
});

// Toggle 'Submit' Button on and off
function toggleSubmitBtn() {
    var media = $("#addPostMedia")[0].files.length > 0;

    var content = quill.root.innerHTML.trim();
    var isContentEmpty = content === '<p><br></p>' || content.length === 0;

    $("#addPostSubmit").toggleClass("deactivate", isContentEmpty && !media);
}

// -------------------------------------------------------------------------------- //
// Cancel Add Post
$("#cancelPost").on("click", function () {
    window.location.href = `/Forum/${ $("#boardID").val() }`;
});

// -------------------------------------------------------------------------------- //
// Drag 'n' Drop
$("#uploadArea").on("dragover", function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).addClass("drag-over");
});
$("#uploadArea").on("dragleave", function () {
    e.preventDefault();
    e.stopPropagation();
    $(this).removeClass("drag-over");
});

$("#uploadArea").on("drop", function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).removeClass("drag-over");

    const file = e.originalEvent.dataTransfer.files[0];
    if (file && file.type.startsWith('image/')) {
        $("#addPostMedia")[0].files = e.originalEvent.dataTransfer.files;
        $("#errorMsg").text("").hide();

        filePreview(file);
        toggleSubmitBtn();
    } else {
        $("#errorMsg").text("Invalid file type. Only images are allowed.").show();
    }
});

// -------------------------------------------------------------------------------- //
// Preview File then Toggle 'Submit' on content
$("#addPostMedia").on("change", function () {
    const file = $(this)[0].files[0];

    if (file) {
        if (file.size > 20 * 1024 * 1024) {
            $("#errorMsg").text("File size exceeds 20 MB").show();
            $(this).val('');
            return;
        }
        if (!file.type.startsWith('image/')) {
            $("#errorMsg").text("Invalid file type. Only images allowed").show();
            $(this).val('');
            return;
        }

        filePreview(file);
        toggleSubmitBtn();
        $("#errorMsg").hide();
    }
});

// Removes selected file 
$("#removeMediaFile").on("click", function () {
    $("#addPostMedia").val('');
    $("#mediaView").hide().empty();

    toggleSubmitBtn();
});

// -------------------------------------------------------------------------------- //
// Submit to DB
async function submitPost(boardID, content, media, mediaType) {
    try {
        const response = await $.ajax({
            url: "AddPost/CreatePost",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                BoardID: boardID,
                Content: content,
                Media: media,
                MediaType: mediaType
            })
        });

        if (response.success) { window.location.href = `/Forum/${response.boardID}`; }
        else {
            $("#errorMsg").show().text(response.message);
            throw new Error(response.message);
        }
    }
    catch (xhr) {
        if (xhr.status === 400 && xhr.responseJSON && xhr.responseJSON.error) {
            $("#errorMsg").text(xhr.responseJSON.error).show();
        } else {
            console.error(`Error: [${xhr.status}] - ${xhr.statusText}`);
        }

        $("#removeMediaFile").removeClass("deactivate");
        $("#addMediaFile").removeClass("deactivate");
        $("#cancelPost").removeClass("deactivate");
        toggleSubmitBtn();
        throw xhr;
    }
}

// Handle Form Submit
$("#addContent").on("submit", async function (e) {
    e.preventDefault();

    $("#errorMsg").text('').hide();
    $("button").addClass("deactivate");

    let media = "NULL", mediaType = "NULL";

    const boardID = $("#boardID").val();
    const file = $("#addPostMedia")[0].files[0];
    const content = quill.root.innerHTML.trim();
    const sanitizedContent = content === '' ? "NULL" : content;

    try {
        if (file) {
            media = await getPresignedURL(boardID, file, "AddPost");
            mediaType = file.type;
        }

        await submitPost(boardID, sanitizedContent, media, mediaType);
    }
    catch (error) {
        $("#removeMediaFile").removeClass("deactivate");
        $("#cancelPost").removeClass("deactivate");
        toggleSubmitBtn();

        console.log(`Error During Upload: ${error}`);
        $("#errorMsg").show().text("An error occurred during the upload.");
    }
});