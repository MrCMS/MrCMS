using System;
using Microsoft.AspNetCore.Mvc;

namespace MrCMS.Website.Controllers
{
    public class ErrorController : MrCMSUIController
    {
        public ViewResult FileNotFound(string path)
        {
            return View(new FileNotFoundModel(path));
        }

        public class FileNotFoundModel 
        {
            public FileNotFoundModel(string path)
            {
                MissingUrl = Uri.TryCreate(path, UriKind.Relative, out Uri missingUrl) ? missingUrl : null;
            }

            public Uri MissingUrl { get; private set; }
        }
    }
}