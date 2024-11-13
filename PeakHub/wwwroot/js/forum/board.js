$(function () { autoloadBoards() });
// -------------------------------------------------------------------------------- //
// Search Bar Implementation
$("#searchInput").on("keyup", function () {
    var searchTerm = $(this).val().toLowerCase(); // Get Input

    // Check Items
    $('.itemBox').each(function () {
        var boardName = $(this).find('strong').text().toLowerCase();
        var boardSection = $(this).find('i').text().toLowerCase();

        // Display Matches Only
        if (boardName.includes(searchTerm) || boardSection.includes(searchTerm)) {
            $(this).show();
        }
        else { $(this).hide(); }
    });
});
// -------------------------------------------------------------------------------- //
// Moving to Forum Page\
var page = 1;
var allBoardsLoaded = false;

function appendBoards(boards) {
    const container = $("#listOfBoards");

    boards.forEach(board => {
        container.append(`
            <div data-id='${board.boardID}' class='itemBox'>
                <img src='${board.imageLink}' alt='Board Location Image' />

                <div class='itemData'>
                    <p> <strong> ${board.name} </strong> </p>
                    <p> <i> ${board.section} </i> </p>
                </div>
            </div>
        `);
    });
}

function loadBoards() {
    if (allBoardsLoaded) { return; }

    $.ajax({
        url: `/Board/GetBoards?pageIndex=${page}`,
        type: "GET",
        success: function (boards) {
            if (boards.length > 0) {
                appendBoards(boards);
                page++;
            } else {
                allBoardsLoaded = true;
                console.log("No Boards Found");
            }
        },
        error: function (response) {
            allBoardsLoaded = true;

            console.log("An error has occured!");
            console.log(response);
        }
    });
}

function autoloadBoards() {
    if (!allBoardsLoaded) {
        setInterval(function () { loadBoards(); }, 500);
    }
}

$("#listOfBoards").on("click", ".itemBox", function () {
    var boardID = $(this).data("id");
    window.location.href = `/Forum/${boardID}`;
});