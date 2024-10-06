let currentImg = 0;
const images = document.querySelectorAll('#carosel-image img');
const totalImages = images.length;

function showImages(n) {
    if (n >= totalImages) {
        currentImg = 0;
    } else if (n < 0) {
        currentImg = totalImages - 1;
    } else {
        currentImg = n;
    }
    const offset = -currentImg * 100;;
    document.getElementById('carosel-image').style.transform = `translateX(${offset}%)`;
}

function nextImg() {
    showImages(currentImg + 1);
}

function previousImg() {
    showImages(currentImg - 1);
}   
showImages(currentImg);