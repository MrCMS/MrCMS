var WebMenu = function (options) {
    var settings = $.extend({
        tabIndexToShow: 1
    }, options);
    var self;
    return {
        init: function () {
            self = this;
            $(document.body).on('click', '.jstree-anchor', function (event) {
                location.href = this.href;
            });
            $(document).on("show", 'a[data-toggle="tab"]', function (e) {
                self.initTree($(e.target));
            });
            self.selectTab(settings.tabIndexToShow);
            return self;
        },
        selectTab: function (tab) {
            if ($('#nav-tabs').length) {
                $("#nav-tabs li:eq(" + tab + ") a").tab('show');
            }
        },
        initTree: function (link) {
            if (link.data('tree-initialized'))
                return;
            var tree = link.data('tree');
            var url = link.data('tree-url');
            $("#" + tree).jstree({
                "core": {
                    "animation": 0,
                    "check_callback": true,
                    'data': {
                        'url': url,
                        'data': function (node) {
                            return { 'id': node.id };
                        }
                    }
                },
                "state": {
                    "key": location.hostname + tree,
                },
                "contextmenu": {
                    "items": self.menuItems
                },
                "plugins": ["state", "contextmenu"]
            });
            link.data('tree-initialized', true);
        },
        menuItems: function (node) {
            var id = node.data.id;
            if (id == null)
                id = "";
            var items = {};
            if (node.data.canAddChild === "True") {
                items.addMenuItem = {
                    label: "Add",
                    action: function () { return location.href = "/Admin/" + node.data.controller + "/Add/" + id; }
                };
            }
            if (node.data.sortable === "True") {
                items.sortMenuItem = {
                    label: "Sort",
                    action: function () { return location.href = "/Admin/" + node.data.controller + "/Sort/" + node.parent; }
                };
            }
            if (node.id !== "0") {
                items.editMenuItem = {
                    label: "Edit",
                    action: function () { return location.href = "/Admin/" + node.data.controller + "/Edit/" + id; }
                };
            }
            if (node.data.haschildren !== "True") {
                items.deleteMenuItem = {
                    label: "Delete",
                    action: function () { return getRemoteModel("/Admin/" + node.data.controller + "/Delete/" + id); }
                };
            }
            return items;
        }
    };
};
var webMenu;
$(function () {
    webMenu = new WebMenu({tabIndexToShow:getTab()}).init();
});

function getTab() {
    var val = $('#controller-name').val();
    if (val !== undefined) {
        switch (val.toLowerCase()) {
            case 'mediacategory':
                return 1;
            case 'layout':
                return 2;
            case 'layoutarea':
                return 2;
            case 'webpage':
            default:
                return 0;
        }
    }
    return 0;
};