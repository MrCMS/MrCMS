$(function () {
    $('#fileupload').on('fileuploadstopped', function (e) {
        var fileList = $(document).find('#file-list-simple');
        if (fileList) {
            $.get('/Admin/MediaCategory/ShowFilesSimple/' + fileList.data('category-id'), function (response) {
                fileList.replaceWith(response);
            });
        }
    });

    $("div").on('click', 'a.delete-file-simple', (function (e) {
        e.preventDefault();
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
    }));
});



