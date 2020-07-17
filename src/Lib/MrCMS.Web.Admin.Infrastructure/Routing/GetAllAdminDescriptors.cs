using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MrCMS.Web.Admin.Infrastructure.Routing
{
    public class GetAllAdminDescriptors : IGetAllAdminDescriptors
    {
        private readonly IActionDescriptorCollectionProvider _descriptorProvider;

        public GetAllAdminDescriptors(IActionDescriptorCollectionProvider descriptorProvider)
        {
            _descriptorProvider = descriptorProvider;
        }

        public List<ControllerActionDescriptor> GetDescriptors()
        {
            var descriptors = _descriptorProvider.ActionDescriptors.Items.OfType<ControllerActionDescriptor>();
            return
                descriptors.Where(x =>
                    x.RouteValues.ContainsKey("area") &&
                    (x.RouteValues["area"] ?? string.Empty).Equals("admin", StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}