$(function () {
    $('.submit-form-btn').click(function () {
        $.post($('.form-update-form').attr('action'),
            $('.form-update-form').serialize(), function (response) {
                if (response.success) {
                    parent.$('#form-properties-tab').load('/Admin/Webpage/FormProperties/' + parent.$('#Id').val() + ' #form-data');
                    parent.$.fancybox.close();
                } else {
                    alert(response.message);
                }
            });
        return false;
    });
})