var webMenu = new function () {
    this.init = function () {
        $(document.body).on('click', '.jstree-anchor', function (event) {
            location.href = this.href;
        });
    };
    this.initWebTree = function () {
        $("#webpage-tree").jstree({
            "core": {
                "animation": 0,
                "check_callback": true,
                'data': {
                    'url': '/Admin/Navigation/WebSiteTree',
                    'data': function (node) {
                        return { 'id': node.id };
                    }
                }
            },
            "state": {
                "key": "webpage",
            },
            "contextmenu": {
                "items": this.menuItems
            },
            "plugins": ["state", "contextmenu"]
        });

    };
    this.initMediaTree = function () {
        $("#media-tree").jstree({
            "core": {
                "animation": 0,
                "check_callback": true,
                'data': {
                    'url': '/Admin/Navigation/MediaTree',
                    'data': function (node) {
                        return { 'id': node.id };
                    }
                },
            },
            "state": {
                "key": "media",
            },
            "contextmenu": {
                "items": this.menuItems
            },
            "plugins": ["state", "contextmenu"]
        });

    };
    this.initLayoutTree = function () {
        $("#layout-tree").jstree({
            "core": {
                "animation": 0,
                "check_callback": true,
                'data': {
                    'url': '/Admin/Navigation/LayoutTree',
                    'data': function (node) {
                        return { 'id': node.id };
                    }
                },
            },
            "state": {
                "key": "layout",
            },
            "contextmenu": {
                "items": this.menuItems
            },
            "plugins": ["state", "contextmenu"]
        });
    };
    this.menuItems = function(node) {
        // The default set of all items
        var id = node.data.id;
        if (id == null)
            id = "";
        var items = {
            addMenuItem: {
                label: "Add",
                action: function () { return location.href = "/Admin/"+node.data.controller+"/Add/" + id; }
            },
            editMenuItem: { 
                label: "Edit",
                action: function () { return location.href = "/Admin/" + node.data.controller + "/Edit/" + id; }
            },
            sortMenuItem: {
                label: "Sort",
                action: function () { return location.href = "/Admin/" + node.data.controller + "/Sort/" + node.parent; }
            },
            deleteMenuItem: {
            label: "Delete",
            action: function () { return getRemoteModel("/Admin/" + node.data.controller + "/Delete/" + id); }
            }
        };

        if (node.data.canAddChild != "True") {
            delete items.addMenuItem;
        }

        if (node.data.sortable != "True") {
            delete items.sortMenuItem;
        }
        
        if (node.id === "0") {
            delete items.editMenuItem;
        }
        
        if (node.data.haschildren === "True") {
            delete items.deleteMenuItem;
        }

        return items;
    };
    this.destroy = function() {
        $("#webpage-tree").jstree('destroy');
        $("#layout-tree").jstree('destroy');
        $("#media-tree").jstree('destroy');
    };
};
$(function () {
    webMenu.init();
    //webMenu.initWebTree();
    $(document).on("show", 'a[data-toggle="tab"]', function (e) {
        if (e.target.id === "pages-tab") {
            webMenu.destroy();
            webMenu.initWebTree();
        } else if (e.target.id === "media-tab") {
            webMenu.destroy();
            webMenu.initMediaTree();
        } else if (e.target.id === "layout-tab") {
            webMenu.destroy();
            webMenu.initLayoutTree();
        }
    });
});