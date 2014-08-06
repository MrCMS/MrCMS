$(function() {
    function updateCharacterCount() {
        var abstractElement = $('#Abstract');
        if (!abstractElement.length)
            return;
        var length = abstractElement.val().length;
        var remaining = 160 - length;
        var abstractCharacterCountMessage = $('#abstract-character-count');
        abstractCharacterCountMessage.html($('<span>').html(remaining + ' characters remaining'));
        if (remaining < 0) {
            abstractCharacterCountMessage.addClass('field-validation-error');
        } else {
            abstractCharacterCountMessage.removeClass('field-validation-error');
        }
    }

    $(document).on('keyup', '#Abstract', updateCharacterCount);
    updateCharacterCount();
});