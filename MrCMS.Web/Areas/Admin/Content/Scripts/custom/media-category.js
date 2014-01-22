var MediaUploader = function (options) {
    var settings = $.extend({
        fileUploadSelector: $("#fileupload"),
        acceptFileTypes: /(\.|\/)(gif|jpeg|jpg|png|rar|zip)$/i,
        sequentialUploads: true,
        maxFileSize: 5000000,
        progressBarSelector: $("#progress"),
        progressBarSelectorInner: $("#progress .bar"),
        percentCompleteSelector: $("#percent-complete"),
        filesSelector: $("#files"),
        dropZoneSelector: $("#dropzone"),
        dragHereText: "Drop Files Here",
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
                dropZone: settings.dropZoneSelector
            });

            $(settings.fileUploadSelector).on('fileuploadstopped', function (e) {
                $.get($("#pager-url").val(), function (response) {
                    $('div[data-paging-type="async"]').replaceWith(response);
                });
            });
            $(document).bind('dragover', function (e) {
                self.dropZoneEffect(e);
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
        },
        dropZoneEffect: function(e) {
            var dropZone = $('#dropzone'),
            timeout = window.dropZoneTimeout;
            if (!timeout) {
                dropZone.addClass('in');
                $("#drop-zone-text").text(settings.dragHereText);
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
        }
    };
};
var mediaUploader;
$(function () {
    var options = {};
    var maxFileSize = $("#maxFileSizeUpload").val();
    if (maxFileSize != null) {
        options.maxFileSize = maxFileSize;
    }
    var allowedFileTypes = $("#allowedFileTypes").val();
    if (allowedFileTypes != null) {
        var filetypes = "(\\.|\\/)($1)$".replace("$1", allowedFileTypes);
        options.acceptFileTypes = new RegExp(filetypes, "i");
    }
    mediaUploader = new MediaUploader(options).init();
});

