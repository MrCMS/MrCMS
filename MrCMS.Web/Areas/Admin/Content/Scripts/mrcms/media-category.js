var mediaUploader;
$(function () {
    mediaUploader = new MediaUploader($(document), {
        onFileUploadStopped: function (e, element) {
            var fileList = element.find('#file-list');
            var searchForm = element.find('#search-form');
            if (fileList && searchForm) {
                var data = searchForm.serialize();
                $.get('/Admin/MediaCategory/ShowFiles/' + fileList.data('category-id'), data, function (response) {
                    fileList.replaceWith(response);
                });
            }
        }
    }).init();
});

