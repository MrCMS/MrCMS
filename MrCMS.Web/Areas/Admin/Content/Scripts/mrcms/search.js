//LiveSearch
var globalTimeout = null;
$('#term').keyup(function (e) {
    var tb = this;
    if (globalTimeout != null) clearTimeout(globalTimeout);
    setTimeout(function () { callLiveSearch(tb); }, 250);
});

function callLiveSearch(textbox) {
    globalTimeout = null;
    $.getJSON('/Admin/Search/GetSearchResults?term=' + textbox.value, function (data1) { insertCallback(data1); });
}

function insertCallback(data1) {
    $("#liveSearchResultsContainer").remove();
    if (data1.length > 0) {
        var results = '<ul id=\'livesearchresults\'>';
        $.each(data1, function (index, value) {
            var doctype = "WebPage";

            results = results + "<li>" +
                "<a href='/Admin/" + doctype + "/Edit/" + value.Id + "'>" + value.Name +
                "</a><br /><span class='grey font-small'>Created: " + value.CreatedOn + "</span></li>";
        });
        var position = $("#term").offset();
        $('body').append("<div id='liveSearchResultsContainer' style='top:42px; left:" + position.left + "px'>" + results + " </ul></div>");
        registerHideEvent();
    }
}

function registerHideEvent() {
    window.jQuery(document.body).click(function (event) {
        var clicked = window.jQuery(event.target);
        if (!(clicked.is('#livesearchresults') || clicked.parents('#livesearchresults').length || clicked.is('input'))) {
            $("#liveSearchResultsContainer").fadeOut('slow').remove();
        }
    });
}