$(document).ready(function() {
    setInterval(resizeWindow, 1500);
});

function resizeWindow() {
    var parentwindowHeight = parent.window.innerHeight;
    var popupHeight = $(document).height();
    var height = popupHeight;
    if (popupHeight > parentwindowHeight)
        height = parentwindowHeight-100;
    
    $('#fancybox-content, #fancybox-wrap, #fancybox-frame', top.document).height(height);
    top.$.fancybox.center();
}
