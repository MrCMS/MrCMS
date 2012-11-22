$.fn.delayKeyup = function(callback, ms) {
    var timer = 0;
    var el = $(this);
    $(this).keyup(function() {
        clearTimeout(timer);
        timer = setTimeout(function() {
            callback(el);
        }, ms);
    });
    return $(this);
};
$(function () {
    $("#suggest-url").click(function(e) {
        e.preventDefault();
        SuggestUrl();
    });

    $("#Name").blur(function() {
        SetStandardUrl();
    });

    $("#Name").delayKeyup(function() {
        SetStandardUrl();
    }, 200);

    function SetStandardUrl() {
        $("#UrlSegment").val($("#Name").val().trim().replace(/\W/g, '-').toLowerCase());
    }

    function SuggestUrl() {
        var pageName = $("#Name").val(),
            parentId = $("#Parent_Id").val();
        if (pageName != "") {
            $.get('/Admin/Webpage/SuggestDocumentUrl', { pageName: pageName, parentId: parentId }, function(data) {
                $("#UrlSegment").val(data);
            });
        }
    }
    
    $("form").validate();
});