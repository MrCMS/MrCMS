var mediaUploaderSimple;
$(function () {

    mediaUploaderSimple = new MediaUploader($('#upload-anywhere'), {
        onFileUploadStopped: function (file, dropzone) {
            var fileList = $(document).find('#file-list-simple');
            if (fileList) {
                $.get('/Admin/MediaCategory/ShowFilesSimple/' + fileList.data('category-id'), function (response) {
                    fileList.replaceWith(response);
                });
            }
            dropzone.removeAllFiles();
        }
    }).init();

    $(document).on('click', 'a.delete-file-simple', (function (e) {
        e.preventDefault();
        if (confirm("Are you sure?")) {
            $.ajax({
                type: "POST",
                url: $(this).attr('href'),
                success: function () {
                    var fileList = $(document).find('#file-list-simple');
                    if (fileList) {
                        $.get('/Admin/MediaCategory/ShowFilesSimple/' + fileList.data('category-id'), function (response) {
                            $(fileList).replaceWith(response);
                        });
                    }
                }
            });
        }
    }));
});



