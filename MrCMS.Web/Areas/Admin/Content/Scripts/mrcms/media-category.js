var mediaUploader;
$(function () {
    mediaUploader = new MediaUploader($(document), {
        onFileUploadStopped: function (e, element) {
            var fileList = element.find('#file-list');
            if (fileList) {
                var category = fileList.data('category-id');
                if (category == 0)
                    category = '';
                $.get('/Admin/MediaCategory/ShowFiles/' + category, function (response) {
                    fileList.replaceWith(response);
                });
            }
        }
    }).init();
});

