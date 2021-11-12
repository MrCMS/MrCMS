import {initializePlugins} from "./initialize-plugins";

let previousValue = '';
let timer = 0;

function getCurrentValue() {
    const scope = $('[data-add-webpage]')
    return {
        name: scope.find('#Name').val(),
        mode: scope.find('#mode').is(':checked'),
        documentType: scope.find("[name=DocumentType]").val(),
        template: scope.find("#PageTemplate_Id").val()
    };
};

function suggestUrl() {
    const scope = $('[data-add-webpage]')
    const pageName = scope.find("#Name").val(),
        documentType = scope.find("[name=DocumentType]").val(),
        parentId = scope.find("#ParentId").val(),
        template = scope.find("#PageTemplate_Id").val(),
        useHierarchy = scope.find("#mode").is(':checked');
    if (pageName !== "") {
        $.get('/Admin/WebpageUrl/Suggest', {
            pageName: pageName,
            parentId: parentId,
            documentType: documentType,
            template: template,
            useHierarchy: useHierarchy
        }, function (data) {
            scope.find("#UrlSegment").val(data);
        });
        previousValue = getCurrentValue();
    } else {
        scope.find("#UrlSegment").val('');
        previousValue = getCurrentValue();
    }
};
const updateUrl = function (event) {
    event.preventDefault();
    if (areValuesChanged()) {
        suggestUrl();
    }
};

function delayedUpdateUrl(event) {
    clearTimeout(timer);
    timer = setTimeout(function () {
        updateUrl(event);
    }, 300);
};

function areValuesChanged() {
    const value = previousValue;
    const currentValue = getCurrentValue();
    if (value === null)
        return true;

    return value.documentType !== currentValue.documentType
        || value.mode !== currentValue.mode
        || value.name !== currentValue.name
        || value.template !== currentValue.template;
};

function triggerKeyUp(event) {
    event.preventDefault();
    $(event.target).keyup();
};

function logCurrentValue(event) {
    event.preventDefault();
    previousValue = getCurrentValue();
};

function updateAdditionalProperties(event) {
    const scope = $('[data-add-webpage]')
    scope.find(".hide-until-document-selected").show();
    scope.find("#message-choose-document").hide();
    const element = scope.find(':radio[name=DocumentType]:checked');
    const webpageType = element.val();
    $.get('/Admin/Webpage/AddProperties', {type: webpageType, parentId: scope.find("#Parent_Id").val()}, function (data) {
        $("[data-additional-properties]").html(data);
        initializePlugins();
    });
    // set page template from data attribute
    const templateId = element.data('page-template-id');
    scope.find('#PageTemplateId').val(templateId);
};

export function setupAddWebpage() {
    $(document).on('focus', '[data-add-webpage] #Name', logCurrentValue);
    $(document).on('blur', '[data-add-webpage] #Name', triggerKeyUp);
    $(document).on('keyup', '[data-add-webpage] #Name', delayedUpdateUrl);
    $(document).on('change', '[data-add-webpage] #mode', delayedUpdateUrl);
    $(document).on('change', '[data-add-webpage] [name=DocumentType]', delayedUpdateUrl);
    $(document).on('change', '[data-add-webpage] #PageTemplate_Id', delayedUpdateUrl);
    $(document).on('change', '[data-add-webpage] :radio[name=DocumentType]', updateAdditionalProperties);
    const scope = $('[data-add-webpage]')
    if (scope.find(':radio[name=DocumentType]:checked').length) {
        updateAdditionalProperties();
    }
}
