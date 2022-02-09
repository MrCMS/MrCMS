export function setupSortForm() {
    $("[data-form-sort-form] #sortable").sortable({
        update: function (event, ui) {
            $('#sortable li').each(function (index, domElement) {
                $(domElement).find('[name*="Order"]').val(index);
            });
        }
    });

    $("[data-form-sort-form] #submit").click(function (e) {
        e.preventDefault();
        const form = $('#sort-fields-form');
        $.ajax({
            data: form.serialize(),
            dataType: "json",
            url: "/Admin/Form/Sort",
            type: "POST",
            beforeSend: function () {
            },
            complete: function () {
                window.parent.location.reload(false);
            },
            success: function (result) {
            },
            error: function (result) {
                //alert("Could not sort fields");
            }
        });
    });

}