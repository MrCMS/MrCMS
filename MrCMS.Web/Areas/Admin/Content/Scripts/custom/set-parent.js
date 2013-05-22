$(function() {
    $.get('/Admin/Webpage/ValidParents',{id:$('#Id')}, function (data) {
        $("#parent").tagit({
            autocomplete: { delay: 0, minLength: 0, source: data },
            availableTags: data,
            showAutocompleteOnFocus: true,
            singleField: true
        });
    });
});