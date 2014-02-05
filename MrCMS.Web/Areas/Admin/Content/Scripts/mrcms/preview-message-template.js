var PreviewMessageTemplate = new function() {
    this.init = function() {
        $('#preview-message').click(function(event) {
            event.preventDefault();
            var form = $(this).parents('form');
            $.get(form.attr('action'), { itemId:$('#itemId').val() }, function(response) {
                $('#preview').html(response);
            });
        });
    };
};

$(function() {
    PreviewMessageTemplate.init();
})