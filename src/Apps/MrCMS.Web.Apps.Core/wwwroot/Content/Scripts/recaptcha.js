export function setupRecaptcha() {
    window.recaptchaOnLoad = function () {
        grecaptcha.enterprise.ready(function () {
            $('[data-recaptcha-holder]').each(function () {
                var self = this;
                var $el = $(self);
                var $form = $el.closest('form');
                var isCheckboxType = $el.data("ischeckbox");
                if (isCheckboxType == "False") {
                    grecaptcha.enterprise.execute($el.data('sitekey')).then(function (token) {
                        $el.append('<input type="hidden" name="g-recaptcha-response" value="' + token + '">');
                    });
                }
                else {
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

                    grecaptcha.enterprise.render(self.id,
                        {
                            'sitekey': $el.data('sitekey'),
                            callback: function () {
                                $button.removeAttr('disabled');
                                $form.removeData('no-recaptcha');
                            }
                        });
                }
            });
        });
    }
}