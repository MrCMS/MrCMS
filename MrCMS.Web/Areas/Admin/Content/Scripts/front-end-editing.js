window.onload = init;

function init() {
    setMenuState();
    checkSetEditMode();
}

function checkSetEditMode() {
    if (getCookieValue() == "true") {
        $("#enable-editing").text("Editing: On");
    } else {
        $("#enable-editing").text("Editing: Off");
    }
    inlineEditing();
}

var showLiveForm = function (el) {
    $.get('/Admin/InPageAdmin/GetFormattedBodyContent/', { id: el.data('id'), property: el.data('property'), type: el.data('type') }, function (response) {
        el.html(response);
    });
};
$(document).ready(function () {
    $("#enable-editing").click(function () {
        if (getCookieValue() == "true") {
            setCookieValue("false");
        } else {
            setCookieValue("true");
        }
        checkSetEditMode();
    });
});


function getCookieValue() {
    return $.cookie('inline_edit');
}

function setCookieValue(value) {
    return $.cookie('inline_edit', value, { expires: 7, path: location.pathname });
}


function inlineEditing() {
    var editable = $(".editable");
    if (getCookieValue() == "true") {
        editable.each(function (index, element) {
            var el = $(element);
            if (el.attr('contenteditable') != 'true')
                el.attr('contenteditable', 'true');
            var isHtml = el.data("is-html") == true;
            if (isHtml)
                CKEDITOR.inline(element);
            var original = null;

            el.focus(function () {
                if (isHtml) {
                    $.get('/Admin/InPageAdmin/GetUnformattedBodyContent/', { id: el.data('id'), property: el.data('property'), type: el.data('type') }, function (response) {
                        el.html(response);
                        original = response;
                    });
                } else {
                    original = el.html();
                }
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
                            if (isHtml)
                                showLiveForm(el);
                        }
                    });
                } else {
                    if (isHtml)
                        showLiveForm(el);
                }
            });
        });
        //foreach widget add the HTML code
        $("div[data-widget-id]").each(function () {
            $(this).prepend("<div class='edit-indicator'><img src='/Areas/Admin/Content/Images/pencil.png' /></div>");
        });

        $('.edit-indicator').click(function () {
            //what is the layout area?
            var layoutAreaContainerId = $(this).parentsUntil(".layout-area").parent().data('layout-area-id');
            //the widget?
            var widgetId = $(this).parent().data('widget-id');

            var menu = '<div class="admin-tools edit-widget gradient-bg" style="background:#ffffff;"><ul><li><a id="" href="/Admin/Widget/Edit/' + widgetId + '" target="_parent" class="btn btn-mini" style="color:#333;text-decoration:none;">Edit this widget</a></li><li><a href="/Admin/Widget/AddPageWidget?pageId=' + $('#Id').val() + '&layoutAreaId=' + layoutAreaContainerId + '" data-toggle="modal" class="btn btn-mini" style="color:#333;text-decoration:none;">Add new widget here</a></li><li><a href="/Admin/Widget/Delete/' + widgetId + '" data-toggle="modal" class="btn btn-mini" style="color:#333;text-decoration:none;">Delete this widget</a></li></ul></div>';

            $(this).parent().prepend(menu.replace('#widget-id#', widgetId).replace('#layoutid#', layoutAreaContainerId));

            //if click outside hide the menu
            $(document).mouseup(function (e) {
                var container = $(".edit-widget");
                if (container.has(e.target).length === 0) {
                    container.hide();
                }
            });
        });


    } else {
        //remove all edit tools
        $(".edit-indicator").remove();

        //kill all ckeditors
        for (k in CKEDITOR.instances) {
            var instance = CKEDITOR.instances[k];
            instance.destroy(true);
        }
        editable.attr("contenteditable", "false");
    }
}

//sliding menu

function getMenuWidth() {
    return parseInt($("#admin-edit-menu-container").width());
}

$(".menu-handle").click(function () {
    if (getCookie('menu-state') == "in") {
        setCookie('menu-state', 'out');
    } else {
        setCookie('menu-state', 'in');
    }

    setMenuState();
});

function setMenuState() {
    var menu = $("#admin-edit-menu-container");
    var leftPos = -102;
    if (getCookie('menu-state') == "in") {
        $(menu).animate({ left: leftPos, }, 500, function () { });
        $(".menu-handle").attr('src', "/Areas/Admin/Content/Images/button-right.png");
    } else {
        $(menu).animate({ left: '0', }, 500, function () { });
        $(".menu-handle").attr('src', "/Areas/Admin/Content/Images/button-left.png");
    }
}

CKEDITOR.on('instanceCreated', function (event) {
    var editor = event.editor,
        element = editor.element;
    editor.on('configLoaded', function () {
        editor.config.toolbar = 'Basic';
    });
});

//general functions
function setCookie(name, value) {
    $.cookie(name, value, { expires: 7, path: '/' });
}

function getCookie(cookieName) {
    return $.cookie(cookieName);
}