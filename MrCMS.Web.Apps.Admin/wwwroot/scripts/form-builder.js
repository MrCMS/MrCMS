$(function () {
    $('.submit-form-btn').on('click', function (e) {
        e.preventDefault();
        $.post($('.form-update-form').attr('action'),
            $('.form-update-form').serialize(), function (response) {
                if (response.success) {
                    parent.$('#form-properties-tab').load('/Admin/Webpage/FormProperties/' + parent.$('#Id').val() + ' #form-data');
                    parent.$.featherlight.close();
                } else {
                    alert(response.message);
                }
            });
        return false;
    });
    $('form.form-update-form input').keypress(function (e) {
        if (e.which == 13) {
            e.preventDefault();
            $('.submit-form-btn').click();
        }
    });
})