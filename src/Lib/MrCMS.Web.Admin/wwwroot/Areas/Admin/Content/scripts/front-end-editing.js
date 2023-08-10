let layoutAreas;
let documentBody;
let editableSelector;
let enableEditingBtn;
let editLayoutIndicators;
let editWidgetIndicators;
let editWidgetMenuIndicators;
let editLayoutMenuIndicators;
let editContentBlockIndicators;

document.addEventListener("DOMContentLoaded", function () {
    initEditing()
});

function initEditing() {
    layoutAreas = document.querySelectorAll(".layout-area");
    documentBody = document.getElementsByTagName("body")[0];
    editableSelector = ".editable";
    enableEditingBtn = document.getElementById("enable-editing");
    CKEDITOR.config.allowedContent = true;
    parent.CKEDITOR.disableAutoInline = true;
    parent.CKEDITOR.on('instanceCreated', function (event) {
        const editor = event.editor;
        editor.on('configLoaded', function () {
            editor.config.toolbar = 'Basic';
        });
    });

    if (enableEditingBtn) {
        enableEditingBtn?.addEventListener("click", async function () {
            if (!getEditingEnabled()) {
                enableEditors();
            } else {
                await disableEditing();
            }
        });
    }

    if (getEditingEnabled() === true)
        enableEditors();

    documentBody.classList.add('mrcms-admin-bar-on');
}


function preventPropagation(e) {
    e.preventDefault();
    e.stopPropagation();
}

async function saveResourceWhenChanged(e) {
    const el = e.target;
    const original = el.dataset.original;
    const html = stripLeadingAndTrailingBrTags(el.innerHTML);
    if (original !== html) {
        const data = {
            key: el.dataset.key,
            value: el.innerHTML?.trim() ?? ''
        }
        await saveResourceEdit(data, el);
        el.dataset.original = html;
    }
}

async function saveContentWhenChanged(e) {
    const el = e.target;
    const original = el.dataset.original;
    const html = stripLeadingAndTrailingBrTags(el.innerHTML);
    if (original !== html) {
        const data = {
            id: el.dataset.id,
            entityProperty: el.dataset.property,
            entityType: el.dataset.type,
            content: html?.trim() ?? ''
        }
        await saveInlineEdit(data, el)
        el.dataType.original = html;
    }
}

async function saveHtmlResource(e) {
    const el = e.editor.element.$;
    let html = stripLeadingAndTrailingBrTags(e.editor.getData());
    if (el.dataset.original !== html) {
        const data = {
            key: el.dataset.key,
            value: html?.trim() ?? ''
        }
        await saveResourceEdit(data, el)
        el.dataset.original = html
    } else {
        await showLiveResource(el);
    }
}


async function saveHtmlContent(e) {
    const el = e.editor.element.$;
    let html = stripLeadingAndTrailingBrTags(e.editor.getData());
    if (el.dataset.original !== html) {
        const data = {
            id: el.dataset.id,
            entityProperty: el.dataset.property,
            entityType: el.dataset.type,
            content: html?.trim() ?? ''
        }
        await saveInlineEdit(data, el)
        el.data.original = html;
    } else {
        await showLiveForm(el);
    }
}


async function loadHtmlResource(e) {
    const el = e.editor.element.$;

    await fetch('/Admin/InPageAdmin/GetUnformattedStringResource/?' + new URLSearchParams({
        key: el.dataset.key,
    })).then(response => response.text()).then(data => {
        e.editor.setData(data);
        el.dataset.original = e.editor.getData();
    })
}

async function loadHtmlContent(e) {
    console.log({e});
    const el = e.editor.element.$;
    await fetch('/Admin/InPageAdmin/GetUnformattedContent/?' + new URLSearchParams({
        id: el.dataset.id,
        property: el.dataset.property,
        type: el.dataset.type
    })).then(response => response.text()).then(data => {
        e.editor.setData(data);
        el.dataset.original = e.editor.getData();
    })
}

async function saveInlineEdit(data, el) {
    await fetch('/Admin/InPageAdmin/SaveContent', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
        .then(response => response.json())
        .then(data => {
            let msgEl = document.getElementById("save-content-msg");
            if (data.success === false) {
                msgEl.innerText = "An error occured."
                alert(data.message);
            } else {
                showLiveForm(el);
                msgEl.innerText = "Saved."
                msgEl.style.display = 'inline-block';

                setTimeout(function () {
                    msgEl.style.display = 'none';
                }, 500)
            }
        })
        .catch((error) => {
            console.error('Error:', error);
        });
}

function enableEditors() {
    documentBody.classList.add("editing-on");
    setEditingEnabled(true);

    if (enableEditingBtn) {
        enableEditingBtn.innerText = "Inline: On";
        enableEditingBtn.classList.add("mrcms-btn-warning");
    }

    layoutAreas.forEach(x => x.classList.add('layout-area-enabled'));

    let editors = document.querySelectorAll(editableSelector);
    for (const x of editors) {
        const el = x;
        if (el.getAttribute('contenteditable') !== 'true')
            el.setAttribute('contenteditable', 'true');

        //check if type is string-resource
        if (el.dataset.type === "string-resource") {
            if (el.dataset.isHtml === 'true') {
                const editor = parent.CKEDITOR.inline(el);
                editor.on('focus', loadHtmlResource);


                editor.on('blur', saveHtmlResource);

            } else {
                el.dataset.original = el.innerHTML;

                el.removeEventListener('click', preventPropagation);
                el.addEventListener("click", preventPropagation);

                el.removeEventListener('blur', saveResourceWhenChanged);
                el.addEventListener("blur", saveResourceWhenChanged);

            }

        } else {
            if (el.dataset.isHtml === 'true') {
                const editor = parent.CKEDITOR.inline(el);
                editor.on('focus', loadHtmlContent);
                editor.on('blur', saveHtmlContent);

            } else {
                el.dataset.original = el.innerHTML

                el.removeEventListener('click', preventPropagation);
                el.addEventListener("click", preventPropagation);

                el.removeEventListener('blur', saveContentWhenChanged);
                el.addEventListener("blur", saveContentWhenChanged);

            }
        }
    }


    //foreach widget add edit indicator
    document.querySelectorAll("div[data-widget-id]").forEach(x => {
        let html = "<div class='edit-indicator-widget' style='diaply:none;'><img src='/Areas/Admin/Content/img/pencil.png' /></div>";
        x.insertAdjacentHTML('afterbegin', html);
    })
    //foreach layout area add edit indicator
    document.querySelectorAll("div[data-layout-area-id]").forEach(x => {
        let html = "";
        if (x.offsetHeight === 0) {
            html = "<div class='edit-indicator-layout corner'><img src='/Areas/Admin/Content/img/layout-2.png' /></div>";
        } else {
            html = "<div class='edit-indicator-layout'><img src='/Areas/Admin/Content/img/layout-1.png' /></div>";
        }
        x.insertAdjacentHTML('afterbegin', html);
    })

    document.querySelectorAll("div[data-content-block-id]").forEach(x => {
        let html = "<div class='edit-indicator-content-block' style='position: absolute;cursor: pointer;z-index: 9999;'><img src='/Areas/Admin/Content/img/pencil.png' /></div>";
        x.insertAdjacentHTML('afterbegin', html);
    })

    editWidgetIndicators = document.querySelectorAll('.edit-indicator-widget');
    editWidgetMenuIndicators = document.querySelectorAll('.edit-widget-menu');
    editLayoutMenuIndicators = document.querySelectorAll('.edit-layout-area-menu');
    editLayoutIndicators = document.querySelectorAll('.edit-indicator-layout');
    editContentBlockIndicators = document.querySelectorAll('.edit-indicator-content-block');
    editWidgetIndicators.forEach(x => x.style.display = 'block');
    editLayoutIndicators.forEach(x => x.style.display = 'block');

    editContentBlockIndicators.forEach(x => x.addEventListener('click', function (event) {
        let blockId = x.parentElement.dataset.contentBlockId;
        let menuSelector = "[data-content-block-menu='" + blockId + "']";
        let menu = document.querySelectorAll(menuSelector)[0];
        menu.style.display = 'block';
        event.stopPropagation();
        document.addEventListener('click', function (e) {
            if (menu && !menu.contains(e.target)) {
                menu.style.display = 'none';
            }
        });
    }));

    //create menu for widget editing
    editWidgetIndicators.forEach(x => x.addEventListener('click', function (x) {
        let el = x.target.parentElement.parentElement;
        const widgetId = el.dataset.widgetId;
        const name = el.dataset.widgetName;
        const menu = `<div class="mrcms-edit-menu mrcms-edit-widget"><h4>${name}</h4><ul><li><a id="" href="/Admin/Widget/Edit/${widgetId}?returnUrl=${window.top.location}" target="_parent" class="mrcms-btn mrcms-btn-mini mrcms-btn-primary">Edit</a></li><li><a id="" href="/Admin/Widget/Delete/${widgetId}" target="_parent" class="mrcms-btn mrcms-btn-mini mrcms-btn-danger">Delete</a></li></ul></div>`;

        el.parentElement.insertAdjacentHTML('afterbegin', menu);
        let menuElement = document.querySelectorAll('.mrcms-edit-widget')[0];

        menuElement.style.display = 'block';
        x.stopPropagation();

        document.addEventListener('click', function (e) {
            if (menuElement && !menuElement.contains(e.target)) {
                menuElement.remove();
            }
        });
    }));

    editLayoutIndicators.forEach(x => x.addEventListener('click', function (x) {
        let el = x.target.parentElement.parentElement;
        const areaId = el.dataset.layoutAreaId;
        const areaName = el.dataset.layoutAreaName;
        const pageId = document.querySelectorAll("[data-webpage-id]")[0].dataset.webpageId;
        const menu = `<div class="mrcms-edit-menu mrcms-edit-layout-area"><h4>${areaName}</h4><ul><li><a tab-index="1" href="/Admin/Widget/Add?pageId=${pageId}&id=${areaId}" data-toggle="fb-modal" class="mrcms-btn mrcms-btn-mini mrcms-btn-primary">Add widget</a></li><li><a tab-index="3" href="/Admin/LayoutArea/SortWidgets/${areaId}?returnUrl=${top.location.href}" class="mrcms-btn mrcms-btn-mini mrcms-btn-secondary" data-toggle="fb-modal">Sort widgets</a></li></ul></div>`;
        el.insertAdjacentHTML('afterbegin', menu);
        let menuElement = document.querySelectorAll('.mrcms-edit-layout-area')[0];
        menuElement.style.display = 'block';
        x.stopPropagation();
        document.addEventListener('click', function (e) {
            if (menuElement && !menuElement.contains(e.target)) {
                menuElement.remove();
            }
        });
        let postLinkElement = document.querySelectorAll("[data-action=post-link]")[0];
        if (postLinkElement) {
            postLinkElement.addEventListener('click', function (e) {
                e.stopPropagation();
                const self = $(this);
                const url = self.attr('href') || self.data('link');
                if (url != null) {
                    window.postToUrl(url, {});
                }
            });
        }
    }));
}

async function disableEditing() {
    setEditingEnabled(false);
    documentBody.classList.remove("editing-on");
    layoutAreas.forEach(x => x.classList.remove('layout-area-enabled'));

    if (enableEditingBtn) {
        enableEditingBtn.innerText = "Inline: Off";
        enableEditingBtn.classList.remove("mrcms-btn-warning");
    }

    editLayoutIndicators.forEach(x => x.remove());
    editWidgetIndicators.forEach(x => x.remove());
    editWidgetMenuIndicators.forEach(x => x.remove());
    editLayoutMenuIndicators.forEach(x => x.remove());
    editContentBlockIndicators.forEach(x => x.remove());
    for (const x of document.querySelectorAll(editableSelector)) {
        if (x.dataset.isHtml === 'true') {
            if (x.dataset.type === "string-resource") {
                await showLiveResource(x);
            } else {
                await showLiveForm(x);
            }
        }
    }
    document.querySelectorAll(editableSelector).forEach(e => e.setAttribute("contenteditable", "false"))
    killCkeditors()
}

function killCkeditors() {
    //kill all ckeditors
    let instances = parent.CKEDITOR.instances;
    if (instances) {
        for (let k in instances) {
            const instance = parent.CKEDITOR.instances[k];
            instance.destroy(true);
        }
    }
}

async function showLiveForm(el) {
    let url = '/Admin/InPageAdmin/GetFormattedContent';
    url += "?";
    let data = {
        id: el.dataset.id,
        property: el.dataset.property,
        type: el.dataset.type
    };
    for (let k in data) {
        url += k + "=" + data[k] + "&";
    }
    url = encodeURI(url.slice(0, -1));
    await fetch(url).then(function (response) {
        return response.text();
    }).then(function (response) {
        el.innerHTML = response;
        //let forms = el.querySelectorAll('form');
        // $.validator.unobtrusive.parse(forms[0]);            $ is not defined

    }).catch(function (err) {
        // There was an error
        console.warn('Something went wrong.', err);
    });
}

async function showLiveResource(el) {

    let parameters = [];

    if (el.dataset.replacements) {
        let parameterJson = JSON.parse(el.dataset.replacements);
        for (const [key, value] of Object.entries(parameterJson)) {
            parameters.push({
                Key: key,
                Value: value
            });
        }
    }

    let url = '/Admin/InPageAdmin/GetFormattedStringResource?' + new URLSearchParams({
        key: el.dataset.key
    });

    url = encodeURI(url);
    await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(parameters),
    }).then(function (response) {
        return response.text();
    }).then(function (response) {
        el.innerHTML = response;
        //let forms = el.querySelectorAll('form');
        // $.validator.unobtrusive.parse(forms[0]);            $ is not defined

    }).catch(function (err) {
        // There was an error
        console.warn('Something went wrong.', err);
    });
}

function getEditingEnabled() {
    return localStorage.getItem('mrcms-inline-edit') === 'true';
}

function setEditingEnabled(value) {
    localStorage.setItem('mrcms-inline-edit', value);
}

function stripHtml(str) {
    return str.replace(/(<([^>]+)>)/gi, "");
}

function stripLeadingAndTrailingBrTags(str) {
    return str?.replace(/^\s*(?:<br\s*\/?\s*>)+|(?:<br\s*\/?\s*>)+\s*$/g, '');
}

async function saveResourceEdit(data, el) {
    await fetch('/Admin/InPageAdmin/SaveStringResource', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
    })
        .then(response => response.json())
        .then(async data => {
            let msgEl = document.getElementById("save-content-msg");
            if (data.success === false) {
                msgEl.innerText = "An error occured."
                alert(data.message);
            } else {
                await showLiveResource(el);
                msgEl.innerText = "Saved."
                msgEl.style.display = 'inline-block';

                setTimeout(function () {
                    msgEl.style.display = 'none';
                }, 500)
            }
        })
        .catch((error) => {
            console.error('Error:', error);
        });
}
