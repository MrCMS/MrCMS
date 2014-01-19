var menu = new function () {
    this.init = function () {
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
                "items": menu.menuItems
            },
            "plugins": ["state", "contextmenu"]
        });
        
        $(document.body).on('click', '.jstree-anchor', function (event) {
            location.href = this.href;
        });
    };
    this.menuItems = function(node) {
        // The default set of all items
        var items = {
            addMenuItem: {
                label: "Add",
                action: function () { return location.href = "/Admin/Webpage/Add/" + node.id; }
            },
            editMenuItem: { 
                label: "Edit",
                action: function () { return location.href = "/Admin/Webpage/Edit/" + node.data.id; }
            },
            sortMenuItem: {
                label: "Sort",
                action: function () { return location.href = "/Admin/Webpage/Sort/" + node.parent; }
            },
            deleteMenuItem: {
            label: "Delete",
            action: function () { return getRemoteModel("/Admin/Webpage/Delete/" + node.id); }
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
    };
};
var mediaTree = new function() {
    this.init = function() {
        $("#media-tree").jstree({
            "core": {
                "animation": 0,
                "check_callback": true,
                'data': {
                    'url': '/Admin/Navigation/MediaTree',
                    'data': function(node) {
                        return { 'id': node.id };
                    }
                },
            },
            "state": {
                "key" : "media",
            },
            "contextmenu": {
                "items": mediaTree.menuItems
            },
            "plugins": ["state", "contextmenu"]
        });

        $(document.body).on('click', '.jstree-anchor', function(event) {
            location.href = this.href;
        });
    };
    this.menuItems = function (node) {
        // The default set of all items
        var items = {
            addMenuItem: {
                label: "Add",
                action: function () { return location.href = "/Admin/MediaCategory/Add/" + node.id; }
            },
            editMenuItem: {
                label: "Edit",
                action: function () { return location.href = "/Admin/MediaCategory/Edit/" + node.data.id; }
            },
            sortMenuItem: {
                label: "Sort",
                action: function () { return location.href = "/Admin/MediaCategory/Sort/" + node.parent; }
            },
            deleteMenuItem: {
                label: "Delete",
                action: function () { return getRemoteModel("/Admin/MediaCategory/Delete/" + node.id); }
            }
        };

        if (node.data.haschildren === "True") {
            delete items.deleteMenuItem;
        }

        return items;
    };

    this.destroy = function() {
        $("#media-tree").jstree('destroy');
    };
};
$(function () {
    $(document).on("shown", 'a[data-toggle="tab"]', function (e) {
        console.log('showing tab ' + e.target); // Active Tab
        console.log('showing tab ' + e.relatedTarget); // Previous Tab
        if (e.target.id === "pages-tab") {
            mediaTree.destroy();
            menu.init();
        } else if (e.target.id === "media-tab") {
            menu.destroy();
            mediaTree.init();
        }
    });
});