$(document).ready(function() {
    setInterval(resizeWindow, 1500);
});

function resizeWindow() {

    top.$.fancybox.center();
    $('#fancybox-content, #fancybox-wrap, #fancybox-frame', top.document).height($('html').height());
}
