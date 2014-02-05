$(function () {
    $('#show-recursive').hide();
    $('input[name=AddType]').change(function () {
        if ($("input[name=AddType]:checked").val() == 'page') {
            $('#show-recursive').show();
        } else {
            $('#show-recursive').hide();
        }
    });

    $("#add-widget-form").submit(function () {
        $.post('/Admin/Widget/Add', $(this).serialize(), function (response) {
            var link = $('<a class="btn btn-mini" id="temp-edit" data-toggle="fb-modal" href="/Admin/Widget/Edit/' + response + '?returnUrl='+window.top.location+'">Edit Widget</a>');
            parent.parent.$("body").append(link);
            parent.parent.$("#temp-edit").click().remove();
        });
        return false;
    });
});