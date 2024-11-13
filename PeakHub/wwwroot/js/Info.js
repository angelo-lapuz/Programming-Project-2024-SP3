let currentImg = 0;
let totalImages = 0;


async function getPeakImages(ID) {
    console.log("called");
    try {
        const peakName = currentPeak.Name.replace(/ /g, "_");
        console.log("peakname was: " + peakName);

        // Fetch all image URLs in a single request
        const peakResponse = await fetch(`/GetPeakImages`, {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
            },
            body: JSON.stringify({ peakName: peakName })
        });

        if (!peakResponse.ok) {
            console.log("Error occurred");
            return;
        }

        const images = await peakResponse.json();
        totalImages = images.length;

        if (totalImages === 0) {
            console.log("No images found");
            return;
        }

        const caroselImages = document.getElementById('carosel-images');
        caroselImages.innerHTML = "";  // Clear existing images in the carousel

        // Load all images concurrently
        const imageElements = await Promise.all(
            images.map((img, index) => {
                return new Promise((resolve, reject) => {
                    const imgDiv = document.createElement('div');
                    imgDiv.classList.add('Image-holder');

                    const imgElement = new Image();
                    imgElement.src = img;
                    imgElement.alt = `peak image ${index + 1}`;
                    imgElement.classList.add('carosel-image');

                    imgElement.onload = () => {
                        imgDiv.appendChild(imgElement);
                        imgDiv.style.display = (index === 0) ? "flex" : "none"; // Show only the first image initially
                        resolve(imgDiv);
                    };

                    imgElement.onerror = () => {
                        console.error(`Error loading image ${img}`);
                        resolve(null); // Resolve even if an image fails to load
                    };
                });
            })
        );

        // Append all loaded image divs to the carousel
        imageElements.forEach((imgDiv) => {
            if (imgDiv) caroselImages.appendChild(imgDiv); // Append only if the image loaded successfully
        });

        updateCarosel(); // Ensure the first image is displayed

    } catch (error) {
        console.error("Error fetching peaks", error);
    }
}

function updateCarosel() {
    const caroselImages = document.querySelectorAll('.Image-holder');
    caroselImages.forEach((img, index) => {
        img.style.display = (index === currentImg) ? "flex" : "none";
        img.style.justifyContent = "center";
        img.style.alignItems = "center";
    });
}

function nextImg() {
    if (totalImages > 0) {
        currentImg = (currentImg + 1) % totalImages;
        updateCarosel();
    }
}

function prevImg() {
    if (totalImages > 0) {
        currentImg = (currentImg - 1 + totalImages) % totalImages;
        updateCarosel();
    }
}
window.onload = function () {
    getPeakImages(currentPeak.PeakID);
};

