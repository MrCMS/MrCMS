var Commenting = function (options) {
    var settings = $.extend(Commenting.defaults, options);
    var self;
   
    var reloadPage = function () {
        window.location.href = window.location.href;
    };
    var showMessage = function (message) {
        console.log(message);
    };
    var showReplyTo = function(event) {
        var id = $(event.target).data('show-reply-to');
        $('[data-reply-to=' + id + ']').show();
    };
    //var loadAddForm = function () {
    //    var addComment = $(settings.addCommentSelector);
    //    $.get('/comments/add/' + addComment.data(settings.addCommentData), function (response) {
    //        addComment.html(response);
    //        $.validator.unobtrusive.parse('form');
    //    });
    //};
    //var loadComments = function () {
    //    var showComment = $(settings.showCommentsSelector);
    //    $.get('/comments/show/' + showComment.data(settings.showCommentsData), function (response) {
    //        showComment.html(response);
    //        $.validator.unobtrusive.parse('form');
    //    });
    //};
    return {
        init: function () {
            self = this;
            //this.loadAdd();
            //this.loadShow();
            //$(document).on('submit', 'form', function (event) {
            //    handleAddForm(event);
            //});
            $(document).on('click', '[data-upvote]', function(event) {
                event.preventDefault();
                $(event.target).parents('form').submit();
            });
            $(document).on('click', '[data-show-reply-to]', function (event) {
                showReplyTo(event);
            });
            return self;
        },
        //loadAdd: function() {
        //    loadAddForm();
        //},
        //loadShow: function() {
        //    loadComments();
        //}
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