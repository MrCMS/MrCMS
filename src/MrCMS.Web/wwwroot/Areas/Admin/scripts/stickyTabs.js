var StickyTabs = function (options) {
    var settings = $.extend({
        
    }, options);
    var self;
    return {
        init: function () {
            self = this;
            $("[data-stickytabs]").each(function() {
                self.setActiveTab(this);

                $(this).on('shown.bs.tab', function (e) {
                    self.persistTab(e);
                });
            });
            return self;
        },
        setActiveTab: function (tab) {
            var key = $(tab).data("stickytabs");
            var currentTab = store.get(key);
            if (location.hash !== '' && $(tab).find(' a[href="' + location.hash + '"]').length > 0) {
                $(tab).find(' a[href="' + location.hash + '"]').tab('show');
            } else if (currentTab) {
                $(tab).find('a[href="' + currentTab + '"]').tab('show');
            } else if ($(tab).find('li a[data-toggle="tab"]').length > 0 && $(tab).find('li.active a[data-toggle="tab"]').length == 0) {
                $(tab).find('a[data-toggle="tab"]').eq(0).click();
            }
        },
        persistTab: function (e) {
            store.set($(e.currentTarget).data("stickytabs"), e.target.hash);
        }
    };
};
var stickyTabs;
$(function () {
    stickyTabs = new StickyTabs().init();
});