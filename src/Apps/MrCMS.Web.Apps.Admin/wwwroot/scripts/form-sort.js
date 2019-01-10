$(document).ready(function () {
    $("#sortable").sortable({
        update: function (event, ui) {
            $('#sortable li').each(function (index, domElement) {
                $(domElement).find('[name*="Order"]').val(index);
            });
        }
    });

    $("#submit").click(function (e) {
        e.preventDefault();
        var form = $('#sort-fields-form');
        $.ajax({
            data: form.serialize(),
            dataType: "json",
            url: "/Admin/Form/Sort",
            type: "POST",
            beforeSend: function () {
            },
            complete: function () {
                window.parent.location.href = window.parent.location.href;
            },
            success: function (result) {
            },
            error: function (result) {
                //alert("Could not sort fields");
            }
        });
    });
});