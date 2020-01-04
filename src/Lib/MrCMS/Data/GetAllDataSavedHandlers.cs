using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Common;

namespace MrCMS.Data
{
    public class GetAllDataSavedHandlers : IGetAllDataSavedHandlers
    {
        public ICollection<OnDataSaved> GetHandlers()
        {
            var types = _reflectionHelper.GetAllConcreteImplementationsOf(typeof(OnDataSaved));
            return types.Select(type => _serviceProvider.GetService(type)).OfType<OnDataSaved>()
                .OrderByDescending(x => x.Priority)
                .ToList();
        }

        private readonly IReflectionHelper _reflectionHelper;
        private readonly IServiceProvider _serviceProvider;

        public GetAllDataSavedHandlers(IReflectionHelper reflectionHelper, IServiceProvider serviceProvider)
        {
            _reflectionHelper = reflectionHelper;
            _serviceProvider = serviceProvider;
        }
    }
}