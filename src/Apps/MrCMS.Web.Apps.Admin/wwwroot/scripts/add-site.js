(function($) {
    function updateAllOptions(event) {
        event.preventDefault();
        var value = $(event.target).val();
        $('[name^="sco-"]').val(value);
    }
    $(function() {
        $(document).on('change', '#copy-all', updateAllOptions);
    });
})(jQuery);