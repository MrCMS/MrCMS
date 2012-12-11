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
    
    $("#Name").blur(function() {
        if ($("#mode").is(':checked')) {
            suggestUrl();
        } else {
            setStandardUrl();
        }
    });

    $("#Name").delayKeyup(function () {
        if ($("#mode").is(':checked')) {
            suggestUrl(); 
        } else {
            setStandardUrl();
        }
    }, 200);

    function setStandardUrl() {
        $("#UrlSegment").val($("#Name").val().trim().replace(/\W/g, '-').toLowerCase());
    }

    function suggestUrl() {
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