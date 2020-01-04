using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.DbConfiguration
{
    public class GetModelCreators : IGetModelCreators
    {
        private readonly IServiceProvider _serviceProvider;

        public GetModelCreators(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<ICreateModel> GetCreators()
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(ICreateModel));
            return types.Select(type => _serviceProvider.GetService(type)).OfType<ICreateModel>();
        }
    }
}