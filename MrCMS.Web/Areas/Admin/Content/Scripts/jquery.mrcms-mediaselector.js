(function ($) {
    $.fn.mediaselector = function (options) {
        var settings = {
            iconImage: '',
            iconClass: 'media-selector-icon',
            noImageSelectedImage: '/Areas/Admin/Content/Images/no-media-selected.jpg',
            previewStyle: 'max-height: 200px; max-width:200px;',
            mediaSelectorUrl: '/Admin/MediaCategory/MediaSelector',
            removeMediaUrl: '/Admin/MediaCategory/RemoveMedia',
            miniUploaderUrl: '/Admin/MediaCategory/MiniUploader/',
            fileResultUrl: '/Admin/MediaCategory/FileResult/',
            getFileUrl: '/Admin/MediaCategory/GetFileUrl/'
        };
        if (options) {
            $.extend(settings, options);
        }

        function isImage(image) {
            var extension = image.split('.').pop().toLowerCase();
            var imageExtensions = ["jpg", "jpeg", "gif", "png"];

            var imageCheck = false;
            for (var i = 0; i < imageExtensions.length; i++) {
                if (extension == imageExtensions[i]) {
                    imageCheck = true;
                }
            }
            return imageCheck;
        }

        function updatePreview(currentImage, deleteSpan, buttonDiv) {
            if (currentImage != null && currentImage != '') {
                deleteSpan.show();
                buttonDiv.addClass('btn-group-vertical');
                if (isImage(currentImage)) {
                    return $('<img src="' + currentImage + '" style="' + settings.previewStyle + '" />');
                } else {
                    return $('<span>' + currentImage + '</span>');
                }
            } else {
                deleteSpan.hide();
                buttonDiv.removeClass('btn-group-vertical');
                return $('<img src="' + settings.noImageSelectedImage + '" style="' + settings.previewStyle + '" />');
            }
        }

        return this.each(function () {
            var that = $(this);
            var deleteSpan = null,
                buttonDiv = null;
            if (that.is("input")) {
                var currentImage = that.val();
                var para = $('<p>');
                buttonDiv = $('<div>').appendTo(para);
                $('<button class="btn btn-success">Select media...</button>')
                    .click(function () {
                        if ($(this).attr('data-toggle') != 'modal') {
                            $(this).attr("data-toggle", "modal").attr("data-link", settings.mediaSelectorUrl);
                        }
                        launchModal($(this), function (element, result) {
                            that.val(result);
                            var update = updatePreview(result, deleteSpan, buttonDiv);
                            preview.replaceWith(update);
                            preview = update;
                        });
                        return false;
                    }).appendTo(buttonDiv);
                deleteSpan = $('<button class="btn btn-danger">Remove media...</button>')
                    .click(function () {
                        if ($(this).attr('data-toggle') != 'modal') {
                            $(this).attr("data-toggle", "modal").attr("data-link", settings.removeMediaUrl);
                        }
                        launchModal($(this), function (element, result) {
                            that.val('');
                            var update = updatePreview('', deleteSpan, buttonDiv);
                            preview.replaceWith(update);
                            preview = update;
                        });
                        return false;
                    }).appendTo(buttonDiv);
                var preview = updatePreview(currentImage, deleteSpan, buttonDiv);
                that.hide().after(para);
                if (preview != undefined)
                    that.before(preview);
            }
        });
    };

    function launchModal(element, callback) {
        var href = element.attr('href') || element.data('link');
        if (href.indexOf('#') == 0) {
            $(href).modal('open').on('hidden', function () {
                $(this).remove();
            });
        } else {
            getRemoteModel(href, element, callback);
        }
    }

    function getRemoteModel(href, element, callback) {
        var div = null;
        $.get(href, function (data) {
            div = $('<div class="modal">' + data + '</div>');
            div.modal({ element: element, callback: callback }).on('hidden', function () {
                $(this).remove();
            });
            $.validator.unobtrusive.parse('.modal form');
        }).success(function () {
            $('input:text:visible:first').focus();
            resizeModal(div);
            div.wrap('<div class="admin-tools" />');
        });
    }
})(jQuery);


$(function () {
    function updateFiles() {
        var imagesId = $('#media-selector #ImagesOnly').is(':checked');
        var categoryId = $('#media-selector #CategoryId').val();

        $('#library').load('/Admin/MediaCategory/MediaSelector' + '?categoryId=' + categoryId + '&imagesOnly=' + imagesId + ' div#library', function () {
            resizeModal($(this));
            $(".set-file").attr('disabled', 'disabled');
        });
    }

    $(document).on('click', '#media-selector .selected-file', function () {
        $(".set-file").removeAttr('disabled');
    }).on('click', '#media-selector .show-hide-file-result', function () {
        var self = $(this);
        $('.show-hide-file-result').not(self).html('Show').parent().siblings('.content').slideUp();
        $('.selected-file').removeAttr('checked');
        if (self.parent().siblings('.content').is(':hidden')) {
            self.html('Hide').parent().siblings('.content').slideDown(function () {
                resizeModal(self);
            });
        } else {
            self.html('Show').parent().siblings('.content').slideUp(function () {
                resizeModal(self);
            });
        }
        $("#set-file").attr('disabled', 'disabled');
        return false;
    }).on('click', '#media-selector .modal-body a[data-toggle=tab]', function () {
        if ($(this).href != "#library") {
            $(".set-file").attr('disabled', 'disabled');
            $('.selected-file').prop('checked', false);
        }
        resizeModal($(this));
    }).on('click', '#media-selector .file-result .header', function () {
        $(this).find('a').click();
        return false;
    }).on('click', '#media-selector .file-result .header a', function () {
        $(".set-file").attr('disabled', 'disabled');
        $('.selected-file').prop('checked', false);
    }).on('change', '#media-selector #UploadCategoryId', function () {
        var categoryId = $(this).val();
        if (categoryId != '') {
            $.get('/Admin/MediaCategory/MiniUploader/', { id: categoryId }, function (response) {
                $('#media-selector-uploader').html(response);
                $('#fileupload').fileupload({
                    add: function (e, data) {
                        var jqXhr = data.submit()
                            .complete(function (result, textStatus, jqXhr) {
                                var res = JSON.parse(result.responseText);
                                $('#media-selector-uploader-result').html('');
                                for (var i = 0; i < res.length; i++) {
                                    $.get('/Admin/MediaCategory/FileResult/' + res[i].Id, function (resp) {
                                        $('#media-selector-uploader-result').append(resp);
                                    });
                                }
                                updateFiles();
                            });
                    },
                    done: function (e, data) {
                    },
                    create: function (e, data) {
                    }
                });
            });
        } else {
            $('#media-selector-uploader').html('');
        }
    }).on('change', '#media-selector #ImagesOnly', function () {
        updateFiles();
    }).on('change', '#media-selector #CategoryId', function () {
        updateFiles();
    }).on('click', 'button[data-action=confirm]', function () {
        var modal = $(this).parents('.modal');
        modal.data('modal').options.callback(modal.data('modal').options.element, true);
        modal.modal('hide');
    }).on('click', '#media-selector input[data-action=select]', function () {
        var fileValue = $('.selected-file').filter(':checked').val();
        if (fileValue != '') {
            var modal = $(this).parents('.modal');
            $.get('/Admin/MediaCategory/GetFileUrl/', { value: fileValue }, function (url) {
                modal.data('modal').options.callback(modal.data('modal').options.element, url);
                modal.modal('hide');
            });
        }
    }).on('click', '#media-selector .pagination a', function () {
        var href = $(this).attr('href');
        if (href != null && href != '') {
            $('#library').load(href + ' div#media-selector-data', function () {
                resizeModal($(this));
            });
        }
        return false;
    });
})