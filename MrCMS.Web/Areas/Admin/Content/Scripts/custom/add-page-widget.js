$(function() {
    $('input[name=AddType]').change(function() {
        if ($("input[name=AddType]:checked").val() == 'page') {
            $('#show-recursive').show();
        } else {
            $('#show-recursive').hide();
        }
        resizeModal();
    });
    function resizeModal() {
        parent.$('#fancybox-content').height(parent.$('#fancybox-frame').contents()[0].documentElement.scrollHeight);
        parent.$.fancybox.center();
    }
});