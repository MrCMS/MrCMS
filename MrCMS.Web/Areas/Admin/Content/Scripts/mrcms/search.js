(function ($) {
    var autoComplete, autoCompleteElement, itemType;
    $(function () {
        autoCompleteElement = $("[data-search-item-term]");
        autoComplete = autoCompleteElement.autocomplete(options).data("ui-autocomplete");
        if (autoComplete != null) {
            autoComplete._renderItem = renderItem;
            autoComplete._renderMenu = renderMenu;
        }
        itemType = $('[data-search-item-type]');
        itemType.on('change', function() {
            autoCompleteElement.trigger('focus');
        });
        autoCompleteElement.on('focus', function() {
            autoComplete.search($(this).val());
        });
    });

    function getUrl(item) {
        if (item.actionUrl)
            return item.actionUrl;
        return '/Admin';
    }
    function renderItem(ul, item) {
        var text = item.label;
        if (item.displayType)
            text += ' (' + item.displayType + ')';
        var actionLink = $('<a>').attr('href', item.actionUrl).html(text);
        var li = $('<li>').html(actionLink);
        return li.appendTo(ul);
    }
    function renderMenu(ul, items) {
        var that = this;
        $.each(items, function (index, item) {
            if (index > 0) {
                $('<li>').appendTo(ul);
            }
            that._renderItemData(ul, item);
        });
    }

    var options = {
        source: function (request, response) {
            console.log(request);
            var type = itemType.val();
            $.get('/Admin/UniversalSearch/QuickSearch', {
                term: request.term,
                type: type
            }, response);
        },
        minLength: 2,
        delay: 400,
        select: function (event, ui) {
            if (ui.item) {
                location.href = getUrl(ui.item);
            }
        }
    };
})(jQuery);
