export function setupSiteSearch() {
    (function ($) {
        const options = {
            source: function (request, response) {
                const type = itemType.val();
                $.get('/Admin/TextSearch/QuickSearch', {
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
        let autoComplete, autoCompleteElement, itemType;
        $(function () {
            autoCompleteElement = $("[data-search-item-term]");
            autoComplete = autoCompleteElement.autocomplete(options).data("ui-autocomplete");
            if (autoComplete != null) {
                autoComplete._renderItem = renderItem;
                autoComplete._renderMenu = renderMenu;
            }
            itemType = $('[data-search-item-type]');
            itemType.on('change', function () {
                autoCompleteElement.trigger('focus');
            });
            autoCompleteElement.on('focus', function () {
                autoComplete.search($(this).val());
            });
        });

        function getUrl(item) {
            if (item.actionUrl)
                return item.actionUrl;
            return '/Admin';
        }

        function renderItem(ul, item) {
            let text = item.label;
            if (item.displayType)
                text += ' (' + item.displayType + ')';
            const actionLink = $('<a>').attr('href', item.actionUrl).html(text);
            const li = $('<li>').html(actionLink);
            return li.appendTo(ul);
        }

        function renderMenu(ul, items) {
            const that = this;
            $.each(items, function (index, item) {
                if (index > 0) {
                    $('<li>').appendTo(ul);
                }
                that._renderItemData(ul, item);
            });
        }

    })(jQuery);
}
