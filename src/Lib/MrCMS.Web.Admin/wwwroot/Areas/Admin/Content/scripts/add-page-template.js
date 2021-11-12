function updateUrlGenerators(event) {
    event.preventDefault();
    $.get('/admin/pageTemplate/getUrlGeneratorOptions', { type: $(event.target).val() }, setUrlGeneratorValues);
}
function setUrlGeneratorValues(data) {
    const $el = $('[data-add-template-url-generator-type]');
    $el.empty(); // remove old options
    $.each(data, function (key, option) {
        $el.append($("<option></option>")
            .attr("value", option.value).text(option.text));
    });
}
export function setupAddPageTemplate(){
    $(document).on('change', '[data-add-template-page-type]', updateUrlGenerators);
}

