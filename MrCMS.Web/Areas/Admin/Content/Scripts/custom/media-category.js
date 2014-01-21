$(function () {

    'use strict';
    // Change this to the location of your server-side upload handler:
    $('#fileupload').fileupload({
        dataType: 'json',
        type: 'POST',
        autoUpload: true,
        sequentialUploads: true,
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png|rar)$/i,
        maxFileSize: 5000000, // 5 MB
        done: function(e, data) {
            $.each(data.files, function(index, file) {
                $('<p/>').text(file.name).appendTo('#files');
            });

        },
        progressall: function(e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progress').show();
            $('#progress .bar').css(
                'width',
                progress + '%'
            );
            $("#percent-complete").html(progress + '%');
        },
        fileuploadfinished: function(e, data) {
            alert("finished");
        },
    }).on('fileuploadprocessalways', function (e, data) {
        var index = data.index,
            file = data.files[index],
            node = $("#files");
        if (file.preview) {
            node
                .prepend('<br>')
                .prepend(file.preview);
        }
        if (file.error) {
            node
                .append('<br>')
                .append($('<span class="text-danger"/>').text(file.name + ' ' + file.error));
        }
        if (index + 1 === data.files.length) {
            data.context.find('button')
                .text('Upload')
                .prop('disabled', !!data.files.error);
        }
    });

    $('#fileupload')
    .bind('fileuploaddestroy', function (e, data) { console.log("fileuploaddestroy"); })
    .bind('fileuploaddestroyed', function (e, data) { console.log("fileuploaddestroyed"); })
    .bind('fileuploadadded', function (e, data) { console.log("fileuploadadded"); })
    .bind('fileuploadsent', function (e, data) { console.log("fileuploadsent"); })
    .bind('fileuploadcompleted', function (e, data) { console.log("fileuploadcompleted"); })
    .bind('fileuploadfailed', function (e, data) { console.log("fileuploadfailed"); })
    .bind('fileuploadfinished', function (e, data) { console.log("fileuploadfinished"); })
    .bind('fileuploadstarted', function (e) { console.log("fileuploadstarted"); })
    .bind('fileuploadstopped', function (e) {
        location.href = location.href; console.log("fileuploadstopped");
    });


})