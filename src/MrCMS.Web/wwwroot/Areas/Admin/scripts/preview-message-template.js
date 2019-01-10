var PreviewMessageTemplate = function () {
    function getPreview(event) {
        event.preventDefault();
        var form = $(event.target);
        $.get(form.attr('action'), { id: $('#id').val() }, function (response) {
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