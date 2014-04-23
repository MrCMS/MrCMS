var Commenting = function (options) {
    var settings = $.extend(Commenting.defaults, options);
    var self;
    var showReplyTo = function (event) {
        var id = $(event.target).data('show-reply-to');
        $('[data-reply-to=' + id + ']').show();
    };
    var hideReplyTo = function (event) {
        event.preventDefault();
        var id = $(event.target).data('hide-reply-to');
        $('[data-reply-to=' + id + ']').hide();
    };
    var showHideComment = function (event) {
        event.preventDefault();
        console.log(event);
        var link = $(event.target);
        var post = link.parents('[data-post]').eq(0);
        post.toggleClass('collapsed-post');
        link.toggleClass('icon-plus icon-minus');
    };
    return {
        init: function () {
            self = this;
            $(document).on('click', '[data-link-action]', function (event) {
                event.preventDefault();
                $(event.target).parents('form').submit();
            });
            $(document).on('click', '[data-show-hide-comment]', function (event) {
                showHideComment(event);
            });
            $(document).on('click', '[data-show-reply-to]', function (event) {
                showReplyTo(event);
            });
            $(document).on('click', '[data-hide-reply-to]', function (event) {
                hideReplyTo(event);
            });
            return self;
        }
    };
};
Commenting.defaults = {
    addCommentSelector: "[data-comments-add]",
    addCommentData: "comments-add",
    showCommentsSelector: "[data-comments-show]",
    showCommentsData: "comments-show",
};
var comments = new Commenting();
$(function () {
    comments.init();
})