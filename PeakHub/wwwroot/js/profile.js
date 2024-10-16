// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var container = document.querySelector('.container');

console.log(window.innerWidth);

checkEgg();

window.addEventListener('resize', function () {
    checkEgg();
});

function checkEgg() {
    if (window.innerWidth > 1900 && window.innerWidth < 2000) {
        addEgg();
    } else {
        removeEgg();
    }
}

function addEgg() {
    var lol = document.createElement('div');
    lol.setAttribute('class', 'lol-container');
    container.appendChild(lol);

    var lolBox = document.querySelector('.lol-container');

    lol = document.createElement('div');
    lol.setAttribute('class', 'img1');
    lolBox.appendChild(lol);

    lol = document.createElement('img');
    document.querySelector('.img1').appendChild(lol);

    document.querySelector('.img1 img').setAttribute('src', "/img/t1.png");

    lol = document.createElement('div');
    lol.setAttribute('class', 'img2');
    lolBox.appendChild(lol);

    lol = document.createElement('img');
    document.querySelector('.img2').appendChild(lol);

    document.querySelector('.img2 img').setAttribute('src', "/img/t2.png");

    lol = document.createElement('div');
    lol.setAttribute('class', 'text1');
    lolBox.appendChild(lol);

    document.querySelector('.text1').innerHTML = "toot toot here i cum!";

    lol = document.createElement('div');
    lol.setAttribute('class', 'text2');
    lolBox.appendChild(lol);

    document.querySelector('.text2').innerHTML = "YES daddy YES!";

}

function removeEgg() {
    var lol = document.querySelector('.lol-container');
    if (lol) {
        lol.parentNode.removeChild(lol);
    }
    
}

document.querySelector('.lol-container').addEventListener('mouseover', function () {

    document.querySelector('.img1').classList.toggle('animate');
    document.querySelector('.img1 img').classList.toggle('animate');
});