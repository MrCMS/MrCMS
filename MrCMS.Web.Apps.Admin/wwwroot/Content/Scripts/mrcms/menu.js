var WebMenu = function (options) {
    var settings = $.extend({
    }, options);
    var self;
    function getId(node) {
        var id = node.data.id;
        if (id == null)
            id = "";
        return id;
    }
    return {
        init: function (tree, url) {
            self = this;
            $(document.body).on('click', '.jstree-anchor', function (event) {
                var clicked = $(event.target);
                var href = this.href;
                if (clicked.data('toggle') == 'fb-modal') {
                    getRemoteModel(href);
                } else {
                    location.href = href;
                }
            });
            self.initTree(tree, url);
            return self;
        },
        initTree: function (tree, url) {
            $("#" + tree).jstree({
                "core": {
                    "animation": 0,
                    "check_callback": true,
                    'data': {
                        'url': url,
                        'data': function (node) {
                            return { 'id': node.id };
                        },
                        cache: false
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
        },
        menuItems: function (node) {
            var items = {};
            for (var i = 0; i < self.rules.length; i++) {
                self.rules[i](node, items);
            }
            return items;
        },
        rules: [
            function (node, items) {
                if (node.data.canAddChild === "True") {
                    items.addMenuItem = {
                        label: "Add",
                        action: function () { return location.href = "/Admin/" + node.data.controller + "/Add/" + getId(node); }
                    };
                }
            },
            function (node, items) {
                if (node.data.sortable === "True") {
                    items.sortMenuItem = {
                        label: "Sort",
                        action: function () { return location.href = "/Admin/" + node.data.controller + "/Sort/" + node.parent; }
                    };
                }
            },
            function (node, items) {
                if (!isNaN(node.id)) {
                    items.editMenuItem = {
                        label: "Edit",
                        action: function () { return location.href = "/Admin/" + node.data.controller + "/Edit/" + getId(node); }
                    };
                }
            },
            function (node, items) {
                if (node.data.candelete === "True") {
                    items.deleteMenuItem = {
                        label: "Delete",
                        action: function () { return getRemoteModel("/Admin/" + node.data.controller + "/Delete/" + getId(node)); }
                    };
                }
            }
        ]
    };
};
//left hand nav open/close and persistence.
(function ($) {
    var mrcmsOpenMenuItems = "mrcms-open-menu-items";
    function storeOpenTabs() {
        var data = $("li[data-menu].open").map(function () {
            return $(this).data('menu');
        }).get().join(",");
        store.set(mrcmsOpenMenuItems, data);
    }
    function openNavItems() {
        var items = store.get(mrcmsOpenMenuItems) || '';
        var keys = items.split(",");
        for (var i = 0; i < keys.length; i++) {
            $("li[data-menu='" + keys[i] + "']").addClass("open");
        }
    }
    $(document).ready(function () {
        $('li[data-menu] > [data-toggle=dropdown-mrcms]').on('click', function (event) {
            event.preventDefault();
            event.stopPropagation();
            $(this).parent().siblings().removeClass('open');
            $(this).parent().toggleClass('open');
            storeOpenTabs();
        });
        openNavItems();
    });
})(jQuery);