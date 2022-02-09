function updateArea(event, area) {
    const areaName = 'update-' + area;
    const selector = '[data-' + areaName + ']';
    $(selector).each(function () {
        const info = $(this);
        $.get(info.data(areaName), function (result) {
            info.replaceWith(result);
            window.admin.initializePlugins($(selector));
        });
    });
}

export function registerUpdateArea() {
    $(document).on('update-area', updateArea);
}
