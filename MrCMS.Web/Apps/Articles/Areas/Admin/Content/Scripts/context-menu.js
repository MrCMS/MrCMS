$(function() {
    webMenu.rules.push(function(node, items) {
        if (node.data.type === "MrCMS.Web.Apps.Articles.Pages.ArticleSection") {
            items.viewArticlesMenuItem = {
                label: "View Articles",
                action: function() { return location.href = "/Admin/Apps/Articles/Articles?SectionId=" + node.data.id; }
            };
        }
    });
    webMenu.rules.push(function(node, items) {
        if (node.data.type === "MrCMS.Web.Apps.Articles.Pages.FeatureSection") {
            items.viewArticlesMenuItem = {
                label: "View Features",
                action: function () { return location.href = "/Admin/Apps/Articles/Features?SectionId=" + node.data.id; }
            };
        }
    });
});