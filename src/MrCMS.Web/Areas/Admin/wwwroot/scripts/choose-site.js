(function($) {
    function ensureTargetIsSelf(event) {
        $(event.target).removeAttr('target');
    }
    $(function() {
        $(document).on('submit','#resource-choose-site',ensureTargetIsSelf);
    })
})(jQuery);