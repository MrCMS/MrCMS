using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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
        public JsonResult Files_Post([IoCModelBinder(typeof(NullableEntityModelBinder))] MediaCategory mediaCategory)
        {
            var list = new List<ViewDataUploadFilesResult>();
            foreach (string files in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[files];
                if (_fileService.IsValidFileType(file.FileName))
                {
                    ViewDataUploadFilesResult dbFile = _fileService.AddFile(file.InputStream, file.FileName,
                        file.ContentType, file.ContentLength,
                        mediaCategory);
                    list.Add(dbFile);
                }
            }
            return Json(list.ToArray(), "text/html", Encoding.UTF8);
        }


        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete_POST(MediaFile file)
        {
            int categoryId = file.MediaCategory.Id;
            _fileService.DeleteFile(file);
            return RedirectToAction("Show", "MediaCategory", new { Id = categoryId });
        }

        [HttpGet]
        public ActionResult Delete(MediaFile file)
        {
            return View("Delete", file);
        }

        [HttpPost]
        public string UpdateSEO(MediaFile mediaFile, string title, string description)
        {
            try
            {
                mediaFile.Title = title;
                mediaFile.Description = description;
                _fileService.SaveFile(mediaFile);

                return "Changes saved";
            }
            catch (Exception ex)
            {
                return string.Format("There was an error saving the SEO values: {0}", ex.Message);
            }
        }

        public ActionResult Edit(MediaFile file)
        {
            return View("Edit", file);
        }

        [HttpPost]
        [ActionName("Edit")]
        public ActionResult Edit_POST(MediaFile file)
        {
            _fileService.SaveFile(file);

            return file.MediaCategory != null
                ? RedirectToAction("Show", "MediaCategory", new {file.MediaCategory.Id})
                : RedirectToAction("Index", "MediaCategory");
        }
    }
}