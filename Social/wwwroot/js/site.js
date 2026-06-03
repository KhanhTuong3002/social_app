// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener('DOMContentLoaded', function () {
    // 1. Global event listener for AJAX Likes and Favorites
    document.addEventListener('click', function (event) {
        const button = event.target.closest('.like-button,.favorite-button');
        if (button) {
            event.preventDefault();
            var form = button.closest('form');
            var postId = form.querySelector('input[name="postId"]').value;
            var postContainer = document.getElementById('post-' + postId);

            fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .then(response => response.text())
                .then(html => {
                    postContainer.innerHTML = html;
                    if (window.UIkit) {
                        UIkit.update(postContainer);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }
    });

    // 2. Global event listener for AJAX Comments (Add and Remove)
    document.addEventListener('submit', function (event) {
        const form = event.target;

        if (form.classList.contains('add-comment-form')) {
            event.preventDefault();
            var postId = form.querySelector('input[name="postId"]').value;
            var postContainer = document.getElementById('post-' + postId);

            fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .then(response => response.text())
                .then(html => {
                    postContainer.innerHTML = html;
                    if (window.UIkit) {
                        UIkit.update(postContainer);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }

        if (form.classList.contains('remove-comments-form')) {
            event.preventDefault();
            var postId = form.querySelector('input[name="postId"]').value;
            var postContainer = document.getElementById('post-' + postId);

            fetch(form.action, {
                method: 'POST',
                body: new FormData(form),
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            })
                .then(response => response.text())
                .then(html => {
                    postContainer.innerHTML = html;
                    if (window.UIkit) {
                        UIkit.update(postContainer);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        }
    });

    // 3. SignalR event listeners for real-time updates from other users
    function setupSignalRListeners() {
        const connection = window.notificationConnection;
        if (connection) {
            console.log("Setting up post/comment real-time SignalR listeners...");
            connection.on("UpdateLikes", (postId, likeCount) => {
                console.log(`Received UpdateLikes for post ${postId}: ${likeCount}`);
                const elements = document.querySelectorAll(`#post-${postId} .like-count`);
                elements.forEach(el => el.textContent = likeCount);
            });

            connection.on("UpdateComments", (postId) => {
                console.log(`Received UpdateComments for post ${postId}`);
                const commentsList = document.querySelector(`#post-${postId} .post-comments-list`);
                if (commentsList) {
                    const showAll = commentsList.getAttribute('data-show-all') === 'true';
                    fetch(`/Home/GetPostComments?postId=${postId}&showAll=${showAll}`)
                        .then(response => response.text())
                        .then(html => {
                            commentsList.outerHTML = html;
                            // update comment count
                            const updatedList = document.querySelector(`#post-${postId} .post-comments-list`);
                            if (updatedList) {
                                const count = updatedList.getAttribute('data-comment-count');
                                const countElements = document.querySelectorAll(`#post-${postId} .comment-count`);
                                countElements.forEach(el => el.textContent = count);
                            }
                        })
                        .catch(err => console.error("Error fetching updated comments: ", err));
                }
            });
        } else {
            // connection not initialized yet, retry shortly
            setTimeout(setupSignalRListeners, 100);
        }
    }

    setupSignalRListeners();
});
