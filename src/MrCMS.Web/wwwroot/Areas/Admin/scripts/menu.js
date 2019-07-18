var WebMenu = function (options) {
    var settings = $.extend({}, options);
    var self;

    function getId(node) {
        var id = node.data.id;
        if (id == null)
            id = "";
        return id;
    }

    function getParentId(node) {
        var id = node.data.parentId;
        if (id == null || (id === 0))
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
                            return {'id': node.id};
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
                        action: function () {
                            return location.href = "/Admin/" + node.data.controller + "/Add/" + getId(node);
                        }
                    };
                }
            },
            function (node, items) {
                if (node.data.sortable === "True") {
                    items.sortMenuItem = {
                        label: "Sort",
                        action: function () {
                            return location.href = "/Admin/" + node.data.controller + "/Sort/" + getParentId(node);
                        }
                    };
                }
            },
            function (node, items) {
                if (!isNaN(node.id)) {
                    items.editMenuItem = {
                        label: "Edit",
                        action: function () {
                            return location.href = "/Admin/" + node.data.controller + "/Edit/" + getId(node);
                        }
                    };
                }
            },
            function (node, items) {
                if (node.data.candelete === "True") {
                    items.deleteMenuItem = {
                        label: "Delete",
                        action: function () {
                            return getRemoteModel("/Admin/" + node.data.controller + "/Delete/" + getId(node));
                        }
                    };
                }
            }
        ]
    };
};
var pushMenuTarget = "a[data-widget='pushmenu']";
var pushMenuState = "mrcms-push-menu-state";

//left hand nav open/close and persistence.
(function ($) {
    var mrcmsOpenMenuItems = "mrcms-admin-menu-items";
    var target = "ul#sidebar-nav";
    
    $(target).on('expanded.lte.treeview', function() {
        logOpenMenus();
    });

    $(target).on('collapsed.lte.treeview', function() {
        logOpenMenus();
    });

    $(pushMenuTarget).on('collapsed.lte.pushmenu', function() {
        store.set(pushMenuState, 'collapsed')
    });

    $(pushMenuTarget).on('shown.lte.pushmenu', function() {
        store.set(pushMenuState, 'shown')
    });

    function logOpenMenus() {
        var data = $(".menu-open").map(function () {
            return $(this).data('menu');
        }).get().join(",");

        store.set(mrcmsOpenMenuItems, data);
    }

    function openNavItems() {
        var items = store.get(mrcmsOpenMenuItems) || '';
        var keys = items.split(",");
        for (var i = 0; i < keys.length; i++) {
            $("li[data-menu='" + keys[i] + "']").addClass('menu-open');
        }
    }

    $(document).ready(function () {
        openNavItems();
        setSideBarState();
    });
})(jQuery);

function setSideBarState(){
    var state = store.get(pushMenuState);
    if (state === 'collapsed' && !$("body").hasClass('sidebar-collapse') && $(window).width() > 992){
        $(window).PushMenu('toggle')
    }
}

