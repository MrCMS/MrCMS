import {getRemoteModel} from "./setup-featherlight";


function getId(node) {
    let id = node.data.id;
    if (id == null)
        id = "";
    return id;
}

function getParentId(node) {
    let id = node.data.parentId;
    if (id == null || (id === 0))
        id = "";
    return id;
}

function menuItems(node) {
    const items = {};
    for (let i = 0; i < menuRules.length; i++) {
        menuRules[i](node, items);
    }
    return items;
}

window.menuRules = [
    (node, items) => {
        if (node.data.canAddChild === "True") {
            items.addMenuItem = {
                label: "Add",
                action: () => {
                    return location.href = "/Admin/" + node.data.controller + "/Add/" + getId(node);
                }
            };
        }
    },
    (node, items) => {
        if (node.data.sortable === "True") {
            items.sortMenuItem = {
                label: "Sort",
                action: () => {
                    return location.href = "/Admin/" + node.data.controller + "/Sort/" + getParentId(node);
                }
            };
        }
    },
    (node, items) => {
        if (!isNaN(node.id)) {
            items.editMenuItem = {
                label: "Edit",
                action: () => {
                    return location.href = "/Admin/" + node.data.controller + "/Edit/" + getId(node);
                }
            };
        }
    },
    (node, items) => {
        if (node.data.candelete === "True") {
            items.deleteMenuItem = {
                label: "Delete",
                action: () => {
                    return getRemoteModel("/Admin/" + node.data.controller + "/Delete/" + getId(node));
                }
            };
        }
    }
]

export function initTree(element, key, url) {
    element.jstree({
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
            "key": `${location.hostname}.${key}`
        },
        "contextmenu": {
            "items": menuItems
        },
        "plugins": ["state", "contextmenu"]
    });
    element.on('click', '.jstree-anchor', function (event) {
        const clicked = $(event.target);
        const href = this.href;
        if (clicked.data('toggle') == 'fb-modal') {
            getRemoteModel(href);
        } else {
            location.href = href;
        }
    });
}
