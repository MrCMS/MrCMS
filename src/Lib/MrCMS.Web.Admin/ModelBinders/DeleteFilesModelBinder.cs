using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.ModelBinders
{
    public class DeleteFilesModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var files = bindingContext.ValueProvider.GetValue("files").FirstValue;
            var folders = bindingContext.ValueProvider.GetValue("folders").FirstValue;

            var model = new DeleteFilesAndFoldersModel();
            if (!string.IsNullOrWhiteSpace(files))
            {
                model.Files = files.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            }

            if (!string.IsNullOrWhiteSpace(folders))
            {
                model.Folders = folders.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}