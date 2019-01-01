$(document).ready(function () {
    $("#sortable").sortable({
        update: function (event, ui) {
            $('#sortable li').each(function (index, domElement) {
                $(domElement).find('[name*="Order"]').val(index);
            });
        }
    });
});