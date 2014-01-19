$(function () {

    'use strict';
    // Change this to the location of your server-side upload handler:
    $('#fileupload').fileupload({
        dataType: 'json',
        type: 'POST',
        autoUpload :true,
        sequentialUploads: true,
        done: function (e, data) {
            $.each(data.files, function (index, file) {
                $('<p/>').text(file.name).appendTo('#files');
            });
            $('#progress .bar').css('width', '0%');

        },
        progressall: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress .bar').css(
                'width',
                progress + '%'
            );
        },
        fileuploadfinished: function (e, data) {
            alert("finished");
        }
    }).prop('disabled', !$.support.fileInput)
        .parent().addClass($.support.fileInput ? undefined : 'disabled');
})