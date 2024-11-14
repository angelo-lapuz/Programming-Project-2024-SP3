// Generate File Preview
export function filePreview(file) {
    const reader = new FileReader();
    reader.onload = function (e) {
        $("#mediaView").html(`<img src="${e.target.result}" alt="Image Preview" />`).show();
    }
    reader.readAsDataURL(file);
}

// Get PresignedURL
export async function getPresignedURL(id, file, controller) {
    const fileType = file.type || "application/octet-stream";
    console.log("Starting getPresignedURL with ID:", id, "and fileType:", fileType);

    try {
        const response = await $.ajax({
            url: `/${controller}/GetPresignedURL`,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ ID: id, FileType: fileType }),
            success: function () {
                console.log("Presigned URL request succeeded.");
            },
            error: function (xhr) {
                console.error("Presigned URL request failed:", xhr);
            }
        });

        console.log("Presigned URL response:", response);

        if (response.success) {
            console.log("Presigned URL response success. Uploading file...");
            const uploadURL = await uploadFile(controller, response.key, response.url, file);
            console.log("Upload URL returned:", uploadURL);
            return uploadURL;
        } else {
            throw new Error(`Presigned URL Error: ${response.message}`);
        }
    } catch (error) {
        console.error("Error inside getPresignedURL:", error);
        throw error;
    }
}


// Upload File to S3 
async function uploadFile(controller, key, url, file) {
    return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        const location = controller === "AddPost" ? "post" : "user";

        xhr.open("PUT", url, true);
        xhr.setRequestHeader("Content-Type", file.type);

        $("#progressContainer").show();

        xhr.upload.onprogress = function (e) {
            if (e.lengthComputable) {
                const percentComplete = (e.loaded / e.total) * 100;
                $("#progressBar").val(percentComplete);

                if (percentComplete === 100) {
                    setTimeout(() => {
                        $("#progressContainer").hide();

                        if (controller === "Profile") {
                            alert("Upload Complete! Remember to hit save to complete the changes!");
                        }
                    }, 500); 
                }
            }
        }

        xhr.onload = function () {
            if (xhr.status == 200) {
                resolve(`https://peakhub-${location}-content.s3.amazonaws.com/${key}`);
            }
            else {
                reject(new Error(`Upload Failed! Status == ${xhr.status}`));
            }
        }

        xhr.onerror = function () { reject(new Error("Error! Error During Upload")); }
        xhr.send(file);
    });
}