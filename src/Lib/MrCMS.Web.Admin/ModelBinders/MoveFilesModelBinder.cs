using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.ModelBinders
{
    public class MoveFilesModelBinder : IModelBinder
    {
        //private readonly IRepository<MediaCategory> _mediaCategoryRepository;
        //private readonly IRepository<MediaFile> _mediaFileRepository;

        //public MoveFilesModelBinder(IKernel kernel, IRepository<MediaCategory> mediaCategoryRepository, IRepository<MediaFile> mediaFileRepository)
        //    : base(kernel)
        //{
        //    _mediaCategoryRepository = mediaCategoryRepository;
        //    _mediaFileRepository = mediaFileRepository;
        //}

        //public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        //{
        //}

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var folderId =
                bindingContext.ValueProvider.GetValue("folderId").FirstValue;
            var files =
                bindingContext.ValueProvider.GetValue("files").FirstValue;
            var folders =
                bindingContext.ValueProvider.GetValue("folders").FirstValue;

            var model = new MoveFilesAndFoldersModel();

            if (folderId != "")
            {
                model.Folder = Convert.ToInt32(folderId);
            }

            if (files != "")
            {
                model.Files = files.Split(',').Select(int.Parse).ToList();
            }

            if (folders != "")
            {
                model.Folders = folders.Split(',').Select(int.Parse).ToList();
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}