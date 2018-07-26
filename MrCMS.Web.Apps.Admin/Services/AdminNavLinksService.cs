using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Models;

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
            return TypeHelper.GetAllConcreteTypesAssignableFrom<IAdminMenuItem>()
                .Where(type => type != typeof (ChildMenuItem))
                .Select(type => _serviceProvider.GetService(type)).Cast<IAdminMenuItem>()
                .OrderBy(item => item.DisplayOrder);
        }
    }
}