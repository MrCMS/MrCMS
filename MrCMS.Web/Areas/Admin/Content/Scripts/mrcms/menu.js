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
        init: function () {
            self = this;
            $(document.body).on('click', '.jstree-anchor', function (event) {
                location.href = this.href;
            });
            $(document).on("shown.bs.tab", 'a[data-toggle="tab"]', function (e) {
                self.initTree($(e.target));
            });
            return self;
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
            link.data('tree-initialized', true);
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
var webMenu = new WebMenu().init();