$(document).ready(function () {
    $("#loading").ajaxStart(function () {
        $(this).show();
    });
    $("#loading").ajaxStop(function () {
        $(this).hide();
    });
    $.ajaxSetup({ cache: false });

    $.validator.methods.number = function(value, element) {
        return this.optional(element) ||
            !isNaN(Globalize.parseFloat(value));
    };

    $.validator.methods.date = function(value, element) {
        return this.optional(element) ||
            !isNaN(Globalize.parseDate(value));
    };
    Globalize.culture($("#UICulture").val());
    
    $().dropdown();
    $("[rel='tooltip']").tooltip();
    
    $(".datepicker").datepicker();

    $(document).on('click', '.date-time-picker', function () {
        var that = $(this);
        if (!that.hasClass('hasDatepicker')) {
            that.datetimepicker({
                timeFormat: 'hh:mm'
            }).blur().focus();
        }
    });

    if ($().mediaselector) {
        $('.media-selector').mediaselector();
    }
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
                $(".fancybox-iframe").contents().find('form').attr('target', '_parent').css('margin', '0');
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

    $(document).on('change', '#admin-site-selector', function () {
        location.href = $(this).val();
    });

});

// used in menu for delete
function getRemoteModel(href) {
    var link = $("<a>");
    link.attr('href', href);
    link.fancybox({
        type: 'iframe',
        autoSize: true,
        minHeight: 200,
        padding: 0,
        afterShow: function () {
            $(".fancybox-iframe").contents().find('form').attr('target', '_parent').css('margin', '0');
        }
    }).click();
}

//general functions
function setCookie(name, value) {
    $.cookie(name, value, { expires: 7, path: '/' });
}

function getCookie(cookieName) {
    return $.cookie(cookieName);
}

function split(val) {
    return val.split(/,\s*/);
}

function extractLast(term) {
    return split(term).pop();
}


$(function () {
    CKEDITOR.replaceAll('ckedit-enabled');
    CKEDITOR.on('instanceReady', function () { $(window).resize(); });
});


