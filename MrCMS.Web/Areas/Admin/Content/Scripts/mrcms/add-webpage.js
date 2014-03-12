var AddWebpage = function () {
    var previousValue = '';
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
        }
    };
    var getCurrentValue = function () {
        return $('#Name').val();
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
    return {
        init: function () {
            $("#Name").focus(logCurrentValue);
            $("#Name").blur(triggerKeyUp);
            $("#Name").delayKeyup(updateUrl, 300);
        }
    };
};

$(function () {
    new AddWebpage().init();
})