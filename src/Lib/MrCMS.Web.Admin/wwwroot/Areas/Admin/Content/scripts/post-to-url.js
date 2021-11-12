export function postToUrl(path, params, method) {
    method = method || "post"; // Set method to post by default, if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);
    form.setAttribute("id", "temp-form");

    for (var key in params) {
        var hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", key);
        hiddenField.setAttribute("value", params[key]);

        form.appendChild(hiddenField);
    }

    let value = $('body').data('antiforgery-token');
    if (value) {
        var antiForgery = document.createElement("input");
        antiForgery.setAttribute("type", "hidden")
        antiForgery.setAttribute("name", "__RequestVerificationToken")
        antiForgery.setAttribute('value', value)
        form.appendChild(antiForgery);
    }

    document.body.appendChild(form);
    form.submit();
}

export function registerPostToUrl() {
    window.postToUrl = postToUrl;
}

export function postToUrlHelper() {
    $('[data-action=post-link]').click(function (e) {
        e.preventDefault();
        var self = $(this);
        var url = self.attr('href') || self.data('link');
        if (url != null) {
            postToUrl(url, {}, 'post');
        }
    });
}