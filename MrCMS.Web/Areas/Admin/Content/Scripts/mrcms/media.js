(function ($) {
    $(function () {
        $(document).on('click', '[data-file-list] > [data-file]', function (event) {
            var clickedElement = $(event.target);
            if (clickedElement.is('a[href]') || clickedElement.is('input[type=checkbox]')) {
                return;
            }
            event.preventDefault();
            var checkbox = clickedElement.closest('[data-file]').find('input[type=checkbox]');
            checkbox.prop("checked", !checkbox.prop("checked"));
        });
    });
})(jQuery);