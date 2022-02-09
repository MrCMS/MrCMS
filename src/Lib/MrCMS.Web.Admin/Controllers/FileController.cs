using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class FileController : MrCMSAdminController
    {
        private readonly IFileAdminService _fileService;

        public FileController(IFileAdminService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        [ActionName("Files")]
        [RequestSizeLimit(500 * 1024 * 1024)] 
        public async Task<JsonResult> Files_Post(int id)
        {
            var list = new List<ViewDataUploadFilesResult>();
            foreach (var file in Request.Form?.Files ?? new FormFileCollection())
            {
                if (_fileService.IsValidFileType(file.FileName))
                {
                    ViewDataUploadFilesResult dbFile = await _fileService.AddFile(file.OpenReadStream(), file.FileName,
                        file.ContentType, file.Length, id);
                    list.Add(dbFile);
                }
            }

            return Json(list.ToArray());
        }


        [HttpPost]
        [ActionName("Delete")]
        //public ActionResult Delete_POST(MediaFile file)
        public async Task<ActionResult> Delete_POST(int id)
        {
            var file = await _fileService.GetFile(id);
            if (file == null)
                return RedirectToAction("Index", "MediaCategory");

            int categoryId = file.MediaCategory.Id;
            await _fileService.DeleteFile(id);
            return RedirectToAction("Show", "MediaCategory", new {Id = categoryId});
        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            return View("Delete", await _fileService.GetFile(id));
        }

        [HttpPost]
        public async Task<string> UpdateSEO(UpdateFileSEOModel model)
        {
            try
            {
                await _fileService.UpdateSEO(model);

                return "Changes saved";
            }
            catch (Exception ex)
            {
                return $"There was an error saving the SEO values: {ex.Message}";
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            return View("Edit", await _fileService.GetEditModel(id));
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<ActionResult> Edit_POST(UpdateFileSEOModel model)
        {
            var file = await _fileService.UpdateSEO(model);

            return file.MediaCategory != null
                ? RedirectToAction("Show", "MediaCategory", new {file.MediaCategory.Id})
                : RedirectToAction("Index", "MediaCategory");
        }
    }
}