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
        filesStorageKey: "files",
        cutFilesStorageKey: "cut-files",
        clearFilesBtn: "#clear-files",
        fileCutClass: ".files-cut",

    }, options);
    var self;
    return {
        init: function () {
            self = this;
            $(document).on('initialize-plugins', function () {
                $(settings.fileList).selectable({
                    filter: ".file, .folder",
                    stop: self.selectedStop,
                    cancel: ".ui-selected, ul.pagination a"
                });
            });

            //enbales double click
            $(document).on('click', settings.uiSelectedClass, function () { $(this).removeClass('ui-selected').parents('.ui-selectable').trigger('selectablestop'); });

            $(document).on('dblclick', settings.fileClass + "," + settings.folderClass, function () {
                location.href = $(this).data('url');
            });

            $(document).on('click', settings.clearFilesBtn, function () {
                self.clearSelectedFiles();
                self.clearCutStyles();
                self.setSelectedFileData('');
                self.setCutFileData('');
                self.disableCut();
                self.disableDelete();
                self.disablePaste();
            });

            $(document).on('click', settings.cutFilesBtn, function (e) {
                e.preventDefault();
                self.clearCutStyles();
                self.copyCutFilesToStorage();
            });

            $(document).on('click', settings.pasteFilesBtn, function (e) {
                e.preventDefault();
                self.paste();
            });

            $(document).on('click', settings.deleteFilesBtn, function (e) {
                e.preventDefault();
                swal({ title: "Are you sure?", text: "Your will not be able to recover this!", type: "warning", showCancelButton: true, confirmButtonColor: "#DD6B55", confirmButtonText: "Yes, delete it!", closeOnConfirm: true }, function () { self.deleteFiles(); });
            });

            self.setSelectedFileData('');
            self.setSelectedFiles();
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
                    self.setButtonState();
                    $(document).trigger('update-area', 'media-folder');
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
                    self.setButtonState();
                    $(document).trigger('update-area', 'media-folder');
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
            self.setButtonState();
        },
        setSelectedFiles: function () {
            //set ui selected on elements
            var files = self.getSelectedFileData();
            var fileIds;
            if (typeof files != 'undefined' && files != '' && files != null) {
                fileIds = files.split(',');
                $.each(fileIds, function (number) {
                    var selector = "[data-id='" + fileIds[number] + "']";
                    $(selector).addClass(settings.uiSelectedClass.replace('.', ''));
                });
            }
            var cutFiles = self.getCutFileData();
            if (typeof cutFiles != 'undefined' && cutFiles != '' && cutFiles != null) {
                fileIds = cutFiles.split(',');
                $.each(fileIds, function (number) {
                    var selector = "[data-id='" + fileIds[number] + "']";
                    $(selector).addClass(settings.fileCutClass.replace('.', ''));
                });
            }
            self.setButtonState();
        },
        setButtonState: function () {
            self.disableCut();
            self.disablePaste();
            self.disableDelete();
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
        clearSelectedFiles: function () {
            $.each($(".file" + settings.uiSelectedClass + "," + " .folder" + settings.uiSelectedClass), function () {
                $(this).removeClass(settings.uiSelectedClass.replace('.', ''));
            });
        },
        clearCutStyles: function () {
            $.each($(settings.fileCutClass), function () {
                $(this).removeClass(settings.fileCutClass.replace('.', ''));
            });
        },
        getSelectedFileData: function () {
            return store.get(settings.filesStorageKey);
        },
        getCutFileData: function () {
            return store.get(settings.cutFilesStorageKey);
        },
        setSelectedFileData: function (data) {
            store.set(settings.filesStorageKey, data);
            self.setButtonState();
        },
        setCutFileData: function (data) {
            store.set(settings.cutFilesStorageKey, data);
            self.setButtonState();
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

(function () {
    var fileTools = new FileTools().init();
})();

function isFileId(element) {
    return element.indexOf("file") > -1;
}

function isFolderId(element) {
    return element.indexOf("folder") > -1;
}

function replaceAll(find, replace, str) {
    return str.replace(new RegExp(find, 'g'), replace);
}