var MediaUploader = function (el, options) {
    var element = el;
    var settings = $.extend(MediaUploader.defaults, options);
    var self;
    return {
        init: function () {
            self = this;
            if (element.find(settings.fileUploadSelector).length) {
                element.find(settings.fileUploadSelector).fileupload({
                    dataType: 'json',
                    type: 'POST',
                    autoUpload: true,
                    sequentialUploads: settings.sequentialUploads,
                    acceptFileTypes: settings.acceptFileTypes(element),
                    maxFileSize: settings.maxFileSize(element),
                    done: this.fileUploaded,
                    progressall: this.progressBar,
                    dropZone: element.find(settings.dropZoneSelector)
                });

                element.find(settings.fileUploadSelector).on('fileuploadstopped', function (e) { settings.onFileUploadStopped(e, element) });
                element.bind('dragover', function (e) {
                    self.dropZoneEffect(e);
                });

                element.on('fileuploadprocessalways', function (e, data) {
                    self.validateFiles(e, data);
                }).on('fileuploadadded', function (e, data) {
                    element.find(settings.filesSelector).html('');
                });

            }

            return self;
        },
        fileUploaded: function (e, data) {
            //$.each(data.files, function (index, file) {
            //    $('<p/>').text(file.name + ' uploaded').appendTo(settings.filesSelector);
            //});
        },
        progressBar: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            element.find(settings.progressBarSelector).show();
            element.find(settings.progressBarSelectorInner).css('width', progress + '%');
            element.find(settings.percentCompleteSelector).html(progress + '%');

        },
        validateFiles: function (e, data) {
            var index = data.index,
                file = data.files[index];

            if (file.error) {
                element.find(settings.filesSelector)
                    .append('<br/>')
                    .append($('<span class="red"/>').text(file.name + ' ' + file.error));
            }
        },
        dropZoneEffect: function (e) {
            var dropZone = element.find('#dropzone'),
            timeout = window.dropZoneTimeout;
            if (!timeout) {
                dropZone.addClass('in');
                element.find("#drop-zone-text").text(settings.dragHereText);
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
MediaUploader.defaults = {
    fileUploadSelector: "#fileupload",
    acceptFileTypes: function (element) {
        var allowedFileTypes = element.find("#allowedFileTypes").val();
        if (allowedFileTypes != null) {
            var filetypes = "(\\.|\\/)($1)$".replace("$1", allowedFileTypes);
            return new RegExp(filetypes, "i");
        }
        return /(\.|\/)(gif|jpeg|jpg|png|rar|zip)$/i;
    },
    sequentialUploads: true,
    maxFileSize: function (element) {
        var maxFileSize = element.find("#maxFileSizeUpload").val();
        return maxFileSize || 5000000;
    },
    progressBarSelector: "#progress",
    progressBarSelectorInner: "#progress .bar",
    percentCompleteSelector: "#percent-complete",
    filesSelector: "#files",
    dropZoneSelector: "#dropzone",
    dragHereText: "Drop Files Here",
    onFileUploadStopped: function (e, element) {
        if (element.find("#pager-url")) {
            $.get(element.find("#pager-url").val(), function (response) {
                element.find('div[data-paging-type="async"]').replaceWith(response);
            });
        }
    }
};