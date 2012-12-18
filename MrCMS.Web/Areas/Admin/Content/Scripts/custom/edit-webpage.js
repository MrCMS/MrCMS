$(function () {
    $("#UrlSegment").blur(function (e) {
        var that = $(this);
        that.val(that.val().trim().replace(/[\s]/g, '-').replace(/[^a-zA-Z0-9-/]/g, ''));
    });

    $("#change-url").click(function (e) {
        e.preventDefault();
        $(this).hide();
        $("#url-span").text('');
        $("#UrlSegment").show();
    });

    $('#my-form-builder').formbuilder({
        'save_url': '/Admin/Webpage/SaveForm/' + $('#Id').val(),
        'load_url': '/Admin/Webpage/GetForm/' + $('#Id').val(),
        'useJson': true,
        on_saved: function () {
            $('#my-form-builder').before($('<div class="alert alert-success"><button type="button" class="close" data-dismiss="alert">×</button>Form Saved</div>'));
        },
        get_object_id: function () {
            return $('#Id').val();
        }
    });

    $("#my-form-builder ul").sortable({ opacity: 0.6, cursor: 'move', handle: 'strong' });

    if ($.cookie('selected-layout-area')) {
        $('#accordion-layout-areas a[href="#' + $.cookie('selected-layout-area') + '"]').click();
    }

    $('#accordion-layout-areas').on('shown', function (e) {
        $.cookie('selected-layout-area', e.target.id, { expires: 1, path: location.pathname });
    });

    $('#accordion-layout-areas').on('hidden', function (e) {
        $.cookie('selected-layout-area', '', { expires: 1, path: location.pathname });
    });
});