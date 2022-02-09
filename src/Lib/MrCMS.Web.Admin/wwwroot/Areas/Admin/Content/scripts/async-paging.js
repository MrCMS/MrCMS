export function setupAsyncPaging() {

    $(document).on('click', 'div[data-paging-type="async"] .pagination a[href]', getResults);
    $(document).on('click', 'div[data-paging-type="async"] button[data-action=update]', updateResults);
}

function getResults(event) {
    event.preventDefault();
    const self = $(event.target);
    $.get(this.href, function (response) {
        self.parents('div[data-paging-type="async"]').replaceWith(response);
    });
}

function updateResults(event) {
    event.preventDefault();
    const self = $(event.target);
    const data = self.parents('div[data-paging-type="async"]').find('input, select, textarea').serialize();
    $.get($(this).data('url'), data, function (response) {
        self.parents('div[data-paging-type="async"]').replaceWith(response);
    });
    return false;
}