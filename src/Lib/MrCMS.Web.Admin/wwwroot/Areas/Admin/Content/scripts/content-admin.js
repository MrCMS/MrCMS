function getBlockHolder() {
    return $('[data-content-admin-blocks]');
}

function getEditorHolder() {
    return $('[data-content-admin-editor]');
}

function loadBlocks(url) {
    const blocks = getBlockHolder();
    if (!blocks.length) {
        return;
    }
    url = url ?? blocks.data('content-admin-blocks');
    $.get(url, function (response) {
        blocks.html(response);
    })
}

function loadEditor(url) {
    const editor = getEditorHolder();
    if (!editor.length) {
        return;
    }
    if (url) {
        $.get(url, function (response) {
            editor.html(response);
            window.admin.initializePlugins();
        })
    } else {
        editor.html('');
    }
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
        alert('success');
    });

    return false;
}

export function setupContentAdmin() {
    loadBlocks();

    $(document).on('click', '[data-content-admin-block-open]', openBlock)
    $(document).on('click', '[data-content-admin-block-close]', closeBlock)
    $(document).on('click', '[data-content-admin-block-select]', selectBlock)
    $(document).on('submit', '[data-content-admin-save-editor]', saveEditor)
}