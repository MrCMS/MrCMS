﻿(function ($) {
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

            $('body').addClass('mrcms-admin-bar-on');

            return this;
        },

        enable: function () {
            $("body").addClass("editing-on");
            setEditingEnabled(true);
            $("#enable-editing").text("Inline Editing: On").addClass("mrcms-btn-warning");
            $(".layout-area").addClass('layout-area-enabled');
            $(settings.editableSelector, document).each(function (index, element) {
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
                                    if (msg.success == false) {
                                        alert(msg.message);
                                    } else {
                                        showLiveForm(el);
                                    }
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
                                    if (msg.success == false) {
                                        alert(msg.message);
                                    } else {
                                        showLiveForm(el);
                                    }
                                }
                            });
                        }
                    });
                }
            });
            //foreach widget add edit indicator
            $("div[data-widget-id]", document).each(function () {
                $(this).prepend("<div class='edit-indicator-widget' style='diaply:none;'><img src='/Areas/Admin/Content/Images/pencil.png' /></div>");
            });
            //foreach layout area add edit indicator
            $("div[data-layout-area-id]", document).each(function (element) {
                if ($(this).height() == 0) {
                    $(this).prepend("<div class='edit-indicator-layout corner'><img src='/Areas/Admin/Content/Images/layout-2.png' /></div>");
                } else {
                    $(this).prepend("<div class='edit-indicator-layout'><img src='/Areas/Admin/Content/Images/layout-1.png' /></div>");
                }

            });

            $(".edit-indicator-layout", document).fadeIn(800);
            $(".edit-indicator-widget", document).fadeIn(800);

            //create menu for widget editing
            $('.edit-indicator-widget', document).click(function () {
                var widgetId = $(this).parent().data('widget-id');
                var name = $(this).parent().data('widget-name');
                var menu = '<div class="mrcms-edit-menu mrcms-edit-widget"><h4>' + name + '</h4>' +
                    '<ul>' +
                    '<li><a id="" data-toggle="fb-modal" href="/Admin/Widget/Edit/' + widgetId + '?returnUrl=' + window.top.location + '" target="_parent" class="mrcms-btn mrcms-btn-mini mrcms-btn-primary">Edit</a></li>' +
                    '<li><a id="" data-toggle="fb-modal" href="/Admin/Widget/Delete/' + widgetId + '" target="_parent" class="mrcms-btn mrcms-btn-mini mrcms-btn-danger">Delete</a></li>' +
                    '</ul></div>';
                $(this).parent().prepend(menu);
                $(".mrcms-edit-widget", document).fadeIn(400);

                //if click outside hide the menu
                $(document).mouseup(function (e) {
                    var container = $(".mrcms-edit-widget", document);
                    if (container.has(e.target).length === 0) {
                        container.remove();
                    }
                });
            });

            //create menu for layout editing
            $('.edit-indicator-layout', document).click(function () {
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
                $(".mrcms-edit-layout-area", document).fadeIn(400);
                //if click outside hide the menu
                $(document).mouseup(function (e) {
                    var container = $(".mrcms-edit-layout-area", document);
                    if (container.has(e.target).length === 0) {
                        container.remove();
                    }
                });

                $('[data-action=post-link]').on('click', function (e) {
                    e.preventDefault();
                    var self = $(this);
                    var url = self.attr('href') || self.data('link');
                    if (url != null) {
                        post_to_url(url, {});
                    }
                });

            });
        },
        disable: function (init) {
            setEditingEnabled(false);
            $("body").removeClass("editing-on");
            $(".layout-area").removeClass('layout-area-enabled');
            $("#enable-editing").text("Inline Editing: Off").removeClass("mrcms-btn-warning");;
            //remove all edit tools
            $(".edit-indicator-layout", document).remove();
            $(".edit-indicator-widget", document).remove();
            $(".edit-widget-menu", document).remove();
            $(".edit-layout-area-menu", document).remove();

            $(settings.editableSelector, document).each(function (index, element) {
                var el = $(element);
                if (el.data("is-html") == true && !init) {
                    showLiveForm(el);
                }
            });
            $(settings.editableSelector, document).attr("contenteditable", "false");
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
        return store.get('mrcms-inline-edit') === true;
    }

    function setEditingEnabled(value) {
        return store.set('mrcms-inline-edit', value);
    }
    function stripHtml(str) {
        return jQuery('<div />', { html: str }).text();
    }


})(jQuery);
var MrCMSFeatherlightSettings = {
    type: 'iframe',
    iframeWidth: 800,
    afterOpen: function () {
        setCloseButtonPosition(this.$instance);
    },
    beforeOpen: function () {
        $(".mrcms-edit-menu", document).hide();
    },
    onResize: function () {
        if (this.autoHeight) {
            // Shrink:
            this.$content.css('height', '10px');
            // Then set to the full height:
            this.$content.css('height', this.$content.contents().find('body')[0].scrollHeight);
        }
        setCloseButtonPosition(this.$instance);
    }
}
function setCloseButtonPosition(contents) {
    var offset = contents.find(".featherlight-content").offset();
    var scrollTop = $(document).scrollTop();
    contents.find(".featherlight-close-icon").css('top', offset.top - scrollTop);
    contents.find(".featherlight-close-icon").css('right', offset.left - 20);
}

$(function () {
    $('.editable', document).mrcmsinline();
    
    var featherlightSettings = $.extend({}, MrCMSFeatherlightSettings, {
        filter: '[data-toggle="fb-modal"]'
    });
    $(document).featherlight(featherlightSettings);

    $("#unpublish-now").click(function () {
        if (window.top.location.pathname == '/') {
            if (!confirm('Are you sure you want to unpublish your home page?')) {
                return false;
            }
        }
        $.post('/admin/webpage/unpublish', { id: $('.mrcms-admin-nav-bar + #Id').val() }, function (response) {
            window.top.location.reload();
        });
        return false;
    });
    $("#publish-now").click(function () {
        $.post('/admin/webpage/publishnow', { id: $('.mrcms-admin-nav-bar + #Id').val() }, function (response) {
            window.top.location.reload();
        });
        return false;
    });

});



function post_to_url(path, params, method) {
    method = method || "post"; // Set method to post by default, if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    for (var key in params) {
        var hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", params[key].name);
        hiddenField.setAttribute("value", params[key].value);

        form.appendChild(hiddenField);
    }

    document.body.appendChild(form);
    form.submit();
}