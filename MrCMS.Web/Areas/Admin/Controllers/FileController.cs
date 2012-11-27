using System;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using NHibernate;

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
            return Json((from string files in Request.Files
                         select Request.Files[files]
                             into file
                             select
                                 _fileService.AddFile(file.InputStream, file.FileName, file.ContentType, file.ContentLength,
                                                      _documentService.GetDocument<MediaCategory>(mediaCategoryId))
                                 into dbFile
                                 select dbFile).ToArray());
        }

        [HttpPost]
        public void Delete(MediaFile file)
        {
            _fileService.DeleteFile(file);
        }

        [HttpPost]
        public string UpdateSEO(int id, string title, string description)
        {
            try
            {
                var mediaFile = _fileService.GetFile(id);
                mediaFile.Title = title;
                mediaFile.Description = description;
                _fileService.SaveFile(mediaFile);

                return "Changes saved";
            }
            catch (Exception ex)
            {
                return "There was an error saving the SEO values: " + ex.Message;
            }
        }
    }
}