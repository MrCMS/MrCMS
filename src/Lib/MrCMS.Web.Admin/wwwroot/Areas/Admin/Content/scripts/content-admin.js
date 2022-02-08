function getBlockHolder() {
    return $('[data-content-admin-blocks]');
}

function getItemBlocks() {
    return $('[data-admin-item-blocks-order]');
}

function getEditorHolder() {
    return $('[data-content-admin-editor]');
}

function loadBlocks(url) {
    const blocks = getBlockHolder();
    if (!blocks.length) {
        return;
    }
    url = url ?? $('[data-content-admin-blocks-current]').data('content-admin-blocks-current') ?? blocks.data('content-admin-blocks');
    $.get(url, function (response) {
        blocks.html(response);
        InitSortBlocks();
        InitToggleHideBlock();
        InitSortItemBlocks();
    })
}

function InitSortBlocks() {
    const blocks = getBlockHolder();
    if (!blocks.length) {
        return;
    }
    blocks.children("ul").sortable({
        handle: ".sort-handle",
        update: function (event, ui) {
            var sortList = [];
            blocks.children("ul").children("li[data-order]").each(function (index, domElement) {
                var order = index + 1;
                $(domElement).data("order", order);
                sortList.push({
                    id: $(domElement).data("id"),
                    order: order
                });
            });
            var url = blocks.children("ul").data("admin-blocks-order");
            $.post(url, { list: sortList })
                .done(function (data) {
                    reloadPreview();
                })
                .fail(function () {
                    loadBlocks();
                });
        }
    });
}

function InitSortItemBlocks() {
    const itemBlocks = getItemBlocks();
    if (!itemBlocks.length) {
        return;
    }

    itemBlocks.each(function () {
        var itemBlock = $(this);
        itemBlock.sortable({
            handle: ".child-sort-handle",
            update: function (event, ui) {
                var sortList = [];
                var blockId = $(event.target).closest('[data-id]').data("id");
                $(event.target).children("li[data-id]").each(function (index, domElement) {
                    var order = index + 1;
                    sortList.push({
                        id: $(domElement).data("id"),
                        order: order
                    });
                });
                var url = $(event.target).data("admin-item-blocks-order");
                $.post(url, { id: blockId, list: sortList })
                    .done(function (data) {
                        reloadPreview();
                        /*loadBlocks();*/ //uncomment for re-order ui again
                    })
                    .fail(function () {
                        loadBlocks();
                    });
            }
        });
    });


}

function InitToggleHideBlock() {
    const blocks = getBlockHolder();
    if (!blocks.length) {
        return;
    }

    blocks.find("[data-content-admin-block-hide]").on("click", function () {
        var self = $(this);
        var url = self.data("content-admin-block-hide");
        $.post(url, { id: self.data("id") })
            .done(function (data) {
                loadBlocks();
                reloadPreview();
            });
    });
}

function loadEditor(url) {
    const editor = getEditorHolder();
    if (!editor.length) {
        return;
    }
    if (url) {
        editor.closest("[data-content-parent]").removeClass("open")
        $.get(url, function (response) {
            editor.html(response);
            window.admin.initializePlugins();
            editor.closest("[data-content-parent]").addClass("open");
        })
    } else {
        editor.closest("[data-content-parent]").removeClass("open")
        let blocks = getBlockHolder();
        blocks.find('li.list-group-item-primary').removeClass('list-group-item-primary');
    }
}

function hideEditor() {
    const editor = getEditorHolder();
    if (!editor.length) {
        return;
    }
    editor.closest("[data-content-parent]").removeClass("open");

    let blocks = getBlockHolder();
    blocks.find('li.list-group-item-primary').removeClass('list-group-item-primary');
}

function loadEditorFromDataKeyUrl(link, dataKey) {
    const url = link.data(dataKey);
    if (!url) {
        return false;
    }

    loadEditor(url);
    return false;
}

function loadBlocksFromDataKeyUrl(link, dataKey) {
    const url = link.data(dataKey);
    if (!url) {
        return false;
    }

    loadBlocks(url);
    return false;
}

function openBlock(event) {
    const link = $(event.currentTarget);
    return loadBlocksFromDataKeyUrl(link, 'content-admin-block-open');
}

function closeBlock(event) {
    const link = $(event.currentTarget);
    return loadBlocksFromDataKeyUrl(link, 'content-admin-block-close');
}

function selectBlock(event) {
    const link = $(event.currentTarget);
    loadEditorFromDataKeyUrl(link, 'content-admin-editor')
    loadBlocksFromDataKeyUrl(link, 'content-admin-block-select');
    return false;
}

function removeBlock(event) {
    const link = $(event.currentTarget);
    if (confirm('Are you sure you want to delete this block?')) {
        const url = link.data('content-admin-block-remove');
        $.post(url, function () {
            loadBlocks();
            loadEditor();
            reloadPreview();
        });
    }
    return false;
}

function addChild(event) {
    const link = $(event.currentTarget);
    const url = link.data('content-admin-add-child');
    $.post(url, function () {
        loadBlocks();
        loadEditor();
        reloadPreview();
    });
    return false;
}

function reloadPreview() {
    document.getElementById('content-admin-preview-pane').contentWindow.location.reload();
}

function saveEditor(event) {
    const form = $(event.currentTarget);
    const obj = form.serializeArray().reduceRight((prev, arr) => {
        prev[arr.name] = arr.value;
        return prev;
    }, {});
    const url = form.attr('action');
    $.post(url, obj, function () {
        loadBlocks();
        loadEditor();
        reloadPreview();
        /*alert('success');*/
    });

    return false;
}

export function setupContentAdmin() {
    loadBlocks();

    $(document).on('click', '[data-content-admin-block-open]', openBlock)
    $(document).on('click', '[data-content-admin-block-close]', closeBlock)
    $(document).on('click', '[data-content-admin-block-select]', selectBlock)
    $(document).on('click', '[data-content-admin-block-remove]', removeBlock)
    $(document).on('click', '[data-content-admin-add-child]', addChild)
    $(document).on('submit', '[data-content-admin-save-editor]', saveEditor)
    $(document).on('click', '[data-content-admin-hide-editor]', hideEditor)
}