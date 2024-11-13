var liking = false;

$(function () {
    loadPosts();

    // Move to AddPost Page
    $("#addPostForm").on("click", function () { $(this).submit(); });

    // Toggle post content display
    $("#postsForBoard").on("click", "[id*='_toggle']", function () {
        const id = $(this).attr("id").split("_")[0];
        const previewContent = $(`#${id}_preview`);
        const ellipsis = $(`#${id}_ellipsis`);
        const fullContent = $(`#${id}_full`);

        if (fullContent.is(':visible')) {
            fullContent.hide();
            previewContent.show();
            ellipsis.show();
            $(this).text('[Display All]');
        }
        else {
            fullContent.show();
            previewContent.hide();
            ellipsis.hide();
            $(this).text('[Hide Most]');
        }
    });

    // Updating a Like Count on Click IF UserID is not "0" [Not Logged In]
    $("#postsForBoard").on("click", "[id*='like_']", function () {
        if ("@addPost" === "deactivate" || liking) return;

        const likeIcon = $(this);
        liking = true;

        const postID = likeIcon.attr("id").split("_")[1];
        const likeField = $(`#count_${postID}`);

        var likeCount = parseInt(likeField.text());
        const hasNotLiked = likeIcon.hasClass("inactiveLike");

        const type = hasNotLiked ? "POST" : "DELETE";
        const action = hasNotLiked ? "LikePost" : "UnlikePost";       

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
                        $("#like_" + postID).removeClass("inactiveLike");

                        likeIcon.addClass("pop-effect");
                        setTimeout(() => { likeIcon.removeClass("pop-effect"); }, 300); 
                    } else {
                        likeCount--;
                        $("#like_" + postID).addClass("inactiveLike");
                    }

                    likeField.text(likeCount.toString());
                }
            },
            complete: function () { liking = false; }
        });
    });
});

// Handle Inifnite Scroll
let pageIndex = 1, loading = false, limit = false;
const pageSize = 2, boardID = $("#BoardID").val(), userID = $("#UserID").val();

function postHTML(post) {
    var likeActivity = post.hasUserLiked ? "" : "inactiveLike";
    var media = post.media ? `<img class='postImg' src="${post.media}" alt="Poster's Media Image" />` : "";

    var formattedDate = new Date(post.transactionTimeUTC).toLocaleDateString('en-US', {
        day: '2-digit', month: '2-digit', year: 'numeric'
    });
    var formattedTime = new Date(post.transactionTimeUTC).toLocaleTimeString('en-US', {
        hour: '2-digit', minute: '2-digit', hour12: true
    });

    var content = "<p style='display: none;'> </p>";

    if (post.content) {
        if (post.content.length >= 500) {
            content =
            `<div>
                <p id='${post.postID}_preview'>${post.content.slice(0, 500)}...</p>
                <p id='${post.postID}_full' style='display: none;'> ${post.content}</p>
                <button id='${post.postID}_toggle' class='textView'>[Read More]</button>
            </div>`;
        }
        else {
            content = `<p>${post.content}</p>`;
        }
    }

    return `
        <div class='forumPost'>
            <div class='postHead'>
                <img src='${post.user.profileImg}' alt="User's Profile Image" class='postUser' loading="lazy" />
                <div class='userContent'>
                    <h3>${post.user.username}</h3>
                    <p> ${formattedDate}, ${formattedTime} </p>
                </div>
            </div>

            <div class='postContent'> ${content} ${media} </div>

            <div class='postFoot'>
                <i id="like_${post.postID}" class="fas fa-thumbs-up ${likeActivity}"></i>
                <p id="count_${post.postID}">${post.likeCount}</p>
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
    $('#loadMsg').html("<div class='spinner'></div>").show();

    $.ajax({
        url: `/Forum/GetForumPosts?boardID=${boardID}&userID=${userID}&pageSize=${pageSize}&pageIndex=${pageIndex}`,
        type: "GET",
        success: function (posts) {
            if (posts.length > 0) {
                pageIndex++;
                appendPosts(posts);
                setTimeout(() => { $('#loadMsg').hide(); }, 300);
            } else {
                limit = true;

                if (pageIndex === 1) {
                    $('#loadMsg').html("<p>No Posts! Please Populate</p>").show();
                } else {
                    $("#loadMsg").empty().hide();
                }
            }
        },
        error: function (response) {
            limit = true;
            console.log(response);
            $('#loadMsg').html("<p>An Error Occured.Please Reload</p>").show();
        },
        complete: function () { loading = false; }
    });
}

function detectAspectRatio(newImages) {
    newImages.each(function () {
        const img = $(this);
        const contentContainer = img.closest('.postContent');

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