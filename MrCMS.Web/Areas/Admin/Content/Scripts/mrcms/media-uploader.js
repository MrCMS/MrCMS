var MediaUploader = function (el, options) {
    var element = el;
    var settings = $.extend(MediaUploader.defaults, options);
    var self;
    return {
        init: function () {
            self = this;
            Dropzone.autoDiscover = false;
            var upload = element.find(settings.fileUploadSelector);
            if (upload.length) {
                var acceptFileTypes = settings.acceptFileTypes(element);
                var val = element.find(settings.uploadUrlSelector).val();
                var myDropzone = new window.Dropzone(upload.get()[0], {
                    url: val,
                    maxFilesize: settings.maxFileSize(element),
                    acceptedFiles: acceptFileTypes,
                    dictDefaultMessage: upload.data('message')
                });

                myDropzone.on("queuecomplete", function (file) {
                    settings.onFileUploadStopped(file, myDropzone);
                    element.find(settings.progressBarSelector).hide();
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
            alert(response);
        },
    };
};
MediaUploader.defaults = {
    fileUploadSelector: "#fileupload",
    acceptFileTypes: function (element) {
        var allowedFileTypes = element.find("#allowedFileTypes").val();
        if (allowedFileTypes != null) {
            return allowedFileTypes;
        }
        return ".jpg, .png";
    },
    sequentialUploads: true,
    maxFileSize: function (element) {
        var maxFileSize = element.find("#maxFileSizeUpload").val();
        return maxFileSize || 5;
    },
    progressBarSelector: "#progress",
    progressBarSelectorInner: "#progress .progress-bar",
    percentCompleteSelector: "#progress .progress-bar",
    filesSelector: "#mrcmsfiles",
    uploadUrlSelector: "#action-url",
    uploadMediaCategoryIdSelector: "#action-category-id",
    onFileUploadStopped: function (file) {
    }
};