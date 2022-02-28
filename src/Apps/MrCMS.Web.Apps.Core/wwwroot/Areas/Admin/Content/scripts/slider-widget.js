import {html} from "./htmlHandler"

const sliderSelector = '[data-id="slider-widget"]';
const sliderInputSelector = '[data-id="inputs"]';
const sliderResultSelector = '[data-id="result"]';
const errorHolderSelector = '[data-id="error-holder"]';

function initInputs() {
    let sliders = $(sliderSelector);
    sliders.each(function (index) {
        let $this = $(this);
        let inputSelector = $this.find(sliderInputSelector);
        let resultSelector = $this.find(sliderResultSelector);
        let errorHolder = inputSelector.find(errorHolderSelector);
        let image = inputSelector.find('input[id="slider-image"]');
        let smallImage = inputSelector.find('input[id="slider-small-image"]');
        let caption = inputSelector.find('input[id="slider-caption"]');
        let link = inputSelector.find('input[id="slider-link"]');

        image.on("change", function () {
            if ($(this).val().length > 0) {
                errorHolder.html('');
            }
        });

        inputSelector.find('input[value="Add"]').on("click", function () {
            if (image.val().length > 0) {
                errorHolder.html('');
                let index = resultSelector.find('table tbody tr').length;
                let imageHtml = html`<a href="${image.val()}" target="_blank"><img src="${image.val()}" height="50"/><input type="hidden" name="${"SlideList[" + index + "].Image"}" value=" ${image.val()}"/></a>`;
                let smallImageHtml = html`<a href="${smallImage.val()}" target="_blank"><img src="${smallImage.val()}" height="50"/><input type="hidden" name="${"SlideList[" + index + "].SmallImage"}" value=" ${smallImage.val()}"/></a>`;
                let captionHtml = html`${caption.val()}<input type="hidden" name="${"SlideList[" + index + "].Caption"}" value=" ${caption.val()}"/>`;
                let linkHtml = (link.val().length == 0) ? html`<input type="hidden" name="${"SlideList[" + index + "].Link"}" value=" ${link.val()}"/>` : html`<a href="${link.val()}" target="_blank">Target Link</a><input type="hidden" name="${"SlideList[" + index + "].Link"}" value=" ${link.val()}"/>`;
                let actionHtml = `<div class="btn-group"><button type="button" data-id="delete" class="btn btn-danger btn-sm"><i class="fa fa-times" aria-hidden="true"></i></button></div>`;
                resultSelector.find('table tbody').append(html`<tr data-index="${index}"><td>${imageHtml}</td><td>${smallImageHtml}</td><td>${captionHtml}</td><td>${linkHtml}</td><td>${actionHtml}</td></tr>`);

                //Rest inputs
                inputSelector.find('[data-media="remove"]').trigger("click");
                image.val('');
                smallImage.val('');
                caption.val('');
                link.val('');
                initDeleteButtons($this);
            } else {
                errorHolder.html('slider image is required');
            }
        });

        initDeleteButtons($this);
    });
}

function initDeleteButtons(selector) {
    selector.find('button[data-id="delete"]').each(function () {
        let $this = $(this);
        $this.off("click");
        let tr = $this.closest("tr");
        $this.on("click", function () {
            tr.remove();
            reIndexRows(selector);
        });
    });
}

function reIndexRows(selector) {
    selector.find('table tbody tr[data-index]').each(function (index) {
        let $this = $(this);
        let oldIndex = $this.data("index");
        $this.data("index", index);
        $this.find('input[type="hidden"][name]').each(function () {
            let self = $(this);
            self.attr("name", self.attr("name").replaceAll(`[${oldIndex}]`, `[${index}]`));
        });
    });
}

export function initializeSliderWidget() {
    initInputs();
}