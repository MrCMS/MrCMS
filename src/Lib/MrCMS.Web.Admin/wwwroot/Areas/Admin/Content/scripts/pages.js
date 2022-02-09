import {initTree} from "./tree-setup";


export function setupWebpageTree() {
    $('[data-webpage-tree]').each((index, element) => {
        initTree($(element), 'webpage-tree', '/Admin/Navigation/WebSiteTree');
    })
}