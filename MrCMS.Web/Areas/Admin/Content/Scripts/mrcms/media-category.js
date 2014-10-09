var mediaUploader;
$(function () {
    mediaUploader = new MediaUploader($(document), {
        onFileUploadStopped: function (e, element) {
            location.href = location.href;
        }
    }).init();
});

