var recaptchaOnLoad = function () {
    $('.g-recaptcha').each(function (index, element) {
        var $el = $(element);
        var $form = $el.closest('form');
        $form.data('no-recaptcha', true);
        $form.on('submit',
            function (event) {
                if ($form.data('no-recaptcha')) {
                    event.preventDefault();
                    alert('Please submit Recaptcha');
                }
            });
        var $button = $form.find('button[type="submit"], input[type="submit"]');
        $button.attr('disabled', 'disabled');

        grecaptcha.render(element.id,
            {
                'sitekey': $el.data('sitekey'),
                callback: function () {
                    $button.removeAttr('disabled');
                    $form.removeData('no-recaptcha');
                }
            });
    });
};