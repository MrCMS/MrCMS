window.locale = {
    "fileupload": {
        "errors": {
            "maxFileSize": "File is too big",
            "minFileSize": "File is too small",
            "acceptFileTypes": "Filetype not allowed",
            "maxNumberOfFiles": "Max number of files exceeded",
            "uploadedBytes": "Uploaded bytes exceed file size",
            "emptyResult": "Empty file upload result"
        },
        "error": "Error",
        "start": "Start",
        "cancel": "Cancel",
        "destroy": "Delete"
    }
};

$(function () {
    'use strict';


    // Initialize the jQuery File Upload widget:
    $('#fileupload').fileupload();

    // Load existing files:
    $('#fileupload').each(function () {
        var that = this;
        $.getJSON(this.action, { v: new Date().getTime() }, function (result) {
            if (result && result.length) {
                var fileupload = $(that).fileupload('option', 'done');
                fileupload.call(that, $.Event('done'), { result: result });
            }
        });
    });

    $(document).on('click', '.seo-attributes', function () {
        var link = $(this);
        link.siblings('.seo-attributes-holder').slideToggle('500', function () {
            if (link.siblings('.seo-attributes-holder').is(":visible")) {
                link.val('Hide SEO attributes');
            } else {
                link.val('Set SEO attributes');
            }
        });
        return false;
    });

    $('#fileupload').addClass('fileupload-processing');
    $.ajax({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
        url: $('#fileupload').fileupload('option', '/Admin/File/Files/13'),
        dataType: 'json',
        context: $('#fileupload')[0]
    }).always(function () {
        $(this).removeClass('fileupload-processing');
    }).done(function (result) {
        $(this).fileupload('option', 'done')
            .call(this, $.Event('done'), { result: result });
    });

    $('#file-items-table').on('click', '.btn-seo-update', function (event) {
        event.preventDefault();
        var form = $(this).parents('.seo-update-form');
        console.log(form.find(':input').serialize());
        $.post(form.data('action'), form.find(':input').serialize(), function (response) {
            form.siblings('.seo-update-message').show().html(response).delay(4000).fadeOut();
        });
        return false;
    });
})