
    $(function () {  
        $("#parent").select2({
            initSelection: function (element, callback) {
                var data = { id: element.val(), text: element.val() };
                callback(data);
            }
        });
    });      