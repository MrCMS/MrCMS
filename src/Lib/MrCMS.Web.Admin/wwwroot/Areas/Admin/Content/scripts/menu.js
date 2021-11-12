const pushMenuTarget = "a[data-widget='pushmenu']";
const pushMenuState = "mrcms-push-menu-state";

const mrcmsOpenMenuItems = "mrcms-admin-menu-items";

//left hand nav open/close and persistence.
function setupNav() {
    const target = "ul#sidebar-nav";

    $(target).on('expanded.lte.treeview', function () {
        logOpenMenus();
    });

    $(target).on('collapsed.lte.treeview', function () {
        logOpenMenus();
    });

    $(pushMenuTarget).on('collapsed.lte.pushmenu', function () {
        store.set(pushMenuState, 'collapsed')
    });

    $(pushMenuTarget).on('shown.lte.pushmenu', function () {
        store.set(pushMenuState, 'shown')
    });
}

function logOpenMenus() {
    const data = $(".menu-open").map(function () {
        return $(this).data('menu');
    }).get().join(",");

    store.set(mrcmsOpenMenuItems, data);
}

function openNavItems() {
    const items = store.get(mrcmsOpenMenuItems) || '';
    const keys = items.split(",");
    for (let i = 0; i < keys.length; i++) {
        $("li[data-menu='" + keys[i] + "']").addClass('menu-open');
    }
}

export function setupMenu() {
    openNavItems();
    setSideBarState();
    setupNav();
}

function setSideBarState() {
    const state = store.get(pushMenuState);
    if (state === 'collapsed' && !$("body").hasClass('sidebar-collapse') && $(window).width() > 992) {
        $(window).PushMenu('toggle')
    }
}

