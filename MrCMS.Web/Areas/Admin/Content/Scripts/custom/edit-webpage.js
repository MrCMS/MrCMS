$(function () {
    $("#UrlSegment").blur(function (e) {
        var that = $(this);
        that.val(that.val().trim().replace(/[^a-zA-Z0-9-/]/g, '-'));
    });

    $('#publish-status-change').click(function (e) {
        e.preventDefault();
        var form = $("#edit-document");
        if ($("#PublishOn").val().length > 0) {
            $("#PublishOn").val('');
            form.submit();
        } else {
            $.get('/Admin/Webpage/GetServerDate', function(response) {
                $("#PublishOn").val(response);
                form.submit();
            });
        }
        return false;
    });

    //Show box for chaging the URL which is hidden by default.
    $("#change-url").click(function (e) {
        e.preventDefault();
        $(this).hide();
        $("#url-span").text('');
        $("#UrlSegment").show();
    });

    if ($('#my-form-builder').formbuilder) {
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
    }

    $("#my-form-builder ul").sortable({ opacity: 0.6, cursor: 'move', handle: 'strong' });

    $('#accordion-layout-areas').on('shown', function (e) {
        $.cookie('selected-layout-area-' + location.pathname, e.target.id, { expires: 1 });
    });

    $('#accordion-layout-areas').on('hidden', function (e) {
        $.cookie('selected-layout-area-' + location.pathname, '', { expires: 1 });
    });

    
    
    $('#PublishOn').change(function () {
        $('#publish-on-hidden').val($(this).val());
    });

    // lazy load iframe preview when tab is selected
    // needs to be before shown from hash/cookie
    $('.main-content a[data-toggle="tab"]').on('shown', function (e) {
        var tab = e.target.toString();
        if (tab.length > 0) {
            if (tab.indexOf("preview") !== -1) {
                $("#previewIframe").attr('src', '/' + $('#LiveUrlSegment').val());
            }
        }
    });

    //Permissions
    $.get('/Admin/Role/GetRolesForPermissions', function(data) {
        $("#FrontEndRoles").tagit({
            autocomplete: { delay: 0, minLength: 0, source: data },
            availableTags: data,
            showAutocompleteOnFocus: true,
            singleField: true,
            placeholderText: "Click to add role."
        });
    });
    
    $("#MetaKeywords").tagit({
        tagLimit: 15,
        allowSpaces: true
    });
});
//Show the accordion which was last shown for layout areas.
$(window).load(function () {
    if ($.cookie('selected-layout-area-' + location.pathname)) {
        $('#accordion-layout-areas a[href="#' + $.cookie('selected-layout-area-' + location.pathname) + '"]').click();
    }
});
