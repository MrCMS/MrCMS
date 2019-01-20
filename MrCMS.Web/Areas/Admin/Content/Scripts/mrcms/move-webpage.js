function showHideMessage() {
    var checked = $('[data-update-urls]').is(':checked');

    $('[data-update-urls-warning]').toggle(checked);
}

$(function () {
    $("[data-parent-chooser]").select2({});
    $('[data-update-urls]').change(showHideMessage);
    showHideMessage();
});