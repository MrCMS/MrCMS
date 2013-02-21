(function ($) {
    var settings, methods = {
        init: function (options) {
            settings = {
                editableSelector: '.editable'
            };
            if (options) {
                $.extend(settings, options);
            }
            CKEDITOR.disableAutoInline = true;
            CKEDITOR.on('instanceCreated', function (event) {
                var editor = event.editor;
                editor.on('configLoaded', function () {
                    editor.config.toolbar = 'Basic';
                });
            });

            $("#enable-editing").click(function () {
                $(this).mrcmsinline(!getEditingEnabled() ? 'enable' : 'disable');
            });
            $(this).mrcmsinline(getEditingEnabled() ? 'enable' : 'disable');

            return this;
        },

        enable: function () {
            setEditingEnabled(true);
            $("#enable-editing").text("Inline Editing: On");

            $(settings.editableSelector).each(function (index, element) {
                var el = $(element);
                if (el.attr('contenteditable') != 'true')
                    el.attr('contenteditable', 'true');
                if (el.data("is-html") == true) {
                    var editor = CKEDITOR.inline(element);
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
                        if (original != el.html()) {
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
                        }
                    });
                }
            });
            //foreach widget add edit indicator
            $("div[data-widget-id]").each(function () {
                $(this).prepend("<div class='edit-indicator-widget'><img src='/Areas/Admin/Content/Images/pencil.png' /></div>");
            });
            //foreach layout area add edit indicator
            $("div[data-layout-area-id]").each(function () {
                $(this).prepend("<div class='edit-indicator-layout'><img src='/Areas/Admin/Content/Images/layout-area-menu.png' /></div>");

            });

            $(".edit-indicator-layout").fadeIn(800);
            $(".edit-indicator-widget").fadeIn(800);

            //create menu for widget editing
            $('.edit-indicator-widget').click(function () {
                var widgetId = $(this).parent().data('widget-id');
                var menu = '<div class="edit-widget-menu">' +
                    '<ul>' +
                    '<li><a id="" href="/Admin/Widget/Edit/' + widgetId + '" target="_parent" class="mrcms-btn mrcms-btn-mini" style="color:#333;text-decoration:none;">Edit this widget</a></li>' +
                    '<li><a id="" data-toggle="fb-modal" href="/Admin/Widget/Delete/' + widgetId + '" target="_parent" class="mrcms-btn mrcms-btn-mini" style="color:#333;text-decoration:none;">Delete this widget</a></li>' +
                    '</ul></div>';
                $(this).parent().prepend(menu);
                $(".edit-widget-menu").fadeIn(400);

                //if click outside hide the menu
                $(document).mouseup(function (e) {
                    var container = $(".edit-widget-menu");
                    if (container.has(e.target).length === 0) {
                        container.remove();
                    }
                });
            });

            //create menu for layout editing
            $('.edit-indicator-layout').click(function () {
                var areaId = $(this).parent().data('layout-area-id');
                var areaName = $(this).parent().data('layout-area-name');
                var menu = '<div class="edit-layout-area-menu"><h4>Layout area: <br /> ' + areaName +
                    '</h4><ul><li><a tab-index="1" href="/Admin/Widget/AddPageWidget?pageId=' + $('#Id').val() + '&id=' + areaId + '" data-toggle="fb-modal" class="mrcms-btn mrcms-btn-mini" style="color:#333;text-decoration:none;">Add new widget here</a></li>' +
                    '<li><a tab-index="1" href="/Admin/Webpage/Edit/' + $('#Id').val() + '" class="mrcms-btn mrcms-btn-mini" style="color:#333;text-decoration:none;">Manage page widgets</a>' +
                    '</li><li><a tab-index="1" href="/Admin/LayoutArea/Edit/' + areaId + '" class="mrcms-btn mrcms-btn-mini" style="color:#333;text-decoration:none;">Manage global widgets</a></li></ul></div>';

                $(this).parent().prepend(menu);
                $(".edit-layout-area-menu").fadeIn(400);
                //if click outside hide the menu
                $(document).mouseup(function (e) {
                    var container = $(".edit-layout-area-menu");
                    if (container.has(e.target).length === 0) {
                        container.remove();
                    }
                });
            });


            //$('.edit-indicator').click(function () {
            //    var areaId = $(this).parent().data('layout-area-id');

            //    var menu = '<div class="admin-tools edit-widget gradient-bg" style="background:#ffffff;"><ul><li><a id="" href="/Admin/Widget/Edit/' + widgetId + '" target="_parent" class="btn btn-mini" style="color:#333;text-decoration:none;">Edit this widget</a></li><li><a href="/Admin/Widget/AddPageWidget?pageId=' + $('#Id').val() + '&layoutAreaId=' + layoutAreaContainerId + '" data-toggle="modal" class="btn btn-mini" style="color:#333;text-decoration:none;">Add new widget here</a></li><li><a href="/Admin/Widget/Delete/' + widgetId + '" data-toggle="modal" class="btn btn-mini" style="color:#333;text-decoration:none;">Delete this widget</a></li></ul></div>';


            //    $(this).parent().prepend(menu);

            //    //if click outside hide the menu
            //    $(document).mouseup(function (e) {
            //        var container = $(".edit-widget");
            //        if (container.has(e.target).length === 0) {
            //            container.hide();
            //        }
            //    });
            //});
        },
        disable: function () {
            setEditingEnabled(false);
            $("#enable-editing").text("Inline Editing: Off");
            //remove all edit tools
            $(".edit-indicator-layout").remove();
            $(".edit-indicator-widget").remove();

            //kill all ckeditors
            for (k in CKEDITOR.instances) {
                var instance = CKEDITOR.instances[k];
                instance.destroy(true);
            }
            $(settings.editableSelector).attr("contenteditable", "false");
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
        });
    };

    function getEditingEnabled() {
        return $.cookie('mrcms-inline-edit') === "true";
    }

    function setEditingEnabled(value) {
        return $.cookie('mrcms-inline-edit', value, { expires: 1 });
    }
})(jQuery);

$(function () {
    $().mrcmsinline();
    $("div.edit-widget").on("focusin", function () {

    });

    $(document).on('click', '[data-toggle="fb-modal"]', function () {
        var clone = $(this).clone();
        clone.attr('data-toggle', '');
        clone.hide();
        clone.fancybox({
            type: 'iframe',
            padding: 0,
            height: 0,
            'onComplete': function () {
                $('#fancybox-frame').load(function () { // wait for frame to load and then gets it's height
                    $(this).contents().find('form').attr('target', '_parent').css('margin', '0');
                    $('#fancybox-content').height($(this).contents()[0].documentElement.scrollHeight);
                    $.fancybox.center();
                });
            }
        }).click().remove();
        return false;
    });
});
