export function handlePushNotifications() {
    $(document).on('submit', '[data-push-notification-form]', submitForm);
}

function submitForm(event) {
    event.preventDefault();
    const form = $(event.target);

    $.post(form.attr('action'), form.serialize(), (response) => {
        if (response) {
            swal({
                    title: "Success!",
                    text: "The notification has been pushed!",
                    icon: "success",
                    button: "OK"

                },
                () => {
                    parent.jQuery.featherlight.close();
                });
        }
    });
}