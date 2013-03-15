(function($) {
    $.fn.postback = function(options) {

        var settings = {
            'notSelector': '.non-postback',
            'doPost': true
        };

        if (options) {
            $.extend(settings, options);
        }
        var allElements = $(this);
        return this.not(settings.notSelector).each(function() {
            $(this).change(function() {
                var data;
                if (settings.doPost) {
                    data = allElements.serializeArray();
                    data = jQuery.grep(data, function(object) {
                        return object.value != null && object.value != "";
                    });
                    post_to_url(window.location.pathname, data);
                } else {
                    data = allElements.serialize();
                    window.location = window.location.pathname + "?" + data;
                    return false;
                }
            });
        });
    };
})(jQuery);


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


function post_to_url_ajax(path, params, method, callback) {
    method = method || "post"; // Set method to post by default, if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);
    form.setAttribute("id", "ajax-form-post");

    for (var key in params) {
        var hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", params[key].name);
        hiddenField.setAttribute("value", params[key].value);

        form.appendChild(hiddenField);
    }

    document.body.appendChild(form);
    var serialize = $('#ajax-form-post').serialize();
    $.post(path, serialize, function(response) {
        $('#ajax-form-post').remove();
        callback();
    });
}

function post_json_to_url(path, params, method) {
    method = method || "post"; // Set method to post by default, if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    for (var key in params) {
        var hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", key);
        hiddenField.setAttribute("value", params[key]);

        form.appendChild(hiddenField);
    }

    document.body.appendChild(form);
    form.submit();
}