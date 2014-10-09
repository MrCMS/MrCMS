var FileTools = function (options) {
    var settings = $.extend({
        fileList: "#file-list",
        fileToolsContainer: "#file-tools",
        uiSelectedClass: ".ui-selected",
        fileClass: ".file",
        cutFilesBtn: "#cut-files",
        deleteFilesBtn: "#delete-files",
        pasteFilesBtn: "#paste-files",
        filesSotrageKey: "files",
        cutFilesStorageKey: "cut-files",
        clearFilesBtn: "#clear-files",
        fileCutCLass: ".files-cut"
}, options);
    var self;
    return {
        init: function () {
            self = this;
            $(settings.fileList).selectable({
                filter: ".file",
                stop: self.selectedStop,
                cancel: ".ui-selected"
            });

            //enbales double click
            $(settings.uiSelectedClass).click(function () { $(this).removeClass('ui-selected').parents('.ui-selectable').trigger('selectablestop'); });

            $(settings.fileClass).dblclick(function () {
                location.href = $(this).data('fileurl');
            });

            $(settings.clearFilesBtn).on('click', function () {
                self.clearSelectedFiles();
                self.clearCutStyles();
                self.setSelectedFileData('');
                self.setCutFileData('');
                self.disableCut();
                self.disableDelete();
                self.disablePaste();
            });

            $(settings.cutFilesBtn).on('click', function (e) {
                e.preventDefault();
                self.clearCutStyles();
                self.copyCutFilesToStorage();
            });
            self.setSelectedFiles();
            self.setButtonState();
            return self;
        },
        copyCutFilesToStorage: function () {
            self.disableCut();
            self.enablePaste();
            self.setCutFileData(self.getSelectedFileData());
            self.setOpacityOnCutFiles(self.getCutFileData());
        },
        setOpacityOnCutFiles: function (files) {
            var fileIds = files.split(',');
            $.each(fileIds, function (number) {
                var selector = "[data-file=" + fileIds[number] + "]";
                $(selector).addClass("files-cut");
            });
        },
        selectedStop: function () {
            var data = $(".file.ui-selected").map(function () {
                return $(this).data('file');
            }).get().join(",");
            if (data != '') {
                self.setSelectedFileData(data);
                self.enableCut();
                self.enableDelete();
            }
        },
        setSelectedFiles: function() {
            //set ui selected on elements
            var files = self.getSelectedFileData();
            var cutFiles = self.getCutFileData();
            if (files != '') {
                var fileIds = files.split(',');
                $.each(fileIds, function (number) {
                    var selector = "[data-file=" + fileIds[number] + "]";
                    $(selector).addClass(settings.uiSelectedClass.replace('.', ''));
                });
                fileIds = cutFiles.split(',');
                $.each(fileIds, function (number) {
                    var selector = "[data-file=" + fileIds[number] + "]";
                    $(selector).addClass(settings.fileCutCLass.replace('.', ''));
                });
                self.enableCut();
                self.enableDelete();
            }
        },
        setButtonState: function () {
            if (self.getSelectedFileData() == '' || self.getCutFileData() == '') {
                self.disableCut();
            } else if (self.getSelectedFileData() != '' && self.getCutFileData() == '') {
                self.enableCut();
                self.enableDelete();
            } else if (self.getCutFileData() != '') {
                self.enablePaste();
            }
        },
        clearSelectedFiles: function() {
            $.each($(".file" + settings.uiSelectedClass), function () {
                $(this).removeClass(settings.uiSelectedClass.replace('.', ''));
            });
        },
        clearCutStyles: function() {
            $.each($(settings.fileCutCLass), function () {
                $(this).removeClass(settings.fileCutCLass.replace('.', ''));
            });
        },
        setSelectedFileData: function (data) {
            store.set(settings.filesSotrageKey, data);
        },
        getSelectedFileData: function () {
            return store.get(settings.filesSotrageKey);
        },
        setCutFileData: function (data) {
            store.set(settings.cutFilesStorageKey, data);
        },
        getCutFileData: function () {
            return store.get(settings.cutFilesStorageKey);
        },
        enableCut: function () {
            $(settings.cutFilesBtn).removeAttr("disabled");
        },
        disableCut: function () {
            $(settings.cutFilesBtn).attr("disabled", "disabled");
        },
        enableDelete: function () {
            $(settings.deleteFilesBtn).removeAttr("disabled");
        },
        disableDelete: function () {
            $(settings.deleteFilesBtn).attr("disabled", "disabled");
        },
        enablePaste: function () {
            $(settings.pasteFilesBtn).removeAttr("disabled");
        },
        disablePaste: function () {
            $(settings.pasteFilesBtn).attr("disabled", "disabled");
        }
    };
};

(function ($) {
    var fileTools = new FileTools().init();
})(jQuery);