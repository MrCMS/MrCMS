var MediaUploader = function (options) {
    var settings = $.extend({
        fileUploadSelector: $("#fileupload"),
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png|rar)$/i,
        sequentialUploads: true,
        maxFileSize: 5000000,
        progressBarSelector: $("#progress"),
        progressBarSelectorInner: $("#progress .bar"),
        percentCompleteSelector: $("#percent-complete"),
        filesSelector: $("#files")
    }, options);
    var self;
    return {
        init: function () {
            self = this;

            $(settings.fileUploadSelector).fileupload({
                dataType: 'json',
                type: 'POST',
                autoUpload: true,
                sequentialUploads: settings.sequentialUploads,
                acceptFileTypes: settings.acceptFileTypes,
                maxFileSize: settings.maxFileSize,
                done: this.fileUploaded,
                progressall: this.progressBar,
            });

            $(settings.fileUploadSelector).on('fileuploadstopped', function (e) {
                $.get($("#pager-url").val(), function (response) {
                    $('div[data-paging-type="async"]').replaceWith(response);
                });
            });
            
            $(settings.fileUploadSelector).fileupload({
                dropZone: $('#dropzone')
            });


            $(document).on('fileuploadprocessalways', function (e, data) {
                self.validateFiles(e, data);
            }).on('fileuploadadded', function(e, data) {
                settings.filesSelector.html('');
            });

            return self;
        },
        fileUploaded: function (e, data) {
            //$.each(data.files, function (index, file) {
            //    $('<p/>').text(file.name + ' uploaded').appendTo(settings.filesSelector);
            //});
        },
        progressBar: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $(settings.progressBarSelector).show();
            $(settings.progressBarSelectorInner).css('width', progress + '%');
            $(settings.percentCompleteSelector).html(progress + '%');

        },
        validateFiles: function (e, data) {
            var index = data.index,
                file = data.files[index];

            if (file.error) {
                settings.filesSelector
                    .append('<br/>')
                    .append($('<span class="red"/>').text(file.name + ' ' + file.error));
            }
        }
    };
};
var mediaUploader;
$(function () {
    mediaUploader = new MediaUploader().init();
});

$(document).bind('dragover', function (e) {
    var dropZone = $('#dropzone'),
        timeout = window.dropZoneTimeout;
    if (!timeout) {
        dropZone.addClass('in');
    } else {
        clearTimeout(timeout);
    }
    var found = false,
      	node = e.target;
    do {
        if (node === dropZone[0]) {
            found = true;
            break;
        }
        node = node.parentNode;
    } while (node != null);
    if (found) {
        dropZone.addClass('hover');
    } else {
        dropZone.removeClass('hover');
    }
    window.dropZoneTimeout = setTimeout(function () {
        window.dropZoneTimeout = null;
        dropZone.removeClass('in hover');
    }, 100);
});