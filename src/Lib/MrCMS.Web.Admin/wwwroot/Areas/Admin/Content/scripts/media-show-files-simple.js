let mediaUploaderSimple;

export function setupSimpleFiles() {

    // mediaUploaderSimple = new MediaUploader($('#upload-anywhere'), {
    //     onFileUploadStopped: function (file, dropzone) {
    //         const fileList = $(document).find('#file-list-simple');
    //         console.log({fileList})
    //         if (fileList) {
    //             $.get('/Admin/MediaCategory/ShowFilesSimple/' + fileList.data('category-id'), function (response) {
    //                 fileList.replaceWith(response);
    //             });
    //         }
    //         dropzone.removeAllFiles();
    //     }
    // }).init();

    $(document).on('click', 'a.delete-file-simple', (function (e) {
        e.preventDefault();
        if (confirm("Are you sure?")) {
            $.ajax({
                type: "POST",
                url: $(this).attr('href'),
                success: function () {
                    const fileList = $(document).find('#file-list-simple');
                    if (fileList) {
                        $.get('/Admin/MediaCategory/ShowFilesSimple/' + fileList.data('category-id'), function (response) {
                            $(fileList).replaceWith(response);
                        });
                    }
                }
            });
        }
    }));


}
