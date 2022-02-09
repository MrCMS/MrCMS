export function initSortable() {
    $("[data-sortable]").each((_, element) => {
        const $element = $(element);
        $element.sortable({
            update: function (event, ui) {
                $element.find('li').each(function (index, domElement) {
                    $(domElement).find('[name*="Order"]').val(index);
                });
            }
        });
    });
}