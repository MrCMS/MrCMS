const MrCMSFeatherlightSettings = {
    type: 'iframe',
    iframeWidth: 800,
    afterOpen: function () {
        setCloseButtonPosition(this.$instance);
    },
    beforeOpen: function () {
    },
    onResize: function () {
        if (this.autoHeight) {
            // Shrink:
            this.$content.css('height', '10px');
            // Then set to the full height:
            this.$content.css('height', this.$content.contents().find('body')[0].scrollHeight);
        }
        setCloseButtonPosition(this.$instance);
    }
};

export function setCloseButtonPosition(contents) {
    // const offset = contents.find(".featherlight-content").offset();
    // const scrollTop = $(document).scrollTop();
    // contents.find(".featherlight-close-icon").css('top', offset.top - scrollTop);
    // contents.find(".featherlight-close-icon").css('right', offset.left - 20);
}

export function getRemoteModel(href) {
    const link = $("<a>");
    link.attr('href', href);
    link.attr('data-toggle', 'fb-modal');
    const settings = {};
    link.featherlight(MrCMSFeatherlightSettings).click();
}

export function setupFeatherlight() {
    const featherlightSettings = $.extend({}, MrCMSFeatherlightSettings, {
        filter: '[data-toggle="fb-modal"]'
    });
    $(document).featherlight(featherlightSettings);
}
