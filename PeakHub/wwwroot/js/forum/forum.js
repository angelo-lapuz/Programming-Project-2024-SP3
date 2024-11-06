$(function () {
    loadPosts();

    // Load remaining Post text
    $("#postsForBoard").on("click", "[id*='_more']", function () {
        const id = $(this).attr("id").split("_")[0];
        $(`#${id}_all`).show();
        $(`#${id}_hide`).show();
        $(this).hide();
    });

    // Hide remaining Post text
    $("#postsForBoard").on("click", "[id*='_hide']", function () {
        const id = $(this).attr("id").split("_")[0];
        $(`#${id}_all`).hide();
        $(`#${id}_more`).show();
        $(this).hide();
    });
});

// Handle Inifnite Scroll
let pageIndex = 1, loading = false, limit = false;
const pageSize = 2, boardID = $("#BoardID").val(), userID = $("#UserID").val();

function postHTML(post) {
    var likeActivity = post.hasUserLiked ? "" : "inactiveLike";
    var media = post.media ? `<img class='postImg' src="${post.media}" alt="Poster's Media Image" />` : "";

    return `
        <div class='forumPost'>
            <div class='leftBar'>
                <img src='${post.user.profileImg}' alt="Poster's Image" />

                <p class='dt'>
                    ${new Date(post.transactionTimeUTC).toLocaleDateString('en-US', {
                        day: '2-digit', month: '2-digit', year: 'numeric'
                    })}
                    <br>
                    ${new Date(post.transactionTimeUTC).toLocaleTimeString('en-US', {
                        hour: '2-digit', minute: '2-digit', hour12: true
                    })}
                </p>
            </div>

            <div class='content'>
                <p> <strong> ${post.user.username} </strong> </p>

                <div class="post">
                    ${post.content ? `<p>
                        ${post.content.length > 500 ?
                            `${post.content.slice(0, 500)}
                            <span id="${post.postID}_all" style="display: none;">${post.content.slice(500)}</span>
                            <span id="${post.postID}_more" class='textView'>... [Display All]</span>
                            <span id="${post.postID}_hide" class='textView' style="display: none;">[Hide Most]</span>`
                    : post.content} </p>` : ""}

                    ${media} 
                </div>
            </div>

            <div class='rightBar'>
                <div class='like'>
                    <i id="like_${post.postID}" class="fas fa-thumbs-up ${likeActivity}"></i>
                    <p id="count_${post.postID}">${post.likeCount}</p>
                </div>
            </div>
        </div>
    `;
}

function appendPosts(posts) {
    const container = $("#postsForBoard");

    posts.forEach(post => {
        container.append(postHTML(post));
    });

    detectAspectRatio(container.find('.postImg').slice(-posts.length));
}

function loadPosts() {
    if (loading || limit) return;

    loading = true;
    $('#loading').html("Searching For Posts... Please Wait").show();

    $.ajax({
        url: `/Forum/GetForumPosts?boardID=${boardID}&userID=${userID}&pageSize=${pageSize}&pageIndex=${pageIndex}`,
        type: "GET",
        success: function (posts) {
            if (posts.length > 0) {
                pageIndex++;
                appendPosts(posts);
                $('#loading').hide();
            } else {
                limit = true;

                if (pageIndex === 1) {
                    $('#loading').html("No Posts! Please Populate").show();
                } else {
                    $("#loading").html("").hide();
                }
            }
        },
        error: function (response) {
            limit = true;
            console.log(response);
        },
        complete: function () { loading = false; }
    });
}

function detectAspectRatio(newImages) {
    newImages.each(function () {
        const img = $(this);
        const contentContainer = img.closest('.post');

        img.on('load', function () {
            const squareThreshold = 0.15,
                aspectRatio = this.naturalWidth / this.naturalHeight;

            if (Math.abs(aspectRatio - 1) <= squareThreshold) {
                contentContainer.addClass("square");
            } else if (aspectRatio >= 2) {
                contentContainer.addClass("long-landscape");
            } else if (aspectRatio > 1) {
                contentContainer.addClass("landscape");
            } else if (aspectRatio <= 0.5) {
                contentContainer.addClass("tall-portrait");
            } else {
                contentContainer.addClass("portrait");
            }
        });

        if (this.complete) { img.trigger('load'); }
    });
}

$('#postsForBoard').on('scroll', function () {
    const container = $(this);

    if (container.scrollTop() + container.innerHeight() >= container[0].scrollHeight - 50) {
        loadPosts();
    }
});

// Move to AddPost Page
$("#addPostForm").on("click", function () { $(this).submit(); });

// Updating a Like Count on Click IF UserID is not "0" [Not Logged In]
$("#postsForBoard").on("click", "[id*='like_']", function () {
    if ("@addPost" === "deactivate") return;

    const postID = $(this).attr("id").split("_")[1];
    const likeField = $(`#count_${postID}`);

    var likeCount = parseInt(likeField.text());

    const hasNotLiked = $(this).hasClass("inactiveLike");
    const action = hasNotLiked ? "LikePost" : "UnlikePost";
    const type = hasNotLiked ? "POST" : "DELETE";

    // AJAX Request
    $.ajax({
        url: `/Forum/${action}`,
        type: `${type}`,
        data: { postID: parseInt(postID) },
        success: function (response) {
            console.log(`Like Update: ${response.message}`);

            if (response.success) {
                if (hasNotLiked) {
                    likeCount++;
                    $("#like_" + postID).removeClass("inactiveLike").addClass("activeLike");
                } else {
                    likeCount--;
                    $("#like_" + postID).removeClass("activeLike").addClass("inactiveLike");
                }

                likeField.text(likeCount.toString());
            }
        }
    });
});