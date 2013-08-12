(function ($) {
    CKEDITOR.config.allowedContent = true;
    var settings, methods = {
        init: function (options) {
            settings = {
                editableSelector: '.editable'
            };
            if (options) {
                $.extend(settings, options);
            }
            parent.CKEDITOR.disableAutoInline = true;
            parent.CKEDITOR.on('instanceCreated', function (event) {
                var editor = event.editor;
                editor.on('configLoaded', function () {
                    editor.config.toolbar = 'Basic';
                });
            });

            $("#enable-editing").click(function () {
                $(this).mrcmsinline(!getEditingEnabled() ? 'enable' : 'disable');
            });
            $(this).mrcmsinline(getEditingEnabled() ? 'enable' : 'disable', true);
            return this;
        },

        enable: function () {
            setEditingEnabled(true);
            $("#enable-editing").text("Inline Editing: On").addClass("mrcms-btn-warning");

            $(settings.editableSelector, top.document).each(function (index, element) {
                var el = $(element);
                if (el.attr('contenteditable') != 'true')
                    el.attr('contenteditable', 'true');
                if (el.data("is-html") == true) {
                    var editor = parent.CKEDITOR.inline(element);
                    var original = null;
                    editor.on('focus', function (e) {
                        $.get('/Admin/InPageAdmin/GetUnformattedBodyContent/', { id: el.data('id'), property: el.data('property'), type: el.data('type') }, function (response) {
                            e.editor.setData(response);
                            original = e.editor.getData();
                        });
                    });
                    editor.on('blur', function (e) {
                        if (original != e.editor.getData()) {
                            $.ajax({
                                type: "POST",
                                url: "/Admin/InPageAdmin/SaveBodyContent",
                                data: {
                                    id: el.data('id'),
                                    property: el.data('property'),
                                    type: el.data('type'),
                                    content: el.html()
                                },
                                success: function (msg) {
                                    showLiveForm(el);
                                }
                            });
                        } else {
                            showLiveForm(el);
                        }
                    });
                } else {
                    el.focus(function () {
                        original = el.html();
                    });
                    el.blur(function () {
                        var html = stripHtml(el.html());
                        if (original != html) {
                            $.ajax({
                                type: "POST",
                                url: "/Admin/InPageAdmin/SaveBodyContent",
                                data: {
                                    id: el.data('id'),
                                    property: el.data('property'),
                                    type: el.data('type'),
                                    content: html
                                },
                                success: function (msg) {
                                    showLiveForm(el);
                                }
                            });
                        }
                    });
                }
            });
            //foreach widget add edit indicator
            $("div[data-widget-id]", top.document).each(function () {
                $(this).prepend("<div class='edit-indicator-widget'><img src='/Areas/Admin/Content/Images/pencil.png' /></div>");
            });
            //foreach layout area add edit indicator
            $("div[data-layout-area-id]", top.document).each(function () {
                $(this).prepend("<div class='edit-indicator-layout'><img src='/Areas/Admin/Content/Images/layout-area-menu.png' /></div>");

            });

            $(".edit-indicator-layout", top.document).fadeIn(800);
            $(".edit-indicator-widget", top.document).fadeIn(800);

            //create menu for widget editing
            $('.edit-indicator-widget', top.document).click(function () {
                var widgetId = $(this).parent().data('widget-id');
                var name = $(this).parent().data('widget-name');
                var menu = '<div class="mrcms-edit-menu mrcms-edit-widget"><h4>' + name + '</h4>' +
                    '<ul>' +    
                    '<li><a id="" data-toggle="fb-modal" href="/Admin/Widget/Edit/' + widgetId + '?returnUrl=' + window.top.location + '" target="_parent" class="mrcms-btn mrcms-btn-mini mrcms-btn-primary">Edit</a></li>' +
                    '<li><a id="" data-toggle="fb-modal" href="/Admin/Widget/Delete/' + widgetId + '" target="_parent" class="mrcms-btn mrcms-btn-mini mrcms-btn-danger">Delete</a></li>' +
                    '</ul></div>';
                $(this).parent().prepend(menu);
                $(".mrcms-edit-widget", top.document).fadeIn(400);

                //if click outside hide the menu
                $(top.document).mouseup(function (e) {
                    var container = $(".mrcms-edit-widget", top.document);
                    if (container.has(e.target).length === 0) {
                        container.remove();
                    }
                });
            });

            //create menu for layout editing
            $('.edit-indicator-layout', top.document).click(function () {
                var areaId = $(this).parent().data('layout-area-id');
                var areaName = $(this).parent().data('layout-area-name');
                var customLayout = $(this).parent().data('layout-area-hascustomsort');
                var pageId = $('#Id').val();
                var resetMenu = '';
                if (customLayout == 'True')
                    resetMenu = '<li><a tab-index="3" href="/Admin/LayoutArea/ResetSorting/' + areaId + '?pageId=' + pageId + '&returnUrl=' + top.location.href + '" data-action="post-link" class="mrcms-btn mrcms-btn-mini mrcms-btn-danger">Reset custom sort</a></li>';
                
                var menu = '<div class="mrcms-edit-menu mrcms-edit-layout-area"><h4>' + areaName +
                    '</h4><ul><li><a tab-index="1" href="/Admin/Widget/Add?pageId=' + pageId + '&id=' + areaId + '" data-toggle="fb-modal" class="mrcms-btn mrcms-btn-mini mrcms-btn-primary">Add widget</a></li>' +
                    '<li><a tab-index="3" href="/Admin/LayoutArea/SortWidgets/' + areaId + '?returnUrl=' + top.location.href + '" class="mrcms-btn mrcms-btn-mini mrcms-btn-default" data-toggle="fb-modal">Sort widgets</a></li>' +
                    resetMenu +
                    '<li><a tab-index="2" href="/Admin/LayoutArea/SortWidgetsForPage/' + areaId + '?pageId=' + pageId + '&returnUrl=' + top.location.href + '" class="mrcms-btn mrcms-btn-mini mrcms-btn-default" data-toggle="fb-modal">Sort widgets for page</a></li></ul></div>';
                
                $(this).parent().prepend(menu);
                $(".mrcms-edit-layout-area", top.document).fadeIn(400);
                //if click outside hide the menu
                $(top.document).mouseup(function (e) {
                    var container = $(".mrcms-edit-layout-area", top.document);
                    if (container.has(e.target).length === 0) {
                        container.remove();
                    }
                });

            });

            //set active tab for layout
            $(document, top.document).on('click', '#mrcms-manage-page-widgets', function () {
                $.cookie('selected-tab-/Admin/Webpage/Edit/' + $('#Id').val(), '#layout-content');
            });
        },
        disable: function (init) {
            setEditingEnabled(false);
            $("#enable-editing").text("Inline Editing: Off").removeClass("mrcms-btn-warning");;
            //remove all edit tools
            $(".edit-indicator-layout", top.document).remove();
            $(".edit-indicator-widget", top.document).remove();
            $(".edit-widget-menu", top.document).remove();
            $(".edit-layout-area-menu", top.document).remove();

            $(settings.editableSelector, top.document).each(function (index, element) {
                var el = $(element);
                if (el.data("is-html") == true && !init) {
                    showLiveForm(el);
                }
            });
            $(settings.editableSelector, top.document).attr("contenteditable", "false");
            //kill all ckeditors
            for (k in parent.CKEDITOR.instances) {
                var instance = parent.CKEDITOR.instances[k];
                instance.destroy(true);
            }
        }
    };

    $.fn.mrcmsinline = function (method) {
        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.mrcms-mrcmsinline');
        }
    };

    function showLiveForm(el) {
        $.get('/Admin/InPageAdmin/GetFormattedBodyContent/', { id: el.data('id'), property: el.data('property'), type: el.data('type') }, function (response) {
            el.html(response);
            $.validator.unobtrusive.parse(el.find('form'));
        });
    };

    function getEditingEnabled() {
        return $.cookie('mrcms-inline-edit') === "true";
    }

    function setEditingEnabled(value) {
        return $.cookie('mrcms-inline-edit', value, { expires: 1 });
    }
    function stripHtml(str) {
        return jQuery('<div />', { html: str }).text();
    }
})(jQuery);

$(function () {
    $('.editable', top.document).mrcmsinline();

    $(top.document).on('click', '[data-toggle="fb-modal"]', function () {
        var clone = $(this).clone();
        clone.attr('data-toggle', '');
        clone.hide();
        $("body", top.document).append(clone);
        clone.fancybox({
            type: 'iframe',
            padding: 0,
            height: 0,
            context: top.document,
            'onComplete': function () {
                $('#fancybox-frame').load(function () { // wait for frame to load and then gets it's height
                    $(this).contents().find('form').attr('target', '_parent').css('margin', '0');
                    var documentWidth = $('html').width() - 200;
                    if (documentWidth > 1000)
                        documentWidth = 1000;
                    var width = (documentWidth);

                    $('#fancybox-content, #fancybox-wrap, #fancybox-frame').height($(top.window).height() - 100);
                    $('#fancybox-content, #fancybox-wrap, #fancybox-frame').width(width);
                    $.fancybox.center();
                });
            }
        }).click();//.remove();
        return false;
    });

    $("#unpublish-now").click(function () {
        $.post('/admin/webpage/unpublish', { id: $('#Id').val() }, function (response) {
            window.top.location.reload();
        });
        return false;
    });
    $("#publish-now").click(function () {
        $.post('/admin/webpage/publishnow', { id: $('#Id').val() }, function (response) {
            window.top.location.reload();
        });
        return false;
    });
});
