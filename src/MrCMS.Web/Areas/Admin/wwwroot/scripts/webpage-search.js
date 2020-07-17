(function($) {
    'use strict';

    $(function() {
        $(document).on('submit', '[data-webpage-search-form]', search);
    });
    function search(event) {
        event.preventDefault();
        var form = $(event.target);

        $.post(form.attr('action'), form.serialize(), function(response) {
            $('[data-webpage-search-results]').replaceWith(response);
        });
    }
})(jQuery);