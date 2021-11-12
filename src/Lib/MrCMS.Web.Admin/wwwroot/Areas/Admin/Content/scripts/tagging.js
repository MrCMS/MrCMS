/// <reference path="./admin.js" />
export function initTagging() {
    $(".system-tagging").tagit({
        autocomplete: {
            delay: 250,
            minLength: 1,
            source: function (request, response) {
                $.getJSON('/Admin/Tag/Search', {
                    term: extractLast(request.term)
                }, response);
            }
        },
        showAutocompleteOnFocus: true,
        singleField: true,
        placeholderText: "Start typing to add tags.",
        allowSpaces: true
    });
}

function extractLast(term) {
    return split(term).pop();
}
function split(val) {
    return val.split(/,\s*/);
}
