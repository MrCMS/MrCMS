(function($) {
    $(function() {
        if ($('#Body').hasClass('message-ckedit')) {
            CKEDITOR.replace('Body', { fullPage: true, allowedContent: true });
        }
    });
})(jQuery);