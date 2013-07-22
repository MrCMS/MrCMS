$(document).ready(function () {

    $('form').validate({
        rules: {
            Password: {
                minlength: 2,
                required: true
            },
            ConfirmPassword: {
                minlength: 2,
                required: true,
                equalTo: '#Password'
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