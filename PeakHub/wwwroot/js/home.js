$(".flipBtn").on("click", function () {
    $(".front").toggleClass("hideSide");
    $(".back").toggleClass("hideSide");

    if ($(".back").is(":visible")) {
        infoForRegion();
        $("#R_1").addClass('selected');
    }
});

$("[id*='R_']").on("click", function () {
    $("[id*='R_']").each(function () {
        $(this).removeClass('selected');
    });
    $(this).addClass('selected');

    let regionId = $(this).attr("id").split("_")[1];
    infoForRegion(regionId); 
});

function infoForRegion(region = 1) {
    let summary = regionSummary[region];

    $(".back").css({
        "background": `url(/img/tasmania-R${region}.png)`,
        "background-size": "contain",
        "background-repeat": "no-repeat",
        "background-position": "center"
    });

    $("#abel_title").text(summary.name);
    $("#abel_info").text(summary.description);
    $("#peak_total").text(summary.peaks);
    $("#elevation").text(summary.elevation);
    $("#difficulty").text(summary.difficulty);
    $("#terrain").text(summary.terrain);
    $("#ideal_season").text(summary.hikeSeason);
}

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

    }
}