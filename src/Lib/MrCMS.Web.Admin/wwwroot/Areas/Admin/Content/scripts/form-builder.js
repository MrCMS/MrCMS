export function setupFormBuilder() {
    $(function () {
        var element = $('.submit-form-btn');
        if (element.length > 0) {
            element.on('click', function (e) {
                if ($('form').valid()) {
                    e.preventDefault();
                    let $form = $('.form-update-form');
                    $.post($form.attr('action'),
                        $form.serialize(), function (response) {
                            if (response.success) {
                                var tab = parent.$('#form-properties-data');
                                $.get(tab.data('reload-url'),
                                    function (response) {
                                        tab.replaceWith(response);
                                        parent.$.featherlight.close();
                                    });
                            } else {
                                alert(response.message);
                            }
                        });
                }

                return false;
            });
            $('form.form-update-form input').keypress(function (e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $('.submit-form-btn').click();
                }
            });
        }
    })
}