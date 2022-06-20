export function setupRecaptcha() {
    window.recaptchaOnLoad = function () {
        grecaptcha.enterprise.ready(function () {
            document.querySelectorAll('[data-recaptcha-holder]').forEach(function (element) {
                let form = element.closest('form');
                let isCheckboxType = element.dataset.ischeckbox;
                if (isCheckboxType == "False") {
                    grecaptcha.enterprise.execute(element.dataset.sitekey).then(function (token) {
                        element.innerHTML = element.innerHTML + '<input type="hidden" name="g-recaptcha-response" value="' + token + '">';
                    });
                }
                else{
                    form.dataset.noRecaptcha = true;
                    form.addEventListener('submit', function (event) {
                        if (form.dataset.noRecaptcha) {
                            event.preventDefault();
                            alert('Please submit Recaptcha');
                        }
                    });
                    let button = form.querySelector('button[type="submit"], input[type="submit"]');
                    button.setAttribute('disabled', 'disabled');

                    grecaptcha.enterprise.render(element.id,
                        {
                            'sitekey': element.dataset.sitekey,
                            callback: function () {
                                button.removeAttribute('disabled');
                                form.removeAttribute('data-no-recaptcha');
                            }
                        });
                }
            });
        });
    }
}