let currentImg = 0;
let totalImages = 0;

async function getPeakImages(ID) {
    console.log("called");
    try {
        const peakName = currentPeak.Name.replace(/ /g, "_");
        console.log("peakname was: " + peakName);

        const peakResponse = await fetch(`/GetPeakImages`, {
            method: 'POST',
            headers: {
                'Content-type': 'application/json',
            },
            body: JSON.stringify({ peakName: peakName })
        });

        if (!peakResponse.ok) {
            console.log("Error occurred");
        }

        const images = await peakResponse.json();
        const caroselImages = document.getElementById('carosel-images');

        caroselImages.innerHTML = "";

        images.forEach((img, index) => {

            const imgDiv = document.createElement('div');
            imgDiv.classList.add('Image-holder');

            const imgElement = document.createElement('img');

            imgElement.src = img;
            imgElement.alt = `peak image ${index + 1}`;
            imgElement.classList.add('carosel-image');

            imgDiv.appendChild(imgElement);

            if (index === 0) {
                
            }
            caroselImages.appendChild(imgDiv);
        });

        totalImages = images.length; 

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
        img.style.minWidth = "100%";              
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

