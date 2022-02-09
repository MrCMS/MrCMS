
export function setupMessageTemplateEditor() {
    (function ($) {
        let templateBody = $('#Body');

        if (templateBody.length > 0) {
            if (templateBody.hasClass('message-ckedit')) {
                CKEDITOR.replace('Body', {fullPage: true, allowedContent: true});
            }
        }
    })(jQuery);
}
export function setupMessageTemplatePreview() {
    if (location.href.match(/Admin\/MessageTemplatePreview/)) {
        var PreviewMessageTemplate = function () {
            function getPreview(event) {
                event.preventDefault();
                var form = $(event.target);
                $.get(form.attr('action'), {id: $('#id').val()}, function (response) {
                    $('#preview').html(response);
                });
            }

            return {
                init: function () {
                    $(document).on('submit', '#preview-message-form', getPreview);
                }
            };
        };

        $(function () {
            new PreviewMessageTemplate().init();
        })
    }
}