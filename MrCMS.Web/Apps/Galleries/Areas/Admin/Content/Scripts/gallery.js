$("#MediaGallery_Id").change(function (element) {
    setManageUrl($(this).val());
});

window.onload = function () {
    var id = $("#MediaGallery_Id").val();

    if (parseInt(id) > 0) {
        setManageUrl($("#MediaGallery_Id").val());
    } else {
        $("#manage-media").html('');
    }
    
    $("#TagList").tagit();
};

function setManageUrl(id) {
    var mediaCategoryManageUrl = "/Admin/MediaCategory/Edit/" + id;
    var ahref = "<a href='" + mediaCategoryManageUrl + "'>Manage images</a>";
    $("#manage-media").html('');
    $("#manage-media").append(ahref);
}