var AddWebpage = function () {
    var previousValue = '';
    var timer = 0;
    var suggestUrl = function () {
        var pageName = $("#Name").val(),
            documentType = $("#DocumentType:checked").val(),
            parentId = $("#Parent_Id").val(),
            template = $("#PageTemplate_Id").val(),
            useHierarchy = $("#mode").is(':checked');
        if (pageName != "") {
            $.get('/Admin/WebpageUrl/Suggest', {
                pageName: pageName,
                id: parentId,
                documentType: documentType,
                template: template,
                useHierarchy: useHierarchy
            }, function (data) {
                $("#UrlSegment").val(data);
            });
            previousValue = getCurrentValue();
        } else {
            $("#UrlSegment").val('');
            previousValue = getCurrentValue();
        }
    };
    var getCurrentValue = function () {
        return {
            name: $('#Name').val(),
            mode: $('#mode').is(':checked'),
            documentType: $("#DocumentType:checked").val(),
            template: $("#PageTemplate_Id").val()
        };
    };
    var delayedUpdateUrl = function (event) {
        clearTimeout(timer);
        timer = setTimeout(function () { updateUrl(event); }, 300);
    };
    var areValuesChanged = function () {
        var value = previousValue;
        var currentValue = getCurrentValue();
        if (value == null)
            return true;

        return value.documentType != currentValue.documentType
            || value.mode != currentValue.mode
            || value.name != currentValue.name
            || value.template != currentValue.template;
    }
    var updateUrl = function (event) {
        event.preventDefault();
        if (areValuesChanged()) {
            suggestUrl();
        }
    };
    var triggerKeyUp = function (event) {
        event.preventDefault();
        $(event.target).keyup();
    };
    var logCurrentValue = function (event) {
        event.preventDefault();
        previousValue = getCurrentValue();
    };
    var updateAdditionalProperties = function (event) {
        $(".hide-until-document-selected").show();
        $("#message-choose-document").hide();
        var webpageType = $(':radio[name=DocumentType]:checked').val();
        $.get('/Admin/Webpage/AddProperties', { type: webpageType, parentId: $("#Parent_Id").val() }, function (data) {
            $("[data-additional-properties]").html(data);
            admin.initializePlugins();
        });
    };
    return {
        init: function () {
            $(document).on('focus', '#Name', logCurrentValue);
            $(document).on('blur', '#Name', triggerKeyUp);
            $(document).on('keyup', '#Name', delayedUpdateUrl);
            $(document).on('change', '#mode', delayedUpdateUrl);
            $(document).on('change', '#DocumentType', delayedUpdateUrl);
            $(document).on('change', '#PageTemplate_Id', delayedUpdateUrl);
            $(document).on('change', ':radio[name=DocumentType]', updateAdditionalProperties);
            if ($(':radio[name=DocumentType]:checked').length) {
                updateAdditionalProperties();
            }
        }
    };
};

$(function () {
    new AddWebpage().init();

})