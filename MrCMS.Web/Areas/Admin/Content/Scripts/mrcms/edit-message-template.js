$(function() {
    $('#IsHtml').click(function () {
        if ($(this).is(':checked')) {
            $('#Body').addClass("ckedit-enabled");
            CKEDITOR.replaceAll("ckedit-enabled");
        } else {
            location.reload();
        }
    });
    
    if ($("#IsHtml").is(':checked')) {
        $('#Body').addClass("ckedit-enabled");
        CKEDITOR.replaceAll("ckedit-enabled");
    }
});
