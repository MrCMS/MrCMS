const uploader = function (el, options) {
    const element = el;
    let requestVerificationToken = $('body').data('antiforgery-token');
    const settings = $.extend(defaults, options);
    let self;
    return {
        init: function () {
            self = this;
            const upload = element.find(settings.fileUploadSelector);
            if (upload.length) {
                const acceptFileTypes = settings.acceptFileTypes(element);
                const val = element.find(settings.uploadUrlSelector).val();
                const dropZoneElement = upload.get()[0];

                let maxFileSize = settings.maxFilesize(element);
                // clear it and start again if it's being re-initialized
                try {
                    window.Dropzone.forElement(dropZoneElement).destroy();
                } catch {
                }
                const myDropzone = new window.Dropzone(dropZoneElement, {
                    url: val,
                    acceptedFiles: acceptFileTypes,
                    timeout: 120000,
                    maxFilesize: maxFileSize,
                    dictDefaultMessage: upload.data('message'),
                    parallelUploads: 1,
                    headers: {
                        'RequestVerificationToken': requestVerificationToken
                    }
                });


                myDropzone.on("queuecomplete", function (file) {
                    settings.onFileUploadStopped(file, myDropzone);
                    element.find(settings.progressBarSelector).hide();
                    $(document).trigger('update-area', 'media-folder');
                });

                myDropzone.on("totaluploadprogress", this.progressBar);

                myDropzone.on("error", self.showMessage);
            }

            return self;
        },
        progressBar: function (totalPercentage, totalBytesToBeSent, totalBytesSent) {
            element.find(settings.progressBarSelector).show();
            element.find(settings.progressBarSelectorInner).css('width', totalPercentage + '%');
            element.find(settings.percentCompleteSelector).html(parseInt(totalPercentage) + '%');

        },
        showMessage: function (file, response) {
            noty({
                text: '<div class="notification-date">' + file.status + '</div><div class="notification-msg">' + response + "</div>",
                layout: 'bottomRight',
                timeout: 8000,
                type: "error"
            });
        },
    };
};
const defaults = {
    fileUploadSelector: "#fileupload",
    acceptFileTypes: function (element) {
        const allowedFileTypes = element.find("#allowedFileTypes").val();
        if (allowedFileTypes != null) {
            return allowedFileTypes;
        }
        return ".jpg, .png";
    },
    maxFilesize: function (element) {
        const maxFileSize = element.find("#maxFileSizeUpload").val();
        if (maxFileSize != null) {
            return parseInt(maxFileSize) / 1024 / 1024;
        }
        return 100;
    },
    sequentialUploads: true,
    progressBarSelector: "#progress",
    progressBarSelectorInner: "#progress .progress-bar",
    percentCompleteSelector: "#progress .progress-bar",
    filesSelector: "#mrcmsfiles",
    uploadUrlSelector: "#action-url",
    uploadMediaCategoryIdSelector: "#action-category-id",
    onFileUploadStopped: function (file) {
    }
};

export function initMediaUploader() {
    window.MediaUploader = uploader;
    window.MediaUploader.defaults = defaults;
}

export const MediaUploader = uploader;