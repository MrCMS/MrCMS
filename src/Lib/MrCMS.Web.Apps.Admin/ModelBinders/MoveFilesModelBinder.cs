using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.ModelBinders
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

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var folderId =
                bindingContext.ValueProvider.GetValue("folderId").FirstValue;
            var files =
                bindingContext.ValueProvider.GetValue("files").FirstValue;
            var folders =
                bindingContext.ValueProvider.GetValue("folders").FirstValue;

            var model = new MoveFilesAndFoldersModel();

            var mediaCategoryRepository = bindingContext.HttpContext.RequestServices.GetRequiredService<IRepository<MediaCategory>>();
            var mediaFileRepository = bindingContext.HttpContext.RequestServices.GetRequiredService<IRepository<MediaFile>>();
            if (folderId != "")
            {
                model.Folder = await mediaCategoryRepository.Load(Convert.ToInt32(folderId));
            }
            if (files != "")
            {
                var filesList = files.Split(',').Select(int.Parse).ToList();
                model.Files = await mediaFileRepository.Query().Where(arg => filesList.Contains(arg.Id)).ToListAsync();
            }
            if (folders != "")
            {
                var foldersList = folders.Split(',').Select(int.Parse).ToList();
                model.Folders = await mediaCategoryRepository.Query().Where(arg => foldersList.Contains(arg.Id)).ToListAsync();
            }

            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}