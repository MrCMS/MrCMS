var FileTools = function (options) {
    var settings = $.extend({
        fileList: "#folder-container",
        folderClass: ".folder",
        fileToolsContainer: "#file-tools",
        uiSelectedClass: ".ui-selected",
        fileClass: ".file",
        cutFilesBtn: "#cut-files",
        deleteFilesBtn: "#delete-files",
        pasteFilesBtn: "#paste-files",
        filesSotrageKey: "files",
        cutFilesStorageKey: "cut-files",
        clearFilesBtn: "#clear-files",
        fileCutCLass: ".files-cut",

}, options);
    var self;
    return {
        init: function () {
            self = this;
            $(settings.fileList).selectable({
                filter: ".file, .folder",
                stop: self.selectedStop,
                cancel: ".ui-selected, ul.pagination a"
            });

            //enbales double click
            $(settings.uiSelectedClass).click(function () { $(this).removeClass('ui-selected').parents('.ui-selectable').trigger('selectablestop'); });

            $(settings.fileClass + "," + settings.folderClass).dblclick(function () {
                location.href = $(this).data('url');
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

            $(settings.pasteFilesBtn).on('click', function (e) {
                e.preventDefault();
                self.paste();
            });

            $(settings.deleteFilesBtn).on('click', function (e) {
                e.preventDefault();
                swal({ title: "Are you sure?", text: "Your will not be able to recover this!", type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Yes, delete it!", closeOnConfirm: false }, function () { self.deleteFiles(); });       
            });

            self.setSelectedFiles();
            self.setButtonState();
            return self;
        },
        deleteFiles: function () {
            var filesAndFolders = self.getSelectedFileData().split(',');
            var files = filesAndFolders.filter(isFileId).join(',');
            files = replaceAll("file-", "", files);
            var folders = filesAndFolders.filter(isFolderId).join(',');
            folders = replaceAll("folder-", "", folders);
               
                $.ajax({
                    type: "POST",
                    url: "/Admin/MediaCategory/DeleteFilesAndFolders",
                    data: {
                        files: files,
                        folders: folders
                    },
                    success: function (data) {
                        self.clearCutStyles();
                        self.clearSelectedFiles();
                        self.setSelectedFileData('');
                        self.setCutFileData('');
                        if (data.message != '')
                            alert(data.message);
                        location.href = location.href;
                    }
                });
        },
        paste: function () {
            var filesAndFolders = self.getCutFileData().split(',');
            var files = filesAndFolders.filter(isFileId).join(',');
            files = replaceAll("file-", "", files);
            var folders = filesAndFolders.filter(isFolderId).join(',');
            folders = replaceAll("folder-", "", folders);
            $.ajax({
                type: "POST",
                url: "/Admin/MediaCategory/MoveFilesAndFolders",
                data: {
                    folderId: $("#FolderId").val(),
                    files: files,
                    folders: folders
                },
                success: function (data) {
                    self.clearCutStyles();
                    self.clearSelectedFiles();
                    self.setSelectedFileData('');
                    self.setCutFileData('');
                    if (data.message != '')
                        alert(data.message);
                    location.href = location.href;
                }
            });
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
                var selector = "[data-id='" + fileIds[number] + "']";
                $(selector).addClass("files-cut");
            });
        },
        selectedStop: function () {
            var data = $(".file.ui-selected, .folder.ui-selected").map(function () {
                return $(this).data('id');
            }).get().join(",");
            self.setSelectedFileData(data);
            if (data != '') {
                self.enableCut();
                self.enableDelete();
            } else {
                self.disableDelete();
            }
        },
        setSelectedFiles: function() {
            //set ui selected on elements
            var files = self.getSelectedFileData();
            var cutFiles = self.getCutFileData();
            if (files != '' && files != 'undefined') {
                var fileIds = files.split(',');
                $.each(fileIds, function (number) {
                    var selector = "[data-id='" + fileIds[number] + "']";
                    $(selector).addClass(settings.uiSelectedClass.replace('.', ''));
                });
                fileIds = cutFiles.split(',');
                $.each(fileIds, function (number) {
                    var selector = "[data-id='" + fileIds[number] + "']";
                    $(selector).addClass(settings.fileCutCLass.replace('.', ''));
                });
                self.enableCut();
            }
        },
        setButtonState: function () {
            if (self.getSelectedFileData() != '') {
                self.enableCut();
            } else if (self.getSelectedFileData() != '' && self.getCutFileData() == '') {
                self.enableCut();
            }
            if (self.getCutFileData() != '') {
                self.enablePaste();
            }
            self.enableDelete();
        },
        clearSelectedFiles: function() {
            $.each($(".file" + settings.uiSelectedClass + "," + " .folder" + settings.uiSelectedClass), function () {
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
            $('.file.ui-selected, .folder.ui-selected').each(function () {
                $(settings.deleteFilesBtn).removeAttr("disabled");
            });
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

function isFileId(element) {
    return element.indexOf("file") > -1;
}

function isFolderId(element) {
    return element.indexOf("folder") > -1;
}

function replaceAll(find, replace, str) {
    return str.replace(new RegExp(find, 'g'), replace);
}