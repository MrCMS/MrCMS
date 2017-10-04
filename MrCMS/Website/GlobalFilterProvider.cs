using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ninject;

namespace MrCMS.Website
{
    public class GlobalFilterProvider : IFilterProvider
    {
        private readonly IKernel _kernel;
        private readonly Type[] _globalFilterTypes;

        public GlobalFilterProvider(IKernel kernel, params Type[] globalFilterTypes)
        {
            _kernel = kernel;
            _globalFilterTypes = globalFilterTypes;
        }

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (_kernel.Get<IEnsureDatabaseIsInstalled>().IsInstalled())
                foreach (var type in _globalFilterTypes)
                    yield return new Filter(_kernel.Get(type), FilterScope.Global, null);
        }
    }
}