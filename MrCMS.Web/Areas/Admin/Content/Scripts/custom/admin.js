$(document).ready(function () {
    $("#loading").ajaxStart(function () {
        $(this).show();
    });
    $("#loading").ajaxStop(function () {
        $(this).hide();
    });
    
    $.ajaxSetup({ cache: false });
    
    $.validator.methods.number = function (value, element) {
        return this.optional(element) ||
            !isNaN(Globalize.parseFloat(value));
    }

    $.validator.methods.date = function (value, element) {
        return this.optional(element) ||
            !isNaN(Globalize.parseDate(value));
    }
    Globalize.culture($("#UICulture").val());
    
    $().dropdown();
    $("[rel='tooltip']").tooltip();


    var currentTab = $.cookie('selected-tab-' + location.pathname);
    if (location.hash !== '') {
        $('.inner-content a[href="' + location.hash + '"]').tab('show');
    } else if (currentTab) {
        $('.inner-content a[href="' + currentTab + '"]').tab('show');
    } else if ($('.inner-content li a[data-toggle="tab"]').length > 0 && $('.inner-content li.active a[data-toggle="tab"]').length == 0) {
        $('.inner-content a[data-toggle="tab"]').eq(0).click();
    }

    $('.inner-content a[data-toggle="tab"]').on('shown', function (e) {
        $.cookie('selected-tab-' + location.pathname, e.target.hash, { expires: 1, path: "/" });
    });

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
            padding:0
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
        padding: 0,
        height: 0,
        'onComplete': function() {
            $('#fancybox-frame').load(function() { // wait for frame to load and then gets it's height
                $(this).contents().find('form').attr('target', '_parent').css('margin', '0');
                $('#fancybox-content').height($(this).contents()[0].documentElement.scrollHeight);
                $.fancybox.center();
            });
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
    CKEDITOR.config.toolbar = 'Basic';
    CKEDITOR.replaceAll('ckedit-enabled');
    CKEDITOR.on('instanceReady', function () { $(window).resize(); });
});


