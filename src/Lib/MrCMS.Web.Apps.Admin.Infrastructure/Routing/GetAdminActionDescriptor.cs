using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Routing
{
    public class GetAdminActionDescriptor : IGetAdminActionDescriptor
    {
        public const string DefaultMethod = "GET";
        private readonly IGetAllAdminDescriptors _getAllAdminDescriptors;

        public GetAdminActionDescriptor(IGetAllAdminDescriptors getAllAdminDescriptors)
        {
            _getAllAdminDescriptors = getAllAdminDescriptors;
        }

        public ControllerActionDescriptor GetDescriptor(string controllerName, string actionName, string method = DefaultMethod)
        {
            var adminDescriptors = _getAllAdminDescriptors.GetDescriptors();
            var getDescriptors = adminDescriptors.Where(x =>
            {
                if (x.ActionConstraints == null)
                    return true;
                var httpMethodConstraints = x.ActionConstraints.OfType<HttpMethodActionConstraint>().ToList();
                return !httpMethodConstraints.Any() ||
                       httpMethodConstraints.Any(constraint =>
                           constraint.HttpMethods.Contains(method ?? HttpMethod.Get.Method));
            });

            return getDescriptors.FirstOrDefault(x => x.ControllerName == controllerName && x.ActionName == actionName);
        }
    }
}