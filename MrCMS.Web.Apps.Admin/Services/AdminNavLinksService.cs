using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Core.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class AdminNavLinksService : IAdminNavLinksService
    {
        private readonly IServiceProvider _serviceProvider;

        public AdminNavLinksService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<IAdminMenuItem> GetNavLinks()
        {
            var allConcreteTypesAssignableFrom = TypeHelper.GetAllConcreteTypesAssignableFrom<IAdminMenuItem>();
            return allConcreteTypesAssignableFrom
                .Where(type => type != typeof (ChildMenuItem))
                .Select(type => _serviceProvider.GetService(type)).Cast<IAdminMenuItem>()
                .OrderBy(item => item.DisplayOrder);
        }
    }
}