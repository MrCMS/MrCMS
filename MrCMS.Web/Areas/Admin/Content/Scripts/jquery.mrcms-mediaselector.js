(function ($) {
    var settings, methods = {
        init: function (options) {
            initializeSettings(options);
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
					        $(this).mediaselector('show', settings.mediaSelectorUrl, function (element, result) {
					            that.val(result);
					            var update = updatePreview(result, deleteSpan, buttonDiv);
					            preview.replaceWith(update);
					            preview = update;
					        });
					        return false;
					    }).appendTo(buttonDiv);
                    deleteSpan = $('<button class="btn btn-danger">Remove media...</button>')
					    .click(function () {
					        $(this).mediaselector('show', settings.removeMediaUrl, function (element, result) {
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

        },
        show: function (url, callback, options) {
            if (!settings) { initializeSettings(options); }
            if (url.indexOf('#') == 0) {
                $(url).modal('open').on('hidden', function () {
                    $(this).remove();
                });
            } else {
                getRemoteModel(url, $(this), callback);
            }
        }
    };

    $.fn.mediaselector = function (method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.mrcms-mediaselector');
        }

    };

    function initializeSettings(options) {
        settings = {
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

    function resizeModal(jqElement) {
        var modal = jqElement.hasClass('modal') ? jqElement : jqElement.parents('.modal');
        var height = modal.outerHeight(),
		    windowHeight = $(window).outerHeight(),
		    width = modal.outerWidth(),
		    windowWidth = $(window).outerWidth();
        var top = (windowHeight - height) / 2,
		    left = (windowWidth - width) / 2;

        modal.css('top', top).css('left', left);
    }

    function getRemoteModel(href, element, callback) {
        var div = null;

        $('<a>').attr('href', href).fancybox({
            type: 'iframe',
            padding: 0,
            height: 0,
            onComplete: function () {
                $('#fancybox-frame').load(function () { // wait for frame to load and then gets it's height
                    $(this).contents().find('form').attr('target', '_parent').css('margin', '0');
                    $(this).data('callback', callback).data('element', element);
                    $('#fancybox-content').height($(this).contents()[0].documentElement.scrollHeight);
                    $.fancybox.center();
                });
            }
        }).click().remove();

        //$.get(href, { v: new Date().getTime() }, function (data) {
        //    div = $('<div class="modal">' + data + '</div>');
        //    div.modal({ element: element, callback: callback }).on('hidden', function () {
        //        $(this).remove();
        //    });
        //    setupSelector(div);
        //    $.validator.unobtrusive.parse('.modal form');
        //}).success(function () {
        //    $('input:text:visible:first').focus();
        //    resizeModal(div);
        //    div.wrap('<div class="admin-tools" />');
        //});
    }
})(jQuery);

$(function () {

    function resizeModal() {
        parent.$('#fancybox-content').height(document.documentElement.scrollHeight);
        parent.$.fancybox.center();
    }
    $(document).on('click', '#media-selector .selected-file', function () {
        $(".set-file").removeAttr('disabled');
    }).on('click', '#media-selector .show-hide-file-result', function () {
        var self = $(this);
        $('.show-hide-file-result').not(self).html('Show').parent().siblings('.content').hide(0);
        $('.selected-file').removeAttr('checked');
        if (self.parent().siblings('.content').is(':hidden')) {
            self.html('Hide').parent().siblings('.content').show(0, function () {
                resizeModal();
            });
        } else {
            self.html('Show').parent().siblings('.content').hide(0, function () {
                resizeModal();
            });
        }
        $("#set-file").attr('disabled', 'disabled');
        return false;
    }).on('click', '#media-selector .modal-body a[data-toggle=tab]', function () {
        if ($(this).href != "#library") {
            $(".set-file").attr('disabled', 'disabled');
            $('.selected-file').prop('checked', false);
        }
        resizeModal();
    }).on('click', '#media-selector .file-result .header', function () {
        $(this).find('a').click();
        return false;
    }).on('click', '#media-selector .file-result .header a', function () {
        $(".set-file").attr('disabled', 'disabled');
        $('.selected-file').prop('checked', false);
    }).on('change', '#media-selector #UploadCategoryId', function () {
        var categoryId = $(this).val();
        $('#media-selector #CategoryId').val(categoryId);
        if (categoryId != '') {
            $.get('/Admin/MediaCategory/MiniUploader/', { id: categoryId }, function (response) {
                $('#media-selector-uploader').html(response);
                $('#fileupload').fileupload({
                    add: function (e, data) {
                        var jqXhr = data.submit()
						    .complete(function (result, textStatus, jqXHR) {
						        updateFiles(function () {
						            $('#media-selector-uploader-result').html('<div class="file-result"></div>');
						            $('#media-selector-uploader-result .file-result').append($('#library .file-result').eq(0).html());
						            $('#media-selector-uploader-result .file-result input').each(function (index, element) {
						                var attr = $(element).attr('id');
						                $(element).attr('id', attr + "-upload");
						            });
						            $('#media-selector-uploader-result .file-result label').each(function (index, element) {
						                var attr = $(element).attr('for');
						                $(element).attr('for', attr + "-upload");
						            });
						            setTimeout(function() {
						                resizeModal();
						            }, 15);
						        });
						    });
                    },
                    done: function (e, data) {
                    },
                    create: function (e, data) {
                    }
                });
                resizeModal();
            });
        } else {
            $('#media-selector-uploader').html('');
        }
    }).on('change', '#media-selector #ImagesOnly', function () {
        updateFiles();
    }).on('change', '#media-selector #CategoryId', function () {
        updateFiles();
    }).on('click', 'button[data-action=confirm]', function () {
        var popup = parent.$('#fancybox-frame');
        popup.data('callback')(popup.data('element'), true);
        parent.$.fancybox.close();
    }).on('click', '#media-selector input[data-action=select]', function () {
        var fileValue = $('.selected-file').filter(':checked').val();
        if (fileValue != '') {
            var popup = parent.$('#fancybox-frame');
            $.get('/Admin/MediaCategory/GetFileUrl/', { value: fileValue }, function (url) {
                popup.data('callback')(popup.data('element'), url);
                parent.$.fancybox.close();
            });
        }
    }).on('click', '#media-selector .pagination a', function () {
        var href = $(this).attr('href');
        if (href != null && href != '') {
            $('#library').load(href + ' div#media-selector-data', function () {
                resizeModal();
            });
        }
        return false;
    });

    function updateFiles(callback) {
        var imagesId = $('#media-selector #ImagesOnly').is(':checked');
        var categoryId = $('#media-selector #CategoryId').val();

        $('#library').load('/Admin/MediaCategory/MediaSelector div#library', { categoryId: categoryId, imagesOnly: imagesId, v: new Date().getTime() }, function () {
            resizeModal($(this));
            $(".set-file").attr('disabled', 'disabled');
            if (callback)
                callback();
        });
        return $('#library');
    }

});