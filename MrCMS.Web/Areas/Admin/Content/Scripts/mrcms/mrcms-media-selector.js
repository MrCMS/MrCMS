var MediaSelector = function (options) {
    var element,
        settings = jQuery.extend(MediaSelector.defaults, options),
        self,
        timer,
        mediaUploader,
        $;

    var toggle = function (link) {
        var id = link.data('media-toggle');
        var thisElement = element.find('[data-file-result=' + id + ']');
        var others = element.find('[data-file-result]').not(thisElement);
        others.each(function () {
            var result = $(this);
            hideResult(result);
        });
        var shown = thisElement.data('content-shown');
        if (shown) {
            hideResult(thisElement);
        } else {
            thisElement.find(settings.mediaResultSelector).slideDown();
            thisElement.data('content-shown', true);
            link.html('Hide');
        }
    };
    var hideResult = function (result) {
        var shown = result.data('content-shown');
        if (shown) {
            result.find(settings.mediaResultSelector).slideUp();
            result.find(settings.mediaToggleSelector).html('Show');
            deselectAll(result);
            result.data('content-shown', false);
        }
    };
    var submitForm = function (event) {
        event.preventDefault();
        var form = element.find(settings.formSelector);
        var url = form.attr('action') + '?' + form.serialize();
        updateResults(url);
    };
    var changePage = function (event) {
        event.preventDefault();
        var url = $(this).attr('href');
        updateResults(url);
    };
    var updateResults = function (url) {
        $.get(url, function (response) {
            var newResults = $(response).find(settings.resultsHolderSelector);
            $(element).find(settings.resultsHolderSelector).replaceWith(newResults);
            initializeMediaUploader();
        });
    };
    var selectFile = function (event) {
        var file = $(this).data('file');
        element.find('[data-action="select"][data-file=' + file + ']').removeAttr('disabled');
    };
    var deselectAll = function (result) {
        result.find('input[data-file]').removeAttr('checked');
        result.find('[data-action="select"]').attr('disabled', 'disabled');
    };
    var selected = function (event) {
        event.preventDefault();
        var fileValue = element.find('input[data-file]').filter(':checked').val();
        if (fileValue != '') {
            if (settings.onSelected) {
                $.get('/Admin/MediaSelector/GetFileInfo/', { value: fileValue }, function (info) {
                    settings.onSelected(info);
                });
            }
        }
    };

    var initializeMediaUploader = function () {
        mediaUploader = new MediaUploader(element, {
            onFileUploadStopped: function (event) {
                var form = element.find(settings.formSelector);
                var url = form.attr('action') + '?' + form.serialize();
                updateResults(url);
            }
        });
        mediaUploader.init();
    };

    return {
        show: function (jQ) {
            self = this;
            $ = jQ;
            var link = $('<a>').attr('href', settings.selectorUrl);
            link.hide();
            link.fancybox({
                type: 'iframe',
                autoSize: true,
                minHeight: 400,
                padding: 0,
                afterShow: function () {
                    element = $('.fancybox-iframe').contents();
                    element.find('form').css('margin', '0');
                    self.init();
                }
            }).click().remove();
            return self;
        },
        init: function () {
            self = this;
            initializeMediaUploader();
            $(element).on('click', 'div.header', function (event) {
                event.preventDefault();
                var link = $(this).find(settings.mediaToggleSelector);
                if (link.length) {
                    toggle(link);
                }
            });
            $(element).on('submit', settings.formSelector, submitForm);
            $(element).on('change', '#CategoryId', submitForm);
            $(element).on('keyup', '#Query', function (event) {
                timer && clearTimeout(timer);
                timer = setTimeout(function () { submitForm(event); }, 300);
            });
            $(element).on('click', '.pagination a', changePage);
            $(element).on('click', 'input[data-file]', selectFile);
            $(element).on('click', '[data-action="select"]', selected);

            return self;
        },
        getSettings: function () {
            return settings;
        }
    };
};

var MediaSelectorWrapper = function (el, options) {
    var element = el,
        settings = $.extend(MediaSelectorWrapper.defaults, options),
        self,
        preview,
        removeButton,
        selectButton,
        buttonHolder,
        eventsRegistered = false;
    var getValue = function () {
        return element.val();
    };
    var valueIsSet = function () {
        var value = getValue();
        return value != null && value != '';
    };
    var getPreview = function () {
        if (valueIsSet()) {
            var value = getValue();
            if (isImage(value)) {
                return $('<img src="' + value + '" style="' + settings.previewStyle + '" />');
            } else {
                return $('<span>' + value + '</span>');
            }
        } else {
            return $('<img src="' + settings.noImageSelectedImage + '" style="' + settings.previewStyle + '" />');
        }
    };
    var isImage = function (image) {
        var extension = image.split('.').pop().toLowerCase();
        var imageExtensions = ["jpg", "jpeg", "gif", "png"];

        var imageCheck = false;
        for (var i = 0; i < imageExtensions.length; i++) {
            if (extension == imageExtensions[i]) {
                imageCheck = true;
            }
        }
        return imageCheck;
    };
    return {
        init: function () {
            self = this;
            if (element.is("input")) {
                var para = $('<p>');
                buttonHolder = $('<div>').appendTo(para);
                preview = $('<div>');
                element.before(preview);
                selectButton = $('<button>').addClass(settings.selectClasses).html(settings.selectMessage).attr('data-media', 'select')
                    .appendTo(buttonHolder);
                removeButton = $('<button>').addClass(settings.removeClasses).html(settings.removeMessage).attr('data-media', 'remove')
                    .appendTo(buttonHolder);
                element.hide().after(para);
                self.update();
                self.registerEvents();

            }
        },
        registerEvents: function () {
            if (!eventsRegistered) {
                selectButton.click(function (event) {
                    event.preventDefault();
                    self.show();
                });
                removeButton.click(function (event) {
                    event.preventDefault();
                    self.reset();
                });
            }
            eventsRegistered = true;
        },
        update: function () {
            var value = this.getValue();
            if (value !== null && value !== '') {
                removeButton.show();
                buttonHolder.addClass('btn-group btn-group-vertical');
            } else {
                removeButton.hide();
                buttonHolder.removeClass('btn-group btn-group-vertical');
            }
            var newPreview = getPreview();
            preview.replaceWith(newPreview);
            preview = newPreview;
        },
        getValue: function () {
            return getValue();
        },
        setValue: function (value) {
            element.val(value);
            self.update();
        },
        show: function () {
            var mediaSelector = new MediaSelector({ onSelected: self.onSelected, element: element, name: $(element).attr('id') });
            mediaSelector.show($);
        },
        reset: function () {
            self.setValue('');
        },
        onSelected: function (data) {
            self.setValue(data.Url);
            $.fancybox.close();
        }
    };
};
MediaSelectorWrapper.defaults =
{
    noImageSelectedImage: '/Areas/Admin/Content/Images/no-media-selected.jpg',
    previewStyle: 'max-height: 120px; max-width:120px;',
    selectClasses: 'btn btn-success ',
    selectMessage: 'Select media...',
    removeClasses: 'btn btn-danger ',
    removeMessage: 'Remove media...'
};
MediaSelector.defaults =
{
    selectorUrl: '/Admin/MediaSelector/Show',
    formSelector: '#search-form',
    resultsHolderSelector: '#results',
    mediaResultSelector: '[data-media-result]',
    mediaToggleSelector: '[data-media-toggle]',
    onSelected: function (info) { }
};

(function ($) {
    $.fn.mediaSelector = function (method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            return $.error('Method ' + method + ' does not exist on mediaselector');
        }
    };
    var methods = {
        init: function (options) {
            return this.each(function () {
                var self = $(this);
                var wrapper = new MediaSelectorWrapper(self, options);
                wrapper.init();
                $.data(self, 'media-selector-wrapper', wrapper);
            });
        }
    };

})(jQuery);
(function ($) {
    $.fn.mediaSelectorPopup = function (method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            return $.error('Method ' + method + ' does not exist on mediaselector');
        }
    };
    var methods = {
        init: function (options) {
            return this.each(function () {
                var self = $(this);
                var selector = new MediaSelector(self, options);
                selector.init();
                $.data(self[0], 'media-selector', selector);
                if (parent.$) {
                    parent.$.data(self[0], 'media-selector', selector);
                }
            });
        }
    };
})(jQuery);

$(function () {
    $('[data-type=media-selector], [class=media-selector]').mediaSelector();
});