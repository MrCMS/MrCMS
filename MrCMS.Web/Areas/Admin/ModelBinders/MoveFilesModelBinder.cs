using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.Binders;
using Ninject;

namespace MrCMS.Web.Areas.Admin.ModelBinders
{

    public class MoveFilesModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;
        private readonly IRepository<MediaFile> _mediaFileRepository;

        public MoveFilesModelBinder(IKernel kernel, IRepository<MediaCategory> mediaCategoryRepository, IRepository<MediaFile> mediaFileRepository)
            : base(kernel)
        {
            _mediaCategoryRepository = mediaCategoryRepository;
            _mediaFileRepository = mediaFileRepository;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var folderId =
                GetValueFromContext(controllerContext, "folderId");
            var files =
                GetValueFromContext(controllerContext, "files");
            var folders =
                GetValueFromContext(controllerContext, "folders");

            var model = new MoveFilesAndFoldersModel();

            if (folderId != "")
            {
                model.Folder = _mediaCategoryRepository.Get(Convert.ToInt32(folderId));
            }
            if (files != "")
            {
                var filesList = files.Split(',').Select(int.Parse).ToList();
                model.Files = _mediaFileRepository.Query().Where(arg => filesList.Contains(arg.Id)).ToList();
            }
            if (folders != "")
            {
                var foldersList = folders.Split(',').Select(int.Parse).ToList();
                model.Folders = _mediaCategoryRepository.Query().Where(arg => foldersList.Contains(arg.Id)).ToList();
            }
            return model;
        }
    }
}