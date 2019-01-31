(function ($) {
    'use strict';

    $(function () {
        $(document).on('click', 'div[data-paging-type="async"] .pagination a[href]', getResults);
    });
    function getResults(event) {
        event.preventDefault();
        var self = $(event.target);
        $.get(this.href, function (response) {
            self.parents('div[data-paging-type="async"]').replaceWith(response);
        });
    }
})(jQuery);