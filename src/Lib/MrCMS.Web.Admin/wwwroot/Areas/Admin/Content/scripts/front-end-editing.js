let layoutAreas;
let documentBody;
let editableSelector;
let enableEditingBtn;
let editLayoutIndicators;
let editWidgetIndicators;
let editWidgetMenuIndicators;
let editLayoutMenuIndicators;

document.addEventListener("DOMContentLoaded",function(){
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
    enableEditingBtn.addEventListener("click", function () {
        if (!getEditingEnabled()) {
            enableEditors();
        } else {
            disableEditing();
        }
    });

    if (getEditingEnabled() === true)
        enableEditors();

    documentBody.classList.add('mrcms-admin-bar-on');
}

function enableEditors() {
    documentBody.classList.add("editing-on");
    setEditingEnabled(true);
    enableEditingBtn.innerText = "Inline: On";
    enableEditingBtn.classList.add("mrcms-btn-warning");

    layoutAreas.forEach(x => x.classList.add('layout-area-enabled'));

    let editors = document.querySelectorAll(editableSelector);
    editors.forEach(x => {
        let original;
        const el = x;
        if (el.getAttribute('contenteditable') !== 'true')
            el.setAttribute('contenteditable', 'true');

        if (el.dataset.isHtml) {
            const editor = parent.CKEDITOR.inline(el);
            original = null;

            editor.on('focus', function (e) {
                fetch('/Admin/InPageAdmin/GetUnformattedContent/?' + new URLSearchParams({
                    id: el.dataset.id,
                    property: el.dataset.property,
                    type: el.dataset.type
                })).then(response => response.text()).then(data => {
                    e.editor.setData(data);
                    original = e.editor.getData();
                })
            });

            editor.on('blur', function (e) {
                if (original !== e.editor.getData()) {
                    const data = {
                        id: el.dataset.id,
                        entityProperty: el.dataset.property,
                        entityType: el.dataset.type,
                        content: el.innerHTML
                    }
                    saveInlineEdit(data, el)
                } else {
                    showLiveForm(el);
                }
            });
        } else {
            el.focus(function () {
                original = el.innerHTML
            });
            el.blur(function () {
                const html = stripHtml(el.innerHTML);
                if (original !== html) {
                    const data = {
                        id: el.dataset.id,
                        entityProperty: el.dataset.property,
                        entityType: el.dataset.type,
                        content: html
                    }
                    saveInlineEdit(data, el)
                }
            });
        }
    })
    
    function saveInlineEdit(data, el){
        fetch('/Admin/InPageAdmin/SaveContent', {
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

                    setTimeout( function(){
                        msgEl.style.display = 'none';
                    }, 500)
                }
            })
            .catch((error) => {
                console.error('Error:', error);
            });
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

    editWidgetIndicators = document.querySelectorAll('.edit-indicator-widget');
    editWidgetMenuIndicators = document.querySelectorAll('.edit-widget-menu');
    editLayoutMenuIndicators = document.querySelectorAll('.edit-layout-area-menu');
    editLayoutIndicators = document.querySelectorAll('.edit-indicator-layout');
    editWidgetIndicators.forEach(x => x.style.display = 'block');
    editLayoutIndicators.forEach(x => x.style.display = 'block');

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

function disableEditing() {
    setEditingEnabled(false);
    documentBody.classList.remove("editing-on");
    layoutAreas.forEach(x => x.classList.remove('layout-area-enabled'));
    enableEditingBtn.innerText = "Inline: Off";
    enableEditingBtn.classList.remove("mrcms-btn-warning");
    editLayoutIndicators.forEach(x => x.remove());
    editWidgetIndicators.forEach(x => x.remove());
    editWidgetMenuIndicators.forEach(x => x.remove());
    editLayoutMenuIndicators.forEach(x => x.remove());
    document.querySelectorAll(editableSelector).forEach(x => {
        if (x.dataset.isHtml === true) {
            showLiveForm(x);
        }
    });
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

function showLiveForm(el) {
    $.get('/Admin/InPageAdmin/GetFormattedContent/', {
        id: el.dataset.id,
        property: el.dataset.property,
        type: el.dataset.type
    }, function (response) {
        el.innerHTML = response;
        let forms = el.querySelectorAll('form');
        $.validator.unobtrusive.parse(forms[0]);
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

/*
function post_to_url(path, params, method) {
    method = method || "post"; // Set method to post by default, if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    const form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    for (let key in params) {
        const hiddenField = document.createElement("input");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", params[key].name);
        hiddenField.setAttribute("value", params[key].value);

        form.appendChild(hiddenField);
    }

    document.body.appendChild(form);
    form.submit();
}*/
