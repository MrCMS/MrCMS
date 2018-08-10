var AddPageTemplate = function($) {
    function updateUrlGenerators(event) {
        event.preventDefault();
        $.get('/admin/pageTemplate/getUrlGeneratorOptions', { type: $(event.target).val() }, setUrlGeneratorValues);
    }
    function setUrlGeneratorValues(data) {
        var $el = $('#UrlGeneratorType');
        $el.empty(); // remove old options
        $.each(data, function (key, option) {
            $el.append($("<option></option>")
               .attr("value", option.value).text(option.text));
        });
    }

    return {
        init: function () {
            $(document).on('change', '#PageType', updateUrlGenerators);
        }
    };
}

$(function() {
    new AddPageTemplate($).init();
})