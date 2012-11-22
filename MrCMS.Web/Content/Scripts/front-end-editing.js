window.onload = init();

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

var showLiveForm = function () {
    $.get('/Admin/Webpage/GetFormattedBodyContent/' + $('#Id').val(), function (response) {
        if (contentObj != null) {
            contentObj.html(response);
        }
    });
};
$("#enable-editing").click(function () {
    if (getCookieValue() == "true") {
        setCookieValue("false");
        showLiveForm();
    } else {
        setCookieValue("true");
    }
    checkSetEditMode();

    //hack until we find out how to recreate ckeditor after killing it
    //location.href = location.href;
});

function getCookieValue() {
    return $.cookie('inline_edit');
}

function setCookieValue(value) {
    return $.cookie('inline_edit', value, { expires: 7, path: '/' });
}

var contentObj;
var contents;

function inlineEditing() {
    if (getCookieValue() == "true") {
        if ($(".editable").length > 0) {
            $(".editable").attr('contenteditable', 'true');
            CKEDITOR.inline('test');

            CKEDITOR.on('instanceCreated', function (event) {
                var editor = event.editor,
                    element = editor.element;
                editor.on('configLoaded', function () {
                    editor.config.toolbar = 'Basic';
                });
            });

            $('.editable').focus(function () {
                var that = $(this);
                contentObj = $('.editable');
                $.get('/Admin/Webpage/GetUnformattedBodyContent/' + $('#Id').val(), function (response) {
                    that.html(response);
                    contents = contentObj.html();
                });
            });
            $('.editable').blur(function () {
                if (contents != $(this).html()) {
                    contents = $(this).html();
                    var data = {
                        id: $('#Id').val(),
                        content: contents
                    };
                    $.ajax({
                        type: "POST",
                        url: "/AdminTools/SaveBodyContent",
                        data: data,
                        success: function (msg) {
                            showLiveForm();
                        }
                    });
                } else {
                    showLiveForm();
                }
            });
        }
        //foreach widget add the HTML code
        $("div[data-widget-id]").each(function () {
            $(this).prepend("<div class='edit-indicator'><img src='/Areas/Admin/Content/Images/pencil.png' /></div>");
        });

        $('.edit-indicator').click(function () {
            //what is the layout area?
            //$("li.item-a").parentsUntil(".level-1")
            var layoutAreaContainerId = $(this).parentsUntil(".layout-area").parent().data('layout-area-id');
            //the widget?
            var widgetId = $(this).parent().data('widget-id');

            var menu = '<div class="admin-tools edit-widget gradient-bg"><ul><li><a id="" href="/Admin/Widget/Edit/' + widgetId + '" target="_parent" class="btn btn-mini">Edit this widget</a></li><li><a href="/Admin/Widget/AddPageWidget?pageId=' + $('#Id').val() + '&layoutAreaId=' + layoutAreaContainerId + '" data-toggle="modal" class="btn btn-mini">Add new widget here</a></li><li><a href="/Admin/Widget/Delete/' + widgetId + '" data-toggle="modal" class="btn btn-mini">Delete this widget</a></li></ul></div>';

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
            instance.destroy();
        }
        $(".editable").attr("contenteditable", "false");
        //$().removeAttr("cke_editable");
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