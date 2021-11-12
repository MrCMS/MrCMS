$(function() {
    $('#StorageType').change(function() {
        if ($(this).val() == 'MrCMS.Services.AzureFileSystem') {
            $('#azure-settings').show();
        } else {
            $('#azure-settings').hide();
        }
    });
    $('#StorageType').change();
})