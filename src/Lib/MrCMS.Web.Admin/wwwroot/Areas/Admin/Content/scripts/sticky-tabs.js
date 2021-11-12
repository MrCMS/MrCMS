function setActiveTab(tab) {
    const key = $(tab).data("stickytabs");
    const currentTab = store.get(key);
    if (location.hash !== '' && $(tab).find(` a[href="${location.hash}"]`).length > 0) {
        $(tab).find(` a[href="${location.hash}"]`).tab('show');
    } else if (currentTab) {
        $(tab).find(`a[href="${currentTab}"]`).tab('show');
    } else if ($(tab).find('li a[data-toggle="tab"]').length > 0 && $(tab).find('li.active a[data-toggle="tab"]').length == 0) {
        $(tab).find('a[data-toggle="tab"]').eq(0).click();
    }
}


function persistTab(e) {
    store.set($(e.currentTarget).data("stickytabs"), e.target.hash);
}

export function setupStickyTabs() {
    $("[data-stickytabs]").each(function () {
        setActiveTab(this);

        $(this).on('shown.bs.tab', function (e) {
            persistTab(e);
        });
    });
}
