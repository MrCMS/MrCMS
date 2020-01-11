using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class DeleteFilesModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var files = bindingContext.ValueProvider.GetValue("files").FirstValue;
            var folders = bindingContext.ValueProvider.GetValue("folders").FirstValue;

            var model = new DeleteFilesAndFoldersModel();
            //var session = bindingContext.HttpContext.RequestServices.GetRequiredService<ISession>();
            if (!string.IsNullOrWhiteSpace(files))
            {
                var fileRepository =
                    bindingContext.HttpContext.RequestServices.GetRequiredService<IRepository<MediaFile>>();
                var fileIds = files.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                model.Files = await fileRepository.Query().Where(arg => fileIds.Contains(arg.Id)).ToListAsync();
            }
            if (!string.IsNullOrWhiteSpace(folders))
            {
                var folderRepository =
                    bindingContext.HttpContext.RequestServices.GetRequiredService<IRepository<MediaCategory>>();
                var folderIds = folders.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                model.Folders = await folderRepository.Query().Where(arg => folderIds.Contains(arg.Id)).ToListAsync();
            }

            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}