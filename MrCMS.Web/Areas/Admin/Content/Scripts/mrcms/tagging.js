/// <reference path="../admin.js" />
$(function() {
    $(".system-tagging").tagit({
        autocomplete: {
            delay: 0,
            minLength: 0,
            source: function (request, response) {
                $.getJSON('/Admin/Tag/Search', {
                    term: extractLast(request.term),
                    id: $('#Id').val()
                }, response);
            }
        },
        showAutocompleteOnFocus: true,
        singleField: true,
        placeholderText: "Start typing to add tags.",
        allowSpaces: true
    });
});
