//LiveSearch
//var globalTimeout = null;
//$('#term').keyup(function (e) {
//    var tb = this;
//    if (globalTimeout != null) clearTimeout(globalTimeout);
//    setTimeout(function () { callLiveSearch(tb); }, 250);
//});

//function callLiveSearch(textbox) {
//    globalTimeout = null;
//    $.getJSON('/Admin/Search/GetSearchResults?term=' + textbox.value, function (data1) { insertCallback(data1); });
//}

//function insertCallback(data1) {
//    $("#liveSearchResultsContainer").remove();
//    if (data1.length > 0) {
//        var results = '<ul id=\'livesearchresults\'>';
//        $.each(data1, function (index, value) {
//            var doctype = "WebPage";

//            results = results + "<li>" +
//                "<a href='/Admin/" + doctype + "/Edit/" + value.Id + "'>" + value.Name +
//                "</a><br /><span class='grey font-small'>Created: " + value.CreatedOn + "</span></li>";
//        });
//        var position = $("#term").offset();
//        $('body').append("<div id='liveSearchResultsContainer' style='top:42px; left:" + position.left + "px'>" + results + " </ul></div>");
//        registerHideEvent();
//    }
//}

//function registerHideEvent() {
//    window.jQuery(document.body).click(function (event) {
//        var clicked = window.jQuery(event.target);
//        if (!(clicked.is('#livesearchresults') || clicked.parents('#livesearchresults').length || clicked.is('input'))) {
//            $("#liveSearchResultsContainer").fadeOut('slow').remove();
//        }
//    });
//}
$(function () {
    function getUrl(item) {
        if (item.editUrl) {
            return item.editUrl;
        } else if (item.viewUrl) {
            return item.viewUrl;
        }
        return '/admin';
    }

    var autoComplete = $("#term").autocomplete({
        source: "/Admin/UniversalSearch/QuickSearch",
        minLength: 2,
        select: function (event, ui) {
            if (ui.item) {
                location.href = getUrl(ui.item);
            }
        }
    }).data("ui-autocomplete");
    autoComplete._renderItem = function (ul, item) {
        var text = item.label;
        if (item.displayType)
            text += ' (' + item.displayType + ')';
        var li = $('<li>').html(text);
        var clearFix = $('<div>').addClass('clearfix').appendTo(li);
        var btnGroup = $('<div>').addClass('btn-group').appendTo(li);
        if (item.editUrl) {
            var editLink = $('<a>').addClass('btn btn-xs btn-default').attr('href', item.editUrl).html('Edit');
            btnGroup.append(editLink);
        }
        if (item.viewUrl) {
            var viewLink = $('<a>').addClass('btn btn-xs btn-default').attr('href', item.viewUrl).html('View');
            btnGroup.append(viewLink);
        }
        return li.appendTo(ul);
    };
    autoComplete._renderMenu = function (ul, items) {
        var that = this;
        $.each(items, function (index, item) {
            if (index > 0) {
                $('<li>').appendTo(ul);
            }
            that._renderItemData(ul, item);
        });
    };
});