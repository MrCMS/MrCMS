$(function () {
    function getUrl(item) {
        if (item.actionUrl)
            return item.actionUrl;
        return '/Admin';
    }
    var autoComplete = $("#term").autocomplete({
        source: "/Admin/UniversalSearch/QuickSearch",
        minLength: 2,
        delay: 400,
        select: function (event, ui) {
            if (ui.item) {
                location.href = getUrl(ui.item);
            }
        }
    }).data("ui-autocomplete");
    if (autoComplete != null) {
        autoComplete._renderItem = function (ul, item) {
            var text = item.label;
            if (item.displayType)
                text += ' (' + item.displayType + ')';
            var actionLink = $('<a>').addClass('').attr('href', item.actionUrl).html(text);
            var li = $('<li>').html(actionLink);
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
    }
});