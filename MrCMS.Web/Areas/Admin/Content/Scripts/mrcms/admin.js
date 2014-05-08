$.ajaxSetup({ cache: false });
$(document).ready(function () {
    $(document).ajaxStart(function () {
        $("#loading").show();
    });
    $(document).ajaxStop(function () {
        $("#loading").hide();
    });
    $().dropdown();
    $("[rel='tooltip']").tooltip();

    Globalize.culture($("#UICulture").val());
    $.validator.methods.number = function(value, element) {
        return this.optional(element) ||
            !isNaN(Globalize.parseFloat(value));
    };

    $.validator.methods.date = function(value, element) {
        return this.optional(element) ||
            !isNaN(Globalize.parseDate(value));
    };

    $(".datepicker").datepicker();

    $(document).on('click', '.date-time-picker', function () {
        var that = $(this);
        if (!that.hasClass('hasDatepicker')) {
            that.datetimepicker({
                timeFormat: 'hh:mm'
            }).blur().focus();
        }
    });

    $(document).on('click', '[data-toggle="fb-modal"]', function () {
        var clone = $(this).clone();
        clone.attr('data-toggle', '');
        clone.hide();
        clone.fancybox({
            type: 'iframe',
            autoSize: true,
            minHeight: 200,
            padding: 0,
            afterShow: function () {
                $('.fancybox-iframe').contents().find('form').attr('target', '_parent').css('margin', '0');
            }
        }).click().remove();
        return false;
    });


    $('[data-action=save]').click(function (e) {
        e.preventDefault();
        var formId = $(this).data('form-id');
        $('#' + formId).submit();
    });

    $('[data-action=post-link]').click(function (e) {
        e.preventDefault();
        var self = $(this);
        var url = self.attr('href') || self.data('link');
        if (url != null) {
            post_to_url(url, {});
        }
    });

    $(window).resize(function () {
        $('.modal').each(function (index, element) {
            resizeModal($(element));
        });
    });

    $(document).on('click', 'div[data-paging-type="async"] .pagination a[href]', function () {
        var self = $(this);
        $.get(this.href, function (response) {
            self.parents('div[data-paging-type="async"]').replaceWith(response);
        });
        return false;
    });

    $(document).on('click', 'div[data-paging-type="async"] button[data-action=update]', function () {
        var self = $(this);
        var data = self.parents('div[data-paging-type="async"]').find('input, select, textarea').serialize();
        $.get($(this).data('url'), data, function (response) {
            self.parents('div[data-paging-type="async"]').replaceWith(response);
        });
        return false;
    });

    $(document).on('click', 'a.more-link', function () {
        return false;
    });

    $(document).on('change', '#admin-site-selector', function () {
        location.href = $(this).val();
    });

});

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

function getRemoteModel(href) {
    var link = $("<a>");
    link.attr('href', href);
    link.fancybox({
        type: 'iframe',
        autoSize: true,
        minHeight: 200,
        padding: 0,
        afterShow: function () {
            $('.fancybox-iframe').contents().find('form').attr('target', '_parent').css('margin', '0');
        }
    }).click();
}

$(function () {
    admin.initializePlugins();
});
window.admin = {
    initializePlugins: function () {
        CKEDITOR.replaceAll('ckedit-enabled');
        CKEDITOR.on('instanceReady', function () { $(window).resize(); });
        $('[data-type=media-selector], [class=media-selector]').mediaSelector();
        var form = $('form');
        form.removeData("validator");
        form.removeData("unobtrusiveValidation");
        form.find('input, select').each(function () {
            $.data(this, "previousValue", null);
        });
        $.validator.unobtrusive.parse("form");
    }
};


function post_to_url(path, params, method) {
    method = method || "post"; // Set method to post by default, if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    for (var key in params) {
        var hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", params[key].name);
        hiddenField.setAttribute("value", params[key].value);

        form.appendChild(hiddenField);
    }

    document.body.appendChild(form);
    form.submit();
}
$.fn.delayKeyup = function (e, callback, ms) {
    var timer = 0;
    $(this).keyup(function (event) {
        clearTimeout(timer);
        timer = setTimeout(function () {
            callback(event);
        }, ms);
    });
    return $(this);
};