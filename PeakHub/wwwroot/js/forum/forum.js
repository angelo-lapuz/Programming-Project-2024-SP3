$(function () { loadPosts(); });

// Handle Inifnite Scroll
let pageIndex = 1, loading = false, limit = false;
const pageSize = 2, boardID = $("#BoardID").val(), userID = $("#UserID").val();

function appendPosts(posts) {
    const container = $("#postsForBoard");

    posts.forEach(post => {
        var content = post.content ? "" : "display: none;";
        var media = post.media ? `<img class='postImg' src="${post.media}" alt="Poster's Media Image" />` : "";

        var likeActivity = post.hasUserLiked ? "" : "inactiveLike";

        container.append(`
            <div class="forumPost">
                <div class="content">
                    <div class="user">
                        <img src="${post.user.profileImg}" alt="Poster's Profile Image" />
                        <p> <strong> ${post.user.username} </strong> </p>
                    </div>

                    <div class="post">
                        <p style='${content}'>${post.content}</p> 
                        ${media} 
                    </div>
                </div>

                <div class="footer">
                    <p>${new Date(post.transactionTimeUTC)
                        .toLocaleString('en-US', { 
                            day: '2-digit',
                            month: '2-digit',
                            year: 'numeric',
                            hour: '2-digit',
                            minute: '2-digit',
                            hour12: true
                        })}</p>

                    <a class="icon">
                        <p id="count_${post.postID}">${post.likeCount}</p>
                        <i id="like_${post.postID}" class="fas fa-thumbs-up ${likeActivity}"></i>
                    </a>
                </div>
            </div>
        `);
    });

    detectAspectRatio(container.find('.postImg').slice(-posts.length));
}

function loadPosts() {
    if (loading || limit) return;

    loading = true;
    $('#loading').html("Searching for more Posts").show();

    $.ajax({
        url: `/Forum/GetForumPosts?boardID=${boardID}&userID=${userID}&pageSize=${pageSize}&pageIndex=${pageIndex}`,
        type: "GET",
        success: function (posts) {
            if (posts.length > 0) {
                pageIndex++;
                appendPosts(posts);
                $('#loading').hide();
            } else {
                var message = "No More Posts Available";
                if (pageIndex === 1) { message = "No Posts! Please Populate"; }

                limit = true;
                $('#loading').html(message).show();
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
            if (this.naturalWidth < this.naturalHeight) {
                contentContainer.addClass('portrait');
            } else {
                contentContainer.addClass('landscape');
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