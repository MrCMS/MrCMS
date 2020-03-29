(function ($, parent$) {
    'use strict';

    $(function () {
        $(document).on('submit', '[data-push-notification-form]', submitForm);
    });
    function submitForm(event) {
        event.preventDefault();
        var form = $(event.target);

        $.post(form.attr('action'), form.serialize(), function (response) {
            if (response) {
                swal({
                        title: "Success!",
                        text: "The notification has been pushed!",
                        icon: "success",
                        button: "OK"

                    },
                    function() {
                        parent$.featherlight.close();
                    });
            }
        });
    }
})(jQuery, parent.jQuery);