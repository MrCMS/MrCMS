using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.ModelBinders
{
    public class DeleteFilesModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var files = bindingContext.ValueProvider.GetValue("files").FirstValue;
            var folders = bindingContext.ValueProvider.GetValue("folders").FirstValue;

            var model = new DeleteFilesAndFoldersModel();
            var session = bindingContext.HttpContext.RequestServices.GetRequiredService<ISession>();
            if (!string.IsNullOrWhiteSpace(files))
            {
                model.Files = session.QueryOver<MediaFile>().Where(arg => arg.Id.IsIn(files.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())).List();
            }
            if (!string.IsNullOrWhiteSpace(folders))
            {
                model.Folders = session.QueryOver<MediaCategory>().Where(arg => arg.Id.IsIn(folders.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList())).List();
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}