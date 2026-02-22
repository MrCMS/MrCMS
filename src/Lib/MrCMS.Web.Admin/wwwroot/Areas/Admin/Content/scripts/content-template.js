export function initiateContentTemplate() {
    function cleanup() {
        $(document).off('input.ContentTemplate', '[data-content-template-container] input');
        $(document).off('input.ContentTemplate', '[data-content-template-container] select');
        $(document).off('input.ContentTemplate', '[data-content-template-container] textarea');

        Object.values(CKEDITOR.instances).forEach(instance => instance.destroy(true));
    }

    function handleAddRepeatableTokensClick(e) {
        // Handle add repeatable item button click
        if (e.target.closest('.add-repeatable-item')) {
            const btn = e.target.closest('.add-repeatable-item');
            const repeatableToken = btn.closest('.repeatable-token');
            const inputArea = repeatableToken.querySelector('.repeatable-input-area');
            const container = repeatableToken.querySelector('.repeatable-items-container');

            // Store CKEditor content and destroy instances before cloning
            const editorContents = {};
            inputArea.querySelectorAll('textarea.enable-editor').forEach(textarea => {
                if (typeof CKEDITOR !== 'undefined' && CKEDITOR.instances[textarea.id]) {
                    editorContents[textarea.id] = CKEDITOR.instances[textarea.id].getData();
                    CKEDITOR.instances[textarea.id].destroy(true);
                }
            });

            // Create new item based on input area values (now without CKEditor elements)
            const newItemContent = inputArea.cloneNode(true);
            const itemIndex = container.querySelectorAll('.repeatable-item').length;
            const repeatableName = repeatableToken.dataset.repeatableName;

            // Update IDs and names for the new item
            newItemContent.querySelectorAll('[id]').forEach(el => {
                const fieldName = el.dataset.fieldName;
                if (fieldName) {
                    // Update ID
                    el.id = `${repeatableName}_${itemIndex}_${fieldName}`;
                    el.setAttribute('name', `${repeatableName}[${itemIndex}].${fieldName}`);
                }
            });

            // Create repeatable item container
            const repeatableItem = document.createElement('div');
            repeatableItem.className = 'repeatable-item';
            repeatableItem.dataset.index = itemIndex;

            const itemContent = document.createElement('div');
            itemContent.className = 'repeatable-item-content';

            const removeButton = document.createElement('button');
            removeButton.type = 'button';
            removeButton.className = 'btn btn-danger btn-sm remove-repeatable-item';
            removeButton.innerHTML = '<i class="fa fa-trash"></i>';

            const hr = document.createElement('hr');
            hr.className = 'my-3';

            // Add content to container
            Array.from(newItemContent.children).forEach(child => {
                itemContent.appendChild(child.cloneNode(true));
            });

            repeatableItem.appendChild(itemContent);
            repeatableItem.appendChild(removeButton);
            repeatableItem.appendChild(hr);
            container.appendChild(repeatableItem);

            // Transfer CKEditor content to new textareas and initialize them
            repeatableItem.querySelectorAll('textarea.enable-editor').forEach(textarea => {
                const originalId = textarea.id.replace(`${repeatableName}_${itemIndex}_`, '');
                const sourceId = Object.keys(editorContents).find(id => id.endsWith(originalId));

                if (sourceId && editorContents[sourceId]) {
                    textarea.value = editorContents[sourceId];
                    if (typeof CKEDITOR !== 'undefined') {
                        CKEDITOR.replace(textarea.id);
                    }
                }
            });

            // Reinitialize CKEditor instances in the input area (with empty content)
            inputArea.querySelectorAll('textarea.enable-editor').forEach(textarea => {
                if (typeof CKEDITOR !== 'undefined') {
                    CKEDITOR.replace(textarea.id);
                }
            });

            // Clear input area
            inputArea.querySelectorAll('input[type="text"], input[type="number"], input[type="email"], textarea:not(.enable-editor)').forEach(el => {
                el.value = '';
            });

            inputArea.querySelectorAll('input[type="checkbox"], input[type="radio"]').forEach(el => {
                el.checked = false;
            });

            inputArea.querySelectorAll('select').forEach(el => {
                el.value = '';
                el.selectedIndex = 0;
            });

            inputArea.querySelectorAll('img').forEach(el => {
                el.src = '/Areas/Admin/Content/img/no-media-selected.jpg';
            });

            handleInputChange(e);
        }
    }

    function handleRemoveRepeatableTokensClick(e) {
        const item = e.target.closest('.repeatable-item');
        const container = item.closest('.repeatable-items-container');

        item.remove();

        // Reindex remaining items
        Array.from(container.querySelectorAll('.repeatable-item')).forEach((repeatableItem, index) => {
            repeatableItem.dataset.index = index;

            // Update field names and IDs
            repeatableItem.querySelectorAll('[name]').forEach(field => {
                const fieldName = field.dataset.fieldName;
                if (!fieldName) return;

                const repeatableToken = field.closest('.repeatable-token');
                if (!repeatableToken) return;

                const repeatableName = repeatableToken.dataset.repeatableName;

                field.setAttribute('name', `${repeatableName}[${index}].${fieldName}`);
                field.id = `${repeatableName}_${index}_${fieldName}`;
            });
        });

        handleInputChange({target: container});
    }

    function serializeContentTemplateForm(container) {
        const formData = {};
        const repeatableData = {};

        // Select form elements
        const elements = [
            ...(container.querySelectorAll('[data-field-name]:not([name])') || []),
            ...(container.querySelectorAll('[name]:not([name="__RequestVerificationToken"])') || [])
        ];

        // Collect form data
        elements.forEach(el => {
            const name = el.getAttribute('name');
            let value;

            // Handle CKEditor instances
            if (el.tagName.toLowerCase() === 'textarea' &&
                typeof CKEDITOR !== 'undefined' &&
                CKEDITOR.instances[el.id]) {
                value = CKEDITOR.instances[el.id].getData();
            } else {
                value = el.type === 'checkbox' ? el.checked : el.value;
            }

            // Process repeatable fields
            if (el.closest('.repeatable-token')) {
                const repeatableArea = el.closest('.repeatable-input-area');
                if (repeatableArea) {
                    return;
                }

                const closestRepeatableToken = el.closest('.repeatable-token');
                const closestRepeatableItem = el.closest('.repeatable-item');

                if (!closestRepeatableToken || !closestRepeatableItem) return;

                if (el.dataset.fieldName) {
                    const repeatableName = closestRepeatableToken.dataset.repeatableName;
                    const index = closestRepeatableItem.dataset.index;
                    const fieldName = el.dataset.fieldName;

                    repeatableData[repeatableName] = repeatableData[repeatableName] || {};
                    repeatableData[repeatableName][index] = repeatableData[repeatableName][index] || {};
                    repeatableData[repeatableName][index][fieldName] = value;
                } else if (name) {
                    const matches = name.match(/^(.+?)\[(\d+)\]\.(.+)$/);
                    if (matches) {
                        const [, repeatableName, index, fieldName] = matches;
                        repeatableData[repeatableName] = repeatableData[repeatableName] || {};
                        repeatableData[repeatableName][index] = repeatableData[repeatableName][index] || {};
                        repeatableData[repeatableName][index][fieldName] = value;
                    }
                }
            } else if (name) {
                formData[name] = value;
            }
        });

        // Convert repeatable data to final format
        for (const repeatableName in repeatableData) {
            const repeatableItems = [];
            const indices = Object.keys(repeatableData[repeatableName]).sort((a, b) => parseInt(a) - parseInt(b));

            for (const index of indices) {
                const itemData = {};
                // Convert object to dictionary format
                Object.entries(repeatableData[repeatableName][index]).forEach(([key, value]) => {
                    itemData[key] = value;
                });
                repeatableItems.push(itemData);
            }

            formData[repeatableName] = JSON.stringify(repeatableItems);
        }

        return JSON.stringify(formData);
    }

    function handleInputChange(e) {
        // Exclude changes from repeatable input area template
        if (e.target.closest('.repeatable-input-area')) return;

        let contentTemplateForm = e.target.closest('form');

        // Serialize form data
        const jsonData = serializeContentTemplateForm(e.target.closest('[data-content-template-container]'));

        // Remove existing hidden input if it exists
        const existingPropertiesInput = contentTemplateForm.querySelector('input[name="Properties"]');
        if (existingPropertiesInput) {
            existingPropertiesInput.remove();
        }

        // Add serialized data as hidden input
        const hiddenInput = document.createElement('input');
        hiddenInput.type = 'hidden';
        hiddenInput.name = 'Properties';
        hiddenInput.value = jsonData;
        contentTemplateForm.appendChild(hiddenInput);
    }

    function initializeRepeatableTokens() {
        $(document).off('click.repeatableTokens', '[data-content-template-container] .add-repeatable-item');
        $(document).on('click.repeatableTokens', '[data-content-template-container] .add-repeatable-item', handleAddRepeatableTokensClick);

        $(document).off('click.repeatableTokens', '[data-content-template-container] .remove-repeatable-item');
        $(document).on('click.repeatableTokens', '[data-content-template-container] .remove-repeatable-item', handleRemoveRepeatableTokensClick);
    }

    function attachInputListeners() {
        $(document).on('input.ContentTemplate', '[data-content-template-container] input', handleInputChange);
        $(document).on('input.ContentTemplate', '[data-content-template-container] select', handleInputChange);
        $(document).on('input.ContentTemplate', '[data-content-template-container] textarea:not(.enable-editor)', handleInputChange);

        CKEDITOR.once('instanceReady', function (event) {
            // Add change event listener to the editor instance
            event.editor.on('change', function () {
                // Create a synthetic event with a target property that contains the editor element
                const syntheticEvent = {
                    target: document.getElementById(event.editor.name)
                };
                handleInputChange(syntheticEvent);
            });
        });
    }

    // Cache DOM selectors
    const contentTemplateContainer = document.querySelector('[data-content-template-container]');
    if (!contentTemplateContainer) return;

    initializeRepeatableTokens();
    cleanup();

    attachInputListeners();
}


export function initContentTemplateTokens() {
    function initializeDragAndDrop() {
        const dragElements = document.querySelectorAll('[content-template-drag]');
        dragElements.forEach(el => {
            el.addEventListener('dragstart', handleDragStart);
        });
    }

    function handleDragStart(e) {
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('Text', e.target.dataset.text);
    }

    function initPopovers() {
        const $popovers = $('[content-template-drag] [data-toggle="popover"]');
        $popovers.popover({
            trigger: 'click'
        });
    }

    initializeDragAndDrop();
    initPopovers();
}