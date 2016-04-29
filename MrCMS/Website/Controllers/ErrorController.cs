using System;
using System.Web.Mvc;

namespace MrCMS.Website.Controllers
{
    public class ErrorController : MrCMSUIController
    {
        public ViewResult FileNotFound(Uri url)
        {
            return View(new FileNotFoundModel(url));
        }

        public class FileNotFoundModel 
        {
            public FileNotFoundModel(Uri url)
            {
                MissingUrl = url;
            }

            public Uri MissingUrl { get; private set; }
        }
    }
}