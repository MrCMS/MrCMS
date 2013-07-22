
$(document).ready(function () {

    $('form').validate({
        rules: {
            Password: {
                minlength: 2,
                required: true
            },
            Email: {
                required: true,
                email: true,
            }
        },
        highlight: function (label) {
            $(label).closest('.control-group').addClass('error');
        },
        success: function (label) {
            label
                .addClass('valid')
                .closest('.control-group').addClass('success');
        }
    });

});