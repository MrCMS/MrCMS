function MediaSelector(options) {
    var element,
        settings = jQuery.extend(MediaSelector.defaults, options),
        self,
        timer,
        mediaUploader,
        $;

    function toggle(link) {
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
    function hideResult(result) {
        var shown = result.data('content-shown');
        if (shown) {
            result.find(settings.mediaResultSelector).slideUp();
            result.find(settings.mediaToggleSelector).html('Show');
            deselectAll(result);
            result.data('content-shown', false);
        }
    };
    function submitForm(event) {
        event.preventDefault();
        var form = element.find(settings.formSelector);
        var url = form.attr('action') + '?' + form.serialize();
        updateResults(url);
    };
    function changePage(event) {
        event.preventDefault();
        var url = $(this).attr('href');
        updateResults(url);
    };
    function updateResults(url) {
        $.get(url, function (response) {
            var newResults = $(response).find(settings.resultsHolderSelector);
            $(element).find(settings.resultsHolderSelector).replaceWith(newResults);
            initializeMediaUploader();
        });
    };
    function selectFile(event) {
        var file = $(this).data('file');
        element.find('[data-action="select"][data-file=' + file + ']').removeAttr('disabled');
    };
    function deselectAll(result) {
        result.find('input[data-file]').removeAttr('checked');
        result.find('[data-action="select"]').attr('disabled', 'disabled');
    };
    function selected(event) {
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

    function initializeMediaUploader() {
        mediaUploader = new MediaUploader(element, {
            onFileUploadStopped: function (event, dropzone) {
                var form = element.find(settings.formSelector);
                var url = form.attr('action') + '?' + form.serialize();
                updateResults(url);
                dropzone.removeAllFiles();
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
            link.featherlight({
                type: 'iframe',
                iframeWidth: 820,
                afterOpen: function () {
                    element = $('.featherlight-inner').contents();
                    element.find('form').css('margin', '0');
                    self.init();
                },
                beforeOpen: function () {
                    $(".mrcms-edit-menu", document).hide();
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

function MediaSelectorWrapper(el, options) {
    var element = el,
        settings = $.extend(MediaSelectorWrapper.defaults, options),
        self,
        preview,
        removeButton,
        selectButton,
        buttonHolder,
        eventsRegistered = false;

    function getValue() {
        return element.val();
    }

    function valueIsSet() {
        var value = getValue();
        return value != null && value != '';
    }

    function getAltInput(value) {
        var altFormGroup = $('<div>').addClass('form-group has-feedback hide').attr('id', el.attr('id') + '-alt-holder');
        var altId = el.attr('id') + '-alt';
        var altLabel = $('<label>').html('Alt').attr('for', altId);
        altFormGroup.append(altLabel);
        var altInput = $('<input>').attr('type', 'text').addClass('form-control input-sm').attr('id', altId).attr('data-url', value);
        altFormGroup.append(altInput);
        var altFeedback = $('<span class="glyphicon glyphicon-ok form-control-feedback hide" aria-hidden="true"></span>');
        altFormGroup.append(altFeedback);
        $.get(settings.altUrl, { url: value }, function (response) {
            altInput.val(response.alt);
            altFormGroup.removeClass('hide');
            altInput.on('blur', updateAlt);
        });
        return altFormGroup;
    }

    function showSaved(inputElement) {
        var formGroup = inputElement.closest('.form-group');
        formGroup.addClass('has-success');
        formGroup.find('.form-control-feedback').removeClass('hide');
        clearTimeout(inputElement.removeSuccess);
        inputElement.removeSuccess = setTimeout(function () {
            formGroup.removeClass('has-success');
            formGroup.find('.form-control-feedback').addClass('hide');
        }, 2000);
    }

    function updateAlt(event) {
        var input = $(event.target);
        $.post(settings.updateAltUrl, { url: input.data('url'), value: input.val() }, function (response) {
            if (response) {
                showSaved(input);
            }
        });
    }
    function updateDescription(event) {
        var input = $(event.target);
        $.post(settings.updateDescriptionUrl, { url: input.data('url'), value: input.val() }, function (response) {
            if (response) {
                showSaved(input);
            }
        });
    }

    function getDescriptionInput(value) {
        var descriptionFormGroup = $('<div>').addClass('form-group has-feedback hide').attr('id', el.attr('id') + '-description-holder');
        var descriptionId = el.attr('id') + '-description';
        var descriptionLabel = $('<label>').html('Description').attr('for', descriptionId);
        descriptionFormGroup.append(descriptionLabel);
        var descriptionInput = $('<textarea>').attr('rows','4').addClass('form-control input-sm').attr('id', descriptionId).attr('data-url', value);
        descriptionFormGroup.append(descriptionInput);

        var descriptionFeedback = $('<span class="glyphicon glyphicon-ok form-control-feedback hide" aria-hidden="true"></span>');
        descriptionFormGroup.append(descriptionFeedback);
        $.get(settings.descriptionUrl, { url: value }, function (response) {
            descriptionInput.val(response.description);
            descriptionFormGroup.removeClass('hide');
            descriptionInput.on('blur', updateDescription);
        });
        return descriptionFormGroup;
    }

    function getPreview() {
        if (valueIsSet()) {
            var value = getValue();
            if (isImage(value)) {
                var holder = $('<div>').addClass('row').css('max-width', '500px');
                var imageCol = $('<div>').addClass('col-sm-4');
                imageCol.append($('<img src="' + value + '" style="' + settings.previewStyle + '" />'));
                holder.append(imageCol);
                var dataCol = $('<div>').addClass('col-sm-8');

                var altFormGroup = getAltInput(value);
                dataCol.append(altFormGroup);

                var descriptionFormGroup = getDescriptionInput(value);
                dataCol.append(descriptionFormGroup);

                holder.append(dataCol);
                return holder;
            } else {
                return $('<span>' + value + '</span>');
            }
        } else {
            return $('<img src="' + settings.noImageSelectedImage + '" style="' + settings.previewStyle + '" />');
        }
    };
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
            $.featherlight.close();
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
    removeMessage: 'Remove media...',
    altUrl: '/admin/mediaselector/alt',
    updateAltUrl: '/admin/mediaselector/updatealt',
    descriptionUrl: '/admin/mediaselector/description',
    updateDescriptionUrl: '/admin/mediaselector/updatedescription'
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
                if (self.hasClass("media-selector-initialized")) {
                    return;
                }
                var wrapper = new MediaSelectorWrapper(self, options);
                wrapper.init();
                self.addClass("media-selector-initialized");
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
