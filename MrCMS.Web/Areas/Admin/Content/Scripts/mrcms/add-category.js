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

    $("#Name").delayKeyup(function() {
        setStandardUrl();
    }, 100);

    function setStandardUrl() {
        var parentUrl = $("#Parent_UrlSegment").val();
        if (parentUrl != '') {
            parentUrl += "/";
        }
        $("#UrlSegment").val(parentUrl + $("#Name").val().trim().replace(/\W/g, '-').toLowerCase());
    }
    
    $("form").validate();
});