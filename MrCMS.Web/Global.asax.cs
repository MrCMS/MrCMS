using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using NHibernate;
using Ninject;

namespace MrCMS.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : MrCMSApplication
    {
        protected override void OnApplicationStart()
        {
          
        }

    }
}