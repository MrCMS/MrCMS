$(function () {
    $(document).on('change', '#SiteId', function () {
        if ($(this).val()) {
            $('#other-site-copy-options').show();
        } else {
            $('#other-site-copy-options').hide();
        }
    });
    $('#SiteId').change();
});