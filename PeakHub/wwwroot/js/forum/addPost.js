// -------------------------------------------------------------------------------- //

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
    window.location.href = `/Forum/Index?boardID=${ $("#boardID").val() }`;
});

// -------------------------------------------------------------------------------- //

// Generate File Preview
function filePreview(file) {
    const reader = new FileReader();

    reader.onload = function (e) {
        $("#mediaView").html(`<img src="${e.target.result}" alt="Image Preview" />`).show();
    }

    reader.readAsDataURL(file);
}

// Preview File then Toggle 'Submit' on content
$("#addPostMedia").on("change", function () {
    const file = $(this)[0].files[0];

    if (file) {
        filePreview(file);
        $("#addMediaFile").addClass("deactivate");
        $("#removeMediaFile").removeClass("deactivate");
    }

    toggleSubmitBtn();
});

// Allows User to select file [img for now [TEMP]]
$("#addMediaFile").on("click", function () { $("#addPostMedia").click(); });

// Removes selected file 
$("#removeMediaFile").on("click", function () {
    $("#addPostMedia").val('');
    $("#mediaView").hide().empty();

    $("#removeMediaFile").addClass("deactivate");
    $("#addMediaFile").removeClass("deactivate");

    toggleSubmitBtn();
});

// -------------------------------------------------------------------------------- //

// Upload File to S3 
function uploadFile(key, url, file, callback) {
    const xhr = new XMLHttpRequest();

    xhr.open("PUT", url, true);
    xhr.setRequestHeader("Content-Type", file.type);

    $("#progressContainer").show();

    xhr.upload.onprogress = function (e) {
        if (e.lengthComputable) {
            const percentComplete = (e.loaded / e.total) * 100;
            $("#progressBar").val(percentComplete);
        }
    }

    xhr.onload = function () {
        if (xhr.status == 200) {
            callback(`https://peakhub-post-content.s3.amazonaws.com/${key}`);
        } else {
            callback(null);
        }
    }

    xhr.onerror = function () { alert("An Error Occured"); }
    xhr.send(file);
}

// Submit to DB
function submitPost(boardID, content, media = "NULL", mediaType = "NULL") {
    $.ajax({
        url: "AddPost/CreatePost",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            BoardID: boardID,
            Content: content,
            Media: media,
            MediaType: mediaType
        }),
        success: function (response) {
            if (response.success) {
                window.location.href = `/Forum/Index?boardID=${response.boardID}`;
            } else {
                $("#errorMsg").show().text(response.message);
            }
        },
        error: function (xhr) {
            // Set Error
            if (xhr.status === 400 && xhr.responseJSON && xhr.responseJSON.error) {
                $("#errorMsg").text(xhr.responseJSON.error).show();
            } else {
                console.log(`Error: [${xhr.status}] - ${xhr.statusText}`);
            }

            // Enable Buttons
            if (Media != null) {
                $("#removeMediaFile").removeClass("deactivate");
            } else {
                $("#addMediaFile").removeClass("deactivate");
            }

            $("#cancelPost").removeClass("deactivate");
            toggleSubmitBtn();
        }
    });
}

// Handle Form Submit
$("#createPost").on("submit", function (e) {
    e.preventDefault();

    $("#errorMsg").text('').hide();
    $("button").addClass("deactivate");

    const boardID = $("#boardID").val();
    const file = $("#addPostMedia")[0].files[0];
    const content = quill.root.innerHTML.trim();
    const sanitizedContent = (content === null || content === '') ? "NULL" : content;

    if (file) {
        const fileType = file.type || "application/octet-stream";

        $.ajax({
            url: "/AddPost/GetPresignedURL",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ BoardID: boardID, FileType: fileType }),
            success: function (response) {
                if (response.success) {
                    uploadFile(response.key, response.url, file, function (url) {
                        if (url) {
                            console.log(`Upload Complete: [${url}]`);
                            submitPost(boardID, sanitizedContent, url, response.type);
                        }
                        else { console.log("Upload Error!"); }
                    });
                }
                else {
                    console.log(`Signed URL Error: [${response.message}]`);
                }
            },
            error: function (response) {
                console.log(`Get URL Error: [${response.message}]`);
            }
        });
    }
    else { submitPost(boardID, sanitizedContent); }
});