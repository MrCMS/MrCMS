function initialize(el) {
    el.select2({
        placeholder: "Search for a webpage",
        minimumInputLength: 1,
        ajax: { // instead of writing the function to execute the request we use Select2's convenient helper
            url: "/Admin/Webpage/Select2Search",
            dataType: 'json',
            data: function (term, page) {
                return {
                    term: term,
                    page: page
                };
            },
            results: function (data, page) {
                const more = (page * 10) < data.total;
                return {results: data.items, more: more}
            }
        },
        initSelection: function (element, callback) {
            const $el = $(element);
            const id = $el.val();
            const name = $el.data('webpage-search-select2-name');
            if (id && name) {
                callback({id: id, text: name});
            }
        },
        formatResult: formatResult,
        formatSelection: formatSelection,
        escapeMarkup: function (m) {
            return m;
        }
    });
}

function formatResult(item) {
    return `${item.text} (${item.id})`;
}

function formatSelection(item) {
    return `${item.text} (${item.id})`;
}

export function setupWebpageSelect2() {
    $('[data-webpage-search-select2]').each(function (index, el) {
        initialize($(el));
    })
}