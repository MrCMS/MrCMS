function updateAdditionalProperties(event) {
    const type = $('#WidgetType').val();
    $.get('/Admin/Widget/AddProperties', {type: type}, function (data) {
        $("[data-additional-properties]").html(data);
        admin.initializePlugins();
    });
}

export function setupAddPageWidget() {
    $(document).on('change', '[data-add-widget-form] #WidgetType', updateAdditionalProperties);
    $('[data-add-widget-form] #WidgetType').change();
}