import {initTree} from "./tree-setup";

export function setupLayoutTree() {
    $('[data-layout-tree]').each((index, element) => {
        initTree($(element), 'layout-tree', '/Admin/Navigation/LayoutTree');
    })
}
