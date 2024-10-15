// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let screenwidth = window.innerWidth;
setResStyles(screenwidth);

function setResStyles(width) {
    if (width < 901) {
        setMidRes();
        setLowRes();
    }

    if (width > 900 && width < 1201) {
        setMidRes();
    }

    if (width > 1200) {
        setMidRes();
        setHighRes();
    }
}



var rootvar = document.querySelector(':root');
var regions = document.querySelectorAll('.info-region-select');
function setLowRes() {

    var rootvar = document.querySelector(':root');
    rootvar.style.setProperty('--who-what-where', '4rem');

    document.getElementById('info-regions-container').style.overflow = "hidden";
}

function setMidRes() {

    var rootvar = document.querySelector(':root');
    var regions = document.querySelectorAll('.info-region-select');
    rootvar.style.setProperty('--who-what-where', '6rem');

    regions.forEach((element) => {
        element.classList.remove('buttonB');
    });

    document.getElementById('info-regions-container').style.overflow = "unset";
}
function setHighRes() {

    var regions = document.querySelectorAll('.info-region-select');

    regions.forEach((element) => {
        element.classList.add('buttonB');
    });

}

let vid = document.getElementById("bg-video");
vid.playbackRate = 0.75;

var regionSelect = document.getElementsByName('info-region');
var infoRegionBtnList = document.querySelector('.info-region-ul');
var whereRegions = document.querySelectorAll('.info-region-select label');


whereRegions.forEach((element, index) => {
    if (screenwidth > 1200) {
        element.addEventListener('click', function () {
            regionSelect[index].checked = true;
            moveMap(index + 1);
            updateRegionText(index + 1);


            if (regionSelect[0].checked) {
                document.querySelector('.region-left').style.display = "none";
            } else {
                document.querySelector('.region-left').style.display = "block";
            }

            if (regionSelect[9].checked) {
                document.querySelector('.region-right').style.display = "none";
            } else {
                document.querySelector('.region-right').style.display = "block";
            }
        
        });
    }
});

document.querySelector('.region-right').addEventListener('click', function () {
    var checkedRadioIndex = parseInt(regionSelect[findChecked()].value);

    if (window.innerWidth < 1201) {
        infoRegionBtnList.style.left = `calc(100vw * -${checkedRadioIndex})`
    }
    regionSelect[checkedRadioIndex].checked = true; 

    var moveMapIndex = checkedRadioIndex + 1;
    
    moveMap(moveMapIndex);
    updateRegionText(moveMapIndex);

    document.querySelector('.region-left').style.display = "block";

    if (checkedRadioIndex === 9) {
        document.querySelector('.region-right').style.display = "none";
    }
});

document.querySelector('.region-left').addEventListener('click', function () {
    var checkedRadioIndex = parseInt(regionSelect[findChecked()].value);

    var newCheck = checkedRadioIndex - 2;
    if (window.innerWidth < 1201) {
        infoRegionBtnList.style.left = `calc(100vw * -${newCheck})`
    }
    regionSelect[newCheck].checked = true;

    var moveMapIndex = checkedRadioIndex - 1;

    moveMap(moveMapIndex);
    updateRegionText(moveMapIndex);

    document.querySelector('.region-right').style.display = "block";

    if (checkedRadioIndex === 2) {
        document.querySelector('.region-left').style.display = "none";
    }

});

function findChecked() {

    for ( i = 0; i < regionSelect.length; i++) {
        if (regionSelect[i].checked) {
            return i ;
        }
    }
}

function moveMap(index) {

    document.querySelector('.info-region-img-container img').src = `/img/tasmania-R${index}.png`;

    var image = document.querySelector('.info-region-img-base');
    if (window.innerWidth < 1201) {
        image.style.width = "200%";

        switch (index) {
            case 1:
                image.style.right = "-95%";
                image.style.top = "10%";
                break;
            case 2:
                image.style.right = "10%";
                image.style.top = "0";
                break;
            case 3:
                image.style.right = "0%";
                image.style.top = "-90%";
                break;
            case 4:
                image.style.right = "-30%";
                image.style.top = "-40%";
                break;
            case 5:
                image.style.right = "-85%";
                image.style.top = "-40%";
                break;
            case 6:
                image.style.right = "-85%";
                image.style.top = "-40%";
                break;
            case 7:
                image.style.right = "-95%";
                image.style.top = "-40%";
                break;
            case 8:
                image.style.right = "-65%";
                image.style.top = "-70%";
                break;
            case 9:
                image.style.right = "-80%";
                image.style.top = "-100%";
                break;
            case 10:
                image.style.right = "-30%";
                image.style.top = "-120%";
                break;
        } 
    } else {
        image.style.width = "100%";

        switch (index) {
            case 1:
                image.style.right = "-40%";
                image.style.top = "0%";
                break;
            case 2:
                image.style.right = "15%";
                image.style.top = "0%";
                break;
            case 3:
                image.style.right = "5%";
                image.style.top = "-50%";
                break;
            case 4:
                image.style.right = "-15%";
                image.style.top = "-20%";
                break;
            case 5:
                image.style.right = "-40%";
                image.style.top = "-20%";
                break;
            case 6:
                image.style.right = "-40%";
                image.style.top = "-20%";
                break;
            case 7:
                image.style.right = "-55%";
                image.style.top = "-20%";
                break;
            case 8:
                image.style.right = "-40%";
                image.style.top = "-50%";
                break;
            case 9:
                image.style.right = "-40%";
                image.style.top = "-50%";
                break;
            case 10:
                image.style.right = "-20%";
                image.style.top = "-50%";
                break;
        }
    }
}

document.querySelector('.info-regions-more').addEventListener('click', function () {
    document.querySelector('.info-regions-more').style.display = "none";
    document.querySelector('.info-region-selector').style.display = "block";
    document.querySelector('.info-region-selector').style.opacity = "100%";

    if (screenwidth > 900) {
        document.getElementById('info-regions-container').style.overflow = "unset";
    }

    if (screenwidth > 1200) {

        regions.forEach((element) => {
            element.classList.add('buttonB');
        });
    } 
    moveMap(1);
    updateRegionText(1);
});

const regionSummary = {
    1: {
        name: "The North West",
        description: "This region is characterized by dense, remote forests and rugged terrain. Peaks such as Mount Emmett and the Black Bluff Range offer challenging hikes, with sweeping views of Tasmania’s wilderness. The North West is lesser-visited, providing a sense of solitude and adventure for hikers.",
        peaks: "13",
        elevation: "1105m - 1559m",
        difficulty: "Moderate to Hard",
        hikeSeason: "Summer to Early Autumn",
        terrain: "Forested slopes, rocky ridges, alpine scrub"

    },
    2: {
        name: "The North East",
        description: "Known for its more temperate climate and lush forests, this region includes peaks such as Ben Nevis and Mount Barrow. The area offers relatively accessible hikes, with green landscapes and forested mountains, making it a popular destination for day hikers and nature lovers.",
        peaks: "13",
        elevation: "1110m - 1573m",
        difficulty: "Easy to Moderate",
        hikeSeason: "Spring to Autumn",
        terrain: "Forested paths, rocky outcrops, grassy slopes"

    },
    3: {
        name: "The South East",
        description: "Featuring more accessible peaks near Hobart, such as Mount Wellington and Mount Field, this region is defined by its proximity to urban centers. Hikes in this area offer stunning views of the Tasman Peninsula and the Derwent River, with a mix of alpine and temperate forests.",
        peaks: "11",
        elevation: "1101m - 1435m",
        difficulty: "Easy to Moderate",
        hikeSeason: "Year-round",
        terrain: "Coastal ridges, alpine meadows, rocky trails"

    },
    4: {
        name: "Central Plateau",
        description: "A vast alpine region characterized by flat-topped mountains, such as Mount Jerusalem, and numerous lakes formed from glacial activity. This area offers more moderate hikes, with wide-open views and dolerite rock formations. It’s known for its high-altitude ecosystems and unique flora.",
        peaks: "21",
        elevation: "1119m - 1499m",
        difficulty: "Moderate",
        hikeSeason: "Late Spring to Early Autumn",
        terrain: "Dolerite peaks, alpine lakes, wide open views"

    },
    5: {
        name: "Pelion & Saint Clair",
        description: "This region includes the famous Cradle Mountain-Lake St Clair National Park, featuring iconic peaks such as Mount Ossa, Tasmania's highest mountain. The region is famous for its rugged alpine terrain, pristine lakes, and the Overland Track, one of Australia's premier long-distance hiking trails.",
        peaks: "29",
        elevation: "1152m - 1617m",
        difficulty: "Moderate to Hard",
        hikeSeason: "Summer to Early Autumn",
        terrain: "Alpine ridges, deep valleys, forested trails"

    },
    6: {
        name: "The Mid West",
        description: "A region known for its rugged and isolated mountain ranges, such as the Dial Range and peaks like Mount Roland. The Mid West features a blend of steep climbs and rewarding vistas, often less crowded than other regions, offering a true wilderness experience.",
        peaks: "25",
        elevation: "1109m - 1447m",
        difficulty: "Moderate",
        hikeSeason: "Spring to Early Autumn",
        terrain: "Rocky slopes, forested areas, steep ridges"

    },
    7: {
        name: "The West",
        description: "Characterized by the remote and wild Western Ranges, this region includes towering peaks like Frenchmans Cap. The West is known for its stark, rugged beauty, with dramatic cliffs, deep valleys, and challenging multi-day hikes through untamed wilderness.",
        peaks: "9",
        elevation: "1123m - 1278m",
        difficulty: "Hard",
        hikeSeason: "Summer",
        terrain: "Quartzite cliffs, rocky ascents, untamed wilderness"

    },
    8: {
        name: "The Gordon",
        description: "Located near the Gordon River, this region is home to peaks like Mount Anne and Mount Lot. The area is known for its dramatic ridges and remote alpine environments, offering some of the most challenging hikes in Tasmania, with sweeping views of the South West wilderness.",
        peaks: "13",
        elevation: "1111m - 1340m",
        difficulty: "Moderate to Hard",
        hikeSeason: "Summer to Early Autumn",
        terrain: "Jagged ridges, alpine scrub, remote trails"

    },
    9: {
        name: "The South West",
        description: "One of Tasmania's wildest regions, the South West is known for its remote and inaccessible peaks, such as Federation Peak. This region offers extreme hiking challenges, with dramatic cliffs, alpine meadows, and vast stretches of wilderness that attract only the most adventurous hikers.",
        peaks: "12",
        elevation: "1106m - 1423m",
        difficulty: "Hard",
        hikeSeason: "Summer",
        terrain: "Alpine meadows, jagged cliffs, untouched wilderness"

    },
    10: {
        name: "The South",
        description: "This region features remote and rugged peaks such as Mount La Perouse and Precipitous Bluff. The South is known for its proximity to the Southern Ocean and its wild, coastal landscapes, offering hikes that combine steep mountain climbs with breathtaking coastal views.",
        peaks: "14",
        elevation: "1110m - 1398m",
        difficulty: "Moderate",
        hikeSeason: "Summer to Early Autumn",
        terrain: "Coastal cliffs, rocky trails, coastal vegetation"

    },

}





function updateRegionText(index) {
    var regionText = document.querySelector('.info-region-text');

    let info = regionSummary[index];

    regionText.innerHTML = `
        <h3>${info.name}</h3>    
        <p>${info.description}</p>
        <table class="region-summary-table">
            <tr>
                <th>Peaks</th>
                <td>${info.peaks}</td>
            </tr>
            <tr>
                <th>Elevations</th>
                <td>${info.elevation}</td>
            </tr>
            <tr>
                <th>Difficulty</th>
                <td>${info.difficulty}</td>
            </tr>
            <tr>
                <th>Terrain</th>
                <td>${info.terrain}</td>
            </tr>
            <tr>
                <th>Ideal Season</th>
                <td>${info.hikeSeason}</td>
            </tr>
        </table>
   `;
}