var AddWebpage = function () {
    var previousValue = '';
    var timer = 0;
    var setStandardUrl = function () {
        $("#UrlSegment").val($("#Name").val().trim().replace(/[^a-zA-Z0-9-/]/g, '-').toLowerCase());
        previousValue = getCurrentValue();
    };
    var suggestUrl = function () {
        var pageName = $("#Name").val(),
            parentId = $("#Parent_Id").val();
        if (pageName != "") {
            $.get('/Admin/Webpage/SuggestDocumentUrl', { pageName: pageName, id: parentId }, function (data) {
                $("#UrlSegment").val(data);
            });
            previousValue = getCurrentValue();
        } else {
            $("#UrlSegment").val('');
            previousValue = getCurrentValue();
        }
    };
    var getCurrentValue = function () {
        return $('#Name').val();
    };
    var delayedUpdateUrl = function (event) {
        clearTimeout(timer);
        timer = setTimeout(function () { updateUrl(event); }, 300);
    };
    var updateUrl = function (event) {
        event.preventDefault();
        if (previousValue != getCurrentValue()) {
            if ($("#mode").is(':checked')) {
                suggestUrl();
            } else {
                setStandardUrl();
            }
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
        var webpageType = $(':radio[name=DocumentType]:checked').val();
        $.get('/Admin/Webpage/AddProperties', { type: webpageType }, function(data) {
            $("[data-additional-properties]").html(data);
            admin.initializePlugins();
        });
    };
    return {
        init: function () {
            $(document).on('focus', '#Name', logCurrentValue);
            $(document).on('blur', '#Name', triggerKeyUp);
            $(document).on('keyup', '#Name', delayedUpdateUrl);
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