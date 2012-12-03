using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Services;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class FileController : AdminController
    {
        private readonly IFileService _fileService;
        private readonly IDocumentService _documentService;

        public FileController(IFileService fileService, IDocumentService documentService)
        {
            _fileService = fileService;
            _documentService = documentService;
        }

        [HttpGet]
        public JsonResult Files(int mediaCategoryId)
        {
            return Json(_fileService.GetFiles(mediaCategoryId));
        }

        [HttpPost]
        [ActionName("Files")]
        public JsonResult Files_Post(int mediaCategoryId)
        {
            var list = new List<ViewDataUploadFilesResult>();
            foreach (string files in Request.Files)
            {
                var file = Request.Files[files];
                var dbFile = _fileService.AddFile(file.InputStream, file.FileName,
                                                  file.ContentType, file.ContentLength,
                                                  _documentService.GetDocument<MediaCategory>(mediaCategoryId));
                list.Add(dbFile);
            }
            return Json(list.ToArray());
        }

        [HttpPost]
        public void Delete(MediaFile file)
        {
            _fileService.DeleteFile(file);
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
    }
}