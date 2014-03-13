var AddWidget = function () {
    var updateAdditionalProperties = function (event) {
        var type = $('#WidgetType').val();
        $.get('/Admin/Widget/AddProperties', { type: type }, function (data) {
            $("[data-additional-properties]").html(data);
            admin.initializePlugins();
        });
    };
    var handleAddWidget = function (event) {
        event.preventDefault();
        $.post('/Admin/Widget/Add', $(this).serialize(), function (response) {
            var link = $('<a class="btn btn-mini" id="temp-edit" data-toggle="fb-modal" href="/Admin/Widget/Edit/' + response + '?returnUrl=' + window.top.location + '">Edit Widget</a>');
            parent.parent.$("body").append(link);
            parent.parent.$("#temp-edit").click().remove();
        });
    };
    return {
        init: function () {
            $(document).on('change', '#WidgetType', updateAdditionalProperties);
            $('#WidgetType').change();
            $(document).on('submit', '#add-widget-form', handleAddWidget);
        }
    };
};
$(function () {
    new AddWidget().init();
})