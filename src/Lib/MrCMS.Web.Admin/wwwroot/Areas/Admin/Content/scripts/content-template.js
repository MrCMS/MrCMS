export function initiateContentTemplate() {
    // Cache DOM selectors
    const contentTemplateContainer = document.querySelector('[data-content-template-container]');
    if (!contentTemplateContainer) return;

    const contentTemplateForm = contentTemplateContainer.closest('form');

    // Early return if form doesn't exist
    if (!contentTemplateForm) return;

    cleanup();

    function cleanup() {
        document.removeEventListener('click', handleRepeatableTokensClick);
        contentTemplateForm.removeEventListener('submit', handleFormSubmit);

        // Clean up popovers
        document.querySelectorAll('[content-template-drag] [data-toggle="popover"]').forEach(el => {
            // This assumes you'll replace popover with a custom implementation
            // or use a vanilla JS library for popovers
            if (el._popover) {
                el._popover.dispose();
            }
        });

        // Clean up CKEditor instances
        if (typeof CKEDITOR !== 'undefined') {
            Object.keys(CKEDITOR.instances).forEach(instance => {
                CKEDITOR.instances[instance].destroy();
            });
        }

        // Remove drag event listeners
        document.querySelectorAll('[content-template-drag]').forEach(el => {
            el.removeEventListener('dragstart', handleDragStart);
        });
    }

    function handleRepeatableTokensClick(e) {
        // Handle add repeatable item button click
        if (e.target.closest('.add-repeatable-item')) {
            const btn = e.target.closest('.add-repeatable-item');
            const repeatableToken = btn.closest('.repeatable-token');
            const inputArea = repeatableToken.querySelector('.repeatable-input-area');
            const container = repeatableToken.querySelector('.repeatable-items-container');

            // Create new item based on input area values
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

            // Clear input area
            inputArea.querySelectorAll('input, select, textarea').forEach(el => {
                el.value = '';
            });

            inputArea.querySelectorAll('input:checkbox').forEach(el => {
                el.checked = false;
            });

            inputArea.querySelectorAll('select').forEach(el => {
                el.selectedIndex = 0;
            });

            inputArea.querySelectorAll('img').forEach(el => {
                el.src = '/Areas/Admin/Content/img/no-media-selected.jpg';
            });
        }

        // Handle repeatable item removal
        if (e.target.closest('.remove-repeatable-item')) {
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
        }
    }

    function serializeContentTemplateForm() {
        const formData = {};
        const repeatableData = {};

        // Select form elements
        const elements = [
            ...contentTemplateContainer.querySelectorAll('[data-field-name]:not([name])'),
            ...contentTemplateContainer.querySelectorAll('[name]:not([name="__RequestVerificationToken"])')
        ];

        // Collect form data
        elements.forEach(el => {
            const name = el.getAttribute('name');
            let value;
            console.log(el);

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
                console.log("Processing repeatable field:", el);
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
                console.log("Processing regular field:", el);
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

        console.log("Final form data:", formData);
        return JSON.stringify(formData);
    }

    function handleFormSubmit(e) {
        console.log("Form submitted");
        e.preventDefault();

        if (contentTemplateForm.dataset.submitting === 'true') {
            return false;
        }

        contentTemplateForm.dataset.submitting = 'true';

        const jsonData = serializeContentTemplateForm();

        console.log("Serialized data:", jsonData);

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

        if (!contentTemplateForm.closest('[data-content-admin-editor]')) {
            // Submit widget form, in content block no need to submit
            contentTemplateForm.submit();
        }

        setTimeout(() => {
            contentTemplateForm.dataset.submitting = 'false';
        }, 500);
    }

    function initializeRepeatableTokens() {
        document.addEventListener('click', handleRepeatableTokensClick);
    }

    function initFormSubmit() {
        contentTemplateForm.addEventListener('submit', handleFormSubmit);
    }

    // Initialize everything
    cleanup();
    initializeRepeatableTokens();
    initFormSubmit();
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