var mediaUploader;
$(function () {
    Dropzone.autoDiscover = false;
    mediaUploader = new MediaUploader($(document), {
    }).init();
});

